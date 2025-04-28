namespace Fahrenheit.Core.FFX;

public enum ElementalFlags : byte {
    NONE    = 0,
    FIRE    = 1 << 0,
    ICE     = 1 << 1,
    THUNDER = 1 << 2,
    WATER   = 1 << 3,
    HOLY    = 1 << 4,
}
public static partial class EnumExt {
    public static bool fire(this ElementalFlags flags)
        => flags.HasFlag(ElementalFlags.FIRE);
    public static bool ice(this ElementalFlags flags)
        => flags.HasFlag(ElementalFlags.ICE);
    public static bool thunder(this ElementalFlags flags)
        => flags.HasFlag(ElementalFlags.THUNDER);
    public static bool water(this ElementalFlags flags)
        => flags.HasFlag(ElementalFlags.WATER);
    public static bool holy(this ElementalFlags flags)
        => flags.HasFlag(ElementalFlags.HOLY);
}