// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit.FFX2;

[StructLayout(LayoutKind.Sequential)]
public struct StatusMap {
    public byte death;
    public byte petrification;
    public byte sleep;
    public byte silence;
    public byte darkness;
    public byte poison;
    public byte confusion;
    public byte berserk;
    public byte curse;
    public byte sentinel;
    public byte eject;
    public byte double_hp;
    public byte double_mp;
    public byte spellspring;
    public byte damage_9999;
    public byte always_critical;
    public byte pointless;
    public byte itchy;
    public byte auto_life;
    public byte unused1;
    public byte unused2;
    public byte unused3;
    public byte unused4;
    public byte unused5;
}

[StructLayout(LayoutKind.Sequential)]
public struct StatusDurationMap {
    public int death;
    public int petrification;
    public int sleep;
    public int silence;
    public int darkness;
    public int poison;
    public int confusion;
    public int berserk;
    public int curse;
    public int sentinel;
    public int eject;
    public int double_hp;
    public int double_mp;
    public int spellspring;
    public int damage_9999;
    public int always_critical;
    public int pointless;
    public int itchy;
    public int auto_life;
    public int unused1;
    public int unused2;
    public int unused3;
    public int unused4;
    public int unused5;
}

[Flags]
public enum StatusFlags : uint {
    NONE            =      0,
    DEATH           = 1 << 0,
    PETRIFICATION   = 1 << 1,
    SLEEP           = 1 << 2,
    SILENCE         = 1 << 3,
    DARKNESS        = 1 << 4,
    POISON          = 1 << 5,
    CONFUSION       = 1 << 6,
    BERSERK         = 1 << 7,
    CURSE           = 1 << 8,
    SENTINEL        = 1 << 9,
    EJECT           = 1 << 10,
    DOUBLE_HP       = 1 << 11,
    DOUBLE_MP       = 1 << 12,
    SPELLSPRING     = 1 << 13,
    DAMAGE_9999     = 1 << 14,
    ALWAYS_CRITICAL = 1 << 15,
    POINTLESS       = 1 << 16,
    ITCHY           = 1 << 17,
    AUTO_LIFE       = 1 << 18,
    UNUSED1         = 1 << 19,
    UNUSED2         = 1 << 20,
    UNUSED3         = 1 << 21,
    UNUSED4         = 1 << 22,
    UNUSED5         = 1 << 23,
}

public static partial class FhEnumExt {
    extension(StatusFlags flags) {
        public bool death {
            get { return flags.HasFlag(StatusFlags.DEATH); }
            set { if (value) flags |= StatusFlags.DEATH; else flags &= ~StatusFlags.DEATH; }
        }

        public bool petrification {
            get { return flags.HasFlag(StatusFlags.PETRIFICATION); }
            set { if (value) flags |= StatusFlags.PETRIFICATION; else flags &= ~StatusFlags.PETRIFICATION; }
        }

        public bool sleep {
            get { return flags.HasFlag(StatusFlags.SLEEP); }
            set { if (value) flags |= StatusFlags.SLEEP; else flags &= ~StatusFlags.SLEEP; }
        }

        public bool silence {
            get { return flags.HasFlag(StatusFlags.SILENCE); }
            set { if (value) flags |= StatusFlags.SILENCE; else flags &= ~StatusFlags.SILENCE; }
        }

        public bool poison {
            get { return flags.HasFlag(StatusFlags.POISON); }
            set { if (value) flags |= StatusFlags.POISON; else flags &= ~StatusFlags.POISON; }
        }

        public bool confusion {
            get { return flags.HasFlag(StatusFlags.CONFUSION); }
            set { if (value) flags |= StatusFlags.CONFUSION; else flags &= ~StatusFlags.CONFUSION; }
        }

        public bool berserk {
            get { return flags.HasFlag(StatusFlags.BERSERK); }
            set { if (value) flags |= StatusFlags.BERSERK; else flags &= ~StatusFlags.BERSERK; }
        }

        public bool curse {
            get { return flags.HasFlag(StatusFlags.CURSE); }
            set { if (value) flags |= StatusFlags.CURSE; else flags &= ~StatusFlags.CURSE; }
        }

        public bool sentinel {
            get { return flags.HasFlag(StatusFlags.SENTINEL); }
            set { if (value) flags |= StatusFlags.SENTINEL; else flags &= ~StatusFlags.SENTINEL; }
        }

        public bool eject {
            get { return flags.HasFlag(StatusFlags.EJECT); }
            set { if (value) flags |= StatusFlags.EJECT; else flags &= ~StatusFlags.EJECT; }
        }

        public bool doubleHP {
            get { return flags.HasFlag(StatusFlags.DOUBLE_HP); }
            set { if (value) flags |= StatusFlags.DOUBLE_HP; else flags &= ~StatusFlags.DOUBLE_HP; }
        }

        public bool doubleMP {
            get { return flags.HasFlag(StatusFlags.DOUBLE_MP); }
            set { if (value) flags |= StatusFlags.DOUBLE_MP; else flags &= ~StatusFlags.DOUBLE_MP; }
        }

        public bool spellspring {
            get { return flags.HasFlag(StatusFlags.SPELLSPRING); }
            set { if (value) flags |= StatusFlags.SPELLSPRING; else flags &= ~StatusFlags.SPELLSPRING; }
        }

        public bool damage9999 {
            get { return flags.HasFlag(StatusFlags.DAMAGE_9999); }
            set { if (value) flags |= StatusFlags.DAMAGE_9999; else flags &= ~StatusFlags.DAMAGE_9999; }
        }

        public bool alwaysCritical {
            get { return flags.HasFlag(StatusFlags.ALWAYS_CRITICAL); }
            set { if (value) flags |= StatusFlags.ALWAYS_CRITICAL; else flags &= ~StatusFlags.ALWAYS_CRITICAL; }
        }

        public bool pointless {
            get { return flags.HasFlag(StatusFlags.POINTLESS); }
            set { if (value) flags |= StatusFlags.POINTLESS; else flags &= ~StatusFlags.POINTLESS; }
        }

        public bool itchy {
            get { return flags.HasFlag(StatusFlags.ITCHY); }
            set { if (value) flags |= StatusFlags.ITCHY; else flags &= ~StatusFlags.ITCHY; }
        }

        public bool autoLife {
            get { return flags.HasFlag(StatusFlags.AUTO_LIFE); }
            set { if (value) flags |= StatusFlags.AUTO_LIFE; else flags &= ~StatusFlags.AUTO_LIFE; }
        }

        public bool unused1 {
            get { return flags.HasFlag(StatusFlags.UNUSED1); }
            set { if (value) flags |= StatusFlags.UNUSED1; else flags &= ~StatusFlags.UNUSED1; }
        }

        public bool unused2 {
            get { return flags.HasFlag(StatusFlags.UNUSED2); }
            set { if (value) flags |= StatusFlags.UNUSED2; else flags &= ~StatusFlags.UNUSED2; }
        }

        public bool unused3 {
            get { return flags.HasFlag(StatusFlags.UNUSED3); }
            set { if (value) flags |= StatusFlags.UNUSED3; else flags &= ~StatusFlags.UNUSED3; }
        }

        public bool unused4 {
            get { return flags.HasFlag(StatusFlags.UNUSED4); }
            set { if (value) flags |= StatusFlags.UNUSED4; else flags &= ~StatusFlags.UNUSED4; }
        }

        public bool unused5 {
            get { return flags.HasFlag(StatusFlags.UNUSED5); }
            set { if (value) flags |= StatusFlags.UNUSED5; else flags &= ~StatusFlags.UNUSED5; }
        }
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct StatusMap2 {
    public byte shell;
    public byte protect;
    public byte reflect;
    public byte regen;
    public byte haste;
    public byte slow;
    public byte stop;
    public byte strength_up_down;
    public byte magic_up_down;
    public byte defence_up_down;
    public byte magic_defence_up_down;
    public byte accuracy_up_down;
    public byte evasion_up_down;
    public byte luck_up_down;
    public byte doom_count;
    public byte nul_physical;
    public byte nul_magical;
    public byte invincible;
    public byte unused1;
    public byte unused2;
    public byte unused3;
    public byte unused4;
    public byte unused5;
    public byte unused6;
}

[StructLayout(LayoutKind.Sequential)]
public struct StatusDurationMap2 {
    public sbyte shell;
    public sbyte protect;
    public sbyte reflect;
    public sbyte regen;
    public sbyte haste;
    public sbyte slow;
    public sbyte stop;
    public sbyte strength_up_down;
    public sbyte magic_up_down;
    public sbyte defence_up_down;
    public sbyte magic_defence_up_down;
    public sbyte accuracy_up_down;
    public sbyte evasion_up_down;
    public sbyte luck_up_down;
    public sbyte doom_count;
    public sbyte nul_physical;
    public sbyte nul_magical;
    public sbyte invincible;
    public sbyte unused1;
    public sbyte unused2;
    public sbyte unused3;
    public sbyte unused4;
    public sbyte unused5;
    public sbyte unused6;
}

/// <remarks> 
/// These flags are used in Auto Ability and MonStats structures to toggle an Auto-Status flag.
/// They are NOT used for checking whether a character currently has the status or not; that is
/// done by checking if the StatusDurationMap2 remaining is greater than 0.
/// </remarks>
[Flags]
public enum StatusFlags2 : uint {
    NONE                  =      0,
    SHELL                 = 1 << 0,
    PROTECT               = 1 << 1,
    REFLECT               = 1 << 2,
    REGEN                 = 1 << 3,
    HASTE                 = 1 << 4,
    SLOW                  = 1 << 5,
    STOP                  = 1 << 6,
    STRENGTH_UP_DOWN      = 1 << 7,
    MAGIC_UP_DOWN         = 1 << 8,
    DEFENCE_UP_DOWN       = 1 << 9,
    MAGIC_DEFENCE_UP_DOWN = 1 << 10,
    ACCURACY_UP_DOWN      = 1 << 11,
    EVASION_UP_DOWN       = 1 << 12,
    LUCK_UP_DOWN          = 1 << 13,
    DOOM                  = 1 << 14,
    NUL_PHYSICAL          = 1 << 15,
    NUL_MAGICAL           = 1 << 16,
    INVINCIBLE            = 1 << 17,
    UNUSED1               = 1 << 18,
    UNUSED2               = 1 << 19,
    UNUSED3               = 1 << 20,
    UNUSED4               = 1 << 21,
    UNUSED5               = 1 << 22,
    UNUSED6               = 1 << 23,
    UNUSED7               = 1 << 24,
    UNUSED8               = 1 << 25,
    UNUSED9               = 1 << 26,
    UNUSED10              = 1 << 27,
    UNUSED11              = 1 << 28,
    UNUSED12              = 1 << 29,
    UNUSED13              = 1 << 30,
}
