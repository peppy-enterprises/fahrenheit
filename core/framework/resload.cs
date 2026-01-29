// SPDX-License-Identifier: MIT

using Hexa.NET.ImGui;

using TerraFX.Interop.DirectX;

namespace Fahrenheit.Core;

/// <summary>
///     Indicates the type of texture the input to <see cref="IFhResourceLoader"/> should be interpreted as.
/// </summary>
public enum FhTextureType {
    NULL = 0,
    DDS  = 1,
    TGA  = 2,
    JPEG = 3,
    PNG  = 4,
    WIC  = 5
}

/* [fkelava 9/8/25 01:19]
 * A direct reimplementation of TexMetadata (https://github.com/microsoft/DirectXTex/wiki/TexMetadata)
 * to abstract Hexa implementation details from consumers and use more correct TerraFX defs.
 */

/// <summary>
///     Describes the properties of a <see cref="FhTexture"/>, such as its dimensions and format.
/// </summary>
public sealed record FhTextureMetadata(
    nuint                    width,
    nuint                    height,
    nuint                    depth,
    nuint                    arraySize,
    nuint                    mipLevels,
    uint                     miscFlags,
    uint                     miscFlags2,
    DXGI_FORMAT              format,
    D3D11_RESOURCE_DIMENSION dimension);

/// <summary>
///     A handle to an image on disk that can be used in ImGui flows.
/// </summary>
public sealed record FhTexture(
    ImTextureRef      TextureRef,
    FhTextureMetadata Metadata);

/* [fkelava 9/8/25 01:52]
 * The internal contract between Core and RT is abstracted here to allow us to arrange it
 * differently from FhResourceLoader's public API, if need be.
 */
internal interface IFhResourceLoader {
    /// <summary>
    ///     Loads a texture of type <paramref name="file_type"/> from a memory buffer
    ///     of size <paramref name="size"/> at <paramref name="ptr"/> and returns, if
    ///     successful, a <see cref="FhTexture"/>.
    /// </summary>
    internal bool load_texture_from_memory(
                                nint          ptr,
                                nuint         size,
                                FhTextureType file_type,
        [NotNullWhen(true)] out FhTexture?    texture);

    /// <summary>
    ///     Loads a texture of type <paramref name="file_type"/> from a
    ///     file at <paramref name="file_path"/> and returns, if
    ///     successful, a <see cref="FhTexture"/>.
    /// </summary>
    internal bool load_texture_from_disk(
                                string        file_path,
                                FhTextureType file_type,
        [NotNullWhen(true)] out FhTexture?    texture);
}

/// <summary>
///     Allows for loading of resources such as images for use in ImGui code.
/// </summary>
public sealed class FhResourceLoader {
    internal readonly FhRuntimeHandle<IFhResourceLoader> loader = new(); // RT connects here.

    /// <summary>
    ///     Attempts to load a DDS-format image from disk.
    /// </summary>
    /// <param name="file_path">The absolute file path to the image on disk.</param>
    /// <param name="texture">A <see cref="FhTexture"/> that can be used in ImGui flows.</param>
    /// <returns>Whether the operation succeeded and <paramref name="texture"/> can be used.</returns>
    public bool load_dds_from_disk(string file_path, [NotNullWhen(true)] out FhTexture? texture) {
        texture = null;
        return loader.get_impl(out IFhResourceLoader? impl) && impl.load_texture_from_disk(file_path, FhTextureType.DDS, out texture);
    }

    /// <summary>
    ///     Attempts to load a WIC-format image from disk.
    /// </summary>
    /// <param name="file_path">The absolute file path to the image on disk.</param>
    /// <param name="texture">A <see cref="FhTexture"/> that can be used in ImGui flows.</param>
    /// <returns>Whether the operation succeeded and <paramref name="texture"/> can be used.</returns>
    public bool load_wic_from_disk(string file_path, [NotNullWhen(true)] out FhTexture? texture) {
        texture = null;
        return loader.get_impl(out IFhResourceLoader? impl) && impl.load_texture_from_disk(file_path, FhTextureType.WIC, out texture);
    }

    /// <summary>
    ///     Attempts to load a TGA-format image from disk.
    /// </summary>
    /// <param name="file_path">The absolute file path to the image on disk.</param>
    /// <param name="texture">A <see cref="FhTexture"/> that can be used in ImGui flows.</param>
    /// <returns>Whether the operation succeeded and <paramref name="texture"/> can be used.</returns>
    public bool load_tga_from_disk(string file_path, [NotNullWhen(true)] out FhTexture? texture) {
        texture = null;
        return loader.get_impl(out IFhResourceLoader? impl) && impl.load_texture_from_disk(file_path, FhTextureType.TGA, out texture);
    }

    /// <summary>
    ///     Attempts to load a TGA-format image from disk.
    /// </summary>
    /// <param name="file_path">The absolute file path to the image on disk.</param>
    /// <param name="texture">A <see cref="FhTexture"/> that can be used in ImGui flows.</param>
    /// <returns>Whether the operation succeeded and <paramref name="texture"/> can be used.</returns>
    public bool load_jpeg_from_disk(string file_path, [NotNullWhen(true)] out FhTexture? texture) {
        texture = null;
        return loader.get_impl(out IFhResourceLoader? impl) && impl.load_texture_from_disk(file_path, FhTextureType.JPEG, out texture);
    }

    /// <summary>
    ///     Attempts to load a TGA-format image from disk.
    /// </summary>
    /// <param name="file_path">The absolute file path to the image on disk.</param>
    /// <param name="texture">A <see cref="FhTexture"/> that can be used in ImGui flows.</param>
    /// <returns>Whether the operation succeeded and <paramref name="texture"/> can be used.</returns>
    public bool load_png_from_disk(string file_path, [NotNullWhen(true)] out FhTexture? texture) {
        texture = null;
        return loader.get_impl(out IFhResourceLoader? impl) && impl.load_texture_from_disk(file_path, FhTextureType.PNG, out texture);
    }
}
