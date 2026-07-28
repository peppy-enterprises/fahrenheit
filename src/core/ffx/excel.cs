// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit.FFX;

/* [fkelava 28/07/26 02:36]
 * The game stores various data in 'Excel' files, containers for an array of objects.
 * It is a form of binary serialization. These types model generic Excel containers.
 *
 * Most of the game's `kernel` directory consists of such files. They contain anything
 * from player command definitions to aeon stat growth curves.
 */

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
[StructLayout(LayoutKind.Sequential, Size = 0x8)]
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
///     The header of an Excel file.
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
    ///     The combined length, in bytes, of all the elements in the file.<br/>
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
    ///     Gets the headers of this Excel file. Each defines a section of the file.
    /// </summary>
    public ReadOnlySpan<ExcelHeader> get_headers() {
        int sz_prolog = sizeof(ExcelFileProlog);
        int sz_header = sizeof(ExcelHeader);

        return MemoryMarshal.TryRead(_bytes, out ExcelFileProlog prolog)
            ? MemoryMarshal.Cast<byte, ExcelHeader>(_bytes [ sz_prolog .. (sz_prolog + prolog.header_count * sz_header) ])
            : [];
    }

    /// <summary>
    ///     Gets the instances of <typeparamref name="T"/> defined in the given <paramref name="header"/>.
    /// </summary>
    public ReadOnlySpan<T> get_elements(ExcelHeader header) {
        int start  = (int)header.data_start;
        int length = (int)header.data_length;

        return MemoryMarshal.Cast<byte, T>(_bytes [ start .. (start + length) ]);
    }
}

/// <summary>
///     The prologue of an Excel file. Describes the file in enough detail to construct a reader.
///     <para/>
///     To iterate over the file's contents, use an <see cref="ExcelFileReader{T}"/>.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 0x8)]
public struct ExcelFileProlog {
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
