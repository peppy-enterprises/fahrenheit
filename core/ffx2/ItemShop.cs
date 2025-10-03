// SPDX-License-Identifier: MIT

// ffx_ps2/ffx2/master/jppc/battle/kernel/item_shop.h
// Switch release of FFX/X-2 HD

namespace Fahrenheit.Core.FFX2;

public unsafe partial struct ItemShop
{
    [NativeTypeName("unsigned short")]
    public ushort shop_rate;

    [NativeTypeName("unsigned short[16]")]
    public fixed ushort shop_item[16];
}
