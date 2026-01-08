// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core.FFX.Battle;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x10)]
public unsafe struct PosAreaSomeInfo {
    [FieldOffset(0x00)] public   uint offset_something;
    [FieldOffset(0x06)] public   byte count_something;
}

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x60)]
public unsafe struct BtlArea {
    [FieldOffset(0x0)]  public   byte    area_type;
    [FieldOffset(0x1)]  public   byte    area_count;
    [FieldOffset(0x4)]  public   byte    count_party_pos;
    [FieldOffset(0x5)]  public   byte    count_aeon_pos;
    [FieldOffset(0x6)]  public   byte    count_enemy_pos;
    [FieldOffset(0x8)]  public   byte    count_some_info;
    [FieldOffset(0x10)] public   uint    offset_party_pos;
    [FieldOffset(0x14)] public   uint    offset_party_run_pos;
    [FieldOffset(0x18)] public   uint    offset_aeon_pos;
    [FieldOffset(0x1C)] public   uint    offset_aeon_run_pos;
    [FieldOffset(0x20)] public   uint    offset_enemy_pos;
    [FieldOffset(0x24)] public   uint    offset_enemy_run_pos;
    [FieldOffset(0x28)] public   uint    offset_some_info;
    [FieldOffset(0x2C)] public   uint    offset_chunk_end;
    [FieldOffset(0x30)] public   Vector4 a;
    [FieldOffset(0x40)] public   Vector4 b;
    [FieldOffset(0x50)] public   Vector4 c;
}

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x60)]
public unsafe struct BtlAreas {
    [FieldOffset(0x0)] private BtlArea areas;
    [FieldOffset(0x0)] public  byte    area_type;
    [FieldOffset(0x1)] public  byte    area_count;

    public ref BtlArea this[int i] {
        get { fixed (BtlArea* pAreas = &areas) { return ref *(pAreas + i); } }
    }

    public Span<Vector4> party_pos(int area) {
        fixed (BtlArea* pBase = &areas)
            return new((Vector4*)((nint)pBase + pBase[area].offset_party_pos), pBase[area].count_party_pos);
    }
    public Span<Vector4> party_run_pos(int area) {
        fixed (BtlArea* pBase = &areas)
            return new((Vector4*)((nint)pBase + pBase[area].offset_party_run_pos), pBase[area].count_party_pos);
    }
    public Span<Vector4> aeon_pos(int area) {
        fixed (BtlArea* pBase = &areas)
            return new((Vector4*)((nint)pBase + pBase[area].offset_aeon_pos), pBase[area].count_aeon_pos);
    }
    public Span<Vector4> aeon_run_pos(int area) {
        fixed (BtlArea* pBase = &areas)
            return new((Vector4*)((nint)pBase + pBase[area].offset_aeon_run_pos), pBase[area].count_aeon_pos);
    }
    public Span<Vector4> enemy_pos(int area) {
        fixed (BtlArea* pBase = &areas)
            return new((Vector4*)((nint)pBase + pBase[area].offset_enemy_pos), pBase[area].count_enemy_pos);
    }
    public Span<Vector4> enemy_run_pos(int area) {
        fixed (BtlArea* pBase = &areas)
            return new((Vector4*)((nint)pBase + pBase[area].offset_enemy_run_pos), pBase[area].count_enemy_pos);
    }
    public Span<PosAreaSomeInfo> some_info(int area) {
        fixed (BtlArea* pBase = &areas)
            return new((PosAreaSomeInfo*)((nint)pBase + pBase[area].offset_some_info), pBase[area].count_some_info);
    }
    public Span<Vector4> something(int area, int some_index) {
        fixed (BtlArea* pBase = &areas) {
            PosAreaSomeInfo info = some_info(area)[some_index];
            return new((Vector4*)((nint)pBase + info.offset_something), info.count_something);
        }
    }

    public Vector4* chunk_end {
        get { fixed (BtlArea* pAreas = &areas) { return (Vector4*)((nint)pAreas + areas.offset_chunk_end); } }
    }
}

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
    [InlineArray(32)]
    public struct CtbList {
        private CtbEntry _data;
    }

    [InlineArray(62)]
    public struct InputCueList {
        private InputCue _data;
    }

    [InlineArray(62)]
    public struct AttackCueList {
        private AttackCue _data;
    }

    [InlineArray(32)]
    public struct ReadCueList {
        private ReadCue _data;
    }

    [InlineArray(14)]
    public struct FieldName {
        private byte _b;
    }

    [FieldOffset(0x10)]   public   byte          battle_state;
    [FieldOffset(0x12)]   public   byte          battle_trigger;

    [FieldOffset(0x28)]   public   BtlDebugFlags debug;

    [FieldOffset(0x5C)]   internal ExcelFile<PCommand>*                  ptr_command_bin;
    [FieldOffset(0x60)]   internal ExcelFile<Command>*                   ptr_monmagic1_bin;
    [FieldOffset(0x64)]   internal ExcelFile<Command>*                   ptr_monmagic2_bin;
    [FieldOffset(0x68)]   internal ExcelFile<PlyRom>*                    ptr_ply_rom_bin;
    [FieldOffset(0x70)]   internal ExcelFile<PCommand>*                  ptr_item_bin;
    [FieldOffset(0x74)]   internal ExcelFile<AutoAbility>*               ptr_a_ability_bin;
    [FieldOffset(0x78)]   internal ExcelFile<AeonStatBattleCountBoosts>* ptr_sum_assure_bin;
    [FieldOffset(0x7C)]   internal ExcelFile<CtbBaseData>*               ptr_ctb_base_bin;

    [FieldOffset(0x80)]   internal nint ptr_magic_bin;
    [FieldOffset(0x84)]   internal nint ptr_mot_bin;

    [FieldOffset(0x88)]   internal ExcelFile<StNumber>*            ptr_st_number_bin;
    [FieldOffset(0x90)]   internal ExcelFile<AeonAbilityRecipe>*   ptr_sum_grow_bin;
    [FieldOffset(0x94)]   internal ExcelFile<CustomizationRecipe>* ptr_kaizou_bin;

    // Fahrenheit overwrites these, and makes the game support nint-sized excel files instead.
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

    [FieldOffset(0xE0)]   public       BtlAreas* ptr_pos_def;

    [FieldOffset(0xF4)]   public       uint   ptr_btl_bin;
    [FieldOffset(0xF8)]   public       uint   ptr_btl_bin_fields;
    [FieldOffset(0xFC)]   public       uint   ptr_btl_bin_encounters;
    [FieldOffset(0x100)]  public       ushort btl_bin_field_count;
    [FieldOffset(0x102)]  public       ushort size_btl_bin;
    [FieldOffset(0x104)]  public       byte   encounter_type;
    [FieldOffset(0x105)]  public       byte   screen_transition;
    [FieldOffset(0x106)]  public       byte   grace;
    [FieldOffset(0x108)]  public       float  walked_dist;
    [FieldOffset(0x10C)]  public       float  walked_dist_total;

    [FieldOffset(0x120)]  public       uint   ptr_btl_bin_cur_field;
    [FieldOffset(0x124)]  public       uint   ptr_btl_bin_cur_encounter;
    [FieldOffset(0x128)]  public       uint   ptr_btl_bin_cur_group;
    [FieldOffset(0x12C)]  public       uint   ptr_btl_bin_cur_formation;

    [FieldOffset(0x130)]  public       CtbList ctb_list;

    [FieldOffset(0x1B0)]  public       InputCueList  input_cues;
    [FieldOffset(0x3a0)]  public       AttackCueList attack_cues;
    [FieldOffset(0x1510)] public       byte          input_cues_size;
    [FieldOffset(0x1511)] public       byte          attack_cues_size;

    // I don't know what this is; `BtlMimic` struct has size 0x48
    // [FieldOffset(0x1568] public        BtlMimic      mimic;

    [FieldOffset(0x15C0)] public       uint   chosen_gil;

    [FieldOffset(0x15D0)] public       ReadCueList   read_cues;
    [FieldOffset(0x1751)] public       byte          read_cues_size;

    // 128 long list of 4-byte structs, something to do with sound
    // [FieldOffset(0x175C)] public       undefined4    sep[128];


    [FieldOffset(0x1984)] public       ushort battlefield_id;
    [FieldOffset(0x1986)] public       ushort field_idx;
    [FieldOffset(0x1988)] public       byte   group_idx;
    [FieldOffset(0x1989)] public       byte   formation_idx;
    [FieldOffset(0x198A)] public FieldName field_name;

    // [FieldOffset(0x1C56)] public undefined2 __0x1C56[200]; // Something related to weapons, model ids?
    // [FieldOffset(0x1DE6)] public short __0x1DE6[112];  // Something related to items
    // [FieldOffset(0x1EC6)] public short __0x1EC6[112];  // Something related to items

    [FieldOffset(0x1FA6)] public fixed byte __0x1FA6[31];
    [FieldOffset(0x1FC5)] public fixed byte __0x1FC5[7];
    [FieldOffset(0x1FCC)] public fixed byte __0x1FCC[7];
    [FieldOffset(0x1FD3)] public fixed byte __0x1FD3[17];
    [FieldOffset(0x1FE4)] public fixed byte __0x1FE4[17];

    [FieldOffset(0x2008)] public       uint   last_com;
    [FieldOffset(0x210C)] public       byte   ambush_state;
    [FieldOffset(0x2121)] public       byte   battle_end_type;
}
