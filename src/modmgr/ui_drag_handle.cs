// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* Jobs:
 * - Render the drag-to-reorder grip in the Enabled panel's last column (see
 *   ui_mod_list.cs's _render_mod_table for the column layout it sits in).
 * - Track an in-progress drag purely in memory (_pending_preview_move) for
 *   responsive live feedback, and queue the final position
 *   (_pending_load_order_drop) once on release, for ui_actions.cs to write to
 *   disk (see UI() in ui.cs for where the queued moves get applied).
 * - Animate the grip's "grab pop" and "release flash" visual feedback.
 */

namespace Fahrenheit.Tools.ModManager;

internal static unsafe partial class FhModManagerUI {
    // Drag-to-reorder (see _render_drag_handle). While a drag is in progress, it
    // only reorders `_catalog.Enabled` in memory (no disk write, no rescan) via
    // _pending_preview_move, so a fast multi-row drag stays cheap and responsive;
    // the real load-order file is only written once, when the mouse is released,
    // via _pending_load_order_drop.
    private static FhInstalledMod? _dragging_mod;

    private sealed record FhPendingPreviewMove(
        int FromIndex,
        int ToIndex);

    private static FhPendingPreviewMove? _pending_preview_move;

    private sealed record FhPendingLoadOrderDrop(
        FhInstalledMod Mod,
        int TargetIndex);

    private static FhPendingLoadOrderDrop? _pending_load_order_drop;

    // How long the release flash (see below) takes to fade back to normal.
    private const double DRAG_HANDLE_RELEASE_FLASH_SECONDS = 0.35;

    // How long the "pop" on grab takes to reach full size.
    private const double DRAG_HANDLE_GRAB_POP_SECONDS = 0.12;

    // Set when a drag handle is released, so the flash fade below knows which
    // mod (if any) is still fading and how far into it we are. Only one grip can
    // ever be mid-animation at a time (one mouse), so this doesn't need to be
    // per-mod state.
    private static string? _drag_handle_released_mod_id;
    private static double  _drag_handle_released_at;
    private static double  _drag_handle_grabbed_at;

    // The row index the mod was at when grabbed - the fixed reference point the
    // "how far has the mouse moved" math below measures from every frame, rather
    // than an incrementally-accumulated position (see the comment on the drag
    // logic below for why that distinction matters).
    private static int _drag_start_index;

    // A grip icon (three stacked bars, like a typical drag handle) that's also the
    // actual drag-to-reorder control. Renders in its own dedicated table column
    // (the last one, in the Enabled panel's mod table - see _render_mod_table),
    // so it sits at the row's right edge; it centers itself within that column
    // both horizontally and vertically against `row_height` (the details
    // column's actual rendered height, which is always the tallest thing in the
    // row). Drawn manually via the draw list rather than a font glyph such as
    // "≡": the loaded font only covers Basic Latin + Latin-1 Supplement (see
    // main.cs's io.Fonts.AddFontFromFileTTF call), so a character outside that
    // range would just render as a blank box.
    //
    // Dragging moves this mod to a new row position - purely in memory
    // (_pending_preview_move) until release, at which point the final position
    // is written once (_pending_load_order_drop). This is deliberately not a
    // formal ImGui payload drag-and-drop (BeginDragDropSource/
    // AcceptDragDropPayload): it's a plain IsItemActive()/GetMouseDragDelta()
    // reposition with no native payload pointers involved.
    private static void _render_drag_handle(
        FhInstalledMod mod,
        int display_index,
        int mod_count,
        float row_height) {
        // Noticeably bigger than a typical small icon button - it's the only
        // control in the row without a native-looking widget to lean on, so it
        // needs to read as clickable/draggable on its own.
        Vector2 size = new(ImGui.GetFrameHeight() * 1.1F, ImGui.GetFrameHeight() * 0.9F);

        // Centered both ways within whatever space the column actually gives it,
        // rather than assuming the column was sized to exactly fit `size` - see
        // `grip_width` in _render_mod_table, which only needs to stay bigger than
        // this, not match it exactly.
        float vertical_offset   = Math.Max(0F, (row_height - size.Y) / 2F);
        float horizontal_offset = Math.Max(0F, (ImGui.GetContentRegionAvail().X - size.X) / 2F);

        ImGui.SetCursorPos(
            ImGui.GetCursorPos() + new Vector2(horizontal_offset, vertical_offset));

        Vector2 top_left = ImGui.GetCursorScreenPos();
        Vector2 center   = top_left + (size / 2F);

        ImGui.InvisibleButton($"##DragHandle.{mod.Id}", size);

        bool hovered = ImGui.IsItemHovered();
        bool active  = ImGui.IsItemActive();

        if (mod.HasValidManifest) {
            if (ImGui.IsItemActivated()) {
                _dragging_mod = mod;
                _drag_start_index = display_index;
                _drag_handle_grabbed_at = ImGui.GetTime();
            }

            bool is_dragging_this = _dragging_mod?.Id == mod.Id;

            if (is_dragging_this && active) {
                /*
                 * Recomputes the target position fresh every frame from the TOTAL
                 * mouse movement since the grab (GetMouseDragDelta never resets),
                 * divided by this row's own actual rendered height - not an
                 * incremental "cross a fixed threshold, move one step, reset and
                 * start counting over" scheme. Recomputing from the fixed start
                 * position every frame instead of accumulating steps means
                 * dragging back up by the same distance always lands exactly back
                 * where you started, with no drift either direction, and dividing
                 * by the row's real height (rather than a fixed guess) keeps the
                 * reorder tracking 1:1 with the mouse regardless of how many lines
                 * of text a given row renders.
                 */
                float drag_delta_y = ImGui.GetMouseDragDelta(ImGuiMouseButton.Left).Y;
                int   row_offset   = (int)MathF.Round(drag_delta_y / row_height);
                int   target_index = Math.Clamp(_drag_start_index + row_offset, 0, mod_count - 1);

                if (target_index != display_index) {
                    _pending_preview_move = new(display_index, target_index);
                }
            }

            if (is_dragging_this && ImGui.IsItemDeactivated()) {
                _pending_load_order_drop = new(mod, display_index);
                _dragging_mod = null;

                _drag_handle_released_mod_id = mod.Id;
                _drag_handle_released_at     = ImGui.GetTime();
            }
        }

        // "Reacts when grabbed": while actively held, everything pops up to
        // ~15% larger over DRAG_HANDLE_GRAB_POP_SECONDS (an ease-out - fast at
        // first, settling in) and gets an accent-tinted background.
        float grab_t = 0F;

        if (active) {
            double grabbed_elapsed = ImGui.GetTime() - _drag_handle_grabbed_at;
            float  grabbed_linear  = Math.Clamp((float)(grabbed_elapsed / DRAG_HANDLE_GRAB_POP_SECONDS), 0F, 1F);

            grab_t = 1F - MathF.Pow(1F - grabbed_linear, 3F);
        }

        float visual_scale = 1F + (0.15F * grab_t);

        // "Reacts when released": a brief accent flash on the bars/background
        // that fades back to normal over DRAG_HANDLE_RELEASE_FLASH_SECONDS.
        double released_elapsed = ImGui.GetTime() - _drag_handle_released_at;

        bool is_flashing =
            _drag_handle_released_mod_id == mod.Id
            && released_elapsed < DRAG_HANDLE_RELEASE_FLASH_SECONDS;

        float flash_t = is_flashing
            ? 1F - (float)(released_elapsed / DRAG_HANDLE_RELEASE_FLASH_SECONDS)
            : 0F;

        // "The whole row lifts, not just the icon": tints the entire row (every
        // column, not just this one) via the table itself, using the same
        // active/flash timing as the grip's own highlight below but softer, since
        // it covers much more area. TableSetBgColor doesn't care that columns 0-2
        // already rendered earlier in this row - the row background is composited
        // once the whole row is done, not as each cell is submitted.
        Vector4 row_background = active
            ? new Vector4(FhTheme.COLOR_ACCENT.X, FhTheme.COLOR_ACCENT.Y, FhTheme.COLOR_ACCENT.Z, 0.16F)
            : new Vector4(0F, 0F, 0F, 0F);

        if (flash_t > 0F) {
            Vector4 flash_row_background = new(FhTheme.COLOR_ACCENT.X, FhTheme.COLOR_ACCENT.Y, FhTheme.COLOR_ACCENT.Z, 0.20F);
            row_background += (flash_row_background - row_background) * flash_t;
        }

        if (row_background.W > 0.01F) {
            ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0, ImGui.GetColorU32(row_background));
        }

        // COLOR_SURFACE_HOVERED is fully opaque (it's meant for solid button
        // backgrounds elsewhere), so it's toned down here for a subtle highlight
        // rather than a solid block behind the bars.
        Vector4 background = active
            ? new Vector4(FhTheme.COLOR_ACCENT.X, FhTheme.COLOR_ACCENT.Y, FhTheme.COLOR_ACCENT.Z, 0.35F)
            : hovered
                ? new Vector4(FhTheme.COLOR_SURFACE_HOVERED.X, FhTheme.COLOR_SURFACE_HOVERED.Y, FhTheme.COLOR_SURFACE_HOVERED.Z, 0.6F)
                : new Vector4(0F, 0F, 0F, 0F);

        if (flash_t > 0F) {
            Vector4 flash_background = new(FhTheme.COLOR_ACCENT.X, FhTheme.COLOR_ACCENT.Y, FhTheme.COLOR_ACCENT.Z, 0.45F);
            background += (flash_background - background) * flash_t;
        }

        ImDrawListPtr draw_list = ImGui.GetWindowDrawList();

        if (background.W > 0.01F) {
            Vector2 background_half_size = (size * visual_scale) / 2F;

            draw_list.AddRectFilled(
                center - background_half_size,
                center + background_half_size,
                ImGui.GetColorU32(background),
                4F);
        }

        Vector4 bar_color = mod.HasValidManifest && (hovered || active)
            ? FhTheme.COLOR_TEXT
            : FhTheme.COLOR_TEXT_MUTED;

        if (flash_t > 0F) {
            bar_color += (FhTheme.COLOR_TEXT - bar_color) * flash_t;
        }

        uint bar_color_u32 = ImGui.GetColorU32(bar_color);

        float scaled_height = size.Y * visual_scale;
        float bar_width     = size.X * visual_scale * 0.7F;
        float bar_x         = center.X - (bar_width / 2F);
        float bar_step      = scaled_height / 4F;
        float bars_top      = center.Y - (scaled_height / 2F);

        for (int i = 1; i <= 3; i++) {
            float y = bars_top + (bar_step * i);

            draw_list.AddLine(
                new Vector2(bar_x, y),
                new Vector2(bar_x + bar_width, y),
                bar_color_u32,
                2F);
        }

        if (hovered) {
            ImGui.SetTooltip(
                mod.HasValidManifest
                    ? "Drag to reorder"
                    : "This mod cannot be reordered");
        }
    }
}
