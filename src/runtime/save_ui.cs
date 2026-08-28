// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

using System.Diagnostics;

namespace Fahrenheit.Runtime;

/* [fkelava 07/05/26 17:39]
 * Fahrenheit completely overrides the game's base save system to allow set functionality
 * and lifting the limit of 200 saves per set. For performance reasons, this requires
 * the game's base Flash-based Iggy UI to be bypassed. This module implements its ImGui replacement.
 *
 * Specifically, the actual save UI is implemented in ActionScript, which the game calls through to.
 * This is extremely inefficient, and computationally scales almost quadratically with the number of saves;
 * raising the set limit from 200 to 500 caused set listing times in excess of ~10s. It is almost certain
 * that the limit of 200 was chosen because it was the largest number that still performed acceptably.
 */

/// <summary>
///     Implements Fahrenheit's replacement save/load user interface.
/// </summary>
[FhLoad(FhGameId.FFX | FhGameId.FFX2 | FhGameId.FFX2LM)]
public sealed class FhSaveUiModule : FhModule {
    private FhModuleHandle<FhSaveUiRendererX>  _handle_render_x;
    // private FhModuleHandle<FhSaveUiRendererX2> _handle_render_x2;

    private FhSaveUiRendererX?  _render_x;
    // private FhSaveUiRendererX2? _render_x2;

    private class FhSaveUiSettings {
        //TODO: Change this to a Set-based dropdown once that's created.
        public readonly FhSettingText renderer = new("renderer", "");
    }

    private readonly FhSaveUiSettings _settings = new();

    public FhSaveUiModule() {
        settings = new FhSettingsCategory("fhsaveui", [
            _settings.renderer,
        ]);

        _handle_render_x  = new(this);
        // _handle_render_x2 = new(this);
    }

    private string get_default_renderer_id() {
        return FhGlobal.game_id switch {
            FhGameId.FFX    => _render_x!.ModuleType,
            // FhGameId.FFX2   or
            // FhGameId.FFX2LM => _render_x2!.ModuleType,

            _ => throw new NotImplementedException(),
        };
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        bool got_modules = FhGlobal.game_id switch {
            FhGameId.FFX    => _handle_render_x.try_get_module(out _render_x),
            // FhGameId.FFX2   or
            // FhGameId.FFX2LM => _handle_render_x2.try_get_module(out _render_x2),
        };

        _settings.renderer.set(get_default_renderer_id());

        return got_modules;
    }

    public override void render_imgui() {
        if (FhApi.Saves.get_system_mode(out FhSaveSystemMode? mode)
            && mode is FhSaveSystemMode.NULL
        ) {
            return;
        }

        if (!FhApi.Saves.get_renderer(_settings.renderer.get(), out FhSaveUiRenderer? renderer)) {
            // Previous renderer is missing (provider mod updated and broke API?)
            // So we try to fall back to default
            _logger.Warning($"Failed to find desired renderer \"{_settings.renderer.get()}\", falling back to default.");

            _settings.renderer.set(get_default_renderer_id());

            if (!FhApi.Saves.get_renderer(_settings.renderer.get(), out renderer)) {
                // Something has gone disasterously wrong – we're missing our default renderer!
                _logger.Error("Failed to find default renderer.");
                throw new NotImplementedException("Failed to find default renderer.");
            }
        }

        // Silence warnings about further uses of 'renderer' potentially being null.
        if (renderer is null) throw new UnreachableException();

        if (FhSavePal.pal_get_screen_state() is FhSaveScreenState.OPEN) {
            renderer.render_ui();
        }
    }
}
