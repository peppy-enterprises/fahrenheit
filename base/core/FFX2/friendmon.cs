namespace Fahrenheit.Core.FFX2;



[StructLayout(LayoutKind.Explicit, Size = 0x180)]
public unsafe struct FriendMonsterCommand {
    [FieldOffset(0x0)]   public       ushort command_id;


    [FieldOffset(0xC6)]  public fixed byte   _0xC6[11];
                                       
    [FieldOffset(0x17F)] public       byte   properties;

    public byte get_double => (byte)(((properties >> 1) & 7) | 8); // Unsure about the | 8
    public byte get_reflect => (byte)((properties >> 5) | 8);      // Unsure about the | 8
}
[InlineArray(8)]
public struct FriendMonsterCommandArray {
    private FriendMonsterCommand _data;
}


[StructLayout(LayoutKind.Explicit, Size = 0xE38)]
public unsafe struct FriendMonster {
    [FieldOffset(0x0)]  public fixed  ushort                    commands_0x3[4];
    [FieldOffset(0x8)]  public fixed  ushort                    commands_0x8[4];
    [FieldOffset(0x10)] public fixed  ushort                    learn_0x3[37];
    [FieldOffset(0x5A)] public fixed  ushort                    learn_0x4[37];
    [FieldOffset(0xA4)] public fixed  ushort                    learn_0x8[16];
    [FieldOffset(0xC4)] public        FriendMonsterCommandArray _0xC4;

    [FieldOffset(0xCC4)] public fixed byte                      _0xCC4[0x172]; // max hp?

    public bool get_command_learned(ushort command_id) {
        int command_type = command_id >> 0xc;
        int offset = (command_id >> 4) & 0xff;
        switch (command_id >> 0xc) {
            case 8:
                if (offset > 15) break;
                return (learn_0x8[offset] & (1 << (command_id & 0xf))) != 0;
            case 4:
                if (offset > 36) break;
                return (learn_0x4[offset] & (1 << (command_id & 0xf))) != 0;
            case 3:
                if (offset > 36) break;
                return (learn_0x3[offset] & (1 << (command_id & 0xf))) != 0;
        }
        return false;
    }
}
