// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core.FFX.Battle;

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
