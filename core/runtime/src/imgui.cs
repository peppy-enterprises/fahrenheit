// SPDX-License-Identifier: MIT

using static Fahrenheit.Core.Runtime.PInvoke;

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

/// <summary>
///     Provides the ability to use the ImGui GUI toolkit within the game.
///     <para/>
///     Do not interface with this module directly. Instead, implement <see cref="FhModule.render_imgui"/>.
/// </summary>
[FhLoad(FhGameType.FFX | FhGameType.FFX2)]
[SupportedOSPlatform("windows")] // To satisfy CA1416 warning about invoking D3D/DXGI API which TerraFX annotates as supported only on Windows.
public unsafe class FhImguiModule : FhModule {
    // WndProc support
    private          HWND                              _hWnd;
    private          nint                              _ptr_o_WndProc;
    private          nint                              _ptr_h_WndProc;
    private readonly WndProcDelegate                   _h_WndProc;
    private readonly FhMethodHandle<graphicInitialize> _handle_wndproc_init;
    private readonly FhMethodHandle<PInputUpdate>      _handle_input_update;

    private          IDXGISwapChain*                                       _p_swap_chain;         // https://learn.microsoft.com/en-us/windows/win32/api/dxgi/nn-dxgi-idxgiswapchain
    private          ID3D11Device*                                         _p_device;             // https://learn.microsoft.com/en-us/windows/win32/api/d3d11/nn-d3d11-id3d11device
    private          ID3D11DeviceContext*                                  _p_device_ctx;         // https://learn.microsoft.com/en-us/windows/win32/api/d3d11/nn-d3d11-id3d11devicecontext
    private          ID3D11Resource*                                       _p_surface;            // https://learn.microsoft.com/en-us/windows/win32/api/d3d11/nn-d3d11-id3d11texture2d
    private          ID3D11RenderTargetView*                               _p_render_target_view; // https://learn.microsoft.com/en-us/windows/win32/api/d3d11/nn-d3d11-id3d11rendertargetview
    private readonly FhMethodHandle<DirectX_D3D11CreateDeviceAndSwapChain> _handle_d3d11_init;
    private          FhMethodHandle<DXGISwapChain_Present>?                _handle_present;
    private          FhMethodHandle<DXGISwapChain_ResizeBuffers>?          _handle_resize_buffers;

    private bool _present_init_complete; // has h_present finished one-time initialization?
    private bool _dx11_init_complete;    // has h_init_d3d11 finished one-time initialization?
    private bool _present_ready;         // Phyre is not ready to render until the 'FINAL FANTASY X PROJECT' logo i.e. the main loop has run at least once.

    public FhImguiModule() {
        FhMethodLocation location_wndproc_init = new(0x241B80, 0x0529A0);
        FhMethodLocation location_input_update = new(0x225930, 0x6B51E0);

        _handle_wndproc_init = new(this, location_wndproc_init, h_init_wndproc);
        _handle_input_update = new(this, location_input_update, h_input_update);
        _handle_d3d11_init   = new(this, "D3D11.dll", "D3D11CreateDeviceAndSwapChain", h_init_d3d11);
        _h_WndProc           = h_wndproc;
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        return _handle_d3d11_init  .hook()
            && _handle_wndproc_init.hook()
            && _handle_input_update.hook();
    }

    public override void post_update() {
        _present_ready = true;
    }

    private void init_imgui() {
        if (_hWnd     == 0    ||                        // h_init_wndproc hasn't run yet?
            _p_device == null || _p_device_ctx == null) // h_init_d3d11 hasn't run yet?
            return;

        ImGuiContextPtr ctx   = ImGui.CreateContext();
        ImGuiIOPtr      io    = ImGui.GetIO();
        ImGuiStylePtr   style = ImGui.GetStyle();

        // Enable controls
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;

        // Enable features
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        //io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable; //TODO: Figure out why dragging a viewport outside of the game window makes it crash

        io.ConfigDpiScaleFonts     = true;
        io.ConfigDpiScaleViewports = true;

        // When viewports are enabled we tweak WindowRounding/WindowBg so platform windows can look identical to regular ones.
        if ((io.ConfigFlags & ImGuiConfigFlags.ViewportsEnable) != 0) {
            style.WindowRounding = 0f;
            style.Colors[(int)ImGuiCol.WindowBg].W = 1f;
        }

        ImGui.StyleColorsDark();

        HexaID3D11DevicePtr        hexa_p_device     = new((HexaID3D11Device*)       _p_device);
        HexaID3D11DeviceContextPtr hexa_p_device_ctx = new((HexaID3D11DeviceContext*)_p_device_ctx);

        ImGuiImplWin32.SetCurrentContext(ctx);
        ImGuiImplD3D11.SetCurrentContext(ctx);
        ImGuiImplWin32.Init(_hWnd);
        ImGuiImplD3D11.Init(hexa_p_device, hexa_p_device_ctx);

        FhApi.ImGuiHelper.init();
    }

    /// <summary>
    ///     Replaces the game's window procedure with <see cref="h_wndproc"/>.
    /// </summary>
    private nint h_init_wndproc() {
        nint result = _handle_wndproc_init.orig_fptr();
        switch (FhGlobal.game_type) {
            case FhGameType.FFX:
                _hWnd = (HWND)FhUtil.get_at<nint>(0x8C9CE8);
                break;
            case FhGameType.FFX2:
                _hWnd = (HWND)FhUtil.get_at<nint>(0x16641B8);
                break;
        }
        _ptr_h_WndProc = Marshal.GetFunctionPointerForDelegate(_h_WndProc);
        _ptr_o_WndProc = Windows.GetWindowLongPtrW(_hWnd, GWLP.GWLP_WNDPROC);
        nint _         = Windows.SetWindowLongPtrW(_hWnd, GWLP.GWLP_WNDPROC, _ptr_h_WndProc);

        init_imgui();
        return result;
    }

    /// <summary>
    ///     Intercepts the game's D3D11 initialization to retrieve a handle to its <see cref="ID3D11Device"/>,
    ///     <see cref="ID3D11DeviceContext"/>, and <see cref="IDXGISwapChain"/>.
    /// </summary>
    private HRESULT h_init_d3d11(
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
        ID3D11DeviceContext** ppImmediateContext) {

        HRESULT result = _handle_d3d11_init.orig_fptr
            (pAdapter, DriverType, Software, Flags, pFeatureLevels, FeatureLevels, SDKVersion, pSwapChainDesc, ppSwapChain, ppDevice, pFeatureLevel, ppImmediateContext);

        if (result != 0 || _dx11_init_complete) return result; // S_FALSE is a possible return

        _p_swap_chain = *ppSwapChain;
        _p_device     = *ppDevice;
        _p_device_ctx = *ppImmediateContext;

        _handle_present        = new(this, new nint(_p_swap_chain->lpVtbl[8]),  h_present);
        _handle_resize_buffers = new(this, new nint(_p_swap_chain->lpVtbl[13]), h_resize_buffers);

        _handle_present       .hook();
        _handle_resize_buffers.hook();

        init_imgui();

        _dx11_init_complete = true;
        return result;
    }

    /// <summary>
    ///     Allows ImGui to intercept window messages sent to the game, such as <see cref="WM.WM_KEYDOWN"/>,
    ///     enabling mouse and keyboard input to be directed to it.
    /// </summary>
    private nint h_wndproc(
        HWND   hWnd,
        uint   msg,
        WPARAM wParam,
        LPARAM lParam) {
        return ImGuiImplWin32.WndProcHandler(hWnd, msg, wParam, lParam) == 1
             ? 1
             : Windows.CallWindowProcW((delegate* unmanaged<HWND, uint, WPARAM, LPARAM, LRESULT>)_ptr_o_WndProc, hWnd, msg, wParam, lParam);
    }

    /// <summary>
    ///     Allows interception of raw input from the game, redirecting it to ImGui if appropriate.
    /// </summary>
    private int h_input_update() {
        if (_hWnd         == 0    // h_init_wndproc hasn't run yet?
         || _p_device     == null // h_init_d3d11 hasn't run yet?
         || _p_device_ctx == null)
            return _handle_input_update.orig_fptr();

        ImGuiIOPtr io = ImGui.GetIO();
        return io.WantCaptureKeyboard
            ? 0
            : _handle_input_update.orig_fptr();
    }

    /// <summary>
    ///     Intercepts attempts to resize the game window to allow ImGui to continue drawing.
    /// </summary>
    private nint h_resize_buffers(IDXGISwapChain* pSwapChain, uint BufferCount, uint Width, uint Height, DXGI_FORMAT NewFormat, uint SwapChainFlags) {
        if (_p_device_ctx == null)
            return _handle_resize_buffers!.orig_fptr(pSwapChain, BufferCount, Width, Height, NewFormat, SwapChainFlags);

        if (_p_render_target_view != null) {
            _p_device_ctx        ->OMSetRenderTargets(0, null, null);
            _p_render_target_view->Release();
        }

        _present_init_complete = false; // forces regeneration of ImGui surfaces/RTV in next `h_present`
        return _handle_resize_buffers!.orig_fptr(pSwapChain, BufferCount, Width, Height, NewFormat, SwapChainFlags);
    }

    /// <summary>
    ///     Overrides the game's <see cref="IDXGISwapChain.Present(uint, uint)"/> call to display mods' user interfaces.
    /// </summary>
    private nint h_present(IDXGISwapChain* pSwapChain, uint SyncInterval, uint Flags) {
        if (_hWnd         == 0    // > h_init_wndproc hasn't run yet?
         || _p_swap_chain == null // |
         || _p_device     == null // |
         || _p_device_ctx == null // | > h_init_d3d11 hasn't run yet?
         || !_present_ready)      // > game main loop hasn't run yet?
            return _handle_present!.orig_fptr(pSwapChain, SyncInterval, Flags);

        if (!_present_init_complete) {
            fixed (ID3D11Resource**         ppSurface          = &_p_surface)
            fixed (ID3D11RenderTargetView** ppRenderTargetView = &_p_render_target_view) {
                _p_swap_chain->GetBuffer(0, Windows.__uuidof<ID3D11Texture2D>(), (void**)ppSurface);
                _p_device    ->CreateRenderTargetView(_p_surface, null, ppRenderTargetView);
                _p_surface   ->Release();
            }

            _present_init_complete = true;
        }

        ImGuiImplD3D11.NewFrame();
        ImGuiImplWin32.NewFrame();

        ImGui.NewFrame();

        foreach (FhModuleContext module_ctx in FhApi.Mods.get_modules()) {
            module_ctx.Module.render_imgui();
        }

        ImGui.Render();

        fixed (ID3D11RenderTargetView** ppRenderTargetView = &_p_render_target_view) {
            _p_device_ctx->OMSetRenderTargets(1, ppRenderTargetView, null);
        }

        ImGuiImplD3D11.RenderDrawData(ImGui.GetDrawData());

        if ((ImGui.GetIO().ConfigFlags & ImGuiConfigFlags.ViewportsEnable) != 0) {
            ImGui.UpdatePlatformWindows();
        }

        return _handle_present!.orig_fptr(pSwapChain, SyncInterval, Flags);
    }
}
