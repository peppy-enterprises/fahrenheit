using System;
using System.Text;

namespace Fahrenheit.CoreLib;

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

public abstract unsafe partial class FhCharset {
    public const char InvalidChar = char.MaxValue;
    public const byte InvalidByte = byte.MaxValue;

    public void to_bytes(in ReadOnlySpan<char> src, in Span<byte> dest) {
        for (int i = 0; i < src.Length; i++) {
            dest[i] = to_byte(src[i]);
        }
    }

    public void to_chars(in ReadOnlySpan<byte> src, in Span<char> dest) {
        for (int i = 0; i < src.Length; i++) {
            dest[i] = to_char(src[i]);
        }
    }

    public byte[] to_bytes(in ReadOnlySpan<char> src) {
        byte[] dest = new byte[src.Length];
        to_bytes(src, dest);
        return dest;
    }

    public char[] to_chars(in ReadOnlySpan<byte> src) {
        char[] dest = new char[src.Length];
        to_chars(src, dest);
        return dest;
    }

    public string to_string(in ReadOnlySpan<byte> src) {
        StringBuilder result = new StringBuilder();

        for (int i = 0; i < src.Length; i++) {
            result.Append(to_char(src[i]));
        }

        return result.ToString();
    }

    public string to_string(byte* src) {
        StringBuilder result = new StringBuilder();

        for (byte b = *src; b != 0x00; b = *++src) {
            result.Append(to_char(b));
        }

        return result.ToString();
    }

    public abstract char to_char(byte b);
    public abstract byte to_byte(char c);
}
