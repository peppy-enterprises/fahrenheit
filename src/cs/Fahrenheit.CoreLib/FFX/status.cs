namespace Fahrenheit.CoreLib.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x19)]
public struct Status {
	[FieldOffset(0x0)] public byte death;
	[FieldOffset(0x1)] public byte zombie;
	[FieldOffset(0x2)] public byte petrification;
	[FieldOffset(0x3)] public byte poison;
	[FieldOffset(0x4)] public byte power_break;
	[FieldOffset(0x5)] public byte magic_break;
	[FieldOffset(0x6)] public byte armor_break;
	[FieldOffset(0x7)] public byte mental_break;
	[FieldOffset(0x8)] public byte confuse;
	[FieldOffset(0x9)] public byte berserk;
	[FieldOffset(0xA)] public byte provoke;
	[FieldOffset(0xB)] public byte threaten;
	[FieldOffset(0xC)] public byte sleep;
	[FieldOffset(0xD)] public byte silence;
	[FieldOffset(0xE)] public byte darkness;
	[FieldOffset(0xF)] public byte shell;
	[FieldOffset(0x10)] public byte protect;
	[FieldOffset(0x11)] public byte reflect;
	[FieldOffset(0x12)] public byte nul_water;
	[FieldOffset(0x13)] public byte nul_fire;
	[FieldOffset(0x14)] public byte nul_thunder;
	[FieldOffset(0x15)] public byte nul_blizzard;
	[FieldOffset(0x16)] public byte regen;
	[FieldOffset(0x17)] public byte haste;
	[FieldOffset(0x18)] public byte slow;
}

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0xD)]
public struct StatusDur {
	[FieldOffset(0x0)] public byte sleep;
	[FieldOffset(0x1)] public byte silence;
	[FieldOffset(0x2)] public byte darkness;
	[FieldOffset(0x3)] public byte shell;
	[FieldOffset(0x4)] public byte protect;
	[FieldOffset(0x5)] public byte reflect;
	[FieldOffset(0x6)] public byte nul_water;
	[FieldOffset(0x7)] public byte nul_fire;
	[FieldOffset(0x8)] public byte nul_thunder;
	[FieldOffset(0x9)] public byte nul_blizzard;
	[FieldOffset(0xA)] public byte regen;
	[FieldOffset(0xB)] public byte haste;
	[FieldOffset(0xC)] public byte slow;
}