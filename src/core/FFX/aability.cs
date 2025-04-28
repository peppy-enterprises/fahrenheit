namespace Fahrenheit.Core.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x6C)]
public partial struct AutoAbility {
    [FieldOffset(0x00)] public ushort                name_offset;
    [FieldOffset(0x04)] public ushort                dash_offset;
    [FieldOffset(0x08)] public ushort                desc_offset;
    [FieldOffset(0x0C)] public ushort                misc_offset;
    [FieldOffset(0x10)] public bool                  is_sos;
    [FieldOffset(0x11)] public ElementFlags          elem_strike;
    [FieldOffset(0x12)] public ElementFlags          elem_absorb;
    [FieldOffset(0x13)] public ElementFlags          elem_ignore;
    [FieldOffset(0x14)] public ElementFlags          elem_resist;
    [FieldOffset(0x15)] public ElementFlags          elem_weak;
    [FieldOffset(0x16)] public StatusMap             status_inflict;
    [FieldOffset(0x2F)] public StatusDurationMap     status_duration;
    [FieldOffset(0x3C)] public StatusMap             status_resist;
    [FieldOffset(0x55)] public byte                  stat_inc_amount;
    [FieldOffset(0x56)] public StatIncreaseFlags     stat_inc_flags;
    [FieldOffset(0x58)] public StatusPermanentFlags  auto_status_permanent;
    [FieldOffset(0x5A)] public StatusTemporalFlags   auto_status_temporal;
    [FieldOffset(0x5C)] public StatusExtraFlags      auto_status_extra;
    [FieldOffset(0x5E)] public StatusExtraFlags      status_inflict_extra;
    [FieldOffset(0x60)] public StatusExtraFlags      status_resist_extra;
    [FieldOffset(0x62)] public AutoAbilityEffectsMap auto_ability_effects;
    [FieldOffset(0x68)] public byte                  icon;
    [FieldOffset(0x69)] public byte                  group_idx;
    [FieldOffset(0x6A)] public byte                  group_level;
    [FieldOffset(0x6B)] public byte                  international_bonus_idx;

}

[Flags]
public enum StatIncreaseFlags : ushort {
    NONE                =       0,
    STRENGTH            = 1 <<  0,
    DEFENSE             = 1 <<  1,
    MAGIC               = 1 <<  2,
    MAGIC_DEFENSE       = 1 <<  3,
    AGILITY             = 1 <<  4,
    LUCK                = 1 <<  5,
    EVASION             = 1 <<  6,
    ACCURACY            = 1 <<  7,
    HP                  = 1 <<  8,
    MP                  = 1 <<  9,
    STRENGTH_BONUS      = 1 << 10,
    DEFENSE_BONUS       = 1 << 11,
    MAGIC_BONUS         = 1 << 12,
    MAGIC_DEFENSE_BONUS = 1 << 13,
}
public static partial class EnumExt {
    public static bool strength(this StatIncreaseFlags flags)
        => flags.HasFlag(StatIncreaseFlags.STRENGTH);

    public static bool defense(this StatIncreaseFlags flags)
        => flags.HasFlag(StatIncreaseFlags.DEFENSE);
    
    public static bool magic(this StatIncreaseFlags flags)
        => flags.HasFlag(StatIncreaseFlags.MAGIC);
    
    public static bool magic_defense(this StatIncreaseFlags flags)
        => flags.HasFlag(StatIncreaseFlags.MAGIC_DEFENSE);
    
    public static bool agility(this StatIncreaseFlags flags)
        => flags.HasFlag(StatIncreaseFlags.AGILITY);
    
    public static bool luck(this StatIncreaseFlags flags)
        => flags.HasFlag(StatIncreaseFlags.LUCK);
    
    public static bool evasion(this StatIncreaseFlags flags)
        => flags.HasFlag(StatIncreaseFlags.EVASION);
    
    public static bool accuracy(this StatIncreaseFlags flags)
        => flags.HasFlag(StatIncreaseFlags.ACCURACY);
    
    public static bool hp(this StatIncreaseFlags flags)
        => flags.HasFlag(StatIncreaseFlags.HP);
    
    public static bool mp(this StatIncreaseFlags flags)
        => flags.HasFlag(StatIncreaseFlags.MP);
    
    public static bool strength_bonus(this StatIncreaseFlags flags)
        => flags.HasFlag(StatIncreaseFlags.STRENGTH_BONUS);
    
    public static bool defense_bonus(this StatIncreaseFlags flags)
        => flags.HasFlag(StatIncreaseFlags.DEFENSE_BONUS);
    
    public static bool magic_bonus(this StatIncreaseFlags flags)
        => flags.HasFlag(StatIncreaseFlags.MAGIC_BONUS);
    public static bool magic_defense_bonus(this StatIncreaseFlags flags)
        => flags.HasFlag(StatIncreaseFlags.MAGIC_DEFENSE_BONUS);
}
