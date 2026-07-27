// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* Jobs:
 * - Render the "Settings" modal: the three installation-location rows (game,
 *   Fahrenheit, mods), each independently browsable/openable with a live
 *   valid/invalid status icon, and the theme picker grid.
 * - Bind each theme color picker to its FhTheme field and persist the change
 *   into FhModManagerSettings once anything in the section actually changes.
 */

namespace Fahrenheit.Tools.ModManager;

internal static unsafe partial class FhModManagerUI {
    private const int GAME_DIRECTORY_INPUT_LENGTH = 1024;

    // State for the "Settings" popup; see the comment on _handle_modals (ui_menu.cs)
    // for why this is a bool flipped from a menu click rather than an
    // ImGui.OpenPopup() call.
    private static bool _show_settings_dialog;

    private static void _render_settings_modal() {
        _center_next_window(
            width_fraction: 0.42F,
            min_width: 900F * FhTheme.UiScale,
            max_width: 1300F * FhTheme.UiScale);

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
        // saved Fahrenheit/Mods overrides, not a cached value - so all three
        // rows' status icons update live as the user types, browses, or an
        // override changes, rather than only refreshing after a save.
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

        // TODO - hidden for now; 
        // Setting 'mods' location logic still has value for mod pack/profiles.
        // Implementation of mod pack/profiles is not yet done, so this is hidden for now.
        //_render_mods_location_row(mods_directory);

        ImGui.Spacing();

        if (FhElements.button_secondary("Open Settings Folder")) {
            _open_folder(Path.GetDirectoryName(FhModManagerSettingsStore.SettingsPath)!);
        }

        ImGui.Spacing();

        _render_theme_section();

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

        bool submitted = ImGui.InputText(
            "##GameLocationInput",
            ref _game_directory_input,
            GAME_DIRECTORY_INPUT_LENGTH,
            ImGuiInputTextFlags.EnterReturnsTrue);

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

        ImGui.InputText(
            "##FahrenheitLocationInput",
            ref display_path,
            GAME_DIRECTORY_INPUT_LENGTH,
            ImGuiInputTextFlags.ReadOnly);

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

    private static void _render_mods_location_row(string mods_directory) {
        ImGui.SeparatorText("Mods Location");

        (bool valid, string? reason) = _check_mods_location(mods_directory);

        FhElements.status_icon(valid, reason);
        ImGui.SameLine();

        float browse_width  = _get_button_width("Browse");
        float open_width    = _get_button_width("Open");
        float buttons_width = browse_width + open_width + (ImGui.GetStyle().ItemSpacing.X * 2F);

        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - buttons_width);

        string display_path = mods_directory;

        ImGui.InputText(
            "##ModsLocationInput",
            ref display_path,
            GAME_DIRECTORY_INPUT_LENGTH,
            ImGuiInputTextFlags.ReadOnly);

        ImGui.SameLine();

        bool browse_pressed = FhElements.button_secondary("Browse##Mods", new Vector2(browse_width, 0F));

        ImGui.SameLine();

        bool open_pressed = FhElements.button_secondary("Open##Mods", new Vector2(open_width, 0F));

        if (browse_pressed) {
            _browse_mods_directory();
        }
        else if (open_pressed) {
            _open_folder(mods_directory);
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

    private static (bool IsValid, string? Reason) _check_mods_location(string mods_directory) {
        if (!Directory.Exists(mods_directory)) {
            return (false, "This folder does not exist.");
        }

        if (!File.Exists(Path.Join(mods_directory, "loadorder"))) {
            return (false, "No loadorder file was found here.");
        }

        return (true, null);
    }

    // Lets the user recolor the app (see FhTheme) and persists whatever they land
    // on. Each picker writes straight to both the live FhTheme field and the
    // corresponding FhModManagerSettings field, then apply()/save() run once at
    // the end if anything actually changed this frame. Laid out as a 3-column
    // table rather than one picker per line, so the section doesn't dominate
    // the modal's height - a table (rather than SameLine() calls sized off a
    // flat column_width) is what actually guarantees every column starts at
    // the same X on every row, since labels like "Text Primary" and "Accent"
    // are naturally different widths and would otherwise throw off each
    // column's start position row to row. NoInputs hides the inline R/G/B/A
    // number boxes each picker would otherwise show - the swatch already
    // opens a full picker (with those same numeric fields) on click, so
    // showing them twice is redundant and eats a lot of width across nine
    // pickers besides.
    private static void _render_theme_section() {
        ImGui.SeparatorText("Theme");

        bool changed = false;

        if (ImGui.BeginTable("##ThemeGrid", 3, ImGuiTableFlags.SizingStretchSame)) {
            ImGui.TableNextColumn();

            changed |= _theme_color_picker(
                "Background Primary",
                ref FhTheme.COLOR_BACKGROUND,
                static (settings, color) => settings.BackgroundColor = color);

            ImGui.TableNextColumn();

            changed |= _theme_color_picker(
                "Title Bar",
                ref FhTheme.COLOR_TITLE_BAR,
                static (settings, color) => settings.TitleBarColor = color);

            ImGui.TableNextColumn();

            changed |= _theme_color_picker(
                "Text Primary",
                ref FhTheme.COLOR_TEXT,
                static (settings, color) => settings.TextColor = color);

            ImGui.TableNextColumn();

            changed |= _theme_color_picker(
                "Background Secondary",
                ref FhTheme.COLOR_FRAME_BACKGROUND,
                static (settings, color) => settings.FrameBackgroundColor = color);

            ImGui.TableNextColumn();

            changed |= _theme_color_picker(
                "Accent",
                ref FhTheme.COLOR_ACCENT,
                static (settings, color) => settings.AccentColor = color);

            ImGui.TableNextColumn();

            changed |= _theme_color_picker(
                "Text Secondary",
                ref FhTheme.COLOR_TEXT_MUTED,
                static (settings, color) => settings.TextMutedColor = color);

            ImGui.TableNextColumn();

            changed |= _theme_color_picker(
                "Success",
                ref FhTheme.COLOR_SUCCESS,
                static (settings, color) => settings.SuccessColor = color);

            ImGui.TableNextColumn();

            changed |= _theme_color_picker(
                "Warning",
                ref FhTheme.COLOR_WARNING,
                static (settings, color) => settings.WarningColor = color);

            ImGui.TableNextColumn();

            changed |= _theme_color_picker(
                "Error",
                ref FhTheme.COLOR_ERROR,
                static (settings, color) => settings.ErrorColor = color);

            ImGui.EndTable();
        }

        ImGui.Spacing();

        if (FhElements.button_secondary("Reset to Default")) {
            FhTheme.reset_to_default();

            _settings.AccentColor          = null;
            _settings.BackgroundColor      = null;
            _settings.TextColor            = null;
            _settings.TextMutedColor       = null;
            _settings.FrameBackgroundColor = null;
            _settings.TitleBarColor        = null;
            _settings.SuccessColor         = null;
            _settings.WarningColor         = null;
            _settings.ErrorColor           = null;

            changed = true;
        }

        if (!changed) {
            return;
        }

        FhTheme.apply();

        if (!FhModManagerSettingsStore.try_save(_settings, out string save_error)) {
            _set_status(save_error, is_error: true);
        }
    }

    // Renders one "Label [swatch]" color picker bound directly to an FhTheme field.
    // On change, also records the new value into `_settings` via `assign` (or, for
    // Reset to Default, the caller nulls the settings field out separately) so the
    // theme section's single save-at-the-end below persists everything at once.
    private static bool _theme_color_picker(
        string label,
        ref Vector4 theme_color,
        Action<FhModManagerSettings, FhThemeColor> assign) {
        if (!ImGui.ColorEdit4(label, ref theme_color, ImGuiColorEditFlags.NoInputs)) {
            return false;
        }

        assign(_settings, FhThemeColor.from_vector4(theme_color));

        return true;
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
            string normalized =
            FhModManagerSettingsStore.normalize_path(
                _game_directory_input);

            _game_directory_input =
                normalized;

            _settings.GameDirectory =
                normalized;

            if (!FhModManagerSettingsStore.try_save(
                    _settings,
                    out string save_error)) {
                _set_status(save_error, is_error: true);
                return;
            }

            _rescan_mods();

            _set_status("Game location saved.");
        }
        catch (Exception exception) {
            _set_status(
                "The game location is invalid.\n\n" + exception.Message,
                is_error: true);
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
                _set_status(save_error, is_error: true);
                return;
            }

            _rescan_mods();

            _set_status("Fahrenheit location saved.");
        }
        catch (Exception exception) {
            _set_status(
                "The Fahrenheit location is invalid.\n\n" + exception.Message,
                is_error: true);
        }
    }

    private static void _browse_mods_directory() {
        DialogResult result = Dialog.FolderPicker(_settings.ModsDirectory ?? _game_directory_input);

        if (!result.IsOk || result.Path == null) {
            return;
        }

        try {
            _settings.ModsDirectory = FhModManagerSettingsStore.normalize_path(result.Path);

            if (!FhModManagerSettingsStore.try_save(_settings, out string save_error)) {
                _set_status(save_error, is_error: true);
                return;
            }

            _rescan_mods();

            _set_status("Mods location saved.");
        }
        catch (Exception exception) {
            _set_status(
                "The mods location is invalid.\n\n" + exception.Message,
                is_error: true);
        }
    }
}
