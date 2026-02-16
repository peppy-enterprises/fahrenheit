// SPDX-License-Identifier: MIT

/* [fkelava 5/7/25 14:16]
 * Hexa bundles some definitions for D3D11 structures that we need to use when interfacing
 * with its API. They are defined this way because we prefer the TerraFX definitions in all other cases.
 */
using HexaID3D11Device           = Hexa.NET.ImGui.Backends.D3D11.ID3D11Device;
using HexaID3D11DeviceContext    = Hexa.NET.ImGui.Backends.D3D11.ID3D11DeviceContext;
using HexaID3D11DeviceContextPtr = Hexa.NET.ImGui.Backends.D3D11.ID3D11DeviceContextPtr;
using HexaID3D11DevicePtr        = Hexa.NET.ImGui.Backends.D3D11.ID3D11DevicePtr;

using ImGuiImplD3D11             = Hexa.NET.ImGui.Backends.D3D11.ImGuiImplD3D11;
using ImGuiImplWin32             = Hexa.NET.ImGui.Backends.Win32.ImGuiImplWin32;

namespace Fahrenheit.Core.Runtime;

[UnmanagedFunctionPointer(CallingConvention.Winapi)]
internal delegate nint WndProc(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam);

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal delegate nint graphicInitialize();

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal delegate int PInputUpdate();

/* [fkelava 6/10/24 01:54]
 * https://github.com/terrafx/terrafx.interop.windows/blob/55590efae0f77f4c8db465a80d18b4f5b679696c/sources/Interop/Windows/DirectX/shared/dxgi/IDXGISwapChain.cs#L93
 */
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal unsafe delegate nint DXGISwapChain_Present(
    IDXGISwapChain* pSwapChain,
    uint            SyncInterval,
    uint            Flags);

/* [fkelava 25/4/24 17:51]
 * https://github.com/terrafx/terrafx.interop.windows/blob/55590efae0f77f4c8db465a80d18b4f5b679696c/sources/Interop/Windows/DirectX/shared/dxgi/IDXGISwapChain.cs#L133
 */
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal unsafe delegate nint DXGISwapChain_ResizeBuffers(
    IDXGISwapChain* pSwapChain,
    uint            BufferCount,
    uint            Width,
    uint            Height,
    DXGI_FORMAT     NewFormat,
    uint            SwapChainFlags);

/* [fkelava 25/4/24 17:51]
 * https://github.com/terrafx/terrafx.interop.windows/blob/55590efae0f77f4c8db465a80d18b4f5b679696c/sources/Interop/Windows/DirectX/um/d3d11/DirectX.cs#L25
 */
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal unsafe delegate HRESULT DirectX_D3D11CreateDeviceAndSwapChain(
    IDXGIAdapter*         pAdapter,
    D3D_DRIVER_TYPE       DriverType,
    HMODULE               Software,
    uint                  Flags,
    D3D_FEATURE_LEVEL*    pFeatureLevels,
    uint                  FeatureLevels,
    uint                  SDKVersion,
    DXGI_SWAP_CHAIN_DESC* pSwapChainDesc,
    IDXGISwapChain**      ppSwapChain,
    ID3D11Device**        ppDevice,
    D3D_FEATURE_LEVEL*    pFeatureLevel,
    ID3D11DeviceContext** ppImmediateContext);

/* [fkelava 16/2/26 15:28]
 * See below comments at `assign_devices` and `h_implw32_setwindowfocus`.
 */
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal delegate void ImGui_ImplWin32_SetWindowFocus(ImGuiViewportPtr viewport);

/// <summary>
///     Provides the ability to use the ImGui GUI toolkit within the game.
///     <para/>
///     Do not interface with this module directly. Instead, implement <see cref="FhModule.render_imgui"/>.
/// </summary>
[FhLoad(FhGameId.FFX | FhGameId.FFX2 | FhGameId.FFX2LM)]
[SupportedOSPlatform("windows")] // To satisfy CA1416 warning about invoking D3D/DXGI API which TerraFX annotates as supported only on Windows.
public unsafe sealed class FhImguiModule : FhModule, IFhNativeGraphicsUser {

    // WndProc support
    private          HWND                         _hWnd;
    private          nint                         _ptr_o_WndProc;
    private          nint                         _ptr_h_WndProc;
    private readonly WndProc                      _h_WndProc;
    private readonly FhMethodHandle<PInputUpdate> _handle_pinput;

    private IDXGISwapChain*         _ptr_swapchain;  // https://learn.microsoft.com/en-us/windows/win32/api/dxgi/nn-dxgi-idxgiswapchain
    private ID3D11Device*           _ptr_device;     // https://learn.microsoft.com/en-us/windows/win32/api/d3d11/nn-d3d11-id3d11device
    private ID3D11DeviceContext*    _ptr_device_ctx; // https://learn.microsoft.com/en-us/windows/win32/api/d3d11/nn-d3d11-id3d11devicecontext
    private ID3D11Resource*         _ptr_surface;    // https://learn.microsoft.com/en-us/windows/win32/api/d3d11/nn-d3d11-id3d11texture2d
    private ID3D11RenderTargetView* _ptr_rtv;        // https://learn.microsoft.com/en-us/windows/win32/api/d3d11/nn-d3d11-id3d11rendertargetview

    private FhMethodHandle<DXGISwapChain_Present>?       _handle_present;
    private FhMethodHandle<DXGISwapChain_ResizeBuffers>? _handle_resize_buffers;

    private bool _rtv_generated;

    public FhImguiModule() {
        FhMethodLocation loc_pinput = new(0x225930, 0x6B51E0);

        _handle_pinput = new(this, loc_pinput, h_pinput);
        _h_WndProc     = h_wndproc;
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        return _handle_pinput.hook();
    }

    unsafe void IFhNativeGraphicsUser.assign_devices(
        ID3D11Device*        ptr_device,
        ID3D11DeviceContext* ptr_device_context,
        IDXGISwapChain*      ptr_swapchain,
        HWND                 hWnd) {

        _hWnd          = hWnd;
        _ptr_h_WndProc = Marshal.GetFunctionPointerForDelegate(_h_WndProc);
        _ptr_o_WndProc = Windows.GetWindowLongPtrW(_hWnd, GWLP.GWLP_WNDPROC);
        _              = Windows.SetWindowLongPtrW(_hWnd, GWLP.GWLP_WNDPROC, _ptr_h_WndProc);

        _ptr_swapchain  = ptr_swapchain;
        _ptr_device     = ptr_device;
        _ptr_device_ctx = ptr_device_context;

        ImGuiContextPtr    ctx   = ImGui.CreateContext();
        ImGuiIOPtr         io    = ImGui.GetIO();
        ImGuiPlatformIOPtr pio   = ImGui.GetPlatformIO();
        ImGuiStylePtr      style = ImGui.GetStyle();

        // Enable controls
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;

        // Enable features
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;

        io.ConfigDpiScaleFonts     = true;
        io.ConfigDpiScaleViewports = true;

        ImGui.StyleColorsDark();

        HexaID3D11DevicePtr        hexa_p_device     = new((HexaID3D11Device*)       _ptr_device);
        HexaID3D11DeviceContextPtr hexa_p_device_ctx = new((HexaID3D11DeviceContext*)_ptr_device_ctx);

        ImGuiImplWin32.SetCurrentContext(ctx);
        ImGuiImplD3D11.SetCurrentContext(ctx);
        ImGuiImplWin32.Init(_hWnd);
        ImGuiImplD3D11.Init(hexa_p_device, hexa_p_device_ctx);

        /* [fkelava 16/02/26 15:12]
         * It would be best to use [UnmanagedCallersOnly] and &h_implw32_setwindowfocus,
         * but this is not possible because PlatformSetWindowFocus is typed `void*` and CS8812 results.
         */

        pio.PlatformSetWindowFocus = (void*)Marshal.GetFunctionPointerForDelegate
            <ImGui_ImplWin32_SetWindowFocus>(h_implw32_setwindowfocus);

        FhApi.ImGuiHelper.init();

        _handle_present        = new(this, new nint(_ptr_swapchain->lpVtbl[8]),  h_present);
        _handle_resize_buffers = new(this, new nint(_ptr_swapchain->lpVtbl[13]), h_resize_buffers);
        _handle_present       .hook();
        _handle_resize_buffers.hook();
    }

    /* [fkelava 16/02/26 14:59]
     * https://github.com/ocornut/imgui/issues/9242
     *
     * The games utilize a separate rendering thread. In other words, the main message
     * pump is on a different thread from the one we call ImGui functions on.
     * This is not supported, but works mostly fine regardless.
     *
     * However, when Square on a gamepad is used to engage the window switcher, then released,
     * a WM_LBUTTONUP event occurs which often results in a deadlock that freezes both ImGui
     * and the game's message pump. Making ImGui's ImGui_ImplWin32_SetWindowFocus() function
     * use SWP_ASYNCWINDOWPOS curtails it, but the true underlying reason is not known.
     *
     * https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos
     */

    private static void h_implw32_setwindowfocus(ImGuiViewportPtr viewport) {
        if (viewport.PlatformHandleRaw == null)
            return;

        Windows.SetWindowPos       ((HWND)viewport.PlatformHandleRaw, HWND.HWND_TOP, 0, 0, 0, 0, SWP.SWP_ASYNCWINDOWPOS | SWP.SWP_NOSIZE | SWP.SWP_NOMOVE);
        Windows.SetForegroundWindow((HWND)viewport.PlatformHandleRaw);
        Windows.SetFocus           ((HWND)viewport.PlatformHandleRaw);
    }

    /// <summary>
    ///     Allows ImGui to intercept window messages sent to the game, such as <see cref="WM.WM_KEYDOWN"/>,
    ///     enabling mouse and keyboard input to be directed to it.
    /// </summary>
    private nint h_wndproc(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam) {
        return ImGuiImplWin32.WndProcHandler(hWnd, msg, wParam, lParam) == 1
             ? 1
             : Windows.CallWindowProcW((delegate* unmanaged<HWND, uint, WPARAM, LPARAM, LRESULT>)_ptr_o_WndProc, hWnd, msg, wParam, lParam);
    }

    /// <summary>
    ///     Allows interception of raw input from the game, redirecting it to ImGui if appropriate.
    /// </summary>
    private int h_pinput() {
        if (_hWnd           == 0    // h_init_wndproc hasn't run yet?
         || _ptr_device     == null // h_init_d3d11 hasn't run yet?
         || _ptr_device_ctx == null)
            return _handle_pinput.orig_fptr();

        ImGuiIOPtr io = ImGui.GetIO();
        return io.WantCaptureKeyboard || io.WantCaptureMouse
            ? 0
            : _handle_pinput.orig_fptr();
    }

    /// <summary>
    ///     Intercepts attempts to resize the game window to allow ImGui to continue drawing.
    /// </summary>
    private nint h_resize_buffers(IDXGISwapChain* pSwapChain, uint BufferCount, uint Width, uint Height, DXGI_FORMAT NewFormat, uint SwapChainFlags) {
        if (_ptr_device_ctx == null)
            return _handle_resize_buffers!.orig_fptr(pSwapChain, BufferCount, Width, Height, NewFormat, SwapChainFlags);

        if (_ptr_rtv != null) {
            _ptr_device_ctx->OMSetRenderTargets(0, null, null);
            _ptr_rtv       ->Release();
        }

        _rtv_generated = false; // forces regeneration of ImGui surfaces/RTV in next `h_present`
        return _handle_resize_buffers!.orig_fptr(pSwapChain, BufferCount, Width, Height, NewFormat, SwapChainFlags);
    }

    /// <summary>
    ///     Overrides the game's <see cref="IDXGISwapChain.Present(uint, uint)"/> call to display mods' user interfaces.
    /// </summary>
    private nint h_present(IDXGISwapChain* pSwapChain, uint SyncInterval, uint Flags) {
        if (!_rtv_generated) {
            fixed (ID3D11Resource**         ppSurface = &_ptr_surface)
            fixed (ID3D11RenderTargetView** ppRTView  = &_ptr_rtv) {
                _ptr_swapchain->GetBuffer(0, Windows.__uuidof<ID3D11Texture2D>(), (void**)ppSurface);
                _ptr_device   ->CreateRenderTargetView(_ptr_surface, null, ppRTView);
                _ptr_surface  ->Release();
            }

            _rtv_generated = true;
        }

        ImGuiImplD3D11.NewFrame();
        ImGuiImplWin32.NewFrame();

        ImGui.NewFrame();

        foreach (FhModuleContext module_ctx in FhApi.Mods.get_modules()) {
            module_ctx.Module.render_imgui();
        }

        ImGui.Render();

        fixed (ID3D11RenderTargetView** ppRenderTargetViews = &_ptr_rtv) {
            _ptr_device_ctx->OMSetRenderTargets(1, ppRenderTargetViews, null);
        }

        ImGuiImplD3D11.RenderDrawData(ImGui.GetDrawData());

        return _handle_present!.orig_fptr(pSwapChain, SyncInterval, Flags);
    }
}
