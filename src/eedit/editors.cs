// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit.Tools.EEdit;

internal abstract class EEditComponent {
    internal abstract bool try_parse(ReadOnlySpan<byte> file_contents);
    internal abstract void render();
}

internal abstract class EEditComponent<T> : EEditComponent where T : unmanaged {
    internal override bool try_parse(ReadOnlySpan<byte> file_contents) {
        return false;
    }
}

internal unsafe class EditorKaizou : EEditComponent<CustomizationRecipe> {
    internal override void render() {

    }
}

internal class EditorSumAssure : EEditComponent<AeonStatBoostsMinimum> {
    internal override void render() {

    }
}
