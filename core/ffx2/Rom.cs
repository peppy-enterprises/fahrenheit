// SPDX-License-Identifier: MIT

// ffx_ps2/ffx2/master/jppc/battle/kernel/rom.h
// Switch release of FFX/X-2 HD

using System.Runtime.CompilerServices;

namespace Fahrenheit.Core.FFX2;

public partial struct Rom
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
    public _count_value_e__FixedBuffer count_value;

    [NativeTypeName("int[3]")]
    public _rapid_shot_e__FixedBuffer rapid_shot;

    [NativeTypeName("short[4]")]
    public _ATB_speed_e__FixedBuffer ATB_speed;

    [NativeTypeName("unsigned int[2]")]
    public _delay_count_e__FixedBuffer delay_count;

    [NativeTypeName("unsigned int[24]")]
    public _off_count_e__FixedBuffer off_count;

    [InlineArray(2)]
    public partial struct _count_value_e__FixedBuffer
    {
        public uint e0;
    }

    [InlineArray(3)]
    public partial struct _rapid_shot_e__FixedBuffer
    {
        public int e0;
    }

    [InlineArray(4)]
    public partial struct _ATB_speed_e__FixedBuffer
    {
        public short e0;
    }

    [InlineArray(2)]
    public partial struct _delay_count_e__FixedBuffer
    {
        public uint e0;
    }

    [InlineArray(24)]
    public partial struct _off_count_e__FixedBuffer
    {
        public uint e0;
    }
}
