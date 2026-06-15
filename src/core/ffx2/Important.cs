// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

// ffx_ps2/ffx2/master/jppc/battle/kernel/important.h 
// Switch release of FFX/X-2 HD

namespace Fahrenheit.FFX2;

public partial struct Important
{
    [NativeTypeName("unsigned int")]
    public uint name;

    [NativeTypeName("unsigned int")]
    public uint help;

    [NativeTypeName("unsigned char")]
    public byte item_type;

    [NativeTypeName("unsigned char")]
    public byte item_value;

    [NativeTypeName("unsigned char")]
    public byte icon;

    [NativeTypeName("unsigned char")]
    public byte number;
}
