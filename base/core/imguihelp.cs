using Hexa.NET.ImGui;

namespace Fahrenheit.Core;

public unsafe class FhImGuiHelper {
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
        init_fonts();
    }

    private void init_fonts() {
        var io = ImGui.GetIO();

        string homedir = Directory.GetCurrentDirectory();
        string fontdir = homedir + @"\fahrenheit\bin\resources\fonts\";

        FONT_DEFAULT = io.Fonts.AddFontFromFileTTF(
            fontdir + "NotoSans-VariableFont_wdth,wght.ttf",
            20f,
            null,
            io.Fonts.GetGlyphRangesDefault()
        );
    }
}
