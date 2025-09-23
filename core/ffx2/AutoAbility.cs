// SPDX-License-Identifier: MIT

// Ported from ffx2/master/jppc/battle/kernel/a_ability.h in the Switch release of FFX/X-2 HD
namespace Fahrenheit.Core.FFX2;

public unsafe partial struct AutoAbility
{
    [NativeTypeName("unsigned int")]
    public uint name;

    [NativeTypeName("unsigned int")]
    public uint help;

    [NativeTypeName("unsigned short[4]")]
    public fixed ushort reserve[4];

    [NativeTypeName("unsigned short")]
    public ushort count_down_type;

    [NativeTypeName("char")]
    public sbyte count_down;

    [NativeTypeName("unsigned char")]
    public byte reserve2;

    [NativeTypeName("unsigned char")]
    public byte special_data;

    [NativeTypeName("unsigned char")]
    public byte atc_element;

    [NativeTypeName("unsigned char")]
    public byte abs_element;

    [NativeTypeName("unsigned char")]
    public byte inv_element;

    [NativeTypeName("unsigned char")]
    public byte half_element;

    [NativeTypeName("unsigned char")]
    public byte weak_element;

    [NativeTypeName("char[10]")]
    public fixed sbyte up_status[10];

    [NativeTypeName("unsigned char[24]")]
    public fixed byte atc_status[24];

    [NativeTypeName("char[24]")]
    public fixed sbyte atc_status2[24];

    [NativeTypeName("unsigned char[24]")]
    public fixed byte def_status[24];

    [NativeTypeName("unsigned char[24]")]
    public fixed byte def_status2[24];

    public int auto_status;

    public int auto_status2;

    [NativeTypeName("char[24]")]
    public fixed sbyte status_time[24];

    [NativeTypeName("unsigned short[3]")]
    public fixed ushort ability_type[3];

    [NativeTypeName("unsigned char")]
    public byte icon;

    [NativeTypeName("unsigned char")]
    public byte reserve3;

    [NativeTypeName("unsigned short")]
    public ushort reserve4;

    [NativeTypeName("unsigned short")]
    public ushort ap;
}
