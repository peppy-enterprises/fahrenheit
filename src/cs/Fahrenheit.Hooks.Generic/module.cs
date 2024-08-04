using System.Diagnostics.CodeAnalysis;

using Fahrenheit.CLRHost;
using Fahrenheit.CoreLib;

using static Fahrenheit.CoreLib.FhHookDelegates;

namespace Fahrenheit.Hooks.Generic;

public sealed record FhHooksExampleModuleConfig : FhModuleConfig
{
    public FhHooksExampleModuleConfig(string configName,
                                      uint   configVersion,
                                      bool   configEnabled) : base(configName, configVersion, configEnabled)
    {
    }

    public override bool TrySpawnModule([NotNullWhen(true)] out FhModule? fm)
    {
        fm = new FhHooksExampleModule(this);
        return fm.ModuleState == FhModuleState.InitSuccess;
    }
}

public partial class FhHooksExampleModule : FhModule
{
    private readonly FhHooksExampleModuleConfig           _moduleConfig;
    private readonly FhMethodHandle<TkIsDebugDelegate>    _tkIsDbg;
    private readonly FhMethodHandle<PrintfVarargDelegate> _printf_22F6B0;
    private readonly FhMethodHandle<PrintfVarargDelegate> _printf_22FDA0;
    private readonly FhMethodHandle<PrintfVarargDelegate> _printf_473C10;
    private readonly FhMethodHandle<PrintfVarargDelegate> _printf_473C20;

    public FhHooksExampleModule(FhHooksExampleModuleConfig cfg) : base(cfg)
    {
        _moduleConfig = cfg;
        _moduleState  = FhModuleState.InitSuccess;

        _tkIsDbg       = new FhMethodHandle<TkIsDebugDelegate>(this, 0x487C80, new TkIsDebugDelegate(TkIsDebugHook));
        _printf_22F6B0 = new FhMethodHandle<PrintfVarargDelegate>(this, 0x22F6B0, new PrintfVarargDelegate(FhHooks.CLRPrintfHookAnsi));
        _printf_22FDA0 = new FhMethodHandle<PrintfVarargDelegate>(this, 0x22FDA0, new PrintfVarargDelegate(FhHooks.CLRPrintfHookAnsi));
        _printf_473C10 = new FhMethodHandle<PrintfVarargDelegate>(this, 0x473C10, new PrintfVarargDelegate(FhHooks.CLRPrintfHookAnsi));
        _printf_473C20 = new FhMethodHandle<PrintfVarargDelegate>(this, 0x473C20, new PrintfVarargDelegate(FhHooks.CLRPrintfHookAnsi));
    }

    public override FhHooksExampleModuleConfig ModuleConfiguration
    {
        get { return _moduleConfig; }
    }

    public override bool FhModuleInit()
    {
        return true;
    }

    public override bool FhModuleOnError()
    {
        return true;
    }

    public override bool FhModuleStart()
    {
        return _tkIsDbg.hook() && 
               _printf_22F6B0.hook() && 
               _printf_22FDA0.hook() && 
               _printf_473C10.hook() && 
               _printf_473C20.hook();
    }

    public override bool FhModuleStop()
    {
        return true;
    }
}
