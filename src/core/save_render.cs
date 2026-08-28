// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit;

/// <summary>The base class for custom renderers of the save/load screen.</summary>
public abstract class FhSaveUiRenderer : FhModule {
    /// <summary>The ratio between the current and reference display sizes.</summary>
    /// <example>
    ///     To scale a rectangle from the reference size to the display size,
    ///     we can use the <c>Rect.scale_raw</c> method.
    ///     <code>
    ///     Rect rect = new Rect {
    ///         pos  = pos,
    ///         size = size,
    ///     };
    ///
    ///     Rect rect_scaled     = rect       .scale_raw(scale_factor);
    ///     UV   rect_screen_uvs = rect_scaled.as_uv();
    ///     </code>
    ///
    ///     To scale a font size, we should use only the vertical scale factor:
    ///     <code>
    ///     float font_size = 36f * scale_factor.Y;
    ///     </code>
    /// </example>
    protected Vector2 scale_factor => FhApi.Gui.display_size / get_ref_size();

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        _logger.Info($"Registering new save UI renderer: {ModuleType}");
        FhApi.Saves.register_renderer(this);

        return true;
    }

    public sealed override void render_imgui() { }

    /// <summary>Retrieve the reference size that is used by the renderer.</summary>
    /// <remarks>
    ///     On-screen coordinates used in the renderer should first be expressed
    ///     within these bounds, then scaled by <see cref="scale_factor"/>.
    /// </remarks>
    /// <returns>The reference size used by the renderer.</returns>
    protected abstract Vector2 get_ref_size();

    /// <summary>Render the save UI.</summary>
    protected internal abstract void render_ui();
}
