// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

// ffx_ps2/ffx2/master/jppc/battle/kernel/plate.h
// Switch release of FFX/X-2 HD

namespace Fahrenheit.FFX2;

[InlineArray(16)]
public partial struct PlateBenefits
{
    public ushort e0;
}

[InlineArray(4)]
public partial struct Abilities {
    public ushort e0;
}

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x80)]
public struct Plate {
    [FieldOffset(0x00)] public  uint          name_offset;
    [FieldOffset(0x04)] public  uint          help_offset;
    [FieldOffset(0x08)] public  uint          message_1;
    [FieldOffset(0x0C)] public  uint          message_2;
    [FieldOffset(0x10)] public  uint          message_3;
    [FieldOffset(0x14)] public  uint          message_4;
    [FieldOffset(0x18)] public  ushort        bonus;
    [FieldOffset(0x1A)] public  byte          icon;
    [FieldOffset(0x1B)] public  byte          hp;
    [FieldOffset(0x1C)] public  byte          mp;
    [FieldOffset(0x1D)] public  byte          strength;
    [FieldOffset(0x1E)] public  byte          defense;
    [FieldOffset(0x1F)] public  byte          magic;
    [FieldOffset(0x20)] public  byte          magic_defense;
    [FieldOffset(0x21)] public  byte          agility;
    [FieldOffset(0x22)] public  byte          accuracy;
    [FieldOffset(0x23)] public  byte          evasion;
    [FieldOffset(0x24)] public  byte          luck;
    [FieldOffset(0x25)] private byte          reserve;
    [FieldOffset(0x26)] private byte          reserve2;
    [FieldOffset(0x27)] private byte          reserve3;
    [FieldOffset(0x28)] public  PlateBenefits skill;
    [FieldOffset(0x48)] public  uint          creature_help_offset;
    [FieldOffset(0x4C)] public  Abilities     creature_abilities;
    // 0x54 - 0x73 unknown?
    [FieldOffset(0x74)] public  byte          bonus_hp;
    [FieldOffset(0x75)] public  byte          bonus_mp;
    [FieldOffset(0x76)] public  byte          bonus_strength;
    [FieldOffset(0x77)] public  byte          bonus_defense;
    [FieldOffset(0x78)] public  byte          bonus_magic;
    [FieldOffset(0x79)] public  byte          bonus_magic_defense;
    [FieldOffset(0x7A)] public  byte          bonus_agility;
    [FieldOffset(0x7B)] public  byte          bonus_accuracy;
    [FieldOffset(0x7C)] public  byte          bonus_evasion;
    [FieldOffset(0x7D)] public  byte          bonus_luck;
    [FieldOffset(0x7E)] private byte          reserve4;
    [FieldOffset(0x7F)] private byte          reserve5;
}
