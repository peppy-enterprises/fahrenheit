using System;

namespace Fahrenheit.CoreLib;

[Flags]
public enum XPlySaveA_B0 : byte
{
    PLY_ABI_POWER_BREAK  = 1 << 0,
    PLY_ABI_MAGIC_BREAK  = 1 << 1,
    PLY_ABI_ARMOR_BREAK  = 1 << 2,
    PLY_ABI_MENTAL_BREAK = 1 << 3,
    PLY_ABI_MUG          = 1 << 4,
    PLY_ABI_QUICK_HIT    = 1 << 5,
    PLY_ABI_STEAL        = 1 << 6,
    PLY_ABI_USE          = 1 << 7
}

[Flags]
public enum XPlySaveA_B1 : byte
{
    PLY_ABI_FLEE   = 1 << 0,
    PLY_ABI_PRAY   = 1 << 1,
    PLY_ABI_CHEER  = 1 << 2,
    PLY_ABI_AIM    = 1 << 3,
    PLY_ABI_FOCUS  = 1 << 4,
    PLY_ABI_REFLEX = 1 << 5,
    PLY_ABI_LUCK   = 1 << 6,
    PLY_ABI_JINX   = 1 << 7
}

[Flags]
public enum XPlySaveA_B2 : byte
{
    PLY_ABI_LANCET      = 1 << 0,
    PLY_ABI_UNUSED      = 1 << 1,
    PLY_ABI_GUARD       = 1 << 2,
    PLY_ABI_SENTINEL    = 1 << 3,
    PLY_ABI_SPARECHANGE = 1 << 4,
    PLY_ABI_THREATEN    = 1 << 5,
    PLY_ABI_PROVOKE     = 1 << 6,
    PLY_ABI_ENTRUST     = 1 << 7
}

[Flags]
public enum XPlySaveA_B3 : byte
{
    PLY_ABI_COPYCAT    = 1 << 0,
    PLY_ABI_DOUBLECAST = 1 << 1,
    PLY_ABI_BRIBE      = 1 << 2,
    PLY_ABI_CURE       = 1 << 3,
    PLY_ABI_CURA       = 1 << 4,
    PLY_ABI_CURAGA     = 1 << 5,
    PLY_ABI_NULFROST   = 1 << 6,
    PLY_ABI_NULBLAZE   = 1 << 7
}

[Flags]
public enum XPlySaveA_B4 : byte
{
    PLY_ABI_NULSHOCK = 1 << 0,
    PLY_ABI_NULTIDE  = 1 << 1,
    PLY_ABI_SCAN     = 1 << 2,
    PLY_ABI_ESUNA    = 1 << 3,
    PLY_ABI_LIFE     = 1 << 4,
    PLY_ABI_FULLLIFE = 1 << 5,
    PLY_ABI_HASTE    = 1 << 6,
    PLY_ABI_HASTEGA  = 1 << 7
}

[Flags]
public enum XPlySaveA_B5 : byte
{
    PLY_ABI_SLOW    = 1 << 0,
    PLY_ABI_SLOWGA  = 1 << 1,
    PLY_ABI_SHELL   = 1 << 2,
    PLY_ABI_PROTECT = 1 << 3,
    PLY_ABI_REFLECT = 1 << 4,
    PLY_ABI_DISPEL  = 1 << 5,
    PLY_ABI_REGEN   = 1 << 6,
    PLY_ABI_HOLY    = 1 << 7
}

[Flags]
public enum XPlySaveA_B6 : byte
{
    PLY_ABI_AUTOLIFE = 1 << 0,
    PLY_ABI_BLIZZARD = 1 << 1,
    PLY_ABI_FIRE     = 1 << 2,
    PLY_ABI_THUNDER  = 1 << 3,
    PLY_ABI_WATER    = 1 << 4,
    PLY_ABI_FIRA     = 1 << 5,
    PLY_ABI_BLIZZARA = 1 << 6,
    PLY_ABI_THUNDARA = 1 << 7
}

[Flags]
public enum XPlySaveA_B7 : byte
{
    PLY_ABI_WATERA   = 1 << 0,
    PLY_ABI_FIRAGA   = 1 << 1,
    PLY_ABI_BLIZZAGA = 1 << 2,
    PLY_ABI_THUNDAGA = 1 << 3,
    PLY_ABI_WATERGA  = 1 << 4,
    PLY_ABI_BIO      = 1 << 5,
    PLY_ABI_DEMI     = 1 << 6,
    PLY_ABI_DEATH    = 1 << 7
}

[Flags]
public enum XPlySaveA_B8 : byte
{
    PLY_ABI_DRAIN  = 1 << 0,
    PLY_ABI_OSMOSE = 1 << 1,
    PLY_ABI_FLARE  = 1 << 2,
    PLY_ABI_ULTIMA = 1 << 3
}

[Flags]
public enum XPlySaveA_B9 : byte
{
    PLY_ABI_PILFER_GIL      = 1 << 0,
    PLY_ABI_FULL_BREAK      = 1 << 1,
    PLY_ABI_EXTRACT_POWER   = 1 << 2,
    PLY_ABI_EXTRACT_MANA    = 1 << 3,
    PLY_ABI_EXTRACT_SPEED   = 1 << 4,
    PLY_ABI_EXTRACT_ABILITY = 1 << 5,
    PLY_ABI_NAB_GIL         = 1 << 6,
    PLY_ABI_QUICK_POCKETS   = 1 << 7
}

[Flags]
public enum XPlySaveOM_B0 : byte
{
    PLY_OVR_MODE_WARRIOR   = 1 << 0,
    PLY_OVR_MODE_COMRADE   = 1 << 1,
    PLY_OVR_MODE_STOIC     = 1 << 2,
    PLY_OVR_MODE_HEALER    = 1 << 3,
    PLY_OVR_MODE_TACTICIAN = 1 << 4,
    PLY_OVR_MODE_VICTIM    = 1 << 5,
    PLY_OVR_MODE_DANCER    = 1 << 6,
    PLY_OVR_MODE_AVENGER   = 1 << 7
}

[Flags]
public enum XPlySaveOM_B1 : byte
{
    PLY_OVR_MODE_SLAYER    = 1 << 0,
    PLY_OVR_MODE_HERO      = 1 << 1,
    PLY_OVR_MODE_ROOK      = 1 << 2,
    PLY_OVR_MODE_VICTOR    = 1 << 3,
    PLY_OVR_MODE_COWARD    = 1 << 4,
    PLY_OVR_MODE_ALLY      = 1 << 5,
    PLY_OVR_MODE_SUFFERER  = 1 << 6,
    PLY_OVR_MODE_DAREDEVIL = 1 << 7
}

[Flags]
public enum XPlySaveOM_B2 : byte
{
    PLY_OVR_MODE_LONER   = 1 << 0,
    PLY_OVR_MODE_UNUSE_1 = 1 << 1,
    PLY_OVR_MODE_UNUSE_2 = 1 << 2,
    PLY_OVR_MODE_AEONS   = 1 << 3,
}

[StructLayout(LayoutKind.Explicit, Size = 0x94)]
public readonly struct FhXPlySave
{
    [FieldOffset(0x00)] public readonly uint          __0x0;
    [FieldOffset(0x04)] public readonly uint          ply_base_hp;
    [FieldOffset(0x08)] public readonly uint          ply_base_mp;
    [FieldOffset(0x0C)] public readonly byte          ply_base_str;
    [FieldOffset(0x0D)] public readonly byte          ply_base_def;
    [FieldOffset(0x0E)] public readonly byte          ply_base_mag;
    [FieldOffset(0x0F)] public readonly byte          ply_base_mdef;
    [FieldOffset(0x10)] public readonly byte          ply_base_agi;
    [FieldOffset(0x11)] public readonly byte          ply_base_luck;
    [FieldOffset(0x12)] public readonly byte          ply_base_eva;
    [FieldOffset(0x13)] public readonly byte          ply_base_acc;
    [FieldOffset(0x14)] public readonly ushort        __0x14;
    [FieldOffset(0x16)] public readonly ushort        __0x18;
    [FieldOffset(0x18)] public readonly uint          ply_cur_ap;
    [FieldOffset(0x1C)] public readonly uint          ply_cur_hp;
    [FieldOffset(0x20)] public readonly uint          ply_cur_mp;
    [FieldOffset(0x24)] public readonly uint          ply_max_hp;
    [FieldOffset(0x28)] public readonly uint          ply_max_mp;
    [FieldOffset(0x2C)] public readonly byte          ply_flags;
    [FieldOffset(0x2D)] public readonly byte          ply_wpn_inv_index;
    [FieldOffset(0x2E)] public readonly byte          ply_arm_inv_index;
    [FieldOffset(0x2F)] public readonly byte          ply_str;
    [FieldOffset(0x30)] public readonly byte          ply_def;
    [FieldOffset(0x31)] public readonly byte          ply_mag;
    [FieldOffset(0x32)] public readonly byte          ply_mdef;
    [FieldOffset(0x33)] public readonly byte          ply_agi;
    [FieldOffset(0x34)] public readonly byte          ply_luck;
    [FieldOffset(0x35)] public readonly byte          ply_eva;
    [FieldOffset(0x36)] public readonly byte          ply_acc;
    [FieldOffset(0x37)] public readonly byte          __0x37;
    [FieldOffset(0x38)] public readonly byte          ply_ovr_mode_select;
    [FieldOffset(0x39)] public readonly byte          ply_ovr_charge_cur_pct;
    [FieldOffset(0x3A)] public readonly byte          ply_ovr_charge_max_pct;
    [FieldOffset(0x3B)] public readonly byte          ply_slv_available;
    [FieldOffset(0x3C)] public readonly byte          ply_slv_expended;
    [FieldOffset(0x3D)] public readonly byte          __0x3D;
    [FieldOffset(0x3E)] public readonly byte          __0x3E;
    [FieldOffset(0x3F)] public readonly byte          __0x3F;
    [FieldOffset(0x40)] public readonly XPlySaveA_B0  ply_abi_map_b0;
    [FieldOffset(0x41)] public readonly XPlySaveA_B1  ply_abi_map_b1;
    [FieldOffset(0x42)] public readonly XPlySaveA_B2  ply_abi_map_b2;
    [FieldOffset(0x43)] public readonly XPlySaveA_B3  ply_abi_map_b3;
    [FieldOffset(0x44)] public readonly XPlySaveA_B4  ply_abi_map_b4;
    [FieldOffset(0x45)] public readonly XPlySaveA_B5  ply_abi_map_b5;
    [FieldOffset(0x46)] public readonly XPlySaveA_B6  ply_abi_map_b6;
    [FieldOffset(0x47)] public readonly XPlySaveA_B7  ply_abi_map_b7;
    [FieldOffset(0x48)] public readonly XPlySaveA_B8  ply_abi_map_b8;
    [FieldOffset(0x49)] public readonly XPlySaveA_B9  ply_abi_map_b9;
    [FieldOffset(0x4A)] public readonly ushort        __0x4A;
    [FieldOffset(0x4C)] public readonly uint          __0x4C;
    [FieldOffset(0x50)] public readonly uint          ply_encounter_count;
    [FieldOffset(0x54)] public readonly uint          ply_enemies_defeated;
    [FieldOffset(0x58)] public readonly uint          __0x58;
    [FieldOffset(0x5C)] public readonly uint          __0x5C;
    [FieldOffset(0x60)] public readonly ushort        ply_ovr_mode_warrior_ctr;
    [FieldOffset(0x62)] public readonly ushort        ply_ovr_mode_comrade_ctr;
    [FieldOffset(0x64)] public readonly ushort        ply_ovr_mode_stoic_ctr;
    [FieldOffset(0x66)] public readonly ushort        ply_ovr_mode_healer_ctr;
    [FieldOffset(0x68)] public readonly ushort        ply_ovr_mode_tactician_ctr;
    [FieldOffset(0x6A)] public readonly ushort        ply_ovr_mode_victim_ctr;
    [FieldOffset(0x6C)] public readonly ushort        ply_ovr_mode_dancer_ctr;
    [FieldOffset(0x6E)] public readonly ushort        ply_ovr_mode_avenger_ctr;
    [FieldOffset(0x70)] public readonly ushort        ply_ovr_mode_slayer_ctr;
    [FieldOffset(0x72)] public readonly ushort        ply_ovr_mode_hero_ctr;
    [FieldOffset(0x74)] public readonly ushort        ply_ovr_mode_rook_ctr;
    [FieldOffset(0x76)] public readonly ushort        ply_ovr_mode_victor_ctr;
    [FieldOffset(0x78)] public readonly ushort        ply_ovr_mode_coward_ctr;
    [FieldOffset(0x7A)] public readonly ushort        ply_ovr_mode_ally_ctr;
    [FieldOffset(0x7C)] public readonly ushort        ply_ovr_mode_sufferer_ctr;
    [FieldOffset(0x7E)] public readonly ushort        ply_ovr_mode_daredevil_ctr;
    [FieldOffset(0x80)] public readonly ushort        ply_ovr_mode_loner_ctr;
    [FieldOffset(0x82)] public readonly ushort        ply_ovr_mode_unuse_1_ctr;
    [FieldOffset(0x84)] public readonly ushort        ply_ovr_mode_unuse_2_ctr;
    [FieldOffset(0x86)] public readonly ushort        ply_ovr_mode_aeon_ctr;
    [FieldOffset(0x88)] public readonly XPlySaveOM_B0 ply_ovr_map_b0;
    [FieldOffset(0x89)] public readonly XPlySaveOM_B1 ply_ovr_map_b1;
    [FieldOffset(0x8A)] public readonly XPlySaveOM_B2 ply_ovr_map_b2;
    [FieldOffset(0x8C)] public readonly uint          __0x8C;
    [FieldOffset(0x90)] public readonly uint          __0x90;
}
