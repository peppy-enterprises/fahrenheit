using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using Fahrenheit.CLRHost;
using Fahrenheit.CoreLib;

namespace Fahrenheit.Modules.Debug;

public sealed record DebugModuleConfig : FhModuleConfig {
    [JsonConstructor]
    public DebugModuleConfig(string configName, uint configVersion, bool configEnabled)
                      : base(configName, configVersion, configEnabled) {}

    public override bool TrySpawnModule([NotNullWhen(true)] out FhModule? fm) {
        fm = new DebugModule(this);
        return fm.ModuleState == FhModuleState.InitSuccess;
    }
}

public class DebugModule : FhModule {
    private readonly DebugModuleConfig _moduleConfig;

    public DebugModule(DebugModuleConfig moduleConfig) : base(moduleConfig) {
        _moduleConfig = moduleConfig;

        _moduleState  = FhModuleState.InitSuccess;
    }

    public override FhModuleConfig ModuleConfiguration => _moduleConfig;

    public override bool FhModuleOnError() {
        return true;
    }

    public override bool FhModuleInit() {
        return true;
    }

    public override bool FhModuleStart() {
        return true;
    }

    public override bool FhModuleStop() {
        return true;
    }

    public override void post_update() {
        SphereGridEditor.update();

        //DebugMenu.render(delta);
    }
}
