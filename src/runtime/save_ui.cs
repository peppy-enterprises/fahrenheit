// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

using TextAlignment = Fahrenheit.FhGui.TextAlignment;

namespace Fahrenheit.Runtime;

/* [fkelava 07/05/26 17:39]
 * Fahrenheit completely overrides the game's base save system to allow set functionality
 * and lifting the limit of 200 saves per set. For performance reasons, this requires
 * the game's base Flash-based Iggy UI to be bypassed. This module implements its ImGui replacement.
 *
 * Specifically, the actual save UI is implemented in ActionScript, which the game calls through to.
 * This is extremely inefficient, and computationally scales almost quadratically with the number of saves;
 * raising the set limit from 200 to 500 caused set listing times in excess of ~10s. It is almost certain
 * that the limit of 200 was chosen because it was the largest number that still performed acceptably.
 */

/// <summary>
///     Implements Fahrenheit's replacement save/load user interface.
/// </summary>
[FhLoad(FhGameId.FFX | FhGameId.FFX2 | FhGameId.FFX2LM)]
public sealed class FhX2SaveUiModule : FhModule {
    private class ScrollableData {
        public int current;
        public int max;
        public int visible;

        public int get_clip_start() {
            return current;
        }

        public int get_clip_end() {
            return Math.Min(current + visible, max);
        }

        public bool is_within_clip(int index) {
            return get_clip_start() <= index && index < get_clip_end();
        }

        public float get_progress() {
            if (max <= visible) return 0;

            // Slightly weird math: we technically only scroll through the first visible items,
            // so we must reduce the amount of max items slightly.
            float progress = current / (float)Math.Max(0, max - visible);

            // When debugging, a visual glitch in the scrollbar may be useful for identifying an issue,
            // so we only clamp in release.
#if !DEBUG
            progress = Math.Clamp(progress, 0f, 1f);
#endif

            return progress;
        }

        public void scroll(int amount) {
            current += amount;
            current = Math.Clamp(current, 0, Math.Max(0, max - visible));
        }

        public void scroll_begin() {
            current = 0;
        }

        public void scroll_end() {
            current = max - visible;
        }
    }

    private readonly ScrollableData _scrollable_saves = new() { visible = 5 };

    private readonly FhModuleHandle<FhSaveExtensionModule> _sem_handle;
    private          FhSaveExtensionModule?                _sem;

    private int                   _display_index;
    private IReadOnlySet<string>? _sets;

    private bool _loaded_all_textures;

    // Scrollbar related
    private float _drag_start_mouse_y;
    private float _drag_start_scroll_y;
    private bool  _dragging_scrollbar = false;

    private const string MENU_D3D11_DIR     = "/FFX-2_Data/GameData/PS3Data/menu/D3D11/";
    private const string MENU_MAHOJIN_DIR   = "/FFX-2_Data/GameData/PS3Data/menu/menu_mahojin/tex/D3D11/";
    private const string MENU_PLATE_DIR     = "/FFX-2_Data/GameData/PS3Data/menu/menu_plate/tex/D3D11/";
    private const string MENU_FACEDATA_DIR  = "/FFX-2_Data/GameData/PS3Data/menu/face_data/D3D11/";
    private const string SAVEDATAICONS_DIR  = "/FFX-2_Data/GameData/PS3Data/savedataicons/";

    private readonly FhTexture _texture_bg       = new(MENU_D3D11_DIR     + "menuback.dds.phyre",             FhTextureType.PHYRE);
    private readonly FhTexture _texture_mahojin  = new(MENU_MAHOJIN_DIR   + "14336_19_0_0_512_512.dds.phyre", FhTextureType.PHYRE);
    private readonly FhTexture _texture_save     = new(MENU_PLATE_DIR     + "12288_19_0_0_256_256.dds.phyre", FhTextureType.PHYRE);
    private readonly FhTexture _texture_freetex  = new(MENU_D3D11_DIR     + "freetex.dds.phyre",              FhTextureType.PHYRE);
    private readonly FhTexture _texture_message  = new(MENU_D3D11_DIR     + "x2_bg.dds.phyre",                FhTextureType.PHYRE);

    private readonly Vector2 _tex_mahojin_size = new(2048f, 2048f);
    private readonly Vector2 _tex_save_size    = new(512f ,  512f);
    private readonly Vector2 _tex_faces_size   = new(256f ,  256f);
    private readonly Vector2 _tex_map_size     = new(320f ,  176f);
    private readonly Vector2 _tex_freetex_size = new(1024f,  768f);
    private readonly Vector2 _tex_message_size = new(2048f, 2048f);

    /* TODO: 
     * Translate Autosave, Help, and Save Count text
     * Sort saves by Create Time
     * Split Play Time and Create Time strings for LM
     * Trim extra 0's from Create Time, i.e. 2026/08/01 -> 2026/8/1
     * Change "Select Set" ImGui popup to use game textures? 
    */

    public FhX2SaveUiModule() {
        _sem_handle = new(this);
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        return _sem_handle.try_get_module(out _sem);
    }

    public override void render_imgui() {
        if (FhSavePal.pal_get_screen_state() is not FhSaveScreenState.OPEN) {
            return;
        }

        if (handle_input()) return;
        if (!try_load_textures()) return;

        ImDrawListPtr draw = ImGui.GetBackgroundDrawList();

        (Vector2 window_size, Vector2 window_offset) = get_window_bounds();
        Vector2 window_min = window_offset;
        Vector2 window_max = window_offset + window_size;

        draw.PushClipRect(window_min, window_max, true); // Draw a scissor box to prevent mahojin glyph drawing out of bounds

        ui_background();
        ui_help();
        ui_setswap();
        ui_mainwindow();

        draw.PopClipRect();
    }

    /// <summary>Handle player input.</summary>
    /// <returns>Whether the save/load screen was closed.</returns>
    private bool handle_input() {
        if (ImGui.IsKeyPressed(ImGuiKey.Escape) || ImGui.IsKeyPressed(ImGuiKey.Backspace)) {
            _sem!.signal_exit_abort();
            return true;
        }

        int scroll_amount = 0;

        float wheel = ImGui.GetIO().MouseWheel;
        if (wheel != 0) {
            scroll_amount = -int.CreateChecked(Math.Truncate(wheel));
        }

        if (ImGui.IsKeyPressed(ImGuiKey.PageUp)) scroll_amount = -_scrollable_saves.visible;
        if (ImGui.IsKeyPressed(ImGuiKey.PageDown)) scroll_amount = _scrollable_saves.visible;
        if (ImGui.IsKeyPressed(ImGuiKey.Home)) { _scrollable_saves.scroll_begin(); _display_index = 0; return false; }
        if (ImGui.IsKeyPressed(ImGuiKey.End)) { _scrollable_saves.scroll_end(); _display_index = _scrollable_saves.max - 1; return false; }

        if (ImGui.IsKeyPressed(ImGuiKey.W)) scroll_amount = -1;
        if (ImGui.IsKeyPressed(ImGuiKey.S)) scroll_amount = 1;

        // Cursor handling, forces scroll to match current cursor pos
        if (scroll_amount != 0) {
            _display_index += scroll_amount;

            if (_display_index < _scrollable_saves.get_clip_start()) {
                _scrollable_saves.scroll(_display_index - _scrollable_saves.get_clip_start());
            }
            else if (_display_index >= _scrollable_saves.get_clip_end()) {
                _scrollable_saves.scroll(_display_index - (_scrollable_saves.get_clip_end() - 1));
            }

            _display_index = Math.Clamp(_display_index, 0, Math.Max(0, _scrollable_saves.max - 1));
        }

        return false;
    }

    /// <summary>
    ///     Attempt to load all of the textures the save/load screen requires to display properly.
    /// </summary>
    /// <returns>
    ///     Whether all textures have been successfully loaded.
    /// </returns>
    private bool try_load_textures() {
        if (_loaded_all_textures) return true;

        FhTexture[] textures = [
            _texture_bg,
            _texture_mahojin,
            _texture_save,
            _texture_freetex,
            _texture_message
        ];

        _loaded_all_textures = true;
        foreach (FhTexture texture in textures) {
            if (!texture.is_loaded() && !FhApi.Resources.load_game_texture_2d(texture)) {
                _loaded_all_textures = false;
            }
        }

        return _loaded_all_textures;
    }

    private (Vector2 size, Vector2 offset) get_window_bounds() {
        Vector2 window_size   = ImGui.GetMainViewport().WorkSize;
        float   target_aspect = 16f / 9f; // Force 16:9 aspect ratio
        float   window_aspect = window_size.X / window_size.Y;

        if (window_aspect > target_aspect) {
            Vector2 size   = new(window_size.Y * target_aspect, window_size.Y);
            Vector2 offset = new((window_size.X - size.X) * 0.5f, 0f);
            return (size, offset);
        }
        else {
            Vector2 size   = new(window_size.X, window_size.X / target_aspect);
            Vector2 offset = new(0f, (window_size.Y - size.Y) * 0.5f);
            return (size, offset);
        }
    }

    private (Vector2 u, Vector2 v) scale_tex_uv(Vector2 tex_size, Vector2 u, Vector2 v) {
        return (u / tex_size, v / tex_size);
    }

    private (Vector2 u, Vector2 v) scale_screen_uv(Vector2 screen_size, Vector2 u, Vector2 v) {
        (Vector2 window_size, Vector2 window_offset) = get_window_bounds();

        Vector2 scaled_u = (u / screen_size) * window_size + window_offset;
        Vector2 scaled_v = (v / screen_size) * window_size + window_offset;
        return (scaled_u, scaled_v);
    }

    void draw_highlight_shadow(ImDrawListPtr draw, Vector2 pos_topleft, Vector2 size, float thickness, uint highlight, uint shadow) {
        // Top Highlight
        draw.AddRectFilled(
            pos_topleft,
            new Vector2(pos_topleft.X + size.X, pos_topleft.Y + thickness),
            highlight
        );

        // Left Highlight
        draw.AddRectFilled(
            new Vector2(pos_topleft.X, pos_topleft.Y + thickness),
            new Vector2(pos_topleft.X + thickness, pos_topleft.Y + size.Y),
            highlight
        );

        // Bottom Shadow - intentional overlap with left highlight to mimic the vanilla game!
        draw.AddRectFilled(
            new Vector2(pos_topleft.X, pos_topleft.Y + size.Y - thickness),
            pos_topleft + size,
            shadow
        );

        // Right Shadow
        draw.AddRectFilled(
            new Vector2(pos_topleft.X + size.X - thickness, pos_topleft.Y + thickness),
            new Vector2(pos_topleft.X + size.X, pos_topleft.Y + size.Y - thickness),
            shadow
        );
    }

    /// <summary>
    ///     Draws the background for the save/load screen.
    /// </summary>
    private void ui_background() {
        if (
            !_texture_bg.try_use(out ImTextureRef bg, out _)
         || !_texture_mahojin.try_use(out ImTextureRef mahojin, out _)
         ) {
            return;
        }

        ImDrawListPtr draw = ImGui.GetBackgroundDrawList();

        Vector2 display_size = ImGui.GetMainViewport().WorkSize;
        Vector2 screen_size  = new(1920f, 1080f);

        (Vector2 bg_su, Vector2 bg_sv) = scale_screen_uv(screen_size, Vector2.Zero, screen_size);

        Vector2 tex_u = new(0f, 1f);
        Vector2 tex_v = new(1f, 0f);

        draw.AddImage(bg, bg_su, bg_sv, tex_u, tex_v);

        (Vector2 mahojin_tu, Vector2 mahojin_tv) = scale_tex_uv(
            _tex_mahojin_size,
            new(6f   , 2038f),
            new(1514f,  529f)
        );

        (Vector2 mahojin_su, Vector2 mahojin_sv) = scale_screen_uv(
            screen_size,
            new(374f ,  -53f),
            new(1600f, 1165f)
        );

        draw.AddImage(mahojin, mahojin_su, mahojin_sv, mahojin_tu, mahojin_tv); // Background glyph

        // Draws a shadow over the corners of the background to match the vanilla menu
        {
            // RGBA8 (little-endian)
            uint grad_l = 0xD0000000;
            uint grad_r = 0x40000000;

            Vector2 tl = new(0f, display_size.Y);
            Vector2 br = new(display_size.X, 0f);

            float midX = (tl.X + br.X) * 0.5f;

            draw.AddRectFilledMultiColor(tl, new Vector2(midX, br.Y), grad_l, grad_r, grad_r, grad_l);
            draw.AddRectFilledMultiColor(new Vector2(midX, tl.Y), br, grad_r, grad_l, grad_l, grad_r);
        }
    }

    /// <summary>
    ///     Draws the help bar/text for the save/load screen.
    /// </summary>
    private void ui_help() {
        if (!_texture_freetex.try_use(out ImTextureRef freetex, out _)) {
            return;
        }

        ImDrawListPtr draw = ImGui.GetBackgroundDrawList();

        (Vector2 window_size, _) = get_window_bounds();
        Vector2  screen_size     = new(1920f, 1080f);
        float    scale_factor    = window_size.Y / 1080f;
        float    font_size       = 42f * scale_factor;

        {
            uint grad_l = 0xFF000000; // Black
            uint grad_r = 0x10000000;

            Vector2 tl = scale_screen_uv(screen_size, new(0f   , 149f), Vector2.Zero).Item1;
            Vector2 br = scale_screen_uv(screen_size, new(1920f, 100f), Vector2.Zero).Item1;

            draw.AddRectFilledMultiColor(tl, br, grad_l, grad_r, grad_r, grad_l);
        }

        {
            uint grad_l = 0xFF00bfb5; // Yellow
            uint grad_r = 0x1000bfb5;

            Vector2 tl = scale_screen_uv(screen_size, new(0f   , 141.5f), Vector2.Zero).Item1;
            Vector2 br = scale_screen_uv(screen_size, new(1920f,   138f), Vector2.Zero).Item1;

            draw.AddRectFilledMultiColor(tl, br, grad_l, grad_r, grad_r, grad_l);
        }

        (Vector2 help_tu, Vector2 help_tv) = scale_tex_uv(
            _tex_freetex_size,
            new(643f, 634f),
            new(783f, 576f)
        );

        (Vector2 help_su, Vector2 help_sv) = scale_screen_uv(
            screen_size,
            new(151f,  94f),
            new(229f, 126f)
        );

        draw.AddImage(freetex, help_su, help_sv, help_tu, help_tv); // "Help" graphic

        bool is_save = _sem!.get_system_state() is FhSaveExtensionSystemState.SAVE;

        (Vector2 text_su, _) = scale_screen_uv(
            screen_size,
            new(240f, 91f),
            Vector2.Zero
        );

        FhApi.Gui.draw_text(draw, text_su, is_save ? "Select save area" : "Select save data", font_size, true, TextAlignment.BEGIN, TextAlignment.BEGIN);
    }

    /// <summary>
    ///     Draws the set swap UI for the save/load screen.
    /// </summary>
    private void ui_setswap() {
        if (!_texture_message.try_use(out ImTextureRef message, out _)) {
            return;
        }

        (Vector2 window_size, Vector2 window_offset) = get_window_bounds();
        Vector2  screen_size                         = new(1920f, 1080f);
        float    scale_factor                        = window_size.Y / 1080f;
        float    font_size                           = 36f * scale_factor;

        ImDrawListPtr draw = ImGui.GetBackgroundDrawList();
        ImFontPtr     font = ImGui.GetFont();

        string active_set = FhInternal.Saves.get_active_set();

        Vector2 text_size = font.CalcTextSizeA(font_size, float.MaxValue, 0f, active_set);

        float padding_x = 250f * scale_factor;
        float min_width = 450f * scale_factor;
        float bg_width  = MathF.Max(min_width, text_size.X + padding_x);

        Vector2 center_top = scale_screen_uv(screen_size, new(960f, 22f), Vector2.Zero).Item1;
        Vector2 tl         = new(center_top.X - (bg_width * 0.5f), center_top.Y);
        Vector2 br         = scale_screen_uv(screen_size, new(960f, 88f), Vector2.Zero).Item1 with { X = tl.X + bg_width };

        draw.AddRectFilled(tl, br, 0x80000000); // Transparent black background

        // Gold accents on message background
        float accent_w = 70f * scale_factor;
        float accent_h = 70f * scale_factor;
        float accent_y = scale_screen_uv(screen_size, new(0f, 20f), Vector2.Zero).Item1.Y;

        Vector2 accent1_tl = new(tl.X - (2f * scale_factor), accent_y);
        Vector2 accent1_br = new(accent1_tl.X + accent_w, accent_y + accent_h);

        Vector2 accent2_br = new(br.X + (2f * scale_factor), accent_y);
        Vector2 accent2_tl = new(accent2_br.X - accent_w, accent_y + accent_h);

        (Vector2 accent1_uv1, Vector2 accent1_uv2) = scale_tex_uv(
            _tex_message_size,
            new(1469f, 1020f),
            new(1755f,  733f)
        );

        (Vector2 accent2_uv1, Vector2 accent2_uv2) = scale_tex_uv(
            _tex_message_size,
            new(1755f, 1020f),
            new(1469f,  733f)
        );

        draw.AddImage(message, accent1_tl, accent1_br, accent1_uv1, accent1_uv2);
        draw.AddImage(message, accent2_tl, accent2_br, accent2_uv1, accent2_uv2);

        // Display current Number of Saves
        Vector2 text_offset = scale_screen_uv(screen_size, new(1717f, 123f), Vector2.Zero).Item1;
        string  save_count  = FhInternal.Saves.get_slots_used() == 1 ? "1 Save" : $"{FhInternal.Saves.get_slots_used()} Saves";

        FhApi.Gui.draw_text(draw, text_offset, save_count, font_size, true, TextAlignment.END, TextAlignment.BEGIN);

        Vector2 bg_size = br - tl;
        ImGui.SetNextWindowPos(tl);
        ImGui.SetNextWindowSize(bg_size);

        ImGuiWindowFlags flags = FhApi.Gui.WINDOW_FLAGS_FULLSCREEN
        & ~ImGuiWindowFlags.NoNavFocus
        | ImGuiWindowFlags.NoBackground;

        if (!ImGui.Begin("Set Swap###Fh.Runtime.SaveSystem.SetSwap", flags)) {
            ImGui.End();
            return;
        }

        Vector2 bg_center = (tl + br) * 0.5f;

        Vector2 cursor_pos = (bg_size - text_size) * 0.5f;
        ImGui.SetCursorPos(cursor_pos);

        if (ImGui.Selectable("###ActiveSetSelectable", false, ImGuiSelectableFlags.None, text_size)) {
            _sets = FhInternal.Saves.get_sets();
            ImGui.OpenPopup("Select Set"u8);
        }

        FhApi.Gui.draw_text(draw, bg_center, active_set, font_size, true, TextAlignment.CENTER, TextAlignment.CENTER);

        Vector2 modal_size = new(Math.Max(600f, window_size.X * 0.35f), Math.Max(400f, window_size.Y * 0.7f));
        Vector2 modal_pos  = window_offset + (window_size - modal_size) * 0.5f;

        ImGui.SetNextWindowPos(modal_pos);
        ImGui.SetNextWindowSize(modal_size);

        if (ImGui.BeginPopupModal("Select Set")) {
            foreach (string set in _sets!) {
                bool is_selected = set == active_set;

                if (ImGui.Selectable(set, is_selected)) {
                    FhInternal.Saves.switch_active_set(set);
                    _scrollable_saves.scroll_begin(); // Reset scroll progress to the top
                    ImGui.CloseCurrentPopup();
                }

                if (is_selected) ImGui.SetItemDefaultFocus();
            }

            ImGui.EndPopup();
        }

        ImGui.End();
    }

    /// <summary>
    ///     Displays the main save/load menu.
    /// </summary>
    private void ui_mainwindow() {
        (Vector2 window_size, Vector2 window_offset) = get_window_bounds();
        float    scale_factor                        = window_size.Y / 1080f;

        Vector2 offset = window_offset + new Vector2(window_size.X * (0f / 1920f), window_size.Y * (161f / 1080f));
        Vector2 size   = new(window_size.X * (1734f / 1920f), window_size.Y * (837f / 1080f));

        ImGui.SetNextWindowPos(offset);
        ImGui.SetNextWindowSize(size);

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);

        ImGuiWindowFlags flags = FhApi.Gui.WINDOW_FLAGS_FULLSCREEN
        & ~ImGuiWindowFlags.NoNavFocus
        | ImGuiWindowFlags.NoScrollbar
        | ImGuiWindowFlags.NoBackground;

        if (!ImGui.Begin("Save/Load###Fh.Runtime.SaveSystem.SaveLoadUI", flags)) {
            ImGui.PopStyleVar();
            ImGui.End();
            return;
        }

        ImGui.SetCursorPos(new Vector2(0f, 11f * scale_factor));

        bool is_save = _sem!.get_system_state() is FhSaveExtensionSystemState.SAVE;
        List<FhSaveDisplayData> display_data = FhInternal.Saves.get_display_data();

        if (!is_save && display_data.Count < 1) {
            ui_no_saves();
            ImGui.Dummy(new Vector2(1f, 1f));
            ImGui.PopStyleVar();
            ImGui.End();
            return;
        }

        // If saving, forces slot 0 to be the New Save option
        List<FhSaveDisplayData> save_list = new();

        if (is_save) {
            for (int i = 0; i < display_data.Count; i++) {
                if (display_data[i].slot != 0) {
                    save_list.Add(display_data[i]);
                }
            }
        }
        else {
            save_list = display_data;
        }

        int total_count = is_save ? save_list.Count + 1 : save_list.Count;

        _scrollable_saves.max = total_count;

        int start = _scrollable_saves.get_clip_start();
        int end   = Math.Min(_scrollable_saves.get_clip_end(), total_count);

        if (total_count > 0) {
            _display_index = Math.Clamp(_display_index, start, Math.Max(start, end - 1));
        }
        Vector2 list_start_pos = ImGui.GetCursorScreenPos();

        for (int i = start; i < end; i++) {
            if (is_save) {
                if (i == 0) {
                    ui_savefile(0, new FhSaveDisplayData { slot = 0 }); // New Save Data
                } else {
                    ui_savefile(i, save_list[i - 1]);
                }
            } else {
                ui_savefile(i, save_list[i]);
            }
        }

        // Persistent Cursor on saves even when not hovered
        if (_scrollable_saves.is_within_clip(_display_index)) {
            int relative_index = _display_index - start;

            float save_x_offset = 157f * scale_factor;
            float slot_height   = 155f * scale_factor;
            float slot_spacing  = 10f * scale_factor;

            Vector2 target_save_start = list_start_pos + new Vector2(
                save_x_offset,
                relative_index * (slot_height + slot_spacing)
            );

            ui_cursor(target_save_start, scale_factor);
        }

        ui_scrollbar();

        ImGui.PopStyleVar();
        ImGui.End();
    }

    /// <summary>
    ///     Draws a custom scrollbar for the save/load menu.
    /// </summary>
    private void ui_scrollbar() {
        if (_scrollable_saves.max <= _scrollable_saves.visible) {
            return;
        }

        ImDrawListPtr draw            = ImGui.GetBackgroundDrawList();
        Vector2       mouse_pos       = ImGui.GetMousePos();
        (Vector2      window_size, _) = get_window_bounds();
        Vector2       screen_size     = new(1920f, 1080f);

        float track_start  = 207f;
        float track_end    = 957f;
        float track_height = track_end - track_start;

        float view_ratio   = Math.Clamp((float)_scrollable_saves.visible / _scrollable_saves.max, 0.08f, 1.0f); // Ensures thumb never shrinks below 8% of track height
        float thumb_height = MathF.Max(30f, track_height * view_ratio);
        float travel_dist  = track_height - thumb_height;

        // Up & Down Arrows
        (Vector2 up_su, Vector2 up_sv) = scale_screen_uv(
            screen_size,
            new(1744f, 173f),
            new(1780f, 192f)
        );

        (Vector2 down_su, Vector2 down_sv) = scale_screen_uv(
            screen_size,
            new(1744f, 972f),
            new(1780f, 991f)
        );

        Vector2 up_p1 = new((up_su.X + up_sv.X) * 0.5f, up_su.Y + 1f);
        Vector2 up_p2 = new(up_su.X, up_sv.Y);
        Vector2 up_p3 = new(up_sv.X, up_sv.Y);

        Vector2 down_p1 = new(down_su.X, down_su.Y);
        Vector2 down_p2 = new(down_sv.X, down_su.Y);
        Vector2 down_p3 = new((down_su.X + down_sv.X) * 0.5f, down_sv.Y);

        draw.AddTriangleFilled(up_p1, up_p2, up_p3, 0xFFA0A0A0);
        draw.AddTriangleFilled(down_p1, down_p2, down_p3, 0xFFA0A0A0);

        // Arrow handling
        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left, repeat: true)) {
            bool IsHovered(Vector2 corner_a, Vector2 corner_b) {
                Vector2 min = Vector2.Min(corner_a, corner_b);
                Vector2 max = Vector2.Max(corner_a, corner_b);
                return mouse_pos.X >= min.X && mouse_pos.X <= max.X &&
                       mouse_pos.Y >= min.Y && mouse_pos.Y <= max.Y;
            }

            if (IsHovered(up_su, up_sv)) {
                _scrollable_saves.scroll(-_scrollable_saves.visible);
            }
            else if (IsHovered(down_su, down_sv)) {
                _scrollable_saves.scroll(_scrollable_saves.visible);
            }
        }

        float progress    = _scrollable_saves.get_progress();
        float thumb_start = track_start + (progress * travel_dist);
        float thumb_end   = thumb_start + thumb_height;

        Vector2 tl = scale_screen_uv(screen_size, new(1755f, thumb_start), Vector2.Zero).Item1;
        Vector2 br = scale_screen_uv(screen_size, new(1768f, thumb_end)  , Vector2.Zero).Item1;

        Vector2 track_top = scale_screen_uv(screen_size, new(1755f, track_start), Vector2.Zero).Item1;
        Vector2 track_bot = scale_screen_uv(screen_size, new(1755f, track_end)  , Vector2.Zero).Item1;

        float total_travel_dist = (track_bot.Y - track_top.Y) * (travel_dist / track_height);

        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left)) {
            if (mouse_pos.X >= tl.X && mouse_pos.X <= br.X && mouse_pos.Y >= tl.Y && mouse_pos.Y <= br.Y) {
                _dragging_scrollbar = true;
                _drag_start_mouse_y = mouse_pos.Y;
                _drag_start_scroll_y = progress;
            }
        }

        if (_dragging_scrollbar) {
            if (ImGui.IsMouseDown(ImGuiMouseButton.Left)) {
                float drag_delta = mouse_pos.Y - _drag_start_mouse_y;
                float new_progress = total_travel_dist > 0f ? _drag_start_scroll_y + (drag_delta / total_travel_dist) : 0f;

                int max_steps = Math.Max(0, _scrollable_saves.max - _scrollable_saves.visible);
                _scrollable_saves.current = Math.Clamp(int.CreateChecked(Math.Round(new_progress * max_steps)), 0, max_steps);
            }
            else {
                _dragging_scrollbar = false;
            }
        }

        {
            uint clr = 0xFF000000;
            Vector2 track_tl = scale_screen_uv(screen_size, new(1753f, 959f), Vector2.Zero).Item1;
            Vector2 track_br = scale_screen_uv(screen_size, new(1770f, 205f), Vector2.Zero).Item1;
            draw.AddRectFilled(track_tl, track_br, clr); // Track
        }

        {
            uint grad_l = 0xFF808080; // Dark grey
            uint grad_r = 0xFFCBCBCB; // Light grey
            draw.AddRectFilledMultiColor(tl, br, grad_r, grad_r, grad_l, grad_l);
        }
    }

    /// <summary>
    ///     Displays "No Saved Data" if the player has no saves in the load menu.
    /// </summary>
    private void ui_no_saves() {
        if (!_texture_message.try_use(out ImTextureRef message, out _)) {
            return;
        }

        ImDrawListPtr draw = ImGui.GetBackgroundDrawList();
        ImFontPtr     font = ImGui.GetFont();

        (Vector2 window_size, _) = get_window_bounds();
        Vector2  screen_size     = new(1920f, 1080f);
        float    scale_factor    = window_size.Y / 1080f;
        float    font_size       = 42f * scale_factor;

        Vector2 tl = scale_screen_uv(screen_size, new(410f , 605f), Vector2.Zero).Item1;
        Vector2 br = scale_screen_uv(screen_size, new(1510f, 455f), Vector2.Zero).Item1;

        draw.AddRectFilled(tl, br, 0x80000000); // Transparent black background

        // Gold accents on message background
        (Vector2 message_tu, Vector2 message_tv) = scale_tex_uv(
            _tex_message_size,
            new(1469f, 1020f),
            new(1755f,  733f)
        );

        (Vector2 accent1_su, Vector2 accent1_sv) = scale_screen_uv(
            screen_size,
            new(406f, 460f),
            new(556f, 610f)
        );

        (Vector2 accent2_su, Vector2 accent2_sv) = scale_screen_uv(
            screen_size,
            new(1514f, 600f),
            new(1364f, 450f)
        );

        draw.AddImage(message, accent1_su, accent1_sv, message_tu, message_tv);
        draw.AddImage(message, accent2_su, accent2_sv, message_tu, message_tv);

        (Vector2 message_su, Vector2 message_sv) = scale_screen_uv(
            screen_size,
            new(410f , 455f),
            new(1510f, 605f)
        );

        Vector2 center = (message_su + message_sv) * 0.5f;

        string[] text = {
            "No Saved Data, please return to the Main Menu",
            "or select another Save Set."
        };

        float total_height = text.Length * font_size;
        float start_y      = center.Y - (total_height * 0.5f);

        for (int i = 0; i < text.Length; i++) {
            float current_y = start_y + (i * font_size);

            FhApi.Gui.draw_text(draw, new Vector2(center.X, current_y), text[i], font_size, true, TextAlignment.CENTER, TextAlignment.BEGIN);
        }
    }

    /// <summary>
    ///     Displays a save file slot.
    /// </summary>
    private void ui_savefile(int index, FhSaveDisplayData data) {
        if (!_texture_save.try_use(out ImTextureRef save_tex, out _)
         || !_texture_freetex.try_use(out ImTextureRef freetex, out _)
         || !_scrollable_saves.is_within_clip(index)
        ) {
            return;
        }

        ImDrawListPtr draw = ImGui.GetWindowDrawList();

        (Vector2 window_size, _) = get_window_bounds();
        float    scale_factor    = window_size.Y / 1080f;
        float    font_size       = 42f * scale_factor;

        int  slot    = data.slot;
        bool hovered = slot == _display_index;

        Vector2 window_start = ImGui.GetCursorScreenPos();

        float   save_x_offset = 157f * scale_factor;
        Vector2 save_start    = window_start + new Vector2(save_x_offset, 0f);

        float header_height  = 47f  * scale_factor;
        float vertical_space = 4f   * scale_factor;
        float info_height    = 104f * scale_factor;
        float total_height   = header_height + vertical_space + info_height;

        float target_width = 1565f * scale_factor;
        Vector2 save_size  = new(target_width, total_height);
        Vector2 save_end   = save_start + save_size;

        // Texture UV changes for each slot
        float slice_size = 64f;
        float max_y = slice_size * ((data.slot % 8) + 1);

        float uv_y_start = max_y;
        float uv_y_end   = max_y - slice_size;
        float uv_y_split = uv_y_start + (uv_y_end - uv_y_start) * (47f / 151f); // Divide Save into a header and info box

        Vector2 header_start = save_start;
        Vector2 header_end   = new(save_end.X, header_start.Y + header_height);
        Vector2 info_start   = new(save_start.X, header_end.Y + vertical_space);
        Vector2 info_end     = save_end;
        Vector2 header_size  = header_end - header_start;
        Vector2 info_size    = info_end - info_start;

        (Vector2 header_tu, Vector2 header_tv) = scale_tex_uv(
            _tex_save_size,
            new(0f  , uv_y_start),
            new(512f, uv_y_split)
        );

        (Vector2 info_tu, Vector2 info_tv) = scale_tex_uv(
            _tex_save_size,
            new(0f  , uv_y_split),
            new(512f,   uv_y_end)
        );

        // Shadows behind saves
        {
            Vector2 shadow_offset = new(10f);

            draw.AddRectFilled(
                header_start + shadow_offset,
                header_start + header_size + shadow_offset,
                0x88000000
            );

            draw.AddRectFilled(
                info_start + shadow_offset,
                info_start + info_size + shadow_offset,
                0x88000000
            );
        }

        bool is_autosave = slot == 0 && _sem!.get_system_state() is not FhSaveExtensionSystemState.SAVE;
        bool is_newsave  = slot == 0 && _sem!.get_system_state() is FhSaveExtensionSystemState.SAVE;

        // Draw Autosave darker
        if (is_autosave) {
            draw.AddImage(save_tex, header_start, header_end, header_tu, header_tv, 0xFFC6B1AF);
            draw.AddImage(save_tex, info_start, info_end, info_tu, info_tv, 0xFFC6B1AF);
            draw.AddRectFilled(header_start, header_start + header_size, 0x63000000);
        }
        else {
            draw.AddImage(save_tex, header_start, header_end, header_tu, header_tv, 0xFFE4F0F1);
            draw.AddImage(save_tex, info_start, info_end, info_tu, info_tv, 0xFFE4F0F1);
            draw.AddRectFilled(header_start, header_start + header_size, 0x33000000);
        }

        // Draw shading/edges on the save texture for definition
        float border_thickness = 5f * scale_factor;
        uint  highlight        = 0x28FFFFFF;
        uint  shadow           = 0x80000000;

        draw_highlight_shadow(draw, header_start, header_size, border_thickness, highlight, shadow);
        draw_highlight_shadow(draw, info_start, info_size, border_thickness, highlight, shadow);

        bool pressed;
        if (is_newsave) {
            Vector2 text_offset = new(16f * scale_factor, 73f * scale_factor);
            FhApi.Gui.draw_text(draw, save_start + text_offset, "New Save Data", font_size, true, TextAlignment.BEGIN, TextAlignment.BEGIN);

            uint grad_l = 0xFF191919; // grey
            uint grad_r = 0xFF252525; // black

            float box_width  = 255f * scale_factor;
            float box_height = 145f * scale_factor;
            float right_edge = 5f  * scale_factor;
            float top_edge   = 5f   * scale_factor;

            Vector2 tl = new(save_start.X + save_size.X - right_edge - box_width, save_start.Y + top_edge);
            Vector2 br = tl + new Vector2(box_width, box_height);

            draw.AddRectFilledMultiColor(tl, br, grad_r, grad_r, grad_l, grad_l);

            (Vector2 plus_tu, Vector2 plus_tv) = scale_tex_uv(
                _tex_freetex_size,
                new(581f, 760f),
                new(617f, 724f)
            );

            Vector2 box_center = (tl + br) * 0.5f;
            Vector2 icon_size  = new(40f * scale_factor, 40f * scale_factor);
            Vector2 icon_start = box_center - (icon_size * 0.5f);
            Vector2 icon_end   = icon_start + icon_size;

            draw.AddImage(freetex, icon_start, icon_end, plus_tu, plus_tv); // "+" icon

            ImGui.SetCursorScreenPos(save_start);
            ImGui.SetNextItemAllowOverlap();

            pressed = ImGui.InvisibleButton("###NewSave.Button", save_size, ImGuiButtonFlags.EnableNav | ImGuiButtonFlags.MouseButtonLeft);
            if (pressed) {
                _sem!.save(0);
            }
        } else {
            switch (FhGlobal.game_id) {
                case FhGameId.FFX2: ui_save_info_x2(data, slot, save_start, scale_factor); break;
                case FhGameId.FFX2LM: ui_save_info_x2lm(data, slot, save_start, scale_factor); break;
            }

            ImGui.SetCursorScreenPos(save_start);
            ImGui.SetNextItemAllowOverlap();

            pressed = ImGui.InvisibleButton($"###Slot{slot}.Button", save_size, ImGuiButtonFlags.EnableNav | ImGuiButtonFlags.MouseButtonLeft);
            if (pressed) {
                switch (_sem!.get_system_state()) {
                    case FhSaveExtensionSystemState.SAVE: _sem!.save(slot); break;
                    case FhSaveExtensionSystemState.LOAD: _sem!.load(slot); break;
                }
            }
        }

        hovered = ImGui.IsItemHovered();
        if (hovered) {
            _display_index = index;
        }

        // Vertical spacing between each slot
        Vector2 next_save_start = new(save_start.X, save_end.Y + 9f * scale_factor);

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, Vector2.Zero);
        ImGui.SetCursorScreenPos(next_save_start);
        ImGui.Dummy(new Vector2(1f, 1f));
        ImGui.PopStyleVar();
    }

    private void ui_cursor(Vector2 save_start, float scale_factor) {
        if (!_texture_freetex.try_use(out ImTextureRef freetex, out _)) {
            return;
        }

        ImDrawListPtr draw = ImGui.GetForegroundDrawList();

        (Vector2 cursor_tu, Vector2 cursor_tv) = scale_tex_uv(
            _tex_freetex_size,
            new(10f , 572f),
            new(175f, 480f)
        );

        float loop_progress = (float)ImGui.GetTime() * 2f % 1f;
        float base_offset   = loop_progress * 12f * scale_factor;

        Vector2 cursor_size = new(103f * scale_factor, 58f * scale_factor);

        Vector2 cursor_offset = new(-80f * scale_factor + base_offset, 80f * scale_factor);
        Vector2 cursor_start  = save_start   + cursor_offset;
        Vector2 cursor_end    = cursor_start + cursor_size;

        // Handles the trail + fade out effect of the cursor
        float ghost_progress = MathF.Max(0f, (loop_progress - 0.12f) / (1f - 0.12f));
        float ghost_x_offset = ghost_progress * 8f * scale_factor;
        float ghost_alpha    = (1f - loop_progress) * 0.75f;
        uint  ghost_color    = ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, ghost_alpha));

        Vector2 ghost_offset = new(-87f * scale_factor + ghost_x_offset, 80f * scale_factor);
        Vector2 ghost_start  = save_start  + ghost_offset;
        Vector2 ghost_end    = ghost_start + cursor_size;

        draw.AddImage(freetex, ghost_start, ghost_end, cursor_tu, cursor_tv, ghost_color);
        draw.AddImage(freetex, cursor_start, cursor_end, cursor_tu, cursor_tv);
    }

    private void ui_save_info_x2(FhSaveDisplayData data, int slot, Vector2 save_start, float scale_factor) {
        ImDrawListPtr draw = ImGui.GetWindowDrawList();
        ImFontPtr     font = ImGui.GetFont();

        float font_size = 42f * scale_factor;

        bool is_autosave = slot == 0 && _sem!.get_system_state() is not FhSaveExtensionSystemState.SAVE;

        uint   slot_text_color = is_autosave ? 0xFF19D8FF : 0xFFFFFFFF; // Yellow : White
        float  location_offset = is_autosave ? 258f : 130f;

        string slot_text        = is_autosave ? "Autosave" : Encoding.UTF8.GetString(data.slot_str);
        string location_text    = Encoding.UTF8.GetString(data.location);
        string create_time_text = Encoding.UTF8.GetString(data.create_time);
        string name_text        = Encoding.UTF8.GetString(data.player_name);
        string chapter_text     = Encoding.UTF8.GetString(data.chapter);
        string completion_text  = Encoding.UTF8.GetString(data.completion);
        string playtime_text    = Encoding.UTF8.GetString(data.play_time);

        Vector2 slot_pos          = new(17f   * scale_factor, -5f * scale_factor);
        Vector2 location_pos      = new(location_offset * scale_factor, -5f * scale_factor);
        Vector2 create_time_pos   = new(1258f * scale_factor, -5f * scale_factor);
        Vector2 name_offset       = new(356f  * scale_factor, 49f * scale_factor);
        Vector2 chapter_offset    = new(356f  * scale_factor, 98f * scale_factor);
        Vector2 completion_offset = new(1259f * scale_factor, 49f * scale_factor);
        Vector2 playtime_offset   = new(1258f * scale_factor, 98f * scale_factor);

        FhApi.Gui.draw_text(draw, save_start + slot_pos, slot_text, font_size, true, TextAlignment.BEGIN, TextAlignment.BEGIN, slot_text_color);
        FhApi.Gui.draw_text(draw, save_start + location_pos, location_text, font_size, true, TextAlignment.BEGIN, TextAlignment.BEGIN);
        FhApi.Gui.draw_text(draw, save_start + create_time_pos, create_time_text, font_size, true, TextAlignment.END, TextAlignment.BEGIN);
        FhApi.Gui.draw_text(draw, save_start + name_offset, name_text, font_size, true, TextAlignment.BEGIN, TextAlignment.BEGIN);
        FhApi.Gui.draw_text(draw, save_start + chapter_offset, chapter_text, font_size, true, TextAlignment.BEGIN, TextAlignment.BEGIN);
        FhApi.Gui.draw_text(draw, save_start + completion_offset, completion_text, font_size, true, TextAlignment.END, TextAlignment.BEGIN);
        FhApi.Gui.draw_text(draw, save_start + playtime_offset, playtime_text, font_size, true, TextAlignment.END, TextAlignment.BEGIN);

        FhSaveHeader2 header2 = MemoryMarshal.Read<FhSaveHeader2>(data.header);

        ReadOnlySpan<(byte chr_id, byte dress_id)> party = [
            (header2.id_chr1, header2.id_chr1_dress),
            (header2.id_chr2, header2.id_chr2_dress),
            (header2.id_chr3, header2.id_chr3_dress)
        ];

        (Vector2 faces_tu, Vector2 faces_tv) = scale_tex_uv(
            _tex_faces_size,
            new(0f  , 239f),
            new(256f,   0f)
        );

        Vector2 face_offset = new(5f   * scale_factor, 56f * scale_factor);
        Vector2 face_size   = new(100f * scale_factor, 94f * scale_factor);

        string filename;
        for (int i = 0; i < 3; i++) {
            (byte chr_id, byte dress_id) = party[i];

            if (dress_id != 0x0) {
                // Leblanc Goon
                if (dress_id == 0x21) filename = "mface_147.dds.phyre"; // She-Goon
                else {
                    string chr = chr_id switch {
                        0 => "yuna",
                        1 => "rikku",
                        2 => "paine",
                        _ => "m"
                    };
                    // Rewires Rikku's and Paine's Dressphere IDs to match the original filenames
                    byte dressphere = dress_id switch {
                        24 or 25 => 12,       // Trainer
                        26 or 27 => 14,       // Mascot
                        28       => 16,       // Psychic
                        29 or 30 or 31 => 17, // Festivalist
                        32       => 18,       // Freelancer
                        _        => dress_id
                    };
                    filename = $"{chr}face_{dressphere:D3}.dds.phyre";
                }

                FhTexture portrait = new(MENU_FACEDATA_DIR + filename, FhTextureType.PHYRE);

                if (!portrait.is_loaded()) {
                    FhApi.Resources.load_game_texture_2d(portrait);
                }

                if (portrait.try_use(out ImTextureRef faces, out _)) {
                    Vector2 next_offset = face_offset + new Vector2(i * 105f * scale_factor, 0f);
                    Vector2 face_start  = save_start  + next_offset;
                    Vector2 face_end    = face_start  + face_size;

                    draw.AddImage(faces, face_start, face_end, faces_tu, faces_tv);
                }
            }
        }

        /*(Vector2 map_tu, Vector2 map_tv) = scale_tex_uv(
            _tex_map_size,
            new(0f  ,   0f),
            new(320f, 176f)
        );
        
        Vector2 map_offset = new(1305f * scale_factor, 5f   * scale_factor);
        Vector2 map_size   = new(255f  * scale_factor, 145f * scale_factor);

        // TODO: Some maps use "{header2.id_location}_1"
        // Those need to be rewired like the dresspheres above
        filename = $"{header2.id_location}_0.png";
        
        FhTexture map = new(SAVEDATAICONS_DIR + filename, FhTextureType.PNG);
        
        if (!map.is_loaded()) {
            FhApi.Resources.load_game_texture_2d(map);
        }
        
        if (map.try_use(out ImTextureRef map_icon, out _)) {
            Vector2 map_start = save_start + map_offset;
            Vector2 map_end   = map_start  + map_size;
        
            draw.AddImage(map_icon, map_start, map_end, map_tu, map_tv);
        }*/
    }

    private void ui_save_info_x2lm(FhSaveDisplayData data, int slot, Vector2 save_start, float scale_factor) {
        ImDrawListPtr draw = ImGui.GetWindowDrawList();
        ImFontPtr     font = ImGui.GetFont();

        float font_size = 42f * scale_factor;

        bool is_autosave = slot == 0 && _sem!.get_system_state() is not FhSaveExtensionSystemState.SAVE;

        string slot_text        = is_autosave ? "Autosave" : Encoding.UTF8.GetString(data.slot_str);
        string location_text    = Encoding.UTF8.GetString(data.location);
        string create_time_text = Encoding.UTF8.GetString(data.create_time);
        string name_text        = Encoding.UTF8.GetString(data.player_name);
        string job_text         = Encoding.UTF8.GetString(data.lm_job);
        string level_text       = Encoding.UTF8.GetString(data.lm_level);
        string playtime_text    = Encoding.UTF8.GetString(data.play_time);

        Vector2 job_width = font.CalcTextSizeA(font_size, float.MaxValue, 0f, job_text);

        uint   slot_text_color = is_autosave ? 0xFF19D8FF : 0xFFFFFFFF; // Yellow : White
        float  location_offset = is_autosave ? 258f : 130f;
        float  level_offset    = 92f * scale_factor;

        Vector2 slot_pos        = new(17f   * scale_factor, -5f * scale_factor);
        Vector2 location_pos    = new(location_offset * scale_factor, -5f * scale_factor);
        Vector2 create_time_pos = new(1259f * scale_factor, -5f * scale_factor);
        Vector2 name_pos        = new(269f  * scale_factor, 49f * scale_factor);
        Vector2 job_pos         = new(269f  * scale_factor, 98f * scale_factor);
        Vector2 level_pos       = new(job_pos.X + job_width.X + level_offset, 98f * scale_factor);
        Vector2 playtime_pos    = new(1259f * scale_factor, 98f * scale_factor);

        FhApi.Gui.draw_text(draw, save_start + slot_pos, slot_text, font_size, true, TextAlignment.BEGIN, TextAlignment.BEGIN, slot_text_color);
        FhApi.Gui.draw_text(draw, save_start + location_pos, location_text, font_size, true, TextAlignment.BEGIN, TextAlignment.BEGIN);
        FhApi.Gui.draw_text(draw, save_start + create_time_pos, create_time_text, font_size, true, TextAlignment.END, TextAlignment.BEGIN);
        FhApi.Gui.draw_text(draw, save_start + name_pos, name_text, font_size, true, TextAlignment.BEGIN, TextAlignment.BEGIN);
        FhApi.Gui.draw_text(draw, save_start + job_pos, job_text, font_size, true, TextAlignment.BEGIN, TextAlignment.BEGIN);
        FhApi.Gui.draw_text(draw, save_start + level_pos, level_text, font_size, true, TextAlignment.BEGIN, TextAlignment.BEGIN);
        FhApi.Gui.draw_text(draw, save_start + playtime_pos, playtime_text, font_size, true, TextAlignment.END, TextAlignment.BEGIN);

        FhSaveHeader2 header2 = MemoryMarshal.Read<FhSaveHeader2>(data.header);

        (Vector2 faces_tu, Vector2 faces_tv) = scale_tex_uv(
            _tex_faces_size,
            new(0f  , 239f),
            new(256f,   0f)
        );

        Vector2 face_offset = new(5f   * scale_factor, 56f * scale_factor);
        Vector2 face_size   = new(100f * scale_factor, 94f * scale_factor);

        string filename;
        for (int i = 3; i >= 0; i--) {
            if (header2.id_job_lm != 0x0) {
                // Leblanc Goon
                if (header2.id_job_lm == 0x21) filename = "mface_147.dds.phyre"; // She-Goon
                else {
                    string chr = header2.id_chr_lm switch {
                        0 => "yuna",
                        1 => "rikku",
                        2 => "paine",
                        _ => "m"
                    };
                    // Rewires Rikku's and Paine's Dressphere IDs to match the original filenames
                    byte dressphere = header2.id_job_lm switch {
                        24 or 25 => 12,       // Trainer
                        26 or 27 => 14,       // Mascot
                        28       => 16,       // Psychic
                        29 or 30 or 31 => 17, // Festivalist
                        32       => 18,       // Freelancer
                        _        => header2.id_job_lm
                    };
                    filename = $"{chr}face_{dressphere:D3}.dds.phyre";
                }

                FhTexture portrait = new(MENU_FACEDATA_DIR + filename, FhTextureType.PHYRE);

                if (!portrait.is_loaded()) {
                    FhApi.Resources.load_game_texture_2d(portrait);
                }

                if (portrait.try_use(out ImTextureRef faces, out _)) {
                    Vector2 next_offset = face_offset + new Vector2(i * 26f * scale_factor, 0f);
                    Vector2 face_start  = save_start   + next_offset;
                    Vector2 face_end    = face_start + face_size;
                    uint    color       = ImGui.GetColorU32(new Vector4(1f, 1f, 1f, 1f - (i * 0.25f)));

                    draw.AddImage(faces, face_start, face_end, faces_tu, faces_tv, color);
                }
            }
        }

        /*(Vector2 map_tu, Vector2 map_tv) = scale_tex_uv(
            _tex_map_size,
            new(0f  ,   0f),
            new(320f, 176f)
        );
        
        Vector2 map_offset = new(1305f * scale_factor, 5f   * scale_factor);
        Vector2 map_size   = new(255f  * scale_factor, 145f * scale_factor);

        // TODO: Some maps use "{header2.id_location}_1"
        // Those need to be rewired like the dresspheres above
        filename = $"{header2.id_location}_0.png";
        
        FhTexture map = new(SAVEDATAICONS_DIR + filename, FhTextureType.PNG);
        
        if (!map.is_loaded()) {
            FhApi.Resources.load_game_texture_2d(map);
        }
        
        if (map.try_use(out ImTextureRef map_icon, out _)) {
            Vector2 map_start = save_start + map_offset;
            Vector2 map_end   = map_start  + map_size;
        
            draw.AddImage(map_icon, map_start, map_end, map_tu, map_tv);
        }*/
    }
}
