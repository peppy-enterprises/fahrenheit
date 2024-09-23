﻿using System;
using System.Runtime.InteropServices;

namespace Fahrenheit.Modules.Core;

internal static unsafe partial class PInvoke {
    internal const int  GWLP_WNDPROC = -4;

    [LibraryImport("user32.dll")]
    internal static partial nint CallWindowProcW(nint lpPrevWndFunc, nint hWnd, uint msg, nint wParam, nint lParam);

    [LibraryImport("user32.dll")]
    internal static partial nint GetWindowLongA(nint hWnd, int nIndex);

    [LibraryImport("user32.dll")]
    internal static partial int SetWindowLongA(nint hWnd, int nIndex, nint dwNewLong);

    // hacky
    [LibraryImport("user32.dll", EntryPoint = "FindWindowA", SetLastError = true, StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
    internal static partial nint FindWindow(string? lpClassName, string lpWindowName);

    public static nint FindWindow(string lpWindowName) {
        return FindWindow(null, lpWindowName);
    }

    [LibraryImport("kernelbase.dll", SetLastError = true)]
    internal static partial void WakeByAddressAll(void* Address);

    [DllImport("cimgui.dll")]
    public static extern nint ImGui_ImplWin32_WndProcHandler(nint hWnd, uint msg, nint wParam, nint lParam);
}