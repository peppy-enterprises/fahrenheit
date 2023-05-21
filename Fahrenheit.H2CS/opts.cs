using System;
using System.CommandLine;
using System.CommandLine.Binding;
using System.IO;

namespace Fahrenheit.H2CS;

internal static class H2CSConfig
{
    public static string DefaultNamespace = string.Empty;
    public static string SrcPath          = string.Empty;
    public static string DestPath         = string.Empty;

    public static void CLIRead(FhH2CSArgs args)
    {
        DefaultNamespace = args.DefaultNamespace;
        SrcPath          = args.SrcPath;
        DestPath         = Directory.Exists(args.DestPath) ? args.DestPath : throw new Exception("E_INVALID_DEST_DIR");
    }
}

internal sealed record FhH2CSArgs(string DefaultNamespace,
                                   string SrcPath,
                                   string DestPath);

internal class H2CSArgsBinder : BinderBase<FhH2CSArgs>
{
    private readonly Option<string> _optDefNs;
    private readonly Option<string> _optSrcPath;
    private readonly Option<string> _optDestPath;

    public H2CSArgsBinder(Option<string> optDefNs, 
                          Option<string> optFilePath,
                          Option<string> optDestPath)
    {
        _optDefNs    = optDefNs;
        _optSrcPath  = optFilePath;
        _optDestPath = optDestPath;
    }

    protected override FhH2CSArgs GetBoundValue(BindingContext bindingContext)
    {
        string defNs    = bindingContext.ParseResult.GetValueForOption(_optDefNs) ?? throw new Exception("E_CLI_ARG_NULL");
        string srcPath  = bindingContext.ParseResult.GetValueForOption(_optSrcPath) ?? throw new Exception("E_CLI_ARG_NULL");
        string destPath = bindingContext.ParseResult.GetValueForOption(_optDestPath) ?? throw new Exception("E_CLI_ARG_NULL");

        return new FhH2CSArgs(defNs, srcPath, destPath);
    }
}
