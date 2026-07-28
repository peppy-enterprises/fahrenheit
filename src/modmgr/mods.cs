// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* Jobs:
 * - Define FhInstalledMod/FhModCatalog, the read-only model of an installed
 *   mod and the scan result the UI renders from.
 * - FhModScanner: scan the game's mods directory and load order file into a
 *   FhModCatalog, surfacing anything wrong (missing directories, malformed
 *   manifests, duplicate/blank load-order lines) as warnings rather than
 *   failing outright.
 * - FhLoadOrderEditor: read/write the load order file - enable/disable a mod,
 *   move it by one step, move it to an arbitrary position, or append a batch
 *   of new IDs - normalizing (trimmed, de-duplicated) the file as a side
 *   effect of every write.
 */

namespace Fahrenheit.Tools.ModManager;


internal sealed record FhInstalledMod(
    FhManifest Manifest,
    string DirectoryPath,
    bool DirectoryExists,
    bool ManifestExists,
    string ManifestError,
    int? LoadOrderIndex) {
    internal bool IsEnabled =>
        LoadOrderIndex.HasValue;

    internal bool HasValidManifest => DirectoryExists && ManifestExists && string.IsNullOrWhiteSpace(ManifestError);
}

/// <summary>
///    Lists of enabled and disabled mods, plus any warnings encountered while scanning the game directory.
/// </summary>
internal sealed record FhModCatalog(
    IReadOnlyList<FhInstalledMod> Enabled,
    IReadOnlyList<FhInstalledMod> Disabled,
    IReadOnlyList<string> Warnings,
    string ModsDirectory,
    string LoadOrderPath);

/// <summary>
///     Derives the `fahrenheit` and `fahrenheit/mods` paths.
/// </summary>
internal static class FhModScanner {
    internal static (string FahrenheitDirectory, string ModsDirectory) resolve_paths(string normalized_game_directory, string? fahrenheit_directory_override = null) {
        string fahrenheit_directory = string.IsNullOrWhiteSpace(fahrenheit_directory_override)
            ? Path.Join(normalized_game_directory, "fahrenheit")
            : fahrenheit_directory_override;

        string mods_directory = Path.Join(fahrenheit_directory, "mods");

        return (fahrenheit_directory, mods_directory);
    }

    /// <summary>
    ///     Scans the game directory for installed mods and returns a catalog of them.
    /// </summary>
    internal static FhModCatalog scan(string game_directory, string? fahrenheit_directory_override = null) {
        List<FhInstalledMod> enabled    = [];
        List<FhInstalledMod> disabled   = [];
        List<string> warnings           = [];

        string normalized_game_directory;

        try {
            normalized_game_directory = FhModManagerSettingsStore.normalize_path(game_directory);
        }
        catch (Exception exception) {
            warnings.Add($"The game directory is invalid: {exception.Message}");

            return new(enabled, disabled, warnings, "", "");
        }

        (string fahrenheit_directory, string mods_directory) = resolve_paths(normalized_game_directory, fahrenheit_directory_override);

        string load_order_path = Path.Join(mods_directory, "loadorder");

        // Check each level of the expected layout in turn so the warning always
        // names the first thing that's actually missing, rather than e.g. reporting
        // a missing mods folder when the real problem is no game directory at all.
        string? missing_directory_warning = null;

        if (!Directory.Exists(normalized_game_directory)) {
            missing_directory_warning = $"The game directory does not exist:\n{normalized_game_directory}";
        }
        else if (!Directory.Exists(fahrenheit_directory)) {
            missing_directory_warning = $"Fahrenheit is not installed at:\n{fahrenheit_directory}";
        }
        else if (!Directory.Exists(mods_directory)) {
            missing_directory_warning = $"The Fahrenheit mods directory does not exist:\n{mods_directory}";
        }

        if (missing_directory_warning != null) {
            warnings.Add(missing_directory_warning);

            return new(enabled, disabled, warnings, mods_directory, load_order_path);
        }

        string[] raw_load_order = [];

        if (File.Exists(load_order_path)) {
            try {
                raw_load_order = File.ReadAllLines(load_order_path);
            }
            catch (Exception exception) {
                warnings.Add($"The load order could not be read:\n{exception.Message}");
            }
        }
        else {
            warnings.Add($"The load order file does not exist:\n{load_order_path}");
        }

        HashSet<string> enabled_ids = new(StringComparer.OrdinalIgnoreCase);

        for (int index = 0; index < raw_load_order.Length; index++) {
            string raw_mod_id = raw_load_order[index];

            if (string.IsNullOrWhiteSpace(raw_mod_id)) {
                warnings.Add($"Load-order line {index + 1} is empty. Fahrenheit currently treats every line as a mod ID.");

                continue;
            }

            bool hasWhiteSpace = !string.Equals(raw_mod_id, raw_mod_id.Trim(), StringComparison.Ordinal);
            if (hasWhiteSpace) {
                warnings.Add( $"Load-order line {index + 1} contains leading or trailing whitespace: '{raw_mod_id}'");
            }

            string mod_id = raw_mod_id;

            if (!enabled_ids.Add(mod_id)) {
                warnings.Add($"The mod '{mod_id}' appears more than once in the load order.");
            }

            enabled.Add(_read_mod(mods_directory, mod_id, index));
        }

        IEnumerable<string> mod_directories = Directory.EnumerateDirectories( mods_directory, "*", SearchOption.TopDirectoryOnly);

        foreach (string mod_directory in mod_directories) {
            string mod_id = Path.GetFileName(mod_directory);

            if (enabled_ids.Contains(mod_id)) {
                continue;
            }

            disabled.Add(_read_mod(mods_directory, mod_id, null));
        }

        disabled.Sort(
            static (left, right) => StringComparer.OrdinalIgnoreCase.Compare(left.Manifest.Name, right.Manifest.Name));

        return new(enabled, disabled, warnings, mods_directory, load_order_path);
    }

    private static FhInstalledMod _read_mod(
        string mods_directory,
        string mod_id,
        int? load_order_index) {
        string mod_directory = Path.Join(
            mods_directory,
            mod_id);

        bool directory_exists = Directory.Exists(mod_directory);

        if (!directory_exists) {
            return new(new FhManifest(mod_id, mod_id, "", "", "", "", [], [], FhManifestFlags.NONE), mod_directory, false, false, "The mod directory does not exist.", load_order_index);
        }

        string manifest_path = Path.Join(mod_directory, $"{mod_id}.manifest.json");

        if (!File.Exists(manifest_path)) {
            return new(new FhManifest(mod_id, mod_id, "", "", "", "", [], [], FhManifestFlags.NONE), mod_directory, true, false, $"Manifest not found: {manifest_path}", load_order_index);
        }

        try {
            using JsonDocument document = JsonDocument.Parse(File.ReadAllText(manifest_path));

            JsonElement root = document.RootElement;

            // Version/Authors are purely cosmetic, so they default to blank and get
            // skipped by the UI; Name falls back to the directory-derived mod_id so
            // there's always something displayable even if the manifest omits it.
            string name = _read_string(root,"Name",mod_id);

            string version = _read_string(root,"Version","");

            string authors = _read_string(root,"Authors","");

            return new(new FhManifest(mod_id, mod_id, "", "", "", "", [], [], FhManifestFlags.NONE), mod_directory, true, true, "", load_order_index);
        }
        catch (Exception exception) {
            return new(new FhManifest(mod_id, mod_id, "", "", "", "", [], [], FhManifestFlags.NONE), mod_directory, true, true, exception.Message, load_order_index);
        }
    }

    private static string _read_string(
        JsonElement root,
        string property_name,
        string fallback) {
        if (!root.TryGetProperty(
                property_name,
                out JsonElement property)) {
            return fallback;
        }

        return property.ValueKind == JsonValueKind.String ? property.GetString() ?? fallback : fallback;
    }
}

internal static class FhLoadOrderEditor {
    // Enables or disables a mod in the load order. Deliberately not gated on
    // FhInstalledMod.HasValidManifest, unlike try_move/try_move_to below: this
    // only ever adds or removes `mod.Id` from the load order text file, which
    // doesn't depend on the mod actually having a valid (or even present)
    // manifest - and disabling has to work on an invalid mod regardless, since
    // it's the only way to clear a broken entry out of the load order from the UI.
    internal static bool try_set_enabled(
        string load_order_path,
        FhInstalledMod mod,
        bool enabled,
        out string error) {
        error = "";

        try {
            string? mods_directory =
                Path.GetDirectoryName(load_order_path);

            if (mods_directory == null || !Directory.Exists(mods_directory)) {
                error = "The Fahrenheit mods directory does not exist.";

                return false;
            }

            List<string> load_order = _read_normalized_load_order(load_order_path);

            bool already_enabled = false;

            foreach (string entry in load_order) {
                if (string.Equals(entry, mod.Manifest.Id, StringComparison.OrdinalIgnoreCase)) {
                    already_enabled = true;
                    break;
                }
            }

            if (enabled) {
                if (!already_enabled) {
                    /*
                     * New mods are appended to the end of the load order.
                     * Later EFL mods supersede earlier replacements.
                     */
                    load_order.Add(mod.Manifest.Id);
                }
            }
            else {
                load_order.RemoveAll(
                    entry => string.Equals(entry, mod.Manifest.Id, StringComparison.OrdinalIgnoreCase));
            }

            _write_load_order(load_order_path, load_order);

            return true;
        }
        catch (UnauthorizedAccessException) {
            error = "Windows denied permission to update loadorder file.";

            return false;
        }
        catch (Exception exception) {
            error = $"The load order could not be updated.\n\n{exception.Message}";

            return false;
        }
    }

    // Appends any of `mod_ids` not already present (case-insensitively) to the end
    // of the load order, preserving the order they're given in. Used when importing
    // a mod pack, so newly installed mods keep the pack's own relative ordering.
    internal static bool try_append_all(string load_order_path, IReadOnlyList<string> mod_ids, out string error) {
        error = "";

        try {
            List<string> load_order = _read_normalized_load_order(load_order_path);

            HashSet<string> existing = new(load_order, StringComparer.OrdinalIgnoreCase);

            foreach (string mod_id in mod_ids) {
                if (existing.Add(mod_id)) {
                    load_order.Add(mod_id);
                }
            }

            _write_load_order(load_order_path, load_order);

            return true;
        }
        catch (Exception exception) {
            error = $"The load order could not be updated.\n\n{exception.Message}";

            return false;
        }
    }

    // Reads the load order file into a de-duplicated, whitespace-trimmed list.
    // FhModScanner.scan() surfaces the same issues as warnings but leaves the file
    // untouched (it's read-only/diagnostic); every edit made through this class
    // instead cleans the file up as a side effect of writing it back, so both
    // try_set_enabled and try_move share this reader to normalize identically.
    private static List<string> _read_normalized_load_order(string load_order_path) {
        List<string> load_order = [];

        if (!File.Exists(load_order_path)) {
            return load_order;
        }

        HashSet<string> seen_mods = new(StringComparer.OrdinalIgnoreCase);

        foreach (string raw_entry in File.ReadAllLines(load_order_path)) {
            string entry = raw_entry.Trim();

            if (string.IsNullOrWhiteSpace(entry)) {
                continue;
            }

            if (seen_mods.Add(entry)) {
                load_order.Add(entry);
            }
        }

        return load_order;
    }

    // Joins the entries back into a loadorder file and writes it atomically.
    private static void _write_load_order(
        string load_order_path,
        IReadOnlyList<string> entries) {
        string contents = string.Join(Environment.NewLine, entries);

        if (entries.Count > 0) {
            contents += Environment.NewLine;
        }

        FhModManagerSettingsStore.write_atomic(load_order_path, contents);
    }

    // Moves the mod up or down by one position in the load order.
    internal static bool try_move(
        string load_order_path,
        FhInstalledMod mod,
        int direction,
        out string error) {
        if (direction != -1 && direction != 1) {
            error = "Load-order direction must be -1 or 1.";
            return false;
        }

        return _try_reposition(load_order_path, mod, current_index => current_index + direction, out error);
    }

    // Moves the mod to an arbitrary position in the load order - used to commit a
    // drag-and-drop reorder once (on mouse release), rather than the whole
    // gesture stepping through try_move repeatedly; see ui.cs's
    // _apply_pending_load_order_drop.
    internal static bool try_move_to(
        string load_order_path,
        FhInstalledMod mod,
        int target_index,
        out string error) {
        return _try_reposition(load_order_path, mod, _ => target_index, out error);
    }

    // Shared by try_move/try_move_to: finds the mod's current position, asks
    // `compute_target_index` where it should go from there, and - unless that's
    // where it already is - removes and reinserts it at the (clamped) result
    // before writing the load order back out.
    private static bool _try_reposition(
        string load_order_path,
        FhInstalledMod mod,
        Func<int, int> compute_target_index,
        out string error) {
        error = "";

        if (!mod.HasValidManifest) {
            error = $"The mod '{mod.Manifest.Id}' does not have a valid manifest.";

            return false;
        }

        try {
            if (!File.Exists(load_order_path)) {
                error = $"The Fahrenheit load-order file does not exist:\n\n{load_order_path}";

                return false;
            }

            List<string> load_order = _read_normalized_load_order(load_order_path);

            int current_index = load_order.FindIndex(
                entry => string.Equals(entry, mod.Manifest.Id, StringComparison.OrdinalIgnoreCase));

            if (current_index < 0) {
                error = $"The enabled mod '{mod.Manifest.Id}' was not found in the load order.";

                return false;
            }

            load_order.RemoveAt(current_index);

            int target_index = Math.Clamp(compute_target_index(current_index), 0, load_order.Count);

            load_order.Insert(target_index, mod.Manifest.Id);

            if (target_index == current_index) {
                // No actual change (e.g. already at the top/bottom, or dropped
                // back where it started) - skip the write.
                return true;
            }

            _write_load_order(load_order_path, load_order);

            return true;
        }
        catch (UnauthorizedAccessException) {
            error = "Windows denied permission to update the load order.";

            return false;
        }
        catch (Exception exception) {
            error = $"The load order could not be changed.\n\n{exception.Message}";

            return false;
        }
    }
}
