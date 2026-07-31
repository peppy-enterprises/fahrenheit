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
    TXT         = 2,
    RATE        = 3,
    A_ABILITY   = 4,
    AMAPDATA    = 5,
    ARMS_SHOP   = 6,
    BTL_TXT     = 7,
    BUKI_GET    = 8,
    C_ABILITY   = 9,
    COMMAND     = 10,
    CTB_BASE    = 11,
    IMPORTANT   = 12,
    ITEM        = 13,
    ITEM_GET    = 14,
    ITEM_SHOP   = 15,
    KAIZOU      = 16,
    MENU        = 17,
    MENU_PANEL  = 18,
    MONMAGIC    = 19,
    MONMAGIC1   = 20,
    MONSTER     = 21,
    PANEL       = 22,
    PARTY       = 23,
    PLY_ROM     = 24,
    PLY_SAVE    = 25,
    PREPARE     = 26,
    SHOP_ARMS   = 27,
    SPHERE      = 28,
    ST_NUMBER   = 29,
    SUM_ASSURE  = 30,
    SUM_GROW    = 31,
    TAKARA      = 32,
    W_NAME      = 33,
    WEAPON      = 34,

    COUNT_TYPES = 35,
}

internal static partial class UI {

    /// <summary>
    ///     Obtains a descriptive display string for the <see cref="EEditComponent"/>
    ///     that implements editing the selected <paramref name="mode"/>.
    /// </summary>
    private static string _get_component_name_by_mode(EEditMode mode) {
        return mode switch {
            EEditMode.TXT        => $"{nameof(NameHelpText)} - (*_txt.bin)",
            EEditMode.RATE       => $"{nameof(Rate)} - (*_rate.bin)",
            EEditMode.A_ABILITY  => $"{nameof(AutoAbility)} - (a_ability.bin)",
            EEditMode.AMAPDATA   => $"(amapdata.bin)",
            EEditMode.ARMS_SHOP  => $"(arms_shop.bin)",
            EEditMode.BTL_TXT    => $"{nameof(HelpText)} - (btl_txt.bin)",
            EEditMode.BUKI_GET   => $"{nameof(UnownedEquipment)} - (buki_get.bin)",
            EEditMode.C_ABILITY  => $"(c_ability.bin)",
            EEditMode.COMMAND    => $"{nameof(PCommand)} - (command.bin)",
            EEditMode.CTB_BASE   => $"(ctb_base.bin)",
            EEditMode.IMPORTANT  => $"{nameof(KeyItem)} - (important.bin)",
            EEditMode.ITEM       => $"{nameof(PCommand)} - (item.bin)",
            EEditMode.ITEM_GET   => $"(item_get.bin)",
            EEditMode.ITEM_SHOP  => $"(item_shop.bin)",
            EEditMode.KAIZOU     => $"{nameof(CustomizationRecipe)} - (kaizou.bin)",
            EEditMode.MENU       => $"(menu.bin)",
            EEditMode.MENU_PANEL => $"(menu_panel.bin)",
            EEditMode.MONMAGIC   => $"(monmagic.bin)",
            EEditMode.MONMAGIC1  => $"{nameof(Command)} - (monmagic*.bin)",
            EEditMode.MONSTER    => $"(monster*.bin)",
            EEditMode.PANEL      => $"{nameof(SphereGridNodeType)} - (panel.bin)",
            EEditMode.PARTY      => $"(party.bin)",
            EEditMode.PLY_ROM    => $"{nameof(PlyRom)} - (ply_rom.bin)",
            EEditMode.PLY_SAVE   => $"{nameof(PlySave)} - (ply_save.bin)",
            EEditMode.PREPARE    => $"{nameof(MixRecipe)} - (prepare.bin)",
            EEditMode.SHOP_ARMS  => $"(shop_arms.bin)",
            EEditMode.SPHERE     => $"{nameof(Sphere)} - (sphere.bin)",
            EEditMode.ST_NUMBER  => $"{nameof(StNumber)} - (st_number.bin)",
            EEditMode.SUM_ASSURE => $"{nameof(AeonStatBoostsMinimum)} - (sum_assure.bin)",
            EEditMode.SUM_GROW   => $"{nameof(AeonAbilityRecipe)} - (sum_grow.bin)",
            EEditMode.TAKARA     => $"{nameof(Treasure)} - (takara.bin)",
            EEditMode.W_NAME     => $"(w_name.bin)",
            EEditMode.WEAPON     => $"{nameof(Equipment)} - (weapon.bin)",
            EEditMode.NULL       => $"(Unknown Type)",
            _                    => throw new NotImplementedException("UNREACHABLE")
        };
    }

    /// <summary>
    ///     Obtains the <see cref="EEditComponent"/> that implements editing the selected <paramref name="mode"/>.
    /// </summary>
    private static EEditComponent _get_component_by_mode(EEditMode mode) {
        return mode switch {
            EEditMode.TXT        => new EditorTextPair(),
            EEditMode.BTL_TXT    => new EditorText(),
            EEditMode.RATE       => new EditorRate(),

            EEditMode.A_ABILITY  => throw new NotImplementedException(),
            EEditMode.AMAPDATA   => throw new NotImplementedException(),
            EEditMode.ARMS_SHOP  => throw new NotImplementedException(),
            EEditMode.BUKI_GET   => throw new NotImplementedException(),
            EEditMode.C_ABILITY  => throw new NotImplementedException(),
            EEditMode.COMMAND    => throw new NotImplementedException(),
            EEditMode.CTB_BASE   => throw new NotImplementedException(),
            EEditMode.IMPORTANT  => throw new NotImplementedException(),
            EEditMode.ITEM       => throw new NotImplementedException(),
            EEditMode.ITEM_GET   => throw new NotImplementedException(),
            EEditMode.ITEM_SHOP  => throw new NotImplementedException(),
            EEditMode.KAIZOU     => throw new NotImplementedException(),
            EEditMode.MENU       => throw new NotImplementedException(),
            EEditMode.MENU_PANEL => throw new NotImplementedException(),
            EEditMode.MONMAGIC   => throw new NotImplementedException(),
            EEditMode.MONMAGIC1  => throw new NotImplementedException(),
            EEditMode.MONSTER    => throw new NotImplementedException(),
            EEditMode.PANEL      => throw new NotImplementedException(),
            EEditMode.PARTY      => throw new NotImplementedException(),
            EEditMode.PLY_ROM    => new EditorPlyRom(),
            EEditMode.PLY_SAVE   => throw new NotImplementedException(),
            EEditMode.PREPARE    => throw new NotImplementedException(),
            EEditMode.SHOP_ARMS  => throw new NotImplementedException(),
            EEditMode.SPHERE     => throw new NotImplementedException(),
            EEditMode.ST_NUMBER  => throw new NotImplementedException(),
            EEditMode.SUM_ASSURE => throw new NotImplementedException(),
            EEditMode.SUM_GROW   => throw new NotImplementedException(),
            EEditMode.TAKARA     => throw new NotImplementedException(),
            EEditMode.W_NAME     => throw new NotImplementedException(),
            EEditMode.WEAPON     => throw new NotImplementedException(),
            EEditMode.NULL       or
            _                    => new EditorNull()
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
            "arms_txt.bin"   or
            "btlend_txt.bin" or
            "build_txt.bin"  or
            "config_txt.bin" or
            "item_txt.bin"   or
            "menu_txt.bin"   or
            "menu_txt2.bin"  or
            "mmain_txt.bin"  or
            "name_txt.bin"   or
            "save_txt.bin"   or
            "status_txt.bin" or
            "summon_txt.bin" => new EditorTextPair(),
            "btl_txt.bin"    => new EditorText(),
            "arms_rate.bin"  or
            "item_rate.bin"  => new EditorRate(),

            "a_ability.bin"  => null,
            "amapdata.bin"   => null,
            "arms_shop.bin"  => null,
            "buki_get.bin"   => null,
            "c_ability.bin"  => null,
            "command.bin"    => null,
            "ctb_base.bin"   => null,
            "help_txt.bin"   => null,
            "important.bin"  => null,
            "item_get.bin"   => null,
            "item_shop.bin"  => null,
            "item.bin"       => null,
            "kaizou.bin"     => null,
            "menu_panel.bin" => null,
            "menu.bin"       => null,
            "monmagic.bin"   => null,
            "monmagic1.bin"  or
            "monmagic2.bin"  => null,
            "monster1.bin"   or
            "monster2.bin"   or
            "monster3.bin"   => null,
            "panel.bin"      => null,
            "party.bin"      => null,
            "ply_rom.bin"    => new EditorPlyRom(),
            "ply_save.bin"   => null,
            "prepare.bin"    => null,
            "shop_arms.bin"  => null,
            "sphere.bin"     => null,
            "st_number.bin"  => null,
            "sum_assure.bin" => null,
            "sum_grow.bin"   => null,
            "takara.bin"     => null,
            "w_name.bin"     => null,
            "weapon.bin"     => null,
            _                => null,
        };
    }
}
