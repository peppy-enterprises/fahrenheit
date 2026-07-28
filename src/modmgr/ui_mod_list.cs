// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* Jobs:
 * - Lay out the Enabled/Disabled mod panels and their scrollable tables.
 * - Render each mod row: load-order arrows, enabled checkbox, and details text
 *   (the drag handle itself lives in ui_drag_handle.cs).
 * - Queue - rather than apply - the toggle/move a row's controls request, since
 *   applying it mid-iteration would mutate the very list being iterated. See
 *   ui_actions.cs's _apply_pending_* methods, which apply these once per frame
 *   after rendering finishes (see UI() in ui.cs).
 */

namespace Fahrenheit.Tools.ModManager;

internal static unsafe partial class FhModManagerUI {
    /*
     * A checkbox/arrow click inside _render_mod_panel's `for` loop can't rescan and
     * mutate `_catalog` on the spot: `_catalog.Enabled`/`Disabled` are the very lists
     * that loop is iterating, and a rescan can also change their length. So clicks
     * just record what was requested, and UI() applies it once rendering has finished
     * for the frame (see the two _apply_pending_* calls after ImGui.End() above).
     */
    private sealed record FhPendingModToggle(FhInstalledMod Mod, bool Enable);

    private static FhPendingModToggle? _pending_mod_toggle;

    private sealed record FhPendingLoadOrderMove(FhInstalledMod Mod, int Direction);

    private static FhPendingLoadOrderMove? _pending_load_order_move;

    // Both panels always share the same width/height as each other - only the
    // formula for that shared size (and whether they sit side by side or
    // stacked) differs between the two layouts below.
    private static void _render_mod_lists() {
        Vector2 available = ImGui.GetContentRegionAvail();
        float   spacing   = ImGui.GetStyle().ItemSpacing.X;

        bool stack_panels = available.X < 760F;

        // The status bar (see ui_status_bar.cs, rendered right after this method
        // returns) sits flush against the window's true bottom edge rather than
        // in normal padded content flow, eating into the space style.WindowPadding
        // would otherwise reserve there. GetContentRegionAvail() already excludes
        // that bottom WindowPadding once, so only the amount the bar exceeds it
        // by still needs to be carved out here - not the bar's full height on
        // top of it, which would double-reserve and leave a gap above the bar.
        float status_bar_extra_reserve = Math.Max(0F, _status_bar_height() - ImGui.GetStyle().WindowPadding.Y);

        float available_height = Math.Max(0F, available.Y - status_bar_extra_reserve);

        float min_panel_height = 150F;

        float panel_width;
        float panel_height;

        if (stack_panels) {
            panel_width  = available.X;
            panel_height = Math.Max(min_panel_height, (available_height - spacing) / 2F);
        }
        else {
            /*
             * Leave a small gutter for DPI rounding and the child-window
             * scrollbars so the right panel remains within the viewport.
             */
            float gutter = 6F;

            panel_width  = MathF.Floor((available.X - spacing - gutter) / 2F);
            panel_height = Math.Max(min_panel_height, available_height - (gutter / 3F));
        }

        _render_mod_panel("Enabled Mods", "##EnabledMods", _catalog.Enabled, panel_width, panel_height, true);

        if (!stack_panels) {
            ImGui.SameLine();
        }

        _render_mod_panel("Disabled Mods", "##DisabledMods", _catalog.Disabled, panel_width, panel_height, false);
    }

    private static void _render_mod_panel(string title, string child_id, IReadOnlyList<FhInstalledMod> mods, float width, float height, bool show_load_order) {
        // An outer, non-scrolling child keeps the "Title (N)" header pinned in
        // place and gives it the same fixed width as the panel below; the mod
        // list scrolls in a nested child instead of taking the header with it.
        bool modPanelHeader = ImGui.BeginChild(child_id, new Vector2(width, height), ImGuiChildFlags.None, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
        if (modPanelHeader) {
            _center_cursor_x(ImGui.CalcTextSize($"{title} ({mods.Count})").X);

            ImGui.TextUnformatted($"{title} ({mods.Count})");
            ImGui.Separator();

            bool modPanelList = ImGui.BeginChild($"{child_id}.List", new Vector2(width, height - ImGui.GetFrameHeightWithSpacing()), ImGuiChildFlags.None, ImGuiWindowFlags.AlwaysVerticalScrollbar);

            if (modPanelList) {
                if (mods.Count == 0) {
                    ImGui.TextDisabled(
                        show_load_order
                            ? "No mods are enabled."
                            : "No disabled mods were found.");
                }
                else {
                    _render_mod_table(child_id, mods, show_load_order);
                }
            }

            ImGui.EndChild();
        }

        ImGui.EndChild();
    }

    // Lays the mod list out as a table (Order arrows / Enabled checkbox / Details
    // / Drag handle columns, the first and last only present for the Enabled
    // panel) rather than manually chaining SameLine()/BeginGroup()/cursor jumps:
    // each column is an independent cursor, and the row height is simply however
    // tall its tallest cell is, regardless of how much the arrows, checkbox,
    // details text (2-5 lines depending on the mod), and drag handle differ in
    // height from each other.
    private static void _render_mod_table(
        string child_id,
        IReadOnlyList<FhInstalledMod> mods,
        bool show_load_order) {
        int column_count = show_load_order ? 4 : 2;

        // Spans the full available width, right up to the scrollbar - the
        // alternating TableRowBgAlt shading (ImGuiCol.TableRowBgAlt) is only ever
        // drawn within the table's own bounds. The Drag column already carries
        // its own built-in padding for the same reason (see grip_width below);
        // the Details column relies on the table's own CellPadding instead.
        float table_width = ImGui.GetContentRegionAvail().X;

        bool table = ImGui.BeginTable($"{child_id}.Table", column_count, ImGuiTableFlags.BordersInnerH | ImGuiTableFlags.RowBg, new Vector2(table_width, 0F));
        if (!table) {
            return;
        }

        float control_width = ImGui.GetFrameHeight();

        // Wider than the grip's own drawn size (see _render_drag_handle's `size`,
        // 1.1x the frame height) by a full ItemSpacing on each side, so the grip
        // has visible padding around it instead of sitting flush against the row.
        // _render_drag_handle centers itself within whatever width this column
        // actually reports rather than assuming an exact fit, so the two only
        // need to agree that this stays bigger than the grip, not on a precise
        // shared size.
        float grip_width = ImGui.GetFrameHeight() * 1.1F + (ImGui.GetStyle().ItemSpacing.X * 2F);

        if (show_load_order) {
            ImGui.TableSetupColumn("##Order",   ImGuiTableColumnFlags.WidthFixed, control_width);
            ImGui.TableSetupColumn("##Enabled", ImGuiTableColumnFlags.WidthFixed, control_width);
            ImGui.TableSetupColumn("##Details", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("##Drag",    ImGuiTableColumnFlags.WidthFixed, grip_width);
        }
        else {
            ImGui.TableSetupColumn("##Enabled", ImGuiTableColumnFlags.WidthFixed, control_width);
            ImGui.TableSetupColumn("##Details", ImGuiTableColumnFlags.WidthStretch);
        }

        for (int index = 0; index < mods.Count; index++) {
            _render_mod(mods[index], show_load_order, index, mods.Count);
        }

        ImGui.EndTable();
    }

    // One of the Order column's up/down arrow buttons: disabled when the mod
    // can't move that way (invalid manifest, or already at that end of the
    // list), and queues a load-order move by `move_direction` when clicked.
    private static void _render_load_order_arrow_button(
        FhInstalledMod mod,
        ImGuiDir direction,
        int move_direction,
        bool enabled,
        string tooltip) {
        if (!enabled) {
            ImGui.BeginDisabled();
        }

        if (ImGui.ArrowButton($"##Move{direction}.{mod.Manifest.Id}", direction)) {
            _pending_load_order_move = new(mod, move_direction);
        }

        if (ImGui.IsItemHovered()) {
            ImGui.SetTooltip(tooltip);
        }

        if (!enabled) {
            ImGui.EndDisabled();
        }
    }

    private static void _render_mod(
        FhInstalledMod mod,
        bool show_load_order,
        int display_index,
        int mod_count) {
        ImGui.TableNextRow();

        if (show_load_order) {
            ImGui.TableSetColumnIndex(0);

            bool can_move_up   = mod.HasValidManifest && display_index > 0;
            bool can_move_down = mod.HasValidManifest && display_index < mod_count - 1;

            _render_load_order_arrow_button(mod, ImGuiDir.Up,   -1, can_move_up,   "Move up");
            _render_load_order_arrow_button(mod, ImGuiDir.Down,  1, can_move_down, "Move down");
        }

        ImGui.TableSetColumnIndex(show_load_order ? 1 : 0);

        float row_top_y = ImGui.GetCursorPosY();

        // Always togglable, even for a mod with a missing directory or a broken
        // manifest: enabling/disabling only adds or removes its ID from the load
        // order text file (see FhLoadOrderEditor.try_set_enabled in mods.cs),
        // which doesn't require anything about the mod itself to be valid - and
        // disabling is the way to get a broken entry out of the load order in
        // the first place, so gating the checkbox on validity would make an
        // already-broken entry impossible to clear from the UI. The details text
        // below still calls out exactly what's wrong with it.
        bool enabled = mod.IsEnabled;

        if (ImGui.Checkbox($"##Enabled.{mod.Manifest.Id}", ref enabled)) {
            _pending_mod_toggle = new( mod, enabled);
        }

        // Where the Order column's down arrow naturally lands: the row's top plus
        // one frame-widget's height (the up arrow/checkbox are both frame widgets
        // of the same height) plus one ItemSpacing gap. The load order number and
        // the Author/path line below it are pinned to this same Y explicitly,
        // rather than left to their own natural flow, so all three "second line"
        // elements - down arrow, number, Author - line up with each other instead
        // of drifting based on how tall a checkbox vs. a plain text line is.
        float second_line_y = row_top_y + ImGui.GetFrameHeight() + ImGui.GetStyle().ItemSpacing.Y;

        if (show_load_order) {
            ImGui.SetCursorPosY(second_line_y);

            _render_centered_load_order_number(display_index + 1);
        }

        ImGui.TableSetColumnIndex(show_load_order ? 2 : 1);

        // The details column is always the tallest cell in the row (2-4 text
        // lines vs. a couple of small buttons/a checkbox/the grip), so its actual
        // rendered height is what the drag handle centers itself against below.
        float details_start_y = ImGui.GetCursorPosY();

        if (!mod.DirectoryExists) {
            _text_colored_wrapped(ImGui.GetStyle().Colors[(int)ImGuiCol.NavCursor], $"Missing: {mod.Manifest.Id}");

            if (show_load_order) {
                ImGui.SetCursorPosY(second_line_y);
            }

            _text_disabled_wrapped(
                mod.DirectoryPath);
        }
        else {
            string version = "" + (string.IsNullOrWhiteSpace(mod.Manifest.Version)
                ? "unknown version"
                : $"v{mod.Manifest.Version}");

            _text_wrapped(mod.Manifest.Name);
            ImGui.SameLine();
            _text_disabled_wrapped(version);

            if (show_load_order) {
                ImGui.SetCursorPosY(second_line_y);
            }

            if (!string.IsNullOrWhiteSpace(mod.Manifest.Authors)) {
                _text_disabled_wrapped($"Author: {mod.Manifest.Authors}");
            }

            if (!mod.ManifestExists) {
                _text_colored_wrapped(ImGui.GetStyle().Colors[(int)ImGuiCol.DragDropTarget], "Invalid mod: expected manifest is missing.");
            }
            else if (!string.IsNullOrWhiteSpace(mod.ManifestError)) {
                _text_colored_wrapped(ImGui.GetStyle().Colors[(int)ImGuiCol.NavCursor], $"Invalid mod: manifest error: {mod.ManifestError}");
            }
        }

        float details_height = ImGui.GetCursorPosY() - details_start_y;

        if (show_load_order) {
            ImGui.TableSetColumnIndex(3);

            _render_drag_handle(mod, display_index, mod_count, details_height);
        }
    }
}
