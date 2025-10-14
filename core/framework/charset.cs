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

public enum FhDialogueIndexType {
    I32_X2 = 1,
    I32_X1 = 2,
    I16_X2 = 3,
    I16_X1 = 4,
}

// incomplete
public enum FhDialogueOpCode {
    PAUSE      = 0x01,
    LINE_BREAK = 0x03,
    CJK_EXT_0  = 0x04,
    SPACE      = 0x07,
    TIME       = 0x09,
    COLOR      = 0x0A,
    CTRL_BTN   = 0x0B,
    CHOICE     = 0x10,
    MACRO      = 0x13,
    KEY        = 0x23,
    CJK_EXT_1  = 0x2C,
    CJK_EXT_2  = 0x2D,
    CJK_EXT_3  = 0x2E,
    CJK_EXT_4  = 0x2F,
}

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

public static class FhCharset {

    /* [fkelava 14/10/25 14:58]
     * Bytes below 0x30 are to be interpreted as a dialogue 'op',
     * which is a modifier to the dialogue that follows it.
     *
     * These ops may take one or two arguments, and the opcode itself
     * sometimes acts as one of them after transformation.
     *
     * These literals and methods control the textual representation of
     * these ops when decoding, and how the encoder shall identify them.
     * Changing them constitutes a breaking change.
     */

    private static ReadOnlySpan<byte> _op_end     => "{END}"u8;
    private static ReadOnlySpan<byte> _op_pause   => "{PAUSE}"u8;
    private static ReadOnlySpan<byte> _op_newline => "{LF}"u8;
    private static ReadOnlySpan<byte> _op_time    => "{TIME:"u8;
    private static ReadOnlySpan<byte> _op_color   => "{COLOR:"u8;
    private static ReadOnlySpan<byte> _op_btn     => "{BTN:"u8;
    private static ReadOnlySpan<byte> _op_choice  => "{CHOICE:"u8;
    private static ReadOnlySpan<byte> _op_var     => "{VAR:"u8;
    private static ReadOnlySpan<byte> _op_macro   => "{MACRO:"u8;
    private static ReadOnlySpan<byte> _op_key     => "{KEY:"u8;
    private static ReadOnlySpan<byte> _op_unknown => "{UNKNOWN:"u8;

    private static byte _op_decode_arg1(in ReadOnlySpan<byte> expression, in ReadOnlySpan<byte> op) {
        return byte.Parse(expression[op.Length .. (op.Length + 2)], NumberStyles.HexNumber);
    }

    private static byte _op_decode_arg2(in ReadOnlySpan<byte> expression, in ReadOnlySpan<byte> op) {
        return byte.Parse(expression[(op.Length + 3) .. (op.Length + 5)], NumberStyles.HexNumber);
    }

    private static ReadOnlySpan<byte> _op_encode_arg1(byte op_arg1) {
        return Encoding.UTF8.GetBytes($"{op_arg1:X2}}}");
    }

    private static ReadOnlySpan<byte> _op_encode_arg2(byte op_arg1, byte op_arg2) {
        return Encoding.UTF8.GetBytes($"{op_arg1:X2}:{op_arg2:X2}}}");
    }

    private static bool _should_ignore_code_point(Rune codepoint) {
        return codepoint.Value is 0x0D    // U+000D - <Carriage Return> (CR)
                               or 0x0A    // U+000A - <End of Line> (EOL, LF, NL)
                               or 0xFEFF; // U+FEFF - Zero Width No-Break Space (BOM, ZWNBSP)
    }

    /// <summary>
    ///     For a given <paramref name="opcode"/>, produces the UTF-8 textual literal that represents it.
    /// </summary>
    private static ReadOnlySpan<byte> _select_op_literal(byte opcode) {
        return opcode switch {
            0x00                => _op_end,
            0x01                => _op_pause,
            0x03                => _op_newline,
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
            _ when expression.IndexOf(_op_time   ) != -1 => 0x09,
            _ when expression.IndexOf(_op_color  ) != -1 => 0x0A,
            _ when expression.IndexOf(_op_btn    ) != -1 => 0x0B,
            _ when expression.IndexOf(_op_choice ) != -1 => 0x10,
            _ when expression.IndexOf(_op_var    ) != -1 => 0x12,
            _ when expression.IndexOf(_op_macro  ) != -1 => byte.CreateChecked(0x13 + _op_decode_arg1(expression, _op_macro)),
            _ when expression.IndexOf(_op_key    ) != -1 => 0x23,
            _                                            => _op_decode_arg1(expression, _op_unknown),
        };
    }

    /* [fkelava 14/10/25 14:58]
     * The custom encoding of the game fundamentally functions by computing indices into a 'table'
     * which is a long UTF-8 string. These are called 'sjistbl', or Shift-JIS tables.
     *
     * Fahrenheit does not support modifying the game's original tables. While doing so is technically
     * possible, it would have no effect since all font atlases would have to be rebuilt, which is currently
     * (and possibly forever) completely out of reach.
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

    /* [fkelava 08/10/25 19:51]
     * FFX.exe+646250 (DecodingGamecodeJPCHKR)
     * literal rewrite - unoptimized
     */

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

        Trace.Assert(game_lang is FhLangId.Chinese or FhLangId.Japanese or FhLangId.Korean);

        // L12-18
        ReadOnlySpan<byte> sjistbl     = _select_sjistbl(game_lang, game_type);
        int                sjistbl_idx = 0; // iVar4

        int  i  = 0;      // simulates increment of `src` pointer
        byte e0 = src[0]; // bVar2

        if (e0 == 0x04) {
            i++;
        }

        byte e1 = src[i]; // bVar1

        // L19-23
        if (0x5B < e1 && e1 < 0x5F && game_lang is FhLangId.Japanese) {
            dest[0] = sjistbl[0x1E];
            dest[1] = sjistbl[0x1F];
            dest[2] = sjistbl[0x20];
            return 3;
        }

        // L24-32
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

        /* [fkelava 08/10/25 16:40]
         * The game doesn't in any way bounds-check indexing into the current `sjistbl`.
         * This is a bug, but doesn't crash the game outright. This is because the game
         * allocates the backing buffer with TkMalloc, which:
         *
         * - aligns the allocation to the nearest 0x40
         * - overallocates to the nearest multiple of ~2/4kB, presumably for page alignment
         *
         * so while natural game expressions can in fact overreach the sjistbl,
         * they don't read garbage but instead return all zeroes. We mirror this more directly.
         */

        if (sjistbl_idx > sjistbl.Length) {
            FhInternal.Log.Error($"Rejected expression {e0:X2} {e1:X2} {e2:X2} in lang {game_lang}");
            return 3;
        }

        // L33-65
        if (sjistbl_idx == 0x1E) {
            if (game_lang is FhLangId.Korean) {
                dest[0] = 0x20;
                return 1;
            }
        }
        else if (0x32 < sjistbl_idx) {
            if (sjistbl_idx == 0x33) {
                dest[0] = sjistbl[0x33];
                dest[1] = sjistbl[0x34];
                return 2;
            }
            if (0x33 < sjistbl_idx && (game_lang is FhLangId.Chinese || sjistbl_idx < 0x78)) {
                dest[0] = sjistbl[sjistbl_idx - 1];
                dest[1] = sjistbl[sjistbl_idx];
                dest[2] = sjistbl[sjistbl_idx + 1];
                return 3;
            }
            if (sjistbl_idx == 0x78) {
                dest[0] = sjistbl[0x77];
                dest[1] = sjistbl[0x78];
                return 2;
            }
            if (sjistbl_idx < 0x79) {
                return 0;
            }
            dest[0] = sjistbl[sjistbl_idx - 2];
            dest[1] = sjistbl[sjistbl_idx - 1];
            dest[2] = sjistbl[sjistbl_idx];
            return 3;
        }

        dest[0] = sjistbl[sjistbl_idx];
        dest[1] = sjistbl[sjistbl_idx + 1];
        dest[2] = sjistbl[sjistbl_idx + 2];
        return 3;
    }

    /* [fkelava 08/10/25 19:51]
     * FFX.exe+6463a0 (DecodingGamecodeUK)
     * literal rewrite - unoptimized
     */
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

        // L16-L116
        if (sjistbl_idx < 0x6D) {
            if (sjistbl_idx < 0xC) {
                dest[0] = sjistbl[sjistbl_idx];
                return 1;
            }
            if (sjistbl_idx == 0xC) {
                dest[0] = sjistbl[0xC];
                dest[1] = sjistbl[0xD];
                dest[2] = sjistbl[0xE];
                return 3;
            }
            if (sjistbl_idx - 0xD < 0x04) {
                dest[0] = sjistbl[sjistbl_idx + 2];
                return 1;
            }
            if (sjistbl_idx != 0x11) {
                if (sjistbl_idx - 0x12 < 0x2D) {
                    dest[0] = sjistbl[sjistbl_idx + 4];
                    return 1;
                }
                if (sjistbl_idx == 0x3F) {
                    dest[0] = sjistbl[0x43];
                    dest[1] = sjistbl[0x44];
                    dest[2] = sjistbl[0x45];
                    return 3;
                }
                if (sjistbl_idx - 0x40 < 0x1E) {
                    dest[0] = sjistbl[sjistbl_idx + 6];
                    return 1;
                }

                switch (sjistbl_idx) {
                    case 0x5E:
                        dest[0] = sjistbl[sjistbl_idx + 0x06];
                        dest[1] = sjistbl[sjistbl_idx + 0x07];
                        return 2;
                    case 0x5F:
                        dest[0] = sjistbl[sjistbl_idx + 0x07];
                        dest[1] = sjistbl[sjistbl_idx + 0x08];
                        dest[2] = sjistbl[sjistbl_idx + 0x09];
                        return 3;
                    case 0x60:
                        dest[0] = sjistbl[sjistbl_idx + 0x09];
                        dest[1] = sjistbl[sjistbl_idx + 0x0A];
                        dest[2] = sjistbl[sjistbl_idx + 0x0B];
                        return 3;
                    case 0x61:
                        dest[0] = sjistbl[sjistbl_idx + 0x0B];
                        dest[1] = sjistbl[sjistbl_idx + 0x0C];
                        dest[2] = sjistbl[sjistbl_idx + 0x0D];
                        return 3;
                    case 0x62:
                        dest[0] = sjistbl[sjistbl_idx + 0x0D];
                        dest[1] = sjistbl[sjistbl_idx + 0x0E];
                        dest[2] = sjistbl[sjistbl_idx + 0x0F];
                        return 3;
                    case 0x63:
                        dest[0] = sjistbl[sjistbl_idx + 0x0F];
                        return 1;
                    case 0x64:
                        dest[0] = sjistbl[sjistbl_idx + 0x0F];
                        dest[1] = sjistbl[sjistbl_idx + 0x10];
                        dest[2] = sjistbl[sjistbl_idx + 0x11];
                        return 3;
                    case 0x65:
                        dest[0] = sjistbl[sjistbl_idx + 0x11];
                        dest[1] = sjistbl[sjistbl_idx + 0x12];
                        dest[2] = sjistbl[sjistbl_idx + 0x13];
                        return 3;
                    case 0x66:
                        dest[0] = sjistbl[sjistbl_idx + 0x13];
                        dest[1] = sjistbl[sjistbl_idx + 0x14];
                        dest[2] = sjistbl[sjistbl_idx + 0x15];
                        return 3;
                    case 0x67:
                        dest[0] = sjistbl[sjistbl_idx + 0x15];
                        return 1;
                    case 0x68:
                        dest[0] = sjistbl[sjistbl_idx + 0x15];
                        dest[1] = sjistbl[sjistbl_idx + 0x16];
                        return 2;
                    case 0x69:
                        dest[0] = sjistbl[sjistbl_idx + 0x16];
                        dest[1] = sjistbl[sjistbl_idx + 0x17];
                        dest[2] = sjistbl[sjistbl_idx + 0x18];
                        return 3;
                    case 0x6A:
                        dest[0] = sjistbl[sjistbl_idx + 0x18];
                        dest[1] = sjistbl[sjistbl_idx + 0x19];
                        dest[2] = sjistbl[sjistbl_idx + 0x1A];
                        return 3;
                    case 0x6B:
                        dest[0] = sjistbl[sjistbl_idx + 0x1A];
                        dest[1] = sjistbl[sjistbl_idx + 0x1B];
                        dest[2] = sjistbl[sjistbl_idx + 0x1C];
                        return 3;
                    case 0x6C:
                        dest[0] = sjistbl[sjistbl_idx + 0x1C];
                        dest[1] = sjistbl[sjistbl_idx + 0x1D];
                        dest[2] = sjistbl[sjistbl_idx + 0x1E];
                        return 3;
                    default:
                        return 0;
                }
            }
        }

        // L117-L244 - control flow is EXTREMELY unclear
        if (sjistbl_idx - 0x6D < 0x03) {
            dest[0] = sjistbl[(sjistbl_idx * 2) - 0x4F];
            dest[1] = sjistbl[(sjistbl_idx * 2) - 0x4F + 1];
            return 2;
        }
        else {
            if (sjistbl_idx == 0x70) {
                dest[0] = sjistbl[0x91];
                return 1;
            }

            if (0x2E >= sjistbl_idx - 0x71U) {
                dest[0] = sjistbl[(sjistbl_idx * 2) - 0x50];
                dest[1] = sjistbl[(sjistbl_idx * 2) - 0x50 + 1];
                return 2;
            }
        }

        if (0x9F < sjistbl_idx) {
            switch (sjistbl_idx) {
                case 0xA0:
                    dest[0] = sjistbl[sjistbl_idx + 0x50];
                    return 1;
                case 0xA1:
                    dest[0] = sjistbl[sjistbl_idx + 0x50];
                    dest[1] = sjistbl[sjistbl_idx + 0x51];
                    return 2;
                case 0xA2:
                    dest[0] = sjistbl[sjistbl_idx + 0x51];
                    dest[1] = sjistbl[sjistbl_idx + 0x52];
                    dest[2] = sjistbl[sjistbl_idx + 0x53];
                    return 3;
                case 0xA3:
                    dest[0] = sjistbl[sjistbl_idx + 0x53];
                    dest[1] = sjistbl[sjistbl_idx + 0x54];
                    dest[2] = sjistbl[sjistbl_idx + 0x55];
                    return 3;
                case 0xA4:
                    dest[0] = sjistbl[sjistbl_idx + 0x55];
                    return 1;
                case 0xA5:
                    dest[0] = sjistbl[sjistbl_idx + 0x55];
                    dest[1] = sjistbl[sjistbl_idx + 0x56];
                    dest[2] = sjistbl[sjistbl_idx + 0x57];
                    return 3;
                case 0xA6:
                    dest[0] = sjistbl[sjistbl_idx + 0x57];
                    dest[1] = sjistbl[sjistbl_idx + 0x58];
                    dest[2] = sjistbl[sjistbl_idx + 0x59];
                    return 3;
                case 0xA7:
                case 0xA8:
                    dest[0] = sjistbl[sjistbl_idx + 0x59];
                    return 1;
                case 0xA9:
                    dest[0] = sjistbl[sjistbl_idx + 0x59];
                    dest[1] = sjistbl[sjistbl_idx + 0x5A];
                    dest[2] = sjistbl[sjistbl_idx + 0x5B];
                    return 3;
                case 0xAA:
                    dest[0] = sjistbl[sjistbl_idx + 0x5B];
                    return 1;
                case 0xAB:
                    dest[0] = sjistbl[sjistbl_idx + 0x5B];
                    dest[1] = sjistbl[sjistbl_idx + 0x5C];
                    dest[2] = sjistbl[sjistbl_idx + 0x5D];
                    return 3;
                case 0xAC:
                    dest[0] = sjistbl[sjistbl_idx + 0x5D];
                    dest[1] = sjistbl[sjistbl_idx + 0x5E];
                    return 2;
                case 0xAD:
                    dest[0] = sjistbl[sjistbl_idx + 0x5E];
                    dest[1] = sjistbl[sjistbl_idx + 0x5F];
                    return 2;
                case 0xAE:
                    dest[0] = sjistbl[sjistbl_idx + 0x5F];
                    dest[1] = sjistbl[sjistbl_idx + 0x60];
                    return 2;
                case 0xAF:
                    dest[0] = sjistbl[sjistbl_idx + 0x60];
                    dest[1] = sjistbl[sjistbl_idx + 0x61];
                    return 2;
                case 0xB0:
                    dest[0] = sjistbl[sjistbl_idx + 0x61];
                    dest[1] = sjistbl[sjistbl_idx + 0x62];
                    return 2;
                case 0xB1:
                    dest[0] = sjistbl[sjistbl_idx + 0x62];
                    dest[1] = sjistbl[sjistbl_idx + 0x63];
                    return 2;
                case 0xB2:
                    dest[0] = sjistbl[sjistbl_idx + 0x63];
                    dest[1] = sjistbl[sjistbl_idx + 0x64];
                    return 2;
                case 0xB3:
                    dest[0] = sjistbl[sjistbl_idx + 0x64];
                    dest[1] = sjistbl[sjistbl_idx + 0x65];
                    return 2;
                case 0xB4:
                    dest[0] = sjistbl[sjistbl_idx + 0x65];
                    dest[1] = sjistbl[sjistbl_idx + 0x66];
                    return 1;
                case 0xB5:
                    dest[0] = sjistbl[sjistbl_idx + 0x66];
                    dest[1] = sjistbl[sjistbl_idx + 0x67];
                    return 2;
                case 0xB6:
                    dest[0] = sjistbl[sjistbl_idx + 0x67];
                    dest[1] = sjistbl[sjistbl_idx + 0x68];
                    return 2;
                case 0xB7:
                    dest[0] = sjistbl[sjistbl_idx + 0x68];
                    dest[1] = sjistbl[sjistbl_idx + 0x69];
                    return 2;
                case 0xB8:
                    dest[0] = sjistbl[sjistbl_idx + 0x69];
                    dest[1] = sjistbl[sjistbl_idx + 0x6A];
                    dest[2] = sjistbl[sjistbl_idx + 0x6B];
                    return 3;
                case 0xB9:
                    dest[0] = sjistbl[sjistbl_idx + 0x6B];
                    dest[1] = sjistbl[sjistbl_idx + 0x6C];
                    dest[2] = sjistbl[sjistbl_idx + 0x6D];
                    return 3;
                case 0xBA:
                    dest[0] = sjistbl[sjistbl_idx + 0x6D];
                    return 1;
                case 0xBB:
                    dest[0] = sjistbl[sjistbl_idx + 0x6D];
                    dest[1] = sjistbl[sjistbl_idx + 0x6E];
                    return 2;
                case 0xBC:
                    dest[0] = sjistbl[sjistbl_idx + 0x6E];
                    dest[1] = sjistbl[sjistbl_idx + 0x6F];
                    dest[2] = sjistbl[sjistbl_idx + 0x70];
                    return 3;
                case 0xBD:
                    dest[0] = sjistbl[sjistbl_idx + 0x70];
                    dest[1] = sjistbl[sjistbl_idx + 0x71];
                    dest[2] = sjistbl[sjistbl_idx + 0x72];
                    return 3;
                case 0xBE:
                    dest[0] = sjistbl[sjistbl_idx + 0x72];
                    dest[1] = sjistbl[sjistbl_idx + 0x73];
                    dest[1] = sjistbl[sjistbl_idx + 0x74];
                    return 3;
                case 0xBF:
                    dest[0] = sjistbl[sjistbl_idx + 0x74];
                    dest[1] = sjistbl[sjistbl_idx + 0x75];
                    dest[0] = sjistbl[sjistbl_idx + 0x76];
                    return 3;
            }
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
    private static int _encode_expr(
        in ReadOnlySpan<byte> expression,
           Span<byte>         dest) {

        byte               op_code = _select_op_code   (expression);
        ReadOnlySpan<byte> op      = _select_op_literal(op_code);

        dest[0] = op_code;

        // {(COLOR|BTN|KEY):X2}
        if (op_code is 0x0A or 0x0B or 0x23) {
            dest[1] = _op_decode_arg1(expression, op);
            return 2;
        }

        // {(SPACE|TIME|CHOICE):(X2 - 0x30)}
        if (op_code is 0x07 or 0x09 or 0x10) {
            dest[1] = byte.CreateChecked(0x30 + _op_decode_arg1(expression, op));
            return 2;
        }

        // {MACRO:(X2 - 0x13):(X2 - 0x30)}
        if (op_code is (>= 0x13 and <= 0x22)) {
            dest[1] = byte.CreateChecked(0x30 + _op_decode_arg2(expression, op));
            return 2;
        }

        // {(END|PAUSE|LF)} or {UNKNOWN:ARG1}
        if (op_code is 0x00 or 0x01 or 0x02 or 0x03 or 0x05) {
            return 1;
        }

        // {UNKNOWN:ARG1:ARG2}
        if (op_code is 0x06 or 0x08 or (>= 0x0C and <= 0x0F) or 0x11 or 0x12 or (>= 0x24 and <= 0x2B)) {
            dest[1] = _op_decode_arg2(expression, op);
            return 2;
        }

        throw new Exception($"unreachable - illegal expr {Convert.ToHexString(expression)}");
    }

    /// <summary>
    ///     Writes into <paramref name="dest"/> the game encoding
    ///     representation of a UTF-8 string contained in <paramref name="src"/>,
    ///     for a given game type and language.
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

            OperationStatus decode_status = Rune.DecodeFromUtf8(src[src_offset..], out Rune code_point, out int consumed);

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

                dest_offset += _encode_expr(src[expr_start_offset .. src_offset], dest[dest_offset .. ]);
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
                byte e0 = byte.CreateChecked((sjistbl_index / 0xD0) + 0x2B);
                byte e1 = byte.CreateChecked((sjistbl_index % 0xD0) + 0x30);
                dest[dest_offset++] = e0;
                dest[dest_offset++] = e1;
            }
            else {
                dest[dest_offset++] = byte.CreateChecked(sjistbl_index + 0x30);
            }

            src_offset += consumed;
        }

        return dest_offset;
    }

    /// <summary>
    ///     Writes into <paramref name="dest"/> the UTF-8 representation of a
    ///     game encoding string contained in <paramref name="src"/>, for a
    ///     given game type and language.
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

        ReadOnlySpan<byte> op;        // for ops which are standalone
        ReadOnlySpan<byte> op_suffix; // for ops which take arguments

        byte op_arg1,    op_arg2;
        int  src_offset, dest_offset;

        for (src_offset = 0, dest_offset = 0; src_offset < src.Length; ) {
            switch (src[src_offset]) {
                case 0x00:
                    op = _select_op_literal(src[src_offset]);
                    op.CopyTo(dest[dest_offset .. ]);

                    // for legibility's sake
                    ReadOnlySpan<byte> newline = "\r\n"u8;
                    newline.CopyTo(dest[(dest_offset + op.Length) .. ]);

                    return dest_offset + op.Length + newline.Length;

                // 0-arg ops

                case 0x01: // {PAUSE}
                case 0x03: // {LF}
                    op = _select_op_literal(src[src_offset]);
                    op.CopyTo(dest[dest_offset..]);

                    src_offset  += 1;
                    dest_offset += op.Length;
                    break;

                // 1-arg ops

                case 0x0A: // {COLOR}
                case 0x0B: // {BTN}
                case 0x23: // {KEY}
                    op        = _select_op_literal(src[src_offset]);
                    op_arg1   = src[src_offset + 1];
                    op_suffix = _op_encode_arg1(op_arg1);

                    op       .CopyTo(dest[dest_offset .. ]);
                    op_suffix.CopyTo(dest[(dest_offset + op.Length) .. ]);

                    src_offset  += 2;
                    dest_offset += (op.Length + op_suffix.Length);
                    break;

                // 1-arg ops with 0x30-based indexing

                case 0x07: // {SPACE}
                case 0x09: // {TIME}
                case 0x10: // {CHOICE}
                    op        = _select_op_literal(src[src_offset]);
                    op_arg1   = byte.CreateChecked(src[src_offset + 1] - 0x30);
                    op_suffix = _op_encode_arg1(op_arg1);

                    op       .CopyTo(dest[dest_offset .. ]);
                    op_suffix.CopyTo(dest[(dest_offset + op.Length) .. ]);

                    src_offset  += 2;
                    dest_offset += (op.Length + op_suffix.Length);
                    break;

                // 2-arg ops where the op itself is transformed to an argument

                case 0x13: // {MACRO}
                case 0x14:
                case 0x15:
                case 0x17:
                case 0x18:
                case 0x19:
                case 0x1A:
                case 0x1B:
                case 0x1C:
                case 0x1D:
                case 0x1E:
                case 0x1F:
                case 0x20:
                case 0x21:
                case 0x22:
                    op        = _select_op_literal(src[src_offset]);
                    op_arg1   = byte.CreateChecked(src[src_offset]     - 0x13);
                    op_arg2   = byte.CreateChecked(src[src_offset + 1] - 0x30);
                    op_suffix = _op_encode_arg2(op_arg1, op_arg2);

                    op       .CopyTo(dest[dest_offset .. ]);
                    op_suffix.CopyTo(dest[(dest_offset + op.Length) .. ]);

                    src_offset  += 2;
                    dest_offset += (op.Length + op_suffix.Length);
                    break;

                // Multi-byte character expressions

                case 0x04: // {CJK_EXT_0}
                    dest_offset += _decode_cjk(src[src_offset..], dest[dest_offset..], game_lang, game_type);
                    src_offset  += src[src_offset + 1] < 0x30 ? 3 : 2;
                    break;

                case 0x2C: // {CJK_EXT_1}
                case 0x2D: // {CJK_EXT_2}
                case 0x2E: // {CJK_EXT_3}
                case 0x2F: // {CJK_EXT_4}
                    dest_offset += game_lang is FhLangId.Japanese or FhLangId.Korean or FhLangId.Chinese
                        ? _decode_cjk(src[src_offset..], dest[dest_offset..], game_lang, game_type)
                        : _decode_us (src[src_offset..], dest[dest_offset..], game_lang, game_type);

                    src_offset += 2;
                    break;

                // Unknown-unhandled ops embedded literally

                case 0x02:
                case 0x05:
                    op        = _select_op_literal(src[src_offset]);
                    op_arg1   = src[src_offset + 1];
                    op_suffix = Encoding.UTF8.GetBytes($"{op_arg1:X2}}}");

                    op       .CopyTo(dest[dest_offset .. ]);
                    op_suffix.CopyTo(dest[(dest_offset + op.Length) .. ]);

                    src_offset  += 1;
                    dest_offset += (op.Length + op_suffix.Length);
                    break;

                case 0x06:
                case 0x08:
                case 0x0C:
                case 0x0D:
                case 0x0E:
                case 0x0F:
                case 0x11:
                case 0x12:
                case 0x24:
                case 0x25:
                case 0x26:
                case 0x27:
                case 0x28:
                case 0x29:
                case 0x2A:
                case 0x2B:
                    op        = _select_op_literal(src[src_offset]);
                    op_arg1   = src[src_offset];
                    op_arg2   = src[src_offset + 1];
                    op_suffix = _op_encode_arg2(op_arg1, op_arg2);

                    op       .CopyTo(dest[dest_offset .. ]);
                    op_suffix.CopyTo(dest[(dest_offset + op.Length) .. ]);

                    src_offset  += 2;
                    dest_offset += (op.Length + op_suffix.Length);
                    break;

                default:
                    dest_offset += game_lang is FhLangId.Japanese or FhLangId.Korean or FhLangId.Chinese
                        ? _decode_cjk(src[src_offset..], dest[dest_offset..], game_lang, game_type)
                        : _decode_us (src[src_offset..], dest[dest_offset..], game_lang, game_type);

                    src_offset += 1;
                    break;
            }
        }

        return dest_offset;
    }
}
