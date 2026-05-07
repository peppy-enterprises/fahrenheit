// SPDX-License-Identifier: MIT

namespace Fahrenheit.Runtime;

/* [fkelava 11/02/26 04:19]
 * For various parts of Fahrenheit's API- such as overlay display and texture loading to name two- we require certain
 * platform-specific objects or runtime constants. In the case of FF X/X-2 HD on Steam, the 'platform' is Win32+D3D11.
 *
 * This module obtains D3D device pointers and the main window HWND the rest of the runtime requires to implement the API.
 */

/* [fkelava 20/01/26 00:24]
 * This interface is internal and thus meant to be implemented explicitly, not implicitly.
 * https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/interfaces#working-with-internal-interfaces
 * > You must use explicit interface implementation to implement interface members that aren't meant to be public.
 */

/// <summary>
///     Implemented by modules that require raw handles to native windowing/graphics APIs.
/// </summary>
internal unsafe interface IFhNativeGraphicsUser {
    /// <summary>
    ///     Called when a valid <see cref="ID3D11Device"/>, <see cref="ID3D11DeviceContext"/>,
    ///     <see cref="IDXGISwapChain"/> and <see cref="HWND"/> are obtained.
    /// </summary>
    internal void assign_devices(
        ID3D11Device*        ptr_device,         // https://learn.microsoft.com/en-us/windows/win32/api/d3d11/nn-d3d11-id3d11device
        ID3D11DeviceContext* ptr_device_context, // https://learn.microsoft.com/en-us/windows/win32/api/d3d11/nn-d3d11-id3d11devicecontext
        IDXGISwapChain*      ptr_swapchain,      // https://learn.microsoft.com/en-us/windows/win32/api/dxgi/nn-dxgi-idxgiswapchain
        HWND                 hWnd);
}

/// <summary>
///     Intercepts the game's initialization to obtain the appropriate native API handles.
/// </summary>
[FhLoad(FhGameId.FFX | FhGameId.FFX2 | FhGameId.FFX2LM)]
[SupportedOSPlatform("windows")] // To satisfy CA1416 warning about invoking D3D/DXGI API which TerraFX annotates as supported only on Windows.
public unsafe sealed class FhNativeGraphicsModule : FhModule {

    /* [fkelava 25/4/24 17:51]
     * https://github.com/terrafx/terrafx.interop.windows/blob/55590efae0f77f4c8db465a80d18b4f5b679696c/sources/Interop/Windows/DirectX/um/d3d11/DirectX.cs#L25
     */
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate HRESULT DirectX_D3D11CreateDeviceAndSwapChain(
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

    /* [fkelava 14/02/26 20:12]
     * A number of ImGui overlays seem to use a dummy window on which a dummy swapchain, device and context are initialized.
     * (ex. https://github.com/rdbo/DX11-BaseHook/blob/main/src/hooks/hooks.cpp)
     *
     * This is to obtain the address of IDXGISwapChain::Present() for hooking. The dummy window,
     * device, context and swapchain are destroyed after this; the _real_ instances of each
     * are obtained when the game first calls through on its own swapchain. A clever trick!
     *
     * None explain _why_ that is necessary or preferable to other methods. My understanding is
     * that it is required because all of these overlays can attach and detach whenever; they
     * cannot assume they will be present for initial device initialization for the process.
     *
     * But in Fahrenheit, we can, and so we do.
     */

    private readonly FhMethodHandle<DirectX_D3D11CreateDeviceAndSwapChain> _h_d3d_init;
    private readonly HashSet<IFhNativeGraphicsUser>                        _users;

    public FhNativeGraphicsModule() {
        _users      = [];
        _h_d3d_init = new(this, "D3D11.dll", "D3D11CreateDeviceAndSwapChain", h_init_d3d);
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        FhModuleHandle<FhImguiModule>          h_imgui   = new(this);
        FhModuleHandle<FhResourceLoaderModule> h_resload = new(this);

        return h_imgui  .try_get_module(out FhImguiModule?          m_imgui)
            && h_resload.try_get_module(out FhResourceLoaderModule? m_resload)
            && _users.Add(m_imgui)
            && _users.Add(m_resload)
            && _h_d3d_init.hook();
    }

    /// <summary>
    ///     Intercepts the game's D3D11 initialization to retrieve a handle to its
    ///     <see cref="ID3D11Device"/>, <see cref="ID3D11DeviceContext"/>, and <see cref="IDXGISwapChain"/>.
    /// </summary>
    [UnmanagedCallConv(CallConvs = [ typeof(CallConvStdcall) ] )]
    private HRESULT h_init_d3d(
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

        /* [fkelava 23/02/26 16:25]
         * To enable D3D debug layer if your system supports it, pass
         * > Flags | (uint)D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_DEBUG.
         */

        HRESULT hr = _h_d3d_init.orig_fptr(
            pAdapter,
            DriverType,
            Software,
            Flags,
            pFeatureLevels,
            FeatureLevels,
            SDKVersion,
            pSwapChainDesc,
            ppSwapChain,
            ppDevice,
            pFeatureLevel,
            ppImmediateContext);

        /* [fkelava 13/02/26 17:46]
         * Special K has hundreds if not thousands of lines of code dedicated to handling many
         * possible scenarios in which the return values of this method are mangled. For our part,
         * we simply check that all the handles are _seemingly_ valid.
         *
         * See https://github.com/SpecialKO/SpecialK/blob/main/src/render/d3d11/d3d11.cpp#L8183.
         */

        if (hr != S.S_OK || ppSwapChain == null || ppDevice == null || ppImmediateContext == null)
            return hr;

        /* [fkelava 13/02/26 17:46]
         * Generally, the HWND to use seems to be determined by most overlays through
         * EnumWindows(), then comparing GetCurrent{Process|Thread}Id() to GetWindowThreadProcessId().
         *
         * While correct, we can skip that since we only deal with known binaries.
         *
         * See https://github.com/rdbo/DX11-BaseHook/blob/main/src/hooks/hooks.cpp#L52,
         * https://github.com/Kaldaien/UnX/blob/master/UnX/window.cpp#L60
         */

        foreach (IFhNativeGraphicsUser user in _users) {
            user.assign_devices(
                *ppDevice,
                *ppImmediateContext,
                *ppSwapChain,
                FhUtil.get_at<HWND>(FhUtil.select(0x8C9CE8, 0x16641B8, 0x16641B8)));
        }

        return hr;
    }
}
