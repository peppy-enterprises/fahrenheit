using Fahrenheit.CoreLib;
using Fahrenheit.CoreLib.FFX;
using static Fahrenheit.Modules.Debug.FuncLib;

namespace Fahrenheit.Modules.Debug;

public static unsafe class SphereGridEditor {
    public const int MAX_NODE_COUNT = 1024;

    private static bool enabled = true;
    private static int node_count;

    private static void try_toggle() {
        if (/* abmap is open && */ Globals.Input.r1.held && Globals.Input.r2.held && Globals.Input.triangle.just_pressed) {
            enabled = !enabled;
        }
    }

    public static void update() {
        try_toggle();
        if (!enabled) return;

        if (Globals.SphereGrid.lpamng == null) return;

        render();
    }

    public static void render() {
        TOMkpCrossExtMesFontLClut(
                0, FhCharset.Us.ToBytes(Globals.SphereGrid.lpamng->selected_node_idx.ToString()),
                0, 0, 0xA0, 0, 0.69f, 0);
    }
}
