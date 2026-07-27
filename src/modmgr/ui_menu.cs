// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* Jobs:
 * - Render the menu bar (Mods / Play / Settings) and dispatch its actions,
 *   plus the app icon (see icon.cs) in its own left inset.
 * - Render the minimize/maximize/close buttons drawn into the menu bar in
 *   place of the OS title bar (see chrome.cs).
 * - Open the Settings/Import EFL Mod popups from outside any menu scope,
 *   since ImGui.OpenPopup() can't be called from inside a BeginMenu() block.
 */

namespace Fahrenheit.Tools.ModManager;

internal static unsafe partial class FhModManagerUI {
    private static Vector2 MENU_BAR_FRAME_PADDING => new Vector2(10F, 8F) * FhTheme.UiScale;

    private static void _render_main_menu() {
        if (!ImGui.BeginMenuBar()) {
            return;
        }

        // Taller than the default FramePadding, so the bar itself grows and both
        // the menu items and the window control buttons below (which size
        // themselves off GetFrameHeight()) get more vertical breathing room
        // instead of sitting flush top-and-bottom against a cramped bar.
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, MENU_BAR_FRAME_PADDING);

        if (ImGui.BeginMenu("Mods")) {
            if (ImGui.MenuItem("Install from File")) {
                _install_mod_from_file();
            }

            if (ImGui.MenuItem("Import EFL Mod")) {
                _show_efl_import_dialog = true;
            }

            ImGui.Separator();

            if (ImGui.MenuItem("Refresh Mod List")) {
                _refresh_mods();
            }

            if (ImGui.MenuItem("Export Mod List")) {
                _export_mod_list();
            }

            ImGui.Separator();

            if (ImGui.MenuItem("Import Mod Pack")) {
                _import_mod_pack();
            }

            if (ImGui.MenuItem("Export Mod Pack")) {
                _export_mod_pack();
            }

            ImGui.EndMenu();
        }

        if (ImGui.BeginMenu("Play")) {
            if (ImGui.MenuItem("Final Fantasy X HD Remaster")) {
                _launch_game(FhLaunchTarget.FFX, false);
            }

            if (ImGui.MenuItem("Final Fantasy X-2 HD Remaster")) {
                _launch_game(FhLaunchTarget.FFX2, false);
            }

            ImGui.Separator();

            if (ImGui.MenuItem("Final Fantasy X HD Remaster (Debug)")) {
                _launch_game(FhLaunchTarget.FFX, true);
            }

            if (ImGui.MenuItem("Final Fantasy X-2 HD Remaster (Debug)")) {
                _launch_game(FhLaunchTarget.FFX2, true);
            }

            ImGui.EndMenu();
        }

        if (ImGui.MenuItem("Settings")) {
            _show_settings_dialog = true;
        }

        Vector2 menu_cluster_max = ImGui.GetItemRectMax();

        ImGui.MenuItem("About");

        ImGui.PopStyleVar();

        ImGui.EndMenuBar();
    }

    /* [modeled on EEdit's _handle_modal]
     * Popups can't be opened from inside a BeginMenu() scope - see
     * https://github.com/ocornut/imgui/issues/5684#issuecomment-1247928651 - so a
     * menu click just sets `_show_efl_import_dialog`, and this (called outside any
     * menu/window scope, at the top of UI()) is what actually opens and renders it.
     */
    private static void _handle_modals() {
        if (_show_efl_import_dialog) {
            ImGui.OpenPopup("Import EFL Mod");
        }

        if (_show_settings_dialog) {
            ImGui.OpenPopup("Settings");
        }

        _render_efl_import_modal();
        _render_settings_modal();
    }
}
