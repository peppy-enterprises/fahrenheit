// SPDX-License-Identifier: MIT

// ffx_ps2/ffx2/master/jppc/battle/kernel/item_shop.h
// Switch release of FFX/X-2 HD

using System.Runtime.CompilerServices;

namespace Fahrenheit.Core.FFX2;

public partial struct ItemShop
{
    [NativeTypeName("unsigned short")]
    public ushort shop_rate;

    [NativeTypeName("unsigned short[16]")]
    public _shop_item_e__FixedBuffer shop_item;

    [InlineArray(16)]
    public partial struct _shop_item_e__FixedBuffer
    {
        public ushort e0;
    }
}
