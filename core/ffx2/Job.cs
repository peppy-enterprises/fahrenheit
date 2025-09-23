// SPDX-License-Identifier: MIT

// ffx2/master/jppc/battle/kernel/job.h
// Switch release of FFX/X-2 HD

namespace Fahrenheit.Core.FFX2;

public unsafe partial struct Job
{
    [NativeTypeName("unsigned int")]
    public uint name;

    [NativeTypeName("unsigned int")]
    public uint help;

    [NativeTypeName("unsigned char")]
    public byte user;

    [NativeTypeName("unsigned char")]
    public byte data;

    [NativeTypeName("unsigned char")]
    public byte seq;

    [NativeTypeName("unsigned char")]
    public byte icon;

    [NativeTypeName("unsigned short")]
    public ushort basaku;

    [NativeTypeName("unsigned char[3]")]
    public fixed byte hp_up[3];

    [NativeTypeName("unsigned char[3]")]
    public fixed byte mp_up[3];

    [NativeTypeName("unsigned char[4]")]
    public fixed byte str_up[4];

    [NativeTypeName("unsigned char[4]")]
    public fixed byte vit_up[4];

    [NativeTypeName("unsigned char[4]")]
    public fixed byte mag_up[4];

    [NativeTypeName("unsigned char[4]")]
    public fixed byte spirit_up[4];

    [NativeTypeName("unsigned char[4]")]
    public fixed byte dex_up[4];

    [NativeTypeName("unsigned char[4]")]
    public fixed byte avoid_up[4];

    [NativeTypeName("unsigned char[4]")]
    public fixed byte hit_up[4];

    [NativeTypeName("unsigned char[4]")]
    public fixed byte luck_up[4];

    [NativeTypeName("unsigned short[32]")]
    public fixed ushort ability[32];

    [NativeTypeName("short[3][8]")]
    public fixed short weapon[3 * 8];
}
