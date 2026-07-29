// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* [fkelava 17/5/23 02:48]
 * A small tool to emit (hopefully) valid C# code from Ghidra symbol JSONs.
 */

namespace Fahrenheit.Tools.STEP;

internal ref struct FhFuncSignatureData {
    public ReadOnlySpan<char>    ReturnType;
    public ReadOnlySpan<char>    FunctionName;
    public List<FhFuncParameter> Parameters;
}

internal record FhFuncParameter(string ParameterType, string ParameterName);

internal static partial class Program {

    /* [fkelava 01/06/26 13:47]
     * Generating all functions for either game results in about a 400,000 line file.
     * IDEs and IntelliSense absolutely cannot handle files that large, not even on
     * top-end machines. We thus split the file at roughly the 50,000 line mark.
     */

    const int LINES_PER_FILE = 50_000;
    const int LINES_PER_DECL = 7; // Guesstimate for file-wraparound
    const int LINES_PER_SKIP = 4; // Guesstimate for file-wraparound

    private static int[]                             _s_noemit  = [];
    private static Dictionary<string, string>        _s_typemap = [];
    private static FhFuncDecl[]                      _s_funcs   = [];
    private static FhDataLabelDecl[]                 _s_data    = [];
    private static Dictionary<int, FhCommonFuncDecl> _s_common  = [];

    private static void Main(string[] args) {
        Option<string>   opt_globals   = new ("-d", "--data") {
            Description = "Set the path to the file containing data definitions.",
            Required    = true
        };
        Option<string>   opt_functions = new ("-f", "--functions") {
            Description = "Set the path to the file containing function definitions.",
            Required    = true
        };
        Option<string>   opt_output    = new ("-o", "--output") {
            Description = "Set the folder where the C# file should be written.",
            Required    = true
        };
        Option<string>   opt_typemap   = new ("-m", "--map") {
            Description = "Set the path to a Ghidra -> Fh type map.",
            Required    = false
        };
        Option<int[]>    opt_noemit    = new ("-ne", "--no-emit") {
            Description  = "Specify a set of addresses for which calls shall not be emitted.",
            Required     = false,
            CustomParser = _parse_arg_noemit
        };
        Option<string>   opt_functions_common = new ("-fc", "--functions-common") {
            Description = "Set the path to the file containing common function definitions.",
            Required    = true
        };
        Option<FhGameId> opt_game      = new ("-g", "--game-id") {
            Description = "Declare which game STEP is generating for.",
            Required    = true
        };

        RootCommand cmd_root = new RootCommand("Process a Ghidra symbol table and create a C# code file.");

        // To permit no-emit addresses to be listed consecutively after a single -ne specifier.
        opt_noemit.AllowMultipleArgumentsPerToken = true;

        cmd_root.Options.Add(opt_globals);
        cmd_root.Options.Add(opt_functions);
        cmd_root.Options.Add(opt_output);
        cmd_root.Options.Add(opt_typemap);
        cmd_root.Options.Add(opt_noemit);
        cmd_root.Options.Add(opt_functions_common);
        cmd_root.Options.Add(opt_game);

        cmd_root.SetAction(parse_result => _emit_symtable(
            parse_result.GetRequiredValue(opt_globals),
            parse_result.GetRequiredValue(opt_functions),
            parse_result.GetRequiredValue(opt_output),
            parse_result.GetValue        (opt_typemap) ?? "",
            parse_result.GetRequiredValue(opt_noemit),
            parse_result.GetRequiredValue(opt_functions_common),
            parse_result.GetRequiredValue(opt_game)
            ));

        ParseResult parse_result = cmd_root.Parse(args);
        parse_result.Invoke();
    }

    /// <summary>
    ///     Parses a set of no-emit addresses in hex form on the command line.
    /// </summary>
    private static int[] _parse_arg_noemit(ArgumentResult arg) {
        int[] rv = new int[arg.Tokens.Count];

        // -ne args are only accepted in form: 0x{ADDR:X}, ex. -ne 0x207DB0
        for (int i = 0; i < arg.Tokens.Count; i++) {
            rv[i] = int.Parse(arg.Tokens[i].Value[ 2 .. ], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }

        return rv;
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
            _            => throw new ArgumentException($"Encountered an unknown calling convention `{call_conv}` while parsing functions."),
        };
    }

    /// <summary>
    ///     Maps a Ghidra-provided type using the user-defined typemap.
    /// </summary>
    /// <example>
    ///     Assuming Ghidra's <c>undefined4</c> type is mapped to C#'s <c>uint</c>,
    ///     calling this function with <c>"undefined4"</c> will return <c>"uint"</c>.
    /// </example>
    /// <param name="type">The string representation of a Ghidra parameter type.</param>
    /// <returns>The mapped parameter type.<br/>Returns <c>"nint"</c> if the given Ghidra type isn't mapped.</returns>
    private static ReadOnlySpan<char> _map_type(string type) {
        return _s_typemap.GetValueOrDefault(type, "nint");
    }

    /// <summary>
    ///     Applies the user-defined type map to a parameter type provided by Ghidra.<br/>
    ///     Unlike <see cref="_map_type"/>, accounts for <c>void</c>.
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
    ///     Modifies a <paramref name="param_name"/> to not conflict with C# keywords.
    /// </summary>
    /// <returns>The modified parameter name.</returns>
    private static ReadOnlySpan<char> _escape_param_name(string param_name) {
        bool is_language_reserved =
            SyntaxFacts.GetKeywordKind          (param_name) != SyntaxKind.None
         || SyntaxFacts.GetContextualKeywordKind(param_name) != SyntaxKind.None;

        return is_language_reserved ? $"_{param_name}" : param_name;
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
    ///     Converts an offset back into its Ghidra equivalent.
    /// </summary>
    private static int _addr_to_ghidra(int address) => address + 0x400000;

    /// <summary>
    ///     Converts a Ghidra function declaration and the associated signature data into valid C# code.
    /// </summary>
    /// <param name="function">A Ghidra-provided function declaration.</param>
    /// <param name="signature_data">The signature data associated with the function.</param>
    /// <returns>A valid C# delegate declaration and associated function address constant.</returns>
    private static string _emit_function(FhFuncDecl function, FhFuncSignatureData signature_data, FhGameId game) {
        int addr_label = _addr_to_ghidra(function.Location);

        string module = game switch {
            FhGameId.FFX    => "FFX.exe",
            FhGameId.FFX2   or
            FhGameId.FFX2LM => "FFX-2.exe",
            _               => throw new NotImplementedException($"invalid game id {game} - cannot generate function"),
        };

        if (_s_noemit.Contains(function.Location)) {
            return $"""
                // Symbol on explicit no-emit list:
                // {function.CallConv} {function.Signature} at {addr_label:x8}

            """;
        }

        return $"""
                // Original after pruning:
                // {function.CallConv} {function.Signature} at {addr_label:x8}

                {_emit_callconv_attr(function.CallConv)}
                public unsafe delegate {signature_data.ReturnType} d_{function.FuncName}{_build_params_string(signature_data.Parameters)};
                public static FhMethodHandle<d_{function.FuncName}> {function.Name} => new( new FhMethodLocation("{module}", 0x{function.Location:X}) );

            """;
    }

    /// <summary>
    ///     Emits valid C# code for a fused handle which permits access to a function identical in both binaries.
    /// </summary>
    /// <param name="function">A Ghidra-provided function declaration.</param>
    /// <param name="signature_data">The signature data associated with the function.</param>
    /// <param name="common_data">Data describing which two functions are being fused.</param>
    /// <returns>A valid C# delegate declaration and associated function address constant.</returns>
    private static string _emit_common_function(FhFuncDecl function, FhFuncSignatureData signature_data, FhCommonFuncDecl common_data) {
        int addr_label_src = _addr_to_ghidra(common_data.SourceAddress);
        int addr_label_dst = _addr_to_ghidra(common_data.DestAddress);

        string fused_label = $"FUN_{addr_label_src:X8}_{addr_label_dst:X8}";

        return $"""
                // Fused identical entry {function.CallConv} {function.Signature}
                // at (FFX.exe+{addr_label_src:X}, FFX-2.exe+{addr_label_dst:X})

                {_emit_callconv_attr(function.CallConv)}
                public unsafe delegate {signature_data.ReturnType} d_{fused_label}{_build_params_string(signature_data.Parameters)};
                public static FhMethodHandle<d_{fused_label}> {fused_label} => new( new FhMethodLocation(0x{common_data.SourceAddress:X}, 0x{common_data.DestAddress:X}) );

            """;
    }

    /// <summary>
    ///     Converts a global symbol provided by Ghidra into valid C# code.
    /// </summary>
    /// <param name="global">A global symbol provided by Ghidra</param>
    /// <returns>A valid C# const declaration for the given global</returns>
    private static string _emit_global(FhDataLabelDecl global) {
        int addr_label = _addr_to_ghidra(global.Location);

        ReadOnlySpan<char> mapped_type = _map_type(global.DataType);

        if (_s_noemit.Contains(global.Location)) {
            return $"""
                // Symbol on explicit no-emit list:
                // {global.DataType} {global.Name} at {addr_label:x8}

            """;
        }

        //TODO: Make sure C# doesn't have issues with the pointer when the global is an array.
        return $"""
                // Original after pruning:
                // {global.DataType} {global.Name} at {addr_label:x8}

                public const nint __addr_{global.Name} = 0x{global.Location:X};
                public static {mapped_type}* {global.Name} => FhUtil.ptr_at<{mapped_type}>(__addr_{global.Name});

            """;
    }

    /// <summary>
    ///     Return FhCall's introductory comment.
    /// </summary>
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
    ///    Loads the JSON file containing type remappings.
    /// </summary>
    private static void _load_typemap(string path_typemap) {
        try {
            string type_map_str = File.ReadAllText(path_typemap);
            _s_typemap = JsonSerializer.Deserialize<Dictionary<string, string>>(type_map_str) ?? [];
        }
        catch {
            Console.WriteLine("Type map load failed or type map path not specified.");
        }
    }

    /// <summary>
    ///     Loads and fixes up the CSV file containing function definitions.
    /// </summary>
    private static void _load_functions(string path_functions) {
        using (StreamReader function_reader = new StreamReader(path_functions))
        using (CsvReader    function_csv    = new CsvReader   (function_reader, CultureInfo.InvariantCulture)) {
            _s_funcs = [ .. function_csv.GetRecords<FhFuncDecl>() ];
        }

        for (int i = 0; i < _s_funcs.Length; i++) {
            _s_funcs[i].Signature = _s_funcs[i].Signature
                .Replace(" *"   , "*") // Ghidra "float * param_1" -> "float* param_1"
                .Replace("\\,"  , ",") // Ghidra CSV unescape
                .Replace("\"\\" , "" );
        }
    }

    /// <summary>
    ///     Loads and fixes up the CSV file containing common function definitions.
    /// </summary>
    private static void _load_functions_common(string path_functions_common, FhGameId game) {
        using (StreamReader common_function_reader = new StreamReader(path_functions_common))
        using (CsvReader    common_function_csv    = new CsvReader   (common_function_reader, CultureInfo.InvariantCulture)) {
            FhCommonFuncDecl[] common_defs = [ .. common_function_csv.GetRecords<FhCommonFuncDecl>() ];

            /* [fkelava 29/07/26 20:55]
             * In common-matching, 'source' is always FFX.exe and 'destination' is always FFX-2.exe.
             * As _s_common is used for exclusion, we have to key it by the game we're emitting for.
             */
            foreach (FhCommonFuncDecl common_def in common_defs) {
                int key = game switch {
                    FhGameId.FFX    => common_def.SourceAddress,
                    FhGameId.FFX2   or
                    FhGameId.FFX2LM => common_def.DestAddress,
                    _               => throw new NotImplementedException("Invalid game ID."),
                };

                _s_common[key] = common_def;
            }
        }
    }

    /// <summary>
    ///     Loads the CSV file containing globals and other data labels.
    /// </summary>
    private static void _load_data(string path_data) {
        using (StreamReader data_reader = new StreamReader(path_data))
        using (CsvReader    data_csv    = new CsvReader   (data_reader, CultureInfo.InvariantCulture)) {
            _s_data = [ .. data_csv.GetRecords<FhDataLabelDecl>() ];
        }
    }

    /// <summary>
    ///     Emits a C# code file to a specified path using exported Ghidra symbols and a user-defined typemap.
    /// </summary>
    /// <param name="path_data">A path to a CSV file containing Ghidra global exports.</param>
    /// <param name="path_functions">A path to a CSV file containing Ghidra function exports.</param>
    private static void _emit_symtable(
        string   path_data,
        string   path_functions,
        string   path_dest,
        string   path_typemap,
        int[]    no_emit_addresses,
        string   path_functions_common,
        FhGameId game) {

        _s_noemit = no_emit_addresses;

        Stopwatch perf = Stopwatch.StartNew();

        int file_count = 1;
        int line_count = 0;

        string output_file_path        = Path.Join(path_dest, $"call_{file_count++}.g.cs");
        string output_file_path_common = Path.Join(path_dest, $"call.g.cs");

        _load_typemap         (path_typemap);
        _load_functions       (path_functions);
        _load_functions_common(path_functions_common, game);
        _load_data            (path_data);

        // This local is reused in the loop
        FhFuncSignatureData signature_data = new FhFuncSignatureData {
            Parameters = [ ],
        };

        // Actual file contents.
        StringBuilder sb        = new(_emit_prologue(game));
        StringBuilder sb_common = new(_emit_prologue(FhGameId.NULL));

        foreach (FhFuncDecl function in _s_funcs) {
            if (line_count >= LINES_PER_FILE) {
                _dump_symtable(output_file_path, sb);

                output_file_path = Path.Join(path_dest, $"call_{file_count++}.g.cs");
                line_count       = 0;
                sb               = new(_emit_prologue(game));
            }

            if (!_should_interpret(function)) {
                sb.AppendLine($"    // Symbol skipped (deemed uninterpretable or explicitly rejected):");
                sb.AppendLine($"    // {function.CallConv} {function.Signature} at {_addr_to_ghidra(function.Location):x8}");
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
                string type = tokens[i];
                string name = tokens[i + 1];

                signature_data.Parameters.Add(new (type, name));
            }

            if (_s_common.TryGetValue(function.Location, out FhCommonFuncDecl common_data)) {
                sb_common.AppendLine(_emit_common_function(function, signature_data, common_data));
            }
            else {
                sb.AppendLine(_emit_function(function, signature_data, game));
            }

            line_count += LINES_PER_DECL; // Guesstimate for file-wraparound

            signature_data.Parameters.Clear();
        }

        foreach (FhDataLabelDecl global in _s_data) {
            if (line_count >= LINES_PER_FILE) {
                _dump_symtable(output_file_path, sb);

                output_file_path = Path.Join(path_dest, $"call_{file_count++}.g.cs");
                line_count       = 0;
                sb               = new(_emit_prologue(game));
            }

            if (!_should_interpret(global)) {
                sb.AppendLine($"    // Global skipped (deemed uninterpretable or explicitly rejected):");
                sb.AppendLine($"    // {global.DataType} {global.Name} at {_addr_to_ghidra(global.Location):x8}");
                sb.AppendLine();

                line_count += LINES_PER_SKIP;
                continue;
            }

            sb.AppendLine(_emit_global(global));
            line_count += LINES_PER_DECL;
        }

        _dump_symtable(output_file_path,        sb);
        _dump_symtable(output_file_path_common, sb_common);

        Console.WriteLine($"Done in {perf.Elapsed}.");
    }
}
