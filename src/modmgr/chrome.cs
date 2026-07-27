// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* Jobs:
 * - Restore drag-to-move and drag-edge-to-resize on the borderless SDL window
 *   via SDL's per-point hit-test callback, since going borderless removes the
 *   OS's own affordances for both.
 * - Track the menu bar's on-screen rect (from ui_menu.cs) so the hit-test can
 *   tell "empty title bar" apart from "an actual menu/button".
 * - Implement minimize/maximize/close and the quit-requested flag main.cs's
 *   loop polls, standing in for the OS title bar buttons this window no
 *   longer has.
 * - On Windows, opt the window out of DWM's own automatic corner rounding, so
 *   FhTheme.apply()'s WindowRounding is the only rounding actually in effect.
 */

namespace Fahrenheit.Tools.ModManager;

// The mod manager's SDL window is created borderless (see main.cs) so the
// minimize/maximize/close buttons can be drawn into the menu bar instead of the
// OS's own title bar. Going borderless also removes the OS's drag-to-move and
// drag-edge-to-resize affordances, so this class puts them back via SDL's
// per-point hit-test callback: SDL asks "what is this point?" for every point
// under the cursor (on Windows, this is driven by WM_NCHITTEST), and we answer
// with "a resize edge", "empty title bar (draggable)", or "an actual widget,
// leave it to the app".
internal static unsafe class FhWindowChrome {
    // Scaled by FhTheme.UiScale (see its own comment in theme.cs) so the resize
    // grab area stays a comfortable physical size at any display scale, rather
    // than shrinking to an unusably thin sliver on a high-DPI display.
    private static int _resize_border_px => (int)(8F * FhTheme.UiScale);

    private static SDLWindow* _window;

    // Kept alive for the process lifetime: SDL only holds a native function
    // pointer to this delegate's thunk, so if nothing kept the managed delegate
    // object itself alive, the GC could collect it and leave SDL calling into
    // freed memory the next time a hit-test fires.
    private static readonly SDLHitTest _hit_test_delegate = _hit_test;

    // Updated once per frame by ui.cs's _render_main_menu, in screen-space
    // coordinates. The callback below can fire outside of our own frame loop -
    // e.g. while the OS is running its own drag-move modal loop - so it can only
    // ever work from whatever was captured as of the last rendered frame.
    private static Vector2 _menu_cluster_min;
    private static Vector2 _menu_cluster_max;
    private static Vector2 _window_controls_min;
    private static Vector2 _window_controls_max;
    private static float   _titlebar_height;

    internal static bool QuitRequested { get; private set; }

    internal static void install(SDLWindow* window) {
        _window = window;
        SDL.SetWindowHitTest(window, _hit_test_delegate, null);

        if (OperatingSystem.IsWindows()) {
            _disable_os_corner_rounding(window);
        }
    }

    // Windows 11 automatically rounds every top-level window's corners at the
    // DWM compositor level, independent of anything the app itself draws - and
    // independent of, and uncoordinated with, FhTheme.apply()'s own
    // WindowRounding. With both in effect, DWM's rounding (a different radius,
    // clipping rather than filling) cuts a small triangle off each of ImGui's
    // own square-drawn corners, revealing whatever is behind the window there
    // instead of our theme - a stray notch/color mismatch right at the corner.
    // Opting this window out of DWM's own rounding leaves ImGui's the only one
    // in effect, so the corner is whatever FhTheme actually drew.
    private static void _disable_os_corner_rounding(SDLWindow* window) {
        const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
        const int DWMWCP_DONOTROUND              = 1;

        uint  props    = SDL.GetWindowProperties(window);
        void* hwnd_ptr = SDL.GetPointerProperty(props, SDL.SDL_PROP_WINDOW_WIN32_HWND_POINTER, null);

        if (hwnd_ptr == null) {
            return;
        }

        int preference = DWMWCP_DONOTROUND;

        _DwmSetWindowAttribute((nint)hwnd_ptr, DWMWA_WINDOW_CORNER_PREFERENCE, ref preference, sizeof(int));
    }

    [DllImport("dwmapi.dll", EntryPoint = "DwmSetWindowAttribute")]
    private static extern int _DwmSetWindowAttribute(nint hwnd, int attribute, ref int value, int value_size);

    internal static void mark_menu_cluster(Vector2 min, Vector2 max) {
        _menu_cluster_min = min;
        _menu_cluster_max = max;
    }

    internal static void mark_window_controls(Vector2 min, Vector2 max) {
        _window_controls_min = min;
        _window_controls_max = max;
    }

    internal static void set_titlebar_height(float height) {
        _titlebar_height = height;
    }

    // Lets the maximize/restore button (ui_menu.cs) pick which icon to draw -
    // native title bars show a single square when the window can be maximized,
    // and an overlapping-squares "restore" glyph once it already is.
    internal static bool IsMaximized =>
        _window != null && SDL.GetWindowFlags(_window).HasFlag(SDLWindowFlags.Maximized);

    internal static void minimize() {
        if (_window != null) {
            SDL.MinimizeWindow(_window);
        }
    }

    internal static void toggle_maximize() {
        if (_window == null) {
            return;
        }

        if (SDL.GetWindowFlags(_window).HasFlag(SDLWindowFlags.Maximized)) {
            SDL.RestoreWindow(_window);
        }
        else {
            SDL.MaximizeWindow(_window);
        }
    }

    // main.cs's loop checks this each frame; there's no window-close event to
    // hook anymore once the OS's own close button is gone.
    internal static void request_quit() {
        QuitRequested = true;
    }

    // `area` is in window-local coordinates; everything we compare it against
    // (the rects mark_*/set_titlebar_height recorded) came from ImGui's
    // GetCursorScreenPos()/GetItemRectMax(), which are in screen space - so this
    // converts via SDL_GetWindowPosition rather than the other way around.
    private static SDLHitTestResult _hit_test(SDLWindow* win, SDLPoint* area, void* data) {
        int window_x, window_y, width, height;

        SDL.GetWindowPosition(win, &window_x, &window_y);
        SDL.GetWindowSize(win, &width, &height);

        int local_x = area->X;
        int local_y = area->Y;

        // Resize borders win over drag/normal, same as a native title bar: you can
        // always grab the very edge, even where it overlaps other UI.
        int resize_border_px = _resize_border_px;

        bool at_left   = local_x < resize_border_px;
        bool at_right  = local_x >= width  - resize_border_px;
        bool at_top    = local_y < resize_border_px;
        bool at_bottom = local_y >= height - resize_border_px;

        if (at_top    && at_left)  return SDLHitTestResult.ResizeTopleft;
        if (at_top    && at_right) return SDLHitTestResult.ResizeTopright;
        if (at_bottom && at_left)  return SDLHitTestResult.ResizeBottomleft;
        if (at_bottom && at_right) return SDLHitTestResult.ResizeBottomright;
        if (at_left)   return SDLHitTestResult.ResizeLeft;
        if (at_right)  return SDLHitTestResult.ResizeRight;
        if (at_top)    return SDLHitTestResult.ResizeTop;
        if (at_bottom) return SDLHitTestResult.ResizeBottom;

        float screen_x = window_x + local_x;
        float screen_y = window_y + local_y;

        if (screen_y >= window_y + _titlebar_height) {
            return SDLHitTestResult.Normal;
        }

        bool inside_menu_cluster =
            screen_x >= _menu_cluster_min.X && screen_x <= _menu_cluster_max.X
            && screen_y >= _menu_cluster_min.Y && screen_y <= _menu_cluster_max.Y;

        bool inside_window_controls =
            screen_x >= _window_controls_min.X && screen_x <= _window_controls_max.X
            && screen_y >= _window_controls_min.Y && screen_y <= _window_controls_max.Y;

        return inside_menu_cluster || inside_window_controls
            ? SDLHitTestResult.Normal
            : SDLHitTestResult.Draggable;
    }
}
