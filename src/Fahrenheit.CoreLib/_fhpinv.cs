namespace Fahrenheit.CoreLib;

/* [fkelava 6/6/23 21:29]
 * Vararg P/Invokes cannot be declared here because of https://github.com/dotnet/runtime/issues/87188.
 *
 * Until this is fixed, vararg P/Invokes must be declared in the hook assembly itself.
 */

internal static unsafe partial class FhPInvoke {
    internal const uint INFINITE = 4294967295;

    [LibraryImport("kernelbase.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool WaitOnAddress(void* Address, void* CompareAddress, nint AddressSize, uint dwMilliseconds);

    [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW", StringMarshalling = StringMarshalling.Utf16)]
    public static partial nint GetModuleHandle(string lpModuleName);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    public static partial int SuspendThread(nint hThread);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    public static partial int ResumeThread(nint hThread);

    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
    public static partial nint GetProcAddress(nint hModule, string lpProcName);

    [LibraryImport("kernel32.dll")]
    public static partial nint GetCurrentThread();

    [LibraryImport("kernel32.dll", EntryPoint = "LoadLibraryW", StringMarshalling = StringMarshalling.Utf16)]
    public static partial nint LoadLibrary(string lpModuleName);

    //[LibraryImport("winmm.dll")]
    //public static partial nint timeBeginPeriod(uint uPeriod);

#if DEBUG
    private const string hook_lib_name = "minhook.x32d.dll";
#else
    private const string hook_lib_name = "minhook.x32.dll";
#endif

    [LibraryImport(hook_lib_name)]
    public static partial int MH_Initialize();

    [LibraryImport(hook_lib_name)]
    public static partial int MH_Uninitialize();

    [LibraryImport(hook_lib_name)]
    public static partial int MH_CreateHook(
            nint pTarget,
            nint pDetour,
        ref nint ppOriginal);

    [LibraryImport(hook_lib_name)]
    public static partial int MH_CreateHookApi(
        [MarshalAs(UnmanagedType.LPWStr)] string pszModule,
        [MarshalAs(UnmanagedType.LPStr)]  string pszProcName,
                                          nint   pDetour,
                                      ref nint   ppOriginal);

    [LibraryImport(hook_lib_name)]
    public static partial int MH_CreateHookApiEx(
        [MarshalAs(UnmanagedType.LPWStr)] string pszModule,
        [MarshalAs(UnmanagedType.LPStr)]  string pszProcName,
                                          nint   pDetour,
                                      ref nint   ppOriginal,
                                      ref nint   ppTarget);

    [LibraryImport(hook_lib_name)]
    public static partial int MH_EnableHook(nint pTarget);

    [LibraryImport(hook_lib_name)]
    public static partial int MH_RemoveHook(nint pTarget);

    [LibraryImport(hook_lib_name)]
    public static partial int MH_DisableHook(nint pTarget);

    [LibraryImport(hook_lib_name)]
    public static partial int MH_QueueEnableHook(nint pTarget);

    [LibraryImport(hook_lib_name)]
    public static partial int MH_QueueDisableHook(nint pTarget);

    [LibraryImport(hook_lib_name)]
    public static partial int MH_ApplyQueued();
}
