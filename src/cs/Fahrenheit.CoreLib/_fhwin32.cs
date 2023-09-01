namespace Fahrenheit.CoreLib;

/* [fkelava 6/6/23 21:29]
 * Vararg P/Invokes cannot be declared here because of https://github.com/dotnet/runtime/issues/87188.
 * 
 * Until this is fixed, vararg P/Invokes must be declared in the hook assembly itself.
 */

internal static unsafe partial class FhPInvoke
{
    internal const uint INFINITE = 4294967295;

    [LibraryImport("kernelbase.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool WaitOnAddress(void* Address, void* CompareAddress, nint AddressSize, uint dwMilliseconds);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    public static partial int SuspendThread(nint hThread);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    public static partial int ResumeThread(nint hThread);
    
    [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW", StringMarshalling = StringMarshalling.Utf16)] 
    public static partial nint GetModuleHandle(string lpModuleName);
}
