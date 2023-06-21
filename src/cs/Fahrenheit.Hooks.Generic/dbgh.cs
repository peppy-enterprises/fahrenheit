using System;
using System.Runtime.InteropServices;

using Fahrenheit.CoreLib;

namespace Fahrenheit.Hooks.Generic;

public static partial class FhHooks
{
    [FhHook(HookTarget.X, 0x487C80, typeof(FhHookDelegates.TkIsDebugDelegate))]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
    public static int TkIsDebugHook()
    {
        FhLog.Log(LogLevel.Info, "Invoked.");
        return 0;
    }
}
