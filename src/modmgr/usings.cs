/* [fkelava 5/7/25 14:16]
 * Hexa bundles some definitions for D3D11 structures that we need to use when interfacing
 * with its API. They are defined this way because we prefer the TerraFX definitions in all other cases.
 */
global using HexaID3D11Device           = Hexa.NET.ImGui.Backends.D3D11.ID3D11Device;
global using HexaID3D11DeviceContext    = Hexa.NET.ImGui.Backends.D3D11.ID3D11DeviceContext;
global using HexaID3D11DeviceContextPtr = Hexa.NET.ImGui.Backends.D3D11.ID3D11DeviceContextPtr;
global using HexaID3D11DevicePtr        = Hexa.NET.ImGui.Backends.D3D11.ID3D11DevicePtr;

global using ImGuiImplD3D11 = Hexa.NET.ImGui.Backends.D3D11.ImGuiImplD3D11;
global using ImGuiImplWin32 = Hexa.NET.ImGui.Backends.Win32.ImGuiImplWin32;

// C# stdlib essentials
global using System;
global using System.Drawing;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Runtime.Versioning;

// ImGui C# binding
global using Hexa.NET.ImGui;

// Win32/D3D11 typedefs
global using Windows.Win32;
global using Windows.Win32.Foundation;
global using Windows.Win32.Storage.FileSystem;
global using Windows.Win32.Graphics.Direct3D11;
global using Windows.Win32.Graphics.Direct3D;
global using Windows.Win32.Graphics.Dxgi;
global using Windows.Win32.Graphics.Dxgi.Common;
global using Windows.Win32.Graphics.Gdi;
global using Windows.Win32.UI.WindowsAndMessaging;
