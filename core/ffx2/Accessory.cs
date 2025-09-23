// SPDX-License-Identifier: MIT

// ffx2/master/jppc/battle/kernel/accessory.h
// Switch release of FFX/X-2 HD

namespace Fahrenheit.Core.FFX2;

public unsafe partial struct Accessory
{
    [NativeTypeName("unsigned int")]
    public uint name;

    [NativeTypeName("unsigned int")]
    public uint help;

    [NativeTypeName("unsigned char")]
    public byte ext_data;

    [NativeTypeName("unsigned char")]
    public byte equip;

    [NativeTypeName("unsigned char")]
    public byte user;

    [NativeTypeName("unsigned char")]
    public byte icon;

    [NativeTypeName("unsigned char")]
    public byte seq;

    [NativeTypeName("unsigned char")]
    public byte reserve;

    [NativeTypeName("char[10]")]
    public fixed sbyte up_status[10];

    [NativeTypeName("unsigned short[4]")]
    public fixed ushort ability[4];

    [NativeTypeName("unsigned int")]
    public uint price;
}
