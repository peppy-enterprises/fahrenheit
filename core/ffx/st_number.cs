namespace Fahrenheit.Core.FFX;

// This is likely to get folded into a different file once we figure out what it's for
[StructLayout(LayoutKind.Explicit, Size=0x4)]
public struct StNumber {
    [FieldOffset(0x0)] public byte __0x0;
    [FieldOffset(0x1)] public byte __0x1;
    [FieldOffset(0x2)] public short __0x2; // This is also used for something else
    [FieldOffset(0x2)] public T_XCommandId command_id;
}
