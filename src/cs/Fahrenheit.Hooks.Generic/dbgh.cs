using System;
using System.Runtime.InteropServices;

using Fahrenheit.CoreLib;

namespace Fahrenheit.Hooks.Generic;

public static partial class FhHooks
{
    [FhHook(FhHookTarget.FFX, 0x487C80, typeof(FhHookDelegates.TkIsDebugDelegate))]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
    public static unsafe int TkIsDebugHook()
    {
        return 0;
    }
}
