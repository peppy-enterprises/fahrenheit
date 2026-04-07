# Crystal Echoes High-Performance Rendering Module

## Overview

This module provides advanced rendering capabilities for Crystal Echoes (formerly Fahrenheit), enabling:

- **60-120+ FPS** frame rate unlocking
- **4K/8K resolution** support with upscaling
- **Modern upscaling technologies**: DLSS, FSR, XeSS
- **DirectX 12/Vulkan** backend interception
- **High-resolution texture** streaming and caching

## Features

### Frame Rate Unlocking
Removes the original game's 30/60 FPS cap, allowing smooth gameplay at higher refresh rates (60-120+ FPS).

### Resolution Enhancement
- Native 4K (3840x2160) and 8K (7680x4320) output
- Intelligent upscaling from lower internal resolutions
- Anisotropic filtering up to 16x

### Upscaling Technologies
- **AMD FSR** (FidelityFX Super Resolution) - Open source, works on all GPUs
- **NVIDIA DLSS** (Deep Learning Super Sampling) - AI-powered upscaling for RTX cards
- **Intel XeSS** (Xe Super Sampling) - AI upscaling for Intel and compatible GPUs

### API Support
- DirectX 9 hooking and interception (primary target for FFX/FFX-2)
- DirectX 12 modern rendering backend
- Vulkan cross-platform support

## Building

### Prerequisites
- Visual Studio 2022 with C++ Desktop Development workload
- Windows SDK 10.0.19041.0 or later
- vcpkg package manager
- Detours library (Microsoft Research)

### Build Steps

1. Install dependencies via vcpkg:
   ```bash
   vcpkg install --manifest-dir=core/render/CrystalEchoes.Render.HighPerf
   ```

2. Open `CrystalEchoes.slnx` in Visual Studio

3. Build the `CrystalEchoes.Render.HighPerf` project in Release configuration

4. Output DLL will be in `bin/Release/CrystalEchoes.Render.HighPerf.dll`

## Usage

### Configuration

The renderer can be configured via the `RenderConfig` structure:

```cpp
RenderConfig config;
config.targetWidth = 3840;              // 4K width
config.targetHeight = 2160;             // 4K height
config.internalWidth = 1920;            // Render internally at 1080p
config.internalHeight = 1080;
config.upscaleTech = UpscaleTechnology::FSR;
config.enableFrameRateUnlock = true;
config.targetFrameRate = 120.0f;        // Target 120 FPS
config.enableHighResTextures = true;
config.anisotropyLevel = 16;
```

### C Interface

The module exports a simple C interface for easy integration:

```cpp
// Initialize the renderer
RenderConfig config = {};
config.targetWidth = 3840;
config.targetHeight = 2160;
config.targetFrameRate = 120.0f;
CE_Render_Initialize(&config);

// Install hooks to intercept rendering calls
CE_Render_InstallHooks();

// During runtime, query current FPS
float fps = CE_Render_GetCurrentFPS();

// Change settings dynamically
CE_Render_SetTargetResolution(7680, 4320);  // Switch to 8K
CE_Render_SetTargetFrameRate(144.0f);       // Target 144 FPS

// Shutdown when done
CE_Render_Shutdown();
```

### Exported Functions

| Function | Description |
|----------|-------------|
| `CE_Render_Initialize` | Initialize the high-performance renderer |
| `CE_Render_Shutdown` | Clean up and release resources |
| `CE_Render_IsInitialized` | Check if renderer is active |
| `CE_Render_SetTargetResolution` | Set output resolution |
| `CE_Render_SetUpscaleTechnology` | Select upscaling method |
| `CE_Render_SetTargetFrameRate` | Set target FPS |
| `CE_Render_GetCurrentFPS` | Get current frame rate |
| `CE_Render_InstallHooks` | Install rendering hooks |
| `CE_Render_RemoveHooks` | Remove all installed hooks |

## Architecture

### Hook System

The module uses Microsoft Detours to intercept rendering API calls:

1. **DirectX 9 Hooks**: Intercepts `IDirect3DDevice9::Present`, `Reset`, and `CreateDevice`
2. **DirectX 12 Hooks**: Intercepts command queue execution and swap chain present
3. **Vulkan Hooks**: Intercepts `vkQueuePresentKHR` and swapchain creation

### Upscaling Pipeline

```
Game Render (1080p) → Upscaler (FSR/DLSS/XeSS) → Output (4K/8K) → Present
```

### Frame Timing

The module implements precise frame timing to achieve target FPS:
- Performance counter-based timing
- Frame pacing algorithms
- VSync override for uncapped frame rates

## Integration with Crystal Echoes

This module integrates with the Crystal Echoes modding framework through Stage 1 loader:

1. Stage 0 loads Stage 1 DLL into suspended game process
2. Stage 1 initializes .NET runtime and managed code
3. High-performance renderer hooks are installed
4. Game execution proceeds with enhanced rendering

## Performance Considerations

- **GPU Requirements**: 
  - 4K @ 60 FPS: GTX 1070 / RX 580 or better
  - 4K @ 120 FPS: RTX 3070 / RX 6800 XT or better
  - 8K @ 60 FPS: RTX 4090 / RX 7900 XTX recommended

- **Memory**: 8GB VRAM minimum for 4K, 16GB+ recommended for 8K

- **CPU**: Modern 6-core CPU recommended for 120+ FPS targets

## Troubleshooting

### Common Issues

1. **Black screen after initialization**
   - Ensure GPU drivers are up to date
   - Try disabling upscaling temporarily
   - Check that DirectX 12 is supported

2. **Frame rate not unlocking**
   - Verify no external frame limiters are active (RTSS, NVIDIA Control Panel)
   - Check game's native frame rate settings
   - Enable verbose logging

3. **Upscaling artifacts**
   - Adjust sharpness settings in upscaler
   - Ensure internal resolution is appropriate for target
   - Try different upscaling technologies

## License

Part of the Crystal Echoes project. See main repository LICENSE file.

## Third-Party Notices

This module utilizes:
- Microsoft Detours (Microsoft Research)
- AMD FSR (Advanced Micro Devices)
- NVIDIA DLSS (NVIDIA Corporation)
- Intel XeSS (Intel Corporation)
- Vulkan (Khronos Group)

See THIRD-PARTY-NOTICES in the root directory for full license information.
