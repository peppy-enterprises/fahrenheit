// SPDX-License-Identifier: MIT

// ffx_ps2/ffx2/master/jppc/battle/kernel/job.h
// Switch release of FFX/X-2 HD

using System.Runtime.CompilerServices;

namespace Fahrenheit.Core.FFX2;

public partial struct Job
{
    [NativeTypeName("unsigned int")]
    public uint name;

    [NativeTypeName("unsigned int")]
    public uint help;

    [NativeTypeName("unsigned char")]
    public byte user;

    [NativeTypeName("unsigned char")]
    public byte data;

    [NativeTypeName("unsigned char")]
    public byte seq;

    [NativeTypeName("unsigned char")]
    public byte icon;

    [NativeTypeName("unsigned short")]
    public ushort basaku;

    [NativeTypeName("unsigned char[3]")]
    public _hp_up_e__FixedBuffer hp_up;

    [NativeTypeName("unsigned char[3]")]
    public _mp_up_e__FixedBuffer mp_up;

    [NativeTypeName("unsigned char[4]")]
    public _str_up_e__FixedBuffer str_up;

    [NativeTypeName("unsigned char[4]")]
    public _vit_up_e__FixedBuffer vit_up;

    [NativeTypeName("unsigned char[4]")]
    public _mag_up_e__FixedBuffer mag_up;

    [NativeTypeName("unsigned char[4]")]
    public _spirit_up_e__FixedBuffer spirit_up;

    [NativeTypeName("unsigned char[4]")]
    public _dex_up_e__FixedBuffer dex_up;

    [NativeTypeName("unsigned char[4]")]
    public _avoid_up_e__FixedBuffer avoid_up;

    [NativeTypeName("unsigned char[4]")]
    public _hit_up_e__FixedBuffer hit_up;

    [NativeTypeName("unsigned char[4]")]
    public _luck_up_e__FixedBuffer luck_up;

    [NativeTypeName("unsigned short[32]")]
    public _ability_e__FixedBuffer ability;

    [NativeTypeName("short[3][8]")]
    public _weapon_e__FixedBuffer weapon;

    [InlineArray(3)]
    public partial struct _hp_up_e__FixedBuffer
    {
        public byte e0;
    }

    [InlineArray(3)]
    public partial struct _mp_up_e__FixedBuffer
    {
        public byte e0;
    }

    [InlineArray(4)]
    public partial struct _str_up_e__FixedBuffer
    {
        public byte e0;
    }

    [InlineArray(4)]
    public partial struct _vit_up_e__FixedBuffer
    {
        public byte e0;
    }

    [InlineArray(4)]
    public partial struct _mag_up_e__FixedBuffer
    {
        public byte e0;
    }

    [InlineArray(4)]
    public partial struct _spirit_up_e__FixedBuffer
    {
        public byte e0;
    }

    [InlineArray(4)]
    public partial struct _dex_up_e__FixedBuffer
    {
        public byte e0;
    }

    [InlineArray(4)]
    public partial struct _avoid_up_e__FixedBuffer
    {
        public byte e0;
    }

    [InlineArray(4)]
    public partial struct _hit_up_e__FixedBuffer
    {
        public byte e0;
    }

    [InlineArray(4)]
    public partial struct _luck_up_e__FixedBuffer
    {
        public byte e0;
    }

    [InlineArray(32)]
    public partial struct _ability_e__FixedBuffer
    {
        public ushort e0;
    }

    [InlineArray(3 * 8)]
    public partial struct _weapon_e__FixedBuffer
    {
        public short e0_0;
    }
}
