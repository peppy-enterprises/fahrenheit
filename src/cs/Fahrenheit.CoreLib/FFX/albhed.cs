namespace Fahrenheit.CoreLib.FFX;

//TODO: Change to LayoutKind.Fixed as soon as convenient
//TODO: Rename this to something more descriptive
[StructLayout(LayoutKind.Sequential)]
public struct AlBhed {
    public byte chr_original;
    public byte chr_albhed;
    public byte primer_index;
    public byte __0x04_ZERO;
}