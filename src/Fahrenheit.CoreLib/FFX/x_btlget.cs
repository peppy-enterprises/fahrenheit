namespace Fahrenheit.CoreLib.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x1B0)]
public unsafe struct BtlRewardData {
    [FieldOffset(0x0)]  public fixed bool in_battle[18];
    [FieldOffset(0x24)] public fixed bool shared_ap[18];
    [FieldOffset(0x38)] public fixed uint get_ap[18];
    [FieldOffset(0x80)] public fixed uint get_ap_temp[18];
    [FieldOffset(0xCC)] public uint gil;
    [FieldOffset(0xD0)] public byte item_count;
    [FieldOffset(0xD2)] public byte gear_count;
    [FieldOffset(0xD4)] public fixed T_XItemId items[8];
    [FieldOffset(0xE4)] public fixed byte items_amounts[8];
    [FieldOffset(0xFE)] private Equipment _gear_arr_start;

    // Please make this better. CSR uses it. - Eve
    public ItemId get_item(int idx) => (ItemId)items[idx];
    public void set_item(int idx, ItemId value) => items[idx] = (T_XItemId)value;

    public System.Span<Equipment> gear => MemoryMarshal.CreateSpan(ref _gear_arr_start, 8);
}