// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core;

public unsafe static class FhUtil {
    public static T* ptr_at<T>(nint address)          where T : unmanaged { return (T*)(FhGlobal.base_addr + address); }
    public static T  get_at<T>(nint address)          where T : unmanaged { return *ptr_at<T>(address);                }
    public static T  set_at<T>(nint address, T value) where T : unmanaged { return *ptr_at<T>(address) = value;        }

    public static FhLangId get_game_lang() {
        string ini_path     = FhInternal.PathFinder.get_path_settings();
        string ini_lang_key = "Language=";
        // linebreaks can be inconsistent depending on whether INI edited by hand or by launcher
        char[] line_breaks  = [ '\r', '\n' ];

        try {
            using (FileStream   ini_stream = File.OpenRead(ini_path))
            using (StreamReader ini_reader = new StreamReader(ini_stream)) {
                string ini = ini_reader.ReadToEnd();

                int ini_lang_s = ini.IndexOf(ini_lang_key) + ini_lang_key.Length;
                int ini_lang_e = ini[ ini_lang_s .. ].IndexOfAny(line_breaks) + ini_lang_s;

                return ini[ ini_lang_s .. ini_lang_e ] switch {
                    "en"         => FhLangId.English,
                    "ch" or "cn" => FhLangId.Chinese,
                    "jp"         => FhLangId.Japanese,
                    "kr"         => FhLangId.Korean,
                    "fr"         => FhLangId.French,
                    "es"         => FhLangId.Spanish,
                    "it"         => FhLangId.Italian,
                    "de"         => FhLangId.German,
                    _            => FhLangId.Japanese, // mirror game default behavior
                };
            }
        }
        catch (Exception e) {
            FhInternal.Log.Error($"faulted while probing game language: {e}");
            FhInternal.Log.Error("falling back to JP to mirror game default");

            return FhLangId.Japanese;
        }
    }

    public static FhGameType get_game_type() {
        FhGameType rv = FhGameType.NULL;

        if (FhPInvoke.GetModuleHandle("FFX.exe")   != 0) rv = FhGameType.FFX;
        if (FhPInvoke.GetModuleHandle("FFX-2.exe") != 0) rv = FhGameType.FFX2;

        return rv;
    }

    public static void cast_to_bytes<T>(in ReadOnlySpan<T> src, in Span<byte> dest, out int bytesWritten) where T : struct {
        bytesWritten = Unsafe.SizeOf<T>() * src.Length;
        MemoryMarshal.AsBytes(src).CopyTo(dest);
    }

    public static void cast_from_bytes<T>(in ReadOnlySpan<byte> src, in Span<T> dest, int srcLen, out int count) where T : struct {
        count = srcLen / Unsafe.SizeOf<T>();
        MemoryMarshal.Cast<byte, T>(src).CopyTo(dest);
    }

    /// <summary>
    ///     Generic bitwise NOT.
    /// </summary>
    public static T not_with_cast<T>(this T x) where T : IBinaryInteger<T> {
        unchecked { return ~x; }
    }

    /// <param name="offset">A 0-based offset of the bit into the <paramref name="bitfield"/>.</param>
    /// <returns>
    ///     Whether bit at <paramref name="offset"/> into <paramref name="bitfield"/> is set.
    /// </returns>
    public unsafe static bool get_bit<T>(this T bitfield, int offset) where T : unmanaged, IBinaryInteger<T> {
        if (offset < 0 || offset >= sizeof(T) * 8) throw new ArgumentOutOfRangeException(nameof(offset));

        return ((bitfield >> offset) & T.One) != T.Zero;
    }

    /// <param name="offset">A 0-based offset into the <paramref name="bitfield"/>.</param>
    /// <param name="len">The amount of bits to read.</param>
    /// <returns>
    ///     The value of type <typeparamref name="T"/> made from <paramref name="len"/> bits <paramref name="offset"/> into <paramref name="bitfield"/>.
    /// </returns>
    public unsafe static T get_bits<T>(this T bitfield, int offset, int len) where T : unmanaged, IBinaryInteger<T> {
        if (offset <  0 || offset >=  sizeof(T) * 8)           throw new ArgumentOutOfRangeException(nameof(offset));
        if (len    <= 0 || len    >  (sizeof(T) * 8) - offset) throw new ArgumentOutOfRangeException(nameof(len));

        return (bitfield >> offset) & ((T.One << len) - T.One);
    }

    /// <summary>
    ///     Sets bit at <paramref name="offset"/> into <paramref name="bitfield"/> based on <paramref name="value"/>.
    /// </summary>
    /// <param name="offset">A 0-based offset into the <paramref name="bitfield"/>.</param>
    /// <param name="value">Whether the bit should be set to <c>1</c> if <c>true</c> or <c>0</c> if <c>false</c>.</param>
    public unsafe static void set_bit<T>(this ref T bitfield, int offset, bool value) where T : unmanaged, IBinaryInteger<T> {
        if (offset < 0 || offset >= sizeof(T) * 8) throw new ArgumentOutOfRangeException(nameof(offset));

        if (value) bitfield |=  T.One << offset;
        else       bitfield &= (T.One << offset).not_with_cast();
    }

    /// <summary>
    ///     Sets bit at <paramref name="offset"/> into <paramref name="bitfield"/> to <paramref name="value"/>.
    /// </summary>
    /// <param name="offset">A 0-based offset into the <paramref name="bitfield"/>.</param>
    /// <param name="len">The amount of bits to write.</param>
    /// <param name="value">The value to set the bits to. Only the first <paramref name="len"/> bits matter.</param>
    public unsafe static void set_bits<T>(this ref T bitfield, int offset, int len, T value) where T : unmanaged, IBinaryInteger<T> {
        if (offset < 0  || offset >=  sizeof(T) * 8)           throw new ArgumentOutOfRangeException(nameof(offset));
        if (len    <= 0 || len    >  (sizeof(T) * 8) - offset) throw new ArgumentOutOfRangeException(nameof(len));

        for (; len > 0; len--, offset++) { bitfield.set_bit(offset, value.get_bit(offset)); }
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
    public static ulong pack_bytes_be(this ReadOnlySpan<byte> bytes) {
        ulong be = 0;

        for (int i = bytes.Length - 1, j = 0; i >= 0; i--, j++)
            be += (ulong)bytes[i] << (8 * j);

        return be;
    }

    public static string get_timestamp_string() {
        DateTime dt = DateTime.UtcNow;
        return $"{dt.Year:D2}{dt.Month:D2}{dt.Day:D2}_{dt.Hour:D2}{dt.Minute:D2}{dt.Second:D2}";
    }

    public static string get_extended_timestamp_string() {
        DateTime dt = DateTime.UtcNow;
        return $"{dt.Year:D2}{dt.Month:D2}{dt.Day:D2}_{dt.Hour:D2}{dt.Minute:D2}{dt.Second:D2}.{dt.Millisecond:D3}";
    }

    internal static JsonSerializerOptions InternalJsonOpts { get; } = new JsonSerializerOptions {
        Converters = {
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

    public static TDelegate get_fptr<TDelegate>(nint funcAddress) {
        return Marshal.GetDelegateForFunctionPointer<TDelegate>(FhGlobal.base_addr + funcAddress);
    }

    public static Vector2 game_remap_720p(this Vector2 vec) {
        return new Vector2 {
            X = vec.X * 512 / 1280,
            Y = vec.Y * 416 / 720,
        };
    }

    public static Vector2 game_remap_1080p(this Vector2 vec) {
        return new Vector2 {
            X = vec.X * 512 / 1920,
            Y = vec.Y * 416 / 1080,
        };
    }

    public static uint as_rgba(this Vector4 vec) {
        return (uint)(
            ((byte)(vec.X * 255f) << 0)
          & ((byte)(vec.Y * 255f) << 8)
          & ((byte)(vec.Z * 255f) << 16)
          & ((byte)(vec.W * 255f) << 24)
        );
    }
}
