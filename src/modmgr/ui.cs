// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* Jobs:
 * - Own FhModManagerUI's core state (settings, game directory input, mod
 *   catalog) and the static constructor that loads it.
 * - Drive the top-level UI() frame: main window, menu bar, header/warnings,
 *   mod lists, status bar, then apply any pending mutations queued this frame.
 * - This file is the entry point and shared state only - see ui_menu.cs,
 *   ui_settings_modal.cs, ui_efl_import_modal.cs, ui_mod_list.cs,
 *   ui_drag_handle.cs, ui_status_bar.cs, ui_actions.cs, and ui_helpers.cs for
 *   everything else.
 */

namespace Fahrenheit.Tools.ModManager;

internal static unsafe partial class FhModManagerUI {
    private static readonly FhModManagerSettings _settings;

    private static string       _game_directory_input;
    private static FhModCatalog _catalog;

    static FhModManagerUI() {
        _settings = FhModManagerSettingsStore.load(
            out string settings_warning);

        // main.cs already called FhTheme.apply() once with the built-in defaults,
        // before this constructor could run (it needs a loaded FhModManagerSettings
        // first). Re-applying here with any saved overrides happens before the
        // first frame renders, so there's no visible flash of the default palette.
        FhTheme.load_from_settings(_settings);
        FhTheme.apply();

        _game_directory_input =
            _settings.GameDirectory;

        _rescan_mods();

        _set_status(settings_warning);
    }

    public static void UI() {
        _handle_modals();

        ImGuiViewportPtr viewport =
            ImGui.GetMainViewport();

        ImGui.SetNextWindowPos(
            viewport.WorkPos);

        ImGui.SetNextWindowSize(
            viewport.WorkSize);

        ImGuiWindowFlags window_flags =
          ImGuiWindowFlags.MenuBar
        | ImGuiWindowFlags.NoTitleBar
        | ImGuiWindowFlags.NoResize
        | ImGuiWindowFlags.NoMove
        | ImGuiWindowFlags.NoCollapse
        | ImGuiWindowFlags.NoSavedSettings
        | ImGuiWindowFlags.NoScrollbar
        | ImGuiWindowFlags.NoScrollWithMouse;

        if (!ImGui.Begin(
                "Fahrenheit Mod Manager###FhModManager",
                window_flags)) {
            ImGui.End();
            return;
        }

        _render_main_menu();

        // No Spacing() before this one (unlike the pair below): the menu bar's
        // own bottom border already sits flush against the separator without it,
        // which is what keeps FhTheme.COLOR_TITLE_BAR's fill looking like one
        // solid, cleanly-bordered box rather than trailing off into a sliver of
        // plain COLOR_BACKGROUND before the line (see _render_main_menu in
        // ui_menu.cs for how the row is centered within that box).
        // ImGui.Separator();
        ImGui.Spacing();

        _render_header();
        _render_warnings();

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        _render_mod_lists();
        _render_status_bar();
        ImGui.End();
        _apply_pending_mod_toggle();
        _apply_pending_load_order_move();
        _apply_pending_preview_move();
        _apply_pending_load_order_drop();
    }

    private static void _render_header() {
        ImGui.Text("Fahrenheit Mod Manager");
        ImGui.TextDisabled(
            "Manage the active load order and launch Final Fantasy X / X-2.");

        ImGui.Spacing();
    }

    private static void _render_warnings() {
        if (_catalog.Warnings.Count == 0) {
            return;
        }

        ImGui.Spacing();

        foreach (string warning in _catalog.Warnings) {
            ImGui.TextColored(
                FhTheme.COLOR_WARNING,
                warning);
        }
    }
}
