namespace Fahrenheit.CoreLib.FFX.Battle;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0xF90)]
public unsafe struct Chr {
    [FieldOffset(0x0)]   public       Actor*            actor;
    [FieldOffset(0x4)]   public       uint              model_id;
    [FieldOffset(0xC)]   public       ushort            id;
    [FieldOffset(0x16)]  public       byte              stat_visible;
    [FieldOffset(0x17)]  public       byte              stat_hide;
    [FieldOffset(0x18)]  public       sbyte             stat_visible_eff;
    [FieldOffset(0x19)]  public       byte              stat_visible_cam;
    [FieldOffset(0x1B)]  public       byte              stat_visible_out;
    [FieldOffset(0x1C)]  public       sbyte             stat_visible_out_on;
    [FieldOffset(0x21)]  public       bool              stat_motion_dispose_flag;
    [FieldOffset(0x22)]  public       bool              stat_model_dispose_flag;
    [FieldOffset(0x23)]  public       bool              stat_fast_model_flag;
    [FieldOffset(0x24)]  public       byte              stat_model;
    [FieldOffset(0x26)]  public       sbyte             stat_shadow;
    [FieldOffset(0x2D)]  public       bool              stat_bodyhit_flag;
    [FieldOffset(0x3B)]  public       byte              stat_eternal_relife;
    [FieldOffset(0x3F4)] public       byte              death_level;
    [FieldOffset(0x3F5)] public       byte              death_pattern;
    [FieldOffset(0x3F8)] public       bool              stat_center_chr_flag;
    [FieldOffset(0x3F9)] public       byte              stat_death_return;
    [FieldOffset(0x400)] public       byte              motion_speed_normal;
    [FieldOffset(0x405)] public       byte              motion_speed_normal_init;
    [FieldOffset(0x408)] public       byte              stat_attack_motion_type;
    [FieldOffset(0x409)] public       byte              stat_return_motion_type;
    [FieldOffset(0x40A)] public       bool              stat_attack_return_flag;
    [FieldOffset(0x40C)] public       byte              stat_attack_normal_frame;
    [FieldOffset(0x40D)] public       byte              stat_attack_near_frame;
    [FieldOffset(0x40E)] public       byte              stat_attack_motion_frame;
    [FieldOffset(0x412)] public       bool              stat_move_flag;
    [FieldOffset(0x416)] public       byte              stat_move_target;
    [FieldOffset(0x41B)] public       bool              stat_direction_fix_flag;
    [FieldOffset(0x41C)] public       bool              stat_direction_change_flag;
    [FieldOffset(0x41D)] public       byte              stat_direction_change_effect;
    [FieldOffset(0x41E)] public       bool              stat_disable_move_flag;
    [FieldOffset(0x41F)] public       bool              stat_disable_jump_flag;
    [FieldOffset(0x420)] public       byte              stat_motionlv;
    [FieldOffset(0x422)] public       byte              stat_damage_chr;
    [FieldOffset(0x423)] public       bool              stat_appear_invisible_flag;
    [FieldOffset(0x424)] public       byte              stat_appear_count;
    [FieldOffset(0x425)] public       bool              stat_avoid_flag;
    [FieldOffset(0x426)] public       bool              stat_adjust_pos_flag;
    [FieldOffset(0x433)] public       bool              stat_hit_terminate_flag;
    [FieldOffset(0x437)] public       bool              stat_neck_target_flag;
    [FieldOffset(0x438)] public       byte              neck_target_id;
    [FieldOffset(0x43B)] public       byte              stat_win_pose;
    [FieldOffset(0x43C)] public       sbyte             stat_win_se;
    [FieldOffset(0x43D)] public       bool              stat_appear_motion_flag;
    [FieldOffset(0x43E)] public       byte              stat_live_motion;
    [FieldOffset(0x442)] public       byte              stat_num_print_element;
    [FieldOffset(0x446)] public       byte              stat_command_type;
    [FieldOffset(0x447)] public       byte              stat_inv_motion;
    [FieldOffset(0x448)] public       bool              stat_wait_motion_flag;
    [FieldOffset(0x44A)] public       sbyte             stat_near_motion;
    [FieldOffset(0x44B)] public       sbyte             stat_near_motion_set;
    [FieldOffset(0x44E)] public       byte              stat_dmg_dir;
    [FieldOffset(0x44F)] public       byte              stat_weak_motion;
    [FieldOffset(0x451)] public       byte              stat_magiclv;
    [FieldOffset(0x4B4)] public       byte              stat_own_attack_near;
    [FieldOffset(0x4B8)] public       byte              stat_idle2_prob;
    [FieldOffset(0x4B9)] public       byte              stat_attack_inc_speed;
    [FieldOffset(0x4BA)] public       byte              stat_attack_dec_speed;
    [FieldOffset(0x4BB)] public       byte              stat_motion_num;
    [FieldOffset(0x4FC)] public       ushort            area;
    [FieldOffset(0x4FE)] public       byte              pos;
    [FieldOffset(0x4FF)] public       byte              stat_far;
    [FieldOffset(0x504)] public       byte              stat_group;
    [FieldOffset(0x505)] public       bool              flying;
    [FieldOffset(0x508)] public       byte              stat_adjust_pos;
    [FieldOffset(0x509)] public       byte              stat_move_area;
    [FieldOffset(0x50B)] public       byte              stat_move_pos;
    [FieldOffset(0x50D)] public       byte              stat_height_on;
    [FieldOffset(0x50E)] public       bool              stat_sleep_recover_flag;
    [FieldOffset(0x540)] public fixed byte              name[40];
    [FieldOffset(0x590)] public       byte              gender;
    [FieldOffset(0x592)] public       byte              wpn_inv_idx;
    [FieldOffset(0x593)] public       byte              arm_inv_idx;
    [FieldOffset(0x594)] public       int               max_hp;
    [FieldOffset(0x598)] public       int               max_mp;
    [FieldOffset(0x59C)] public       int               base_max_hp;
    [FieldOffset(0x5A0)] public       int               base_max_mp;
    [FieldOffset(0x5A4)] public       int               overkill_threshold;
    [FieldOffset(0x5A8)] public       byte              strength;
    [FieldOffset(0x5A9)] public       byte              defense;
    [FieldOffset(0x5AA)] public       byte              magic;
    [FieldOffset(0x5AB)] public       byte              magic_defense;
    [FieldOffset(0x5AC)] public       byte              agility;
    [FieldOffset(0x5AD)] public       byte              luck;
    [FieldOffset(0x5AE)] public       byte              evasion;
    [FieldOffset(0x5AF)] public       byte              accuracy;
    [FieldOffset(0x5B0)] public       byte              strength_up;
    [FieldOffset(0x5B1)] public       byte              defense_up;
    [FieldOffset(0x5B2)] public       byte              magic_up;
    [FieldOffset(0x5B3)] public       byte              magic_defense_up;
    [FieldOffset(0x5B4)] public       byte              agility_up;
    [FieldOffset(0x5B5)] public       byte              luck_up;
    [FieldOffset(0x5B6)] public       byte              evasion_up;
    [FieldOffset(0x5B7)] public       byte              accuracy_up;
    [FieldOffset(0x5B8)] public       ushort            extra_resist; //TODO: Figure out a better name for this
    [FieldOffset(0x5BA)] public       byte              poison_dmg;
    [FieldOffset(0x5BB)] public       byte              ovr_mode_selected;
    [FieldOffset(0x5BC)] public       byte              ovr_charge;
    [FieldOffset(0x5BD)] public       byte              ovr_charge_max;
    [FieldOffset(0x5C1)] public       byte              wpn_dmg_formula;
    [FieldOffset(0x5C3)] public       byte              stat_icon_number;
    [FieldOffset(0x5C4)] public       byte              provoked_by_id;
    [FieldOffset(0x5C5)] public       byte              threatened_by_id;
    [FieldOffset(0x5C7)] public       byte              wpn_power;
    [FieldOffset(0x5C8)] public       byte              doom_counter;
    [FieldOffset(0x5C9)] public       byte              doom_counter_init;
    [FieldOffset(0x5CB)] public       bool              stat_prov_command_flag;
    [FieldOffset(0x5D0)] public       int               hp;
    [FieldOffset(0x5D4)] public       int               mp;
    [FieldOffset(0x5D8)] public       byte              wpn_crit_bonus;
    [FieldOffset(0x5D9)] public       byte              elem_wpn;
    [FieldOffset(0x5DA)] public       byte              elem_absorb;
    [FieldOffset(0x5DB)] public       byte              elem_ignore;
    [FieldOffset(0x5DC)] public       byte              elem_resist;
    [FieldOffset(0x5DD)] public       byte              elem_weak;
    [FieldOffset(0x5DE)] public       StatusMap         status_inflict;
    [FieldOffset(0x5F7)] public       StatusDurationMap status_duration_inflict;
    [FieldOffset(0x604)] public       ushort            status_inflict_extra;
    [FieldOffset(0x606)] public       ushort            status_suffer;
    [FieldOffset(0x608)] public       StatusDurationMap status_suffer_turns_left;
    [FieldOffset(0x616)] public       ushort            status_suffer_extra;
    [FieldOffset(0x62A)] public       ushort            status_full_auto_1;
    [FieldOffset(0x62C)] public       ushort            status_full_auto_2;
    [FieldOffset(0x62E)] public       ushort            status_full_auto_3;
    [FieldOffset(0x630)] public       ushort            status_innate_auto_1;
    [FieldOffset(0x632)] public       ushort            status_innate_auto_2;
    [FieldOffset(0x634)] public       ushort            status_innate_auto_3;
    [FieldOffset(0x636)] public       ushort            status_sos_auto_1;
    [FieldOffset(0x638)] public       ushort            status_sos_auto_2;
    [FieldOffset(0x63A)] public       ushort            status_sos_auto_3;
    [FieldOffset(0x63D)] public       byte              weak_level_full;
    [FieldOffset(0x63E)] public       byte              weak_level_hp;
    [FieldOffset(0x641)] public       StatusMap         status_resist;
    [FieldOffset(0x65A)] public       ushort            status_resist_extra;
    [FieldOffset(0x65C)] public       byte              ctb;
    [FieldOffset(0x65D)] public       byte              max_ctb;
    [FieldOffset(0x65E)] public       byte              cheer_stacks;
    [FieldOffset(0x65F)] public       byte              aim_stacks;
    [FieldOffset(0x660)] public       byte              focus_stacks;
    [FieldOffset(0x661)] public       byte              reflex_stacks;
    [FieldOffset(0x662)] public       byte              luck_stacks;
    [FieldOffset(0x663)] public       byte              jinx_stacks;
    [FieldOffset(0x664)] public       AbilityMap        abilities;
    [FieldOffset(0x6BC)] public       ushort            auto_abilities_1;
    [FieldOffset(0x6BE)] public       ushort            auto_abilities_2;
    [FieldOffset(0x6C0)] public       ushort            auto_abilities_3;
    [FieldOffset(0x6CE)] public       byte              stat_use_mp0;
    [FieldOffset(0x6D1)] public       byte              summoned_by_id;
    [FieldOffset(0x6D2)] public       byte              regen_strength;
    [FieldOffset(0x6DB)] public       byte              stat_attack_num;
    [FieldOffset(0x6DF)] public       byte              stat_command_exe_count;
    [FieldOffset(0x6E0)] public       byte              stat_consent;
    [FieldOffset(0x6E1)] public       byte              stat_energy;
    [FieldOffset(0x6E4)] public       int               current_hp;
    [FieldOffset(0x6E8)] public       int               current_mp;
    [FieldOffset(0x6EC)] public       int               current_ctb;
    [FieldOffset(0x700)] public       sbyte             stat_death_status;
    [FieldOffset(0x704)] public       uint              bribe_gil_spent;
    [FieldOffset(0x718)] public       bool              stat_limit_bar_flag;
    [FieldOffset(0x719)] public       byte              stat_limit_bar_flag_cam;
    [FieldOffset(0xD34)] public       int               damage_hp;
    [FieldOffset(0xD38)] public       int               damage_mp;
    [FieldOffset(0xD3C)] public       int               damage_ctb;
    [FieldOffset(0xDC8)] public       byte              in_battle;
    [FieldOffset(0xDCA)] public       byte              stat_target_list;
    [FieldOffset(0xDCC)] public       byte              stat_death;
    [FieldOffset(0xDCD)] public       bool              stat_escape_flag;
    [FieldOffset(0xDCE)] public       byte              stat_stone;
    [FieldOffset(0xDD2)] public       bool              stat_exist_flag;
    [FieldOffset(0xDD6)] public       byte              stat_action;
    [FieldOffset(0xDD7)] public       bool              in_ctb_list;
    [FieldOffset(0xDD8)] public       bool              in_hp_list;
    [FieldOffset(0xDD9)] public       byte              stat_cursor;
    [FieldOffset(0xDDA)] public       bool              stat_effect_target_flag;
    [FieldOffset(0xDDB)] public       bool              stat_regen_damage_flag;
    [FieldOffset(0xDDD)] public       byte              stat_event_chr;
    [FieldOffset(0xDDE)] public       bool              stat_blow_exist_flag;
    [FieldOffset(0xDEB)] public       byte              steal_count;
    [FieldOffset(0xDEC)] public       byte              stat_will_die;
    [FieldOffset(0xDF9)] public       byte              stat_cursor_element;
    [FieldOffset(0xE0C)] public       byte              stat_effvar;
    [FieldOffset(0xE0E)] public       byte              stat_effect_hit_num;
    [FieldOffset(0xE10)] public       byte              stat_magic_effect_ground;
    [FieldOffset(0xE11)] public       byte              stat_magic_effect_water;
    [FieldOffset(0xE1A)] public       short             stat_sound_hit_num;
    [FieldOffset(0xF74)] public       byte              stat_info_mes_id;
    [FieldOffset(0xF75)] public       byte              stat_live_mes_id;
    [FieldOffset(0xF78)] public       nint              ptr_script_chunks;
    [FieldOffset(0xF7C)] public       nint              ptr_script_data;
    [FieldOffset(0xF80)] public       nint              ptr_base_stats;
    [FieldOffset(0xF84)] public       nint              ptr_base_en_stats;
    [FieldOffset(0xF88)] public       ChrLoot*          loot;

    public bool stat_inv_physic_motion { readonly get { return stat_inv_motion.get_bit(0); } set { stat_inv_motion.set_bit(0, value); } }
    public bool stat_inv_magic_motion  { readonly get { return stat_inv_motion.get_bit(1); } set { stat_inv_motion.set_bit(1, value); } }

    // Gender
    public bool is_male   { readonly get { return gender.get_bit(0); } set { gender.set_bit(0, value); } }
    public bool is_female { readonly get { return gender.get_bit(1); } set { gender.set_bit(1, value); } }
    public bool is_aeon   { readonly get { return gender.get_bit(2); } set { gender.set_bit(2, value); } }

    // Extra Resistances (Properties)
    public bool is_armored           { readonly get { return extra_resist.get_bit(0); } set { extra_resist.set_bit(0, value); } }
    public bool ignores_gravity_dmg  { readonly get { return extra_resist.get_bit(1); } set { extra_resist.set_bit(1, value); } }
    public bool ignores_physical_dmg { readonly get { return extra_resist.get_bit(2); } set { extra_resist.set_bit(2, value); } }
    public bool ignores_magical_dmg  { readonly get { return extra_resist.get_bit(3); } set { extra_resist.set_bit(3, value); } }
    public bool is_invincible        { readonly get { return extra_resist.get_bit(4); } set { extra_resist.set_bit(4, value); } }
    public bool ignores_ctb_dmg      { readonly get { return extra_resist.get_bit(5); } set { extra_resist.set_bit(5, value); } }
    public bool ignores_zanmato      { readonly get { return extra_resist.get_bit(6); } set { extra_resist.set_bit(6, value); } }
    public bool ignores_bribe        { readonly get { return extra_resist.get_bit(7); } set { extra_resist.set_bit(7, value); } }

    // Elemental properties
    public bool has_firestrike    { readonly get { return elem_wpn.get_bit(0); } set { elem_wpn.set_bit(0, value); } }
    public bool has_icestrike     { readonly get { return elem_wpn.get_bit(1); } set { elem_wpn.set_bit(1, value); } }
    public bool has_thunderstrike { readonly get { return elem_wpn.get_bit(2); } set { elem_wpn.set_bit(2, value); } }
    public bool has_waterstrike   { readonly get { return elem_wpn.get_bit(3); } set { elem_wpn.set_bit(3, value); } }
    public bool has_holystrike    { readonly get { return elem_wpn.get_bit(4); } set { elem_wpn.set_bit(4, value); } }

    public bool absorbs_fire    { readonly get { return elem_absorb.get_bit(0); } set { elem_absorb.set_bit(0, value); } }
    public bool absorbs_ice     { readonly get { return elem_absorb.get_bit(1); } set { elem_absorb.set_bit(1, value); } }
    public bool absorbs_thunder { readonly get { return elem_absorb.get_bit(2); } set { elem_absorb.set_bit(2, value); } }
    public bool absorbs_water   { readonly get { return elem_absorb.get_bit(3); } set { elem_absorb.set_bit(3, value); } }
    public bool absorbs_holy    { readonly get { return elem_absorb.get_bit(4); } set { elem_absorb.set_bit(4, value); } }

    public bool ignores_fire    { readonly get { return elem_ignore.get_bit(0); } set { elem_ignore.set_bit(0, value); } }
    public bool ignores_ice     { readonly get { return elem_ignore.get_bit(1); } set { elem_ignore.set_bit(1, value); } }
    public bool ignores_thunder { readonly get { return elem_ignore.get_bit(2); } set { elem_ignore.set_bit(2, value); } }
    public bool ignores_water   { readonly get { return elem_ignore.get_bit(3); } set { elem_ignore.set_bit(3, value); } }
    public bool ignores_holy    { readonly get { return elem_ignore.get_bit(4); } set { elem_ignore.set_bit(4, value); } }

    public bool resists_fire    { readonly get { return elem_resist.get_bit(0); } set { elem_resist.set_bit(0, value); } }
    public bool resists_ice     { readonly get { return elem_resist.get_bit(1); } set { elem_resist.set_bit(1, value); } }
    public bool resists_thunder { readonly get { return elem_resist.get_bit(2); } set { elem_resist.set_bit(2, value); } }
    public bool resists_water   { readonly get { return elem_resist.get_bit(3); } set { elem_resist.set_bit(3, value); } }
    public bool resists_holy    { readonly get { return elem_resist.get_bit(4); } set { elem_resist.set_bit(4, value); } }

    public bool weak_fire       { readonly get { return elem_weak.get_bit(0); } set { elem_weak.set_bit(0, value); } }
    public bool weak_ice        { readonly get { return elem_weak.get_bit(1); } set { elem_weak.set_bit(1, value); } }
    public bool weak_thunder    { readonly get { return elem_weak.get_bit(2); } set { elem_weak.set_bit(2, value); } }
    public bool weak_water      { readonly get { return elem_weak.get_bit(3); } set { elem_weak.set_bit(3, value); } }
    public bool weak_holy       { readonly get { return elem_weak.get_bit(4); } set { elem_weak.set_bit(4, value); } }

    // Inflicts Extra Statuses
    public bool inflicts_scan            { readonly get { return status_inflict_extra.get_bit( 0); } set { status_inflict_extra.set_bit( 0, value); } }
    public bool inflicts_distill_power   { readonly get { return status_inflict_extra.get_bit( 1); } set { status_inflict_extra.set_bit( 1, value); } }
    public bool inflicts_distill_mana    { readonly get { return status_inflict_extra.get_bit( 2); } set { status_inflict_extra.set_bit( 2, value); } }
    public bool inflicts_distill_speed   { readonly get { return status_inflict_extra.get_bit( 3); } set { status_inflict_extra.set_bit( 3, value); } }
    public bool inflicts_distill_move    { readonly get { return status_inflict_extra.get_bit( 4); } set { status_inflict_extra.set_bit( 4, value); } }
    public bool inflicts_distill_ability { readonly get { return status_inflict_extra.get_bit( 5); } set { status_inflict_extra.set_bit( 5, value); } }
    public bool inflicts_shield          { readonly get { return status_inflict_extra.get_bit( 6); } set { status_inflict_extra.set_bit( 6, value); } }
    public bool inflicts_boost           { readonly get { return status_inflict_extra.get_bit( 7); } set { status_inflict_extra.set_bit( 7, value); } }
    public bool inflicts_eject           { readonly get { return status_inflict_extra.get_bit( 8); } set { status_inflict_extra.set_bit( 8, value); } }
    public bool inflicts_auto_life       { readonly get { return status_inflict_extra.get_bit( 9); } set { status_inflict_extra.set_bit( 9, value); } }
    public bool inflicts_curse           { readonly get { return status_inflict_extra.get_bit(10); } set { status_inflict_extra.set_bit(10, value); } }
    public bool inflicts_defend          { readonly get { return status_inflict_extra.get_bit(11); } set { status_inflict_extra.set_bit(11, value); } }
    public bool inflicts_guard           { readonly get { return status_inflict_extra.get_bit(12); } set { status_inflict_extra.set_bit(12, value); } }
    public bool inflicts_sentinel        { readonly get { return status_inflict_extra.get_bit(13); } set { status_inflict_extra.set_bit(13, value); } }
    public bool inflicts_doom            { readonly get { return status_inflict_extra.get_bit(14); } set { status_inflict_extra.set_bit(14, value); } }

    // Statuses
    public bool suffers_ko            { readonly get { return status_suffer.get_bit(0); } set { status_suffer.set_bit(0, value); } }
    public bool suffers_zombie        { readonly get { return status_suffer.get_bit(1); } set { status_suffer.set_bit(1, value); } }
    public bool suffers_petrification { readonly get { return status_suffer.get_bit(2); } set { status_suffer.set_bit(2, value); } }
    public bool suffers_poison        { readonly get { return status_suffer.get_bit(3); } set { status_suffer.set_bit(3, value); } }
    public bool suffers_power_break   { readonly get { return status_suffer.get_bit(4); } set { status_suffer.set_bit(4, value); } }
    public bool suffers_magic_break   { readonly get { return status_suffer.get_bit(5); } set { status_suffer.set_bit(5, value); } }
    public bool suffers_armor_break   { readonly get { return status_suffer.get_bit(6); } set { status_suffer.set_bit(6, value); } }
    public bool suffers_mental_break  { readonly get { return status_suffer.get_bit(7); } set { status_suffer.set_bit(7, value); } }
    public bool suffers_confusion     { readonly get { return status_suffer.get_bit(8); } set { status_suffer.set_bit(8, value); } }
    public bool suffers_berserk       { readonly get { return status_suffer.get_bit(9); } set { status_suffer.set_bit(9, value); } }
    public bool suffers_provoke       { readonly get { return status_suffer.get_bit(10); } set { status_suffer.set_bit(10, value); } }
    public bool suffers_threaten      { readonly get { return status_suffer.get_bit(11); } set { status_suffer.set_bit(11, value); } }

    public bool suffers_scan            { readonly get { return status_suffer_extra.get_bit( 0); } set { status_suffer_extra.set_bit( 0, value); } }
    public bool suffers_distill_power   { readonly get { return status_suffer_extra.get_bit( 1); } set { status_suffer_extra.set_bit( 1, value); } }
    public bool suffers_distill_mana    { readonly get { return status_suffer_extra.get_bit( 2); } set { status_suffer_extra.set_bit( 2, value); } }
    public bool suffers_distill_speed   { readonly get { return status_suffer_extra.get_bit( 3); } set { status_suffer_extra.set_bit( 3, value); } }
    public bool suffers_distill_move    { readonly get { return status_suffer_extra.get_bit( 4); } set { status_suffer_extra.set_bit( 4, value); } }
    public bool suffers_distill_ability { readonly get { return status_suffer_extra.get_bit( 5); } set { status_suffer_extra.set_bit( 5, value); } }
    public bool suffers_shield          { readonly get { return status_suffer_extra.get_bit( 6); } set { status_suffer_extra.set_bit( 6, value); } }
    public bool suffers_boost           { readonly get { return status_suffer_extra.get_bit( 7); } set { status_suffer_extra.set_bit( 7, value); } }
    public bool suffers_eject           { readonly get { return status_suffer_extra.get_bit( 8); } set { status_suffer_extra.set_bit( 8, value); } }
    public bool suffers_auto_life       { readonly get { return status_suffer_extra.get_bit( 9); } set { status_suffer_extra.set_bit( 9, value); } }
    public bool suffers_curse           { readonly get { return status_suffer_extra.get_bit(10); } set { status_suffer_extra.set_bit(10, value); } }
    public bool suffers_defend          { readonly get { return status_suffer_extra.get_bit(11); } set { status_suffer_extra.set_bit(11, value); } }
    public bool suffers_guard           { readonly get { return status_suffer_extra.get_bit(12); } set { status_suffer_extra.set_bit(12, value); } }
    public bool suffers_sentinel        { readonly get { return status_suffer_extra.get_bit(13); } set { status_suffer_extra.set_bit(13, value); } }
    public bool suffers_doom            { readonly get { return status_suffer_extra.get_bit(14); } set { status_suffer_extra.set_bit(14, value); } }

    // Currently Applicable Auto-Statuses
    public bool has_auto_death         { readonly get { return status_full_auto_1.get_bit( 0); } set { status_full_auto_1.set_bit( 0, value); } }
    public bool has_auto_zombie        { readonly get { return status_full_auto_1.get_bit( 1); } set { status_full_auto_1.set_bit( 1, value); } }
    public bool has_auto_petrification { readonly get { return status_full_auto_1.get_bit( 2); } set { status_full_auto_1.set_bit( 2, value); } }
    public bool has_auto_poison        { readonly get { return status_full_auto_1.get_bit( 3); } set { status_full_auto_1.set_bit( 3, value); } }
    public bool has_auto_power_break   { readonly get { return status_full_auto_1.get_bit( 4); } set { status_full_auto_1.set_bit( 4, value); } }
    public bool has_auto_magic_break   { readonly get { return status_full_auto_1.get_bit( 5); } set { status_full_auto_1.set_bit( 5, value); } }
    public bool has_auto_armor_break   { readonly get { return status_full_auto_1.get_bit( 6); } set { status_full_auto_1.set_bit( 6, value); } }
    public bool has_auto_mental_break  { readonly get { return status_full_auto_1.get_bit( 7); } set { status_full_auto_1.set_bit( 7, value); } }
    public bool has_auto_confuse       { readonly get { return status_full_auto_1.get_bit( 8); } set { status_full_auto_1.set_bit( 8, value); } }
    public bool has_auto_berserk       { readonly get { return status_full_auto_1.get_bit( 9); } set { status_full_auto_1.set_bit( 9, value); } }
    public bool has_auto_provoke       { readonly get { return status_full_auto_1.get_bit(10); } set { status_full_auto_1.set_bit(10, value); } }
    public bool has_auto_threaten      { readonly get { return status_full_auto_1.get_bit(11); } set { status_full_auto_1.set_bit(11, value); } }
    public bool has_auto_sleep         { readonly get { return status_full_auto_1.get_bit(12); } set { status_full_auto_1.set_bit(12, value); } }
    public bool has_auto_silence       { readonly get { return status_full_auto_1.get_bit(13); } set { status_full_auto_1.set_bit(13, value); } }
    public bool has_auto_darkness      { readonly get { return status_full_auto_1.get_bit(14); } set { status_full_auto_1.set_bit(14, value); } }
    public bool has_auto_shell         { readonly get { return status_full_auto_1.get_bit(15); } set { status_full_auto_1.set_bit(15, value); } }

    public bool has_auto_protect     { readonly get { return status_full_auto_2.get_bit(0); } set { status_full_auto_2.set_bit(0, value); } }
    public bool has_auto_reflect     { readonly get { return status_full_auto_2.get_bit(1); } set { status_full_auto_2.set_bit(1, value); } }
    public bool has_auto_nul_water   { readonly get { return status_full_auto_2.get_bit(2); } set { status_full_auto_2.set_bit(2, value); } }
    public bool has_auto_nul_fire    { readonly get { return status_full_auto_2.get_bit(3); } set { status_full_auto_2.set_bit(3, value); } }
    public bool has_auto_nul_thunder { readonly get { return status_full_auto_2.get_bit(4); } set { status_full_auto_2.set_bit(4, value); } }
    public bool has_auto_nul_ice     { readonly get { return status_full_auto_2.get_bit(5); } set { status_full_auto_2.set_bit(5, value); } }
    public bool has_auto_regen       { readonly get { return status_full_auto_2.get_bit(6); } set { status_full_auto_2.set_bit(6, value); } }
    public bool has_auto_haste       { readonly get { return status_full_auto_2.get_bit(7); } set { status_full_auto_2.set_bit(7, value); } }
    public bool has_auto_slow        { readonly get { return status_full_auto_2.get_bit(8); } set { status_full_auto_2.set_bit(8, value); } }

    // Innate Auto-Statuses
    public bool has_innate_auto_death         { readonly get { return status_innate_auto_1.get_bit( 0); } set { status_innate_auto_1.set_bit( 0, value); } }
    public bool has_innate_auto_zombie        { readonly get { return status_innate_auto_1.get_bit( 1); } set { status_innate_auto_1.set_bit( 1, value); } }
    public bool has_innate_auto_petrification { readonly get { return status_innate_auto_1.get_bit( 2); } set { status_innate_auto_1.set_bit( 2, value); } }
    public bool has_innate_auto_poison        { readonly get { return status_innate_auto_1.get_bit( 3); } set { status_innate_auto_1.set_bit( 3, value); } }
    public bool has_innate_auto_power_break   { readonly get { return status_innate_auto_1.get_bit( 4); } set { status_innate_auto_1.set_bit( 4, value); } }
    public bool has_innate_auto_magic_break   { readonly get { return status_innate_auto_1.get_bit( 5); } set { status_innate_auto_1.set_bit( 5, value); } }
    public bool has_innate_auto_armor_break   { readonly get { return status_innate_auto_1.get_bit( 6); } set { status_innate_auto_1.set_bit( 6, value); } }
    public bool has_innate_auto_mental_break  { readonly get { return status_innate_auto_1.get_bit( 7); } set { status_innate_auto_1.set_bit( 7, value); } }
    public bool has_innate_auto_confuse       { readonly get { return status_innate_auto_1.get_bit( 8); } set { status_innate_auto_1.set_bit( 8, value); } }
    public bool has_innate_auto_berserk       { readonly get { return status_innate_auto_1.get_bit( 9); } set { status_innate_auto_1.set_bit( 9, value); } }
    public bool has_innate_auto_provoke       { readonly get { return status_innate_auto_1.get_bit(10); } set { status_innate_auto_1.set_bit(10, value); } }
    public bool has_innate_auto_threaten      { readonly get { return status_innate_auto_1.get_bit(11); } set { status_innate_auto_1.set_bit(11, value); } }
    public bool has_innate_auto_sleep         { readonly get { return status_innate_auto_1.get_bit(12); } set { status_innate_auto_1.set_bit(12, value); } }
    public bool has_innate_auto_silence       { readonly get { return status_innate_auto_1.get_bit(13); } set { status_innate_auto_1.set_bit(13, value); } }
    public bool has_innate_auto_darkness      { readonly get { return status_innate_auto_1.get_bit(14); } set { status_innate_auto_1.set_bit(14, value); } }
    public bool has_innate_auto_shell         { readonly get { return status_innate_auto_1.get_bit(15); } set { status_innate_auto_1.set_bit(15, value); } }

    public bool has_innate_auto_protect     { readonly get { return status_innate_auto_2.get_bit(0); } set { status_innate_auto_2.set_bit(0, value); } }
    public bool has_innate_auto_reflect     { readonly get { return status_innate_auto_2.get_bit(1); } set { status_innate_auto_2.set_bit(1, value); } }
    public bool has_innate_auto_nul_water   { readonly get { return status_innate_auto_2.get_bit(2); } set { status_innate_auto_2.set_bit(2, value); } }
    public bool has_innate_auto_nul_fire    { readonly get { return status_innate_auto_2.get_bit(3); } set { status_innate_auto_2.set_bit(3, value); } }
    public bool has_innate_auto_nul_thunder { readonly get { return status_innate_auto_2.get_bit(4); } set { status_innate_auto_2.set_bit(4, value); } }
    public bool has_innate_auto_nul_ice     { readonly get { return status_innate_auto_2.get_bit(5); } set { status_innate_auto_2.set_bit(5, value); } }
    public bool has_innate_auto_regen       { readonly get { return status_innate_auto_2.get_bit(6); } set { status_innate_auto_2.set_bit(6, value); } }
    public bool has_innate_auto_haste       { readonly get { return status_innate_auto_2.get_bit(7); } set { status_innate_auto_2.set_bit(7, value); } }
    public bool has_innate_auto_slow        { readonly get { return status_innate_auto_2.get_bit(8); } set { status_innate_auto_2.set_bit(8, value); } }

    // SOS Auto-Statuses
    public bool has_sos_auto_death         { readonly get { return status_sos_auto_1.get_bit( 0); } set { status_sos_auto_1.set_bit( 0, value); } }
    public bool has_sos_auto_zombie        { readonly get { return status_sos_auto_1.get_bit( 1); } set { status_sos_auto_1.set_bit( 1, value); } }
    public bool has_sos_auto_petrification { readonly get { return status_sos_auto_1.get_bit( 2); } set { status_sos_auto_1.set_bit( 2, value); } }
    public bool has_sos_auto_poison        { readonly get { return status_sos_auto_1.get_bit( 3); } set { status_sos_auto_1.set_bit( 3, value); } }
    public bool has_sos_auto_power_break   { readonly get { return status_sos_auto_1.get_bit( 4); } set { status_sos_auto_1.set_bit( 4, value); } }
    public bool has_sos_auto_magic_break   { readonly get { return status_sos_auto_1.get_bit( 5); } set { status_sos_auto_1.set_bit( 5, value); } }
    public bool has_sos_auto_armor_break   { readonly get { return status_sos_auto_1.get_bit( 6); } set { status_sos_auto_1.set_bit( 6, value); } }
    public bool has_sos_auto_mental_break  { readonly get { return status_sos_auto_1.get_bit( 7); } set { status_sos_auto_1.set_bit( 7, value); } }
    public bool has_sos_auto_confuse       { readonly get { return status_sos_auto_1.get_bit( 8); } set { status_sos_auto_1.set_bit( 8, value); } }
    public bool has_sos_auto_berserk       { readonly get { return status_sos_auto_1.get_bit( 9); } set { status_sos_auto_1.set_bit( 9, value); } }
    public bool has_sos_auto_provoke       { readonly get { return status_sos_auto_1.get_bit(10); } set { status_sos_auto_1.set_bit(10, value); } }
    public bool has_sos_auto_threaten      { readonly get { return status_sos_auto_1.get_bit(11); } set { status_sos_auto_1.set_bit(11, value); } }
    public bool has_sos_auto_sleep         { readonly get { return status_sos_auto_1.get_bit(12); } set { status_sos_auto_1.set_bit(12, value); } }
    public bool has_sos_auto_silence       { readonly get { return status_sos_auto_1.get_bit(13); } set { status_sos_auto_1.set_bit(13, value); } }
    public bool has_sos_auto_darkness      { readonly get { return status_sos_auto_1.get_bit(14); } set { status_sos_auto_1.set_bit(14, value); } }
    public bool has_sos_auto_shell         { readonly get { return status_sos_auto_1.get_bit(15); } set { status_sos_auto_1.set_bit(15, value); } }

    public bool has_sos_auto_protect     { readonly get { return status_sos_auto_2.get_bit(0); } set { status_sos_auto_2.set_bit(0, value); } }
    public bool has_sos_auto_reflect     { readonly get { return status_sos_auto_2.get_bit(1); } set { status_sos_auto_2.set_bit(1, value); } }
    public bool has_sos_auto_nul_water   { readonly get { return status_sos_auto_2.get_bit(2); } set { status_sos_auto_2.set_bit(2, value); } }
    public bool has_sos_auto_nul_fire    { readonly get { return status_sos_auto_2.get_bit(3); } set { status_sos_auto_2.set_bit(3, value); } }
    public bool has_sos_auto_nul_thunder { readonly get { return status_sos_auto_2.get_bit(4); } set { status_sos_auto_2.set_bit(4, value); } }
    public bool has_sos_auto_nul_ice     { readonly get { return status_sos_auto_2.get_bit(5); } set { status_sos_auto_2.set_bit(5, value); } }
    public bool has_sos_auto_regen       { readonly get { return status_sos_auto_2.get_bit(6); } set { status_sos_auto_2.set_bit(6, value); } }
    public bool has_sos_auto_haste       { readonly get { return status_sos_auto_2.get_bit(7); } set { status_sos_auto_2.set_bit(7, value); } }
    public bool has_sos_auto_slow        { readonly get { return status_sos_auto_2.get_bit(8); } set { status_sos_auto_2.set_bit(8, value); } }

    // Extra Status Resistances
    public bool resists_scan            { readonly get { return status_resist_extra.get_bit( 0); } set { status_resist_extra.set_bit( 0, value); } }
    public bool resists_distill_power   { readonly get { return status_resist_extra.get_bit( 1); } set { status_resist_extra.set_bit( 1, value); } }
    public bool resists_distill_mana    { readonly get { return status_resist_extra.get_bit( 2); } set { status_resist_extra.set_bit( 2, value); } }
    public bool resists_distill_speed   { readonly get { return status_resist_extra.get_bit( 3); } set { status_resist_extra.set_bit( 3, value); } }
    public bool resists_distill_move    { readonly get { return status_resist_extra.get_bit( 4); } set { status_resist_extra.set_bit( 4, value); } }
    public bool resists_distill_ability { readonly get { return status_resist_extra.get_bit( 5); } set { status_resist_extra.set_bit( 5, value); } }
    public bool resists_shield          { readonly get { return status_resist_extra.get_bit( 6); } set { status_resist_extra.set_bit( 6, value); } }
    public bool resists_boost           { readonly get { return status_resist_extra.get_bit( 7); } set { status_resist_extra.set_bit( 7, value); } }
    public bool resists_eject           { readonly get { return status_resist_extra.get_bit( 8); } set { status_resist_extra.set_bit( 8, value); } }
    public bool resists_auto_life       { readonly get { return status_resist_extra.get_bit( 9); } set { status_resist_extra.set_bit( 9, value); } }
    public bool resists_curse           { readonly get { return status_resist_extra.get_bit(10); } set { status_resist_extra.set_bit(10, value); } }
    public bool resists_defend          { readonly get { return status_resist_extra.get_bit(11); } set { status_resist_extra.set_bit(11, value); } }
    public bool resists_guard           { readonly get { return status_resist_extra.get_bit(12); } set { status_resist_extra.set_bit(12, value); } }
    public bool resists_sentinel        { readonly get { return status_resist_extra.get_bit(13); } set { status_resist_extra.set_bit(13, value); } }
    public bool resists_doom            { readonly get { return status_resist_extra.get_bit(14); } set { status_resist_extra.set_bit(14, value); } }

    // Auto-Abilities
    public bool has_sensor            { readonly get { return auto_abilities_1.get_bit( 0); } set { auto_abilities_1.set_bit( 0, value); } }
    public bool has_first_strike      { readonly get { return auto_abilities_1.get_bit( 1); } set { auto_abilities_1.set_bit( 1, value); } }
    public bool has_initiative        { readonly get { return auto_abilities_1.get_bit( 2); } set { auto_abilities_1.set_bit( 2, value); } }
    public bool has_counter_attack    { readonly get { return auto_abilities_1.get_bit( 3); } set { auto_abilities_1.set_bit( 3, value); } }
    public bool has_evade_and_counter { readonly get { return auto_abilities_1.get_bit( 4); } set { auto_abilities_1.set_bit( 4, value); } }
    public bool has_magic_counter     { readonly get { return auto_abilities_1.get_bit( 5); } set { auto_abilities_1.set_bit( 5, value); } }
    public bool has_magic_booster     { readonly get { return auto_abilities_1.get_bit( 6); } set { auto_abilities_1.set_bit( 6, value); } }
    public bool has_alchemy           { readonly get { return auto_abilities_1.get_bit( 9); } set { auto_abilities_1.set_bit( 9, value); } }
    public bool has_auto_potion       { readonly get { return auto_abilities_1.get_bit(10); } set { auto_abilities_1.set_bit(10, value); } }
    public bool has_auto_med          { readonly get { return auto_abilities_1.get_bit(11); } set { auto_abilities_1.set_bit(11, value); } }
    public bool has_auto_phoenix      { readonly get { return auto_abilities_1.get_bit(12); } set { auto_abilities_1.set_bit(12, value); } }
    public bool has_piercing          { readonly get { return auto_abilities_1.get_bit(13); } set { auto_abilities_1.set_bit(13, value); } }
    public bool has_half_mp_cost      { readonly get { return auto_abilities_1.get_bit(14); } set { auto_abilities_1.set_bit(14, value); } }
    public bool has_one_mp_cost       { readonly get { return auto_abilities_1.get_bit(15); } set { auto_abilities_1.set_bit(15, value); } }

    public bool has_double_overdrive   { readonly get { return auto_abilities_2.get_bit( 0); } set { auto_abilities_2.set_bit( 0, value); } }
    public bool has_triple_overdrive   { readonly get { return auto_abilities_2.get_bit( 1); } set { auto_abilities_2.set_bit( 1, value); } }
    public bool has_sos_overdrive      { readonly get { return auto_abilities_2.get_bit( 2); } set { auto_abilities_2.set_bit( 2, value); } }
    public bool has_overdrive_to_ap    { readonly get { return auto_abilities_2.get_bit( 3); } set { auto_abilities_2.set_bit( 3, value); } }
    public bool has_double_ap          { readonly get { return auto_abilities_2.get_bit( 4); } set { auto_abilities_2.set_bit( 4, value); } }
    public bool has_triple_ap          { readonly get { return auto_abilities_2.get_bit( 5); } set { auto_abilities_2.set_bit( 5, value); } }
    public bool has_no_ap              { readonly get { return auto_abilities_2.get_bit( 6); } set { auto_abilities_2.set_bit( 6, value); } }
    public bool has_pickpocket         { readonly get { return auto_abilities_2.get_bit( 7); } set { auto_abilities_2.set_bit( 7, value); } }
    public bool has_master_thief       { readonly get { return auto_abilities_2.get_bit( 8); } set { auto_abilities_2.set_bit( 8, value); } }
    public bool has_break_hp_limit     { readonly get { return auto_abilities_2.get_bit( 9); } set { auto_abilities_2.set_bit( 9, value); } }
    public bool has_break_mp_limit     { readonly get { return auto_abilities_2.get_bit(10); } set { auto_abilities_2.set_bit(10, value); } }
    public bool has_break_damage_limit { readonly get { return auto_abilities_2.get_bit(11); } set { auto_abilities_2.set_bit(11, value); } }
    public bool has_gillionaire        { readonly get { return auto_abilities_2.get_bit(14); } set { auto_abilities_2.set_bit(14, value); } }
    public bool has_hp_stroll          { readonly get { return auto_abilities_2.get_bit(15); } set { auto_abilities_2.set_bit(15, value); } }

    public bool has_mp_stroll     { readonly get { return auto_abilities_3.get_bit(0); } set { auto_abilities_3.set_bit(0, value); } }
    public bool has_no_encounters { readonly get { return auto_abilities_3.get_bit(1); } set { auto_abilities_3.set_bit(1, value); } }
    public bool has_capture       { readonly get { return auto_abilities_3.get_bit(2); } set { auto_abilities_3.set_bit(2, value); } }
}