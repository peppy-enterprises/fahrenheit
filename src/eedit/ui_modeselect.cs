// SPDX-License-Identifier: MIT

namespace Fahrenheit.Tools.EEdit;

/// <summary>
///     The well-known Excel file types in enum form, used for internal ordering.
/// </summary>
internal enum EEditMode {
    KAIZOU           = 0,
    SUM_ASSURE       = 1,
    FILE_TYPES_COUNT = 2
}

internal static unsafe partial class UI {

    /// <summary>
    ///     Obtains a descriptive display string for the <see cref="EEditComponent"/>
    ///     that implements editing the selected <paramref name="mode"/>.
    /// </summary>
    private static string _get_component_name_by_mode(EEditMode mode) {
        return mode switch {
            EEditMode.KAIZOU     => $"{nameof(CustomizationRecipe)} - (kaizou.bin)",
            EEditMode.SUM_ASSURE => $"{nameof(AeonStatBoostsScaling)} - (sum_assure.bin)",
            _                    => throw new NotImplementedException("UNREACHABLE")
        };
    }

    /// <summary>
    ///     Obtains the <see cref="EEditComponent"/> that implements editing the selected <paramref name="mode"/>.
    /// </summary>
    private static EEditComponent _get_component_by_mode(EEditMode mode) {
        return mode switch {
            EEditMode.KAIZOU     => new EditorKaizou(),
            EEditMode.SUM_ASSURE => new EditorSumAssure(),
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
            "btl.bin"        => null,
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
            "kaizou.bin"     => new EditorKaizou(),
            "magic.bin"      => null,
            "menu_panel.bin" => null,
            "menu_txt.bin"   => null,
            "menu_txt2.bin"  => null,
            "menu.bin"       => null,
            "mmain_txt.bin"  => null,
            "monmagic.bin"   => null,
            "monmagic1.bin"  => null,
            "monmagic2.bin"  => null,
            "monster1.bin"   => null,
            "monster2.bin"   => null,
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
            "sum_assure.bin" => new EditorSumAssure(),
            "sum_grow.bin"   => null,
            "summon_txt.bin" => null,
            "takara.bin"     => null,
            "w_name.bin"     => null,
            "weapon.bin"     => null,
            _                => null,
        };
    }
}
