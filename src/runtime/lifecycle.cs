// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit.Runtime;

/// <summary>
///     Executes the lifecycle methods of <see cref="FhModule"/>.<br/>
///     Also renders the modlist on the main menu.
///     <para/>
///     In your module, override <see cref="FhModule.handle_input">.
/// </summary>
[FhLoad(FhGameId.FFX | FhGameId.FFX2 | FhGameId.FFX2LM)]
public unsafe class FhCoreModule : FhModule {

    private static readonly FhSettingsCategory _settings = new("fhr", [
        new FhSettingToggle("display_mod_count", true),
    ]);

    public FhCoreModule() {
        settings = _settings;
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        return FhCall.h_AtelExec_Internal_871D10.hook(this, handle_input);
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
    [UnmanagedCallConv(CallConvs = [ typeof(CallConvStdcall) ] )]
    private void h_update_input() {
        FhApi.Input.update();

        FhCall.h_AtelExec_Internal_871D10.chain_from(h_update_input).fnptr!();

        foreach (FhModuleContext module_ctx in FhApi.Mods.get_modules()) {
            module_ctx.Module.handle_input();
        }
    }
}
