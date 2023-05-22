using System;

namespace Fahrenheit.CoreLib;

public enum FhCharsetId
{
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

public abstract partial class FhCharset
{
    public const char InvalidChar = char.MaxValue;
    public const byte InvalidByte = byte.MaxValue;

    public bool ToBytes(in ReadOnlySpan<char> src, in Span<byte> dest)
    {
        for (int i = 0; i < src.Length; i++)
        {
            byte b;
            if ((b = ToByte(src[i])) != InvalidByte)
                dest[i] = b;
        }

        return true;
    }

    public bool ToChars(in ReadOnlySpan<byte> src, in Span<char> dest)
    {
        for (int i = 0; i < src.Length; i++)
        {
            char c;
            if ((c = ToChar(src[i])) != InvalidChar)
                dest[i] = c;
        }

        return true;
    }

    public abstract char ToChar(byte b);
    public abstract byte ToByte(char c);
}
