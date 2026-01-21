// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core;

/// <summary>
///     Provides access to a module's essential files and directories.
/// </summary>
internal sealed record FhModulePaths(
    string GlobalStatePath);

/// <summary>
///     Provides access to a mod's essential files and directories.
/// </summary>
public sealed record FhModPaths(
    string        ManifestPath,
    string        SettingsPath,
    DirectoryInfo ModDir,
    DirectoryInfo ResourcesDir,
    DirectoryInfo EflDir,
    DirectoryInfo LangDir);

/// <summary>
///     Resolves the paths of directories and files required by the framework.
/// </summary>
internal sealed class FhFinder {
    private const string _dirname_bin   = "bin";
    private const string _dirname_mods  = "mods";
    private const string _dirname_logs  = "logs";
    private const string _dirname_state = "state";
    private const string _dirname_saves = "saves";
    private const string _dirname_lang  = "lang";
    private const string _dirname_rsrc  = "resources";
    private const string _dirname_efl   = "efl";

    internal readonly DirectoryInfo Binaries;
    internal readonly DirectoryInfo Mods;
    internal readonly DirectoryInfo Logs;
    internal readonly DirectoryInfo State;
    internal readonly DirectoryInfo Saves;

    internal FhFinder() {
        string cwd_parent = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName ??
                            throw new Exception("E_CWD_PARENT_DIR_UNIDENTIFIABLE");

        Binaries = Directory.CreateDirectory(Path.Join(cwd_parent, _dirname_bin));
        Mods     = Directory.CreateDirectory(Path.Join(cwd_parent, _dirname_mods));
        Logs     = Directory.CreateDirectory(Path.Join(cwd_parent, _dirname_logs));
        State    = Directory.CreateDirectory(Path.Join(cwd_parent, _dirname_state));
        Saves    = Directory.CreateDirectory(Path.Join(cwd_parent, _dirname_saves));
    }

    /// <summary>
    ///     Gets the full path of the game's INI setting file.
    /// </summary>
    public string get_path_settings() {
        return Path.Join(
            Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            "SQUARE ENIX",
            "FINAL FANTASY X&X-2 HD Remaster",
            "GameSetting.ini");
    }

    /// <summary>
    ///     Returns path information for the DLL belonging to mod <paramref name="mod_name"/>.
    /// </summary>
    public string get_for_dll(string mod_name) {
        bool   is_runtime = mod_name.Equals("fhruntime", StringComparison.OrdinalIgnoreCase);
        string mod_dir    = is_runtime ? Binaries.FullName : Path.Join(Mods.FullName, mod_name);

        return Path.Join(mod_dir, $"{mod_name}.dll");
    }

    /// <summary>
    ///     Returns path information for mod <paramref name="mod_name"/>.
    /// </summary>
    public FhModPaths get_for_mod(string mod_name) {
        bool   is_runtime = mod_name.Equals("fhruntime", StringComparison.OrdinalIgnoreCase);
        string mod_dir    = is_runtime ? Binaries.FullName : Path.Join(Mods.FullName, mod_name);

        return new FhModPaths(
            ManifestPath: Path.Join(mod_dir, $"{mod_name}.manifest.json"),
            SettingsPath: Path.Join(mod_dir, $"{mod_name}.config.json"),
            ModDir:       Directory.CreateDirectory(mod_dir),
            ResourcesDir: Directory.CreateDirectory(Path.Join(mod_dir, _dirname_rsrc)),
            EflDir:       Directory.CreateDirectory(Path.Join(mod_dir, _dirname_efl)),
            LangDir:      Directory.CreateDirectory(Path.Join(mod_dir, _dirname_lang))
            );
    }

    /// <summary>
    ///     Returns path information for module <paramref name="module_name"/> of mod <paramref name="mod_name"/>.
    /// </summary>
    public FhModulePaths get_for_module(string mod_name, string module_name) {
        string global_state_dir  = Path.Join(State.FullName, "global", mod_name);
        string global_state_path = Path.Join(global_state_dir, module_name);

        Directory.CreateDirectory(global_state_dir);

        return new FhModulePaths(
            GlobalStatePath: global_state_path
            );
    }
}
