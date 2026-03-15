// SPDX-License-Identifier: MIT

namespace Fahrenheit.FFX.Ids;

public static class BlitzPrizeId {
    public const T_XBlitzPrizeId BLTZ_TREASURE_BASE = 0x0000;
    public const T_XBlitzPrizeId BLTZ_TECH_BASE     = 0x0065;
    public const T_XBlitzPrizeId BLTZ_LIMIT_BASE    = 0x00BB;

    public enum BlitzTechs: T_XBlitzPrizeId {
        JECHT_SHOT      = 0x0000,
        JECHT_SHOT_2    = 0x0001,
        SPHERE_SHOT     = 0x0002,
        INVISIBLE_SHOT  = 0x0003,
        VENOM_SHOT      = 0x0004,
        VENOM_SHOT_2    = 0x0005,
        VENOM_SHOT_3    = 0x0006,
        NAP_SHOT        = 0x0007,
        NAP_SHOT_2      = 0x0008,
        NAP_SHOT_3      = 0x0009,
        WITHER_SHOT     = 0x000A,
        WITHER_SHOT_2   = 0x000B,
        WITHER_SHOT_3   = 0x000C,
        VENOM_PASS      = 0x000D,
        VENOM_PASS_2    = 0x000E,
        VENOM_PASS_3    = 0x000F,
        NAP_PASS        = 0x0010,
        NAP_PASS_2      = 0x0011,
        NAP_PASS_3      = 0x0012,
        WITHER_PASS     = 0x0013,
        WITHER_PASS_2   = 0x0014,
        WITHER_PASS_3   = 0x0015,
        VOLLEY_SHOT     = 0x0016,
        VOLLEY_SHOT_2   = 0x0017,
        VOLLEY_SHOT_3   = 0x0018,
        VENOM_TACKLE    = 0x0019,
        VENOM_TACKLE_2  = 0x001A,
        VENOM_TACKLE_3  = 0x001B,
        NAP_TACKLE      = 0x001C,
        NAP_TACKLE_2    = 0x001D,
        NAP_TACKLE_3    = 0x001E,
        WITHER_TACKLE   = 0x001F,
        WITHER_TACKLE_2 = 0x0020,
        WITHER_TACKLE_3 = 0x0021,
        DRAIN_TACKLE    = 0x0022,
        DRAIN_TACKLE_2  = 0x0023,
        DRAIN_TACKLE_3  = 0x0024,
        TACKLE_SLIP     = 0x0025,
        TACKLE_SLIP_2   = 0x0026,
        ANTI_VENOM      = 0x0027,
        ANTI_VENOM_2    = 0x0028,
        ANTI_NAP        = 0x0029,
        ANTI_NAP_2      = 0x002A,
        ANTI_WITHER     = 0x002B,
        ANTI_WITHER_2   = 0x002C,
        ANTI_DRAIN      = 0x002D,
        ANTI_DRAIN_2    = 0x002E,
        SPIN_BALL       = 0x002F,
        GRIP_GLOVES     = 0x0030,
        ELITE_DEFENSE   = 0x0031,
        BRAWLER         = 0x0032,
        PILE_VENOM      = 0x0033,
        PILE_WITHER     = 0x0034,
        REGEN           = 0x0035,
        GOOD_MORNING    = 0x0036,
        HI_RISK         = 0x0037,
        GOLDEN_ARM      = 0x0038,
        GAMBLE          = 0x0039,
        SUPER_GOALIE    = 0x003A,
        AUROCHS_SPIRIT  = 0x003B,
    };

    public enum BlitzLimits: T_XBlitzPrizeId {
        ATTACK_REELS    = 0x0000,
        STATUS_REELS    = 0x0001,
        AUROCHS_REELS   = 0x0002,
};

    /// <summary>
    /// Calculates the prize index corresponding to the specified treasure index for blitzball prizes.
    /// Treasure indexes come from takara.bin
    /// </summary>
    /// <param name="treasure_idx">The treasure index to convert to a prize index. Must be between 220 and 320, inclusive.</param>
    /// <returns>The prize index associated with the specified treasure index.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when the value of treasure_idx is less than 220 or greater than 320.</exception>
    public static T_XBlitzPrizeId prize_index_for(int treasure_idx) {
        if (treasure_idx < 220 || treasure_idx > 320) {
            throw new IndexOutOfRangeException("Out of bounds index for blitzball prizes");
        }
        return (ushort)(treasure_idx - 220);
    }

    /// <summary>
    /// Calculates the prize index corresponding to the specified Blitz tech.
    /// </summary>
    /// <param name="tech">The Blitz tech for which to determine the associated prize index.</param>
    /// <returns>The prize index that corresponds to the specified Blitz tech.</returns>
    public static T_XBlitzPrizeId prize_index_for(BlitzTechs tech) {
        return (ushort)(BLTZ_TECH_BASE + tech);
    }

    /// <summary>
    /// Calculates the prize identifier corresponding to the specified Blitz limit.
    /// </summary>
    /// <param name="limit">The Blitz limit for which to retrieve the associated prize identifier.</param>
    /// <returns>The prize identifier associated with the specified Blitz limit.</returns>
    public static T_XBlitzPrizeId prize_index_for(BlitzLimits limit) {
        return (ushort)(BLTZ_LIMIT_BASE + limit);
    }
}
