using Hexa.NET.ImGui;
using System.Reflection.PortableExecutable;
using System.Runtime.Versioning;
using System.Text;
using TerraFX.Interop.DirectX;         // D3D/DXGI bindings
using TerraFX.Interop.Windows;         // Win32 bindings
namespace Fahrenheit.Core;

[SupportedOSPlatform("windows")] // To satisfy CA1416 warning about invoking D3D/DXGI API which TerraFX annotates as supported only on Windows.
public static unsafe class FhTextureLoader {
    [Flags]
    enum dds_pixelformat_flags {
        DDPF_ALPHAPIXELS = 0x00001, // Texture contains alpha data; dwRGBAlphaBitMask contains valid data.
        DDPF_ALPHA       = 0x00002, // Used in some older DDS files for alpha channel only uncompressed data (dwRGBBitCount contains the alpha channel bitcount; dwABitMask contains valid data)
        DDPF_FOURCC      = 0x00004, // Texture contains compressed RGB data; dwFourCC contains valid data.
        DDPF_RGB         = 0x00040, // Texture contains uncompressed RGB data; dwRGBBitCount and the RGB masks(dwRBitMask, dwGBitMask, dwBBitMask) contain valid data.
        DDPF_YUV         = 0x00200, // Used in some older DDS files for YUV uncompressed data(dwRGBBitCount contains the YUV bit count; dwRBitMask contains the Y mask, dwGBitMask contains the U mask, dwBBitMask contains the V mask)
        DDPF_LUMINANCE   = 0x20000, // Used in some older DDS files for single channel color uncompressed data(dwRGBBitCount contains the luminance channel bit count; dwRBitMask contains the channel mask). Can be combined with DDPF_ALPHAPIXELS for a two channel DDS file.
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct DDS_PIXELFORMAT {
        public       uint                  dwSize;
        public       dds_pixelformat_flags dwFlags;
        public fixed byte                  dwFourCC[4];
        public       uint                  dwRGBBitCount;
        public       uint                  dwRBitMask;
        public       uint                  dwGBitMask;
        public       uint                  dwBBitMask;
        public       uint                  dwABitMask;
    };
    [Flags]
    enum dds_header_flags {
        DDSD_CAPS        = 0x000001, //Required in every .dds file.
        DDSD_HEIGHT      = 0x000002, //Required in every .dds file.
        DDSD_WIDTH       = 0x000004, //Required in every .dds file.
        DDSD_PITCH       = 0x000008, //Required when pitch is provided for an uncompressed texture.
        DDSD_PIXELFORMAT = 0x001000, //Required in every .dds file.
        DDSD_MIPMAPCOUNT = 0x020000, //Required in a mipmapped texture.
        DDSD_LINEARSIZE  = 0x080000, //Required when pitch is provided for a compressed texture.
        DDSD_DEPTH       = 0x800000, //Required in a depth texture.
    
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct DDS_HEADER {
        public       uint             dwSize;
        public       dds_header_flags dwFlags;
        public       uint             dwHeight;
        public       uint             dwWidth;
        public       uint             dwPitchOrLinearSize;
        public       uint             dwDepth;
        public       uint             dwMipMapCount;
        public fixed uint             dwReserved1[11];
        public       DDS_PIXELFORMAT  ddspf;
        public       uint             dwCaps;
        public       uint             dwCaps2;
        public       uint             dwCaps3;
        public       uint             dwCaps4;
        public       uint             dwReserved2;
    }
    enum D3D10_RESOURCE_DIMENSION : uint {
        D3D10_RESOURCE_DIMENSION_UNKNOWN   = 0,
        D3D10_RESOURCE_DIMENSION_BUFFER    = 1,
        D3D10_RESOURCE_DIMENSION_TEXTURE1D = 2,
        D3D10_RESOURCE_DIMENSION_TEXTURE2D = 3,
        D3D10_RESOURCE_DIMENSION_TEXTURE3D = 4,
    };
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct DDS_HEADER_DXT10 {
        public DXGI_FORMAT              dxgiFormat;
        public D3D10_RESOURCE_DIMENSION resourceDimension;
        public uint                     miscFlag;
        public uint                     arraySize;
        public uint                     miscFlags2;
    }
    public enum D3D11_SRV_DIMENSION {
        D3D11_SRV_DIMENSION_UNKNOWN           =  0,
        D3D11_SRV_DIMENSION_BUFFER            =  1,
        D3D11_SRV_DIMENSION_TEXTURE1D         =  2,
        D3D11_SRV_DIMENSION_TEXTURE1DARRAY    =  3,
        D3D11_SRV_DIMENSION_TEXTURE2D         =  4,
        D3D11_SRV_DIMENSION_TEXTURE2DARRAY    =  5,
        D3D11_SRV_DIMENSION_TEXTURE2DMS       =  6,
        D3D11_SRV_DIMENSION_TEXTURE2DMSARRAY  =  7,
        D3D11_SRV_DIMENSION_TEXTURE3D         =  8,
        D3D11_SRV_DIMENSION_TEXTURECUBE       =  9,
        D3D11_SRV_DIMENSION_TEXTURECUBEARRAY  = 10,
        D3D11_SRV_DIMENSION_BUFFEREX          = 11,
    };
    public struct D3D11_TEX2D_SRV {
        public uint MostDetailedMip;
        public uint MipLevels;
    };
    
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

    private static readonly Dictionary<string, DXGI_FORMAT> fourCC_to_format = new(){
        {"DXT1", DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM},
        {"DXT3", DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM},
        {"DXT5", DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM},
        {"BC4U", DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM},
        {"BC4S", DXGI_FORMAT.DXGI_FORMAT_BC4_SNORM},
        {"ATI2", DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM},
        {"BC5S", DXGI_FORMAT.DXGI_FORMAT_BC5_SNORM},
        {"RGBG", DXGI_FORMAT.DXGI_FORMAT_R8G8_B8G8_UNORM},
        {"GRGB", DXGI_FORMAT.DXGI_FORMAT_G8R8_G8B8_UNORM},
        {"36",   DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_UNORM},
        {"110",  DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_SNORM},
        {"111",  DXGI_FORMAT.DXGI_FORMAT_R16_FLOAT},
        {"112",  DXGI_FORMAT.DXGI_FORMAT_R16G16_FLOAT},
        {"113",  DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_FLOAT},
        {"114",  DXGI_FORMAT.DXGI_FORMAT_R32_FLOAT},
        {"115",  DXGI_FORMAT.DXGI_FORMAT_R32G32_FLOAT},
        {"116",  DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT},
    };

    private static uint format_to_bytes_per_block(DXGI_FORMAT format) {
        switch (format) {
            case DXGI_FORMAT.DXGI_FORMAT_BC1_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB:
            case DXGI_FORMAT.DXGI_FORMAT_BC4_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC4_SNORM:
                return 8;
            case DXGI_FORMAT.DXGI_FORMAT_BC2_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM_SRGB:
            case DXGI_FORMAT.DXGI_FORMAT_BC3_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM_SRGB:
            case DXGI_FORMAT.DXGI_FORMAT_BC5_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC5_SNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC6H_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC6H_UF16:
            case DXGI_FORMAT.DXGI_FORMAT_BC6H_SF16:
            case DXGI_FORMAT.DXGI_FORMAT_BC7_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB:
                return 16;
        }
        return 0;
    }

    public unsafe delegate HRESULT CreateTexture2D(D3D11_TEXTURE2D_DESC* pDesc, D3D11_SUBRESOURCE_DATA* pInitialData, ID3D11Texture2D** ppTexture2D);
    public unsafe delegate HRESULT CreateShaderResourceView(ID3D11Resource* pResource, D3D11_SHADER_RESOURCE_VIEW_DESC* pDesc, ID3D11ShaderResourceView** ppSRView);

    public static CreateTexture2D?          createTexture2D;
    public static CreateShaderResourceView? createShaderResourceView;

    public record FhImTexture(ImTextureRef imTextureRef, uint width, uint height);

    public static FhImTexture? loadTexture(PTexture2DBase* pTexture) {

        ID3D11ShaderResourceView* pSrv = (ID3D11ShaderResourceView*)0;
        HRESULT resultCode = createShaderResourceView(pTexture->buffer, null, &pSrv);
        if (resultCode.FAILED) {
            // Error
            return null;
        }
        return new FhImTexture(new ImTextureRef(null, (nint)pSrv), pTexture->width, pTexture->height);
    }
    public static FhImTexture? loadDDS(FileInfo file, DXGI_FORMAT format = DXGI_FORMAT.DXGI_FORMAT_UNKNOWN) {
        if (createTexture2D == null || createShaderResourceView == null) return null;
        byte[] image_data;
        try {
            image_data = File.ReadAllBytes(file.FullName);
        }
        catch {
            return null;
        }
        if (Encoding.UTF8.GetString(image_data, 0, 4) != "DDS ") return null;
        // DXT5 = DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM
        // RGBA8 = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM (reversed for some reason?)

        ID3D11Texture2D* pTexture;
        ID3D11ShaderResourceView* pSrv;
        fixed (byte* image_pointer = image_data) {
            DDS_HEADER header = *(DDS_HEADER*)(image_pointer+4);
            int header_size = 0x80;
            uint pitch = 0;
            if (header.ddspf.dwFlags.HasFlag(dds_pixelformat_flags.DDPF_FOURCC)) {
                string fourCC = Encoding.UTF8.GetString(header.ddspf.dwFourCC, 4);
                if (fourCC == "DX10") {
                    DDS_HEADER_DXT10 dxt10_header = *(DDS_HEADER_DXT10*)(image_pointer + 0x84);
                    format = dxt10_header.dxgiFormat;
                    header_size += 0x14;
                }
                else if (format == DXGI_FORMAT.DXGI_FORMAT_UNKNOWN) {
                    fourCC_to_format.TryGetValue(fourCC, out format);
                }
                uint bytes_per_block = format_to_bytes_per_block(format);
                if (header.dwWidth > 0 && bytes_per_block != 0) {
                    pitch = Math.Max(1, (header.dwWidth + 3) / 4) * bytes_per_block;
                }

            }
            else if (header.ddspf.dwFlags.HasFlag(dds_pixelformat_flags.DDPF_RGB)) {
                if (header.dwFlags.HasFlag(dds_header_flags.DDSD_PITCH)) {
                    pitch = header.dwPitchOrLinearSize;
                } else {
                    pitch = (header.dwWidth * header.ddspf.dwRGBBitCount + 7) / 8;
                }
                if (format == DXGI_FORMAT.DXGI_FORMAT_UNKNOWN) {
                    // Read color bitmasks

                }
            }


            D3D11_TEXTURE2D_DESC desc = new() {
                Width = header.dwWidth,
                Height = header.dwHeight,
                MipLevels = 1,
                ArraySize = 1,
                Format = format,
                Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT,
                BindFlags = (uint)D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE,
                CPUAccessFlags = 0,
            };
            desc.SampleDesc.Count = 1;

            D3D11_SUBRESOURCE_DATA subResource = new() {
                pSysMem = (void*)((nint)image_pointer + header_size),
                SysMemPitch = pitch != 0 ? pitch : desc.Width * 4, //Or Math.Max(1, ((desc.Width+3)/4)) * 16?
                SysMemSlicePitch = 0,
            };
            HRESULT resultCode = createTexture2D(&desc, &subResource, &pTexture);
            if (resultCode.FAILED) {
                // Error
                return null;
            }
            resultCode = createShaderResourceView((ID3D11Resource*)pTexture, null, &pSrv);
            if (resultCode.FAILED) {
                // Error
                return null;
            }
            return new FhImTexture(new ImTextureRef(null, (nint)pSrv), header.dwWidth, header.dwHeight);
        }
    }
}
