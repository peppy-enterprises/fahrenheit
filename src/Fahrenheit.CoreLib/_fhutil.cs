using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fahrenheit.CoreLib;

public static unsafe class FhUtil {
    public static T* ptr_at<T>(nint address)          where T : unmanaged { return (T*)(FhGlobal.base_addr + address); }
    public static T  get_at<T>(nint address)          where T : unmanaged { return *ptr_at<T>(address);                }
    public static T  set_at<T>(nint address, T value) where T : unmanaged { return *ptr_at<T>(address) = value;        }

    public static nint get_mbase_or_throw() {
        nint mbase;
        if ((mbase = FhPInvoke.GetModuleHandle("FFX.exe")) == nint.Zero) {
            if ((mbase = FhPInvoke.GetModuleHandle("FFX-2.exe")) == nint.Zero)
                throw new Exception("FH_E_HOOK_TARGET_INDETERMINATE");
        }
        return mbase;
    }

    /// <summary>
    ///     Generic bitwise NOT.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T not_with_cast<T>(this T x) where T : IBinaryInteger<T> {
        unchecked { return ~x; }
    }

    /// <param name="offset">A 0-based offset of the bit into the <paramref name="bitField"/>.</param>
    /// <returns>
    ///     Whether bit at <paramref name="offset"/> into <paramref name="bitField"/> is set.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool get_bit<T>(this T bitField, int offset) where T : unmanaged, IBinaryInteger<T> {
        if (offset < 0 || offset >= sizeof(T) * 8) throw new ArgumentOutOfRangeException(nameof(offset));

        return ((bitField >> offset) & T.One) != T.Zero;
    }

    /// <param name="offset">A 0-based offset into the <paramref name="bitField"/>.</param>
    /// <param name="len">The amount of bits to read.</param>
    /// <returns>
    ///     The value of type <typeparamref name="T"/> made from <paramref name="len"/> bits <paramref name="offset"/> into <paramref name="bitField"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe T get_bits<T>(this T bitField, int offset, int len) where T : unmanaged, IBinaryInteger<T> {
        if (offset <  0 || offset >=  sizeof(T) * 8)           throw new ArgumentOutOfRangeException(nameof(offset));
        if (len    <= 0 || len    >  (sizeof(T) * 8) - offset) throw new ArgumentOutOfRangeException(nameof(len));

        return (bitField >> offset) & ((T.One << len) - T.One);
    }

    /// <summary>
    ///     Sets bit at <paramref name="offset"/> into <paramref name="bitField"/> based on <paramref name="value"/>.
    /// </summary>
    /// <param name="offset">A 0-based offset into the <paramref name="bitField"/>.</param>
    /// <param name="value">Whether the bit should be set to <c>1</c> if <c>true</c> or <c>0</c> if <c>false</c>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void set_bit<T>(this ref T bitField, int offset, bool value) where T : unmanaged, IBinaryInteger<T> {
        if (offset < 0 || offset >= sizeof(T) * 8) throw new ArgumentOutOfRangeException(nameof(offset));

        if (value) bitField |=  T.One << offset;
        else       bitField &= (T.One << offset).not_with_cast();
    }

    /// <summary>
    ///     Sets bit at <paramref name="offset"/> into <paramref name="bitField"/> to <paramref name="value"/>.
    /// </summary>
    /// <param name="offset">A 0-based offset into the <paramref name="bitField"/>.</param>
    /// <param name="len">The amount of bits to write.</param>
    /// <param name="value">The value to set the bits to. Only the first <paramref name="len"/> bits matter.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void set_bits<T>(this ref T bitField, int offset, int len, T value) where T : unmanaged, IBinaryInteger<T> {
        if (offset < 0  || offset >=  sizeof(T) * 8)           throw new ArgumentOutOfRangeException(nameof(offset));
        if (len    <= 0 || len    >  (sizeof(T) * 8) - offset) throw new ArgumentOutOfRangeException(nameof(len));

        for (; len > 0; len--, offset++) { bitField.set_bit(offset, value.get_bit(offset)); }
    }

    public static ulong u64_swap_endian(ulong x) {
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
    public static ulong pack_bytes_le(this ReadOnlySpan<byte> bytes) {
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
    public static ulong pack_bytes_be(this ReadOnlySpan<byte> bytes) {
        ulong be = 0;

        for (int i = bytes.Length - 1, j = 0; i >= 0; i--, j++)
            be += (ulong)bytes[i] << (8 * j);

        return be;
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
    public static string get_timestamp_string() {
        DateTime dt = DateTime.UtcNow;
        return $"{dt.Day.ToString("D2")}{dt.Month.ToString("D2")}{dt.Year.ToString("D2")}_{dt.Hour.ToString("D2")}{dt.Minute.ToString("D2")}{dt.Second.ToString("D2")}";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string get_extended_timestamp_string() {
        DateTime dt = DateTime.UtcNow;
        return $"{dt.Day.ToString("D2")}{dt.Month.ToString("D2")}{dt.Year.ToString("D2")}_{dt.Hour.ToString("D2")}{dt.Minute.ToString("D2")}{dt.Second.ToString("D2")}.{dt.Millisecond.ToString("D3")}";
    }

    internal static JsonSerializerOptions InternalJsonOpts { get; } = new JsonSerializerOptions {
        Converters = {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        },
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
    };

    public static JsonSerializerOptions JsonOpts { get; } = new JsonSerializerOptions {
        Converters = {
            new FhConfigParser<FhModuleConfig>(),
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        },
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
    };

    internal static void enter_json_object(this ref Utf8JsonReader reader) {
        while (reader.TokenType != JsonTokenType.StartObject) {
            reader.Read();
        }
        reader.Read();
    }

    internal static void enter_json_array(this ref Utf8JsonReader reader) {
        while (reader.TokenType != JsonTokenType.StartArray) {
            reader.Read();
        }
        reader.Read();
    }

    internal static T deserialize_and_advance<T>(this ref Utf8JsonReader reader, string key) {
        if (reader.GetString() != key)
            throw new JsonException($"Expected {key}, got {reader.GetString()}.");

        reader.Read();
        T tval = JsonSerializer.Deserialize<T>(ref reader, JsonOpts) ?? throw new JsonException();
        reader.Read();

        return tval;
    }
}

// TODO: unfuck this garbage
public ref struct FhTokenizer {
    private readonly ReadOnlySpan<char> _span;
    private readonly int                _spanLength;
    private readonly ReadOnlySpan<char> _delimiters;
    private          int                _currentPosition;

    public FhTokenizer(ReadOnlySpan<char> span,
                       ReadOnlySpan<char> delimiters,
                       int                startPos = 0) {
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
    public ReadOnlySpan<char> GetNextTokenBetween(char startToken, char endToken) {
        if (_currentPosition == _span.Length) // We are at the end of the span. Nothing to read.
            return ReadOnlySpan<char>.Empty;

        int methodStartPos = _currentPosition;
        int startTokenPos  = _span[_currentPosition.._spanLength].IndexOf(startToken) + _currentPosition;
        int endTokenPos    = _span[startTokenPos.._spanLength].IndexOf(endToken) + startTokenPos;

        while (startTokenPos == methodStartPos) {
            _currentPosition++;
            methodStartPos++;
            startTokenPos = _span[_currentPosition.._spanLength].IndexOf(startToken) + _currentPosition;
        }

        if (endTokenPos == methodStartPos - 1) { // IndexOf(endToken) returned -1; there is no endToken in the span.
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
    public ReadOnlySpan<char> GetNextTokenUpTo(char endToken) {
        if (_currentPosition == _spanLength) // We are at the end of the span. Nothing to read.
            return ReadOnlySpan<char>.Empty;

        int startPos = _currentPosition;
        int endPos = _span[_currentPosition.._spanLength].IndexOf(endToken) + _currentPosition;

        while (endPos == startPos) {
            _currentPosition++;
            startPos++;
            endPos = _span[_currentPosition.._spanLength].IndexOf(endToken) + _currentPosition;
        }

        if (endPos == startPos - 1) {
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
    public ReadOnlySpan<char> GetNextToken() {
        if (_currentPosition == _span.Length) // We are at the end of the span. Nothing to read.
            return ReadOnlySpan<char>.Empty;

        int sliceStart = _currentPosition;
        int sliceEnd   = _span[_currentPosition.._spanLength].IndexOfAny(_delimiters) + _currentPosition;

        while (sliceEnd == sliceStart) {
            _currentPosition++;
            sliceStart++;
            sliceEnd = _span[_currentPosition.._spanLength].IndexOfAny(_delimiters) + _currentPosition;
        }

        if (sliceEnd == sliceStart - 1) {
            _currentPosition = _spanLength;
            return _span[sliceStart.._spanLength];
        }

        _currentPosition = sliceEnd + 1;
        return _span[sliceStart..sliceEnd];
    }

    public void SkipToken() {
        ReadOnlySpan<char> _ = GetNextToken();
    }
}