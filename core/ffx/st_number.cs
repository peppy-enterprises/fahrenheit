namespace Fahrenheit.Core.FFX;

// This is likely to get folded into a different file once we figure out what it's for
[StructLayout(LayoutKind.Explicit, Size = 0x4)]
public struct StNumber {
    [FieldOffset(0x0)] public byte __0x0;
    [FieldOffset(0x1)] public byte __0x1;

    // Can be a command id or some other unknown id if __0x0 == 1
    [FieldOffset(0x2)] public short __0x2;
}
