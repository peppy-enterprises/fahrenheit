namespace Fahrenheit.CoreLib.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x68C0)]
public unsafe struct SaveDataStruct {
    [FieldOffset(0x0)] public ushort now_eventjump_id;
    [FieldOffset(0x2)] public ushort last_eventjump_id;
    [FieldOffset(0x4)] public ushort now_eventjump_map_no;
    [FieldOffset(0x6)] public ushort last_eventjump_map_no;
    [FieldOffset(0x8)] public ushort now_eventjump_map_id;
    [FieldOffset(0xA)] public ushort last_eventjump_map_id;
    [FieldOffset(0xC)] public byte now_eventjump_idx;
    [FieldOffset(0xD)] public byte last_eventjump_idx;
    [FieldOffset(0xE)] public ushort atel_save_dic_index;
    [FieldOffset(0x10)] public byte atel_battle_scene_group;
    [FieldOffset(0x20)] public byte atel_is_push_member;
    [FieldOffset(0x28)] public byte cam_is_underwater;
    [FieldOffset(0x29)] public byte map_is_underwater;
    [FieldOffset(0x2B)] public byte tk_event_new_game;
    [FieldOffset(0x2C)] public fixed uint affection[8];
    [FieldOffset(0xD1)] public byte albhed_rikku;
    [FieldOffset(0xD6)] public byte atel_force_place_id;
    [FieldOffset(0xD7)] public byte atel_water_btl_effect;
    [FieldOffset(0xD8)] public uint on_memory_movie_file_no;
    [FieldOffset(0xDC)] public uint on_memory_movie_mode;
    [FieldOffset(0xE0)] public uint on_memory_movie;
    [FieldOffset(0xE4)] public int rand_encounter_modifiers;
    public bool rand_encounters_no_fanfare  { get => rand_encounter_modifiers.get_bit( 8); set => rand_encounter_modifiers.set_bit( 8, value); }
    public bool rand_encounters_no_gameover { get => rand_encounter_modifiers.get_bit( 9); set => rand_encounter_modifiers.set_bit( 9, value); }
    public bool rand_encounters_no_music    { get => rand_encounter_modifiers.get_bit(10); set => rand_encounter_modifiers.set_bit(10, value); }

    [FieldOffset(0xE8)] public ushort btl_end_tag_always;
    [FieldOffset(0xEA)] public ushort sphere_monitor;
    [FieldOffset(0x3D0C)] public uint config;
    public bool config_stereo          { get => config.get_bit( 0); set => config.set_bit( 0, value); }
    public bool config_memory_cursor   { get => config.get_bit( 1); set => config.set_bit( 1, value); }
    public bool config_controller      { get => config.get_bit( 2); set => config.set_bit( 2, value); }
    public bool config_hiragana        { get => config.get_bit( 3); set => config.set_bit( 3, value); }
    public bool config_vibrate         { get => config.get_bit( 4); set => config.set_bit( 4, value); }
    public bool config_subtitles_names { get => config.get_bit( 5); set => config.set_bit( 5, value); }
    public bool config_show_map        { get => config.get_bit( 6); set => config.set_bit( 6, value); }
    public bool config_subtitles       { get => config.get_bit( 9); set => config.set_bit( 9, value); }
    public bool config_show_help       { get => config.get_bit(10); set => config.set_bit(10, value); }
    public bool config_short_aeons     { get => config.get_bit(11); set => config.set_bit(11, value); }
    // 0: Original, 1: Standard, 2: Expert, Default: Expert
    // not sure how to do that abstraction, probably an enum?
    public uint config_grid_type	   { get => config.get_bits(14, 2); set => config.set_bits(14, 2, value); }

    [FieldOffset(0x3D10)] public uint unlocked_primers;
    public bool unlocked_primer_1  { get => unlocked_primers.get_bit( 0); set => unlocked_primers.set_bit( 0, value); }
    public bool unlocked_primer_2  { get => unlocked_primers.get_bit( 1); set => unlocked_primers.set_bit( 1, value); }
    public bool unlocked_primer_3  { get => unlocked_primers.get_bit( 2); set => unlocked_primers.set_bit( 2, value); }
    public bool unlocked_primer_4  { get => unlocked_primers.get_bit( 3); set => unlocked_primers.set_bit( 3, value); }
    public bool unlocked_primer_5  { get => unlocked_primers.get_bit( 4); set => unlocked_primers.set_bit( 4, value); }
    public bool unlocked_primer_6  { get => unlocked_primers.get_bit( 5); set => unlocked_primers.set_bit( 5, value); }
    public bool unlocked_primer_7  { get => unlocked_primers.get_bit( 6); set => unlocked_primers.set_bit( 6, value); }
    public bool unlocked_primer_8  { get => unlocked_primers.get_bit( 7); set => unlocked_primers.set_bit( 7, value); }
    public bool unlocked_primer_9  { get => unlocked_primers.get_bit( 8); set => unlocked_primers.set_bit( 8, value); }
    public bool unlocked_primer_10 { get => unlocked_primers.get_bit( 9); set => unlocked_primers.set_bit( 9, value); }
    public bool unlocked_primer_11 { get => unlocked_primers.get_bit(10); set => unlocked_primers.set_bit(10, value); }
    public bool unlocked_primer_12 { get => unlocked_primers.get_bit(11); set => unlocked_primers.set_bit(11, value); }
    public bool unlocked_primer_13 { get => unlocked_primers.get_bit(12); set => unlocked_primers.set_bit(12, value); }
    public bool unlocked_primer_14 { get => unlocked_primers.get_bit(13); set => unlocked_primers.set_bit(13, value); }
    public bool unlocked_primer_15 { get => unlocked_primers.get_bit(14); set => unlocked_primers.set_bit(14, value); }
    public bool unlocked_primer_16 { get => unlocked_primers.get_bit(15); set => unlocked_primers.set_bit(15, value); }
    public bool unlocked_primer_17 { get => unlocked_primers.get_bit(16); set => unlocked_primers.set_bit(16, value); }
    public bool unlocked_primer_18 { get => unlocked_primers.get_bit(17); set => unlocked_primers.set_bit(17, value); }
    public bool unlocked_primer_19 { get => unlocked_primers.get_bit(18); set => unlocked_primers.set_bit(18, value); }
    public bool unlocked_primer_20 { get => unlocked_primers.get_bit(19); set => unlocked_primers.set_bit(19, value); }
    public bool unlocked_primer_21 { get => unlocked_primers.get_bit(20); set => unlocked_primers.set_bit(20, value); }
    public bool unlocked_primer_22 { get => unlocked_primers.get_bit(21); set => unlocked_primers.set_bit(21, value); }
    public bool unlocked_primer_23 { get => unlocked_primers.get_bit(22); set => unlocked_primers.set_bit(22, value); }
    public bool unlocked_primer_24 { get => unlocked_primers.get_bit(23); set => unlocked_primers.set_bit(23, value); }
    public bool unlocked_primer_25 { get => unlocked_primers.get_bit(24); set => unlocked_primers.set_bit(24, value); }
    public bool unlocked_primer_26 { get => unlocked_primers.get_bit(25); set => unlocked_primers.set_bit(25, value); }

    [FieldOffset(0x3D14)] public uint battle_count;
    [FieldOffset(0x3D48)] public uint gil;
    [FieldOffset(0x3D58)] public fixed byte party_order[7];
    [FieldOffset(0x3DA4)] public uint yojimbo_compatibility;
    [FieldOffset(0x3DB0)] public uint successful_rikku_steals;
    [FieldOffset(0x3DB4)] public uint bribe_gil_spent;
    [FieldOffset(0x3ECC)] public fixed ushort inventory_ids[70];
    [FieldOffset(0x40CC)] public fixed byte inventory_counts[70];
    [FieldOffset(0x448C)] public uint important_bin_ptr;
    [FieldOffset(0x55CC)] public PlySave ply_tidus;
    [FieldOffset(0x5660)] public PlySave ply_yuna;
    [FieldOffset(0x56F4)] public PlySave ply_auron;
    [FieldOffset(0x5788)] public PlySave ply_kimahri;
    [FieldOffset(0x581C)] public PlySave ply_wakka;
    [FieldOffset(0x58B0)] public PlySave ply_lulu;
    [FieldOffset(0x5944)] public PlySave ply_rikku;
    [FieldOffset(0x59D8)] public PlySave ply_seymour;
    [FieldOffset(0x5A6C)] public PlySave ply_valefor;
    [FieldOffset(0x5B00)] public PlySave ply_ifrit;
    [FieldOffset(0x5B94)] public PlySave ply_ixion;
    [FieldOffset(0x5C28)] public PlySave ply_shiva;
    [FieldOffset(0x5CBC)] public PlySave ply_bahamut;
    [FieldOffset(0x5D50)] public PlySave ply_anima;
    [FieldOffset(0x5DE4)] public PlySave ply_yojimbo;
    [FieldOffset(0x5E78)] public PlySave ply_cindy;
    [FieldOffset(0x5F0C)] public PlySave ply_sandy;
    [FieldOffset(0x5FA0)] public PlySave ply_mindy;
    public PlySave* get_ply_at(int idx) {
        fixed (PlySave* arr = &ply_tidus) {
            return arr + idx;
        }
    }
}
