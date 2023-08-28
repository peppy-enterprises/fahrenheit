using System;
using System.Runtime.InteropServices;

using Fahrenheit.CoreLib;

using static Fahrenheit.CoreLib.FhHookDelegates;

namespace Fahrenheit.Hooks.Generic;

public partial class FhHooksExampleModule
{
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
    public unsafe int TkIsDebugHook()
    {
        if (_tkIsDbg.GetOriginalFptrSafe(out TkIsDebugDelegate? fptr))
        {
            FhLog.Log(LogLevel.Info, $"Calling original.");
            return fptr.Invoke();
        }

        FhLog.Log(LogLevel.Info, $"Original was not called.");
        return 0;
    }
}
