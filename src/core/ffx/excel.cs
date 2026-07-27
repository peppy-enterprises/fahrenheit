// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit.FFX;

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
public struct ExcelHeader {
    /// <summary>
    ///     The index of the first element in the file.
    /// </summary>
    public ushort index_first;

    /// <summary>
    ///     The index of the last element in the file.
    /// </summary>
    public ushort index_last;

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
    public uint data_start;

    /// <summary>
    ///     The length of the array of elements defined by this header.
    /// </summary>
    public readonly int length => index_last + 1 - index_first;
}

/// <summary>
///     Allows iteration over an Excel file of <typeparamref name="T"/>.
/// </summary>
public unsafe ref struct ExcelFileReader<T>(ReadOnlySpan<byte> excel_bytes) where T : unmanaged {
    private readonly ReadOnlySpan<byte> _bytes = excel_bytes;

    /// <summary>
    ///     The headers in this file. Each header defines a section of the file.
    /// </summary>
    public ReadOnlySpan<ExcelHeader> headers() {
        int sz_prolog = sizeof(ExcelFileProlog<T>);
        int sz_header = sizeof(ExcelHeader);

        return MemoryMarshal.TryRead(_bytes, out ExcelFileProlog<T> prolog)
            ? MemoryMarshal.Cast<byte, ExcelHeader>(_bytes [ sz_prolog .. (sz_prolog + prolog.header_count * sz_header) ])
            : [];
    }

    /// <summary>
    ///     The elements defined in the given <paramref name="header"/>.
    /// </summary>
    public ReadOnlySpan<T> elements(ExcelHeader header) {
        int start  = (int)header.data_start;
        int length = (int)header.data_length;

        return MemoryMarshal.Cast<byte, T>(_bytes [ start .. (start + length) ]);
    }
}

/// <summary>
///     A container for an array of <typeparamref name="T"/>.
///     These files make up most of the game's <c>kernel</c> folder.
///     <para/>
///     To iterate over its contents, use an <see cref="ExcelFileReader{T}"/>.
/// </summary>
/// <typeparam name="T">The type of elements in the file.</typeparam>
[StructLayout(LayoutKind.Sequential, Size = 0x8)]
public struct ExcelFileProlog<T> where T : unmanaged {
    /// <summary>
    ///     The amount of headers that map out this file.
    ///     <remarks>
    ///         In the games, this amount is always 1.
    ///         Both the games and Fahrenheit support amounts higher than 1.
    ///     </remarks>
    /// </summary>
    public  ushort header_count;
    private ushort _0x02;
    private ushort _0x04;
    private ushort _0x06;
}
