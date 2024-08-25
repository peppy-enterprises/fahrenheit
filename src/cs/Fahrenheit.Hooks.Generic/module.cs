using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using ImGuiNET;

using Fahrenheit.CLRHost;
using Fahrenheit.CoreLib;

using static Fahrenheit.CoreLib.FhHookDelegates;

namespace Fahrenheit.Hooks.Generic;

public sealed record FhHooksBaseModuleConfig : FhModuleConfig {
    public FhHooksBaseModuleConfig(string configName,
                                      uint   configVersion,
                                      bool   configEnabled) : base(configName, configVersion, configEnabled) {
    }

    public override bool TrySpawnModule([NotNullWhen(true)] out FhModule? fm) {
        fm = new FhHooksBaseModule(this);
        return fm.ModuleState == FhModuleState.InitSuccess;
    }
}

public unsafe partial class FhHooksBaseModule : FhModule {
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Sg_MainLoop(float delta);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void AtelExecInternal_00871d10();

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void D3D11CreateDeviceAndSwapChain(
            nint pAdapter,
            nint DriverType,
            nint software,
            uint flags,
            nint pFeatureLevels,
            uint FeatureLevels,
            uint SDKVersion,
            nint pSwapChainDesc,
            nint ppSwapChain,
            nint ppDevice,
            nint pFeatureLevel,
            nint ppImmediateContext);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate nint D3DKMTPresent(nint param1);

    private readonly FhHooksBaseModuleConfig           _moduleConfig;

    private readonly FhMethodHandle<TkIsDebugDelegate>    _tkIsDbg;
    private readonly FhMethodHandle<PrintfVarargDelegate> _printf_22F6B0;
    private readonly FhMethodHandle<PrintfVarargDelegate> _printf_22FDA0;
    private readonly FhMethodHandle<PrintfVarargDelegate> _printf_473C10;
    private readonly FhMethodHandle<PrintfVarargDelegate> _printf_473C20;
    private readonly FhMethodHandle<Sg_MainLoop> _main_loop;
    private readonly FhMethodHandle<AtelExecInternal_00871d10> _update_input;
    private readonly FhMethodHandle<D3DKMTPresent> _render;
    private readonly FhMethodHandle<D3D11CreateDeviceAndSwapChain> _init_imgui;

    public override FhHooksBaseModuleConfig ModuleConfiguration => _moduleConfig;

    private static bool ready_to_init_imgui = false;
    private static bool initialized_imgui = false;
    private static void* pDevice;
    private static void* pContext;

    public FhHooksBaseModule(FhHooksBaseModuleConfig cfg) : base(cfg) {
        _moduleConfig = cfg;

        _tkIsDbg       = new FhMethodHandle<TkIsDebugDelegate>(this, 0x487C80, TkIsDebugHook);
        _printf_22F6B0 = new FhMethodHandle<PrintfVarargDelegate>(this, 0x22F6B0, FhHooks.CLRPrintfHookAnsi);
        _printf_22FDA0 = new FhMethodHandle<PrintfVarargDelegate>(this, 0x22FDA0, FhHooks.CLRPrintfHookAnsi);
        _printf_473C10 = new FhMethodHandle<PrintfVarargDelegate>(this, 0x473C10, FhHooks.CLRPrintfHookAnsi);
        _printf_473C20 = new FhMethodHandle<PrintfVarargDelegate>(this, 0x473C20, FhHooks.CLRPrintfHookAnsi);

        // Providing basic functionality to other modules
        _main_loop = new FhMethodHandle<Sg_MainLoop>(this, 0x420C00, main_loop);
        _update_input = new FhMethodHandle<AtelExecInternal_00871d10>(this, 0x471D10, update_input);
        _init_imgui = new FhMethodHandle<D3D11CreateDeviceAndSwapChain>(
                this,
                FhPInvoke.GetProcAddress(FhPInvoke.GetModuleHandle("D3D11.dll"), "D3D11CreateDeviceAndSwapChain") - FhGlobal.base_addr,
                init_imgui);
        _render = new FhMethodHandle<D3DKMTPresent>(
                this,
                FhPInvoke.GetProcAddress(FhPInvoke.GetModuleHandle("D3D11.dll"), "D3DKMTPresent") - FhGlobal.base_addr,
                render_imgui);

        _moduleState  = FhModuleState.InitSuccess;
    }

    public override bool FhModuleInit() {
        return true;
    }

    public override bool FhModuleOnError() {
        return true;
    }

    public override bool FhModuleStart() {
        return _tkIsDbg.hook()
            //&& _printf_22F6B0.hook()
            //&& _printf_22FDA0.hook()
            //&& _printf_473C10.hook()
            //&& _printf_473C20.hook()
            && _main_loop.hook()
            && _update_input.hook()
            && _init_imgui.hook()
            && _render.hook();
    }

    public override bool FhModuleStop() {
        return _tkIsDbg.unhook()
            //&& _printf_22F6B0.unhook()
            //&& _printf_22FDA0.unhook()
            //&& _printf_473C10.unhook()
            //&& _printf_473C20.unhook()
            && _main_loop.unhook()
            && _update_input.unhook()
            && _init_imgui.unhook()
            && _render.unhook();
    }

    public unsafe void init_imgui(
            nint pAdapter,
            nint DriverType,
            nint Software,
            uint Flags,
            nint pFeatureLevels,
            uint FeatureLevels,
            uint SDKVersion,
            nint pSwapChainDesc,
            nint ppSwapChain,
            nint ppDevice,
            nint pFeatureLevel,
            nint ppImmediateContext) {
        if (_init_imgui.try_get_original_fptr(out D3D11CreateDeviceAndSwapChain? fptr)) {
            fptr.Invoke(
                    pAdapter,
                    DriverType,
                    Software,
                    Flags,
                    pFeatureLevels,
                    FeatureLevels,
                    SDKVersion,
                    pSwapChainDesc,
                    ppSwapChain,
                    ppDevice,
                    pFeatureLevel,
                    ppImmediateContext);
        }

        if (initialized_imgui) return;

        pDevice = (void*)*(nint*)ppDevice;
        pContext = (void*)*(nint*)ppImmediateContext;

        ready_to_init_imgui = true;
    }

    public void main_loop(float delta) {
        foreach (FhModule module in FhModuleController.FindAll()) {
            module.pre_update();
        }

        if (_main_loop.try_get_original_fptr(out Sg_MainLoop? fptr)) {
            fptr.Invoke(delta);
        }

        foreach (FhModule module in FhModuleController.FindAll()) {
            module.post_update();
        }

        if (initialized_imgui || !ready_to_init_imgui) return;

        ImGui.CreateContext();
        ImGuiIOPtr io = ImGui.GetIO();

        // Enable controls
        //io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
        //io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;

        ImGui.StyleColorsDark();

        FhLog.Debug($"Initializing ImGui win32 with hwnd {FhPInvoke.FindWindow("FINAL FANTASY X"):X8}...");
        ImGui.ImGui_ImplWin32_Init(FhPInvoke.FindWindow("FINAL FANTASY X"));
        FhLog.Debug("Initializing ImGui DX11...");
        ImGui.ImGui_ImplDX11_Init(pDevice, pContext);

        FhLog.Info("ImGui initialized!");
        FhLog.Debug($"BackendPlatformName: {io.BackendPlatformName}: BackendPlatformUserData: {io.BackendPlatformUserData:X8}");

        initialized_imgui = true;
    }

    public void update_input() {
        CoreLib.FFX.Globals.Input.update();

        if (_update_input.try_get_original_fptr(out AtelExecInternal_00871d10? fptr)) {
            fptr.Invoke();
        }

        foreach (FhModule module in FhModuleController.FindAll()) {
            module.handle_input();
        }
    }

    public nint render_imgui(nint data) {
        foreach (FhModule module in FhModuleController.FindAll()) {
            module.render();
        }

        if (_render.try_get_original_fptr(out D3DKMTPresent? fptr)) {
            return fptr.Invoke(data);
        }

        return 0;
    }
}
