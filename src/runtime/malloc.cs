// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit.Runtime;

[FhLoad(FhGameId.FFX | FhGameId.FFX2 | FhGameId.FFX2LM)]
public unsafe sealed class FhMallocModule : FhModule {

    private nuint _reserved  => FhUtil.get_at<nuint>(FhUtil.select(0x153CD44, 0x14E6AB4, 0x14E6AB4));
    private nuint _committed => FhUtil.get_at<nuint>(FhUtil.select(0x153CD48, 0x14E6AB8, 0x14E6AB8));

    public FhMallocModule() { }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        return FhCall._VirtualAlloc_Reserve_NA              .hook(this, h__VirtualAlloc_Reserve_NA)
            && FhCall._VirtualAlloc_Commit_RW               .hook(this, h__VirtualAlloc_Commit_RW)
            && FhCall._VirtualAlloc_ReserveCommit_TopDown_RW.hook(this, h__VirtualAlloc_ReserveCommit_TopDown_RW)
            && FhCall._VirtualFree_Decommit                 .hook(this, h__VirtualFree_Decommit);
    }

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void* h__VirtualAlloc_Reserve_NA(nuint size) {
        _logger.Info($"MEM_RESERVE(0x{size:X8})");
        void* rv = FhCall._VirtualAlloc_ReserveCommit_TopDown_RW.chain_from(h__VirtualAlloc_ReserveCommit_TopDown_RW).fnptr!(size);
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
