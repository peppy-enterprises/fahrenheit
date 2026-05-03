// SPDX-License-Identifier: MIT

namespace Fahrenheit.Runtime;

/// <summary>
///     Loads textures and other resources at runtime.
///     <para/>
///     In your module, call <see cref="FhApi.Resources"/>.
/// </summary>
[FhLoad(FhGameId.FFX | FhGameId.FFX2 | FhGameId.FFX2LM)]
[SupportedOSPlatform("windows")] // To satisfy CA1416 warning about invoking D3D/DXGI API which TerraFX annotates as supported only on Windows.
public unsafe sealed class FhResourceLoaderModule : FhModule, IFhResourceLoader, IFhNativeGraphicsUser {
    private ID3D11Device* _p_device; // https://learn.microsoft.com/en-us/windows/win32/api/d3d11/nn-d3d11-id3d11device

    private          FhPhyreLoaderModule? _plm;
    private readonly HashSet<FhTexture>   _release_queue;
    private readonly Lock                 _release_lock;

    public FhResourceLoaderModule() {
        _release_queue = [];
        _release_lock    = new Lock();
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        FhApi.Resources.loader.set_impl(this);
        FhModuleHandle<FhPhyreLoaderModule> plm_handle = new FhModuleHandle<FhPhyreLoaderModule>(this);

        return plm_handle.try_get_module(out _plm);
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
    private bool _helper_create_srv(Hexa_ScratchImage hexa_image, Hexa_TexMetadata hexa_metadata, [NotNullWhen(true)] out FhTexture? texture) {
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

        bool rv = _helper_create_srv(image, image_metadata, out texture);
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

        bool rv = _helper_create_srv(image, image_metadata, out texture);
        Hexa_Extensions.Release(image);
        return rv;
    }

    /* [fkelava 01/05/26 18:47]
     * These functions deal with game textures and call through the Phyre asset load system.
     * They have a special rule that the 'file path' parameter has to be in the game's canonical form.
     */

    /// <summary>
    ///     Attempts to load a 2D Phyre game texture at <paramref name="file_path"/> in the VBF.
    /// </summary>
    bool IFhResourceLoader.load_game_texture_2d(string file_path, [NotNullWhen(true)] out FhTexture? texture) {
        texture = null;
        if (_p_device == null) {
            _logger.Info($"{file_path} -> device not ready");
            return false;
        }

        using FhPClusterScope cluster_scope = _plm!.cluster_load(file_path);

        if (!cluster_scope.enter(out PCluster* ptr_cluster)) {
            _logger.Info($"{file_path} -> cluster load failed");
            return false;
        }

        FhPDoubleListIterator<PInstanceList> iter_instances = new(&ptr_cluster->_0x1C_instance_lists);

        while (iter_instances.next(out PInstanceList* ptr_instance_list)) {
            if (Marshal.PtrToStringAnsi(ptr_instance_list->_0x08_free_list._0x10_name) != nameof(PTexture2D))
                continue;

            PFilteredInstanceList          <PTexture2D> texture_filter = new(ptr_instance_list);
            FhPFilteredInstanceListIterator<PTexture2D> texture_iter   = new(texture_filter);

            // We don't know (or even care) what happens in the case that the instance list returns multiple texture objects.
            if (!texture_iter.next(out PTexture2D* ptr_texture)) {
                _logger.Error($"{file_path} - no texture object found in filtered instance list");
                return false;
            }

            ID3D11ShaderResourceView* srv;
            if (_p_device->CreateShaderResourceView(ptr_texture->base_PTexture2DD3D11.ptr_d3d_resource, null, &srv) != S.S_OK) {
                _logger.Error($"{file_path} - SRV instantiation failed");
                return false;
            }

            // TODO: Can we obtain the rest of the image metadata?
            FhTextureMetadata image_metadata = new(
                ptr_texture->base_PTexture2DD3D11.base_PTexture2DBase.m_width,
                ptr_texture->base_PTexture2DD3D11.base_PTexture2DBase.m_height,
                0,
                0,
                ptr_texture->base_PTexture2DD3D11.base_PTexture2DBase.base_PTextureCommonBase.m_maxMipLevel,
                0,
                0,
                DXGI_FORMAT             .DXGI_FORMAT_UNKNOWN,
                D3D11_RESOURCE_DIMENSION.D3D11_RESOURCE_DIMENSION_TEXTURE2D);
            ImTextureRef imgui_ref = new ImTextureRef(null, srv);

            texture = new(imgui_ref, image_metadata);
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Attempts to load a 3D Phyre game texture at <paramref name="file_path"/> in the VBF.
    /// </summary>
    bool IFhResourceLoader.load_game_texture_3d(string file_path, [NotNullWhen(true)] out FhTexture? texture) {
        texture = null;
        if (_p_device == null) {
            _logger.Info($"{file_path} -> device not ready");
            return false;
        }

        using FhPClusterScope cluster_scope = _plm!.cluster_load(file_path);

        if (!cluster_scope.enter(out PCluster* ptr_cluster)) {
            _logger.Info($"{file_path} -> cluster load failed");
            return false;
        }

        FhPDoubleListIterator<PInstanceList> iter_instances = new(&ptr_cluster->_0x1C_instance_lists);

        while (iter_instances.next(out PInstanceList* ptr_instance_list)) {
            if (Marshal.PtrToStringAnsi(ptr_instance_list->_0x08_free_list._0x10_name) != nameof(PTexture3D))
                continue;

            PFilteredInstanceList          <PTexture3D> texture_filter = new(ptr_instance_list);
            FhPFilteredInstanceListIterator<PTexture3D> texture_iter   = new(texture_filter);

            // We don't know (or even care) what happens in the case that the instance list returns multiple texture objects.
            if (!texture_iter.next(out PTexture3D* ptr_texture)) {
                _logger.Error($"{file_path} - no texture object found in filtered instance list");
                return false;
            }

            // TODO: impl after reversing PTexture3D
        }

        return false;
    }

    /// <summary>
    ///     Attempts to load a cubemap Phyre game texture at <paramref name="file_path"/> in the VBF.
    /// </summary>
    bool IFhResourceLoader.load_game_texture_cubemap(string file_path, [NotNullWhen(true)] out FhTexture? texture) {
        texture = null;
        if (_p_device == null) {
            _logger.Info($"{file_path} -> device not ready");
            return false;
        }

        using FhPClusterScope cluster_scope = _plm!.cluster_load(file_path);

        if (!cluster_scope.enter(out PCluster* ptr_cluster)) {
            _logger.Info($"{file_path} -> cluster load failed");
            return false;
        }

        FhPDoubleListIterator<PInstanceList> iter_instances = new(&ptr_cluster->_0x1C_instance_lists);

        while (iter_instances.next(out PInstanceList* ptr_instance_list)) {
            if (Marshal.PtrToStringAnsi(ptr_instance_list->_0x08_free_list._0x10_name) != nameof(PTextureCubeMap))
                continue;

            PFilteredInstanceList          <PTextureCubeMap> texture_filter = new(ptr_instance_list);
            FhPFilteredInstanceListIterator<PTextureCubeMap> texture_iter   = new(texture_filter);

            // We don't know (or even care) what happens in the case that the instance list returns multiple texture objects.
            if (!texture_iter.next(out PTextureCubeMap* ptr_texture)) {
                _logger.Error($"{file_path} - no texture object found in filtered instance list");
                return false;
            }

            // TODO: impl after reversing PTextureCubeMap
        }

        return false;
    }

   /* [fkelava 29/04/26 14:41]
    * Releases are deferred to after the next frame presentation.
    *
    * This is because `render_imgui` does not take effect instantly. Each module's callback
    * adds to the draw list for the current frame, but the actual act of _rendering_
    * only occurs once all have run. Thus, resources which modules use
    * _must survive an unspecified duration past the end of the callback_.
    *
    * If a user issues an ImGui call with a resource, then releases it immediately thereafter,
    * by the time the ImGui module actually attempts to render the frame the resource is freed
    * and an access violation results.
    *
    * Users are unlikely to intuitively understand and obey this rule, so we do this
    * to ensure resources are valid at all points during the current frame.
    */

    /// <summary>
    ///     Releases a <paramref name="texture"/> attained through any previous load call.
    ///     <para/>
    ///     You may not call this method twice for the same texture, nor with an already otherwise released SRV.
    /// </summary>
    bool IFhResourceLoader.release_texture(FhTexture texture) {
        lock (_release_lock) {
            return _release_queue.Add(texture);
        }
    }

    /* [fkelava 01/05/26 23:58]
     * A lock sufficiently protects from a new release being enregistered during enumeration,
     * which would cause a throw. This is because this method is only invoked from `h_present` in
     * the ImGui module, which executes on the Phyre render thread.
     *
     * No `render_imgui` callback will thus run concurrently with this method. This leaves only the case
     * where a release is attempted from a _different_ thread, and the lock suffices.
     */

    /// <summary>
    ///     Flushes the queue of pending resource releases.
    /// </summary>
    /// <remarks>
    ///     It is only valid to call this method on the Phyre render thread, and only
    ///     after draw data for the frame has been rendered. Failing to observe this
    ///     will result in access violations at rendering time or exceptions at release time.
    /// </remarks>
    internal void release_pending_resources() {
        lock (_release_lock) {
            foreach (FhTexture texture in _release_queue) {
                /* [fkelava 01/05/26 18:51]
                 * Testing the return value is meaningless because it is not guaranteed to be precise.
                 *
                 * If you believe you're leaking textures, turn on the D3D debug layer instead.
                 */

                ((ID3D11ShaderResourceView*)(void*)texture.TextureRef.GetTexID())->Release();
            }

            _release_queue.Clear();
        }
    }
}
