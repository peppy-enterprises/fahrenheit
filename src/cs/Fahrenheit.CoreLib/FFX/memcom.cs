namespace Fahrenheit.CoreLib.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x60)]
public struct MemCom {
	[FieldOffset(0x00)] public ushort name_offset;
	[FieldOffset(0x04)] public ushort dash_offset;
	[FieldOffset(0x08)] public ushort desc_offset;
	[FieldOffset(0x0C)] public ushort misc_offset;
	[FieldOffset(0x10)] public ushort anim1;
	[FieldOffset(0x12)] public ushort anim2;
	[FieldOffset(0x14)] public byte icon;
	[FieldOffset(0x15)] public byte caster_anim;
	[FieldOffset(0x16)] public byte menu_props;
	public bool top_level_in_menu	{ get => menu_props.get_bit(0); set => menu_props.set_bit(0, value); }
	public bool menu_flag4			{ get => menu_props.get_bit(3); set => menu_props.set_bit(3, value); }
	public bool opens_sub_menu		{ get => menu_props.get_bit(4); set => menu_props.set_bit(4, value); }

	[FieldOffset(0x17)] public byte sub_menu_cat2;
	[FieldOffset(0x18)] public byte sub_menu_cat;
	[FieldOffset(0x19)] public byte user_id;
	[FieldOffset(0x1A)] public byte target_props;
	public bool can_target			{ get => target_props.get_bit(0); set => target_props.set_bit(0, value); }
	public bool targets_enemies		{ get => target_props.get_bit(1); set => target_props.set_bit(1, value); }
	public bool targets_multiple	{ get => target_props.get_bit(2); set => target_props.set_bit(2, value); }
	public bool targets_self_only	{ get => target_props.get_bit(3); set => target_props.set_bit(3, value); }
	public bool targets_flag5		{ get => target_props.get_bit(4); set => target_props.set_bit(4, value); }
	public bool targets_team		{ get => target_props.get_bit(5); set => target_props.set_bit(5, value); }
	public bool targets_dead		{ get => target_props.get_bit(6); set => target_props.set_bit(6, value); }
	public bool targets_flag8		{ get => target_props.get_bit(7); set => target_props.set_bit(7, value); }

	[FieldOffset(0x1C)] public uint misc_props;
	public bool usable_outside_combat	{ get => misc_props.get_bit( 0); set => misc_props.set_bit( 0, value); }
	public bool usable_in_combat		{ get => misc_props.get_bit( 1); set => misc_props.set_bit( 1, value); }
	public bool display_move_name		{ get => misc_props.get_bit( 2); set => misc_props.set_bit( 2, value); }
	public uint accuracy_formula		{ get => misc_props.get_bits(3, 3); set => misc_props.set_bits(3, 3, value); }
	public bool affected_by_darkness	{ get => misc_props.get_bit( 6); set => misc_props.set_bit( 6, value); }
	public bool affected_by_reflect		{ get => misc_props.get_bit( 7); set => misc_props.set_bit( 7, value); }
	public bool absorbs_dmg				{ get => misc_props.get_bit( 8); set => misc_props.set_bit( 8, value); }
	public bool steals_item				{ get => misc_props.get_bit( 9); set => misc_props.set_bit( 9, value); }
	public bool in_use_menu				{ get => misc_props.get_bit(10); set => misc_props.set_bit(10, value); }
	public bool in_sub_menu				{ get => misc_props.get_bit(11); set => misc_props.set_bit(11, value); }
	public bool in_trigger_menu			{ get => misc_props.get_bit(12); set => misc_props.set_bit(12, value); }
	public bool inflicts_delay_weak		{ get => misc_props.get_bit(13); set => misc_props.set_bit(13, value); }
	public bool inflicts_delay_strong	{ get => misc_props.get_bit(14); set => misc_props.set_bit(14, value); }
	public bool targets_random			{ get => misc_props.get_bit(15); set => misc_props.set_bit(15, value); }
	public bool pierces					{ get => misc_props.get_bit(16); set => misc_props.set_bit(16, value); }
	public bool affected_by_silence		{ get => misc_props.get_bit(17); set => misc_props.set_bit(17, value); }
	public bool uses_weapon_props		{ get => misc_props.get_bit(18); set => misc_props.set_bit(18, value); }
	public bool trigger_command			{ get => misc_props.get_bit(19); set => misc_props.set_bit(19, value); }
	public bool uses_tier1_cast_anim	{ get => misc_props.get_bit(20); set => misc_props.set_bit(20, value); }
	public bool uses_tier3_cast_anim	{ get => misc_props.get_bit(21); set => misc_props.set_bit(21, value); }
	public bool destroys_user			{ get => misc_props.get_bit(22); set => misc_props.set_bit(22, value); }
	public bool misses_living_targets	{ get => misc_props.get_bit(23); set => misc_props.set_bit(23, value); }
	public bool anim_flag1				{ get => misc_props.get_bit(24); set => misc_props.set_bit(24, value); }
	public bool anim_flag2				{ get => misc_props.get_bit(25); set => misc_props.set_bit(25, value); }
	public bool anim_flag3				{ get => misc_props.get_bit(26); set => misc_props.set_bit(26, value); }
	public bool anim_flag4				{ get => misc_props.get_bit(27); set => misc_props.set_bit(27, value); }
	public bool anim_flag5				{ get => misc_props.get_bit(28); set => misc_props.set_bit(28, value); }
	public bool anim_flag6				{ get => misc_props.get_bit(29); set => misc_props.set_bit(29, value); }
	public bool anim_flag7				{ get => misc_props.get_bit(30); set => misc_props.set_bit(30, value); }
	public bool anim_flag8				{ get => misc_props.get_bit(31); set => misc_props.set_bit(31, value); }

	[FieldOffset(0x20)] public byte dmg_props;
	public bool is_physical			{ get => dmg_props.get_bit(0); set => dmg_props.set_bit(0, value); }
	public bool is_magical			{ get => dmg_props.get_bit(1); set => dmg_props.set_bit(1, value); }
	public bool can_crit			{ get => dmg_props.get_bit(2); set => dmg_props.set_bit(2, value); }
	public bool gives_crit_bonus	{ get => dmg_props.get_bit(3); set => dmg_props.set_bit(3, value); }
	public bool is_healing			{ get => dmg_props.get_bit(4); set => dmg_props.set_bit(4, value); }
	public bool is_cleansing		{ get => dmg_props.get_bit(5); set => dmg_props.set_bit(5, value); }
	public bool supresses_bdl		{ get => dmg_props.get_bit(6); set => dmg_props.set_bit(6, value); }
	public bool innate_bdl			{ get => dmg_props.get_bit(7); set => dmg_props.set_bit(7, value); }

	[FieldOffset(0x21)] public bool steals_gil;
	[FieldOffset(0x22)] public bool has_party_preview;
	[FieldOffset(0x23)] public byte dmg_class;
	public bool damages_hp	{ get => dmg_class.get_bit(0); set => dmg_props.set_bit(0, value); }
	public bool damages_mp	{ get => dmg_class.get_bit(1); set => dmg_props.set_bit(1, value); }
	public bool damages_ctb	{ get => dmg_class.get_bit(2); set => dmg_props.set_bit(2, value); }

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
	public bool firestrike		{ get => elements.get_bit(0); set => elements.set_bit(0, value); }
	public bool icestrike		{ get => elements.get_bit(1); set => elements.set_bit(1, value); }
	public bool thunderstrike	{ get => elements.get_bit(2); set => elements.set_bit(2, value); }
	public bool waterstrike		{ get => elements.get_bit(3); set => elements.set_bit(3, value); }
	public bool holystrike		{ get => elements.get_bit(4); set => elements.set_bit(4, value); }

	[FieldOffset(0x2E)] public Status status_inflict;
	[FieldOffset(0x47)] public StatusDur status_duration;
	[FieldOffset(0x54)] public ushort extra_status;
	public bool inflicts_scan				{ get => extra_status.get_bit(0); set => extra_status.set_bit(0, value); }
	public bool inflicts_distill_power		{ get => extra_status.get_bit(0); set => extra_status.set_bit(0, value); }
	public bool inflicts_distill_mana		{ get => extra_status.get_bit(0); set => extra_status.set_bit(0, value); }
	public bool inflicts_distill_speed		{ get => extra_status.get_bit(0); set => extra_status.set_bit(0, value); }
	public bool inflicts_distill_move		{ get => extra_status.get_bit(0); set => extra_status.set_bit(0, value); }
	public bool inflicts_distill_ability	{ get => extra_status.get_bit(0); set => extra_status.set_bit(0, value); }
	public bool inflicts_shield				{ get => extra_status.get_bit(0); set => extra_status.set_bit(0, value); }
	public bool inflicts_boost				{ get => extra_status.get_bit(0); set => extra_status.set_bit(0, value); }
	public bool inflicts_eject				{ get => extra_status.get_bit(0); set => extra_status.set_bit(0, value); }
	public bool inflicts_auto_life			{ get => extra_status.get_bit(0); set => extra_status.set_bit(0, value); }
	public bool inflicts_curse				{ get => extra_status.get_bit(0); set => extra_status.set_bit(0, value); }
	public bool inflicts_defend				{ get => extra_status.get_bit(0); set => extra_status.set_bit(0, value); }
	public bool inflicts_guard				{ get => extra_status.get_bit(0); set => extra_status.set_bit(0, value); }
	public bool inflicts_sentinel			{ get => extra_status.get_bit(0); set => extra_status.set_bit(0, value); }
	public bool inflicts_doom				{ get => extra_status.get_bit(0); set => extra_status.set_bit(0, value); }

	[FieldOffset(0x56)] public byte stat_buffs;
	public bool inflicts_cheer	{ get => stat_buffs.get_bit(0); set => stat_buffs.set_bit(0, value); }
	public bool inflicts_aim	{ get => stat_buffs.get_bit(1); set => stat_buffs.set_bit(1, value); }
	public bool inflicts_focus	{ get => stat_buffs.get_bit(2); set => stat_buffs.set_bit(2, value); }
	public bool inflicts_reflex	{ get => stat_buffs.get_bit(3); set => stat_buffs.set_bit(3, value); }
	public bool inflicts_luck	{ get => stat_buffs.get_bit(4); set => stat_buffs.set_bit(4, value); }
	public bool inflicts_jinx	{ get => stat_buffs.get_bit(5); set => stat_buffs.set_bit(5, value); }

	[FieldOffset(0x58)] public byte overdrive_category;
	[FieldOffset(0x59)] public byte stat_buffs_amount;
	[FieldOffset(0x5A)] public byte special_buffs;
	public bool inflicts_double_hp		{ get => special_buffs.get_bit(0); set => special_buffs.set_bit(0, value); }
	public bool inflicts_double_mp		{ get => special_buffs.get_bit(1); set => special_buffs.set_bit(1, value); }
	public bool inflicts_spellspring	{ get => special_buffs.get_bit(2); set => special_buffs.set_bit(2, value); }
	public bool inflicts_dmg_9999		{ get => special_buffs.get_bit(3); set => special_buffs.set_bit(3, value); }
	public bool inflicts_always_crit	{ get => special_buffs.get_bit(4); set => special_buffs.set_bit(4, value); }
	public bool inflicts_overdrive_150	{ get => special_buffs.get_bit(5); set => special_buffs.set_bit(5, value); }
	public bool inflicts_overdrive_200	{ get => special_buffs.get_bit(6); set => special_buffs.set_bit(6, value); }

	[FieldOffset(0x5C)] public byte ordering_idx;
	[FieldOffset(0x5D)] public byte sphere_grid_role;
}