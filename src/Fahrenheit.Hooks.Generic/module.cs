using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using ImGuiNET;

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
    public delegate void TODrawMessageWindow();

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void AtelExecInternal_00871d10();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate nint WndProcDelegate(nint hWnd, uint msg, nint wParam, nint lParam);

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

    private readonly FhHooksBaseModuleConfig           _moduleConfig;

    private readonly FhMethodHandle<TkIsDebugDelegate>    _tkIsDbg;
    private readonly FhMethodHandle<PrintfVarargDelegate> _printf_22F6B0;
    private readonly FhMethodHandle<PrintfVarargDelegate> _printf_22FDA0;
    private readonly FhMethodHandle<PrintfVarargDelegate> _printf_473C10;
    private readonly FhMethodHandle<PrintfVarargDelegate> _printf_473C20;
    private readonly FhMethodHandle<Sg_MainLoop> _main_loop;
    private readonly FhMethodHandle<AtelExecInternal_00871d10> _update_input;
    private readonly FhMethodHandle<TODrawMessageWindow> _render_game; // best I can do atm - Evelyn
    private readonly FhMethodHandle<D3D11CreateDeviceAndSwapChain> _prep_init_imgui;

    private nint                                 _o_WndProcPtr;
    private WndProcDelegate?                     _h_WndProc;
    private nint                                 _h_WndProcPtr;
    private bool                                 _wndProc_init;

    public override FhHooksBaseModuleConfig ModuleConfig => _moduleConfig;

    private static bool hooked_wndproc = false;
    private static bool ready_to_init_imgui = false;
    private static bool initialized_imgui = false;
    private static void* pDevice;
    private static void* pContext;

    public FhHooksBaseModule(FhHooksBaseModuleConfig cfg) : base(cfg) {
        _moduleConfig = cfg;

        const string game = "FFX.exe";

        // Debug things that really should be in Modules.Debug instead
        _tkIsDbg       = new FhMethodHandle<TkIsDebugDelegate>(this, game, 0x487C80, TkIsDebugHook);
        _printf_22F6B0 = new FhMethodHandle<PrintfVarargDelegate>(this, game, 0x22F6B0, FhHooks.CLRPrintfHookAnsi);
        _printf_22FDA0 = new FhMethodHandle<PrintfVarargDelegate>(this, game, 0x22FDA0, FhHooks.CLRPrintfHookAnsi);
        _printf_473C10 = new FhMethodHandle<PrintfVarargDelegate>(this, game, 0x473C10, FhHooks.CLRPrintfHookAnsi);
        _printf_473C20 = new FhMethodHandle<PrintfVarargDelegate>(this, game, 0x473C20, FhHooks.CLRPrintfHookAnsi);

        // Providing basic functionality to other modules
        _main_loop = new FhMethodHandle<Sg_MainLoop>(this, game, 0x420C00, main_loop);

        _update_input = new FhMethodHandle<AtelExecInternal_00871d10>(this, game, 0x471D10, update_input);
        _prep_init_imgui = new FhMethodHandle<D3D11CreateDeviceAndSwapChain>(this, "D3D11.dll", "D3D11CreateDeviceAndSwapChain", prep_init_imgui);

        _render_game = new(this, game, 0x4abce0, render_game);

        _moduleState  = FhModuleState.InitSuccess;
    }

    public override bool FhModuleStart() {
        return _tkIsDbg.hook()
            //&& _printf_22F6B0.hook()
            //&& _printf_22FDA0.hook()
            //&& _printf_473C10.hook()
            //&& _printf_473C20.hook()
            && _main_loop.hook()
            && _update_input.hook()
            && _render_game.hook()
            //&& _prep_init_imgui.hook()
            ;//&& _render.hook();
    }

    public nint h_wndproc(nint hWnd, uint msg, nint wParam, nint lParam) {
        if (_o_WndProcPtr == 0) throw new Exception("Original WndProcPtr is null");

        if (FhPInvoke.ImGui_ImplWin32_WndProcHandler(hWnd, msg, wParam, lParam) == 1) {
            return 1;
        }

        //FhLog.Debug($"WndProc({hWnd:X8}, {msg}, {wParam:X8}, {lParam:X8})");

        return FhPInvoke.CallWindowProc(_o_WndProcPtr, hWnd, msg, wParam, lParam);
    }

    private bool hook_wndproc() {
        _h_WndProcPtr = Marshal.GetFunctionPointerForDelegate(h_wndproc);
        nint hWnd = FhPInvoke.FindWindow(null, "FINAL FANTASY X"); // shitfuck hack

        if (hWnd == 0) {
            FhLog.Error("Failed in FindWindow.");
            return false;
        }

        _o_WndProcPtr = FhPInvoke.GetWindowLong(hWnd, FhPInvoke.GWLP_WNDPROC);
        FhPInvoke.SetWindowLongPtr(hWnd, FhPInvoke.GWLP_WNDPROC, _h_WndProcPtr);

        if (_o_WndProcPtr == 0) {
            FhLog.Error("Failed in GetWindowLongPtr.");
            return false;
        }

        hooked_wndproc = true;
        return true;
    }

    public unsafe void prep_init_imgui(
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
        if (_prep_init_imgui.try_get_original_fptr(out D3D11CreateDeviceAndSwapChain? fptr)) {
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

    private void init_imgui() {
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

    public void main_loop(float delta) {
        //if (!hooked_wndproc)
        //    hook_wndproc();

        if (!initialized_imgui && ready_to_init_imgui)
            init_imgui();

        foreach (FhModule module in FhModuleController.FindAll())
            module.pre_update();

        _main_loop.original(delta);

        foreach (FhModule module in FhModuleController.FindAll())
            module.post_update();
    }

    public void update_input() {
        CoreLib.FFX.Globals.Input.update();

        _update_input.original();

        foreach (FhModule module in FhModuleController.FindAll()) {
            module.handle_input();
        }
    }

    public new nint render_imgui() {
        foreach (FhModule module in FhModuleController.FindAll()) {
            module.render_imgui();
        }

        /*if (_render.try_get_original_fptr(out WIP? fptr)) {
            return fptr.Invoke();
        }*/

        return 0;
    }

    public new void render_game() {
        _render_game.original();

        foreach (FhModule module in FhModuleController.FindAll()) {
            module.render_game();
        }
    }
}
