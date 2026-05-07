// SPDX-License-Identifier: MIT

namespace Fahrenheit.Runtime;

/* [fkelava 21/03/26 22:55]
 * The game utilizes the Phyre engine as a middleware for modern assets.
 * All are stored in pre-processed, platform-specific form.
 *
 * We would like to load them at runtime, that they need not be distributed
 * with mods. We have the game load them, since it can unwrap the Phyre containers.
 *
 * Regardless of what derived asset type is at hand (a texture, shader, model...),
 * all are loaded the same way, in the same base form- a Phyre 'cluster',
 * containing 'instance lists' of specialized objects.
 *
 * In Phyre, an asset/cluster can only be loaded once until released. The game
 * thus has a ref-counting system, the 'cluster manager'. This module interfaces with it.
 *
 * See:
 * - src/runtime/resload.cs
 * - src/core/petypes.cs
 * - src/core/petypes.g.cs
 * - src/core/peinterop.cs
 */

/// <summary>
///     A disposable structure returned when loading a <see cref="PCluster"/>
///     which ensures its release once the scope is exited.
/// </summary>
internal readonly unsafe ref struct FhPClusterScope(PCluster* ptr_cluster, FhPhyreLoaderModule owner) {
    private readonly PCluster*           _ptr_cluster = ptr_cluster;
    private readonly FhPhyreLoaderModule _owner       = owner;

    public bool enter(out PCluster* ptr_cluster) {
        return (ptr_cluster = _ptr_cluster) != null;
    }

    /* [fkelava 29/04/26 15:09]
     * Remember that structs always have an implicit parameterless constructor.
     * default(FhPClusterScope) will result in _both_ pointers being null, so both must be guarded.
     */

    public void Dispose() {
        if (_ptr_cluster == null) return;
        _owner?.cluster_release(_ptr_cluster);
    }
}

/// <summary>
///     Provides Phyre asset load services to the Fahrenheit runtime.
/// </summary>
[FhLoad(FhGameId.FFX | FhGameId.FFX2 | FhGameId.FFX2LM)]
public unsafe sealed class FhPhyreLoaderModule : FhModule {

    // Game functions through which we load Phyre assets
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate PCluster* ClusterManager_loadPCluster(nint ptr_this, byte* ptr_file_name);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate void ClusterManager_releasePCluster(nint ptr_this, PCluster* ptr_cluster);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate void ClusterManager_forceReleasePCluster(nint ptr_this, byte* ptr_file_name);

    private readonly FhMethodHandle<ClusterManager_loadPCluster>          _h_pcluster_ld;
    private readonly FhMethodHandle<ClusterManager_releasePCluster>       _h_pcluster_rel;
    private readonly FhMethodHandle<ClusterManager_forceReleasePCluster>? _h_pcluster_frel; // FFX only?

    private readonly delegate* unmanaged[Cdecl]<PCluster**, int, int> _fnptr_PApplication_FixupClusters;

    public FhPhyreLoaderModule() {
        _fnptr_PApplication_FixupClusters = (delegate* unmanaged[Cdecl]<PCluster**, int, int>)
            (FhEnvironment.BaseAddr + FhUtil.select(0x223740, 0x6B3020, 0x6B3020));

        _h_pcluster_ld  = new(this, new FhMethodLocation(0x29BA80, 0x9E880), h_pcluster_ld);
        _h_pcluster_rel = new(this, new FhMethodLocation(0x29BEF0, 0x9ED00), h_pcluster_rel);

        if (FhGlobal.game_id is FhGameId.FFX) {
            _h_pcluster_frel = new(this, "FFX.exe", 0x29B450, h_pcluster_frel);
        }
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        return  _h_pcluster_ld   .hook()
            &&  _h_pcluster_rel  .hook()
            && (_h_pcluster_frel?.hook() ?? true);
    }

    /* [fkelava 02/05/26 02:35]
     * The logging here can be removed after a full game run test.
     *
     * In fact, the method handles can be folded into fnptrs, since we do not alter
     * the behavior of these methods in any way.
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvThiscall) ] )]
    private PCluster* h_pcluster_ld(nint ptr_this, byte* ptr_file_name) {
        PCluster* rv = _h_pcluster_ld.orig_fptr(ptr_this, ptr_file_name);
        _logger.Info($"{Marshal.PtrToStringAnsi((nint)ptr_file_name)!} -> 0x{(nint)rv:X}");
        return rv;
    }

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvThiscall) ] )]
    private void h_pcluster_rel(nint ptr_this, PCluster* ptr_cluster) {
        _logger.Info($"0x{(nint)ptr_cluster:X}");
        _h_pcluster_rel.orig_fptr(ptr_this, ptr_cluster);
    }

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvThiscall) ] )]
    private void h_pcluster_frel(nint ptr_this, byte* ptr_file_name) {
        _logger.Info(Marshal.PtrToStringAnsi((nint)ptr_file_name)!);
        _h_pcluster_frel!.orig_fptr(ptr_this, ptr_file_name);
    }

    /// <summary>
    ///     Attempts to load a Phyre asset at <paramref name="file_path"/>.
    ///     If successful, returns a scope over the resulting cluster.
    /// </summary>
    internal FhPClusterScope cluster_load(string file_path) {
        byte[] file_path_u8    = Encoding.UTF8.GetBytes(file_path);
        nint   ptr_cluster_mgr = FhUtil.get_at<nint>(FhUtil.select(0x8CCA44, 0x9CFE48, 0x9CFE48));

        if (ptr_cluster_mgr == 0) {
            _logger.Warning($"ClusterManager not ready - {file_path}");
            return new FhPClusterScope(null, this);
        }

        PCluster* ptr_cluster;
        fixed (byte* ptr_path_u8 = file_path_u8) {
            ptr_cluster = _h_pcluster_ld.orig_fptr(ptr_cluster_mgr, ptr_path_u8);
        }

        if (ptr_cluster == null) {
            _logger.Warning($"ClusterManager::loadPCluster failed for {file_path}");
            return new FhPClusterScope(null, this);
        }

        int rv_fixup = _fnptr_PApplication_FixupClusters(&ptr_cluster, 1);

        if (rv_fixup != 0) {
            _logger.Warning($"PApplication::FixupClusters returned {rv_fixup} for {file_path}");
            return new FhPClusterScope(null, this);
        }

        return new FhPClusterScope(ptr_cluster, this);
    }

    /// <summary>
    ///     Releases a cluster attained by a previous call to <see cref="cluster_load"/>.
    /// </summary>
    internal void cluster_release(PCluster* ptr_cluster) {
        nint ptr_cluster_mgr = FhUtil.get_at<nint>(FhUtil.select(0x8CCA44, 0x9CFE48, 0x9CFE48));
        _h_pcluster_rel.orig_fptr(ptr_cluster_mgr, ptr_cluster);
    }
}
