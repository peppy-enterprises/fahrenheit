namespace Fahrenheit.Core.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x16)]
public unsafe struct Equipment {
    [FieldOffset(0x0)] public        ushort name_id;
    [FieldOffset(0x2)] public        bool   exists;
    [FieldOffset(0x3)] public        byte   flags;
    [FieldOffset(0x4)] public        byte   owner;
    [FieldOffset(0x5)] public        byte   type;
    [FieldOffset(0x6)] private       byte   __0x6;
    [FieldOffset(0x7)] private       byte   __0x7;
    [FieldOffset(0x8)] public        byte   dmg_formula;
    [FieldOffset(0x9)] public        byte   power;
    [FieldOffset(0xA)] public        byte   crit_bonus;
    [FieldOffset(0xB)] public        byte   slot_count;
    [FieldOffset(0xC)] public        ushort model_id;
    [FieldOffset(0xE)] public  fixed ushort abilities[4];

    public bool is_hidden       { readonly get { return flags.get_bit(1); } set { flags.set_bit(1, value); } }
    public bool is_celestial    { readonly get { return flags.get_bit(2); } set { flags.set_bit(2, value); } }
    public bool is_brotherhood  { readonly get { return flags.get_bit(3); } set { flags.set_bit(3, value); } }

    public readonly bool is_weapon { get { return (type & 1) == 0; } }
    public readonly bool is_armor  { get { return (type & 1) != 0; } }
}

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x10)]
public unsafe struct UnownedEquipment {
    [FieldOffset(0x0)] public        byte   flags;
    [FieldOffset(0x1)] public        byte   owner;
    [FieldOffset(0x2)] public        byte   type;
    [FieldOffset(0x3)] private       byte   __0x6;
    [FieldOffset(0x4)] public        byte   dmg_formula;
    [FieldOffset(0x5)] public        byte   power;
    [FieldOffset(0x6)] public        byte   crit_bonus;
    [FieldOffset(0x7)] public        byte   slot_count;
    [FieldOffset(0x8)] public  fixed ushort abilities[4];

    public bool is_hidden       { readonly get { return flags.get_bit(1); } set { flags.set_bit(1, value); } }
    public bool is_celestial    { readonly get { return flags.get_bit(2); } set { flags.set_bit(2, value); } }
    public bool is_brotherhood  { readonly get { return flags.get_bit(3); } set { flags.set_bit(3, value); } }

    public readonly bool is_weapon { get { return (type & 1) == 0; } }
    public readonly bool is_armor  { get { return (type & 1) != 0; } }
}