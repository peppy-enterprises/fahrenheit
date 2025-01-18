using System;
using System.IO;

namespace Fahrenheit.Core;

public sealed record FhDirLink {
    public string LinkSymbol { get; }
    public string Path       { get; }

    public FhDirLink(string linkSymbol, string path) {
        LinkSymbol = linkSymbol;

        /* [fkelava 31/3/23 13:10]
         * On Windows, Path.Join uses backwards slashes which do not escape properly in JSON.
         * Since we cannot tell it to use AltDirectorySeparatorChar, we just patch the path _anyway_
         * because both Windows and Linux deal with forward slashes well in modern versions.
         */
        Path = path.Replace(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);

        Directory.CreateDirectory(path);
    }
}

public static class FhRuntimeConst {
    internal const string _binDirName     = "bin";
    internal const string _modulesDirName = "modules";
    internal const string _logsDirName    = "logs";
    internal const string _rsrcDirName    = "rsrc";

    static FhRuntimeConst() {
        string cwdParent = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName ??
                           throw new Exception("E_CWD_PARENT_DIR_UNIDENTIFIABLE");

        string binDirPath     = Path.Join(cwdParent, _binDirName);
        string modulesDirPath = Path.Join(cwdParent, _modulesDirName);
        string logsDirPath    = Path.Join(cwdParent, _logsDirName);
        string rsrcDirPath    = Path.Join(cwdParent, _rsrcDirName);

        BinDir     = new FhDirLink("$bindir",     binDirPath);
        ModulesDir = new FhDirLink("$modulesdir", modulesDirPath);
        DiagLogDir = new FhDirLink("$diaglogdir", logsDirPath);
        RsrcDir    = new FhDirLink("$rsrcdir",    rsrcDirPath);
    }

    public static readonly FhDirLink BinDir;
    public static readonly FhDirLink ModulesDir;
    public static readonly FhDirLink DiagLogDir;
    public static readonly FhDirLink RsrcDir;
}