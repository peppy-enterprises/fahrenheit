// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x6C)]
public struct AutoAbility {
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
    [FieldOffset(0x58)] public StatusPermanentFlags  status_auto_permanent;
    [FieldOffset(0x5A)] public StatusTemporalFlags   status_auto_temporal;
    [FieldOffset(0x5C)] public StatusExtraFlags      status_auto_extra;
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
    MAGIC_BONUS         = 1 << 11,
    DEFENSE_BONUS       = 1 << 12,
    MAGIC_DEFENSE_BONUS = 1 << 13,
}

public static partial class FhEnumExt {
    extension(StatIncreaseFlags flags) {
        public bool strength {
            get { return flags.HasFlag(StatIncreaseFlags.STRENGTH); }
            set { if (value) flags |= (StatIncreaseFlags.STRENGTH); else flags &= ~(StatIncreaseFlags.STRENGTH); }
        }

        public bool defense {
            get { return flags.HasFlag(StatIncreaseFlags.DEFENSE); }
            set { if (value) flags |= (StatIncreaseFlags.DEFENSE); else flags &= ~(StatIncreaseFlags.DEFENSE); }
        }

        public bool magic {
            get { return flags.HasFlag(StatIncreaseFlags.MAGIC); }
            set { if (value) flags |= (StatIncreaseFlags.MAGIC); else flags &= ~(StatIncreaseFlags.MAGIC); }
        }

        public bool magic_defense {
            get { return flags.HasFlag(StatIncreaseFlags.MAGIC_DEFENSE); }
            set { if (value) flags |= (StatIncreaseFlags.MAGIC_DEFENSE); else flags &= ~(StatIncreaseFlags.MAGIC_DEFENSE); }
        }

        public bool agility {
            get { return flags.HasFlag(StatIncreaseFlags.AGILITY); }
            set { if (value) flags |= (StatIncreaseFlags.AGILITY); else flags &= ~(StatIncreaseFlags.AGILITY); }
        }

        public bool luck {
            get { return flags.HasFlag(StatIncreaseFlags.LUCK); }
            set { if (value) flags |= (StatIncreaseFlags.LUCK); else flags &= ~(StatIncreaseFlags.LUCK); }
        }

        public bool evasion {
            get { return flags.HasFlag(StatIncreaseFlags.EVASION); }
            set { if (value) flags |= (StatIncreaseFlags.EVASION); else flags &= ~(StatIncreaseFlags.EVASION); }
        }

        public bool accuracy {
            get { return flags.HasFlag(StatIncreaseFlags.ACCURACY); }
            set { if (value) flags |= (StatIncreaseFlags.ACCURACY); else flags &= ~(StatIncreaseFlags.ACCURACY); }
        }


        public bool hp {
            get { return flags.HasFlag(StatIncreaseFlags.HP); }
            set { if (value) flags |= (StatIncreaseFlags.HP); else flags &= ~(StatIncreaseFlags.HP); }
        }

        public bool mp {
            get { return flags.HasFlag(StatIncreaseFlags.MP); }
            set { if (value) flags |= (StatIncreaseFlags.MP); else flags &= ~(StatIncreaseFlags.MP); }
        }


        public bool strength_bonus {
            get { return flags.HasFlag(StatIncreaseFlags.STRENGTH_BONUS); }
            set { if (value) flags |= (StatIncreaseFlags.STRENGTH_BONUS); else flags &= ~(StatIncreaseFlags.STRENGTH_BONUS); }
        }

        public bool defense_bonus {
            get { return flags.HasFlag(StatIncreaseFlags.DEFENSE_BONUS); }
            set { if (value) flags |= (StatIncreaseFlags.DEFENSE_BONUS); else flags &= ~(StatIncreaseFlags.DEFENSE_BONUS); }
        }

        public bool magic_bonus {
            get { return flags.HasFlag(StatIncreaseFlags.MAGIC_BONUS); }
            set { if (value) flags |= (StatIncreaseFlags.MAGIC_BONUS); else flags &= ~(StatIncreaseFlags.MAGIC_BONUS); }
        }

        public bool magic_defense_bonus {
            get { return flags.HasFlag(StatIncreaseFlags.MAGIC_DEFENSE_BONUS); }
            set { if (value) flags |= (StatIncreaseFlags.MAGIC_DEFENSE_BONUS); else flags &= ~(StatIncreaseFlags.MAGIC_DEFENSE_BONUS); }
        }
    }
}

[Flags]
public enum GearType : byte {
    NONE   = 0,
    WEAPON = 1,
    ARMOR  = 2,
}

public static partial class FhEnumExt {
    extension(GearType gear_type) {
        public bool is_weapon => gear_type.HasFlag(GearType.WEAPON);
        public bool is_armor  => gear_type.HasFlag(GearType.ARMOR);
    }
}

/// <summary>
///     Recipe for customizing an auto-ability onto gear using a set amount of an item.
/// </summary>
public struct CustomizationRecipe {
    /// <summary>
    ///     The gear type that can be customized using this recipe.
    /// </summary>
    public GearType target_gear_type;

    /// <summary>
    ///     The auto-ability that results from this recipe.
    /// </summary>
    public T_XAutoAbilityId auto_ability;

    /// <summary>
    ///     The item to be spent on the customization.
    /// </summary>
    public T_XCommandId item;

    /// <summary>
    ///     The amount of the item that is needed.
    /// </summary>
    public ushort item_cost;
}
