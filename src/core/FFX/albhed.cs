namespace Fahrenheit.Core.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x4)]
public struct AlBhed {
    [FieldOffset(0x00)] public  byte chr_original;
    [FieldOffset(0x01)] public  byte chr_albhed;
    [FieldOffset(0x02)] public  byte primer_index;
    [FieldOffset(0x03)] private byte __0x04_ZERO;
}
