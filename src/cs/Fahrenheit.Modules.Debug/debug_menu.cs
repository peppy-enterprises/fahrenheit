using ImGuiNET;
using Fahrenheit.CoreLib.FFX;

namespace Fahrenheit.Modules.Debug;

public static unsafe class DebugMenu {
    public static void render(float delta) {
        ImGui.Text("Hehehe");
        ImGui.Text($"Tri count: {*Globals.Map.tri_count}");
        ImGui.Text("This is just some more text");
        ImGui.Text("To pad the window a bit");
    }
}
