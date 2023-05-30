/* [ct2cs 21/5/23 00:09]
 * This file was generated by Fahrenheit.CT2CS (https://github.com/fkelava/fahrenheit/).
 * 
 * Source file: x_btlactor.ct
 */

namespace Fahrenheit.CoreLib;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x118)]
public struct FhXItemMem 
{
    [FieldOffset(0x0)] public ushort stat_get_gill;
    [FieldOffset(0x2)] public ushort stat_get_ap;
    [FieldOffset(0x4)] public ushort stat_get_over_ap;
    [FieldOffset(0x6)] public ushort stat_blue_magic;
    [FieldOffset(0xC)] public ushort stat_item1_com;
    [FieldOffset(0xE)] public ushort stat_item1_rare;
    [FieldOffset(0x10)] public ushort stat_item2_com;
    [FieldOffset(0x12)] public ushort stat_item2_rare;
    [FieldOffset(0x18)] public ushort stat_item1_com_over_kill;
    [FieldOffset(0x1A)] public ushort stat_item1_rare_over_kill;
    [FieldOffset(0x1C)] public ushort stat_item2_com_over_kill;
    [FieldOffset(0x1E)] public ushort stat_item2_rare_over_kill;
    [FieldOffset(0x14)] public byte stat_item1_com_num;
    [FieldOffset(0x15)] public byte stat_item1_rare_num;
    [FieldOffset(0x16)] public byte stat_item2_com_num;
    [FieldOffset(0x17)] public byte stat_item2_rare_num;
    [FieldOffset(0x20)] public byte stat_item1_com_over_kill_num;
    [FieldOffset(0x21)] public byte stat_item1_rare_over_kill_num;
    [FieldOffset(0x22)] public byte stat_item2_com_over_kill_num;
    [FieldOffset(0x23)] public byte stat_item2_rare_over_kill_num;
    [FieldOffset(0x8)] public byte stat_drop1;
    [FieldOffset(0x9)] public byte stat_drop2;
    [FieldOffset(0xB)] public byte stat_weapon_drop;
    [FieldOffset(0xA)] public byte stat_steal;
    [FieldOffset(0x24)] public ushort stat_item;
    [FieldOffset(0x28)] public byte stat_item_num;
    [FieldOffset(0x26)] public ushort stat_rareitem;
    [FieldOffset(0x29)] public byte stat_rareitem_num;
    [FieldOffset(0x112)] public byte stat_monster_value_max;
}
