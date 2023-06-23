using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

using Fahrenheit.CoreLib;

namespace Fahrenheit.Modules.RNGFix;

public sealed record RNGFixModuleConfig : FhModuleConfig
{
    [JsonConstructor]
    public RNGFixModuleConfig(string configName,
                              uint   configVersion,
                              bool   configEnabled) : base(configName, configVersion, configEnabled)
    {
    }

    public override bool TrySpawnModule([NotNullWhen(true)] out FhModule? fm)
    {
        fm = new RNGFixModule(this);
        return fm.ModuleState == FhModuleState.InitSuccess;
    }
}

public class RNGFixModule : FhModule
{
    private readonly byte[]             _patchBytes = new byte[] { 0x31, 0xD2, 0x90 };
    private readonly RNGFixModuleConfig _moduleConfig;

    public RNGFixModule(RNGFixModuleConfig moduleConfig) : base(moduleConfig)
    {
        _moduleConfig = moduleConfig;
        _moduleState  = FhModuleState.InitSuccess;
    }

    public override FhModuleConfig ModuleConfiguration
    {
        get { return _moduleConfig; }
    }

    public override bool FhModuleFaultHandler()
    {
        return true;
    }

    public override bool FhModuleInit()
    {
        throw new NotImplementedException();
    }

    public override bool FhModuleStart()
    {
        throw new NotImplementedException();
    }

    public override bool FhModuleStop()
    {
        throw new NotImplementedException();
    }
}
