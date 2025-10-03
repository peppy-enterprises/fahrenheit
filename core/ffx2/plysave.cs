// SPDX-License-Identifier: MIT

// ffx_ps2/ffx2/master/jppc/battle/kernel/ply_save.h
// Switch release of FFX/X-2 HD

using System.Runtime.CompilerServices;

namespace Fahrenheit.Core.FFX2;

public partial struct PlySave
{
    [NativeTypeName("unsigned int")]
    public uint name;

    [NativeTypeName("unsigned int")]
    public uint hp_bonus;

    [NativeTypeName("unsigned int")]
    public uint mp_bonus;

    [NativeTypeName("unsigned char")]
    public byte str_bonus;

    [NativeTypeName("unsigned char")]
    public byte vit_bonus;

    [NativeTypeName("unsigned char")]
    public byte mag_bonus;

    [NativeTypeName("unsigned char")]
    public byte spirit_bonus;

    [NativeTypeName("unsigned char")]
    public byte dex_bonus;

    [NativeTypeName("unsigned char")]
    public byte luck_bonus;

    [NativeTypeName("unsigned char")]
    public byte avoid_bonus;

    [NativeTypeName("unsigned char")]
    public byte hit_bonus;

    [NativeTypeName("unsigned int")]
    public uint exp;

    [NativeTypeName("unsigned int")]
    public uint next_exp;

    [NativeTypeName("unsigned int")]
    public uint hp;

    [NativeTypeName("unsigned int")]
    public uint mp;

    [NativeTypeName("unsigned int")]
    public uint hp_max;

    [NativeTypeName("unsigned int")]
    public uint mp_max;

    [NativeTypeName("unsigned char")]
    public byte party;

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
    public byte reserve;

    [NativeTypeName("unsigned short")]
    public ushort job;

    [NativeTypeName("unsigned short")]
    public ushort plate;

    [NativeTypeName("unsigned short[2]")]
    public _accessory_e__FixedBuffer accessory;

    [NativeTypeName("unsigned short")]
    public ushort command;

    [NativeTypeName("unsigned int")]
    public uint escape_count;

    [NativeTypeName("unsigned int")]
    public uint kill_count;

    [NativeTypeName("unsigned int")]
    public uint death_count;

    public int status;

    [NativeTypeName("unsigned short[3]")]
    public _ability_type_e__FixedBuffer ability_type;

    [NativeTypeName("unsigned short")]
    public ushort before_job;

    [InlineArray(2)]
    public partial struct _accessory_e__FixedBuffer
    {
        public ushort e0;
    }

    [InlineArray(3)]
    public partial struct _ability_type_e__FixedBuffer
    {
        public ushort e0;
    }
}
