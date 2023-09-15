using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fahrenheit.CoreLib;

public static class FhUtil
{
    /// <summary>
    ///     Reads bytes as primitives- unsigned, little-endian, variant length (multiple of 8, power of 2, &lt;= 64).
    ///     <para></para>
    ///     Returns an <see cref="ulong"/>. Downcast it to your desired return type.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong BPrimReadULE(this ReadOnlySpan<byte> bytes, int start, int len)
    {
        return len switch
        {
            8  => bytes[start / 8],
            16 => BinaryPrimitives.ReadUInt16LittleEndian(bytes[(start / 8)..]),
            32 => BinaryPrimitives.ReadUInt32LittleEndian(bytes[(start / 8)..]),
            64 => BinaryPrimitives.ReadUInt64LittleEndian(bytes[(start / 8)..]),
            _  => 0
        };
    }

    /// <summary>
    ///     Reads bytes as primitives- signed, little-endian, variant length (multiple of 8, power of 2, &lt;= 64).
    ///     <para></para>
    ///     Returns an <see cref="ulong"/>. Downcast it to your desired return type.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long BPrimReadSLE(this ReadOnlySpan<byte> bytes, int start, int len)
    {
        return len switch
        {
            8  => bytes[start / 8],
            16 => BinaryPrimitives.ReadInt16LittleEndian(bytes[(start / 8)..]),
            32 => BinaryPrimitives.ReadInt32LittleEndian(bytes[(start / 8)..]),
            64 => BinaryPrimitives.ReadInt64LittleEndian(bytes[(start / 8)..]),
            _  => 0
        };
    }

    /// <summary>
    ///     Reads bytes as primitives- unsigned, big-endian, variant length (multiple of 8, power of 2, &lt;= 64).
    ///     <para></para>
    ///     Returns a <see cref="long"/>. Downcast it to your desired return type.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong BPrimReadUBE(this ReadOnlySpan<byte> bytes, int start, int len)
    {
        return len switch
        {
            8  => bytes[start / 8],
            16 => BinaryPrimitives.ReadUInt16BigEndian(bytes[(start / 8)..]),
            32 => BinaryPrimitives.ReadUInt32BigEndian(bytes[(start / 8)..]),
            64 => BinaryPrimitives.ReadUInt64BigEndian(bytes[(start / 8)..]),
            _  => 0
        };
    }

    /// <summary>
    ///     Reads bytes as primitives- signed, big-endian, variant length (multiple of 8, power of 2, &lt;= 64).
    ///     <para></para>
    ///     Returns a <see cref="long"/>. Downcast it to your desired return type.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long BPrimReadSBE(this ReadOnlySpan<byte> bytes, int start, int len)
    {
        return len switch
        {
            8  => bytes[start / 8],
            16 => BinaryPrimitives.ReadInt16BigEndian(bytes[(start / 8)..]),
            32 => BinaryPrimitives.ReadInt32BigEndian(bytes[(start / 8)..]),
            64 => BinaryPrimitives.ReadInt64BigEndian(bytes[(start / 8)..]),
            _  => 0
        };
    }

    public static ulong U64SwapEndian(ulong x)
    {
        // swap adjacent 32-bit blocks
        x = (x >> 32) | (x << 32);
        // swap adjacent 16-bit blocks
        x = ((x & 0xFFFF0000FFFF0000) >> 16) | ((x & 0x0000FFFF0000FFFF) << 16);
        // swap adjacent 8-bit blocks
        return ((x & 0xFF00FF00FF00FF00) >> 8) | ((x & 0x00FF00FF00FF00FF) << 8);
    }

    /// <summary>
    ///     Packs a byte span into an <see cref="ulong"/>, little-endian. 
    ///     Undefined on inputs over 8 bytes in length.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong PackBytesLE(this ReadOnlySpan<byte> bytes)
    {
        ulong le = 0;

        for (int i = bytes.Length - 1; i >= 0; i--)
            le += (ulong)bytes[i] << (8 * i);

        return le;
    }

    /// <summary>
    ///     Packs a byte span into an <see cref="ulong"/>, big-endian. 
    ///     Undefined on inputs over 8 bytes in length.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong PackBytesBE(this ReadOnlySpan<byte> bytes)
    {
        ulong be = 0;

        for (int i = bytes.Length - 1, j = 0; i >= 0; i--, j++)
            be += (ulong)bytes[i] << (8 * j);

        return be;
    }

    /* [fkelava 13/8/22 11:19]
     * These methods provide reasonably fast bitfield slicing. They are not _optimal_, but are dozens of times faster than BitArray.
     * If a bitfield slice is requested on byte boundaries, the operation is fast-cased to a simple read and performs slightly worse
     * than a direct BitConverter.To{...} call.
     * 
|               Method |       N |        Mean |     Error |    StdDev |
|--------------------- |-------- |------------:|----------:|----------:|
|      IOT_BExtrUInt16 | 1000000 |    723.2 us |   1.81 us |   1.61 us |
|    BConv_BExtrUInt16 | 1000000 |    531.7 us |   1.24 us |   1.10 us |
     * 
     * Impl. note: the `if ($INTRINTYPE.IsSupported)` check is treated as a JIT runtime constant, so the instruction set if costs nothing.
     * 
     * An ARM intrinsic to wrap the ubfx/sbfx instruction does not exist in C#, and would be very useful here.
     */

    /* [fkelava 5/4/23 19:54] 
     * It turns out .NET codegen for `_bextr_u64` is suboptimal (https://github.com/dotnet/runtime/issues/13164), benchmarks show this is still unsolved.
     * The shifts done below are just as fast and still correct so BMI1 x64 intrinsic path has been removed.
     */

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte BExtr_U8LE(this ReadOnlySpan<byte> bytes, int start, int len)
    {
        int sb = start % 8;

        return sb == 0 && (len % 8) == 0
            ? bytes[start / 8]
            : (byte)((bytes.PackBytesLE() >> start) & ((1UL << len) - 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte BExtr_U8BE(this ReadOnlySpan<byte> bytes, int start, int len)
    {
        int sb = start % 8;

        return sb == 0 && (len % 8) == 0
            ? bytes[start / 8]
            : (byte)((bytes.PackBytesBE() >> start) & ((1UL << len) - 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte UBExtr_U8(this ulong bytesPacked, int start, int len)
    {
        return (byte)((bytesPacked >> start) & ((1UL << len) - 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte BExtr_I8LE(this ReadOnlySpan<byte> bytes, int start, int len)
    {
        int sb = start % 8;

        return sb == 0 && (len % 8) == 0
            ? (sbyte)bytes[start / 8]
            : (sbyte)((bytes.PackBytesLE() >> start) & ((1UL << len) - 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte BExtr_I8BE(this ReadOnlySpan<byte> bytes, int start, int len)
    {
        int sb = start % 8;

        return sb == 0 && (len % 8) == 0
            ? (sbyte)bytes[start / 8]
            : (sbyte)((bytes.PackBytesBE() >> start) & ((1UL << len) - 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte UBExtr_I8(this ulong bytesPacked, int start, int len)
    {
        return (sbyte)((bytesPacked >> start) & ((1UL << len) - 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort BExtr_U16LE(this ReadOnlySpan<byte> bytes, int start, int len)
    {
        int sb = start % 8;

        return sb == 0 && (len % 8) == 0
            ? (ushort)BPrimReadULE(bytes, start, len)
            : (ushort)((bytes.PackBytesLE() >> start) & ((1UL << len) - 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort BExtr_U16BE(this ReadOnlySpan<byte> bytes, int start, int len)
    {
        int sb = start % 8;

        return sb == 0 && (len % 8) == 0
            ? (ushort)BPrimReadUBE(bytes, start, len)
            : (ushort)((bytes.PackBytesBE() >> start) & ((1UL << len) - 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort UBExtr_U16(this ulong bytesPacked, int start, int len)
    {
        return (ushort)((bytesPacked >> start) & ((1UL << len) - 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short BExtr_I16LE(this ReadOnlySpan<byte> bytes, int start, int len)
    {
        int sb = start % 8;

        return sb == 0 && (len % 8) == 0
            ? (short)BPrimReadSLE(bytes, start, len)
            : (short)((bytes.PackBytesLE() >> start) & ((1UL << len) - 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short BExtr_I16BE(this ReadOnlySpan<byte> bytes, int start, int len)
    {
        int sb = start % 8;

        return sb == 0 && (len % 8) == 0
            ? (short)BPrimReadSBE(bytes, start, len)
            : (short)((bytes.PackBytesBE() >> start) & ((1UL << len) - 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short UBExtr_I16(this ulong bytesPacked, int start, int len)
    {
        return (short)((bytesPacked >> start) & ((1UL << len) - 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint BExtr_U32LE(this ReadOnlySpan<byte> bytes, int start, int len)
    {
        int sb = start % 8;

        return sb == 0 && (len % 8) == 0
            ? (uint)BPrimReadULE(bytes, start, len)
            : (uint)((bytes.PackBytesLE() >> start) & ((1UL << len) - 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint BExtr_U32BE(this ReadOnlySpan<byte> bytes, int start, int len)
    {
        int sb = start % 8;

        return sb == 0 && (len % 8) == 0
            ? (uint)BPrimReadUBE(bytes, start, len)
            : (uint)((bytes.PackBytesBE() >> start) & ((1UL << len) - 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint UBExtr_U32(this ulong bytesPacked, int start, int len)
    {
        return (uint)((bytesPacked >> start) & ((1UL << len) - 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BExtr_I32LE(this ReadOnlySpan<byte> bytes, int start, int len)
    {
        int sb = start % 8;

        return sb == 0 && (len % 8) == 0
            ? (int)BPrimReadSLE(bytes, start, len)
            : (int)((bytes.PackBytesLE() >> start) & ((1UL << len) - 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BExtr_I32BE(this ReadOnlySpan<byte> bytes, int start, int len)
    {
        int sb = start % 8;

        return sb == 0 && (len % 8) == 0
            ? (int)BPrimReadSBE(bytes, start, len)
            : (int)((bytes.PackBytesBE() >> start) & ((1UL << len) - 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int UBExtr_I32(this ulong bytesPacked, int start, int len)
    {
        return (int)((bytesPacked >> start) & ((1UL << len) - 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong BExtr_U64LE(this ReadOnlySpan<byte> bytes, int start, int len)
    {
        int sb = start % 8;

        return sb == 0 && (len % 8) == 0
            ? BPrimReadULE(bytes, start, len)
            : (bytes.PackBytesLE() >> start) & ((1UL << len) - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong BExtr_U64BE(this ReadOnlySpan<byte> bytes, int start, int len)
    {
        int sb = start % 8;

        return sb == 0 && (len % 8) == 0
            ? BPrimReadUBE(bytes, start, len)
            : (bytes.PackBytesBE() >> start) & ((1UL << len) - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong UBExtr_U64(this ulong bytesPacked, int start, int len)
    {
        return (bytesPacked >> start) & ((1UL << len) - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long BExtr_I64LE(this ReadOnlySpan<byte> bytes, int start, int len)
    {
        int sb = start % 8;

        return sb == 0 && (len % 8) == 0
            ? bytes.BPrimReadSLE(start, len)
            : (long)((bytes.PackBytesLE() >> start) & ((1UL << len) - 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long BExtr_I64BE(this ReadOnlySpan<byte> bytes, int start, int len)
    {
        int sb = start % 8;

        return sb == 0 && (len % 8) == 0
            ? bytes.BPrimReadSBE(start, len)
            : (long)((bytes.PackBytesBE() >> start) & ((1UL << len) - 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long UBExtr_I64(this ulong bytesPacked, int start, int len)
    {
        return (long)((bytesPacked >> start) & ((1UL << len) - 1));
    }

    /* [fkelava 29/3/23 03:33]
     * IDE0071 wants you to simplify interpolations by removing the ToString() call.
     * 
     * Don't- the simplified syntax is not a zero-cost abstraction, but rather involves boxing and unboxing of value types. An explicit string conversion
     * both eliminates that instance of boxing _and_ allows string.Concat() to be emitted in the actual string construction IL.
     * 
     * https://github.com/dotnet/roslyn/issues/43711. Yes, I know that technically https://github.com/dotnet/roslyn/issues/55240, but it 
     * costs me nothing to include the .ToString() explicitly as opposed to guessing whether the compiler will reduce it.
     */

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetTimestampString()
    {
        DateTime dt = DateTime.UtcNow;
        return $"{dt.Day.ToString("D2")}{dt.Month.ToString("D2")}{dt.Year.ToString("D2")}_{dt.Hour.ToString("D2")}{dt.Minute.ToString("D2")}{dt.Second.ToString("D2")}";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetExtendedTimestampString()
    {
        DateTime dt = DateTime.UtcNow;
        return $"{dt.Day.ToString("D2")}{dt.Month.ToString("D2")}{dt.Year.ToString("D2")}_{dt.Hour.ToString("D2")}{dt.Minute.ToString("D2")}{dt.Second.ToString("D2")}.{dt.Millisecond.ToString("D3")}";
    }

    internal static JsonSerializerOptions InternalJsonOpts { get; } = new JsonSerializerOptions
    {
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        },
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
    };

    public static JsonSerializerOptions JsonOpts { get; } = new JsonSerializerOptions
    {
        Converters =
        {
            new FhConfigParser<FhModuleConfig>(),
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        },
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
    };

    internal static void EnterObject(this ref Utf8JsonReader reader)
    {
        while (reader.TokenType != JsonTokenType.StartObject) reader.Read();
        reader.Read();
    }

    internal static void EnterArray(this ref Utf8JsonReader reader)
    {
        while (reader.TokenType != JsonTokenType.StartArray) reader.Read();
        reader.Read();
    }

    internal static T DeserializeAndAdvance<T>(this ref Utf8JsonReader reader, string key)
    {
        if (reader.GetString() != key)
            throw new JsonException($"Expected {key}, got {reader.GetString()}.");
        
        reader.Read();
        T tval = JsonSerializer.Deserialize<T>(ref reader, JsonOpts) ?? throw new JsonException();
        reader.Read();

        return tval;
    }
}

// TODO: unfuck this garbage
public ref struct FhTokenizer
{
    private readonly ReadOnlySpan<char> _span;
    private readonly int                _spanLength;
    private readonly ReadOnlySpan<char> _delimiters;
    private          int                _currentPosition;

    public FhTokenizer(ReadOnlySpan<char> span,
                       ReadOnlySpan<char> delimiters,
                       int                startPos = 0)
    {
        _span            = span;
        _spanLength      = span.Length;
        _delimiters      = delimiters;
        _currentPosition = startPos;
    }

    /// <summary>
    ///    Retrieves the next slice between an explicit <paramref name="startToken"/> and
    ///    <paramref name="endToken"/>. Useful for when a <typeparamref name="T"/> that you
    ///    already use as a delimiter is in the middle of a span you wish to retrieve in its entirety.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<char> GetNextTokenBetween(char startToken, char endToken)
    {
        if (_currentPosition == _span.Length) // We are at the end of the span. Nothing to read.
            return ReadOnlySpan<char>.Empty;
       
        int methodStartPos = _currentPosition;
        int startTokenPos  = _span[_currentPosition.._spanLength].IndexOf(startToken) + _currentPosition;
        int endTokenPos    = _span[startTokenPos.._spanLength].IndexOf(endToken) + startTokenPos;

        while (startTokenPos == methodStartPos)
        {
            _currentPosition++;
            methodStartPos++;
            startTokenPos = _span[_currentPosition.._spanLength].IndexOf(startToken) + _currentPosition;
        }

        if (endTokenPos == methodStartPos - 1) // IndexOf(endToken) returned -1; there is no endToken in the span.
        {
            _currentPosition = _spanLength;
            return _span[methodStartPos.._spanLength];
        }

        _currentPosition = endTokenPos + 1; // Move past the endToken we consumed.

        return endTokenPos == startTokenPos + 1 // The resulting slice is length zero; the span would be empty.
            ? ReadOnlySpan<char>.Empty
            : _span[startTokenPos..endTokenPos];
    }

    /// <summary>
    ///    Retrieves the next slice up to an explicit <paramref name="endToken"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<char> GetNextTokenUpTo(char endToken)
    {
        if (_currentPosition == _spanLength) // We are at the end of the span. Nothing to read.
            return ReadOnlySpan<char>.Empty;
      
        int startPos = _currentPosition;
        int endPos = _span[_currentPosition.._spanLength].IndexOf(endToken) + _currentPosition;

        while (endPos == startPos)
        {
            _currentPosition++;
            startPos++;
            endPos = _span[_currentPosition.._spanLength].IndexOf(endToken) + _currentPosition;
        }

        if (endPos == startPos - 1)
        {
            _currentPosition = _spanLength;
            return ReadOnlySpan<char>.Empty;
        }

        _currentPosition = endPos + 1;
        return _span[startPos..endPos];
    }

    /// <summary>
    ///    Retrieves the next slice between any two delimiters passed to <see cref="FiTokenizer{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<char> GetNextToken()
    {
        if (_currentPosition == _span.Length) // We are at the end of the span. Nothing to read.
            return ReadOnlySpan<char>.Empty;
        
        int sliceStart = _currentPosition;
        int sliceEnd   = _span[_currentPosition.._spanLength].IndexOfAny(_delimiters) + _currentPosition;

        while (sliceEnd == sliceStart)
        {
            _currentPosition++;
            sliceStart++;
            sliceEnd = _span[_currentPosition.._spanLength].IndexOfAny(_delimiters) + _currentPosition;
        }

        if (sliceEnd == sliceStart - 1)
        {
            _currentPosition = _spanLength;
            return _span[sliceStart.._spanLength];
        }

        _currentPosition = sliceEnd + 1;
        return _span[sliceStart..sliceEnd];
    }

    public void SkipToken()
    {
        ReadOnlySpan<char> _ = GetNextToken();
    }
}

public static class Extensions {
	/// <summary>
	/// Bitwise NOT operator but it is automatically cast back to T
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T bnot_with_cast<T>(this T x)
            where T: System.Numerics.IBinaryInteger<T> {
        unchecked { return (T)~x; }
    }

	/// <param name="bit_offset">A 0-based offset of the bit into the <paramref name="bitfield"/>.</param>
	/// <returns>Whether bit at <paramref name="bit_offset"/> into <paramref name="bitfield"/> is set.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe bool get_bit<T>(this T bitfield, int bit_offset)
			where T : unmanaged, System.Numerics.IBinaryInteger<T> {
		if (bit_offset < 0 || bit_offset >= sizeof(T) * 8) throw new System.ArgumentOutOfRangeException();
		return (bitfield >> bit_offset & T.One) != T.Zero;
	}

	/// <param name="start_offset">A 0-based offset into the <paramref name="bitfield"/>.</param>
	/// <param name="len">The amount of bits to read.</param>
	/// <returns>The value of type <typeparamref name="T"/> made from <paramref name="len"/> bits <paramref name="start_offset"/> into <paramref name="bitfield"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe T get_bits<T>(this T bitfield, int start_offset, int len)
			where T : unmanaged, System.Numerics.IBinaryInteger<T> {
		if (start_offset < 0 || start_offset >= sizeof(T) * 8) throw new System.ArgumentOutOfRangeException();
		if (len <= 0 || len > (sizeof(T) * 8) - start_offset) throw new System.ArgumentOutOfRangeException();
		return (bitfield >> start_offset) & ((T.One << len) - T.One);
	}

	/// <summary>
	/// Sets bit at <paramref name="bit_offset"/> into <paramref name="bitfield"/> based on <paramref name="value"/>.
	/// </summary>
	/// <param name="bit_offset">A 0-based offset into the <paramref name="bitfield"/>.</param>
	/// <param name="value">Whether the bit should be set to <c>1</c> if <c>true</c> or <c>0</c> if <c>false</c>.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void set_bit<T>(this ref T bitfield, int bit_offset, bool value)
            where T: unmanaged, System.Numerics.IBinaryInteger<T> {
		if (bit_offset < 0 || bit_offset >= sizeof(T) * 8) throw new System.ArgumentOutOfRangeException();
        if (value) bitfield |= T.One << bit_offset;
        else bitfield &= (T.One << bit_offset).bnot_with_cast();
    }
	
	/// <summary>
	/// Sets bit at <paramref name="start_offset"/> into <paramref name="bitfield"/> to <paramref name="value"/>.
	/// </summary>
	/// <param name="start_offset">A 0-based offset into the <paramref name="bitfield"/>.</param>
	/// <param name="len">The amount of bits to write.</param>
	/// <param name="value">The value to set the bits to. Only the first <paramref name="len"/> bits matter.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void set_bits<T>(this ref T bitfield, int start_offset, int len, T value)
            where T: unmanaged, System.Numerics.IBinaryInteger<T> {
		if (start_offset < 0 || start_offset >= sizeof(T) * 8) throw new System.ArgumentOutOfRangeException();
		if (len <= 0 || len > (sizeof(T) * 8) - start_offset) throw new System.ArgumentOutOfRangeException();
		for (;len > 0; len--, start_offset++) {
			bitfield.set_bit(start_offset, value.get_bit(start_offset));
		}
    }
}