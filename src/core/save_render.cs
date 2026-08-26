// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit;

public abstract class FhSaveUiRenderer : FhModule {
    /// <summary>The current display size.</summary>
    protected static Vector2 display_size => ImGui.GetMainViewport().WorkSize;

    /// <summary>The scale between the current and reference display sizes.</summary>
    protected Vector2 scale_factor => display_size / get_ref_size();


    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        _logger.Info($"Registering new save UI renderer: {get_id()}");
        FhApi.Saves.register_renderer(get_id(), this);

        return true;
    }

    public sealed override void render_imgui() { }


    /// <summary>Retrieve the reference size that is used by the renderer.</summary>
    /// <returns>The reference size used by the renderer.</returns>
    protected abstract Vector2 get_ref_size();

    /// <summary>Retrieved the ID of this renderer.</summary>
    /// <returns>The ID to be used for this renderer.</returns>
    protected abstract string get_id();

    /// <summary>Render the save UI.</summary>
    protected internal abstract void render_ui();

    /// <summary>Handle input to control the UI.</summary>
    /// <remarks>
    ///     This method is called after <see cref="render_ui"/> every frame.
    ///     Due to how ImGui works, you are encouraged to handle input
    ///     in your rendering methods as well.
    /// </remarks>
    protected internal abstract void handle_input();
}
