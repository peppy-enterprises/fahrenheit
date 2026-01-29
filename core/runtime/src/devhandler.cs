// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core.Runtime;

/* [fkelava 20/01/26 00:24]
 * This interface is internal and thus meant to be implemented explicitly, not implicitly.
 * https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/interfaces#working-with-internal-interfaces
 * > You must use explicit interface implementation to implement interface members that aren't meant to be public.
 */

/// <summary>
///     Implemented by modules that require raw handles to D3D devices.
/// </summary>
internal unsafe interface IFhD3DUser {
    /// <summary>
    ///     Called when a valid <see cref="ID3D11Device"/>,
    ///     <see cref="ID3D11DeviceContext"/>, <see cref="IDXGISwapChain"/> are obtained.
    /// </summary>
    internal void assign_devices(
        ID3D11Device*        ptr_device,         // https://learn.microsoft.com/en-us/windows/win32/api/d3d11/nn-d3d11-id3d11device
        ID3D11DeviceContext* ptr_device_context, // https://learn.microsoft.com/en-us/windows/win32/api/d3d11/nn-d3d11-id3d11devicecontext
        IDXGISwapChain*      ptr_swapchain);     // https://learn.microsoft.com/en-us/windows/win32/api/dxgi/nn-dxgi-idxgiswapchain
}

/// <summary>
///     Intercepts the game's D3D initialization to obtain the appropriate device handles.
///     <para/>
///     Do not interface with this module directly.
/// </summary>
[FhLoad(FhGameId.FFX | FhGameId.FFX2 | FhGameId.FFX2LM)]
[SupportedOSPlatform("windows")] // To satisfy CA1416 warning about invoking D3D/DXGI API which TerraFX annotates as supported only on Windows.
public unsafe sealed class FhDeviceHandlerModule : FhModule {

    private readonly FhMethodHandle<DirectX_D3D11CreateDeviceAndSwapChain> _h_d3d_init;
    private readonly HashSet<IFhD3DUser>                                   _users;

    public FhDeviceHandlerModule() {
        _users      = [];
        _h_d3d_init = new(this, "D3D11.dll", "D3D11CreateDeviceAndSwapChain", h_init_d3d);
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        FhModuleHandle<FhImguiModule>          h_imgui   = new(this);
        FhModuleHandle<FhResourceLoaderModule> h_resload = new(this);

        return h_imgui  .try_get_module(out FhImguiModule?          m_imgui)
            && h_resload.try_get_module(out FhResourceLoaderModule? m_resload)
            && _h_d3d_init.hook()
            && _users.Add(m_imgui)
            && _users.Add(m_resload);
    }

    /// <summary>
    ///     Intercepts the game's D3D11 initialization to retrieve a handle to its
    ///     <see cref="ID3D11Device"/>, <see cref="ID3D11DeviceContext"/>, and <see cref="IDXGISwapChain"/>.
    /// </summary>
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

        /* [fkelava 19/11/25 15:08]
         * The game has strange behavior on multi-GPU systems. The method will be invoked several
         * times, usually with garbage. For this reason we ignore a result that doesn't contain
         * all three of a valid device, device context, and swap chain.
         */

        if (hr != S.S_OK || ppSwapChain == null || ppDevice == null || ppImmediateContext == null)
            return hr;

        foreach (IFhD3DUser user in _users) {
            user.assign_devices(
                *ppDevice,
                *ppImmediateContext,
                *ppSwapChain);
        }

        return hr;
    }
}
