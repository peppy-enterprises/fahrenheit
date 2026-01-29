// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core.Runtime;

/* [fkelava 27/01/26 19:30]
 * The 'CD' from the eponymous `GetCD()` function in either game is a struct that partially
 * emulates the original PS2 file I/O semantics, in which files were looked up by a numeric ID using
 * several tables that described the CD's layout and certain file attributes such as their size.
 *
 * For reasons unknown, in the Remaster certain PS2 assets such as kernel `.bin` files are still
 * loaded using this method. This prevents their easy modification. This module makes the needed
 * alterations so 'CD' loading falls through to disk I/O where appropriate.
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
    private delegate nint CDfileName_PC(int ordinal);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nint check_ex_file_size(int arg1, int arg2);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nint CDfileSize_CD_EX(int arg1);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nint CDfileSize_PC(int arg1);

    private readonly Cd*                                _ptr_cd;
    private readonly FhMethodHandle<CDfileName_PC>      _handle_fname;
    private readonly FhMethodHandle<check_ex_file_size> _handle_fsize_chk;
    private readonly FhMethodHandle<CDfileSize_CD_EX>   _handle_fsize_ex;
    private readonly FhMethodHandle<CDfileSize_PC>      _handle_fsize_pc;

    public FhCdInterfaceModule() {
        FhMethodLocation loc_fname     = new(0x642C00, 0x74EE70);
        FhMethodLocation loc_fsize_pc  = new(0x6428A0, 0x74E9A0);
        FhMethodLocation loc_fsize_chk = new(0x36D770, 0x1396A0);
        FhMethodLocation loc_fsize_ex  = new(0x36C520, 0x138910);

        _handle_fname     = new(this, loc_fname,     h_fname);
        _handle_fsize_pc  = new(this, loc_fsize_pc,  h_fsize_pc);
        _handle_fsize_chk = new(this, loc_fsize_chk, h_fsize_chk);
        _handle_fsize_ex  = new(this, loc_fsize_ex,  h_fsize_ex);

        _ptr_cd = (Cd*)(FhEnvironment.BaseAddr + FhUtil.select(0x1F10C40, 0x16CA080, 0x16CA080));
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        return _handle_fname    .hook()
            && _handle_fsize_pc .hook()
            && _handle_fsize_chk.hook()
            && _handle_fsize_ex .hook();
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
     * This is a bit obtuse. Follow carefully.
     *
     * FFX.exe+642957 (in `CDfileSize_PC`):
     * > if (((pCVar1->ptr_cdrom_fnd != 0)
     * > &&   (iVar3 = FUN_00a42c00(iVar3 + param_1), iVar3 != 0))
     * > && ((((cd_read.module != 0xe &&
     * >    ((((cd_read.module != 0x10 && (cd_read.module != 0x1a)) && (cd_read.module != 0x1c)) &&
     * >      ((cd_read.module != 0x1d && (cd_read.module != 0x1e)))))) &&
     * >      ((cd_read.module != 0x1f &&
     * >     (((cd_read.module != 0x20 && (cd_read.module != 0x21)) &&
     * >      ((cd_read.module != 0x22 && ((cd_read.module != 0x28 && (cd_read.module != 0x29))))))))))
     * > || (uVar2 = CDfileSize_CD_EX(param_1), uVar2 == 0xffffffff))))
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

    // This can be erased after sufficient testing.

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private nint h_fname(int arg1) {
        nint rv = _handle_fname.orig_fptr(arg1);
        _logger.Info($"{arg1} -> {Marshal.PtrToStringAnsi(rv)}");
        return rv;
    }

    // This can be erased after sufficient testing proves it is not called.

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private nint h_fsize_ex(int arg1) {
        _logger.Warning("in fsize2; something has gone wrong");
        nint rv = _handle_fsize_ex.orig_fptr(arg1);
        _logger.Info($"{arg1} -> {rv}");
        return rv;
    }
}
