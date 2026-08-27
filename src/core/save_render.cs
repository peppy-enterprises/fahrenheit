// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit;

public abstract class FhSaveUiRenderer : FhModule {
    /// <summary>The scale between the current and reference display sizes.</summary>
    protected Vector2 scale_factor => FhApi.Gui.display_size / get_ref_size();


    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        _logger.Info($"Registering new save UI renderer: {ModuleType}");
        FhApi.Saves.register_renderer(this);

        return true;
    }

    public sealed override void render_imgui() { }


    /// <summary>Retrieve the reference size that is used by the renderer.</summary>
    /// <returns>The reference size used by the renderer.</returns>
    protected abstract Vector2 get_ref_size();

    /// <summary>Render the save UI.</summary>
    protected internal abstract void render_ui();
}
