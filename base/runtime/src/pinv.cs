namespace Fahrenheit.Core.Runtime;

internal static unsafe partial class PInvoke {
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate nint WndProcDelegate(
        HWND   hWnd,
        uint   msg,
        WPARAM wParam,
        LPARAM lParam);
}
