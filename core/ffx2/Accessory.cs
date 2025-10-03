// SPDX-License-Identifier: MIT

// ffx2/master/jppc/battle/kernel/accessory.h
// Switch release of FFX/X-2 HD

using System.Runtime.CompilerServices;

namespace Fahrenheit.Core.FFX2;

public partial struct Accessory
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
    public _up_status_e__FixedBuffer up_status;

    [NativeTypeName("unsigned short[4]")]
    public _ability_e__FixedBuffer ability;

    [NativeTypeName("unsigned int")]
    public uint price;

    [InlineArray(10)]
    public partial struct _up_status_e__FixedBuffer
    {
        public sbyte e0;
    }

    [InlineArray(4)]
    public partial struct _ability_e__FixedBuffer
    {
        public ushort e0;
    }
}
