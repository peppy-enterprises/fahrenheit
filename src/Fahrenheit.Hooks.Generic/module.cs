using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Fahrenheit.CLRHost;
using Fahrenheit.CoreLib;

using static Fahrenheit.CoreLib.FhHookDelegates;

namespace Fahrenheit.Hooks.Generic;

public sealed record FhHooksExampleModuleConfig : FhModuleConfig {
    public FhHooksExampleModuleConfig(string configName,
                                      uint   configVersion,
                                      bool   configEnabled) : base(configName, configVersion, configEnabled) {
    }

    public override bool TrySpawnModule([NotNullWhen(true)] out FhModule? fm) {
        fm = new FhHooksExampleModule(this);
        return fm.ModuleState == FhModuleState.InitSuccess;
    }
}

public partial class FhHooksExampleModule : FhModule {
    private readonly FhHooksExampleModuleConfig           _moduleConfig;
    private readonly FhMethodHandle<TkIsDebugDelegate>    _tkIsDbg;
    private readonly FhMethodHandle<PrintfVarargDelegate> _printf_22F6B0;
    private readonly FhMethodHandle<PrintfVarargDelegate> _printf_22FDA0;
    private readonly FhMethodHandle<PrintfVarargDelegate> _printf_473C10;
    private readonly FhMethodHandle<PrintfVarargDelegate> _printf_473C20;
    private readonly FhMethodHandle<Sg_MainLoop>          _mainLoop;
    private          nint                                 _o_WndProcPtr;
    private          WndProcDelegate?                     _h_WndProc;
    private          nint                                 _h_WndProcPtr;
    private          bool                                 _wndProc_init;

    public FhHooksExampleModule(FhHooksExampleModuleConfig cfg) : base(cfg) {
        _moduleConfig = cfg;
        _moduleState  = FhModuleState.InitSuccess;

        _tkIsDbg       = new FhMethodHandle<TkIsDebugDelegate>   (this, "FFX.exe", 0x487C80, new TkIsDebugDelegate   (TkIsDebugHook));
        _printf_22F6B0 = new FhMethodHandle<PrintfVarargDelegate>(this, "FFX.exe", 0x22F6B0, new PrintfVarargDelegate(FhHooks.CLRPrintfHookAnsi));
        _printf_22FDA0 = new FhMethodHandle<PrintfVarargDelegate>(this, "FFX.exe", 0x22FDA0, new PrintfVarargDelegate(FhHooks.CLRPrintfHookAnsi));
        _printf_473C10 = new FhMethodHandle<PrintfVarargDelegate>(this, "FFX.exe", 0x473C10, new PrintfVarargDelegate(FhHooks.CLRPrintfHookAnsi));
        _printf_473C20 = new FhMethodHandle<PrintfVarargDelegate>(this, "FFX.exe", 0x473C20, new PrintfVarargDelegate(FhHooks.CLRPrintfHookAnsi));
        _mainLoop      = new FhMethodHandle<Sg_MainLoop>         (this, "FFX.exe", 0x420C00, new Sg_MainLoop         (main_loop));
    }

    public override FhHooksExampleModuleConfig ModuleConfiguration {
        get { return _moduleConfig; }
    }

    public override bool FhModuleInit() {
        return true;
    }

    public override bool FhModuleOnError() {
        return true;
    }

    public override bool FhModuleStart() {
        return _tkIsDbg      .try_hook() &&
               _printf_22F6B0.try_hook() &&
               _printf_22FDA0.try_hook() &&
               _printf_473C10.try_hook() &&
               _printf_473C20.try_hook() &&
               _mainLoop     .try_hook();
    }

    public override bool FhModuleStop() {
        return true;
    }

    public nint h_wndproc(nint hWnd, uint msg, nint wParam, nint lParam) {
        if (_o_WndProcPtr == 0) throw new Exception("FH_E_OWNDPROC_NUL");

        FhLog.Log(LogLevel.Warning, $"h_wndproc: hwnd {hWnd} msg {msg} wparam {wParam} lparam {lParam}");
        return FhPInvoke.CallWindowProc(_o_WndProcPtr, hWnd, msg, wParam, lParam);
    }

    private bool try_hook_wndproc() {
        _h_WndProcPtr = Marshal.GetFunctionPointerForDelegate(new WndProcDelegate(h_wndproc));
        nint hWnd     = FhPInvoke.FindWindow(null, "FINAL FANTASY X"); // shitfuck hack

        if (hWnd == 0) {
            FhLog.Log(LogLevel.Error, "Failed in FindWindow.");
            return false;
        }

        _o_WndProcPtr = FhPInvoke.GetWindowLong(hWnd, FhPInvoke.GWLP_WNDPROC);
        FhPInvoke.SetWindowLongPtr(hWnd, FhPInvoke.GWLP_WNDPROC, _h_WndProcPtr);

        if (_o_WndProcPtr == 0) {
            FhLog.Log(LogLevel.Error, "Failed in GetWindowLongPtr.");
            return false;
        }

        return true;
    }

    public void main_loop(float delta) {
        if (_mainLoop.try_get_original_fn(out Sg_MainLoop? handle)) {
            handle.Invoke(delta);
        }

        if (_wndProc_init) return;
        bool rv = try_hook_wndproc();
        _wndProc_init = true;
    }
}
