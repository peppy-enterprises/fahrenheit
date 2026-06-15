// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit.Runtime;

/* [fkelava 13/11/25 22:03]
 * The game's original save system is limiting. A global limit of 200 saves exist,
 * and they cannot be separated between modsets. While applications like Mod Organizer 2
 * can retrofit set functionality to any game, we must interface with most of the
 * save system _anyway_ to provide local state callbacks.
 *
 * My first attempt at doing this involved hooking the game's Flash-based Iggy UI to display custom
 * save lists. While this worked, it proved both hideously complex and untenably slow.
 *
 * A complete UI replacement in ImGui was decided instead. As the vanilla UI and save system are
 * tightly bound, this effectively meant I had to re-implement the entire save system.
 */

internal enum FhSaveExtensionSystemState {
    NULL = 0,
    LOAD = 1,
    SAVE = 2,
    ALBD = 3 // FFX only; Al Bhed Compilation Sphere mode
}

/// <summary>
///     Implements Fahrenheit's extended save system.
/// </summary>
[FhLoad(FhGameId.FFX | FhGameId.FFX2 | FhGameId.FFX2LM)]
public unsafe sealed class FhSaveExtensionModule : FhModule {

    private FhLocalStateModule?        _lsm;
    private FhSaveManagerModule?       _smm;
    private int                        _load_pending_slot;
    private FhSaveExtensionSystemState _state;

    public FhSaveExtensionModule() { }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        FhModuleHandle<FhLocalStateModule>  lsm_handle = new(this);
        FhModuleHandle<FhSaveManagerModule> smm_handle = new(this);

        bool is_ffx = FhGlobal.game_id is FhGameId.FFX;

        return FhCall.h_SaveDataManager_debugSave_Internal_6F0650.hook(this, impl_autosave)
            && FhCall.h_TkMenuJumpToLoadedScene                  .hook(this, impl_copy)
            && FhCall.h_SaveDataToSave                           .hook(this, signal_enter_save)
            && FhCall.h_SaveDataToLoad                           .hook(this, signal_enter_load)
            && (!is_ffx || FFX.FhCall.h_FUN_2EFFF0.hook(this, signal_enter_albd))
            && lsm_handle.try_get_module(out _lsm)
            && smm_handle.try_get_module(out _smm);
    }

    internal FhSaveExtensionSystemState get_system_state() => _state;

    /* [fkelava 27/11/25 02:15]
     * These five functions are the transition points to and from the save system UI.
     *
     * The entry points correspond to real game functions:
     * 'signal_enter_save' <-> 'SaveDataToSave'
     * 'signal_enter_load' <-> 'SaveDataToLoad'
     * 'signal_enter_albd' <-> CT {TBD} in FFX
     *
     * while the exit transitions are 'constructed' from bits and pieces that would normally occur
     * in Iggy state handlers 0x06, 0x11, and abort bits in HandleSaveDataScreen.
     */

    /// <summary>
    ///     Signals to both Fahrenheit and the game that the next save/load operation will be a save.
    /// </summary>
    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void signal_enter_save() {
        _smm!.index_active_set();
        _state = FhSaveExtensionSystemState.SAVE;
        FhSavePal.pal_set_system_state(FhSaveSystemState.SAVE);
    }

    /// <summary>
    ///     Signals to both Fahrenheit and the game that the next save/load operation will be a load.
    /// </summary>
    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void signal_enter_load() {
        _smm!.index_active_set();
        _state = FhSaveExtensionSystemState.LOAD;
        FhSavePal.pal_set_system_state(FhSaveSystemState.LOAD);
    }

    /// <summary>
    ///     Signals to both Fahrenheit and the game that the next save/load operation will be
    ///     an Al Bhed Compilation Sphere load. Only valid in FF-X.
    /// </summary>
    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl ) ] )]
    private void signal_enter_albd() {
        _smm!.index_active_set();
        _state = FhSaveExtensionSystemState.ALBD;
        FhSavePal.pal_set_system_state(FhSaveSystemState.LOAD);
    }

    /// <summary>
    ///     Signals to the game that the pending save/load operation ended in failure.
    /// </summary>
    internal void signal_exit_abort() {
        FhSavePal.pal_set_cancel_state(1);
        FhCall.h_SaveDataSaveLoadSucceed.fnptr!(_state is FhSaveExtensionSystemState.SAVE
            ? FhSaveSystemState.SAVE
            : FhSaveSystemState.LOAD);
        FhSavePal.pal_set_dialog_state(FhSaveDialogState.CLOSED);
    }

    /// <summary>
    ///     Signals to the game that the pending save/load operation ended in success.
    /// </summary>
    internal void signal_exit_success() {
        FhSavePal.pal_set_cancel_state(0);
        FhCall.h_SaveDataSaveLoadSucceed.fnptr!(_state is FhSaveExtensionSystemState.SAVE
            ? FhSaveSystemState.SAVE
            : FhSaveSystemState.LOAD);
        FhSavePal.pal_set_dialog_state(FhSaveDialogState.CLOSED);
    }

    /* [fkelava 16/01/26 14:29]
     * https://github.com/fahrenheit-crew/fahrenheit/issues/70
     *
     * Loading in the game is a multi-step process:
     * 1) Data from the disk is loaded to `SaveDataManager::getRefBuffer`.
     * 2) CRC checking and other validation is performed on the ref buffer.
     * 3) If verification passed, the save data is later copied to `save_ram`.
     * 4) Fahrenheit's SaveData accessors (and the rest of the game) only look at `save_ram`.
     *
     * with the following constraints:
     * - Between 1) and 3) there is at least one main loop iteration of delay.
     * - SaveData must be inspectable at `load_local_state` time.
     * - SaveExtensionModule must load into the ref buffer for correctness, because
     *   `SaveDataCheckCrc` et al. implicitly operate on the ref buffer.
     * - Information about the slot being loaded is lost after 1).
     *
     * Thus we defer `load_local_state` to after 3), storing the pending load slot at 1).
     *
     * As the actual function performing the copy to `save_ram` is inlined on some platforms,
     * we hook its parent function, TkMenuJumpToLoadedScene.
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvStdcall) ] )]
    private void impl_copy() {
        FhCall.h_TkMenuJumpToLoadedScene.chain_from(impl_copy).fnptr!();
        _lsm!.state_load_slot(_load_pending_slot);

        FhApi.Events.Common.GameLoop.PostLoadGame.invoke(new() { save_slot_idx = _load_pending_slot });
    }

    /// <summary>
    ///     Creates the autosave, the save game in the reserved slot 0.
    /// </summary>
    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void impl_autosave(int size, byte* ptr) {
        FhCall.h_SaveDataWriteCrc       .fnptr!(ptr);
        FhCall.h__SetUpDefaultSaveFolder.fnptr!();

        string             save_path = _smm!.get_save_path_for_slot(0);
        ReadOnlySpan<byte> save      = new(ptr, size);

        using (FileStream save_stream = File.OpenWrite(save_path)) {
            save_stream.Write(save);
        }

        _lsm!.state_save_slot(0);
    }

    /* [fkelava 19/01/26 16:12]
     * The game exhibits no-throw behavior on most violations of save system invariants. Load fails
     * with 'Save data is corrupt'. Autosave no-ops without indicating fault. Saving crashes.
     *
     * We do not follow this. Attempting to suppress failure to perform I/O in Fahrenheit dirs is
     * just kicking the ball down the curb to mod authors, who can do nothing in such cases.
     */

    /// <summary>
    ///     Creates a save file in the slot corresponding to the
    ///     selected <paramref name="index"/> in the save/load menu.
    /// </summary>
    internal void save(int index) {
        int    slot      = _smm!.get_slot_save(index);
        string save_path = _smm!.get_save_path_for_slot(slot);

        ReadOnlySpan<byte> save = new(FhSavePal.pal_addr_buf_save(), FhSavePal.pal_sz_buf_save());
        FhCall.h_SaveDataWriteCrc.fnptr!(FhSavePal.pal_addr_buf_save());

        // TODO: add popups on success/failure

        using (FileStream save_stream = File.OpenWrite(save_path)) {
            save_stream.Write(save);
        }

        _lsm!.state_save_slot(slot);
        signal_exit_success();
    }

    /// <summary>
    ///     Loads the save file in the slot corresponding to the
    ///     selected <paramref name="index"/> in the save/load menu.
    /// </summary>
    internal void load(int index) {
        int    slot      = _smm!.get_slot_load(index);
        string save_name = _smm!.get_save_path_for_slot(slot);

        Span<byte> save = new(FhSavePal.pal_addr_buf_save(), FhSavePal.pal_sz_buf_save());

        // TODO: add popups on success/failure
        using (FileStream save_stream = File.OpenRead(save_name)) {
            save_stream.ReadExactly(save);
        }

        // From Iggy state handler 0xF
        if (FhCall.h_SaveDataCheckCrc.fnptr!() == 0) {
            save.Clear();

            signal_exit_abort();
            return;
        }

        bool player_needs_rename = FhCall.h_isNeedRenamePlayer.fnptr!(
            save[ FhSavePal.pal_header_offset_playerrename() ]);

        FhUtil.set_at(FhSavePal.pal_addr_force_player_rename(), player_needs_rename);

        _load_pending_slot = slot;
        signal_exit_success(); // TODO: popup if success
    }

    /// <summary>
    ///     Performs an Al Bhed Compilation Sphere load from the save
    ///     at the given <paramref name="index"/> in the save/load menu.
    /// </summary>
    internal void load_albd(int index) {
        int    slot      = _smm!.get_slot_load(index);
        string save_name = _smm!.get_save_path_for_slot(slot);

        //Span<byte> save = new(
        //    FhUtil.ptr_at<byte>(pal_addr_buf_save()),
        //    pal_buf_save_size()
        //    );

        //using (FileStream save_stream = File.OpenRead(save_name)) {
        //    save_stream.ReadExactly(save);
        //}

        signal_exit_success();
    }
}
