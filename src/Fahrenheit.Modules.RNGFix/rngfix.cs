using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using Fahrenheit.CLRHost;
using Fahrenheit.CoreLib;

namespace Fahrenheit.Modules.RNGFix;

public sealed record RNGFixModuleConfig : FhModuleConfig {
    [JsonConstructor]
    public RNGFixModuleConfig(string configName,
                              uint   configVersion,
                              bool   configEnabled) : base(configName, configVersion, configEnabled) {
    }

    public override bool TrySpawnModule([NotNullWhen(true)] out FhModule? fm) {
        fm = new RNGFixModule(this);
        return fm.ModuleState == FhModuleState.InitSuccess;
    }
}

public class RNGFixModule : FhModule {
    private readonly RNGFixModuleConfig _moduleConfig;
    private FhMethodHandle<brndDelegate> _brnd;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate nint brndDelegate(nint rng_seed_idx);

    public RNGFixModule(RNGFixModuleConfig moduleConfig) : base(moduleConfig) {
        _moduleConfig = moduleConfig;

        _brnd = new FhMethodHandle<brndDelegate>(this, 0x398900, brndRngFix);

        _moduleState  = FhModuleState.InitSuccess;
    }

    public override FhModuleConfig ModuleConfiguration {
        get { return _moduleConfig; }
    }

    public override bool FhModuleOnError() {
        return true;
    }

    public override bool FhModuleInit() {
        return true;
    }

    public override bool FhModuleStart() {
        return _brnd.hook();
    }

    public override bool FhModuleStop() {
        return _brnd.unhook();
    }

    public nint brndRngFix(nint rng_seed_idx) {
        if (_brnd.try_get_original_fptr(out brndDelegate? fptr)) {
            return fptr.Invoke(0);
        }

        //FhLog.Error("Failed to call original of brnd, supplying own rng instead.");
        return new Random().Next();
    }
}
