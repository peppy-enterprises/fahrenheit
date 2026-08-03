// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

// ffx_ps2/ffx2/master/jppc/lastmiss/kernel/lm_monster.h
// Steam release of FFX/X-2 HD

namespace Fahrenheit.FFX2;

public partial struct LmMonster
{
    [NativeTypeName("unsigned int")]
    public uint name;

    [NativeTypeName("unsigned int")]
    public uint help;

    [NativeTypeName("unsigned char")]
    public byte lv;

    [NativeTypeName("unsigned char")]
    public byte dummy1;

    [NativeTypeName("unsigned char")]
    public byte dummy2;

    [NativeTypeName("unsigned char")]
    public byte dummy3;

    [NativeTypeName("unsigned int")]
    public uint hp;

    [NativeTypeName("unsigned int")]
    public uint mp;

    [NativeTypeName("unsigned char")]
    public byte str;

    [NativeTypeName("unsigned char")]
    public byte mag;

    [NativeTypeName("unsigned char")]
    public byte vit;

    [NativeTypeName("unsigned char")]
    public byte spirit;

    [NativeTypeName("unsigned char")]
    public byte hit;

    [NativeTypeName("unsigned char")]
    public byte avoid;

    [NativeTypeName("unsigned char")]
    public byte dummy4;

    [NativeTypeName("unsigned char")]
    public byte dummy5;

    [NativeTypeName("unsigned int")]
    public uint os_hp;

    [NativeTypeName("unsigned int")]
    public uint os_mp;

    [NativeTypeName("unsigned char")]
    public byte os_str;

    [NativeTypeName("unsigned char")]
    public byte os_mag;

    [NativeTypeName("unsigned char")]
    public byte os_vit;

    [NativeTypeName("unsigned char")]
    public byte os_spirit;

    [NativeTypeName("unsigned char")]
    public byte os_hit;

    [NativeTypeName("unsigned char")]
    public byte os_avoid;

    [NativeTypeName("unsigned char")]
    public byte move;

    [NativeTypeName("unsigned char")]
    public byte fix_dmg;

    [NativeTypeName("unsigned char")]
    public byte stair_move;

    [NativeTypeName("unsigned char")]
    public byte size_square;

    [NativeTypeName("unsigned char")]
    public byte dummy6;

    [NativeTypeName("unsigned char")]
    public byte dummy7;

    [NativeTypeName("unsigned int")]
    public uint size_real;

    [NativeTypeName("unsigned char")]
    public byte think_movepat;

    [NativeTypeName("unsigned char")]
    public byte dummy8;

    [NativeTypeName("unsigned char")]
    public byte dummy9;

    [NativeTypeName("unsigned char")]
    public byte dummy10;

    [NativeTypeName("unsigned int")]
    public uint view_dist_normal;

    [NativeTypeName("unsigned char")]
    public byte view_range_normal;

    [NativeTypeName("unsigned char")]
    public byte dummy11;

    [NativeTypeName("unsigned char")]
    public byte dummy12;

    [NativeTypeName("unsigned char")]
    public byte dummy13;

    [NativeTypeName("unsigned int")]
    public uint view_dist_battle;

    [NativeTypeName("unsigned char")]
    public byte view_range_battle;

    [NativeTypeName("unsigned char")]
    public byte view_obstacle;

    [NativeTypeName("unsigned char")]
    public byte effe_zantetsu;

    [NativeTypeName("unsigned char")]
    public byte hit_zantetsu;

    [NativeTypeName("unsigned char")]
    public byte hit_carry_mon;

    [NativeTypeName("unsigned char")]
    public byte ele_holy;

    [NativeTypeName("unsigned char")]
    public byte ele_gravit;

    [NativeTypeName("unsigned char")]
    public byte ele_fire;

    [NativeTypeName("unsigned char")]
    public byte ele_thunder;

    [NativeTypeName("unsigned char")]
    public byte ele_ice;

    [NativeTypeName("unsigned char")]
    public byte ele_water;

    [NativeTypeName("unsigned char")]
    public byte dummy14;

    [NativeTypeName("unsigned short")]
    public ushort item;

    [NativeTypeName("unsigned char")]
    public byte item_data;

    [NativeTypeName("unsigned char")]
    public byte steal_item_hit;

    [NativeTypeName("unsigned short")]
    public ushort os_item;

    [NativeTypeName("unsigned char")]
    public byte os_item_data;

    [NativeTypeName("unsigned char")]
    public byte os_steal_item_hit;

    public int steal_mon_skill;

    [NativeTypeName("unsigned char")]
    public byte steal_mon_skill_hit;

    [NativeTypeName("unsigned char")]
    public byte dummy15;

    [NativeTypeName("unsigned char")]
    public byte dummy16;

    [NativeTypeName("unsigned char")]
    public byte dummy17;

    public int exp;

    [NativeTypeName("unsigned char")]
    public byte ap;

    [NativeTypeName("unsigned char")]
    public byte steal_exp_hit;

    [NativeTypeName("unsigned char")]
    public byte steal_exp_rate;

    [NativeTypeName("unsigned char")]
    public byte mgun_atk;

    [NativeTypeName("unsigned char")]
    public byte type_sky;

    [NativeTypeName("unsigned char")]
    public byte prohibit_timestop;

    [NativeTypeName("unsigned char")]
    public byte prohibit_smell_player;

    [NativeTypeName("unsigned char")]
    public byte prohibit_ratedmg;

    [NativeTypeName("unsigned char")]
    public byte prohibit_posion;

    [NativeTypeName("unsigned char")]
    public byte prohibit_blindness;

    [NativeTypeName("unsigned char")]
    public byte prohibit_sleep;

    [NativeTypeName("unsigned char")]
    public byte prohibit_confusion;

    [NativeTypeName("unsigned char")]
    public byte prohibit_stop;

    [NativeTypeName("unsigned char")]
    public byte prohibit_dead_count;

    [NativeTypeName("unsigned char")]
    public byte prohibit_silence;

    [NativeTypeName("unsigned char")]
    public byte prohibit_berserk;

    [NativeTypeName("unsigned char")]
    public byte prohibit_slow;

    [NativeTypeName("unsigned char")]
    public byte prohibit_movestop;

    [NativeTypeName("unsigned char")]
    public byte prohibit_consump_2mp;

    [NativeTypeName("unsigned char")]
    public byte prohibit_drop_money;

    [NativeTypeName("unsigned char")]
    public byte prohibit_oversoul;

    [NativeTypeName("unsigned char")]
    public byte prohibit_change_money;

    [NativeTypeName("unsigned char")]
    public byte prohibit_change_dress;
}
