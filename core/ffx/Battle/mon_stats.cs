// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core.FFX.Battle;

[StructLayout(LayoutKind.Sequential)]
public struct MonStats {
    public ushort                name_offset;
    public ushort                name_key;
    public ushort                sensor_text_offset;
    public ushort                sensor_text_key;
    public ushort                sensor_text_simplified_offset;
    public ushort                sensor_text_simplified_key;
    public ushort                scan_text_offset;
    public ushort                scan_text_key;
    public ushort                scan_text_simplified_offset;
    public ushort                scan_text_simplified_key;
    public uint                  hp;
    public uint                  mp;
    public uint                  overkill_threshold;
    public byte                  strength;
    public byte                  defense;
    public byte                  magic;
    public byte                  magic_defense;
    public byte                  agility;
    public byte                  luck;
    public byte                  evasion;
    public byte                  accuracy;
    public ChrPropFlags          chr_props;
    public byte                  poison_dmg;
    public ElementFlags          elem_absorb;
    public ElementFlags          elem_ignore;
    public ElementFlags          elem_resist;
    public ElementFlags          elem_weak;
    public StatusMap             status_resist;
    public StatusPermanentFlags  status_auto_permanent;
    public StatusTemporalFlags   status_auto_temporal;
    public StatusExtraFlags      status_auto_extra;
    public StatusExtraFlags      status_resist_extra;
    public InlineArray16<ushort> command_list;
    public ushort                forced_move;
    public ushort                monster_idx;
    public ushort                model_idx;
    public byte                  ctb_icon_type;
    public byte                  doom_counter;
    public ushort                monster_arena_idx;
    public ushort                sound_bank_ref;
    public ushort                always_zero_7c;
    public ushort                always_zero_7d;
    public ushort                always_zero_7e;
    public ushort                always_zero_7f;
}
