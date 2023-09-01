using System.Runtime.CompilerServices;

namespace Fahrenheit.CoreLib;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x68C0)]
public unsafe struct FhXSaveDataStruct {
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
	public bool rand_encounters_no_fanfare { get { return (rand_encounter_modifiers >> 8 & 1) != 0; } set { if (value) { rand_encounter_modifiers |= 0x100; } else { rand_encounter_modifiers &= ~0x100; }  ; } }
	public bool rand_encounters_no_gameover { get { return (rand_encounter_modifiers >> 9 & 1) != 0; } set { if (value) { rand_encounter_modifiers |= 0x200; } else { rand_encounter_modifiers &= ~0x200; }  ; } }
	public bool rand_encounters_no_music { get { return (rand_encounter_modifiers >> 10 & 1) != 0; } set { if (value) { rand_encounter_modifiers |= 0x400; } else { rand_encounter_modifiers &= ~0x400; }  ; } }
	
	[FieldOffset(0xE8)] public ushort btl_end_tag_always;
	[FieldOffset(0xEA)] public ushort sphere_monitor;
	[FieldOffset(0x3D14)] public uint battle_count;
	[FieldOffset(0x3D48)] public uint gil;
	[FieldOffset(0x3D58)] public fixed byte party_order[7];
	[FieldOffset(0x3DA4)] public uint yojimbo_compatibility;
	[FieldOffset(0x3DB0)] public uint successful_rikku_steals;
	[FieldOffset(0x3DB4)] public uint bribe_gil_spent;
	[FieldOffset(0x3ECC)] public fixed ushort inventory_ids[70];
	[FieldOffset(0x40CC)] public fixed byte inventory_counts[70];
	[FieldOffset(0x448C)] public uint important_bin_ptr;
	[FieldOffset(0x55CC)] public fixed byte ply_arr[0xA68]; // Is actually FhXPlySave[18] but C# won't let me make an array of those
	[FieldOffset(0x55CC)] public FhXPlySave ply_tidus;
	[FieldOffset(0x5660)] public FhXPlySave ply_yuna;
	[FieldOffset(0x56F4)] public FhXPlySave ply_auron;
	[FieldOffset(0x5788)] public FhXPlySave ply_kimahri;
	[FieldOffset(0x581C)] public FhXPlySave ply_wakka;
	[FieldOffset(0x58B0)] public FhXPlySave ply_lulu;
	[FieldOffset(0x5944)] public FhXPlySave ply_rikku;
	[FieldOffset(0x59D8)] public FhXPlySave ply_seymour;
	[FieldOffset(0x5A6C)] public FhXPlySave ply_valefor;
	[FieldOffset(0x5B00)] public FhXPlySave ply_ifrit;
	[FieldOffset(0x5B94)] public FhXPlySave ply_ixion;
	[FieldOffset(0x5C28)] public FhXPlySave ply_shiva;
	[FieldOffset(0x5CBC)] public FhXPlySave ply_bahamut;
	[FieldOffset(0x5D50)] public FhXPlySave ply_anima;
	[FieldOffset(0x5DE4)] public FhXPlySave ply_yojimbo;
	[FieldOffset(0x5E78)] public FhXPlySave ply_cindy;
	[FieldOffset(0x5F0C)] public FhXPlySave ply_sandy;
	[FieldOffset(0x5FA0)] public FhXPlySave ply_mindy;
}