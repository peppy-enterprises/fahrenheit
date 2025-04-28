namespace Fahrenheit.Core.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x19)]
public struct StatusMap {
    [FieldOffset(0x00)] public byte death;
    [FieldOffset(0x01)] public byte zombie;
    [FieldOffset(0x02)] public byte petrification;
    [FieldOffset(0x03)] public byte poison;
    [FieldOffset(0x04)] public byte power_break;
    [FieldOffset(0x05)] public byte magic_break;
    [FieldOffset(0x06)] public byte armor_break;
    [FieldOffset(0x07)] public byte mental_break;
    [FieldOffset(0x08)] public byte confuse;
    [FieldOffset(0x09)] public byte berserk;
    [FieldOffset(0x0A)] public byte provoke;
    [FieldOffset(0x0B)] public byte threaten;
    [FieldOffset(0x0C)] public byte sleep;
    [FieldOffset(0x0D)] public byte silence;
    [FieldOffset(0x0E)] public byte darkness;
    [FieldOffset(0x0F)] public byte shell;
    [FieldOffset(0x10)] public byte protect;
    [FieldOffset(0x11)] public byte reflect;
    [FieldOffset(0x12)] public byte nul_tide;
    [FieldOffset(0x13)] public byte nul_blaze;
    [FieldOffset(0x14)] public byte nul_shock;
    [FieldOffset(0x15)] public byte nul_frost;
    [FieldOffset(0x16)] public byte regen;
    [FieldOffset(0x17)] public byte haste;
    [FieldOffset(0x18)] public byte slow;
}

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0xD)]
public struct StatusDurationMap {
    [FieldOffset(0x00)] public byte sleep;
    [FieldOffset(0x01)] public byte silence;
    [FieldOffset(0x02)] public byte darkness;
    [FieldOffset(0x03)] public byte shell;
    [FieldOffset(0x04)] public byte protect;
    [FieldOffset(0x05)] public byte reflect;
    [FieldOffset(0x06)] public byte nul_tide;
    [FieldOffset(0x07)] public byte nul_blaze;
    [FieldOffset(0x08)] public byte nul_shock;
    [FieldOffset(0x09)] public byte nul_frost;
    [FieldOffset(0x0A)] public byte regen;
    [FieldOffset(0x0B)] public byte haste;
    [FieldOffset(0x0C)] public byte slow;
}


[Flags]
public enum StatusPermanentFlags : ushort {
    NONE          =       0,
    DEATH         = 1 <<  0,
    ZOMBIE        = 1 <<  1,
    PETRIFICATION = 1 <<  2,
    POISON        = 1 <<  3,
    POWER_BREAK   = 1 <<  4,
    MAGIC_BREAK   = 1 <<  5,
    ARMOR_BREAK   = 1 <<  6,
    MENTAL_BREAK  = 1 <<  7,
    CONFUSE       = 1 <<  8,
    BERSERK       = 1 <<  9,
    PROVOKE       = 1 << 10,
    THREATEN      = 1 << 11,
}
public static partial class EnumExt {
    public static bool death(this StatusPermanentFlags flags)
        => flags.HasFlag(StatusPermanentFlags.DEATH);

    public static bool zombie(this StatusPermanentFlags flags)
        => flags.HasFlag(StatusPermanentFlags.ZOMBIE);
    
    public static bool petrification(this StatusPermanentFlags flags)
        => flags.HasFlag(StatusPermanentFlags.PETRIFICATION);
    
    public static bool poison(this StatusPermanentFlags flags)
        => flags.HasFlag(StatusPermanentFlags.POISON);
    
    public static bool power_break(this StatusPermanentFlags flags)
        => flags.HasFlag(StatusPermanentFlags.POWER_BREAK);
    
    public static bool magic_break(this StatusPermanentFlags flags)
        => flags.HasFlag(StatusPermanentFlags.MAGIC_BREAK);
    
    public static bool armor_break(this StatusPermanentFlags flags)
        => flags.HasFlag(StatusPermanentFlags.ARMOR_BREAK);
    
    public static bool mental_break(this StatusPermanentFlags flags)
        => flags.HasFlag(StatusPermanentFlags.MENTAL_BREAK);
    
    public static bool confuse(this StatusPermanentFlags flags)
        => flags.HasFlag(StatusPermanentFlags.CONFUSE);
    
    public static bool berserk(this StatusPermanentFlags flags)
        => flags.HasFlag(StatusPermanentFlags.BERSERK);
    
    public static bool provoke(this StatusPermanentFlags flags)
        => flags.HasFlag(StatusPermanentFlags.PROVOKE);
    
    public static bool threaten(this StatusPermanentFlags flags)
        => flags.HasFlag(StatusPermanentFlags.THREATEN);
}

[Flags]
public enum StatusTemporalFlags : ushort {
    NONE        =       0,
    SLEEP       = 1 <<  0,
    SILENCE     = 1 <<  1,
    DARKNESS    = 1 <<  2,
    SHELL       = 1 <<  3,
    PROTECT     = 1 <<  4,
    REFLECT     = 1 <<  5,
    NUL_WATER   = 1 <<  6,
    NUL_FIRE    = 1 <<  7,
    NUL_THUNDER = 1 <<  8,
    NUL_ICE     = 1 <<  9,
    REGEN       = 1 << 10,
    HASTE       = 1 << 11,
    SLOW        = 1 << 12,
}
public static partial class EnumExt {
    public static bool sleep(this StatusTemporalFlags flags)
        => flags.HasFlag(StatusTemporalFlags.SLEEP);
    
    public static bool silence(this StatusTemporalFlags flags)
        => flags.HasFlag(StatusTemporalFlags.SILENCE);
    
    public static bool darkness(this StatusTemporalFlags flags)
        => flags.HasFlag(StatusTemporalFlags.DARKNESS);
    
    public static bool shell(this StatusTemporalFlags flags)
        => flags.HasFlag(StatusTemporalFlags.SHELL);
    
    public static bool protect(this StatusTemporalFlags flags)
        => flags.HasFlag(StatusTemporalFlags.PROTECT);
    
    public static bool reflect(this StatusTemporalFlags flags)
        => flags.HasFlag(StatusTemporalFlags.REFLECT);
    
    public static bool nul_water(this StatusTemporalFlags flags)
        => flags.HasFlag(StatusTemporalFlags.NUL_WATER);
    
    public static bool nul_fire(this StatusTemporalFlags flags)
        => flags.HasFlag(StatusTemporalFlags.NUL_FIRE);
    
    public static bool nul_thunder(this StatusTemporalFlags flags)
        => flags.HasFlag(StatusTemporalFlags.NUL_THUNDER);
    
    public static bool nul_ice(this StatusTemporalFlags flags)
        => flags.HasFlag(StatusTemporalFlags.NUL_ICE);
    
    public static bool regen(this StatusTemporalFlags flags)
        => flags.HasFlag(StatusTemporalFlags.REGEN);
    
    public static bool haste(this StatusTemporalFlags flags)
        => flags.HasFlag(StatusTemporalFlags.HASTE);
    
    public static bool slow(this StatusTemporalFlags flags)
        => flags.HasFlag(StatusTemporalFlags.SLOW);
}

[Flags]
public enum StatusExtraFlags : ushort {
    NONE            =       0,
    SCAN            = 1 <<  0,
    DISTILL_POWER   = 1 <<  1,
    DISTILL_MANA    = 1 <<  2,
    DISTILL_SPEED   = 1 <<  3,
    DISTILL_MOVE    = 1 <<  4,
    DISTILL_ABILITY = 1 <<  5,
    SHIELD          = 1 <<  6,
    BOOST           = 1 <<  7,
    EJECT           = 1 <<  8,
    AUTO_LIFE       = 1 <<  9,
    CURSE           = 1 << 10,
    DEFEND          = 1 << 11,
    GUARD           = 1 << 12,
    SENTINEL        = 1 << 13,
    DOOM            = 1 << 14,
}
public static partial class EnumExt {
    public static bool scan(this StatusExtraFlags flags)
        => flags.HasFlag(StatusExtraFlags.SCAN);
    
    public static bool distill_power(this StatusExtraFlags flags)
        => flags.HasFlag(StatusExtraFlags.DISTILL_POWER);
   
    public static bool distill_mana(this StatusExtraFlags flags)
        => flags.HasFlag(StatusExtraFlags.DISTILL_MANA);
    
    public static bool distill_speed(this StatusExtraFlags flags)
        => flags.HasFlag(StatusExtraFlags.DISTILL_SPEED);
    
    public static bool distill_move(this StatusExtraFlags flags)
        => flags.HasFlag(StatusExtraFlags.DISTILL_MOVE);
    
    public static bool distill_ability(this StatusExtraFlags flags)
        => flags.HasFlag(StatusExtraFlags.DISTILL_ABILITY);
    
    public static bool shield(this StatusExtraFlags flags)
        => flags.HasFlag(StatusExtraFlags.SHIELD);
    
    public static bool boost(this StatusExtraFlags flags)
        => flags.HasFlag(StatusExtraFlags.BOOST);
    
    public static bool eject(this StatusExtraFlags flags)
        => flags.HasFlag(StatusExtraFlags.EJECT);
    
    public static bool auto_life(this StatusExtraFlags flags)
        => flags.HasFlag(StatusExtraFlags.AUTO_LIFE);
    
    public static bool curse(this StatusExtraFlags flags)
        => flags.HasFlag(StatusExtraFlags.CURSE);
    
    public static bool defend(this StatusExtraFlags flags)
        => flags.HasFlag(StatusExtraFlags.DEFEND);
    
    public static bool guard(this StatusExtraFlags flags)
        => flags.HasFlag(StatusExtraFlags.GUARD);
    
    public static bool sentinel(this StatusExtraFlags flags)
        => flags.HasFlag(StatusExtraFlags.SENTINEL);
    
    public static bool doom(this StatusExtraFlags flags)
        => flags.HasFlag(StatusExtraFlags.DOOM);
}
