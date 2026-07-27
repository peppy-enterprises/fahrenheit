// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* Jobs:
 * - Own the status message state (_status/_status_is_error/_status_set_at),
 *   written by _set_status (ui_helpers.cs) from action handlers all over the
 *   app.
 * - Render it as a fixed-height bar pinned to the bottom of the main window,
 *   fading the message out on its own a few seconds after it was set, rather
 *   than leaving it to sit there until the next action overwrites it.
 */

namespace Fahrenheit.Tools.ModManager;

internal static unsafe partial class FhModManagerUI {
    private static string _status = "";
    private static bool   _status_is_error;
    private static double _status_set_at;

    // How long a status message stays fully visible before it starts fading.
    private const double STATUS_BAR_VISIBLE_SECONDS = 4.0;

    // How long the fade-out itself takes, once it starts.
    private const double STATUS_BAR_FADE_SECONDS = 0.6;

    // Top/bottom padding around the single line of text the bar ever holds -
    // deliberately just enough to read as its own strip, not a full frame-sized
    // control like a button or input box has room for. Scaled by FhTheme.UiScale
    // (see its own comment in theme.cs), same as every other raw pixel number
    // in this app.
    private static float STATUS_BAR_VERTICAL_PADDING => 5F * FhTheme.UiScale;

    // The bar's own fixed height, regardless of whether it currently has
    // anything to show - see _render_mod_lists (ui_mod_list.cs), which reserves
    // exactly this much space below the mod panels so the bar never has to
    // overlap them, and the panels never have to resize as messages come and go.
    private static float _status_bar_height() {
        return ImGui.GetTextLineHeight() + (STATUS_BAR_VERTICAL_PADDING * 2F);
    }

    // A strip spanning the full window width and sitting flush against its
    // bottom edge - unlike everything else in the window, which sits inset
    // within style.WindowPadding - holding whatever _set_status last wrote.
    // Always rendered (background included) even with nothing to say, so its
    // height is a constant the layout above it can rely on; only the text
    // itself fades in and out.
    private static void _render_status_bar() {
        float height       = _status_bar_height();
        float window_width = ImGui.GetWindowWidth();

        ImGui.SetCursorPos(new Vector2(0F, ImGui.GetWindowHeight() - height));

        ImGui.PushStyleColor(ImGuiCol.ChildBg, FhTheme.COLOR_BG_RAISED);

        if (ImGui.BeginChild(
                "##StatusBar",
                new Vector2(window_width, height),
                ImGuiChildFlags.None,
                ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)) {
            float alpha = _status_alpha();

            if (alpha > 0F) {
                Vector4 color = _status_is_error ? FhTheme.COLOR_ERROR : FhTheme.COLOR_SUCCESS;
                color.W *= alpha;

                // Lines up with the header/mod-list text above, which all sit
                // inset by this same WindowPadding.X from the window's edge.
                ImGui.SetCursorPos(new Vector2(
                    ImGui.GetStyle().WindowPadding.X,
                    STATUS_BAR_VERTICAL_PADDING));

                ImGui.PushStyleColor(ImGuiCol.Text, color);
                ImGui.TextUnformatted(_status);
                ImGui.PopStyleColor();
            }
        }

        ImGui.EndChild();

        ImGui.PopStyleColor();
    }

    // 1 for STATUS_BAR_VISIBLE_SECONDS after the message was set, then eases
    // down to 0 over STATUS_BAR_FADE_SECONDS. 0 whenever there's no message at
    // all, so a stale one from much earlier in the session can never reappear.
    private static float _status_alpha() {
        if (string.IsNullOrWhiteSpace(_status)) {
            return 0F;
        }

        double elapsed = ImGui.GetTime() - _status_set_at;

        if (elapsed < STATUS_BAR_VISIBLE_SECONDS) {
            return 1F;
        }

        double fade_elapsed = elapsed - STATUS_BAR_VISIBLE_SECONDS;

        if (fade_elapsed >= STATUS_BAR_FADE_SECONDS) {
            return 0F;
        }

        return 1F - (float)(fade_elapsed / STATUS_BAR_FADE_SECONDS);
    }
}
