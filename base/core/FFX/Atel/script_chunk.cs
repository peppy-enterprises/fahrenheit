namespace Fahrenheit.Core.FFX.Atel;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x20)]
public unsafe struct MapEntrance {
    [FieldOffset(0x00)] public int   unknown00;
    [FieldOffset(0x02)] public short unknown02;
    [FieldOffset(0x04)] public short unknown04;
    [FieldOffset(0x06)] public short unknown06;
    [FieldOffset(0x08)] public float rotation;
    [FieldOffset(0x0C)] public float x;
    [FieldOffset(0x10)] public float y;
    [FieldOffset(0x14)] public float z;
    [FieldOffset(0x18)] public int   unknown18;
    [FieldOffset(0x1C)] public int   unknown1C;

    public readonly Vector3 pos => new(x, y, z);
}

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x38)]
public unsafe struct AtelScriptChunk {
    [FieldOffset(0x00)] public  uint   code_length;
    [FieldOffset(0x04)] public  uint   map_start;
    [FieldOffset(0x08)] public  uint   offset_author;
    [FieldOffset(0x0C)] public  uint   offset_name;
    [FieldOffset(0x10)] public  uint   offset_jumps_end;
    [FieldOffset(0x14)] private ushort __0x14;
    [FieldOffset(0x16)] private ushort __0x16;
    [FieldOffset(0x18)] public  ushort main_script_idx;
    [FieldOffset(0x1A)] private ushort __0x1A;
    [FieldOffset(0x1C)] private ushort __0x1C;
    [FieldOffset(0x1E)] public  ushort zone_bytes;
    [FieldOffset(0x20)] public  uint   offset_event_data;
    [FieldOffset(0x24)] private uint   __0x24;
    [FieldOffset(0x28)] public  uint   offset_area;
    [FieldOffset(0x2C)] public  uint   offset_other;
    [FieldOffset(0x30)] public  uint   offset_code;
    [FieldOffset(0x34)] public  ushort script_num;
    [FieldOffset(0x36)] public  ushort script_num_except_subroutines;

    public ushort* script_header_offsets { get { fixed (AtelScriptChunk* address = &this) { return (ushort*)(address + 1); } } }

    public readonly ReadOnlySpan<MapEntrance> map_entrances {
        get {
            fixed (AtelScriptChunk* address = &this) {
                return new((MapEntrance*)((nint)address + map_start), (int)(offset_author - map_start) / 0x20);
            }
        }
    }
}
