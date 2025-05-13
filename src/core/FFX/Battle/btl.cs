namespace Fahrenheit.Core.FFX.Battle;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x28)]
public struct BtlDebugFlags {
    [FieldOffset(0x0)] public bool invincible_mon;
    [FieldOffset(0x1)] public bool invincible_ply;
    [FieldOffset(0x2)] public bool mon_control;

    [FieldOffset(0x4)] public bool free_camera;

    [FieldOffset(0x8)] public bool no_magic_effects;
    [FieldOffset(0x9)] public bool no_mp_cost;

    [FieldOffset(0x10)] public bool no_variance;
    [FieldOffset(0x11)] public bool never_crit;
    [FieldOffset(0x12)] public bool always_hit;

    [FieldOffset(0x14)] public bool always_available_overdrive;
    [FieldOffset(0x15)] public bool always_crit;
    [FieldOffset(0x16)] public bool always_1_dmg;
    [FieldOffset(0x17)] public bool always_9999_dmg;
    [FieldOffset(0x18)] public bool always_99999_dmg;
    [FieldOffset(0x19)] public bool always_rare_steal;
    [FieldOffset(0x1A)] public bool ap_x100;
    [FieldOffset(0x1B)] public bool gil_x100;
    [FieldOffset(0x1C)] public bool never_overkill;
    [FieldOffset(0x1D)] public bool permanent_sensor;
    [FieldOffset(0x1E)] public bool never_charge_overdrive;

    [FieldOffset(0x27)] public bool never_hit;
}

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x2150)]
public unsafe struct Btl {
    [FieldOffset(0x10)]   public       byte   battle_state;
    [FieldOffset(0x12)]   public       byte   battle_trigger;

    [FieldOffset(0x28)]   public       BtlDebugFlags debug;

    [FieldOffset(0x5C)]   public       nint   ptr_command_bin;
    [FieldOffset(0x60)]   public       nint   ptr_monmagic1_bin;
    [FieldOffset(0x64)]   public       nint   ptr_monmagic2_bin;
    [FieldOffset(0x68)]   public       nint   ptr_ply_rom_bin;
    [FieldOffset(0x70)]   public       nint   ptr_item_bin;
    [FieldOffset(0x74)]   public       nint   ptr_a_ability_bin;
    [FieldOffset(0x78)]   public       nint   ptr_sum_assure_bin;
    [FieldOffset(0x7C)]   public       nint   ptr_ctb_base_bin;
    [FieldOffset(0x80)]   public       nint   ptr_magic_bin;
    [FieldOffset(0x84)]   public       nint   ptr_mot_bin;
    [FieldOffset(0x88)]   public       nint   ptr_st_number_bin;
    [FieldOffset(0x90)]   public       nint   ptr_sum_grow_bin;
    [FieldOffset(0x94)]   public       nint   ptr_kaizou_bin;

    [FieldOffset(0x98)]   public       ushort size_command_bin;
    [FieldOffset(0x9A)]   public       ushort size_ply_rom_bin;
    [FieldOffset(0x9E)]   public       ushort size_item_bin;
    [FieldOffset(0xA0)]   public       ushort size_a_ability_bin;
    [FieldOffset(0xA2)]   public       ushort size_sum_assure_bin;
    [FieldOffset(0xA4)]   public       ushort size_ctb_base_bin;
    [FieldOffset(0xA6)]   public       ushort size_magic_bin;
    [FieldOffset(0xA8)]   public       ushort size_st_number_bin;
    [FieldOffset(0xAA)]   public       ushort size_mot_bin;
    [FieldOffset(0xAE)]   public       ushort size_sum_grow_bin;
    [FieldOffset(0xB0)]   public       ushort size_kaizou_bin;

    [FieldOffset(0xF4)]   public       uint   ptr_btl_bin;
    [FieldOffset(0xF8)]   public       uint   ptr_btl_bin_fields;
    [FieldOffset(0xFC)]   public       uint   ptr_btl_bin_encounters;
    [FieldOffset(0x100)]  public       ushort btl_bin_field_count;
    [FieldOffset(0x102)]  public       ushort size_btl_bin;

    [FieldOffset(0x106)] public        byte   grace;
    [FieldOffset(0x108)] public        float  walked_dist;
    [FieldOffset(0x10C)] public        float  walked_dist_total;

    [FieldOffset(0x120)]  public       uint   ptr_btl_bin_cur_field;
    [FieldOffset(0x124)]  public       uint   ptr_btl_bin_cur_encounter;
    [FieldOffset(0x128)]  public       uint   ptr_btl_bin_cur_group;
    [FieldOffset(0x12C)]  public       uint   ptr_btl_bin_cur_formation;
    [FieldOffset(0x15C0)] public       uint   chosen_gil;
    [FieldOffset(0x1984)] public       ushort battlefield_id;
    [FieldOffset(0x1986)] public       ushort field_idx;
    [FieldOffset(0x1988)] public       byte   group_idx;
    [FieldOffset(0x1989)] public       byte   formation_idx;
    [FieldOffset(0x198A)] public fixed byte   field_name[8];

    [FieldOffset(0x1FC5)] public fixed byte   frontline[3];
    [FieldOffset(0x1FD3)] public fixed byte   backline[4];

    [FieldOffset(0x2008)] public       uint   last_com;
    [FieldOffset(0x210C)] public       byte   ambush_state;
    [FieldOffset(0x2121)] public       byte   battle_end_type;
}
