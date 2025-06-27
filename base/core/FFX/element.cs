namespace Fahrenheit.Core.FFX;

public enum ElementFlags : byte {
    NONE    = 0,
    FIRE    = 1 << 0,
    ICE     = 1 << 1,
    THUNDER = 1 << 2,
    WATER   = 1 << 3,
    HOLY    = 1 << 4,
}

public static partial class FhEnumExt {
    public static bool fire   (this ElementFlags flags) => flags.HasFlag(ElementFlags.FIRE);
    public static bool ice    (this ElementFlags flags) => flags.HasFlag(ElementFlags.ICE);
    public static bool thunder(this ElementFlags flags) => flags.HasFlag(ElementFlags.THUNDER);
    public static bool water  (this ElementFlags flags) => flags.HasFlag(ElementFlags.WATER);
    public static bool holy   (this ElementFlags flags) => flags.HasFlag(ElementFlags.HOLY);
}
