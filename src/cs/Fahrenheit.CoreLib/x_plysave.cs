namespace Fahrenheit.CoreLib;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x94)]
public unsafe struct FhXPlySave 
{
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
	[FieldOffset(0x3E)] public FhXAbiMap abilities;
    [FieldOffset(0x4A)] public ushort __0x4A;
    [FieldOffset(0x4C)] public uint __0x4C;
    [FieldOffset(0x50)] public uint battle_count;
    [FieldOffset(0x54)] public uint enemies_defeated;
    [FieldOffset(0x58)] public uint __0x58;
    [FieldOffset(0x5C)] public uint __0x5C;
	[FieldOffset(0x60)] public fixed ushort ovr_mode_counters[20];
	public ushort ovr_counter_warrior	{ get => ovr_mode_counters[0]; set => ovr_mode_counters[0] = value; }
	public ushort ovr_counter_comrade	{ get => ovr_mode_counters[0]; set => ovr_mode_counters[0] = value; }
	public ushort ovr_counter_stoic		{ get => ovr_mode_counters[0]; set => ovr_mode_counters[0] = value; }
	public ushort ovr_counter_healer	{ get => ovr_mode_counters[0]; set => ovr_mode_counters[0] = value; }
	public ushort ovr_counter_tactician	{ get => ovr_mode_counters[0]; set => ovr_mode_counters[0] = value; }
	public ushort ovr_counter_victim	{ get => ovr_mode_counters[0]; set => ovr_mode_counters[0] = value; }
	public ushort ovr_counter_dancer	{ get => ovr_mode_counters[0]; set => ovr_mode_counters[0] = value; }
	public ushort ovr_counter_avenger	{ get => ovr_mode_counters[0]; set => ovr_mode_counters[0] = value; }
	public ushort ovr_counter_slayer	{ get => ovr_mode_counters[0]; set => ovr_mode_counters[0] = value; }
	public ushort ovr_counter_hero		{ get => ovr_mode_counters[0]; set => ovr_mode_counters[0] = value; }
	public ushort ovr_counter_rook		{ get => ovr_mode_counters[0]; set => ovr_mode_counters[0] = value; }
	public ushort ovr_counter_victor	{ get => ovr_mode_counters[0]; set => ovr_mode_counters[0] = value; }
	public ushort ovr_counter_coward	{ get => ovr_mode_counters[0]; set => ovr_mode_counters[0] = value; }
	public ushort ovr_counter_ally		{ get => ovr_mode_counters[0]; set => ovr_mode_counters[0] = value; }
	public ushort ovr_counter_sufferer	{ get => ovr_mode_counters[0]; set => ovr_mode_counters[0] = value; }
	public ushort ovr_counter_daredevil	{ get => ovr_mode_counters[0]; set => ovr_mode_counters[0] = value; }
	public ushort ovr_counter_liner		{ get => ovr_mode_counters[0]; set => ovr_mode_counters[0] = value; }
	public ushort ovr_counter_unused1	{ get => ovr_mode_counters[0]; set => ovr_mode_counters[0] = value; }
	public ushort ovr_counter_unused2	{ get => ovr_mode_counters[0]; set => ovr_mode_counters[0] = value; }
	public ushort ovr_counter_aeons		{ get => ovr_mode_counters[0]; set => ovr_mode_counters[0] = value; }

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

