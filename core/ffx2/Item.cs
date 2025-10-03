// SPDX-License-Identifier: MIT

// ffx_ps2/ffx2/master/jppc/battle/kernel/item.h
// Switch release of FFX/X-2 HD

namespace Fahrenheit.Core.FFX2;

public unsafe partial struct Item
{
    [NativeTypeName("unsigned int")]
    public uint name;

    [NativeTypeName("unsigned int")]
    public uint help;

    [NativeTypeName("short[2]")]
    public fixed short effect[2];

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
    public fixed byte atc_status[24];

    [NativeTypeName("unsigned char[24]")]
    public fixed byte atc_status2[24];

    [NativeTypeName("char[24]")]
    public fixed sbyte status_time[24];

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

    [NativeTypeName("unsigned char")]
    public byte seq;

    [NativeTypeName("unsigned char")]
    public byte get_ap;

    [NativeTypeName("unsigned char")]
    public byte item_element;

    [NativeTypeName("unsigned char")]
    public byte item_level;

    [NativeTypeName("unsigned int")]
    public uint price;
}
