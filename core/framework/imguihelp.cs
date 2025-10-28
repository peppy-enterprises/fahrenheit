// SPDX-License-Identifier: MIT

using Hexa.NET.ImGui;

namespace Fahrenheit.Core;

public unsafe class FhImGuiHelper {

    private void _init_fonts() {
        ImGuiIOPtr io      = ImGui.GetIO();
        string     fontdir = Path.Join(FhInternal.PathFinder.Binaries.Path, "resources", "fonts");

        FONT_DEFAULT = io.Fonts.AddFontFromFileTTF(
            Path.Join(fontdir, "NotoSans-VariableFont_wdth,wght.ttf"),
            20f,
            null,
            io.Fonts.GetGlyphRangesDefault()
        );
    }

    private static void _init_style() {
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

        style.Colors[(int)ImGuiCol.Text]                  = new Vector4(1.0f,         1.0f,        1.0f,        1.0f);
        style.Colors[(int)ImGuiCol.TextDisabled]          = new Vector4(0.832618f,    0.83260965f, 0.83260965f, 1.0f);
        style.Colors[(int)ImGuiCol.WindowBg]              = new Vector4(0.11372549f,  0.19607843f, 0.4117647f,  1.0f);
        style.Colors[(int)ImGuiCol.ChildBg]               = new Vector4(0.22745098f,  0.42745098f, 0.6039216f,  0.4549356f);
        style.Colors[(int)ImGuiCol.PopupBg]               = new Vector4(0.11303344f,  0.1973811f,  0.4117647f,  0.90128756f);
        style.Colors[(int)ImGuiCol.Border]                = new Vector4(0.28235295f,  0.41568628f, 0.64705884f, 0.78431374f);
        style.Colors[(int)ImGuiCol.BorderShadow]          = new Vector4(0.16078432f,  0.25490198f, 0.41960785f, 0.5019608f);
        style.Colors[(int)ImGuiCol.FrameBg]               = new Vector4(0.08627451f,  0.33333334f, 0.6392157f,  0.78431374f);
        style.Colors[(int)ImGuiCol.FrameBgHovered]        = new Vector4(0.22745098f,  0.44313726f, 0.7137255f,  0.78431374f);
        style.Colors[(int)ImGuiCol.FrameBgActive]         = new Vector4(0.21568628f,  0.5019608f,  0.8627451f,  0.78431374f);
        style.Colors[(int)ImGuiCol.TitleBg]               = new Vector4(0.085081704f, 0.33149344f, 0.639485f,   1.0f);
        style.Colors[(int)ImGuiCol.TitleBgActive]         = new Vector4(0.11764706f,  0.41568628f, 0.7882353f,  1.0f);
        style.Colors[(int)ImGuiCol.TitleBgCollapsed]      = new Vector4(0.15686275f,  0.28627452f, 0.47843137f, 0.695279f);
        style.Colors[(int)ImGuiCol.MenuBarBg]             = new Vector4(0.20663485f,  0.38567176f, 0.60944206f, 1.0f);
        style.Colors[(int)ImGuiCol.ScrollbarBg]           = new Vector4(0.1312973f,   0.15301669f, 0.28326178f, 0.19742489f);
        style.Colors[(int)ImGuiCol.ScrollbarGrab]         = new Vector4(0.45882353f,  0.56078434f, 0.7607843f,  1.0f);
        style.Colors[(int)ImGuiCol.ScrollbarGrabHovered]  = new Vector4(0.6529499f,   0.7382615f,  0.9055794f,  1.0f);
        style.Colors[(int)ImGuiCol.ScrollbarGrabActive]   = new Vector4(0.8014515f,   0.86270785f, 0.9828326f,  1.0f);
        style.Colors[(int)ImGuiCol.CheckMark]             = new Vector4(0.8f,         0.8627451f,  0.9843137f,  1.0f);
        style.Colors[(int)ImGuiCol.SliderGrab]            = new Vector4(0.45882353f,  0.56078434f, 0.7607843f,  1.0f);
        style.Colors[(int)ImGuiCol.SliderGrabActive]      = new Vector4(0.8f,         0.8627451f,  0.9843137f,  1.0f);
        style.Colors[(int)ImGuiCol.Button]                = new Vector4(0.25058484f,  0.48189464f, 0.76824033f, 0.78431374f);
        style.Colors[(int)ImGuiCol.ButtonHovered]         = new Vector4(0.3592256f,   0.5804144f,  0.8540772f,  0.78431374f);
        style.Colors[(int)ImGuiCol.ButtonActive]          = new Vector4(0.558161f,    0.7268827f,  0.93562233f, 0.78431374f);
        style.Colors[(int)ImGuiCol.Header]                = new Vector4(0.14905414f,  0.31066072f, 0.5107296f,  1.0f);
        style.Colors[(int)ImGuiCol.HeaderHovered]         = new Vector4(0.08627451f,  0.33333334f, 0.6392157f,  1.0f);
        style.Colors[(int)ImGuiCol.HeaderActive]          = new Vector4(0.11764706f,  0.41568628f, 0.7882353f,  1.0f);
        style.Colors[(int)ImGuiCol.Separator]             = new Vector4(0.18039216f,  0.26666668f, 0.5019608f,  1.0f);
        style.Colors[(int)ImGuiCol.SeparatorHovered]      = new Vector4(0.29564f,     0.38905093f, 0.64377683f, 1.0f);
        style.Colors[(int)ImGuiCol.SeparatorActive]       = new Vector4(0.39455506f,  0.52360123f, 0.8755365f,  1.0f);
        style.Colors[(int)ImGuiCol.ResizeGrip]            = new Vector4(0.45882353f,  0.56078434f, 0.7607843f,  0.7854077f);
        style.Colors[(int)ImGuiCol.ResizeGripHovered]     = new Vector4(0.654902f,    0.7372549f,  0.90588236f, 0.78431374f);
        style.Colors[(int)ImGuiCol.ResizeGripActive]      = new Vector4(0.8f,         0.8627451f,  0.9843137f,  0.78431374f);
        style.Colors[(int)ImGuiCol.Tab]                   = new Vector4(0.25198478f,  0.42469925f, 0.65236056f, 0.7854077f);
        style.Colors[(int)ImGuiCol.TabHovered]            = new Vector4(0.32156864f,  0.5764706f,  0.8745098f,  0.78431374f);
        style.Colors[(int)ImGuiCol.TabSelected]           = new Vector4(0.27880418f,  0.48808578f, 0.7553648f,  1.0f);
        style.Colors[(int)ImGuiCol.TabDimmed]             = new Vector4(0.09799407f,  0.23222034f, 0.40772533f, 0.7854077f);
        style.Colors[(int)ImGuiCol.TabDimmedSelected]     = new Vector4(0.16493213f,  0.31888875f, 0.51931334f, 1.0f);
        style.Colors[(int)ImGuiCol.PlotLines]             = new Vector4(0.8f,         0.8627451f,  0.9843137f,  1.0f);
        style.Colors[(int)ImGuiCol.PlotLinesHovered]      = new Vector4(1.0f,         0.9652587f,  0.64806867f, 1.0f);
        style.Colors[(int)ImGuiCol.PlotHistogram]         = new Vector4(0.78922063f,  0.8428954f,  0.9527897f,  0.8497854f);
        style.Colors[(int)ImGuiCol.PlotHistogramHovered]  = new Vector4(0.97424895f,  0.98301554f, 1.0f,        0.78431374f);
        style.Colors[(int)ImGuiCol.TableHeaderBg]         = new Vector4(0.11764706f,  0.41568628f, 0.7882353f,  1.0f);
        style.Colors[(int)ImGuiCol.TableBorderStrong]     = new Vector4(0.18431373f,  0.29803923f, 0.5019608f,  1.0f);
        style.Colors[(int)ImGuiCol.TableBorderLight]      = new Vector4(0.28235295f,  0.41568628f, 0.64705884f, 1.0f);
        style.Colors[(int)ImGuiCol.TableRowBg]            = new Vector4(0.19766435f,  0.42783663f, 0.6309013f,  0.41568628f);
        style.Colors[(int)ImGuiCol.TableRowBgAlt]         = new Vector4(0.35576266f,  0.56356f,    0.7467811f,  0.41568628f);
        style.Colors[(int)ImGuiCol.TextSelectedBg]        = new Vector4(0.56991285f,  0.7436949f,  0.94849783f, 0.48497856f);
        style.Colors[(int)ImGuiCol.DragDropTarget]        = new Vector4(0.9098712f,   0.79023933f, 0.5740389f,  0.64806867f);
        style.Colors[(int)ImGuiCol.NavCursor]             = new Vector4(0.99141634f,  0.60492307f, 0.3616755f,  1.0f);
        style.Colors[(int)ImGuiCol.NavWindowingHighlight] = new Vector4(1.0f,         1.0f,        1.0f,        0.7f);
        style.Colors[(int)ImGuiCol.NavWindowingDimBg]     = new Vector4(0.8f,         0.8f,        0.8f,        0.2f);
        style.Colors[(int)ImGuiCol.ModalWindowDimBg]      = new Vector4(0.8f,         0.8f,        0.8f,        0.35f);
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
    internal void init() {
        _init_fonts();
        _init_style();
    }
}
