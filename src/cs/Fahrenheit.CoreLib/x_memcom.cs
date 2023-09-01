namespace Fahrenheit.CoreLib;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x60)]
public struct FhXMemCom {
	[FieldOffset(0x00)] public ushort name_offset;
	[FieldOffset(0x04)] public ushort dash_offset;
	[FieldOffset(0x08)] public ushort desc_offset;
	[FieldOffset(0x0C)] public ushort misc_offset;
	[FieldOffset(0x10)] public ushort anim1;
	[FieldOffset(0x12)] public ushort anim2;
	[FieldOffset(0x14)] public byte icon;
	[FieldOffset(0x15)] public byte caster_anim;
	[FieldOffset(0x16)] public byte menu_props;
	public bool top_level_in_menu { get { return (menu_props & 1) != 0; } }
	public bool opens_sub_menu { get { return (menu_props >> 4 & 1) != 0; } }
	[FieldOffset(0x17)] public byte sub_menu_cat2;
	[FieldOffset(0x18)] public byte sub_menu_cat;
	[FieldOffset(0x19)] public byte user_id;
	[FieldOffset(0x1A)] public byte targets_flag;
	public bool can_target { get { return (targets_flag & 1) != 0; } }
	public bool targets_enemies { get { return (targets_flag >> 1 & 1) != 0; } }
	public bool targets_multiple { get { return (targets_flag >> 2 & 1) != 0; } }
	public bool targets_self_only { get { return (targets_flag >> 3 & 1) != 0; } }
	public bool targets_flag5 { get { return (targets_flag >> 4 & 1) != 0; } }
	public bool targets_team { get { return (targets_flag >> 5 & 1) != 0; } }
	public bool targets_dead { get { return (targets_flag >> 6 & 1) != 0; } }
	public bool targets_flag8 { get { return (targets_flag >> 7 & 1) != 0; } }
	[FieldOffset(0x1C)] public uint misc_props;
	public bool usable_outside_combat { get { return (misc_props & 1) != 0; } }
	public bool usable_in_combat { get { return (misc_props >> 1 & 1) != 0; } }
	public bool display_move_name { get { return (misc_props >> 2 & 1) != 0; } }
	public uint accuracy_formula { get { return misc_props >> 3 & 7; } }
	public bool affected_by_darkness { get { return (misc_props >> 6 & 1) != 0; } }
	public bool affected_by_reflect { get { return (misc_props >> 7 & 1) != 0; } }
	public bool absorbs_dmg { get { return (misc_props >> 8 & 1) != 0; } }
	public bool steals_item { get { return (misc_props >> 9 & 1) != 0; } }
	public bool in_use_menu { get { return (misc_props >> 10 & 1) != 0; } }
	public bool in_sub_menu { get { return (misc_props >> 11 & 1) != 0; } }
	public bool in_trigger_menu { get { return (misc_props >> 12 & 1) != 0; } }
	public bool inflicts_delay_weak { get { return (misc_props >> 13 & 1) != 0; } }
	public bool inflicts_delay_strong { get { return (misc_props >> 14 & 1) != 0; } }
	public bool targets_random { get { return (misc_props >> 15 & 1) != 0; } }
	public bool pierces { get { return (misc_props >> 16 & 1) != 0; } }
	public bool affected_by_silence { get { return (misc_props >> 17 & 1) != 0; } }
	public bool uses_weapon_props { get { return (misc_props >> 18 & 1) != 0; } }
	public bool trigger_command { get { return (misc_props >> 19 & 1) != 0; } }
	public bool uses_tier1_cast_anim { get { return (misc_props >> 20 & 1) != 0; } }
	public bool uses_tier3_cast_anim { get { return (misc_props >> 21 & 1) != 0; } }
	public bool destroys_user { get { return (misc_props >> 22 & 1) != 0; } }
	public bool misses_living_targets { get { return (misc_props >> 23 & 1) != 0; } }
	public bool anim_flag1 { get { return (misc_props >> 24 & 1) != 0; } }
	public bool anim_flag2 { get { return (misc_props >> 25 & 1) != 0; } }
	public bool anim_flag3 { get { return (misc_props >> 26 & 1) != 0; } }
	public bool anim_flag4 { get { return (misc_props >> 27 & 1) != 0; } }
	public bool anim_flag5 { get { return (misc_props >> 28 & 1) != 0; } }
	public bool anim_flag6 { get { return (misc_props >> 29 & 1) != 0; } }
	public bool anim_flag7 { get { return (misc_props >> 30 & 1) != 0; } }
	public bool anim_flag8 { get { return (misc_props >> 31 & 1) != 0; } }
	[FieldOffset(0x20)] public byte dmg_props;
	public bool is_physical { get { return (misc_props & 1) != 0; } }
	public bool is_magical { get { return (misc_props >> 1 & 1) != 0; } }
	public bool can_crit { get { return (misc_props >> 2 & 1) != 0; } }
	public bool gives_crit_bonus { get { return (misc_props >> 3 & 1) != 0; } }
	public bool is_healing { get { return (misc_props >> 4 & 1) != 0; } }
	public bool is_cleansing { get { return (misc_props >> 5 & 1) != 0; } }
	public bool supresses_bdl { get { return (misc_props >> 6 & 1) != 0; } }
	public bool innate_bdl { get { return (misc_props >> 7 & 1) != 0; } }
	[FieldOffset(0x21)] public bool steals_gil;
	[FieldOffset(0x22)] public bool has_party_preview;
	[FieldOffset(0x23)] public byte dmg_class;
	public bool damages_hp { get { return (dmg_class & 1) != 0; } }
	public bool damages_mp { get { return (dmg_class >> 1 & 1) != 0; } }
	public bool damages_ctb { get { return (dmg_class >> 2 & 1) != 0; } }
	[FieldOffset(0x24)] public byte ctb_rank;
	[FieldOffset(0x25)] public byte mp_cost;
	[FieldOffset(0x26)] public byte limit_cost;
	[FieldOffset(0x27)] public byte crit_bonus;
	[FieldOffset(0x28)] public byte dmg_formula;
	[FieldOffset(0x29)] public byte accuracy;
	[FieldOffset(0x2A)] public byte power;
	[FieldOffset(0x2B)] public byte hit_count;
	[FieldOffset(0x2C)] public byte shatter_chance;
	[FieldOffset(0x2D)] public byte elements;
	public bool firestrike { get { return (elements & 1) != 0; } }
	public bool icestrike { get { return (elements >> 1 & 1) != 0; } }
	public bool thunderstrike { get { return (elements >> 2 & 1) != 0; } }
	public bool waterstrike { get { return (elements >> 3 & 1) != 0; } }
	public bool holystrike { get { return (elements >> 4 & 1) != 0; } }
	[FieldOffset(0x2E)] public byte chance_death;
	[FieldOffset(0x2F)] public byte chance_zombie;
	[FieldOffset(0x30)] public byte chance_petrify;
	[FieldOffset(0x31)] public byte chance_poison;
	[FieldOffset(0x32)] public byte chance_power_break;
	[FieldOffset(0x33)] public byte chance_magic_break;
	[FieldOffset(0x34)] public byte chance_armor_break;
	[FieldOffset(0x35)] public byte chance_mental_break;
	[FieldOffset(0x36)] public byte chance_confuse;
	[FieldOffset(0x37)] public byte chance_berserk;
	[FieldOffset(0x38)] public byte chance_provoke;
	[FieldOffset(0x39)] public byte chance_threaten;
	[FieldOffset(0x3A)] public byte chance_sleep;
	[FieldOffset(0x3B)] public byte chance_silence;
	[FieldOffset(0x3C)] public byte chance_darkness;
	[FieldOffset(0x3D)] public byte chance_shell;
	[FieldOffset(0x3E)] public byte chance_protect;
	[FieldOffset(0x3F)] public byte chance_reflect;
	[FieldOffset(0x40)] public byte chance_nul_water;
	[FieldOffset(0x41)] public byte chance_nul_fire;
	[FieldOffset(0x42)] public byte chance_nul_thunder;
	[FieldOffset(0x43)] public byte chance_nul_ice;
	[FieldOffset(0x44)] public byte chance_regen;
	[FieldOffset(0x45)] public byte chance_haste;
	[FieldOffset(0x46)] public byte chance_slow;
	[FieldOffset(0x47)] public byte dur_sleep;
	[FieldOffset(0x48)] public byte dur_silence;
	[FieldOffset(0x49)] public byte dur_darkness;
	[FieldOffset(0x4A)] public byte dur_shell;
	[FieldOffset(0x4B)] public byte dur_protect;
	[FieldOffset(0x4C)] public byte dur_reflect;
	[FieldOffset(0x4D)] public byte dur_nul_water;
	[FieldOffset(0x4E)] public byte dur_nul_fire;
	[FieldOffset(0x4F)] public byte dur_nul_thunder;
	[FieldOffset(0x50)] public byte dur_nul_ice;
	[FieldOffset(0x51)] public byte dur_regen;
	[FieldOffset(0x52)] public byte dur_haste;
	[FieldOffset(0x53)] public byte dur_slow;
	[FieldOffset(0x54)] public ushort extra_status;
	public bool inflicts_scan { get { return (extra_status & 1) != 0; } }
	public bool inflicts_distill_power { get { return (extra_status >> 1 & 1) != 0; } }
	public bool inflicts_distill_mana { get { return (extra_status >> 2 & 1) != 0; } }
	public bool inflicts_distill_speed { get { return (extra_status >> 3 & 1) != 0; } }
	public bool inflicts_distill_move { get { return (extra_status >> 4 & 1) != 0; } }
	public bool inflicts_distill_ability { get { return (extra_status >> 5 & 1) != 0; } }
	public bool inflicts_shield { get { return (extra_status >> 6 & 1) != 0; } }
	public bool inflicts_boost { get { return (extra_status >> 7 & 1) != 0; } }
	public bool inflicts_eject { get { return (extra_status >> 8 & 1) != 0; } }
	public bool inflicts_auto_life { get { return (extra_status >> 9 & 1) != 0; } }
	public bool inflicts_curse { get { return (extra_status >> 10 & 1) != 0; } }
	public bool inflicts_defend { get { return (extra_status >> 11 & 1) != 0; } }
	public bool inflicts_guard { get { return (extra_status >> 12 & 1) != 0; } }
	public bool inflicts_sentinel { get { return (extra_status >> 13 & 1) != 0; } }
	public bool inflicts_doom { get { return (extra_status >> 14 & 1) != 0; } }
	[FieldOffset(0x56)] public byte stat_buffs;
	public bool inflicts_cheer { get { return (stat_buffs & 1) != 0; } }
	public bool inflicts_aim { get { return (stat_buffs >> 1 & 1) != 0; } }
	public bool inflicts_focus { get { return (stat_buffs >> 2 & 1) != 0; } }
	public bool inflicts_reflex { get { return (stat_buffs >> 3 & 1) != 0; } }
	public bool inflicts_luck { get { return (stat_buffs >> 4 & 1) != 0; } }
	public bool inflicts_jinx { get { return (stat_buffs >> 5 & 1) != 0; } }
	[FieldOffset(0x58)] public byte overdrive_category;
	[FieldOffset(0x59)] public byte stat_buffs_amount;
	[FieldOffset(0x5A)] public byte special_buffs;
	public bool inflicts_double_hp { get { return (special_buffs & 1) != 0; } }
	public bool inflicts_double_mp { get { return (special_buffs >> 1 & 1) != 0; } }
	public bool inflicts_spellspring { get { return (special_buffs >> 2 & 1) != 0; } }
	public bool inflicts_dmg_9999 { get { return (special_buffs >> 3 & 1) != 0; } }
	public bool inflicts_always_crit { get { return (special_buffs >> 4 & 1) != 0; } }
	public bool inflicts_overdrive_150 { get { return (special_buffs >> 5 & 1) != 0; } }
	public bool inflicts_overdrive_200 { get { return (special_buffs >> 6 & 1) != 0; } }
	[FieldOffset(0x5C)] public byte ordering_idx;
	[FieldOffset(0x5D)] public byte sphere_grid_role;
}