/* [fkelava 15/8/25 17:41]
 * Place the mod manager UI in this method. The program stub will invoke it.
 */

namespace Fahrenheit.Core.ModManager;

internal static unsafe class ModManager {
    private static bool _show_demo_window = true;

    public static void UI() {
        ImGui.ShowDemoWindow(ref _show_demo_window);
    }
}
