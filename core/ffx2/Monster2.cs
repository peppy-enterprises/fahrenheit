// SPDX-License-Identifier: MIT

// ffx2/master/jppc/battle/kernel/monster2.h
// Switch release of FFX/X-2 HD

namespace Fahrenheit.Core.FFX2;

public unsafe partial struct Monster2
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
    public fixed byte def_status[24];

    [NativeTypeName("unsigned char[24]")]
    public fixed byte def_status2[24];

    public int auto_status;

    public int auto_status2;

    [NativeTypeName("char[24]")]
    public fixed sbyte status_time[24];

    [NativeTypeName("unsigned short[16]")]
    public fixed ushort waza[16];

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
    public fixed ushort drop_item[4];

    [NativeTypeName("unsigned short[4]")]
    public fixed ushort steal_item[4];

    [NativeTypeName("unsigned short[4]")]
    public fixed ushort bribery_item[4];

    [NativeTypeName("unsigned char")]
    public byte def_zantetu;

    [NativeTypeName("unsigned char")]
    public byte reserve1;

    [NativeTypeName("unsigned short")]
    public ushort reserve2;
}
