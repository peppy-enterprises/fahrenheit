using System;
using System.IO;

namespace Fahrenheit.CoreLib;

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
    internal const string _binDirName      = "bin";
    internal const string _clrHooksDirName = "clrhooks";
    internal const string _cppHooksDirName = "cpphooks";
    internal const string _modulesDirName  = "modules";
    internal const string _confDirName     = "config";
    internal const string _diagLogDirName  = "diaglog";
    internal const string _rsrcDirName     = "rsrc";
    internal const string _miscDirName     = "misc";

    static FhRuntimeConst() {
        string cwdParent = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName ??
                           throw new Exception("E_CWD_PARENT_DIR_UNIDENTIFIABLE");

        string binDirPath      = Path.Join(cwdParent, _binDirName);
        string clrHooksDirPath = Path.Join(cwdParent, _clrHooksDirName);
        string cppHooksDirPath = Path.Join(cwdParent, _cppHooksDirName);
        string modulesDirPath  = Path.Join(cwdParent, _modulesDirName);
        string confDirPath     = Path.Join(cwdParent, _confDirName);
        string diagLogDirPath  = Path.Join(cwdParent, _diagLogDirName);
        string rsrcDirPath     = Path.Join(cwdParent, _rsrcDirName);
        string miscDirPath     = Path.Join(cwdParent, _miscDirName);
        string byRunDirPath    = Path.Join(miscDirPath, $"run_{FhUtil.get_timestamp_string()}");

        BinDir      = new FhDirLink("$bindir", binDirPath);
        CLRHooksDir = new FhDirLink("$clrhookdir", clrHooksDirPath);
        CPPHooksDir = new FhDirLink("$cpphookdir", cppHooksDirPath);
        ModulesDir  = new FhDirLink("$modulesdir", modulesDirPath);
        ConfigDir   = new FhDirLink("$confdir", confDirPath);
        DiagLogDir  = new FhDirLink("$diaglogdir", diagLogDirPath);
        RsrcDir     = new FhDirLink("$rsrcdir", rsrcDirPath);
        MiscDir     = new FhDirLink("$miscdir", miscDirPath);
        ByRunDir    = new FhDirLink("$rundir", byRunDirPath);
    }

    public static readonly FhDirLink BinDir;
    public static readonly FhDirLink CLRHooksDir;
    public static readonly FhDirLink CPPHooksDir;
    public static readonly FhDirLink ModulesDir;
    public static readonly FhDirLink ConfigDir;
    public static readonly FhDirLink DiagLogDir;
    public static readonly FhDirLink RsrcDir;
    public static readonly FhDirLink MiscDir;
    public static readonly FhDirLink ByRunDir;
}