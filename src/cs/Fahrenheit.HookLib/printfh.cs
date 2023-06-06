using System;
using System.IO;
using System.Runtime.InteropServices;

using Fahrenheit.CoreLib;

namespace Fahrenheit.HookLib;

public static class Hook
{
    private static readonly object       _hookLock;
    private static readonly StreamWriter _logFile;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void CLRPrintfDelegate(
        string fmt, 
        nint   va0,
        nint   va1, 
        nint   va2, 
        nint   va3, 
        nint   va4, 
        nint   va5,
        nint   va6,
        nint   va7,
        nint   va8,
        nint   va9,
        nint   va10,
        nint   va11,
        nint   va12,
        nint   va13,
        nint   va14,
        nint   va15
        );

    static Hook()
    {
        _hookLock = new object();
        _logFile  = new StreamWriter(File.Open("clrhost.log", FileMode.Append, FileAccess.Write, FileShare.Read));
    }

    [FhHook(HookTarget.X, 0x22F6B0, typeof(CLRPrintfDelegate))]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    public static void CLRPrintfHook(
        string fmt,
        nint   va0,
        nint   va1,
        nint   va2,
        nint   va3,
        nint   va4,
        nint   va5,
        nint   va6,
        nint   va7,
        nint   va8,
        nint   va9,
        nint   va10,
        nint   va11,
        nint   va12,
        nint   va13,
        nint   va14,
        nint   va15
        )
    {
        int argc = 0;

        for (int i = 0; i < fmt.Length; i++) if (fmt[i] == '%') argc++;
        FhLog.Log(LogLevel.Info, $"fmt: {fmt}");
    }
}
