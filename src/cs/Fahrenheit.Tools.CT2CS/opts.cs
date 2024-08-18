using System;
using System.CommandLine;
using System.CommandLine.Binding;
using System.IO;

namespace Fahrenheit.Tools.CT2CS;

internal static class CT2CSConfig {
    public static string DefaultNamespace = string.Empty;
    public static string SrcPath          = string.Empty;
    public static string DestPath         = string.Empty;

    public static void CLIRead(FhCT2CSArgs args) {
        DefaultNamespace = args.DefaultNamespace;
        SrcPath          = args.SrcPath;
        DestPath         = Directory.Exists(args.DestPath) ? args.DestPath : throw new Exception("E_INVALID_DEST_DIR");
    }
}

internal sealed record FhCT2CSArgs(string DefaultNamespace,
                                   string SrcPath,
                                   string DestPath);

internal class CT2CSArgsBinder : BinderBase<FhCT2CSArgs> {
    private readonly Option<string> _optDefNs;
    private readonly Option<string> _optSrcPath;
    private readonly Option<string> _optDestPath;

    public CT2CSArgsBinder(Option<string> optDefNs, 
                           Option<string> optFilePath,
                           Option<string> optDestPath) {
        _optDefNs    = optDefNs;
        _optSrcPath  = optFilePath;
        _optDestPath = optDestPath;
    }

    protected override FhCT2CSArgs GetBoundValue(BindingContext bindingContext) {
        string defNs    = bindingContext.ParseResult.GetValueForOption(_optDefNs) ?? throw new Exception("E_CLI_ARG_NULL");
        string srcPath  = bindingContext.ParseResult.GetValueForOption(_optSrcPath) ?? throw new Exception("E_CLI_ARG_NULL");
        string destPath = bindingContext.ParseResult.GetValueForOption(_optDestPath) ?? throw new Exception("E_CLI_ARG_NULL");

        return new FhCT2CSArgs(defNs, srcPath, destPath);
    }
}
