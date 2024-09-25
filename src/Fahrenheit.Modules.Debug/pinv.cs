using System;
using System.Runtime.InteropServices;

namespace Fahrenheit.Modules.Debug;

internal static class PInvoke {
    [DllImport("msvcrt.dll", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern int _scprintf(string format, __arglist);

    [DllImport("msvcrt.dll", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern int sprintf(IntPtr buffer, string format, __arglist);
}