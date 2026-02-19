// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core.FFX.Battle;

[StructLayout(LayoutKind.Explicit, Size = 0x80)]
public struct MonStats {
    [FieldOffset(0x14)] public uint                  hp;
    [FieldOffset(0x18)] public uint                  mp;
    [FieldOffset(0x1c)] public uint                  overkill_threshold;
    [FieldOffset(0x20)] public byte                  strength;
    [FieldOffset(0x21)] public byte                  defense;
    [FieldOffset(0x22)] public byte                  magic;
    [FieldOffset(0x23)] public byte                  magic_defense;
    [FieldOffset(0x24)] public byte                  agility;
    [FieldOffset(0x25)] public byte                  luck;
    [FieldOffset(0x26)] public byte                  evasion;
    [FieldOffset(0x27)] public byte                  accuracy;
    [FieldOffset(0x28)] public ChrPropFlags          chr_props;
    [FieldOffset(0x2a)] public byte                  poison_dmg;
    [FieldOffset(0x2b)] public ElementFlags          elem_absorb;
    [FieldOffset(0x2c)] public ElementFlags          elem_ignore;
    [FieldOffset(0x2d)] public ElementFlags          elem_resist;
    [FieldOffset(0x2e)] public ElementFlags          elem_weak;
    [FieldOffset(0x2f)] public StatusMap             status_resist;
    [FieldOffset(0x48)] public StatusPermanentFlags  status_auto_permanent;
    [FieldOffset(0x4a)] public StatusTemporalFlags   status_auto_temporal;
    [FieldOffset(0x4c)] public StatusExtraFlags      status_auto_extra;
    [FieldOffset(0x4e)] public StatusExtraFlags      status_resist_extra;
    [FieldOffset(0x50)] public InlineArray16<ushort> command_list;
    [FieldOffset(0x70)] public ushort                forced_move;
    [FieldOffset(0x72)] public ushort                monster_idx;
    [FieldOffset(0x74)] public ushort                model_idx;
    [FieldOffset(0x76)] public byte                  ctb_icon_type;
    [FieldOffset(0x77)] public byte                  doom_counter;
    [FieldOffset(0x78)] public ushort                monster_arena_idx;
    [FieldOffset(0x7a)] public ushort                sound_bank_ref;
    [FieldOffset(0x7c)] public ushort                always_zero_7c;
    [FieldOffset(0x7d)] public ushort                always_zero_7d;
    [FieldOffset(0x7e)] public ushort                always_zero_7e;
    [FieldOffset(0x7f)] public ushort                always_zero_7f;
}
