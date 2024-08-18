using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
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

public partial class FhHooksBaseModule : FhModule {
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Sg_MainLoop(float delta);

    private readonly FhHooksBaseModuleConfig           _moduleConfig;

    private readonly FhMethodHandle<TkIsDebugDelegate>    _tkIsDbg;
    private readonly FhMethodHandle<PrintfVarargDelegate> _printf_22F6B0;
    private readonly FhMethodHandle<PrintfVarargDelegate> _printf_22FDA0;
    private readonly FhMethodHandle<PrintfVarargDelegate> _printf_473C10;
    private readonly FhMethodHandle<PrintfVarargDelegate> _printf_473C20;
    private readonly FhMethodHandle<Sg_MainLoop> _mainLoop;

    public override FhHooksBaseModuleConfig ModuleConfiguration => _moduleConfig;

    public FhHooksBaseModule(FhHooksBaseModuleConfig cfg) : base(cfg) {
        _moduleConfig = cfg;

        _tkIsDbg       = new FhMethodHandle<TkIsDebugDelegate>(this, 0x487C80, TkIsDebugHook);
        _printf_22F6B0 = new FhMethodHandle<PrintfVarargDelegate>(this, 0x22F6B0, FhHooks.CLRPrintfHookAnsi);
        _printf_22FDA0 = new FhMethodHandle<PrintfVarargDelegate>(this, 0x22FDA0, FhHooks.CLRPrintfHookAnsi);
        _printf_473C10 = new FhMethodHandle<PrintfVarargDelegate>(this, 0x473C10, FhHooks.CLRPrintfHookAnsi);
        _printf_473C20 = new FhMethodHandle<PrintfVarargDelegate>(this, 0x473C20, FhHooks.CLRPrintfHookAnsi);
        _mainLoop = new FhMethodHandle<Sg_MainLoop>(this, 0x420C00, main_loop);

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
            && _mainLoop.hook();
    }

    public override bool FhModuleStop() {
        return _tkIsDbg.unhook()
            //&& _printf_22F6B0.unhook()
            //&& _printf_22FDA0.unhook()
            //&& _printf_473C10.unhook()
            //&& _printf_473C20.unhook()
            && _mainLoop.unhook();
    }

    public void main_loop(float delta) {
        foreach (FhModule module in FhModuleController.FindAll()) {
            module.pre_update();
        }

        CoreLib.FFX.Globals.Input.update();

        if (_mainLoop.try_get_original_fptr(out Sg_MainLoop? handle)) {
            handle.Invoke(delta);
        }

        foreach (FhModule module in FhModuleController.FindAll()) {
            module.post_update();
        }
    }
}
