// SPDX-License-Identifier: MIT

// ffx2/master/jppc/battle/kernel/plate.h
// Switch release of FFX/X-2 HD

namespace Fahrenheit.Core.FFX2;

public unsafe partial struct Plate
{
    [NativeTypeName("unsigned int")]
    public uint name;

    [NativeTypeName("unsigned int")]
    public uint help;

    [NativeTypeName("unsigned int")]
    public uint message1;

    [NativeTypeName("unsigned int")]
    public uint message2;

    [NativeTypeName("unsigned int")]
    public uint message3;

    [NativeTypeName("unsigned int")]
    public uint message4;

    [NativeTypeName("unsigned short")]
    public ushort bonus;

    [NativeTypeName("unsigned char")]
    public byte icon;

    [NativeTypeName("char[10]")]
    public fixed sbyte up_status[10];

    [NativeTypeName("unsigned char")]
    public byte reserve;

    [NativeTypeName("unsigned char")]
    public byte reserve2;

    [NativeTypeName("unsigned char")]
    public byte reserve3;

    [NativeTypeName("unsigned short[16]")]
    public fixed ushort skill[16];
}
