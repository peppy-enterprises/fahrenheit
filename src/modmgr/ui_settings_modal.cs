// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* Jobs:
 * - Render the "Settings" modal: the three installation-location rows (game,
 *   Fahrenheit, mods), each independently browsable/openable with a live
 *   valid/invalid status icon.
 */

namespace Fahrenheit.Tools.ModManager;

internal static unsafe partial class FhModManagerUI {
    private const int GAME_DIRECTORY_INPUT_LENGTH = 1024;
    private static bool _show_settings_dialog;

    private static void _render_settings_modal() {
        _center_next_window(width_fraction: 0.42F, min_width: 900F, max_width: 1300F);

 
        bool modal_open = true;
        bool popup_open = ImGui.BeginPopupModal("Settings", ref modal_open, ImGuiWindowFlags.NoResize);


        if (!modal_open) {
            _show_settings_dialog = false;
        }

        if (!popup_open) {
            return;
        }

        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left)
            && !ImGui.IsWindowHovered(
                ImGuiHoveredFlags.RootAndChildWindows | ImGuiHoveredFlags.AllowWhenBlockedByPopup)) {
            _show_settings_dialog = false;
            ImGui.CloseCurrentPopup();
        }

        _render_game_directory_row();

        string normalized_game_directory;

        try {
            normalized_game_directory = FhModManagerSettingsStore.normalize_path(_game_directory_input);
        }
        catch {
            normalized_game_directory = _game_directory_input;
        }

        (string fahrenheit_directory, string mods_directory) = FhModScanner.resolve_paths(normalized_game_directory, _settings.FahrenheitDirectory);

        _render_fahrenheit_location_row(fahrenheit_directory);

        ImGui.EndPopup();
    }

    private static void _render_game_directory_row() {
        ImGui.SeparatorText("FF X/X-2 HD Remaster Location");

        bool    valid;
        string? reason;

        try {
            string normalized = FhModManagerSettingsStore.normalize_path(_game_directory_input);
            (valid, reason) = _check_game_location(normalized);
        }
        catch (Exception exception) {
            valid  = false;
            reason = exception.Message;
        }

        _status_icon(valid, reason);
        ImGui.SameLine();

        float browse_width  = _get_button_width("Browse");
        float open_width    = _get_button_width("Open");
        float buttons_width = browse_width + open_width + (ImGui.GetStyle().ItemSpacing.X * 2F);

        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - buttons_width);

        bool submitted = ImGui.InputText("##GameLocationInput", ref _game_directory_input, GAME_DIRECTORY_INPUT_LENGTH, ImGuiInputTextFlags.EnterReturnsTrue);

        ImGui.SameLine();

        bool browse_pressed = ImGui.Button("Browse##Game", new Vector2(browse_width, 0F));

        ImGui.SameLine();

        bool open_pressed = ImGui.Button("Open##Game", new Vector2(open_width, 0F));

        if (browse_pressed) {
            _browse_game_directory();
        }
        else if (submitted) {
            _save_game_directory();
        }
        else if (open_pressed) {
            _open_folder(_game_directory_input);
        }
    }

    private static void _render_fahrenheit_location_row(string fahrenheit_directory) {
        ImGui.SeparatorText("Fahrenheit Location");

        (bool valid, string? reason) = _check_fahrenheit_location(fahrenheit_directory);

        _status_icon(valid, reason);
        ImGui.SameLine();

        float browse_width  = _get_button_width("Browse");
        float open_width    = _get_button_width("Open");
        float buttons_width = browse_width + open_width + (ImGui.GetStyle().ItemSpacing.X * 2F);

        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - buttons_width);

        string display_path = fahrenheit_directory;

        ImGui.InputText("##FahrenheitLocationInput", ref display_path, GAME_DIRECTORY_INPUT_LENGTH, ImGuiInputTextFlags.ReadOnly);

        ImGui.SameLine();

        bool browse_pressed = ImGui.Button("Browse##Fahrenheit", new Vector2(browse_width, 0F));

        ImGui.SameLine();

        bool open_pressed = ImGui.Button("Open##Fahrenheit", new Vector2(open_width, 0F));

        if (browse_pressed) {
            _browse_fahrenheit_directory();
        }
        else if (open_pressed) {
            _open_folder(fahrenheit_directory);
        }
    }

    private static (bool IsValid, string? Reason) _check_game_location(string normalized_game_directory) {
        if (!Directory.Exists(normalized_game_directory)) {
            return (false, "This folder does not exist.");
        }

        if (!File.Exists(Path.Join(normalized_game_directory, "FFX&X-2_LAUNCHER.exe"))) {
            return (false, "FFX&X-2_LAUNCHER.exe was not found here.");
        }

        return (true, null);
    }

    private static (bool IsValid, string? Reason) _check_fahrenheit_location(string fahrenheit_directory) {
        if (!Directory.Exists(fahrenheit_directory)) {
            return (false, "This folder does not exist.");
        }

        if (!File.Exists(Path.Join(fahrenheit_directory, "bin", "fhstage0.exe"))) {
            return (false, "fhstage0.exe was not found in its bin folder.");
        }

        return (true, null);
    }

    private static void _browse_game_directory() {
        DialogResult result = Dialog.FolderPicker(_game_directory_input);

        if (!result.IsOk || result.Path == null) {
            return;
        }

        _game_directory_input = result.Path;

        _save_game_directory();
    }

    private static void _save_game_directory() {
        try {
            string normalized = FhModManagerSettingsStore.normalize_path(_game_directory_input);

            _game_directory_input = normalized;

            _settings.GameDirectory = normalized;

            if (!FhModManagerSettingsStore.try_save( _settings, out string save_error)) {
                _set_status(save_error,true);
                return;
            }

            _rescan_mods();

            _set_status("Game location saved.");
        }
        catch (Exception exception) {
            _set_status($"The game location is invalid.\n\n{exception.Message}", true);
        }
    }

    private static void _browse_fahrenheit_directory() {
        DialogResult result = Dialog.FolderPicker(_settings.FahrenheitDirectory ?? _game_directory_input);

        if (!result.IsOk || result.Path == null) {
            return;
        }

        try {
            _settings.FahrenheitDirectory = FhModManagerSettingsStore.normalize_path(result.Path);

            if (!FhModManagerSettingsStore.try_save(_settings, out string save_error)) {
                _set_status(save_error, true);
                return;
            }

            _rescan_mods();

            _set_status("Fahrenheit location saved.");
        }
        catch (Exception exception) {
            _set_status($"The Fahrenheit location is invalid.\n\n{exception.Message}", true);
        }
    }
}
