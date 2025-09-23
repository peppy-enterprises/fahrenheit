// SPDX-License-Identifier: MIT

// ffx2/master/jppc/battle/kernel/oversoul.h
// Switch release of FFX/X-2 HD

namespace Fahrenheit.Core.FFX2;

public unsafe partial struct Oversoul
{
    [NativeTypeName("unsigned int")]
    public uint name;

    [NativeTypeName("short[2]")]
    public fixed short count[2];
}
