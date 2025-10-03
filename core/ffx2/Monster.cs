// SPDX-License-Identifier: MIT

// ffx_ps2/ffx2/master/jppc/battle/kernel/monster.h
// Switch release of FFX/X-2 HD

using System.Runtime.CompilerServices;

namespace Fahrenheit.Core.FFX2;

public partial struct Monster
{
    [NativeTypeName("unsigned int")]
    public uint name;

    [NativeTypeName("unsigned int")]
    public uint help;

    [NativeTypeName("unsigned int")]
    public uint hp_max;

    [NativeTypeName("unsigned int")]
    public uint mp_max;

    [NativeTypeName("unsigned char")]
    public byte level;

    [NativeTypeName("unsigned char")]
    public byte str;

    [NativeTypeName("unsigned char")]
    public byte vit;

    [NativeTypeName("unsigned char")]
    public byte mag;

    [NativeTypeName("unsigned char")]
    public byte spirit;

    [NativeTypeName("unsigned char")]
    public byte dex;

    [NativeTypeName("unsigned char")]
    public byte hit;

    [NativeTypeName("unsigned char")]
    public byte avoid;

    [NativeTypeName("unsigned char")]
    public byte luck;

    [NativeTypeName("unsigned char")]
    public byte thinking_time;

    [NativeTypeName("unsigned short")]
    public ushort special;

    [NativeTypeName("unsigned char")]
    public byte abs_element;

    [NativeTypeName("unsigned char")]
    public byte inv_element;

    [NativeTypeName("unsigned char")]
    public byte half_element;

    [NativeTypeName("unsigned char")]
    public byte weak_element;

    [NativeTypeName("unsigned char[24]")]
    public _def_status_e__FixedBuffer def_status;

    [NativeTypeName("unsigned char[24]")]
    public _def_status2_e__FixedBuffer def_status2;

    public int auto_status;

    public int auto_status2;

    [NativeTypeName("char[24]")]
    public _status_time_e__FixedBuffer status_time;

    [NativeTypeName("unsigned short[16]")]
    public _waza_e__FixedBuffer waza;

    [NativeTypeName("unsigned short")]
    public ushort basaku;

    [NativeTypeName("unsigned short")]
    public ushort mon_model;

    [NativeTypeName("unsigned short")]
    public ushort mon_motion;

    [NativeTypeName("unsigned short")]
    public ushort monster_sound;

    [NativeTypeName("unsigned short")]
    public ushort oversoul;

    [NativeTypeName("unsigned short")]
    public ushort monster_type;

    public int exp;

    public int gill;

    public int steal_gill;

    [NativeTypeName("unsigned short")]
    public ushort get_ap;

    [NativeTypeName("unsigned char")]
    public byte drop;

    [NativeTypeName("unsigned char")]
    public byte steal;

    [NativeTypeName("unsigned short[4]")]
    public _drop_item_e__FixedBuffer drop_item;

    [NativeTypeName("unsigned short[4]")]
    public _steal_item_e__FixedBuffer steal_item;

    [NativeTypeName("unsigned short[4]")]
    public _bribery_item_e__FixedBuffer bribery_item;

    [NativeTypeName("unsigned char")]
    public byte def_zantetu;

    [NativeTypeName("unsigned char")]
    public byte reserve1;

    [NativeTypeName("unsigned short")]
    public ushort reserve2;

    [InlineArray(24)]
    public partial struct _def_status_e__FixedBuffer
    {
        public byte e0;
    }

    [InlineArray(24)]
    public partial struct _def_status2_e__FixedBuffer
    {
        public byte e0;
    }

    [InlineArray(24)]
    public partial struct _status_time_e__FixedBuffer
    {
        public sbyte e0;
    }

    [InlineArray(16)]
    public partial struct _waza_e__FixedBuffer
    {
        public ushort e0;
    }

    [InlineArray(4)]
    public partial struct _drop_item_e__FixedBuffer
    {
        public ushort e0;
    }

    [InlineArray(4)]
    public partial struct _steal_item_e__FixedBuffer
    {
        public ushort e0;
    }

    [InlineArray(4)]
    public partial struct _bribery_item_e__FixedBuffer
    {
        public ushort e0;
    }
}
