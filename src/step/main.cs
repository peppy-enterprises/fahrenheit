// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* [fkelava 17/5/23 02:48]
 * A small tool to emit (hopefully) valid C# code from Ghidra symbol JSONs.
 */

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;

using CsvHelper;
using CsvHelper.Configuration.Attributes;

namespace Fahrenheit.Tools.STEP;

// In Ghidra, select fields:
// Name, Location, Function Signature, Symbol Source,
// Symbol Type, Function Name, Call Conv, Namespace
internal struct FhFuncDecl {
    [Index(0)] public string Name      { get; set; }
    [Index(1)] public string Location  { get; set; }
    [Index(2)] public string Signature { get; set; }
    [Index(3)] public string Source    { get; set; }
    [Index(4)] public string Type      { get; set; }
    [Index(5)] public string FuncName  { get; set; }
    [Index(6)] public string CallConv  { get; set; }
    [Index(7)] public string Namespace { get; set; }
}

// In Ghidra, select fields: Name, Location, Type, Data Type, Namespace, Source
// Filter by: Source - User Defined, Type - Data Label
internal struct FhDataLabelDecl {
    [Index(0)] public string Name      { get; set; }
    [Index(1)] public string Location  { get; set; }
    [Index(2)] public string Type      { get; set; }
    [Index(3)] public string DataType  { get; set; }
    [Index(4)] public string Namespace { get; set; }
    [Index(5)] public string Source    { get; set; }
}

internal ref struct FhFuncSignatureData {
    public ReadOnlySpan<char>    ReturnType;
    public ReadOnlySpan<char>    FunctionName;
    public List<FhFuncParameter> Parameters;
}

internal struct FhFuncParameter(ReadOnlySpan<char> type, ReadOnlySpan<char> name) {
    public string ParameterType = new string(type);
    public string ParameterName = new string(name);
}

internal static class Program {
    private static Dictionary<string, string> _type_map = [];

    private static void Main(string[] args) {
        Option<string>   opt_src_path     = new Option<string>("--src")
            { Description = "Set the path to the directory containing the exported symbol files." };
        Option<string>   opt_dest_path    = new Option<string>("--dest")
            { Description = "Set the folder where the C# file should be written." };
        Option<string>   opt_typemap_path = new Option<string>("--map")
            { Description = "Set the path to a Ghidra -> Fh type map." };
        Option<FhGameId> opt_game         = new Option<FhGameId>("--game")
            { Description = "Declare which game STEP is generating for." };

        opt_src_path    .Required = true;
        opt_dest_path   .Required = true;
        opt_game        .Required = true;
        opt_typemap_path.Required = false;

        RootCommand cmd_root = new RootCommand("Process a Ghidra symbol table and create a C# code file.");

        cmd_root.Options.Add(opt_src_path);
        cmd_root.Options.Add(opt_dest_path);
        cmd_root.Options.Add(opt_typemap_path);
        cmd_root.Options.Add(opt_game);

        cmd_root.SetAction(parse_result => _emit_symtable(
            parse_result.GetRequiredValue(opt_src_path),
            parse_result.GetRequiredValue(opt_dest_path),
            parse_result.GetValue        (opt_typemap_path) ?? "",
            parse_result.GetRequiredValue(opt_game)
            ));

        ParseResult parse_result = cmd_root.Parse(args);
        parse_result.Invoke();
    }

    /// <summary>
    ///     Determines whether a specific function declaration provided by Ghidra should be interpreted.
    /// </summary>
    /// <param name="function">The function declaration to be checked.</param>
    /// <returns>Whether the provided function declaration should be interpreted.</returns>
    private static bool _should_interpret(FhFuncDecl function) {
        return function is {
                   Type:      "Function",
                   Namespace: "Global", // Exclude potentially proprietary symbols
               } &&
               !function.Name.Contains("operator") && // ignore operator.new, operator.delete
               !function.Name.Contains('@')        && // ignore Unwind@{ADDR}, Catch_All@{ADDR} thunks
               !function.Signature.Contains('.')   && // ignore vararg functions
               !function.Signature.Contains(':')   && // ignore anything that even vaguely resembles a C++ namespace
               !function.Signature.Contains('`')   && // ignore vector ctors/dtors
               !function.Signature.Contains('<')   && // ignore template specializations
               !function.Signature.Contains('-')   &&
               !function.Signature.Contains('+');     // ignore descriptively labeled but not authoritatively named functions
    }

    /// <summary>
    ///     Determines whether a specific global declaration provided by Ghidra should be interpreted.
    /// </summary>
    /// <param name="data_label">The global declaration to be checked.</param>
    /// <returns>Whether the provided global declaration should be interpreted.</returns>
    private static bool _should_interpret(FhDataLabelDecl data_label) {
        return !data_label.Name.Contains('+'); // ignore descriptively labeled but not authoritatively named globals
    }

    /// <summary>
    ///     Modifies provided functions, fixing formatting and undoing Ghidra's CSV escapes.
    /// </summary>
    /// <param name="functions">Ghidra-provided function declarations to format and unescape.</param>
    private static void _format_functions(Span<FhFuncDecl> functions) {
        for (int i = 0; i < functions.Length; i++) {
            functions[i].Signature = functions[i].Signature
                .Replace(" *"   , "*") // Ghidra "float * param_1" -> "float* param_1"
                .Replace("\\,"  , ",") // Ghidra CSV unescape
                .Replace("\"\\" , "" );
        }
    }

    /// <summary>
    ///     Convert from a C++/Ghidra calling convention specifier to the equivalent C# attribute for delegates.
    /// </summary>
    /// <param name="call_conv">The C++/Ghidra-style calling convention specifier.</param>
    /// <returns>An equivalent C# attribute applicable to delegates.</returns>
    /// <exception cref="ArgumentException">Thrown if the C++/Ghidra-style calling convention specifier is not recognized.</exception>
    private static ReadOnlySpan<char> _emit_callconv_attr(ReadOnlySpan<char> call_conv) {
        return call_conv switch {
            "__thiscall" => "[UnmanagedFunctionPointer(CallingConvention.ThisCall)]",
            "__cdecl"    => "[UnmanagedFunctionPointer(CallingConvention.Cdecl)]",
            "__stdcall"  => "[UnmanagedFunctionPointer(CallingConvention.StdCall)]",
            "__fastcall" => "[UnmanagedFunctionPointer(CallingConvention.FastCall)]",
            "unknown"    => "[UnmanagedFunctionPointer(CallingConvention.Cdecl)]",
            _ => throw new ArgumentException($"Encountered an unknown calling convention `{call_conv}` while parsing functions."),
        };
    }

    /// <summary>
    ///     Maps a Ghidra-provided type using the user-defined typemap.
    /// </summary>
    /// <example>
    /// Assuming Ghidra's <c>undefined4</c> type is mapped to C#'s <c>uint</c>,
    /// calling this function with <c>"undefined4"</c> will return <c>"uint"</c>.
    /// </example>
    /// <param name="type">The string representation of a Ghidra parameter type.</param>
    /// <returns>The mapped parameter type.<br/>Returns <c>"nint"</c> if the given Ghidra type isn't mapped.</returns>
    private static ReadOnlySpan<char> _map_type(string type) {
        return _type_map.GetValueOrDefault(type, "nint");
    }

    /// <summary>
    ///     Applies the user-defined type map to a parameter type provided by Ghidra.<br/>
    /// Unlike <see cref="_map_type"/>, accounts for <c>void</c>.
    /// </summary>
    /// <param name="return_type">The string representation of a Ghidra return type.</param>
    /// <returns>The mapped return type.</returns>
    private static ReadOnlySpan<char> _map_return_type(string return_type) {
        return return_type switch {
            "void"      => "void",
            "undefined" => "void",
            _           => _map_type(return_type),
        };
    }

    /// <summary>
    ///     Modifies a parameter name to not conflict with C# keywords.
    /// </summary>
    /// <param name="param_name">A parameter name to modify.</param>
    /// <returns>The modified parameter name.</returns>
    private static ReadOnlySpan<char> _escape_param_name(ReadOnlySpan<char> param_name) {
        /*TODO: Consider https://stackoverflow.com/a/44728184 or https://stackoverflow.com/a/44728208
                Something akin to `return cs.IsValidIdentifier(param_name) ? param_name : $"_{param_name}";`*/
        return param_name switch {
            "this" => "_this",
            _      => param_name,
        };
    }

    /// <summary>
    ///     Translates a function's parameter list to a string with types and names mapped.
    /// </summary>
    /// <param name="parameters">The list of parameters</param>
    /// <returns>A string representation of the parameter list, valid as C# code.</returns>
    private static string _build_params_string(List<FhFuncParameter> parameters) {
        List<string> param_str = [];

        foreach (FhFuncParameter param in parameters) {
            param_str.Add($"{_map_type(param.ParameterType)} {_escape_param_name(param.ParameterName)}");
        }

        return $"({string.Join(", ", param_str)})";
    }

    /// <summary>
    ///     Converts a Ghidra function declaration and the associated signature data into valid C# code.
    /// </summary>
    /// <param name="function">A Ghidra-provided function declaration.</param>
    /// <param name="signature_data">The signature data associated with the function.</param>
    /// <returns>A valid C# delegate declaration and associated function address constant.</returns>
    private static string _emit_function(FhFuncDecl function, FhFuncSignatureData signature_data, FhGameId game) {
        int    addr   = int.Parse(function.Location, NumberStyles.HexNumber, CultureInfo.InvariantCulture) - 0x400000;
        string module = game switch {
            FhGameId.FFX    => "FFX.exe",
            FhGameId.FFX2   or
            FhGameId.FFX2LM => "FFX-2.exe",
            _               => throw new NotImplementedException($"invalid game id {game} - cannot generate function"),
        };

        return $"""
                // Original after pruning:
                // {function.CallConv} {function.Signature} at {function.Location}

                {_emit_callconv_attr(function.CallConv)}
                public unsafe delegate {signature_data.ReturnType} {function.FuncName}{_build_params_string(signature_data.Parameters)};
                public static FhMethodHandle<{function.FuncName}> h_{function.Name} => new( new FhMethodLocation("{module}", 0x{addr:X}) );

            """;
    }

    /// <summary>
    ///     Converts a global symbol provided by Ghidra into valid C# code.
    /// </summary>
    /// <param name="global">A global symbol provided by Ghidra</param>
    /// <returns>A valid C# const declaration for the given global</returns>
    private static string _emit_global(FhDataLabelDecl global) {
        int                addr        = int.Parse(global.Location, NumberStyles.HexNumber, CultureInfo.InvariantCulture) - 0x400000;
        ReadOnlySpan<char> mapped_type = _map_type(global.DataType);

        //TODO: Make sure C# doesn't have issues with the pointer when the global is an array.
        return $"""
                // Original after pruning:
                // {global.DataType} {global.Name} at {global.Location}

                public static {mapped_type}* {global.Name} => FhUtil.ptr_at<{mapped_type}>(0x{addr:X});
                public const nint __addr_{global.Name} = 0x{addr:X};

            """;
    }

    /// <summary>
    ///     Return FhCall's introductory comment.
    /// </summary>
    /// <returns>An introductory comment.</returns>
    private static string _emit_prologue(FhGameId game) {
        string ns = game switch {
            FhGameId.FFX    => "namespace Fahrenheit.FFX;",
            FhGameId.FFX2   or
            FhGameId.FFX2LM => "namespace Fahrenheit.FFX2;",
            _               => "namespace Fahrenheit;",
        };

        return $$"""
            /* [STEP {{DateTime.UtcNow:dd/M/yy HH:mm}}]
             * This file was generated by Fahrenheit's STEP tool (https://github.com/fahrenheit-crew/fh-tools-step/).
             *
             * Its purpose is to provide auto-generated delegates to allow you to call or hook game functions without having
             * to go through an extensive reverse-engineering process. These are, for the time being, quite rudimentary;
             * many parameters whose types are known to us are still mapped only to `nint`.
             *
             * The presence of a delegate or function signature in this file does not imply it has been tested. You have been warned.
             *
             * To improve the call map quality, add new entries to `typemap.json` in the STEP source code or annotate further
             * functions in Ghidra. Every so often, STEP generation will be rerun and Fahrenheit updated with the result.
             */

            {{ns}}

            public static unsafe partial class FhCall {

            """;
    }

    /// <summary>
    ///     Flushes a written out symbol table to disk.
    /// </summary>
    private static void _dump_symtable(string dest_path, StringBuilder sb) {
        sb.AppendLine("}");
        File.WriteAllText(dest_path, sb.ToString());

        Console.WriteLine($"{dest_path}");
    }

    /// <summary>
    ///     Emits a C# code file to a specified path using exported Ghidra symbols and a user-defined typemap.
    /// </summary>
    /// <param name="src_path">
    ///     The path to the directory containing the Ghidra symbol exports.<br/>
    ///     The directory must contain appropriately exported <c>functions.csv</c> and <c>globals.csv</c>.
    /// </param>
    /// <param name="dest_path">The path to write the C# code file to.</param>
    /// <param name="typemap_path">The path to the user-defined typemap. Typemap must be a valid JSON.</param>
    private static void _emit_symtable(string src_path, string dest_path, string typemap_path, FhGameId game) {

        /* [fkelava 01/06/26 13:47]
         * Generating all functions for either game results in about a 400,000 line file.
         * IDEs and IntelliSense absolutely cannot handle files that large, not even on
         * top-end machines. We thus split the file at roughly the 50,000 line mark.
         */

        const int LINES_PER_FILE = 50_000;
        const int LINES_PER_DECL = 7; // Guesstimate for file-wraparound
        const int LINES_PER_SKIP = 4; // Guesstimate for file-wraparound

        Stopwatch perf = Stopwatch.StartNew();

        int file_count = 1;
        int line_count = 0;

        string file_path = Path.Join(dest_path, $"call_{file_count++}.g.cs");

        try {
            string type_map_str = File.ReadAllText(typemap_path);
            _type_map = JsonSerializer.Deserialize<Dictionary<string, string>>(type_map_str) ?? [];
        }
        catch {
            Console.WriteLine("Type map load failed or type map path not specified.");
        }

        string global_file_path   = Path.Join(src_path, "globals.csv");
        string function_file_path = Path.Join(src_path, "functions.csv");

        FhFuncDecl[]      functions;
        FhDataLabelDecl[] globals;

        using (StreamReader function_reader = new StreamReader(function_file_path))
        using (CsvReader    function_csv    = new CsvReader(function_reader, CultureInfo.InvariantCulture)) {
            functions = [ .. function_csv.GetRecords<FhFuncDecl>() ];
        }

        using (StreamReader global_reader = new StreamReader(global_file_path))
        using (CsvReader    global_csv    = new CsvReader   (global_reader, CultureInfo.InvariantCulture)) {
            globals = [ .. global_csv.GetRecords<FhDataLabelDecl>() ];
        }

        _format_functions(functions);

        // This local is reused in the loop
        FhFuncSignatureData signature_data = new FhFuncSignatureData {
            Parameters = [ ],
        };

        // Actual file contents.
        StringBuilder sb = new(_emit_prologue(game));

        foreach (FhFuncDecl function in functions) {
            if (line_count >= LINES_PER_FILE) {
                _dump_symtable(file_path, sb);

                file_path  = Path.Join(dest_path, $"call_{file_count++}.g.cs");
                line_count = 0;
                sb         = new(_emit_prologue(game));
            }

            if (!_should_interpret(function)) {
                sb.AppendLine($"    // Symbol skipped (deemed uninterpretable or explicitly rejected):");
                sb.AppendLine($"    // {function.CallConv} {function.Signature} at {function.Location}");
                sb.AppendLine();

                line_count += LINES_PER_SKIP;
                continue;
            }

            // We lex the function signature in the form {RETURN_TYPE} {NAME}({PARAMETER_TYPE} {PARAMETER_NAME} ... );
            string[] tokens = function.Signature.Split(
                [ ' ', '(', ',', ')' ],
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
            );

            /* Tokens:
             * [0] -> Return type
             * [1] -> Function name
             * [2] -> Type of parameter 1
             * [3] -> Name of parameter 1
             * [4] -> Type of parameter 2
             * [5] -> Name of parameter 2
             * ... and so on
             */

            signature_data.ReturnType   = _map_return_type(tokens[0]);
            signature_data.FunctionName = tokens[1]; //TODO: Add cleanup of function name (remove the '+' prefix)

            // Parse parameters
            for (int i = 2; i < tokens.Length - 1; i += 2) {
                ReadOnlySpan<char> type = tokens[i];
                ReadOnlySpan<char> name = tokens[i + 1];

                signature_data.Parameters.Add(new(type, name));
            }

            sb.AppendLine(_emit_function(function, signature_data, game));
            line_count += LINES_PER_DECL; // Guesstimate for file-wraparound

            signature_data.Parameters.Clear();
        }

        foreach (FhDataLabelDecl global in globals) {
            if (line_count >= LINES_PER_FILE) {
                _dump_symtable(file_path, sb);

                file_path  = Path.Join(dest_path, $"call_{file_count++}.g.cs");
                line_count = 0;
                sb         = new(_emit_prologue(game));
            }

            if (!_should_interpret(global)) {
                sb.AppendLine($"    // Global skipped (deemed uninterpretable or explicitly rejected):");
                sb.AppendLine($"    // {global.DataType} {global.Name} at {global.Location}");
                sb.AppendLine();

                line_count += LINES_PER_SKIP;
                continue;
            }

            sb.AppendLine(_emit_global(global));
            line_count += LINES_PER_DECL;
        }

        _dump_symtable(file_path, sb);
        Console.WriteLine($"Done in {perf.Elapsed}.");
    }
}
