using System;
using System.CommandLine;
using System.CommandLine.Binding;
using System.IO;

namespace Fahrenheit.Tools.H2CS;

internal static class H2CSConfig
{
    public static string DefaultNamespace = string.Empty;
    public static bool   EmitPrologue     = false;
    public static bool   EmitDeduplicated = true;
    public static string TypeAliasName    = string.Empty;
    public static string SrcPath          = string.Empty;
    public static string DestPath         = string.Empty;

    public static void CLIRead(FhH2CSArgs args)
    {
        DefaultNamespace = args.DefaultNamespace;
        EmitPrologue     = args.EmitPrologue;
        EmitDeduplicated = args.EmitDeduplicated;
        TypeAliasName    = args.TypeAliasName;
        SrcPath          = args.SrcPath;
        DestPath         = Directory.Exists(args.DestPath) ? args.DestPath : throw new Exception("E_INVALID_DEST_DIR");
    }
}

internal sealed record FhH2CSArgs(string DefaultNamespace,
                                  bool   EmitPrologue,
                                  bool   EmitDeduplicated,
                                  string TypeAliasName,
                                  string SrcPath,
                                  string DestPath);

internal class H2CSArgsBinder : BinderBase<FhH2CSArgs>
{
    private readonly Option<string> _optDefNs;
    private readonly Option<bool>   _optEmitProlog;
    private readonly Option<bool>   _optEmitDedup;
    private readonly Option<string> _optTypeName;
    private readonly Option<string> _optSrcPath;
    private readonly Option<string> _optDestPath;

    public H2CSArgsBinder(Option<string> optDefNs, 
                          Option<bool>   optEmitProlog,
                          Option<bool>   optEmitDedup,
                          Option<string> optTypeName,
                          Option<string> optFilePath,
                          Option<string> optDestPath)
    {
        _optDefNs      = optDefNs;
        _optEmitProlog = optEmitProlog;
        _optEmitDedup  = optEmitDedup;
        _optTypeName   = optTypeName;
        _optSrcPath    = optFilePath;
        _optDestPath   = optDestPath;
    }

    protected override FhH2CSArgs GetBoundValue(BindingContext bindingContext)
    {
        string defNs      = bindingContext.ParseResult.GetValueForOption(_optDefNs) ?? throw new Exception("E_CLI_ARG_NULL");
        bool   emitProlog = bindingContext.ParseResult.GetValueForOption(_optEmitProlog);
        bool   emitDedup  = bindingContext.ParseResult.GetValueForOption(_optEmitDedup);
        string typeName   = bindingContext.ParseResult.GetValueForOption(_optTypeName) ?? throw new Exception("E_CLI_ARG_NULL");
        string srcPath    = bindingContext.ParseResult.GetValueForOption(_optSrcPath) ?? throw new Exception("E_CLI_ARG_NULL");
        string destPath   = bindingContext.ParseResult.GetValueForOption(_optDestPath) ?? throw new Exception("E_CLI_ARG_NULL");

        return new FhH2CSArgs(defNs, emitProlog, emitDedup, typeName, srcPath, destPath);
    }
}
