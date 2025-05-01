using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

using static Fahrenheit.Core.Runtime.PInvoke;

namespace Fahrenheit.Core.Runtime;

public struct FhFfxFile {
    public nint handle_os;
    public nint handle_vbf;
}

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate FhFfxFile* fiosOpen(nint path_ptr, bool read_only);

[FhLoaderMark]
public unsafe class FhEFLModule : FhModule {
    private readonly Dictionary<string, string> _index;
    private readonly FhMethodHandle<fiosOpen>   _h_fiosOpen;

    public FhEFLModule() {
        _index      = [];
        _h_fiosOpen = new(this, "FFX.exe", h_open, offset: 0x2798E0);
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
        FhModContext[] mods = [ .. FhInternal.ModController.get_all() ];

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
                    FhLog.Warning($"{normalized_relative_path} is being superseded by module {mod.Manifest.Name}");
                }

                _index[normalized_relative_path] = normalized_absolute_path;
            }
        }

        index_swatch.Stop();
        FhLog.Warning($"EFL indexing complete in {index_swatch.ElapsedMilliseconds} ms.");
    }

    public FhFfxFile* h_open(nint path_ptr, bool read_only) {
        string     path            = Marshal.PtrToStringAnsi(path_ptr) ?? throw new Exception("FH_E_EFL_FIOS_OPEN_PATH_NUL");
        string     normalized_path = normalize_path(path);
        FhFfxFile* file            = _h_fiosOpen.orig_fptr.Invoke(path_ptr, read_only);

        if (!_index.TryGetValue(normalized_path, out string? modded_path)) return file;

        /* [fkelava 01/10/24 16:49]
         * FFX.exe+208100 at +2081B9 onward:
         * if (readOnly) { pvVar4 = CreateFileW(path, 1, 1, 0, 3, 0x08000000, 0); }
         * else          { pvVar4 = CreateFileW(path, 2, 0, 0, 4, 0x08000000, 0); }
         *
         * No bookkeeping of the returned handle is necessary. The game closes it itself.
         */

        file->handle_vbf = 0;
        file->handle_os  = CreateFileW(
            lpFileName:            modded_path,
            dwDesiredAccess:       read_only ? FILE_READ_DATA  : FILE_WRITE_DATA,
            dwShareMode:           read_only ? FILE_SHARE_READ : 0U,
            lpSecurityAttributes:  0,
            dwCreationDisposition: read_only ? OPEN_EXISTING   : OPEN_ALWAYS,
            dwFlagsAndAttributes:  FILE_FLAG_SEQUENTIAL_SCAN,
            hTemplateFile:         0);

        FhLog.Info($"{path} -> {modded_path}");
        return file;
    }

    public override bool init() {
        construct_index();
        return _h_fiosOpen.hook();
    }
}
