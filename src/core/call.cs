// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* [fkelava 26/04/26 15:26]
 * Unlike `call.g.cs`, which contains source-generated delegates with no guarantee of accuracy,
 * this file contains manually annotated calls with proper Fahrenheit types that are vetted for functionality.
 *
 * This file contains calls which are analogous between both games. Unlike the per-game `call.cs` files,
 * functions included here are expected to also provide a 'select' helper for automatic .
 */

using Fahrenheit.Atel;

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

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int d_FUN_0088E6C0_0074C2B0(int param_1);
    public static FhMethodHandle<d_FUN_0088E6C0_0074C2B0> FUN_0088E6C0_0074C2B0 =>
        new( new FhMethodLocation(0x48E6C0, 0x34C2B0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int d_AtelGetSaveDic(AtelBasicWorker* work, int* storage, AtelStack* stack);
    public static FhMethodHandle<d_AtelGetSaveDic> AtelGetSaveDic =>
        new( new FhMethodLocation(0x46C3A0, 0x326B60) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void d_MsSetSavePartyMember(uint param_1, uint param_2, uint param_3);
    public static FhMethodHandle<d_MsSetSavePartyMember> MsSetSavePartyMember =>
        new( new FhMethodLocation(0x386A10, 0x20EB20) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int d_FUN_0088E6A0_0074C290(int param_1);
    public static FhMethodHandle<d_FUN_0088E6A0_0074C290> FUN_0088E6A0_0074C290 =>
        new( new FhMethodLocation(0x48E6A0, 0x34C290) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate uint d_MsGetSaveItemNum(uint param_1);
    public static FhMethodHandle<d_MsGetSaveItemNum> MsGetSaveItemNum =>
        new( new FhMethodLocation(0x390500, 0x220BC0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int d_CT_RetInt_0172_fillPartyMemberMp(AtelBasicWorker* work, int* storage, AtelStack* stack);
    public static FhMethodHandle<d_CT_RetInt_0172_fillPartyMemberMp> CT_RetInt_0172_fillPartyMemberMp =>
        new( new FhMethodLocation(0x45C6B0, 0x319360) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int d_AtelPushMember(AtelBasicWorker* work, int* storage, AtelStack* stack);
    public static FhMethodHandle<d_AtelPushMember> AtelPushMember =>
        new( new FhMethodLocation(0x46E2A0, 0x328DA0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int d_AtelPopMember(AtelBasicWorker* work, int* storage, AtelStack* stack);
    public static FhMethodHandle<d_AtelPopMember> AtelPopMember =>
        new( new FhMethodLocation(0x46DD40, 0x3287E0) );

    // RT - Input tracking

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate void d_AtelExec_Internal_871D10();
    internal static FhMethodHandle<d_AtelExec_Internal_871D10> AtelExec_Internal_871D10 =>
        new( new FhMethodLocation(0x471D10, 0x32CE90) );

    // RT - Platform bind

    /* [fkelava 25/4/24 17:51]
     * https://github.com/terrafx/terrafx.interop.windows/blob/55590efae0f77f4c8db465a80d18b4f5b679696c/sources/Interop/Windows/DirectX/um/d3d11/DirectX.cs#L25
     */
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate HRESULT d_D3D11_D3D11CreateDeviceAndSwapChain(
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
    internal static FhMethodHandle<d_D3D11_D3D11CreateDeviceAndSwapChain> D3D11_D3D11CreateDeviceAndSwapChain =>
        new( new FhMethodLocation("D3D11.dll", "D3D11CreateDeviceAndSwapChain") );

    // RT - Game UI

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate void d_TODrawMessageWindow();
    internal static FhMethodHandle<d_TODrawMessageWindow> TODrawMessageWindow
        => new( new FhMethodLocation(0x4ABCE0, 0x391D00) );

    // RT - ImGui

    /* [fkelava 6/10/24 01:54]
     * See src/core/native/Windows.Win32.IDXGISwapChain.g.cs for swapchain method signatures.
     */
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate nint d_DXGI_IDXGISwapChain_Present(
        IDXGISwapChain* pSwapChain,
        uint            SyncInterval,
        DXGI_PRESENT    Flags);
    internal static FhMethodHandle<d_DXGI_IDXGISwapChain_Present> DXGI_IDXGISwapChain_Present
        (IDXGISwapChain* ptr_swapchain)
        => new( new FhMethodLocation(ptr_swapchain->lpVtbl[8]) );

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate nint d_DXGI_IDXGISwapChain_ResizeBuffers(
        IDXGISwapChain* pSwapChain,
        uint            BufferCount,
        uint            Width,
        uint            Height,
        DXGI_FORMAT     NewFormat,
        uint            SwapChainFlags);
    internal static FhMethodHandle<d_DXGI_IDXGISwapChain_ResizeBuffers> DXGI_IDXGISwapChain_ResizeBuffers
        (IDXGISwapChain* ptr_swapchain)
        => new( new FhMethodLocation(ptr_swapchain->lpVtbl[13]) );

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int d_Phyre_PFramework_PInput_Update();
    internal static FhMethodHandle<d_Phyre_PFramework_PInput_Update> Phyre_PFramework_PInput_Update
        => new( new FhMethodLocation(0x225930, 0x6B51E0) );

    // RT - Game loop

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void d_Sg_MainLoop(float delta);
    internal static FhMethodHandle<d_Sg_MainLoop> Sg_MainLoop
        => new( new FhMethodLocation(0x420C00, 0x205150) );

    // RT - EFL

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    internal delegate PStreamFile* d_Phyre_PSerialization_PStreamFile_ctor(
        PStreamFile* ptr_this,
        nint         ptr_path,
        bool         read_only,
        nint         p3,  // unused?
        nint         p4,  // unused?
        bool         p5); // unused?
    internal static FhMethodHandle<d_Phyre_PSerialization_PStreamFile_ctor> Phyre_PSerialization_PStreamFile_ctor
        => new( new FhMethodLocation(0x207D80, 0x490E40) );

    // RT - Phyre loader

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    internal delegate PCluster* d_ClusterManager_loadPCluster(nint ptr_this, byte* ptr_file_name);
    internal static FhMethodHandle<d_ClusterManager_loadPCluster> ClusterManager_loadPCluster
        => new( new FhMethodLocation(0x29BA80, 0x9E880) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate int d_PApplication_FixupClusters(PCluster** ptr_clusters, int nb_clusters);
    internal static FhMethodHandle<d_PApplication_FixupClusters> PApplication_FixupClusters
        => new( new FhMethodLocation(0x223740, 0x6B3020) );

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    internal delegate void d_ClusterManager_releasePCluster(nint ptr_this, PCluster* ptr_cluster);
    internal static FhMethodHandle<d_ClusterManager_releasePCluster> ClusterManager_releasePCluster
        => new( new FhMethodLocation(0x29BEF0, 0x9ED00) );

    // RT - CD

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate nint d_CDfileSize_PC(int arg1);
    internal static FhMethodHandle<d_CDfileSize_PC> CDfileSize_PC
        => new( new FhMethodLocation(0x6428A0, 0x74E9A0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate nint d_check_ex_file_size(int arg1, int arg2);
    internal static FhMethodHandle<d_check_ex_file_size> check_ex_file_size
        => new( new FhMethodLocation(0x36D770, 0x1396A0) );

    // Save PAL
    // RT - Save impl

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void d_SaveDataManager_debugSave_Internal_6F0650(int size, byte* ptr);
    internal static FhMethodHandle<d_SaveDataManager_debugSave_Internal_6F0650> SaveDataManager_debugSave_Internal_6F0650
        => new( new FhMethodLocation(0x2F0650, 0x11D510) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void d_SaveDataToSave();
    internal static FhMethodHandle<d_SaveDataToSave> SaveDataToSave
        => new( new FhMethodLocation(0x248950, 0x884D0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void d_SaveDataToLoad();
    internal static FhMethodHandle<d_SaveDataToLoad> SaveDataToLoad
        => new( new FhMethodLocation(0x248910, 0x884A0) );

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate void d_TkMenuJumpToLoadedScene();
    internal static FhMethodHandle<d_TkMenuJumpToLoadedScene> TkMenuJumpToLoadedScene
        => new( new FhMethodLocation(0x4B4E70, 0x36AD50 ) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate int d_fix_mappic(ushort arg1);
    internal static FhMethodHandle<d_fix_mappic> fix_mappic
        => new( new FhMethodLocation(0x2EF830, 0x11C9B0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate int d_isNeedShowJapanLogo();
    internal static FhMethodHandle<d_isNeedShowJapanLogo> isNeedShowJapanLogo
        => new( new FhMethodLocation(0x387450, 0x20F500) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate byte* d_AtelGetSaveDicName(ushort arg1, uint arg2);
    internal static FhMethodHandle<d_AtelGetSaveDicName> AtelGetSaveDicName
        => new( new FhMethodLocation(0x46C3C0, 0x326B80) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void d_SaveDataGetLoc(int arg1, byte* arg2);
    internal static FhMethodHandle<d_SaveDataGetLoc> SaveDataGetLoc
        => new( new FhMethodLocation(0x2480E0, 0x87CB0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate nint d_SaveDataWriteCrc(byte* arg1);
    internal static FhMethodHandle<d_SaveDataWriteCrc> SaveDataWriteCrc
        => new( new FhMethodLocation(0x2490D0, 0x889C0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate int d_SaveDataCheckCrc();
    internal static FhMethodHandle<d_SaveDataCheckCrc> SaveDataCheckCrc
        => new( new FhMethodLocation(0x247F20, 0x87B10) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void d__SetUpDefaultSaveFolder();
    internal static FhMethodHandle<d__SetUpDefaultSaveFolder> _SetUpDefaultSaveFolder
        => new( new FhMethodLocation(0x2F0470, 0x11D310) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate bool d_isNeedRenamePlayer(byte arg1);
    internal static FhMethodHandle<d_isNeedRenamePlayer> isNeedRenamePlayer
        => new( new FhMethodLocation(0x387430, 0x20F4E0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void d_SaveDataSaveLoadSucceed(FhSaveSystemState arg1);
    internal static FhMethodHandle<d_SaveDataSaveLoadSucceed> SaveDataSaveLoadSucceed
        => new( new FhMethodLocation(0x2486F0, 0x88290) );

    // INTERNAL/RESTRICTED - END

    // PUBLIC/UNRESTRICTED - BEGIN

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void d_CT_Init(AtelBasicWorker* work, int* storage, AtelStack* stack);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int d_CT_Exec(AtelBasicWorker* work, AtelStack* stack);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int d_CT_RetInt(AtelBasicWorker* work, int* storage, AtelStack* stack);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate float d_CT_RetFloat(AtelBasicWorker* work, int* storage, AtelStack* stack);

    // `printf` and similar methods for use by the debug mod

    /* [fkelava 17/7/25 02:33]
     * For vararg functions the delegate signature should have an argument count >=
     * the argument count of the invocation with the most varargs in the executable.
     *
     * For now we assume sixteen. If you crash with a buffer/stack overrun, increase it.
     */

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void d__Printf(string fmt,
        nint va0,  nint va1,  nint va2,  nint va3,
        nint va4,  nint va5,  nint va6,  nint va7,
        nint va8,  nint va9,  nint va10, nint va11,
        nint va12, nint va13, nint va14, nint va15);

    public static FhMethodHandle<d__Printf> dbgPrintf =>
        new( new FhMethodLocation(0x22F6B0, 0x9ADD0) );
    public static FhMethodHandle<d__Printf> scePrintf =>
        new( new FhMethodLocation(0x22FDA0, 0x9B4B0) );
    public static FhMethodHandle<d__Printf> AtelPs2DebugString =>
        new( new FhMethodLocation(0x473C10, 0x30E9E0) );
    public static FhMethodHandle<d__Printf> AtelPs2DebugString2 =>
        new( new FhMethodLocation(0x473C20, 0x30E9F0) );
    public static FhMethodHandle<d__Printf> rcPrint =>
        new( new FhMethodLocation(0x527550, 0x3D9690) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void d_Phyre_PhyrePrintf(int rc, string fmt,
        nint va0,  nint va1,  nint va2,  nint va3,
        nint va4,  nint va5,  nint va6,  nint va7,
        nint va8,  nint va9,  nint va10, nint va11,
        nint va12, nint va13, nint va14, nint va15);

    public static FhMethodHandle<d_Phyre_PhyrePrintf> Phyre_PhyrePrintf =>
        new ( new FhMethodLocation(0x0353F0, 0x48CC60) );

    // PUBLIC/UNRESTRICTED - END

}
