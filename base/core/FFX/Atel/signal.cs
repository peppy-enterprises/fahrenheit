// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core.FFX.Atel;

public enum AtelSignalState : byte {
    Ignore       = 0x0,
    Run          = 0x1,
    Complete     = 0x2,
    Acknowledged = 0x3,
}

[StructLayout(LayoutKind.Explicit, Pack = 2, Size = 0x16)]
public unsafe struct AtelSignal {
    [FieldOffset(0x00)] public  AtelSignal*     next;
    [FieldOffset(0x04)] public  AtelSignal*     prev;
    [FieldOffset(0x08)] public  ushort          entry_point;
    [FieldOffset(0x0A)] public  ushort          src_work_idx;
    [FieldOffset(0x0C)] public  ushort          tgt_work_idx;
    [FieldOffset(0x0E)] public  byte            flags;
    [FieldOffset(0x0F)] public  AtelSignalState state;
    [FieldOffset(0x10)] private short           __0x10;
    [FieldOffset(0x12)] private short           __0x12;
    [FieldOffset(0x14)] public  ushort          ctrl_idx;

    public byte priority       { readonly get { return flags.get_bits(0, 4); } set { flags.set_bits(0, 4, value); } }
    public byte process_status { readonly get { return flags.get_bits(4, 4); } set { flags.set_bits(4, 4, value); } }
}
