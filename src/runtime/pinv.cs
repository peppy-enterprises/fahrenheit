using System.Runtime.InteropServices;

namespace Fahrenheit.Core.Runtime;

internal static unsafe partial class PInvoke {
    internal const int GWLP_WNDPROC = -4;

    [LibraryImport("user32.dll")]
    internal static partial nint CallWindowProcW(
        nint lpPrevWndFunc,
        nint hWnd,
        uint msg,
        nint wParam,
        nint lParam);

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
    [LibraryImport("user32.dll", EntryPoint = "FindWindowA", SetLastError = true, StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
    internal static partial nint FindWindow(
        string? lpClassName,
        string  lpWindowName);

    [LibraryImport("kernelbase.dll", SetLastError = true)]
    internal static partial void WakeByAddressAll(
        void* Address);

    [DllImport("cimgui.dll")]
    public static extern nint ImGui_ImplWin32_WndProcHandler(
        nint hWnd,
        uint msg,
        nint wParam,
        nint lParam);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate nint WndProcDelegate(
        nint hWnd,
        uint msg,
        nint wParam,
        nint lParam);

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