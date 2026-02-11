// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core.Runtime;

/* [fkelava 11/02/26 04:03]
 * By default, the game only probes its VBF data archives for files and fails if it cannot find them.
 * Repacking the archives is tiresome, so we want to permit direct loading of modded files from disk.
 *
 * While the game never uses it, it has full support for native file I/O. On Windows, this manifests as HANDLEs.
 * Thus, at file load time, we can silently swap out what the game _intended_ to load for
 * a HANDLE to a file on disk, and the game will perform all necessary book-keeping for us.
 *
 * There is a limited exception to this rule which must be handled. See `cd.cs`.
 */

/// <summary>
///     Provides the ability to replace files loaded by the game with files outside the VBF archives.
///     <para/>
///     Do not interface with this module directly. Instead, place any files you wish to use in this way
///     in the <c>efl\x</c> or <c>efl\x2</c> subdirectory of your mod folder.
///     <para/>
///     These subdirectories are treated as the root of the VBF archive for their respective games;
///     from that point, you must mirror the VBF's directory structure.
///     <para/>
///     For example, to replace <c>FFX_Data\ffx_ps2\ffx\master\jppc\battle\kernel\takara.bin</c>,
///     the full path is <c>{...}\efl\x\FFX_Data\ffx_ps2\ffx\master\jppc\battle\kernel\takara.bin</c>.
/// </summary>
[FhLoad(FhGameId.FFX | FhGameId.FFX2 | FhGameId.FFX2LM)]
[SupportedOSPlatform("windows")]
public unsafe sealed class FhFileLoaderModule : FhModule {

    /// <summary>
    ///     A file as seen by the game. Can be backed by either a VBF or OS handle.
    /// </summary>
    private struct PStreamFile {
        public nint handle_os;
        public nint handle_vbf;
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate PStreamFile* _PStreamFile_ctor(
        PStreamFile* ptr_this,
        nint         ptr_path,
        bool         read_only,
        nint         p3,  // unused?
        nint         p4,  // unused?
        bool         p5); // unused?

    private readonly Dictionary<string, string>        _index;
    private readonly FhMethodHandle<_PStreamFile_ctor> _handle_fctor;

    public FhFileLoaderModule() {
        FhMethodLocation method_location = new FhMethodLocation(0x207D80, 0x490E40);

        _index        = [];
        _handle_fctor = new(this, method_location, h_fopen);
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        construct_index();
        return _handle_fctor.hook();
    }

    /* [fkelava 11/02/26 03:39]
     * The game internally uses a number of file addressing schemes, including, but not limited to:
     *
     * - host0:/ffx/master/jppc/event/obj/sc/scene1/scene1.ebp
     * - pfs0:sizetbl.bin
     * - /FFX_Data/GameData/PS3Data/chr/mon/m220/fp/tex/GCM/16128_0_0_8_256_128.dds.phyre
     * - /ffx_ps2/ffx/master/new_depc
     * - /help/test_proj/test_proj_page.sps2
     * - ../../../ffx_ps2/ffx/proj/map/masaki/
     *
     * Some are leftovers from the PS2, some properly relative to the root of the VBF,
     * others rooted in subdirectories of the VBF. No universal standard exists.
     *
     * All of them pass through 'fiosUnifyFilename', which performs further absurdities
     * like prepending '../../..' to every single path. EFL normalizes any and all paths to
     * a path relative to the root of the VBF archive, with forward slashes as a separator.
     *
     * If you experience issues with files not being replaced, your best bet is to check
     * the inputs and outputs to this function. While I tested by logging millions of file open
     * calls, it is entirely possible some edge case was skipped or not encountered.
     */

    /// <summary>
    ///     Normalizes the paths the game uses to address files.
    /// </summary>
    private static string normalize_path(string path) {
        string path_no_host0   = path.Replace("host0:", "ffx_ps2");
        int    path_prefix_end = path_no_host0.IndexOf('f', StringComparison.OrdinalIgnoreCase);
        string path_prefixless = path_no_host0[ path_prefix_end .. ];

        /* [fkelava 28/01/26 01:05]
         * A forward slash is universally recognized as a path separator, and the game internally prefers it.
         * This is a no-op on Unix-like systems where the forward slash is the natural path separator.
         */

        return path_prefixless.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    /// <summary>
    ///     Creates the immutable map of EFL replacements for this game session.
    /// </summary>
    private void construct_index() {
        string efl_subdir_name = FhUtil.select("x", "x2", "x2");

        string         path_efl_dir;
        FhModContext[] mods = [ .. FhApi.Mods.get_mods() ];

        foreach (FhModContext mod in mods) {
            path_efl_dir = Path.Join(mod.Paths.EflDir.FullName, efl_subdir_name);
            if (!Directory.Exists(path_efl_dir)) continue;

            foreach (string path_efl_file in Directory.GetFiles(path_efl_dir, "*.*", SearchOption.AllDirectories)) {
                string path_rel            = Path.GetRelativePath(path_efl_dir, path_efl_file);
                string path_rel_normalized = normalize_path(path_rel);

                if (_index.ContainsKey(path_rel_normalized)) {
                    _logger.Warning($"{path_rel_normalized} is being superseded by mod {mod.Manifest.Name}");
                }

                _index[path_rel_normalized] = path_efl_file;
                _logger.Info($"Mod {mod.Manifest.Name} replaces file {path_rel_normalized}");
            }
        }
    }

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvThiscall) ] )]
    private PStreamFile* h_fopen(PStreamFile* ptr_this, nint ptr_path, bool read_only, nint p3, nint p4, bool p5) {
        string path            = Marshal.PtrToStringAnsi(ptr_path)!;
        string path_normalized = normalize_path(path);

        if (!_index.TryGetValue(path_normalized, out string? path_modded)) {
            return _handle_fctor.orig_fptr(ptr_this, ptr_path, read_only, p3, p4, p5);
        }

        /* [fkelava 01/10/24 16:49]
         * FFX.exe+208100 at +2081B9 onward:
         * if (readOnly) { pvVar4 = CreateFileW(path, 1, 1, 0, 3, 0x08000000, 0); }
         * else          { pvVar4 = CreateFileW(path, 2, 0, 0, 4, 0x08000000, 0); }
         */

        fixed (char* ptr_path_modded = path_modded) {
            ptr_this->handle_vbf = 0;
            ptr_this->handle_os  = Windows.CreateFileW(
                ptr_path_modded,
                (uint)(read_only ? FILE.FILE_READ_DATA  : FILE.FILE_WRITE_DATA),
                (uint)(read_only ? FILE.FILE_SHARE_READ : 0),
                null,
                (uint)(read_only ? OPEN.OPEN_EXISTING   : OPEN.OPEN_ALWAYS),
                FILE.FILE_FLAG_SEQUENTIAL_SCAN,
                HANDLE.NULL);
        }

        if (ptr_this->handle_os == HANDLE.INVALID_VALUE) {
            _logger.Error($"File open failed for {path_modded} - bailing out");
            return _handle_fctor.orig_fptr(ptr_this, ptr_path, read_only, p3, p4, p5);
        }

        return ptr_this;
    }
}
