// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core.FFX.Battle;

public struct ChrItemLoot {
    public ushort item_primary_common;
    public ushort item_primary_rare;
    public ushort item_secondary_common;
    public ushort item_secondary_rare;
    public byte   amount_primary_common;
    public byte   amount_primary_rare;
    public byte   amount_secondary_common;
    public byte   amount_secondary_rare;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ChrStealLoot {
    public ushort item_common;
    public ushort item_rare;
    public byte   amount_common;
    public byte   amount_rare;
    public ushort item_bribe;
    public byte   amount_bribe;
}

[InlineArray(8)]
public struct ChrEquipmentLootAbilitiesArray {
    private ushort _u;
}

public struct ChrEquipmentLootAbilities {
    public ChrEquipmentLootAbilitiesArray weapon_abilities;
    public ChrEquipmentLootAbilitiesArray armor_abilities;
}

public struct ChrEquipmentLoot {
    public byte slot_count;
    public byte dmg_formula;
    public byte crit_bonus;
    public byte power;
    public byte ability_count;

    public ChrEquipmentLootAbilities abilities_tidus;
    public ChrEquipmentLootAbilities abilities_yuna;
    public ChrEquipmentLootAbilities abilities_auron;
    public ChrEquipmentLootAbilities abilities_kimahri;
    public ChrEquipmentLootAbilities abilities_wakka;
    public ChrEquipmentLootAbilities abilities_lulu;
    public ChrEquipmentLootAbilities abilities_rikku;
}

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x118)]
public unsafe struct ChrLoot {
    [FieldOffset(0x00)] public ushort gil;
    [FieldOffset(0x02)] public ushort ap;
    [FieldOffset(0x04)] public ushort ap_overkill;
    [FieldOffset(0x06)] public ushort ronso_rage;

    [FieldOffset(0x08)] public byte drop_chance_primary;
    [FieldOffset(0x09)] public byte drop_chance_secondary;
    [FieldOffset(0x0A)] public byte steal_chance;
    [FieldOffset(0x0B)] public byte drop_chance_equipment;

    [FieldOffset(0x0C)] public ChrItemLoot      item_loot;
    [FieldOffset(0x18)] public ChrItemLoot      item_loot_overkill;
    [FieldOffset(0x24)] public ChrStealLoot     steal_loot;
    [FieldOffset(0x2D)] public ChrEquipmentLoot equipment_loot;

    [FieldOffset(0x112)] public byte zanmato_level;
}
