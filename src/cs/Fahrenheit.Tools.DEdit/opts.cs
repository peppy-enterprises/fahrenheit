using System;
using System.CommandLine;
using System.CommandLine.Binding;
using System.IO;

using Fahrenheit.CoreLib;

namespace Fahrenheit.Tools.DEdit;

internal enum FhDEditMode
{
    ReadCharsets = 1,
    Compile      = 2,
    Decompile    = 3
}

internal static class DEditConfig
{
    public static FhDEditMode Mode;
    public static string      SrcPath  = string.Empty;
    public static string      DestPath = string.Empty;
    
    public static DEditCharsetReaderConfig? CharsetReader;
    public static DEditDecompileConfig?     Decompile;

    public static void CLIRead(FhDEditArgs args)
    {
        Mode             = args.Mode;
        SrcPath          = args.SrcPath;
        DestPath         = Directory.Exists(args.DestPath) ? args.DestPath : throw new Exception("E_INVALID_DEST_DIR");

        CharsetReader = new DEditCharsetReaderConfig(args.DefaultNamespace);
        Decompile     = new DEditDecompileConfig(args.CharSet);
    }
}

internal sealed record DEditCharsetReaderConfig(string? DefaultNamespace);
internal sealed record DEditDecompileConfig(FhCharsetId CharSet);

internal class DEditArgsBinder : BinderBase<FhDEditArgs>
{
    private readonly Option<FhDEditMode> _optMode;
    private readonly Option<string>      _optDefNs;
    private readonly Option<string>      _optSrcPath;
    private readonly Option<string>      _optDestPath;
    private readonly Option<FhCharsetId> _optCharSet;

    public DEditArgsBinder(Option<FhDEditMode> optMode,
                           Option<string>      optDefNs, 
                           Option<string>      optFilePath,
                           Option<string>      optDestPath,
                           Option<FhCharsetId> optCharSet)
    {
        _optMode     = optMode;
        _optDefNs    = optDefNs;
        _optSrcPath  = optFilePath;
        _optDestPath = optDestPath;
        _optCharSet  = optCharSet;
    }

    protected override FhDEditArgs GetBoundValue(BindingContext bindingContext)
    {
        // Mandatory
        FhDEditMode mode     = bindingContext.ParseResult.GetValueForOption(_optMode);
        string      srcPath  = bindingContext.ParseResult.GetValueForOption(_optSrcPath) ?? throw new Exception("E_CLI_ARG_NULL");
        string      destPath = bindingContext.ParseResult.GetValueForOption(_optDestPath) ?? throw new Exception("E_CLI_ARG_NULL");

        // ReadCharsets mandatory
        string? defNs = bindingContext.ParseResult.GetValueForOption(_optDefNs);

        // Decompile mandatory
        FhCharsetId charSet = bindingContext.ParseResult.GetValueForOption(_optCharSet);

        return new FhDEditArgs(mode, srcPath, destPath, defNs, charSet);
    }
}

internal sealed record FhDEditArgs(FhDEditMode Mode,
                                   string      SrcPath,
                                   string      DestPath,
                                   string?     DefaultNamespace,
                                   FhCharsetId CharSet);