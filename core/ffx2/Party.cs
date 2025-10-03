// SPDX-License-Identifier: MIT

// ffx_ps2/ffx2/master/jppc/battle/kernel/party.h
// Switch release of FFX/X-2 HD

using System.Runtime.CompilerServices;

namespace Fahrenheit.Core.FFX2;

public partial struct Party
{
    public int config;

    [NativeTypeName("unsigned int")]
    public uint albhed;

    [NativeTypeName("unsigned int")]
    public uint gil;

    [NativeTypeName("unsigned int")]
    public uint play_time;

    [NativeTypeName("unsigned int")]
    public uint battle_time;

    [NativeTypeName("unsigned int")]
    public uint battle_count;

    [NativeTypeName("unsigned char[3]")]
    public _party_e__FixedBuffer party;

    [NativeTypeName("unsigned char")]
    public byte ATB_speed;

    [NativeTypeName("unsigned short[8]")]
    public _item_type_e__FixedBuffer item_type;

    [NativeTypeName("unsigned char[8]")]
    public _item_num_e__FixedBuffer item_num;

    [NativeTypeName("int[2]")]
    public _plate_e__FixedBuffer plate;

    [NativeTypeName("char[26]")]
    public _dre_sphere_e__FixedBuffer dre_sphere;

    [NativeTypeName("char")]
    public sbyte reserve2;

    [NativeTypeName("char")]
    public sbyte reserve3;

    public int escape_count;

    public int reserve5;

    [InlineArray(3)]
    public partial struct _party_e__FixedBuffer
    {
        public byte e0;
    }

    [InlineArray(8)]
    public partial struct _item_type_e__FixedBuffer
    {
        public ushort e0;
    }

    [InlineArray(8)]
    public partial struct _item_num_e__FixedBuffer
    {
        public byte e0;
    }

    [InlineArray(2)]
    public partial struct _plate_e__FixedBuffer
    {
        public int e0;
    }

    [InlineArray(26)]
    public partial struct _dre_sphere_e__FixedBuffer
    {
        public sbyte e0;
    }
}
