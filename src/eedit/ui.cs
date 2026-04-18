// SPDX-License-Identifier: MIT

/* [fkelava 15/8/25 17:41]
 * Place the UI in this method. The program stub will invoke it.
 */

namespace Fahrenheit.Tools.EEdit;

internal static class EEdit {
    public static EEditDisplayData Display = new();
    public static EEditEditorData  Editors = new();
}

internal class EEditDisplayData {
    public bool       show_demo_window;
    public bool       show_mode_select;
    public EEditMode  mode_select_selected_item;

    public EEditComponent? active_editor;
}

internal class EEditEditorData {
    public FileStream? active_file;
}

[SupportedOSPlatform("windows")]
internal static unsafe partial class UI {

    /// <summary>
    ///     Edits the static display data in response to keypresses.
    /// </summary>
    private static void _handle_keybinds() {
        if (ImGui.IsKeyPressed(ImGuiKey.F7)) {
            EEdit.Display.show_demo_window = !EEdit.Display.show_demo_window;
        }
    }

    /* [fkelava 15/02/26 22:52]
     * Extracted into its own method due to https://github.com/ocornut/imgui/issues/5684#issuecomment-1247928651.
     *
     * Effectively modals cannot be issued within the scope of a BeginMenu() statement. Thus that segment
     * simply sets booleans which trigger the rendering of modals here.
     */

    /// <summary>
    ///     Initiates rendering of modal dialogs when required.
    /// </summary>
    private static void _handle_modal() {
        if (EEdit.Display.show_mode_select) {
            ImGui.OpenPopup("Mode Select");
        }

        _render_mode_select_modal();
    }

    /// <summary>
    ///     Renders the 'mode select' dialog, used when EEdit cannot
    ///     autodetect the element type of a given Excel file.
    /// </summary>
    private static void _render_mode_select_modal() {
        ImGuiIOPtr io = ImGui.GetIO();

        /*
         * Center the modal.
         */

        ImGui.SetNextWindowPos(io.DisplaySize / 2, pivot: new(0.5f, 0.5f));

        if (!ImGui.BeginPopupModal("Mode Select", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize))
            return;

        ImGui.Text("EEdit can't autodetect the element type of this Excel file. Select the correct one.");

        /*
         * The label is typically laid out on the right side, which is not particularly nice-looking.
         */

        if (ImGui.BeginCombo("##File Types", _get_component_name_by_mode(EEdit.Display.mode_select_selected_item))) {
            for (EEditMode mode = 0; mode < EEditMode.FILE_TYPES_COUNT; mode++) {
                bool is_selected = mode == EEdit.Display.mode_select_selected_item;

                if (ImGui.Selectable(_get_component_name_by_mode(mode), is_selected)) {
                    EEdit.Display.mode_select_selected_item = mode;
                }

                if (is_selected) ImGui.SetItemDefaultFocus();
            }

            ImGui.EndCombo();
        }

        ImGui.SameLine();

        if (ImGui.Button("OK")) {
            EEdit.Display.active_editor    = _get_component_by_mode(EEdit.Display.mode_select_selected_item);
            EEdit.Display.show_mode_select = false;

            ImGui.CloseCurrentPopup();
        }

        ImGui.EndPopup();
    }

    /// <summary>
    ///     Renders the topmost, immovable EEdit menu.
    /// </summary>
    private static void _render_main_menu() {
        if (!ImGui.BeginMenuBar())
            return;

        if (ImGui.BeginMenu("File")) {
            if (ImGui.MenuItem("Open", "Ctrl+O") && _dialog_fopen(out FileStream? opened_file)) {
                EEdit.Editors.active_file = opened_file;

                /*
                 * If we can't deduce what kind of file this is, present a mode-select dialog.
                 */

                if ((EEdit.Display.active_editor = _get_component_for_file(opened_file.Name)) == null) {
                    EEdit.Display.show_mode_select = true;
                }
            }

            /*
             * Disallow saving if no file is loaded.
             */

            ImGui.BeginDisabled(EEdit.Editors.active_file == null);
            if (ImGui.MenuItem("Save", "Ctrl+S") && _dialog_fsave(EEdit.Editors.active_file!)) {
                EEdit.Editors.active_file!.Dispose();
                EEdit.Editors.active_file = null;
            }
            ImGui.EndDisabled();

            ImGui.EndMenu();
        }

        if (ImGui.BeginMenu("Mode")) {

            ImGui.EndMenu();
        }

        ImGui.EndMenuBar();
    }

    public static void Render() {
        _handle_keybinds();
        _handle_modal();

        /*
         * This is to be removed. A handy reference for me while programming.
         */

        if (EEdit.Display.show_demo_window) {
            ImGui.ShowDemoWindow(ref EEdit.Display.show_demo_window);
        }

        /*
         * Maximize the next window, which is the EEdit main window.
         * Abort if it cannot be displayed.
         */

        ImGuiIOPtr io = ImGui.GetIO();

        ImGui.SetNextWindowPos (Vector2.Zero);
        ImGui.SetNextWindowSize(io.DisplaySize);

        if (!ImGui.Begin("EEdit.Main",
              ImGuiWindowFlags.MenuBar
            | ImGuiWindowFlags.NoResize
            | ImGuiWindowFlags.NoTitleBar
            | ImGuiWindowFlags.NoMove
            | ImGuiWindowFlags.NoCollapse)
        ) {
            ImGui.End();
            return;
        }

        _render_main_menu();

        /*
         * Ensure the active editor occupies the entire remaining screen space.
         */

        ImGui.SetNextWindowPos(ImGui.GetContentRegionAvail());

        EEdit.Display.active_editor?.render();

        ImGui.End();
    }
}
