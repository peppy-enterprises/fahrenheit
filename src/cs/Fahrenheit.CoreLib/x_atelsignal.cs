namespace Fahrenheit.CoreLib;

public enum FhXAtelSignalState : byte {
	Ignore = 0x0,
	Run = 0x1,
	Complete = 0x2,
	Acknowledged = 0x3,
}

[StructLayout(LayoutKind.Explicit, Pack = 2, Size = 0x8)]
public unsafe struct FhXAtelSignal {
	[FieldOffset(0x0)] public FhXAtelSignal* next;
	[FieldOffset(0x4)] public FhXAtelSignal* prev;
	[FieldOffset(0x8)] public ushort entry_point;
	[FieldOffset(0xA)] public ushort src_work_idx;
	[FieldOffset(0xC)] public ushort tgt_work_idx;
	[FieldOffset(0xE)] public byte properties;
	public byte priority { get => properties.get_bits(0, 4); set => properties.set_bits(0, 4, value); }
	public byte process_status { get => properties.get_bits(4, 4); set => properties.set_bits(4, 4, value); }

	[FieldOffset(0xF)] public FhXAtelSignalState state;
	[FieldOffset(0x14)] public ushort ctrl_idx;
}