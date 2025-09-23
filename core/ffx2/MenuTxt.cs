// SPDX-License-Identifier: MIT

// ffx2/master/jppc/battle/kernel/menu_txt.h
// Switch release of FFX/X-2 HD

namespace Fahrenheit.Core.FFX2;

public partial struct MenuTxt
{
    [NativeTypeName("unsigned int")]
    public uint command;

    [NativeTypeName("unsigned int")]
    public uint help;
}
