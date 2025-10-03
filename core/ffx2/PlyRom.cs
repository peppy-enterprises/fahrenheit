// SPDX-License-Identifier: MIT

// ffx_ps2/ffx2/master/jppc/battle/kernel/ply_rom.h
// Switch release of FFX/X-2 HD

using System.Runtime.CompilerServices;

namespace Fahrenheit.Core.FFX2;

public partial struct PlyRom
{
    [NativeTypeName("unsigned int")]
    public uint help;

    [NativeTypeName("unsigned char")]
    public byte data;

    [NativeTypeName("unsigned char[2]")]
    public _level_up_e__FixedBuffer level_up;

    [NativeTypeName("unsigned char")]
    public byte dummy;

    [InlineArray(2)]
    public partial struct _level_up_e__FixedBuffer
    {
        public byte e0;
    }
}
