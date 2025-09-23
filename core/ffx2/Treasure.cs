// SPDX-License-Identifier: MIT

// ffx2/master/jppc/battle/kernel/takara.h
// ffx2/master/jppc/battle/kernel/takara.ath
// Switch release of FFX/X-2 HD

namespace Fahrenheit.Core.FFX2;

public partial struct Treasure
{
    [NativeTypeName("unsigned char")]
    public byte type;

    [NativeTypeName("unsigned char")]
    public byte kazu;

    [NativeTypeName("unsigned short")]
    public ushort item_name;
}
