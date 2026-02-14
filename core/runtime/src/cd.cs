// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core.Runtime;

/* [fkelava 11/02/26 04:19]
 * Broadly speaking, the game has two file load paths. Most files are looked up by name, and the EFL
 * intercepts file open calls to substitute what the game loads with a user's modified file on disk.
 *
 * Assets from the original PS2 releases are instead loaded using PS2 'CD' I/O semantics, in which
 * files are addressed by a numeric ID which indexes into tables that describe the CD's layout and
 * file attributes such as their name, size, and sector location.
 *
 * See:
 * - ffx_ps2\ffx\proj\prog\cdidx\${REGION}\cdrom.mdg
 * - ffx_ps2\ffx\proj\battle\jp\cddata\cdrom.fnd
 * - ffx_ps2\ffx\proj\prog\cdidx\jp\sizetbl.vita.bin
 *
 * When the game loads a PS2 asset, it will look up these fixed tables and allocate a buffer according
 * to the file's original size. The EFL will correctly intercept the file open call and provide the user's
 * modified file, but if it differs in size from the original, the game will crash.
 *
 * While users realized they can create a fixed-up size table, that won't work if mods want to coexist.
 * This module exists to bypass the lookup, so the game properly falls through to disk I/O to query size.
 */

[StructLayout(LayoutKind.Explicit, Size = 0x94)]
internal unsafe struct Cd {
    [FieldOffset(0x20)] public int mdg_index;
    [FieldOffset(0x24)] public int module;
}

/// <summary>
///     Performs certain overrides on the game's 'CD' reading emulation
///     to allow for modified files to be properly loaded.
///     <para/>
///     Do not interface with this module. It has no public API.
/// </summary>
[FhLoad(FhGameId.FFX | FhGameId.FFX2 | FhGameId.FFX2LM)]
public unsafe sealed class FhCdInterfaceModule : FhModule {

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nint check_ex_file_size(int arg1, int arg2);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nint CDfileSize_PC(int arg1);

    private readonly Cd*                                _ptr_cd;
    private readonly FhMethodHandle<check_ex_file_size> _handle_fsize_chk;
    private readonly FhMethodHandle<CDfileSize_PC>      _handle_fsize_pc;

    public FhCdInterfaceModule() {
        FhMethodLocation loc_fsize_pc  = new(0x6428A0, 0x74E9A0);
        FhMethodLocation loc_fsize_chk = new(0x36D770, 0x1396A0);

        _handle_fsize_pc  = new(this, loc_fsize_pc,  h_fsize_pc);
        _handle_fsize_chk = new(this, loc_fsize_chk, h_fsize_chk);

        _ptr_cd = (Cd*)(FhEnvironment.BaseAddr + FhUtil.select(0x1F10C40, 0x16CA080, 0x16CA080));
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        return _handle_fsize_pc .hook()
            && _handle_fsize_chk.hook();
    }

    /* [fkelava 27/01/26 17:14]
     * We shorten this method compared to the base game.
     *
     * FFX.exe+36D77A (in `check_ex_file_size`):
     * > if (DAT_01128ab8 == 0) {
     * >     return 0;
     * > }
     *
     * FFX.exe+36CD04 (in `CdEnvInit`):
     * > DAT_01128ab8 = hdd_install_check(DAT_01128ae4);
     *
     * FFX.exe+379373 (in `hdd_install_check`):
     * > dbgPrintf("ffx_hddmode=%d  hdd_tbl_cnt=%d\n",(int)ffx_hddmode,hdd_tbl_cnt);
     * > return hdd_tbl_cnt;
     *
     * `hdd_tbl_cnt` is always zero, hence `check_ex_file_size` always returns zero.
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private nint h_fsize_chk(int arg1, int arg2) {
        return 0;
    }

    /* [fkelava 27/01/26 19:04]
     * FFX.exe+642957 (in `CDfileSize_PC`):
     * > if (pCVar1->ptr_cdrom_fnd != 0 && FUN_00a42c00(iVar3 + param_1) != 0))
     * > && (cd_read.module != [ 0xE, 0x10, 0x1A, 0x1C, 0x1D, 0x1E, 0x1F, 0x20, 0x21, 0x22, 0x28, 0x29 ])
     * > || (CDfileSize_CD_EX(param_1) == -1)
     * > {
     * >     iVar4 = sceOpen(iVar3,1);
     * >     if (iVar4 == 0) {
     * >         dbgPrintf("BUZZ: CDfileSize_PC: MISSING? [%s]\n",iVar3);
     * >         return 0;
     * >     }
     * >     iVar3 = sceFileSize(iVar4);
     * >     uVar2 = (iVar3 + 0xf >> 0x1f & 0xfU) + iVar3 + 0xf & 0xfffffff0;
     * >     sceClose(iVar4);
     * > }
     *
     * `CDfileSize_CD_EX` looks up the fixed values in `sizetbl.vita.bin`. Our goal is to avoid it,
     * or rather that execution falls through to `sceOpen`/`sceFileSize`. These methods call Phyre's
     * PStreamFile constructor, which is hooked by EFL to return users' modded files on disk when
     * needed. The resulting FS I/O returns the correct size of the modded file.
     *
     * So we set the module to always meet the conditional and restore it after the method call.
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private nint h_fsize_pc(int arg1) {
        int original_module = _ptr_cd->module;

        _ptr_cd->module = 0;
        nint rv = _handle_fsize_pc.orig_fptr(arg1);

        _ptr_cd->module = original_module;
        return rv;
    }
}
