// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x68C0)]
public unsafe struct SaveData {
    public struct Name {
        private fixed byte _name[20];

        public byte* raw { get { fixed (byte* temp = _name) return temp; } }
    }

    [InlineArray(18)]
    public struct NamesGroup {
        private Name _data;
    }

    [InlineArray(200)]
    public struct EquipmentArray {
        private Equipment _data;
    }

    [FieldOffset(0x0)]    public       ushort         current_room_id;
    [FieldOffset(0x2)]    public       ushort         last_room_id;
    [FieldOffset(0x4)]    public       ushort         now_eventjump_map_no;
    [FieldOffset(0x6)]    public       ushort         last_eventjump_map_no;
    [FieldOffset(0x8)]    public       ushort         now_eventjump_map_id;
    [FieldOffset(0xA)]    public       ushort         last_eventjump_map_id;
    [FieldOffset(0xC)]    public       byte           current_spawnpoint;
    [FieldOffset(0xD)]    public       byte           last_spawnpoint;
    [FieldOffset(0xE)]    public       ushort         atel_save_dic_index;
    [FieldOffset(0x10)]   public       byte           atel_battle_scene_group;
    [FieldOffset(0x11)]   public       byte           fade_mode;
    [FieldOffset(0x12)]   public       byte           fade_time;
    [FieldOffset(0x13)]   public       byte           battle_status; // bitfield
    [FieldOffset(0x18)]   public fixed uint           flying_ship_pos[2]; // bitfield
    [FieldOffset(0x20)]   public       byte           atel_is_push_member;
    [FieldOffset(0x21)]   public fixed byte           atel_push_frontline[3];
    [FieldOffset(0x24)]   public       byte           atel_push_party;
    [FieldOffset(0x28)]   public       byte           is_cam_underwater;
    [FieldOffset(0x29)]   public       byte           is_map_underwater;
    [FieldOffset(0x2B)]   public       byte           tk_event_new_game;
    [FieldOffset(0x2C)]   public fixed uint           affection[8];
    [FieldOffset(0x4C)]   public fixed uint           affection_room_flags[20]; // bitfield
    [FieldOffset(0xB4)]   public       ushort         item_map_x; // meaning unknown
    [FieldOffset(0xB6)]   public       ushort         item_map_y; // meaning unknown
    [FieldOffset(0xBC)]   public       int            time;
    [FieldOffset(0xD1)]   public       byte           albhed_rikku;
    [FieldOffset(0xD2)]   public       byte           drop_shadow_mode;
    [FieldOffset(0xD4)]   public       ushort         atel_force_place_id_value;
    [FieldOffset(0xD6)]   public       byte           atel_force_place_id;
    [FieldOffset(0xD7)]   public       byte           atel_water_btl_effect;
    [FieldOffset(0xD8)]   public       uint           on_memory_movie_file_no;
    [FieldOffset(0xDC)]   public       uint           on_memory_movie_mode;
    [FieldOffset(0xE0)]   public       uint           on_memory_movie;
    [FieldOffset(0xE4)]   public       int            rand_encounter_modifiers;
    [FieldOffset(0xE8)]   public       ushort         btl_end_tag_always;
    [FieldOffset(0xEA)]   public       ushort         sphere_monitor;
    [FieldOffset(0xBEC)]  public       ushort         story_progress;
    [FieldOffset(0xC81)]  public       ushort         unlocked_airship_destinations;
    [FieldOffset(0x3D0C)] public       uint           config;
    [FieldOffset(0x3D10)] public       uint           unlocked_primers;
    [FieldOffset(0x3D14)] public       uint           battle_count;
    [FieldOffset(0x3D48)] public       uint           gil;
    [FieldOffset(0x3D58)] public fixed byte           party_order[7];
    [FieldOffset(0x3DA4)] public       uint           yojimbo_compatibility;
    [FieldOffset(0x3DA8)] public       uint           yojimbo_type; // unknown size
    [FieldOffset(0x3DAC)] public       uint           tidus_limit_uses;
    [FieldOffset(0x3DB0)] public       uint           successful_rikku_steals;
    [FieldOffset(0x3DB4)] public       uint           bribe_gil_spent;
    [FieldOffset(0x3DCC)] public fixed byte           event_flags[0x80];
    [FieldOffset(0x3E4C)] public fixed byte           help_flags[0x80]; // Size unconfirmed
    [FieldOffset(0x3ECC)] public fixed T_XCommandId   inventory_ids[70];
    [FieldOffset(0x40CC)] public fixed byte           inventory_counts[70];
    [FieldOffset(0x41CC)] public       ushort         inventory_check; // bitfield of unknown size
    [FieldOffset(0x41EC)] public       byte           inventory_use;   // unused?
    [FieldOffset(0x448C)] public fixed ushort         important_items[8];
    [FieldOffset(0x449C)] public       EquipmentArray equipment;
    [FieldOffset(0x55CC)] public       PlySave        ply_tidus;
    [FieldOffset(0x5660)] public       PlySave        ply_yuna;
    [FieldOffset(0x56F4)] public       PlySave        ply_auron;
    [FieldOffset(0x5788)] public       PlySave        ply_kimahri;
    [FieldOffset(0x581C)] public       PlySave        ply_wakka;
    [FieldOffset(0x58B0)] public       PlySave        ply_lulu;
    [FieldOffset(0x5944)] public       PlySave        ply_rikku;
    [FieldOffset(0x59D8)] public       PlySave        ply_seymour;
    [FieldOffset(0x5A6C)] public       PlySave        ply_valefor;
    [FieldOffset(0x5B00)] public       PlySave        ply_ifrit;
    [FieldOffset(0x5B94)] public       PlySave        ply_ixion;
    [FieldOffset(0x5C28)] public       PlySave        ply_shiva;
    [FieldOffset(0x5CBC)] public       PlySave        ply_bahamut;
    [FieldOffset(0x5D50)] public       PlySave        ply_anima;
    [FieldOffset(0x5DE4)] public       PlySave        ply_yojimbo;
    [FieldOffset(0x5E78)] public       PlySave        ply_cindy;
    [FieldOffset(0x5F0C)] public       PlySave        ply_sandy;
    [FieldOffset(0x5FA0)] public       PlySave        ply_mindy;
    [FieldOffset(0x634C)] public       NamesGroup     character_names;

    public ReadOnlySpan<PlySave> ply_arr => MemoryMarshal.CreateReadOnlySpan(ref ply_tidus, 18);

    public bool get_affection_room_gained(int room_id) {
        return room_id < 0x280 && affection_room_flags[room_id / 32].get_bit(room_id % 32);
    }

    public void set_affection_room_gained(int room_id, bool value) {
        if (room_id >= 0x280) return;
        affection_room_flags[room_id / 32].set_bit(room_id % 32, value);
    }

    public uint get_item_count(int item_id) {
        for (int i = 0; i < 70; i++) {
            if (inventory_ids[i] == item_id) return inventory_counts[i];
        }
        return 0;
    }

    public bool has_key_item(int key_item_id) {
        return key_item_id < 0x80 && important_items[key_item_id / 16].get_bit(key_item_id % 16);
    }

    public void set_key_item(int key_item_id, bool value) {
        if (key_item_id >= 0x80) return;
        important_items[key_item_id / 16].set_bit(key_item_id % 16, value);
    }

    public bool rand_encounters_no_fanfare  { readonly get { return rand_encounter_modifiers.get_bit( 8); } set { rand_encounter_modifiers.set_bit( 8, value); } }
    public bool rand_encounters_no_gameover { readonly get { return rand_encounter_modifiers.get_bit( 9); } set { rand_encounter_modifiers.set_bit( 9, value); } }
    public bool rand_encounters_no_music    { readonly get { return rand_encounter_modifiers.get_bit(10); } set { rand_encounter_modifiers.set_bit(10, value); } }

    public bool battle_status_sys       { readonly get { return battle_status.get_bit(1); } set { battle_status.set_bit(1, value); } }
    public bool battle_status_dummy_enc { readonly get { return battle_status.get_bit(2); } set { battle_status.set_bit(2, value); } }

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

    public bool has_unlocked_airship_destination_baaj_temple    { readonly get { return unlocked_airship_destinations.get_bit( 0); } set { unlocked_airship_destinations.set_bit( 0, value); } }
    public bool has_unlocked_airship_destination_sin            { readonly get { return unlocked_airship_destinations.get_bit( 1); } set { unlocked_airship_destinations.set_bit( 1, value); } }
    public bool has_unlocked_airship_destination_omega_ruins    { readonly get { return unlocked_airship_destinations.get_bit( 2); } set { unlocked_airship_destinations.set_bit( 2, value); } }
    public bool has_unlocked_airship_destination_highbridge     { readonly get { return unlocked_airship_destinations.get_bit( 3); } set { unlocked_airship_destinations.set_bit( 3, value); } }
    public bool has_unlocked_airship_destination_besaid_ruins_1 { readonly get { return unlocked_airship_destinations.get_bit( 4); } set { unlocked_airship_destinations.set_bit( 4, value); } }
    public bool has_unlocked_airship_destination_mushroom_rock  { readonly get { return unlocked_airship_destinations.get_bit( 5); } set { unlocked_airship_destinations.set_bit( 5, value); } }
    public bool has_unlocked_airship_destination_besaid_ruins_2 { readonly get { return unlocked_airship_destinations.get_bit( 6); } set { unlocked_airship_destinations.set_bit( 6, value); } }
    public bool has_unlocked_airship_destination_besaid_falls   { readonly get { return unlocked_airship_destinations.get_bit( 7); } set { unlocked_airship_destinations.set_bit( 7, value); } }
    public bool has_unlocked_airship_destination_miihen_ruins   { readonly get { return unlocked_airship_destinations.get_bit( 8); } set { unlocked_airship_destinations.set_bit( 8, value); } }
    public bool has_unlocked_airship_destination_battle_site    { readonly get { return unlocked_airship_destinations.get_bit( 9); } set { unlocked_airship_destinations.set_bit( 9, value); } }
    public bool has_unlocked_airship_destination_sanubia_sands  { readonly get { return unlocked_airship_destinations.get_bit(10); } set { unlocked_airship_destinations.set_bit(10, value); } }
    public bool has_unlocked_airship_destination_penance        { readonly get { return unlocked_airship_destinations.get_bit(11); } set { unlocked_airship_destinations.set_bit(11, value); } }
}
