namespace Fahrenheit.Core;

public sealed record FhGhidraSymbolDecl(
    string Name,
    string Location,
    string Signature,
    string Source,
    string Type,
    string FuncName,
    string CallConv,
    string Namespace);

public static class FhSymbolTableGenerator {
    private static bool _is_interpretable(FhGhidraSymbolDecl symbol) {
        return symbol.Type      == "Function"     &&
               symbol.Source    == "USER_DEFINED" &&
               symbol.Namespace == "Global"; // might be a removable restriction
    }

    private static string _unescape(string symtable_json) {
        return symtable_json.Replace("\\,",  ",")
                            .Replace("\"\\", "");
    }

    private static string _translate_callconv(string call_conv) {
        return call_conv switch {
            "__thiscall" => "[UnmanagedFunctionPointer(CallingConvention.ThisCall)]",
            "__cdecl"    => "[UnmanagedFunctionPointer(CallingConvention.Cdecl)]",
            "__stdcall"  => "[UnmanagedFunctionPointer(CallingConvention.StdCall)]",
            "__fastcall" => "[UnmanagedFunctionPointer(CallingConvention.FastCall)]",
            _            => throw new Exception("FH_E_FNTBL_CALLCONV_UNKNOWN")
        };
    }

    private static string _translate_arg_type(string arg_type) {
        if (arg_type.Contains('*')) return "nint";

        return arg_type switch {
            "undefined"  => "nint",
            "undefined1" => "byte",
            "undefined2" => "ushort",
            "undefined4" => "uint",
            "undefined8" => "ulong",
            _            => throw new Exception("FH_E_FNTBL_ARGTYPE_UNKNOWN")
        };
    }

    public static bool _try_parse_symtable(string symtable_full_path) {
        try {
            FhGhidraSymbolDecl[] symbol_declarations = JsonSerializer.Deserialize<FhGhidraSymbolDecl[]>(File.ReadAllText(symtable_full_path)) ?? [];
            return true;
        }
        catch (Exception e) {
            FhLog.Error(e.Message);
            return false;
        }
    }
}