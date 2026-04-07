/*
 * Crystal Echoes Upscaling Module
 * 
 * Implements DLSS, FSR, and XeSS upscaling technologies
 * for high-performance rendering at 4K/8K resolutions.
 */

#pragma once

#include <windows.h>
#include <d3d12.h>
#include <wrl/client.h>
#include <cstdint>
#include <string>
#include <functional>

// NVidia DLSS headers (when available)
#ifdef HAVE_DLSS
#include <nvngx_dlss.h>
#endif

// AMD FSR headers (when available)
#ifdef HAVE_FSR
#include <amd_fsr2.h>
#endif

// Intel XeSS headers (when available)
#ifdef HAVE_XESS
#include <xess.h>
#endif

namespace CrystalEchoes {
namespace Upscaling {

    // Upscaling quality presets
    enum class UpscaleQuality {
        Performance,      // Maximum performance, lower quality
        Balanced,         // Balance between quality and performance
        Quality,          // Maximum quality, lower performance
        UltraPerformance, // Extreme performance mode (4x rendering)
        Native            // No upscaling (1:1)
    };

    // Upscaling technology selection
    enum class UpscaleTech {
        None,
        DLSS,      // NVIDIA Deep Learning Super Sampling
        FSR,       // AMD FidelityFX Super Resolution
        XeSS       // Intel Xe Super Sampling
    };

    // Upscaling configuration
    struct UpscaleConfig {
        UpscaleTech technology;
        UpscaleQuality quality;
        
        uint32_t inputWidth;
        uint32_t inputHeight;
        uint32_t outputWidth;
        uint32_t outputHeight;
        
        bool enableSharpening;
        float sharpeningStrength;
        
        bool enableTransparency;
        bool enableHDR;
        
        UpscaleConfig() :
            technology(UpscaleTech::FSR),
            quality(UpscaleQuality::Quality),
            inputWidth(1920),
            inputHeight(1080),
            outputWidth(3840),
            outputHeight(2160),
            enableSharpening(true),
            sharpeningStrength(0.5f),
            enableTransparency(false),
            enableHDR(false)
        {}
    };

    // Frame timing data for temporal upscaling
    struct FrameData {
        Microsoft::WRL::ComPtr<ID3D12Resource> colorBuffer;
        Microsoft::WRL::ComPtr<ID3D12Resource> depthBuffer;
        Microsoft::WRL::ComPtr<ID3D12Resource> motionVectors;
        Microsoft::WRL::ComPtr<ID3D12Resource> exposure;
        
        D3D12_VIEWPORT viewport;
        D3D12_RECT scissorRect;
        
        float deltaTime;
        uint64_t frameIndex;
        
        // Camera data for temporal stability
        float viewMatrix[16];
        float projectionMatrix[16];
        float prevViewMatrix[16];
        float prevProjectionMatrix[16];
    };

    // Upscaler state
    struct UpscalerState {
        bool initialized;
        UpscaleTech activeTech;
        UpscaleQuality activeQuality;
        
        // Internal resources
        Microsoft::WRL::ComPtr<ID3D12Resource> internalColor;
        Microsoft::WRL::ComPtr<ID3D12Resource> internalDepth;
        Microsoft::WRL::ComPtr<ID3D12Resource> upscaledColor;
        Microsoft::WRL::ComPtr<ID3D12Resource> motionVectors;
        
        // Pipeline state objects
        Microsoft::WRL::ComPtr<ID3D12PipelineState> upscalePSO;
        Microsoft::WRL::ComPtr<ID3D12RootSignature> rootSignature;
        
        // Descriptor heaps
        Microsoft::WRL::ComPtr<ID3D12DescriptorHeap> srvHeap;
        Microsoft::WRL::ComPtr<ID3D12DescriptorHeap> uavHeap;
        
        uint32_t currentInputWidth;
        uint32_t currentInputHeight;
        uint32_t currentOutputWidth;
        uint32_t currentOutputHeight;
    };

    // Main upscaler class
    class Upscaler {
    public:
        static Upscaler& Instance();
        
        // Initialize upscaler with configuration
        bool Initialize(ID3D12Device* device, const UpscaleConfig& config);
        
        // Shutdown and release all resources
        void Shutdown();
        
        // Reconfigure upscaler (dynamic resolution changes)
        bool Reconfigure(const UpscaleConfig& config);
        
        // Execute upscaling pass
        void ExecuteUpscale(
            ID3D12GraphicsCommandList* commandList,
            const FrameData& frameData,
            ID3D12Resource* outputTarget);
        
        // Get current upscaling technology
        UpscaleTech GetActiveTech() const { return m_state.activeTech; }
        
        // Get current quality preset
        UpscaleQuality GetActiveQuality() const { return m_state.activeQuality; }
        
        // Check if upscaler is active
        bool IsActive() const { return m_state.initialized && m_state.activeTech != UpscaleTech::None; }
        
        // Get recommended internal resolution for target output
        static void GetRecommendedResolution(
            UpscaleTech tech,
            UpscaleQuality quality,
            uint32_t outputWidth,
            uint32_t outputHeight,
            uint32_t& outInputWidth,
            uint32_t& outInputHeight);
        
        // Get upscaling scale factor
        static float GetScaleFactor(UpscaleQuality quality);
        
        // Set sharpening strength (0.0 - 1.0)
        void SetSharpeningStrength(float strength);
        float GetSharpeningStrength() const { return m_config.sharpeningStrength; }
        
        // Update camera matrices for temporal stability
        void UpdateCameraMatrices(
            const float* viewMatrix,
            const float* projectionMatrix,
            const float* prevViewMatrix,
            const float* prevProjectionMatrix);
        
    private:
        Upscaler();
        ~Upscaler();
        Upscaler(const Upscaler&) = delete;
        Upscaler& operator=(const Upscaler&) = delete;
        
        // Technology-specific initialization
        bool InitializeDLSS(ID3D12Device* device);
        bool InitializeFSR(ID3D12Device* device);
        bool InitializeXeSS(ID3D12Device* device);
        
        // Technology-specific execution
        void ExecuteDLSS(ID3D12GraphicsCommandList* commandList, const FrameData& frameData);
        void ExecuteFSR(ID3D12GraphicsCommandList* commandList, const FrameData& frameData);
        void ExecuteXeSS(ID3D12GraphicsCommandList* commandList, const FrameData& frameData);
        
        // Create common resources
        bool CreateInternalResources(ID3D12Device* device);
        bool CreatePipelineState(ID3D12Device* device);
        
        // Cleanup
        void ReleaseDLSSResources();
        void ReleaseFSRResources();
        void ReleaseXeSSResources();
        
        UpscaleConfig m_config;
        UpscalerState m_state;
        ID3D12Device* m_device;
        
        // DLSS-specific state
#ifdef HAVE_DLSS
        void* m_dlssSession;
        NVSDKNGX_Handle m_dlssHandle;
#endif
        
        // FSR-specific state
#ifdef HAVE_FSR
        void* m_fsrContext;
        FSR2_Context m_fsr2Context;
#endif
        
        // XeSS-specific state
#ifdef HAVE_XESS
        xess_context_t m_xessContext;
#endif
        
        // Camera data for temporal upscaling
        float m_viewMatrix[16];
        float m_projectionMatrix[16];
        float m_prevViewMatrix[16];
        float m_prevProjectionMatrix[16];
    };

    // C interface for easy interop
    extern "C" {
        __declspec(dllexport) bool CE_Upscale_Initialize(void* d3d12Device, int tech, int quality,
                                                         uint32_t inputWidth, uint32_t inputHeight,
                                                         uint32_t outputWidth, uint32_t outputHeight);
        __declspec(dllexport) void CE_Upscale_Shutdown();
        __declspec(dllexport) void CE_Upscale_Execute(void* commandList, void* colorBuffer, 
                                                      void* depthBuffer, void* outputTarget);
        __declspec(dllexport) void CE_Upscale_SetSharpening(float strength);
        __declspec(dllexport) int CE_Upscale_GetActiveTech();
        __declspec(dllexport) bool CE_Upscale_IsActive();
    }

} // namespace Upscaling
} // namespace CrystalEchoes
