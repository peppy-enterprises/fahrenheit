using System.Runtime.InteropServices;

using Fahrenheit.Core.ImGuiNET;

namespace Fahrenheit.Core.Runtime;

[FhLoaderMark]
public unsafe class FhCoreModule : FhModule {
    private readonly FhMethodHandle<FhCall.Sg_MainLoop>               _main_loop;
    private readonly FhMethodHandle<FhCall.AtelExecInternal_00871d10> _update_input;
    private readonly FhMethodHandle<FhCall.TODrawMessageWindow>       _render_game;

    public FhCoreModule() {
        _main_loop    = new(this, "FFX.exe", main_loop,    offset: FhCall.__addr_Sg_MainLoop);
        _update_input = new(this, "FFX.exe", update_input, offset: FhCall.__addr_AtelExecInternal_00871d10);
        _render_game  = new(this, "FFX.exe", render_game,  offset: FhCall.__addr_TODrawMessageWindow);
    }

    public override bool init() {
        return _main_loop   .hook()
            && _update_input.hook()
            && _render_game .hook();
    }

    public void main_loop(float delta) {
        foreach (FhModContext mod_ctx in FhInternal.ModController.get_all()) {
            foreach (FhModuleContext module_ctx in mod_ctx.Modules) {
                module_ctx.Module.pre_update();
            }
        }

         _main_loop.orig_fptr(delta);

        foreach (FhModContext mod_ctx in FhInternal.ModController.get_all()) {
            foreach (FhModuleContext module_ctx in mod_ctx.Modules) {
                module_ctx.Module.post_update();
            }
        }

        foreach (nint addr in FhPointer.get_pending_wait_addresses()) {
            PInvoke.WakeByAddressAll(addr.ToPointer());
        }
    }

    public void update_input() {
        FFX.Globals.Input.update();

        _update_input.orig_fptr();

        foreach (FhModContext mod_ctx in FhInternal.ModController.get_all()) {
            foreach (FhModuleContext module_ctx in mod_ctx.Modules) {
                module_ctx.Module.handle_input();
            }
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

        foreach (FhModContext mod_ctx in FhInternal.ModController.get_all()) {
            foreach (FhModuleContext module_ctx in mod_ctx.Modules) {
                module_ctx.Module.render_game();
            }
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
                foreach (FhModContext mod_ctx in FhInternal.ModController.get_all()) {
                    foreach (FhModuleContext module_ctx in mod_ctx.Modules) {
                        mod_count++;
                    }
                }
                ImGui.Text($"{mod_count} mods loaded");
                foreach (FhModContext mod_ctx in FhInternal.ModController.get_all()) {
                    foreach (FhModuleContext module_ctx in mod_ctx.Modules) {
                        ImGui.Text($"{module_ctx.Module.ModuleType} v{module_ctx.Module.GetType().Assembly.GetName().Version}");
                    }
                }
                ImGui.End();
            }
        }
    }
}

