/* [fkelava 5/7/25 14:16]
 * Hexa bundles some definitions for D3D11 structures that we need to use when interfacing
 * with its API. They are defined this way because we prefer the TerraFX definitions in all other cases.
 */
using DirectXTex        = Hexa.NET.DirectXTex.DirectXTex;
using Hexa_DDSFlags     = Hexa.NET.DirectXTex.DDSFlags;
using Hexa_Extensions   = Hexa.NET.DirectXTex.Extensions;
using Hexa_HRESULT      = HexaGen.Runtime.HResult;
using Hexa_ID3D11Device = Hexa.NET.DirectXTex.ID3D11Device;
using Hexa_ID3D11SRV    = Hexa.NET.DirectXTex.ID3D11ShaderResourceView;
using Hexa_ScratchImage = Hexa.NET.DirectXTex.ScratchImage;
using Hexa_TexMetadata  = Hexa.NET.DirectXTex.TexMetadata;
using Hexa_TGAFlags     = Hexa.NET.DirectXTex.TGAFlags;
using Hexa_WICFlags     = Hexa.NET.DirectXTex.WICFlags;

namespace Fahrenheit.Core.Runtime;

/// <summary>
///     Loads textures and other resources at runtime.
///     <para/>
///     Do not interface with this module directly. Instead, call <see cref="FhApi.ResourceLoader"/>.
/// </summary>
[FhLoad(FhGameType.FFX)]
public unsafe class FhResourceLoaderModule : FhModule, IFhResourceLoader {
    private          ID3D11Device*                                         _p_device; // https://learn.microsoft.com/en-us/windows/win32/api/d3d11/nn-d3d11-id3d11device
    private readonly FhMethodHandle<DirectX_D3D11CreateDeviceAndSwapChain> _handle_d3d11_init;

    public FhResourceLoaderModule() {
        _handle_d3d11_init = new(this, "D3D11.dll", h_init_d3d11, fn_name: "D3D11CreateDeviceAndSwapChain");
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        FhApi.ResourceLoader.loader.set_impl(this);
        return _handle_d3d11_init.hook();
    }

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

        if (result != 0) return result; // S_FALSE is a possible return

        _p_device = *ppDevice;
        return result;
    }

    private bool _create_srv(Hexa_ScratchImage hexa_image, Hexa_TexMetadata hexa_metadata, [NotNullWhen(true)] out FhTexture? texture) {
        texture = null;

        Hexa_ID3D11SRV* srv;
        Hexa_HRESULT    rc = DirectXTex.CreateShaderResourceView(
            (Hexa_ID3D11Device*)_p_device,
            Hexa_Extensions.GetImages    (hexa_image),
            Hexa_Extensions.GetImageCount(hexa_image),
            &hexa_metadata,
            &srv);

        if (rc.IsFailure) {
            _logger.Info($"0x{rc:X}");
            return false;
        }

        FhTextureMetadata image_metadata = new(
            hexa_metadata.Width,
            hexa_metadata.Height,
            hexa_metadata.Depth,
            hexa_metadata.ArraySize,
            hexa_metadata.MipLevels,
            hexa_metadata.MiscFlags,
            hexa_metadata.MiscFlags2,
            (DXGI_FORMAT)             hexa_metadata.Format,
            (D3D11_RESOURCE_DIMENSION)hexa_metadata.Dimension);
        ImTextureRef imgui_ref = new ImTextureRef(null, srv);

        texture = new(imgui_ref, image_metadata);
        return true;
    }

    public bool load_texture_from_memory(nint ptr, nuint size, FhTextureType texture_type, [NotNullWhen(true)] out FhTexture? texture) {
        texture = null;
        if (_p_device == null) {
            _logger.Info($"device not ready");
            return false;
        }

        Hexa_TexMetadata  image_metadata = default;
        Hexa_ScratchImage image = DirectXTex.CreateScratchImage();
        Hexa_HRESULT      rc    = texture_type switch {
            FhTextureType.DDS  => DirectXTex.LoadFromDDSMemory (ptr.ToPointer(), size, Hexa_DDSFlags.None, &image_metadata, &image),
            FhTextureType.TGA  => DirectXTex.LoadFromTGAMemory (ptr.ToPointer(), size, Hexa_TGAFlags.None, &image_metadata, &image),
            FhTextureType.JPEG => -1,
            FhTextureType.PNG  => -1,
            FhTextureType.WIC  => DirectXTex.LoadFromWICMemory (ptr.ToPointer(), size, Hexa_WICFlags.None, &image_metadata, &image, null),
            _                  => -1
        };

        if (rc.IsFailure) {
            _logger.Info($"0x{rc:X}");
            return false;
        }

        bool rv = _create_srv(image, image_metadata, out texture);
        Hexa_Extensions.Release(image);
        return rv;
    }

    public bool load_texture_from_disk(string file_path, FhTextureType texture_type, [NotNullWhen(true)] out FhTexture? texture) {
        texture = null;
        if (_p_device == null) {
            _logger.Info($"{file_path} -> device not ready");
            return false;
        }

        Hexa_TexMetadata  image_metadata = default;
        Hexa_ScratchImage image = DirectXTex.CreateScratchImage();
        Hexa_HRESULT      rc    = texture_type switch {
            FhTextureType.DDS  => DirectXTex.LoadFromDDSFile (file_path, Hexa_DDSFlags.None, &image_metadata, &image),
            FhTextureType.TGA  => DirectXTex.LoadFromTGAFile (file_path, Hexa_TGAFlags.None, &image_metadata, &image),
            FhTextureType.JPEG => DirectXTex.LoadFromJPEGFile(file_path,                     &image_metadata, &image),
            FhTextureType.PNG  => DirectXTex.LoadFromPNGFile (file_path,                     &image_metadata, &image),
            FhTextureType.WIC  => DirectXTex.LoadFromWICFile (file_path, Hexa_WICFlags.None, &image_metadata, &image, null),
            _                  => -1
        };

        if (rc.IsFailure) {
            _logger.Info($"{file_path} -> 0x{rc:X}");
            return false;
        }

        bool rv = _create_srv(image, image_metadata, out texture);
        Hexa_Extensions.Release(image);
        return rv;
    }
}
