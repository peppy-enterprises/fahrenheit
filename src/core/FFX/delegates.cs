using Fahrenheit.Core.FFX.Atel;
using Fahrenheit.Core.FFX.Battle;

namespace Fahrenheit.Core.FFX;

public static unsafe class Call {
    /*===== RNG =====*/
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint getChrRngIdx(
        uint chr_id,
        uint type);

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
