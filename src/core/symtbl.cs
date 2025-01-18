using System;

namespace Fahrenheit.Core;

public static class FhSymbolTableGenerator {
    public static string translate_callconv(string callConv) {
        return callConv switch {
            "__thiscall" => "[UnmanagedFunctionPointer(CallingConvention.ThisCall)]",
            "__cdecl"    => "[UnmanagedFunctionPointer(CallingConvention.Cdecl)]",
            "__stdcall"  => "[UnmanagedFunctionPointer(CallingConvention.StdCall)]",
            "__fastcall" => "[UnmanagedFunctionPointer(CallingConvention.FastCall)]",
            _            => throw new Exception("FH_E_FNTBL_CALLCONV_UNKNOWN")
        };
    }

    public static string translate_arg_type(string argType) {
        if (argType.Contains('*')) return "nint";

        return argType switch {
            "undefined"  => "nint",
            "undefined1" => "byte",
            "undefined2" => "ushort",
            "undefined4" => "uint",
            "undefined8" => "ulong",
            _            => throw new Exception("FH_E_FNTBL_ARGTYPE_UNKNOWN")
        };
    }
}