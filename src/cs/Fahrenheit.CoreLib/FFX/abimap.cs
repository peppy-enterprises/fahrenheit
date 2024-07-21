namespace Fahrenheit.CoreLib.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0xC)]
public struct AbiMap {
    [FieldOffset(0x0)] public uint part1;
    [FieldOffset(0x4)] public uint part2;
    [FieldOffset(0x8)] public uint part3;

    public bool has_attack			{ get => part1.get_bit(0); set => part1.set_bit(0, value); }
    public bool has_item			{ get => part1.get_bit(1); set => part1.set_bit(1, value); }
    public bool has_switch			{ get => part1.get_bit(2); set => part1.set_bit(2, value); }
    public bool has_escape			{ get => part1.get_bit(3); set => part1.set_bit(3, value); }
    public bool has_weapon_change	{ get => part1.get_bit(4); set => part1.set_bit(4, value); }
    public bool has_armor_change	{ get => part1.get_bit(5); set => part1.set_bit(5, value); }
    public bool has_delay_attack	{ get => part1.get_bit(6); set => part1.set_bit(6, value); }
    public bool has_delay_buster	{ get => part1.get_bit(7); set => part1.set_bit(7, value); }
    public bool has_sleep_attack	{ get => part1.get_bit(8); set => part1.set_bit(8, value); }
    public bool has_silence_attack	{ get => part1.get_bit(9); set => part1.set_bit(9, value); }
    public bool has_dark_attack		{ get => part1.get_bit(10); set => part1.set_bit(10, value); }
    public bool has_zombie_attack	{ get => part1.get_bit(11); set => part1.set_bit(11, value); }
    public bool has_sleep_buster	{ get => part1.get_bit(12); set => part1.set_bit(12, value); }
    public bool has_silence_buster	{ get => part1.get_bit(13); set => part1.set_bit(13, value); }
    public bool has_dark_buster		{ get => part1.get_bit(14); set => part1.set_bit(14, value); }
    public bool has_triple_foul		{ get => part1.get_bit(15); set => part1.set_bit(15, value); }

    public bool has_power_break		{ get => part1.get_bit(16); set => part1.set_bit(16, value); }
    public bool has_magic_break		{ get => part1.get_bit(17); set => part1.set_bit(17, value); }
    public bool has_armor_break		{ get => part1.get_bit(18); set => part1.set_bit(18, value); }
    public bool has_mental_break	{ get => part1.get_bit(19); set => part1.set_bit(19, value); }
    public bool has_mug				{ get => part1.get_bit(20); set => part1.set_bit(20, value); }
    public bool has_quick_hit		{ get => part1.get_bit(21); set => part1.set_bit(21, value); }
    public bool has_steal			{ get => part1.get_bit(22); set => part1.set_bit(22, value); }
    public bool has_use				{ get => part1.get_bit(23); set => part1.set_bit(23, value); }
    public bool has_flee			{ get => part1.get_bit(24); set => part1.set_bit(24, value); }
    public bool has_pray			{ get => part1.get_bit(25); set => part1.set_bit(25, value); }
    public bool has_cheer			{ get => part1.get_bit(26); set => part1.set_bit(26, value); }
    public bool has_aim				{ get => part1.get_bit(27); set => part1.set_bit(27, value); }
    public bool has_focus			{ get => part1.get_bit(28); set => part1.set_bit(28, value); }
    public bool has_reflex			{ get => part1.get_bit(29); set => part1.set_bit(29, value); }
    public bool has_luck			{ get => part1.get_bit(30); set => part1.set_bit(30, value); }
    public bool has_jinx			{ get => part1.get_bit(31); set => part1.set_bit(31, value); }

    public bool has_lancet			{ get => part2.get_bit(0); set => part2.set_bit(0, value); }
    public bool has_unused			{ get => part2.get_bit(1); set => part2.set_bit(1, value); }
    public bool has_guard			{ get => part2.get_bit(2); set => part2.set_bit(2, value); }
    public bool has_sentinel		{ get => part2.get_bit(3); set => part2.set_bit(3, value); }
    public bool has_spare_change	{ get => part2.get_bit(4); set => part2.set_bit(4, value); }
    public bool has_threaten		{ get => part2.get_bit(5); set => part2.set_bit(5, value); }
    public bool has_provoke			{ get => part2.get_bit(6); set => part2.set_bit(6, value); }
    public bool has_entrust			{ get => part2.get_bit(7); set => part2.set_bit(7, value); }
    public bool has_copycat			{ get => part2.get_bit(8); set => part2.set_bit(8, value); }
    public bool has_doublecast		{ get => part2.get_bit(9); set => part2.set_bit(9, value); }
    public bool has_bribe			{ get => part2.get_bit(10); set => part2.set_bit(10, value); }
    public bool has_cure			{ get => part2.get_bit(11); set => part2.set_bit(11, value); }
    public bool has_cura			{ get => part2.get_bit(12); set => part2.set_bit(12, value); }
    public bool has_curaga			{ get => part2.get_bit(13); set => part2.set_bit(13, value); }
    public bool has_nul_ice			{ get => part2.get_bit(14); set => part2.set_bit(14, value); }
    public bool has_nul_fire		{ get => part2.get_bit(15); set => part2.set_bit(15, value); }

    public bool has_nul_thunder		{ get => part2.get_bit(16); set => part2.set_bit(16, value); }
    public bool has_nul_water		{ get => part2.get_bit(17); set => part2.set_bit(17, value); }
    public bool has_scan			{ get => part2.get_bit(18); set => part2.set_bit(18, value); }
    public bool has_esuna			{ get => part2.get_bit(19); set => part2.set_bit(19, value); }
    public bool has_life			{ get => part2.get_bit(20); set => part2.set_bit(20, value); }
    public bool has_full_life		{ get => part2.get_bit(21); set => part2.set_bit(21, value); }
    public bool has_haste			{ get => part2.get_bit(22); set => part2.set_bit(22, value); }
    public bool has_hastega			{ get => part2.get_bit(23); set => part2.set_bit(23, value); }
    public bool has_slow			{ get => part2.get_bit(24); set => part2.set_bit(24, value); }
    public bool has_slowga			{ get => part2.get_bit(25); set => part2.set_bit(25, value); }
    public bool has_shell			{ get => part2.get_bit(26); set => part2.set_bit(26, value); }
    public bool has_protect			{ get => part2.get_bit(27); set => part2.set_bit(27, value); }
    public bool has_reflect			{ get => part2.get_bit(28); set => part2.set_bit(28, value); }
    public bool has_dispel			{ get => part2.get_bit(29); set => part2.set_bit(29, value); }
    public bool has_regen			{ get => part2.get_bit(30); set => part2.set_bit(30, value); }
    public bool has_holy			{ get => part2.get_bit(31); set => part2.set_bit(31, value); }

    public bool has_auto_life		{ get => part3.get_bit(0); set => part3.set_bit(0, value); }
    public bool has_blizzard		{ get => part3.get_bit(1); set => part3.set_bit(1, value); }
    public bool has_fire			{ get => part3.get_bit(2); set => part3.set_bit(2, value); }
    public bool has_thunder			{ get => part3.get_bit(3); set => part3.set_bit(3, value); }
    public bool has_water			{ get => part3.get_bit(4); set => part3.set_bit(4, value); }
    public bool has_fira			{ get => part3.get_bit(5); set => part3.set_bit(5, value); }
    public bool has_blizzara		{ get => part3.get_bit(6); set => part3.set_bit(6, value); }
    public bool has_thundara		{ get => part3.get_bit(7); set => part3.set_bit(7, value); }
    public bool has_watera			{ get => part3.get_bit(8); set => part3.set_bit(8, value); }
    public bool has_firaga			{ get => part3.get_bit(9); set => part3.set_bit(9, value); }
    public bool has_blizzaga		{ get => part3.get_bit(10); set => part3.set_bit(10, value); }
    public bool has_thundaga		{ get => part3.get_bit(11); set => part3.set_bit(11, value); }
    public bool has_waterga			{ get => part3.get_bit(12); set => part3.set_bit(12, value); }
    public bool has_bio				{ get => part3.get_bit(13); set => part3.set_bit(13, value); }
    public bool has_demi			{ get => part3.get_bit(14); set => part3.set_bit(14, value); }
    public bool has_death			{ get => part3.get_bit(15); set => part3.set_bit(15, value); }

    public bool has_drain			{ get => part3.get_bit(16); set => part3.set_bit(16, value); }
    public bool has_osmose			{ get => part3.get_bit(17); set => part3.set_bit(17, value); }
    public bool has_flare			{ get => part3.get_bit(18); set => part3.set_bit(18, value); }
    public bool has_ultima			{ get => part3.get_bit(19); set => part3.set_bit(19, value); }
    public bool has_shield			{ get => part3.get_bit(20); set => part3.set_bit(20, value); }
    public bool has_boost			{ get => part3.get_bit(21); set => part3.set_bit(21, value); }
    public bool has_dismiss			{ get => part3.get_bit(22); set => part3.set_bit(22, value); }
    public bool has_dismiss_yojimbo	{ get => part3.get_bit(23); set => part3.set_bit(23, value); }
    public bool has_pilfer_gil		{ get => part3.get_bit(24); set => part3.set_bit(24, value); }
    public bool has_full_break		{ get => part3.get_bit(25); set => part3.set_bit(25, value); }
    public bool has_extract_power	{ get => part3.get_bit(26); set => part3.set_bit(26, value); }
    public bool has_extract_mana	{ get => part3.get_bit(27); set => part3.set_bit(27, value); }
    public bool has_extract_speed	{ get => part3.get_bit(28); set => part3.set_bit(28, value); }
    public bool has_extract_ability	{ get => part3.get_bit(29); set => part3.set_bit(29, value); }
    public bool has_nab_gil			{ get => part3.get_bit(30); set => part3.set_bit(30, value); }
    public bool has_quick_pockets	{ get => part3.get_bit(31); set => part3.set_bit(31, value); }
}
