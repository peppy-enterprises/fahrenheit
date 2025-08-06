namespace Fahrenheit.Core;

/// <summary>
///     Contains path information required for the loader to process a DLL in the <see cref="FhManifest.DllList"/> of a Fahrenheit mod.
/// </summary>
internal sealed record FhLoaderPathInfo(
    string DllPath,
    string SettingsPath);

/// <summary>
///     Contains path information required for the mod controller and Fahrenheit runtime to handle a module's lifecycle.
/// </summary>
internal sealed record FhModulePathInfo(
    string GlobalStatePath
    );

/// <summary>
///     Contains path information required for the mod controller and Fahrenheit runtime to handle a mod's lifecycle.
/// </summary>
public sealed record FhModPathInfo(
    string        ManifestPath,
    DirectoryInfo ModuleDir,
    DirectoryInfo ResourcesDir,
    DirectoryInfo EflDir,
    DirectoryInfo LangDir,
    DirectoryInfo StateDir);

/// <summary>
///     Maps a <paramref name="Path"/> on disk to a shorthand <paramref name="Symbol"/>.
///     Instances of the symbol can then be substituted by the full path.
/// </summary>
public sealed record FhDirLink(
    string Symbol,
    string Path);

/// <summary>
///     Internally resolves the paths of certain well-known directories and files required by the framework.
/// </summary>
internal class FhPathFinder {
    private const string _dirname_bin   = "bin";
    private const string _dirname_mods  = "mods";
    private const string _dirname_logs  = "logs";
    private const string _dirname_state = "state";
    private const string _dirname_saves = "saves";
    private const string _dirname_lang  = "lang";
    private const string _dirname_rsrc  = "resources";
    private const string _dirname_efl   = "efl";

    internal readonly FhDirLink Binaries;
    internal readonly FhDirLink Mods;
    internal readonly FhDirLink Logs;
    internal readonly FhDirLink State;
    internal readonly FhDirLink Saves;

    public FhPathFinder() {
        string cwd_parent = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName ??
                            throw new Exception("E_CWD_PARENT_DIR_UNIDENTIFIABLE");

        string path_bin   = Path.Join(cwd_parent, _dirname_bin);
        string path_mods  = Path.Join(cwd_parent, _dirname_mods);
        string path_logs  = Path.Join(cwd_parent, _dirname_logs);
        string path_state = Path.Join(cwd_parent, _dirname_state);
        string path_saves = Path.Join(cwd_parent, _dirname_saves);

        Directory.CreateDirectory(path_bin);
        Directory.CreateDirectory(path_mods);
        Directory.CreateDirectory(path_logs);
        Directory.CreateDirectory(path_state);
        Directory.CreateDirectory(path_saves);

        Binaries = new FhDirLink("$bin",   path_bin);
        Mods     = new FhDirLink("$mods",  path_mods);
        Logs     = new FhDirLink("$logs",  path_logs);
        State    = new FhDirLink("$state", path_state);
        Saves    = new FhDirLink("$saves", path_saves);
    }

    public string get_save_path_for_index(int save_index) {
        bool   is_ffx         = FhGlobal.game_type == FhGameType.FFX;
        string save_subfolder = is_ffx ? "FINAL FANTASY X" : "FINAL FANTASY X-2";
        string save_prefix    = is_ffx ? "ffx"             : "ffx2";
        string save_name      = $"{save_prefix}_{save_index:000}";

        return Path.Join(
            Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            "SQUARE ENIX",
            "FINAL FANTASY X&X-2 HD Remaster",
            save_subfolder,
            save_name);
    }

    public string get_save_name_for_index(int save_index) {
        bool   is_ffx      = FhGlobal.game_type == FhGameType.FFX;
        string save_prefix = is_ffx ? "ffx" : "ffx2";
        string save_name   = $"{save_prefix}_{save_index:000}";

        return save_name;
    }

    public FhLoaderPathInfo create_loader_paths(string mod_name, string dll_name) {
        bool   is_runtime = mod_name.Equals("fhruntime", StringComparison.InvariantCultureIgnoreCase);
        string module_dir = is_runtime ? Binaries.Path : Path.Join(Mods.Path, mod_name);

        return new FhLoaderPathInfo(
            DllPath:      Path.Join(module_dir, $"{dll_name}.dll"),
            SettingsPath: Path.Join(module_dir, $"{dll_name}.config.json")
            );
    }

    public FhModPathInfo create_mod_paths(string mod_name) {
        bool   is_runtime = mod_name.Equals("fhruntime", StringComparison.InvariantCultureIgnoreCase);
        string module_dir = is_runtime ? Binaries.Path : Path.Join(Mods.Path, mod_name);

        return new FhModPathInfo(
            ManifestPath: Path.Join(module_dir, $"{mod_name}.manifest.json"),
            ModuleDir:    Directory.CreateDirectory(module_dir),
            ResourcesDir: Directory.CreateDirectory(Path.Join(module_dir, _dirname_rsrc)),
            EflDir:       Directory.CreateDirectory(Path.Join(module_dir, _dirname_efl)),
            LangDir:      Directory.CreateDirectory(Path.Join(module_dir, _dirname_lang)),
            StateDir:     Directory.CreateDirectory(Path.Join(State.Path, mod_name))
            );
    }

    public FhModulePathInfo create_module_paths(string mod_name, string module_name) {
        string global_state_dir  = Path.Join(State.Path, mod_name, "global");
        string global_state_path = Path.Join(global_state_dir, module_name);

        Directory.CreateDirectory(global_state_dir);

        return new FhModulePathInfo(
            GlobalStatePath: global_state_path
            );
    }

    /// <summary>
    ///     Replaces instances of well-known directory substitution strings in a input string
    ///     with the actual locations of these directories. This is provided so you can use the same, well-defined
    ///     file path relative to the binary for any file across all supported platforms.
    /// <para></para>
    ///     e.g. $resdir (Linux) -> /opt/fahrenheit/resources, $resdir (Windows) -> C:\Users\USER1\fahrenheit\resources
    /// </summary>
    public string fix_paths(string input) {
        return input.Replace(Binaries.Symbol, Binaries.Path).
                     Replace(Mods    .Symbol, Mods    .Path).
                     Replace(Logs    .Symbol, Logs    .Path).
                     Replace(State   .Symbol, State   .Path).
                     Replace(Saves   .Symbol, Saves   .Path);
    }
}
