/* [ct2cs 21/5/23 00:09]
 * This file was generated by Fahrenheit.CT2CS (https://github.com/fkelava/fahrenheit/).
 * 
 * Source file: x_btlactor.ct
 */

namespace Fahrenheit.CoreLib.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0xC)]
public struct ChrItemLoot {
	[FieldOffset(0x0)] public ushort item_primary_common;
	[FieldOffset(0x2)] public ushort item_primary_rare;
	[FieldOffset(0x4)] public ushort item_secondary_common;
	[FieldOffset(0x6)] public ushort item_secondary_rare;
	[FieldOffset(0x8)] public byte amount_primary_common;
	[FieldOffset(0x9)] public byte amount_primary_rare;
	[FieldOffset(0xA)] public byte amount_secondary_common;
	[FieldOffset(0xB)] public byte amount_secondary_rare;
}

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x9)]
public struct ChrStealLoot {
	[FieldOffset(0x0)] public ushort item_common;
	[FieldOffset(0x2)] public ushort item_rare;
	[FieldOffset(0x4)] public byte amount_common;
	[FieldOffset(0x5)] public byte amount_rare;
	[FieldOffset(0x6)] public ushort item_bribe;
	[FieldOffset(0x8)] public byte amount_bribe;
}

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x20)]
public unsafe struct ChrGearLootAbilities {
	[FieldOffset(0x0)] public fixed ushort weapon_abilities[8];
	[FieldOffset(0x10)] public fixed ushort armor_abilities[8];
}

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0xE5)]
public struct ChrGearLoot {
	[FieldOffset(0x0)] public byte slot_count;
	[FieldOffset(0x1)] public byte dmg_formula;
	[FieldOffset(0x2)] public byte crit_bonus;
	[FieldOffset(0x3)] public byte power;
	[FieldOffset(0x4)] public byte ability_count;
	[FieldOffset(0x5)] public ChrGearLootAbilities tidus_abilities;
	[FieldOffset(0x25)] public ChrGearLootAbilities yuna_abilities;
	[FieldOffset(0x45)] public ChrGearLootAbilities auron_abilities;
	[FieldOffset(0x65)] public ChrGearLootAbilities kimahri_abilities;
	[FieldOffset(0x85)] public ChrGearLootAbilities wakka_abilities;
	[FieldOffset(0xA5)] public ChrGearLootAbilities lulu_abilities;
	[FieldOffset(0xC5)] public ChrGearLootAbilities rikku_abilities;
}

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x118)]
public unsafe struct ChrLoot {
    [FieldOffset(0x0)] public ushort gil;
    [FieldOffset(0x2)] public ushort ap;
    [FieldOffset(0x4)] public ushort ap_overkill;
    [FieldOffset(0x6)] public ushort ronso_rage;

    [FieldOffset(0x8)] public byte drop_chance_primary;
    [FieldOffset(0x9)] public byte drop_chance_secondary;
    [FieldOffset(0xA)] public byte steal_chance;
    [FieldOffset(0xB)] public byte drop_chance_gear;

	[FieldOffset(0xC)] public ChrItemLoot item_loot;
	[FieldOffset(0x18)] public ChrItemLoot item_loot_overkill;
	[FieldOffset(0x24)] public ChrStealLoot steal_loot;
	[FieldOffset(0x2D)] public ChrGearLoot gear_loot;

    [FieldOffset(0x112)] public byte zanmato_level;
}
