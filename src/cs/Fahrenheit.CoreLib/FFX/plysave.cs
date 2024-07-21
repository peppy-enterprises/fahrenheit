namespace Fahrenheit.CoreLib.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x94)]
public unsafe struct PlySave {
    [FieldOffset(0x0)] public uint __0x0;
    [FieldOffset(0x4)] public uint base_hp;
    [FieldOffset(0x8)] public uint base_mp;
    [FieldOffset(0xC)] public byte base_strength;
    [FieldOffset(0xD)] public byte base_defense;
    [FieldOffset(0xE)] public byte base_magic;
    [FieldOffset(0xF)] public byte base_magic_defense;
    [FieldOffset(0x10)] public byte base_agility;
    [FieldOffset(0x11)] public byte base_luck;
    [FieldOffset(0x12)] public byte base_evasion;
    [FieldOffset(0x13)] public byte base_accuracy;
    [FieldOffset(0x14)] public ushort __0x14;
    [FieldOffset(0x16)] public ushort __0x16;
    [FieldOffset(0x18)] public uint ap;
    [FieldOffset(0x1C)] public uint hp;
    [FieldOffset(0x20)] public uint mp;
    [FieldOffset(0x24)] public uint max_hp;
    [FieldOffset(0x28)] public uint max_mp;
    [FieldOffset(0x2C)] public byte ply_flags;
    public bool join { get => ply_flags.get_bit(0); set => ply_flags.set_bit(0, value); }
    public bool joined { get => ply_flags.get_bit(4); set => ply_flags.set_bit(4, value); }

    [FieldOffset(0x2D)] public byte wpn_inv_idx;
    [FieldOffset(0x2E)] public byte arm_inv_idx;
    [FieldOffset(0x2F)] public byte strength;
    [FieldOffset(0x30)] public byte defense;
    [FieldOffset(0x31)] public byte magic;
    [FieldOffset(0x32)] public byte magic_defense;
    [FieldOffset(0x33)] public byte agility;
    [FieldOffset(0x34)] public byte luck;
    [FieldOffset(0x35)] public byte evasion;
    [FieldOffset(0x36)] public byte accuracy;
    [FieldOffset(0x37)] public byte poison_dmg;
    [FieldOffset(0x38)] public byte ovr_mode_selected;
    [FieldOffset(0x39)] public byte ovr_charge;
    [FieldOffset(0x3A)] public byte ovr_charge_max;
    [FieldOffset(0x3B)] public byte slv_available;
    [FieldOffset(0x3C)] public byte slv_spent;
    [FieldOffset(0x3D)] public byte __0x3D;
    [FieldOffset(0x3E)] public AbiMap abilities;
    [FieldOffset(0x4A)] public ushort auto_abilities1;
    [FieldOffset(0x4C)] public ushort auto_abilities2;
    [FieldOffset(0x4E)] public ushort auto_abilities3;
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

    [FieldOffset(0x50)] public uint battle_count;
    [FieldOffset(0x54)] public uint enemies_defeated;
    [FieldOffset(0x58)] public uint __0x58;
    [FieldOffset(0x5C)] public uint __0x5C;
    [FieldOffset(0x60)] public fixed ushort ovr_mode_counters[20];
    public ushort ovr_counter_warrior	{ get => ovr_mode_counters[0]; set => ovr_mode_counters[0] = value; }
    public ushort ovr_counter_comrade	{ get => ovr_mode_counters[1]; set => ovr_mode_counters[1] = value; }
    public ushort ovr_counter_stoic		{ get => ovr_mode_counters[2]; set => ovr_mode_counters[2] = value; }
    public ushort ovr_counter_healer	{ get => ovr_mode_counters[3]; set => ovr_mode_counters[3] = value; }
    public ushort ovr_counter_tactician	{ get => ovr_mode_counters[4]; set => ovr_mode_counters[4] = value; }
    public ushort ovr_counter_victim	{ get => ovr_mode_counters[5]; set => ovr_mode_counters[5] = value; }
    public ushort ovr_counter_dancer	{ get => ovr_mode_counters[6]; set => ovr_mode_counters[6] = value; }
    public ushort ovr_counter_avenger	{ get => ovr_mode_counters[7]; set => ovr_mode_counters[7] = value; }
    public ushort ovr_counter_slayer	{ get => ovr_mode_counters[8]; set => ovr_mode_counters[8] = value; }
    public ushort ovr_counter_hero		{ get => ovr_mode_counters[9]; set => ovr_mode_counters[9] = value; }
    public ushort ovr_counter_rook		{ get => ovr_mode_counters[10]; set => ovr_mode_counters[10] = value; }
    public ushort ovr_counter_victor	{ get => ovr_mode_counters[11]; set => ovr_mode_counters[11] = value; }
    public ushort ovr_counter_coward	{ get => ovr_mode_counters[12]; set => ovr_mode_counters[12] = value; }
    public ushort ovr_counter_ally		{ get => ovr_mode_counters[13]; set => ovr_mode_counters[13] = value; }
    public ushort ovr_counter_sufferer	{ get => ovr_mode_counters[14]; set => ovr_mode_counters[14] = value; }
    public ushort ovr_counter_daredevil	{ get => ovr_mode_counters[15]; set => ovr_mode_counters[15] = value; }
    public ushort ovr_counter_liner		{ get => ovr_mode_counters[16]; set => ovr_mode_counters[16] = value; }
    public ushort ovr_counter_unused1	{ get => ovr_mode_counters[17]; set => ovr_mode_counters[17] = value; }
    public ushort ovr_counter_unused2	{ get => ovr_mode_counters[18]; set => ovr_mode_counters[18] = value; }
    public ushort ovr_counter_aeons		{ get => ovr_mode_counters[19]; set => ovr_mode_counters[19] = value; }

    [FieldOffset(0x88)] public uint ovr_modes_unlocked;
    public bool ovr_has_warrior		{ get => ovr_modes_unlocked.get_bit(0); set => ovr_modes_unlocked.set_bit(0, value); }
    public bool ovr_has_comrade		{ get => ovr_modes_unlocked.get_bit(1); set => ovr_modes_unlocked.set_bit(0, value); }
    public bool ovr_has_stoic		{ get => ovr_modes_unlocked.get_bit(2); set => ovr_modes_unlocked.set_bit(0, value); }
    public bool ovr_has_healer		{ get => ovr_modes_unlocked.get_bit(3); set => ovr_modes_unlocked.set_bit(0, value); }
    public bool ovr_has_tactician	{ get => ovr_modes_unlocked.get_bit(4); set => ovr_modes_unlocked.set_bit(0, value); }
    public bool ovr_has_victim		{ get => ovr_modes_unlocked.get_bit(5); set => ovr_modes_unlocked.set_bit(0, value); }
    public bool ovr_has_dancer		{ get => ovr_modes_unlocked.get_bit(6); set => ovr_modes_unlocked.set_bit(0, value); }
    public bool ovr_has_avenger		{ get => ovr_modes_unlocked.get_bit(7); set => ovr_modes_unlocked.set_bit(0, value); }
    public bool ovr_has_slayer		{ get => ovr_modes_unlocked.get_bit(8); set => ovr_modes_unlocked.set_bit(0, value); }
    public bool ovr_has_hero		{ get => ovr_modes_unlocked.get_bit(9); set => ovr_modes_unlocked.set_bit(0, value); }
    public bool ovr_has_rook		{ get => ovr_modes_unlocked.get_bit(10); set => ovr_modes_unlocked.set_bit(0, value); }
    public bool ovr_has_victor		{ get => ovr_modes_unlocked.get_bit(11); set => ovr_modes_unlocked.set_bit(0, value); }
    public bool ovr_has_coward		{ get => ovr_modes_unlocked.get_bit(12); set => ovr_modes_unlocked.set_bit(0, value); }
    public bool ovr_has_ally		{ get => ovr_modes_unlocked.get_bit(13); set => ovr_modes_unlocked.set_bit(0, value); }
    public bool ovr_has_sufferer	{ get => ovr_modes_unlocked.get_bit(14); set => ovr_modes_unlocked.set_bit(0, value); }
    public bool ovr_has_daredevil	{ get => ovr_modes_unlocked.get_bit(15); set => ovr_modes_unlocked.set_bit(0, value); }
    public bool ovr_has_loner		{ get => ovr_modes_unlocked.get_bit(16); set => ovr_modes_unlocked.set_bit(0, value); }
    public bool ovr_has_unused1		{ get => ovr_modes_unlocked.get_bit(17); set => ovr_modes_unlocked.set_bit(0, value); }
    public bool ovr_has_unused2		{ get => ovr_modes_unlocked.get_bit(18); set => ovr_modes_unlocked.set_bit(0, value); }
    public bool ovr_has_aeons		{ get => ovr_modes_unlocked.get_bit(19); set => ovr_modes_unlocked.set_bit(0, value); }

    [FieldOffset(0x8C)] public uint __0x8C;
    [FieldOffset(0x90)] public uint __0x90;
}

