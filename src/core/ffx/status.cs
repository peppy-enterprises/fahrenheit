// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit.FFX;

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
    extension(StatusPermanentFlags flags) {
        public bool death         => flags.HasFlag(StatusPermanentFlags.DEATH);
        public bool zombie        => flags.HasFlag(StatusPermanentFlags.ZOMBIE);
        public bool petrification => flags.HasFlag(StatusPermanentFlags.PETRIFICATION);
        public bool poison        => flags.HasFlag(StatusPermanentFlags.POISON);
        public bool confuse       => flags.HasFlag(StatusPermanentFlags.CONFUSE);
        public bool berserk       => flags.HasFlag(StatusPermanentFlags.BERSERK);
        public bool provoke       => flags.HasFlag(StatusPermanentFlags.PROVOKE);
        public bool threaten      => flags.HasFlag(StatusPermanentFlags.THREATEN);

        public bool power_break  => flags.HasFlag(StatusPermanentFlags.POWER_BREAK);
        public bool magic_break  => flags.HasFlag(StatusPermanentFlags.MAGIC_BREAK);
        public bool armor_break  => flags.HasFlag(StatusPermanentFlags.ARMOR_BREAK);
        public bool mental_break => flags.HasFlag(StatusPermanentFlags.MENTAL_BREAK);
    }
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
    extension(StatusTemporalFlags flags) {
        public bool sleep    => flags.HasFlag(StatusTemporalFlags.SLEEP);
        public bool silence  => flags.HasFlag(StatusTemporalFlags.SILENCE);
        public bool darkness => flags.HasFlag(StatusTemporalFlags.DARKNESS);

        public bool shell   => flags.HasFlag(StatusTemporalFlags.SHELL);
        public bool protect => flags.HasFlag(StatusTemporalFlags.PROTECT);
        public bool reflect => flags.HasFlag(StatusTemporalFlags.REFLECT);
        public bool regen   => flags.HasFlag(StatusTemporalFlags.REGEN);
        public bool haste   => flags.HasFlag(StatusTemporalFlags.HASTE);
        public bool slow    => flags.HasFlag(StatusTemporalFlags.SLOW);

        public bool nul_water   => flags.HasFlag(StatusTemporalFlags.NUL_WATER);
        public bool nul_fire    => flags.HasFlag(StatusTemporalFlags.NUL_FIRE);
        public bool nul_thunder => flags.HasFlag(StatusTemporalFlags.NUL_THUNDER);
        public bool nul_ice     => flags.HasFlag(StatusTemporalFlags.NUL_ICE);
    }
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
    extension(StatusExtraFlags flags) {
        public bool distill_power   => flags.HasFlag(StatusExtraFlags.DISTILL_POWER);
        public bool distill_mana    => flags.HasFlag(StatusExtraFlags.DISTILL_MANA);
        public bool distill_speed   => flags.HasFlag(StatusExtraFlags.DISTILL_SPEED);
        public bool distill_move    => flags.HasFlag(StatusExtraFlags.DISTILL_MOVE);
        public bool distill_ability => flags.HasFlag(StatusExtraFlags.DISTILL_ABILITY);

        public bool shield    => flags.HasFlag(StatusExtraFlags.SHIELD);
        public bool boost     => flags.HasFlag(StatusExtraFlags.BOOST);
        public bool scan      => flags.HasFlag(StatusExtraFlags.SCAN);
        public bool eject     => flags.HasFlag(StatusExtraFlags.EJECT);
        public bool auto_life => flags.HasFlag(StatusExtraFlags.AUTO_LIFE);
        public bool curse     => flags.HasFlag(StatusExtraFlags.CURSE);
        public bool defend    => flags.HasFlag(StatusExtraFlags.DEFEND);
        public bool guard     => flags.HasFlag(StatusExtraFlags.GUARD);
        public bool sentinel  => flags.HasFlag(StatusExtraFlags.SENTINEL);
        public bool doom      => flags.HasFlag(StatusExtraFlags.DOOM);
    }
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
    extension(ChrResistFlags flags) {
        public bool is_armored                => flags.HasFlag(ChrResistFlags.ARMORED);
        public bool resists_fractional_damage => flags.HasFlag(ChrResistFlags.IMMUNITY_FRACTIONAL_DAMAGE);
        public bool resists_life              => flags.HasFlag(ChrResistFlags.IMMUNITY_LIFE);
        public bool resists_sensor            => flags.HasFlag(ChrResistFlags.IMMUNITY_SENSOR);
        public bool resists_scan              => flags.HasFlag(ChrResistFlags.IMMUNITY_SCAN);
        public bool resists_physical_damage   => flags.HasFlag(ChrResistFlags.IMMUNITY_PHYSICAL_DAMAGE);
        public bool resists_magical_damage    => flags.HasFlag(ChrResistFlags.IMMUNITY_MAGICAL_DAMAGE);
        public bool resists_hp_damage         => flags.HasFlag(ChrResistFlags.IMMUNITY_HP_DAMAGE);
        public bool resists_ctb_damage        => flags.HasFlag(ChrResistFlags.IMMUNITY_CTB_DAMAGE);
        public bool resists_zanmato           => flags.HasFlag(ChrResistFlags.IMMUNITY_ZANMATO);
        public bool resists_bribe             => flags.HasFlag(ChrResistFlags.IMMUNITY_BRIBE);
    }
}
