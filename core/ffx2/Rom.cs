// SPDX-License-Identifier: MIT

// ffx_ps2/ffx2/master/jppc/battle/kernel/rom.h
// Switch release of FFX/X-2 HD

namespace Fahrenheit.Core.FFX2;

public unsafe partial struct Rom
{
    [NativeTypeName("unsigned int")]
    public uint poison_time;

    [NativeTypeName("unsigned int")]
    public uint poison_damage;

    [NativeTypeName("unsigned int")]
    public uint regen_time;

    [NativeTypeName("unsigned int")]
    public uint regen_damage;

    [NativeTypeName("unsigned int[2]")]
    public fixed uint count_value[2];

    [NativeTypeName("int[3]")]
    public fixed int rapid_shot[3];

    [NativeTypeName("short[4]")]
    public fixed short ATB_speed[4];

    [NativeTypeName("unsigned int[2]")]
    public fixed uint delay_count[2];

    [NativeTypeName("unsigned int[24]")]
    public fixed uint off_count[24];
}
