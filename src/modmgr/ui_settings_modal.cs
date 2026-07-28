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

    // State for the "Settings" popup; see the comment on _handle_modals (ui_menu.cs)
    // for why this is a bool flipped from a menu click rather than an
    // ImGui.OpenPopup() call.
    private static bool _show_settings_dialog;

    private static void _render_settings_modal() {
        _center_next_window(width_fraction: 0.42F, min_width: 900F * FhTheme.UiScale, max_width: 1300F * FhTheme.UiScale);

        // Passing `ref modal_open` (rather than null) gives the popup its own
        // native title-bar close button, so there's no need for a separate
        // "Close" button in the body.
        bool modal_open = true;
        bool popup_open = ImGui.BeginPopupModal("Settings", ref modal_open, ImGuiWindowFlags.NoResize);

        // BeginPopupModal can return false on the very frame the title-bar X is
        // clicked (as well as when the popup isn't open at all), so this has to
        // be checked before the early return below, not only at the bottom of
        // the function - otherwise that click never actually clears
        // _show_settings_dialog, and _handle_modals() (ui_menu.cs) just
        // re-opens the same popup again next frame since, as far as it's
        // concerned, nothing ever asked to close it.
        if (!modal_open) {
            _show_settings_dialog = false;
        }

        if (!popup_open) {
            return;
        }

        // Closes on an outside click too, same as a non-modal popup already
        // does by default - BeginPopupModal on its own only supports that via
        // the title-bar X or Escape. AllowWhenBlockedByPopup matters here: a
        // color picker's own popup (opened by clicking a swatch below) counts
        // as "blocking" this window, and IsWindowHovered excludes a blocked
        // window by default - without this flag, clicking anywhere in Settings
        // at all while a color picker happened to be open would already read
        // as "not hovering Settings" and incorrectly close the whole modal.
        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left)
            && !ImGui.IsWindowHovered(
                ImGuiHoveredFlags.RootAndChildWindows | ImGuiHoveredFlags.AllowWhenBlockedByPopup)) {
            _show_settings_dialog = false;
            ImGui.CloseCurrentPopup();
        }

        _render_game_directory_row();

        // Derived from the (possibly not-yet-saved) game directory input and the
        // saved Fahrenheit/Mods overrides, not cached value - so validity icons update live.
        string normalized_game_directory;

        try {
            normalized_game_directory = FhModManagerSettingsStore.normalize_path(_game_directory_input);
        }
        catch {
            normalized_game_directory = _game_directory_input;
        }

        (string fahrenheit_directory, string mods_directory) = FhModScanner.resolve_paths(
            normalized_game_directory,
            _settings.FahrenheitDirectory,
            _settings.ModsDirectory);

        _render_fahrenheit_location_row(fahrenheit_directory);

        ImGui.EndPopup();
    }

    // The one location that's plain text-editable (Enter to save, matching the
    // Browse button beside it) rather than Browse-only - everything else about
    // its row (icon, Browse, Open) matches the other two.
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

        FhElements.status_icon(valid, reason);
        ImGui.SameLine();

        float browse_width  = _get_button_width("Browse");
        float open_width    = _get_button_width("Open");
        float buttons_width = browse_width + open_width + (ImGui.GetStyle().ItemSpacing.X * 2F);

        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - buttons_width);

        bool submitted = ImGui.InputText("##GameLocationInput", ref _game_directory_input, GAME_DIRECTORY_INPUT_LENGTH, ImGuiInputTextFlags.EnterReturnsTrue);

        ImGui.SameLine();

        // "##Game" suffixes here (and "##Fahrenheit"/"##Mods" on the two rows
        // below) keep all three rows' otherwise-identically-labeled Browse/Open
        // buttons from colliding on the same ImGui ID - everything past "##" is
        // stripped from the visible label but still part of the ID, so all
        // three still just read as "Browse"/"Open".
        bool browse_pressed = FhElements.button_secondary("Browse##Game", new Vector2(browse_width, 0F));

        ImGui.SameLine();

        bool open_pressed = FhElements.button_secondary("Open##Game", new Vector2(open_width, 0F));

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

        FhElements.status_icon(valid, reason);
        ImGui.SameLine();

        float browse_width  = _get_button_width("Browse");
        float open_width    = _get_button_width("Open");
        float buttons_width = browse_width + open_width + (ImGui.GetStyle().ItemSpacing.X * 2F);

        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - buttons_width);

        string display_path = fahrenheit_directory;

        ImGui.InputText("##FahrenheitLocationInput", ref display_path, GAME_DIRECTORY_INPUT_LENGTH, ImGuiInputTextFlags.ReadOnly);

        ImGui.SameLine();

        bool browse_pressed = FhElements.button_secondary("Browse##Fahrenheit", new Vector2(browse_width, 0F));

        ImGui.SameLine();

        bool open_pressed = FhElements.button_secondary("Open##Fahrenheit", new Vector2(open_width, 0F));

        if (browse_pressed) {
            _browse_fahrenheit_directory();
        }
        else if (open_pressed) {
            _open_folder(fahrenheit_directory);
        }
    }

    // A location "looks valid" if it both exists and contains what's actually
    // expected there, not just that the folder is present.
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
