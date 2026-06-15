// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

// ffx_ps2/ffx2/master/jppc/battle/kernel/plate.h
// Switch release of FFX/X-2 HD

namespace Fahrenheit.FFX2;

public partial struct Plate
{
    [NativeTypeName("unsigned int")]
    public uint name;

    [NativeTypeName("unsigned int")]
    public uint help;

    [NativeTypeName("unsigned int")]
    public uint message1;

    [NativeTypeName("unsigned int")]
    public uint message2;

    [NativeTypeName("unsigned int")]
    public uint message3;

    [NativeTypeName("unsigned int")]
    public uint message4;

    [NativeTypeName("unsigned short")]
    public ushort bonus;

    [NativeTypeName("unsigned char")]
    public byte icon;

    [NativeTypeName("char[10]")]
    public _up_status_e__FixedBuffer up_status;

    [NativeTypeName("unsigned char")]
    public byte reserve;

    [NativeTypeName("unsigned char")]
    public byte reserve2;

    [NativeTypeName("unsigned char")]
    public byte reserve3;

    [NativeTypeName("unsigned short[16]")]
    public _skill_e__FixedBuffer skill;

    [InlineArray(10)]
    public partial struct _up_status_e__FixedBuffer
    {
        public sbyte e0;
    }

    [InlineArray(16)]
    public partial struct _skill_e__FixedBuffer
    {
        public ushort e0;
    }
}
