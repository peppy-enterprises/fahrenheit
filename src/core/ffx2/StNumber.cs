// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

// ffx_ps2/ffx2/master/jppc/battle/kernel/st_number.h
// Switch release of FFX/X-2 HD

namespace Fahrenheit.FFX2;

public partial struct StNumber
{
    [NativeTypeName("unsigned char")]
    public byte category;

    [NativeTypeName("unsigned char")]
    public byte type;

    [NativeTypeName("unsigned short")]
    public ushort command_name;
}
