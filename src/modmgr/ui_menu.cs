// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* Jobs:
 * - Render the menu bar (Mods / Play / Settings) and dispatch its actions,
 *   plus the app icon (see icon.cs) in its own left inset.
 * - Render the minimize/maximize/close buttons drawn into the menu bar in
 *   place of the OS title bar (see chrome.cs).
 * - Open the Settings/Import EFL Mod popups from outside any menu scope,
 *   since ImGui.OpenPopup() can't be called from inside a BeginMenu() block.
 */

namespace Fahrenheit.Tools.ModManager;

internal static unsafe partial class FhModManagerUI {
    // ImGui renders the menu bar flush against the window's own top/left/right
    // edges - it doesn't participate in style.WindowPadding the way normal window
    // content does - so both the horizontal inset and the extra vertical room
    // below have to be added here instead, rather than coming for free the way
    // they do for everything rendered under the menu bar. Computed properties
    // rather than a `const`/a `static readonly` field initializer, since both
    // need FhTheme.UiScale (see its own comment in theme.cs) applied fresh -
    // a field initializer would run at type-init time, which isn't guaranteed
    // to be after main.cs has set UiScale.
    private static float MENU_BAR_EDGE_INSET => 12F * FhTheme.UiScale;

    private static Vector2 MENU_BAR_FRAME_PADDING => new Vector2(10F, 8F) * FhTheme.UiScale;

    private static void _render_main_menu() {
        if (!ImGui.BeginMenuBar()) {
            return;
        }

        // Taller than the default FramePadding, so the bar itself grows and both
        // the menu items and the window control buttons below (which size
        // themselves off GetFrameHeight()) get more vertical breathing room
        // instead of sitting flush top-and-bottom against a cramped bar.
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, MENU_BAR_FRAME_PADDING);

        // Drawn straight to the draw list rather than as an ImGui.Image()
        // widget: a widget call participates in the menu bar's own horizontal-
        // layout tracking the same way BeginMenu()/MenuItem() do, and an
        // out-of-band cursor jump right before/after it (needed to size and
        // center the icon) was enough to desync that tracking - "Play",
        // "Settings", and "About" wrapped onto what looked like a second row.
        // The draw list paints on top of whatever's already there without the
        // cursor ever knowing about it, the same way the drag handle
        // (ui_drag_handle.cs) and window control icons below draw themselves.
        _render_menu_bar_icon();

        // The separator right below the bar (see UI() in ui.cs) sits flush
        // against its bottom edge, with no gap of a different color in between -
        // so the bar's own FramePadding, pushed above, is the only padding the
        // row needs: it's already symmetric top-to-bottom, which is what centers
        // the row in the (now fully COLOR_TITLE_BAR-filled) box on its own.
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + _menu_row_start_x());

        Vector2 menu_cluster_min = ImGui.GetCursorScreenPos();

        if (ImGui.BeginMenu("Mods")) {
            if (ImGui.MenuItem("Install from File")) {
                _install_mod_from_file();
            }

            if (ImGui.MenuItem("Import EFL Mod")) {
                _show_efl_import_dialog = true;
            }

            ImGui.Separator();

            if (ImGui.MenuItem("Refresh Mod List")) {
                _refresh_mods();
            }

            if (ImGui.MenuItem("Export Mod List")) {
                _export_mod_list();
            }

            ImGui.Separator();

            if (ImGui.MenuItem("Import Mod Pack")) {
                _import_mod_pack();
            }

            if (ImGui.MenuItem("Export Mod Pack")) {
                _export_mod_pack();
            }

            ImGui.EndMenu();
        }

        if (ImGui.BeginMenu("Play")) {
            if (ImGui.MenuItem("Final Fantasy X HD Remaster")) {
                _launch_game(FhLaunchTarget.FFX, false);
            }

            if (ImGui.MenuItem("Final Fantasy X-2 HD Remaster")) {
                _launch_game(FhLaunchTarget.FFX2, false);
            }

            ImGui.Separator();

            if (ImGui.MenuItem("Final Fantasy X HD Remaster (Debug)")) {
                _launch_game(FhLaunchTarget.FFX, true);
            }

            if (ImGui.MenuItem("Final Fantasy X-2 HD Remaster (Debug)")) {
                _launch_game(FhLaunchTarget.FFX2, true);
            }

            ImGui.EndMenu();
        }

        if (ImGui.MenuItem("Settings")) {
            _show_settings_dialog = true;
        }

        Vector2 menu_cluster_max = ImGui.GetItemRectMax();

        ImGui.MenuItem("About");

        _render_window_controls();

        FhWindowChrome.mark_menu_cluster(menu_cluster_min, menu_cluster_max);
        FhWindowChrome.set_titlebar_height(ImGui.GetFrameHeight());

        ImGui.PopStyleVar();

        ImGui.EndMenuBar();
    }

    // The icon's height matches the row's own text line height (not the
    // taller frame height, which includes FramePadding on top of the text) so
    // it reads as "the same size as the text", with width derived from
    // FhAppIcon's native pixel dimensions to keep its proportions.
    private static float _menu_bar_icon_height() {
        return ImGui.GetTextLineHeight();
    }

    private static float _menu_bar_icon_width() {
        if (FhAppIcon.Texture == null || FhAppIcon.Size.Y <= 0F) {
            return 0F;
        }

        return _menu_bar_icon_height() * (FhAppIcon.Size.X / FhAppIcon.Size.Y);
    }

    // Where "Mods" (and the rest of the row) starts: past the icon plus a
    // small gap on each side of it, or just the plain edge inset if the icon
    // failed to load.
    private static float _menu_row_start_x() {
        float icon_width = _menu_bar_icon_width();

        return icon_width > 0F
            ? MENU_BAR_EDGE_INSET + icon_width + MENU_BAR_EDGE_INSET
            : MENU_BAR_EDGE_INSET;
    }

    // Draws the Fahrenheit airship mark (see icon.cs) into the space left of
    // "Mods", vertically centered in the bar. No-ops if the texture failed to
    // load (see FhAppIcon.load).
    private static void _render_menu_bar_icon() {
        if (FhAppIcon.Texture is not ImTextureRef texture) {
            return;
        }

        float icon_width  = _menu_bar_icon_width();
        float icon_height = _menu_bar_icon_height();

        if (icon_width <= 0F) {
            return;
        }

        float bar_height       = ImGui.GetFrameHeight();
        float vertical_offset  = Math.Max(0F, (bar_height - icon_height) / 2F);

        Vector2 top_left = ImGui.GetCursorScreenPos() + new Vector2(MENU_BAR_EDGE_INSET, vertical_offset);
        Vector2 bottom_right = top_left + new Vector2(icon_width, icon_height);

        ImGui.GetWindowDrawList().AddImage(texture, top_left, bottom_right);
    }

    // Minimize/maximize/close, right-aligned in the menu bar, in place of the OS's
    // own title bar buttons (the SDL window is created borderless - see main.cs
    // and chrome.cs). Icons are drawn by hand via the draw list (see
    // _window_control_button below) rather than as text glyphs like "_" "[ ]" "X":
    // a glyph's vertical position is set by the font (an underscore in
    // particular sits at the baseline, near the bottom of the line - on this bar's
    // taller buttons it could render almost entirely below the visible area), so
    // hand-drawn lines/rects are centered on the button exactly instead, and read
    // closer to a native title bar's icons besides. Reports its own screen-space
    // rect to FhWindowChrome too, so the native hit-test callback doesn't treat
    // clicking these as a window-drag.
    private static void _render_window_controls() {
        float button_width = 40F * FhTheme.UiScale;

        Vector2 button_size = new(button_width, ImGui.GetFrameHeight());
        float   controls_width = button_width * 3F;

        ImGui.SetCursorPosX(ImGui.GetWindowWidth() - controls_width - MENU_BAR_EDGE_INSET);

        Vector2 controls_min = ImGui.GetCursorScreenPos();

        if (_window_control_button("##Minimize", button_size, FhTheme.COLOR_SURFACE_HOVERED, _draw_minimize_icon)) {
            FhWindowChrome.minimize();
        }

        ImGui.SameLine(0F, 0F);

        if (_window_control_button(
                "##MaximizeRestore",
                button_size,
                FhTheme.COLOR_SURFACE_HOVERED,
                FhWindowChrome.IsMaximized ? _draw_restore_icon : _draw_maximize_icon)) {
            FhWindowChrome.toggle_maximize();
        }

        ImGui.SameLine(0F, 0F);

        if (_window_control_button("##Close", button_size, FhTheme.COLOR_ERROR, _draw_close_icon)) {
            FhWindowChrome.request_quit();
        }

        Vector2 controls_max = controls_min + new Vector2(controls_width, button_size.Y);

        FhWindowChrome.mark_window_controls(controls_min, controls_max);
    }

    // An InvisibleButton (for hit-testing/hover/active state) plus a manually
    // drawn background rect and icon, centered on the button's own midpoint
    // regardless of font metrics. `hover_color` fills the button while
    // hovered/active, same as FhElements.button_chrome used to (transparent
    // otherwise, so it reads as title bar chrome rather than competing for
    // attention with the Mods/Play/Settings menu next to it); it's also handed
    // to `draw_icon` as the button's current background, so an icon that draws
    // overlapping shapes (see _draw_restore_icon) can punch out the overlap in
    // whatever color is actually showing right now.
    private static bool _window_control_button(
        string id,
        Vector2 size,
        Vector4 hover_color,
        Action<ImDrawListPtr, Vector2, float, Vector4> draw_icon) {
        Vector2 top_left = ImGui.GetCursorScreenPos();
        Vector2 center   = top_left + size / 2F;

        bool pressed = ImGui.InvisibleButton(id, size);
        bool hovered = ImGui.IsItemHovered();
        bool active  = ImGui.IsItemActive();

        Vector4 background_color = hovered || active
            ? hover_color
            : FhTheme.COLOR_BACKGROUND;

        ImDrawListPtr draw_list = ImGui.GetWindowDrawList();

        if (hovered || active) {
            draw_list.AddRectFilled(top_left, top_left + size, ImGui.GetColorU32(background_color));
        }

        float icon_extent = MathF.Min(size.X, size.Y) * 0.32F;

        draw_icon(draw_list, center, icon_extent, background_color);

        return pressed;
    }

    private static void _draw_minimize_icon(
        ImDrawListPtr draw_list,
        Vector2 center,
        float extent,
        Vector4 background_color) {
        float thickness = MathF.Max(1F, extent * 0.22F);

        draw_list.AddLine(
            center + new Vector2(-extent, 0F),
            center + new Vector2(extent, 0F),
            ImGui.GetColorU32(FhTheme.COLOR_TEXT),
            thickness);
    }

    private static void _draw_maximize_icon(
        ImDrawListPtr draw_list,
        Vector2 center,
        float extent,
        Vector4 background_color) {
        float   thickness = MathF.Max(1F, extent * 0.22F);
        Vector2 half       = new(extent * 0.72F, extent * 0.72F);

        draw_list.AddRect(
            center - half,
            center + half,
            ImGui.GetColorU32(FhTheme.COLOR_TEXT),
            0F,
            ImDrawFlags.None,
            thickness);
    }

    // Windows' own "restore" glyph: two overlapping square outlines - a back
    // square (upper-right) and a front square (lower-left). The front square
    // paints its own interior in the button's current background color before
    // drawing its outline, so the back square's edge doesn't show through the
    // overlap - the same trick the OS icon itself relies on, just done by hand
    // here since we don't get to draw behind the button for free.
    private static void _draw_restore_icon(
        ImDrawListPtr draw_list,
        Vector2 center,
        float extent,
        Vector4 background_color) {
        float   thickness = MathF.Max(1F, extent * 0.22F);
        Vector2 half       = new(extent * 0.58F, extent * 0.58F);
        Vector2 offset     = new(extent * 0.34F, extent * 0.34F);
        uint    line_color = ImGui.GetColorU32(FhTheme.COLOR_TEXT);

        Vector2 back_center = center + new Vector2(offset.X, -offset.Y);

        draw_list.AddRect(back_center - half, back_center + half, line_color, 0F, ImDrawFlags.None, thickness);

        Vector2 front_center = center - new Vector2(offset.X, -offset.Y);
        Vector2 front_min    = front_center - half;
        Vector2 front_max    = front_center + half;

        draw_list.AddRectFilled(front_min, front_max, ImGui.GetColorU32(background_color));
        draw_list.AddRect(front_min, front_max, line_color, 0F, ImDrawFlags.None, thickness);
    }

    private static void _draw_close_icon(
        ImDrawListPtr draw_list,
        Vector2 center,
        float extent,
        Vector4 background_color) {
        float   thickness = MathF.Max(1F, extent * 0.22F);
        Vector2 half       = new(extent * 0.72F, extent * 0.72F);
        uint    line_color = ImGui.GetColorU32(FhTheme.COLOR_TEXT);

        draw_list.AddLine(center - half, center + half, line_color, thickness);
        draw_list.AddLine(
            center + new Vector2(half.X, -half.Y),
            center + new Vector2(-half.X, half.Y),
            line_color,
            thickness);
    }

    /* [modeled on EEdit's _handle_modal]
     * Popups can't be opened from inside a BeginMenu() scope - see
     * https://github.com/ocornut/imgui/issues/5684#issuecomment-1247928651 - so a
     * menu click just sets `_show_efl_import_dialog`, and this (called outside any
     * menu/window scope, at the top of UI()) is what actually opens and renders it.
     */
    private static void _handle_modals() {
        if (_show_efl_import_dialog) {
            ImGui.OpenPopup("Import EFL Mod");
        }

        if (_show_settings_dialog) {
            ImGui.OpenPopup("Settings");
        }

        _render_efl_import_modal();
        _render_settings_modal();
    }
}
