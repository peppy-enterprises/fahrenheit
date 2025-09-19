using Fahrenheit.Core.FFX.Atel;

namespace Fahrenheit.Core.FFX;

public static unsafe class Call {
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
