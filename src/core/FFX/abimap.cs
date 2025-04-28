using System.Runtime.CompilerServices;

namespace Fahrenheit.Core.FFX;

[InlineArray(3)]
public struct AbilityMap {
    private uint _u;

    public bool has_attack          { readonly get { return this[0].get_bit( 0); } set { this[0].set_bit( 0, value); } }
    public bool has_item            { readonly get { return this[0].get_bit( 1); } set { this[0].set_bit( 1, value); } }
    public bool has_switch          { readonly get { return this[0].get_bit( 2); } set { this[0].set_bit( 2, value); } }
    public bool has_escape          { readonly get { return this[0].get_bit( 3); } set { this[0].set_bit( 3, value); } }
    public bool has_weapon_change   { readonly get { return this[0].get_bit( 4); } set { this[0].set_bit( 4, value); } }
    public bool has_armor_change    { readonly get { return this[0].get_bit( 5); } set { this[0].set_bit( 5, value); } }
    public bool has_delay_attack    { readonly get { return this[0].get_bit( 6); } set { this[0].set_bit( 6, value); } }
    public bool has_delay_buster    { readonly get { return this[0].get_bit( 7); } set { this[0].set_bit( 7, value); } }
    public bool has_sleep_attack    { readonly get { return this[0].get_bit( 8); } set { this[0].set_bit( 8, value); } }
    public bool has_silence_attack  { readonly get { return this[0].get_bit( 9); } set { this[0].set_bit( 9, value); } }
    public bool has_dark_attack     { readonly get { return this[0].get_bit(10); } set { this[0].set_bit(10, value); } }
    public bool has_zombie_attack   { readonly get { return this[0].get_bit(11); } set { this[0].set_bit(11, value); } }
    public bool has_sleep_buster    { readonly get { return this[0].get_bit(12); } set { this[0].set_bit(12, value); } }
    public bool has_silence_buster  { readonly get { return this[0].get_bit(13); } set { this[0].set_bit(13, value); } }
    public bool has_dark_buster     { readonly get { return this[0].get_bit(14); } set { this[0].set_bit(14, value); } }
    public bool has_triple_foul     { readonly get { return this[0].get_bit(15); } set { this[0].set_bit(15, value); } }

    public bool has_power_break     { readonly get { return this[0].get_bit(16); } set { this[0].set_bit(16, value); } }
    public bool has_magic_break     { readonly get { return this[0].get_bit(17); } set { this[0].set_bit(17, value); } }
    public bool has_armor_break     { readonly get { return this[0].get_bit(18); } set { this[0].set_bit(18, value); } }
    public bool has_mental_break    { readonly get { return this[0].get_bit(19); } set { this[0].set_bit(19, value); } }
    public bool has_mug             { readonly get { return this[0].get_bit(20); } set { this[0].set_bit(20, value); } }
    public bool has_quick_hit       { readonly get { return this[0].get_bit(21); } set { this[0].set_bit(21, value); } }
    public bool has_steal           { readonly get { return this[0].get_bit(22); } set { this[0].set_bit(22, value); } }
    public bool has_use             { readonly get { return this[0].get_bit(23); } set { this[0].set_bit(23, value); } }
    public bool has_flee            { readonly get { return this[0].get_bit(24); } set { this[0].set_bit(24, value); } }
    public bool has_pray            { readonly get { return this[0].get_bit(25); } set { this[0].set_bit(25, value); } }
    public bool has_cheer           { readonly get { return this[0].get_bit(26); } set { this[0].set_bit(26, value); } }
    public bool has_aim             { readonly get { return this[0].get_bit(27); } set { this[0].set_bit(27, value); } }
    public bool has_focus           { readonly get { return this[0].get_bit(28); } set { this[0].set_bit(28, value); } }
    public bool has_reflex          { readonly get { return this[0].get_bit(29); } set { this[0].set_bit(29, value); } }
    public bool has_luck            { readonly get { return this[0].get_bit(30); } set { this[0].set_bit(30, value); } }
    public bool has_jinx            { readonly get { return this[0].get_bit(31); } set { this[0].set_bit(31, value); } }

    public bool has_lancet          { readonly get { return this[1].get_bit( 0); } set { this[1].set_bit( 0, value); } }
    public bool has_unused          { readonly get { return this[1].get_bit( 1); } set { this[1].set_bit( 1, value); } }
    public bool has_guard           { readonly get { return this[1].get_bit( 2); } set { this[1].set_bit( 2, value); } }
    public bool has_sentinel        { readonly get { return this[1].get_bit( 3); } set { this[1].set_bit( 3, value); } }
    public bool has_spare_change    { readonly get { return this[1].get_bit( 4); } set { this[1].set_bit( 4, value); } }
    public bool has_threaten        { readonly get { return this[1].get_bit( 5); } set { this[1].set_bit( 5, value); } }
    public bool has_provoke         { readonly get { return this[1].get_bit( 6); } set { this[1].set_bit( 6, value); } }
    public bool has_entrust         { readonly get { return this[1].get_bit( 7); } set { this[1].set_bit( 7, value); } }
    public bool has_copycat         { readonly get { return this[1].get_bit( 8); } set { this[1].set_bit( 8, value); } }
    public bool has_doublecast      { readonly get { return this[1].get_bit( 9); } set { this[1].set_bit( 9, value); } }
    public bool has_bribe           { readonly get { return this[1].get_bit(10); } set { this[1].set_bit(10, value); } }
    public bool has_cure            { readonly get { return this[1].get_bit(11); } set { this[1].set_bit(11, value); } }
    public bool has_cura            { readonly get { return this[1].get_bit(12); } set { this[1].set_bit(12, value); } }
    public bool has_curaga          { readonly get { return this[1].get_bit(13); } set { this[1].set_bit(13, value); } }
    public bool has_nul_frost       { readonly get { return this[1].get_bit(14); } set { this[1].set_bit(14, value); } }
    public bool has_nul_blaze       { readonly get { return this[1].get_bit(15); } set { this[1].set_bit(15, value); } }

    public bool has_nul_shock       { readonly get { return this[1].get_bit(16); } set { this[1].set_bit(16, value); } }
    public bool has_nul_tide        { readonly get { return this[1].get_bit(17); } set { this[1].set_bit(17, value); } }
    public bool has_scan            { readonly get { return this[1].get_bit(18); } set { this[1].set_bit(18, value); } }
    public bool has_esuna           { readonly get { return this[1].get_bit(19); } set { this[1].set_bit(19, value); } }
    public bool has_life            { readonly get { return this[1].get_bit(20); } set { this[1].set_bit(20, value); } }
    public bool has_full_life       { readonly get { return this[1].get_bit(21); } set { this[1].set_bit(21, value); } }
    public bool has_haste           { readonly get { return this[1].get_bit(22); } set { this[1].set_bit(22, value); } }
    public bool has_hastega         { readonly get { return this[1].get_bit(23); } set { this[1].set_bit(23, value); } }
    public bool has_slow            { readonly get { return this[1].get_bit(24); } set { this[1].set_bit(24, value); } }
    public bool has_slowga          { readonly get { return this[1].get_bit(25); } set { this[1].set_bit(25, value); } }
    public bool has_shell           { readonly get { return this[1].get_bit(26); } set { this[1].set_bit(26, value); } }
    public bool has_protect         { readonly get { return this[1].get_bit(27); } set { this[1].set_bit(27, value); } }
    public bool has_reflect         { readonly get { return this[1].get_bit(28); } set { this[1].set_bit(28, value); } }
    public bool has_dispel          { readonly get { return this[1].get_bit(29); } set { this[1].set_bit(29, value); } }
    public bool has_regen           { readonly get { return this[1].get_bit(30); } set { this[1].set_bit(30, value); } }
    public bool has_holy            { readonly get { return this[1].get_bit(31); } set { this[1].set_bit(31, value); } }

    public bool has_auto_life       { readonly get { return this[2].get_bit( 0); } set { this[2].set_bit( 0, value); } }
    public bool has_blizzard        { readonly get { return this[2].get_bit( 1); } set { this[2].set_bit( 1, value); } }
    public bool has_fire            { readonly get { return this[2].get_bit( 2); } set { this[2].set_bit( 2, value); } }
    public bool has_thunder         { readonly get { return this[2].get_bit( 3); } set { this[2].set_bit( 3, value); } }
    public bool has_water           { readonly get { return this[2].get_bit( 4); } set { this[2].set_bit( 4, value); } }
    public bool has_fira            { readonly get { return this[2].get_bit( 5); } set { this[2].set_bit( 5, value); } }
    public bool has_blizzara        { readonly get { return this[2].get_bit( 6); } set { this[2].set_bit( 6, value); } }
    public bool has_thundara        { readonly get { return this[2].get_bit( 7); } set { this[2].set_bit( 7, value); } }
    public bool has_watera          { readonly get { return this[2].get_bit( 8); } set { this[2].set_bit( 8, value); } }
    public bool has_firaga          { readonly get { return this[2].get_bit( 9); } set { this[2].set_bit( 9, value); } }
    public bool has_blizzaga        { readonly get { return this[2].get_bit(10); } set { this[2].set_bit(10, value); } }
    public bool has_thundaga        { readonly get { return this[2].get_bit(11); } set { this[2].set_bit(11, value); } }
    public bool has_waterga         { readonly get { return this[2].get_bit(12); } set { this[2].set_bit(12, value); } }
    public bool has_bio             { readonly get { return this[2].get_bit(13); } set { this[2].set_bit(13, value); } }
    public bool has_demi            { readonly get { return this[2].get_bit(14); } set { this[2].set_bit(14, value); } }
    public bool has_death           { readonly get { return this[2].get_bit(15); } set { this[2].set_bit(15, value); } }

    public bool has_drain           { readonly get { return this[2].get_bit(16); } set { this[2].set_bit(16, value); } }
    public bool has_osmose          { readonly get { return this[2].get_bit(17); } set { this[2].set_bit(17, value); } }
    public bool has_flare           { readonly get { return this[2].get_bit(18); } set { this[2].set_bit(18, value); } }
    public bool has_ultima          { readonly get { return this[2].get_bit(19); } set { this[2].set_bit(19, value); } }
    public bool has_shield          { readonly get { return this[2].get_bit(20); } set { this[2].set_bit(20, value); } }
    public bool has_boost           { readonly get { return this[2].get_bit(21); } set { this[2].set_bit(21, value); } }
    public bool has_dismiss         { readonly get { return this[2].get_bit(22); } set { this[2].set_bit(22, value); } }
    public bool has_dismiss_yojimbo { readonly get { return this[2].get_bit(23); } set { this[2].set_bit(23, value); } }
    public bool has_pilfer_gil      { readonly get { return this[2].get_bit(24); } set { this[2].set_bit(24, value); } }
    public bool has_full_break      { readonly get { return this[2].get_bit(25); } set { this[2].set_bit(25, value); } }
    public bool has_extract_power   { readonly get { return this[2].get_bit(26); } set { this[2].set_bit(26, value); } }
    public bool has_extract_mana    { readonly get { return this[2].get_bit(27); } set { this[2].set_bit(27, value); } }
    public bool has_extract_speed   { readonly get { return this[2].get_bit(28); } set { this[2].set_bit(28, value); } }
    public bool has_extract_ability { readonly get { return this[2].get_bit(29); } set { this[2].set_bit(29, value); } }
    public bool has_nab_gil         { readonly get { return this[2].get_bit(30); } set { this[2].set_bit(30, value); } }
    public bool has_quick_pockets   { readonly get { return this[2].get_bit(31); } set { this[2].set_bit(31, value); } }
}