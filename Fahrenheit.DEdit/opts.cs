using System;
using System.CommandLine;
using System.CommandLine.Binding;
using System.IO;

namespace Fahrenheit.DEdit;

internal enum FhDEditMode
{
    ReadCharsets = 1,
    Compile      = 2,
    Decompile    = 3
}

internal static class DEditConfig
{
    public static FhDEditMode Mode;
    public static string      DefaultNamespace = string.Empty;
    public static string      SrcPath          = string.Empty;
    public static string      DestPath         = string.Empty;

    public static void CLIRead(FhDEditArgs args)
    {
        Mode             = args.Mode;
        DefaultNamespace = args.DefaultNamespace;
        SrcPath          = args.SrcPath;
        DestPath         = Directory.Exists(args.DestPath) ? args.DestPath : throw new Exception("E_INVALID_DEST_DIR");
    }
}

internal sealed record FhDEditArgs(FhDEditMode Mode,
                                   string      DefaultNamespace,
                                   string      SrcPath,
                                   string      DestPath);

internal class DEditArgsBinder : BinderBase<FhDEditArgs>
{
    private readonly Option<FhDEditMode> _optMode;
    private readonly Option<string>      _optDefNs;
    private readonly Option<string>      _optSrcPath;
    private readonly Option<string>      _optDestPath;

    public DEditArgsBinder(Option<FhDEditMode> optMode,
                           Option<string>      optDefNs, 
                           Option<string>      optFilePath,
                           Option<string>      optDestPath)
    {
        _optMode     = optMode;
        _optDefNs    = optDefNs;
        _optSrcPath  = optFilePath;
        _optDestPath = optDestPath;
    }

    protected override FhDEditArgs GetBoundValue(BindingContext bindingContext)
    {
        FhDEditMode mode     = bindingContext.ParseResult.GetValueForOption(_optMode);
        string      defNs    = bindingContext.ParseResult.GetValueForOption(_optDefNs) ?? throw new Exception("E_CLI_ARG_NULL");
        string      srcPath  = bindingContext.ParseResult.GetValueForOption(_optSrcPath) ?? throw new Exception("E_CLI_ARG_NULL");
        string      destPath = bindingContext.ParseResult.GetValueForOption(_optDestPath) ?? throw new Exception("E_CLI_ARG_NULL");

        return new FhDEditArgs(mode, defNs, srcPath, destPath);
    }
}
