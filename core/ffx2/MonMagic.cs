// SPDX-License-Identifier: MIT

// ffx_ps2/ffx2/master/jppc/battle/kernel/monmagic.h
// Switch release of FFX/X-2 HD

using System.Runtime.CompilerServices;

namespace Fahrenheit.Core.FFX2;

public partial struct MonMagic
{
    [NativeTypeName("unsigned int")]
    public uint name;

    [NativeTypeName("unsigned int")]
    public uint help;

    [NativeTypeName("unsigned short[2]")]
    public _effect_e__FixedBuffer effect;

    [NativeTypeName("unsigned char")]
    public byte process;

    [NativeTypeName("unsigned char")]
    public byte sub_command;

    [NativeTypeName("unsigned char")]
    public byte system;

    [NativeTypeName("unsigned char")]
    public byte flow_system;

    [NativeTypeName("unsigned int")]
    public uint cursor;

    [NativeTypeName("unsigned int")]
    public uint exp_data;

    [NativeTypeName("unsigned int")]
    public uint dmg_data;

    [NativeTypeName("unsigned short")]
    public ushort sub_window;

    [NativeTypeName("unsigned short")]
    public ushort atb_cost;

    [NativeTypeName("unsigned short")]
    public ushort chant_cost;

    [NativeTypeName("unsigned char")]
    public byte mp;

    [NativeTypeName("unsigned char")]
    public byte target;

    [NativeTypeName("unsigned char")]
    public byte calc_ps;

    [NativeTypeName("unsigned char")]
    public byte critical;

    [NativeTypeName("unsigned char")]
    public byte hit;

    [NativeTypeName("unsigned char")]
    public byte power;

    [NativeTypeName("unsigned char")]
    public byte atc_num;

    [NativeTypeName("unsigned char")]
    public byte atc_stone;

    [NativeTypeName("unsigned char")]
    public byte atc_element;

    [NativeTypeName("unsigned char[24]")]
    public _atc_status_e__FixedBuffer atc_status;

    [NativeTypeName("unsigned char[24]")]
    public _atc_status2_e__FixedBuffer atc_status2;

    [NativeTypeName("char[24]")]
    public _status_time_e__FixedBuffer status_time;

    [NativeTypeName("unsigned char")]
    public byte icon;

    [NativeTypeName("unsigned short")]
    public ushort monseter_killer;

    [NativeTypeName("unsigned char")]
    public byte magic_cancel;

    [NativeTypeName("unsigned char")]
    public byte reserve1;

    [NativeTypeName("unsigned short")]
    public ushort blue_magic;

    [NativeTypeName("unsigned short")]
    public ushort reserve2;

    [InlineArray(2)]
    public partial struct _effect_e__FixedBuffer
    {
        public ushort e0;
    }

    [InlineArray(24)]
    public partial struct _atc_status_e__FixedBuffer
    {
        public byte e0;
    }

    [InlineArray(24)]
    public partial struct _atc_status2_e__FixedBuffer
    {
        public byte e0;
    }

    [InlineArray(24)]
    public partial struct _status_time_e__FixedBuffer
    {
        public sbyte e0;
    }
}
