using System;

namespace Fahrenheit.CoreLib;

public enum FhCharsetId {
    INVALID = 0,
    JP      = 1,
    CH      = 2,
    CN      = 3,
    KR      = 4,
    US      = 5,
    NEW_JP  = 6,
    NEW_CH  = 7,
    NEW_CN  = 8,
    NEW_KR  = 9,
    NEW_US  = 10,
    NEW_DE  = 11,
    NEW_FR  = 12,
    NEW_IT  = 13,
    NEW_SP  = 14
}

public abstract partial class FhCharset {
    public const char InvalidChar = char.MaxValue;
    public const byte InvalidByte = byte.MaxValue;

    public bool to_bytes(in ReadOnlySpan<char> src, in Span<byte> dest) {
        for (int i = 0; i < src.Length; i++) {
            byte b;
            if ((b = to_byte(src[i])) != InvalidByte)
                dest[i] = b;
        }

        return true;
    }

    public bool to_chars(in ReadOnlySpan<byte> src, in Span<char> dest) {
        for (int i = 0; i < src.Length; i++) {
            char c;
            if ((c = to_char(src[i])) != InvalidChar)
                dest[i] = c;
        }

        return true;
    }

    public byte[] to_bytes(in ReadOnlySpan<char> src) {
        byte[] bytes = new byte[src.Length];

        for (int i = 0; i < src.Length; i++) {
            byte b;
            if ((b = to_byte(src[i])) != InvalidByte)
                bytes[i] = b;
        }

        return bytes;
    }

    public string to_string(in ReadOnlySpan<byte> src) {
        string result = "";

        for (int i = 0; i < src.Length; i++) {
            char c;
            if ((c = to_char(src[i])) != InvalidChar)
                result += c;
        }

        return result;
    }

    public unsafe string to_string(byte* src) {
        string result = "";

        for (byte b = *src; b != 0x00; b = *++src) {
            result += to_char(b);
        }

        return result;
    }

    public abstract char to_char(byte b);
    public abstract byte to_byte(char c);
}
