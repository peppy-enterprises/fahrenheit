// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core;

public enum FhLangId {
    Japanese = 0,
    English  = 1,
    French   = 2,
    Spanish  = 3,
    German   = 4,
    Italian  = 5,
    Korean   = 9,
    Chinese  = 10,
    Debug    = 11
}

/* [fkelava 16/10/25 19:54]
 * In game dialogue files, the raw dialogue lines are preceded
 * by a section of indices, which embed line offsets, option count
 * for multiple-choice selections, and so on.
 */
public enum FhDialogueIndexType {
    I32_X2 = 1,
    I32_X1 = 2,
    I16_X2 = 3,
    I16_X1 = 4,
}

/* [fkelava 16/10/25 20:13]
 * A macro dictionary is conceptually a concatenation of up to ten dialogue files
 * with a 64-byte header that indicates the start of each 'section', or individual
 * dialogue file within the dictionary. In dialogue, the op {MACRO:X:Y} then points
 * to the Y'th line of section X.
 */

[InlineArray(10)]
public struct FhMacroDictSections {
    private int _i;
}

public struct FhMacroDictHeader {
    public int _0x00;
    public int _0x04;
    public int _0x08;
    public int _0x0C;
    public int _0x10;
    public int _0x14;
    public FhMacroDictSections sections;
}

/// <summary>
///     Exposes low-level transforms between UTF-8 and game encoding.
/// </summary>
public static class FhCharset {

    /* [fkelava 14/10/25 14:58]
     * These literals and methods control the textual representation of
     * dialogue ops when decoding, and how the encoder shall identify them.
     * Changing them constitutes a breaking change.
     */

    private static ReadOnlySpan<byte> _op_end     => "{END}"u8;     // 0x00
    private static ReadOnlySpan<byte> _op_pause   => "{PAUSE}"u8;   // 0x01
    private static ReadOnlySpan<byte> _op_newline => "{LF}"u8;      // 0x03
    private static ReadOnlySpan<byte> _op_space   => "{SPACE:"u8;   // 0x07
    private static ReadOnlySpan<byte> _op_time    => "{TIME:"u8;    // 0x09
    private static ReadOnlySpan<byte> _op_color   => "{COLOR:"u8;   // 0x0A
    private static ReadOnlySpan<byte> _op_btn     => "{BTN:"u8;     // 0x0B
    private static ReadOnlySpan<byte> _op_choice  => "{CHOICE:"u8;  // 0x10
    private static ReadOnlySpan<byte> _op_var     => "{VAR:"u8;     // 0x12
    private static ReadOnlySpan<byte> _op_macro   => "{MACRO:"u8;   // 0x13-0x22
    private static ReadOnlySpan<byte> _op_key     => "{KEY:"u8;     // 0x23
    private static ReadOnlySpan<byte> _op_unknown => "{UNKNOWN:"u8; // any other

    // the effective inverse of $"{arg1:X2}:{arg2:X2}" - see _decode_op() below
    private static byte _get_op_arg1(in ReadOnlySpan<byte> expression, in ReadOnlySpan<byte> op) {
        return byte.Parse(expression[ op.Length .. (op.Length + 2) ], NumberStyles.HexNumber);
    }

    private static byte _get_op_arg2(in ReadOnlySpan<byte> expression, in ReadOnlySpan<byte> op) {
        return byte.Parse(expression[ (op.Length + 3) .. (op.Length + 5) ], NumberStyles.HexNumber);
    }

    private static int _decode_op(in Span<byte> dest, byte op_code) {
        ReadOnlySpan<byte> op = _select_op_literal(op_code);
        op.CopyTo(dest);

        return op.Length;
    }

    private static int _decode_op(in Span<byte> dest, byte op_code, byte op_arg1) {
        ReadOnlySpan<byte> op = _select_op_literal(op_code);
        op.CopyTo(dest);

        return op.Length + Encoding.UTF8.GetBytes($"{op_arg1:X2}}}", dest[op.Length .. ]);
    }

    private static int _decode_op(in Span<byte> dest, byte op_code, byte op_arg1, byte op_arg2) {
        ReadOnlySpan<byte> op = _select_op_literal(op_code);
        op.CopyTo(dest);

        return op.Length + Encoding.UTF8.GetBytes($"{op_arg1:X2}:{op_arg2:X2}}}", dest[op.Length .. ]);
    }

    private static int _decode_char(in ReadOnlySpan<byte> src, Span<byte> dest, FhLangId game_lang, FhGameType game_type) {
        return game_lang is FhLangId.Japanese or FhLangId.Korean or FhLangId.Chinese
            ? _decode_cjk(src, dest, game_lang, game_type)
            : _decode_us (src, dest, game_lang, game_type);
    }

    private static bool _should_ignore_code_point(Rune code_point) {
        return code_point.Value is 0x0D    // U+000D - <Carriage Return> (CR)
                                or 0x0A    // U+000A - <End of Line> (EOL, LF, NL)
                                or 0xFEFF; // U+FEFF - Zero Width No-Break Space (BOM, ZWNBSP)
    }

    /// <summary>
    ///     For a given <paramref name="op_code"/>, produces the UTF-8 textual literal that represents it.
    /// </summary>
    private static ReadOnlySpan<byte> _select_op_literal(byte op_code) {
        return op_code switch {
            0x00                => _op_end,
            0x01                => _op_pause,
            0x03                => _op_newline,
            0x07                => _op_space,
            0x09                => _op_time,
            0x0A                => _op_color,
            0x0B                => _op_btn,
            0x10                => _op_choice,
            0x12                => _op_var,
            >= 0x13 and <= 0x22 => _op_macro,
            0x23                => _op_key,
            _                   => _op_unknown,
        };
    }

    /// <summary>
    ///     For a given UTF-8 <paramref name="expression"/> that represents a dialogue op,
    ///     produces the corresponding opcode.
    /// </summary>
    private static byte _select_op_code(ReadOnlySpan<byte> expression) {
        return expression switch {
            _ when expression.IndexOf(_op_end    ) != -1 => 0x00,
            _ when expression.IndexOf(_op_pause  ) != -1 => 0x01,
            _ when expression.IndexOf(_op_newline) != -1 => 0x03,
            _ when expression.IndexOf(_op_space  ) != -1 => 0x07,
            _ when expression.IndexOf(_op_time   ) != -1 => 0x09,
            _ when expression.IndexOf(_op_color  ) != -1 => 0x0A,
            _ when expression.IndexOf(_op_btn    ) != -1 => 0x0B,
            _ when expression.IndexOf(_op_choice ) != -1 => 0x10,
            _ when expression.IndexOf(_op_var    ) != -1 => 0x12,
            _ when expression.IndexOf(_op_macro  ) != -1 => byte.CreateChecked(0x13 + _get_op_arg1(expression, _op_macro)),
            _ when expression.IndexOf(_op_key    ) != -1 => 0x23,
            _                                            => _get_op_arg1(expression, _op_unknown),
        };
    }

    /* [fkelava 14/10/25 14:58]
     * The custom encoding of the game fundamentally functions by computing indices into a 'table'
     * which is a long UTF-8 string. These are called 'sjistbl', or Shift-JIS tables.
     *
     * Fahrenheit does not support modifying the game's original tables. While doing so is technically
     * possible, it would have no effect since all font atlases would have to be rebuilt, which is out of reach.
     *
     * The game's original Shift-JIS tables are constant-folded into the library and selected from here.
     */

    /// <summary>
    ///     Selects the encoding table appropriate for the given <paramref name="game_lang"/> and <paramref name="game_type"/>.
    /// </summary>
    private static ReadOnlySpan<byte> _select_sjistbl(FhLangId game_lang, FhGameType game_type) {
        return game_lang switch {
            FhLangId.Japanese => game_type is FhGameType.FFX ? FhShiftJisTables.ffx_jp : FhShiftJisTables.ffx2_jp,
            FhLangId.Korean   => game_type is FhGameType.FFX ? FhShiftJisTables.ffx_kr : FhShiftJisTables.ffx2_kr,
            FhLangId.Chinese  => game_type is FhGameType.FFX ? FhShiftJisTables.ffx_ch : FhShiftJisTables.ffx2_ch,
            FhLangId.English  or
            FhLangId.French   or
            FhLangId.Spanish  or
            FhLangId.German   or
            FhLangId.Italian  => game_type is FhGameType.FFX ? FhShiftJisTables.ffx_us : FhShiftJisTables.ffx2_us,
            FhLangId.Debug    => game_type is FhGameType.FFX ? FhShiftJisTables.ffx_jp : FhShiftJisTables.ffx2_jp,
            _                 => throw new Exception($"no SJIS table known for game type {game_type} and language {game_lang}"),
        };
    }

    /// <summary>
    ///     Decodes an expression at the start of <paramref name="src"/> and
    ///     writes the resulting UTF-8 character into <paramref name="dest"/>.
    ///     <para/>
    ///     Only valid for Chinese, Japanese, and Korean game languages.
    ///     <para/>
    ///     Returns the amount of bytes written to <paramref name="dest"/>.
    /// </summary>
    private static int _decode_cjk(
        in ReadOnlySpan<byte> src,
           Span        <byte> dest,
           FhLangId           game_lang,
           FhGameType         game_type) {
        // FFX.exe+646250 (DecodingGamecodeJPCHKR) - cleaned up rewrite
        Trace.Assert(game_lang is FhLangId.Chinese or FhLangId.Japanese or FhLangId.Korean);

        ReadOnlySpan<byte> sjistbl     = _select_sjistbl(game_lang, game_type);
        int                sjistbl_idx = 0; // iVar4

        byte e0 = src[0];             // bVar2
        int  i  = e0 == 0x04 ? 1 : 0; // simulates increment of `src` pointer
        byte e1 = src[i];             // bVar1

        if (0x5B < e1 && e1 < 0x5F && game_lang is FhLangId.Japanese) {
            sjistbl[ 0x1E .. 0x21 ].CopyTo(dest);
            return 3;
        }

        if (e1 < 0x30) {
            sjistbl_idx = (e1 - 0x2B) * 0xD0;
            i++;
        }

        byte e2 = src[i];
        sjistbl_idx += (e2 - 0x30);

        if (e0 == 0x04) {
            sjistbl_idx += 0x410;
        }

        sjistbl_idx *= 3;

        if (sjistbl_idx > sjistbl.Length) {
            throw new Exception($"Rejected expression {e0:X2} {e1:X2} {e2:X2} in lang {game_lang}");
        }

        Range? copy_range = sjistbl_idx switch {
               0x1E when game_lang is FhLangId.Korean                        => 0x20              .. 0x21,
            <= 0x32                                                          => sjistbl_idx       .. (sjistbl_idx + 3),
               0x33                                                          => 0x33              .. 0x35,
             > 0x33 when game_lang is FhLangId.Chinese || sjistbl_idx < 0x78 => (sjistbl_idx - 1) .. (sjistbl_idx + 2),
               0x78                                                          => 0x77              .. 0x79,
             < 0x79                                                          => null,
               _                                                             => (sjistbl_idx - 2) .. (sjistbl_idx + 1)
        };

        if (copy_range is not Range valid_range)
            return 0;

        sjistbl[valid_range].CopyTo(dest);
        return valid_range.End.Value - valid_range.Start.Value;
    }

    /// <summary>
    ///     Decodes an expression at the start of <paramref name="src"/> and
    ///     writes the resulting UTF-8 character into <paramref name="dest"/>.
    ///     <para/>
    ///     Only valid for game languages that are not Chinese, Japanese, or Korean.
    ///     <para/>
    ///     Returns the amount of bytes written to <paramref name="dest"/>.
    /// </summary>
    private static int _decode_us(
        in ReadOnlySpan<byte> src,
           Span        <byte> dest,
           FhLangId           game_lang,
           FhGameType         game_type) {
        // FFX.exe+6463a0 (DecodingGamecodeUK) - cleaned up rewrite
        Trace.Assert(game_lang is not (FhLangId.Chinese or FhLangId.Korean or FhLangId.Japanese));

        // L9-15
        ReadOnlySpan<byte> sjistbl     = _select_sjistbl(game_lang, game_type);
        int                sjistbl_idx = 0; // iVar2

        int  i  = 0; // simulates increment of `src` pointer
        byte e0 = src[0];

        if (e0 < 0x30) {
            sjistbl_idx = (e0 - 0x2B) * 0xD0;
            i++;
        }

        byte e1 = src[i];
        sjistbl_idx += (e1 - 0x30);

        Range? copy_range = sjistbl_idx switch {
          < 0xC  => sjistbl_idx          .. (sjistbl_idx + 0x01),
            0xC  => 0xC                  .. 0xF,
          < 0x11 => (sjistbl_idx + 0x02) .. (sjistbl_idx + 0x03),
            0x11 => 0x13                 .. 0x16,
          < 0x3F => (sjistbl_idx + 0x04) .. (sjistbl_idx + 0x05),
            0x3F => 0x43                 .. 0x46,
          < 0x5E => (sjistbl_idx + 0x06) .. (sjistbl_idx + 0x07),
            0x5E => (sjistbl_idx + 0x06) .. (sjistbl_idx + 0x08),
            0x5F => (sjistbl_idx + 0x07) .. (sjistbl_idx + 0x0A),
            0x60 => (sjistbl_idx + 0x09) .. (sjistbl_idx + 0x0C),
            0x61 => (sjistbl_idx + 0x0B) .. (sjistbl_idx + 0x0E),
            0x62 => (sjistbl_idx + 0x0D) .. (sjistbl_idx + 0x10),
            0x63 => (sjistbl_idx + 0x0F) .. (sjistbl_idx + 0x10),
            0x64 => (sjistbl_idx + 0x0F) .. (sjistbl_idx + 0x12),
            0x65 => (sjistbl_idx + 0x11) .. (sjistbl_idx + 0x14),
            0x66 => (sjistbl_idx + 0x13) .. (sjistbl_idx + 0x16),
            0x67 => (sjistbl_idx + 0x15) .. (sjistbl_idx + 0x16),
            0x68 => (sjistbl_idx + 0x15) .. (sjistbl_idx + 0x17),
            0x69 => (sjistbl_idx + 0x16) .. (sjistbl_idx + 0x19),
            0x6A => (sjistbl_idx + 0x18) .. (sjistbl_idx + 0x1B),
            0x6B => (sjistbl_idx + 0x1A) .. (sjistbl_idx + 0x1D),
            0x6C => (sjistbl_idx + 0x1C) .. (sjistbl_idx + 0x1F),
            0x70 => 0x91                 .. 0x92,
            0xA0 => (sjistbl_idx + 0x50) .. (sjistbl_idx + 0x51),
            0xA1 => (sjistbl_idx + 0x50) .. (sjistbl_idx + 0x52),
            0xA2 => (sjistbl_idx + 0x51) .. (sjistbl_idx + 0x54),
            0xA3 => (sjistbl_idx + 0x53) .. (sjistbl_idx + 0x56),
            0xA4 => (sjistbl_idx + 0x55) .. (sjistbl_idx + 0x56),
            0xA5 => (sjistbl_idx + 0x55) .. (sjistbl_idx + 0x58),
            0xA6 => (sjistbl_idx + 0x57) .. (sjistbl_idx + 0x5A),
            0xA7 or
            0xA8 => (sjistbl_idx + 0x59) .. (sjistbl_idx + 0x5A),
            0xA9 => (sjistbl_idx + 0x59) .. (sjistbl_idx + 0x5C),
            0xAA => (sjistbl_idx + 0x5B) .. (sjistbl_idx + 0x5C),
            0xAB => (sjistbl_idx + 0x5B) .. (sjistbl_idx + 0x5E),
            0xAC => (sjistbl_idx + 0x5D) .. (sjistbl_idx + 0x5F),
            0xAD => (sjistbl_idx + 0x5E) .. (sjistbl_idx + 0x60),
            0xAE => (sjistbl_idx + 0x5F) .. (sjistbl_idx + 0x61),
            0xAF => (sjistbl_idx + 0x60) .. (sjistbl_idx + 0x62),
            0xB0 => (sjistbl_idx + 0x61) .. (sjistbl_idx + 0x63),
            0xB1 => (sjistbl_idx + 0x62) .. (sjistbl_idx + 0x64),
            0xB2 => (sjistbl_idx + 0x63) .. (sjistbl_idx + 0x65),
            0xB3 => (sjistbl_idx + 0x64) .. (sjistbl_idx + 0x66),
            0xB4 => (sjistbl_idx + 0x65) .. (sjistbl_idx + 0x67),
            0xB5 => (sjistbl_idx + 0x66) .. (sjistbl_idx + 0x68),
            0xB6 => (sjistbl_idx + 0x67) .. (sjistbl_idx + 0x69),
            0xB7 => (sjistbl_idx + 0x68) .. (sjistbl_idx + 0x6A),
            0xB8 => (sjistbl_idx + 0x69) .. (sjistbl_idx + 0x6C),
            0xB9 => (sjistbl_idx + 0x6B) .. (sjistbl_idx + 0x6E),
            0xBA => (sjistbl_idx + 0x6D) .. (sjistbl_idx + 0x6E),
            0xBB => (sjistbl_idx + 0x6D) .. (sjistbl_idx + 0x6F),
            0xBC => (sjistbl_idx + 0x6E) .. (sjistbl_idx + 0x71),
            0xBD => (sjistbl_idx + 0x70) .. (sjistbl_idx + 0x73),
            0xBE => (sjistbl_idx + 0x72) .. (sjistbl_idx + 0x75),
            0xBF => (sjistbl_idx + 0x74) .. (sjistbl_idx + 0x77),
            _    => null,
        };

        if (copy_range is Range valid_range) {
            sjistbl[valid_range].CopyTo(dest);
            return valid_range.End.Value - valid_range.Start.Value;
        }

        // L117-L244
        if (sjistbl_idx < 0x70) {
            dest[0] = sjistbl[(sjistbl_idx * 2) - 0x4F];
            dest[1] = sjistbl[(sjistbl_idx * 2) - 0x4F + 1];
            return 2;
        }
        else if (sjistbl_idx <= 0x9F) {
            dest[0] = sjistbl[(sjistbl_idx * 2) - 0x50];
            dest[1] = sjistbl[(sjistbl_idx * 2) - 0x50 + 1];
            return 2;
        }

        return 0;
    }

    /// <summary>
    ///     Given some DEdit syntax text in <paramref name="src"/>, computes the size of the buffer required to write all indices
    ///     of a given <paramref name="index_type"/> necessary for the game to access that text once encoded.
    ///     <para/>
    ///     Such a buffer is given as the second argument to <see cref="create_indices(Span{byte}, Span{byte}, FhDialogueIndexType)"/>.
    /// </summary>
    public static int compute_index_buffer_size(Span<byte> src, FhDialogueIndexType index_type) {
        int offset = 0;
        int size   = 0;

        while ((offset += src[offset .. ].IndexOf(_op_end) + _op_end.Length) < src.Length) {
            size += index_type switch {
                FhDialogueIndexType.I32_X2 => 8,
                FhDialogueIndexType.I32_X1 => 4,
                FhDialogueIndexType.I16_X2 => 4,
                FhDialogueIndexType.I16_X1 => 2,
                _                          => throw new NotImplementedException($"Unknown index type {index_type}"),
            };
        }

        return size;
    }

    /// <summary>
    ///     Given some game-encoded text in <paramref name="src"/>, writes to <paramref name="dest"/> the indices
    ///     of a given <paramref name="index_type"/> necessary for the game to access that text.
    ///     <para/>
    ///     <paramref name="dest"/> must be properly sized. To obtain the correct size, perform
    ///     <see cref="compute_index_buffer_size(Span{byte}, FhDialogueIndexType)"/> on the source DEdit text.
    /// </summary>
    public static void create_indices(Span<byte> src, Span<byte> dest, FhDialogueIndexType index_type) {
        int src_offset  = 0;
        int dest_offset = 0;
        int index       = short.CreateChecked(dest.Length);

        ReadOnlySpan<byte> nul = "\0"u8;

        while (dest_offset < dest.Length) {
            int        index_next_nul = src[src_offset .. ].IndexOf(nul) + nul.Length;
            Span<byte> line           = src[src_offset .. (src_offset + index_next_nul)];

            int option_count = 0;
            foreach (byte b in line) {
                if (b is 0x10) // {CHOICE}
                    option_count++;
            }

            src_offset  += index_next_nul;
            dest_offset += _write_index(dest[dest_offset .. ], index, option_count, index_type);
            index       += index_next_nul;
        }
    }

    /// <summary>
    ///     Reads a dialogue file index of a given <paramref name="index_type"/>.
    /// </summary>
    public static int read_index(ReadOnlySpan<byte> input, FhDialogueIndexType index_type, out int consumed) {
        consumed = index_type switch {
            FhDialogueIndexType.I32_X2 => 8,
            FhDialogueIndexType.I32_X1 or
            FhDialogueIndexType.I16_X2 => 4,
            FhDialogueIndexType.I16_X1 => 2,
            _                          => throw new Exception("UNREACHABLE")
        };

        short e0 = index_type switch {
            FhDialogueIndexType.I32_X2 or
            FhDialogueIndexType.I32_X1 => BinaryPrimitives.ReadInt16LittleEndian(input),
            FhDialogueIndexType.I16_X2 or
            FhDialogueIndexType.I16_X1 => BinaryPrimitives.ReadInt16LittleEndian(input),
            _                          => throw new Exception("UNREACHABLE")
        };

        byte option_count = index_type is FhDialogueIndexType.I32_X2 or FhDialogueIndexType.I32_X1
            ? input[3]
            : (byte)0;

        short e1 = index_type switch {
            FhDialogueIndexType.I32_X2 => BinaryPrimitives.ReadInt16LittleEndian(input[sizeof(int)   .. ]),
            FhDialogueIndexType.I32_X1 => 0,
            FhDialogueIndexType.I16_X2 => BinaryPrimitives.ReadInt16LittleEndian(input[sizeof(short) .. ]),
            FhDialogueIndexType.I16_X1 => 0,
            _                          => throw new Exception("UNREACHABLE")
        };

        ArgumentOutOfRangeException.ThrowIfNotEqual(e0, e1);
        return e0;
    }

    /// <summary>
    ///     Writes to <paramref name="dest"/> an index of a given <paramref name="index_type"/> with the literal
    ///     offset <paramref name="value"/> and option count <paramref name="options"/>.
    /// </summary>
    private static int _write_index(Span<byte> dest, int value, int options, FhDialogueIndexType index_type) {
        short index        = short.CreateChecked(value);
        byte  option_count = byte .CreateChecked(options);

        BinaryPrimitives.WriteInt16LittleEndian(dest, index);
        if (index_type is FhDialogueIndexType.I16_X1) {
            return 2;
        }

        if (index_type is FhDialogueIndexType.I16_X2) {
            BinaryPrimitives.WriteInt16LittleEndian(dest, index);
            return 4;
        }

        if (index_type is FhDialogueIndexType.I32_X1 or FhDialogueIndexType.I32_X2) {
            dest[2] = 0x00; // TODO: check actual purpose
            dest[3] = option_count;

            if (index_type is FhDialogueIndexType.I32_X1) return 4;

            dest[0 .. 4].CopyTo(dest[4 .. 8]);
            return 8;
        }

        throw new Exception("UNREACHABLE");
    }

    /// <summary>
    ///     Converts a given DEdit syntax <paramref name="expression"/>- a UTF-8 byte sequence starting with
    ///     U+007B and ending with U+007D- to game encoding and writes it to <paramref name="dest"/>.
    /// </summary>
    private static int _encode_expr(in ReadOnlySpan<byte> expression, Span<byte> dest) {
        byte               op_code = _select_op_code   (expression);
        ReadOnlySpan<byte> op      = _select_op_literal(op_code);

        dest[0] = op_code;

        // {(COLOR|BTN|KEY):X2}
        if (op_code is 0x0A or 0x0B or 0x23) {
            dest[1] = _get_op_arg1(expression, op);
            return 2;
        }

        // {(SPACE|TIME|CHOICE):(X2 - 0x30)}
        if (op_code is 0x07 or 0x09 or 0x10) {
            dest[1] = byte.CreateChecked(0x30 + _get_op_arg1(expression, op));
            return 2;
        }

        // {MACRO:(X2 - 0x13):(X2 - 0x30)}
        if (op_code is (>= 0x13 and <= 0x22)) {
            dest[1] = byte.CreateChecked(0x30 + _get_op_arg2(expression, op));
            return 2;
        }

        // {(END|PAUSE|LF)} or {UNKNOWN:ARG1}
        if (op_code is 0x00 or 0x01 or 0x02 or 0x03 or 0x05) {
            return 1;
        }

        // {UNKNOWN:ARG1:ARG2}
        if (op_code is 0x06 or 0x08 or (>= 0x0C and <= 0x0F) or 0x11 or 0x12 or (>= 0x24 and <= 0x2B)) {
            dest[1] = _get_op_arg2(expression, op);
            return 2;
        }

        throw new Exception($"unreachable - illegal expr {Convert.ToHexString(expression)}");
    }

    public static int compute_encode_buffer_size(
        in ReadOnlySpan<byte> src,
           FhLangId?          game_lang_override = default,
           FhGameType?        game_type_override = default) {

        FhLangId   game_lang = game_lang_override ?? FhGlobal.game_lang;
        FhGameType game_type = game_type_override ?? FhGlobal.game_type;

        ReadOnlySpan<byte> sjistbl = _select_sjistbl(game_lang, game_type);

        int src_offset, size;

        for (src_offset = 0, size = 0; src_offset < src.Length; ) {
            OperationStatus decode_status = Rune.DecodeFromUtf8(src[ src_offset .. ], out Rune code_point, out int consumed);

            if (decode_status != OperationStatus.Done) {
                FhInternal.Log.Info($"failed to get complete UTF8 codepoint in input; aborting (pos {src_offset}, R {decode_status})");
                return size;
            }

            if (_should_ignore_code_point(code_point)) {
                src_offset += consumed;
                continue;
            }

            ReadOnlySpan<byte> bytes = src[ src_offset .. (src_offset + consumed) ];

            if (code_point.Value == 0x007B) { // '{' (U+007B) - dialogue op statement opener
                while (code_point.Value != 0x007D) {  // '}' (U+007D) - dialogue op statement terminator
                    decode_status = Rune.DecodeFromUtf8(src[src_offset..], out code_point, out consumed);

                    if (decode_status != OperationStatus.Done) {
                        FhInternal.Log.Info($"failed to get complete UTF8 codepoint while parsing op statement; aborting (pos {src_offset}, R {decode_status})");
                        return size;
                    }

                    src_offset += consumed;
                }

                size += 2; // worst-case
                continue;
            }

            int sjistbl_index = sjistbl.IndexOf(bytes) + bytes.Length;

            if (sjistbl_index == -1) {
                FhInternal.Log.Error($"sequence {Convert.ToHexString(bytes)} not in sjistbl of lang {game_lang}; aborting (pos {src_offset})");
                continue;
            }

            if (game_lang is FhLangId.Chinese or FhLangId.Japanese or FhLangId.Korean) {
                // CJK-specific unwind - TODO: PROBABLY MORE REQUIRED
                if (0x33 < (sjistbl_index + 0x30) && (game_lang is FhLangId.Chinese || (sjistbl_index + 0x30) < 0x78)) {
                    sjistbl_index -= 1;
                }

                sjistbl_index /= 3;

                if (sjistbl_index >= 0x410) {
                    sjistbl_index -= 0x410;
                    size++;
                }
            }

            size       += (sjistbl_index + 0x30) > 0xFF ? 2 : 1;
            src_offset += consumed;
        }

        return size;
    }

    /// <summary>
    ///     Writes into <paramref name="dest"/> the game encoding representation of a
    ///     UTF-8 string contained in <paramref name="src"/>, for a given game type and language.
    ///     <para/>
    ///     If <paramref name="game_lang_override"/> or <paramref name="game_type_override"/>
    ///     are not explicitly specified, the active game type and language are used.
    ///     <para/>
    ///     If this function is called while Fahrenheit is not loaded into the game process,
    ///     <paramref name="game_lang_override"/> and <paramref name="game_type_override"/> MUST be specified.
    /// </summary>
    public static int encode(
        in ReadOnlySpan<byte> src,
           Span        <byte> dest,
           FhLangId?          game_lang_override = default,
           FhGameType?        game_type_override = default) {

        FhLangId   game_lang = game_lang_override ?? FhGlobal.game_lang;
        FhGameType game_type = game_type_override ?? FhGlobal.game_type;

        ReadOnlySpan<byte> sjistbl = _select_sjistbl(game_lang, game_type);

        int src_offset, dest_offset;

        for (src_offset = 0, dest_offset = 0; src_offset < src.Length; ) {

            /* [fkelava 14/10/25 19:27]
             * Obtain the next complete UTF-8 code point in the input buffer.
             * If it is U+000D (CR), U+000A (LF), or U+FEFF (BOM), bypass it.
             */

            OperationStatus decode_status = Rune.DecodeFromUtf8(src[ src_offset .. ], out Rune code_point, out int consumed);

            if (decode_status != OperationStatus.Done) {
                FhInternal.Log.Info($"failed to get complete UTF8 codepoint in input; aborting (pos {src_offset}, R {decode_status})");
                return dest_offset;
            }

            if (_should_ignore_code_point(code_point)) {
                src_offset += consumed;
                continue;
            }

            ReadOnlySpan<byte> bytes = src[ src_offset .. (src_offset + consumed) ];

            /* [fkelava 14/10/25 19:27]
             * If we have encountered a op statement opener (U+007B '{'), process the op by
             * consuming complete UTF-8 code points until a statement terminator (U+007D '}'),
             * then encoding the complete expression. The usual form is {CMD:ARG1:ARG2}.
             */

            if (code_point.Value == 0x007B) { // '{' (U+007B) - dialogue op statement opener
                int expr_start_offset = src_offset;

                while (code_point.Value != 0x007D) {  // '}' (U+007D) - dialogue op statement terminator
                    decode_status = Rune.DecodeFromUtf8(src[src_offset..], out code_point, out consumed);

                    if (decode_status != OperationStatus.Done) {
                        FhInternal.Log.Info($"failed to get complete UTF8 codepoint while parsing op statement; aborting (pos {src_offset}, R {decode_status})");
                        return dest_offset;
                    }

                    src_offset += consumed;
                }

                dest_offset += _encode_expr(src[ expr_start_offset .. src_offset ], dest[ dest_offset .. ]);
                continue;
            }

            /* [fkelava 14/10/25 19:27]
             * If the character is NOT an op statement opener, find it in the relevant
             * sjistable and emit the index required to reach it. This is the inverse
             * of _decode_us() and _decode_cjk().
             */

            int sjistbl_index = sjistbl.IndexOf(bytes) + bytes.Length;

            if (sjistbl_index == -1) {
                FhInternal.Log.Error($"sequence {Convert.ToHexString(bytes)} not in sjistbl of lang {game_lang}; aborting (pos {src_offset})");
                continue;
            }

            if (game_lang is FhLangId.Chinese or FhLangId.Japanese or FhLangId.Korean) {
                // CJK-specific unwind - TODO: PROBABLY MORE REQUIRED
                if (0x33 < (sjistbl_index + 0x30) && (game_lang is FhLangId.Chinese || (sjistbl_index + 0x30) < 0x78)) {
                    sjistbl_index -= 1;
                }

                sjistbl_index /= 3;

                if (sjistbl_index >= 0x410) {
                    sjistbl_index -= 0x410;
                    dest[dest_offset++] = 0x04;
                }
            }

            if ((sjistbl_index + 0x30) > 0xFF) {
                dest[dest_offset++] = byte.CreateChecked((sjistbl_index / 0xD0) + 0x2B);
                dest[dest_offset++] = byte.CreateChecked((sjistbl_index % 0xD0) + 0x30);
            }
            else {
                dest[dest_offset++] = byte.CreateChecked(sjistbl_index + 0x30);
            }

            src_offset += consumed;
        }

        return dest_offset;
    }

    /// <summary>
    ///     Computes, in bytes, how large a buffer should be to store the decoded contents of <paramref name="src"/>.
    /// </summary>
    public static int compute_decode_buffer_size(
        in ReadOnlySpan<byte> src,
           FhLangId?          game_lang_override = default,
           FhGameType?        game_type_override = default) {

        FhLangId   game_lang = game_lang_override ?? FhGlobal.game_lang;
        FhGameType game_type = game_type_override ?? FhGlobal.game_type;

        int src_offset, size;
        Span<byte> scratch_buffer = stackalloc byte[64]; // no single expression can exceed this size

        for (src_offset = 0, size = 0; src_offset < src.Length; ) {
            byte current_op = src[src_offset];
            (int src_increment, int size_increment) = current_op switch {
                0x00                  => (0, _decode_op(scratch_buffer, src[src_offset])),
                0x01 or 0x03          => (1, _decode_op(scratch_buffer, src[src_offset])),
                0x0A or 0x0B or 0x23  => (2, _decode_op(scratch_buffer, src[src_offset], src[src_offset + 1])),
                0x07 or 0x09 or 0x10  => (2, _decode_op(scratch_buffer, src[src_offset], byte.CreateChecked(src[src_offset + 1] - 0x30))),
                >= 0x13 and <= 0x22   => (2, _decode_op(scratch_buffer, src[src_offset], byte.CreateChecked(src[src_offset] - 0x13), byte.CreateChecked(src[src_offset + 1] - 0x30))),
                0x04                  => (src[src_offset + 1] < 0x30 ? 3 : 2, _decode_cjk (src[ src_offset .. ], scratch_buffer, game_lang, game_type)),
                >= 0x2C and <= 0x2F   => (2,                                  _decode_char(src[ src_offset .. ], scratch_buffer, game_lang, game_type)),
                0x02 or 0x05          => (1, _decode_op(scratch_buffer, src[src_offset], src[src_offset + 1])),
                0x06 or
                0x08 or
                (>= 0x0C and <= 0x0F) or
                0x11 or
                0x12 or
                (>= 0x24 and <= 0x2B) => (2, _decode_op(scratch_buffer, src[src_offset], src[src_offset], src[src_offset + 1])),
                _                     => (1, _decode_char(src[ src_offset .. ], scratch_buffer, game_lang, game_type))
            };

            src_offset += src_increment;
            size       += size_increment;

            if (current_op == 0x00) {
                return size + "\r\n"u8.Length;
            }
        }

        return size;
    }

    /// <summary>
    ///     Writes into <paramref name="dest"/> the UTF-8 representation of a
    ///     game encoding string contained in <paramref name="src"/>, for a given game type and language.
    ///     <para/>
    ///     If <paramref name="game_lang_override"/> or <paramref name="game_type_override"/>
    ///     are not explicitly specified, the active game type and language are used.
    ///     <para/>
    ///     If this function is called while Fahrenheit is not loaded into the game process,
    ///     <paramref name="game_lang_override"/> and <paramref name="game_type_override"/> MUST be specified.
    /// </summary>
    public static int decode(
        in ReadOnlySpan<byte> src,
           Span        <byte> dest,
           FhLangId?          game_lang_override = default,
           FhGameType?        game_type_override = default) {

        FhLangId   game_lang = game_lang_override ?? FhGlobal.game_lang;
        FhGameType game_type = game_type_override ?? FhGlobal.game_type;

        int src_offset, dest_offset;

        for (src_offset = 0, dest_offset = 0; src_offset < src.Length; ) {
            byte current_op = src[src_offset];

            (int src_increment, int dest_increment) = current_op switch {
                // {END} - statement terminator
                0x00                  => (0, _decode_op(dest[ dest_offset .. ], src[src_offset])),
                // {PAUSE|LF} - 0-arg ops
                0x01 or 0x03          => (1, _decode_op(dest[ dest_offset .. ], src[src_offset])),
                // {COLOR|BTN|KEY} - 1-arg ops
                0x0A or 0x0B or 0x23  => (2, _decode_op(dest[ dest_offset .. ], src[src_offset], src[src_offset + 1])),
                // {SPACE|TIME|CHOICE} - 1-arg ops with 0x30-based indexing
                0x07 or 0x09 or 0x10  => (2, _decode_op(dest[ dest_offset .. ], src[src_offset], byte.CreateChecked(src[src_offset + 1] - 0x30))),
                // {MACRO} - 2-arg ops where the op itself is transformed to an argument:
                >= 0x13 and <= 0x22   => (2, _decode_op(dest[ dest_offset .. ], src[src_offset], byte.CreateChecked(src[src_offset] - 0x13), byte.CreateChecked(src[src_offset + 1] - 0x30))),
                // Multi-byte character expressions - 0x04 is CJK-only, 0x2C-2F for any charset
                0x04                  => (src[src_offset + 1] < 0x30 ? 3 : 2, _decode_cjk (src[ src_offset .. ], dest[ dest_offset .. ], game_lang, game_type)),
                >= 0x2C and <= 0x2F   => (2,                                  _decode_char(src[ src_offset .. ], dest[ dest_offset .. ], game_lang, game_type)),
                // Unknown ops, embedded literally
                0x02 or 0x05          => (1, _decode_op(dest[ dest_offset .. ], src[src_offset], src[src_offset + 1])),
                0x06 or
                0x08 or
                (>= 0x0C and <= 0x0F) or
                0x11 or
                0x12 or
                (>= 0x24 and <= 0x2B) => (2, _decode_op(dest[ dest_offset .. ], src[src_offset], src[src_offset], src[src_offset + 1])),
                // fall through: regular character
                _                     => (1, _decode_char(src[ src_offset .. ], dest[ dest_offset .. ], game_lang, game_type))
            };

            src_offset  += src_increment;
            dest_offset += dest_increment;

            if (current_op == 0x00) {
                ReadOnlySpan<byte> newline = "\r\n"u8;
                newline.CopyTo(dest[dest_offset .. ]);

                return dest_offset + newline.Length;
            }
        }

        return dest_offset;
    }
}
