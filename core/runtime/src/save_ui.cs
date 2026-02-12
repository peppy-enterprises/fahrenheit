// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core.Runtime;

/* [fkelava 26/11/25 17:25]
 * This structure contains all the fields the game would show as part of the standard Iggy
 * save menu display. These inline arrays are in reality UTF-8 strings, since both Iggy
 * and ImGui accept them as input. The sizes are taken from the base game.
 */

[StructLayout(LayoutKind.Sequential)]
internal struct FhSaveDisplayData {

    /* [fkelava 19/01/26 13:14]
     * An array of these of size DEFAULT_SET_SIZE is allocated by the save UI module on boot.
     * These instances are continually reused. To prevent garbage from being displayed when a slot
     * occupied in the previous set becomes empty, the save manager module (un)sets 'valid'.
     */

    internal bool valid;

    public InlineArray64 <byte> header;
    public InlineArray16 <byte> slot;
    public InlineArray64 <byte> create_time;
    public InlineArray128<byte> location;
    public InlineArray128<byte> play_time;
    public InlineArray32 <byte> player_name;
    public InlineArray16 <byte> icon_chr1;
    public InlineArray16 <byte> icon_chr2;
    public InlineArray16 <byte> icon_chr3;
    public InlineArray16 <byte> icon_map;
    public InlineArray128<byte> chapter;
    public InlineArray128<byte> completion;
    public InlineArray64 <byte> lm_level;
    public InlineArray64 <byte> lm_job;
}

[FhLoad(FhGameId.FFX | FhGameId.FFX2 | FhGameId.FFX2LM)]
public unsafe sealed class FhSaveUiModule : FhModule {

    private readonly FhModuleHandle<FhSaveExtensionModule> _sem_handle;
    private          FhSaveExtensionModule?                _sem;
    private readonly FhModuleHandle<FhSaveManagerModule>   _smm_handle;
    private          FhSaveManagerModule?                  _smm;

    private int                   _display_index;
    private IReadOnlySet<string>? _sets;
    private bool                  _pressed;

    public FhSaveUiModule() {
        _sem_handle = new(this);
        _smm_handle = new(this);
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        return _sem_handle.try_get_module(out _sem)
            && _smm_handle.try_get_module(out _smm);
    }

    public override void render_imgui() {
        if (FhSavePal.pal_get_screen_state() is not FhSaveScreenState.OPEN)
            return;

        /*
         * TODO: Address the fact ImGui does not intercept input when unfocused.
         */

        if (ImGui.IsKeyPressed(ImGuiKey.Escape) || ImGui.IsKeyPressed(ImGuiKey.Backspace)) {
            _sem!.signal_exit_abort();
            return;
        }

        ui_setswap();
        ui_mainwindow();
    }

    /// <summary>
    ///     Displays the main save/load menu,
    /// </summary>
    private void ui_mainwindow() {
        ImGuiIOPtr io = ImGui.GetIO();

        ImGui.SetNextWindowPos (new (io.DisplaySize.X * 0.008F, io.DisplaySize.Y * 0.105F));
        ImGui.SetNextWindowSize(new (io.DisplaySize.X * 0.984F, io.DisplaySize.Y * 0.88F));

        if (!ImGui.Begin("Save/Load###Fh.Runtime.SaveSystem.SaveLoadUI", FhApi.ImGuiHelper.WINDOW_FLAGS_FULLSCREEN & (~ImGuiWindowFlags.NoScrollbar) & (~ImGuiWindowFlags.NoNavFocus))) {
            ImGui.End();
            return;
        }

        int slot = 0, index = 0;
        if (_sem!.get_system_state() is FhSaveExtensionSystemState.SAVE) {
            if (_smm!.get_slots_used() < _smm!.get_slots_total()) {
                Vector2 size_new_save_btn = new(ImGui.GetContentRegionAvail().X, 0F);

                if (ImGui.Button("New Save Data", size_new_save_btn)) {
                    _sem!.save(slot);
                }
            }
            slot = 1;
        }

        ReadOnlySpan<FhSaveDisplayData> display_data = _smm!.get_display_data();

        for (; slot < _smm!.get_slots_total(); slot++) {
            if (display_data[slot].valid) {
                ui_savefile(index++, display_data[slot]);
            }
        }

        ImGui.End();
    }

    private void ui_setswap() {
        ImGuiIOPtr    io    = ImGui.GetIO();
        ImGuiStylePtr style = ImGui.GetStyle();

        float width_modal  = Math.Max(600, io.DisplaySize.X / 3);    // 33% margin
        float height_modal = Math.Max(400, io.DisplaySize.Y * 0.8F); // 10% margin

        ImGui.SetNextWindowPos (new ((io.DisplaySize.X - width_modal) / 2, io.DisplaySize.Y * 0.015F));
        ImGui.SetNextWindowSize(new (width_modal,                          io.DisplaySize.Y * 0.075F));

        if (!ImGui.Begin("Set Swap###Fh.Runtime.SaveSystem.SetSwap", FhApi.ImGuiHelper.WINDOW_FLAGS_FULLSCREEN & (~ImGuiWindowFlags.NoNavFocus))) {
            ImGui.End();
            return;
        }

        string active_set      = _smm!.get_active_set();
        string slots_text      = $"({_smm!.get_slots_used()}/{_smm!.get_slots_total()})";
        string active_set_text = $"{active_set} {slots_text}";

        float width_window = ImGui.GetWindowSize().X;
        float width_text   = ImGui.CalcTextSize(active_set_text).X;

        ImGui.SetCursorPosX((width_window - width_text) * 0.5f);
        ImGui.Text(active_set_text);

        FhApi.ImGuiHelper.set_next_align("Change Set"u8, 0.5F, style.FramePadding.X * 2.0F);

        if (ImGui.Button("Change Set"u8)) {
            /* [fkelava 22/01/26 14:14]
             * The save manager performs expensive disk I/O on many operations.
             * It is important to only retrieve values from it once, not every frame.
             */

            _sets = _smm!.get_sets();
            ImGui.OpenPopup("Select Set"u8);
        }

        ImGui.SetNextWindowPos (new ((io.DisplaySize.X - width_modal) / 2, (io.DisplaySize.Y - height_modal) / 2));
        ImGui.SetNextWindowSize(new (width_modal, height_modal));

        if (ImGui.BeginPopupModal("Select Set")) {
            foreach (string set in _sets!) {
                bool is_selected = set == active_set;

                if (ImGui.Selectable(set, is_selected)) {
                    _smm!.switch_active_set(set);
                    ImGui.CloseCurrentPopup();
                }

                if (is_selected) ImGui.SetItemDefaultFocus();
            }

            ImGui.EndPopup();
        };

        ImGui.End();
    }

    private void ui_savefile(int index, FhSaveDisplayData data) {
        ImGuiStylePtr style       = ImGui.GetStyle();
        Vector2       spacer_size = new(0F, style.FramePadding.Y);
        Vector2       window_size = new(ImGui.GetContentRegionAvail().X, 0F);

        // TODO: Change this to ItemSpacing instead of FramePadding
        if (index != 0) {
            ImGui.Dummy(spacer_size);
        }
        Vector2 start = ImGui.GetCursorScreenPos();
        bool hovered = index == _display_index;
        bool pressed = _pressed;
        if (hovered) _pressed = false; // I'm not sure this is visible since it's delayed by (at least) 1 frame

        bool is_highlighted = hovered && ImGui.IsWindowFocused();
        bool is_selected    = is_highlighted && pressed;

        ImGui.PushStyleColor(ImGuiCol.ChildBg, is_highlighted switch {
            true when is_selected => ImGui.GetColorU32(ImGuiCol.FrameBgActive),
            true                  => ImGui.GetColorU32(ImGuiCol.FrameBgHovered),
            _                     => ImGui.GetColorU32(ImGuiCol.FrameBg)
        });

        if (ImGui.BeginChild($"###Slot{index}", window_size, ImGuiChildFlags.AutoResizeY, FhApi.ImGuiHelper.WINDOW_FLAGS_FULLSCREEN | ImGuiWindowFlags.NoInputs)) {
            ImGui.PushStyleColor(ImGuiCol.ChildBg, ImGui.GetColorU32(ImGuiCol.TitleBg));
            if (ImGui.BeginChild("##Title", window_size, ImGuiChildFlags.AutoResizeY, FhApi.ImGuiHelper.WINDOW_FLAGS_FULLSCREEN | ImGuiWindowFlags.NoInputs)) {
                ImGui.Indent();
                ui_save_info_generic(data, index);
                ImGui.Unindent();
            }

            ImGui.EndChild();
            ImGui.PopStyleColor();

            ImGui.Indent();
            switch (FhGlobal.game_id) {
                case FhGameId.FFX:    ui_save_info_x   (data); break;
                case FhGameId.FFX2:   ui_save_info_x2  (data); break;
                case FhGameId.FFX2LM: ui_save_info_x2lm(data); break;
            }
            ImGui.Unindent();

            ImGui.Dummy(spacer_size);
        }
        ImGui.EndChild();
        ImGui.PopStyleColor();

        Vector2 itemSize = ImGui.GetItemRectSize();
        ImGui.SetCursorScreenPos(start);
        ImGui.SetNextItemAllowOverlap();
        pressed = ImGui.InvisibleButton($"###Slot{index}.Button", itemSize, ImGuiButtonFlags.EnableNav | ImGuiButtonFlags.MouseButtonLeft);
        if (pressed) {
            _pressed = true;
            switch (_sem!.get_system_state()) {
                case FhSaveExtensionSystemState.SAVE: _sem!.save(index); break;
                case FhSaveExtensionSystemState.LOAD: _sem!.load(index); break;
                case FhSaveExtensionSystemState.ALBD: _sem!.load_albd(index); break;
            }
        }
        hovered = ImGui.IsItemHovered();
        if (hovered) _display_index = index;
    }

    private void ui_save_info_generic(FhSaveDisplayData data, int slot) {
        ImGuiStylePtr style   = ImGui.GetStyle();
        float         padding = style.FramePadding.X + style.IndentSpacing;

        bool is_autosave = slot == 0 && _sem!.get_system_state() is FhSaveExtensionSystemState.LOAD;

        ImGui.Text(is_autosave ? "Autosave"u8 : data.slot);
        ImGui.SameLine(is_autosave ? 100 : 60);
        ImGui.Text(data.location);
        ImGui.SameLine();
        FhApi.ImGuiHelper.set_next_align(data.create_time, 1.0F, style.FramePadding.X + style.IndentSpacing);
        ImGui.Text(data.create_time);
    }

    private void ui_save_info_x(FhSaveDisplayData data) {
        ImGui.Text(data.player_name);
        ImGui.Text(data.play_time);
    }

    private void ui_save_info_x2(FhSaveDisplayData data) {
        ImGuiStylePtr style   = ImGui.GetStyle();
        float         padding = style.FramePadding.X + style.IndentSpacing;

        ImGui.Text(data.icon_chr1);
        ImGui.SameLine();
        FhApi.ImGuiHelper.set_next_align(data.completion, 1.0F, padding);
        ImGui.Text(data.completion);
        ImGui.Text(data.chapter);
        ImGui.SameLine();
        FhApi.ImGuiHelper.set_next_align(data.play_time, 1.0F, padding);
        ImGui.Text(data.play_time);
    }

    private void ui_save_info_x2lm(FhSaveDisplayData data) {
        ImGui.Text("Yuna");
        ImGui.Text(data.lm_job);
        ImGui.SameLine(80);
        ImGui.Text(data.lm_level);
    }
}
