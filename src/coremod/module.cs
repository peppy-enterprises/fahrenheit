using Fahrenheit.CoreLib;

using static Fahrenheit.CoreLib.FhHookDelegates;

namespace Fahrenheit.Modules.Core;

public sealed record FhCoreModuleConfig : FhModuleConfig {
    public FhCoreModuleConfig(string configName, bool configEnabled) : base(configName, configEnabled) { }

    public override FhModule SpawnModule() {
        return new FhCoreModule(this);
    }
}

public unsafe class FhCoreModule : FhModule {
    private readonly FhCoreModuleConfig                        _moduleConfig;

    private readonly FhMethodHandle<Sg_MainLoop>               _main_loop;
    private readonly FhMethodHandle<AtelExecInternal_00871d10> _update_input;
    private readonly FhMethodHandle<TODrawMessageWindow>       _render_game;

    public FhCoreModule(FhCoreModuleConfig cfg) : base(cfg) {
        _moduleConfig = cfg;

        _main_loop    = new(this, "FFX.exe", main_loop,    offset: 0x420C00);
        _update_input = new(this, "FFX.exe", update_input, offset: 0x471d10);
        _render_game  = new(this, "FFX.exe", render_game,  offset: 0x4abce0);
    }

    public override bool init() {
        return _main_loop   .hook()
            && _update_input.hook()
            && _render_game .hook();
    }

    public void main_loop(float delta) {
        foreach (FhModuleContext fmctx in FhModuleController.find_all()) {
            fmctx.Module.pre_update();
        }

        _main_loop.orig_fptr(delta);

        foreach (FhModuleContext fmctx in FhModuleController.find_all()) {
            fmctx.Module.post_update();
        }

        foreach (nint addr in FhPointer.get_pending_wait_addresses()) {
            PInvoke.WakeByAddressAll(addr.ToPointer());
        }
    }

    public void update_input() {
        CoreLib.FFX.Globals.Input.update();

        _update_input.orig_fptr();

        foreach (FhModuleContext fmctx in FhModuleController.find_all()) {
            fmctx.Module.handle_input();
        }
    }

    public new void render_game() {
        _render_game.orig_fptr();

        foreach (FhModuleContext fmctx in FhModuleController.find_all()) {
            fmctx.Module.render_game();
        }
    }
}