// SPDX-License-Identifier: MIT

// ffx2/master/jppc/battle/kernel/ply_rom.h
// Switch release of FFX/X-2 HD

namespace Fahrenheit.Core.FFX2;

public unsafe partial struct PlyRom
{
    [NativeTypeName("unsigned int")]
    public uint help;

    [NativeTypeName("unsigned char")]
    public byte data;

    [NativeTypeName("unsigned char[2]")]
    public fixed byte level_up[2];

    [NativeTypeName("unsigned char")]
    public byte dummy;
}
