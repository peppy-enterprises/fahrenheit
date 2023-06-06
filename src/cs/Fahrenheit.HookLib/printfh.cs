using System;
using System.IO;
using System.Runtime.InteropServices;

using Fahrenheit.CoreLib;

namespace Fahrenheit.HookLib;

/* [fkelava 6/6/23 21:29]
 * P/Invokes must be declared here because of https://github.com/dotnet/runtime/issues/87188.
 * 
 * Until this is fixed, P/Invokes cannot be declared in fhcorlib and you must therefore carry your own P/Invokes.
 * 
 * Always declare your P/Invokes internal if bundling them in the hook library as such!
 */

internal static class FhPInvoke
{
#pragma warning disable SYSLIB1054 // Specifically disabled because LibraryImportAttribute cannot handle __arglist, presumably.
    [DllImport("msvcrt.dll", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern int sprintf(IntPtr buffer, string format, __arglist);

    [DllImport("msvcrt.dll", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern int _scprintf(string format, __arglist);
#pragma warning restore SYSLIB1054
}

public static class FhHookDelegates
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void PrintfVarargDelegate(string fmt, nint va0, nint va1, nint va2, nint va3, nint va4, nint va5, nint va6, nint va7, nint va8, nint va9, nint va10, nint va11, nint va12, nint va13, nint va14, nint va15);
}

public static class FhHooks
{
    private static readonly StreamWriter _logFile;

    static FhHooks()
    {
        _logFile = new StreamWriter(File.Open("dbgPrintf.log", FileMode.Append, FileAccess.Write, FileShare.Read));
    }

    [FhHook(HookTarget.X, 0x22F6B0, typeof(FhHookDelegates.PrintfVarargDelegate))]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    public static void CLRPrintfHookAnsi(string fmt, nint va0, nint va1, nint va2, nint va3, nint va4, nint va5, nint va6, nint va7, nint va8, nint va9, nint va10, nint va11, nint va12, nint va13, nint va14, nint va15)
    {
        int argc = 0;

        for (int i = 0; i < fmt.Length; i++) if (fmt[i] == '%') argc++;

        int bl = argc switch
        {
            0  => FhPInvoke._scprintf(fmt, __arglist()),
            1  => FhPInvoke._scprintf(fmt, __arglist(va0)),
            2  => FhPInvoke._scprintf(fmt, __arglist(va0, va1)),
            3  => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2)),
            4  => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3)),
            5  => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4)),
            6  => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5)),
            7  => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6)),
            8  => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7)),
            9  => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8)),
            10 => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9)),
            11 => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10)),
            12 => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11)),
            13 => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12)),
            14 => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12, va13)),
            15 => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12, va13, va14)),
            16 => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12, va13, va14, va15)),
            _  => throw new Exception("FH_E_PFHOOK_RAH_OVERREACH")
        };

        nint buf = Marshal.AllocHGlobal(bl + 1);

        try
        {
            int rv = argc switch
            {
                0  => FhPInvoke.sprintf(buf, fmt, __arglist()),
                1  => FhPInvoke.sprintf(buf, fmt, __arglist(va0)),
                2  => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1)),
                3  => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2)),
                4  => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3)),
                5  => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4)),
                6  => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5)),
                7  => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6)),
                8  => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7)),
                9  => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8)),
                10 => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9)),
                11 => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10)),
                12 => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11)),
                13 => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12)),
                14 => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12, va13)),
                15 => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12, va13, va14)),
                16 => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12, va13, va14, va15)),
                _  => throw new Exception("FH_E_PFHOOK_RAH_OVERREACH")
            };

            _logFile.Write(Marshal.PtrToStringAnsi(buf));
        }
        finally
        {
            Marshal.FreeHGlobal(buf);
        }
    }
}
