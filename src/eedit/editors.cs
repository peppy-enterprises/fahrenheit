// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit.Tools.EEdit;

internal abstract class EEditComponent {
    internal abstract void render();
}

internal abstract class EEditComponent<T> : EEditComponent where T : unmanaged {

    protected static readonly string  _type_name = typeof(T).Name;
    protected        readonly byte[]  _file      = new byte[EEdit.active_file!.Length];
    protected        readonly List<T> _elements  = [];

    internal EEditComponent() {
        EEdit.active_file.ReadExactly(_file);

        ExcelReader<T> iter = new(_file);

        foreach (ExcelHeader header in iter.get_headers()) {
            _elements.AddRange(iter.get_elements(header));
        }
    }

}

// TODO: insert actual UI per wishes

internal class EditorNull : EEditComponent {
    internal override void render() { }
}

internal class EditorSphere : EEditComponent<Sphere> {
    internal override void render() { }
}

internal class EditorWeaponName : EEditComponent<WeaponName> {
    internal override void render() { }
}

internal class EditorRate : EEditComponent<Rate> {
    internal override void render() { }
}

internal class EditorText : EEditComponent<HelpText> {
    internal override void render() { }
}

internal class EditorTextPair : EEditComponent<NameHelpText> {
    internal override void render() { }
}

internal class EditorPlyRom : EEditComponent<PlyRom> {
    internal override void render() { }
}

internal class EditorMonmagic : EEditComponent<Command> {
    internal override void render() { }
}

internal class EditorKaizou : EEditComponent<CustomizationRecipe> {
    internal override void render() { }
}

internal class EditorSumAssure : EEditComponent<AeonStatBoostsMinimum> {
    internal override void render() { }
}

internal class EditorStNumber : EEditComponent<StNumber> {
    internal override void render() { }
}
