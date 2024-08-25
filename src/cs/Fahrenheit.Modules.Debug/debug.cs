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

public unsafe partial class DebugModule : FhModule {
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
        init_hooks();
        return true;
    }

    public override bool FhModuleStart() {
        return hook();
    }

    public override bool FhModuleStop() {
        return unhook();
    }

    public override void render() {
        SphereGridEditor.render();
    }

    public override void handle_input() {
        SphereGridEditor.handle_input();
    }
}
