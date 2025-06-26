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
    internal const int  GWLP_WNDPROC              = -4;

    [LibraryImport("user32.dll")]
    internal static partial nint CallWindowProcW(
        nint  lpPrevWndFunc,
        nint  hWnd,
        uint  msg,
        nuint wParam,
        nint  lParam);

    [LibraryImport("user32.dll")]
    internal static partial nint GetWindowLongA(
        nint hWnd,
        int  nIndex);

    [LibraryImport("user32.dll")]
    internal static partial int SetWindowLongA(
        nint hWnd,
        int  nIndex,
        nint dwNewLong);

    // hacky
    [LibraryImport("user32.dll",
        EntryPoint                  = "FindWindowA",
        SetLastError                = true,
        StringMarshalling           = StringMarshalling.Custom,
        StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
    internal static partial nint FindWindow(
        string? lpClassName,
        string  lpWindowName);

    [LibraryImport("kernelbase.dll",
        SetLastError = true)]
    internal static partial void WakeByAddressAll(
        void* Address);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate nint WndProcDelegate(
        nint  hWnd,
        uint  msg,
        nuint wParam,
        nint  lParam);

    /* [fkelava 6/10/2024 00:57]
     * https://learn.microsoft.com/en-us/windows/win32/api/d3d11/nf-d3d11-d3d11createdeviceandswapchain
     */

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate nint D3D11CreateDeviceAndSwapChain(
        nint*  pAdapter,
        nint   DriverType,
        nint   Software,
        uint   Flags,
        nint*  pFeatureLevels,
        uint   FeatureLevels,
        uint   SDKVersion,
        nint*  pSwapChainDesc,
        nint** ppSwapChain,
        nint** ppDevice,
        nint*  pFeatureLevel,
        nint** ppImmediateContext);
}
