// SPDX-License-Identifier: MIT

// ffx_ps2/ffx2/master/jppc/battle/kernel/party.h
// Switch release of FFX/X-2 HD

namespace Fahrenheit.Core.FFX2;

public unsafe partial struct Party
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
    public fixed byte party[3];

    [NativeTypeName("unsigned char")]
    public byte ATB_speed;

    [NativeTypeName("unsigned short[8]")]
    public fixed ushort item_type[8];

    [NativeTypeName("unsigned char[8]")]
    public fixed byte item_num[8];

    [NativeTypeName("int[2]")]
    public fixed int plate[2];

    [NativeTypeName("char[26]")]
    public fixed sbyte dre_sphere[26];

    [NativeTypeName("char")]
    public sbyte reserve2;

    [NativeTypeName("char")]
    public sbyte reserve3;

    public int escape_count;

    public int reserve5;
}
