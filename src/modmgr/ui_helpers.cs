// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* Jobs:
 * - Small, reusable pieces used across the other ui_*.cs rendering files:
 *   state-mutation helpers that touch FhModManagerUI's own private fields,
 *   plus plain ImGui layout/text utilities that don't have anywhere more
 *   specific to live.
 */

namespace Fahrenheit.Tools.ModManager;

internal static unsafe partial class FhModManagerUI {
    // Sets the message shown in the status bar (see _render_status_bar in
    // ui_status_bar.cs). Centralizes the `_status`/`_status_is_error`/
    // `_status_set_at` trio so action handlers don't each assign all three by
    // hand - `_status_set_at` is what the status bar's fade-out times itself
    // against, so it has to be stamped fresh on every call, not just the first.
    private static void _set_status(string message, bool is_error = false) {
        _status = message;
        _status_is_error = is_error;
        _status_set_at = ImGui.GetTime();
    }

    // Re-scans the configured game directory and refreshes `_catalog`. Used after
    // any operation that changes the mods directory or load order file on disk
    // (rather than patching `_catalog` in place), so the in-memory catalog reflects
    // exactly what was written, including anything else that changed underneath us.
    private static void _rescan_mods() {
        _catalog = FhModScanner.scan(_settings.GameDirectory, _settings.FahrenheitDirectory, _settings.ModsDirectory);
    }

    // Centers the next popup on the app's own viewport, not the desktop. With
    // multi-viewport enabled, ImGui popups are positioned in absolute desktop
    // coordinates, so naively centering on io.DisplaySize/2 anchors near the
    // primary monitor's origin instead of wherever this window actually is -
    // very visible once the window has been moved, or on a multi-monitor setup.
    //
    // Width is a fraction of the viewport's own width rather than a flat pixel
    // count, clamped to [min_width, max_width], while still not ballooning to
    // something absurd on an ultrawide or 8K screen. Height is left at 0
    // (auto-fit content) unless the caller passes one.
    private static void _center_next_window(
        float width_fraction,
        float min_width,
        float max_width,
        float height = 0F) {
        ImGuiViewportPtr viewport = ImGui.GetMainViewport();

        float width = Math.Clamp(viewport.Size.X * width_fraction, min_width, max_width);

        Vector2 center = viewport.Pos + viewport.Size / 2F;

        ImGui.SetNextWindowPos(center, pivot: new Vector2(0.5F, 0.5F));
        ImGui.SetNextWindowSize(new Vector2(width, height));
    }

    // ImGui auto-sizes a button to its label plus padding on both sides; the
    // fixed +4F just adds a little breathing room so the text never looks flush
    // against the button's edge. This is used to size buttons up front so the
    // input field next to them can be given the exact remaining width.
    private static float _get_button_width(string label) {
        ImGuiStylePtr style = ImGui.GetStyle();

        return ImGui.CalcTextSize(label).X + (style.FramePadding.X * 2F) + 4F;
    }

    // Nudges the cursor right so that something `content_width` wide, drawn from
    // here, ends up centered within whatever width is left in the current column/
    // content region. Left alone (not centered) if it wouldn't fit.
    private static void _center_cursor_x(float content_width) {
        float available_width = ImGui.GetContentRegionAvail().X;

        if (content_width < available_width) {
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + (available_width - content_width) / 2F);
        }
    }

    // The load order position, centered under the Enabled checkbox in its own
    // column rather than prefixed onto the mod name in the Details column.
    private static void _render_centered_load_order_number(int position) {
        string text = position.ToString();

        _center_cursor_x(ImGui.CalcTextSize(text).X);

        ImGui.TextDisabled(text);
    }

    private static void _text_wrapped(string text) {
        float wrap_position = ImGui.GetCursorPosX() + ImGui.GetContentRegionAvail().X;

        ImGui.PushTextWrapPos(wrap_position);
        ImGui.TextUnformatted(text);
        ImGui.PopTextWrapPos();
    }

    private static void _text_disabled_wrapped(string text) {
        ImGui.PushStyleColor(ImGuiCol.Text, ImGui.GetColorU32(ImGuiCol.TextDisabled));

        _text_wrapped(text);

        ImGui.PopStyleColor();
    }

    private static void _text_colored_wrapped( Vector4 color, string text) {
        ImGui.PushStyleColor(ImGuiCol.Text, color);

        _text_wrapped(text);

        ImGui.PopStyleColor();
    }

    // A small green check (valid) or red X (invalid) glyph next to a directory.
    // Hand-drawn via the draw list rather than a "✓"/"✗" character: the loaded
    // font only covers Basic Latin + Latin-1 Supplement, so either glyph would
    // just render as a blank box.
    private static void _status_icon(bool is_valid, string? tooltip) {
        float extent = ImGui.GetTextLineHeight();
        Vector2 size = new(extent, extent);

        Vector2 top_left = ImGui.GetCursorScreenPos();
        Vector2 center    = top_left + (size / 2F);

        ImGui.Dummy(size);

        ImDrawListPtr draw_list  = ImGui.GetWindowDrawList();
        uint          color      = ImGui.GetColorU32(is_valid ? ImGuiCol.CheckMark : ImGuiCol.NavCursor);
        float         glyph      = extent * 0.5F;
        float         thickness  = MathF.Max(1F, glyph * 0.3F);

        if (is_valid) {
            Vector2 p1 = center + new Vector2(-glyph * 0.5F, 0F);
            Vector2 p2 = center + new Vector2(-glyph * 0.05F, glyph * 0.45F);
            Vector2 p3 = center + new Vector2(glyph * 0.55F, -glyph * 0.5F);

            draw_list.AddLine(p1, p2, color, thickness);
            draw_list.AddLine(p2, p3, color, thickness);
        }
        else {
            Vector2 half = new(glyph * 0.5F, glyph * 0.5F);

            draw_list.AddLine(center - half, center + half, color, thickness);
            draw_list.AddLine(
                center + new Vector2(half.X, -half.Y),
                center + new Vector2(-half.X, half.Y),
                color,
                thickness);
        }

        if (!string.IsNullOrWhiteSpace(tooltip) && ImGui.IsItemHovered()) {
            ImGui.SetTooltip(tooltip);
        }
    }
}
