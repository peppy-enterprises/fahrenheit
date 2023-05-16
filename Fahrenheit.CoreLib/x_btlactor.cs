/* [fkelava 13/9/22 08:11]
 * Based entirely on my own observations of game memory and decompilation.
 */

namespace Fahrenheit.CoreLib;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0xF90)]
internal unsafe struct FhXChr
{
    [FieldOffset(0x000)] public byte TEMPLATE;

    [FieldOffset(0x590)] private byte   __0x590;
    [FieldOffset(0x5B8)] private byte   __0x5B8;
    [FieldOffset(0x606)] private ushort __0x606;
    [FieldOffset(0x6BC)] private ushort __0x6BC;
    [FieldOffset(0x6BE)] private ushort __0x6BE;
    [FieldOffset(0x6C0)] private ushort __0x6C0;

    [FieldOffset(0x5D0)] public uint stat_hp;              // 0
    [FieldOffset(0x5D4)] public uint stat_mp;              // 1
    [FieldOffset(0x594)] public uint stat_maxhp;           // 2
    [FieldOffset(0x598)] public uint stat_maxmp;           // 3
    [FieldOffset(0xDCC)] public byte stat_death;           // 4

    public int stat_poison { get { return (__0x606 >> 3) & 1; } } // 5

    [FieldOffset(0xDCE)] public byte stat_stone;           // 6

    public int stat_zombie { get { return (__0x606 >> 1) & 1; } } // 7

    [FieldOffset(0x63D)] public byte stat_weak;            // 8
    [FieldOffset(0x5A8)] public byte stat_str;             // 9 
    [FieldOffset(0x5A9)] public byte stat_vit;             // 10
    [FieldOffset(0x5AA)] public byte stat_mag;             // 11
    [FieldOffset(0x5AB)] public byte stat_spirit;          // 12
    [FieldOffset(0x5AC)] public byte stat_dex;             // 13
    [FieldOffset(0x5AD)] public byte stat_luck;            // 14
    [FieldOffset(0x5AE)] public byte stat_avoid;           // 15
    [FieldOffset(0x5AF)] public byte stat_hit;             // 16
    [FieldOffset(0x5BA)] public byte stat_poison_per;      // 17
    [FieldOffset(0x5BB)] public byte stat_limit_type;      // 18
    [FieldOffset(0x5BC)] public byte stat_limit_gauge;     // 19
    [FieldOffset(0x5BD)] public byte stat_limit_gauge_max; // 20
    [FieldOffset(0xDC8)] public byte stat_inbattle;        // 21

    public int stat_man          { get { return __0x590 & 1; } } // 22
    public int stat_female       { get { return __0x590 & 2; } } // 23
    public int stat_monster      { get { return __0x590 & 4; } } // 24

    [FieldOffset(0x505)] public byte stat_fly;      // 26
    [FieldOffset(0xDEC)] public byte stat_will_die; // 27
    [FieldOffset(0x4FC)] public byte stat_area;     // 28
    [FieldOffset(0x4FE)] public byte stat_pos;      // 29
    [FieldOffset(0x4FF)] public byte stat_far;      // 30
    [FieldOffset(0x504)] public byte stat_group;    // 31

    public int stat_sp_hard      { get { return __0x5B8 & 1; } }        // 32
    public int stat_sp_ratio     { get { return (__0x5B8 >> 1) & 1; } } // 33
    public int stat_sp_zombie    { get { return (__0x5B8 >> 2) & 1; } } // 34
    public int stat_sp_see       { get { return (__0x5B8 >> 3) & 1; } } // 35
    public int stat_sp_live      { get { return (__0x5B8 >> 4) & 1; } } // 36
    public int stat_power_break  { get { return (__0x606 >> 4) & 1; } } // 37
    public int stat_magic_break  { get { return (__0x606 >> 5) & 1; } } // 38
    public int stat_armor_break  { get { return (__0x606 >> 6) & 1; } } // 39
    public int stat_mental_break { get { return (__0x606 >> 7) & 1; } } // 40
    public int stat_confuse      { get { return (__0x606 >> 8) & 1; } } // 41
    public int stat_berserk      { get { return (__0x606 >> 9) & 1; } } // 42
    public int stat_prov         { get { return (__0x606 >> 10) & 1; } } // 43
    public int stat_threat       { get { return (__0x606 >> 11) & 1; } } // 44

    [FieldOffset(0x608)] public byte stat_sleep;     // 45
    [FieldOffset(0x609)] public byte stat_silence;   // 46
    [FieldOffset(0x60A)] public byte stat_dark;      // 47
    [FieldOffset(0x60B)] public byte stat_shell;     // 48
    [FieldOffset(0x60C)] public byte stat_protess;   // 49
    [FieldOffset(0x60D)] public byte stat_reflect;   // 50
    [FieldOffset(0x60E)] public byte stat_bawater;   // 51
    [FieldOffset(0x60F)] public byte stat_bafire;    // 52
    [FieldOffset(0x610)] public byte stat_bathunder; // 53
    [FieldOffset(0x611)] public byte stat_bacold;    // 54
    [FieldOffset(0x612)] public byte stat_regen;     // 55
    [FieldOffset(0x613)] public byte stat_haste;     // 56
    [FieldOffset(0x614)] public byte stat_slow;      // 57

    public int ability_see           { get { return __0x6BC & 1; } }        // 58
    public int ability_lead          { get { return (__0x6BC >> 1) & 1; } } // 59
    public int ability_first         { get { return (__0x6BC >> 2) & 1; } } // 60
    public int ability_counter       { get { return (__0x6BC >> 3) & 1; } } // 61
    public int ability_counter2      { get { return (__0x6BC >> 4) & 1; } } // 62
    public int ability_dark          { get { return (__0x6BC >> 7) & 1; } } // 63

    public int ability_ap2           { get { return (__0x6BC >> 1); } } // 64
    public int ability_exp2          { get { return (__0x6BC >> 1); } } // 65
    
    public int ability_booster       { get { return (__0x6BC >> 6) & 1; } } // 66
    public int ability_magic_counter { get { return (__0x6BC >> 5) & 1; } } // 67
    public int ability_medicine      { get { return (__0x6BC >> 9) & 1; } } // 68
    public int ability_auto_potion   { get { return (__0x6BC >> 10) & 1; } } // 69
    public int ability_auto_cureall  { get { return (__0x6BC >> 11) & 1; } } // 70
    public int ability_auto_phenix   { get { return (__0x6BC >> 12) & 1; } } // 71
    
    public int ability_limitup       { get { return (__0x6BC >> 1); } } // 72
    public int ability_dream         { get { return (__0x6BC >> 1); } } // 73

    public int ability_pierce        { get { return (__0x6BC >> 13) & 1; } } // 74

    public int ability_exchange      { get { return (__0x6BC >> 1); } } // 75

    public int ability_hp_recover    { get { return __0x6BE >> 15; } }      // 76
    public int ability_mp_recover    { get { return __0x6C0 & 1; } }        // 77
    public int ability_nonencount    { get { return (__0x6C0 >> 1) & 1; } } // 78
}
