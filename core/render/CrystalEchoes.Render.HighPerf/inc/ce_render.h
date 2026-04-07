#pragma once

/*
 * Crystal Echoes High-Performance Rendering Module
 * 
 * This module provides:
 * - DirectX 12 / Vulkan rendering backend interception
 * - DLSS/FSR upscaling support
 * - Frame rate unlocking (60-120+ FPS)
 * - 4K/8K resolution support
 * - High-resolution texture streaming
 */

#include <windows.h>
#include <d3d12.h>
#include <dxgi1_6.h>
#include <vulkan/vulkan.h>
#include <wrl/client.h>
#include <memory>
#include <functional>
#include <string>
#include <vector>
#include <atomic>

// Forward declarations for Detours
extern "C" {
    #include <detours.h>
}

// Upscaling technology enums
enum class UpscaleTechnology {
    None,
    DLSS,
    FSR,
    XeSS
};

enum class RenderAPI {
    DirectX9,
    DirectX12,
    Vulkan,
    OpenGL
};

struct RenderConfig {
    uint32_t targetWidth;
    uint32_t targetHeight;
    uint32_t internalWidth;
    uint32_t internalHeight;
    UpscaleTechnology upscaleTech;
    bool enableFrameRateUnlock;
    float targetFrameRate;
    bool enableHighResTextures;
    uint32_t textureCacheSizeMB;
    bool enableAnisotropicFiltering;
    uint32_t anisotropyLevel;
    bool enableAntiAliasing;
    uint32_t msaaCount;
    bool enableHDR;
    bool enableRayTracing;
    
    RenderConfig() :
        targetWidth(3840),
        targetHeight(2160),
        internalWidth(1920),
        internalHeight(1080),
        upscaleTech(UpscaleTechnology::FSR),
        enableFrameRateUnlock(true),
        targetFrameRate(120.0f),
        enableHighResTextures(true),
        textureCacheSizeMB(2048),
        enableAnisotropicFiltering(true),
        anisotropyLevel(16),
        enableAntiAliasing(true),
        msaaCount(4),
        enableHDR(false),
        enableRayTracing(false)
    {}
};

// Hook function types for DirectX 9
typedef HRESULT (WINAPI *D3D9_Present_t)(IDirect3DDevice9* pDevice, const RECT* pSourceRect, const RECT* pDestRect, HWND hDestWindowOverride, const RGNDATA* pDirtyRegion);
typedef HRESULT (WINAPI *D3D9_Reset_t)(IDirect3DDevice9* pDevice, D3DPRESENT_PARAMETERS* pPresentationParameters);
typedef HRESULT (WINAPI *D3D9_CreateDevice_t)(IDirect3D9* pD3D, UINT Adapter, D3DDEVTYPE DeviceType, HWND hFocusWindow, DWORD BehaviorFlags, D3DPRESENT_PARAMETERS* pPresentationParameters, IDirect3DDevice9** ppReturnedDeviceInterface);

// Hook function types for DirectX 12
typedef HRESULT (WINAPI *D3D12_ExecuteCommandLists_t)(ID3D12CommandQueue* pCommandQueue, UINT NumCommandLists, ID3D12CommandList* ppCommandLists);
typedef HRESULT (WINAPI *D3D12_Present_t)(IDXGISwapChain3* pSwapChain, UINT SyncInterval, UINT Flags);

// Hook function types for Vulkan
typedef VkResult (VKAPI_PTR *VK_QueuePresentKHR_t)(VkQueue queue, const VkPresentInfoKHR* pPresentInfo);
typedef VkResult (VKAPI_PTR *VK_CreateSwapchainKHR_t)(VkDevice device, const VkSwapchainCreateInfoKHR* pCreateInfo, const VkAllocationCallbacks* pAllocator, VkSwapchainKHR* pSwapchain);

class HighPerfRenderer {
public:
    static HighPerfRenderer& Instance();
    
    bool Initialize(const RenderConfig& config);
    void Shutdown();
    
    // Rendering state management
    void BeginFrame();
    void EndFrame();
    void Present();
    
    // Upscaling
    bool InitializeUpscaler(UpscaleTechnology tech, uint32_t inputWidth, uint32_t inputHeight, uint32_t outputWidth, uint32_t outputHeight);
    void ExecuteUpscale(ID3D12GraphicsCommandList* cmdList, ID3D12Resource* input, ID3D12Resource* output);
    
    // Frame rate management
    void UnlockFrameRate(float targetFPS);
    void ApplyFrameRateLimit();
    
    // Texture management
    void LoadHighResTexture(const std::wstring& path, uint32_t width, uint32_t height);
    void CacheTexture(const std::string& key, ID3D12Resource* texture);
    ID3D12Resource* GetCachedTexture(const std::string& key);
    
    // Hook installation
    bool InstallDirectX9Hooks();
    bool InstallDirectX12Hooks();
    bool InstallVulkanHooks();
    void RemoveAllHooks();
    
    // Configuration
    void SetConfig(const RenderConfig& config);
    const RenderConfig& GetConfig() const { return m_config; }
    
    // State queries
    bool IsInitialized() const { return m_initialized; }
    RenderAPI GetCurrentAPI() const { return m_currentAPI; }
    float GetCurrentFPS() const { return m_currentFPS; }
    
private:
    HighPerfRenderer();
    ~HighPerfRenderer();
    HighPerfRenderer(const HighPerfRenderer&) = delete;
    HighPerfRenderer& operator=(const HighPerfRenderer&) = delete;
    
    // Internal methods
    bool CreateRenderDevice();
    bool CreateSwapChain();
    bool CreateRenderTargets();
    void DestroyRenderTargets();
    
    // Hook handlers - DirectX 9
    static HRESULT WINAPI Hooked_D3D9_Present(IDirect3DDevice9* pDevice, const RECT* pSourceRect, const RECT* pDestRect, HWND hDestWindowOverride, const RGNDATA* pDirtyRegion);
    static HRESULT WINAPI Hooked_D3D9_Reset(IDirect3DDevice9* pDevice, D3DPRESENT_PARAMETERS* pPresentationParameters);
    static HRESULT WINAPI Hooked_D3D9_CreateDevice(IDirect3D9* pD3D, UINT Adapter, D3DDEVTYPE DeviceType, HWND hFocusWindow, DWORD BehaviorFlags, D3DPRESENT_PARAMETERS* pPresentationParameters, IDirect3DDevice9** ppReturnedDeviceInterface);
    
    // Hook handlers - DirectX 12
    static HRESULT WINAPI Hooked_D3D12_ExecuteCommandLists(ID3D12CommandQueue* pCommandQueue, UINT NumCommandLists, ID3D12CommandList* ppCommandLists);
    static HRESULT WINAPI Hooked_D3D12_Present(IDXGISwapChain3* pSwapChain, UINT SyncInterval, UINT Flags);
    
    // Hook handlers - Vulkan
    static VkResult VKAPI_PTR Hooked_VK_QueuePresentKHR(VkQueue queue, const VkPresentInfoKHR* pPresentInfo);
    static VkResult VKAPI_PTR Hooked_VK_CreateSwapchainKHR(VkDevice device, const VkSwapchainCreateInfoKHR* pCreateInfo, const VkAllocationCallbacks* pAllocator, VkSwapchainKHR* pSwapchain);
    
    // Original function pointers
    D3D9_Present_t m_originalD3D9Present = nullptr;
    D3D9_Reset_t m_originalD3D9Reset = nullptr;
    D3D9_CreateDevice_t m_originalD3D9CreateDevice = nullptr;
    
    D3D12_ExecuteCommandLists_t m_originalD3D12ExecuteCommandLists = nullptr;
    D3D12_Present_t m_originalD3D12Present = nullptr;
    
    VK_QueuePresentKHR_t m_originalVKQueuePresent = nullptr;
    VK_CreateSwapchainKHR_t m_originalVKCreateSwapchain = nullptr;
    
    // State
    std::atomic<bool> m_initialized{false};
    RenderAPI m_currentAPI{RenderAPI::DirectX9};
    RenderConfig m_config;
    float m_currentFPS{0.0f};
    
    // DirectX 12 resources
    Microsoft::WRL::ComPtr<ID3D12Device> m_d3d12Device;
    Microsoft::WRL::ComPtr<ID3D12CommandQueue> m_commandQueue;
    Microsoft::WRL::ComPtr<IDXGISwapChain3> m_swapChain;
    
    // Timing
    LARGE_INTEGER m_performanceFrequency;
    LARGE_INTEGER m_lastFrameTime;
    uint32_t m_frameCount;
    float m_fpsAccumulator;
};

// Exported C interface for easy interop
extern "C" {
    __declspec(dllexport) bool CE_Render_Initialize(const RenderConfig* config);
    __declspec(dllexport) void CE_Render_Shutdown();
    __declspec(dllexport) bool CE_Render_IsInitialized();
    __declspec(dllexport) void CE_Render_SetTargetResolution(uint32_t width, uint32_t height);
    __declspec(dllexport) void CE_Render_SetUpscaleTechnology(UpscaleTechnology tech);
    __declspec(dllexport) void CE_Render_SetTargetFrameRate(float fps);
    __declspec(dllexport) float CE_Render_GetCurrentFPS();
    __declspec(dllexport) bool CE_Render_InstallHooks();
    __declspec(dllexport) void CE_Render_RemoveHooks();
}
