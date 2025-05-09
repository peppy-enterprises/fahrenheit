﻿namespace Fahrenheit.Core.FFX.Battle;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x0C)]
public struct ChrItemLoot {
    [FieldOffset(0x00)] public ushort item_primary_common;
    [FieldOffset(0x02)] public ushort item_primary_rare;
    [FieldOffset(0x04)] public ushort item_secondary_common;
    [FieldOffset(0x06)] public ushort item_secondary_rare;
    [FieldOffset(0x08)] public byte   amount_primary_common;
    [FieldOffset(0x09)] public byte   amount_primary_rare;
    [FieldOffset(0x0A)] public byte   amount_secondary_common;
    [FieldOffset(0x0B)] public byte   amount_secondary_rare;
}

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x09)]
public struct ChrStealLoot {
    [FieldOffset(0x00)] public ushort item_common;
    [FieldOffset(0x02)] public ushort item_rare;
    [FieldOffset(0x04)] public byte   amount_common;
    [FieldOffset(0x05)] public byte   amount_rare;
    [FieldOffset(0x06)] public ushort item_bribe;
    [FieldOffset(0x08)] public byte   amount_bribe;
}

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x20)]
public unsafe struct ChrEquipmentLootAbilities {
    [FieldOffset(0x00)] public fixed ushort weapon_abilities[8];
    [FieldOffset(0x10)] public fixed ushort armor_abilities [8];
}

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0xE5)]
public struct ChrEquipmentLoot {
    [FieldOffset(0x00)] public byte slot_count;
    [FieldOffset(0x01)] public byte dmg_formula;
    [FieldOffset(0x02)] public byte crit_bonus;
    [FieldOffset(0x03)] public byte power;
    [FieldOffset(0x04)] public byte ability_count;

    [FieldOffset(0x05)] public ChrEquipmentLootAbilities abilities_tidus;
    [FieldOffset(0x25)] public ChrEquipmentLootAbilities abilities_yuna;
    [FieldOffset(0x45)] public ChrEquipmentLootAbilities abilities_auron;
    [FieldOffset(0x65)] public ChrEquipmentLootAbilities abilities_kimahri;
    [FieldOffset(0x85)] public ChrEquipmentLootAbilities abilities_wakka;
    [FieldOffset(0xA5)] public ChrEquipmentLootAbilities abilities_lulu;
    [FieldOffset(0xC5)] public ChrEquipmentLootAbilities abilities_rikku;
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
