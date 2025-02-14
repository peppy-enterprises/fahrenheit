namespace Fahrenheit.Core;

public sealed record FhDirLink {
    public string LinkSymbol { get; }
    public string LinkPath   { get; }

    public FhDirLink(string link_symbol, string path) {
        /* [fkelava 31/3/23 13:10]
         * On Windows, Path.Join uses backwards slashes which do not escape properly in JSON.
         * Since we cannot tell it to use AltDirectorySeparatorChar, we just patch the path _anyway_
         * because both Windows and Linux deal with forward slashes well in modern versions.
         */
        LinkSymbol = link_symbol;
        LinkPath   = path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        Directory.CreateDirectory(path);
    }
}

public static class FhRuntimeConst {
    private const string _dirname_bin     = "bin";
    private const string _dirname_modules = "modules";
    private const string _dirname_logs    = "logs";
    private const string _dirname_state   = "state";
    private const string _dirname_saves   = "saves";

    static FhRuntimeConst() {
        string cwd_parent = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName ??
                           throw new Exception("E_CWD_PARENT_DIR_UNIDENTIFIABLE");

        string path_bin     = Path.Join(cwd_parent, _dirname_bin);
        string path_modules = Path.Join(cwd_parent, _dirname_modules);
        string path_logs    = Path.Join(cwd_parent, _dirname_logs);
        string path_state   = Path.Join(cwd_parent, _dirname_state);
        string path_saves   = Path.Join(cwd_parent, _dirname_saves);

        Binaries  = new FhDirLink("$bin",     path_bin);
        Modules   = new FhDirLink("$modules", path_modules);
        Logs      = new FhDirLink("$logs",    path_logs);
        State     = new FhDirLink("$state",   path_state);
        Saves     = new FhDirLink("$saves",   path_saves);
    }

    public static readonly FhDirLink Binaries;
    public static readonly FhDirLink Modules;
    public static readonly FhDirLink Logs;
    public static readonly FhDirLink State;
    public static readonly FhDirLink Saves;
}