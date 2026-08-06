// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit.Runtime;

/// <summary>
///     An opaque structure used by the game's allocator.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct __ALLOC_STRUCT {
    public CRITICAL_SECTION crit_sec;
    public __ALLOC_DATA     data;
}

/// <summary>
///     An opaque structure used by the game's allocator.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct __ALLOC_DATA {
    public uint size;
    public uint _0x04;
    public uint _0x08;
    public uint _0x0C;
    public uint align;
    public uint _0x14;
}

/// <summary>
///     Manipulates the game's allocator to improve its behavior.
/// </summary>
[FhLoad(FhGameId.FFX | FhGameId.FFX2 | FhGameId.FFX2LM)]
[SupportedOSPlatform("windows5.1.2600")]
public unsafe sealed class FhMallocModule : FhModule {

    // TODO: UI to optionally display memory statistics
    private static nuint _reserved  => FhUtil.get_at<nuint>(FhUtil.select(0x153CD44, 0x14E6AB4, 0x14E6AB4));
    private static nuint _committed => FhUtil.get_at<nuint>(FhUtil.select(0x153CD48, 0x14E6AB8, 0x14E6AB8));

    public FhMallocModule() { }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        return FhCall._malloc_pool_init                     .hook(this, h__malloc_pool_init)
            && FhCall._VirtualAlloc_Reserve_NA              .hook(this, h__VirtualAlloc_Reserve_NA)
            && FhCall._VirtualAlloc_Commit_RW               .hook(this, h__VirtualAlloc_Commit_RW)
            && FhCall._VirtualAlloc_ReserveCommit_TopDown_RW.hook(this, h__VirtualAlloc_ReserveCommit_TopDown_RW)
            && FhCall._VirtualFree_Decommit                 .hook(this, h__VirtualFree_Decommit);
    }

    /* [fkelava 06/08/26 14:02]
     * See generally issue #253.
     *
     * Effectively, the game reserves 37.5% of the default address space (0x3000_0000 bytes)
     * for its primary memory pool. It then commits things into it slowly over time.
     *
     * Reserved memory is considered used by the system regardless of how much of it is actually
     * _committed_ (i.e. in actual use). If the game serviced all memory requests from the pool,
     * there would be no problem, but it doesn't. For things like FMVs, there still has to be
     * enough additional contiguous free memory to load them.
     *
     * This worked ten or so years ago when it was made, and it works on most other platforms
     * because the high reaches of the address space should be entirely empty.
     *
     * But in the decade since, ASLR started shipping on Windows, and is the default for anything
     * built today. When such a DLL is loaded, the OS loader will place it at a random high address.
     * The amount of free memory hasn't changed, but it's now carved up into smaller blocks between
     * the randomly-placed images. There is no longer a free block large enough to fit an FMV
     * between the image at the lowest address and the game's reserved pool.
     *
     * The true solution is to reserve less. By deferring the moment memory is considered 'used'
     * to the _actual point of usage_, the truth is revealed; there was never a shortage of memory.
     */

    /// <summary>
    ///     Replaces the game's primary memory pool initializer.
    /// </summary>
    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void h__malloc_pool_init() {

        /* [fkelava 06/08/26 14:49]
         * If the game's default pool initializer fails to reserve 0x3000_0000 bytes,
         * it'll decrement that by 0x100_0000 and keep trying until it succeeds or throws.
         *
         * For sanity's sake, we don't replicate that. If we can't reserve much less than
         * the game normally uses, then failing here gives a clear indication of what is wrong.
         */

        FhCall.FUN_009428A0_008772A0.fnptr!();

        __ALLOC_STRUCT* alloc_struct = (__ALLOC_STRUCT*) NativeMemory.Alloc(0x30);
        nuint           pool_size    = 0x200_0000; // default: 0x3000_0000

        PInvoke.InitializeCriticalSection(&alloc_struct->crit_sec);

        FhUtil.set_at(FhUtil.select(0x8E900C, 0x9EDBE4, 0x9EDBE4), (uint)&alloc_struct->crit_sec);
        _ = FhCall.FUN_00942A40_00877440.fnptr!(&alloc_struct->data, pool_size, 0, uint.CreateChecked( pool_size - 0x100000 ));

        PInvoke.EnterCriticalSection(&alloc_struct->crit_sec);
        void* rv = FhCall.FUN_00942B60_00877560.fnptr!(&alloc_struct->data, 0x10);
        PInvoke.LeaveCriticalSection(&alloc_struct->crit_sec);

        if (rv == null) {
            throw new Exception($"Failed to allocate primary memory pool of size 0x{pool_size:X8}.");
        }

        FhUtil.set_at(FhUtil.select(0x8E9010, 0x9EDBE8, 0x9EDBE8), (uint)rv);
        FhUtil.set_at(FhUtil.select(0x8E9014, 0x9EDBEC, 0x9EDBEC), pool_size);
    }

    /* [fkelava 06/08/26 14:29]
     * As it were, the game doesn't (that we know of) perform any pointer truncation. It could use
     * 4G of address space safely, but Square/Virtuos forgot to specify the appropriate linker flag.
     * That's why the game "requires" the "4GB patch". As per the above, it doesn't.
     *
     * But let's say the user applies the 4GB patch anyway. Not much changes. The OS loader still
     * places images from circa 0x5000_0000 to 0x7FFF_FFFF like roadblocks, and the large
     * reserved pool still congests the low reaches of address space. Can we do better?
     *
     * The game eventually defers any action by its primary allocator to VirtualAlloc/VirtualFree.
     * See https://learn.microsoft.com/en-us/windows/win32/api/memoryapi/nf-memoryapi-virtualalloc,
     * https://learn.microsoft.com/en-us/windows/win32/api/memoryapi/nf-memoryapi-virtualfree.
     *
     * By passing MEM_TOP_DOWN, VirtualAlloc can be induced to go top-down; that is, start allocating
     * from 7FFF_FFFF downwards. This applies to the primary pool as well! Effectively, we can move
     * it (and all other primary allocator actions) above the 2G mark.
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void* h__VirtualAlloc_Reserve_NA(nuint size) {
        _logger.Info($"MEM_RESERVE(0x{size:X8})");
        void* rv = FhCall._VirtualAlloc_Reserve_NA.chain_from(h__VirtualAlloc_Reserve_NA).fnptr!(size);
        _logger.Info($"0x{(nint)rv:X8}");
        _logger.Info($"TOTAL RESERVED: 0x{_reserved:X8}");
        return rv;
    }

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void* h__VirtualAlloc_Commit_RW(void* ptr, nuint size) {
        _logger.Info($"MEM_COMMIT(0x{(nint)ptr:X8}, 0x{size:X8})");
        void* rv = FhCall._VirtualAlloc_Commit_RW.chain_from(h__VirtualAlloc_Commit_RW).fnptr!(ptr, size);
        _logger.Info($"TOTAL COMMITTED: 0x{_committed:X8}");
        return rv;
    }

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void* h__VirtualFree_Decommit(void* ptr, nuint size) {
        _logger.Info($"MEM_DECOMMIT(0x{(nint)ptr:X8}, 0x{size:X8})");
        void* rv = FhCall._VirtualFree_Decommit.chain_from(h__VirtualFree_Decommit).fnptr!(ptr, size);
        _logger.Info($"TOTAL COMMITTED: 0x{_committed:X8}");
        return rv;
    }

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void* h__VirtualAlloc_ReserveCommit_TopDown_RW(nuint size) {
        _logger.Info($"MEM_RESERVE+COMMIT(TOP_DOWN, 0x{size:X8})");
        void* rv = FhCall._VirtualAlloc_ReserveCommit_TopDown_RW.chain_from(h__VirtualAlloc_ReserveCommit_TopDown_RW).fnptr!(size);
        _logger.Info($"0x{(nint)rv:X8}");
        _logger.Info($"TOTAL RESERVED: 0x{_reserved:X8}");
        return rv;
    }

}
