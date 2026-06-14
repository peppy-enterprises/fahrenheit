// SPDX-License-Identifier: MIT

namespace Fahrenheit.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x2c)]
public unsafe struct TOMesWinWork {
    [FieldOffset(0x08)] public byte* text;
    [FieldOffset(0x0c)] public byte* _0xc;
    [FieldOffset(0x14)] public short status;

    [FieldOffset(0x16)] public short _0x16;

    [FieldOffset(0x18)] public int   _0x18;
    [FieldOffset(0x1d)] public byte  _0x1d;
    [FieldOffset(0x20)] public byte  _0x20;

}
