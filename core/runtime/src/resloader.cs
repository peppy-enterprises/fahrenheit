// SPDX-License-Identifier: MIT

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
///     Do not interface with this module directly. Instead, call <see cref="FhApi.Resources"/>.
/// </summary>
[FhLoad(FhGameId.FFX | FhGameId.FFX2 | FhGameId.FFX2LM)]
public unsafe sealed class FhResourceLoaderModule : FhModule, IFhResourceLoader, IFhNativeGraphicsUser {
    private ID3D11Device* _p_device; // https://learn.microsoft.com/en-us/windows/win32/api/d3d11/nn-d3d11-id3d11device

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        FhApi.Resources.loader.set_impl(this);
        return true;
    }

    void IFhNativeGraphicsUser.assign_devices(
        ID3D11Device*        ptr_device,
        ID3D11DeviceContext* ptr_device_context,
        IDXGISwapChain*      ptr_swapchain,
        HWND                 hWnd) {
        _p_device = ptr_device;
    }

    /// <summary>
    ///     Creates a <see cref="ID3D11ShaderResourceView"/> from a given texture, then wraps it in a <see cref="FhTexture"/>.
    /// </summary>
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

    /// <summary>
    ///     Attempts to load a texture of type <paramref name="texture_type"/> located in
    ///     a memory buffer of size <paramref name="size"/> pointed to by <paramref name="ptr"/>.
    /// </summary>
    bool IFhResourceLoader.load_texture_from_memory(nint ptr, nuint size, FhTextureType texture_type, [NotNullWhen(true)] out FhTexture? texture) {
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
            _logger.Info($"0x{rc.Value:X}");
            return false;
        }

        bool rv = _create_srv(image, image_metadata, out texture);
        Hexa_Extensions.Release(image);
        return rv;
    }

    /// <summary>
    ///     Attempts to load a texture of type <paramref name="texture_type"/> located at <paramref name="file_path"/> on disk.
    /// </summary>
    bool IFhResourceLoader.load_texture_from_disk(string file_path, FhTextureType texture_type, [NotNullWhen(true)] out FhTexture? texture) {
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
            _logger.Info($"{file_path} -> 0x{rc.Value:X}");
            return false;
        }

        bool rv = _create_srv(image, image_metadata, out texture);
        Hexa_Extensions.Release(image);
        return rv;
    }
}
