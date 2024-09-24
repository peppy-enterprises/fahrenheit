using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Fahrenheit.CoreLib;

//TODO: Add ImGui dependency
//using ImGuiNET;

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

    private readonly FhMethodHandle<Sg_MainLoop>          _main_loop;
    private readonly FhMethodHandle<AtelExecInternal_00871d10> _update_input;
    private readonly FhMethodHandle<TODrawMessageWindow> _render_game;
    //TODO: Add ImGui rendering
    //private readonly FhMethodHandle<???> _render_imgui;
    //TODO: Add ImGui dependency
    //private readonly FhMethodHandle<D3D11CreateDeviceAndSwapChain> _prep_init_imgui;

    //TODO: Move these to debug
    //private readonly FhMethodHandle<PrintfVarargDelegate> _printf_22F6B0;
    //private readonly FhMethodHandle<PrintfVarargDelegate> _printf_22FDA0;
    //private readonly FhMethodHandle<PrintfVarargDelegate> _printf_473C10;
    //private readonly FhMethodHandle<PrintfVarargDelegate> _printf_473C20;

    private          nint                                 _o_WndProcPtr;
    private readonly WndProcDelegate                      _h_WndProc;
    private          nint                                 _h_WndProcPtr;

    private bool hooked_wndproc = false;
    //TODO: Add ImGui dependency
    //private bool ready_to_init_imgui = false;
    //private bool initialized_imgui = false;
    //private void* pDevice;
    //private void* pContext;

    public FhCoreModule(FhCoreModuleConfig cfg) : base(cfg) {
        _moduleConfig  = cfg;

        const string game = "FFX.exe";

        //TODO: Move these to Modules.Debug
        //_printf_22F6B0 = new (this, "FFX.exe", h_printf_ansi, offset: 0x22F6B0);
        //_printf_22FDA0 = new (this, "FFX.exe", h_printf_ansi, offset: 0x22FDA0);
        //_printf_473C10 = new (this, "FFX.exe", h_printf_ansi, offset: 0x473C10);
        //_printf_473C20 = new (this, "FFX.exe", h_printf_ansi, offset: 0x473C20);

        _h_WndProc = new (h_wndproc);
        //TODO: Add ImGui dependency
        //_prep_init_imgui = new (this, "D3D11.dll", prep_init_imgui, fn_name: "D3D11CreateDeviceAndSwapChain");

        _main_loop =     new (this, game, main_loop, offset: 0x420C00);
        _update_input = new (this, game, update_input, offset: 0x420C00);
        _render_game =  new(this, game, render_game, offset: 0x4abce0);

        _moduleState   = FhModuleState.InitSuccess;
    }

    public override bool FhModuleInit() {
        //TODO: Add ImGui dependency
        return //_prep_init_imgui.hook()
               _main_loop.hook()
            && _update_input.hook()
            && _render_game.hook();
    }

    public void main_loop(float delta) {
        if (!hooked_wndproc)
            try_hook_wndproc();

        //TODO: Add ImGui dependency
        //if (!initialized_imgui && ready_to_init_imgui)
        //    init_imgui();

        foreach (FhModule module in FhModuleController.find_all().Select(context => context.Module)) {
            module.pre_update();
        }

        _main_loop.orig_fptr(delta);

        foreach (FhModule module in FhModuleController.find_all().Select(context => context.Module)) {
            module.post_update();
        }

        foreach (nint addr in FhPointer.get_pending_wait_addresses()) {
            PInvoke.WakeByAddressAll(addr.ToPointer());
        }
    }

    public void update_input() {
        CoreLib.FFX.Globals.Input.update();

        _update_input.orig_fptr();

        foreach (FhModule module in FhModuleController.find_all().Select(context => context.Module)) {
            module.handle_input();
        }
    }

    public new nint render_imgui() {
        foreach (FhModule module in FhModuleController.find_all().Select(context => context.Module)) {
            module.render_imgui();
        }

        //_render_imgui.orig_fptr();

        return 0;
    }

    public new void render_game() {
        _render_game.orig_fptr();

        foreach (FhModule module in FhModuleController.find_all().Select(context => context.Module)) {
            module.render_game();
        }
    }

    //TODO: Add ImGui dependency
    //public void prep_init_imgui(
    //        nint pAdapter,
    //        nint DriverType,
    //        nint Software,
    //        uint Flags,
    //        nint pFeatureLevels,
    //        uint FeatureLevels,
    //        uint SDKVersion,
    //        nint pSwapChainDesc,
    //        nint ppSwapChain,
    //        nint ppDevice,
    //        nint pFeatureLevel,
    //        nint ppImmediateContext) {
    //    _prep_init_imgui.orig_fptr(
    //            pAdapter,
    //            DriverType,
    //            Software,
    //            Flags,
    //            pFeatureLevels,
    //            FeatureLevels,
    //            SDKVersion,
    //            pSwapChainDesc,
    //            ppSwapChain,
    //            ppDevice,
    //            pFeatureLevel,
    //            ppImmediateContext);

    //    if (initialized_imgui) return;

    //    pDevice = (void*)*(nint*)ppDevice;
    //    pContext = (void*)*(nint*)ppImmediateContext;

    //    ready_to_init_imgui = true;
    //}

    //TODO: Add ImGui dependency
    //private void init_imgui() {
    //    ImGui.CreateContext();
    //    ImGuiIOPtr io = ImGui.GetIO();

    //    // Enable controls
    //    //io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
    //    //io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;

    //    ImGui.StyleColorsDark();

    //    FhLog.Debug($"Initializing ImGui win32 with hwnd {PInvoke.FindWindow("FINAL FANTASY X"):X8}...");
    //    ImGui.ImGui_ImplWin32_Init(PInvoke.FindWindow("FINAL FANTASY X"));
    //    FhLog.Debug("Initializing ImGui DX11...");
    //    ImGui.ImGui_ImplDX11_Init(pDevice, pContext);

    //    FhLog.Info("ImGui initialized!");
    //    FhLog.Debug($"BackendPlatformName: {io.BackendPlatformName}: BackendPlatformUserData: {io.BackendPlatformUserData:X8}");

    //    initialized_imgui = true;
    //}

    public nint h_wndproc(nint hWnd, uint msg, nint wParam, nint lParam) {
        if (_o_WndProcPtr == 0) throw new Exception("FH_E_OWNDPROC_NUL");

        if (PInvoke.ImGui_ImplWin32_WndProcHandler(hWnd, msg, wParam, lParam) == 1) {
            return 1;
        }

        return PInvoke.CallWindowProcW(_o_WndProcPtr, hWnd, msg, wParam, lParam);
    }

    private bool try_hook_wndproc() {
        _h_WndProcPtr = Marshal.GetFunctionPointerForDelegate(_h_WndProc);
        nint hWnd     = PInvoke.FindWindow(null, "FINAL FANTASY X"); // shitfuck hack

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

    //TODO: Move this to Modules.Debug
    //[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    //public static void h_printf_ansi(string fmt, nint va0, nint va1, nint va2, nint va3, nint va4, nint va5, nint va6, nint va7, nint va8, nint va9, nint va10, nint va11, nint va12, nint va13, nint va14, nint va15) {
    //    int argc = 0;
    //    fmt = fmt.Trim();

    //    for (int i = 0; i < fmt.Length; i++) if (fmt[i] == '%') argc++;

    //    int bl = argc switch {
    //        0  => FhPInvoke._scprintf(fmt, __arglist()),
    //        1  => FhPInvoke._scprintf(fmt, __arglist(va0)),
    //        2  => FhPInvoke._scprintf(fmt, __arglist(va0, va1)),
    //        3  => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2)),
    //        4  => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3)),
    //        5  => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4)),
    //        6  => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5)),
    //        7  => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6)),
    //        8  => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7)),
    //        9  => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8)),
    //        10 => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9)),
    //        11 => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10)),
    //        12 => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11)),
    //        13 => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12)),
    //        14 => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12, va13)),
    //        15 => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12, va13, va14)),
    //        16 => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12, va13, va14, va15)),
    //        _  => throw new Exception("FH_E_PFHOOK_RAH_OVERREACH")
    //    };

    //    nint buf = Marshal.AllocHGlobal(bl + 1);

    //    try {
    //        int rv = argc switch {
    //            0  => FhPInvoke.sprintf(buf, fmt, __arglist()),
    //            1  => FhPInvoke.sprintf(buf, fmt, __arglist(va0)),
    //            2  => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1)),
    //            3  => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2)),
    //            4  => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3)),
    //            5  => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4)),
    //            6  => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5)),
    //            7  => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6)),
    //            8  => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7)),
    //            9  => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8)),
    //            10 => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9)),
    //            11 => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10)),
    //            12 => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11)),
    //            13 => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12)),
    //            14 => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12, va13)),
    //            15 => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12, va13, va14)),
    //            16 => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12, va13, va14, va15)),
    //            _  => throw new Exception("FH_E_PFHOOK_RAH_OVERREACH")
    //        };

    //        FhLog.Info(Marshal.PtrToStringAnsi(buf) ?? "FH_E_PFHOOK_STRING_NUL");
    //    }
    //    finally {
    //        Marshal.FreeHGlobal(buf);
    //    }
    //}
}