using System.Numerics;

namespace Fahrenheit.Core.Runtime;

[FhLoad(FhGameType.FFX)]
public unsafe class FhModConfigModule : FhModule {
    private FhModContext _context;
    private FileStream _global_state;

    private bool _no_settings_warning_pass_done;

    //private bool _dockbuilder_initialized = false;
    internal bool is_open;
    private int _selected_mod_idx;

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        _context = mod_context;
        _global_state = global_state_file;

        return true;
    }

    internal void open() {
        is_open = true;
        //TODO: Prevent the game from playing the Zanarkand scene while the config menu is open
    }

    internal void close() {
        is_open = false;
        _selected_mod_idx = 0;
    }

    public override void render_imgui() {
        //TODO: Add a proper open/close button in the topright corner
        if (ImGui.IsKeyPressed(ImGuiKey.F7)) {
            if (is_open) close();
            else open();
        }

        if (!is_open) return;

        ImGuiViewportPtr viewport = ImGui.GetMainViewport();

        ImGui.SetNextWindowPos(viewport.WorkPos);
        ImGui.SetNextWindowSize(viewport.WorkSize);
        ImGui.SetNextWindowViewport(viewport.ID);

        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
        ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0, 0, 0, 1));
        ImGui.PushFont(FhApi.ImGuiHelper.FONT_DEFAULT, 20f);

        if (ImGui.Begin("ModConfig", FhApi.ImGuiHelper.WINDOW_FLAGS_FULLSCREEN)) {

            //TODO: Make this work

        // Build the dock
        // ImGuiIOPtr io = ImGui.GetIO();
        // if ((io.ConfigFlags & ImGuiConfigFlags.DockingEnable) != 0) {
        //     uint dockspace_id = ImGui.GetID("ModConfigDockspace");
        //     ImGui.DockSpace(dockspace_id, new Vector2(0, 0), ImGuiDockNodeFlags.NoResize | ImGuiDockNodeFlags.NoUndocking);
        //
        //     if (!dockbuilder_initialized) {
        //         dockbuilder_initialized = true;
        //
        //         ImGuiP.DockBuilderRemoveNode(dockspace_id);
        //         ImGuiP.DockBuilderAddNode(dockspace_id, (ImGuiDockNodeFlags)((int)ImGuiDockNodeFlagsPrivate.NoTabBar));
        //         ImGuiP.DockBuilderSetNodeSize(dockspace_id, viewport.Size);
        //
        //         uint tabs_id     = 0;
        //         uint settings_id = 0;
        //         uint help_id     = 0;
        //
        //         ImGuiP.DockBuilderSplitNode(dockspace_id, ImGuiDir.Left, 0.16f, &tabs_id, null);
        //         ImGuiP.DockBuilderSplitNode(dockspace_id, ImGuiDir.Up, 0.05f, &help_id, null);
        //
        //         ImGuiP.DockBuilderSetNodeSize(tabs_id,     new(viewport.WorkSize.X * 0.10f, viewport.WorkSize.Y));
        //         ImGuiP.DockBuilderSetNodeSize(help_id,     new(viewport.WorkSize.X * 0.80f, viewport.WorkSize.Y * 0.05f));
        //         ImGuiP.DockBuilderSetNodeSize(settings_id, new(viewport.WorkSize.X * 0.80f, viewport.WorkSize.Y * 0.90f));
        //
        //         ImGuiP.DockBuilderAddNode(tabs_id,     ImGuiDockNodeFlags.NoUndocking);
        //         ImGuiP.DockBuilderAddNode(settings_id, ImGuiDockNodeFlags.NoUndocking);
        //         ImGuiP.DockBuilderAddNode(help_id,     ImGuiDockNodeFlags.NoUndocking);
        //
        //         ImGuiP.DockBuilderDockWindow("ModTabs", tabs_id);
        //         ImGuiP.DockBuilderDockWindow("ModSettings", settings_id);
        //         ImGuiP.DockBuilderDockWindow("ModSettingHelp", help_id);
        //
        //         ImGuiP.DockBuilderFinish(dockspace_id);
        //     }
        // }

            ImGui.SetNextWindowPos(viewport.WorkPos);
            ImGui.SetNextWindowSize(new Vector2(viewport.WorkSize.X * 0.16f, viewport.WorkSize.Y));
            if (ImGui.Begin("ModTabs", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove)) {
                int mod_idx = 0;

                float tab_width = ImGui.GetContentRegionAvail().X;
                foreach (FhModContext mod in FhApi.ModController.get_all()) {
                    if (has_settings(mod)) {
                        render_mod_tab(mod, mod_idx++, tab_width);
                    } else {
                        if (!_no_settings_warning_pass_done) {
                            _logger.Warning($"Mod {mod.Manifest.Name} has no settings!");
                        }
                        mod_idx++;
                    }
                }
                _no_settings_warning_pass_done = true;
            }
            ImGui.End();

            ImGui.SetNextWindowPos(new Vector2(viewport.WorkPos.X + viewport.WorkSize.X * 0.17f, viewport.WorkPos.Y));
            ImGui.SetNextWindowSize(new Vector2(viewport.WorkSize.X * 0.83f, viewport.WorkSize.Y));
            if (ImGui.Begin("ModSettings", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove)) {
                FhModContext[] mods = [ .. FhApi.ModController.get_all() ];
                foreach (FhModuleContext module in mods[_selected_mod_idx].Modules) {
                    module.Module.settings?.render_name();
                    module.Module.settings?.render();
                }
            }
            ImGui.End();
        }


        // ImGui uses the style var in `Begin()` so we're free to pop it before `End()`
        // See: https://github.com/ocornut/imgui/issues/1797#issuecomment-644131003
        ImGui.PopStyleVar(3);
        ImGui.PopStyleColor();
        ImGui.PopFont();

        ImGui.End(); // Closing the fullscreen window!
    }

    private void render_mod_tab(FhModContext mod, int mod_idx, float tab_width) {
        if (ImGui.Button($"{mod.Manifest.Name}##mod{mod_idx}", new Vector2(tab_width, 0)))
            _selected_mod_idx = mod_idx;
    }

    private static bool has_settings(FhModContext mod) {
        foreach (FhModuleContext module in mod.Modules) {
            if (module.Module.settings is not null) return true;
        }

        return false;
    }
}
