/*
 * Crystal Echoes Frame Rate Unlocker Module
 * 
 * This module specifically handles frame rate unlocking for the original
 * game engine, removing hardcoded 30/60 FPS limits.
 */

#pragma once

#include <windows.h>
#include <cstdint>
#include <atomic>

namespace CrystalEchoes {
namespace FrameRate {

    // Frame rate unlocker configuration
    struct FrameRateConfig {
        float targetFPS;           // Target frame rate (0 = unlimited)
        bool enableVSync;          // Enable V-Sync
        bool enableFramePacing;    // Enable frame pacing
        float minFrameTime;        // Minimum frame time in ms
        float maxFrameTime;        // Maximum frame time in ms
        
        FrameRateConfig() :
            targetFPS(120.0f),
            enableVSync(false),
            enableFramePacing(true),
            minFrameTime(0.0f),
            maxFrameTime(1000.0f / 30.0f) // Default to 30 FPS cap if pacing enabled
        {}
    };

    // Frame rate unlocker class
    class FrameRateUnlocker {
    public:
        static FrameRateUnlocker& Instance();
        
        // Initialize the frame rate unlocker
        bool Initialize(const FrameRateConfig& config);
        
        // Shutdown and restore original behavior
        void Shutdown();
        
        // Set target frame rate (0 = unlimited)
        void SetTargetFPS(float fps);
        
        // Get current target FPS
        float GetTargetFPS() const { return m_config.targetFPS; }
        
        // Get current actual FPS
        float GetCurrentFPS() const { return m_currentFPS; }
        
        // Enable/disable V-Sync
        void SetVSync(bool enabled);
        bool IsVSyncEnabled() const { return m_config.enableVSync; }
        
        // Apply frame rate limit (call each frame)
        void ApplyFrameLimit();
        
        // Patch game's frame timing functions
        bool PatchGameFrameTiming();
        
        // Restore original frame timing
        void RestoreFrameTiming();
        
        // Check if unlocker is active
        bool IsActive() const { return m_active.load(); }
        
    private:
        FrameRateUnlocker();
        ~FrameRateUnlocker();
        FrameRateUnlocker(const FrameRateUnlocker&) = delete;
        FrameRateUnlocker& operator=(const FrameRateUnlocker&) = delete;
        
        // Internal methods
        void CalculateFrameTime();
        void PatchDirectX9FrameTiming();
        void PatchEngineFrameTiming();
        
        // State
        std::atomic<bool> m_active{false};
        FrameRateConfig m_config;
        float m_currentFPS{0.0f};
        
        // Timing
        LARGE_INTEGER m_performanceFrequency;
        LARGE_INTEGER m_lastFrameTime;
        double m_frameTimeAccumulator;
        uint32_t m_frameCount;
        
        // Original function pointers (for patching)
        void* m_originalSleep = nullptr;
        void* m_originalQueryPerformanceCounter = nullptr;
        
        // Game-specific offsets (to be determined via reverse engineering)
        uintptr_t m_gameFrameTimeOffset = 0;
        uintptr_t m_gameVSyncOffset = 0;
    };

    // C interface for easy interop
    extern "C" {
        __declspec(dllexport) bool CE_FR_Initialize(float targetFPS, bool enableVSync);
        __declspec(dllexport) void CE_FR_Shutdown();
        __declspec(dllexport) void CE_FR_SetTargetFPS(float fps);
        __declspec(dllexport) float CE_FR_GetCurrentFPS();
        __declspec(dllexport) void CE_FR_SetVSync(bool enabled);
        __declspec(dllexport) bool CE_FR_IsActive();
        __declspec(dllexport) void CE_FR_ApplyFrameLimit();
    }

} // namespace FrameRate
} // namespace CrystalEchoes
