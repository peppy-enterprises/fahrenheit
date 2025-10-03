// SPDX-License-Identifier: MIT

// ffx_ps2/ffx2/master/jppc/battle/kernel/prepare.h
// Switch release of FFX/X-2 HD

using System.Runtime.CompilerServices;

namespace Fahrenheit.Core.FFX2;

public partial struct Prepare
{
    [NativeTypeName("unsigned short[112]")]
    public _prepare_e__FixedBuffer prepare;

    [InlineArray(112)]
    public partial struct _prepare_e__FixedBuffer
    {
        public ushort e0;
    }
}
