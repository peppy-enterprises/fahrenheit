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
    private FhMethodHandle<Sg_MainLoopDelegate> _postMainLoop;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Sg_MainLoopDelegate(float delta);

    public DebugModule(DebugModuleConfig moduleConfig) : base(moduleConfig) {
        _moduleConfig = moduleConfig;

        _postMainLoop = new FhMethodHandle<Sg_MainLoopDelegate>(this, 0x420c00, render);

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
        return _postMainLoop.ApplyHook();
    }

    public override bool FhModuleStop() {
        return _postMainLoop.RemoveHook();
    }

    public void render(float delta) {
        if (_postMainLoop.GetOriginalFptrSafe(out Sg_MainLoopDelegate? fptr)) {
            fptr.Invoke(0);
        }

        ImGuiNET.ImColor accent_color = new ImGuiNET.ImColor() {
            Value = new System.Numerics.Vector4(0.7f, 0.9f, 0.1f, 0.8f)
        };
        //DebugMenu.render(delta);
    }
}
