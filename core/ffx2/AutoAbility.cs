// SPDX-License-Identifier: MIT

// ffx_ps2/ffx2/master/jppc/battle/kernel/a_ability.h
// Switch release of FFX/X-2 HD

using System.Runtime.CompilerServices;

namespace Fahrenheit.Core.FFX2;

public partial struct AutoAbility
{
    [NativeTypeName("unsigned int")]
    public uint name;

    [NativeTypeName("unsigned int")]
    public uint help;

    [NativeTypeName("unsigned short[4]")]
    public _reserve_e__FixedBuffer reserve;

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
    public _up_status_e__FixedBuffer up_status;

    [NativeTypeName("unsigned char[24]")]
    public _atc_status_e__FixedBuffer atc_status;

    [NativeTypeName("char[24]")]
    public _atc_status2_e__FixedBuffer atc_status2;

    [NativeTypeName("unsigned char[24]")]
    public _def_status_e__FixedBuffer def_status;

    [NativeTypeName("unsigned char[24]")]
    public _def_status2_e__FixedBuffer def_status2;

    public int auto_status;

    public int auto_status2;

    [NativeTypeName("char[24]")]
    public _status_time_e__FixedBuffer status_time;

    [NativeTypeName("unsigned short[3]")]
    public _ability_type_e__FixedBuffer ability_type;

    [NativeTypeName("unsigned char")]
    public byte icon;

    [NativeTypeName("unsigned char")]
    public byte reserve3;

    [NativeTypeName("unsigned short")]
    public ushort reserve4;

    [NativeTypeName("unsigned short")]
    public ushort ap;

    [InlineArray(4)]
    public partial struct _reserve_e__FixedBuffer
    {
        public ushort e0;
    }

    [InlineArray(10)]
    public partial struct _up_status_e__FixedBuffer
    {
        public sbyte e0;
    }

    [InlineArray(24)]
    public partial struct _atc_status_e__FixedBuffer
    {
        public byte e0;
    }

    [InlineArray(24)]
    public partial struct _atc_status2_e__FixedBuffer
    {
        public sbyte e0;
    }

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

    [InlineArray(3)]
    public partial struct _ability_type_e__FixedBuffer
    {
        public ushort e0;
    }
}
