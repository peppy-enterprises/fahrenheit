/* [fkelava 16/5/23 10:46]
 * source: Steam ver.
 * header: header_btl_command
 *
 * /ffx_ps2/ffx2/master/jppc/battle/kernel/command.h
 */

namespace Fahrenheit.CoreLib.FFX2;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct PCommand {
    public       uint   name;
    public       uint   help;
    public fixed short  effect[2];
    public       byte   process;
    public       byte   sub_command;
    public       byte   system;
    public       byte   flow_system;
    public       uint   cursor;
    public       uint   exp_data;
    public       uint   dmg_data;
    public       ushort sub_window;
    public       ushort atb_cost;
    public       ushort chant_cost;
    public       byte   mp;
    public       byte   target;
    public       byte   calc_ps;
    public       byte   critical;
    public       byte   hit;
    public       byte   power;
    public       byte   atc_num;
    public       byte   atc_stone;
    public       byte   atc_element;
    public fixed byte   atc_status[24];
    public fixed byte   atc_status2[24];
    public fixed sbyte  status_time[24];
    public       byte   icon;
    public       ushort monster_killer;
    public       byte   magic_cancel;
    public       byte   __UNUSED1;
    public       ushort blue_magic;
    public       ushort __UNUSED2;
    public       byte   btl_seq;
    public       byte   get_ap;
    public       ushort ap;
    public       ushort use_job;
    public       ushort __UNUSED4;
}
