// SPDX-License-Identifier: MIT

// Ported from ffx2/master/jppc/battle/kernel/takara.h in the Switch release of FFX/X-2 HD
// Ported from ffx2/master/jppc/battle/kernel/takara.ath in the Switch release of FFX/X-2 HD
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
