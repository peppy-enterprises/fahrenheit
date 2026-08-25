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
    private class FhSaveUiSettings {
        //TODO: Change this to a Set-based dropdown once that's created.
        public readonly FhSettingText renderer = new("renderer", FhSaves.DEFAULT_RENDERER_ID);
    }

    private readonly FhSaveUiSettings _settings = new();

    public FhSaveUiModule() {
        settings = new FhSettingsCategory("fhsaveui", [
            _settings.renderer,
        ]);
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        return true;
    }

    public override void render_imgui() {
        if (FhApi.Saves.get_system_mode(out FhExtendedSaveSystemMode? mode)
            && mode is FhExtendedSaveSystemMode.NULL
        ) {
            return;
        }

        if (!FhApi.Saves.get_renderer(_settings.renderer.get(), out FhSaveUiRenderer? renderer)) {
            // Previous renderer is missing (provider mod updated and broke API?)
            // So we try to fall back to default
            _logger.Warning("Failed to find desired renderer, falling back to default.");

            if (!FhApi.Saves.get_renderer(_settings.renderer.get(), out renderer)) {
                // Something has gone disasterously wrong – we're missing our default renderer!
                _logger.Error("Failed to find default renderer.");
                throw new NotImplementedException("Failed to find default renderer.");
            }
        }

        // Silence warnings about further uses of 'renderer' potentially being null.
        if (renderer is null) throw new UnreachableException();

        switch (FhSavePal.pal_get_screen_state()) {
            case FhSaveScreenState.OPENING:
                renderer.load_data();
                break;

            case FhSaveScreenState.OPEN:
                renderer.render_ui();
                renderer.handle_input();
                break;
        }
    }
}
