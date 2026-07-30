// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit.Tools.STEP;

internal static class Program {

    private static void Main(string[] args) {
        Option<string>   opt_output     = new ("-o", "--output") {
            Description = "Set the folder where the C# files should be written.",
            Required    = true
        };
        Option<string>   opt_global_x   = new ("-gx", "--global-x") {
            Description = "Set the path to the file containing globals for FF X.",
            Required    = true
        };
        Option<string>   opt_global_x2  = new ("-gx2", "--global-x2") {
            Description = "Set the path to the file containing globals for FF X-2.",
            Required    = true
        };
        Option<string>   opt_fn         = new ("-f", "--functions") {
            Description = "Set the path to the file containing common function definitions.",
            Required    = true
        };
        Option<string>   opt_fn_x       = new ("-fx", "--functions-x") {
            Description = "Set the path to the file containing function definitions for FF X.",
            Required    = true
        };
        Option<string>   opt_fn_x2      = new ("-fx2", "--functions-x2") {
            Description = "Set the path to the file containing function definitions for FF X-2.",
            Required    = true
        };
        Option<string>   opt_remap_x    = new ("-rx", "--remap-x") {
            Description = "Set the path to the file containing Ghidra -> Fh remappings for FF X.",
            Required    = false
        };
        Option<string>   opt_remap_x2   = new ("-rx2", "--remap-x2") {
            Description = "Set the path to the file containing Ghidra -> Fh remappings for FF X-2.",
            Required    = false
        };
        Option<string>   opt_reject_x   = new ("-rex", "--reject-x") {
            Description  = "Set the path to the file specifying addresses not to emit calls for in FF X.",
            Required     = true
        };
        Option<string>   opt_reject_x2  = new ("-rex2", "--reject-x2") {
            Description  = "Set the path to the file specifyin addresses not to emit calls for in FF X-2.",
            Required     = true
        };

        RootCommand cmd_root = new RootCommand("Process a Ghidra symbol table and create a C# code file.");

        cmd_root.Options.Add(opt_output);
        cmd_root.Options.Add(opt_global_x);
        cmd_root.Options.Add(opt_global_x2);
        cmd_root.Options.Add(opt_fn);
        cmd_root.Options.Add(opt_fn_x);
        cmd_root.Options.Add(opt_fn_x2);
        cmd_root.Options.Add(opt_remap_x);
        cmd_root.Options.Add(opt_remap_x2);
        cmd_root.Options.Add(opt_reject_x);
        cmd_root.Options.Add(opt_reject_x2);

        cmd_root.SetAction(parse_result => _generate(
            parse_result.GetRequiredValue(opt_global_x),
            parse_result.GetRequiredValue(opt_global_x2),
            parse_result.GetRequiredValue(opt_fn),
            parse_result.GetRequiredValue(opt_fn_x),
            parse_result.GetRequiredValue(opt_fn_x2),
            parse_result.GetRequiredValue(opt_remap_x),
            parse_result.GetRequiredValue(opt_remap_x2),
            parse_result.GetRequiredValue(opt_reject_x),
            parse_result.GetRequiredValue(opt_reject_x2),
            parse_result.GetRequiredValue(opt_output)
            ));

        ParseResult parse_result = cmd_root.Parse(args);
        parse_result.Invoke();
    }

    /// <summary>
    ///     Generates all symbol tables.
    /// </summary>
    private static void _generate(
        string path_global_x,
        string path_global_x2,
        string path_fn,
        string path_fn_x,
        string path_fn_x2,
        string path_remap_x,
        string path_remap_x2,
        string path_reject_x,
        string path_reject_x2,
        string path_output_dir)
    {
        Stopwatch perf = Stopwatch.StartNew();

        string path_output_x  = Path.Join(path_output_dir, "ffx");
        string path_output_x2 = Path.Join(path_output_dir, "ffx2");

        _load_functions_common(path_fn);

        FhStepEmitter emitter_x = new FhStepEmitter(
            Directory.CreateDirectory(path_output_x),
            _load_reject (path_reject_x),
            _load_remap  (path_remap_x),
            _load_fn     (path_fn_x),
            _load_globals(path_global_x),
            [],
            FhGameId.FFX);
        emitter_x.generate_code();

        FhStepEmitter emitter_x2 = new FhStepEmitter(
            Directory.CreateDirectory(path_output_x2),
            _load_reject (path_reject_x2),
            _load_remap  (path_remap_x2),
            _load_fn     (path_fn_x2),
            _load_globals(path_global_x2),
            [],
            FhGameId.FFX2);
        emitter_x2.generate_code();

        Console.WriteLine($"Complete in {perf.Elapsed}.");
    }

    /// <summary>
    ///     Loads a file specifying addresses to reject during
    ///     code generation from the given absolute <paramref name="file_path"/>.
    /// </summary>
    private static RejectData _load_reject(string file_path) {
        string[] reject_lines = File.ReadAllLines(file_path);
        int   [] reject       = new int[reject_lines.Length];

        // -ne* args are only accepted in form: 0x{ADDR:X}, ex. -ne 0x207DB0
        for (int i = 0; i < reject.Length; i++) {
            reject[i] = int.Parse(reject_lines[i][ 2 .. ], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }

        return reject;
    }

    /// <summary>
    ///    Loads a JSON file containing type remappings
    ///    from the given absolute <paramref name="file_path"/>.
    /// </summary>
    private static RemapData _load_remap(string file_path) {
        return JsonSerializer.Deserialize<RemapData>(File.ReadAllText(file_path)) ??
            throw new Exception($"Remap file at {file_path} is illegible or invalid.");
    }

    /// <summary>
    ///     Loads and fixes up the CSV file containing function definitions
    ///     from the given absolute <paramref name="file_path"/>.
    /// </summary>
    private static FuncData _load_fn(string file_path) {
        using StreamReader function_reader = new StreamReader(file_path);
        using CsvReader    function_csv    = new CsvReader   (function_reader, CultureInfo.InvariantCulture);

        Span<FhFuncDecl> data  = [ .. function_csv.GetRecords<FhFuncDecl>() ];
        FuncData         funcs = [];

        for (int i = 0; i < data.Length; i++) {
            data[i].Signature = data[i].Signature
                .Replace(" *"   , "*") // Ghidra "float * param_1" -> "float* param_1"
                .Replace("\\,"  , ",") // Ghidra CSV unescape
                .Replace("\"\\" , "" );
        }

        foreach (FhFuncDecl func in data) {
            funcs[func.Location] = func;
        }

        return funcs;
    }

    /// <summary>
    ///     Loads and fixes up the CSV file containing common function definitions
    ///     from the given absolute <paramref name="file_path"/>.
    /// </summary>
    private static CommonData _load_functions_common(string file_path) {
        using StreamReader common_function_reader = new StreamReader(file_path);
        using CsvReader    common_function_csv    = new CsvReader   (common_function_reader, CultureInfo.InvariantCulture);

        FhCommonFuncDecl[] data   = [ .. common_function_csv.GetRecords<FhCommonFuncDecl>() ];
        CommonData         common = [];

        foreach (FhCommonFuncDecl common_def in data) {
            common[common_def.SourceAddress] = common_def;
        }

        return common;
    }

    /// <summary>
    ///     Loads the CSV file containing globals and other data labels
    ///     from the given absolute <paramref name="file_path"/>.
    /// </summary>
    private static GlobalData _load_globals(string file_path) {
        using StreamReader global_reader = new StreamReader(file_path);
        using CsvReader    global_csv    = new CsvReader   (global_reader, CultureInfo.InvariantCulture);

        Span<FhDataLabelDecl> data    = [ .. global_csv.GetRecords<FhDataLabelDecl>() ];
        GlobalData            globals = [];

        foreach (FhDataLabelDecl item in data) {
            globals[item.Location] = item;
        }

        return globals;
    }
}
