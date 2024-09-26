using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Fahrenheit.CoreLib;

using ImGuiNET;

using static Fahrenheit.CoreLib.FhHookDelegates;

namespace Fahrenheit.Modules.Core;

public sealed record FhCoreModuleConfig : FhModuleConfig {
    public FhCoreModuleConfig(string configName, bool configEnabled) : base(configName, configEnabled) { }

    public override bool TrySpawnModule([NotNullWhen(true)] out FhModule? fm) {
        fm = new FhCoreModule(this);
        return fm.ModuleState == FhModuleState.InitSuccess;
    }
}

public unsafe class FhCoreModule : FhModule {
    private readonly FhCoreModuleConfig                   _moduleConfig;

    private readonly FhMethodHandle<Sg_MainLoop>               _main_loop;
    private readonly FhMethodHandle<AtelExecInternal_00871d10> _update_input;
    private readonly FhMethodHandle<TODrawMessageWindow>       _render_game;
    //TODO: Add ImGui rendering
    //private readonly FhMethodHandle<???> _render_imgui;
    private readonly FhMethodHandle<PInvoke.D3D11CreateDeviceAndSwapChain> _prep_init_imgui;

    private          nint                                 _o_WndProcPtr;
    private readonly PInvoke.WndProcDelegate              _h_WndProc;
    private          nint                                 _h_WndProcPtr;

    private bool ready_to_init_imgui = false;
    private bool initialized_imgui = false;
    private nint* pDevice;
    private nint* pContext;

    public FhCoreModule(FhCoreModuleConfig cfg) : base(cfg) {
        _moduleConfig  = cfg;

        const string game = "FFX.exe";

        _h_WndProc = new(h_wndproc);
        _prep_init_imgui = new (this, "D3D11.dll", prep_init_imgui, fn_name: "D3D11CreateDeviceAndSwapChain");

        _main_loop    = new(this, game, main_loop,    offset: 0x420C00);
        _update_input = new(this, game, update_input, offset: 0x471d10);
        _render_game  = new(this, game, render_game,  offset: 0x4abce0);

        _moduleState = FhModuleState.InitSuccess;
    }

    public override bool FhModuleInit() {
        return _prep_init_imgui.hook()
            && _main_loop.hook()
            && _update_input.hook()
            && _render_game.hook();
    }

    public void main_loop(float delta) {
        if (!initialized_imgui && ready_to_init_imgui) {
            init_imgui();
            try_hook_wndproc();
        }

        foreach (FhModuleContext fmctx in FhModuleController.find_all()) {
            fmctx.Module.pre_update();
        }

        _main_loop.orig_fptr(delta);

        foreach (FhModuleContext fmctx in FhModuleController.find_all()) {
            fmctx.Module.post_update();
        }

        foreach (nint addr in FhPointer.get_pending_wait_addresses()) {
            PInvoke.WakeByAddressAll(addr.ToPointer());
        }
    }

    public void update_input() {
        CoreLib.FFX.Globals.Input.update();

        _update_input.orig_fptr();

        foreach (FhModuleContext fmctx in FhModuleController.find_all()) {
            fmctx.Module.handle_input();
        }
    }

    public new nint render_imgui() {
        foreach (FhModuleContext fmctx in FhModuleController.find_all()) {
            fmctx.Module.render_imgui();
        }

        //_render_imgui.orig_fptr();
        return 0;
    }

    public new void render_game() {
        _render_game.orig_fptr();

        foreach (FhModuleContext fmctx in FhModuleController.find_all()) {
            fmctx.Module.render_game();
        }
    }

    public nint prep_init_imgui(
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
        nint result = _prep_init_imgui.orig_fptr(
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

        if (initialized_imgui || result != 0)
            return result;

        pDevice = *(nint**)ppDevice;
        pContext = *(nint**)ppImmediateContext;

        ready_to_init_imgui = true;
        return result;
    }

    private void init_imgui() {
        ImGui.CreateContext();
        ImGuiIOPtr io = ImGui.GetIO();

        // Enable controls
        //io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
        //io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;

        ImGui.StyleColorsDark();

        FhLog.Debug($"Initializing ImGui win32 with hwnd {PInvoke.FindWindow(null, "FINAL FANTASY X"):X8}...");
        ImGui.ImGui_ImplWin32_Init(PInvoke.FindWindow(null, "FINAL FANTASY X"));
        FhLog.Debug("Initializing ImGui DX11...");
        ImGui.ImGui_ImplDX11_Init(pDevice, pContext);

        FhLog.Info("ImGui initialized!");
        FhLog.Debug($"BackendPlatformName: {io.BackendPlatformName}: BackendPlatformUserData: {io.BackendPlatformUserData:X8}");

        initialized_imgui = true;
    }

    public nint h_wndproc(nint hWnd, uint msg, nint wParam, nint lParam) {
        if (_o_WndProcPtr == 0) throw new Exception("FH_E_OWNDPROC_NUL");

        if (PInvoke.ImGui_ImplWin32_WndProcHandler(hWnd, msg, wParam, lParam) == 1) {
            return 1;
        }

        //FhLog.Debug($"WndProc [ hWnd: {hWnd:X8}h, msg: {msg:X4}h, wParam: {wParam:X4}h, lParam: {lParam:X4}h ]");

        return PInvoke.CallWindowProcW(_o_WndProcPtr, hWnd, msg, wParam, lParam);
    }

    private bool try_hook_wndproc() {
        _h_WndProcPtr = Marshal.GetFunctionPointerForDelegate(_h_WndProc);
        nint hWnd     = PInvoke.FindWindow(null, "FINAL FANTASY X");

        if (hWnd == 0) {
            FhLog.Error("Could not find Final Fantasy X window");
            return false;
        }

        _o_WndProcPtr = PInvoke.GetWindowLongA(hWnd, PInvoke.GWLP_WNDPROC);
        PInvoke.SetWindowLongA(hWnd, PInvoke.GWLP_WNDPROC, _h_WndProcPtr);

        if (_o_WndProcPtr == 0) {
            FhLog.Error("Failed in GetWindowLongA.");
            return false;
        }

        return true;
    }
}