namespace Fahrenheit.Core;

public sealed record FhDirLink(
    string Symbol,
    string Path);

internal class FhPathFinder {
    private const string _dirname_bin     = "bin";
    private const string _dirname_modules = "modules";
    private const string _dirname_logs    = "logs";
    private const string _dirname_state   = "state";
    private const string _dirname_saves   = "saves";
    private const string _dirname_lang    = "lang";
    private const string _dirname_rsrc    = "resources";
    private const string _dirname_efl     = "efl";

    internal readonly FhDirLink Binaries;
    internal readonly FhDirLink Modules;
    internal readonly FhDirLink Logs;
    internal readonly FhDirLink State;
    internal readonly FhDirLink Saves;

    public FhPathFinder() {
        string cwd_parent = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName ??
                            throw new Exception("E_CWD_PARENT_DIR_UNIDENTIFIABLE");

        string path_bin     = Path.Join(cwd_parent, _dirname_bin);
        string path_modules = Path.Join(cwd_parent, _dirname_modules);
        string path_logs    = Path.Join(cwd_parent, _dirname_logs);
        string path_state   = Path.Join(cwd_parent, _dirname_state);
        string path_saves   = Path.Join(cwd_parent, _dirname_saves);

        Directory.CreateDirectory(path_bin);
        Directory.CreateDirectory(path_modules);
        Directory.CreateDirectory(path_logs);
        Directory.CreateDirectory(path_state);
        Directory.CreateDirectory(path_saves);

        Binaries = new FhDirLink("$bin",     path_bin);
        Modules  = new FhDirLink("$modules", path_modules);
        Logs     = new FhDirLink("$logs",    path_logs);
        State    = new FhDirLink("$state",   path_state);
        Saves    = new FhDirLink("$saves",   path_saves);
    }

    public FhModulePathInfo create_module_paths(string mod_name, string dll_name) {
        bool   is_runtime = mod_name.Equals("fhruntime", StringComparison.InvariantCultureIgnoreCase);
        string module_dir = is_runtime ? Binaries.Path : Path.Join(Modules.Path, mod_name);

        return new FhModulePathInfo(
            DllPath:    Path.Join(module_dir, $"{dll_name}.dll"),
            ConfigPath: Path.Join(module_dir, $"{dll_name}.config.json")
            );
    }

    public FhModPathInfo create_mod_paths(string mod_name) {
        bool   is_runtime = mod_name.Equals("fhruntime", StringComparison.InvariantCultureIgnoreCase);
        string module_dir = is_runtime ? Binaries.Path : Path.Join(Modules.Path, mod_name);

        return new FhModPathInfo(
            ManifestPath: Path.Join(module_dir, $"{mod_name}.manifest.json"),
            ModuleDir:    Directory.CreateDirectory(module_dir),
            ResourcesDir: Directory.CreateDirectory(Path.Join(module_dir, _dirname_rsrc)),
            EflDir:       Directory.CreateDirectory(Path.Join(module_dir, _dirname_efl)),
            LangDir:      Directory.CreateDirectory(Path.Join(module_dir, _dirname_lang)),
            StateDir:     Directory.CreateDirectory(Path.Join(State.Path, mod_name))
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
                     Replace(Modules .Symbol, Modules .Path).
                     Replace(Logs    .Symbol, Logs    .Path).
                     Replace(State   .Symbol, State   .Path).
                     Replace(Saves   .Symbol, Saves   .Path);
    }
}
