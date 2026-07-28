// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* Jobs:
 * - Action handlers invoked from menu items and modal buttons: refreshing the
 *   catalog, opening a folder in the OS file browser, installing a mod from
 *   file (stub), exporting/importing a mod list or mod pack, and launching
 *   the game.
 * - Apply the pending mod-list mutations queued this frame by ui_mod_list.cs
 *   and ui_drag_handle.cs (toggle, arrow move, drag preview, drag drop), once
 *   rendering has finished (see UI() in ui.cs).
 */

namespace Fahrenheit.Tools.ModManager;

internal static unsafe partial class FhModManagerUI {
    private static void _refresh_mods() {
        try {
            string normalized = FhModManagerSettingsStore.normalize_path(_game_directory_input);

            _catalog = FhModScanner.scan(normalized, _settings.FahrenheitDirectory);

            _set_status("Mod list refreshed.");
        }
        catch (Exception exception) {
            _set_status($"The mod list could not be refreshed.\n\n{exception.Message}", true);
        }
    }

    private static void _open_folder(string path) {
        if (!FhShell.try_open_folder(path, out string error)) {
            _set_status(error, true);
        }
    }

    // Stubbed: only opens the file picker for now. Wiring up the actual install
    // flow (what a "mod file" is - a manifest, an archive, something else) is a
    // follow-up once that's decided.
    private static void _install_mod_from_file() {
        Dialog.FileOpen(string.Empty, string.Empty);
    }

    /// <summary>
    ///     Exports current list of enabled mods to a typeless file; one per line.
    ///     Mod must have a name in the manifest to be included in the export.
    /// </summary>
    private static void _export_mod_list() {
        DialogResult result = Dialog.FileSave(string.Empty, string.Empty);

        if (!result.IsOk || result.Path == null) {
            return;
        }

        try {
            List<string> lines = [];

            foreach (FhInstalledMod mod in _catalog.Enabled) {
                if (string.IsNullOrEmpty(mod.Manifest.Name)) {
                    continue;
                }
                lines.Add(mod.Manifest.Name);
            }

            string contents = string.Join(Environment.NewLine, lines);

            if (lines.Count > 0) {
                contents += Environment.NewLine;
            }

            File.WriteAllText(result.Path, contents);

            _set_status("Mod list exported.");
        }
        catch (Exception exception) {
            _set_status($"The mod list could not be exported.\n\n{exception.Message}", true);
        }
    }

    private static void _import_mod_pack() {
        DialogResult result = Dialog.FileOpen("zip", string.Empty);

        if (!result.IsOk || result.Path == null) {
            return;
        }

        ResultsMessage pack_result = FhModPackImporter.import(_catalog.ModsDirectory, _catalog.LoadOrderPath, result.Path);

        _set_status(pack_result.Message, !pack_result.Success);

        if (pack_result.Success) {
            _rescan_mods();
        }
    }

    private static void _export_mod_pack() {
        DialogResult result = Dialog.FileSave("zip", string.Empty);

        if (!result.IsOk || result.Path == null) {
            return;
        }

        ResultsMessage pack_result = FhModPackExporter.export(result.Path, _catalog.Enabled);

        _set_status(pack_result.Message, !pack_result.Success);
    }

    private static void _launch_game(FhGameId target, string[]? args = null) {
        ResultsMessage result = FhGameLauncher.launch(target, _game_directory_input, args ?? []);

        _set_status(result.Message, !result.Success);
    }

    private static void _apply_pending_mod_toggle() {
        if (_pending_mod_toggle == null) {
            return;
        }

        FhPendingModToggle action = _pending_mod_toggle;

        _pending_mod_toggle = null;

        if (!FhLoadOrderEditor.try_set_enabled(_catalog.LoadOrderPath, action.Mod, action.Enable, out string error)) {
            _set_status(error, true);
            return;
        }

        _rescan_mods();

        _set_status(action.Enable ? $"Enabled {action.Mod.Manifest.Name}." : $"Disabled {action.Mod.Manifest.Name}.");
    }

    private static void _apply_pending_load_order_move() {
        if (_pending_load_order_move == null) {
            return;
        }

        FhPendingLoadOrderMove action = _pending_load_order_move;

        _pending_load_order_move = null;

        if (!FhLoadOrderEditor.try_move(_catalog.LoadOrderPath, action.Mod, action.Direction, out string error)) {
            _set_status(error, true);
            return;
        }

        _rescan_mods();

        string direction_name = action.Direction < 0 ? "up" : "down";

        _set_status($"Moved {action.Mod.Manifest.Name} {direction_name}.");
    }

    // In-memory-only reorder for live drag feedback: no disk write, no rescan -
    // see the comment on _dragging_mod (ui_drag_handle.cs) above for why.
    private static void _apply_pending_preview_move() {
        if (_pending_preview_move == null) {
            return;
        }

        FhPendingPreviewMove action = _pending_preview_move;
        _pending_preview_move = null;

        if (action.FromIndex < 0 || action.FromIndex >= _catalog.Enabled.Count
            || action.ToIndex < 0 || action.ToIndex >= _catalog.Enabled.Count) {
            return;
        }

        List<FhInstalledMod> reordered = [.. _catalog.Enabled];

        FhInstalledMod moved = reordered[action.FromIndex];
        reordered.RemoveAt(action.FromIndex);
        reordered.Insert(action.ToIndex, moved);

        _catalog = _catalog with { Enabled = reordered };
    }

    // Commits a drag-and-drop reorder to disk once the mouse is released, using
    // wherever the mod's in-memory preview position (see _apply_pending_preview_move)
    // ended up as the target.
    private static void _apply_pending_load_order_drop() {
        if (_pending_load_order_drop == null) {
            return;
        }

        FhPendingLoadOrderDrop action = _pending_load_order_drop;
        _pending_load_order_drop = null;

        bool didMove = FhLoadOrderEditor.try_move_to(_catalog.LoadOrderPath, action.Mod, action.TargetIndex, out string error);
        if (!didMove) {
            _set_status(error, true);
            return;
        }

        _rescan_mods();

        _set_status($"Moved {action.Mod.Manifest.Name}.");
    }
}
