// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* Jobs:
 * - Define FhModManagerSettings, the JSON-serializable shape of fhmodmgr.json
 *   (game directory, optional Fahrenheit/mods location overrides, and
 *   optional theme color overrides).
 * - FhModManagerSettingsStore: locate, load, and atomically save that file
 *   beside the executable (this tool is a portable install, so settings
 *   don't live in per-machine state), plus the shared normalize_path/
 *   write_atomic helpers FhLoadOrderEditor (mods.cs) also relies on.
 */

namespace Fahrenheit.Tools.ModManager;

internal sealed class FhModManagerSettings {

    public string GameDirectory { get; set; } = FhModManagerSettingsStore.DEFAULT_GAME_DIRECTORY;

    // Null means "not overridden" - use the default derived from GameDirectory
    // (<game>/fahrenheit, <game>/fahrenheit/mods respectively; see
    // FhModScanner.resolve_paths). Set via the Settings modal's own Browse
    // buttons for these two locations (ui_settings_modal.cs).
    public string? FahrenheitDirectory { get; set; }
    public string? ModsDirectory       { get; set; }

    // Null means "not customized" - use FhTheme's DEFAULT_* value. Kept null rather
    // than snapshotting the current default, so a future rebuild that changes the
    // defaults doesn't leave an un-customized install pinned to the old palette.
    public FhThemeColor? AccentColor          { get; set; }
    public FhThemeColor? SuccessColor         { get; set; }
    public FhThemeColor? ErrorColor           { get; set; }
    public FhThemeColor? WarningColor         { get; set; }
    public FhThemeColor? BackgroundColor      { get; set; }
    public FhThemeColor? TextColor            { get; set; }
    public FhThemeColor? TextMutedColor       { get; set; }
    public FhThemeColor? FrameBackgroundColor { get; set; }
    public FhThemeColor? TitleBarColor        { get; set; }
}

internal static class FhModManagerSettingsStore {
    internal const string DEFAULT_GAME_DIRECTORY =
        @"C:\Program Files (x86)\Steam\steamapps\common\FINAL FANTASY FFX&FFX-2 HD Remaster";

    private static readonly JsonSerializerOptions _json_options = new() {
        WriteIndented = true
    };

    /*
     * fhmodmgr.exe is deployed to <game>/fahrenheit/bin/, alongside fhstage0.exe.
     * Fahrenheit and its tools are meant to be fully portable installs, so the
     * settings file lives beside the executable rather than in per-machine state
     * such as the Windows user profile. If it exists, it overrides the default
     * game directory above.
     */
    internal static string SettingsPath {
        get {
            return Path.Join( AppContext.BaseDirectory, "fhmodmgr.json");
        }
    }

    internal static FhModManagerSettings load(out string warning) {
        warning = "";

        if (!File.Exists(SettingsPath)) {
            return new();
        }

        try {
            string json = File.ReadAllText(SettingsPath);
            FhModManagerSettings? settings = JsonSerializer.Deserialize<FhModManagerSettings>(json, _json_options);

            if (settings == null || string.IsNullOrWhiteSpace(settings.GameDirectory)) {
                warning = "The saved settings were empty. Using the default game location.";
                return new();
            }

            settings.GameDirectory = normalize_path(settings.GameDirectory);
            return settings;
        }
        catch (Exception exception) {
            warning = $"The saved settings could not be read. Using the default game location.\n\n{exception.Message}";

            return new();
        }
    }

    internal static bool try_save(FhModManagerSettings settings, out string error) {
        error = "";

        try {
            // Defensive: normalize even though every current caller already
            // does, so a save can never persist a non-canonical path.
            settings.GameDirectory = normalize_path(settings.GameDirectory);
            string json = JsonSerializer.Serialize(settings,_json_options);
            write_atomic(SettingsPath, json);
            return true;
        }
        catch (Exception exception) {
            error = $"The settings could not be saved.\n\n{exception.Message}";
            return false;
        }
    }

    internal static string normalize_path(string path) {
        string cleaned_path = path.Trim().Trim('"');
        string full_path = Path.GetFullPath(cleaned_path);
        return Path.TrimEndingDirectorySeparator(full_path);
    }

    // Writes a file by staging it under a temporary name and renaming it into place,
    // so a crash or power loss mid-write can never leave `destination` truncated.
    // Shared with FhLoadOrderEditor, which persists the load order the same way.
    internal static void write_atomic(string destination, string contents) {
        string temporary_path = $"{ destination}.tmp";

        try {
            File.WriteAllText(temporary_path, contents);
            File.Move(temporary_path, destination, true);
        }
        finally {
            if (File.Exists(temporary_path)) {
                File.Delete(temporary_path);
            }
        }
    }
}
