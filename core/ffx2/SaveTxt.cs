// SPDX-License-Identifier: MIT

// ffx_ps2/ffx2/master/jppc/battle/kernel/save_txt.h
// Switch release of FFX/X-2 HD

using System.Runtime.CompilerServices;

namespace Fahrenheit.Core.FFX2;

public partial struct SaveTxt
{
    [NativeTypeName("unsigned int[2]")]
    public _command_e__FixedBuffer command;

    [NativeTypeName("unsigned int[2]")]
    public _help_e__FixedBuffer help;

    [InlineArray(2)]
    public partial struct _command_e__FixedBuffer
    {
        public uint e0;
    }

    [InlineArray(2)]
    public partial struct _help_e__FixedBuffer
    {
        public uint e0;
    }
}
