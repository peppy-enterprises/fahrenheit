/*
 * Crystal Echoes High-Resolution Texture Manager
 * 
 * Handles loading, caching, and streaming of high-resolution textures
 * for 4K/8K rendering.
 */

#pragma once

#include <windows.h>
#include <d3d12.h>
#include <wrl/client.h>
#include <string>
#include <unordered_map>
#include <mutex>
#include <memory>
#include <vector>

namespace CrystalEchoes {
namespace Texture {

    // Texture information structure
    struct TextureInfo {
        std::wstring path;
        uint32_t width;
        uint32_t height;
        uint32_t mipLevels;
        DXGI_FORMAT format;
        size_t memorySize;  // in bytes
        uint64_t lastAccessTime;
        bool isStreamed;
        
        TextureInfo() :
            width(0),
            height(0),
            mipLevels(1),
            format(DXGI_FORMAT_UNKNOWN),
            memorySize(0),
            lastAccessTime(0),
            isStreamed(false)
        {}
    };

    // Texture loading configuration
    struct TextureConfig {
        uint32_t maxCacheSizeMB;      // Maximum texture cache size in MB
        uint32_t maxTextureDimension; // Maximum texture dimension (0 = unlimited)
        bool enableCompression;       // Enable BC compression
        bool enableStreaming;         // Enable texture streaming
        uint32_t streamingThreshold;  // Distance threshold for streaming
        
        TextureConfig() :
            maxCacheSizeMB(2048),
            maxTextureDimension(8192),
            enableCompression(true),
            enableStreaming(true),
            streamingThreshold(100)
        {}
    };

    // High-resolution texture manager
    class TextureManager {
    public:
        static TextureManager& Instance();
        
        // Initialize the texture manager
        bool Initialize(ID3D12Device* device, const TextureConfig& config);
        
        // Shutdown and release all resources
        void Shutdown();
        
        // Load a high-resolution texture
        Microsoft::WRL::ComPtr<ID3D12Resource> LoadTexture(const std::wstring& path);
        
        // Load texture from memory
        Microsoft::WRL::ComPtr<ID3D12Resource> LoadTextureFromMemory(
            const void* data, 
            size_t dataSize,
            uint32_t width,
            uint32_t height);
        
        // Get cached texture by key
        ID3D12Resource* GetCachedTexture(const std::string& key);
        
        // Check if texture is in cache
        bool IsTextureCached(const std::string& key);
        
        // Remove texture from cache
        void RemoveTexture(const std::string& key);
        
        // Clear entire cache
        void ClearCache();
        
        // Get cache statistics
        size_t GetCacheSize() const;        // Current cache size in bytes
        size_t GetCacheCount() const;       // Number of textures in cache
        size_t GetCacheLimit() const;       // Cache limit in bytes
        
        // Set texture LOD bias
        void SetLODBias(int bias);
        int GetLODBias() const { return m_lodBias; }
        
        // Update texture streaming (call each frame)
        void UpdateStreaming(float cameraPosition[3]);
        
        // Get texture info
        const TextureInfo* GetTextureInfo(const std::string& key);
        
        // Check if manager is initialized
        bool IsInitialized() const { return m_initialized.load(); }
        
    private:
        TextureManager();
        ~TextureManager();
        TextureManager(const TextureManager&) = delete;
        TextureManager& operator=(const TextureManager&) = delete;
        
        // Internal methods
        bool CreateTextureResources(
            const std::wstring& path,
            Microsoft::WRL::ComPtr<ID3D12Resource>& texture);
            
        void EvictOldestTextures(size_t bytesToFree);
        void UpdateTextureAccessTime(const std::string& key);
        DXGI_FORMAT DetermineFormat(const std::wstring& path);
        
        // State
        std::atomic<bool> m_initialized{false};
        TextureConfig m_config;
        int m_lodBias{0};
        
        // D3D12 device
        ID3D12Device* m_device;
        
        // Texture cache
        struct CachedTexture {
            Microsoft::WRL::ComPtr<ID3D12Resource> resource;
            TextureInfo info;
            std::string key;
        };
        
        std::unordered_map<std::string, CachedTexture> m_textureCache;
        std::vector<std::string> m_accessOrder;  // For LRU eviction
        std::mutex m_cacheMutex;
        
        // Cache statistics
        size_t m_currentCacheSize;
        
        // Streaming state
        float m_lastCameraPosition[3];
    };

    // C interface for easy interop
    extern "C" {
        __declspec(dllexport) bool CE_Tex_Initialize(void* d3d12Device, uint32_t maxCacheMB);
        __declspec(dllexport) void CE_Tex_Shutdown();
        __declspec(dllexport) void* CE_Tex_LoadTexture(const wchar_t* path);
        __declspec(dllexport) void* CE_Tex_GetCachedTexture(const char* key);
        __declspec(dllexport) bool CE_Tex_IsCached(const char* key);
        __declspec(dllexport) void CE_Tex_Remove(const char* key);
        __declspec(dllexport) void CE_Tex_ClearCache();
        __declspec(dllexport) size_t CE_Tex_GetCacheSize();
        __declspec(dllexport) size_t CE_Tex_GetCacheCount();
        __declspec(dllexport) void CE_Tex_SetLODBias(int bias);
    }

} // namespace Texture
} // namespace CrystalEchoes
