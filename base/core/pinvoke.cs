namespace Fahrenheit.Core;

/* [fkelava 6/6/23 21:29]
 * Vararg P/Invokes must be declared assembly-local due to https://github.com/dotnet/runtime/issues/87188.
 */

internal static unsafe partial class FhPInvoke {
    [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial nint GetModuleHandle(string? lpModuleName);

    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
    internal static partial nint GetProcAddress(nint hModule, string lpProcName);

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
            nint  pTarget,
            nint  pDetour,
            nint* ppOriginal);

    [LibraryImport(hook_lib_name)]
    internal static partial int MH_CreateHookApi(
        [MarshalAs(UnmanagedType.LPWStr)] string pszModule,
        [MarshalAs(UnmanagedType.LPStr)]  string pszProcName,
                                          nint   pDetour,
                                          nint*  ppOriginal);

    [LibraryImport(hook_lib_name)]
    internal static partial int MH_CreateHookApiEx(
        [MarshalAs(UnmanagedType.LPWStr)] string pszModule,
        [MarshalAs(UnmanagedType.LPStr)]  string pszProcName,
                                          nint   pDetour,
                                          nint*  ppOriginal,
                                          nint*  ppTarget);

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
