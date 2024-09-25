using System;

namespace Fahrenheit.CoreLib;

/* [fkelava 6/6/23 21:29]
 * Vararg P/Invokes must be declared assembly-local due to https://github.com/dotnet/runtime/issues/87188.
 */

internal static unsafe partial class FhPInvoke {
    internal const uint INFINITE     = 4294967295;
    internal const int  GWLP_WNDPROC = -4;

    [LibraryImport("user32.dll")]
    internal static partial nint CallWindowProcW(nint lpPrevWndFunc, nint hWnd, uint msg, nint wParam, nint lParam);

    [LibraryImport("user32.dll")]
    internal static partial nint GetWindowLongA(nint hWnd, int nIndex);

    [LibraryImport("user32.dll")]
    internal static partial nint GetWindowLongPtrW(nint hWnd, int nIndex);

    [LibraryImport("user32.dll")]
    internal static partial int SetWindowLongA(nint hWnd, int nIndex, nint dwNewLong);

    [LibraryImport("user32.dll")]
    internal static partial nint SetWindowLongPtrW(nint hWnd, int nIndex, nint dwNewLong);

    // shitfuck hack
    [LibraryImport("user32.dll", EntryPoint = "FindWindowA", SetLastError = true, StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
    internal static partial nint FindWindow(string? lpClassName, string lpWindowName);

    [LibraryImport("kernelbase.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool WaitOnAddress(void* Address, void* CompareAddress, nint AddressSize, uint dwMilliseconds);

    [LibraryImport("kernelbase.dll", SetLastError = true)]
    internal static partial void WakeByAddressSingle(void* Address);

    [LibraryImport("kernelbase.dll", SetLastError = true)]
    internal static partial void WakeByAddressAll(void* Address);

    [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial nint GetModuleHandle(string lpModuleName);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    internal static partial int SuspendThread(nint hThread);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    internal static partial int ResumeThread(nint hThread);

    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
    internal static partial nint GetProcAddress(nint hModule, string lpProcName);

    [LibraryImport("kernel32.dll")]
    internal static partial nint GetCurrentThread();

    [LibraryImport("kernel32.dll", EntryPoint = "LoadLibraryW", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial nint LoadLibrary(string lpModuleName);

#pragma warning disable SYSLIB1054 // Specifically disabled because LibraryImportAttribute cannot handle __arglist, presumably.
    [DllImport("msvcrt.dll", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern int sprintf(IntPtr buffer, string format, __arglist);

    [DllImport("msvcrt.dll", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern int _scprintf(string format, __arglist);
#pragma warning restore SYSLIB1054

    //[LibraryImport("winmm.dll")]
    //public static partial nint timeBeginPeriod(uint uPeriod);

#if DEBUG
    private const string hook_lib_name = "minhook.x32d.dll";
#else
    private const string hook_lib_name = "minhook.x32.dll";
#endif

    [LibraryImport(hook_lib_name)]
    internal static partial int MH_Initialize();

    [LibraryImport(hook_lib_name)]
    internal static partial int MH_Uninitialize();

    [LibraryImport(hook_lib_name)]
    internal static partial int MH_CreateHook(
            nint pTarget,
            nint pDetour,
        ref nint ppOriginal);

    [LibraryImport(hook_lib_name)]
    internal static partial int MH_CreateHookApi(
        [MarshalAs(UnmanagedType.LPWStr)] string pszModule,
        [MarshalAs(UnmanagedType.LPStr)]  string pszProcName,
                                          nint   pDetour,
                                      ref nint   ppOriginal);

    [LibraryImport(hook_lib_name)]
    internal static partial int MH_CreateHookApiEx(
        [MarshalAs(UnmanagedType.LPWStr)] string pszModule,
        [MarshalAs(UnmanagedType.LPStr)]  string pszProcName,
                                          nint   pDetour,
                                      ref nint   ppOriginal,
                                      ref nint   ppTarget);

    [LibraryImport(hook_lib_name)]
    internal static partial int MH_EnableHook(nint pTarget);

    [LibraryImport(hook_lib_name)]
    internal static partial int MH_RemoveHook(nint pTarget);

    [LibraryImport(hook_lib_name)]
    internal static partial int MH_DisableHook(nint pTarget);

    [LibraryImport(hook_lib_name)]
    internal static partial int MH_QueueEnableHook(nint pTarget);

    [LibraryImport(hook_lib_name)]
    internal static partial int MH_QueueDisableHook(nint pTarget);

    [LibraryImport(hook_lib_name)]
    internal static partial int MH_ApplyQueued();
}
