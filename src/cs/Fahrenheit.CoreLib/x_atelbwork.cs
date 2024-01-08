namespace Fahrenheit.CoreLib;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x700)]
public unsafe struct FhXAtelBasicWorker {
	[FieldOffset(0x0)] public nint embedded_struct_ptr;
	[FieldOffset(0x4)] public nint ebp_ptr;
	[FieldOffset(0xC)] public ushort event_actor_total;
	[FieldOffset(0x2E)] public ushort worker_idx;
	[FieldOffset(0x34)] public ushort __0x34;
	[FieldOffset(0x36)] public ushort __0x36;
	[FieldOffset(0x44)] public ushort* request_table;
	[FieldOffset(0x80)] public FhXAtelSignal* pending_signal_queue;
	[FieldOffset(0x94)] public float __0x94;
	[FieldOffset(0x98)] public float __0x98;
	[FieldOffset(0x9C)] public FhXChr* chr_handle;
	[FieldOffset(0xA8)] public ushort event_chr_id;
	[FieldOffset(0xC4)] public FhXAtelStack stack;
}