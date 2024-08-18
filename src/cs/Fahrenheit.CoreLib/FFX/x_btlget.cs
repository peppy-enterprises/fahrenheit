namespace Fahrenheit.CoreLib.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x1B0)]
public unsafe struct BtlRewardData
{
	[FieldOffset(0x0)]  public fixed bool in_battle[18];
	[FieldOffset(0x24)] public fixed bool shared_ap[18];
	[FieldOffset(0x38)] public fixed uint get_ap[18];
	[FieldOffset(0x80)] public fixed uint get_ap_temp[18];
}