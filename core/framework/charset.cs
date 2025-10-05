// SPDX-License-Identifier: MIT

using System.Text;

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

public abstract unsafe partial class FhCharset {
    public void to_bytes(in ReadOnlySpan<char> src, in Span<byte> dest) {
        throw new NotImplementedException(); // TODO: handle 2b encoding properly
    }

    public void to_bytes(in ReadOnlySpan<char> src, byte* dest) {
        throw new NotImplementedException(); // TODO: handle 2b encoding properly
    }

    public void to_chars(in ReadOnlySpan<byte> src, in Span<char> dest) {
        for (int i = 0; i < src.Length; i++) {
            dest[i] = decode(src[i]);
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
            result.Append(decode(src[i]));
        }

        return result.ToString();
    }

    public string to_string(byte* src) {
        StringBuilder result = new StringBuilder();

        for (byte b = *src; b != 0x00; b = *++src) {
            result.Append(decode(b));
        }

        return result.ToString();
    }

    public abstract char   decode(ushort b);
    public abstract ushort encode(char   c);
}
