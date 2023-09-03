using System.Runtime.CompilerServices;

namespace Fahrenheit.CoreLib;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x4)]
public unsafe struct FhXParamDataStatPlus {
	[FieldOffset(0x0)] public byte strength;
	[FieldOffset(0x1)] public byte magic;
	[FieldOffset(0x2)] public byte defense;
	[FieldOffset(0x3)] public byte magic_defense;
}

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x7C)]
public unsafe struct FhXParamDataStruct {
	[FieldOffset(0x0)] public FhXParamDataStatPlus* stat_plus;
}

