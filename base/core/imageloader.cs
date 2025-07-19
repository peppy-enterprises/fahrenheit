using Hexa.NET.ImGui;
using Hexa.NET.DirectXTex;
using HexaGen.Runtime;
using System.Runtime.Versioning;
using TerraFX.Interop.DirectX;         // D3D/DXGI bindings
using TerraFX.Interop.Windows;         // Win32 bindings
using ID3D11Resource = Hexa.NET.DirectXTex.ID3D11Resource;
using ID3D11ShaderResourceView = Hexa.NET.DirectXTex.ID3D11ShaderResourceView;
using Image = Hexa.NET.DirectXTex.Image;
namespace Fahrenheit.Core;

[SupportedOSPlatform("windows")] // To satisfy CA1416 warning about invoking D3D/DXGI API which TerraFX annotates as supported only on Windows.
public static unsafe class FhTextureLoader {
    [StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x74)]
    public struct PTexture2DBase {
        [FieldOffset(0x00)] public nint            unknown1;
        [FieldOffset(0x0c)] public nint            unknown2;
        [FieldOffset(0x10)] public int             mipCount;
        [FieldOffset(0x14)] public int             flags;
        [FieldOffset(0x1c)] public uint            width;
        [FieldOffset(0x20)] public uint            height;
        [FieldOffset(0x24)] public int             num_buffers; //??
        [FieldOffset(0x28)] public ID3D11Resource* buffer;
        [FieldOffset(0x70)] public int             isBound;
    }

    // TerraFX
    public unsafe delegate HRESULT CreateShaderResourceViewRaw(ID3D11Resource* pResource, D3D11_SHADER_RESOURCE_VIEW_DESC* pDesc, ID3D11ShaderResourceView** ppSRView);

    // DirectXTex
    public unsafe delegate HResult CreateTextureEx2(ScratchImage img, uint usage, uint bindFlags, uint cpuAccessFlags, uint miscFlags, CreateTexFlags createFlags, ID3D11Resource** ppResource);
    public unsafe delegate HResult CreateShaderResourceView(Image* srcImages, nuint nimages, TexMetadata* metadata, ID3D11ShaderResourceView** ppSRV);

    public static CreateShaderResourceViewRaw? createShaderResourceViewRaw;
    public static CreateShaderResourceView?    createShaderResourceView;
    public static CreateTextureEx2?            createTextureEx2;

    public record FhImTexture(ImTextureRef imTextureRef, uint width, uint height);

    public static FhImTexture? loadTexture(PTexture2DBase* pTexture) {
        if (createShaderResourceViewRaw == null) return null;

        ID3D11ShaderResourceView* pSrv;
        HRESULT resultCode = createShaderResourceViewRaw(pTexture->buffer, null, &pSrv);
        if (resultCode.FAILED) {
            // Error
            return null;
        }
        return new FhImTexture(new ImTextureRef(null, (nint)pSrv), pTexture->width, pTexture->height);
    }

    public static FhImTexture? createSRV(ScratchImage image, TexMetadata metadata) {
        if (createShaderResourceView == null) return null;

        ID3D11ShaderResourceView* pSrv;
        HResult resultCode = createShaderResourceView(image.GetImages(), image.GetImageCount(), &metadata, &pSrv);
        if (resultCode.IsFailure) {
            // Error
            return null;
        }
        return new FhImTexture(new ImTextureRef(null, (nint)pSrv), (uint)metadata.Width, (uint)metadata.Height);
    }

    public static FhImTexture? loadWIC(FileInfo file) {
        if (createShaderResourceView == null) return null;

        ScratchImage image = DirectXTex.CreateScratchImage();
        TexMetadata metadata = default;
        HResult resultCode = DirectXTex.LoadFromWICFile(file.FullName, WICFlags.None, &metadata, &image, null);
        if (resultCode.IsFailure) {
            // Error
            return null;
        }
        FhImTexture? fhImTexture = createSRV(image, metadata);
        image.Release();
        return fhImTexture;

    }

    public static FhImTexture? loadDDS(FileInfo file) {
        if (createShaderResourceView == null) return null;

        ScratchImage image = DirectXTex.CreateScratchImage();
        TexMetadata metadata = default;
        HResult resultCode = DirectXTex.LoadFromDDSFile(file.FullName, DDSFlags.None, &metadata, &image);
        if (resultCode.IsFailure) {
            // Error
            return null;
        }
        FhImTexture? fhImTexture = createSRV(image, metadata);
        image.Release();
        return fhImTexture;
    }
}
