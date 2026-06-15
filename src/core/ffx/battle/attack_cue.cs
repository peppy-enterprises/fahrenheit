// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit.FFX.Battle;

[StructLayout(LayoutKind.Explicit, Size = 0x10)]
public struct AttackCommandInfo {
    [FieldOffset(0x8)] public uint targets; // bitfield?
}

[StructLayout(LayoutKind.Explicit, Size=0x48)]
public struct AttackCue {
    [InlineArray(4)]
    public struct CommandList {
        private AttackCommandInfo __data;
    }

    [FieldOffset(0x0)] public byte        attacker_id;
    [FieldOffset(0x3)] public byte        command_count;
    [FieldOffset(0x8)] public CommandList command_list;
}
