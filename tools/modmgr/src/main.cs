/* [fkelava 5/7/25 14:16]
 * Hexa bundles some definitions for D3D11 structures that we need to use when interfacing
 * with its API. They are defined this way because we prefer the TerraFX definitions in all other cases.
 */
using HexaID3D11Device           = Hexa.NET.ImGui.Backends.D3D11.ID3D11Device;
using HexaID3D11DeviceContext    = Hexa.NET.ImGui.Backends.D3D11.ID3D11DeviceContext;
using HexaID3D11DeviceContextPtr = Hexa.NET.ImGui.Backends.D3D11.ID3D11DeviceContextPtr;
using HexaID3D11DevicePtr        = Hexa.NET.ImGui.Backends.D3D11.ID3D11DevicePtr;

using ImGuiImplD3D11 = Hexa.NET.ImGui.Backends.D3D11.ImGuiImplD3D11;
using ImGuiImplWin32 = Hexa.NET.ImGui.Backends.Win32.ImGuiImplWin32;

namespace Fahrenheit.Tools.ModManager;

/* [fkelava 15/8/25 17:41]
 * This is the mod manager program stub. Do not edit this unless you encounter a bug.
 * To edit the mod manager UI, please open `ui.cs`.
 */

[SupportedOSPlatform("windows")]
internal sealed unsafe class Program {

    // Win32
    private static HWND        _hWnd;
    private static WNDCLASSEXW _wndClass;

    // D3D11
    private static IDXGISwapChain*         _p_swap_chain;         // https://learn.microsoft.com/en-us/windows/win32/api/dxgi/nn-dxgi-idxgiswapchain
    private static ID3D11Device*           _p_device;             // https://learn.microsoft.com/en-us/windows/win32/api/d3d11/nn-d3d11-id3d11device
    private static ID3D11DeviceContext*    _p_device_ctx;         // https://learn.microsoft.com/en-us/windows/win32/api/d3d11/nn-d3d11-id3d11devicecontext
    private static ID3D11RenderTargetView* _p_render_target_view; // https://learn.microsoft.com/en-us/windows/win32/api/d3d11/nn-d3d11-id3d11rendertargetview

    // Window state
    private static          bool    _swap_chain_occluded;
    private static          uint    _resize_w;
    private static          uint    _resize_h;
    private static readonly float[] _rtv_clear_color = [ 0.45F, 0.55F, 0.60F, 1F ];

    /* [fkelava 15/8/25 20:15]
     * the example gives a confusing usage of `_rtv_clear_color`/`clear_color`:
     *
     * L93:  ImVec4 clear_color = ImVec4(0.45f, 0.55f, 0.60f, 1.00f);
     * L150: ImGui::ColorEdit3("clear color", (float*)&clear_color); // Edit 3 floats representing a color
     * L173: const float clear_color_with_alpha[4] = { clear_color.x * clear_color.w, clear_color.y * clear_color.w, clear_color.z * clear_color.w, clear_color.w };
     * L175: g_pd3dDeviceContext->ClearRenderTargetView(g_mainRenderTargetView, clear_color_with_alpha);
     *
     * but clear_color.w is always going to be 1F, so L173 seems spurious. i opted to use it directly as a result.
     */

    /* [fkelava 15/8/25 17:13]
     * https://github.com/ocornut/imgui/blob/master/examples/example_win32_directx11/main.cpp#L260.
     */
    [UnmanagedCallersOnly]
    private static LRESULT _wnd_proc(HWND window, uint message, WPARAM wParam, LPARAM lParam) {
        if (ImGuiImplWin32.WndProcHandler(window, message, wParam, lParam) == 1)
            return 1;

        switch (message) {
            case WM.WM_SIZE:
                if (wParam == Windows.SIZE_MINIMIZED)
                    return 0;

                _resize_w = Windows.LOWORD(lParam);
                _resize_h = Windows.HIWORD(lParam);
                return 0;
            case WM.WM_SYSCOMMAND:
                if ((wParam & 0xFFF0) == SC.SC_KEYMENU) // Disable ALT application menu
                    return 0;
                break;
            case WM.WM_DESTROY:
                Windows.PostQuitMessage(0);
                return 0;
        }

        return Windows.DefWindowProcW(window, message, wParam, lParam);
    }

    /* [fkelava 15/8/25 17:13]
     * https://github.com/ocornut/imgui/blob/master/examples/example_win32_directx11/main.cpp#L198
     */
    private static bool _create_d3d_device() {
        DXGI_SWAP_CHAIN_DESC swap_chain_desc;

        swap_chain_desc.BufferCount        = 2;
        swap_chain_desc.BufferDesc.Width   = 0;
        swap_chain_desc.BufferDesc.Height  = 0;
        swap_chain_desc.BufferDesc.Format  = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
        swap_chain_desc.BufferDesc.RefreshRate.Numerator   = 60;
        swap_chain_desc.BufferDesc.RefreshRate.Denominator = 1;
        swap_chain_desc.Flags              = (uint)DXGI_SWAP_CHAIN_FLAG.DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH;
        swap_chain_desc.BufferUsage        = DXGI.DXGI_USAGE_RENDER_TARGET_OUTPUT;
        swap_chain_desc.OutputWindow       = _hWnd;
        swap_chain_desc.SampleDesc.Count   = 1;
        swap_chain_desc.SampleDesc.Quality = 0;
        swap_chain_desc.Windowed           = true;
        swap_chain_desc.SwapEffect         = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_DISCARD;

        D3D11_CREATE_DEVICE_FLAG create_device_flags = 0;
        D3D_FEATURE_LEVEL[]      d3d_feature_levels  = [
            D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0,
            D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_10_0 ];

        D3D_FEATURE_LEVEL        actual_feature_level;

        fixed (ID3D11Device**        pp_device       = &_p_device)
        fixed (ID3D11DeviceContext** pp_device_ctx   = &_p_device_ctx)
        fixed (IDXGISwapChain**      pp_swap_chain   = &_p_swap_chain)
        fixed (D3D_FEATURE_LEVEL*    p_feature_level = d3d_feature_levels) {
            HRESULT hr = DirectX.D3D11CreateDeviceAndSwapChain(
                pAdapter:           null,
                DriverType:         D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE,
                Software:           HMODULE.NULL,
                Flags:              (uint)create_device_flags,
                pFeatureLevels:     p_feature_level,
                FeatureLevels:      (uint)d3d_feature_levels.Length,
                SDKVersion:         D3D11.D3D11_SDK_VERSION,
                pSwapChainDesc:     &swap_chain_desc,
                ppSwapChain:        pp_swap_chain,
                ppDevice:           pp_device,
                pFeatureLevel:      &actual_feature_level,
                ppImmediateContext: pp_device_ctx);

            if (hr == DXGI.DXGI_ERROR_UNSUPPORTED) {
                hr = DirectX.D3D11CreateDeviceAndSwapChain(
                    pAdapter:           null,
                    DriverType:         D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_WARP,
                    Software:           HMODULE.NULL,
                    Flags:              (uint)create_device_flags,
                    pFeatureLevels:     p_feature_level,
                    FeatureLevels:      (uint)d3d_feature_levels.Length,
                    SDKVersion:         D3D11.D3D11_SDK_VERSION,
                    pSwapChainDesc:     &swap_chain_desc,
                    ppSwapChain:        pp_swap_chain,
                    ppDevice:           pp_device,
                    pFeatureLevel:      &actual_feature_level,
                    ppImmediateContext: pp_device_ctx);
            }

            if (hr != S.S_OK) return false;

            _create_render_target();
            return true;
        }
    }

    /* [fkelava 15/8/25 17:13]
     * https://github.com/ocornut/imgui/blob/master/examples/example_win32_directx11/main.cpp#L231
     */
    private static void _destroy_d3d_device() {
        _destroy_render_target();

        if (_p_swap_chain != null) { _p_swap_chain->Release(); _p_swap_chain = null; }
        if (_p_device_ctx != null) { _p_device_ctx->Release(); _p_device_ctx = null; }
        if (_p_device     != null) { _p_device    ->Release(); _p_device     = null; }
    }

    /* [fkelava 15/8/25 17:13]
     * https://github.com/ocornut/imgui/blob/master/examples/example_win32_directx11/main.cpp#L239
     */
    private static void _create_render_target() {
        ID3D11Texture2D* pBackBuffer;
        Guid             guid = new("6F15AAF2-D208-4E89-9AB4-489535D34F9C");

        fixed (ID3D11RenderTargetView** pp_rtv = &_p_render_target_view) {
            _p_swap_chain->GetBuffer(0, &guid, (void**)&pBackBuffer);
            _p_device    ->CreateRenderTargetView((ID3D11Resource*)pBackBuffer, null, pp_rtv);
            pBackBuffer  ->Release();
        }
    }

    /* [fkelava 15/8/25 17:13]
     * https://github.com/ocornut/imgui/blob/master/examples/example_win32_directx11/main.cpp#L247
     */
    private static void _destroy_render_target() {
        if (_p_render_target_view == null) return;

        _p_render_target_view->Release();
        _p_render_target_view = null;
    }

    /* [fkelava 15/8/25 17:15]
     * Slightly modified from https://github.com/ocornut/imgui/blob/master/examples/example_win32_directx11/main.cpp#L38
     */
    private static HWND _create_window() {
        /* [fkelava 15/8/25 20:12]
         * Instead of only giving WS.WS_OVERLAPPEDWINDOW and doing Show/UpdateWindow later as the example does,
         * we can just set WS.WS_VISIBLE now and skip those calls.
         */
        string class_name      = "FhModMgrWindowClass";
        string window_name     = "Fahrenheit Mod Manager";
        uint   window_style    = WS.WS_OVERLAPPEDWINDOW | WS.WS_VISIBLE;
        uint   window_style_ex = 0;

        fixed (char* window_name_ptr = window_name)
        fixed (char* class_name_ptr  = class_name) {
            _wndClass.cbSize        = (uint)sizeof(WNDCLASSEXW);
            _wndClass.style         = 0; // we do not specify CS.CS_CLASSDC as example does; this is a bad idea https://devblogs.microsoft.com/oldnewthing/20060602-00/?p=30993
            _wndClass.lpfnWndProc   = &_wnd_proc;
            _wndClass.cbClsExtra    = 0;
            _wndClass.cbWndExtra    = 0;
            _wndClass.hInstance     = Windows.GetModuleHandleW(null);
            _wndClass.hIcon         = HICON.NULL;
            _wndClass.hCursor       = HCURSOR.NULL;
            _wndClass.hbrBackground = HBRUSH.NULL;
            _wndClass.lpszMenuName  = null;
            _wndClass.lpszClassName = class_name_ptr;
            _wndClass.hIconSm       = HICON.NULL;

            fixed (WNDCLASSEXW* wnd_class_ptr = &_wndClass) {
                ushort class_id = Windows.RegisterClassExW(wnd_class_ptr);
            }

            return Windows.CreateWindowExW(
                dwExStyle:    window_style_ex,
                lpClassName:  class_name_ptr,
                lpWindowName: window_name_ptr,
                dwStyle:      window_style,
                X:            Windows.CW_USEDEFAULT, // let Windows infer a viable X, Y, width, height
                Y:            Windows.CW_USEDEFAULT,
                nWidth:       Windows.CW_USEDEFAULT,
                nHeight:      Windows.CW_USEDEFAULT,
                hWndParent:   HWND.NULL,
                hMenu:        HMENU.NULL,
                hInstance:    _wndClass.hInstance,
                lpParam:      null);
        }
    }

    /* [fkelava 15/8/25 17:28]
     * Copied verbatim from https://github.com/ocornut/imgui/blob/master/examples/example_win32_directx11/main.cpp#L96.
     */
    private static void _main_loop() {
        bool should_quit = false;

        while (!should_quit) {
            // Pump message queue
            MSG msg;
            while (Windows.PeekMessageW(&msg, HWND.NULL, 0, 0, PM.PM_REMOVE)) {
                Windows.TranslateMessage(&msg);
                Windows.DispatchMessageW(&msg);

                if (msg.message == WM.WM_QUIT) {
                    should_quit = true;
                }
            }

            if (should_quit) break;

            // Window is minimized, or screen is locked
            if (_swap_chain_occluded && _p_swap_chain->Present(0, DXGI.DXGI_PRESENT_TEST) == DXGI.DXGI_STATUS_OCCLUDED) {
                Windows.Sleep(10);
                continue;
            }
            _swap_chain_occluded = false;

            // Handle window resize (we don't resize directly in the WM_SIZE handler)
            if (_resize_h != 0 && _resize_w != 0) {
                _destroy_render_target();

                _p_swap_chain->ResizeBuffers(0, _resize_w, _resize_h, DXGI_FORMAT.DXGI_FORMAT_UNKNOWN, 0);

                _resize_w = 0;
                _resize_h = 0;
                _create_render_target();
            }

            // Start the Dear ImGui frame
            ImGuiImplD3D11.NewFrame();
            ImGuiImplWin32.NewFrame();
            ImGui.NewFrame();

            // Draw the mod manager UI
            ModManager.UI();

            // Render and present frame
            ImGui.Render();

            fixed (float*                   p_clear_color = _rtv_clear_color)
            fixed (ID3D11RenderTargetView** pp_rtv        = &_p_render_target_view) {
                _p_device_ctx->OMSetRenderTargets(1, pp_rtv, null);
                _p_device_ctx->ClearRenderTargetView(_p_render_target_view, p_clear_color);

                ImGuiImplD3D11.RenderDrawData(ImGui.GetDrawData());
            }

            HRESULT hr = _p_swap_chain->Present(1, 0);
            _swap_chain_occluded = hr == DXGI.DXGI_STATUS_OCCLUDED;
        }

        // ImGui impl cleanup
        ImGuiImplD3D11.Shutdown();
        ImGuiImplWin32.Shutdown();

        // D3D11 cleanup
        _destroy_d3d_device();

        // Win32 cleanup
        Windows.DestroyWindow   (_hWnd);
        Windows.UnregisterClassW(_wndClass.lpszClassName, _wndClass.hInstance);
    }

    private static void Main(string[] args) {
        // Make process DPI aware and obtain main monitor scale
        POINT    nul_point = new POINT(0, 0);
        HMONITOR monitor   = Windows.MonitorFromPoint(nul_point, MONITOR.MONITOR_DEFAULTTOPRIMARY);

        ImGuiImplWin32.EnableDpiAwareness();
        float dpi_scale = ImGuiImplWin32.GetDpiScaleForMonitor(monitor);

        // Win32 init
        _hWnd = _create_window();

        // D3D11 init
        if (!_create_d3d_device()) {
            _destroy_d3d_device();
            Windows.UnregisterClassW(_wndClass.lpszClassName, _wndClass.hInstance);

            return;
        }

        // ImGui setup
        ImGuiContextPtr ctx   = ImGui.CreateContext();
        ImGuiIOPtr      io    = ImGui.GetIO();
        ImGuiStylePtr   style = ImGui.GetStyle();

        io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;
        io.ConfigDpiScaleFonts = true;

        // Bake a fixed style scale. (until we have a solution for dynamic style scaling, changing this requires resetting Style + calling this again)
        style.ScaleAllSizes(dpi_scale);

        ImGui.StyleColorsDark();

        // Platform/Renderer backend setup
        HexaID3D11DevicePtr        hexa_p_device     = new((HexaID3D11Device*)       _p_device);
        HexaID3D11DeviceContextPtr hexa_p_device_ctx = new((HexaID3D11DeviceContext*)_p_device_ctx);

        ImGuiImplWin32.SetCurrentContext(ctx);
        ImGuiImplD3D11.SetCurrentContext(ctx);
        ImGuiImplWin32.Init(_hWnd);
        ImGuiImplD3D11.Init(hexa_p_device, hexa_p_device_ctx);

        _main_loop();
    }
}
