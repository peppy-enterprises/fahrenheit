namespace Fahrenheit.CoreLib;

/* [fkelava 6/6/23 21:29]
 * Vararg P/Invokes cannot be declared here because of https://github.com/dotnet/runtime/issues/87188.
 * 
 * Until this is fixed, vararg P/Invokes must be declared in the hook assembly itself.
 */

internal static partial class FhPInvoke
{
    internal const uint INFINITE = 4294967295;

    [LibraryImport("KERNELBASE.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static unsafe partial bool WaitOnAddress(void* Address, void* CompareAddress, nint AddressSize, uint dwMilliseconds);
    // Actually void* Address, void* CompareAddress, but nint is more practical.
}
