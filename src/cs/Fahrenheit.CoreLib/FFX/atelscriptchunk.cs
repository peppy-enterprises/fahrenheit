namespace Fahrenheit.CoreLib.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x38)]
public unsafe struct AtelScriptChunk {
    [FieldOffset(0x0)] public uint code_length;
    [FieldOffset(0x4)] public uint map_start;
    [FieldOffset(0x8)] public uint author_offset;
    [FieldOffset(0xC)] public uint name_offset;
    [FieldOffset(0x10)] public uint jumps_end_offset;
    [FieldOffset(0x14)] public ushort __0x14;
    [FieldOffset(0x16)] public ushort __0x16;
    [FieldOffset(0x18)] public ushort main_script_idx;
    [FieldOffset(0x1A)] public ushort __0x1A;
    [FieldOffset(0x1C)] public ushort __0x1C;
    [FieldOffset(0x1E)] public ushort zone_bytes;
    [FieldOffset(0x20)] public uint event_data_offset;
    [FieldOffset(0x24)] public uint __0x24;
    [FieldOffset(0x28)] public uint area_offset;
    [FieldOffset(0x2C)] public uint other_offset;
    [FieldOffset(0x30)] public uint code_offset;
    [FieldOffset(0x34)] public ushort script_num;
    [FieldOffset(0x36)] public ushort script_num_except_subroutines;

    public ushort* script_header_offsets { get {
        fixed (AtelScriptChunk* address = &this) {
            return (ushort*)(address + 1);
        }
    } }
}