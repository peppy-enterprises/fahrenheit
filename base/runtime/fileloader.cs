﻿using System.Diagnostics;

using static Fahrenheit.Core.Runtime.PInvoke;

namespace Fahrenheit.Core.Runtime;

internal struct PStreamFile {
    public nint handle_os;
    public nint handle_vbf;
}

[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
internal unsafe delegate nint PStreamFile_ctor(PStreamFile* this_ptr, nint path_ptr, bool read_only, nint param_3, nint param_4, bool param_5);

/// <summary>
///     Provides the ability to replace files loaded by the game with files outside the VBF archives.
///     <para/>
///     Do not interface with this module directly. Instead, place any files you wish to use in this way
///     in the <b>efl\x</b> or <b>efl\x2</b> subdirectory of your mod folder.
///     <para/>
///     These subdirectories are treated as the root of the VBF archive for their respective games;
///     from that point, you must mirror the VBF's directory structure.
///     <para/>
///     For example, to replace <b>FFX_Data\ffx_ps2\ffx\master\jppc\battle\kernel\takara.bin</b>,
///     the full path is <b>{...}\efl\x\FFX_Data\ffx_ps2\ffx\master\jppc\battle\kernel\takara.bin</b>.
/// </summary>
[FhLoad(FhGameType.FFX)]
public unsafe class FhFileLoaderModule : FhModule {
    private readonly Dictionary<string, string>       _index;
    private readonly FhMethodHandle<PStreamFile_ctor> _h_pstream_ctor;

    public FhFileLoaderModule() {
        _index          = [];
        _h_pstream_ctor = new(this, "FFX.exe", h_pstream_ctor, offset: 0x207D80);
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        construct_index();
        return _h_pstream_ctor.hook();
    }

    /// <summary>
    ///     Normalizes the relative paths the game uses to address files in the VBF archives.
    /// </summary>
    private static string normalize_path(string path) {
        string host0_fixed_path  = path.Replace("host0:", "ffx_ps2");
        int    stream_prefix_end = host0_fixed_path.IndexOf('f', StringComparison.InvariantCultureIgnoreCase);
        string prefixless_path   = host0_fixed_path[stream_prefix_end..];

        return OperatingSystem.IsWindows()
            ? prefixless_path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
            : prefixless_path;
    }

    private void construct_index() {
        Stopwatch index_timer     = Stopwatch.StartNew();
        string    efl_subdir_name = FhGlobal.game_type switch {
            FhGameType.FFX  => "x",
            FhGameType.FFX2 => "x2",
            _               => throw new Exception("FH_E_INVALID_GAME_TYPE"),
        };

        string         path_efl_dir;
        FhModContext[] mods = [ .. FhApi.ModController.get_all() ];

        foreach (FhModContext mod in mods) {
            path_efl_dir = Path.Join(mod.Paths.EflDir.FullName, efl_subdir_name);
            if (!Directory.Exists(path_efl_dir)) continue;

            foreach (string path_efl_file in Directory.GetFiles(path_efl_dir, "*.*", SearchOption.AllDirectories)) {
                string path_absolute_nt         = @$"\\?\{path_efl_file}";
                string path_relative            = Path.GetRelativePath(path_efl_dir, path_efl_file);
                string path_relative_normalized = normalize_path(path_relative);
                string path_absolute = OperatingSystem.IsWindows()
                    ? path_absolute_nt
                    : path_efl_file;

                if (_index.ContainsKey(path_relative_normalized)) {
                    _logger.Warning($"{path_relative_normalized} is being superseded by mod {mod.Manifest.Name}");
                }

                _index[path_relative_normalized] = path_absolute;
                _logger.Info($"Mod {mod.Manifest.Name} replaces file {path_relative_normalized}");
            }
        }

        index_timer.Stop();
        _logger.Warning($"EFL indexing complete in {index_timer.ElapsedMilliseconds} ms.");
    }

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvThiscall) ] )]
    private nint h_pstream_ctor(PStreamFile* this_ptr, nint path_ptr, bool read_only, nint param_3, nint param_4, bool param_5) {
        string path            = Marshal.PtrToStringAnsi(path_ptr) ?? throw new Exception("FH_E_EFL_PSTREAM_CTOR_OPEN_PATH_NUL");
        string normalized_path = normalize_path(path);

        _logger.Info(normalized_path);

        if (!_index.TryGetValue(normalized_path, out string? modded_path)) {
            return _h_pstream_ctor.orig_fptr.Invoke(this_ptr, path_ptr, read_only, param_3, param_4, param_5);
        }

        /* [fkelava 01/10/24 16:49]
         * FFX.exe+208100 at +2081B9 onward:
         * if (readOnly) { pvVar4 = CreateFileW(path, 1, 1, 0, 3, 0x08000000, 0); }
         * else          { pvVar4 = CreateFileW(path, 2, 0, 0, 4, 0x08000000, 0); }
         *
         * No bookkeeping of the returned handle is necessary. The game closes it itself.
         */

        this_ptr->handle_vbf = 0;
        this_ptr->handle_os  = CreateFileW(
            lpFileName:            modded_path,
            dwDesiredAccess:       read_only ? FILE_READ_DATA  : FILE_WRITE_DATA,
            dwShareMode:           read_only ? FILE_SHARE_READ : 0U,
            lpSecurityAttributes:  0,
            dwCreationDisposition: read_only ? OPEN_EXISTING   : OPEN_ALWAYS,
            dwFlagsAndAttributes:  FILE_FLAG_SEQUENTIAL_SCAN,
            hTemplateFile:         0);

        _logger.Info($"{path} -> {modded_path}");
        return new nint(this_ptr);
    }
}
