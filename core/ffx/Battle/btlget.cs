// SPDX-License-Identifier: MIT

using Fahrenheit.Core.FFX.Ids;

namespace Fahrenheit.Core.FFX.Battle;

[InlineArray(18)]
public struct PlyArrayU32 {
    private uint _u;

    public uint tidus   { readonly get { return this[PlySaveId.PC_TIDUS  ]; } set { this[PlySaveId.PC_TIDUS  ] = value; } }
    public uint yuna    { readonly get { return this[PlySaveId.PC_YUNA   ]; } set { this[PlySaveId.PC_YUNA   ] = value; } }
    public uint auron   { readonly get { return this[PlySaveId.PC_AURON  ]; } set { this[PlySaveId.PC_AURON  ] = value; } }
    public uint kimahri { readonly get { return this[PlySaveId.PC_KIMAHRI]; } set { this[PlySaveId.PC_KIMAHRI] = value; } }
    public uint wakka   { readonly get { return this[PlySaveId.PC_WAKKA  ]; } set { this[PlySaveId.PC_WAKKA  ] = value; } }
    public uint lulu    { readonly get { return this[PlySaveId.PC_LULU   ]; } set { this[PlySaveId.PC_LULU   ] = value; } }
    public uint rikku   { readonly get { return this[PlySaveId.PC_RIKKU  ]; } set { this[PlySaveId.PC_RIKKU  ] = value; } }
    public uint seymour { readonly get { return this[PlySaveId.PC_SEYMOUR]; } set { this[PlySaveId.PC_SEYMOUR] = value; } }
    public uint valefor { readonly get { return this[PlySaveId.PC_VALEFOR]; } set { this[PlySaveId.PC_VALEFOR] = value; } }
    public uint ifrit   { readonly get { return this[PlySaveId.PC_IFRIT  ]; } set { this[PlySaveId.PC_IFRIT  ] = value; } }
    public uint ixion   { readonly get { return this[PlySaveId.PC_IXION  ]; } set { this[PlySaveId.PC_IXION  ] = value; } }
    public uint shiva   { readonly get { return this[PlySaveId.PC_SHIVA  ]; } set { this[PlySaveId.PC_SHIVA  ] = value; } }
    public uint bahamut { readonly get { return this[PlySaveId.PC_BAHAMUT]; } set { this[PlySaveId.PC_BAHAMUT] = value; } }
    public uint anima   { readonly get { return this[PlySaveId.PC_ANIMA  ]; } set { this[PlySaveId.PC_ANIMA  ] = value; } }
    public uint yojimbo { readonly get { return this[PlySaveId.PC_YOJIMBO]; } set { this[PlySaveId.PC_YOJIMBO] = value; } }
    public uint magus1  { readonly get { return this[PlySaveId.PC_MAGUS1 ]; } set { this[PlySaveId.PC_MAGUS1 ] = value; } }
    public uint magus2  { readonly get { return this[PlySaveId.PC_MAGUS2 ]; } set { this[PlySaveId.PC_MAGUS2 ] = value; } }
    public uint magus3  { readonly get { return this[PlySaveId.PC_MAGUS3 ]; } set { this[PlySaveId.PC_MAGUS3 ] = value; } }
}

[InlineArray(18)]
public struct PlyArrayBool {
    private bool _b;

    public bool tidus   { readonly get { return this[PlySaveId.PC_TIDUS  ]; } set { this[PlySaveId.PC_TIDUS  ] = value; } }
    public bool yuna    { readonly get { return this[PlySaveId.PC_YUNA   ]; } set { this[PlySaveId.PC_YUNA   ] = value; } }
    public bool auron   { readonly get { return this[PlySaveId.PC_AURON  ]; } set { this[PlySaveId.PC_AURON  ] = value; } }
    public bool kimahri { readonly get { return this[PlySaveId.PC_KIMAHRI]; } set { this[PlySaveId.PC_KIMAHRI] = value; } }
    public bool wakka   { readonly get { return this[PlySaveId.PC_WAKKA  ]; } set { this[PlySaveId.PC_WAKKA  ] = value; } }
    public bool lulu    { readonly get { return this[PlySaveId.PC_LULU   ]; } set { this[PlySaveId.PC_LULU   ] = value; } }
    public bool rikku   { readonly get { return this[PlySaveId.PC_RIKKU  ]; } set { this[PlySaveId.PC_RIKKU  ] = value; } }
    public bool seymour { readonly get { return this[PlySaveId.PC_SEYMOUR]; } set { this[PlySaveId.PC_SEYMOUR] = value; } }
    public bool valefor { readonly get { return this[PlySaveId.PC_VALEFOR]; } set { this[PlySaveId.PC_VALEFOR] = value; } }
    public bool ifrit   { readonly get { return this[PlySaveId.PC_IFRIT  ]; } set { this[PlySaveId.PC_IFRIT  ] = value; } }
    public bool ixion   { readonly get { return this[PlySaveId.PC_IXION  ]; } set { this[PlySaveId.PC_IXION  ] = value; } }
    public bool shiva   { readonly get { return this[PlySaveId.PC_SHIVA  ]; } set { this[PlySaveId.PC_SHIVA  ] = value; } }
    public bool bahamut { readonly get { return this[PlySaveId.PC_BAHAMUT]; } set { this[PlySaveId.PC_BAHAMUT] = value; } }
    public bool anima   { readonly get { return this[PlySaveId.PC_ANIMA  ]; } set { this[PlySaveId.PC_ANIMA  ] = value; } }
    public bool yojimbo { readonly get { return this[PlySaveId.PC_YOJIMBO]; } set { this[PlySaveId.PC_YOJIMBO] = value; } }
    public bool magus1  { readonly get { return this[PlySaveId.PC_MAGUS1 ]; } set { this[PlySaveId.PC_MAGUS1 ] = value; } }
    public bool magus2  { readonly get { return this[PlySaveId.PC_MAGUS2 ]; } set { this[PlySaveId.PC_MAGUS2 ] = value; } }
    public bool magus3  { readonly get { return this[PlySaveId.PC_MAGUS3 ]; } set { this[PlySaveId.PC_MAGUS3 ] = value; } }
}

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x1B0)]
public unsafe struct BtlRewardData {
    [FieldOffset(0x0)]  public        PlyArrayBool in_battle;
    [FieldOffset(0x24)] public        PlyArrayBool shared_ap;
    [FieldOffset(0x38)] public        PlyArrayU32  get_ap;
    [FieldOffset(0x80)] public        PlyArrayU32  get_ap_temp;
    [FieldOffset(0xCC)] public        uint         gil;
    [FieldOffset(0xD0)] public        byte         item_count;
    [FieldOffset(0xD1)] public        byte         key_item_count;
    [FieldOffset(0xD2)] public        byte         gear_count;
    [FieldOffset(0xD4)] public  fixed T_XCommandId items[8];
    [FieldOffset(0xE4)] public  fixed byte         items_amounts[8];
    [FieldOffset(0xEC)] public        ushort       key_item;
    [FieldOffset(0xEE)] public        ushort       gear_inv_idx;
    [FieldOffset(0xFE)] private       Equipment    _gear_arr_start;

    public Span<Equipment> gear => MemoryMarshal.CreateSpan(ref _gear_arr_start, 8);
}
