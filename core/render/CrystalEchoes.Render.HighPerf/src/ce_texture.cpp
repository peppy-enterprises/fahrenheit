/*
 * Crystal Echoes High-Resolution Texture Manager Implementation
 * 
 * Handles loading, caching, and streaming of high-resolution textures
 * for 4K/8K rendering with efficient memory management.
 */

#include "ce_texture.h"
#include <iostream>
#include <fstream>
#include <algorithm>
#include <d3dx12.h>

namespace CrystalEchoes {
namespace Texture {

static TextureManager* g_instance = nullptr;
static std::once_flag g_initFlag;

TextureManager& TextureManager::Instance() {
    std::call_once(g_initFlag, []() {
        g_instance = new TextureManager();
    });
    return *g_instance;
}

TextureManager::TextureManager()
    : m_device(nullptr)
    , m_currentCacheSize(0)
{
    m_lastCameraPosition[0] = 0.0f;
    m_lastCameraPosition[1] = 0.0f;
    m_lastCameraPosition[2] = 0.0f;
}

TextureManager::~TextureManager() {
    Shutdown();
}

bool TextureManager::Initialize(ID3D12Device* device, const TextureConfig& config) {
    if (!device) {
        std::wcerr << L"TextureManager: Invalid D3D12 device." << std::endl;
        return false;
    }

    m_device = device;
    m_config = config;
    m_currentCacheSize = 0;
    
    std::wcout << L"Crystal Echoes Texture Manager initializing..." << std::endl;
    std::wcout << L"  Max Cache Size: " << config.maxCacheSizeMB << L" MB" << std::endl;
    std::wcout << L"  Max Texture Dimension: " << (config.maxTextureDimension > 0 ? std::to_wstring(config.maxTextureDimension) : L"Unlimited") << std::endl;
    std::wcout << L"  Compression: " << (config.enableCompression ? L"Enabled" : L"Disabled") << std::endl;
    std::wcout << L"  Streaming: " << (config.enableStreaming ? L"Enabled" : L"Disabled") << std::endl;

    m_initialized.store(true);
    std::wcout << L"Texture Manager initialized successfully." << std::endl;
    
    return true;
}

void TextureManager::Shutdown() {
    if (!m_initialized.load()) {
        return;
    }

    std::wcout << L"Shutting down texture manager..." << std::endl;
    
    ClearCache();
    
    m_device = nullptr;
    m_initialized.store(false);
    
    std::wcout << L"Texture Manager shutdown complete." << std::endl;
}

Microsoft::WRL::ComPtr<ID3D12Resource> TextureManager::LoadTexture(const std::wstring& path) {
    if (!m_initialized.load()) {
        return nullptr;
    }

    // Generate cache key from path
    std::string cacheKey(path.begin(), path.end());
    
    // Check if already cached
    auto it = m_textureCache.find(cacheKey);
    if (it != m_textureCache.end()) {
        UpdateTextureAccessTime(cacheKey);
        return it->second.resource;
    }

    std::wcout << L"Loading texture: " << path << std::endl;

    Microsoft::WRL::ComPtr<ID3D12Resource> texture;
    
    if (!CreateTextureResources(path, texture)) {
        std::wcerr << L"Failed to load texture: " << path << std::endl;
        return nullptr;
    }

    // Get texture info
    D3D12_RESOURCE_DESC desc = texture->GetDesc();
    
    TextureInfo info;
    info.path = path;
    info.width = static_cast<uint32_t>(desc.Width);
    info.height = desc.Height;
    info.mipLevels = desc.MipLevels;
    info.format = desc.Format;
    info.memorySize = desc.GetSampledPatternSize() * info.width * info.height * info.mipLevels / 4;
    info.lastAccessTime = GetTickCount64();
    info.isStreamed = m_config.enableStreaming;

    // Add to cache
    CachedTexture cachedTex;
    cachedTex.resource = texture;
    cachedTex.info = info;
    cachedTex.key = cacheKey;

    std::lock_guard<std::mutex> lock(m_cacheMutex);
    
    // Check if we need to evict textures
    if (m_currentCacheSize + info.memorySize > m_config.maxCacheSizeMB * 1024 * 1024) {
        EvictOldestTextures(info.memorySize);
    }

    m_textureCache[cacheKey] = cachedTex;
    m_accessOrder.push_back(cacheKey);
    m_currentCacheSize += info.memorySize;

    std::wcout << L"  Loaded: " << info.width << L"x" << info.height 
               << L", " << (info.memorySize / 1024 / 1024) << L" MB" << std::endl;

    return texture;
}

Microsoft::WRL::ComPtr<ID3D12Resource> TextureManager::LoadTextureFromMemory(
    const void* data,
    size_t dataSize,
    uint32_t width,
    uint32_t height)
{
    if (!m_initialized.load() || !data || !m_device) {
        return nullptr;
    }

    // Create upload heap for texture data
    CD3DX12_HEAP_PROPERTIES uploadHeapProps(D3D12_HEAP_TYPE_UPLOAD);
    
    D3D12_RESOURCE_DESC bufferDesc = {};
    bufferDesc.Dimension = D3D12_RESOURCE_DIMENSION_BUFFER;
    bufferDesc.Width = dataSize;
    bufferDesc.Height = 1;
    bufferDesc.DepthOrArraySize = 1;
    bufferDesc.MipLevels = 1;
    bufferDesc.Format = DXGI_FORMAT_UNKNOWN;
    bufferDesc.SampleDesc.Count = 1;
    bufferDesc.Flags = D3D12_RESOURCE_FLAG_NONE;

    Microsoft::WRL::ComPtr<ID3D12Resource> uploadBuffer;
    HRESULT hr = m_device->CreateCommittedResource(
        &uploadHeapProps,
        D3D12_HEAP_FLAG_NONE,
        &bufferDesc,
        D3D12_RESOURCE_STATE_GENERIC_READ,
        nullptr,
        IID_PPV_ARGS(&uploadBuffer));

    if (FAILED(hr)) {
        std::wcerr << L"Failed to create upload buffer." << std::endl;
        return nullptr;
    }

    // Copy texture data to upload buffer
    void* mappedData = nullptr;
    uploadBuffer->Map(0, nullptr, &mappedData);
    memcpy(mappedData, data, dataSize);
    uploadBuffer->Unmap(0, nullptr);

    // Create texture resource
    CD3DX12_HEAP_PROPERTIES defaultHeapProps(D3D12_HEAP_TYPE_DEFAULT);
    
    D3D12_RESOURCE_DESC textureDesc = {};
    textureDesc.Dimension = D3D12_RESOURCE_DIMENSION_TEXTURE2D;
    textureDesc.Width = width;
    textureDesc.Height = height;
    textureDesc.DepthOrArraySize = 1;
    textureDesc.MipLevels = 1;
    textureDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
    textureDesc.SampleDesc.Count = 1;
    textureDesc.Flags = D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET;

    Microsoft::WRL::ComPtr<ID3D12Resource> texture;
    hr = m_device->CreateCommittedResource(
        &defaultHeapProps,
        D3D12_HEAP_FLAG_NONE,
        &textureDesc,
        D3D12_RESOURCE_STATE_COPY_DEST,
        nullptr,
        IID_PPV_ARGS(&texture));

    if (FAILED(hr)) {
        std::wcerr << L"Failed to create texture." << std::endl;
        return nullptr;
    }

    // Copy from upload buffer to texture
    // (Would need command list to execute the copy)

    return texture;
}

ID3D12Resource* TextureManager::GetCachedTexture(const std::string& key) {
    if (!m_initialized.load()) {
        return nullptr;
    }

    std::lock_guard<std::mutex> lock(m_cacheMutex);
    
    auto it = m_textureCache.find(key);
    if (it != m_textureCache.end()) {
        UpdateTextureAccessTime(key);
        return it->second.resource.Get();
    }

    return nullptr;
}

bool TextureManager::IsTextureCached(const std::string& key) {
    if (!m_initialized.load()) {
        return false;
    }

    std::lock_guard<std::mutex> lock(m_cacheMutex);
    return m_textureCache.find(key) != m_textureCache.end();
}

void TextureManager::RemoveTexture(const std::string& key) {
    if (!m_initialized.load()) {
        return;
    }

    std::lock_guard<std::mutex> lock(m_cacheMutex);
    
    auto it = m_textureCache.find(key);
    if (it != m_textureCache.end()) {
        m_currentCacheSize -= it->second.info.memorySize;
        
        // Remove from access order
        auto accessIt = std::find(m_accessOrder.begin(), m_accessOrder.end(), key);
        if (accessIt != m_accessOrder.end()) {
            m_accessOrder.erase(accessIt);
        }
        
        m_textureCache.erase(it);
        
        std::wcout << L"Removed texture from cache: " << key.c_str() << std::endl;
    }
}

void TextureManager::ClearCache() {
    if (!m_initialized.load()) {
        return;
    }

    std::lock_guard<std::mutex> lock(m_cacheMutex);
    
    std::wcout << L"Clearing texture cache (" << (m_currentCacheSize / 1024 / 1024) << L" MB)" << std::endl;
    
    m_textureCache.clear();
    m_accessOrder.clear();
    m_currentCacheSize = 0;
}

size_t TextureManager::GetCacheSize() const {
    return m_currentCacheSize;
}

size_t TextureManager::GetCacheCount() const {
    return m_textureCache.size();
}

size_t TextureManager::GetCacheLimit() const {
    return m_config.maxCacheSizeMB * 1024 * 1024;
}

void TextureManager::SetLODBias(int bias) {
    m_lodBias = bias;
    std::wcout << L"Texture LOD bias set to: " << bias << std::endl;
}

void TextureManager::UpdateStreaming(float cameraPosition[3]) {
    if (!m_initialized.load() || !m_config.enableStreaming) {
        return;
    }

    // Calculate distance from last camera position
    float dx = cameraPosition[0] - m_lastCameraPosition[0];
    float dy = cameraPosition[1] - m_lastCameraPosition[1];
    float dz = cameraPosition[2] - m_lastCameraPosition[2];
    float distance = sqrt(dx * dx + dy * dy + dz * dz);

    // Only update if camera moved significantly
    if (distance < 1.0f) {
        return;
    }

    memcpy(m_lastCameraPosition, cameraPosition, sizeof(m_lastCameraPosition));

    // Update texture streaming based on distance
    // In production, this would:
    // 1. Calculate which textures are visible
    // 2. Determine appropriate LOD for each texture
    // 3. Load/unload textures based on visibility and memory budget
    
    std::lock_guard<std::mutex> lock(m_cacheMutex);
    
    for (auto& pair : m_textureCache) {
        CachedTexture& cachedTex = pair.second;
        
        // Update mip levels based on distance
        // This is a simplified example - real implementation would be more sophisticated
        if (distance > m_config.streamingThreshold) {
            // Reduce quality for distant textures
            cachedTex.info.mipLevels = std::max(1u, cachedTex.info.mipLevels / 2);
        }
    }
}

const TextureInfo* TextureManager::GetTextureInfo(const std::string& key) {
    if (!m_initialized.load()) {
        return nullptr;
    }

    std::lock_guard<std::mutex> lock(m_cacheMutex);
    
    auto it = m_textureCache.find(key);
    if (it != m_textureCache.end()) {
        return &it->second.info;
    }

    return nullptr;
}

bool TextureManager::CreateTextureResources(
    const std::wstring& path,
    Microsoft::WRL::ComPtr<ID3D12Resource>& texture)
{
    if (!m_device) {
        return false;
    }

    // Try to open file
    std::ifstream file(path, std::ios::binary | std::ios::ate);
    if (!file.is_open()) {
        std::wcerr << L"Cannot open texture file: " << path << std::endl;
        return false;
    }

    // Get file size
    std::streamsize fileSize = file.tellg();
    file.seekg(0, std::ios::beg);

    // Read file data
    std::vector<char> buffer(fileSize);
    if (!file.read(buffer.data(), fileSize)) {
        std::wcerr << L"Failed to read texture file." << std::endl;
        return false;
    }

    file.close();

    // Determine texture format
    DXGI_FORMAT format = DetermineFormat(path);
    
    // For simplicity, assume DDS or raw RGBA data
    // In production, use a proper image loading library (stb_image, DirectXTex, etc.)
    
    uint32_t width = 1024;  // Default values - would be read from file header
    uint32_t height = 1024;
    uint32_t mipLevels = 1;

    // Apply max dimension limit
    if (m_config.maxTextureDimension > 0) {
        if (width > m_config.maxTextureDimension) {
            float scale = static_cast<float>(m_config.maxTextureDimension) / width;
            width = m_config.maxTextureDimension;
            height = static_cast<uint32_t>(height * scale);
        }
        if (height > m_config.maxTextureDimension) {
            float scale = static_cast<float>(m_config.maxTextureDimension) / height;
            height = m_config.maxTextureDimension;
            width = static_cast<uint32_t>(width * scale);
        }
    }

    // Create texture
    CD3DX12_HEAP_PROPERTIES heapProps(D3D12_HEAP_TYPE_DEFAULT);
    
    D3D12_RESOURCE_DESC texDesc = {};
    texDesc.Dimension = D3D12_RESOURCE_DIMENSION_TEXTURE2D;
    texDesc.Width = width;
    texDesc.Height = height;
    texDesc.DepthOrArraySize = 1;
    texDesc.MipLevels = mipLevels;
    texDesc.Format = format;
    texDesc.SampleDesc.Count = 1;
    texDesc.Flags = D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET | D3D12_RESOURCE_FLAG_ALLOW_UNORDERED_ACCESS;

    D3D12_CLEAR_VALUE clearVal = {};
    clearVal.Format = format;
    clearVal.Color[0] = 1.0f;
    clearVal.Color[1] = 0.0f;
    clearVal.Color[2] = 1.0f;
    clearVal.Color[3] = 1.0f;

    HRESULT hr = m_device->CreateCommittedResource(
        &heapProps,
        D3D12_HEAP_FLAG_NONE,
        &texDesc,
        D3D12_RESOURCE_STATE_COMMON,
        &clearVal,
        IID_PPV_ARGS(&texture));

    if (FAILED(hr)) {
        std::wcerr << L"Failed to create texture resource." << std::endl;
        return false;
    }

    // Upload texture data (simplified - would need command list in production)
    // ...

    return true;
}

void TextureManager::EvictOldestTextures(size_t bytesToFree) {
    if (m_accessOrder.empty()) {
        return;
    }

    std::wcout << L"Evicting textures to free " << (bytesToFree / 1024 / 1024) << L" MB..." << std::endl;

    size_t freed = 0;
    
    while (freed < bytesToFree && !m_accessOrder.empty()) {
        // Get oldest texture (front of access order queue)
        std::string oldestKey = m_accessOrder.front();
        m_accessOrder.erase(m_accessOrder.begin());
        
        auto it = m_textureCache.find(oldestKey);
        if (it != m_textureCache.end()) {
            freed += it->second.info.memorySize;
            m_textureCache.erase(it);
            
            std::wcout << L"  Evicted: " << oldestKey.c_str() << std::endl;
        }
    }

    m_currentCacheSize -= freed;
    std::wcout << L"Freed " << (freed / 1024 / 1024) << L" MB" << std::endl;
}

void TextureManager::UpdateTextureAccessTime(const std::string& key) {
    // Move accessed texture to end of access order (most recently used)
    auto it = std::find(m_accessOrder.begin(), m_accessOrder.end(), key);
    if (it != m_accessOrder.end()) {
        m_accessOrder.erase(it);
        m_accessOrder.push_back(key);
    }
    
    // Update last access time in cache
    auto cacheIt = m_textureCache.find(key);
    if (cacheIt != m_textureCache.end()) {
        cacheIt->second.info.lastAccessTime = GetTickCount64();
    }
}

DXGI_FORMAT TextureManager::DetermineFormat(const std::wstring& path) {
    // Simple format detection based on file extension
    // In production, would read actual file header
    
    size_t dotPos = path.find_last_of(L".");
    if (dotPos == std::wstring::npos) {
        return DXGI_FORMAT_R8G8B8A8_UNORM;
    }

    std::wstring ext = path.substr(dotPos);
    std::transform(ext.begin(), ext.end(), ext.begin(), ::towlower);

    if (ext == L".dds") {
        // Would parse DDS header for actual format
        return DXGI_FORMAT_BC7_UNORM;
    } else if (ext == L".png" || ext == L".jpg" || ext == L".jpeg") {
        return DXGI_FORMAT_R8G8B8A8_UNORM;
    } else if (ext == L".hdr") {
        return DXGI_FORMAT_R32G32B32A32_FLOAT;
    } else if (ext == L".tga") {
        return DXGI_FORMAT_R8G8B8A8_UNORM;
    }

    return DXGI_FORMAT_R8G8B8A8_UNORM;
}

} // namespace Texture
} // namespace CrystalEchoes

// C Interface Implementation
extern "C" {

__declspec(dllexport) bool CE_Tex_Initialize(void* d3d12Device, uint32_t maxCacheMB) {
    using namespace CrystalEchoes::Texture;
    
    TextureConfig config;
    config.maxCacheSizeMB = maxCacheMB;
    
    return TextureManager::Instance().Initialize(static_cast<ID3D12Device*>(d3d12Device), config);
}

__declspec(dllexport) void CE_Tex_Shutdown() {
    CrystalEchoes::Texture::TextureManager::Instance().Shutdown();
}

__declspec(dllexport) void* CE_Tex_LoadTexture(const wchar_t* path) {
    using namespace CrystalEchoes::Texture;
    
    auto texture = TextureManager::Instance().LoadTexture(std::wstring(path));
    return texture.Detach();  // Caller must release
}

__declspec(dllexport) void* CE_Tex_GetCachedTexture(const char* key) {
    return CrystalEchoes::Texture::TextureManager::Instance().GetCachedTexture(std::string(key));
}

__declspec(dllexport) bool CE_Tex_IsCached(const char* key) {
    return CrystalEchoes::Texture::TextureManager::Instance().IsTextureCached(std::string(key));
}

__declspec(dllexport) void CE_Tex_Remove(const char* key) {
    CrystalEchoes::Texture::TextureManager::Instance().RemoveTexture(std::string(key));
}

__declspec(dllexport) void CE_Tex_ClearCache() {
    CrystalEchoes::Texture::TextureManager::Instance().ClearCache();
}

__declspec(dllexport) size_t CE_Tex_GetCacheSize() {
    return CrystalEchoes::Texture::TextureManager::Instance().GetCacheSize();
}

__declspec(dllexport) size_t CE_Tex_GetCacheCount() {
    return CrystalEchoes::Texture::TextureManager::Instance().GetCacheCount();
}

__declspec(dllexport) void CE_Tex_SetLODBias(int bias) {
    CrystalEchoes::Texture::TextureManager::Instance().SetLODBias(bias);
}

}
