// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit.Tools.EEdit;

/// <summary>
///     The well-known Excel file types in enum form, used for internal ordering.
/// </summary>
internal enum EEditMode {
    NULL        = 1,
    A_ABILITY   = 2,
    AMAPDATA    = 3,
    ARMS_RATE   = 4,
    ARMS_SHOP   = 5,
    ARMS_TXT    = 6,
    BTL_TXT     = 7,
    BTLEND_TXT  = 8,
    BUILD_TXT   = 9,
    BUKI_GET    = 10,
    C_ABILITY   = 11,
    COMMAND     = 12,
    CONFIG_TXT  = 13,
    CTB_BASE    = 14,
    HELP_TXT    = 15,
    IMPORTANT   = 16,
    ITEM        = 17,
    ITEM_GET    = 18,
    ITEM_RATE   = 19,
    ITEM_SHOP   = 20,
    ITEM_TXT    = 21,
    KAIZOU      = 22,
    MENU        = 23,
    MENU_PANEL  = 24,
    MENU_TXT    = 25,
    MENU_TXT2   = 26,
    MMAIN_TXT   = 27,
    MONMAGIC    = 28,
    MONMAGIC1   = 29,
    MONSTER     = 30,
    NAME_TXT    = 31,
    PANEL       = 32,
    PARTY       = 33,
    PLY_ROM     = 34,
    PLY_SAVE    = 35,
    PREPARE     = 36,
    SAVE_TXT    = 37,
    SHOP_ARMS   = 38,
    SPHERE      = 39,
    ST_NUMBER   = 40,
    STATUS_TXT  = 41,
    SUM_ASSURE  = 42,
    SUM_GROW    = 43,
    SUMMON_TXT  = 44,
    TAKARA      = 45,
    W_NAME      = 46,
    WEAPON      = 47,
    COUNT_TYPES = 48
}

internal static partial class UI {

    /// <summary>
    ///     Obtains a descriptive display string for the <see cref="EEditComponent"/>
    ///     that implements editing the selected <paramref name="mode"/>.
    /// </summary>
    private static string _get_component_name_by_mode(EEditMode mode) {
        return mode switch {
            EEditMode.A_ABILITY  => $"{nameof(AutoAbility)} - (a_ability.bin)",
            EEditMode.AMAPDATA   => $"(amapdata.bin)",
            EEditMode.ARMS_RATE  => $"(arms_rate.bin)",
            EEditMode.ARMS_SHOP  => $"(arms_shop.bin)",
            EEditMode.ARMS_TXT   => $"(arms_txt.bin)",
            EEditMode.BTL_TXT    => $"(btl_txt.bin)",
            EEditMode.BTLEND_TXT => $"(btlend_txt.bin)",
            EEditMode.BUILD_TXT  => $"(build_txt.bin)",
            EEditMode.BUKI_GET   => $"(buki_get.bin)",
            EEditMode.C_ABILITY  => $"(c_ability.bin)",
            EEditMode.COMMAND    => $"{nameof(PCommand)} - (command.bin)",
            EEditMode.CONFIG_TXT => $"(config_txt.bin)",
            EEditMode.CTB_BASE   => $"(ctb_base.bin)",
            EEditMode.HELP_TXT   => $"(help_txt.bin)",
            EEditMode.IMPORTANT  => $"(important.bin)",
            EEditMode.ITEM       => $"{nameof(PCommand)} - (item.bin)",
            EEditMode.ITEM_GET   => $"(item_get.bin)",
            EEditMode.ITEM_RATE  => $"(item_rate.bin)",
            EEditMode.ITEM_SHOP  => $"(item_shop.bin)",
            EEditMode.ITEM_TXT   => $"(item_txt.bin)",
            EEditMode.KAIZOU     => $"{nameof(CustomizationRecipe)} - (kaizou.bin)",
            EEditMode.MENU       => $"(menu.bin)",
            EEditMode.MENU_PANEL => $"(menu_panel.bin)",
            EEditMode.MENU_TXT   => $"(menu_txt.bin)",
            EEditMode.MENU_TXT2  => $"(menu_txt2.bin)",
            EEditMode.MMAIN_TXT  => $"(mmain_txt.bin)",
            EEditMode.MONMAGIC   => $"(monmagic.bin)",
            EEditMode.MONMAGIC1  => $"{nameof(Command)} - (monmagic*.bin)",
            EEditMode.MONSTER    => $"(monster*.bin)",
            EEditMode.NAME_TXT   => $"(name_txt.bin)",
            EEditMode.PANEL      => $"(panel.bin)",
            EEditMode.PARTY      => $"(party.bin)",
            EEditMode.PLY_ROM    => $"{nameof(PlyRom)} - (ply_rom.bin)",
            EEditMode.PLY_SAVE   => $"{nameof(PlySave)} - (ply_save.bin)",
            EEditMode.PREPARE    => $"(prepare.bin)",
            EEditMode.SAVE_TXT   => $"(save_txt.bin)",
            EEditMode.SHOP_ARMS  => $"(shop_arms.bin)",
            EEditMode.SPHERE     => $"(sphere.bin)",
            EEditMode.ST_NUMBER  => $"(st_number.bin)",
            EEditMode.STATUS_TXT => $"(status_txt.bin)",
            EEditMode.SUM_ASSURE => $"{nameof(AeonStatBoostsMinimum)} - (sum_assure.bin)",
            EEditMode.SUM_GROW   => $"{nameof(AeonAbilityRecipe)} - (sum_grow.bin)",
            EEditMode.SUMMON_TXT => $"(summon_txt.bin)",
            EEditMode.TAKARA     => $"(takara.bin)",
            EEditMode.W_NAME     => $"(w_name.bin)",
            EEditMode.WEAPON     => $"(weapon.bin)",
            EEditMode.NULL       => $"(Unknown Type)",
            _                    => throw new NotImplementedException("UNREACHABLE")
        };
    }

    /// <summary>
    ///     Obtains the <see cref="EEditComponent"/> that implements editing the selected <paramref name="mode"/>.
    /// </summary>
    private static EEditComponent _get_component_by_mode(EEditMode mode) {
        return mode switch {
            EEditMode.A_ABILITY  => throw new NotImplementedException(),
            EEditMode.AMAPDATA   => throw new NotImplementedException(),
            EEditMode.ARMS_RATE  => throw new NotImplementedException(),
            EEditMode.ARMS_SHOP  => throw new NotImplementedException(),
            EEditMode.ARMS_TXT   => throw new NotImplementedException(),
            EEditMode.BTL_TXT    => throw new NotImplementedException(),
            EEditMode.BTLEND_TXT => throw new NotImplementedException(),
            EEditMode.BUILD_TXT  => throw new NotImplementedException(),
            EEditMode.BUKI_GET   => throw new NotImplementedException(),
            EEditMode.C_ABILITY  => throw new NotImplementedException(),
            EEditMode.COMMAND    => throw new NotImplementedException(),
            EEditMode.CONFIG_TXT => throw new NotImplementedException(),
            EEditMode.CTB_BASE   => throw new NotImplementedException(),
            EEditMode.HELP_TXT   => throw new NotImplementedException(),
            EEditMode.IMPORTANT  => throw new NotImplementedException(),
            EEditMode.ITEM       => throw new NotImplementedException(),
            EEditMode.ITEM_GET   => throw new NotImplementedException(),
            EEditMode.ITEM_RATE  => throw new NotImplementedException(),
            EEditMode.ITEM_SHOP  => throw new NotImplementedException(),
            EEditMode.ITEM_TXT   => throw new NotImplementedException(),
            EEditMode.KAIZOU     => throw new NotImplementedException(),
            EEditMode.MENU       => throw new NotImplementedException(),
            EEditMode.MENU_PANEL => throw new NotImplementedException(),
            EEditMode.MENU_TXT   => throw new NotImplementedException(),
            EEditMode.MENU_TXT2  => throw new NotImplementedException(),
            EEditMode.MMAIN_TXT  => throw new NotImplementedException(),
            EEditMode.MONMAGIC   => throw new NotImplementedException(),
            EEditMode.MONMAGIC1  => throw new NotImplementedException(),
            EEditMode.MONSTER    => throw new NotImplementedException(),
            EEditMode.NAME_TXT   => throw new NotImplementedException(),
            EEditMode.PANEL      => throw new NotImplementedException(),
            EEditMode.PARTY      => throw new NotImplementedException(),
            EEditMode.PLY_ROM    => throw new NotImplementedException(),
            EEditMode.PLY_SAVE   => throw new NotImplementedException(),
            EEditMode.PREPARE    => throw new NotImplementedException(),
            EEditMode.SAVE_TXT   => throw new NotImplementedException(),
            EEditMode.SHOP_ARMS  => throw new NotImplementedException(),
            EEditMode.SPHERE     => throw new NotImplementedException(),
            EEditMode.ST_NUMBER  => throw new NotImplementedException(),
            EEditMode.STATUS_TXT => throw new NotImplementedException(),
            EEditMode.SUM_ASSURE => throw new NotImplementedException(),
            EEditMode.SUM_GROW   => throw new NotImplementedException(),
            EEditMode.SUMMON_TXT => throw new NotImplementedException(),
            EEditMode.TAKARA     => throw new NotImplementedException(),
            EEditMode.W_NAME     => throw new NotImplementedException(),
            EEditMode.WEAPON     => throw new NotImplementedException(),
            EEditMode.NULL       or
            _                    => throw new NotImplementedException("UNREACHABLE")
        };
    }

    /// <summary>
    ///     If the name of the file at <paramref name="file_path"/> is well-known,
    ///     returns the correct <see cref="EEditComponent"/> to edit it.
    /// </summary>
    private static EEditComponent? _get_component_for_file(string file_path) {

        /* [fkelava 15/02/26 17:28]
         * PS {...}\ffx_ps2\ffx\master\jppc\battle\kernel> gci | Select Name
         */

        return Path.GetFileName(file_path) switch {
            "a_ability.bin"  => null,
            "amapdata.bin"   => null,
            "arms_rate.bin"  => null,
            "arms_shop.bin"  => null,
            "arms_txt.bin"   => null,
            "btl_txt.bin"    => null,
            "btlend_txt.bin" => null,
            "build_txt.bin"  => null,
            "buki_get.bin"   => null,
            "c_ability.bin"  => null,
            "command.bin"    => null,
            "config_txt.bin" => null,
            "ctb_base.bin"   => null,
            "help_txt.bin"   => null,
            "important.bin"  => null,
            "item_get.bin"   => null,
            "item_rate.bin"  => null,
            "item_shop.bin"  => null,
            "item_txt.bin"   => null,
            "item.bin"       => null,
            "kaizou.bin"     => null,
            "menu_panel.bin" => null,
            "menu_txt.bin"   => null,
            "menu_txt2.bin"  => null,
            "menu.bin"       => null,
            "mmain_txt.bin"  => null,
            "monmagic.bin"   => null,
            "monmagic1.bin"  or
            "monmagic2.bin"  => null,
            "monster1.bin"   or
            "monster2.bin"   or
            "monster3.bin"   => null,
            "name_txt.bin"   => null,
            "panel.bin"      => null,
            "party.bin"      => null,
            "ply_rom.bin"    => null,
            "ply_save.bin"   => null,
            "prepare.bin"    => null,
            "save_txt.bin"   => null,
            "shop_arms.bin"  => null,
            "sphere.bin"     => null,
            "st_number.bin"  => null,
            "status_txt.bin" => null,
            "sum_assure.bin" => null,
            "sum_grow.bin"   => null,
            "summon_txt.bin" => null,
            "takara.bin"     => null,
            "w_name.bin"     => null,
            "weapon.bin"     => null,
            _                => null,
        };
    }
}
