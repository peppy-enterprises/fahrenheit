namespace Fahrenheit.Core.FFX;

public enum GearType : byte {
    WEAPON = 0,
    ARMOR  = 1,
}

[Flags]
public enum GearProperties : byte {
    HIDDEN      = 1 << 1,
    CELESTIAL   = 1 << 2,
    BROTHERHOOD = 1 << 3,
}

public static partial class FhEnumExt {
    public static bool is_hidden     (this GearProperties flags) => flags.HasFlag(GearProperties.HIDDEN);
    public static bool is_celestial  (this GearProperties flags) => flags.HasFlag(GearProperties.CELESTIAL);
    public static bool is_brotherhood(this GearProperties flags) => flags.HasFlag(GearProperties.BROTHERHOOD);
}

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x16)]
public unsafe struct Gear {
    [FieldOffset(0x0)] public       ushort       name_id;
    [FieldOffset(0x2)] public       bool         exists; // Used
    [FieldOffset(0x3)] public       byte         properties;
    [FieldOffset(0x4)] public       T_XPlySaveId owner;
    [FieldOffset(0x5)] public       GearType     type;
    [FieldOffset(0x8)] public       byte         dmg_formula;
    [FieldOffset(0x9)] public       byte         power;
    [FieldOffset(0xA)] public       byte         crit_bonus;
    [FieldOffset(0xB)] public       byte         slot_count;
    [FieldOffset(0xC)] public       ushort       model_id;
    [FieldOffset(0xE)] public fixed ushort       auto_abilities[4];
}
