namespace Fahrenheit.CoreLib;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x16)]
public unsafe struct FhXGear {
	[FieldOffset(0x0)] public ushort name_id;
	[FieldOffset(0x2)] public bool exists;
	[FieldOffset(0x3)] public byte properties;
	public bool hide			{ get => properties.get_bit(1); set => properties.set_bit(1, value); }
	public bool is_celestial	{ get => properties.get_bit(2); set => properties.set_bit(2, value); }
	public bool is_brotherhood	{ get => properties.get_bit(3); set => properties.set_bit(3, value); }

	[FieldOffset(0x4)] public byte owner;
	[FieldOffset(0x5)] public byte type;
	public readonly bool is_weapon => (type & 1) == 0;
	public readonly bool is_armor  => (type & 1) != 0;

	[FieldOffset(0x6)] public byte __0x6;
	[FieldOffset(0x7)] public byte __0x7;
	[FieldOffset(0x8)] public byte dmg_formula;
	[FieldOffset(0x9)] public byte power;
	[FieldOffset(0xA)] public byte crit_bonus;
	[FieldOffset(0xB)] public byte slot_count;
	[FieldOffset(0xC)] public ushort model_id;
	[FieldOffset(0xE)] public fixed ushort abilities[4];
}