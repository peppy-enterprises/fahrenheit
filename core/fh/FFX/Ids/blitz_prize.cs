// SPDX-License-Identifier: MIT

/*
 * [Andrewki44 16/03/26]
 * BlitzPrizeID 0x00 -> 0x64 == Takara.bin 220 -> 320
 * BlitzPrizeID 0x65 -> 0xA0 == Techs
 * BlitzPrizeID 0xBB -> 0xBD == Wakka Overdrives
 * 
 * Tech & Limit ID's mapped manually in-game via cheat engine manipulation
 * Tech's enumerated in blitz_tech.cs
 * 
 * Takara reference confirmed via 'blitzballOutput' script parse
 * * call Common.obtainTreasure [015Bh](msgWindow=0 [00h], treasure=BlitzballLeaguePrizeIndex[priv10AAD4 - 1 [01h]] + 220 [DCh])
 */

namespace Fahrenheit.FFX.Ids;

public static class BlitzPrizeId {
    public const T_XBlitzPrizeId BLTZ_TREASURE_BASE = 0x0000;
    public const T_XBlitzPrizeId BLTZ_TECH_BASE     = 0x0065;
    public const T_XBlitzPrizeId BLTZ_LIMIT_BASE    = 0x00BB;

    public enum BlitzLimitPrizes : T_XBlitzPrizeId {
        ATTACK_REELS    = 0x0000,
        STATUS_REELS    = 0x0001,
        AUROCHS_REELS   = 0x0002,
    }

    /// <summary>
    ///     Calculates the prize index corresponding to the specified treasure index for blitzball prizes.
    ///     Treasure indexes come from takara.bin
    /// </summary>
    /// <param name="treasure_idx">The treasure index to convert to a prize index. Must be between 220 and 320, inclusive.</param>
    /// <returns>The prize index associated with the specified treasure index.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when the value of treasure_idx is less than 220 or greater than 320.</exception>
    public static T_XBlitzPrizeId prize_index_for(int treasure_idx) {
        if (treasure_idx < 220 || treasure_idx > 320) {
            throw new IndexOutOfRangeException("Out of bounds index for blitzball prizes");
        }
        return T_XBlitzPrizeId.CreateChecked(treasure_idx - 220);
    }

    /// <summary>
    ///     Calculates the prize index corresponding to the specified Blitz tech.
    /// </summary>
    /// <param name="tech">The Blitz tech for which to determine the associated prize index.</param>
    /// <returns>The prize index that corresponds to the specified Blitz tech.</returns>
    public static T_XBlitzPrizeId prize_index_for(BlitzTechs tech) {
        if (tech < BlitzTechs.JECHT_SHOT || tech > BlitzTechs.AUROCHS_SPIRIT) {
            throw new IndexOutOfRangeException("Out of bounds index for blitzball techs");
        }
        return T_XBlitzPrizeId.CreateChecked(BLTZ_TECH_BASE + (ushort)tech);
    }

    /// <summary>
    ///     Calculates the prize identifier corresponding to the specified Blitz limit.
    /// </summary>
    /// <param name="limit">The Blitz limit for which to retrieve the associated prize identifier.</param>
    /// <returns>The prize identifier associated with the specified Blitz limit.</returns>
    public static T_XBlitzPrizeId prize_index_for(BlitzLimitPrizes limit) {
        if (limit < BlitzLimitPrizes.ATTACK_REELS || limit > BlitzLimitPrizes.AUROCHS_REELS) {
            throw new IndexOutOfRangeException("Out of bounds index for blitzball limits");
        }
        return T_XBlitzPrizeId.CreateChecked(BLTZ_LIMIT_BASE + (ushort)limit);
    }
}
