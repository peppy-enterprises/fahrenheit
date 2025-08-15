global using System;                          // primitives
global using System.Collections.Generic;      // List<T>, Dictionary<T,U> and others
global using System.Diagnostics.CodeAnalysis; // [NotNullWhen] contract
global using System.IO;                       // Path, File, and similar
global using System.Runtime.CompilerServices; // [InlineArray], [CallConvStdcall] et al.
global using System.Runtime.InteropServices;  // [UnmanagedFunctionPointer], [DllImport], [LibraryImport] et al.
global using System.Runtime.Versioning;       // [SupportedOSPlatform] guard
global using System.Text;                     // Encoding.UTF8 et al.
global using System.Text.Json;                // For JSON (de)serialization, we use STJ.

global using Hexa.NET.ImGui;                  // ImGui is required to render large parts of the runtime UI.

global using TerraFX.Interop.DirectX;         // D3D/DXGI bindings
global using TerraFX.Interop.Windows;         // Win32 bindings

