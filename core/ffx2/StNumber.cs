// SPDX-License-Identifier: MIT

// ffx2/master/jppc/battle/kernel/st_number.h
// Switch release of FFX/X-2 HD

namespace Fahrenheit.Core.FFX2;

public partial struct StNumber
{
    [NativeTypeName("unsigned char")]
    public byte category;

    [NativeTypeName("unsigned char")]
    public byte type;

    [NativeTypeName("unsigned short")]
    public ushort command_name;
}
