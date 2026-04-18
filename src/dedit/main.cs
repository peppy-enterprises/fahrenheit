// SPDX-License-Identifier: MIT

namespace Fahrenheit.Tools.DEdit;

/* [fkelava 26/01/26 12:40]
 * Some text files (e.g. `cdrom.fnd`) contain plain ANSI or UTF-8 text,
 * but come in the standard indexed 'container' that the game usually uses for text files.
 *
 * Direct UTF8 passthrough is purely a DEdit concept. FhEncoding only deals with game-encoded text.
 * This decision may be revisited later.
 */

public enum FhDEditEncoding {
    NULL = 0,
    GAME = 1,
    UTF8 = 2
}

internal static class Program {

    /* [fkelava 11/10/25 14:10]
     * Since we only offer macro dictionary compatibility to the level required
     * to decode base game text, we adhere to the restrictions of the format fully.
     *
     * A macro dictionary in the base game can only contain sixteen sections.
     */

    private readonly static Dictionary<int, Dictionary<int, string>> _macro_refs = new() {
        { 0,  [] },
        { 1,  [] },
        { 2,  [] },
        { 3,  [] },
        { 4,  [] },
        { 5,  [] },
        { 6,  [] },
        { 7,  [] },
        { 8,  [] },
        { 9,  [] },
        { 10, [] },
        { 11, [] },
        { 12, [] },
        { 13, [] },
        { 14, [] },
        { 15, [] },
    };

    private static Range _args_validate_segment(ArgumentResult argr) {
        if (argr.Tokens.Count != 1) {
            argr.AddError("Invalid segment format; ignoring");
            return new Range(0, ^0);
        }

        string[] segment_tokens = argr.Tokens[0].Value.Split(':', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (segment_tokens.Length != 2) {
            argr.AddError("Invalid segment format; ignoring");
            return new Range(0, ^0);
        }

        if (!int.TryParse(segment_tokens[0], out int segment_start) &&
            !int.TryParse(segment_tokens[0], NumberStyles.HexNumber, null, out segment_start)) {
            argr.AddError("Start of segment uninterpretable; ignoring");
            return new Range(0, ^0);
        }

        if (!int.TryParse(segment_tokens[1], out int segment_end) &&
            !int.TryParse(segment_tokens[1], NumberStyles.HexNumber, null, out segment_end)) {
            argr.AddError("End of segment uninterpretable; ignoring");
            return new Range(0, ^0);
        }

        return new Range(segment_start, segment_end);
    }

    private static int Main(string[] args) {
        RootCommand cmd_root = new("Perform various operations on FFX/X-2 dialogue files and character sets.");

        Option<List<FileInfo>> opt_input = new("--input", "-i") {
            Description                    = "Input file(s) to process.",
            Arity                          = ArgumentArity.OneOrMore,
            Recursive                      = true,
            AllowMultipleArgumentsPerToken = true
        };

        Option<string> opt_output = new("--output", "-o") {
            Description = "What folder to emit outputs to. The folder must already exist.",
            Arity       = ArgumentArity.ExactlyOne,
            Recursive   = true
        };

        Option<Range> opt_segment = new("--segment", "-s") {
            Description         = "Which part of the file to interpret. Specify byte offsets as START:END.",
            Arity               = ArgumentArity.ExactlyOne,
            Recursive           = true,
            CustomParser        = _args_validate_segment,
            DefaultValueFactory = argr => new Range(0, ^0)
        };

        Option<FhDEditEncoding> opt_encoding = new("--encoding", "-e") {
            Description = "What encoding to interpret the input file as having.",
            Arity       = ArgumentArity.ExactlyOne,
            Recursive   = true
        };

        Option<FhLangId> opt_lang = new Option<FhLangId>("--lang", "-l") {
            Description = "Set the language to interpret the input file as.",
            Arity       = ArgumentArity.ExactlyOne,
            Recursive   = true
        };

        Option<FhTextIndexType> opt_index = new Option<FhTextIndexType>("--index-type", "-it") {
            Description = "Set the indexing type for the dialogue file in question.",
            Arity       = ArgumentArity.ExactlyOne,
            Recursive   = true
        };

        Option<FhGameId> opt_game = new Option<FhGameId>("--game-type", "-g") {
            Description = "Set the game type for the dialogue file in question.",
            Arity       = ArgumentArity.ExactlyOne,
            Recursive   = true
        };

        Option<FileInfo> opt_macrodict = new("--macro-dict", "-m") {
            Description = "The dictionary to use to resolve any encountered macro refs.",
            Arity       = ArgumentArity.ExactlyOne,
            Recursive   = true
        };

        cmd_root.Options.Add(opt_input);
        cmd_root.Options.Add(opt_output);
        cmd_root.Options.Add(opt_segment);
        cmd_root.Options.Add(opt_encoding);
        cmd_root.Options.Add(opt_lang);
        cmd_root.Options.Add(opt_index);
        cmd_root.Options.Add(opt_game);
        cmd_root.Options.Add(opt_macrodict);

        Command cmd_decompile = new("decompile", "Decompiles a dialogue file.");

        cmd_decompile.SetAction(parse_result => _c_decompile(
            parse_result.GetRequiredValue(opt_input),
            parse_result.GetRequiredValue(opt_output),
            parse_result.GetValue        (opt_segment),
            parse_result.GetRequiredValue(opt_encoding),
            parse_result.GetRequiredValue(opt_lang),
            parse_result.GetRequiredValue(opt_index),
            parse_result.GetRequiredValue(opt_game)
            ));

        Command cmd_decompile_macro = new("decompile-macro", "Decompiles a macro dictionary file.");

        Command cmd_compile = new("compile", "Compiles a dialogue file.");

        cmd_compile.SetAction(parse_result => _c_compile(
            parse_result.GetRequiredValue(opt_input),
            parse_result.GetRequiredValue(opt_output),
            parse_result.GetRequiredValue(opt_encoding),
            parse_result.GetRequiredValue(opt_lang),
            parse_result.GetRequiredValue(opt_index),
            parse_result.GetRequiredValue(opt_game),
            parse_result.GetRequiredValue(opt_macrodict)
            ));

        cmd_root.Subcommands.Add(cmd_decompile);
        cmd_root.Subcommands.Add(cmd_compile);

        ParseResult argparse_result = cmd_root.Parse(args);
        return argparse_result.Invoke();
    }

    /// <summary>
    ///     Converts all game-encoded dialogue files in <paramref name="input"/> into text files in DEdit syntax, for a specified
    ///     <paramref name="game"/>, <paramref name="index_type"/>, and <paramref name="lang"/>, emitting them to <paramref name="output"/>.
    /// </summary>
    private static void _c_decompile(
        List<FileInfo>  input,
        string          output,
        Range           segment,
        FhDEditEncoding encoding,
        FhLangId        lang,
        FhTextIndexType index_type,
        FhGameId        game)
    {
        Stopwatch perf = Stopwatch.StartNew();

        foreach (FileInfo input_file in input) {
            string output_path = Path.Join(output, $"{input_file.Name}.txt");

            using (FileStream input_file_stream  = input_file.OpenRead())
            using (FileStream output_file_stream = new FileStream(output_path, FileMode.Create, FileAccess.Write, FileShare.None)) {
                _decompile(input_file_stream, output_file_stream, segment, encoding, lang, index_type, game);
            }

            Console.WriteLine($"{input_file.Name} -> {output_path}");
        }

        Console.WriteLine($"processed {input.Count} files in {perf.Elapsed}");
    }

    /// <summary>
    ///     Converts all text files in DEdit syntax in <paramref name="input_files"/> into game-encoded dialogue files,
    ///     for a specified <paramref name="game"/>, <paramref name="index_type"/>, and <paramref name="lang"/>, emitting them to <paramref name="output_dir"/>.
    /// </summary>
    private static void _c_compile(
        List<FileInfo>  input_files,
        string          output_dir,
        FhDEditEncoding encoding,
        FhLangId        lang,
        FhTextIndexType index_type,
        FhGameId        game,
        FileInfo        macro_dict
        )
    {
        Stopwatch perf = Stopwatch.StartNew();

        foreach (FileInfo input_file in input_files) {
            string output_path = Path.Join(output_dir, $"{Path.GetFileNameWithoutExtension(input_file.FullName)}");

            using (FileStream fs_input  = input_file.OpenRead())
            using (FileStream fs_output = new FileStream(output_path, FileMode.Create, FileAccess.Write, FileShare.None))
            using (FileStream fs_macro  = macro_dict.OpenRead()) {
                _compile(fs_input, fs_macro, fs_output, encoding, lang, index_type, game);
            }

            Console.WriteLine($"{input_file.Name} -> {output_path}");
        }

        Console.WriteLine($"processed {input_files.Count} files in {perf.Elapsed}");
    }

   /* [fkelava 14/10/25 20:09]
    * Section offsets in macro dictionaries can arbitrarily be zero. This helper is used to skip to the next non-zero section.
    * One always exists because DEdit injects the length of the file as the beginning of its pseudo-'17th' section.
    */
    private static int _next_non_null_macro_section(ReadOnlySpan<int> sections) {
        foreach (int section in sections) {
            if (section != 0) return section;
        }

        throw new Exception("UNREACHABLE");
    }

    /// <summary>
    ///     Processes a input <paramref name="macro_dict_file"/>, enabling its use to resolve {MACRO} ops during (de)compilation.
    ///     <para/>
    ///     The correct <paramref name="lang"/> and <paramref name="game"/> must be specified.
    /// </summary>
    private static void _macrodict_load(
        FileStream macro_dict_file,
        FhLangId   lang,
        FhGameId   game)
    {
        Span<byte> macro_dict_bytes = new byte[macro_dict_file.Length];
        macro_dict_file.ReadExactly(macro_dict_bytes);

        /* [fkelava 14/10/25 20:09]
         * A macro dictionary is a concatenation of up to sixteen dialogue files with a 64-byte header that indicates
         * the offsets at which each section (i.e. dialogue file) begins. Index type is always I16_X2.
         */

        FhMacroDictHeader macro_dict_header   = MemoryMarshal.Cast<byte, FhMacroDictHeader>(macro_dict_bytes[0 .. 0x40])[0];
        int[]             macro_dict_sections = [ .. macro_dict_header.sections, macro_dict_bytes.Length ];

        for (int i = 0; i < macro_dict_sections.Length - 1; i++) {
            int section_offset = macro_dict_sections[i];
            if (section_offset == 0) continue;

            // Begin reading indices.

            ReadOnlySpan<byte> section_bytes = macro_dict_bytes[ section_offset .. ];

            int   index_end      = FhEncoding.read_index(section_bytes, FhTextIndexType.I16_X2, out int in_section_offset);
            int   indices_length = index_end / in_section_offset;
            int[] indices        = new int[indices_length + 1];

            indices[0]  = index_end + section_offset;
            indices[^1] = _next_non_null_macro_section(macro_dict_sections.AsSpan()[ (i + 1) .. ]);

            for (int j = 1; j < indices_length; j++) { // fill indices array
                indices[j] = FhEncoding.read_index(section_bytes[ in_section_offset .. ], FhTextIndexType.I16_X2, out int consumed) + section_offset;
                in_section_offset += consumed;
            }

            // Begin reading dialogue.

            for (int k = 0; k < indices_length; k++) {
                int start = indices[k];
                int end   = indices[k + 1];

                ReadOnlySpan<byte> src  = macro_dict_bytes[ start .. end ];
                byte[]             dest = ArrayPool<byte>.Shared.Rent(FhEncoding.compute_decode_buffer_size(src, lang, game));

                int dest_written  = FhEncoding.decode(src, dest, lang, game);
                _macro_refs[i][k] = Encoding.UTF8.GetString(dest[ .. dest_written ]);

                ArrayPool<byte>.Shared.Return(dest);
            }
        }
    }

    /// <summary>
    ///     Converts a game or UTF8-encoded text <paramref name="input"/> into a text file in DEdit syntax, for a specified
    ///     <paramref name="game"/>, <paramref name="index_type"/>, and <paramref name="lang"/>.
    /// </summary>
    private static void _decompile(
        FileStream      input,
        FileStream      output,
        Range           segment,
        FhDEditEncoding encoding,
        FhLangId        lang,
        FhTextIndexType index_type,
        FhGameId        game)
    {
        Span<byte> input_bytes = new byte[input.Length];
        input.ReadExactly(input_bytes);

        // Begin reading indices.

        /* [fkelava 26/01/26 12:50]
         * The entire file must be loaded regardless of the 'segment' argument because indices
         * may contain absolute offsets, which will fail to resolve if we scope too early.
         *
         * The end of 'segment' is as of yet unused, because it has no effect in indexed mode
         * and DEdit does not (yet) support indexless operation.
         */

        int   index_end      = FhEncoding.read_index(input_bytes[segment], index_type, out int offset);
        int   indices_length = index_end / offset;
        int[] indices        = new int[indices_length + 1];

        indices[0]  = index_end;
        indices[^1] = int.CreateChecked(input.Length);

        for (int i = 1; i < indices_length; i++) {
            indices[i] = FhEncoding.read_index(input_bytes[offset .. ], index_type, out int consumed);
            offset += consumed;
        }

        // Begin reading dialogue.

        for (int i = 0; i < indices_length; i++) {
            int start = indices[i];
            int end   = indices[i + 1];

            ReadOnlySpan<byte> src = input_bytes[start .. end];

            /* [fkelava 26/01/26 12:40]
             * In UTF8 mode we must mimic what FhEncoding would do and emit an {END} mark.
             */

            if (encoding is FhDEditEncoding.UTF8) {
                output.Write(src);
                output.Write("{END}\r\n"u8);
                continue;
            }

            byte[] dest = ArrayPool<byte>.Shared.Rent(FhEncoding.compute_decode_buffer_size(src, lang, game));

            int dest_written = FhEncoding.decode(src, dest, lang, game);
            output.Write(dest.AsSpan()[ .. dest_written ]);

            ArrayPool<byte>.Shared.Return(dest);
        }
    }

    /// <summary>
    ///     Converts a text <paramref name="input_file"/> in DEdit syntax into a game or UTF8 encoded dialogue <paramref name="output"/>,
    ///     for a specified <paramref name="game"/>, <paramref name="index_type"/>, and <paramref name="lang"/>.
    /// </summary>
    private static void _compile(
        FileStream      input_file,
        FileStream      macro_dict_file,
        FileStream      output,
        FhDEditEncoding encoding,
        FhLangId        lang,
        FhTextIndexType index_type,
        FhGameId        game)
    {
        _macrodict_load(macro_dict_file, lang, game);

        Span<byte> input_bytes = new byte[input_file.Length];
        input_file.ReadExactly(input_bytes);

        Span<byte> dest = new byte[FhEncoding.compute_encode_buffer_size(input_bytes, lang, game)];
        int dest_written = FhEncoding.encode(input_bytes, dest, lang, game);

        Span<byte> indices = new byte[FhEncoding.compute_index_buffer_size(input_bytes, index_type)];
        FhEncoding.write_indices(dest, indices, index_type);

        output.Write(indices);
        output.Write(dest[ .. dest_written ]);
    }
}
