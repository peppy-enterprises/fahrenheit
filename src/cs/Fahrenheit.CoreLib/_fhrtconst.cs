using System.IO;

namespace Fahrenheit.CoreLib;

public sealed record FhDirLink
{
    public string LinkSymbol { get; }
    public string Path       { get; }

    public FhDirLink(string linkSymbol, string path)
    {
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

public static class FhRuntimeConst
{
    internal const string _clrPluginsDirName = "clrplugins";
    internal const string _confDirName       = "config";
    internal const string _diagLogDirName    = "diaglog";
}