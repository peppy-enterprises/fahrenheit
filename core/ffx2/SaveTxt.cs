// SPDX-License-Identifier: MIT

// ffx_ps2/ffx2/master/jppc/battle/kernel/save_txt.h
// Switch release of FFX/X-2 HD

namespace Fahrenheit.Core.FFX2;

public unsafe partial struct SaveTxt
{
    [NativeTypeName("unsigned int[2]")]
    public fixed uint command[2];

    [NativeTypeName("unsigned int[2]")]
    public fixed uint help[2];
}
