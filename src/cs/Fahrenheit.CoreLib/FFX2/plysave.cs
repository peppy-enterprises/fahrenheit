/* [fkelava 16/5/23 10:46]
 * source: Steam ver.
 * header: header_btl_ply_save
 *
 * /ffx_ps2/ffx2/master/jppc/battle/kernel/ply_save.h
 */

namespace Fahrenheit.CoreLib.FFX2;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct PlySave
{
    public       uint   name;
    public       uint   hp_bonus;
    public       uint   mp_bonus;
    public       byte   str_bonus;
    public       byte   vit_bonus;
    public       byte   mag_bonus;
    public       byte   spirit_bonus;
    public       byte   dex_bonus;
    public       byte   luck_bonus;
    public       byte   avoid_bonus;
    public       byte   hit_bonus;
    public       uint   exp;
    public       uint   next_exp;
    public       uint   hp;
    public       uint   mp;
    public       uint   hp_max;
    public       uint   mp_max;
    public       byte   party;
    public       byte   str;
    public       byte   vit;
    public       byte   mag;
    public       byte   spirit;
    public       byte   dex;
    public       byte   hit;
    public       byte   avoid;
    public       byte   luck;
    public       byte   reserve;
    public       ushort job;
    public       ushort plate;
    public fixed ushort accessory[2];
    public       ushort command;
    public       uint   escape_count;
    public       uint   kill_count;
    public       uint   death_count;
    public       int    status;
    public fixed ushort ability_type[3];
    public       ushort before_job;
}
