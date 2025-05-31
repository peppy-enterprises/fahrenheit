using System.Diagnostics;

using static Fahrenheit.Core.Runtime.PInvoke;

namespace Fahrenheit.Core.Runtime;

public struct PStreamFile {
    public nint handle_os;
    public nint handle_vbf;
}

[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
public unsafe delegate nint PStreamFile_ctor(PStreamFile* this_ptr, nint path_ptr, bool read_only, nint param_3, nint param_4, bool param_5);

/// <summary>
///     Provides the ability to replace files loaded by the game with files outside the VBF archives.
/// </summary>
[FhLoad(FhGameType.FFX)]
public unsafe class FhFileLoaderModule : FhModule {
    private readonly Dictionary<string, string>       _index;
    private readonly FhMethodHandle<PStreamFile_ctor> _h_pstream_ctor;

    public FhFileLoaderModule() {
        _index          = [];
        _h_pstream_ctor = new(this, "FFX.exe", h_pstream_ctor, offset: 0x207D80);
    }

    // the game uses a fixed stream prefix "../../../" - I don't see why ffgriever handled the other edge cases (yet)
    private static string normalize_path(string path) {
        string prefixless_path = path.Replace("../../../", "");

        return OperatingSystem.IsWindows()
            ? prefixless_path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
            : prefixless_path;
    }

    public void construct_index() {
        Stopwatch index_swatch     = Stopwatch.StartNew();
        string    data_subdir_name = FhGlobal.game_type switch {
            FhGameType.FFX  => "x",
            FhGameType.FFX2 => "x2",
            _               => throw new Exception("FH_E_INVALID_GAME_TYPE"),
        };

        string         efl_data_dir;
        FhModContext[] mods = [ .. FhApi.ModController.get_all() ];

        foreach (FhModContext mod in mods) {
            efl_data_dir = normalize_path(Path.Join(mod.Paths.EflDir.FullName, data_subdir_name));
            if (!Directory.Exists(efl_data_dir)) continue;

            foreach (string absolute_mod_file_path in Directory.GetFiles(efl_data_dir, "*.*", SearchOption.AllDirectories)) {
                string nt_absolute_mod_file_path = @$"\\?\{absolute_mod_file_path}";
                string relative_mod_file_path    = Path.GetRelativePath(efl_data_dir, absolute_mod_file_path);
                string normalized_relative_path  = normalize_path(relative_mod_file_path);
                string normalized_absolute_path  = normalize_path(OperatingSystem.IsWindows()
                    ? nt_absolute_mod_file_path
                    : absolute_mod_file_path);

                if (_index.ContainsKey(normalized_relative_path)) {
                    _logger.Warning($"{normalized_relative_path} is being superseded by mod {mod.Manifest.Name}");
                }

                _index[normalized_relative_path] = normalized_absolute_path;
                _logger.Info($"Mod {mod.Manifest.Name} replaces file {normalized_relative_path}");
            }
        }

        index_swatch.Stop();
        _logger.Warning($"EFL indexing complete in {index_swatch.ElapsedMilliseconds} ms.");
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

    public override bool init(FileStream global_state_file) {
        construct_index();
        return _h_pstream_ctor.hook();
    }
}
