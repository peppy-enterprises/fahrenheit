namespace Fahrenheit.CoreLib;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x700)]
public unsafe struct FhXAtelBasicWorker {
	[FieldOffset(0x0)] public nint embedded_struct_ptr;
	[FieldOffset(0x4)] public nint ebp_ptr;
	[FieldOffset(0xC)] public ushort event_actor_total;
	[FieldOffset(0x9C)] public FhXChr* chr_handle;
	[FieldOffset(0xA8)] public ushort event_chr_id;
	[FieldOffset(0xC4)] public nint stack_idx;
	[FieldOffset(0xC8)] public fixed uint stack_values[20]; // can actually be a mix of int or float
	[FieldOffset(0x118)] public fixed byte stack_types[20];
}