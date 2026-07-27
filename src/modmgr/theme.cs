// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* Jobs:
 * - FhThemeColor: a JSON-friendly RGBA color used to persist theme overrides
 *   in fhmodmgr.json (a plain System.Numerics.Vector4 doesn't round-trip
 *   through the settings store's JSON options).
 * - FhTheme: the single source of truth for the app's palette and the
 *   derived ImGui style values built from it - the Settings modal's theme
 *   section (ui_settings_modal.cs) is the only other place allowed to write
 *   the base COLOR_* fields.
 * - FhElements: small reusable styled ImGui widgets (primary/secondary
 *   buttons, a hand-drawn valid/invalid status icon) that pull their colors
 *   from FhTheme, so widget code never has to push/pop a one-off color by
 *   hand. The window control buttons in ui_menu.cs are hand-drawn directly
 *   instead - see the comment there for why.
 *   
 *   TODO: consider supporting theme "profiles" (light/dark modes, or user-saved palettes) in the future.
 */

namespace Fahrenheit.Tools.ModManager;

// A JSON-serializable RGBA color, used to persist user theme customizations in
// fhmodmgr.json. System.Numerics.Vector4 isn't a good fit for that directly: its
// components are public fields, and FhModManagerSettingsStore's JsonSerializerOptions
// don't set IncludeFields, so a Vector4 would round-trip as an empty object.
internal sealed record FhThemeColor(float R, float G, float B, float A) {
    internal Vector4 to_vector4() {
        return new Vector4(R, G, B, A);
    }

    internal static FhThemeColor from_vector4(Vector4 color) {
        return new FhThemeColor(color.X, color.Y, color.Z, color.W);
    }
}

// A single source of truth for the mod manager's look: a named color palette and
// the ImGui style values derived from it. Widget-level code (ui.cs) should never
// write a raw `new Vector4(...)` color or push one-off style vars; it should either
// rely on the global style this sets up, or reach for a named FhTheme color, or a
// FhElements helper below. That way the whole app stays visually consistent, and a
// palette tweak - including a user's own, via the Settings > Theme section - only
// has to happen in one place.
internal static class FhTheme {
    // The display's content scale (e.g. 1.5 at 150% Windows scaling), set once
    // by main.cs before the first apply() call. Every layout number apply()
    // writes is multiplied by this, so a HiDPI display gets a proportionally
    // bigger UI instead of the same raw pixel counts a 100%-scale display gets -
    // deliberately baked into apply() itself, rather than a one-time external
    // ImGui style.ScaleAllSizes() call, since apply() runs again every time a
    // theme color changes (see ui_settings_modal.cs) or the app first loads
    // saved settings (see FhModManagerUI's static constructor); a one-time
    // external bake would just get overwritten back to the raw, unscaled
    // numbers by either of those - which is exactly what used to happen.
    internal static float UiScale = 1F;

    // Drawn from the Fahrenheit airship mark (assets/fh_base_256.png): a lavender
    // hull with warm gold trim, rendered here as a dark UI rather than the
    // storybook/postcard look of the project's marketing art. These are the values
    // "Reset to Default" restores.
    internal static readonly Vector4 DEFAULT_ACCENT           = new(0.56F, 0.47F, 0.86F, 1.00F);
    internal static readonly Vector4 DEFAULT_SUCCESS          = new(0.45F, 0.85F, 0.55F, 1.00F);
    internal static readonly Vector4 DEFAULT_ERROR            = new(0.95F, 0.40F, 0.40F, 1.00F);
    internal static readonly Vector4 DEFAULT_WARNING          = new(0.95F, 0.75F, 0.30F, 1.00F);
    internal static readonly Vector4 DEFAULT_BACKGROUND       = new(0.09F, 0.09F, 0.11F, 1.00F);
    internal static readonly Vector4 DEFAULT_TEXT             = new(0.93F, 0.93F, 0.95F, 1.00F);
    internal static readonly Vector4 DEFAULT_TEXT_MUTED       = new(0.58F, 0.58F, 0.64F, 1.00F);
    internal static readonly Vector4 DEFAULT_FRAME_BACKGROUND = new(0.16F, 0.16F, 0.20F, 1.00F);
    internal static readonly Vector4 DEFAULT_TITLE_BAR        = new(0.14F, 0.13F, 0.19F, 1.00F);

    // The user-customizable base colors (see the "Theme" section of the Settings
    // modal). Hover/active states, the secondary-button surface, separators, etc.
    // are derived from these in apply(), so editing these few colors is enough to
    // reshade the whole app coherently. Every color a widget can actually show
    // lives here or is derived from something here - nothing is hardcoded past this
    // list, which is what keeps e.g. a text box's background or "subtitle"-style
    // muted text from getting stuck looking like the old default after a re-theme.
    internal static Vector4 COLOR_ACCENT           = DEFAULT_ACCENT;
    internal static Vector4 COLOR_SUCCESS          = DEFAULT_SUCCESS;
    internal static Vector4 COLOR_ERROR            = DEFAULT_ERROR;
    internal static Vector4 COLOR_WARNING          = DEFAULT_WARNING;
    internal static Vector4 COLOR_BACKGROUND       = DEFAULT_BACKGROUND;
    internal static Vector4 COLOR_TEXT             = DEFAULT_TEXT;
    internal static Vector4 COLOR_TEXT_MUTED       = DEFAULT_TEXT_MUTED;
    internal static Vector4 COLOR_FRAME_BACKGROUND = DEFAULT_FRAME_BACKGROUND;

    // Shared by the app's own menu bar (which stands in for the OS title bar -
    // see FhWindowChrome) and every popup modal's native title bar (Settings,
    // Import EFL Mod), so the two read as the same kind of chrome rather than
    // two different shades that happen to sit near each other.
    internal static Vector4 COLOR_TITLE_BAR = DEFAULT_TITLE_BAR;

    // Derived colors, recomputed by apply(). Widget code may read these (e.g.
    // FhElements' secondary button uses COLOR_SURFACE*), but should never write them.
    internal static Vector4 COLOR_ACCENT_HOVERED         { get; private set; }
    internal static Vector4 COLOR_ACCENT_ACTIVE          { get; private set; }
    internal static Vector4 COLOR_SURFACE                { get; private set; }
    internal static Vector4 COLOR_SURFACE_HOVERED        { get; private set; }
    internal static Vector4 COLOR_SURFACE_ACTIVE         { get; private set; }
    internal static Vector4 COLOR_BG_RAISED              { get; private set; }
    internal static Vector4 COLOR_FRAME_BACKGROUND_HOVER { get; private set; }
    internal static Vector4 COLOR_FRAME_BACKGROUND_ACTIVE { get; private set; }

    // Copies any saved color overrides into the live palette; anything the user
    // hasn't customized keeps its DEFAULT_* value. Called once from FhModManagerUI's
    // static constructor, before the first frame is drawn.
    internal static void load_from_settings(FhModManagerSettings settings) {
        COLOR_ACCENT           = settings.AccentColor?.to_vector4()         ?? DEFAULT_ACCENT;
        COLOR_SUCCESS          = settings.SuccessColor?.to_vector4()        ?? DEFAULT_SUCCESS;
        COLOR_ERROR            = settings.ErrorColor?.to_vector4()          ?? DEFAULT_ERROR;
        COLOR_WARNING          = settings.WarningColor?.to_vector4()        ?? DEFAULT_WARNING;
        COLOR_BACKGROUND       = settings.BackgroundColor?.to_vector4()     ?? DEFAULT_BACKGROUND;
        COLOR_TEXT             = settings.TextColor?.to_vector4()           ?? DEFAULT_TEXT;
        COLOR_TEXT_MUTED       = settings.TextMutedColor?.to_vector4()      ?? DEFAULT_TEXT_MUTED;
        COLOR_FRAME_BACKGROUND = settings.FrameBackgroundColor?.to_vector4() ?? DEFAULT_FRAME_BACKGROUND;
        COLOR_TITLE_BAR        = settings.TitleBarColor?.to_vector4()       ?? DEFAULT_TITLE_BAR;
    }

    // Restores the built-in palette. Doesn't touch settings persistence - that's the
    // Settings modal's job (clearing the saved overrides), so a future rebuild of
    // this tool that changes the defaults doesn't leave someone's "reset" pinned to
    // the old values.
    internal static void reset_to_default() {
        COLOR_ACCENT           = DEFAULT_ACCENT;
        COLOR_SUCCESS          = DEFAULT_SUCCESS;
        COLOR_ERROR            = DEFAULT_ERROR;
        COLOR_WARNING          = DEFAULT_WARNING;
        COLOR_BACKGROUND       = DEFAULT_BACKGROUND;
        COLOR_TEXT             = DEFAULT_TEXT;
        COLOR_TEXT_MUTED       = DEFAULT_TEXT_MUTED;
        COLOR_FRAME_BACKGROUND = DEFAULT_FRAME_BACKGROUND;
        COLOR_TITLE_BAR        = DEFAULT_TITLE_BAR;
    }

    // Applies the current palette to the ImGui style. Called from main.cs at
    // startup (right after ImGui.StyleColorsDark(), which this refines rather than
    // replaces), and again any time a color changes in the Settings modal.
    internal static void apply() {
        COLOR_ACCENT_HOVERED = _lighten(COLOR_ACCENT, 0.08F);
        COLOR_ACCENT_ACTIVE  = _darken(COLOR_ACCENT, 0.10F);

        // Derived from the background (rather than a fixed gray) so secondary
        // buttons stay visually related to a customized background instead of
        // clashing with it.
        COLOR_SURFACE         = _lighten(COLOR_BACKGROUND, 0.11F);
        COLOR_SURFACE_HOVERED = _lighten(COLOR_BACKGROUND, 0.17F);
        COLOR_SURFACE_ACTIVE  = _lighten(COLOR_BACKGROUND, 0.22F);
        COLOR_BG_RAISED       = _lighten(COLOR_BACKGROUND, 0.03F);

        COLOR_FRAME_BACKGROUND_HOVER  = _lighten(COLOR_FRAME_BACKGROUND, 0.06F);
        COLOR_FRAME_BACKGROUND_ACTIVE = _lighten(COLOR_FRAME_BACKGROUND, 0.10F);

        ImGuiStylePtr style = ImGui.GetStyle();

        // Deliberately 0, unlike every other *Rounding below: the main window's
        // own WindowBg fill is the one shape in this app that sits directly
        // against the raw SDL window edge, with nothing else of ours behind it
        // to fall back on. A rounded main window leaves small corner triangles
        // that ImGui's own fill doesn't paint - what shows there instead depends
        // on the OS compositor and the GL clear color, neither of which this app
        // fully controls, and in practice it's shown up as the background color
        // visibly poking out past the window's own rounded/bordered edge. A
        // square outer window has no such gap: every pixel in it is something
        // ImGui explicitly draws, full stop. Every rounded element elsewhere
        // (buttons, panels, the Settings/EFL modals, frames) doesn't have this
        // problem, since none of them ever sit directly against that raw edge.
        style.WindowRounding    = 0F;
        style.ChildRounding     = 6F * UiScale;
        style.PopupRounding     = 8F * UiScale;
        style.FrameRounding     = 4F * UiScale;
        style.GrabRounding      = 4F * UiScale;
        style.ScrollbarRounding = 8F * UiScale;
        style.TabRounding       = 4F * UiScale;

        style.WindowPadding   = new Vector2(20F, 18F) * UiScale;
        style.FramePadding    = new Vector2(8F, 5F) * UiScale;
        style.ItemSpacing     = new Vector2(8F, 6F) * UiScale;
        style.PopupBorderSize = 1F * UiScale;

        style.Colors[(int)ImGuiCol.WindowBg] = COLOR_BACKGROUND;
        style.Colors[(int)ImGuiCol.PopupBg]  = COLOR_BG_RAISED;
        style.Colors[(int)ImGuiCol.ChildBg]  = new Vector4(0F, 0F, 0F, 0F);
        style.Colors[(int)ImGuiCol.Border]   = new Vector4(0.27F, 0.26F, 0.32F, 0.60F);

        style.Colors[(int)ImGuiCol.FrameBg]        = COLOR_FRAME_BACKGROUND;
        style.Colors[(int)ImGuiCol.FrameBgHovered] = COLOR_FRAME_BACKGROUND_HOVER;
        style.Colors[(int)ImGuiCol.FrameBgActive]  = COLOR_FRAME_BACKGROUND_ACTIVE;

        style.Colors[(int)ImGuiCol.TitleBg]       = COLOR_TITLE_BAR;
        style.Colors[(int)ImGuiCol.TitleBgActive] = COLOR_TITLE_BAR;
        style.Colors[(int)ImGuiCol.MenuBarBg]     = COLOR_TITLE_BAR;

        style.Colors[(int)ImGuiCol.Button]        = COLOR_ACCENT;
        style.Colors[(int)ImGuiCol.ButtonHovered] = COLOR_ACCENT_HOVERED;
        style.Colors[(int)ImGuiCol.ButtonActive]  = COLOR_ACCENT_ACTIVE;

        style.Colors[(int)ImGuiCol.CheckMark]        = COLOR_ACCENT_HOVERED;
        style.Colors[(int)ImGuiCol.SliderGrab]       = COLOR_ACCENT;
        style.Colors[(int)ImGuiCol.SliderGrabActive] = COLOR_ACCENT_ACTIVE;

        style.Colors[(int)ImGuiCol.Header]        = new Vector4(COLOR_ACCENT.X, COLOR_ACCENT.Y, COLOR_ACCENT.Z, 0.45F);
        style.Colors[(int)ImGuiCol.HeaderHovered] = new Vector4(COLOR_ACCENT.X, COLOR_ACCENT.Y, COLOR_ACCENT.Z, 0.65F);
        style.Colors[(int)ImGuiCol.HeaderActive]  = new Vector4(COLOR_ACCENT.X, COLOR_ACCENT.Y, COLOR_ACCENT.Z, 0.85F);

        style.Colors[(int)ImGuiCol.Separator]        = new Vector4(0.30F, 0.29F, 0.36F, 1.00F);
        style.Colors[(int)ImGuiCol.SeparatorHovered] = COLOR_ACCENT_HOVERED;
        style.Colors[(int)ImGuiCol.SeparatorActive]  = COLOR_ACCENT_ACTIVE;

        // Used by the mod list's row table (see ui.cs's _render_mod_panel) - tied
        // to the same tone as Separator above rather than StyleColorsDark's
        // default (bluish-gray), so the table's row dividers don't clash with the
        // rest of the palette.
        style.Colors[(int)ImGuiCol.TableBorderLight]  = new Vector4(0.30F, 0.29F, 0.36F, 0.60F);
        style.Colors[(int)ImGuiCol.TableBorderStrong] = new Vector4(0.30F, 0.29F, 0.36F, 0.90F);
        style.Colors[(int)ImGuiCol.TableRowBg]        = new Vector4(0F, 0F, 0F, 0F);
        style.Colors[(int)ImGuiCol.TableRowBgAlt]     = new Vector4(COLOR_TEXT.X, COLOR_TEXT.Y, COLOR_TEXT.Z, 0.03F);

        style.Colors[(int)ImGuiCol.ScrollbarBg]          = COLOR_BACKGROUND;
        style.Colors[(int)ImGuiCol.ScrollbarGrab]        = new Vector4(0.28F, 0.27F, 0.34F, 1.00F);
        style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.37F, 0.35F, 0.45F, 1.00F);
        style.Colors[(int)ImGuiCol.ScrollbarGrabActive]  = COLOR_ACCENT;

        style.Colors[(int)ImGuiCol.Text]         = COLOR_TEXT;
        style.Colors[(int)ImGuiCol.TextDisabled] = COLOR_TEXT_MUTED;
    }

    private static Vector4 _lighten(Vector4 color, float amount) {
        return new Vector4(
            Math.Clamp(color.X + amount, 0F, 1F),
            Math.Clamp(color.Y + amount, 0F, 1F),
            Math.Clamp(color.Z + amount, 0F, 1F),
            color.W);
    }

    private static Vector4 _darken(Vector4 color, float amount) {
        return _lighten(color, -amount);
    }
}

// Small, reusable ImGui widgets that carry FhTheme's styling automatically, so call
// sites don't each have to remember which color means "primary action" and push/pop
// it by hand.
internal static class FhElements {
    // The default look (see FhTheme.apply's Button/ButtonHovered/ButtonActive) - use
    // for the one action on a row/dialog that the user is most likely to want, e.g.
    // "Save location", "Import".
    internal static bool button_primary(string label, Vector2 size = default) {
        return ImGui.Button(label, size);
    }

    // A small green check (valid) or red X (invalid) glyph, e.g. next to a
    // location the Settings modal is showing the validity of. Hand-drawn via
    // the draw list rather than a "✓"/"✗" character: the loaded font only
    // covers Basic Latin + Latin-1 Supplement (see main.cs's
    // io.Fonts.AddFontFromFileTTF call), so either glyph would just render as
    // a blank box. Hovering shows `tooltip` if one is given - pass null/empty
    // for a valid location that doesn't need explaining.
    internal static void status_icon(bool is_valid, string? tooltip) {
        float extent = ImGui.GetTextLineHeight();
        Vector2 size = new(extent, extent);

        Vector2 top_left = ImGui.GetCursorScreenPos();
        Vector2 center    = top_left + (size / 2F);

        ImGui.Dummy(size);

        ImDrawListPtr draw_list  = ImGui.GetWindowDrawList();
        uint          color      = ImGui.GetColorU32(is_valid ? FhTheme.COLOR_SUCCESS : FhTheme.COLOR_ERROR);
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

    // A muted variant for secondary actions that shouldn't visually compete with a
    // primary button on the same row/dialog, e.g. "Browse", "Cancel", "Close".
    internal static bool button_secondary(string label, Vector2 size = default) {
        return _button_with_colors(
            label,
            size,
            FhTheme.COLOR_SURFACE,
            FhTheme.COLOR_SURFACE_HOVERED,
            FhTheme.COLOR_SURFACE_ACTIVE);
    }

    // Shared by button_secondary: swaps ImGuiCol.Button/Hovered/Active for the
    // duration of one ImGui.Button() call so each variant only has to name its
    // three colors, not repeat the push/draw/pop dance.
    private static bool _button_with_colors(
        string label,
        Vector2 size,
        Vector4 normal_color,
        Vector4 hovered_color,
        Vector4 active_color) {
        ImGui.PushStyleColor(ImGuiCol.Button, normal_color);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, hovered_color);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, active_color);

        bool pressed = ImGui.Button(label, size);

        ImGui.PopStyleColor(3);

        return pressed;
    }
}
