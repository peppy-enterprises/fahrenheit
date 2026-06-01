// SPDX-License-Identifier: MIT

/* [fkelava 26/04/26 15:26]
 * Unlike `call.g.cs`, which contains source-generated delegates with no guarantee of accuracy,
 * this file contains manually annotated calls with proper Fahrenheit types that are vetted for functionality.
 *
 * This file contains calls which are analogous between both games. Unlike the per-game `call.cs` files,
 * functions included here are expected to also provide a 'select' helper for automatic .
 */

namespace Fahrenheit;

/// <summary>
///     An accessor for game function calls available in both titles.
/// </summary>
public static unsafe partial class FhCall {

    // INTERNAL/RESTRICTED - BEGIN

    /* [fkelava 28/05/26 13:40]
     * Some methods are `restricted` - only meant to be overridden by the runtime.
     * We neither permit nor support any other mod tampering with them.
     *
     * Since the runtime (and only the runtime) has IVT into the core,
     * we provide for this by marking such methods' delegates `internal`.
     *
     * Attempting to actively prohibit method handles being constructed
     * over `restricted` methods is pointless; the user always has a means to
     * circumvent it. We simply refuse to support any such scenario.
     */

    // RT - Input tracking

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate void AtelExec_Internal_871D10();
    internal static FhMethodHandle<AtelExec_Internal_871D10> h_AtelExec_Internal_871D10 =>
        new( new FhMethodLocation(0x471D10, 0x32CE90) );

    // RT - Platform bind

    /* [fkelava 25/4/24 17:51]
     * https://github.com/terrafx/terrafx.interop.windows/blob/55590efae0f77f4c8db465a80d18b4f5b679696c/sources/Interop/Windows/DirectX/um/d3d11/DirectX.cs#L25
     */
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate HRESULT D3D11_D3D11CreateDeviceAndSwapChain(
        IDXGIAdapter*         pAdapter,
        D3D_DRIVER_TYPE       DriverType,
        HMODULE               Software,
        uint                  Flags,
        D3D_FEATURE_LEVEL*    pFeatureLevels,
        uint                  FeatureLevels,
        uint                  SDKVersion,
        DXGI_SWAP_CHAIN_DESC* pSwapChainDesc,
        IDXGISwapChain**      ppSwapChain,
        ID3D11Device**        ppDevice,
        D3D_FEATURE_LEVEL*    pFeatureLevel,
        ID3D11DeviceContext** ppImmediateContext);
    internal static FhMethodHandle<D3D11_D3D11CreateDeviceAndSwapChain> h_D3D11_D3D11CreateDeviceAndSwapChain =>
        new( new FhMethodLocation("D3D11.dll", "D3D11CreateDeviceAndSwapChain") );

    // RT - Game UI

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate void TODrawMessageWindow();
    internal static FhMethodHandle<TODrawMessageWindow> h_TODrawMessageWindow
        => new( new FhMethodLocation(0x4ABCE0, 0x391D00) );

    // RT - ImGui

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int Phyre_PFramework_PInput_Update();
    internal static FhMethodHandle<Phyre_PFramework_PInput_Update> h_Phyre_PFramework_PInput_Update
        => new( new FhMethodLocation(0x225930, 0x6B51E0) );

    // RT - Game loop

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void Sg_MainLoop(float delta);
    internal static FhMethodHandle<Sg_MainLoop> h_Sg_MainLoop
        => new ( new FhMethodLocation(0x420C00, 0x205150) );

    // RT - EFL

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    internal delegate PStreamFile* Phyre_PSerialization_PStreamFile_ctor(
        PStreamFile* ptr_this,
        nint         ptr_path,
        bool         read_only,
        nint         p3,  // unused?
        nint         p4,  // unused?
        bool         p5); // unused?
    internal static FhMethodHandle<Phyre_PSerialization_PStreamFile_ctor> h_Phyre_PSerialization_PStreamFile_ctor
        => new ( new FhMethodLocation(0x207D80, 0x490E40) );

    // RT - Phyre loader

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    internal delegate PCluster* ClusterManager_loadPCluster(nint ptr_this, byte* ptr_file_name);
    internal static FhMethodHandle<ClusterManager_loadPCluster> h_ClusterManager_loadPCluster
        => new( new FhMethodLocation(0x29BA80, 0x9E880) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate int PApplication_FixupClusters(PCluster** ptr_clusters, int nb_clusters);
    internal static FhMethodHandle<PApplication_FixupClusters> h_PApplication_FixupClusters
        => new( new FhMethodLocation(0x223740, 0x6B3020) );

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    internal delegate void ClusterManager_releasePCluster(nint ptr_this, PCluster* ptr_cluster);
    internal static FhMethodHandle<ClusterManager_releasePCluster> h_ClusterManager_releasePCluster
        => new( new FhMethodLocation(0x29BEF0, 0x9ED00) );

    // RT - CD

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate nint CDfileSize_PC(int arg1);
    internal static FhMethodHandle<CDfileSize_PC> h_CDfileSize_PC
        => new( new FhMethodLocation(0x6428A0, 0x74E9A0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate nint check_ex_file_size(int arg1, int arg2);
    internal static FhMethodHandle<check_ex_file_size> h_check_ex_file_size
        => new( new FhMethodLocation(0x36D770, 0x1396A0) );

    // Save PAL
    // RT - Save impl

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void SaveDataManager_debugSave_Internal_6F0650(int size, byte* ptr);
    internal static FhMethodHandle<SaveDataManager_debugSave_Internal_6F0650> h_SaveDataManager_debugSave_Internal_6F0650
        => new ( new FhMethodLocation(0x2F0650, 0x11D510) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void SaveDataToSave();
    internal static FhMethodHandle<SaveDataToSave> h_SaveDataToSave
        => new ( new FhMethodLocation(0x248950, 0x884D0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void SaveDataToLoad();
    internal static FhMethodHandle<SaveDataToLoad> h_SaveDataToLoad
        => new( new FhMethodLocation(0x248910, 0x884A0) );

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate void TkMenuJumpToLoadedScene();
    internal static FhMethodHandle<TkMenuJumpToLoadedScene> h_TkMenuJumpToLoadedScene
        => new ( new FhMethodLocation(0x4B4E70, 0x36AD50 ) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate int fix_mappic(ushort arg1);
    internal static FhMethodHandle<fix_mappic> h_fix_mappic
        => new( new FhMethodLocation(0x2EF830, 0x11C9B0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate int isNeedShowJapanLogo();
    internal static FhMethodHandle<isNeedShowJapanLogo> h_isNeedShowJapanLogo
        => new( new FhMethodLocation(0x387450, 0x20F500) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate byte* AtelGetSaveDicName(ushort arg1, uint arg2);
    internal static FhMethodHandle<AtelGetSaveDicName> h_AtelGetSaveDicName
        => new( new FhMethodLocation(0x46C3C0, 0x326B80) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void SaveDataGetLoc(int arg1, byte* arg2);
    internal static FhMethodHandle<SaveDataGetLoc> h_SaveDataGetLoc
        => new( new FhMethodLocation(0x2480E0, 0x87CB0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate nint SaveDataWriteCrc(byte* arg1);
    internal static FhMethodHandle<SaveDataWriteCrc> h_SaveDataWriteCrc
        => new( new FhMethodLocation(0x2490D0, 0x889C0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate int SaveDataCheckCrc();
    internal static FhMethodHandle<SaveDataCheckCrc> h_SaveDataCheckCrc
        => new( new FhMethodLocation(0x247F20, 0x87B10) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void _SetUpDefaultSaveFolder();
    internal static FhMethodHandle<_SetUpDefaultSaveFolder> h__SetUpDefaultSaveFolder
        => new( new FhMethodLocation(0x2F0470, 0x11D310) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate bool isNeedRenamePlayer(byte arg1);
    internal static FhMethodHandle<isNeedRenamePlayer> h_isNeedRenamePlayer
        => new( new FhMethodLocation(0x387430, 0x20F4E0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void SaveDataSaveLoadSucceed(FhSaveSystemState arg1);
    internal static FhMethodHandle<SaveDataSaveLoadSucceed> h_SaveDataSaveLoadSucceed
        => new( new FhMethodLocation(0x2486F0, 0x88290) );

    // INTERNAL/RESTRICTED - END

    // PUBLIC/UNRESTRICTED - BEGIN

    // PUBLIC/UNRESTRICTED - END

}
