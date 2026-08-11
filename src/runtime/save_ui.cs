// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

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
public sealed class FhSaveUiModule : FhModule {

    private readonly FhModuleHandle<FhSaveExtensionModule> _sem_handle;
    private          FhSaveExtensionModule?                _sem;

    private int                   _display_index;
    private IReadOnlySet<string>? _sets;

    private bool _loaded_all_textures;

    // Scrollbar related
    private float _scroll_y;
    private float _scroll_max_y;
    private float _window_height;
    private float _drag_start_mouse_y;
    private float _drag_start_scroll_y;
    private bool  _dragging_scrollbar = false;

    private const string MENU_D3D11_DIR     = "/FFX-2_Data/GameData/PS3Data/menu/D3D11/";
    private const string MENU_MAHOJIN_DIR   = "/FFX-2_Data/GameData/PS3Data/menu/menu_mahojin/tex/D3D11/";
    private const string MENU_CLOUDSAVE_DIR = "/FFX-2_Data/GameData/PS3Data/menu/cloudsavetex/D3D11/";
    private const string MENU_FACEDATA_DIR  = "/FFX-2_Data/GameData/PS3Data/menu/face_data/D3D11/";
    private const string SAVEDATAICONS_DIR  = "/FFX-2_Data/GameData/PS3Data/savedataicons/";

    private readonly FhTexture _texture_bg      = new(MENU_D3D11_DIR     + "menuback.dds.phyre",             FhTextureType.PHYRE);
    private readonly FhTexture _texture_mahojin = new(MENU_MAHOJIN_DIR   + "14336_19_0_0_512_512.dds.phyre", FhTextureType.PHYRE);
    private readonly FhTexture _texture_save    = new(MENU_CLOUDSAVE_DIR + "texture.dds.phyre",              FhTextureType.PHYRE);
    private readonly FhTexture _texture_freetex = new(MENU_D3D11_DIR     + "freetex.dds.phyre",              FhTextureType.PHYRE);
    private readonly FhTexture _texture_bmenu1  = new(MENU_D3D11_DIR     + "b_menu1.dds.phyre",              FhTextureType.PHYRE);
    private readonly FhTexture _texture_message = new(MENU_D3D11_DIR     + "x2_bg.dds.phyre",                FhTextureType.PHYRE);

    private readonly Vector2 _tex_mahojin_size = new(2048f, 2048f);
    private readonly Vector2 _tex_save_size    = new(512f ,  512f);
    private readonly Vector2 _tex_faces_size   = new(256f ,  256f);
    private readonly Vector2 _tex_map_size     = new(320f ,  176f);
    private readonly Vector2 _tex_freetex_size = new(1024f,  768f);
    private readonly Vector2 _tex_bmenu1_size  = new(1024f, 1024f);
    private readonly Vector2 _tex_message_size = new(2048f, 2048f);

    // TODO: Location .pngs, readjust text, fix up ui_setswap

    public FhSaveUiModule() {
        _sem_handle = new(this);
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        return _sem_handle.try_get_module(out _sem);
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
            _texture_bmenu1,
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
        float   target_aspect = 16.0f / 9.0f; // Force 16:9 aspect ratio
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
            new(374f ,   -53),
            new(1600f, 1165f)
        );

        draw.AddImage(mahojin, mahojin_su, mahojin_sv, mahojin_tu, mahojin_tv); // Background glyph

        // Draws a half & half shadow over the background to match the lighting of the Remaster menu
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
    private unsafe void ui_help() {
        if (!_texture_freetex.try_use(out ImTextureRef freetex, out _)) {
            return;
        }

        ImDrawListPtr draw = ImGui.GetBackgroundDrawList();

        Vector2 display_size = ImGui.GetMainViewport().WorkSize;
        Vector2 screen_size  = new(1920f, 1080f);

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

            Vector2 tl = scale_screen_uv(screen_size, new(   0f, 141.5f), Vector2.Zero).Item1;
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
            new(240f, 92f),
            Vector2.Zero
        );

        float    base_font_size  = 50f; // Nearly perfect match to the original Flash UI font size at 1920x1080
        (Vector2 window_size, _) = get_window_bounds();
        float    scale_factor    = window_size.Y / 1080.0f;
        float    font_size       = base_font_size * scale_factor;

        draw.AddText(ImGui.GetFont(), font_size, text_su, 0xFFFFFFFF, is_save ? "Select save area" : "Select save data");
    }

    /// <summary>
    ///     Draws the set swap UI for the save/load screen.
    /// </summary>
    private unsafe void ui_setswap() {
        if (!_texture_message.try_use(out ImTextureRef message, out _))
            return;

        // This is a mess :/

        (Vector2 window_size, Vector2 window_offset) = get_window_bounds();

        Vector2 base_screen_size = new(1920f, 1080f);
        float scale_factor       = window_size.Y / 1080.0f;
        float font_size          = 50.0f * scale_factor;

        ImDrawListPtr draw = ImGui.GetBackgroundDrawList();
        ImFontPtr font     = ImGui.GetFont();

        Vector2 tl = scale_screen_uv(base_screen_size, new(692f, 22f), Vector2.Zero).Item1;
        Vector2 br = scale_screen_uv(base_screen_size, new(1244f, 88f), Vector2.Zero).Item1;
        draw.AddRectFilled(tl, br, 0x80000000); // Transparent black message box

        (Vector2 tex_uv1, Vector2 tex_uv2) = scale_tex_uv(
            _tex_message_size,
            new(1469f, 1020f),
            new(1755f, 733f)
        );

        (Vector2 accent1_tl, Vector2 accent1_br) = scale_screen_uv(
            base_screen_size,
            new(690f, 90f),
            new(760f, 20f)
        );

        (Vector2 accent2_tl, Vector2 accent2_br) = scale_screen_uv(
            base_screen_size,
            new(1246f, 20f),
            new(1176f, 90f)
        );

        draw.AddImage(message, accent1_tl, accent1_br, tex_uv1, tex_uv2);
        draw.AddImage(message, accent2_tl, accent2_br, tex_uv1, tex_uv2);

        string save_count = FhInternal.Saves.get_slots_used() == 1 ? "(1 Save)" : $"({FhInternal.Saves.get_slots_used()} Saves)";

        Vector2 text_offset = scale_screen_uv(base_screen_size, new(1717f, 121f), Vector2.Zero).Item1;
        float   text_width  = font.CalcTextSizeA(font_size, float.MaxValue, 0f, save_count).X;
        Vector2 text_start  = new(text_offset.X - text_width, text_offset.Y);

        draw.AddText(font, font_size, text_start, 0xFFFFFFFF, save_count); // (X Saves), drawn right to left

        Vector2 box_size = br - tl;
        ImGui.SetNextWindowPos(tl);
        ImGui.SetNextWindowSize(box_size);

        ImGuiWindowFlags flags = FhApi.ImGuiHelper.WINDOW_FLAGS_FULLSCREEN
        & ~ImGuiWindowFlags.NoNavFocus
        | ImGuiWindowFlags.NoBackground;

        if (!ImGui.Begin("Set Swap###Fh.Runtime.SaveSystem.SetSwap", flags)) {
            ImGui.End();
            return;
        }

        string active_set = FhInternal.Saves.get_active_set();
        Vector2 set_label_size = font.CalcTextSizeA(font_size, float.MaxValue, 0f, active_set);

        Vector2 win_pos       = ImGui.GetWindowPos();
        Vector2 win_size      = ImGui.GetWindowSize();
        Vector2 set_label_pos = win_pos + new Vector2((win_size.X - set_label_size.X) * 0.5f, 4.0f * scale_factor);

        ImGui.SetCursorScreenPos(set_label_pos);

        if (ImGui.Selectable("###ActiveSetSelectable", false, ImGuiSelectableFlags.None, set_label_size)) {
            _sets = FhInternal.Saves.get_sets();
            ImGui.OpenPopup("Select Set"u8);
        }

        draw.AddText(font, font_size, set_label_pos, 0xFFFFFFFF, active_set);

        Vector2 modal_size = new(Math.Max(600f, window_size.X * 0.35f), Math.Max(400f, window_size.Y * 0.7f));
        Vector2 modal_pos  = window_offset + (window_size - modal_size) * 0.5f;

        ImGui.SetNextWindowPos(modal_pos);
        ImGui.SetNextWindowSize(modal_size);

        if (ImGui.BeginPopupModal("Select Set") && _sets != null) {
            foreach (string set in _sets) {
                bool is_selected = set == active_set;

                if (ImGui.Selectable(set, is_selected)) {
                    FhInternal.Saves.switch_active_set(set);
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

        float scale_factor = window_size.Y / 1080.0f;

        Vector2 offset = window_offset + new Vector2(window_size.X * (146f / 1920f), window_size.Y * (161f / 1080f));
        Vector2 size   = new(window_size.X * (1587f / 1920f), window_size.Y * (828f / 1080f));

        ImGui.SetNextWindowPos(offset);
        ImGui.SetNextWindowSize(size);

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);

        ImGuiWindowFlags flags = FhApi.ImGuiHelper.WINDOW_FLAGS_FULLSCREEN
        & ~ImGuiWindowFlags.NoNavFocus
        | ImGuiWindowFlags.NoScrollbar
        | ImGuiWindowFlags.NoBackground;

        if (!ImGui.Begin("Save/Load###Fh.Runtime.SaveSystem.SaveLoadUI", flags)) {
            ImGui.PopStyleVar();
            ImGui.End();
            return;
        }

        ImGui.SetCursorPos(new Vector2(0.0f, 11.0f * scale_factor));

        _scroll_y      = ImGui.GetScrollY();
        _scroll_max_y  = ImGui.GetScrollMaxY();
        _window_height = ImGui.GetWindowHeight();

        bool is_save = _sem!.get_system_state() is FhSaveExtensionSystemState.SAVE;

        if (is_save) {
            ui_new_save();
        }

        if (!is_save && FhInternal.Saves.get_display_data().Count < 1) {
            ui_no_saves();
            ImGui.Dummy(new Vector2(0, 11.0f * scale_factor));
            ImGui.PopStyleVar();
            ImGui.End();
            return;
        }

        foreach (FhSaveDisplayData save_file in FhInternal.Saves.get_display_data()) {
            ui_savefile(save_file);
        }

        ui_scrollbar();

        ImGui.PopStyleVar();
        ImGui.End();
    }

    /// <summary>
    ///     Draws a custom scrollbar for the save/load menu.
    /// </summary>
    private void ui_scrollbar() {
        if (!_texture_bmenu1.try_use(out ImTextureRef b_menu1, out _)
          || _scroll_max_y <= 0f) {
            return;
        }

        ImDrawListPtr draw        = ImGui.GetBackgroundDrawList();
        Vector2       screen_size = new(1920f, 1080f);
        Vector2       mouse_pos   = ImGui.GetMousePos();

        float track_start  = 207f;
        float track_end    = 957f;
        float track_height = track_end - track_start;

        float saves_height    = _scroll_max_y + _window_height; // Total height of save slot window that can be scrolled through
        float view_ratio      = saves_height > 0f ? Math.Clamp(_window_height / saves_height, 0.05f, 1.0f) : 1.0f; // Ensures thumb never shrinks below 5% of track height
        float thumb_height    = MathF.Max(30f, track_height * view_ratio);
        float travel_distance = track_height - thumb_height;

        Vector2 track_top = scale_screen_uv(screen_size, new(1755f, track_start), Vector2.Zero).Item1;
        Vector2 track_bot = scale_screen_uv(screen_size, new(1755f, track_end), Vector2.Zero).Item1;

        float total_track_height    = track_bot.Y - track_top.Y;
        float total_travel_distance = total_track_height * (travel_distance / track_height);

        if (!_dragging_scrollbar) {
            _scroll_y = ImGui.GetScrollY();
        }

        (Vector2 up_su, Vector2 up_sv) = scale_screen_uv(
            screen_size,
            new(1744f, 172f),
            new(1780f, 191f)
        );

        (Vector2 down_su, Vector2 down_sv) = scale_screen_uv(
            screen_size,
            new(1744f, 971f),
            new(1780f, 990f)
        );

        (Vector2 up_tu, Vector2 up_tv) = scale_tex_uv(
            _tex_bmenu1_size,
            new(965f, 913f),
            new(1019f, 943f)
        );

        (Vector2 down_tu, Vector2 down_tv) = scale_tex_uv(
            _tex_bmenu1_size,
            new(965f, 943f),
            new(1019f, 913f)
        );

        // Up & Down arrows
        draw.AddImage(b_menu1, up_su, up_sv, up_tu, up_tv);
        draw.AddImage(b_menu1, down_su, down_sv, down_tu, down_tv);

        // Arrow handling
        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left, repeat: true)) {
            float scroll_jump = 825f; // Skips ~5 saves

            bool IsHovered(Vector2 corner_a, Vector2 corner_b) {
                Vector2 min = Vector2.Min(corner_a, corner_b);
                Vector2 max = Vector2.Max(corner_a, corner_b);
                return mouse_pos.X >= min.X && mouse_pos.X <= max.X &&
                       mouse_pos.Y >= min.Y && mouse_pos.Y <= max.Y;
            }

            if (IsHovered(up_su, up_sv)) {
                _scroll_y = Math.Clamp(_scroll_y - scroll_jump, 0f, _scroll_max_y);
                ImGui.SetScrollY(_scroll_y);
            }
            else if (IsHovered(down_su, down_sv)) {
                _scroll_y = Math.Clamp(_scroll_y + scroll_jump, 0f, _scroll_max_y);
                ImGui.SetScrollY(_scroll_y);
            }
        }

        float scroll_ratio = _scroll_max_y > 0f ? Math.Clamp(_scroll_y / _scroll_max_y, 0f, 1f) : 0f;
        float thumb_start  = track_start + (scroll_ratio * travel_distance);
        float thumb_end    = thumb_start + thumb_height;

        Vector2 tl = scale_screen_uv(screen_size, new(1755f, thumb_start), Vector2.Zero).Item1;
        Vector2 br = scale_screen_uv(screen_size, new(1768f, thumb_end), Vector2.Zero).Item1;

        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left)) {
            if (mouse_pos.X >= tl.X && mouse_pos.X <= br.X && mouse_pos.Y >= tl.Y && mouse_pos.Y <= br.Y) {
                _dragging_scrollbar = true;
                _drag_start_mouse_y = mouse_pos.Y;
                _drag_start_scroll_y = _scroll_y;
            }
        }

        if (_dragging_scrollbar) {
            if (ImGui.IsMouseDown(ImGuiMouseButton.Left)) {
                float drag_distance = mouse_pos.Y - _drag_start_mouse_y;
                float scroll_amount = total_travel_distance > 0f ? (drag_distance / total_travel_distance) * _scroll_max_y : 0f;

                _scroll_y = Math.Clamp(_drag_start_scroll_y + scroll_amount, 0f, _scroll_max_y);
                ImGui.SetScrollY(_scroll_y);
            }
            else {
                _dragging_scrollbar = false;
            }
        }

        {
            uint clr = 0xFF000000;
            Vector2 track_tl = scale_screen_uv(screen_size, new(1753f, 959f), Vector2.Zero).Item1;
            Vector2 track_br = scale_screen_uv(screen_size, new(1770f, 205f), Vector2.Zero).Item1;
            draw.AddRectFilledMultiColor(track_tl, track_br, clr, clr, clr, clr); // Track
        }

        {
            uint grad_l = 0xFF808080; // Dark grey
            uint grad_r = 0xFFCBCBCB; // Light grey
            draw.AddRectFilledMultiColor(tl, br, grad_r, grad_r, grad_l, grad_l); // Thumb
        }
    }

    /// <summary>
    ///     Displays "No Saved Data" if the player has no saves in the load menu.
    /// </summary>
    private unsafe void ui_no_saves() {
        if (!_texture_message.try_use(out ImTextureRef message, out _)) {
            return;
        }

        ImDrawListPtr draw = ImGui.GetBackgroundDrawList();
        ImFontPtr     font = ImGui.GetFont();

        Vector2 display_size = ImGui.GetMainViewport().WorkSize;
        Vector2 screen_size  = new(1920f, 1080f);

        Vector2 tl = scale_screen_uv(screen_size, new(482f , 605f), Vector2.Zero).Item1;
        Vector2 br = scale_screen_uv(screen_size, new(1425f, 455f), Vector2.Zero).Item1;

        draw.AddRectFilled(tl, br, 0x80000000); // Transparent black background

        (Vector2 message_tu, Vector2 message_tv) = scale_tex_uv(
            _tex_message_size,
            new(1469f, 1020f),
            new(1755f,  733f)
        );

        (Vector2 accent1_su, Vector2 accent1_sv) = scale_screen_uv(
            screen_size,
            new(478f, 600f),
            new(628f, 450f)
        );

        (Vector2 accent2_su, Vector2 accent2_sv) = scale_screen_uv(
            screen_size,
            new(1429f, 460f),
            new(1279f, 610f)
        );

        // Gold accents in the top left and bottom right corners of the message box
        draw.AddImage(message, accent1_su, accent1_sv, message_tu, message_tv);
        draw.AddImage(message, accent2_su, accent2_sv, message_tu, message_tv);

        // Total size of the message box
        (Vector2 message_su, Vector2 message_sv) = scale_screen_uv(
            screen_size,
            new(482f , 455f),
            new(1425f, 605f)
        );

        Vector2 screen_center = (message_su + message_sv) * 0.5f;

        (Vector2 text_su, _) = scale_screen_uv(
            screen_size,
            new(240f, 92f),
            Vector2.Zero
        );

        float    base_font_size  = 50f;
        (Vector2 window_size, _) = get_window_bounds();
        float    scale_factor    = window_size.Y / 1080.0f;
        float    font_size       = base_font_size * scale_factor;
        string[] text            = {
            "No Saved Data, please return to the Main Menu",
            "or select another Save Set."
        };

        float total_height = text.Length * font_size;
        float text_y       = screen_center.Y - (total_height * 0.5f);

        for (int i = 0; i < text.Length; i++) {
            Vector2 text_size = font.CalcTextSizeA(font_size, float.MaxValue, 0.0f, text[i]);

            float current_text_x = screen_center.X - (text_size.X * 0.5f);
            float current_text_y = text_y + (i * font_size);

            draw.AddText(font, font_size, new Vector2(current_text_x, current_text_y), 0xFFFFFFFF, text[i]);
        }
    }

    /// <summary>
    ///     Draws the "New Save Data" option for the save menu.
    /// </summary>
    private unsafe void ui_new_save() {
        if (!_texture_save.try_use(out ImTextureRef save_tex, out _)
         || !_texture_freetex.try_use(out ImTextureRef freetex, out _)
         ) {
            return;
        }

        ImDrawListPtr draw      = ImGui.GetWindowDrawList();
        ImDrawListPtr draw_fg   = ImGui.GetForegroundDrawList();
        ImDrawListPtr slot_draw = ImGui.GetWindowDrawList();
        ImFontPtr     font      = ImGui.GetFont();

        (Vector2 window_size, _) = get_window_bounds();
        float    scale_factor    = window_size.Y / 1080.0f;

        Vector2 save_start  = ImGui.GetCursorScreenPos();
        Vector2 avail_width = new(ImGui.GetContentRegionAvail().X, 0F);

        float target_height = 155.0f * scale_factor;

        Vector2 save_size = new(avail_width.X, target_height);
        Vector2 save_end  = save_start + save_size;

        (Vector2 save_tu, Vector2 save_tv) = scale_tex_uv(
            _tex_save_size,
            new(0f  , 506f),
            new(511f, 375f)
        );

        draw.AddImage(save_tex, save_start, save_end, save_tu, save_tv); // Save slot

        float   font_size   = 50f * scale_factor;
        Vector2 text_offset = new(25f * scale_factor, 7f * scale_factor);

        slot_draw.AddText(font, font_size, save_start + text_offset, 0xFFFFFFFF, "New Save Data");

        {
            uint grad_l = 0xFF191919; // grey
            uint grad_r = 0xFF252525; // black

            float box_width  = 255f * scale_factor;
            float box_height = 138f * scale_factor;
            float right_edge = 19f  * scale_factor;
            float top_edge   = 9f   * scale_factor;

            Vector2 tl = new(save_start.X + save_size.X - right_edge - box_width, save_start.Y + top_edge);
            Vector2 br = tl + new Vector2(box_width, box_height);

            draw.AddRectFilledMultiColor(tl, br, grad_r, grad_r, grad_l, grad_l);

            (Vector2 plus_tu, Vector2 plus_tv) = scale_tex_uv(
                _tex_freetex_size,
                new(581f, 760f),
                new(617f, 724f)
            );

            Vector2 icon_size = new(40f * scale_factor, 40f * scale_factor);

            Vector2 box_center = (tl + br) * 0.5f;
            Vector2 icon_start = box_center - (icon_size * 0.5f);
            Vector2 icon_end   = icon_start + icon_size;

            draw.AddImage(freetex, icon_start, icon_end, plus_tu, plus_tv); // + symbol
        }

        ImGui.SetCursorScreenPos(save_start);
        ImGui.SetNextItemAllowOverlap();

        bool pressed = ImGui.InvisibleButton("###NewSave.Button", save_size, ImGuiButtonFlags.EnableNav | ImGuiButtonFlags.MouseButtonLeft);
        if (pressed) {
            _sem!.save(0);
        }

        bool hovered = ImGui.IsItemHovered();
        if (hovered) {
            _display_index = 0;
            ui_cursor(save_start, scale_factor);
        }

        // Vertical spacing between each slot
        Vector2 next_save_start = new(save_start.X, save_end.Y + 9.0f * scale_factor);

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, Vector2.Zero);
        ImGui.SetCursorScreenPos(next_save_start);
        ImGui.Dummy(new Vector2(1.0f, 1.0f));
        ImGui.PopStyleVar();
    }

    /// <summary>
    ///     Displays a save file slot.
    /// </summary>
    private void ui_savefile(FhSaveDisplayData data) {
        if (data.slot == 0 && _sem!.get_system_state() is FhSaveExtensionSystemState.SAVE
        || !_texture_save.try_use(out ImTextureRef save_tex, out _)
        ) {
            return;
        }

        ImDrawListPtr draw = ImGui.GetWindowDrawList();

        (Vector2 window_size, _) = get_window_bounds();
        float    scale_factor    = window_size.Y / 1080.0f;

        Vector2 save_start = ImGui.GetCursorScreenPos();
        Vector2 avail_width = new(ImGui.GetContentRegionAvail().X, 0F);

        int slot = data.slot;
        bool hovered = slot == _display_index;

        float   target_height = 155.0f * scale_factor;
        Vector2 slot_size     = new(avail_width.X, target_height);
        Vector2 save_end      = save_start + slot_size;

        (Vector2 save_tu, Vector2 save_tv) = scale_tex_uv(
            _tex_save_size,
            new(0f  , 506f),
            new(511f, 375f)
        );

        draw.AddImage(save_tex, save_start, save_end, save_tu, save_tv); // Save texture

        ImGui.PushStyleColor(ImGuiCol.ChildBg, 0x00000000);
        if (ImGui.BeginChild($"###Slot{slot}", slot_size, ImGuiChildFlags.None, FhApi.ImGuiHelper.WINDOW_FLAGS_FULLSCREEN | ImGuiWindowFlags.NoInputs)) {
            ImGui.Indent();
            ui_save_info_generic(data, slot, save_start, scale_factor);
            ImGui.Unindent();

            ImGui.Indent();
            switch (FhGlobal.game_id) {
                case FhGameId.FFX: ui_save_info_x(data); break;
                case FhGameId.FFX2: ui_save_info_x2(data, save_start, scale_factor); break;
                case FhGameId.FFX2LM: ui_save_info_x2lm(data, save_start, scale_factor); break;
            }
            ImGui.Unindent();
        }
        ImGui.EndChild();
        ImGui.PopStyleColor();

        ImGui.SetCursorScreenPos(save_start);
        ImGui.SetNextItemAllowOverlap();
        bool pressed = ImGui.InvisibleButton($"###Slot{slot}.Button", slot_size, ImGuiButtonFlags.EnableNav | ImGuiButtonFlags.MouseButtonLeft);
        if (pressed) {
            switch (_sem!.get_system_state()) {
                case FhSaveExtensionSystemState.SAVE: _sem!.save(slot); break;
                case FhSaveExtensionSystemState.LOAD: _sem!.load(slot); break;
                case FhSaveExtensionSystemState.ALBD: _sem!.load_albd(slot); break;
            }
        }
        hovered = ImGui.IsItemHovered();
        if (hovered) {
            _display_index = slot;
            ui_cursor(save_start, scale_factor);
        }

        // Vertical spacing between each slot
        Vector2 next_save_start = new(save_start.X, save_end.Y + 9.0f * scale_factor);

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, Vector2.Zero);
        ImGui.SetCursorScreenPos(next_save_start);
        ImGui.Dummy(new Vector2(1.0f, 1.0f));
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

        Vector2 cursor_size = new(103f * scale_factor, 58f * scale_factor);

        float loop_progress = ((float)ImGui.GetTime() * 2f) % 1f;
        float base_offset   = loop_progress * 12f * scale_factor;

        Vector2 cursor_offset = new((-80f * scale_factor) + base_offset, 8f * scale_factor);
        Vector2 cursor_start  = save_start + cursor_offset;
        Vector2 cursor_end    = cursor_start + cursor_size;

        // Handles the trail + fade out effect of the cursor
        float ghost_progress = MathF.Max(0f, (loop_progress - 0.12f) / (1f - 0.12f));
        float ghost_x_offset = ghost_progress * 8f * scale_factor;
        float ghost_alpha    = (1f - loop_progress) * 0.75f;
        uint  ghost_color    = ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, ghost_alpha));
        Vector2 ghost_offset = new((-87f * scale_factor) + ghost_x_offset, 8f * scale_factor);
        Vector2 ghost_start  = save_start + ghost_offset;
        Vector2 ghost_end    = ghost_start + cursor_size;

        draw.AddImage(freetex, ghost_start, ghost_end, cursor_tu, cursor_tv, ghost_color);
        draw.AddImage(freetex, cursor_start, cursor_end, cursor_tu, cursor_tv, 0xFFFFFFFF);
    }

    private unsafe void ui_save_info_generic(FhSaveDisplayData data, int slot, Vector2 save_start, float scale_factor) {
        ImDrawListPtr draw = ImGui.GetWindowDrawList();
        ImFontPtr     font = ImGui.GetFont();

        float font_size = 50f * scale_factor;

        bool   is_autosave = slot == 0 && _sem!.get_system_state() is not FhSaveExtensionSystemState.SAVE;

        string slot_text       = is_autosave ? "Autosave" : Encoding.UTF8.GetString(((ReadOnlySpan<byte>)data.slot_str).TrimEnd((byte)0));
        uint   slot_text_color = is_autosave ? 0xFF17BEE0 : 0xFFFFFFFF; // Yellow : White
        float  location_offset = is_autosave ? 269f : 119f;

        Vector2 slot_offset          = new(25f * scale_factor, 7f * scale_factor);
        Vector2 location_true_offset = new(location_offset * scale_factor, 7f * scale_factor);

        draw.AddText(font, 50f * scale_factor, save_start + slot_offset, slot_text_color, slot_text);
        draw.AddText(font, font_size, save_start + location_true_offset, 0xFFFFFFFF, data.location);

        float   base_offset          = 1259f * scale_factor;
        float   creation_time_width  = font.CalcTextSizeA(font_size, float.MaxValue, 0f, data.create_time).X;
        Vector2 creation_time_offset = new(base_offset - creation_time_width, 7f * scale_factor);

        draw.AddText(font, font_size, save_start + creation_time_offset, 0xFFFFFFFF, data.create_time);
    }

    private void ui_save_info_x(FhSaveDisplayData data) {
        ImGui.Text(data.player_name);
        ImGui.Text(data.play_time);
    }

    private unsafe void ui_save_info_x2(FhSaveDisplayData data, Vector2 save_start, float scale_factor) {
        ImDrawListPtr draw = ImGui.GetWindowDrawList();
        ImFontPtr     font = ImGui.GetFont();

        float font_size = 50f * scale_factor;

        Vector2 name_offset    = new(359f * scale_factor, 55f * scale_factor);
        Vector2 chapter_offset = new(359f * scale_factor, 101f * scale_factor);

        draw.AddText(font, font_size, save_start + name_offset, 0xFFFFFFFF, data.player_name);
        draw.AddText(font, font_size, save_start + chapter_offset, 0xFFFFFFFF, data.chapter);

        float   base_offset       = 1259f * scale_factor;
        float   completion_width  = font.CalcTextSizeA(font_size, float.MaxValue, 0f, data.completion).X;
        Vector2 completion_offset = new(base_offset - completion_width, 55f * scale_factor);

        float   playtime_width  = font.CalcTextSizeA(font_size, float.MaxValue, 0f, data.play_time).X;
        Vector2 playtime_offset = new(base_offset - playtime_width, 101f * scale_factor);

        draw.AddText(font, font_size, save_start + completion_offset, 0xFFFFFFFF, data.completion);
        draw.AddText(font, font_size, save_start + playtime_offset, 0xFFFFFFFF, data.play_time);

        ReadOnlySpan<byte> header_span = data.header;
        ref readonly FhSaveHeader2 header2 = ref MemoryMarshal.AsRef<FhSaveHeader2>(header_span);

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

        Vector2 face_offset = new(19.0f * scale_factor, 61.0f * scale_factor);
        Vector2 face_size   = new(92.0f * scale_factor, 86.0f * scale_factor);

        for (int i = 0; i < 3; i++) {
            (byte chr_id, byte dress_id) = party[i];

            string filename;

            // No Dressphere
            if (dress_id == 0x0) filename = "mface_000.dds.phyre"; // Empty
            // Leblanc Goon
            else if (dress_id == 0x21) filename = "mface_147.dds.phyre"; // She-Goon
            else {
                string chr = chr_id switch {
                    0 => "yuna",
                    1 => "rikku",
                    2 => "paine",
                    _ => "m"
                };
                // Rewires Rikku and Paine's Dressphere IDs to match the original filenames
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
                float x_spacing = 95.0f * scale_factor;

                Vector2 next_offset = face_offset + new Vector2(i * x_spacing, 0f);
                Vector2 face_start  = save_start + next_offset;
                Vector2 face_end    = face_start + face_size;

                draw.AddImage(faces, face_start, face_end, faces_tu, faces_tv);
            }
        }
    }

    private unsafe void ui_save_info_x2lm(FhSaveDisplayData data, Vector2 save_start, float scale_factor) {
        ImDrawListPtr draw = ImGui.GetWindowDrawList();
        ImFontPtr     font = ImGui.GetFont();

        float font_size = 50f * scale_factor;

        Vector2 name_offset       = new(272f * scale_factor, 55f * scale_factor);
        Vector2 dressphere_offset = new(272f * scale_factor, 101f * scale_factor);
        Vector2 level_offset      = new(544f * scale_factor, 101f * scale_factor);

        draw.AddText(font, font_size, save_start + name_offset, 0xFFFFFFFF, data.player_name);
        draw.AddText(font, font_size, save_start + dressphere_offset, 0xFFFFFFFF, data.lm_job);
        draw.AddText(font, font_size, save_start + level_offset, 0xFFFFFFFF, data.lm_level);

        float   base_offset     = 1259f * scale_factor;
        float   playtime_width  = font.CalcTextSizeA(font_size, float.MaxValue, 0f, data.play_time).X;
        Vector2 playtime_offset = new(base_offset - playtime_width, 101f * scale_factor);

        draw.AddText(font, font_size, save_start + playtime_offset, 0xFFFFFFFF, data.play_time);

        ReadOnlySpan<byte> header_span = data.header;
        ref readonly FhSaveHeader2 header2 = ref MemoryMarshal.AsRef<FhSaveHeader2>(header_span);

        (Vector2 faces_tu, Vector2 faces_tv) = scale_tex_uv(
            _tex_faces_size,
            new(0f  , 239f),
            new(256f,   0f)
        );

        Vector2 face_offset = new(19.0f * scale_factor, 61.0f * scale_factor);
        Vector2 face_size   = new(92.0f * scale_factor, 86.0f * scale_factor);

        for (int i = 3; i >= 0; i--) {
            string filename;

            // No Dressphere
            if (header2.id_job_lm == 0x0) filename = "mface_000.dds.phyre"; // Empty
            // Leblanc Goon
            else if (header2.id_job_lm == 0x21) filename = "mface_147.dds.phyre"; // She-Goon
            else {
                string chr = header2.id_chr_lm switch {
                    0 => "yuna",
                    1 => "rikku",
                    2 => "paine",
                    _ => "m"
                };
                // Rewires Rikku and Paine's Dressphere IDs to match the original filenames
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
                float x_spacing = 21.0f * scale_factor;

                Vector2 next_offset = face_offset + new Vector2(i * x_spacing, 0f);
                Vector2 face_start  = save_start + next_offset;
                Vector2 face_end    = face_start + face_size;

                uint color = ImGui.GetColorU32(new Vector4(1f, 1f, 1f, 1f - (i * 0.25f)));

                draw.AddImage(faces, face_start, face_end, faces_tu, faces_tv, color);
            }
        }
    }
}
