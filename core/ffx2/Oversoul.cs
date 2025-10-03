// SPDX-License-Identifier: MIT

// ffx_ps2/ffx2/master/jppc/battle/kernel/oversoul.h
// Switch release of FFX/X-2 HD

using System.Runtime.CompilerServices;

namespace Fahrenheit.Core.FFX2;

public partial struct Oversoul
{
    [NativeTypeName("unsigned int")]
    public uint name;

    [NativeTypeName("short[2]")]
    public _count_e__FixedBuffer count;

    [InlineArray(2)]
    public partial struct _count_e__FixedBuffer
    {
        public short e0;
    }
}
