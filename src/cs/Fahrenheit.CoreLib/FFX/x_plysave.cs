namespace Fahrenheit.CoreLib.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x94)]
public unsafe struct PlySave {
    [FieldOffset(0x00)] private      uint       __0x0;
    [FieldOffset(0x04)] public       uint       base_hp;
    [FieldOffset(0x08)] public       uint       base_mp;
    [FieldOffset(0x0C)] public       byte       base_strength;
    [FieldOffset(0x0D)] public       byte       base_defense;
    [FieldOffset(0x0E)] public       byte       base_magic;
    [FieldOffset(0x0F)] public       byte       base_magic_defense;
    [FieldOffset(0x10)] public       byte       base_agility;
    [FieldOffset(0x11)] public       byte       base_luck;
    [FieldOffset(0x12)] public       byte       base_evasion;
    [FieldOffset(0x13)] public       byte       base_accuracy;
    [FieldOffset(0x14)] private      ushort     __0x14;
    [FieldOffset(0x16)] private      ushort     __0x16;
    [FieldOffset(0x18)] public       uint       ap;
    [FieldOffset(0x1C)] public       uint       hp;
    [FieldOffset(0x20)] public       uint       mp;
    [FieldOffset(0x24)] public       uint       max_hp;
    [FieldOffset(0x28)] public       uint       max_mp;
    [FieldOffset(0x2C)] public       byte       ply_flags;
    [FieldOffset(0x2D)] public       byte       wpn_inv_idx;
    [FieldOffset(0x2E)] public       byte       arm_inv_idx;
    [FieldOffset(0x2F)] public       byte       strength;
    [FieldOffset(0x30)] public       byte       defense;
    [FieldOffset(0x31)] public       byte       magic;
    [FieldOffset(0x32)] public       byte       magic_defense;
    [FieldOffset(0x33)] public       byte       agility;
    [FieldOffset(0x34)] public       byte       luck;
    [FieldOffset(0x35)] public       byte       evasion;
    [FieldOffset(0x36)] public       byte       accuracy;
    [FieldOffset(0x37)] public       byte       poison_dmg;
    [FieldOffset(0x38)] public       byte       ovr_mode_index;
    [FieldOffset(0x39)] public       byte       ovr_charge;
    [FieldOffset(0x3A)] public       byte       ovr_charge_max;
    [FieldOffset(0x3B)] public       byte       slv_available;
    [FieldOffset(0x3C)] public       byte       slv_spent;
    [FieldOffset(0x3D)] private      byte       __0x3D;
    [FieldOffset(0x3E)] public       AbilityMap abi_map;
    [FieldOffset(0x4A)] public       ushort     aabi_map_0x4a;
    [FieldOffset(0x4C)] public       ushort     aabi_map_0x4c;
    [FieldOffset(0x4E)] public       ushort     aabi_map_0x4e;
    [FieldOffset(0x50)] public       uint       battle_count;
    [FieldOffset(0x54)] public       uint       enemies_defeated;
    [FieldOffset(0x58)] private      uint       __0x58;
    [FieldOffset(0x5C)] private      uint       __0x5C;
    [FieldOffset(0x60)] public fixed ushort     ovr_mode_counters[20];
    [FieldOffset(0x88)] public       uint       ovr_mode_flags;
    [FieldOffset(0x8C)] private      uint       __0x8C;
    [FieldOffset(0x90)] private      uint       __0x90;

    public bool join   { readonly get { return ply_flags.get_bit(0); } set { ply_flags.set_bit(0, value); } }
    public bool joined { readonly get { return ply_flags.get_bit(4); } set { ply_flags.set_bit(4, value); } }

    public bool has_sensor            { readonly get { return aabi_map_0x4a.get_bit(0);  } set { aabi_map_0x4a.set_bit(0,  value); } }
    public bool has_first_strike      { readonly get { return aabi_map_0x4a.get_bit(1);  } set { aabi_map_0x4a.set_bit(1,  value); } }
    public bool has_initiative        { readonly get { return aabi_map_0x4a.get_bit(2);  } set { aabi_map_0x4a.set_bit(2,  value); } }
    public bool has_counter_attack    { readonly get { return aabi_map_0x4a.get_bit(3);  } set { aabi_map_0x4a.set_bit(3,  value); } }
    public bool has_evade_and_counter { readonly get { return aabi_map_0x4a.get_bit(4);  } set { aabi_map_0x4a.set_bit(4,  value); } }
    public bool has_magic_counter     { readonly get { return aabi_map_0x4a.get_bit(5);  } set { aabi_map_0x4a.set_bit(5,  value); } }
    public bool has_magic_booster     { readonly get { return aabi_map_0x4a.get_bit(6);  } set { aabi_map_0x4a.set_bit(6,  value); } }
    public bool has_alchemy           { readonly get { return aabi_map_0x4a.get_bit(9);  } set { aabi_map_0x4a.set_bit(9,  value); } }
    public bool has_auto_potion       { readonly get { return aabi_map_0x4a.get_bit(10); } set { aabi_map_0x4a.set_bit(10, value); } }
    public bool has_auto_med          { readonly get { return aabi_map_0x4a.get_bit(11); } set { aabi_map_0x4a.set_bit(11, value); } }
    public bool has_auto_phoenix      { readonly get { return aabi_map_0x4a.get_bit(12); } set { aabi_map_0x4a.set_bit(12, value); } }
    public bool has_piercing          { readonly get { return aabi_map_0x4a.get_bit(13); } set { aabi_map_0x4a.set_bit(13, value); } }
    public bool has_half_mp_cost      { readonly get { return aabi_map_0x4a.get_bit(14); } set { aabi_map_0x4a.set_bit(14, value); } }
    public bool has_one_mp_cost       { readonly get { return aabi_map_0x4a.get_bit(15); } set { aabi_map_0x4a.set_bit(15, value); } }

    public bool has_double_overdrive    { readonly get { return aabi_map_0x4c.get_bit(0);  } set { aabi_map_0x4c.set_bit(0,  value); } }
    public bool has_triple_overdrive    { readonly get { return aabi_map_0x4c.get_bit(1);  } set { aabi_map_0x4c.set_bit(1,  value); } }
    public bool has_sos_overdrive       { readonly get { return aabi_map_0x4c.get_bit(2);  } set { aabi_map_0x4c.set_bit(2,  value); } }
    public bool has_overdrive_to_ap     { readonly get { return aabi_map_0x4c.get_bit(3);  } set { aabi_map_0x4c.set_bit(3,  value); } }
    public bool has_double_ap           { readonly get { return aabi_map_0x4c.get_bit(4);  } set { aabi_map_0x4c.set_bit(4,  value); } }
    public bool has_triple_ap           { readonly get { return aabi_map_0x4c.get_bit(5);  } set { aabi_map_0x4c.set_bit(5,  value); } }
    public bool has_no_ap               { readonly get { return aabi_map_0x4c.get_bit(6);  } set { aabi_map_0x4c.set_bit(6,  value); } }
    public bool has_pickpocket          { readonly get { return aabi_map_0x4c.get_bit(7);  } set { aabi_map_0x4c.set_bit(7,  value); } }
    public bool has_master_thief        { readonly get { return aabi_map_0x4c.get_bit(8);  } set { aabi_map_0x4c.set_bit(8,  value); } }
    public bool has_break_hp_limit      { readonly get { return aabi_map_0x4c.get_bit(9);  } set { aabi_map_0x4c.set_bit(9,  value); } }
    public bool has_break_mp_limit      { readonly get { return aabi_map_0x4c.get_bit(10); } set { aabi_map_0x4c.set_bit(10, value); } }
    public bool has_break_damage_limit  { readonly get { return aabi_map_0x4c.get_bit(11); } set { aabi_map_0x4c.set_bit(11, value); } }
    public bool has_gillionaire         { readonly get { return aabi_map_0x4c.get_bit(14); } set { aabi_map_0x4c.set_bit(14, value); } }
    public bool has_hp_stroll           { readonly get { return aabi_map_0x4c.get_bit(15); } set { aabi_map_0x4c.set_bit(15, value); } }

    public bool has_mp_stroll           { readonly get { return aabi_map_0x4e.get_bit(0); } set { aabi_map_0x4e.set_bit(0, value); } }
    public bool has_no_encounters       { readonly get { return aabi_map_0x4e.get_bit(1); } set { aabi_map_0x4e.set_bit(1, value); } }
    public bool has_capture             { readonly get { return aabi_map_0x4e.get_bit(2); } set { aabi_map_0x4e.set_bit(2, value); } }

    public ushort ovr_mode_ctr_warrior   { readonly get { return ovr_mode_counters[0];  } set { ovr_mode_counters[0]  = value; } }
    public ushort ovr_mode_ctr_comrade   { readonly get { return ovr_mode_counters[1];  } set { ovr_mode_counters[1]  = value; } }
    public ushort ovr_mode_ctr_stoic     { readonly get { return ovr_mode_counters[2];  } set { ovr_mode_counters[2]  = value; } }
    public ushort ovr_mode_ctr_healer    { readonly get { return ovr_mode_counters[3];  } set { ovr_mode_counters[3]  = value; } }
    public ushort ovr_mode_ctr_tactician { readonly get { return ovr_mode_counters[4];  } set { ovr_mode_counters[4]  = value; } }
    public ushort ovr_mode_ctr_victim    { readonly get { return ovr_mode_counters[5];  } set { ovr_mode_counters[5]  = value; } }
    public ushort ovr_mode_ctr_dancer    { readonly get { return ovr_mode_counters[6];  } set { ovr_mode_counters[6]  = value; } }
    public ushort ovr_mode_ctr_avenger   { readonly get { return ovr_mode_counters[7];  } set { ovr_mode_counters[7]  = value; } }
    public ushort ovr_mode_ctr_slayer    { readonly get { return ovr_mode_counters[8];  } set { ovr_mode_counters[8]  = value; } }
    public ushort ovr_mode_ctr_hero      { readonly get { return ovr_mode_counters[9];  } set { ovr_mode_counters[9]  = value; } }
    public ushort ovr_mode_ctr_rook      { readonly get { return ovr_mode_counters[10]; } set { ovr_mode_counters[10] = value; } }
    public ushort ovr_mode_ctr_victor    { readonly get { return ovr_mode_counters[11]; } set { ovr_mode_counters[11] = value; } }
    public ushort ovr_mode_ctr_coward    { readonly get { return ovr_mode_counters[12]; } set { ovr_mode_counters[12] = value; } }
    public ushort ovr_mode_ctr_ally      { readonly get { return ovr_mode_counters[13]; } set { ovr_mode_counters[13] = value; } }
    public ushort ovr_mode_ctr_sufferer  { readonly get { return ovr_mode_counters[14]; } set { ovr_mode_counters[14] = value; } }
    public ushort ovr_mode_ctr_daredevil { readonly get { return ovr_mode_counters[15]; } set { ovr_mode_counters[15] = value; } }
    public ushort ovr_mode_ctr_liner     { readonly get { return ovr_mode_counters[16]; } set { ovr_mode_counters[16] = value; } }
    public ushort ovr_mode_ctr_unused1   { readonly get { return ovr_mode_counters[17]; } set { ovr_mode_counters[17] = value; } }
    public ushort ovr_mode_ctr_unused2   { readonly get { return ovr_mode_counters[18]; } set { ovr_mode_counters[18] = value; } }
    public ushort ovr_mode_ctr_aeons     { readonly get { return ovr_mode_counters[19]; } set { ovr_mode_counters[19] = value; } }

    public bool has_ovr_mode_warrior     { readonly get { return ovr_mode_flags.get_bit(0);  } set { ovr_mode_flags.set_bit(0, value); } }
    public bool has_ovr_mode_comrade     { readonly get { return ovr_mode_flags.get_bit(1);  } set { ovr_mode_flags.set_bit(0, value); } }
    public bool has_ovr_mode_stoic       { readonly get { return ovr_mode_flags.get_bit(2);  } set { ovr_mode_flags.set_bit(0, value); } }
    public bool has_ovr_mode_healer      { readonly get { return ovr_mode_flags.get_bit(3);  } set { ovr_mode_flags.set_bit(0, value); } }
    public bool has_ovr_mode_tactician   { readonly get { return ovr_mode_flags.get_bit(4);  } set { ovr_mode_flags.set_bit(0, value); } }
    public bool has_ovr_mode_victim      { readonly get { return ovr_mode_flags.get_bit(5);  } set { ovr_mode_flags.set_bit(0, value); } }
    public bool has_ovr_mode_dancer      { readonly get { return ovr_mode_flags.get_bit(6);  } set { ovr_mode_flags.set_bit(0, value); } }
    public bool has_ovr_mode_avenger     { readonly get { return ovr_mode_flags.get_bit(7);  } set { ovr_mode_flags.set_bit(0, value); } }
    public bool has_ovr_mode_slayer      { readonly get { return ovr_mode_flags.get_bit(8);  } set { ovr_mode_flags.set_bit(0, value); } }
    public bool has_ovr_mode_hero        { readonly get { return ovr_mode_flags.get_bit(9);  } set { ovr_mode_flags.set_bit(0, value); } }
    public bool has_ovr_mode_rook        { readonly get { return ovr_mode_flags.get_bit(10); } set { ovr_mode_flags.set_bit(0, value); } }
    public bool has_ovr_mode_victor      { readonly get { return ovr_mode_flags.get_bit(11); } set { ovr_mode_flags.set_bit(0, value); } }
    public bool has_ovr_mode_coward      { readonly get { return ovr_mode_flags.get_bit(12); } set { ovr_mode_flags.set_bit(0, value); } }
    public bool has_ovr_mode_ally        { readonly get { return ovr_mode_flags.get_bit(13); } set { ovr_mode_flags.set_bit(0, value); } }
    public bool has_ovr_mode_sufferer    { readonly get { return ovr_mode_flags.get_bit(14); } set { ovr_mode_flags.set_bit(0, value); } }
    public bool has_ovr_mode_daredevil   { readonly get { return ovr_mode_flags.get_bit(15); } set { ovr_mode_flags.set_bit(0, value); } }
    public bool has_ovr_mode_loner       { readonly get { return ovr_mode_flags.get_bit(16); } set { ovr_mode_flags.set_bit(0, value); } }
    public bool has_ovr_mode_unused1     { readonly get { return ovr_mode_flags.get_bit(17); } set { ovr_mode_flags.set_bit(0, value); } }
    public bool has_ovr_mode_unused2     { readonly get { return ovr_mode_flags.get_bit(18); } set { ovr_mode_flags.set_bit(0, value); } }
    public bool has_ovr_mode_aeons       { readonly get { return ovr_mode_flags.get_bit(19); } set { ovr_mode_flags.set_bit(0, value); } }
}