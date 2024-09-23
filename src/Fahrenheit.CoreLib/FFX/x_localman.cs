namespace Fahrenheit.CoreLib.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x1C)]
public struct LocalizationManager {
    [FieldOffset(0x4)] public Language current_language;
}