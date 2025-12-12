using System.Text;

namespace Fahrenheit.Core.FFX;

/// <summary>
/// The header of Excel files.
/// </summary>
[StructLayout(LayoutKind.Explicit, Size=0xC)]
public struct ExcelHeader {
    /// <summary>
    /// The index of the first element in the file.
    /// </summary>
    [FieldOffset(0x0)] public ushort first_idx;

    /// <summary>
    /// The index of the last element in the file.
    /// </summary>
    [FieldOffset(0x2)] public ushort last_idx;

    /// <summary>
    /// The size of one element in the file.
    /// </summary>
    [FieldOffset(0x4)] public ushort element_size;

    /// <summary>
    /// The length of all the elements in the file.
    /// Beyond the data there may be text, referenced using offsets in the elements.
    /// </summary>
    [FieldOffset(0x6)] public ushort data_length;

    /// <summary>
    /// An offset from the start of the file to the start of the data.
    /// In vanilla, always equivalent to the size of this header.
    /// </summary>
    [FieldOffset(0x8)] public nint data_start;
}

/// <summary>
/// An excel file. These files make up most of the game's <c>kernel</c> folder. Stores an array of objects.
/// </summary>
/// <typeparam name="T">The type of elements in the file.</typeparam>
[StructLayout(LayoutKind.Explicit, Size=0x14)]
public unsafe struct ExcelFile<T> where T : unmanaged {
    /// <summary>
    /// The amount of headers that map out this file.
    ///
    /// <remarks>
    /// In the games, this amount is always 1.
    /// Fahrenheit does not support amounts higher than 1.
    /// </remarks>
    /// </summary>
    [FieldOffset(0x0)] public ushort header_count;

    /// <summary>
    /// The file's header.
    ///
    /// <remarks>
    /// Technically, this is an <c>ExcelHeader</c> <i>array</i>, <see cref="header_count"/> long.
    /// However, if we tried to accomodate this, we would not be able to provide <see cref="elements"/>.
    /// Other ways to provide support for this appear unsafe.
    /// If anyone ever <i>really</i> needs to use multi-headered excel files,
    /// the game is equipped to handle those requests through <c>MsGetExcelData</c>.
    /// </remarks>
    /// </summary>
    [FieldOffset(0x8)] public ExcelHeader header;

    /// <summary>
    /// The length of the array of elements in the file.
    /// </summary>
    public readonly int length => header.last_idx + 1 - header.first_idx;

    [FieldOffset(0x14)] private T _first_element; // Private field used for the MemoryMarshal call below.

    /// <summary>
    /// The elements in this file.
    /// </summary>
    [UnscopedRef]
    public Span<T> elements => MemoryMarshal.CreateSpan(ref _first_element, length);

    /// <summary>
    /// Gets the element at the specified index, with boundary checking.
    /// </summary>
    /// <param name="idx">The index of the desired element. Must be withing the bounds specified by the header.</param>
    /// <returns>The element at the specified index.</returns>
    /// <exception cref="IndexOutOfRangeException">
    /// index is less than <see cref="ExcelHeader.first_idx"><c>header.first_idx</c></see> or greater than <see cref="ExcelHeader.last_idx"><c>header.last_idx</c></see>.
    /// </exception>
    [UnscopedRef]
    public ref T get(int idx) {
        if (idx < header.first_idx || idx > header.last_idx)
            throw new IndexOutOfRangeException($"Cannot access element {idx} of excel file. Index must be between {header.first_idx} and {header.last_idx}");

        idx -= header.first_idx;
        return ref elements[idx];
    }

    /// <summary>
    /// Gets the text at the specified offset from the end of the data.
    /// <remarks>
    /// This method performs no bound checking, it is up to the caller to ensure the offset is valid.
    /// </remarks>
    /// </summary>
    /// <param name="offset">The offset of the text.</param>
    /// <returns>The decoded text from the specified offset.</returns>
    public string get_text(nint offset = 0) {
        ReadOnlySpan<byte> encoded_text;

        fixed (ExcelFile<T>* this_ptr = &this) {
            encoded_text = new((byte*)((nint)this_ptr + header.data_start + header.data_length + offset), int.MaxValue);
        }

        int len = FhCharset.compute_decode_buffer_size(encoded_text);
        byte[] out_text = new byte[len];
        FhCharset.decode(encoded_text, out_text);
        return Encoding.UTF8.GetString(out_text);
    }
}
