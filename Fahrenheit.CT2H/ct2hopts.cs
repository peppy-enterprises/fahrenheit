using System;
using System.CommandLine;
using System.CommandLine.Binding;

namespace Fahrenheit.CT2H;

internal static class CT2HConfig
{
    public static string DefaultNamespace = string.Empty;
    public static string FilePath         = string.Empty;

    public static void CLIRead(FhCT2HArgs args)
    {
        DefaultNamespace = args.DefaultNamespace;
        FilePath         = args.FilePath;
    }
}

internal sealed record FhCT2HArgs(string DefaultNamespace,
                                  string FilePath);

internal class CT2HArgsBinder : BinderBase<FhCT2HArgs>
{
    private readonly Option<string> _optDefaultNamespace;
    private readonly Option<string> _optFilePath;

    public CT2HArgsBinder(Option<string> optDefaultNamespace, 
                          Option<string> optFilePath)
    {
        _optDefaultNamespace = optDefaultNamespace;
        _optFilePath         = optFilePath;
    }

    protected override FhCT2HArgs GetBoundValue(BindingContext bindingContext)
    {
        string defaultNamespace = bindingContext.ParseResult.GetValueForOption(_optDefaultNamespace) ?? throw new Exception("E_CLI_ARG_NULL");
        string filePath         = bindingContext.ParseResult.GetValueForOption(_optFilePath) ?? throw new Exception("E_CLI_ARG_NULL");

        return new FhCT2HArgs(defaultNamespace, filePath);
    }
}
