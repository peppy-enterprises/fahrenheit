namespace Fahrenheit.CoreLib.FFX;

using System.Runtime.InteropServices;

public static unsafe class Call
{
    /*===== Util =====*/
    [UnmanagedFunctionPointer(CallingConvention.FastCall)]
    public delegate void SecCookieCheck(
        uint p1);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate int MsCheckRange(
        int n,
        int min,
        int max);

    /*===== RNG =====*/
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint getChrRngIdx(
        uint chr_id,
        uint type);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint brnd(
        uint rng_seed_idx);

    /*===== Save Data =====*/
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int MsSaveItemUse(
        uint item_id,
        int  amount);

    /*===== Excel Data =====*/
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate nint MsGetExcelData(
        uint req_elem_idx,
        nint excel_data_ptr,
        nint storage_var);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate Command* MsGetRomItem(
        uint item_id,
        nint param_2);

    /*===== MsChr =====*/
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate Chr* MsGetChr(
        uint chr_id);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate Chr* MsGetMon(
        byte mon_id);

    /*===== Battle =====*/
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool MsCalcEscape(
        uint chr_id);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate nint MsApUp(
        uint chr_id,
        Chr* chr,
        uint base_ap_add,
        nint param_4);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int MsCalcDamage(
        Chr*     user,
        Chr*     target,
        Command* command,
        byte     dmg_formula,
        int      power,
        byte     target_status__0x606,
        byte     targetted_stat,
        uint     should_vary,
        byte*    ref_used_def,
        byte*    ref_used_mdef,
        int      base_dmg);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool MsSetRikkuLimit(
        uint chr_id,
        Chr* chr,
        nint param_3);

    /*===== ATEL =====*/
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void AtelJumpGameOver();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void AtelInitCallFunc(
        uint             func_selector,
        AtelBasicWorker* work,
        nint*            storage,
        AtelStack*       stack);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate nint AtelExecCallFunc(
        uint             func_selector,
        AtelBasicWorker* work,
        AtelStack*       stack);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void AtelResultCallFunc(
        uint             func_selector,
        AtelBasicWorker* work,
        nint*            storage,
        AtelStack*       stack);

    /*===== CALL TARGETS =====*/
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void CallTargetInit(
        AtelBasicWorker* work,
        nint*            storage,
        AtelStack*       stack);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void CallTargetExec(
        AtelBasicWorker* work,
        AtelStack*       stack);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate float CallTargetResultFloat(
        AtelBasicWorker* work,
        nint*            storage,
        AtelStack*       stack);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int CallTargetResultInt(
        AtelBasicWorker* work,
        nint*            storage,
        AtelStack*       stack);
}
