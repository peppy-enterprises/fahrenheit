namespace Fahrenheit.Core.Runtime;

/* [fkelava 21/6/25 01:52]
 * Temporary until FhCall is restored to `ffx-v3` RE state.
 */
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal delegate void Sg_MainLoop(float delta);

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal delegate void TODrawMessageWindow();

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal delegate void AtelExecInternal_00871d10();

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate void TOMkpCrossExtMesFontLClutTypeRGBA(
    uint  p1,
    byte* text,
    float x,
    float y,
    byte  color,
    byte  p6,
    byte  tint_r, byte tint_g, byte tint_b, byte tint_a,
    float scale,
    float _);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate nint TOAdpMesFontLXYZClutTypeRGBAChangeFontType(
    nint p1,
    uint p2,
    byte* text,
    float x,
    float y,
    nint p6,
    nint p7,
    nint p8,
    nint tint_r, nint tint_g, nint tint_b, nint tint_a,
    bool p13);

/// <summary>
///     Executes the lifecycle methods of <see cref="FhModule"/>.
///     <para/>
///     Do not interface with this module directly. Instead, implement:
///     <br/> - <see cref="FhModule.pre_update"/>
///     <br/> - <see cref="FhModule.post_update"/>
///     <br/> - <see cref="FhModule.render_game"/>
/// </summary>
[FhLoad(FhGameType.FFX | FhGameType.FFX2)]
public unsafe class FhCoreModule : FhModule {
    private readonly FhMethodHandle<Sg_MainLoop>                _main_loop;
    private readonly FhMethodHandle<AtelExecInternal_00871d10>  _update_input;
    private readonly FhMethodHandle<TODrawMessageWindow>        _render_game;

    private readonly TOMkpCrossExtMesFontLClutTypeRGBA?          _draw_delegate_x;
    private readonly TOAdpMesFontLXYZClutTypeRGBAChangeFontType? _draw_delegate_x2;

    private static readonly FhSettingsCategory _settings = new("fhruntime", [
        new FhSettingToggle("display_mod_count", true),
    ]);

    public FhCoreModule() {
        FhMethodLocation location_main_loop    = new(0x420C00, 0x205150);
        FhMethodLocation location_update_input = new(0x471D10, 0x32CE90);
        FhMethodLocation location_render_game  = new(0x4ABCE0, 0x391D00);

        settings = _settings;

        _main_loop    = new(this, location_main_loop,    h_main_loop);
        _update_input = new(this, location_update_input, h_update_input);
        _render_game  = new(this, location_render_game,  h_render_game);

        switch (FhGlobal.game_type) {
            case FhGameType.FFX:
                _draw_delegate_x = FhUtil.get_fptr<TOMkpCrossExtMesFontLClutTypeRGBA>(0x501700);
                break;
            case FhGameType.FFX2:
                _draw_delegate_x2 = FhUtil.get_fptr<TOAdpMesFontLXYZClutTypeRGBAChangeFontType>(0x3A7600);
                break;
        }
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        return _main_loop   .hook()
            && _update_input.hook()
            && _render_game .hook();
    }

    public override void render_imgui() {
        int curr_event_id = FhGlobal.game_type switch {
            FhGameType.FFX  => *FFX.Globals.event_id,
            FhGameType.FFX2 => *FFX2.Globals.event_id
        };

        if (curr_event_id != 0x17) return; // Deactivate the mod list outside the main menu.

        // Create a window for the mod list and render all the mods
        ImGui.SetNextWindowPos (new System.Numerics.Vector2 { X = 0,   Y = 0   });
        ImGui.SetNextWindowSize(new System.Numerics.Vector2 { X = 350, Y = 500 });

        if (ImGui.Begin("Fh.ModList", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoInputs)) {
            ImGui.PushFont(FhApi.ImGuiHelper.FONT_DEFAULT, 18f);
            FhModContext[] mods = [ .. FhApi.ModController.get_mods() ];

            ImGui.Text($"{mods.Length} mods loaded");
            foreach (FhModContext mod_ctx in mods) {
                ImGui.Text($"{mod_ctx.Manifest.Name} v{mod_ctx.Manifest.Version}");
            }
            ImGui.PopFont();
        }
        ImGui.End();

        //ImGui.ShowDemoWindow();
    }

    /// <summary>
    ///     Overrides the game's main loop to execute the <see cref="FhModule.pre_update"/> and
    ///     <see cref="FhModule.post_update"/> callbacks before and after every iteration, respectively.
    /// </summary>
    private void h_main_loop(float delta) {
        foreach (FhModuleContext module_ctx in FhApi.ModController.get_modules()) {
            module_ctx.Module.pre_update();
        }

        _main_loop.orig_fptr(delta);

        foreach (FhModuleContext module_ctx in FhApi.ModController.get_modules()) {
            module_ctx.Module.post_update();
        }
    }

    private void h_update_input() {
        FhApi.Input.update();

        _update_input.orig_fptr();

        foreach (FhModuleContext module_ctx in FhApi.ModController.get_modules()) {
            module_ctx.Module.handle_input();
        }
    }

    private void draw_text_rgba(
        byte[] text,
        float  x,
        float  y,
        byte   color,
        float  scale
    ) {
        fixed (byte* text_ptr = text)
            _draw_delegate_x?.Invoke(0, text_ptr, x, y, color, 0, 0x80, 0x80, 0x80, 0x80, scale, 0);
    }

    private void h_render_game() {
        _render_game.orig_fptr();

        foreach (FhModuleContext module_ctx in FhApi.ModController.get_modules()) {
            module_ctx.Module.render_game();
        }

        string text = $"Fahrenheit v{typeof(FhGlobal).Assembly.GetName().Version}";
        switch (FhGlobal.game_type) {
            case FhGameType.FFX:
                // In the main menu...
                if (*FFX.Globals.event_id == 0x17) {
                    // render some text so that people can't easily hide their use of Fahrenheit
                    draw_text_rgba(FhCharset.Us.to_bytes(text), 5f, 400f, 0x00, 0.65f);
                }
                break;
            case FhGameType.FFX2:
                // Main menu draws over this
                //if (*FFX2.Globals.event_id == 0x17) {
                //    fixed (byte* s = FhCharset.Us.to_bytes(text)) {
                //        _TOAdpMesFontLXYZClutTypeRGBAChangeFontType(0, 0xffffffff, s, 5f, 400f, 0x10, 0, 0, 0x00, 0x80, 0x80, 0x80, false);
                //    }
                //}
                break;
        }
    }
}
