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
            EEditMode.W_NAME     => $"{nameof(WeaponName)} - (w_name.bin)",
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

            EEditMode.A_ABILITY  => new EditorAAbility(),
            EEditMode.AMAPDATA   => throw new NotImplementedException(),
            EEditMode.ARMS_SHOP  => throw new NotImplementedException(),
            EEditMode.BUKI_GET   => new EditorBukiGet(),
            EEditMode.C_ABILITY  => throw new NotImplementedException(),
            EEditMode.COMMAND    => new EditorCommand(),
            EEditMode.CTB_BASE   => throw new NotImplementedException(),
            EEditMode.IMPORTANT  => new EditorImportant(),
            EEditMode.ITEM       => new EditorItem(),
            EEditMode.ITEM_GET   => throw new NotImplementedException(),
            EEditMode.ITEM_SHOP  => throw new NotImplementedException(),
            EEditMode.KAIZOU     => new EditorKaizou(),
            EEditMode.MENU       => throw new NotImplementedException(),
            EEditMode.MENU_PANEL => throw new NotImplementedException(),
            EEditMode.MONMAGIC   => throw new NotImplementedException(),
            EEditMode.MONMAGIC1  => new EditorMonmagic1(),
            EEditMode.MONSTER    => throw new NotImplementedException(),
            EEditMode.PANEL      => new EditorPanel(),
            EEditMode.PARTY      => throw new NotImplementedException(),
            EEditMode.PLY_ROM    => new EditorPlyRom(),
            EEditMode.PLY_SAVE   => new EditorPlySave(),
            EEditMode.PREPARE    => new EditorPrepare(),
            EEditMode.SHOP_ARMS  => throw new NotImplementedException(),
            EEditMode.SPHERE     => new EditorSphere(),
            EEditMode.ST_NUMBER  => new EditorStNumber(),
            EEditMode.SUM_ASSURE => new EditorSumAssure(),
            EEditMode.SUM_GROW   => new EditorSumGrow(),
            EEditMode.TAKARA     => new EditorTakara(),
            EEditMode.W_NAME     => new EditorWeaponName(),
            EEditMode.WEAPON     => new EditorWeapon(),
            EEditMode.NULL       or
            _                    => new EditorNull()
        };
    }

    /// <summary>
    ///     If the name of the file at <paramref name="file_path"/> is well-known,
    ///     returns the correct <see cref="EEditComponent"/> to edit it.
    /// </summary>
    private static EEditMode _get_mode_by_file_name(string file_path) {

        /* [fkelava 15/02/26 17:28]
         * PS {...}\ffx_ps2\ffx\master\jppc\battle\kernel> gci | Select Name
         */

        return Path.GetFileName(file_path) switch {
            "arms_txt.bin"   or
            "btlend_txt.bin" or
            "build_txt.bin"  or
            "config_txt.bin" or
            "help_txt.bin"   or
            "item_txt.bin"   or
            "menu_txt.bin"   or
            "menu_txt2.bin"  or
            "mmain_txt.bin"  or
            "name_txt.bin"   or
            "save_txt.bin"   or
            "status_txt.bin" or
            "summon_txt.bin" => EEditMode.TXT,
            "btl_txt.bin"    => EEditMode.BTL_TXT,
            "arms_rate.bin"  or
            "item_rate.bin"  => EEditMode.RATE,

            "a_ability.bin"  => EEditMode.A_ABILITY,
            "amapdata.bin"   => EEditMode.AMAPDATA,
            "arms_shop.bin"  => EEditMode.ARMS_SHOP,
            "buki_get.bin"   => EEditMode.BUKI_GET,
            "c_ability.bin"  => EEditMode.C_ABILITY,
            "command.bin"    => EEditMode.COMMAND,
            "ctb_base.bin"   => EEditMode.CTB_BASE,
            "important.bin"  => EEditMode.IMPORTANT,
            "item_get.bin"   => EEditMode.ITEM_GET,
            "item_shop.bin"  => EEditMode.ITEM_SHOP,
            "item.bin"       => EEditMode.ITEM,
            "kaizou.bin"     => EEditMode.KAIZOU,
            "menu_panel.bin" => EEditMode.MENU_PANEL,
            "menu.bin"       => EEditMode.MENU,
            "monmagic.bin"   => EEditMode.MONMAGIC,
            "monmagic1.bin"  or
            "monmagic2.bin"  => EEditMode.MONMAGIC1,
            "monster1.bin"   or
            "monster2.bin"   or
            "monster3.bin"   => EEditMode.MONSTER,
            "panel.bin"      => EEditMode.PANEL,
            "party.bin"      => EEditMode.PARTY,
            "ply_rom.bin"    => EEditMode.PLY_ROM,
            "ply_save.bin"   => EEditMode.PLY_SAVE,
            "prepare.bin"    => EEditMode.PREPARE,
            "shop_arms.bin"  => EEditMode.SHOP_ARMS,
            "sphere.bin"     => EEditMode.SPHERE,
            "st_number.bin"  => EEditMode.ST_NUMBER,
            "sum_assure.bin" => EEditMode.SUM_ASSURE,
            "sum_grow.bin"   => EEditMode.SUM_GROW,
            "takara.bin"     => EEditMode.TAKARA,
            "w_name.bin"     => EEditMode.W_NAME,
            "weapon.bin"     => EEditMode.WEAPON,
            _                => EEditMode.NULL,
        };
    }
}
