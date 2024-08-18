using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Fahrenheit.CLRHost;

internal static partial class FhPInvoke
{
    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
    public static partial nint GetProcAddress(nint hModule, string lpProcName);

    [LibraryImport("kernel32.dll")]
    public static partial nint GetCurrentThread();

    [LibraryImport("kernel32.dll", EntryPoint = "LoadLibraryW", StringMarshalling = StringMarshalling.Utf16)]
    public static partial nint LoadLibrary(string lpModuleName);

    [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW", StringMarshalling = StringMarshalling.Utf16)]
    public static partial nint GetModuleHandle(string lpModuleName);

    //[LibraryImport("winmm.dll")]
    //public static partial nint timeBeginPeriod(uint uPeriod);

    [LibraryImport("fhdetour.dll")]
    public static partial long DetourAttach(ref nint a, nint b);

    [LibraryImport("fhdetour.dll")]
    public static partial long DetourDetach(ref nint a, nint b);

    [LibraryImport("fhdetour.dll")]
    public static partial long DetourUpdateThread(nint a);

    [LibraryImport("fhdetour.dll")]
    public static partial long DetourTransactionBegin();

    [LibraryImport("fhdetour.dll")]
    public static partial long DetourTransactionCommit();

    [LibraryImport("fhdetour.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FhDetourPatchIAT(nint hModule, nint import, nint real);

    [LibraryImport("fhdetour.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FhDetourUnpatchIAT(nint hModule, nint import, nint real);

    [LibraryImport("fhclrldr.dll", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller))]
    public static partial void DetoursCLRSetGetProcAddressCache(nint hModule, string procName, nint real);
}
