// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

// ffx_ps2/ffx2/master/jppc/lastmiss/kernel/lm_command.h
// ffx_ps2/ffx2/master/jppc/lastmiss/kernel/lm_command.ath
// Steam release of FFX/X-2 HD

namespace Fahrenheit.FFX2;

public partial struct LmCommand
{
    [NativeTypeName("unsigned int")]
    public uint name;

    [NativeTypeName("unsigned char")]
    public byte name_yn;

    [NativeTypeName("unsigned char")]
    public byte dummy1;

    [NativeTypeName("unsigned char")]
    public byte dummy2;

    [NativeTypeName("unsigned char")]
    public byte dummy3;

    [NativeTypeName("unsigned int")]
    public uint help;

    [NativeTypeName("unsigned char")]
    public byte help_yn;

    [NativeTypeName("unsigned char")]
    public byte dummy4;

    [NativeTypeName("unsigned char")]
    public byte dummy5;

    [NativeTypeName("unsigned char")]
    public byte dummy6;

    [NativeTypeName("unsigned int")]
    public uint information;

    [NativeTypeName("unsigned short")]
    public ushort personal_job;

    [NativeTypeName("unsigned char")]
    public byte char_job;

    [NativeTypeName("unsigned char")]
    public byte short_dist;

    [NativeTypeName("unsigned char")]
    public byte shot_range;

    [NativeTypeName("unsigned char")]
    public byte long_dist;

    [NativeTypeName("unsigned char")]
    public byte long_range;

    [NativeTypeName("unsigned char")]
    public byte stairchk_yn;

    [NativeTypeName("unsigned char")]
    public byte cursol_dist_yn;

    [NativeTypeName("unsigned char")]
    public byte cursol_cat;

    [NativeTypeName("unsigned char")]
    public byte target_pos;

    [NativeTypeName("unsigned char")]
    public byte cmdend;

    [NativeTypeName("unsigned char")]
    public byte cmdend_menu;

    [NativeTypeName("unsigned char")]
    public byte dummy7;

    public short effect_no;

    [NativeTypeName("unsigned char")]
    public byte read_motion;

    [NativeTypeName("unsigned char")]
    public byte read_effect;

    [NativeTypeName("unsigned char")]
    public byte trun_change;

    [NativeTypeName("unsigned char")]
    public byte motion;

    [NativeTypeName("unsigned char")]
    public byte steal_st;

    [NativeTypeName("unsigned char")]
    public byte steal_hit;

    [NativeTypeName("unsigned char")]
    public byte use_mp;

    [NativeTypeName("unsigned char")]
    public byte use_hp;

    [NativeTypeName("unsigned char")]
    public byte target_param;

    [NativeTypeName("unsigned char")]
    public byte retdmg_motion;

    [NativeTypeName("unsigned char")]
    public byte critical;

    [NativeTypeName("unsigned char")]
    public byte calc_id;

    [NativeTypeName("unsigned short")]
    public ushort calc_no;

    [NativeTypeName("unsigned short")]
    public ushort atk_cnt;

    [NativeTypeName("unsigned short")]
    public ushort target_item_category;

    [NativeTypeName("unsigned char")]
    public byte category_abilty;

    [NativeTypeName("unsigned char")]
    public byte category_dmg_ret;

    [NativeTypeName("unsigned char")]
    public byte hit;

    [NativeTypeName("unsigned char")]
    public byte dark_hit;

    [NativeTypeName("unsigned char")]
    public byte hit_calc_id;

    [NativeTypeName("unsigned char")]
    public byte blue_magic;

    [NativeTypeName("unsigned char")]
    public byte attribute_atk;

    [NativeTypeName("unsigned char")]
    public byte conf_use;

    [NativeTypeName("unsigned char")]
    public byte jibaku;

    [NativeTypeName("unsigned char")]
    public byte status_chg_target;

    [NativeTypeName("unsigned char")]
    public byte status_chg;

    [NativeTypeName("unsigned char")]
    public byte status_onoff;

    [NativeTypeName("unsigned char")]
    public byte status_hit;
}
