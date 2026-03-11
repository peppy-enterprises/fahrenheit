// SPDX-License-Identifier: MIT

namespace Fahrenheit.Runtime;

/* [fkelava 21/6/25 01:52]
 * Temporary until FhCall is restored to `ffx-v3` RE state.
 */

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal delegate void AtelExecInternal_00871d10();

/// <summary>
///     Executes the lifecycle methods of <see cref="FhModule"/>.<br/>
///     Also renders the modlist on the main menu.
///     <para/>
///     Do not interface with this module directly. Instead, override <see cref="FhModule.handle_input">.
/// </summary>
[FhLoad(FhGameId.FFX | FhGameId.FFX2 | FhGameId.FFX2LM)]
public unsafe class FhCoreModule : FhModule {
    private readonly FhMethodHandle<AtelExecInternal_00871d10> _update_input;

    private static readonly FhSettingsCategory _settings = new("fhruntime", [
        new FhSettingToggle("display_mod_count", true),
    ]);

    public FhCoreModule() {
        FhMethodLocation location_update_input = new(0x471D10, 0x32CE90);

        settings = _settings;

        _update_input = new(this, location_update_input, h_update_input);
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        return _update_input.hook();
    }

    public override void render_imgui() {
        int curr_event_id = FhUtil.select(*FFX.Globals.event_id, *FFX2.Globals.event_id, *FFX2.Globals.event_id);

        if (curr_event_id != 0x17) return; // Deactivate the mod list outside the main menu.

        // Create a window for the mod list and render all the mods
        ImGui.SetNextWindowPos (new Vector2(0,   0  ));
        ImGui.SetNextWindowSize(new Vector2(350, 500));

        if (ImGui.Begin("Fh.ModList", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoInputs)) {
            ImGui.PushFont(FhApi.ImGuiHelper.FONT_DEFAULT, 18f);
            FhModContext[] mods = [ .. FhApi.Mods.get_mods() ];

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
    ///     Overrides the game's input handler to execute the
    ///     <see cref="FhModule.handle_input"/> callback with the latest input state.
    /// </summary>
    private void h_update_input() {
        FhApi.Input.update();

        _update_input.orig_fptr();

        foreach (FhModuleContext module_ctx in FhApi.Mods.get_modules()) {
            module_ctx.Module.handle_input();
        }
    }
}
