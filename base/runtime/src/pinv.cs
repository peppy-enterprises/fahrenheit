namespace Fahrenheit.Core.Runtime;

internal static unsafe partial class PInvoke {
    [LibraryImport("kernel32.dll",
        StringMarshalling = StringMarshalling.Utf16)]
    internal static partial nint CreateFileW(
        string lpFileName,
        uint   dwDesiredAccess,
        uint   dwShareMode,
        nint   lpSecurityAttributes,
        uint   dwCreationDisposition,
        uint   dwFlagsAndAttributes,
        nint   hTemplateFile);

    // https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-createfilew
    internal const uint FILE_READ_DATA            = 1;
    internal const uint FILE_WRITE_DATA           = 2;
    internal const uint FILE_SHARE_READ           = 1;
    internal const uint OPEN_EXISTING             = 3;
    internal const uint OPEN_ALWAYS               = 4;
    internal const uint FILE_FLAG_SEQUENTIAL_SCAN = 0x08000000;

    [LibraryImport("user32.dll")]
    internal static partial nint CallWindowProcW(
        nint  lpPrevWndFunc,
        nint  hWnd,
        uint  msg,
        nuint wParam,
        nint  lParam);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate nint WndProcDelegate(
        nint  hWnd,
        uint  msg,
        nuint wParam,
        nint  lParam);
}
