using System.Text;

namespace Fahrenheit.Core.FFX;

/// <summary>
///     A representation of an offset to text commonly used in Excel files.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 0x4)]
public struct ExcelTextOffset {
    /// <summary>
    ///     The offset to the text.
    /// </summary>
    public  ushort text_offset;
    private ushort __0x2; // Clearly related to the text, but unknown
}

/// <summary>
///     A representation of offsets to text commonly used in Excel files.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size=0x8)]
public struct ExcelSimplifiableTextOffset {
    /// <summary>
    ///     The offset to the standard text.
    /// </summary>
    public ExcelTextOffset standard;

    /// <summary>
    ///     The offset to the simplified text.<br/>
    ///     In Japanese, this would have been the hiragana version of the text.<br/>
    ///     This is completely unused.
    /// </summary>
    internal ExcelTextOffset simplified;
}

/// <summary>
///     The header of Excel files.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct ExcelHeader {
    /// <summary>
    ///     The index of the first element in the file.
    /// </summary>
    public ushort first_idx;

    /// <summary>
    ///     The index of the last element in the file.
    /// </summary>
    public ushort last_idx;

    /// <summary>
    ///     The size of one element in the file.
    /// </summary>
    public ushort element_size;

    /// <summary>
    ///     The length of all the elements in the file.<br/>
    ///     Beyond the data there may be text, referenced using offsets in the elements.
    /// </summary>
    public ushort data_length;

    /// <summary>
    ///     An offset from the start of the file to the start of the data.<br/>
    ///     In vanilla, always equivalent to the size of this header.
    /// </summary>
    public nint data_start;

    /// <summary>
    ///     The length of the array of elements defined by this header.
    /// </summary>
    public readonly int length => last_idx + 1 - first_idx;
}

/// <summary>
///     An excel file. Stores an array of objects.<br/>
///     These files make up most of the game's <c>kernel</c> folder.
/// </summary>
/// <typeparam name="T">The type of elements in the file.</typeparam>
[StructLayout(LayoutKind.Explicit, Size=0x14)]
internal unsafe struct ExcelFile<T> where T : unmanaged {
    /// <summary>
    ///     The amount of headers that map out this file.
    ///     <remarks>
    ///         In the games, this amount is always 1.
    ///         Both the games and Fahrenheit support amounts higher than 1.
    ///     </remarks>
    /// </summary>
    [FieldOffset(0x0)] public ushort header_count;

    // Private fields for creating spans through MemoryMarshal.CreateSpan
    [FieldOffset(0x0)] private T _dummy_element;
    [FieldOffset(0x8)] private ExcelHeader _first_header;

    /// <summary>
    ///     The headers in this file. Each header defines a section of the file.
    /// </summary>
    [UnscopedRef]
    public Span<ExcelHeader> headers
        => MemoryMarshal.CreateSpan(ref _first_header, header_count);


    /// <summary>
    ///     The elements indicated by a given header.
    /// </summary>
    [UnscopedRef]
    public Span<T> elements(ExcelHeader header)
        => MemoryMarshal.CreateSpan(ref Unsafe.AddByteOffset(ref _dummy_element, header.data_start), header.length);

    /// <summary>
    ///     Finds a header that contains the element at a given index.
    /// </summary>
    /// <param name="index">The index of the desired element.</param>
    /// <returns>The header containing the element at the given index, or <c>null</c> if not found.</returns>
    public ExcelHeader? find_header_for_index(int index) {
        foreach (ExcelHeader header in headers) {
            if (header.first_idx <= index && index <= header.last_idx) {
                return header;
            }
        }

        return null;
    }

    /// <summary>
    ///     Gets the element at the specified index, with boundary checking.
    /// </summary>
    /// <param name="index">The index of the desired element. Must be withing the bounds specified by one of the headers.</param>
    /// <returns>The element at the specified index.</returns>
    /// <exception cref="IndexOutOfRangeException">
    ///     index is less than <see cref="ExcelHeader.first_idx"><c>header.first_idx</c></see>
    ///     or greater than <see cref="ExcelHeader.last_idx"><c>header.last_idx</c></see>
    ///     for all <see cref="headers"/>.
    /// </exception>
    [UnscopedRef]
    public ref T get(int index) {
        ExcelHeader header = find_header_for_index(index)
            ?? throw new IndexOutOfRangeException($"Cannot access element {index} of excel file. No header contains this index.");

        return ref elements(header)[index - header.first_idx];
    }

    /// <summary>
    ///     Gets the text at the specified offset from a header containing the given index.
    ///     <remarks>
    ///         This method performs no bound checking on the <paramref name="offset"/>
    ///         and <i>may</i> read garbage data if it is not valid.
    ///     </remarks>
    /// </summary>
    /// <param name="index">The index of the element containing the offset.</param>
    /// <param name="offset">The offset of the text.</param>
    /// <returns>The decoded text from the specified offset.</returns>
    /// <exception cref="IndexOutOfRangeException">
    ///     index is less than <see cref="ExcelHeader.first_idx"><c>header.first_idx</c></see>
    ///     or greater than <see cref="ExcelHeader.last_idx"><c>header.last_idx</c></see>
    ///     for all <see cref="headers"/>.
    /// </exception>
    internal string get_text(int index, nint offset) {
        ExcelHeader header = find_header_for_index(index)
            ?? throw new IndexOutOfRangeException($"Cannot find header for index {index} in excel file.");

        return get_text(header, offset);
    }

    /// <summary>
    ///     Gets the text at the specified offset from the given header.
    ///     <remarks>
    ///         This method performs no bound checking on the <paramref name="offset"/>
    ///         and <i>may</i> read garbage data if it is not valid.
    ///     </remarks>
    /// </summary>
    /// <param name="header">The header containing the text.</param>
    /// <param name="offset">The offset of the text.</param>
    /// <returns>The decoded text from the specified offset.</returns>
    internal string get_text(ExcelHeader header, nint offset) {
        ReadOnlySpan<byte> encoded_text;

        fixed (ExcelFile<T>* this_ptr = &this) {
            encoded_text = new((byte*)((nint)this_ptr + header.data_start + header.data_length + offset), int.MaxValue);
        }

        int len = FhEncoding.compute_decode_buffer_size(encoded_text);
        byte[] out_text = new byte[len];
        FhEncoding.decode(encoded_text, out_text);
        return Encoding.UTF8.GetString(out_text);
    }
}
