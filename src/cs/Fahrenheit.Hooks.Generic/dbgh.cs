using System;
using System.Runtime.InteropServices;

using Fahrenheit.CoreLib;

using static Fahrenheit.CoreLib.FhHookDelegates;

namespace Fahrenheit.Hooks.Generic;

public partial class FhHooksBaseModule {
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
    public unsafe int TkIsDebugHook() {
        if (_tkIsDbg.try_get_original_fptr(out TkIsDebugDelegate? fptr)) {
            FhLog.Log(LogLevel.Info, $"Calling original.");
            return fptr.Invoke();
        }

        //FhLog.Info($"Original was not called.");
        return 0;
    }
}
