namespace Fahrenheit.CoreLib;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0xF90)]
public unsafe struct FhXChr {
	[FieldOffset(0x0)] public FhXActor* actor;
	[FieldOffset(0x4)] public uint model_id;
	[FieldOffset(0xC)] public ushort id;
    [FieldOffset(0x16)] public byte stat_visible;
    [FieldOffset(0x17)] public byte stat_hide;
    [FieldOffset(0x18)] public sbyte stat_visible_eff;
    [FieldOffset(0x19)] public byte stat_visible_cam;
    [FieldOffset(0x1B)] public byte stat_visible_out;
    [FieldOffset(0x1C)] public sbyte stat_visible_out_on;
    [FieldOffset(0x21)] public bool stat_motion_dispose_flag;
    [FieldOffset(0x22)] public bool stat_model_dispose_flag;
    [FieldOffset(0x23)] public bool stat_fast_model_flag;
    [FieldOffset(0x24)] public byte stat_model;
    [FieldOffset(0x26)] public sbyte stat_shadow;
    [FieldOffset(0x2D)] public bool stat_bodyhit_flag;
    [FieldOffset(0x3B)] public byte stat_eternal_relife;
	[FieldOffset(0x3F4)] public byte death_level;
    [FieldOffset(0x3F5)] public byte death_pattern;
    [FieldOffset(0x3F8)] public bool stat_center_chr_flag;
    [FieldOffset(0x3F9)] public byte stat_death_return;
    [FieldOffset(0x400)] public byte motion_speed_normal;
    [FieldOffset(0x405)] public byte motion_speed_normal_init;
    [FieldOffset(0x408)] public byte stat_attack_motion_type;
    [FieldOffset(0x409)] public byte stat_return_motion_type;
    [FieldOffset(0x40A)] public bool stat_attack_return_flag;
    [FieldOffset(0x40C)] public byte stat_attack_normal_frame;
    [FieldOffset(0x40D)] public byte stat_attack_near_frame;
    [FieldOffset(0x40E)] public byte stat_attack_motion_frame;
    [FieldOffset(0x412)] public bool stat_move_flag;
    [FieldOffset(0x416)] public byte stat_move_target;
    [FieldOffset(0x41B)] public bool stat_direction_fix_flag;
    [FieldOffset(0x41C)] public bool stat_direction_change_flag;
    [FieldOffset(0x41D)] public byte stat_direction_change_effect;
    [FieldOffset(0x41E)] public bool stat_disable_move_flag;
    [FieldOffset(0x41F)] public bool stat_disable_jump_flag;
    [FieldOffset(0x420)] public byte stat_motionlv;
    [FieldOffset(0x422)] public byte stat_damage_chr;
    [FieldOffset(0x423)] public bool stat_appear_invisible_flag;
    [FieldOffset(0x424)] public byte stat_appear_count;
    [FieldOffset(0x425)] public bool stat_avoid_flag;
    [FieldOffset(0x426)] public bool stat_adjust_pos_flag;
    [FieldOffset(0x433)] public bool stat_hit_terminate_flag;
    [FieldOffset(0x437)] public bool stat_neck_target_flag;
	[FieldOffset(0x438)] public byte neck_target_id;
    [FieldOffset(0x43B)] public byte stat_win_pose;
    [FieldOffset(0x43C)] public sbyte stat_win_se;
    [FieldOffset(0x43D)] public bool stat_appear_motion_flag;
    [FieldOffset(0x43E)] public byte stat_live_motion;
    [FieldOffset(0x442)] public byte stat_num_print_element;
    [FieldOffset(0x446)] public byte stat_command_type;
    [FieldOffset(0x447)] public byte stat_inv_motion;
    public bool stat_inv_physic_motion	{ get => stat_inv_motion.get_bit(0); set => stat_inv_motion.set_bit(0, value); }
    public bool stat_inv_magic_motion	{ get => stat_inv_motion.get_bit(1); set => stat_inv_motion.set_bit(1, value); }

    [FieldOffset(0x448)] public bool stat_wait_motion_flag;
    [FieldOffset(0x44A)] public sbyte stat_near_motion;
    [FieldOffset(0x44B)] public sbyte stat_near_motion_set;
    [FieldOffset(0x44E)] public byte stat_dmg_dir;
    [FieldOffset(0x44F)] public byte stat_weak_motion;
    [FieldOffset(0x451)] public byte stat_magiclv;
    [FieldOffset(0x4B4)] public byte stat_own_attack_near;
    [FieldOffset(0x4B8)] public byte stat_idle2_prob;
    [FieldOffset(0x4B9)] public byte stat_attack_inc_speed;
    [FieldOffset(0x4BA)] public byte stat_attack_dec_speed;
    [FieldOffset(0x4BB)] public byte stat_motion_num;
    [FieldOffset(0x4FC)] public ushort area;
    [FieldOffset(0x4FE)] public byte pos;
    [FieldOffset(0x4FF)] public byte stat_far;
    [FieldOffset(0x504)] public byte stat_group;
    [FieldOffset(0x505)] public bool flying;
    [FieldOffset(0x508)] public byte stat_adjust_pos;
    [FieldOffset(0x509)] public byte stat_move_area;
    [FieldOffset(0x50B)] public byte stat_move_pos;
    [FieldOffset(0x50D)] public byte stat_height_on;
    [FieldOffset(0x50E)] public bool stat_sleep_recover_flag;
	[FieldOffset(0x540)] public fixed byte name[40];
    [FieldOffset(0x590)] public byte gender;
    public bool male { get => gender.get_bit(0); set => gender.set_bit(0, value); }
    public bool female { get => gender.get_bit(1); set => gender.set_bit(1, value); }
    public bool aeon { get => gender.get_bit(2); set => gender.set_bit(2, value); }

	[FieldOffset(0x592)] public byte wpn_inv_idx;
	[FieldOffset(0x593)] public byte arm_inv_idx;
	[FieldOffset(0x594)] public uint max_hp;
	[FieldOffset(0x598)] public uint max_mp;
	[FieldOffset(0x59C)] public uint base_max_hp;
	[FieldOffset(0x5A0)] public uint base_max_mp;
    [FieldOffset(0x5A4)] public uint overkill_threshold;
	[FieldOffset(0x5A8)] public byte strength;
	[FieldOffset(0x5A9)] public byte defense;
	[FieldOffset(0x5AA)] public byte magic;
	[FieldOffset(0x5AB)] public byte magic_defense;
	[FieldOffset(0x5AC)] public byte agility;
	[FieldOffset(0x5AD)] public byte luck;
	[FieldOffset(0x5AE)] public byte evasion;
	[FieldOffset(0x5AF)] public byte accuracy;
	[FieldOffset(0x5B0)] public byte strength_up;
	[FieldOffset(0x5B1)] public byte defense_up;
	[FieldOffset(0x5B2)] public byte magic_up;
	[FieldOffset(0x5B3)] public byte magic_defense_up;
	[FieldOffset(0x5B4)] public byte agility_up;
	[FieldOffset(0x5B5)] public byte luck_up;
	[FieldOffset(0x5B6)] public byte evasion_up;
	[FieldOffset(0x5B7)] public byte accuracy_up;
    [FieldOffset(0x5B8)] public ushort extra_resist; //TODO: Figure out a better name for this
	public bool armored					{ get => extra_resist.get_bit(0); set => extra_resist.set_bit(0, value); }
	public bool ignores_gravity_dmg		{ get => extra_resist.get_bit(1); set => extra_resist.set_bit(1, value); }
    public bool ignores_physical_dmg	{ get => extra_resist.get_bit(2); set => extra_resist.set_bit(2, value); }
    public bool ignores_magical_dmg		{ get => extra_resist.get_bit(3); set => extra_resist.set_bit(3, value); }
    public bool invincible				{ get => extra_resist.get_bit(4); set => extra_resist.set_bit(4, value); }
    public bool ignores_ctb_dmg			{ get => extra_resist.get_bit(5); set => extra_resist.set_bit(5, value); }
    public bool ignores_zanmato			{ get => extra_resist.get_bit(6); set => extra_resist.set_bit(6, value); }
    public bool ignores_bribe			{ get => extra_resist.get_bit(7); set => extra_resist.set_bit(7, value); }

    [FieldOffset(0x5BA)] public byte poison_dmg;
    [FieldOffset(0x5BB)] public byte ovr_mode_selected;
    [FieldOffset(0x5BC)] public byte ovr_charge;
    [FieldOffset(0x5BD)] public byte ovr_charge_max;
	[FieldOffset(0x5C1)] public byte wpn_dmg_formula;
    [FieldOffset(0x5C3)] public byte stat_icon_number;
    [FieldOffset(0x5C4)] public byte provoked_by_id;
	[FieldOffset(0x5C5)] public byte threatened_by_id;
	[FieldOffset(0x5C7)] public byte wpn_power;
    [FieldOffset(0x5C8)] public byte doom_counter;
    [FieldOffset(0x5C9)] public byte doom_counter_init;
    [FieldOffset(0x5CB)] public bool stat_prov_command_flag;
	[FieldOffset(0x5D0)] public uint hp;
	[FieldOffset(0x5D4)] public uint mp;
	[FieldOffset(0x5D8)] public byte wpn_crit_bonus;
	[FieldOffset(0x5D9)] public byte wpn_elem;
	public bool has_firestrike		{ get => wpn_elem.get_bit(0); set => wpn_elem.set_bit(0, value); }
	public bool has_icestrike		{ get => wpn_elem.get_bit(1); set => wpn_elem.set_bit(1, value); }
	public bool has_thunderstrike	{ get => wpn_elem.get_bit(2); set => wpn_elem.set_bit(2, value); }
	public bool has_waterstrike		{ get => wpn_elem.get_bit(3); set => wpn_elem.set_bit(3, value); }
	public bool has_holystrike		{ get => wpn_elem.get_bit(4); set => wpn_elem.set_bit(4, value); }

    [FieldOffset(0x5DA)] public byte elem_absorb;
    public bool absorbs_fire	{ get => elem_absorb.get_bit(0); set => elem_absorb.set_bit(0, value); }
    public bool absorbs_ice		{ get => elem_absorb.get_bit(1); set => elem_absorb.set_bit(1, value); }
    public bool absorbs_thunder	{ get => elem_absorb.get_bit(2); set => elem_absorb.set_bit(2, value); }
    public bool absorbs_water	{ get => elem_absorb.get_bit(3); set => elem_absorb.set_bit(3, value); }
    public bool absorbs_holy	{ get => elem_absorb.get_bit(4); set => elem_absorb.set_bit(4, value); }

    [FieldOffset(0x5DB)] public byte elem_ignore;
    public bool ignores_fire	{ get => elem_ignore.get_bit(0); set => elem_ignore.set_bit(0, value); }
    public bool ignores_ice		{ get => elem_ignore.get_bit(1); set => elem_ignore.set_bit(1, value); }
    public bool ignores_thunder	{ get => elem_ignore.get_bit(2); set => elem_ignore.set_bit(2, value); }
    public bool ignores_water	{ get => elem_ignore.get_bit(3); set => elem_ignore.set_bit(3, value); }
    public bool ignores_holy	{ get => elem_ignore.get_bit(4); set => elem_ignore.set_bit(4, value); }

    [FieldOffset(0x5DC)] public byte elem_resist;
    public bool resists_fire	{ get => elem_resist.get_bit(0); set => elem_resist.set_bit(0, value); }
    public bool resists_ice		{ get => elem_resist.get_bit(1); set => elem_resist.set_bit(1, value); }
    public bool resists_thunder	{ get => elem_resist.get_bit(2); set => elem_resist.set_bit(2, value); }
    public bool resists_water	{ get => elem_resist.get_bit(3); set => elem_resist.set_bit(3, value); }
    public bool resists_holy	{ get => elem_resist.get_bit(4); set => elem_resist.set_bit(4, value); }

	[FieldOffset(0x5DD)] public byte elem_weak;
    public bool weak_fire		{ get => elem_weak.get_bit(0); set => elem_weak.set_bit(0, value); }
    public bool weak_ice		{ get => elem_weak.get_bit(1); set => elem_weak.set_bit(1, value); }
    public bool weak_thunder	{ get => elem_weak.get_bit(2); set => elem_weak.set_bit(2, value); }
    public bool weak_water		{ get => elem_weak.get_bit(3); set => elem_weak.set_bit(3, value); }
    public bool weak_holy		{ get => elem_weak.get_bit(4); set => elem_weak.set_bit(4, value); }

	[FieldOffset(0x5DE)] public FhXStatus status_inflict;
	[FieldOffset(0x5F7)] public FhXStatusDur status_duration_inflict;
	[FieldOffset(0x604)] public ushort extra_status_inflict;
	public bool inflicts_scan				{ get => extra_status_inflict.get_bit(0); set => extra_status_inflict.set_bit(0, value); }
	public bool inflicts_distill_power		{ get => extra_status_inflict.get_bit(1); set => extra_status_inflict.set_bit(1, value); }
	public bool inflicts_distill_mana		{ get => extra_status_inflict.get_bit(2); set => extra_status_inflict.set_bit(2, value); }
	public bool inflicts_distill_speed		{ get => extra_status_inflict.get_bit(3); set => extra_status_inflict.set_bit(3, value); }
	public bool inflicts_distill_move		{ get => extra_status_inflict.get_bit(4); set => extra_status_inflict.set_bit(4, value); }
	public bool inflicts_distill_ability	{ get => extra_status_inflict.get_bit(5); set => extra_status_inflict.set_bit(5, value); }
	public bool inflicts_shield				{ get => extra_status_inflict.get_bit(6); set => extra_status_inflict.set_bit(6, value); }
	public bool inflicts_boost				{ get => extra_status_inflict.get_bit(7); set => extra_status_inflict.set_bit(7, value); }
	public bool inflicts_eject				{ get => extra_status_inflict.get_bit(8); set => extra_status_inflict.set_bit(8, value); }
	public bool inflicts_auto_life			{ get => extra_status_inflict.get_bit(9); set => extra_status_inflict.set_bit(9, value); }
	public bool inflicts_curse				{ get => extra_status_inflict.get_bit(10); set => extra_status_inflict.set_bit(10, value); }
	public bool inflicts_defend				{ get => extra_status_inflict.get_bit(11); set => extra_status_inflict.set_bit(11, value); }
	public bool inflicts_guard				{ get => extra_status_inflict.get_bit(12); set => extra_status_inflict.set_bit(12, value); }
	public bool inflicts_sentinel			{ get => extra_status_inflict.get_bit(13); set => extra_status_inflict.set_bit(13, value); }
	public bool inflicts_doom				{ get => extra_status_inflict.get_bit(14); set => extra_status_inflict.set_bit(14, value); }

    [FieldOffset(0x606)] public ushort status_suffer;
	public bool suffers_ko				{ get => status_suffer.get_bit(0); set => status_suffer.set_bit(0, value); }
    public bool suffers_zombie			{ get => status_suffer.get_bit(1); set => status_suffer.set_bit(1, value); }
	public bool suffers_petrification	{ get => status_suffer.get_bit(2); set => status_suffer.set_bit(2, value); }
    public bool suffers_poison			{ get => status_suffer.get_bit(3); set => status_suffer.set_bit(3, value); }
    public bool suffers_power_break		{ get => status_suffer.get_bit(4); set => status_suffer.set_bit(4, value); }
    public bool suffers_magic_break		{ get => status_suffer.get_bit(5); set => status_suffer.set_bit(5, value); }
    public bool suffers_armor_break		{ get => status_suffer.get_bit(6); set => status_suffer.set_bit(6, value); }
    public bool suffers_mental_break	{ get => status_suffer.get_bit(7); set => status_suffer.set_bit(7, value); }
    public bool suffers_confusion		{ get => status_suffer.get_bit(8); set => status_suffer.set_bit(8, value); }
    public bool suffers_berserk			{ get => status_suffer.get_bit(9); set => status_suffer.set_bit(9, value); }
    public bool suffers_provoke			{ get => status_suffer.get_bit(10); set => status_suffer.set_bit(10, value); }
    public bool suffers_threaten		{ get => status_suffer.get_bit(11); set => status_suffer.set_bit(11, value); }

    [FieldOffset(0x608)] public FhXStatusDur status_suffer_turns_left;
    [FieldOffset(0x616)] public ushort extra_status_suffer;
	public bool suffers_scan				{ get => extra_status_suffer.get_bit(0); set => extra_status_suffer.set_bit(0, value); }
	public bool suffers_distill_power		{ get => extra_status_suffer.get_bit(1); set => extra_status_suffer.set_bit(1, value); }
	public bool suffers_distill_mana		{ get => extra_status_suffer.get_bit(2); set => extra_status_suffer.set_bit(2, value); }
	public bool suffers_distill_speed		{ get => extra_status_suffer.get_bit(3); set => extra_status_suffer.set_bit(3, value); }
	public bool suffers_distill_move		{ get => extra_status_suffer.get_bit(4); set => extra_status_suffer.set_bit(4, value); }
	public bool suffers_distill_ability		{ get => extra_status_suffer.get_bit(5); set => extra_status_suffer.set_bit(5, value); }
	public bool suffers_shield				{ get => extra_status_suffer.get_bit(6); set => extra_status_suffer.set_bit(6, value); }
	public bool suffers_boost				{ get => extra_status_suffer.get_bit(7); set => extra_status_suffer.set_bit(7, value); }
	public bool suffers_eject				{ get => extra_status_suffer.get_bit(8); set => extra_status_suffer.set_bit(8, value); }
	public bool suffers_auto_life			{ get => extra_status_suffer.get_bit(9); set => extra_status_suffer.set_bit(9, value); }
	public bool suffers_curse				{ get => extra_status_suffer.get_bit(10); set => extra_status_suffer.set_bit(10, value); }
	public bool suffers_defend				{ get => extra_status_suffer.get_bit(11); set => extra_status_suffer.set_bit(11, value); }
	public bool suffers_guard				{ get => extra_status_suffer.get_bit(12); set => extra_status_suffer.set_bit(12, value); }
	public bool suffers_sentinel			{ get => extra_status_suffer.get_bit(13); set => extra_status_suffer.set_bit(13, value); }
	public bool suffers_doom				{ get => extra_status_suffer.get_bit(14); set => extra_status_suffer.set_bit(14, value); }

	[FieldOffset(0x62A)] public ushort full_auto_status1;
	[FieldOffset(0x62C)] public ushort full_auto_status2;
	[FieldOffset(0x62E)] public ushort full_auto_status3;
	public bool has_auto_death			{ get => full_auto_status1.get_bit(0); set => full_auto_status1.set_bit(0, value); }
	public bool has_auto_zombie			{ get => full_auto_status1.get_bit(1); set => full_auto_status1.set_bit(1, value); }
	public bool has_auto_petrification	{ get => full_auto_status1.get_bit(2); set => full_auto_status1.set_bit(2, value); }
	public bool has_auto_poison			{ get => full_auto_status1.get_bit(3); set => full_auto_status1.set_bit(3, value); }
	public bool has_auto_power_break	{ get => full_auto_status1.get_bit(4); set => full_auto_status1.set_bit(4, value); }
	public bool has_auto_magic_break	{ get => full_auto_status1.get_bit(5); set => full_auto_status1.set_bit(5, value); }
	public bool has_auto_armor_break	{ get => full_auto_status1.get_bit(6); set => full_auto_status1.set_bit(6, value); }
	public bool has_auto_mental_break	{ get => full_auto_status1.get_bit(7); set => full_auto_status1.set_bit(7, value); }
	public bool has_auto_confuse		{ get => full_auto_status1.get_bit(8); set => full_auto_status1.set_bit(8, value); }
	public bool has_auto_berserk		{ get => full_auto_status1.get_bit(9); set => full_auto_status1.set_bit(9, value); }
	public bool has_auto_provoke		{ get => full_auto_status1.get_bit(10); set => full_auto_status1.set_bit(10, value); }
	public bool has_auto_threaten		{ get => full_auto_status1.get_bit(11); set => full_auto_status1.set_bit(11, value); }
	public bool has_auto_sleep			{ get => full_auto_status1.get_bit(12); set => full_auto_status1.set_bit(12, value); }
	public bool has_auto_silence		{ get => full_auto_status1.get_bit(13); set => full_auto_status1.set_bit(13, value); }
	public bool has_auto_darkness		{ get => full_auto_status1.get_bit(14); set => full_auto_status1.set_bit(14, value); }
	public bool has_auto_shell			{ get => full_auto_status1.get_bit(15); set => full_auto_status1.set_bit(15, value); }

	public bool has_auto_protect		{ get => full_auto_status2.get_bit(0); set => full_auto_status2.set_bit(0, value); }
	public bool has_auto_reflect		{ get => full_auto_status2.get_bit(1); set => full_auto_status2.set_bit(1, value); }
	public bool has_auto_nul_water		{ get => full_auto_status2.get_bit(2); set => full_auto_status2.set_bit(2, value); }
	public bool has_auto_nul_fire		{ get => full_auto_status2.get_bit(3); set => full_auto_status2.set_bit(3, value); }
	public bool has_auto_nul_thunder	{ get => full_auto_status2.get_bit(4); set => full_auto_status2.set_bit(4, value); }
	public bool has_auto_nul_ice		{ get => full_auto_status2.get_bit(5); set => full_auto_status2.set_bit(5, value); }
	public bool has_auto_regen			{ get => full_auto_status2.get_bit(6); set => full_auto_status2.set_bit(6, value); }
	public bool has_auto_haste			{ get => full_auto_status2.get_bit(7); set => full_auto_status2.set_bit(7, value); }
	public bool has_auto_slow			{ get => full_auto_status2.get_bit(8); set => full_auto_status2.set_bit(8, value); }

	[FieldOffset(0x630)] public ushort always_auto_status1;
	[FieldOffset(0x632)] public ushort always_auto_status2;
	[FieldOffset(0x634)] public ushort always_auto_status3;
	public bool has_always_auto_death			{ get => always_auto_status1.get_bit(0); set => always_auto_status1.set_bit(0, value); }
	public bool has_always_auto_zombie			{ get => always_auto_status1.get_bit(1); set => always_auto_status1.set_bit(1, value); }
	public bool has_always_auto_petrification	{ get => always_auto_status1.get_bit(2); set => always_auto_status1.set_bit(2, value); }
	public bool has_always_auto_poison			{ get => always_auto_status1.get_bit(3); set => always_auto_status1.set_bit(3, value); }
	public bool has_always_auto_power_break		{ get => always_auto_status1.get_bit(4); set => always_auto_status1.set_bit(4, value); }
	public bool has_always_auto_magic_break		{ get => always_auto_status1.get_bit(5); set => always_auto_status1.set_bit(5, value); }
	public bool has_always_auto_armor_break		{ get => always_auto_status1.get_bit(6); set => always_auto_status1.set_bit(6, value); }
	public bool has_always_auto_mental_break	{ get => always_auto_status1.get_bit(7); set => always_auto_status1.set_bit(7, value); }
	public bool has_always_auto_confuse			{ get => always_auto_status1.get_bit(8); set => always_auto_status1.set_bit(8, value); }
	public bool has_always_auto_berserk			{ get => always_auto_status1.get_bit(9); set => always_auto_status1.set_bit(9, value); }
	public bool has_always_auto_provoke			{ get => always_auto_status1.get_bit(10); set => always_auto_status1.set_bit(10, value); }
	public bool has_always_auto_threaten		{ get => always_auto_status1.get_bit(11); set => always_auto_status1.set_bit(11, value); }
	public bool has_always_auto_sleep			{ get => always_auto_status1.get_bit(12); set => always_auto_status1.set_bit(12, value); }
	public bool has_always_auto_silence			{ get => always_auto_status1.get_bit(13); set => always_auto_status1.set_bit(13, value); }
	public bool has_always_auto_darkness		{ get => always_auto_status1.get_bit(14); set => always_auto_status1.set_bit(14, value); }
	public bool has_always_auto_shell			{ get => always_auto_status1.get_bit(15); set => always_auto_status1.set_bit(15, value); }

	public bool has_always_auto_protect			{ get => always_auto_status2.get_bit(0); set => always_auto_status2.set_bit(0, value); }
	public bool has_always_auto_reflect			{ get => always_auto_status2.get_bit(1); set => always_auto_status2.set_bit(1, value); }
	public bool has_always_auto_nul_water		{ get => always_auto_status2.get_bit(2); set => always_auto_status2.set_bit(2, value); }
	public bool has_always_auto_nul_fire		{ get => always_auto_status2.get_bit(3); set => always_auto_status2.set_bit(3, value); }
	public bool has_always_auto_nul_thunder		{ get => always_auto_status2.get_bit(4); set => always_auto_status2.set_bit(4, value); }
	public bool has_always_auto_nul_ice			{ get => always_auto_status2.get_bit(5); set => always_auto_status2.set_bit(5, value); }
	public bool has_always_auto_regen			{ get => always_auto_status2.get_bit(6); set => always_auto_status2.set_bit(6, value); }
	public bool has_always_auto_haste			{ get => always_auto_status2.get_bit(7); set => always_auto_status2.set_bit(7, value); }
	public bool has_always_auto_slow			{ get => always_auto_status2.get_bit(8); set => always_auto_status2.set_bit(8, value); }

	[FieldOffset(0x636)] public ushort sos_auto_status1;
	[FieldOffset(0x638)] public ushort sos_auto_status2;
	[FieldOffset(0x63A)] public ushort sos_auto_status3;
	public bool has_sos_auto_death			{ get => sos_auto_status1.get_bit(0); set => sos_auto_status1.set_bit(0, value); }
	public bool has_sos_auto_zombie			{ get => sos_auto_status1.get_bit(1); set => sos_auto_status1.set_bit(1, value); }
	public bool has_sos_auto_petrification	{ get => sos_auto_status1.get_bit(2); set => sos_auto_status1.set_bit(2, value); }
	public bool has_sos_auto_poison			{ get => sos_auto_status1.get_bit(3); set => sos_auto_status1.set_bit(3, value); }
	public bool has_sos_auto_power_break	{ get => sos_auto_status1.get_bit(4); set => sos_auto_status1.set_bit(4, value); }
	public bool has_sos_auto_magic_break	{ get => sos_auto_status1.get_bit(5); set => sos_auto_status1.set_bit(5, value); }
	public bool has_sos_auto_armor_break	{ get => sos_auto_status1.get_bit(6); set => sos_auto_status1.set_bit(6, value); }
	public bool has_sos_auto_mental_break	{ get => sos_auto_status1.get_bit(7); set => sos_auto_status1.set_bit(7, value); }
	public bool has_sos_auto_confuse		{ get => sos_auto_status1.get_bit(8); set => sos_auto_status1.set_bit(8, value); }
	public bool has_sos_auto_berserk		{ get => sos_auto_status1.get_bit(9); set => sos_auto_status1.set_bit(9, value); }
	public bool has_sos_auto_provoke		{ get => sos_auto_status1.get_bit(10); set => sos_auto_status1.set_bit(10, value); }
	public bool has_sos_auto_threaten		{ get => sos_auto_status1.get_bit(11); set => sos_auto_status1.set_bit(11, value); }
	public bool has_sos_auto_sleep			{ get => sos_auto_status1.get_bit(12); set => sos_auto_status1.set_bit(12, value); }
	public bool has_sos_auto_silence		{ get => sos_auto_status1.get_bit(13); set => sos_auto_status1.set_bit(13, value); }
	public bool has_sos_auto_darkness		{ get => sos_auto_status1.get_bit(14); set => sos_auto_status1.set_bit(14, value); }
	public bool has_sos_auto_shell			{ get => sos_auto_status1.get_bit(15); set => sos_auto_status1.set_bit(15, value); }

	public bool has_sos_auto_protect		{ get => sos_auto_status2.get_bit(0); set => sos_auto_status2.set_bit(0, value); }
	public bool has_sos_auto_reflect		{ get => sos_auto_status2.get_bit(1); set => sos_auto_status2.set_bit(1, value); }
	public bool has_sos_auto_nul_water		{ get => sos_auto_status2.get_bit(2); set => sos_auto_status2.set_bit(2, value); }
	public bool has_sos_auto_nul_fire		{ get => sos_auto_status2.get_bit(3); set => sos_auto_status2.set_bit(3, value); }
	public bool has_sos_auto_nul_thunder	{ get => sos_auto_status2.get_bit(4); set => sos_auto_status2.set_bit(4, value); }
	public bool has_sos_auto_nul_ice		{ get => sos_auto_status2.get_bit(5); set => sos_auto_status2.set_bit(5, value); }
	public bool has_sos_auto_regen			{ get => sos_auto_status2.get_bit(6); set => sos_auto_status2.set_bit(6, value); }
	public bool has_sos_auto_haste			{ get => sos_auto_status2.get_bit(7); set => sos_auto_status2.set_bit(7, value); }
	public bool has_sos_auto_slow			{ get => sos_auto_status2.get_bit(8); set => sos_auto_status2.set_bit(8, value); }

    [FieldOffset(0x63D)] public byte weak_level_full;
	[FieldOffset(0x63E)] public byte weak_level_hp;
    [FieldOffset(0x641)] public FhXStatus status_resist;

	[FieldOffset(0x65A)] public ushort extra_status_resist;
	public bool resists_scan				{ get => extra_status_resist.get_bit(0); set => extra_status_resist.set_bit(0, value); }
	public bool resists_distill_power		{ get => extra_status_resist.get_bit(1); set => extra_status_resist.set_bit(1, value); }
	public bool resists_distill_mana		{ get => extra_status_resist.get_bit(2); set => extra_status_resist.set_bit(2, value); }
	public bool resists_distill_speed		{ get => extra_status_resist.get_bit(3); set => extra_status_resist.set_bit(3, value); }
	public bool resists_distill_move		{ get => extra_status_resist.get_bit(4); set => extra_status_resist.set_bit(4, value); }
	public bool resists_distill_ability		{ get => extra_status_resist.get_bit(5); set => extra_status_resist.set_bit(5, value); }
	public bool resists_shield				{ get => extra_status_resist.get_bit(6); set => extra_status_resist.set_bit(6, value); }
	public bool resists_boost				{ get => extra_status_resist.get_bit(7); set => extra_status_resist.set_bit(7, value); }
	public bool resists_eject				{ get => extra_status_resist.get_bit(8); set => extra_status_resist.set_bit(8, value); }
	public bool resists_auto_life			{ get => extra_status_resist.get_bit(9); set => extra_status_resist.set_bit(9, value); }
	public bool resists_curse				{ get => extra_status_resist.get_bit(10); set => extra_status_resist.set_bit(10, value); }
	public bool resists_defend				{ get => extra_status_resist.get_bit(11); set => extra_status_resist.set_bit(11, value); }
	public bool resists_guard				{ get => extra_status_resist.get_bit(12); set => extra_status_resist.set_bit(12, value); }
	public bool resists_sentinel			{ get => extra_status_resist.get_bit(13); set => extra_status_resist.set_bit(13, value); }
	public bool resists_doom				{ get => extra_status_resist.get_bit(14); set => extra_status_resist.set_bit(14, value); }

    [FieldOffset(0x65C)] public byte ctb;
	[FieldOffset(0x65D)] public uint max_ctb;
	[FieldOffset(0x65E)] public byte cheer_stacks;
	[FieldOffset(0x65F)] public byte aim_stacks;
	[FieldOffset(0x660)] public byte focus_stacks;
	[FieldOffset(0x661)] public byte reflex_stacks;
	[FieldOffset(0x662)] public byte luck_stacks;
	[FieldOffset(0x663)] public byte jinx_stacks;
	[FieldOffset(0x664)] public FhXAbiMap abilities;
	[FieldOffset(0x6BC)] public ushort auto_abilities1;
	[FieldOffset(0x6BE)] public ushort auto_abilities2;
	[FieldOffset(0x6C0)] public ushort auto_abilities3;
	public bool has_sensor				{ get => auto_abilities1.get_bit(0); set => auto_abilities1.set_bit(0, value); }
	public bool has_first_strike		{ get => auto_abilities1.get_bit(1); set => auto_abilities1.set_bit(1, value); }
	public bool has_initiative			{ get => auto_abilities1.get_bit(2); set => auto_abilities1.set_bit(2, value); }
	public bool has_counter_attack		{ get => auto_abilities1.get_bit(3); set => auto_abilities1.set_bit(3, value); }
	public bool has_evade_and_counter	{ get => auto_abilities1.get_bit(4); set => auto_abilities1.set_bit(4, value); }
	public bool has_magic_counter		{ get => auto_abilities1.get_bit(5); set => auto_abilities1.set_bit(5, value); }
	public bool has_magic_booster		{ get => auto_abilities1.get_bit(6); set => auto_abilities1.set_bit(6, value); }
	public bool has_alchemy				{ get => auto_abilities1.get_bit(9); set => auto_abilities1.set_bit(9, value); }
	public bool has_auto_potion			{ get => auto_abilities1.get_bit(10); set => auto_abilities1.set_bit(10, value); }
	public bool has_auto_med			{ get => auto_abilities1.get_bit(11); set => auto_abilities1.set_bit(11, value); }
	public bool has_auto_phoenix		{ get => auto_abilities1.get_bit(12); set => auto_abilities1.set_bit(12, value); }
	public bool has_piercing			{ get => auto_abilities1.get_bit(13); set => auto_abilities1.set_bit(13, value); }
	public bool has_half_mp_cost		{ get => auto_abilities1.get_bit(14); set => auto_abilities1.set_bit(14, value); }
	public bool has_one_mp_cost			{ get => auto_abilities1.get_bit(15); set => auto_abilities1.set_bit(15, value); }

	public bool has_double_overdrive	{ get => auto_abilities2.get_bit(0); set => auto_abilities2.set_bit(0, value); }
	public bool has_triple_overdrive	{ get => auto_abilities2.get_bit(1); set => auto_abilities2.set_bit(1, value); }
	public bool has_sos_overdrive		{ get => auto_abilities2.get_bit(2); set => auto_abilities2.set_bit(2, value); }
	public bool has_overdrive_to_ap		{ get => auto_abilities2.get_bit(3); set => auto_abilities2.set_bit(3, value); }
	public bool has_double_ap			{ get => auto_abilities2.get_bit(4); set => auto_abilities2.set_bit(4, value); }
	public bool has_triple_ap			{ get => auto_abilities2.get_bit(5); set => auto_abilities2.set_bit(5, value); }
	public bool has_no_ap				{ get => auto_abilities2.get_bit(6); set => auto_abilities2.set_bit(6, value); }
	public bool has_pickpocket			{ get => auto_abilities2.get_bit(7); set => auto_abilities2.set_bit(7, value); }
	public bool has_master_thief		{ get => auto_abilities2.get_bit(8); set => auto_abilities2.set_bit(8, value); }
	public bool has_break_hp_limit		{ get => auto_abilities2.get_bit(9); set => auto_abilities2.set_bit(9, value); }
	public bool has_break_mp_limit		{ get => auto_abilities2.get_bit(10); set => auto_abilities2.set_bit(10, value); }
	public bool has_break_damage_limit	{ get => auto_abilities2.get_bit(11); set => auto_abilities2.set_bit(11, value); }
	public bool has_gillionaire			{ get => auto_abilities2.get_bit(14); set => auto_abilities2.set_bit(14, value); }
	public bool has_hp_stroll			{ get => auto_abilities2.get_bit(15); set => auto_abilities2.set_bit(15, value); }

	public bool has_mp_stroll			{ get => auto_abilities3.get_bit(0); set => auto_abilities3.set_bit(0, value); }
	public bool has_no_encounters		{ get => auto_abilities3.get_bit(1); set => auto_abilities3.set_bit(1, value); }
	public bool has_capture				{ get => auto_abilities3.get_bit(2); set => auto_abilities3.set_bit(2, value); }

    [FieldOffset(0x6CE)] public byte stat_use_mp0;
    [FieldOffset(0x6D1)] public byte summoned_by_id;
    [FieldOffset(0x6D2)] public byte regen_strength;
    [FieldOffset(0x6DB)] public byte stat_attack_num;
    [FieldOffset(0x6DF)] public byte stat_command_exe_count;
    [FieldOffset(0x6E0)] public byte stat_consent;
    [FieldOffset(0x6E1)] public byte stat_energy;
	[FieldOffset(0x6E4)] public int current_hp;
	[FieldOffset(0x6E8)] public int current_mp;
	[FieldOffset(0x6EC)] public int current_ctb;
    [FieldOffset(0x700)] public sbyte stat_death_status;
	[FieldOffset(0x704)] public uint bribe_gil_spent;
    [FieldOffset(0x718)] public bool stat_limit_bar_flag;
    [FieldOffset(0x719)] public byte stat_limit_bar_flag_cam;
    [FieldOffset(0xD34)] public int damage_hp;
    [FieldOffset(0xD38)] public int damage_mp;
    [FieldOffset(0xD3C)] public int damage_ctb;
    [FieldOffset(0xDC8)] public byte in_battle;
    [FieldOffset(0xDCA)] public byte stat_target_list;
    [FieldOffset(0xDCC)] public byte stat_death;
    [FieldOffset(0xDCD)] public bool stat_escape_flag;
    [FieldOffset(0xDCE)] public byte stat_stone;
    [FieldOffset(0xDD2)] public bool stat_exist_flag;
    [FieldOffset(0xDD6)] public byte stat_action;
    [FieldOffset(0xDD7)] public bool in_ctb_list;
	[FieldOffset(0xDD8)] public bool in_hp_list;
    [FieldOffset(0xDD9)] public byte stat_cursor;
    [FieldOffset(0xDDA)] public bool stat_effect_target_flag;
    [FieldOffset(0xDDB)] public bool stat_regen_damage_flag;
    [FieldOffset(0xDDD)] public byte stat_event_chr;
    [FieldOffset(0xDDE)] public bool stat_blow_exist_flag;
    [FieldOffset(0xDEB)] public byte steal_count;
    [FieldOffset(0xDEC)] public byte stat_will_die;
    [FieldOffset(0xDF9)] public byte stat_cursor_element;
    [FieldOffset(0xE0C)] public byte stat_effvar;
    [FieldOffset(0xE0E)] public byte stat_effect_hit_num;
    [FieldOffset(0xE10)] public byte stat_magic_effect_ground;
    [FieldOffset(0xE11)] public byte stat_magic_effect_water;
    [FieldOffset(0xE1A)] public short stat_sound_hit_num;
    [FieldOffset(0xF74)] public byte stat_info_mes_id;
    [FieldOffset(0xF75)] public byte stat_live_mes_id;
	[FieldOffset(0xF78)] public nint script_chunks_ptr;
	[FieldOffset(0xF7C)] public nint script_data_ptr;
	[FieldOffset(0xF80)] public nint base_stats_ptr;
	[FieldOffset(0xF84)] public nint base_en_stats_ptr;
    [FieldOffset(0xF88)] public FhXChrLoot* loot;
}

