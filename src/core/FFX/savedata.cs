namespace Fahrenheit.Core.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x68C0)]
public unsafe struct SaveData {
    [FieldOffset(0x0)]    public       ushort    current_room_id;
    [FieldOffset(0x2)]    public       ushort    last_room_id;
    [FieldOffset(0x4)]    public       ushort    now_eventjump_map_no;
    [FieldOffset(0x6)]    public       ushort    last_eventjump_map_no;
    [FieldOffset(0x8)]    public       ushort    now_eventjump_map_id;
    [FieldOffset(0xA)]    public       ushort    last_eventjump_map_id;
    [FieldOffset(0xC)]    public       byte      current_spawnpoint;
    [FieldOffset(0xD)]    public       byte      last_spawnpoint;
    [FieldOffset(0xE)]    public       ushort    atel_save_dic_index;
    [FieldOffset(0x10)]   public       byte      atel_battle_scene_group;
    [FieldOffset(0x20)]   public       byte      atel_is_push_member;
    [FieldOffset(0x28)]   public       byte      is_cam_underwater;
    [FieldOffset(0x29)]   public       byte      is_map_underwater;
    [FieldOffset(0x2B)]   public       byte      tk_event_new_game;
    [FieldOffset(0x2C)]   public fixed uint      affection[8];
    [FieldOffset(0xD1)]   public       byte      albhed_rikku;
    [FieldOffset(0xD6)]   public       byte      atel_force_place_id;
    [FieldOffset(0xD7)]   public       byte      atel_water_btl_effect;
    [FieldOffset(0xD8)]   public       uint      on_memory_movie_file_no;
    [FieldOffset(0xDC)]   public       uint      on_memory_movie_mode;
    [FieldOffset(0xE0)]   public       uint      on_memory_movie;
    [FieldOffset(0xE4)]   public       int       rand_encounter_modifiers;
    [FieldOffset(0xE8)]   public       ushort    btl_end_tag_always;
    [FieldOffset(0xEA)]   public       ushort    sphere_monitor;
    [FieldOffset(0xBEC)]  public       ushort    story_progress;
    [FieldOffset(0x3D0C)] public       uint      config;
    [FieldOffset(0x3D10)] public       uint      unlocked_primers;
    [FieldOffset(0x3D14)] public       uint      battle_count;
    [FieldOffset(0x3D48)] public       uint      gil;
    [FieldOffset(0x3D58)] public fixed byte      party_order[7];
    [FieldOffset(0x3DA4)] public       uint      yojimbo_compatibility;
    [FieldOffset(0x3DB0)] public       uint      successful_rikku_steals;
    [FieldOffset(0x3DB4)] public       uint      bribe_gil_spent;
    [FieldOffset(0x3ECC)] public fixed T_XItemId inventory_ids[70];
    [FieldOffset(0x40CC)] public fixed byte      inventory_counts[70];
    [FieldOffset(0x448C)] public       uint      ptr_important_bin;
    [FieldOffset(0x55CC)] public       PlySave   ply_tidus;
    [FieldOffset(0x5660)] public       PlySave   ply_yuna;
    [FieldOffset(0x56F4)] public       PlySave   ply_auron;
    [FieldOffset(0x5788)] public       PlySave   ply_kimahri;
    [FieldOffset(0x581C)] public       PlySave   ply_wakka;
    [FieldOffset(0x58B0)] public       PlySave   ply_lulu;
    [FieldOffset(0x5944)] public       PlySave   ply_rikku;
    [FieldOffset(0x59D8)] public       PlySave   ply_seymour;
    [FieldOffset(0x5A6C)] public       PlySave   ply_valefor;
    [FieldOffset(0x5B00)] public       PlySave   ply_ifrit;
    [FieldOffset(0x5B94)] public       PlySave   ply_ixion;
    [FieldOffset(0x5C28)] public       PlySave   ply_shiva;
    [FieldOffset(0x5CBC)] public       PlySave   ply_bahamut;
    [FieldOffset(0x5D50)] public       PlySave   ply_anima;
    [FieldOffset(0x5DE4)] public       PlySave   ply_yojimbo;
    [FieldOffset(0x5E78)] public       PlySave   ply_cindy;
    [FieldOffset(0x5F0C)] public       PlySave   ply_sandy;
    [FieldOffset(0x5FA0)] public       PlySave   ply_mindy;
    [FieldOffset(0x634C)] public fixed byte      character_names[360]; // each name is at most 20 bytes long. C# won't let me make a 2D array

    public ReadOnlySpan<PlySave> ply_arr => MemoryMarshal.CreateReadOnlySpan(ref ply_tidus, 18);

    public bool rand_encounters_no_fanfare  { readonly get { return rand_encounter_modifiers.get_bit( 8); } set { rand_encounter_modifiers.set_bit( 8, value); } }
    public bool rand_encounters_no_gameover { readonly get { return rand_encounter_modifiers.get_bit( 9); } set { rand_encounter_modifiers.set_bit( 9, value); } }
    public bool rand_encounters_no_music    { readonly get { return rand_encounter_modifiers.get_bit(10); } set { rand_encounter_modifiers.set_bit(10, value); } }

    public bool config_stereo          { readonly get { return config.get_bit ( 0);    } set { config.set_bit ( 0, value);    } }
    public bool config_memory_cursor   { readonly get { return config.get_bit ( 1);    } set { config.set_bit ( 1, value);    } }
    public bool config_controller      { readonly get { return config.get_bit ( 2);    } set { config.set_bit ( 2, value);    } }
    public bool config_hiragana        { readonly get { return config.get_bit ( 3);    } set { config.set_bit ( 3, value);    } }
    public bool config_vibrate         { readonly get { return config.get_bit ( 4);    } set { config.set_bit ( 4, value);    } }
    public bool config_subtitles_names { readonly get { return config.get_bit ( 5);    } set { config.set_bit ( 5, value);    } }
    public bool config_show_map        { readonly get { return config.get_bit ( 6);    } set { config.set_bit ( 6, value);    } }
    public bool config_subtitles       { readonly get { return config.get_bit ( 9);    } set { config.set_bit ( 9, value);    } }
    public bool config_show_help       { readonly get { return config.get_bit (10);    } set { config.set_bit (10, value);    } }
    public bool config_short_aeons     { readonly get { return config.get_bit (11);    } set { config.set_bit (11, value);    } }
    public uint config_grid_type       { readonly get { return config.get_bits(14, 2); } set { config.set_bits(14, 2, value); } } // 0: Original, 1: Standard, 2: Expert, Default: Expert [citation needed]

    public bool has_unlocked_primer_1  { readonly get { return unlocked_primers.get_bit( 0); } set { unlocked_primers.set_bit( 0, value); } }
    public bool has_unlocked_primer_2  { readonly get { return unlocked_primers.get_bit( 1); } set { unlocked_primers.set_bit( 1, value); } }
    public bool has_unlocked_primer_3  { readonly get { return unlocked_primers.get_bit( 2); } set { unlocked_primers.set_bit( 2, value); } }
    public bool has_unlocked_primer_4  { readonly get { return unlocked_primers.get_bit( 3); } set { unlocked_primers.set_bit( 3, value); } }
    public bool has_unlocked_primer_5  { readonly get { return unlocked_primers.get_bit( 4); } set { unlocked_primers.set_bit( 4, value); } }
    public bool has_unlocked_primer_6  { readonly get { return unlocked_primers.get_bit( 5); } set { unlocked_primers.set_bit( 5, value); } }
    public bool has_unlocked_primer_7  { readonly get { return unlocked_primers.get_bit( 6); } set { unlocked_primers.set_bit( 6, value); } }
    public bool has_unlocked_primer_8  { readonly get { return unlocked_primers.get_bit( 7); } set { unlocked_primers.set_bit( 7, value); } }
    public bool has_unlocked_primer_9  { readonly get { return unlocked_primers.get_bit( 8); } set { unlocked_primers.set_bit( 8, value); } }
    public bool has_unlocked_primer_10 { readonly get { return unlocked_primers.get_bit( 9); } set { unlocked_primers.set_bit( 9, value); } }
    public bool has_unlocked_primer_11 { readonly get { return unlocked_primers.get_bit(10); } set { unlocked_primers.set_bit(10, value); } }
    public bool has_unlocked_primer_12 { readonly get { return unlocked_primers.get_bit(11); } set { unlocked_primers.set_bit(11, value); } }
    public bool has_unlocked_primer_13 { readonly get { return unlocked_primers.get_bit(12); } set { unlocked_primers.set_bit(12, value); } }
    public bool has_unlocked_primer_14 { readonly get { return unlocked_primers.get_bit(13); } set { unlocked_primers.set_bit(13, value); } }
    public bool has_unlocked_primer_15 { readonly get { return unlocked_primers.get_bit(14); } set { unlocked_primers.set_bit(14, value); } }
    public bool has_unlocked_primer_16 { readonly get { return unlocked_primers.get_bit(15); } set { unlocked_primers.set_bit(15, value); } }
    public bool has_unlocked_primer_17 { readonly get { return unlocked_primers.get_bit(16); } set { unlocked_primers.set_bit(16, value); } }
    public bool has_unlocked_primer_18 { readonly get { return unlocked_primers.get_bit(17); } set { unlocked_primers.set_bit(17, value); } }
    public bool has_unlocked_primer_19 { readonly get { return unlocked_primers.get_bit(18); } set { unlocked_primers.set_bit(18, value); } }
    public bool has_unlocked_primer_20 { readonly get { return unlocked_primers.get_bit(19); } set { unlocked_primers.set_bit(19, value); } }
    public bool has_unlocked_primer_21 { readonly get { return unlocked_primers.get_bit(20); } set { unlocked_primers.set_bit(20, value); } }
    public bool has_unlocked_primer_22 { readonly get { return unlocked_primers.get_bit(21); } set { unlocked_primers.set_bit(21, value); } }
    public bool has_unlocked_primer_23 { readonly get { return unlocked_primers.get_bit(22); } set { unlocked_primers.set_bit(22, value); } }
    public bool has_unlocked_primer_24 { readonly get { return unlocked_primers.get_bit(23); } set { unlocked_primers.set_bit(23, value); } }
    public bool has_unlocked_primer_25 { readonly get { return unlocked_primers.get_bit(24); } set { unlocked_primers.set_bit(24, value); } }
    public bool has_unlocked_primer_26 { readonly get { return unlocked_primers.get_bit(25); } set { unlocked_primers.set_bit(25, value); } }
}