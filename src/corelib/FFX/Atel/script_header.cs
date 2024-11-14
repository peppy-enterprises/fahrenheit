using System;

namespace Fahrenheit.CoreLib.FFX.Atel;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x34)]
public unsafe struct AtelScriptHeader {
    [FieldOffset(0x00)] public ushort script_type;
    [FieldOffset(0x02)] public ushort var_count;
    [FieldOffset(0x04)] public ushort ref_int_count;
    [FieldOffset(0x06)] public ushort ref_float_count;
    [FieldOffset(0x08)] public ushort entry_point_count;
    [FieldOffset(0x0A)] public ushort jump_count;
    [FieldOffset(0x10)] public uint   priv_data_len;
    [FieldOffset(0x14)] public uint   offset_var_table;
    [FieldOffset(0x18)] public uint   offset_int_table;
    [FieldOffset(0x1C)] public uint   offset_float_table;
    [FieldOffset(0x20)] public uint   offset_entry_points_table;
    [FieldOffset(0x24)] public uint   offset_jumps_table;
    [FieldOffset(0x28)] public uint   offset_data;
    [FieldOffset(0x2C)] public uint   offset_priv_data;
    [FieldOffset(0x30)] public uint   offset_shared_data;
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
    U8  = 0x00,
    I8  = 0x01,
    U16 = 0x02,
    I16 = 0x03,
    U32 = 0x04,
    I32 = 0x05,
    F32 = 0x06
}

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x4)]
public unsafe struct AtelScriptVarValue {
    [FieldOffset(0x0)] public byte   as_byte;
    [FieldOffset(0x0)] public sbyte  as_sbyte;
    [FieldOffset(0x0)] public ushort as_ushort;
    [FieldOffset(0x0)] public short  as_short;
    [FieldOffset(0x0)] public uint   as_uint;
    [FieldOffset(0x0)] public int    as_int;
    [FieldOffset(0x0)] public float  as_float;
}

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x8)]
public unsafe struct AtelScriptVar {
    [FieldOffset(0x0)] public long raw;
    [FieldOffset(0x0)] public uint lo;
    [FieldOffset(0x4)] public uint hi;

    [FieldOffset(0x0)] public fixed byte val[3];
    [FieldOffset(0x3)] public       byte properties;
    [FieldOffset(0x4)] public       uint element_count;

    public AtelScriptVarType     type     { get { return (AtelScriptVarType)   ((properties & 0xF0) >> 4); } }
    public AtelScriptVarLocation location { get { return (AtelScriptVarLocation)(properties & 0x0F);       } }

    public double value {
        get {
            return type switch {
                AtelScriptVarType.U8  =>        val[0],
                AtelScriptVarType.I8  => (sbyte)val[0],
                AtelScriptVarType.U16 => (ushort)((val[1] << 0x8) | val[0]),
                AtelScriptVarType.I16 => (short) ((val[1] << 0x8) | val[0]),
                AtelScriptVarType.U32 => (uint)((val[2] << 0x10) | (val[1] << 0x8) | val[0]),
                AtelScriptVarType.I32 =>        (val[2] << 0x10) | (val[1] << 0x8) | val[0],
                AtelScriptVarType.F32 => BitConverter.Int32BitsToSingle((val[2] << 0x10) | (val[1] << 0x8) | val[0]),
                _                     => throw new ArgumentException(),
            };
        }
    }
}