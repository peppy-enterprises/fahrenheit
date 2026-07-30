// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit;

public unsafe class FhImGuiHelper {
    public void set_next_align(ReadOnlySpan<byte> label, float t, float padding = 0F) {
        float size      = ImGui.CalcTextSize(label).X + padding;
        float available = ImGui.GetContentRegionAvail().X;
        float offset    = (available - size) * t;

        if (offset > 0) {
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + offset);
        }
    }

    public enum FhImGuiThemes {
        CLASSIC_FF = 0,
    }

    private void _init_fonts() {
        ImGuiIOPtr io      = ImGui.GetIO();
        string     fontdir = Path.Join(FhEnvironment.Finder.Binaries.FullName, "resources", "fonts");

        FONT_DEFAULT = io.Fonts.AddFontFromFileTTF(
            Path.Join(fontdir, "NotoSans-VariableFont_wdth,wght.ttf"),
            20f,
            null,
            io.Fonts.GetGlyphRangesDefault()
        );
    }

    private static void _init_style(FhImGuiThemes? theme = null) {
        // Fahrenheit style from ImThemes
        ImGuiStylePtr style = ImGui.GetStyle();

        style.Alpha                          = 1.0f;
        style.DisabledAlpha                  = 0.5f;
        style.WindowPadding                  = new Vector2(11.0f, 11.1f);
        style.WindowRounding                 = 3.0f;
        style.WindowBorderSize               = 1.0f;
        style.WindowMinSize                  = new Vector2(20.0f, 20.0f);
        style.WindowTitleAlign               = new Vector2(0.5f, 0.5f);
        style.WindowMenuButtonPosition       = ImGuiDir.Right;
        style.ChildRounding                  = 5.0f;
        style.ChildBorderSize                = 1.0f;
        style.PopupRounding                  = 5.0f;
        style.PopupBorderSize                = 1.0f;
        style.FramePadding                   = new Vector2(4.0f, 3.0f);
        style.FrameRounding                  = 3.0f;
        style.FrameBorderSize                = 0.0f;
        style.ItemSpacing                    = new Vector2(5.0f, 7.0f);
        style.ItemInnerSpacing               = new Vector2(4.0f, 4.0f);
        style.CellPadding                    = new Vector2(5.0f, 3.0f);
        style.IndentSpacing                  = 16.0f;
        style.ColumnsMinSpacing              = 10.0f;
        style.ScrollbarSize                  = 13.0f;
        style.ScrollbarRounding              = 3.0f;
        style.GrabMinSize                    = 9.0f;
        style.GrabRounding                   = 3.0f;
        style.TabRounding                    = 3.0f;
        style.TabBorderSize                  = 0.0f;
        style.TabCloseButtonMinWidthSelected = 0.0f;
        style.ColorButtonPosition            = ImGuiDir.Left;
        style.ButtonTextAlign                = new Vector2(0.5f, 0.5f);
        style.SelectableTextAlign            = new Vector2(0.0f, 0.5f);

        switch (theme) {
            case FhImGuiThemes.CLASSIC_FF:
                _set_colors_ff_classic();
                break;
            default:
                throw new NotImplementedException($"Unknown theme: {theme}");
        }
    }

    private static void _set_colors_ff_classic() {
        ImGuiStylePtr style = ImGui.GetStyle();

        style.Colors[(int)ImGuiCol.Text]                  = new Vector4(1.0f  , 1.0f  , 1.0f  , 1.0f  );
        style.Colors[(int)ImGuiCol.TextDisabled]          = new Vector4(0.729f, 0.729f, 0.729f, 1.0f  );
        style.Colors[(int)ImGuiCol.WindowBg]              = new Vector4(0.114f, 0.196f, 0.412f, 1.0f  );
        style.Colors[(int)ImGuiCol.ChildBg]               = new Vector4(0.227f, 0.427f, 0.604f, 0.108f);
        style.Colors[(int)ImGuiCol.PopupBg]               = new Vector4(0.118f, 0.169f, 0.290f, 0.953f);
        style.Colors[(int)ImGuiCol.Border]                = new Vector4(0.153f, 0.153f, 0.153f, 0.349f);
        style.Colors[(int)ImGuiCol.BorderShadow]          = new Vector4(0.0f  , 0.0f  , 0.0f  , 0.129f);
        style.Colors[(int)ImGuiCol.FrameBg]               = new Vector4(0.196f, 0.263f, 0.4f  , 1.0f  );
        style.Colors[(int)ImGuiCol.FrameBgHovered]        = new Vector4(0.242f, 0.309f, 0.454f, 1.0f  );
        style.Colors[(int)ImGuiCol.FrameBgActive]         = new Vector4(0.303f, 0.384f, 0.569f, 1.0f  );
        style.Colors[(int)ImGuiCol.TitleBg]               = new Vector4(0.252f, 0.393f, 0.714f, 1.0f  );
        style.Colors[(int)ImGuiCol.TitleBgActive]         = new Vector4(0.252f, 0.405f, 0.753f, 1.0f  );
        style.Colors[(int)ImGuiCol.TitleBgCollapsed]      = new Vector4(0.253f, 0.383f, 0.678f, 0.694f);
        style.Colors[(int)ImGuiCol.MenuBarBg]             = new Vector4(0.159f, 0.294f, 0.635f, 1.0f  );
        style.Colors[(int)ImGuiCol.ScrollbarBg]           = new Vector4(0.131f, 0.152f, 0.282f, 0.196f);
        style.Colors[(int)ImGuiCol.ScrollbarGrab]         = new Vector4(0.463f, 0.569f, 0.765f, 1.0f  );
        style.Colors[(int)ImGuiCol.ScrollbarGrabHovered]  = new Vector4(0.542f, 0.648f, 0.843f, 1.0f  );
        style.Colors[(int)ImGuiCol.ScrollbarGrabActive]   = new Vector4(0.543f, 0.69f , 0.961f, 1.0f  );
        style.Colors[(int)ImGuiCol.CheckMark]             = new Vector4(0.674f, 0.758f, 0.914f, 1.0f  );
        style.Colors[(int)ImGuiCol.SliderGrab]            = new Vector4(0.607f, 0.684f, 0.824f, 1.0f  );
        style.Colors[(int)ImGuiCol.SliderGrabActive]      = new Vector4(0.640f, 0.733f, 0.902f, 1.0f  );
        style.Colors[(int)ImGuiCol.Button]                = new Vector4(0.140f, 0.307f, 0.557f, 1.0f  );
        style.Colors[(int)ImGuiCol.ButtonHovered]         = new Vector4(0.190f, 0.376f, 0.655f, 1.0f  );
        style.Colors[(int)ImGuiCol.ButtonActive]          = new Vector4(0.184f, 0.404f, 0.733f, 1.0f  );
        style.Colors[(int)ImGuiCol.Header]                = new Vector4(0.183f, 0.375f, 0.765f, 1.0f  );
        style.Colors[(int)ImGuiCol.HeaderHovered]         = new Vector4(0.240f, 0.445f, 0.863f, 1.0f  );
        style.Colors[(int)ImGuiCol.HeaderActive]          = new Vector4(0.225f, 0.461f, 0.941f, 1.0f  );
        style.Colors[(int)ImGuiCol.Separator]             = new Vector4(0.417f, 0.566f, 0.902f, 1.0f  );
        style.Colors[(int)ImGuiCol.SeparatorHovered]      = new Vector4(0.753f, 0.753f, 0.753f, 1.0f  );
        style.Colors[(int)ImGuiCol.SeparatorActive]       = new Vector4(1.0f  , 1.0f  , 1.0f  , 1.0f  );
        style.Colors[(int)ImGuiCol.ResizeGrip]            = new Vector4(0.535f, 0.659f, 0.920f, 0.514f);
        style.Colors[(int)ImGuiCol.ResizeGripHovered]     = new Vector4(0.633f, 0.741f, 0.961f, 0.784f);
        style.Colors[(int)ImGuiCol.ResizeGripActive]      = new Vector4(0.541f, 0.692f, 1.0f  , 0.784f);
        style.Colors[(int)ImGuiCol.Tab]                   = new Vector4(0.25f , 0.344f, 0.580f, 0.863f);
        style.Colors[(int)ImGuiCol.TabHovered]            = new Vector4(0.319f, 0.421f, 0.678f, 0.863f);
        style.Colors[(int)ImGuiCol.TabSelected]           = new Vector4(0.335f, 0.46f , 0.776f, 1.0f  );
        style.Colors[(int)ImGuiCol.TabDimmed]             = new Vector4(0.098f, 0.232f, 0.408f, 0.785f);
        style.Colors[(int)ImGuiCol.TabDimmedSelected]     = new Vector4(0.165f, 0.319f, 0.519f, 1.0f  );
        style.Colors[(int)ImGuiCol.DockingPreview]        = new Vector4(0.831f, 0.684f, 0.564f, 0.345f);
        style.Colors[(int)ImGuiCol.DockingEmptyBg]        = new Vector4(0.2f  , 0.2f  , 0.2f  , 1.0f  );
        style.Colors[(int)ImGuiCol.PlotLines]             = new Vector4(0.8f  , 0.863f, 0.984f, 1.0f  );
        style.Colors[(int)ImGuiCol.PlotLinesHovered]      = new Vector4(1.0f  , 0.965f, 0.648f, 1.0f  );
        style.Colors[(int)ImGuiCol.PlotHistogram]         = new Vector4(0.789f, 0.843f, 0.953f, 0.85f );
        style.Colors[(int)ImGuiCol.PlotHistogramHovered]  = new Vector4(0.974f, 0.983f, 1.0f  , 0.784f);
        style.Colors[(int)ImGuiCol.TableHeaderBg]         = new Vector4(0.180f, 0.361f, 0.729f, 1.0f  );
        style.Colors[(int)ImGuiCol.TableBorderStrong]     = new Vector4(0.565f, 0.663f, 0.863f, 0.776f);
        style.Colors[(int)ImGuiCol.TableBorderLight]      = new Vector4(0.452f, 0.562f, 0.784f, 0.502f);
        style.Colors[(int)ImGuiCol.TableRowBg]            = new Vector4(0.508f, 0.548f, 0.631f, 0.031f);
        style.Colors[(int)ImGuiCol.TableRowBgAlt]         = new Vector4(0.657f, 0.686f, 0.745f, 0.11f );
        style.Colors[(int)ImGuiCol.TextLink]              = new Vector4(0.98f,  0.683f, 0.26f , 1.0f  );
        style.Colors[(int)ImGuiCol.TextSelectedBg]        = new Vector4(0.570f, 0.744f, 0.948f, 0.485f);
        style.Colors[(int)ImGuiCol.DragDropTarget]        = new Vector4(0.910f, 0.79f , 0.574f, 0.648f);
        style.Colors[(int)ImGuiCol.NavCursor]             = new Vector4(0.991f, 0.605f, 0.362f, 1.0f  );
        style.Colors[(int)ImGuiCol.NavWindowingHighlight] = new Vector4(1.0f  , 1.0f  , 1.0f  , 0.7f  );
        style.Colors[(int)ImGuiCol.NavWindowingDimBg]     = new Vector4(0.8f  , 0.8f  , 0.8f  , 0.2f  );
        style.Colors[(int)ImGuiCol.ModalWindowDimBg]      = new Vector4(0.8f  , 0.8f  , 0.8f  , 0.35f );
    }

    public ImGuiWindowFlags WINDOW_FLAGS_FULLSCREEN =
            ImGuiWindowFlags.NoDecoration
          | ImGuiWindowFlags.NoMove
          | ImGuiWindowFlags.NoResize
          | ImGuiWindowFlags.NoBringToFrontOnFocus
          | ImGuiWindowFlags.NoNavFocus;

    // Fonts for standardized style across Fahrenheit
    //TODO: Add more fonts
    public ImFontPtr FONT_DEFAULT { get; private set; }

    //TODO: Add more constants for standardized style

    /// <summary>
    /// Initialize values that require ImGui to be running. Called by Runtime.
    /// </summary>
    internal void init(FhImGuiThemes? theme = null) {
        _init_fonts();
        _init_style(theme ?? FhImGuiThemes.CLASSIC_FF);
    }
}
