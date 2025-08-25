/* [fkelava 16/5/23 10:46]
 * source: Steam ver.
 * header: header_btl_ply_save
 *
 * /ffx_ps2/ffx2/master/jppc/battle/kernel/ply_save.h
 */

namespace Fahrenheit.Core.FFX2;

[StructLayout(LayoutKind.Explicit, Size = 0x80)]
public unsafe struct PlySave {
    [FieldOffset(0x0)]  public       uint   name;
    [FieldOffset(0x4)]  public       uint   hp_bonus;
    [FieldOffset(0x8)]  public       uint   mp_bonus;
    [FieldOffset(0xC)]  public       byte   str_bonus;
    [FieldOffset(0xD)]  public       byte   vit_bonus;
    [FieldOffset(0xE)]  public       byte   mag_bonus;
    [FieldOffset(0xF)]  public       byte   spirit_bonus;
    [FieldOffset(0x10)] public       byte   dex_bonus;
    [FieldOffset(0x11)] public       byte   luck_bonus;
    [FieldOffset(0x12)] public       byte   avoid_bonus;
    [FieldOffset(0x13)] public       byte   hit_bonus;
    [FieldOffset(0x14)] public       uint   exp;
    [FieldOffset(0x18)] public       uint   next_exp;
    [FieldOffset(0x1C)] public       uint   hp;
    [FieldOffset(0x20)] public       uint   mp;
    [FieldOffset(0x24)] public       uint   hp_max;
    [FieldOffset(0x28)] public       uint   mp_max;
    [FieldOffset(0x2C)] public       byte   party;
    [FieldOffset(0x2D)] public       byte   str;
    [FieldOffset(0x2E)] public       byte   vit;
    [FieldOffset(0x2F)] public       byte   mag;
    [FieldOffset(0x30)] public       byte   spirit;
    [FieldOffset(0x31)] public       byte   dex;
    [FieldOffset(0x32)] public       byte   hit;
    [FieldOffset(0x33)] public       byte   avoid;
    [FieldOffset(0x34)] public       byte   luck;
    [FieldOffset(0x35)] public       byte   reserve;
    [FieldOffset(0x36)] public       ushort job;
    [FieldOffset(0x38)] public       ushort plate;
    [FieldOffset(0x3A)] public fixed ushort accessory[2];
    [FieldOffset(0x3E)] public       ushort command;
    [FieldOffset(0x40)] public       uint   escape_count;
    [FieldOffset(0x44)] public       uint   kill_count;
    [FieldOffset(0x48)] public       uint   death_count;
    [FieldOffset(0x4C)] public       int    status;
    [FieldOffset(0x50)] public fixed ushort ability_type[3];
    [FieldOffset(0x56)] public       ushort before_job;

    [FieldOffset(0x7A)] public       byte   size;

    public bool join   { readonly get { return party.get_bit(0); } set { party.set_bit(0, value); } }
    public bool left   { readonly get { return party.get_bit(1); } set { party.set_bit(1, value); } }
    public bool fix    { readonly get { return party.get_bit(2); } set { party.set_bit(2, value); } }
    public bool joined { readonly get { return party.get_bit(4); } set { party.set_bit(4, value); } }
}
