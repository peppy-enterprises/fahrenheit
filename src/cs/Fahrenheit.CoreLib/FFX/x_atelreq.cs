namespace Fahrenheit.CoreLib.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 2, Size = 0x8)]
public unsafe struct AtelRequest
{
    [FieldOffset(0x0)] public  ushort worker_idx;
    [FieldOffset(0x2)] private ushort __0x2;
    [FieldOffset(0x4)] public  ushort slot;
    [FieldOffset(0x6)] public  ushort entry_point_idx;
}
