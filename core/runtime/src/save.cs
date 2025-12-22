// SPDX-License-Identifier: MIT

using System.Numerics;

namespace Fahrenheit.Core.Runtime;

/* [fkelava 13/11/25 22:03]
 * This module is fairly complex. Read through slowly and carefully.
 *
 * Originally, we simply overrode the actual act of (auto)saving and loading,
 * and hooked the game's Flash-based Iggy UI to display custom save lists.
 * This proved untenably slow, so it was replaced with a custom ImGui display.
 *
 * Thus, this is split into three parts:
 * - the actual implementing hooks (impl_*)
 * - the UI replacement (ui_*)
 * - the 'platform abstraction layer' (pal_*), which wraps the game's save manager.
 *
 * The save PAL, being a binding to implementation details of each game,
 * is virtually illegible without consulting the original method bodies.
 *
 * For your convenience, most PAL methods are annotated with a source line you can look up.
 */

/// <summary>
///     Implements Fahrenheit's extended save system.
///     <para/>
///     Do not interface with this module. It has no public API.
/// </summary>
[FhLoad(FhGameId.FFX | FhGameId.FFX2 | FhGameId.FFX2LM)]
public unsafe sealed class FhSaveExtensionModule : FhModule {

    /* [fkelava 26/11/25 17:25]
     * This structure contains all the fields the game would show as part of the standard Iggy
     * save menu display. These inline arrays are in reality UTF-8 strings, since both Iggy
     * and ImGui accept them as input. The sizes are taken from the base game.
     */

    [StructLayout(LayoutKind.Sequential)]
    private struct FhSaveDisplayData {
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

    private enum FhSaveSystemState {
        NULL = 0,
        LOAD = 1,
        SAVE = 2,
        ALBD = 3 // FFX only; Al Bhed Compilation Sphere mode
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void __autosave(int size, nint ptr);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void __save_state_transition();

    // Game functions required by the PAL
    private readonly delegate* unmanaged[Cdecl] <ushort, uint, byte*> _fnptr_AtelGetSaveDicName  = pal_addr_func_AtelGetSaveDicName();
    private readonly delegate* unmanaged[Cdecl] <ushort, int>         _fnptr_fix_mappic          = pal_addr_func_fix_mappic();
    private readonly delegate* unmanaged[Cdecl] <int>                 _fnptr_isNeedShowJapanLogo = pal_addr_func_isNeedShowJapanLogo();
    private readonly delegate* unmanaged[Cdecl] <int, byte*, void>    _fnptr_SaveDataGetLoc      = pal_addr_func_SaveDataGetLoc();

    // X-2 LM exclusive function doesn't require address abstraction
    private readonly delegate* unmanaged[Cdecl] <byte, byte, byte*> _fnptr_GetLastMissionJobName =
        (delegate* unmanaged[Cdecl] <byte, byte, byte*>)(FhEnvironment.BaseAddr + 0x368570);

    // Game functions required by the implementation
    private readonly delegate* unmanaged[Cdecl] <void>       _fnptr_SetUpDefaultSaveFolder  = pal_addr_func__SetUpDefaultSaveFolder();
    private readonly delegate* unmanaged[Cdecl] <byte, bool> _fnptr_isNeedRenamePlayer      = pal_addr_func_isNeedRenamePlayer();
    private readonly delegate* unmanaged[Cdecl] <nint, nint> _fnptr_SaveDataWriteCrc        = pal_addr_func_SaveDataWriteCrc();
    private readonly delegate* unmanaged[Cdecl] <int>        _fnptr_SaveDataCheckCrc        = pal_addr_func_SaveDataCheckCrc();
    private readonly delegate* unmanaged[Cdecl] <int, void>  _fnptr_SaveDataSaveLoadSucceed = pal_addr_func_SaveDataSaveLoadSucceed();

    private readonly FhMethodHandle<__autosave>               _handle_autosave;
    private readonly FhMethodHandle<__save_state_transition>  _handle_tosave;
    private readonly FhMethodHandle<__save_state_transition>  _handle_toload;
    private readonly FhMethodHandle<__save_state_transition>? _handle_toalbd; // FFX only

    private readonly FhModuleHandle<FhLocalStateModule> _lsm_handle;
    private          FhModuleContext?                   _lsm;

    private          FhSaveSystemState _ui_system_state;
    private readonly FhSaveDisplayData _ui_display_data;
    private          int               _ui_display_index;

    public FhSaveExtensionModule() {
        FhMethodLocation loc_autosave = new(0x2F0650, 0x11D510);
        FhMethodLocation loc_tosave   = new(0x248950, 0x884D0);
        FhMethodLocation loc_toload   = new(0x248910, 0x884A0);

        _handle_autosave = new(this, loc_autosave, impl_autosave);
        _handle_tosave   = new(this, loc_tosave,   impl_transition_save);
        _handle_toload   = new(this, loc_toload,   impl_transition_load);

        if (FhGlobal.game_id is FhGameId.FFX) {
            _handle_toalbd = new(this, "FFX.exe", 0x2EFFF0, impl_transition_albd);
        }

        _lsm_handle      = new(this);
        _ui_display_data = new();
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        return _handle_autosave.hook()
            && _handle_tosave  .hook()
            && _handle_toload  .hook()
            && (_handle_toalbd?.hook() ?? true)
            && _lsm_handle     .try_get(out _lsm);
    }

    private void ui_mainwindow() {
        ImGuiIOPtr    io    = ImGui.GetIO();
        ImGuiStylePtr style = ImGui.GetStyle();

        ImGuiWindowFlags flags = FhApi.ImGuiHelper.WINDOW_FLAGS_FULLSCREEN & (~ImGuiWindowFlags.NoScrollbar);

        ImGui.SetNextWindowPos (new Vector2(io.DisplaySize.X * 0.008F, io.DisplaySize.Y * 0.105F));
        ImGui.SetNextWindowSize(new Vector2(io.DisplaySize.X * 0.984F, io.DisplaySize.Y * 0.88F));

        if (!ImGui.Begin("###Fh.Runtime.SaveSystem.SaveLoadUI", flags)) {
            ImGui.End();
            return;
        }

        int index = 0;
        if (_ui_system_state is FhSaveSystemState.SAVE) {
            if (FhInternal.Saves.get_slots_used() < FhInternal.Saves.get_slots_total()) {
                if (ImGui.Button("New Save Data", new(ImGui.GetContentRegionAvail().X, 0F))) {
                    impl_save(index);
                }
            }
            index = 1;
        }

        for (; index < FhInternal.Saves.get_slots_total(); index++) {
            ui_savefile(index);
        }

        ImGui.End();
    }

    private void ui_setswap() {
        ReadOnlySpan<string> sets = FhInternal.Saves.get_sets();

        ImGuiIOPtr    io    = ImGui.GetIO();
        ImGuiStylePtr style = ImGui.GetStyle();

        float width_modal  = Math.Max(600, io.DisplaySize.X / 3);    // 33% margin
        float height_modal = Math.Max(400, io.DisplaySize.Y * 0.8F); // 10% margin

        ImGui.SetNextWindowPos (new ((io.DisplaySize.X - width_modal) / 2, io.DisplaySize.Y * 0.015F));
        ImGui.SetNextWindowSize(new (width_modal,                          io.DisplaySize.Y * 0.075F));

        if (!ImGui.Begin("###Fh.Runtime.SaveSystem.SetSwap", FhApi.ImGuiHelper.WINDOW_FLAGS_FULLSCREEN)) {
            ImGui.End();
            return;
        }

        int    active_set      = FhInternal.Saves.get_active_set();
        string slots_text      = $"({FhInternal.Saves.get_slots_used()}/{FhInternal.Saves.get_slots_total()})";
        string active_set_text = $"{Path.GetFileName(sets[active_set])} {slots_text}";

        float width_window = ImGui.GetWindowSize().X;
        float width_text   = ImGui.CalcTextSize(active_set_text).X;

        ImGui.SetCursorPosX((width_window - width_text) * 0.5f);
        ImGui.Text(active_set_text);

        FhApi.ImGuiHelper.set_next_align("Change Set"u8, 0.5F, style.FramePadding.X * 2.0F);

        ImGui.BeginDisabled(FhInternal.Saves.set_swap_locked());
        if (ImGui.Button("Change Set")) {
            ImGui.OpenPopup("Select Set");
        }
        ImGui.EndDisabled();

        ImGui.SetNextWindowPos (new ((io.DisplaySize.X - width_modal) / 2, (io.DisplaySize.Y - height_modal) / 2));
        ImGui.SetNextWindowSize(new (width_modal, height_modal));

        if (ImGui.BeginPopupModal("Select Set")) {
            for (int i = 0; i < sets.Length; i++) {
                bool is_selected = (i == active_set);
                if (ImGui.Selectable(Path.GetFileName(sets[i]), is_selected)) {
                    FhInternal.Saves.set_swap(i);
                    ImGui.CloseCurrentPopup();
                }
                if (is_selected) ImGui.SetItemDefaultFocus();
            }

            ImGui.EndPopup();
        };

        ImGui.End();
    }

    public void ui_savefile(int index) {
        FhSaveDisplayData data = _ui_display_data;

        string   save_file_path = FhInternal.Saves.get_save_path_for_slot(index);
        FileInfo save_file      = new FileInfo(save_file_path);

        if (!save_file.Exists) return;

        using (FileStream save_file_stream = save_file.OpenRead()) {
            save_file_stream.ReadExactly(data.header);
        }

        pal_get_location   (data.header, data.location);
        pal_get_icon_chr   (data.header, data.icon_chr1, 0);
        pal_get_icon_chr   (data.header, data.icon_chr2, 1);
        pal_get_icon_chr   (data.header, data.icon_chr3, 2);
        pal_get_icon_map   (data.header, data.icon_map);
        pal_get_player_name(data.header, data.player_name);
        pal_get_playtime   (data.header, data.play_time);
        pal_get_chapter    (data.header, data.chapter);
        pal_get_completion (data.header, data.completion);
        pal_get_lm_job     (data.header, data.lm_job);
        pal_get_lm_level   (data.header, data.lm_level);

        _ = Encoding.UTF8.GetBytes($"{index}\0", data.slot);
        _ = Encoding.UTF8.GetBytes($"{save_file.LastWriteTimeUtc:yyyy/MM/dd HH:mm:ss}\0", data.create_time);

        ImGuiStylePtr style       = ImGui.GetStyle();
        bool          is_autosave = index == 0 && _ui_system_state is FhSaveSystemState.LOAD;
        Vector2       spacer_size = new(0F, style.FramePadding.Y);
        Vector2       window_size = new(ImGui.GetContentRegionAvail().X, 0F);

        // TODO: Change this to ItemSpacing instead of FramePadding
        if (index != 0) {
            ImGui.Dummy(spacer_size);
        }

        bool is_highlighted = index == _ui_display_index && ImGui.IsWindowFocused();
        bool is_selected    = is_highlighted && ImGui.IsMouseDown(ImGuiMouseButton.Left);

        ImGui.PushStyleColor(ImGuiCol.ChildBg, is_highlighted switch {
            true when is_selected => ImGui.GetColorU32(ImGuiCol.FrameBgActive),
            true                  => ImGui.GetColorU32(ImGuiCol.FrameBgHovered),
            _                     => ImGui.GetColorU32(ImGuiCol.FrameBg)
        });

        ImGui.BeginChild($"###Slot{index}", window_size, ImGuiChildFlags.AutoResizeY, FhApi.ImGuiHelper.WINDOW_FLAGS_FULLSCREEN);

        ImGui.PushStyleColor(ImGuiCol.ChildBg, ImGui.GetColorU32(ImGuiCol.TitleBg));
        ImGui.BeginChild("##Title", window_size, ImGuiChildFlags.AutoResizeY, FhApi.ImGuiHelper.WINDOW_FLAGS_FULLSCREEN);

        ImGui.Indent();
        {
            ImGui.Text(is_autosave ? "Autosave"u8 : data.slot);
            ImGui.SameLine(is_autosave ? 100 : 60);
            ImGui.Text(data.location);
            ImGui.SameLine();
            FhApi.ImGuiHelper.set_next_align(data.create_time, 1.0F, style.FramePadding.X + style.IndentSpacing);
            ImGui.Text(data.create_time);
        }
        ImGui.Unindent();

        ImGui.EndChild();
        ImGui.PopStyleColor();

        ImGui.Indent();
        {
            switch (FhGlobal.game_id) {
                case FhGameId.FFX:
                    ImGui.Text(data.player_name);
                    ImGui.Text(data.play_time);
                    break;
                case FhGameId.FFX2:
                    ImGui.Text(data.icon_chr1);
                    ImGui.SameLine();
                    FhApi.ImGuiHelper.set_next_align(data.completion, 1.0F, style.FramePadding.X + style.IndentSpacing);
                    ImGui.Text(data.completion);
                    ImGui.Text(data.chapter);
                    ImGui.SameLine();
                    FhApi.ImGuiHelper.set_next_align(data.play_time, 1.0F, style.FramePadding.X + style.IndentSpacing);
                    ImGui.Text(data.play_time);
                    break;
                case FhGameId.FFX2LM:
                    ImGui.Text("Yuna");
                    ImGui.Text(data.lm_job);
                    ImGui.SameLine(80);
                    ImGui.Text(data.lm_level);
                    break;
            }
        }
        ImGui.Unindent();

        if (ImGui.IsWindowHovered()) {
            _ui_display_index = index;

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left)) {
                switch (_ui_system_state) {
                    case FhSaveSystemState.SAVE: impl_save(index); break;
                    case FhSaveSystemState.LOAD: impl_load(index); break;
                    case FhSaveSystemState.ALBD: impl_albd(index); break;
                }
            }
        }

        ImGui.Dummy(spacer_size);

        ImGui.EndChild();
        ImGui.PopStyleColor();
    }

    public override void render_imgui() {
        if (pal_get_screen_state() is not FhSaveScreenState.OPEN)
            return;

        if (ImGui.IsKeyPressed(ImGuiKey.Escape) || ImGui.IsKeyPressed(ImGuiKey.Backspace)) {
            // pal_set_operation_canceled(1);
            return;
        }

        ui_setswap();
        ui_mainwindow();
    }

    /* [fkelava 27/11/25 02:15]
     * These three functions are the transition points where the game invokes the Iggy UI.
     * In our case we only need to set a flag and ImGui appears on the next frame present.
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ])]
    private void impl_transition_save() {
        _ui_system_state = FhSaveSystemState.SAVE;
        pal_set_system_state(FhSaveDataManagerState.SAVE);
    }

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ])]
    private void impl_transition_load() {
        _ui_system_state = FhSaveSystemState.LOAD;
        pal_set_system_state(FhSaveDataManagerState.LOAD);
    }

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl )])]
    private void impl_transition_albd() {
        _ui_system_state = FhSaveSystemState.ALBD;
        pal_set_system_state(FhSaveDataManagerState.LOAD);
    }

    /// <summary>
    ///     Creates the autosave, the save game in the reserved slot 0.
    /// </summary>
    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ])]
    private void impl_autosave(int size, nint ptr) {
        // TODO: Consider stubbing out both the CRC write and check
        _fnptr_SaveDataWriteCrc(ptr);
        _fnptr_SetUpDefaultSaveFolder();

        string save_path = FhInternal.Saves.get_save_path_for_slot(0);

        ReadOnlySpan<byte> save = new(
            ptr.ToPointer(),
            size);

        using (FileStream save_stream = File.OpenWrite(save_path)) {
            save_stream.Write(save);
        }

        // This binding cannot fail in normal circumstances. If it does, throwing is appropriate.
        (_lsm!.Module as FhLocalStateModule)!.state_save_slot(0);
    }

    /// <summary>
    ///     Creates a save file in the slot corresponding to the
    ///     selected <paramref name="index"/> in the save/load menu.
    /// </summary>
    private void impl_save(int index) {
        int    slot      = FhInternal.Saves.get_slot_save(index);
        string save_path = FhInternal.Saves.get_save_path_for_slot(slot);

        ReadOnlySpan<byte> save = new(
            FhUtil.ptr_at<byte>(pal_addr_buf_save()),
            pal_buf_save_size()
            );

        using (FileStream save_stream = File.OpenWrite(save_path)) {
            save_stream.Write(save);
        }

        FhUtil.set_at(pal_addr_status1(), 0x1E);

        // From Iggy state handler 0x06
        _fnptr_SaveDataSaveLoadSucceed(1);
        pal_set_dialog_state(FhSaveDialogState.CLOSED);

        // TODO: popup if success

        (_lsm!.Module as FhLocalStateModule)!.state_save_slot(slot);
    }

    /// <summary>
    ///     Loads the save file in the slot corresponding to the
    ///     selected <paramref name="index"/> in the save/load menu.
    /// </summary>
    private void impl_load(int index) {
        int    slot      = FhInternal.Saves.get_slot_load(index);
        string save_name = FhInternal.Saves.get_save_path_for_slot(slot);

        Span<byte> save = new(
            FhUtil.ptr_at<byte>(pal_addr_buf_save()),
            pal_buf_save_size()
            );

        using (FileStream save_stream = File.OpenRead(save_name)) {
            save_stream.ReadExactly(save);
        }

        // From Iggy state handler 0xF
        if (_fnptr_SaveDataCheckCrc() == 0) { // TODO: popup if save is corrupted
            save.Clear();
            return;
        }

        // TODO: popup if player name change will occur
        bool player_needs_rename = _fnptr_isNeedRenamePlayer(
            save[ pal_header_offset_playerrename() ]);

        FhUtil.set_at(pal_addr_status1(),             0x1E);
        FhUtil.set_at(pal_addr_force_player_rename(), player_needs_rename);

        // From Iggy state handler 0x11
        _fnptr_SaveDataSaveLoadSucceed(3);
        pal_set_dialog_state(FhSaveDialogState.CLOSED);

        (_lsm!.Module as FhLocalStateModule)!.state_load_slot(slot);
    }

    /// <summary>
    ///     Performs an Al Bhed Compilation Sphere load from the save
    ///     at the given <paramref name="index"/> in the save/load menu.
    /// </summary>
    private void impl_albd(int index) {
        int    slot      = FhInternal.Saves.get_slot_load(index);
        string save_name = FhInternal.Saves.get_save_path_for_slot(slot);

        //Span<byte> save = new(
        //    FhUtil.ptr_at<byte>(pal_addr_buf_save()),
        //    pal_buf_save_size()
        //    );

        //using (FileStream save_stream = File.OpenRead(save_name)) {
        //    save_stream.ReadExactly(save);
        //}

        FhUtil.set_at(pal_addr_status1(), 0x1E);

        // From Iggy state handler 0x11
        _fnptr_SaveDataSaveLoadSucceed(3);
        pal_set_dialog_state(FhSaveDialogState.CLOSED);
    }

    internal static FhSaveDialogState pal_get_dialog_state()                        => FhUtil.get_at<FhSaveDialogState>(pal_addr_dialog_state());
    internal static void              pal_set_dialog_state(FhSaveDialogState value) => FhUtil.set_at(pal_addr_dialog_state(), value);
    internal static FhSaveScreenState pal_get_screen_state()                        => FhUtil.get_at<FhSaveScreenState>(pal_addr_screen_state());
    internal static void              pal_set_screen_state(FhSaveScreenState value) => FhUtil.set_at(pal_addr_screen_state(), value);

    /// <summary>
    ///     Sets the state of the in-game save manager to the given <paramref name="state"/>.
    /// </summary>
    private void pal_set_system_state(FhSaveDataManagerState state) {
        FhSaveDataManager*  mgr_x  = *(FhSaveDataManager **)pal_addr_save_mgr();
        FhSaveDataManager2* mgr_x2 = *(FhSaveDataManager2**)pal_addr_save_mgr();

        if (FhGlobal.game_id is FhGameId.FFX) {
            mgr_x->state = FhSaveDataManagerState.SAVE;
            return;
        }

        mgr_x2->state = FhSaveDataManagerState.SAVE;
    }

    /* [fkelava 12/11/25 16:51]
     * To show `Tower {X}F` in LM or `Chapter {X}` and `Story Completion: {X}%` in X-2,
     * the game gets a template from SaveDataGetLoc(), with a 0x05 byte marking a fill point.
     * The string must be shifted left after filling to remove the mark byte.
     *
     * It is unclear whether this is the only use of the 0x05 opcode.
     */

    /* [fkelava 13/11/25 22:05]
     * FFX-2.exe+88000 (X-2, X-2 LM)
     */

    /// <summary>
    ///     Inserts <paramref name="fill"/> into the empty space
    ///     in a game-encoded <paramref name="template"/> string.
    /// </summary>
    private static void pal_fill_template(Span<byte> template, byte fill) {
        Span<byte> scratch = stackalloc byte[8];

        int fill_length = Encoding.UTF8.GetBytes($"{fill}", scratch);
        int fill_target = template.IndexOf((byte)0x05);

        _ = FhEncoding.encode(scratch[ .. fill_length ], template[ (fill_target + 1) .. ]);

        template [ (fill_target + 1) .. ].CopyTo(template[ fill_target .. ]);
    }

    /* [fkelava 18/11/25 21:27]
     * Both Iggy and ImGui take null-terminated UTF-8 strings. Neither FhEncoding nor Encoding.UTF8
     * emit terminating null bytes by default, so we take care to do so manually in the PAL.
     */

    /* [fkelava 13/11/25 21:59]
     * FFX.exe  +2F0DA0, L66-80   (X)
     * FFX-2.exe+11DC50, L83-97   (X-2)
     * FFX-2.exe+11DC50, L161-165 (X-2 LM)
     */

    /// <summary>
    ///     Writes the icon ID of the current map in the save represented by
    ///     <paramref name="header"/> to <paramref name="dest"/> as a UTF-8 string.
    /// </summary>
    private unsafe void pal_get_icon_map(in ReadOnlySpan<byte> header, in Span<byte> dest) {
        bool not_lm      = FhGlobal.game_id is not FhGameId.FFX2LM;
        int  id_icon_map = not_lm
            ? _fnptr_fix_mappic(BinaryPrimitives.ReadUInt16LittleEndian(header[ pal_header_offset_locationid() .. ]))
            : ((int.Clamp(header[0x25] >> 1, 0, 0x50) - 1) / 0x14) + 1;

        if (not_lm && id_icon_map == pal_id_map_icon_clear() && _fnptr_isNeedShowJapanLogo() != 0) {
            id_icon_map = 999;
        }

        string str_icon_map = id_icon_map < 0x3E9
            ? $"m{id_icon_map}"
            : $"m{id_icon_map - 1000}_l";

        int len_icon_map = Encoding.UTF8.GetBytes(str_icon_map, dest);
        dest [ len_icon_map ] = 0x00;
    }

    /* [fkelava 13/11/25 22:08]
     * FFX-2.exe+11DC50, L66-69 (X-2)
     */

    /// <summary>
    ///     Writes the story chapter in the FF X-2 save represented by
    ///     <paramref name="header"/> to <paramref name="dest"/> as a UTF-8 string.
    /// </summary>
    private unsafe void pal_get_chapter(in ReadOnlySpan<byte> header, in Span<byte> dest) {
        if (FhGlobal.game_id is not FhGameId.FFX2) {
            dest[0] = 0x00;
            return;
        }

        byte*      ptr_chapter_encoded = FhUtil.ptr_at<byte>(0x9ED648);
        Span<byte> chapter_encoded     = new(ptr_chapter_encoded, 0x80);

        _fnptr_SaveDataGetLoc(0x4D8, ptr_chapter_encoded);
        pal_fill_template(chapter_encoded, header[0x0B]);

        int len_chapter = FhEncoding.decode(chapter_encoded, dest, flags: FhEncodingFlags.IMPLICIT_END);
        dest[ len_chapter ] = 0x00;
    }

    /* [fkelava 13/11/25 22:08]
     * FFX-2.exe+11DC50, L70-73 (X-2)
     */

    /// <summary>
    ///     Writes the story completion in the FF X-2 save represented by
    ///     <paramref name="header"/> to <paramref name="dest"/> as a UTF-8 string.
    /// </summary>
    private unsafe void pal_get_completion(in ReadOnlySpan<byte> header, in Span<byte> dest) {
        if (FhGlobal.game_id is not FhGameId.FFX2) {
            dest[0] = 0x00;
            return;
        }

        byte*      ptr_completion_encoded = FhUtil.ptr_at<byte>(0x9ED7C8);
        Span<byte> completion_encoded     = new(ptr_completion_encoded, 0x80);

        _fnptr_SaveDataGetLoc(0x39A, ptr_completion_encoded);
        pal_fill_template(completion_encoded, header[0x0C]);

        int len_completion = FhEncoding.decode(completion_encoded, dest, flags: FhEncodingFlags.IMPLICIT_END);
        dest [ len_completion ] = 0x00;
    }

    /* [fkelava 13/11/25 22:08]
     * FFX.exe  +2F0DA0, L57-61   (X)
     * FFX-2.exe+11DC50, L74-78   (X-2)
     * FFX-2.exe+11DC50, L139-144 (X-2 LM)
     */

    /// <summary>
    ///     Writes the total play time in the save represented by
    ///     <paramref name="header"/> to <paramref name="dest"/> as a UTF-8 string.
    /// </summary>
    private unsafe void pal_get_playtime(in ReadOnlySpan<byte> header, in Span<byte> dest) {
        uint playtime_secs = BinaryPrimitives.ReadUInt32LittleEndian(header [ 0x10 .. ]);
        uint playtime_mins = playtime_secs / 60;

        byte*              ptr_playtime_prefix_encoded = FhUtil.ptr_at<byte>(pal_addr_buf_playtime_prefix_encoded());
        ReadOnlySpan<byte> playtime_prefix_encoded     = new(ptr_playtime_prefix_encoded, int.MaxValue);

        _fnptr_SaveDataGetLoc(pal_id_playtime_prefix_SaveDataGetLoc(), ptr_playtime_prefix_encoded);

        int len_playtime_prefix = FhEncoding.decode(playtime_prefix_encoded, dest, flags: FhEncodingFlags.IMPLICIT_END);
        int len_playtime        = len_playtime_prefix + Encoding.UTF8.GetBytes($"  {playtime_mins / 60:D3}:{playtime_mins % 60:D2}:{playtime_secs % 60:D2}", dest[ len_playtime_prefix .. ]);

        dest [ len_playtime ] = 0x00;
    }

    /* [fkelava 13/11/25 22:08]
     * FFX.exe  +2F0DA0, L55      (X)
     * FFX-2.exe+11DC50, L64-65   (X-2)
     * FFX-2.exe+11DC50, L122-132 (X-2 LM)
     */

    /// <summary>
    ///     Writes the player character's name in the save represented by
    ///     <paramref name="header"/> to <paramref name="dest"/> as a UTF-8 string.
    /// </summary>
    private unsafe void pal_get_player_name(in ReadOnlySpan<byte> header, in Span<byte> dest) {
        int len_player_name;

        if (FhGlobal.game_id is FhGameId.FFX) {
            len_player_name = FhEncoding.decode(header[ 0x20 .. ], dest, flags: FhEncodingFlags.IMPLICIT_END);
            dest [ len_player_name ] = 0x00;
            return;
        }

        byte*              ptr_player_name_encoded = FhUtil.ptr_at<byte>(pal_addr_buf_player_name_encoded());
        ReadOnlySpan<byte> player_name_encoded     = new(ptr_player_name_encoded, int.MaxValue);

        _fnptr_SaveDataGetLoc(0xDD + header[0x21], ptr_player_name_encoded);

        len_player_name = FhEncoding.decode(player_name_encoded, dest, flags: FhEncodingFlags.IMPLICIT_END);
        dest [ len_player_name ] = 0x00;
    }

    /* [fkelava 13/11/25 22:08]
     * FFX-2.exe+11DC50, L137-138 (X-2 LM)
     */

    /// <summary>
    ///     Writes the player's job in the FF X-2 LM save represented by
    ///     <paramref name="header"/> to <paramref name="dest"/> as a UTF-8 string.
    /// </summary>
    private unsafe void pal_get_lm_job(in ReadOnlySpan<byte> header, in Span<byte> dest) {
        if (FhGlobal.game_id is not FhGameId.FFX2LM) {
            dest[0] = 0x00;
            return;
        }

        byte*              ptr_lm_job_encoded = _fnptr_GetLastMissionJobName(header[0x21], header[0x23]);
        ReadOnlySpan<byte> lm_job_encoded     = new(ptr_lm_job_encoded, int.MaxValue);

        int len_lm_job = FhEncoding.decode(lm_job_encoded, dest, flags: FhEncodingFlags.IMPLICIT_END);
        dest [ len_lm_job ] = 0x00;
    }

    /* [fkelava 13/11/25 22:08]
     * FFX-2.exe+11DC50, L133-136 (X-2 LM)
     */

    /// <summary>
    ///     Writes the player's level in the FF X-2 LM save represented by
    ///     <paramref name="header"/> to <paramref name="dest"/> as a UTF-8 string.
    /// </summary>
    private unsafe void pal_get_lm_level(in ReadOnlySpan<byte> header, in Span<byte> dest) {
        if (FhGlobal.game_id is not FhGameId.FFX2LM) {
            dest[0] = 0x00;
            return;
        }

        byte*              ptr_player_level_prefix_encoded = FhUtil.ptr_at<byte>(0x9ED378);
        ReadOnlySpan<byte> player_level_prefix_encoded     = new(ptr_player_level_prefix_encoded, int.MaxValue);

        _fnptr_SaveDataGetLoc(0x36B, ptr_player_level_prefix_encoded);

        int len_player_level_prefix = FhEncoding.decode(player_level_prefix_encoded, dest, flags: FhEncodingFlags.IMPLICIT_END);
        int len_player_level        = len_player_level_prefix + Encoding.UTF8.GetBytes($" {header[0x22]}", dest[ len_player_level_prefix .. ]);

        dest [ len_player_level ] = 0x00;
    }

    /// <summary>
    ///     Writes the current location in the save represented by <paramref name="header"/>
    ///     to <paramref name="dest"/> as a UTF-8 string.
    /// </summary>
    private unsafe void pal_get_location(in ReadOnlySpan<byte> header, in Span<byte> dest) {
        /* [fkelava 05/11/25 00:44]
         * Strings from AtelGetSaveDicName and SaveDataGetLoc are null-terminated. You can pass
         * a span with a bogus length to FhEncoding and it will properly handle it.
         *
         * Decodes like these (UTF-8 that is directly consumed by the game)
         * MUST specify the IMPLICIT_END flag to suppress unwanted {END} on every line.
         */
        if (FhGlobal.game_id is not FhGameId.FFX2LM) {
            ushort location_id = BinaryPrimitives.ReadUInt16LittleEndian(header[ 0x18 .. ]);

            byte*              ptr_location_name_encoded = _fnptr_AtelGetSaveDicName(location_id, 0);
            ReadOnlySpan<byte> location_name_encoded     = new(ptr_location_name_encoded, int.MaxValue);

            int len_location = FhEncoding.decode(location_name_encoded, dest, flags: FhEncodingFlags.IMPLICIT_END);
            dest [ len_location ] = 0x00;
            return;
        }

        byte*      ptr_lm_location_prefix_encoded = FhUtil.ptr_at<byte>(0x9ED058);
        byte*      ptr_lm_location_suffix_encoded = FhUtil.ptr_at<byte>(0x9ED158);
        Span<byte> lm_location_prefix_encoded     = new(ptr_lm_location_prefix_encoded, int.MaxValue);
        Span<byte> lm_location_suffix_encoded     = new(ptr_lm_location_suffix_encoded, 0x40);

        _fnptr_SaveDataGetLoc(0x4C1, ptr_lm_location_prefix_encoded);
        _fnptr_SaveDataGetLoc(0x4C2, ptr_lm_location_suffix_encoded);

        pal_fill_template(lm_location_suffix_encoded, (byte)(header[0x25] >> 1));

        int len_lm_location_prefix = FhEncoding.decode(lm_location_prefix_encoded, dest, flags: FhEncodingFlags.IMPLICIT_END);
        int len_lm_location        = len_lm_location_prefix + FhEncoding.decode(lm_location_suffix_encoded, dest[ len_lm_location_prefix .. ], flags: FhEncodingFlags.IMPLICIT_END);

        dest [ len_lm_location ] = 0x00;
    }

    /* [fkelava 13/11/25 22:08]
     * FFX.exe  +2F0DA0, L63-65   (X)
     * FFX-2.exe+11DC50, L80-82   (X-2)
     * FFX-2.exe+11DC50, L146-160 (X-2 LM)
     */

    // todo: not according to spec for LM

    /// <summary>
    ///     Writes to <paramref name="dest"/> the UTF-8 string ID of the icon for the
    ///     <paramref name="index"/>'th player character in the save represented by <paramref name="header"/>.
    /// </summary>
    private void pal_get_icon_chr(in ReadOnlySpan<byte> header, in Span<byte> dest, int index) {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, 2); // inform JIT of true access bounds

        int len_icon_chr = FhGlobal.game_id switch {
            FhGameId.FFX    => Encoding.UTF8.GetBytes($"_{header[0x05 + index] + 1}",                       dest),
            FhGameId.FFX2   or
            FhGameId.FFX2LM => Encoding.UTF8.GetBytes($"{header[0x05 + index] + 1}_{header[0x0D + index]}", dest),
            _               => throw new Exception("Invalid game type")
        };

        dest [ len_icon_chr ] = 0x00;
    }

    /* [fkelava 14/11/25 01:52]
     * The rest of the PAL are address or struct offset mappings between the same calls in
     * different games. You can go to these addresses in Ghidra and navigate up the XREFs/call graph.
     */

    internal static nint pal_addr_save_mgr() {
        return FhEnvironment.BaseAddr + FhGlobal.game_id switch {
            FhGameId.FFX    => 0x8E81E4,
            FhGameId.FFX2   or
            FhGameId.FFX2LM => 0x9EDABC,
            _               => throw new NotImplementedException("Invalid game type"),
        };
    }

    internal static delegate* unmanaged[Cdecl]<ushort, int> pal_addr_func_fix_mappic() {
        return (delegate* unmanaged[Cdecl]<ushort, int>)
        (FhEnvironment.BaseAddr + FhGlobal.game_id switch {
            FhGameId.FFX    => 0x2EF830,
            FhGameId.FFX2   or
            FhGameId.FFX2LM => 0x11C9B0,
            _               => throw new NotImplementedException("Invalid game type"),
        });
    }

    internal static delegate* unmanaged[Cdecl]<int> pal_addr_func_isNeedShowJapanLogo() {
        return (delegate* unmanaged[Cdecl]<int>)
        (FhEnvironment.BaseAddr + FhGlobal.game_id switch {
            FhGameId.FFX    => 0x387450,
            FhGameId.FFX2   or
            FhGameId.FFX2LM => 0x20F500,
            _               => throw new NotImplementedException("Invalid game type"),
        });
    }

    internal static delegate* unmanaged[Cdecl]<ushort, uint, byte*> pal_addr_func_AtelGetSaveDicName() {
        return (delegate* unmanaged[Cdecl]<ushort, uint, byte*>)
        (FhEnvironment.BaseAddr + FhGlobal.game_id switch {
            FhGameId.FFX    => 0x46C3C0,
            FhGameId.FFX2   or
            FhGameId.FFX2LM => 0x326B80,
            _               => throw new NotImplementedException("Invalid game type"),
        });
    }

    internal static delegate* unmanaged[Cdecl]<int, byte*, void> pal_addr_func_SaveDataGetLoc() {
        return (delegate* unmanaged[Cdecl]<int, byte*, void>)
        (FhEnvironment.BaseAddr + FhGlobal.game_id switch {
            FhGameId.FFX    => 0x2480E0,
            FhGameId.FFX2   or
            FhGameId.FFX2LM => 0x87CB0,
            _               => throw new NotImplementedException("Invalid game type"),
        });
    }

    internal static delegate* unmanaged[Cdecl]<nint, nint> pal_addr_func_SaveDataWriteCrc() {
        return (delegate* unmanaged[Cdecl]<nint, nint>)
        (FhEnvironment.BaseAddr + FhGlobal.game_id switch {
            FhGameId.FFX    => 0x2490D0,
            FhGameId.FFX2   or
            FhGameId.FFX2LM => 0x889C0,
            _               => throw new NotImplementedException("Invalid game type"),
        });
    }

    internal static delegate* unmanaged[Cdecl]<int> pal_addr_func_SaveDataCheckCrc() {
        return (delegate* unmanaged[Cdecl]<int>)
        (FhEnvironment.BaseAddr + FhGlobal.game_id switch {
            FhGameId.FFX    => 0x247F20,
            FhGameId.FFX2   or
            FhGameId.FFX2LM => 0x87B10,
            _               => throw new NotImplementedException("Invalid game type"),
        });
    }

    internal static delegate* unmanaged[Cdecl]<void> pal_addr_func__SetUpDefaultSaveFolder() {
        return (delegate* unmanaged[Cdecl]<void>)
        (FhEnvironment.BaseAddr + FhGlobal.game_id switch {
            FhGameId.FFX    => 0x2F0470,
            FhGameId.FFX2   or
            FhGameId.FFX2LM => 0x11D310,
            _               => throw new NotImplementedException("Invalid game type"),
        });
    }

    internal static delegate* unmanaged[Cdecl]<byte, bool> pal_addr_func_isNeedRenamePlayer() {
        return (delegate* unmanaged[Cdecl]<byte, bool>)
        (FhEnvironment.BaseAddr + FhGlobal.game_id switch {
            FhGameId.FFX    => 0x387430,
            FhGameId.FFX2   or
            FhGameId.FFX2LM => 0x20F4E0,
            _               => throw new NotImplementedException("Invalid game type"),
        });
    }

    internal static delegate* unmanaged[Cdecl]<int, void> pal_addr_func_SaveDataSaveLoadSucceed() {
        return (delegate* unmanaged[Cdecl]<int, void>)
        (FhEnvironment.BaseAddr + FhGlobal.game_id switch {
            FhGameId.FFX    => 0x2486F0,
            FhGameId.FFX2   or
            FhGameId.FFX2LM => 0x88290,
            _               => throw new NotImplementedException("Invalid game type"),
        });
    }

    internal static nint pal_addr_buf_player_name_encoded() {
        return FhGlobal.game_id switch {
            FhGameId.FFX2   => 0x9ED628,
            FhGameId.FFX2LM => 0x9ED358,
            _               => throw new NotImplementedException("Invalid game type"),
        };
    }

    internal static nint pal_addr_buf_playtime_prefix_encoded() {
        return FhGlobal.game_id switch {
            FhGameId.FFX    => 0x8E8058,
            FhGameId.FFX2   => 0x9ED948,
            FhGameId.FFX2LM => 0x9ED480,
            _               => throw new NotImplementedException("Invalid game type"),
        };
    }

    internal static nint pal_addr_screen_state() {
        return FhGlobal.game_id switch {
            FhGameId.FFX    => 0x8CB994,
            FhGameId.FFX2   or
            FhGameId.FFX2LM => 0x9CEA50,
            _               => throw new NotImplementedException("Invalid game type"),
        };
    }

    internal static nint pal_addr_dialog_state() {
        return FhGlobal.game_id switch {
            FhGameId.FFX    => 0x8CB998,
            FhGameId.FFX2   or
            FhGameId.FFX2LM => 0x9CEA54,
            _               => throw new NotImplementedException("Invalid game type"),
        };
    }

    internal static nint pal_addr_buf_save() {
        return FhGlobal.game_id switch {
            FhGameId.FFX    => 0x1197F30,
            FhGameId.FFX2   or
            FhGameId.FFX2LM => 0xF9E500,
            _               => throw new NotImplementedException("Invalid game type"),
        };
    }

    internal static int pal_buf_save_size() {
        return FhGlobal.game_id switch {
            FhGameId.FFX    => 0x6900,
            FhGameId.FFX2   or
            FhGameId.FFX2LM => 0x166A0,
            _               => throw new NotImplementedException("Invalid game type"),
        };
    }

    internal static nint pal_addr_status1() {
        return FhGlobal.game_id switch {
            FhGameId.FFX    => 0x8E72D4,
            FhGameId.FFX2   or
            FhGameId.FFX2LM => 0x9EC398,
            _               => throw new NotImplementedException("Invalid game type"),
        };
    }

    internal static nint pal_addr_force_player_rename() {
        return FhGlobal.game_id switch {
            FhGameId.FFX    => 0xD33350,
            FhGameId.FFX2   or
            FhGameId.FFX2LM => 0xA0FB70,
            _               => throw new NotImplementedException("Invalid game type"),
        };
    }

    internal static int pal_header_offset_playerrename() {
        return FhGlobal.game_id switch {
            FhGameId.FFX    => 0x0C, // FFX.exe+2F022E
            FhGameId.FFX2   or
            FhGameId.FFX2LM => 0x28, // FFX-2.exe+11D0BE
            _               => throw new NotImplementedException("Invalid game type"),
        };
    }

    internal static int pal_header_offset_locationid() {
        return FhGlobal.game_id switch {
            FhGameId.FFX  => 0x18, // FFX.exe+2F0E8D
            FhGameId.FFX2 => 0x2A, // FFX-2.exe+11E2A3
            _             => throw new NotImplementedException("Invalid game type"),
        };
    }

    internal static int pal_id_playtime_prefix_SaveDataGetLoc() {
        return FhGlobal.game_id switch {
            FhGameId.FFX    => 0x52, // FFX.exe+2F0F8E
            FhGameId.FFX2   or       // FFX-2.exe+11E203
            FhGameId.FFX2LM => 0x5C, // FFX-2.exe+11DF4D
            _               => throw new NotImplementedException("Invalid game type"),
        };
    }

    internal static int pal_id_map_icon_clear() {
        return FhGlobal.game_id switch {
            FhGameId.FFX  => 0x00, // FFX.exe+2F1039
            FhGameId.FFX2 => 0x17, // FFX-2.exe+11E2AD
            _             => throw new NotImplementedException("Invalid game type"),
        };
    }
}
