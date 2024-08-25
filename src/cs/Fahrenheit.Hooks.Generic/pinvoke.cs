using System;
using System.Runtime.InteropServices;

namespace Fahrenheit.Hooks.Generic;

/* [fkelava 6/6/23 21:29]
 * P/Invokes must be declared here because of https://github.com/dotnet/runtime/issues/87188.
 * 
 * Until this is fixed, P/Invokes cannot be declared in fhcorlib and you must therefore carry your own P/Invokes.
 * 
 * Always declare your P/Invokes internal if bundling them in the hook library as such!
 */

internal static partial class FhPInvoke {
    internal const uint INFINITE = 4294967295;

#pragma warning disable SYSLIB1054 // Specifically disabled because LibraryImportAttribute cannot handle __arglist, presumably.
    [DllImport("msvcrt.dll", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern int sprintf(IntPtr buffer, string format, __arglist);

    [DllImport("msvcrt.dll", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern int _scprintf(string format, __arglist);

    [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW", StringMarshalling = StringMarshalling.Utf16)]
    public static partial nint GetModuleHandle(string lpModuleName);
#pragma warning restore SYSLIB1054

    [DllImport("kernel32.dll")]
    public static extern nint GetProcAddress(nint hModule, string lpProcName);

    [DllImport("user32.dll", SetLastError = true)]
    static extern nint FindWindow(string lpClassName, string lpWindowName);

    public static nint FindWindow(string caption) {
        return FindWindow(null, caption);
    }
}
