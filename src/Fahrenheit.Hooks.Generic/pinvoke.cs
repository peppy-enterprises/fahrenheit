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
    internal const uint INFINITE     = 4294967295;
    internal const int  GWLP_WNDPROC = -4;

    [LibraryImport("user32.dll", EntryPoint = "CallWindowProcW")]
    public static partial nint CallWindowProc(nint lpPrevWndFunc, nint hWnd, uint msg, nint wParam, nint lParam);

    public static nint GetWindowLong(nint hWnd, int nIndex) {
        return nint.Size == 8
            ? GetWindowLongPtr64(hWnd, nIndex)
            : GetWindowLong32   (hWnd, nIndex);
    }

    [LibraryImport("user32.dll", EntryPoint = "GetWindowLongA")]
    private static partial nint GetWindowLong32(nint hWnd, int nIndex);

    [LibraryImport("user32.dll", EntryPoint = "GetWindowLongPtrW")]
    private static partial nint GetWindowLongPtr64(nint hWnd, int nIndex);

    // This static method is required because legacy OSes do not support
    // SetWindowLongPtr
    public static nint SetWindowLongPtr(nint hWnd, int nIndex, nint dwNewLong) {
        return nint.Size == 8
            ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong)
            : SetWindowLong32   (hWnd, nIndex, dwNewLong);
    }

    [LibraryImport("user32.dll", EntryPoint = "SetWindowLongA")]
    private static partial int SetWindowLong32(nint hWnd, int nIndex, nint dwNewLong);

    [LibraryImport("user32.dll", EntryPoint = "SetWindowLongPtrW")]
    private static partial nint SetWindowLongPtr64(nint hWnd, int nIndex, nint dwNewLong);

    // hacky
    [LibraryImport("user32.dll", EntryPoint = "FindWindowA", SetLastError = true, StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
    public static partial IntPtr FindWindow(string? lpClassName, string lpWindowName);

    public static nint FindWindow(string lpWindowName) {
        return FindWindow(null, lpWindowName);
    }

#pragma warning disable SYSLIB1054 // Specifically disabled because LibraryImportAttribute cannot handle __arglist, presumably.
    [DllImport("msvcrt.dll", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern int sprintf(IntPtr buffer, string format, __arglist);

    [DllImport("msvcrt.dll", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern int _scprintf(string format, __arglist);

    [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW", StringMarshalling = StringMarshalling.Utf16)]
    public static partial nint GetModuleHandle(string lpModuleName);

    // what the *fuck* is this
    [DllImport("cimgui.dll")]
    public static extern nint ImGui_ImplWin32_WndProcHandler(nint hWnd, uint msg, nint wParam, nint lParam);
#pragma warning restore SYSLIB1054
}
