namespace Fahrenheit.CoreLib.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x34)]
public unsafe struct AtelScriptHeader {
    [FieldOffset(0x0)] public ushort script_type;
    [FieldOffset(0x2)] public ushort var_count;
    [FieldOffset(0x4)] public ushort ref_int_count;
    [FieldOffset(0x6)] public ushort ref_float_count;
    [FieldOffset(0x8)] public ushort entry_point_count;
    [FieldOffset(0xA)] public ushort jump_count;

    [FieldOffset(0x10)] public uint priv_data_len;
    [FieldOffset(0x14)] public uint var_table_offset;
    [FieldOffset(0x18)] public uint int_table_offset;
    [FieldOffset(0x1C)] public uint float_table_offset;
    [FieldOffset(0x20)] public uint script_begin_table_offset;
    [FieldOffset(0x24)] public uint jumps_begin_table_offset;
    [FieldOffset(0x28)] public uint data_offset;
    [FieldOffset(0x2C)] public uint priv_data_offset;
    [FieldOffset(0x30)] public uint shared_data_offset;
}

public enum AtelScriptVarLocation {
    SaveData,
    CommonVars,
    Data,
    Private,
    Shared,
    IntRegisters,
    EventData
}

public enum AtelScriptVarType {
    Unsigned8,
    Signed8,
    Unsigned16,
    Signed16,
    Unsigned32,
    Signed32,
    Float
}

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x4)]
public unsafe struct AtelScriptVarValue {
    [FieldOffset(0x0)] public   byte   as_byte;
    [FieldOffset(0x0)] public  sbyte  as_sbyte;
    [FieldOffset(0x0)] public ushort as_ushort;
    [FieldOffset(0x0)] public  short  as_short;
    [FieldOffset(0x0)] public   uint   as_uint;
    [FieldOffset(0x0)] public    int    as_int;
    [FieldOffset(0x0)] public  float  as_float;
}

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x8)]
public unsafe struct AtelScriptVar {
    [FieldOffset(0x0)] public long raw;
    [FieldOffset(0x0)] public uint low4;
    [FieldOffset(0x4)] public uint high4;

    [FieldOffset(0x0)] fixed byte val[3];
    [FieldOffset(0x3)] byte properties;
    public AtelScriptVarType type => (AtelScriptVarType)((properties & 0xF0) >> 4);
    public AtelScriptVarLocation location => (AtelScriptVarLocation)(properties & 0x0F);

    [FieldOffset(0x4)] public uint element_count;

    public byte as_byte => val[0];
    public sbyte as_sbyte => (sbyte)as_byte;

    public ushort as_ushort => (ushort)((val[1] << 0x8) | val[0]);
    public short as_short => (short)as_ushort;

    public uint as_uint => (uint)((val[2] << 0x10) | (val[1] << 0x8) | val[0]);
    public int as_int => (int)as_uint;

    public float as_float => System.BitConverter.Int32BitsToSingle(as_int);

    public AtelScriptVarValue value => new AtelScriptVarValue { as_uint = as_uint };

    public double good_value => ((properties & 0xF0) >> 4) switch {
        0 => (double)as_byte, 1 => (double)as_sbyte,
        2 => (double)as_ushort, 3 => (double)as_short,
        4 => (double)as_uint, 5 => (double)as_int,
        6 => (double)as_float, _ => throw new System.ArgumentException(),
    };
}