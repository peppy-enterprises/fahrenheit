﻿using System.Runtime.InteropServices;

using Fahrenheit.Core.ImGuiNET;

using static Fahrenheit.Core.FhHookDelegates;

namespace Fahrenheit.Core.Runtime;

public sealed record FhCoreModuleConfig : FhModuleConfig {
    public FhCoreModuleConfig(string name) : base(name) { }

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
        FFX.Globals.Input.update();

        _update_input.orig_fptr();

        foreach (FhModuleContext fmctx in FhModuleController.find_all()) {
            fmctx.Module.handle_input();
        }
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void TOMkpCrossExtMesFontLClutTypeRGBA(
            uint p1,
            byte *text,
            float x, float y,
            byte color,
            byte p6,
            byte tint_r, byte tint_g, byte tint_b, byte tint_a,
            float scale,
            float _);

    public static void draw_text_rgba(
        byte[] text,
        float x, float y,
        byte color,
        float scale
    ) {
        fixed (byte *text_ = text)
            FhUtil.get_fptr<TOMkpCrossExtMesFontLClutTypeRGBA>(0x501700)
                (0, text_, x, y, color, 0, 0x80, 0x80, 0x80, 0x80, scale, 0);
    }

    public new void render_game() {
        _render_game.orig_fptr();

        foreach (FhModuleContext fmctx in FhModuleController.find_all()) {
            fmctx.Module.render_game();
        }

        // In the main menu...
        if (*FFX.Globals.event_id == 0x17) {
            // render some text so that people can't easily hide their use of Fahrenheit
            string text = $"Fahrenheit v{typeof(FhGlobal).Assembly.GetName().Version}";
            draw_text_rgba(FhCharset.Us.to_bytes(text), 5f, 400f, 0x00, 0.65f);
        }
    }

    public override void render_imgui() {
        // In the main menu
        if (*FFX.Globals.event_id == 0x17) {
            // Create a window for the mod list and render all the mods
            ImGui.SetNextWindowPos(new System.Numerics.Vector2 { X = 0, Y = 0 });
            ImGui.SetNextWindowSize(new System.Numerics.Vector2 { X = 350, Y = 500 });
            if (ImGui.Begin("Fh.ModList", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoInputs)) {
                // I tried to increase this text's font size, but couldn't get ImGui.PushFont() to not throw an Access Violation (0xC0000005)
                // - Eve
                int mod_count = 0;
                foreach (FhModuleContext fmctx in FhModuleController.find_all()) mod_count++;
                ImGui.Text($"{mod_count} mods loaded");
                foreach (FhModuleContext fmctx in FhModuleController.find_all()) {
                    ImGui.Text($"{fmctx.Module.ModuleName} v{fmctx.Module.GetType().Assembly.GetName().Version}");
                }
                ImGui.End();
            }
        }
    }
}

