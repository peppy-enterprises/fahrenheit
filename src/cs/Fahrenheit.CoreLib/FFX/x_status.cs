namespace Fahrenheit.CoreLib.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x19)]
public struct StatusMap
{
    [FieldOffset(0x00)] public byte death;
    [FieldOffset(0x01)] public byte zombie;
    [FieldOffset(0x02)] public byte petrification;
    [FieldOffset(0x03)] public byte poison;
    [FieldOffset(0x04)] public byte power_break;
    [FieldOffset(0x05)] public byte magic_break;
    [FieldOffset(0x06)] public byte armor_break;
    [FieldOffset(0x07)] public byte mental_break;
    [FieldOffset(0x08)] public byte confuse;
    [FieldOffset(0x09)] public byte berserk;
    [FieldOffset(0x0A)] public byte provoke;
    [FieldOffset(0x0B)] public byte threaten;
    [FieldOffset(0x0C)] public byte sleep;
    [FieldOffset(0x0D)] public byte silence;
    [FieldOffset(0x0E)] public byte darkness;
    [FieldOffset(0x0F)] public byte shell;
    [FieldOffset(0x10)] public byte protect;
    [FieldOffset(0x11)] public byte reflect;
    [FieldOffset(0x12)] public byte nul_tide;
    [FieldOffset(0x13)] public byte nul_blaze;
    [FieldOffset(0x14)] public byte nul_shock;
    [FieldOffset(0x15)] public byte nul_frost;
    [FieldOffset(0x16)] public byte regen;
    [FieldOffset(0x17)] public byte haste;
    [FieldOffset(0x18)] public byte slow;
}

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0xD)]
public struct StatusDurationMap
{
    [FieldOffset(0x00)] public byte sleep;
    [FieldOffset(0x01)] public byte silence;
    [FieldOffset(0x02)] public byte darkness;
    [FieldOffset(0x03)] public byte shell;
    [FieldOffset(0x04)] public byte protect;
    [FieldOffset(0x05)] public byte reflect;
    [FieldOffset(0x06)] public byte nul_tide;
    [FieldOffset(0x07)] public byte nul_blaze;
    [FieldOffset(0x08)] public byte nul_shock;
    [FieldOffset(0x09)] public byte nul_frost;
    [FieldOffset(0x0A)] public byte regen;
    [FieldOffset(0x0B)] public byte haste;
    [FieldOffset(0x0C)] public byte slow;
}
