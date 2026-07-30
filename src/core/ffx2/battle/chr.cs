// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit.FFX2.Battle;

[StructLayout(LayoutKind.Explicit, Size = 0x17E0)]
public struct Chr {
    [FieldOffset(0x396)]   public byte strength;
    [FieldOffset(0x397)]   public byte defense;
    [FieldOffset(0x398)]   public byte magic;
    [FieldOffset(0x399)]   public byte magic_defense;
    [FieldOffset(0x39A)]   public byte agility;
    [FieldOffset(0x39B)]   public byte luck;
    [FieldOffset(0x39C)]   public byte evasion;
    [FieldOffset(0x39D)]   public byte accuracy;
}
