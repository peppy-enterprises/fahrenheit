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
public sealed class FhSaveUiModule : FhModule {
    private class ScrollableData {
        public int current;
        public int max;
        public int visible;
        public int hovered;

        public void reset() {
            current = 0;
            hovered = 0;
        }

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
            int old_current = current;

            current += amount;
            current = Math.Clamp(current, 0, max - visible);

            if (is_within_clip(hovered)) {
                if (current != old_current) {
                    hovered += amount;
                    hovered = Math.Clamp(hovered, 0, max - 1);
                }
            }
            else {
                // Clip the hovered index to the range of visible indices so it never goes off-screen
                if (Math.Sign(amount) > 0) {
                    hovered = get_clip_start();
                } else {
                    hovered = get_clip_end() - 1;
                }
            }
            // New Save button handling
            hovered = Math.Clamp(hovered, 0, max - 1);
        }

        public void move_hover(int amount) {
            hovered += amount;
            hovered = Math.Clamp(hovered, 0, max - 1);

            if (is_within_clip(hovered)) return;

            // Move the clip to the hovered index
            if (hovered < get_clip_start()) {
                current = hovered;
            }
            else {
                current = hovered - visible + 1;
            }
        }

        public void scroll_begin() {
            current = hovered = 0;
        }

        public void scroll_end() {
            current = max - visible;
            hovered = max - 1;
        }

        public void handle_input() {
            // Various scrolling methods
            bool hover_up   =
                ImGui.IsKeyPressed(ImGuiKey.W)
             || ImGui.IsKeyPressed(ImGuiKey.UpArrow)
             || ImGui.IsKeyPressed(ImGuiKey.GamepadDpadUp)
             || ImGui.IsKeyPressed(ImGuiKey.GamepadLStickUp);

            bool hover_down =
                ImGui.IsKeyPressed(ImGuiKey.S)
             || ImGui.IsKeyPressed(ImGuiKey.DownArrow)
             || ImGui.IsKeyPressed(ImGuiKey.GamepadDpadDown)
             || ImGui.IsKeyPressed(ImGuiKey.GamepadLStickDown);

            float mouse_wheel = ImGui.GetIO().MouseWheel;

            bool scroll_page_up =
                ImGui.IsKeyPressed(ImGuiKey.PageUp)
             || ImGui.IsKeyPressed(ImGuiKey.A)
             || ImGui.IsKeyPressed(ImGuiKey.LeftArrow)
             || ImGui.IsKeyPressed(ImGuiKey.GamepadDpadLeft)
             || ImGui.IsKeyPressed(ImGuiKey.GamepadL1);

            bool scroll_page_down =
                ImGui.IsKeyPressed(ImGuiKey.PageDown)
             || ImGui.IsKeyPressed(ImGuiKey.D)
             || ImGui.IsKeyPressed(ImGuiKey.RightArrow)
             || ImGui.IsKeyPressed(ImGuiKey.GamepadDpadRight)
             || ImGui.IsKeyPressed(ImGuiKey.GamepadR1);

            bool scroll_to_start =
                ImGui.IsKeyPressed(ImGuiKey.Home)
             || ImGui.IsKeyPressed(ImGuiKey.GamepadL2);

            bool scroll_to_end =
                ImGui.IsKeyPressed(ImGuiKey.End)
             || ImGui.IsKeyPressed(ImGuiKey.GamepadR2);

            ImGui.GetIO().WantCaptureKeyboard |=
                hover_up
             || hover_down
             || scroll_page_up
             || scroll_page_down
             || scroll_to_start
             || scroll_to_end;

            if (hover_up) {
                move_hover(-1);
            }

            if (hover_down) {
                move_hover(1);
            }

            if (mouse_wheel > 0) {
                scroll(-1);
            }

            if (mouse_wheel < 0) {
                scroll(1);
            }

            if (scroll_page_up) {
                if (current == 0) {
                    hovered = 0;
                }
                else {
                    scroll(-visible);
                }
            }

            if (scroll_page_down) {
                if (current == max - visible) {
                    hovered = max - 1;
                }
                else {
                    scroll(visible);
                }
            }

            if (scroll_to_start) {
                scroll_begin();
            }

            if (scroll_to_end) {
                scroll_end();
            }
        }
    }

    /// <summary>Possible open windows of the save/load menu.</summary>
    public enum FhSaveUiMode {
        /// <summary>The list of saves to save/load/compile from.</summary>
        SAVE_LIST = 0,

        /// <summary>The set save selection window.</summary>
        SET_SWAP = 1,

        /// <summary>The popup for displaying save/load errors and notifications.</summary>
        SAVE_POPUP = 2,
    }

    /// <summary>Targets the player can put their cursor on.</summary>
    public enum FhSaveUiFocus {
        /// <summary>The scrollable list of either saves or sets.</summary>
        LIST = 0,

        /// <summary>The active set button that opens the set list.</summary>
        ACTIVE_SET = 1,

        //TODO: Add sorting and sort selection button
    }

    public FhSaveUiMode mode { get; private set; }
    public FhSaveUiFocus focus { get; private set; }

    private readonly FhModuleHandle<FhSaveExtensionModule> _sem_handle;
    private          FhSaveExtensionModule?                _sem;

    private bool _loaded_all_textures;

    // Scrollbar related
    private float _drag_start_mouse_y;
    private float _drag_start_scroll_y;
    private bool  _dragging_scrollbar = false;

    private const string MENU_D3D11_DIR    = "/FFX-2_Data/GameData/PS3Data/menu/D3D11/";
    private const string MENU_MAHOJIN_DIR  = "/FFX-2_Data/GameData/PS3Data/menu/menu_mahojin/tex/D3D11/";
    private const string MENU_PLATE_DIR    = "/FFX-2_Data/GameData/PS3Data/menu/menu_plate/tex/D3D11/";
    private const string MENU_FACEDATA_DIR = "/FFX-2_Data/GameData/PS3Data/menu/face_data/D3D11/";
    private const string SAVEDATAICONS_DIR = "/FFX-2_Data/GameData/PS3Data/savedataicons/";

    private readonly FhTexture _texture_bg      = new(MENU_D3D11_DIR   + "menuback.dds.phyre",             FhTextureType.PHYRE);
    private readonly FhTexture _texture_mahojin = new(MENU_MAHOJIN_DIR + "14336_19_0_0_512_512.dds.phyre", FhTextureType.PHYRE);
    private readonly FhTexture _texture_save    = new(MENU_PLATE_DIR   + "12288_19_0_0_256_256.dds.phyre", FhTextureType.PHYRE);
    private readonly FhTexture _texture_freetex = new(MENU_D3D11_DIR   + "freetex.dds.phyre",              FhTextureType.PHYRE);
    private readonly FhTexture _texture_message = new(MENU_D3D11_DIR   + "x2_bg.dds.phyre",                FhTextureType.PHYRE);

    private readonly Vector2 _tex_mahojin_size = new(2048f, 2048f);
    private readonly Vector2 _tex_save_size    = new(512f ,  512f);
    private readonly Vector2 _tex_faces_size   = new(256f ,  256f);
    private readonly Vector2 _tex_map_size     = new(320f ,  176f);
    private readonly Vector2 _tex_freetex_size = new(1024f,  768f);
    private readonly Vector2 _tex_message_size = new(2048f, 2048f);

    /* TODO: 
     * Translate Autosave, Help, New Save Data, and Save Count text
     * Split Play Time and Create Time strings for LM
     * Trim extra 0's from Create Time, i.e. 2026/08/01 -> 2026/8/1
     * Handle input in set swapping ui
     * Fix Escape menu handling
     * Fix portrait + map drawing to be safer
    */

    private ScrollableData? _current_scrollable;

    private readonly ScrollableData _scrollable_saves = new() {
        visible = 5,
    };

    private readonly ScrollableData _scrollable_sets = new() {
        visible = 9,
    };

    public FhSaveUiModule() {
        _sem_handle = new(this);
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        return _sem_handle.try_get_module(out _sem);
    }

    public override void render_imgui() {
        switch (FhSavePal.pal_get_screen_state()) {
            case FhSaveScreenState.OPEN:
                break;

            case FhSaveScreenState.OPENING:
                mode  = FhSaveUiMode.SAVE_LIST;
                focus = FhSaveUiFocus.LIST;

                //populate_map_icons();
                try_load_textures();

                // We update this max here because it won't change easily and it's decently expensive to update
                _scrollable_sets.max = FhInternal.Saves.get_sets().Count;

                return;

            default:
                //_map_icon_textures.Clear();
                return;
        }

        _current_scrollable = mode switch {
            FhSaveUiMode.SET_SWAP => _scrollable_sets,
            _                     => _scrollable_saves,
        };

        // We update this one every frame because it's relatively inexpensive and can change easily
        _scrollable_saves.max =
            _sem!.get_system_state() == FhSaveExtensionSystemState.SAVE
                ? FhInternal.Saves.get_slots_used()
                : FhInternal.Saves.get_display_data().Count;

        if (!try_load_textures()) return;

        ImDrawListPtr draw = ImGui.GetBackgroundDrawList();

        (Vector2 window_size, Vector2 window_offset) = get_window_bounds();
        Vector2  window_min                          = window_offset;
        Vector2  window_max                          = window_offset + window_size;

        draw.PushClipRect(window_min, window_max, true); // Draw a scissor box to prevent mahojin glyph drawing out of bounds

        ui_background();
        ui_help();

        if (mode == FhSaveUiMode.SET_SWAP) {
            ui_setswap();
        }
        else {
            ui_active_set();

            bool is_saving = _sem!.get_system_state() is FhSaveExtensionSystemState.SAVE;
            List<FhSaveDisplayData> display_data = FhInternal.Saves.get_display_data();

            if (!is_saving && display_data.Count < 1) {
                ui_no_saves();
                return;
            }

            // Filter out autosave when saving
            List<FhSaveDisplayData> save_list = new();
            if (is_saving) {
                for (int i = 0; i < display_data.Count; i++) {
                    if (display_data[i].slot != 0) {
                        save_list.Add(display_data[i]);
                    }
                }
            }
            else {
                save_list = display_data;
            }

            int total_count = is_saving ? save_list.Count + 1 : save_list.Count;
            _scrollable_saves.max = total_count;

            if (handle_input()) return;

            for (int i = _scrollable_saves.get_clip_start(); i < _scrollable_saves.max; i++) {
                FhSaveDisplayData save_data = is_saving && i == 0
                    ? new FhSaveDisplayData { slot = 0 }
                    : save_list[is_saving ? i - 1 : i];

                ui_savefile(i, save_data);
            }
        }

        ui_scrollbar();

        draw.PopClipRect();
    }

    private void change_mode(FhSaveUiMode new_mode) {
        _current_scrollable!.reset();
        mode = new_mode;
    }

    private void execute(int slot) {
        switch (_sem!.get_system_state()) {
            case FhSaveExtensionSystemState.SAVE: _sem!.save(slot); break;
            case FhSaveExtensionSystemState.LOAD: _sem!.load(slot); break;
        }
    }

    private void switch_set(string set_name) {
        FhInternal.Saves.switch_active_set(set_name);
        change_mode(FhSaveUiMode.SAVE_LIST);
    }

    private bool mouse_clicked(
        Vector2 topleft,
        Vector2 bottomright,
        ImGuiMouseButton button = ImGuiMouseButton.Left
    ) {
        return ImGui.IsMouseHoveringRect(topleft, bottomright, false)
            && ImGui.IsMouseClicked(button);
    }

    private bool pressed_up() {
        return ImGui.IsKeyPressed(ImGuiKey.W)
            || ImGui.IsKeyPressed(ImGuiKey.UpArrow)
            || ImGui.IsKeyPressed(ImGuiKey.GamepadDpadUp)
            || ImGui.IsKeyPressed(ImGuiKey.GamepadLStickUp);
    }

    private bool pressed_down() {
        return ImGui.IsKeyPressed(ImGuiKey.S)
            || ImGui.IsKeyPressed(ImGuiKey.DownArrow)
            || ImGui.IsKeyPressed(ImGuiKey.GamepadDpadDown)
            || ImGui.IsKeyPressed(ImGuiKey.GamepadLStickDown);
    }

    private bool pressed_left() {
        return ImGui.IsKeyPressed(ImGuiKey.A)
            || ImGui.IsKeyPressed(ImGuiKey.LeftArrow)
            || ImGui.IsKeyPressed(ImGuiKey.GamepadDpadLeft)
            || ImGui.IsKeyPressed(ImGuiKey.GamepadLStickLeft);
    }

    private bool pressed_right() {
        return ImGui.IsKeyPressed(ImGuiKey.D)
            || ImGui.IsKeyPressed(ImGuiKey.RightArrow)
            || ImGui.IsKeyPressed(ImGuiKey.GamepadDpadRight)
            || ImGui.IsKeyPressed(ImGuiKey.GamepadLStickRight);
    }

    private bool pressed_confirm() {
        ImGuiKey gamepad_confirm =
            FhGlobal.lang_id == FhLangId.Japanese
                ? ImGuiKey.GamepadFaceRight
                : ImGuiKey.GamepadFaceDown;

        return ImGui.IsKeyPressed(ImGuiKey.Enter)
            || ImGui.IsKeyPressed(gamepad_confirm);
    }

    private bool pressed_cancel() {
        ImGuiKey gamepad_cancel =
            FhGlobal.lang_id == FhLangId.Japanese
                ? ImGuiKey.GamepadFaceDown
                : ImGuiKey.GamepadFaceRight;

        return ImGui.IsKeyPressed(ImGuiKey.Escape)
            || ImGui.IsKeyPressed(ImGuiKey.Backspace)
            || ImGui.IsKeyPressed(gamepad_cancel);
    }

    /// <summary>Handle player input.</summary>
    /// <returns>Whether the save/load screen was closed.</returns>
    private bool handle_input() {
        bool is_saving = _sem!.get_system_state() is FhSaveExtensionSystemState.SAVE;

        switch (focus) {
            case FhSaveUiFocus.LIST: {
                    if (mode == FhSaveUiMode.SAVE_LIST && pressed_up() && _current_scrollable!.hovered == 0) {
                        focus = FhSaveUiFocus.ACTIVE_SET;
                        break;
                    }

                    _current_scrollable!.handle_input();

                    if (mode == FhSaveUiMode.SAVE_LIST && pressed_confirm()) {
                        FhSaveDisplayData save = FhInternal.Saves.get_display_data()[_current_scrollable!.hovered];

                        if (is_saving && _current_scrollable!.hovered == 0) {
                            _sem!.save(0);
                        }
                        else {
                            execute(FhInternal.Saves.get_display_data()[_current_scrollable!.hovered].slot);
                        }

                        break;
                    }

                    break;
                }

            case FhSaveUiFocus.ACTIVE_SET: {
                    if (mode == FhSaveUiMode.SAVE_LIST && pressed_down()) {
                        focus = FhSaveUiFocus.LIST;

                        if (focus == FhSaveUiFocus.LIST) {
                            _current_scrollable!.hovered = _current_scrollable!.current;
                        }

                        break;
                    }

                    if (pressed_confirm()) {
                        change_mode(FhSaveUiMode.SET_SWAP);
                    }

                    break;
                }

            default: throw new NotImplementedException();
        }

        return false;
    }

    /// <summary>Attempt to load all of the textures the save/load screen requires to display properly.</summary>
    /// <returns>Whether all textures have been successfully loaded.</returns>
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

    /// <summary>Draws the highlights/shadows for the save slot texture.</summary>
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

    /// <summary>Render the background for the save/load screen.</summary>
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
            new(   6f, 2038f),
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

    /// <summary>Render the help text for the save/load screen.</summary>
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

        (Vector2 text_su, _) = scale_screen_uv(
            screen_size,
            new(240f, 118f),
            Vector2.Zero
        );

        //TODO: Add localization

        string text = "";

        switch (mode) {
            case FhSaveUiMode.SET_SWAP:
                text = "Select save set";
                break;

            default:
                text = _sem!.get_system_state() switch {
                    FhSaveExtensionSystemState.LOAD => "Select save data",
                    FhSaveExtensionSystemState.SAVE => "Select save area",

                    _ => throw new NotImplementedException($"Unknown system state: {_sem!.get_system_state()}"),
                };
                break;
        }

        FhApi.Gui.draw_text(draw, text_su, text, font_size, true, TextAlignment.BEGIN, TextAlignment.CENTER);
    }

    /// <summary>Render the cursor.</summary>
    /// <param name="target_pos">The position the cursor should point at.</param>
    private void ui_cursor(Vector2 target_pos) {
        if (!_texture_freetex.try_use(out ImTextureRef freetex, out _)) {
            return;
        }

        ImDrawListPtr draw = ImGui.GetForegroundDrawList();

        (Vector2 window_size, _) = get_window_bounds();
        float    scale_factor    = window_size.Y / 1080f;

        (Vector2 cursor_tu, Vector2 cursor_tv) = scale_tex_uv(
            _tex_freetex_size,
            new Vector2( 10f, 572f),
            new Vector2(175f, 480f)
        );

        Vector2 cursor_size   = new(103f * scale_factor, 58f * scale_factor);

        float overlap = cursor_size.X * 0.1f;
        Vector2 cursor_center = new(
            target_pos.X - cursor_size.X / 2f + overlap,
            target_pos.Y + 4f // Cursor midpoint is weird, this + 4f actually centres the tip
        );
        Vector2 cursor_offset = cursor_center - (cursor_size / 2f);

        float loop_time     = (float)(ImGui.GetTime() % 0.53f); // Takes ~0.53 secs to complete 1 animation loop
        float loop_progress = loop_time / 0.53f;
        float travel_dist   = 18f * scale_factor; // Cursor moves ~18px across a 1920x1080 screen

        float   fade_time;
        float   offset;
        float   alpha;
        uint    color;
        Vector2 pos;
        float   slide_progress;

        // Draw trailing/fade out effect for cursor
        {
            // Ghost Cursor 1
            if (loop_progress >= 0.12f) {
                offset = -6f * scale_factor;
                alpha = 0.25f;

                // Sync movement with other cursors
                if (loop_progress >= 0.48f) {
                    fade_time = (loop_progress - 0.48f) / 0.52f;
                    offset += fade_time * (travel_dist * 0.55f);
                    alpha *= 1f - fade_time;
                }

                color = ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, alpha));
                pos = cursor_offset + new Vector2(offset, 0f);
                draw.AddImage(freetex, pos, pos + cursor_size, cursor_tu, cursor_tv, color);
            }

            // Ghost Cursor 2
            if (loop_progress >= 0.25f) {
                offset = -4f * scale_factor;
                alpha = 0.85f;

                // Start moving
                if (loop_progress >= 0.38f && loop_progress < 0.48f) {
                    slide_progress = (loop_progress - 0.38f) / 0.10f;
                    offset += slide_progress * (travel_dist * 0.20f);
                }
                // Sync movement with other cursors
                else if (loop_progress >= 0.48f) {
                    fade_time = (loop_progress - 0.48f) / 0.52f;
                    offset += (travel_dist * 0.20f) + (fade_time * (travel_dist * 0.55f));
                    alpha *= 1f - fade_time;
                }

                color = ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, alpha));
                pos   = cursor_offset + new Vector2(offset, 0f);
                draw.AddImage(freetex, pos, pos + cursor_size, cursor_tu, cursor_tv, color);
            }
        }

        // Draw Main Cursor
        Vector2 main_pos = cursor_offset + new Vector2(loop_progress * travel_dist, 0f);
        draw.AddImage(freetex, main_pos, main_pos + cursor_size, cursor_tu, cursor_tv);
    }

    /// <summary>Render the set name.</summary>
    private void ui_active_set() {
        if (!_texture_message.try_use(out ImTextureRef message, out _)) {
            return;
        }

        (Vector2 window_size, Vector2 window_offset) = get_window_bounds();
        Vector2  screen_size                         = new(1920f, 1080f);
        float    scale_factor                        = window_size.Y / 1080f;
        float    font_size                           = 36f * scale_factor;

        ImDrawListPtr draw = ImGui.GetBackgroundDrawList();
        ImFontPtr     font = ImGui.GetFont();

        string  active_set = FhInternal.Saves.get_active_set();
        Vector2 text_size  = font.CalcTextSizeA(font_size, float.MaxValue, 0f, active_set);

        // Dynamically change box size based on text
        float padding_x = 250f * scale_factor;
        float padding_y = 20f  * scale_factor;
        float min_width = 450f * scale_factor;

        float bg_width  = MathF.Max(min_width, text_size.X + padding_x);
        float bg_height = text_size.Y + padding_y;

        Vector2 center_top = scale_screen_uv(screen_size, new(960f, 22f), Vector2.Zero).Item1;
        Vector2 tl         = new(center_top.X - (bg_width * 0.5f), center_top.Y);
        Vector2 br         = tl + new Vector2(bg_width, bg_height);

        if (ImGui.IsMouseHoveringRect(tl, br, false)) {
            focus = FhSaveUiFocus.ACTIVE_SET;
        }

        draw.AddRectFilled(tl, br, 0x80000000); // Transparent black background

        // Gold accents on message background
        float accent_w = bg_height;
        float accent_h = bg_height;

        Vector2 accent1_tl = new(tl.X - (2f * scale_factor), tl.Y + (2f * scale_factor));
        Vector2 accent1_br = accent1_tl + new Vector2(accent_w, accent_h);

        Vector2 accent2_br = new(br.X + (1f * scale_factor), br.Y - (2f * scale_factor));
        Vector2 accent2_tl = new(accent2_br.X - accent_w, tl.Y - (2f * scale_factor));

        (Vector2 accent1_uv1, Vector2 accent1_uv2) = scale_tex_uv(
            _tex_message_size,
            new(1469f, 1020f),
            new(1755f,  733f)
        );

        (Vector2 accent2_uv1, Vector2 accent2_uv2) = scale_tex_uv(
            _tex_message_size,
            new(1755f,  733f),
            new(1469f, 1020f)
        );

        draw.AddImage(message, accent1_tl, accent1_br, accent1_uv1, accent1_uv2);
        draw.AddImage(message, accent2_tl, accent2_br, accent2_uv1, accent2_uv2);

        Vector2 bg_center = (tl + br) * 0.5f;

        FhApi.Gui.draw_text(draw, bg_center, active_set, font_size, true, TextAlignment.CENTER, TextAlignment.CENTER);

        // Display current set's total number of Saves
        Vector2 text_pos = scale_screen_uv(screen_size, new(1717f, 146f), Vector2.Zero).Item1;

        FhApi.Gui.draw_text(draw, text_pos, $"{FhInternal.Saves.get_slots_used()} Saves", font_size, true, TextAlignment.END, TextAlignment.CENTER);

        if (focus == FhSaveUiFocus.ACTIVE_SET) {
            float text_left   = bg_center.X - (text_size.X * 0.5f);
            float text_margin = 70f * scale_factor;

            Vector2 cursor_target = new(
                text_left - text_margin,
                bg_center.Y
            );

            ui_cursor(cursor_target);
        }

        // Input handling

        // Prevent accidentally capturing input when the user is focused on an actual ImGui window
        if (ImGui.GetIO().WantCaptureMouse) return;

        if (mouse_clicked(tl, br)) {
            change_mode(FhSaveUiMode.SET_SWAP);
        }
    }

    private void ui_set(int set_idx, string name, int save_count) {
        if (!_texture_save.try_use(out ImTextureRef save_tex, out _)
         || !_texture_freetex.try_use(out ImTextureRef freetex, out _)
        ) {
            return;
        }

        (Vector2 window_size, Vector2 window_offset) = get_window_bounds();
        Vector2  screen_size                         = new(1920f, 1080f);
        float    scale_factor                        = window_size.Y / 1080f;
        float    font_size                           = 42f * scale_factor;

        ImDrawListPtr draw = ImGui.GetBackgroundDrawList();
        ImGuiIOPtr    io   = ImGui.GetIO();

        Vector2 slot_offset = new( 347f * scale_factor, 178f * scale_factor);
        Vector2 slot_size   = new(1216f * scale_factor,  84f * scale_factor);

        float slot_gap = 10f * scale_factor;

        Vector2 slot_topleft = new(
            window_offset.X + slot_offset.X,
            window_offset.Y + slot_offset.Y + (slot_size.Y + slot_gap) * (set_idx - _scrollable_sets.get_clip_start())
        );

        Vector2 set_start = slot_topleft;
        Vector2 set_end   = slot_topleft + slot_size;

        // Texture UV changes for each slot
        float slice_size = 64f;
        float max_y      = slice_size * ((set_idx % 8) + 1);

        float uv_y_start = max_y;
        float uv_y_end   = max_y - slice_size;

        (Vector2 set_tu, Vector2 set_tv) = scale_tex_uv(
            _tex_save_size,
            new(  0f, uv_y_start),
            new(512f,   uv_y_end)
        );

        // Shadows behind slots
        {
            Vector2 shadow_offset = new(10f * scale_factor);

            draw.AddRectFilled(
                set_start + shadow_offset,
                set_end + shadow_offset,
                0x88000000
            );
        }

        // Draw hovered set darker
        if (ImGui.IsMouseHoveringRect(set_start, set_end, false)) {
            focus = FhSaveUiFocus.LIST;
            _scrollable_sets.hovered = set_idx;
            draw.AddImage(save_tex, set_start, set_end, set_tu, set_tv, 0xFFC6B1AF);
            draw.AddRectFilled(set_start, set_end, 0x63000000);
        }
        else {
            draw.AddImage(save_tex, set_start, set_end, set_tu, set_tv, 0xFFE4F0F1);
            draw.AddRectFilled(set_start, set_end, 0x33000000);
        }

        // Draw shading/edges on the save texture for definition
        float border_thickness = 5f * scale_factor;
        uint  highlight        = 0x28FFFFFF;
        uint  shadow           = 0x80000000;

        draw_highlight_shadow(draw, set_start, slot_size, border_thickness, highlight, shadow);

        // If hovered
        if (_scrollable_sets.hovered == set_idx) {
            Vector2 cursor_target = new(
                set_start.X,
                set_start.Y + slot_size.Y / 2f
            );

            ui_cursor(cursor_target);
        }

        Vector2 name_pos      = set_start + new Vector2(30f * scale_factor, 40f * scale_factor);
        Vector2 set_count_pos = set_start + new Vector2(slot_size.X - 30f * scale_factor, 40f * scale_factor);

        string is_active_set = FhInternal.Saves.get_active_set();
        uint   color         = name == is_active_set ? 0xFF19D8FF : 0xFFFFFFFF;

        FhApi.Gui.draw_text(draw, name_pos, name, font_size, true, TextAlignment.BEGIN, TextAlignment.CENTER, color);
        FhApi.Gui.draw_text(draw, set_count_pos, $"{save_count} Saves", font_size, true, TextAlignment.END, TextAlignment.CENTER);

        // Handle input
        if (!io.WantCaptureMouse && mouse_clicked(set_start, set_end)) {
            io.WantCaptureMouse = true;
            switch_set(name);
        }
    }

    /// <summary>
    ///     Draws the set swap UI for the save/load screen.
    /// </summary>
    private void ui_setswap() {
        if (mode != FhSaveUiMode.SET_SWAP) return;

        (Vector2 window_size, Vector2 window_offset) = get_window_bounds();
        Vector2  screen_size                         = new(1920f, 1080f);
        float    scale_factor                        = window_size.Y / 1080f;

        // Screen coordinates
        //TODO: Somehow consolidate the two sources of truth we have for button_start and button_size here and in ui_set
        /*Vector2 button_start  = new( 290f, 148f);
        Vector2 button_size   = new(1024f,  70f);
        float   button_y_diff = button_size.Y + 8f * scale_factor;

        (Vector2 button_base_su, Vector2 button_base_sv) = scale_screen_uv(
            screen_size,
            button_start,
            button_start + button_size
        );*/

        // Get a list of sets, sorted
        string active_set = FhInternal.Saves.get_active_set();

        List<string> set_list = new(FhInternal.Saves.get_sets());
        set_list.Sort(
            (a, b) => {
                if (a == active_set) return -1;
                if (b == active_set) return 1;

                return String.Compare(a, b, StringComparison.Ordinal);
            }
        );

        int start_idx = _scrollable_sets.get_clip_start();
        int end_idx   = _scrollable_sets.get_clip_end();

        /*
         * We have to loop over everything once first to set the proper hovered index
         * Otherwise we may render two cursors at once if we scroll down
         * and the previously hovered button is still on screen.
         *
         * A           | B < We haven't updated the hovered index, so this is still hovered and gets a cursor
         * B < Hovered | C < Updated hovered index, this is now hovered! It also gets a cursor
         * C           | D
         */
        //TODO: Triage whether this fix is still desired for the
        //      extreme edge case of simultaneous mouse and keyboard input

        // for (int set_idx = start_idx; set_idx < end_idx; set_idx++) {
        //     float y_diff = button_y_diff * (set_idx - start_idx);
        //
        //     Vector2 button_su = button_base_su;
        //     Vector2 button_sv = button_base_sv;
        //
        //     button_su.Y += y_diff;
        //     button_sv.Y += y_diff;
        //
        //     if (ImGui.IsMouseHoveringRect(button_su, button_sv, false)) {
        //         _scrollable_sets.hovered = set_idx;
        //     }
        // }

        // Drawing time

        for (int set_idx = start_idx; set_idx < end_idx; set_idx++) {
            string set_name   = set_list[set_idx];
            int    save_count = Math.Max(0, FhInternal.Saves.get_save_counts()[set_name] - 1); // Remove autosave from count

            ui_set(set_idx, set_name, save_count);
        }
    }

    /// <summary>Render "No Saved Data" if the player has no saves in the load menu.</summary>
    private void ui_no_saves() {
        if (!_texture_message.try_use(out ImTextureRef message, out _)) {
            return;
        }

        ImDrawListPtr draw = ImGui.GetBackgroundDrawList();
        ImFontPtr     font = ImGui.GetFont();

        (Vector2 window_size, Vector2 window_offset) = get_window_bounds();
        Vector2  screen_size                         = new(1920f, 1080f);
        float    scale_factor                        = window_size.Y / 1080f;
        float    font_size                           = 42f * scale_factor;

        string[] text = {
            "No Saved Data, please return to the Main Menu",
            "or select another Save Set."
        };

        float text_width = 0f;
        foreach (string line in text) {
            Vector2 line_size = font.CalcTextSizeA(font_size, float.MaxValue, 0f, line);
            if (line_size.X > text_width) {
                text_width = line_size.X;
            }
        }
        float text_height = text.Length * font_size;

        float padding_x = 350f * scale_factor;
        float padding_y = 60f  * scale_factor;
        float min_width = 800f * scale_factor;

        float bg_width  = MathF.Max(min_width, text_width + padding_x);
        float bg_height = text_height + padding_y;

        Vector2 screen_center = window_offset + (window_size * 0.5f);
        Vector2 tl            = screen_center - new Vector2(bg_width * 0.5f, bg_height * 0.5f);
        Vector2 br            = tl + new Vector2(bg_width, bg_height);

        draw.AddRectFilled(tl, br, 0x80000000); // Transparent black background

        // Gold accents on message background
        float accent_w = 150f * scale_factor;
        float accent_h = bg_height;

        Vector2 accent1_tl = new(tl.X - (5f * scale_factor), tl.Y + (5f * scale_factor));
        Vector2 accent1_br = new(accent1_tl.X + accent_w, accent1_tl.Y + accent_h);

        Vector2 accent2_br = new(br.X + (4f * scale_factor), br.Y - (5f * scale_factor));
        Vector2 accent2_tl = new(accent2_br.X - accent_w, (br.Y - accent_h) - (5f * scale_factor));

        (Vector2 accent1_tu, Vector2 accent1_tv) = scale_tex_uv(
            _tex_message_size,
            new(1469f, 1020f),
            new(1755f,  733f)
        );

        (Vector2 accent2_tu, Vector2 accent2_tv) = scale_tex_uv(
            _tex_message_size,
            new(1755f,  733f),
            new(1469f, 1020f)
        );

        draw.AddImage(message, accent1_tl, accent1_br, accent1_tu, accent1_tv);
        draw.AddImage(message, accent2_tl, accent2_br, accent2_tu, accent2_tv);

        float start_y = screen_center.Y - (text_height * 0.5f) + (font_size * 0.5f);

        for (int i = 0; i < text.Length; i++) {
            float current_y = start_y + (i * font_size);
            FhApi.Gui.draw_text(draw, new Vector2(screen_center.X, current_y), text[i], font_size, true, TextAlignment.CENTER, TextAlignment.CENTER);
        }
    }

    private void ui_savefile(int index, FhSaveDisplayData save) {
        if (!_texture_save.try_use(out ImTextureRef save_tex, out _)
         || !_texture_freetex.try_use(out ImTextureRef freetex, out _)
         || !_scrollable_saves.is_within_clip(index)
        ) {
            return;
        }

        ImDrawListPtr draw = ImGui.GetBackgroundDrawList();
        ImFontPtr     font = ImGui.GetFont();

        (Vector2 window_size, Vector2 window_offset) = get_window_bounds();
        float    scale_factor                        = window_size.Y / 1080f;
        float    font_size                           = 42f * scale_factor;

        Vector2 save_offset = new( 157f * scale_factor, 172f * scale_factor);
        Vector2 save_size   = new(1565f * scale_factor, 155f * scale_factor);

        float save_gap = 10f * scale_factor;

        Vector2 save_topleft = new(
            window_offset.X + save_offset.X,
            window_offset.Y + save_offset.Y + (save_size.Y + save_gap) * (index - _scrollable_saves.current)
        );

        if (ImGui.IsMouseHoveringRect(save_topleft, save_topleft + save_size, false)) {
            focus = FhSaveUiFocus.LIST;
            _scrollable_saves.hovered = index;
        }

        float header_height  = 47f  * scale_factor;
        float vertical_space = 4f   * scale_factor;

        Vector2 header_start = save_topleft;
        Vector2 header_end   = new(save_topleft.X + save_size.X, header_start.Y + header_height);

        Vector2 info_start = new(save_topleft.X, header_end.Y + vertical_space);
        Vector2 info_end   = save_topleft + save_size;

        Vector2 header_size = header_end - header_start;
        Vector2 info_size   = info_end - info_start;

        // Texture UV changes for each slot
        float slice_size = 64f;
        float max_y = slice_size * ((save.slot % 8) + 1);

        float uv_y_start = max_y;
        float uv_y_end   = max_y - slice_size;
        float uv_y_split = uv_y_start + (uv_y_end - uv_y_start) * (47f / 151f); // Divides Save texture into a header and info box

        (Vector2 header_tu, Vector2 header_tv) = scale_tex_uv(
            _tex_save_size,
            new(  0f, uv_y_start),
            new(512f, uv_y_split)
        );

        (Vector2 info_tu, Vector2 info_tv) = scale_tex_uv(
            _tex_save_size,
            new(  0f, uv_y_split),
            new(512f,   uv_y_end)
        );

        // Shadows behind saves
        {
            Vector2 shadow_offset = new(10f * scale_factor);

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

        bool is_autosave = save.slot == 0 && _sem!.get_system_state() is not FhSaveExtensionSystemState.SAVE;
        bool is_newsave  = save.slot == 0 && _sem!.get_system_state() is     FhSaveExtensionSystemState.SAVE;

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

        if (is_newsave) {
            Vector2 text_offset = new(16f * scale_factor, 100f * scale_factor);
            FhApi.Gui.draw_text(draw, save_topleft + text_offset, "New Save Data", font_size, true, TextAlignment.BEGIN, TextAlignment.CENTER);

            uint grad_l = 0xFF191919; // grey
            uint grad_r = 0xFF252525; // black

            float box_width  = 255f * scale_factor;
            float box_height = 145f * scale_factor;
            float right_edge =   5f * scale_factor;
            float top_edge   =   5f * scale_factor;

            Vector2 tl = new(save_topleft.X + save_size.X - right_edge - box_width, save_topleft.Y + top_edge);
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
        } else {
            uint  slot_text_color = is_autosave ? 0xFF19D8FF : 0xFFFFFFFF; // Yellow : White
            float location_offset = is_autosave ? 258f : 130f;

            string slot_text        = is_autosave ? "Autosave" : Encoding.UTF8.GetString(save.slot_str);
            string location_text    = Encoding.UTF8.GetString(save.location);
            string create_time_text = Encoding.UTF8.GetString(save.create_time);

            Vector2 slot_pos        = new(  17f * scale_factor, 22f  * scale_factor);
            Vector2 location_pos    = new(location_offset * scale_factor, 22f * scale_factor);
            Vector2 create_time_pos = new(1258f * scale_factor, 22f  * scale_factor);

            FhApi.Gui.draw_text(draw, save_topleft + slot_pos, slot_text, font_size, true, TextAlignment.BEGIN, TextAlignment.CENTER, slot_text_color);
            FhApi.Gui.draw_text(draw, save_topleft + location_pos, location_text, font_size, true, TextAlignment.BEGIN, TextAlignment.CENTER);
            FhApi.Gui.draw_text(draw, save_topleft + create_time_pos, create_time_text, font_size, true, TextAlignment.END, TextAlignment.CENTER);

            FhSaveHeader2 header2 = MemoryMarshal.Read<FhSaveHeader2>(save.header);

            // TODO: Redo this texture loading to be nicer/safer
            (Vector2 faces_tu, Vector2 faces_tv) = scale_tex_uv(
                _tex_faces_size,
                new(  0f, 239f),
                new(256f,   0f)
            );

            Vector2 face_offset = new(  5f * scale_factor, 56f * scale_factor);
            Vector2 face_size   = new(100f * scale_factor, 94f * scale_factor);

            string filename;

            /*(Vector2 map_tu, Vector2 map_tv) = scale_tex_uv(
                _tex_map_size,
                new(0f  ,   0f),
                new(320f, 176f)
            );
        
            Vector2 map_offset = new(1305f * scale_factor, 5f   * scale_factor);
            Vector2 map_size   = new(255f  * scale_factor, 145f * scale_factor);

            // TODO: Some maps use "{header2.id_location}_1"
            // Those need to be rewired like the dresspheres below
            filename = $"{header2.id_location}_0.png";

            FhTexture map = new(SAVEDATAICONS_DIR + filename, FhTextureType.PNG);

            if (!map.is_loaded()) {
                FhApi.Resources.load_game_texture_2d(map);
            }
        
            if (map.try_use(out ImTextureRef map_icon, out _)) {
                Vector2 map_start = save_topleft + map_offset;
                Vector2 map_end   = map_start    + map_size;
        
                draw.AddImage(map_icon, map_start, map_end, map_tu, map_tv);
            }*/

            if (FhGlobal.game_id == FhGameId.FFX2) {
                string name_text       = Encoding.UTF8.GetString(save.player_name);
                string chapter_text    = Encoding.UTF8.GetString(save.chapter);
                string completion_text = Encoding.UTF8.GetString(save.completion);
                string playtime_text   = Encoding.UTF8.GetString(save.play_time);

                Vector2 name_offset       = new( 356f * scale_factor,  76f * scale_factor);
                Vector2 chapter_offset    = new( 356f * scale_factor, 125f * scale_factor);
                Vector2 completion_offset = new(1259f * scale_factor,  76f * scale_factor);
                Vector2 playtime_offset   = new(1258f * scale_factor, 125f * scale_factor);

                FhApi.Gui.draw_text(draw, save_topleft + name_offset, name_text, font_size, true, TextAlignment.BEGIN, TextAlignment.CENTER);
                FhApi.Gui.draw_text(draw, save_topleft + chapter_offset, chapter_text, font_size, true, TextAlignment.BEGIN, TextAlignment.CENTER);
                FhApi.Gui.draw_text(draw, save_topleft + completion_offset, completion_text, font_size, true, TextAlignment.END, TextAlignment.CENTER);
                FhApi.Gui.draw_text(draw, save_topleft + playtime_offset, playtime_text, font_size, true, TextAlignment.END, TextAlignment.CENTER);

                ReadOnlySpan<(byte chr_id, byte dress_id)> party = [
                    (header2.id_chr1, header2.id_chr1_dress),
                    (header2.id_chr2, header2.id_chr2_dress),
                    (header2.id_chr3, header2.id_chr3_dress)
                ];

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
                            Vector2 next_offset = face_offset  + new Vector2(i * 105f * scale_factor, 0f);
                            Vector2 face_start  = save_topleft + next_offset;
                            Vector2 face_end    = face_start   + face_size;

                            draw.AddImage(faces, face_start, face_end, faces_tu, faces_tv);
                        }
                    }
                }
            }
            else {
                string name_text     = Encoding.UTF8.GetString(save.player_name);
                string job_text      = Encoding.UTF8.GetString(save.lm_job);
                string level_text    = Encoding.UTF8.GetString(save.lm_level);
                string playtime_text = Encoding.UTF8.GetString(save.play_time);

                Vector2 job_width = font.CalcTextSizeA(font_size, float.MaxValue, 0f, job_text);

                float  level_offset = 92f * scale_factor;

                Vector2 name_pos     = new( 269f * scale_factor,  76f * scale_factor);
                Vector2 job_pos      = new( 269f * scale_factor, 125f * scale_factor);
                Vector2 level_pos    = new(job_pos.X + job_width.X + level_offset, 125f * scale_factor);
                Vector2 playtime_pos = new(1259f * scale_factor, 125f * scale_factor);

                FhApi.Gui.draw_text(draw, save_topleft + name_pos, name_text, font_size, true, TextAlignment.BEGIN, TextAlignment.CENTER);
                FhApi.Gui.draw_text(draw, save_topleft + job_pos, job_text, font_size, true, TextAlignment.BEGIN, TextAlignment.CENTER);
                FhApi.Gui.draw_text(draw, save_topleft + level_pos, level_text, font_size, true, TextAlignment.BEGIN, TextAlignment.CENTER);
                FhApi.Gui.draw_text(draw, save_topleft + playtime_pos, playtime_text, font_size, true, TextAlignment.END, TextAlignment.CENTER);

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
                            Vector2 next_offset = face_offset  + new Vector2(i * 26f * scale_factor, 0f);
                            Vector2 face_start  = save_topleft + next_offset;
                            Vector2 face_end    = face_start   + face_size;
                            uint    color       = ImGui.GetColorU32(new Vector4(1f, 1f, 1f, 1f - (i * 0.25f)));

                            draw.AddImage(faces, face_start, face_end, faces_tu, faces_tv, color);
                        }
                    }
                }
            }
        }

        if (focus == FhSaveUiFocus.LIST && _scrollable_saves.hovered == index) {
            float offset = 28f * scale_factor;
            Vector2 cursor_target = new(
                header_start.X,
                header_start.Y + save_size.Y / 2f + offset
            );

            ui_cursor(cursor_target);
        }

        if (mouse_clicked(save_topleft, save_topleft + save_size)) {
            execute(save.slot);
        }
    }

    /// <summary>Render the scrollbar.</summary>
    private void ui_scrollbar() {
        if (mode == FhSaveUiMode.SAVE_LIST && _scrollable_saves.max <= _scrollable_saves.visible
         || mode == FhSaveUiMode.SET_SWAP  && _scrollable_sets.max  <= _scrollable_sets.visible
         ) {
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
}
