using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Fahrenheit.CoreLib;

namespace Fahrenheit.Modules.Debug;

public sealed record DebugModuleConfig : FhModuleConfig {
    [JsonConstructor]
    public DebugModuleConfig(string configName, bool configEnabled) : base(configName, configEnabled) { }

    public override bool TrySpawnModule([NotNullWhen(true)] out FhModule? fm) {
        fm = new DebugModule(this);
        return fm.ModuleState == FhModuleState.InitSuccess;
    }
}

public unsafe partial class DebugModule : FhModule {
    private readonly DebugModuleConfig _moduleConfig;

    public DebugModule(DebugModuleConfig moduleConfig) : base(moduleConfig) {
        _moduleConfig = moduleConfig;

        init_hooks();

        _moduleState  = FhModuleState.InitSuccess;
    }

    public override bool FhModuleInit() {
        return hook();
    }

    public override void render_game() {
        SphereGridEditor.render();
    }

    public override void handle_input() {
        SphereGridEditor.handle_input();
    }
}
