/* [fkelava 13/9/22 08:11]
 * source: MS Store ver.
 * header: __SE_TYPE_ATEL__
 *
 * /ffx_ps2/ffx/proj2/chr/ath/battle/setype.ath
 */

namespace Fahrenheit.CoreLib.FFX;

public enum SeTypeId : T_XSeTypeId
{
    SE_WOOD_PIER     = 0,
    SE_CARPET        = 1,
    SE_CEMENT        = 2,
    SE_LAND          = 3, // 荒れ地 - not sure about this
    SE_GRASS         = 4,
    SE_GRAVEL        = 5,
    SE_ICE           = 6,
    SE_MARBLE        = 7,
    SE_IRON          = 8,
    SE_ROCK          = 9,
    SE_SAND          = 10,
    SE_SNOW          = 11,
    SE_STONE         = 12,
    SE_TREE          = 13,
    SE_SHALLOW_WATER = 14,
    SE_DEEP_WATER    = 15,
    SE_UNKNOWN01     = 16, // 水中バトル - what does this mean...?
    SE_UNKNOWN02     = 17, // 濡れた鉄床 - what does this mean...?
}