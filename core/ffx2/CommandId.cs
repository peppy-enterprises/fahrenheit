// SPDX-License-Identifier: MIT

// ffx_ps2/ffx2/master/jppc/battle/kernel/command.h 
// Switch release of FFX/X-2 HD

namespace Fahrenheit.Core.FFX2;

public static partial class CommandId
{
    [NativeTypeName("#define pcom_item 0x03000")]
    public const int pcom_item = 0x03000;

    [NativeTypeName("#define pcom_escape 0x03001")]
    public const int pcom_escape = 0x03001;

    [NativeTypeName("#define pcom_change 0x03002")]
    public const int pcom_change = 0x03002;

    [NativeTypeName("#define pcom_reserve00 0x03003")]
    public const int pcom_reserve00 = 0x03003;

    [NativeTypeName("#define pcom_reserve01 0x03004")]
    public const int pcom_reserve01 = 0x03004;

    [NativeTypeName("#define pcom_reserve02 0x03005")]
    public const int pcom_reserve02 = 0x03005;

    [NativeTypeName("#define pcom_reserve03 0x03006")]
    public const int pcom_reserve03 = 0x03006;

    [NativeTypeName("#define pcom_reserve04 0x03007")]
    public const int pcom_reserve04 = 0x03007;

    [NativeTypeName("#define pcom_acrobat 0x03008")]
    public const int pcom_acrobat = 0x03008;

    [NativeTypeName("#define pcom_pgunnner 0x03009")]
    public const int pcom_pgunnner = 0x03009;

    [NativeTypeName("#define pcom_monmagic 0x0300a")]
    public const int pcom_monmagic = 0x0300a;

    [NativeTypeName("#define pcom_anti_mons 0x0300b")]
    public const int pcom_anti_mons = 0x0300b;

    [NativeTypeName("#define pcom_dance 0x0300c")]
    public const int pcom_dance = 0x0300c;

    [NativeTypeName("#define pcom_song 0x0300d")]
    public const int pcom_song = 0x0300d;

    [NativeTypeName("#define pcom_yuna_kigu_skill 0x0300e")]
    public const int pcom_yuna_kigu_skill = 0x0300e;

    [NativeTypeName("#define pcom_rikku_kigu_skill 0x0300f")]
    public const int pcom_rikku_kigu_skill = 0x0300f;

    [NativeTypeName("#define pcom_pain_kigu_skill 0x03010")]
    public const int pcom_pain_kigu_skill = 0x03010;

    [NativeTypeName("#define pcom_steal_skill 0x03011")]
    public const int pcom_steal_skill = 0x03011;

    [NativeTypeName("#define pcom_gambl_skill 0x03012")]
    public const int pcom_gambl_skill = 0x03012;

    [NativeTypeName("#define pcom_dog_skill 0x03013")]
    public const int pcom_dog_skill = 0x03013;

    [NativeTypeName("#define pcom_monkey_skill 0x03014")]
    public const int pcom_monkey_skill = 0x03014;

    [NativeTypeName("#define pcom_bird_skill 0x03015")]
    public const int pcom_bird_skill = 0x03015;

    [NativeTypeName("#define pcom_yuna_sp1skill 0x03016")]
    public const int pcom_yuna_sp1skill = 0x03016;

    [NativeTypeName("#define pcom_yuna_sp2skill 0x03017")]
    public const int pcom_yuna_sp2skill = 0x03017;

    [NativeTypeName("#define pcom_yuna_sp3skill 0x03018")]
    public const int pcom_yuna_sp3skill = 0x03018;

    [NativeTypeName("#define pcom_rikku_sp1skill 0x03019")]
    public const int pcom_rikku_sp1skill = 0x03019;

    [NativeTypeName("#define pcom_rikku_sp2skill 0x0301a")]
    public const int pcom_rikku_sp2skill = 0x0301a;

    [NativeTypeName("#define pcom_rikku_sp3skill 0x0301b")]
    public const int pcom_rikku_sp3skill = 0x0301b;

    [NativeTypeName("#define pcom_pain_sp1skill 0x0301c")]
    public const int pcom_pain_sp1skill = 0x0301c;

    [NativeTypeName("#define pcom_pain_sp2skill 0x0301d")]
    public const int pcom_pain_sp2skill = 0x0301d;

    [NativeTypeName("#define pcom_pain_sp3skill 0x0301e")]
    public const int pcom_pain_sp3skill = 0x0301e;

    [NativeTypeName("#define pcom_kenwaza 0x0301f")]
    public const int pcom_kenwaza = 0x0301f;

    [NativeTypeName("#define pcom_ougi 0x03020")]
    public const int pcom_ougi = 0x03020;

    [NativeTypeName("#define pcom_dark_magic 0x03021")]
    public const int pcom_dark_magic = 0x03021;

    [NativeTypeName("#define pcom_kyobou 0x03022")]
    public const int pcom_kyobou = 0x03022;

    [NativeTypeName("#define pcom_black_magic 0x03023")]
    public const int pcom_black_magic = 0x03023;

    [NativeTypeName("#define pcom_white_magic 0x03024")]
    public const int pcom_white_magic = 0x03024;

    [NativeTypeName("#define pcom_re_life_ex 0x03025")]
    public const int pcom_re_life_ex = 0x03025;

    [NativeTypeName("#define pcom_death_ex 0x03026")]
    public const int pcom_death_ex = 0x03026;

    [NativeTypeName("#define pcom_berserk_gun 0x03027")]
    public const int pcom_berserk_gun = 0x03027;

    [NativeTypeName("#define pcom_berserk_gun2 0x03028")]
    public const int pcom_berserk_gun2 = 0x03028;

    [NativeTypeName("#define pcom_berserk_thi 0x03029")]
    public const int pcom_berserk_thi = 0x03029;

    [NativeTypeName("#define pcom_berserk_sol 0x0302a")]
    public const int pcom_berserk_sol = 0x0302a;

    [NativeTypeName("#define pcom_berserk_tr 0x0302b")]
    public const int pcom_berserk_tr = 0x0302b;

    [NativeTypeName("#define pcom_attack_gun 0x0302c")]
    public const int pcom_attack_gun = 0x0302c;

    [NativeTypeName("#define pcom_attack_sol 0x0302d")]
    public const int pcom_attack_sol = 0x0302d;

    [NativeTypeName("#define pcom_attack_thi 0x0302e")]
    public const int pcom_attack_thi = 0x0302e;

    [NativeTypeName("#define pcom_attack_tr 0x0302f")]
    public const int pcom_attack_tr = 0x0302f;

    [NativeTypeName("#define pcom_attack_yuna_sp 0x03030")]
    public const int pcom_attack_yuna_sp = 0x03030;

    [NativeTypeName("#define pcom_attack_pain_sp 0x03031")]
    public const int pcom_attack_pain_sp = 0x03031;

    [NativeTypeName("#define pcom_quick_shot 0x03032")]
    public const int pcom_quick_shot = 0x03032;

    [NativeTypeName("#define pcom_certain_shot 0x03033")]
    public const int pcom_certain_shot = 0x03033;

    [NativeTypeName("#define pcom_weak_shot 0x03034")]
    public const int pcom_weak_shot = 0x03034;

    [NativeTypeName("#define pcom_magic_shot 0x03035")]
    public const int pcom_magic_shot = 0x03035;

    [NativeTypeName("#define pcom_mental_shot 0x03036")]
    public const int pcom_mental_shot = 0x03036;

    [NativeTypeName("#define pcom_rate_shot 0x03037")]
    public const int pcom_rate_shot = 0x03037;

    [NativeTypeName("#define pcom_level_shot 0x03038")]
    public const int pcom_level_shot = 0x03038;

    [NativeTypeName("#define pcom_critical_shot 0x03039")]
    public const int pcom_critical_shot = 0x03039;

    [NativeTypeName("#define pcom_armor_shot 0x0303a")]
    public const int pcom_armor_shot = 0x0303a;

    [NativeTypeName("#define pcom_grape_shot 0x0303b")]
    public const int pcom_grape_shot = 0x0303b;

    [NativeTypeName("#define pcom_grape_shot2 0x0303c")]
    public const int pcom_grape_shot2 = 0x0303c;

    [NativeTypeName("#define pcom_libra 0x0303d")]
    public const int pcom_libra = 0x0303d;

    [NativeTypeName("#define pcom_kille_shell 0x0303e")]
    public const int pcom_kille_shell = 0x0303e;

    [NativeTypeName("#define pcom_kille_wing 0x0303f")]
    public const int pcom_kille_wing = 0x0303f;

    [NativeTypeName("#define pcom_kille_wolf 0x03040")]
    public const int pcom_kille_wolf = 0x03040;

    [NativeTypeName("#define pcom_kille_pudding 0x03041")]
    public const int pcom_kille_pudding = 0x03041;

    [NativeTypeName("#define pcom_kille_element 0x03042")]
    public const int pcom_kille_element = 0x03042;

    [NativeTypeName("#define pcom_kille_lizard 0x03043")]
    public const int pcom_kille_lizard = 0x03043;

    [NativeTypeName("#define pcom_kille_dragon 0x03044")]
    public const int pcom_kille_dragon = 0x03044;

    [NativeTypeName("#define pcom_kille_machine 0x03045")]
    public const int pcom_kille_machine = 0x03045;

    [NativeTypeName("#define pcom_kille_magic 0x03046")]
    public const int pcom_kille_magic = 0x03046;

    [NativeTypeName("#define pcom_kille_devil 0x03047")]
    public const int pcom_kille_devil = 0x03047;

    [NativeTypeName("#define pcom_fire_breathe 0x03048")]
    public const int pcom_fire_breathe = 0x03048;

    [NativeTypeName("#define pcom_seed_cannon 0x03049")]
    public const int pcom_seed_cannon = 0x03049;

    [NativeTypeName("#define pcom_stone_breathe 0x0304a")]
    public const int pcom_stone_breathe = 0x0304a;

    [NativeTypeName("#define pcom_dragon_sword 0x0304b")]
    public const int pcom_dragon_sword = 0x0304b;

    [NativeTypeName("#define pcom_white_wind 0x0304c")]
    public const int pcom_white_wind = 0x0304c;

    [NativeTypeName("#define pcom_badbreath 0x0304d")]
    public const int pcom_badbreath = 0x0304d;

    [NativeTypeName("#define pcom_mighty_guard 0x0304e")]
    public const int pcom_mighty_guard = 0x0304e;

    [NativeTypeName("#define pcom_sunshine 0x0304f")]
    public const int pcom_sunshine = 0x0304f;

    [NativeTypeName("#define pcom_seymour_special 0x03050")]
    public const int pcom_seymour_special = 0x03050;

    [NativeTypeName("#define pcom_explosive_shell 0x03051")]
    public const int pcom_explosive_shell = 0x03051;

    [NativeTypeName("#define pcom_mortar 0x03052")]
    public const int pcom_mortar = 0x03052;

    [NativeTypeName("#define pcom_annihilate_gun 0x03053")]
    public const int pcom_annihilate_gun = 0x03053;

    [NativeTypeName("#define pcom_supersonic 0x03054")]
    public const int pcom_supersonic = 0x03054;

    [NativeTypeName("#define pcom_1000needle 0x03055")]
    public const int pcom_1000needle = 0x03055;

    [NativeTypeName("#define pcom_nuclear 0x03056")]
    public const int pcom_nuclear = 0x03056;

    [NativeTypeName("#define pcom_blaster 0x03057")]
    public const int pcom_blaster = 0x03057;

    [NativeTypeName("#define pcom_cyougou 0x03058")]
    public const int pcom_cyougou = 0x03058;

    [NativeTypeName("#define pcom_hidden_potion 0x03059")]
    public const int pcom_hidden_potion = 0x03059;

    [NativeTypeName("#define pcom_hidden_hipotion 0x0305a")]
    public const int pcom_hidden_hipotion = 0x0305a;

    [NativeTypeName("#define pcom_hidden_megapotion 0x0305b")]
    public const int pcom_hidden_megapotion = 0x0305b;

    [NativeTypeName("#define pcom_hidden_expotion 0x0305c")]
    public const int pcom_hidden_expotion = 0x0305c;

    [NativeTypeName("#define pcom_hidden_antidote 0x0305d")]
    public const int pcom_hidden_antidote = 0x0305d;

    [NativeTypeName("#define pcom_hidden_dispel 0x0305e")]
    public const int pcom_hidden_dispel = 0x0305e;

    [NativeTypeName("#define pcom_hidden_phenix 0x0305f")]
    public const int pcom_hidden_phenix = 0x0305f;

    [NativeTypeName("#define pcom_hidden_megaphenix 0x03060")]
    public const int pcom_hidden_megaphenix = 0x03060;

    [NativeTypeName("#define pcom_hidden_atel 0x03061")]
    public const int pcom_hidden_atel = 0x03061;

    [NativeTypeName("#define pcom_hidden_elixir 0x03062")]
    public const int pcom_hidden_elixir = 0x03062;

    [NativeTypeName("#define pcom_charge 0x03063")]
    public const int pcom_charge = 0x03063;

    [NativeTypeName("#define pcom_defence 0x03064")]
    public const int pcom_defence = 0x03064;

    [NativeTypeName("#define pcom_power_break 0x03065")]
    public const int pcom_power_break = 0x03065;

    [NativeTypeName("#define pcom_armor_break 0x03066")]
    public const int pcom_armor_break = 0x03066;

    [NativeTypeName("#define pcom_magic_break 0x03067")]
    public const int pcom_magic_break = 0x03067;

    [NativeTypeName("#define pcom_mental_break 0x03068")]
    public const int pcom_mental_break = 0x03068;

    [NativeTypeName("#define pcom_fire_sword 0x03069")]
    public const int pcom_fire_sword = 0x03069;

    [NativeTypeName("#define pcom_cold_sword 0x0306a")]
    public const int pcom_cold_sword = 0x0306a;

    [NativeTypeName("#define pcom_thunder_sword 0x0306b")]
    public const int pcom_thunder_sword = 0x0306b;

    [NativeTypeName("#define pcom_water_sword 0x0306c")]
    public const int pcom_water_sword = 0x0306c;

    [NativeTypeName("#define pcom_gravity_sword 0x0306d")]
    public const int pcom_gravity_sword = 0x0306d;

    [NativeTypeName("#define pcom_holy_sword 0x0306e")]
    public const int pcom_holy_sword = 0x0306e;

    [NativeTypeName("#define pcom_order 0x0306f")]
    public const int pcom_order = 0x0306f;

    [NativeTypeName("#define pcom_order2 0x03070")]
    public const int pcom_order2 = 0x03070;

    [NativeTypeName("#define pcom_sagaku_ken 0x03071")]
    public const int pcom_sagaku_ken = 0x03071;

    [NativeTypeName("#define pcom_mp_giri 0x03072")]
    public const int pcom_mp_giri = 0x03072;

    [NativeTypeName("#define pcom_cansel_ken 0x03073")]
    public const int pcom_cansel_ken = 0x03073;

    [NativeTypeName("#define pcom_kiai_ken 0x03074")]
    public const int pcom_kiai_ken = 0x03074;

    [NativeTypeName("#define pcom_aura_shoot_solo 0x03075")]
    public const int pcom_aura_shoot_solo = 0x03075;

    [NativeTypeName("#define pcom_aura_shoot_all 0x03076")]
    public const int pcom_aura_shoot_all = 0x03076;

    [NativeTypeName("#define pcom_training_ken 0x03077")]
    public const int pcom_training_ken = 0x03077;

    [NativeTypeName("#define pcom_iainuki 0x03078")]
    public const int pcom_iainuki = 0x03078;

    [NativeTypeName("#define pcom_samurai_hit 0x03079")]
    public const int pcom_samurai_hit = 0x03079;

    [NativeTypeName("#define pcom_samurai_guard 0x0307a")]
    public const int pcom_samurai_guard = 0x0307a;

    [NativeTypeName("#define pcom_samurai_soul 0x0307b")]
    public const int pcom_samurai_soul = 0x0307b;

    [NativeTypeName("#define pcom_samurai_speed 0x0307c")]
    public const int pcom_samurai_speed = 0x0307c;

    [NativeTypeName("#define pcom_zeninage 0x0307d")]
    public const int pcom_zeninage = 0x0307d;

    [NativeTypeName("#define pcom_zantetu 0x0307e")]
    public const int pcom_zantetu = 0x0307e;

    [NativeTypeName("#define pcom_dark_power 0x0307f")]
    public const int pcom_dark_power = 0x0307f;

    [NativeTypeName("#define pcom_Kamikaze 0x03080")]
    public const int pcom_Kamikaze = 0x03080;

    [NativeTypeName("#define pcom_drain 0x03081")]
    public const int pcom_drain = 0x03081;

    [NativeTypeName("#define pcom_demi 0x03082")]
    public const int pcom_demi = 0x03082;

    [NativeTypeName("#define pcom_confu 0x03083")]
    public const int pcom_confu = 0x03083;

    [NativeTypeName("#define pcom_stone 0x03084")]
    public const int pcom_stone = 0x03084;

    [NativeTypeName("#define pcom_bio 0x03085")]
    public const int pcom_bio = 0x03085;

    [NativeTypeName("#define pcom_sentence 0x03086")]
    public const int pcom_sentence = 0x03086;

    [NativeTypeName("#define pcom_death 0x03087")]
    public const int pcom_death = 0x03087;

    [NativeTypeName("#define pcom_darksky 0x03088")]
    public const int pcom_darksky = 0x03088;

    [NativeTypeName("#define pcom_berserk 0x03089")]
    public const int pcom_berserk = 0x03089;

    [NativeTypeName("#define pcom_kiai_bakuhatu 0x0308a")]
    public const int pcom_kiai_bakuhatu = 0x0308a;

    [NativeTypeName("#define pcom_hp_half 0x0308b")]
    public const int pcom_hp_half = 0x0308b;

    [NativeTypeName("#define pcom_collision 0x0308c")]
    public const int pcom_collision = 0x0308c;

    [NativeTypeName("#define pcom_barrier_break 0x0308d")]
    public const int pcom_barrier_break = 0x0308d;

    [NativeTypeName("#define pcom_strike_blow 0x0308e")]
    public const int pcom_strike_blow = 0x0308e;

    [NativeTypeName("#define pcom_hit_avoid_down 0x0308f")]
    public const int pcom_hit_avoid_down = 0x0308f;

    [NativeTypeName("#define pcom_slow_claw 0x03090")]
    public const int pcom_slow_claw = 0x03090;

    [NativeTypeName("#define pcom_poison_claw 0x03091")]
    public const int pcom_poison_claw = 0x03091;

    [NativeTypeName("#define pcom_rest_hp_damage 0x03092")]
    public const int pcom_rest_hp_damage = 0x03092;

    [NativeTypeName("#define pcom_dark_dance 0x03093")]
    public const int pcom_dark_dance = 0x03093;

    [NativeTypeName("#define pcom_silence_dance 0x03094")]
    public const int pcom_silence_dance = 0x03094;

    [NativeTypeName("#define pcom_use_mp0_dance 0x03095")]
    public const int pcom_use_mp0_dance = 0x03095;

    [NativeTypeName("#define pcom_stguard_dance 0x03096")]
    public const int pcom_stguard_dance = 0x03096;

    [NativeTypeName("#define pcom_sleep_dance 0x03097")]
    public const int pcom_sleep_dance = 0x03097;

    [NativeTypeName("#define pcom_hp_double_dance 0x03098")]
    public const int pcom_hp_double_dance = 0x03098;

    [NativeTypeName("#define pcom_slow_dance 0x03099")]
    public const int pcom_slow_dance = 0x03099;

    [NativeTypeName("#define pcom_stop_dance 0x0309a")]
    public const int pcom_stop_dance = 0x0309a;

    [NativeTypeName("#define pcom_haste_dance 0x0309b")]
    public const int pcom_haste_dance = 0x0309b;

    [NativeTypeName("#define pcom_critical_dance 0x0309c")]
    public const int pcom_critical_dance = 0x0309c;

    [NativeTypeName("#define pcom_str_song 0x0309d")]
    public const int pcom_str_song = 0x0309d;

    [NativeTypeName("#define pcom_vit_song 0x0309e")]
    public const int pcom_vit_song = 0x0309e;

    [NativeTypeName("#define pcom_mag_song 0x0309f")]
    public const int pcom_mag_song = 0x0309f;

    [NativeTypeName("#define pcom_spirit_song 0x030a0")]
    public const int pcom_spirit_song = 0x030a0;

    [NativeTypeName("#define pcom_hit_song 0x030a1")]
    public const int pcom_hit_song = 0x030a1;

    [NativeTypeName("#define pcom_avoid_song 0x030a2")]
    public const int pcom_avoid_song = 0x030a2;

    [NativeTypeName("#define pcom_concent 0x030a3")]
    public const int pcom_concent = 0x030a3;

    [NativeTypeName("#define pcom_mp_get 0x030a4")]
    public const int pcom_mp_get = 0x030a4;

    [NativeTypeName("#define pcom_fire 0x030a5")]
    public const int pcom_fire = 0x030a5;

    [NativeTypeName("#define pcom_blizzard 0x030a6")]
    public const int pcom_blizzard = 0x030a6;

    [NativeTypeName("#define pcom_thunder 0x030a7")]
    public const int pcom_thunder = 0x030a7;

    [NativeTypeName("#define pcom_water 0x030a8")]
    public const int pcom_water = 0x030a8;

    [NativeTypeName("#define pcom_fira 0x030a9")]
    public const int pcom_fira = 0x030a9;

    [NativeTypeName("#define pcom_blizzara 0x030aa")]
    public const int pcom_blizzara = 0x030aa;

    [NativeTypeName("#define pcom_thundara 0x030ab")]
    public const int pcom_thundara = 0x030ab;

    [NativeTypeName("#define pcom_watera 0x030ac")]
    public const int pcom_watera = 0x030ac;

    [NativeTypeName("#define pcom_firaga 0x030ad")]
    public const int pcom_firaga = 0x030ad;

    [NativeTypeName("#define pcom_blizzaga 0x030ae")]
    public const int pcom_blizzaga = 0x030ae;

    [NativeTypeName("#define pcom_thundaga 0x030af")]
    public const int pcom_thundaga = 0x030af;

    [NativeTypeName("#define pcom_wateraga 0x030b0")]
    public const int pcom_wateraga = 0x030b0;

    [NativeTypeName("#define pcom_bless 0x030b1")]
    public const int pcom_bless = 0x030b1;

    [NativeTypeName("#define pcom_self_healing 0x030b2")]
    public const int pcom_self_healing = 0x030b2;

    [NativeTypeName("#define pcom_cure 0x030b3")]
    public const int pcom_cure = 0x030b3;

    [NativeTypeName("#define pcom_cura 0x030b4")]
    public const int pcom_cura = 0x030b4;

    [NativeTypeName("#define pcom_curaga 0x030b5")]
    public const int pcom_curaga = 0x030b5;

    [NativeTypeName("#define pcom_regen 0x030b6")]
    public const int pcom_regen = 0x030b6;

    [NativeTypeName("#define pcom_esuna 0x030b7")]
    public const int pcom_esuna = 0x030b7;

    [NativeTypeName("#define pcom_dispel 0x030b8")]
    public const int pcom_dispel = 0x030b8;

    [NativeTypeName("#define pcom_life 0x030b9")]
    public const int pcom_life = 0x030b9;

    [NativeTypeName("#define pcom_full_life 0x030ba")]
    public const int pcom_full_life = 0x030ba;

    [NativeTypeName("#define pcom_shell 0x030bb")]
    public const int pcom_shell = 0x030bb;

    [NativeTypeName("#define pcom_protess 0x030bc")]
    public const int pcom_protess = 0x030bc;

    [NativeTypeName("#define pcom_reflect 0x030bd")]
    public const int pcom_reflect = 0x030bd;

    [NativeTypeName("#define pcom_full_cure 0x030be")]
    public const int pcom_full_cure = 0x030be;

    [NativeTypeName("#define pcom_steal 0x030bf")]
    public const int pcom_steal = 0x030bf;

    [NativeTypeName("#define pcom_steal_gill 0x030c0")]
    public const int pcom_steal_gill = 0x030c0;

    [NativeTypeName("#define pcom_steal_time 0x030c1")]
    public const int pcom_steal_time = 0x030c1;

    [NativeTypeName("#define pcom_steal_hp 0x030c2")]
    public const int pcom_steal_hp = 0x030c2;

    [NativeTypeName("#define pcom_steal_mp 0x030c3")]
    public const int pcom_steal_mp = 0x030c3;

    [NativeTypeName("#define pcom_steal_sure 0x030c4")]
    public const int pcom_steal_sure = 0x030c4;

    [NativeTypeName("#define pcom_steal_good 0x030c5")]
    public const int pcom_steal_good = 0x030c5;

    [NativeTypeName("#define pcom_steal_heart 0x030c6")]
    public const int pcom_steal_heart = 0x030c6;

    [NativeTypeName("#define pcom_steal_will 0x030c7")]
    public const int pcom_steal_will = 0x030c7;

    [NativeTypeName("#define pcom_nigeru 0x030c8")]
    public const int pcom_nigeru = 0x030c8;

    [NativeTypeName("#define pcom_dog_fire 0x030c9")]
    public const int pcom_dog_fire = 0x030c9;

    [NativeTypeName("#define pcom_dog_ice 0x030ca")]
    public const int pcom_dog_ice = 0x030ca;

    [NativeTypeName("#define pcom_dog_thunder 0x030cb")]
    public const int pcom_dog_thunder = 0x030cb;

    [NativeTypeName("#define pcom_dog_water 0x030cc")]
    public const int pcom_dog_water = 0x030cc;

    [NativeTypeName("#define pcom_dog_cannon 0x030cd")]
    public const int pcom_dog_cannon = 0x030cd;

    [NativeTypeName("#define pcom_dog_sentence 0x030ce")]
    public const int pcom_dog_sentence = 0x030ce;

    [NativeTypeName("#define pcom_dog_cure 0x030cf")]
    public const int pcom_dog_cure = 0x030cf;

    [NativeTypeName("#define pcom_dog_esuna 0x030d0")]
    public const int pcom_dog_esuna = 0x030d0;

    [NativeTypeName("#define pcom_dog_holy 0x030d1")]
    public const int pcom_dog_holy = 0x030d1;

    [NativeTypeName("#define pcom_dog_friend 0x030d2")]
    public const int pcom_dog_friend = 0x030d2;

    [NativeTypeName("#define pcom_monkey_dark 0x030d3")]
    public const int pcom_monkey_dark = 0x030d3;

    [NativeTypeName("#define pcom_monkey_silence 0x030d4")]
    public const int pcom_monkey_silence = 0x030d4;

    [NativeTypeName("#define pcom_monkey_steal 0x030d5")]
    public const int pcom_monkey_steal = 0x030d5;

    [NativeTypeName("#define pcom_monkey_berserk 0x030d6")]
    public const int pcom_monkey_berserk = 0x030d6;

    [NativeTypeName("#define pcom_monkey_cancel 0x030d7")]
    public const int pcom_monkey_cancel = 0x030d7;

    [NativeTypeName("#define pcom_monkey_cure 0x030d8")]
    public const int pcom_monkey_cure = 0x030d8;

    [NativeTypeName("#define pcom_monkey_esuna 0x030d9")]
    public const int pcom_monkey_esuna = 0x030d9;

    [NativeTypeName("#define pcom_monkey_support 0x030da")]
    public const int pcom_monkey_support = 0x030da;

    [NativeTypeName("#define pcom_monkey_money 0x030db")]
    public const int pcom_monkey_money = 0x030db;

    [NativeTypeName("#define pcom_monkey_friend 0x030dc")]
    public const int pcom_monkey_friend = 0x030dc;

    [NativeTypeName("#define pcom_bird_poison 0x030dd")]
    public const int pcom_bird_poison = 0x030dd;

    [NativeTypeName("#define pcom_bird_stone 0x030de")]
    public const int pcom_bird_stone = 0x030de;

    [NativeTypeName("#define pcom_bird_death 0x030df")]
    public const int pcom_bird_death = 0x030df;

    [NativeTypeName("#define pcom_bird_protess 0x030e0")]
    public const int pcom_bird_protess = 0x030e0;

    [NativeTypeName("#define pcom_bird_heist 0x030e1")]
    public const int pcom_bird_heist = 0x030e1;

    [NativeTypeName("#define pcom_bird_shell 0x030e2")]
    public const int pcom_bird_shell = 0x030e2;

    [NativeTypeName("#define pcom_bird_cure 0x030e3")]
    public const int pcom_bird_cure = 0x030e3;

    [NativeTypeName("#define pcom_bird_esuna 0x030e4")]
    public const int pcom_bird_esuna = 0x030e4;

    [NativeTypeName("#define pcom_bird_carry 0x030e5")]
    public const int pcom_bird_carry = 0x030e5;

    [NativeTypeName("#define pcom_bird_friend 0x030e6")]
    public const int pcom_bird_friend = 0x030e6;

    [NativeTypeName("#define pcom_slot01 0x030e7")]
    public const int pcom_slot01 = 0x030e7;

    [NativeTypeName("#define pcom_slot02 0x030e8")]
    public const int pcom_slot02 = 0x030e8;

    [NativeTypeName("#define pcom_slot03 0x030e9")]
    public const int pcom_slot03 = 0x030e9;

    [NativeTypeName("#define pcom_slot04 0x030ea")]
    public const int pcom_slot04 = 0x030ea;

    [NativeTypeName("#define pcom_2dice_1 0x030eb")]
    public const int pcom_2dice_1 = 0x030eb;

    [NativeTypeName("#define pcom_4dice_1 0x030ec")]
    public const int pcom_4dice_1 = 0x030ec;

    [NativeTypeName("#define pcom_luck_myself 0x030ed")]
    public const int pcom_luck_myself = 0x030ed;

    [NativeTypeName("#define pcom_luck_all 0x030ee")]
    public const int pcom_luck_all = 0x030ee;

    [NativeTypeName("#define pcom_tempt 0x030ef")]
    public const int pcom_tempt = 0x030ef;

    [NativeTypeName("#define pcom_wairo 0x030f0")]
    public const int pcom_wairo = 0x030f0;

    [NativeTypeName("#define pcom_mog_cure 0x030f1")]
    public const int pcom_mog_cure = 0x030f1;

    [NativeTypeName("#define pcom_mog_regen 0x030f2")]
    public const int pcom_mog_regen = 0x030f2;

    [NativeTypeName("#define pcom_mog_wall 0x030f3")]
    public const int pcom_mog_wall = 0x030f3;

    [NativeTypeName("#define pcom_mog_full_life 0x030f4")]
    public const int pcom_mog_full_life = 0x030f4;

    [NativeTypeName("#define pcom_mog_cure2 0x030f5")]
    public const int pcom_mog_cure2 = 0x030f5;

    [NativeTypeName("#define pcom_mog_regen2 0x030f6")]
    public const int pcom_mog_regen2 = 0x030f6;

    [NativeTypeName("#define pcom_mog_wall2 0x030f7")]
    public const int pcom_mog_wall2 = 0x030f7;

    [NativeTypeName("#define pcom_mog_full_life2 0x030f8")]
    public const int pcom_mog_full_life2 = 0x030f8;

    [NativeTypeName("#define pcom_mog_mp_up 0x030f9")]
    public const int pcom_mog_mp_up = 0x030f9;

    [NativeTypeName("#define pcom_mog_last_attack 0x030fa")]
    public const int pcom_mog_last_attack = 0x030fa;

    [NativeTypeName("#define pcom_kett_fire 0x030fb")]
    public const int pcom_kett_fire = 0x030fb;

    [NativeTypeName("#define pcom_kett_cold 0x030fc")]
    public const int pcom_kett_cold = 0x030fc;

    [NativeTypeName("#define pcom_kett_thunder 0x030fd")]
    public const int pcom_kett_thunder = 0x030fd;

    [NativeTypeName("#define pcom_kett_water 0x030fe")]
    public const int pcom_kett_water = 0x030fe;

    [NativeTypeName("#define pcom_kett_str_down 0x030ff")]
    public const int pcom_kett_str_down = 0x030ff;

    [NativeTypeName("#define pcom_kett_vit_down 0x03100")]
    public const int pcom_kett_vit_down = 0x03100;

    [NativeTypeName("#define pcom_kett_mag_down 0x03101")]
    public const int pcom_kett_mag_down = 0x03101;

    [NativeTypeName("#define pcom_kett_spr_down 0x03102")]
    public const int pcom_kett_spr_down = 0x03102;

    [NativeTypeName("#define pcom_kett_avoid_down 0x03103")]
    public const int pcom_kett_avoid_down = 0x03103;

    [NativeTypeName("#define pcom_kett_last_attack 0x03104")]
    public const int pcom_kett_last_attack = 0x03104;

    [NativeTypeName("#define pcom_tom_sleep 0x03105")]
    public const int pcom_tom_sleep = 0x03105;

    [NativeTypeName("#define pcom_tom_dark 0x03106")]
    public const int pcom_tom_dark = 0x03106;

    [NativeTypeName("#define pcom_tom_silence 0x03107")]
    public const int pcom_tom_silence = 0x03107;

    [NativeTypeName("#define pcom_tom_poison 0x03108")]
    public const int pcom_tom_poison = 0x03108;

    [NativeTypeName("#define pcom_tom_berserk 0x03109")]
    public const int pcom_tom_berserk = 0x03109;

    [NativeTypeName("#define pcom_tom_stop 0x0310a")]
    public const int pcom_tom_stop = 0x0310a;

    [NativeTypeName("#define pcom_tom_stone 0x0310b")]
    public const int pcom_tom_stone = 0x0310b;

    [NativeTypeName("#define pcom_tom_death 0x0310c")]
    public const int pcom_tom_death = 0x0310c;

    [NativeTypeName("#define pcom_tom_full_break 0x0310d")]
    public const int pcom_tom_full_break = 0x0310d;

    [NativeTypeName("#define pcom_tom_last_attack 0x0310e")]
    public const int pcom_tom_last_attack = 0x0310e;

    [NativeTypeName("#define pcom_sp_yuna_fire 0x0310f")]
    public const int pcom_sp_yuna_fire = 0x0310f;

    [NativeTypeName("#define pcom_sp_yuna_cold 0x03110")]
    public const int pcom_sp_yuna_cold = 0x03110;

    [NativeTypeName("#define pcom_sp_yuna_thunder 0x03111")]
    public const int pcom_sp_yuna_thunder = 0x03111;

    [NativeTypeName("#define pcom_sp_yuna_water 0x03112")]
    public const int pcom_sp_yuna_water = 0x03112;

    [NativeTypeName("#define pcom_sp_yuna_guard_phy 0x03113")]
    public const int pcom_sp_yuna_guard_phy = 0x03113;

    [NativeTypeName("#define pcom_sp_yuna_guard_mag 0x03114")]
    public const int pcom_sp_yuna_guard_mag = 0x03114;

    [NativeTypeName("#define pcom_sp_yuna_libra 0x03115")]
    public const int pcom_sp_yuna_libra = 0x03115;

    [NativeTypeName("#define pcom_sp_yuna_flare 0x03116")]
    public const int pcom_sp_yuna_flare = 0x03116;

    [NativeTypeName("#define pcom_sp_yuna_life 0x03117")]
    public const int pcom_sp_yuna_life = 0x03117;

    [NativeTypeName("#define pcom_sp_yuna_last_attack 0x03118")]
    public const int pcom_sp_yuna_last_attack = 0x03118;

    [NativeTypeName("#define pcom_sp_yuna_rcure 0x03119")]
    public const int pcom_sp_yuna_rcure = 0x03119;

    [NativeTypeName("#define pcom_sp_yuna_r_regen 0x0311a")]
    public const int pcom_sp_yuna_r_regen = 0x0311a;

    [NativeTypeName("#define pcom_sp_yuna_r_shell 0x0311b")]
    public const int pcom_sp_yuna_r_shell = 0x0311b;

    [NativeTypeName("#define pcom_sp_yuna_r_protess 0x0311c")]
    public const int pcom_sp_yuna_r_protess = 0x0311c;

    [NativeTypeName("#define pcom_sp_yuna_r_reflect 0x0311d")]
    public const int pcom_sp_yuna_r_reflect = 0x0311d;

    [NativeTypeName("#define pcom_sp_yuna_r_heist 0x0311e")]
    public const int pcom_sp_yuna_r_heist = 0x0311e;

    [NativeTypeName("#define pcom_sp_yuna_r_str_down 0x0311f")]
    public const int pcom_sp_yuna_r_str_down = 0x0311f;

    [NativeTypeName("#define pcom_sp_yuna_r_vit_down 0x03120")]
    public const int pcom_sp_yuna_r_vit_down = 0x03120;

    [NativeTypeName("#define pcom_sp_yuna_r_mag_down 0x03121")]
    public const int pcom_sp_yuna_r_mag_down = 0x03121;

    [NativeTypeName("#define pcom_sp_yuna_r_spr_down 0x03122")]
    public const int pcom_sp_yuna_r_spr_down = 0x03122;

    [NativeTypeName("#define pcom_sp_yuna_r_attack 0x03123")]
    public const int pcom_sp_yuna_r_attack = 0x03123;

    [NativeTypeName("#define pcom_sp_yuna_l_sleep 0x03124")]
    public const int pcom_sp_yuna_l_sleep = 0x03124;

    [NativeTypeName("#define pcom_sp_yuna_l_berserk 0x03125")]
    public const int pcom_sp_yuna_l_berserk = 0x03125;

    [NativeTypeName("#define pcom_sp_yuna_l_slow 0x03126")]
    public const int pcom_sp_yuna_l_slow = 0x03126;

    [NativeTypeName("#define pcom_sp_yuna_l_demi 0x03127")]
    public const int pcom_sp_yuna_l_demi = 0x03127;

    [NativeTypeName("#define pcom_sp_yuna_l_bio 0x03128")]
    public const int pcom_sp_yuna_l_bio = 0x03128;

    [NativeTypeName("#define pcom_sp_yuna_l_death 0x03129")]
    public const int pcom_sp_yuna_l_death = 0x03129;

    [NativeTypeName("#define pcom_sp_yuna_l_stone 0x0312a")]
    public const int pcom_sp_yuna_l_stone = 0x0312a;

    [NativeTypeName("#define pcom_sp_yuna_l_confu 0x0312b")]
    public const int pcom_sp_yuna_l_confu = 0x0312b;

    [NativeTypeName("#define pcom_sp_yuna_l_stop 0x0312c")]
    public const int pcom_sp_yuna_l_stop = 0x0312c;

    [NativeTypeName("#define pcom_sp_yuna_l_dark 0x0312d")]
    public const int pcom_sp_yuna_l_dark = 0x0312d;

    [NativeTypeName("#define pcom_sp_yuna_l_attack 0x0312e")]
    public const int pcom_sp_yuna_l_attack = 0x0312e;

    [NativeTypeName("#define pcom_attack_rikku_sp 0x0312f")]
    public const int pcom_attack_rikku_sp = 0x0312f;

    [NativeTypeName("#define pcom_sp_rikku_death 0x03130")]
    public const int pcom_sp_rikku_death = 0x03130;

    [NativeTypeName("#define pcom_sp_rikku_poison 0x03131")]
    public const int pcom_sp_rikku_poison = 0x03131;

    [NativeTypeName("#define pcom_sp_rikku_stone 0x03132")]
    public const int pcom_sp_rikku_stone = 0x03132;

    [NativeTypeName("#define pcom_sp_rikku_berserk 0x03133")]
    public const int pcom_sp_rikku_berserk = 0x03133;

    [NativeTypeName("#define pcom_sp_rikku_stop 0x03134")]
    public const int pcom_sp_rikku_stop = 0x03134;

    [NativeTypeName("#define pcom_sp_rikku_confu 0x03135")]
    public const int pcom_sp_rikku_confu = 0x03135;

    [NativeTypeName("#define pcom_sp_rikku_shock 0x03136")]
    public const int pcom_sp_rikku_shock = 0x03136;

    [NativeTypeName("#define pcom_sp_rikku_shock2 0x03137")]
    public const int pcom_sp_rikku_shock2 = 0x03137;

    [NativeTypeName("#define pcom_sp_rikku_life 0x03138")]
    public const int pcom_sp_rikku_life = 0x03138;

    [NativeTypeName("#define pcom_sp_rikku_last_attack 0x03139")]
    public const int pcom_sp_rikku_last_attack = 0x03139;

    [NativeTypeName("#define pcom_sp_rikku_r_attack 0x0313a")]
    public const int pcom_sp_rikku_r_attack = 0x0313a;

    [NativeTypeName("#define pcom_sp_rikku_r_cure 0x0313b")]
    public const int pcom_sp_rikku_r_cure = 0x0313b;

    [NativeTypeName("#define pcom_sp_rikku_r_mp_up 0x0313c")]
    public const int pcom_sp_rikku_r_mp_up = 0x0313c;

    [NativeTypeName("#define pcom_sp_rikku_r_grenade 0x0313d")]
    public const int pcom_sp_rikku_r_grenade = 0x0313d;

    [NativeTypeName("#define pcom_sp_rikku_r_sleep 0x0313e")]
    public const int pcom_sp_rikku_r_sleep = 0x0313e;

    [NativeTypeName("#define pcom_sp_rikku_r_slow 0x0313f")]
    public const int pcom_sp_rikku_r_slow = 0x0313f;

    [NativeTypeName("#define pcom_sp_rikku_r_str_down 0x03140")]
    public const int pcom_sp_rikku_r_str_down = 0x03140;

    [NativeTypeName("#define pcom_sp_rikku_r_vit_down 0x03141")]
    public const int pcom_sp_rikku_r_vit_down = 0x03141;

    [NativeTypeName("#define pcom_sp_rikku_r_libra 0x03142")]
    public const int pcom_sp_rikku_r_libra = 0x03142;

    [NativeTypeName("#define pcom_sp_rikku_r_shell 0x03143")]
    public const int pcom_sp_rikku_r_shell = 0x03143;

    [NativeTypeName("#define pcom_sp_rikku_r_protess 0x03144")]
    public const int pcom_sp_rikku_r_protess = 0x03144;

    [NativeTypeName("#define pcom_sp_rikku_l_attack 0x03145")]
    public const int pcom_sp_rikku_l_attack = 0x03145;

    [NativeTypeName("#define pcom_sp_rikku_l_cure 0x03146")]
    public const int pcom_sp_rikku_l_cure = 0x03146;

    [NativeTypeName("#define pcom_sp_rikku_l_mp_up 0x03147")]
    public const int pcom_sp_rikku_l_mp_up = 0x03147;

    [NativeTypeName("#define pcom_sp_rikku_l_grenade 0x03148")]
    public const int pcom_sp_rikku_l_grenade = 0x03148;

    [NativeTypeName("#define pcom_sp_rikku_l_dark 0x03149")]
    public const int pcom_sp_rikku_l_dark = 0x03149;

    [NativeTypeName("#define pcom_sp_rikku_l_silence 0x0314a")]
    public const int pcom_sp_rikku_l_silence = 0x0314a;

    [NativeTypeName("#define pcom_sp_rikku_l_mag_down 0x0314b")]
    public const int pcom_sp_rikku_l_mag_down = 0x0314b;

    [NativeTypeName("#define pcom_sp_rikku_l_spr_down 0x0314c")]
    public const int pcom_sp_rikku_l_spr_down = 0x0314c;

    [NativeTypeName("#define pcom_sp_rikku_l_haste 0x0314d")]
    public const int pcom_sp_rikku_l_haste = 0x0314d;

    [NativeTypeName("#define pcom_sp_rikku_l_str_up 0x0314e")]
    public const int pcom_sp_rikku_l_str_up = 0x0314e;

    [NativeTypeName("#define pcom_sp_rikku_l_vit_up 0x0314f")]
    public const int pcom_sp_rikku_l_vit_up = 0x0314f;

    [NativeTypeName("#define pcom_sp_pain_fire 0x03150")]
    public const int pcom_sp_pain_fire = 0x03150;

    [NativeTypeName("#define pcom_sp_pain_cold 0x03151")]
    public const int pcom_sp_pain_cold = 0x03151;

    [NativeTypeName("#define pcom_sp_pain_thunder 0x03152")]
    public const int pcom_sp_pain_thunder = 0x03152;

    [NativeTypeName("#define pcom_sp_pain_water 0x03153")]
    public const int pcom_sp_pain_water = 0x03153;

    [NativeTypeName("#define pcom_sp_pain_ratio 0x03154")]
    public const int pcom_sp_pain_ratio = 0x03154;

    [NativeTypeName("#define pcom_sp_pain_critical 0x03155")]
    public const int pcom_sp_pain_critical = 0x03155;

    [NativeTypeName("#define pcom_sp_pain_death 0x03156")]
    public const int pcom_sp_pain_death = 0x03156;

    [NativeTypeName("#define pcom_sp_pain_holy 0x03157")]
    public const int pcom_sp_pain_holy = 0x03157;

    [NativeTypeName("#define pcom_sp_pain_kick 0x03158")]
    public const int pcom_sp_pain_kick = 0x03158;

    [NativeTypeName("#define pcom_sp_pain_last_attack 0x03159")]
    public const int pcom_sp_pain_last_attack = 0x03159;

    [NativeTypeName("#define pcom_sp_pain_r_poison 0x0315a")]
    public const int pcom_sp_pain_r_poison = 0x0315a;

    [NativeTypeName("#define pcom_sp_pain_r_dark 0x0315b")]
    public const int pcom_sp_pain_r_dark = 0x0315b;

    [NativeTypeName("#define pcom_sp_pain_r_silence 0x0315c")]
    public const int pcom_sp_pain_r_silence = 0x0315c;

    [NativeTypeName("#define pcom_sp_pain_r_stone 0x0315d")]
    public const int pcom_sp_pain_r_stone = 0x0315d;

    [NativeTypeName("#define pcom_sp_pain_r_sleep 0x0315e")]
    public const int pcom_sp_pain_r_sleep = 0x0315e;

    [NativeTypeName("#define pcom_sp_pain_r_berserk 0x0315f")]
    public const int pcom_sp_pain_r_berserk = 0x0315f;

    [NativeTypeName("#define pcom_sp_pain_r_stop 0x03160")]
    public const int pcom_sp_pain_r_stop = 0x03160;

    [NativeTypeName("#define pcom_sp_pain_r_confu 0x03161")]
    public const int pcom_sp_pain_r_confu = 0x03161;

    [NativeTypeName("#define pcom_sp_pain_r_cure 0x03162")]
    public const int pcom_sp_pain_r_cure = 0x03162;

    [NativeTypeName("#define pcom_sp_pain_r_mp_up 0x03163")]
    public const int pcom_sp_pain_r_mp_up = 0x03163;

    [NativeTypeName("#define pcom_sp_pain_r_life 0x03164")]
    public const int pcom_sp_pain_r_life = 0x03164;

    [NativeTypeName("#define pcom_sp_pain_l_str_up 0x03165")]
    public const int pcom_sp_pain_l_str_up = 0x03165;

    [NativeTypeName("#define pcom_sp_pain_l_vit_up 0x03166")]
    public const int pcom_sp_pain_l_vit_up = 0x03166;

    [NativeTypeName("#define pcom_sp_pain_l_heist 0x03167")]
    public const int pcom_sp_pain_l_heist = 0x03167;

    [NativeTypeName("#define pcom_sp_pain_l_libra 0x03168")]
    public const int pcom_sp_pain_l_libra = 0x03168;

    [NativeTypeName("#define pcom_sp_pain_l_str_down 0x03169")]
    public const int pcom_sp_pain_l_str_down = 0x03169;

    [NativeTypeName("#define pcom_sp_pain_l_vit_dpwn 0x0316a")]
    public const int pcom_sp_pain_l_vit_dpwn = 0x0316a;

    [NativeTypeName("#define pcom_sp_pain_l_mag_down 0x0316b")]
    public const int pcom_sp_pain_l_mag_down = 0x0316b;

    [NativeTypeName("#define pcom_sp_pain_l_spr_down 0x0316c")]
    public const int pcom_sp_pain_l_spr_down = 0x0316c;

    [NativeTypeName("#define pcom_sp_pain_l_cure 0x0316d")]
    public const int pcom_sp_pain_l_cure = 0x0316d;

    [NativeTypeName("#define pcom_sp_pain_l_mp_up 0x0316e")]
    public const int pcom_sp_pain_l_mp_up = 0x0316e;

    [NativeTypeName("#define pcom_sp_pain_l_life 0x0316f")]
    public const int pcom_sp_pain_l_life = 0x0316f;

    [NativeTypeName("#define pcom_flare 0x03170")]
    public const int pcom_flare = 0x03170;

    [NativeTypeName("#define pcom_ultima 0x03171")]
    public const int pcom_ultima = 0x03171;

    [NativeTypeName("#define pcom_holy 0x03172")]
    public const int pcom_holy = 0x03172;

    [NativeTypeName("#define pcom_re_life 0x03173")]
    public const int pcom_re_life = 0x03173;

    [NativeTypeName("#define pcom_steal_attack 0x03174")]
    public const int pcom_steal_attack = 0x03174;

    [NativeTypeName("#define pcom_steal_gill_attack 0x03175")]
    public const int pcom_steal_gill_attack = 0x03175;

    [NativeTypeName("#define pcom_heist 0x03176")]
    public const int pcom_heist = 0x03176;

    [NativeTypeName("#define pcom_heist2 0x03177")]
    public const int pcom_heist2 = 0x03177;

    [NativeTypeName("#define pcom_brain 0x03178")]
    public const int pcom_brain = 0x03178;

    [NativeTypeName("#define pcom_silence 0x03179")]
    public const int pcom_silence = 0x03179;

    [NativeTypeName("#define pcom_sleep 0x0317a")]
    public const int pcom_sleep = 0x0317a;

    [NativeTypeName("#define pcom_aspirx 0x0317b")]
    public const int pcom_aspirx = 0x0317b;

    [NativeTypeName("#define pcom_dragon_sword2 0x0317c")]
    public const int pcom_dragon_sword2 = 0x0317c;

    [NativeTypeName("#define pcom_2dice_2 0x0317d")]
    public const int pcom_2dice_2 = 0x0317d;

    [NativeTypeName("#define pcom_2dice_3 0x0317e")]
    public const int pcom_2dice_3 = 0x0317e;

    [NativeTypeName("#define pcom_4dice_2 0x0317f")]
    public const int pcom_4dice_2 = 0x0317f;

    [NativeTypeName("#define pcom_4dice_3 0x03180")]
    public const int pcom_4dice_3 = 0x03180;

    [NativeTypeName("#define pcom_reserve05 0x03181")]
    public const int pcom_reserve05 = 0x03181;

    [NativeTypeName("#define pcom_last_attack 0x03182")]
    public const int pcom_last_attack = 0x03182;

    [NativeTypeName("#define pcom_cmp_grenade_Lv2 0x03183")]
    public const int pcom_cmp_grenade_Lv2 = 0x03183;

    [NativeTypeName("#define pcom_cmp_grenade_Lv3 0x03184")]
    public const int pcom_cmp_grenade_Lv3 = 0x03184;

    [NativeTypeName("#define pcom_cmp_grenade_Lv4 0x03185")]
    public const int pcom_cmp_grenade_Lv4 = 0x03185;

    [NativeTypeName("#define pcom_cmp_final_attack 0x03186")]
    public const int pcom_cmp_final_attack = 0x03186;

    [NativeTypeName("#define pcom_cmp_st_grenade_Lv1 0x03187")]
    public const int pcom_cmp_st_grenade_Lv1 = 0x03187;

    [NativeTypeName("#define pcom_cmp_st_grenade_Lv2 0x03188")]
    public const int pcom_cmp_st_grenade_Lv2 = 0x03188;

    [NativeTypeName("#define pcom_cmp_fire_Lv1 0x03189")]
    public const int pcom_cmp_fire_Lv1 = 0x03189;

    [NativeTypeName("#define pcom_cmp_fire_Lv2 0x0318a")]
    public const int pcom_cmp_fire_Lv2 = 0x0318a;

    [NativeTypeName("#define pcom_cmp_fire_Lv3 0x0318b")]
    public const int pcom_cmp_fire_Lv3 = 0x0318b;

    [NativeTypeName("#define pcom_cmp_st_fire_Lv1 0x0318c")]
    public const int pcom_cmp_st_fire_Lv1 = 0x0318c;

    [NativeTypeName("#define pcom_cmp_st_fire_Lv2 0x0318d")]
    public const int pcom_cmp_st_fire_Lv2 = 0x0318d;

    [NativeTypeName("#define pcom_cmp_cold_Lv1 0x0318e")]
    public const int pcom_cmp_cold_Lv1 = 0x0318e;

    [NativeTypeName("#define pcom_cmp_cold_Lv2 0x0318f")]
    public const int pcom_cmp_cold_Lv2 = 0x0318f;

    [NativeTypeName("#define pcom_cmp_cold_Lv3 0x03190")]
    public const int pcom_cmp_cold_Lv3 = 0x03190;

    [NativeTypeName("#define pcom_cmp_st_cold_Lv1 0x03191")]
    public const int pcom_cmp_st_cold_Lv1 = 0x03191;

    [NativeTypeName("#define pcom_cmp_st_cold_Lv2 0x03192")]
    public const int pcom_cmp_st_cold_Lv2 = 0x03192;

    [NativeTypeName("#define pcom_cmp_thunder_Lv1 0x03193")]
    public const int pcom_cmp_thunder_Lv1 = 0x03193;

    [NativeTypeName("#define pcom_cmp_thunder_Lv2 0x03194")]
    public const int pcom_cmp_thunder_Lv2 = 0x03194;

    [NativeTypeName("#define pcom_cmp_thunder_Lv3 0x03195")]
    public const int pcom_cmp_thunder_Lv3 = 0x03195;

    [NativeTypeName("#define pcom_cmp_st_thunder_Lv1 0x03196")]
    public const int pcom_cmp_st_thunder_Lv1 = 0x03196;

    [NativeTypeName("#define pcom_cmp_st_thunder_Lv2 0x03197")]
    public const int pcom_cmp_st_thunder_Lv2 = 0x03197;

    [NativeTypeName("#define pcom_cmp_water_Lv1 0x03198")]
    public const int pcom_cmp_water_Lv1 = 0x03198;

    [NativeTypeName("#define pcom_cmp_water_Lv2 0x03199")]
    public const int pcom_cmp_water_Lv2 = 0x03199;

    [NativeTypeName("#define pcom_cmp_water_Lv3 0x0319a")]
    public const int pcom_cmp_water_Lv3 = 0x0319a;

    [NativeTypeName("#define pcom_cmp_st_water_Lv1 0x0319b")]
    public const int pcom_cmp_st_water_Lv1 = 0x0319b;

    [NativeTypeName("#define pcom_cmp_st_water_Lv2 0x0319c")]
    public const int pcom_cmp_st_water_Lv2 = 0x0319c;

    [NativeTypeName("#define pcom_cmp_gravity_Lv1 0x0319d")]
    public const int pcom_cmp_gravity_Lv1 = 0x0319d;

    [NativeTypeName("#define pcom_cmp_gravity_Lv2 0x0319e")]
    public const int pcom_cmp_gravity_Lv2 = 0x0319e;

    [NativeTypeName("#define pcom_cmp_st_gravity_Lv1 0x0319f")]
    public const int pcom_cmp_st_gravity_Lv1 = 0x0319f;

    [NativeTypeName("#define pcom_cmp_st_gravity_Lv2 0x031a0")]
    public const int pcom_cmp_st_gravity_Lv2 = 0x031a0;

    [NativeTypeName("#define pcom_cmp_holy_Lv1 0x031a1")]
    public const int pcom_cmp_holy_Lv1 = 0x031a1;

    [NativeTypeName("#define pcom_cmp_holy_Lv2 0x031a2")]
    public const int pcom_cmp_holy_Lv2 = 0x031a2;

    [NativeTypeName("#define pcom_cmp_panacea 0x031a3")]
    public const int pcom_cmp_panacea = 0x031a3;

    [NativeTypeName("#define pcom_cmp_panacea_all 0x031a4")]
    public const int pcom_cmp_panacea_all = 0x031a4;

    [NativeTypeName("#define pcom_cmp_panacea_full 0x031a5")]
    public const int pcom_cmp_panacea_full = 0x031a5;

    [NativeTypeName("#define pcom_cmp_megaphenix 0x031a6")]
    public const int pcom_cmp_megaphenix = 0x031a6;

    [NativeTypeName("#define pcom_cmp_megaphenix_kai 0x031a7")]
    public const int pcom_cmp_megaphenix_kai = 0x031a7;

    [NativeTypeName("#define pcom_cmp_lastphenix 0x031a8")]
    public const int pcom_cmp_lastphenix = 0x031a8;

    [NativeTypeName("#define pcom_cmp_hipotion 0x031a9")]
    public const int pcom_cmp_hipotion = 0x031a9;

    [NativeTypeName("#define pcom_cmp_megapotion 0x031aa")]
    public const int pcom_cmp_megapotion = 0x031aa;

    [NativeTypeName("#define pcom_cmp_expotion_full 0x031ab")]
    public const int pcom_cmp_expotion_full = 0x031ab;

    [NativeTypeName("#define pcom_cmp_megaelixir 0x031ac")]
    public const int pcom_cmp_megaelixir = 0x031ac;

    [NativeTypeName("#define pcom_cmp_reflect 0x031ad")]
    public const int pcom_cmp_reflect = 0x031ad;

    [NativeTypeName("#define pcom_cmp_haste_all 0x031ae")]
    public const int pcom_cmp_haste_all = 0x031ae;

    [NativeTypeName("#define pcom_cmp_ryuken 0x031af")]
    public const int pcom_cmp_ryuken = 0x031af;

    [NativeTypeName("#define pcom_cmp_ryuken2 0x031b0")]
    public const int pcom_cmp_ryuken2 = 0x031b0;

    [NativeTypeName("#define pcom_cmp_wall 0x031b1")]
    public const int pcom_cmp_wall = 0x031b1;

    [NativeTypeName("#define pcom_cmp_mighty_guard 0x031b2")]
    public const int pcom_cmp_mighty_guard = 0x031b2;

    [NativeTypeName("#define pcom_cmp_mighty_guard2 0x031b3")]
    public const int pcom_cmp_mighty_guard2 = 0x031b3;

    [NativeTypeName("#define pcom_cmp_hp_double 0x031b4")]
    public const int pcom_cmp_hp_double = 0x031b4;

    [NativeTypeName("#define pcom_cmp_hpmp_double 0x031b5")]
    public const int pcom_cmp_hpmp_double = 0x031b5;

    [NativeTypeName("#define pcom_cmp_mp_double 0x031b6")]
    public const int pcom_cmp_mp_double = 0x031b6;

    [NativeTypeName("#define pcom_cmp_hero 0x031b7")]
    public const int pcom_cmp_hero = 0x031b7;

    [NativeTypeName("#define pcom_cmp_holywar 0x031b8")]
    public const int pcom_cmp_holywar = 0x031b8;

    [NativeTypeName("#define pcom_slot_atc_777 0x031b9")]
    public const int pcom_slot_atc_777 = 0x031b9;

    [NativeTypeName("#define pcom_slot_atc_BAR 0x031ba")]
    public const int pcom_slot_atc_BAR = 0x031ba;

    [NativeTypeName("#define pcom_slot_atc_cherry3 0x031bb")]
    public const int pcom_slot_atc_cherry3 = 0x031bb;

    [NativeTypeName("#define pcom_slot_atc_soldier3 0x031bc")]
    public const int pcom_slot_atc_soldier3 = 0x031bc;

    [NativeTypeName("#define pcom_slot_atc_berserker3 0x031bd")]
    public const int pcom_slot_atc_berserker3 = 0x031bd;

    [NativeTypeName("#define pcom_slot_atc_samurai3 0x031be")]
    public const int pcom_slot_atc_samurai3 = 0x031be;

    [NativeTypeName("#define pcom_slot_atc_cherry2 0x031bf")]
    public const int pcom_slot_atc_cherry2 = 0x031bf;

    [NativeTypeName("#define pcom_slot_atc_soldier2 0x031c0")]
    public const int pcom_slot_atc_soldier2 = 0x031c0;

    [NativeTypeName("#define pcom_slot_atc_berserker2 0x031c1")]
    public const int pcom_slot_atc_berserker2 = 0x031c1;

    [NativeTypeName("#define pcom_slot_atc_samurai2 0x031c2")]
    public const int pcom_slot_atc_samurai2 = 0x031c2;

    [NativeTypeName("#define pcom_slot_atc_cherry1 0x031c3")]
    public const int pcom_slot_atc_cherry1 = 0x031c3;

    [NativeTypeName("#define pcom_slot_mag_777 0x031c4")]
    public const int pcom_slot_mag_777 = 0x031c4;

    [NativeTypeName("#define pcom_slot_mag_BAR 0x031c5")]
    public const int pcom_slot_mag_BAR = 0x031c5;

    [NativeTypeName("#define pcom_slot_mag_cherry3 0x031c6")]
    public const int pcom_slot_mag_cherry3 = 0x031c6;

    [NativeTypeName("#define pcom_slot_mag_black3 0x031c7")]
    public const int pcom_slot_mag_black3 = 0x031c7;

    [NativeTypeName("#define pcom_slot_mag_white3 0x031c8")]
    public const int pcom_slot_mag_white3 = 0x031c8;

    [NativeTypeName("#define pcom_slot_mag_dark3 0x031c9")]
    public const int pcom_slot_mag_dark3 = 0x031c9;

    [NativeTypeName("#define pcom_slot_mag_cherry2 0x031ca")]
    public const int pcom_slot_mag_cherry2 = 0x031ca;

    [NativeTypeName("#define pcom_slot_mag_black2 0x031cb")]
    public const int pcom_slot_mag_black2 = 0x031cb;

    [NativeTypeName("#define pcom_slot_mag_white2 0x031cc")]
    public const int pcom_slot_mag_white2 = 0x031cc;

    [NativeTypeName("#define pcom_slot_mag_dark2 0x031cd")]
    public const int pcom_slot_mag_dark2 = 0x031cd;

    [NativeTypeName("#define pcom_slot_mag_cherry1 0x031ce")]
    public const int pcom_slot_mag_cherry1 = 0x031ce;

    [NativeTypeName("#define pcom_slot_item_777 0x031cf")]
    public const int pcom_slot_item_777 = 0x031cf;

    [NativeTypeName("#define pcom_slot_item_BAR 0x031d0")]
    public const int pcom_slot_item_BAR = 0x031d0;

    [NativeTypeName("#define pcom_slot_item_cherry3 0x031d1")]
    public const int pcom_slot_item_cherry3 = 0x031d1;

    [NativeTypeName("#define pcom_slot_item_attack3 0x031d2")]
    public const int pcom_slot_item_attack3 = 0x031d2;

    [NativeTypeName("#define pcom_slot_item_recover3 0x031d3")]
    public const int pcom_slot_item_recover3 = 0x031d3;

    [NativeTypeName("#define pcom_slot_item_support3 0x031d4")]
    public const int pcom_slot_item_support3 = 0x031d4;

    [NativeTypeName("#define pcom_slot_item_cherry2 0x031d5")]
    public const int pcom_slot_item_cherry2 = 0x031d5;

    [NativeTypeName("#define pcom_slot_item_attack2 0x031d6")]
    public const int pcom_slot_item_attack2 = 0x031d6;

    [NativeTypeName("#define pcom_slot_item_recover2 0x031d7")]
    public const int pcom_slot_item_recover2 = 0x031d7;

    [NativeTypeName("#define pcom_slot_item_support2 0x031d8")]
    public const int pcom_slot_item_support2 = 0x031d8;

    [NativeTypeName("#define pcom_slot_item_cherry1 0x031d9")]
    public const int pcom_slot_item_cherry1 = 0x031d9;

    [NativeTypeName("#define pcom_slot_sp_777 0x031da")]
    public const int pcom_slot_sp_777 = 0x031da;

    [NativeTypeName("#define pcom_slot_sp_BAR 0x031db")]
    public const int pcom_slot_sp_BAR = 0x031db;

    [NativeTypeName("#define pcom_slot_sp_cherry3 0x031dc")]
    public const int pcom_slot_sp_cherry3 = 0x031dc;

    [NativeTypeName("#define pcom_slot_sp_yuna3 0x031dd")]
    public const int pcom_slot_sp_yuna3 = 0x031dd;

    [NativeTypeName("#define pcom_slot_sp_rikku3 0x031de")]
    public const int pcom_slot_sp_rikku3 = 0x031de;

    [NativeTypeName("#define pcom_slot_sp_pain3 0x031df")]
    public const int pcom_slot_sp_pain3 = 0x031df;

    [NativeTypeName("#define pcom_slot_sp_cherry2 0x031e0")]
    public const int pcom_slot_sp_cherry2 = 0x031e0;

    [NativeTypeName("#define pcom_slot_sp_yuna2 0x031e1")]
    public const int pcom_slot_sp_yuna2 = 0x031e1;

    [NativeTypeName("#define pcom_slot_sp_rikku2 0x031e2")]
    public const int pcom_slot_sp_rikku2 = 0x031e2;

    [NativeTypeName("#define pcom_slot_sp_pain2 0x031e3")]
    public const int pcom_slot_sp_pain2 = 0x031e3;

    [NativeTypeName("#define pcom_slot_sp_cherry1 0x031e4")]
    public const int pcom_slot_sp_cherry1 = 0x031e4;

    [NativeTypeName("#define pcom_slot_miss 0x031e5")]
    public const int pcom_slot_miss = 0x031e5;

    [NativeTypeName("#define pcom_reserve06 0x031e6")]
    public const int pcom_reserve06 = 0x031e6;
}
