// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core.FFX;

public struct StatusMap {
    public byte death;
    public byte zombie;
    public byte petrification;
    public byte poison;
    public byte power_break;
    public byte magic_break;
    public byte armor_break;
    public byte mental_break;
    public byte confuse;
    public byte berserk;
    public byte provoke;
    public byte threaten;
    public byte sleep;
    public byte silence;
    public byte darkness;
    public byte shell;
    public byte protect;
    public byte reflect;
    public byte nul_tide;
    public byte nul_blaze;
    public byte nul_shock;
    public byte nul_frost;
    public byte regen;
    public byte haste;
    public byte slow;
}

public struct StatusDurationMap {
    public byte sleep;
    public byte silence;
    public byte darkness;
    public byte shell;
    public byte protect;
    public byte reflect;
    public byte nul_tide;
    public byte nul_blaze;
    public byte nul_shock;
    public byte nul_frost;
    public byte regen;
    public byte haste;
    public byte slow;
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

public static partial class FhEnumExt {
    public static bool death        (this StatusPermanentFlags flags) => flags.HasFlag(StatusPermanentFlags.DEATH);
    public static bool zombie       (this StatusPermanentFlags flags) => flags.HasFlag(StatusPermanentFlags.ZOMBIE);
    public static bool petrification(this StatusPermanentFlags flags) => flags.HasFlag(StatusPermanentFlags.PETRIFICATION);
    public static bool poison       (this StatusPermanentFlags flags) => flags.HasFlag(StatusPermanentFlags.POISON);
    public static bool confuse      (this StatusPermanentFlags flags) => flags.HasFlag(StatusPermanentFlags.CONFUSE);
    public static bool berserk      (this StatusPermanentFlags flags) => flags.HasFlag(StatusPermanentFlags.BERSERK);
    public static bool provoke      (this StatusPermanentFlags flags) => flags.HasFlag(StatusPermanentFlags.PROVOKE);
    public static bool threaten     (this StatusPermanentFlags flags) => flags.HasFlag(StatusPermanentFlags.THREATEN);

    public static bool power_break (this StatusPermanentFlags flags) => flags.HasFlag(StatusPermanentFlags.POWER_BREAK);
    public static bool magic_break (this StatusPermanentFlags flags) => flags.HasFlag(StatusPermanentFlags.MAGIC_BREAK);
    public static bool armor_break (this StatusPermanentFlags flags) => flags.HasFlag(StatusPermanentFlags.ARMOR_BREAK);
    public static bool mental_break(this StatusPermanentFlags flags) => flags.HasFlag(StatusPermanentFlags.MENTAL_BREAK);
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

public static partial class FhEnumExt {
    public static bool sleep   (this StatusTemporalFlags flags) => flags.HasFlag(StatusTemporalFlags.SLEEP);
    public static bool silence (this StatusTemporalFlags flags) => flags.HasFlag(StatusTemporalFlags.SILENCE);
    public static bool darkness(this StatusTemporalFlags flags) => flags.HasFlag(StatusTemporalFlags.DARKNESS);

    public static bool shell  (this StatusTemporalFlags flags) => flags.HasFlag(StatusTemporalFlags.SHELL);
    public static bool protect(this StatusTemporalFlags flags) => flags.HasFlag(StatusTemporalFlags.PROTECT);
    public static bool reflect(this StatusTemporalFlags flags) => flags.HasFlag(StatusTemporalFlags.REFLECT);
    public static bool regen  (this StatusTemporalFlags flags) => flags.HasFlag(StatusTemporalFlags.REGEN);
    public static bool haste  (this StatusTemporalFlags flags) => flags.HasFlag(StatusTemporalFlags.HASTE);
    public static bool slow   (this StatusTemporalFlags flags) => flags.HasFlag(StatusTemporalFlags.SLOW);

    public static bool nul_water  (this StatusTemporalFlags flags) => flags.HasFlag(StatusTemporalFlags.NUL_WATER);
    public static bool nul_fire   (this StatusTemporalFlags flags) => flags.HasFlag(StatusTemporalFlags.NUL_FIRE);
    public static bool nul_thunder(this StatusTemporalFlags flags) => flags.HasFlag(StatusTemporalFlags.NUL_THUNDER);
    public static bool nul_ice    (this StatusTemporalFlags flags) => flags.HasFlag(StatusTemporalFlags.NUL_ICE);
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

public static partial class FhEnumExt {
    public static bool distill_power  (this StatusExtraFlags flags) => flags.HasFlag(StatusExtraFlags.DISTILL_POWER);
    public static bool distill_mana   (this StatusExtraFlags flags) => flags.HasFlag(StatusExtraFlags.DISTILL_MANA);
    public static bool distill_speed  (this StatusExtraFlags flags) => flags.HasFlag(StatusExtraFlags.DISTILL_SPEED);
    public static bool distill_move   (this StatusExtraFlags flags) => flags.HasFlag(StatusExtraFlags.DISTILL_MOVE);
    public static bool distill_ability(this StatusExtraFlags flags) => flags.HasFlag(StatusExtraFlags.DISTILL_ABILITY);

    public static bool shield   (this StatusExtraFlags flags) => flags.HasFlag(StatusExtraFlags.SHIELD);
    public static bool boost    (this StatusExtraFlags flags) => flags.HasFlag(StatusExtraFlags.BOOST);
    public static bool scan     (this StatusExtraFlags flags) => flags.HasFlag(StatusExtraFlags.SCAN);
    public static bool eject    (this StatusExtraFlags flags) => flags.HasFlag(StatusExtraFlags.EJECT);
    public static bool auto_life(this StatusExtraFlags flags) => flags.HasFlag(StatusExtraFlags.AUTO_LIFE);
    public static bool curse    (this StatusExtraFlags flags) => flags.HasFlag(StatusExtraFlags.CURSE);
    public static bool defend   (this StatusExtraFlags flags) => flags.HasFlag(StatusExtraFlags.DEFEND);
    public static bool guard    (this StatusExtraFlags flags) => flags.HasFlag(StatusExtraFlags.GUARD);
    public static bool sentinel (this StatusExtraFlags flags) => flags.HasFlag(StatusExtraFlags.SENTINEL);
    public static bool doom     (this StatusExtraFlags flags) => flags.HasFlag(StatusExtraFlags.DOOM);
}

[Flags]
public enum ChrResistFlags : ushort {
    NONE                       =       0,
    ARMORED                    = 1 <<  0,
    IMMUNITY_FRACTIONAL_DAMAGE = 1 <<  1,
    IMMUNITY_LIFE              = 1 <<  2,
    IMMUNITY_SENSOR            = 1 <<  3,
    IMMUNITY_SCAN              = 1 <<  4,
    IMMUNITY_PHYSICAL_DAMAGE   = 1 <<  5,
    IMMUNITY_MAGICAL_DAMAGE    = 1 <<  6,
    IMMUNITY_HP_DAMAGE         = 1 <<  7,
    IMMUNITY_CTB_DAMAGE        = 1 <<  8,
    IMMUNITY_ZANMATO           = 1 <<  9,
    IMMUNITY_BRIBE             = 1 << 10,
}

public static partial class FhEnumExt {
    public static bool is_armored               (this ChrResistFlags flags) => flags.HasFlag(ChrResistFlags.ARMORED);
    public static bool resists_fractional_damage(this ChrResistFlags flags) => flags.HasFlag(ChrResistFlags.IMMUNITY_FRACTIONAL_DAMAGE);
    public static bool resists_life             (this ChrResistFlags flags) => flags.HasFlag(ChrResistFlags.IMMUNITY_LIFE);
    public static bool resists_sensor           (this ChrResistFlags flags) => flags.HasFlag(ChrResistFlags.IMMUNITY_SENSOR);
    public static bool resists_scan             (this ChrResistFlags flags) => flags.HasFlag(ChrResistFlags.IMMUNITY_SCAN);
    public static bool resists_physical_damage  (this ChrResistFlags flags) => flags.HasFlag(ChrResistFlags.IMMUNITY_PHYSICAL_DAMAGE);
    public static bool resists_magical_damage   (this ChrResistFlags flags) => flags.HasFlag(ChrResistFlags.IMMUNITY_MAGICAL_DAMAGE);
    public static bool resists_hp_damage        (this ChrResistFlags flags) => flags.HasFlag(ChrResistFlags.IMMUNITY_HP_DAMAGE);
    public static bool resists_ctb_damage       (this ChrResistFlags flags) => flags.HasFlag(ChrResistFlags.IMMUNITY_CTB_DAMAGE);
    public static bool resists_zanmato          (this ChrResistFlags flags) => flags.HasFlag(ChrResistFlags.IMMUNITY_ZANMATO);
    public static bool resists_bribe            (this ChrResistFlags flags) => flags.HasFlag(ChrResistFlags.IMMUNITY_BRIBE);
}
