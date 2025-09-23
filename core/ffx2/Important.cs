// SPDX-License-Identifier: MIT

// ffx2/master/jppc/battle/kernel/important.h 
// Switch release of FFX/X-2 HD

namespace Fahrenheit.Core.FFX2;

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
