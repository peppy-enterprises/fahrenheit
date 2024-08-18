using System;
using System.Runtime.InteropServices;

using Fahrenheit.CoreLib;

namespace Fahrenheit.Hooks.Generic;

public static partial class FhHooks {
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    public static void CLRPrintfHookAnsi(string fmt, nint va0, nint va1, nint va2, nint va3, nint va4, nint va5, nint va6, nint va7, nint va8, nint va9, nint va10, nint va11, nint va12, nint va13, nint va14, nint va15) {
        int argc = 0;
        fmt = fmt.Trim();

        for (int i = 0; i < fmt.Length; i++) if (fmt[i] == '%') argc++;

        int bl = argc switch {
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

        try {
            int rv = argc switch {
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

            FhLog.Log(LogLevel.Info, Marshal.PtrToStringAnsi(buf) ?? "FH_E_PFHOOK_STRING_NUL");
        }
        finally {
            Marshal.FreeHGlobal(buf);
        }
    }
}
