using System;

namespace Fahrenheit.CoreLib;

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
