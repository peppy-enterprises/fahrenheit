// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* [fkelava 15/8/25 17:41]
 * Place the mod manager UI in this method. The program stub will invoke it.
 */

namespace Fahrenheit.Tools.ModManager;

internal static unsafe class ModManager {
    private static bool _show_demo_window = true;

    public static void UI() {
        ImGui.ShowDemoWindow(ref _show_demo_window);
    }
}
