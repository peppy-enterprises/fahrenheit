// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit.Runtime;

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

using EflIndex = Dictionary<string, string>;

/// <summary>
///     Provides the ability to replace files loaded by the game with files outside the VBF archives,
///     and the ability to cross-load the inactive game's assets from the active game.
/// </summary>
/// <remarks>
///     Place any files you wish to use in this way
///     in the <c>efl\x</c> or <c>efl\x2</c> subdirectory of your mod folder.
///     <para/>
///     These subdirectories are treated as the root of the VBF archive for their respective games;
///     from that point, you must mirror the VBF's directory structure.
///     <para/>
///     For example, to replace <c>FFX_Data\ffx_ps2\ffx\master\jppc\battle\kernel\takara.bin</c>,
///     the full path is <c>{...}\efl\x\FFX_Data\ffx_ps2\ffx\master\jppc\battle\kernel\takara.bin</c>.
/// </remarks>
[FhLoad(FhGameId.FFX | FhGameId.FFX2 | FhGameId.FFX2LM)]
[SupportedOSPlatform("windows6.1")]
public unsafe sealed class FhFileLoaderModule : FhModule {

    private static ReadOnlySpan<byte> _stream_prefix      => "/"u8;
    private static ReadOnlySpan<byte> _vbf_secondary_path => FhGlobal.game_id is FhGameId.FFX 
        ? @"data\FFX2_Data.vbf"u8
        : @"data\FFX_Data.vbf"u8;

    /* [fkelava 21/08/26 02:12]
     * BigFileStream and BigFileHandle (and PStreamFile) will store pointers
     * to their stream prefixes and/or VBF names, and expect them to be permanently valid.
     * 
     * Thus we allocate some unmanaged memory to keep them alive and pinned forever.
     */

    private readonly EflIndex       _index;
    private readonly BigFileStream* _ptr_vbf_secondary;
    private readonly byte*          _ptr_vbf_secondary_path;

    public FhFileLoaderModule() {
        _index                  = [];
        _ptr_vbf_secondary      = (BigFileStream*) NativeMemory.AllocZeroed((nuint) sizeof(BigFileStream));
        _ptr_vbf_secondary_path = (byte*)          NativeMemory.AllocZeroed((nuint) _vbf_secondary_path.Length);

        _vbf_secondary_path.CopyTo(new (_ptr_vbf_secondary_path, _vbf_secondary_path.Length));
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        _init_index();
        _init_crossload();

        return FhCall.Phyre_PSerialization_PStreamFile_ctor           .hook(this, h_fopen)
            && FhCall.Phyre_PSerialization_PStreamFile_SetStreamPrefix.hook(this, h_sf_sp_set)
            && FhCall.BigFileStream_setStreamPrefix                   .hook(this, h_vbf_sp_set);
    }

    /* [fkelava 21/08/26 14:10]
     * The game has the concept of a 'stream prefix', prepended to any and all paths. For some silly reason,
     * the default is '../../..', but it doesn't have to be. We can simplify it, which is desirable so the
     * user need not remember it.
     * 
     * Note that the prefix can't be empty. The game will take an access violation if so.
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvThiscall) ] )]
    private void h_vbf_sp_set(BigFileStream* ptr_this, byte* ptr_stream_prefix) {
        byte* ptr_prefix = (byte*) NativeMemory.AllocZeroed((nuint) _stream_prefix.Length);
        _stream_prefix.CopyTo(new (ptr_prefix, _stream_prefix.Length));

        FhCall.BigFileStream_setStreamPrefix.chain_from(h_vbf_sp_set).fnptr!(ptr_this, ptr_prefix);
    }

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void h_sf_sp_set(byte* ptr_stream_prefix) {
        byte* ptr_prefix = (byte*) NativeMemory.AllocZeroed((nuint) _stream_prefix.Length);
        _stream_prefix.CopyTo(new (ptr_prefix, _stream_prefix.Length));

        FhCall.Phyre_PSerialization_PStreamFile_SetStreamPrefix.chain_from(h_sf_sp_set).fnptr!(ptr_prefix);
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
     * EFL normalizes any and all paths to a path relative to the root 
     * of the VBF archive, with forward slashes as a separator.
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
         * The game internally prefers a forward slash as path separator. Additionally, both major OSes support it well.
         *
         * https://learn.microsoft.com/en-us/dotnet/standard/base-types/best-practices-strings#recommendations-for-string-usage
         * > Use the String.ToUpperInvariant method instead of the String.ToLowerInvariant method when you normalize strings for comparison.
         */

        return path_prefixless.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).ToUpperInvariant();
    }

    /// <summary>
    ///     Indexes one of a given mod's root EFL directories.
    /// </summary>
    /// <param name="mod">The mod for which indexing is being carried out.</param>
    /// <param name="path_efl_dir">The absolute path to the EFL directory to index.</param>
    private void _index_dir(FhModContext mod, string path_efl_dir) {
        foreach (FileInfo efl_file in Directory.CreateDirectory(path_efl_dir).GetFiles("*.*", SearchOption.AllDirectories)) {
            string path_rel            = Path.GetRelativePath(path_efl_dir, efl_file.FullName);
            string path_rel_normalized = normalize_path(path_rel);

            if (_index.ContainsKey(path_rel_normalized)) {
                _logger.Warning($"{path_rel} is being superseded by mod {mod.Manifest.Name}");
            }

            _index[path_rel_normalized] = efl_file.FullName;
            _logger.Info($"Mod {mod.Manifest.Name} replaces file {path_rel}");
        }
    }

    /// <summary>
    ///     Indexes all mods' EFL directories to gather the replacement files for this session.
    /// </summary>
    /// <remarks>
    ///     After this method is called, any files subsequently added to the EFL directories
    ///     will not be available in this game session.
    /// </remarks>
    private void _init_index() {
        foreach (FhModContext mod in FhApi.Mods.get_mods()) {
            string path_efl_subdir_primary   = Path.Join(mod.Paths.EflDir.FullName, FhUtil.select("x" , "x2", "x2"));
            string path_efl_subdir_secondary = Path.Join(mod.Paths.EflDir.FullName, FhUtil.select("x2", "x" , "x" ));

            _index_dir(mod, path_efl_subdir_secondary);
            _index_dir(mod, path_efl_subdir_primary);
        }
    }

    private void _init_crossload() {
        byte* ptr_prefix = (byte*) NativeMemory.AllocZeroed((nuint) _stream_prefix.Length);
        _stream_prefix.CopyTo(new (ptr_prefix, _stream_prefix.Length));

        FhCall.BigFileStream_ctor           .fnptr!(_ptr_vbf_secondary);
        FhCall.BigFileStream_setStreamPrefix.fnptr!(_ptr_vbf_secondary, ptr_prefix);
        
        if (FhCall.BigFileStream_registerBigFile.fnptr!(_ptr_vbf_secondary, _ptr_vbf_secondary_path) == 0)
            throw new Exception("Failed to initialize cross-loader function. Your game data may be corrupt or missing.");
    }

    /// <summary>
    ///     Searches the secondary VBF for an asset to match the given path, 
    ///     and attempts to load it into the given <see cref="PStreamFile"/>.
    /// </summary>
    /// <param name="ptr_this">The <see cref="PStreamFile"/> to attempt a crossload into.</param>
    /// <param name="ptr_path">The asset's relative path from the VBF root.</param>
    /// <returns>
    ///     The unmodified input if it already contained a valid file, or
    ///     the input modified to contain the result of a load into the secondary VBF.
    /// </returns>
    private PStreamFile* _crossload(PStreamFile* ptr_this, byte* ptr_path) {
        if (ptr_this->handle_vbf != null)
            return ptr_this;

        VFile* ptr_crossload_file = FhCall.BigFileStream_openFile.fnptr!(_ptr_vbf_secondary, ptr_path);
        ptr_this->handle_vbf = ptr_crossload_file;

        return ptr_this;
    }

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvThiscall) ] )]
    private PStreamFile* h_fopen(PStreamFile* ptr_this, byte* ptr_path, bool read_only, uint p3, uint p4, bool p5) {
        string path            = new ((sbyte*) ptr_path);
        string path_normalized = normalize_path(path);

        if (!_index.TryGetValue(path_normalized, out string? path_modded)) {
            PStreamFile* rv = FhCall.Phyre_PSerialization_PStreamFile_ctor.chain_from(h_fopen).fnptr!(ptr_this, ptr_path, read_only, p3, p4, p5);
            return _crossload(rv, ptr_path);
        }

        /* [fkelava 01/10/24 16:49]
         * FFX.exe+208100 at +2081B9 onward:
         * if (readOnly) { pvVar4 = CreateFileW(path, 1, 1, 0, 3, 0x08000000, 0); }
         * else          { pvVar4 = CreateFileW(path, 2, 0, 0, 4, 0x08000000, 0); }
         */

        fixed (char* ptr_path_modded = path_modded) {
            FILE_ACCESS_RIGHTS        access      = read_only
                ? FILE_ACCESS_RIGHTS.FILE_READ_DATA
                : FILE_ACCESS_RIGHTS.FILE_WRITE_DATA;
            FILE_SHARE_MODE           sharing     = read_only
                ? FILE_SHARE_MODE.FILE_SHARE_READ
                : FILE_SHARE_MODE.FILE_SHARE_NONE;
            FILE_CREATION_DISPOSITION disposition = read_only
                ? FILE_CREATION_DISPOSITION.OPEN_EXISTING
                : FILE_CREATION_DISPOSITION.OPEN_ALWAYS;

            FILE_FLAGS_AND_ATTRIBUTES flags = FILE_FLAGS_AND_ATTRIBUTES.FILE_FLAG_SEQUENTIAL_SCAN;

            ptr_this->handle_vbf = null;
            ptr_this->handle_os  = PInvoke.CreateFileW(
                ptr_path_modded,
                (uint)access,
                sharing,
                null,
                disposition,
                flags,
                HANDLE.Null);
        }

        if (ptr_this->handle_os == HANDLE.INVALID_HANDLE_VALUE) {
            _logger.Error($"Replacement file open failed for {path_modded} - bailing out");

            PStreamFile* rv = FhCall.Phyre_PSerialization_PStreamFile_ctor.chain_from(h_fopen).fnptr!(ptr_this, ptr_path, read_only, p3, p4, p5);
            return _crossload(rv, ptr_path);
        }

        return ptr_this;
    }
}
