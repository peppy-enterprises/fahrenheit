// SPDX-License-Identifier: MIT

using System.Diagnostics;
using System.Threading;

namespace Fahrenheit.Core.Runtime;

/// <summary>
///     Allows multiple sets of saves to exist.
///     <para/>
///     Do not interface with this module. It has no public API.
/// </summary>
[FhLoad(FhGameId.FFX | FhGameId.FFX2 | FhGameId.FFX2LM)]
public sealed class FhSaveManagerModule : FhModule {

    /* [fkelava 07/11/25 15:01]
     * Fh computes a load-order sensitive hash over all mods that declare 'separate saves'.
     * It creates a 'default' save set for that hash. The user may create manual sets.
     * At runtime, you may swap between all sets for the hash.
     */

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void Sg_MainInit(nint arg1, nint arg2);

    private          int                 _sm_lock;
    private readonly HashSet<string>     _sm_sets;
    private          string              _sm_active_set;
    private readonly int[]               _sm_active_set_slots;
    private readonly int[]               _sm_active_set_saves;
    private          int                 _sm_active_set_count;
    private readonly string              _sm_path_base;
    private readonly string              _sm_path_default_set;
    private readonly FhSaveDisplayData[] _sm_display_data;

    private readonly FhMethodHandle<Sg_MainInit> _handle_sginit;

    public FhSaveManagerModule() {
        FhMethodLocation loc_ginit = new(0x420970, 0x204EE0);

        _sm_path_base        = Path.Join(FhEnvironment.Finder.Saves.FullName, FhEnvironment.StateHash);
        _sm_path_default_set = Path.Join(_sm_path_base, FhSavePal.DEFAULT_SET_NAME, FhSavePal.pal_get_save_subfolder());
        _sm_sets             = [];
        _sm_active_set_slots = new int[FhSavePal.DEFAULT_SET_SIZE];
        _sm_active_set_saves = new int[FhSavePal.DEFAULT_SET_SIZE];
        _sm_active_set       = FhSavePal.DEFAULT_SET_NAME;
        _sm_display_data     = new FhSaveDisplayData[FhSavePal.DEFAULT_SET_SIZE];

        _handle_sginit = new(this, loc_ginit, h_sginit);
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        _sm_create_default_set();
        _sm_query_sets();

        return true;
    }

    /* [fkelava 19/01/26 18:13]
     * Save PAL is not ready to perform indexing operations at 'init' time because
     * AtelGetSaveDicName & co. are not usable before game initialization has run.
     *
     * Thus we index the default set after the game has had the chance to initialize.
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvStdcall) ] )]
    private void h_sginit(nint arg1, nint arg2) {
        _handle_sginit.orig_fptr(arg1, arg2);
        _sm_index_active_set();
    }

    /* [fkelava 19/01/26 12:03]
     * The save manager is considered essential, as is any part of the runtime. We throw
     * if we cannot do basic file I/O; there is no point trying to handle it gracefully.
     */

    /// <summary>
    ///     Ensures the default set exists. A default set must exist for every state hash.
    /// </summary>
    private void _sm_create_default_set() {
        _ = Directory.CreateDirectory(_sm_path_default_set);
    }

    /// <summary>
    ///     Indexes the active save set's directory. This function must be called under lock.
    /// </summary>
    private void _sm_index_active_set() {

        /* [fkelava 08/11/25 21:21]
         * As in the base game, `-1` indicates the absence and `1` the presence of a save in a slot.
         */

        _sm_active_set_count = 0;
        _sm_active_set_slots.AsSpan().Fill(-1);

        for (int slot = 0; slot < FhSavePal.DEFAULT_SET_SIZE; slot++) {
            _sm_display_data[slot].valid = false;

            string   save_file_path = get_save_path_for_slot(slot);
            FileInfo save_file      = new FileInfo(save_file_path);

            if (!save_file.Exists) continue;

            using (FileStream save_file_stream = save_file.OpenRead()) {
                save_file_stream.ReadExactly(_sm_display_data[slot].header);
            }

            FhSavePal.pal_get_location   (_sm_display_data[slot].header, _sm_display_data[slot].location);
            FhSavePal.pal_get_icon_chr   (_sm_display_data[slot].header, _sm_display_data[slot].icon_chr1, 0);
            FhSavePal.pal_get_icon_chr   (_sm_display_data[slot].header, _sm_display_data[slot].icon_chr2, 1);
            FhSavePal.pal_get_icon_chr   (_sm_display_data[slot].header, _sm_display_data[slot].icon_chr3, 2);
            FhSavePal.pal_get_icon_map   (_sm_display_data[slot].header, _sm_display_data[slot].icon_map);
            FhSavePal.pal_get_player_name(_sm_display_data[slot].header, _sm_display_data[slot].player_name);
            FhSavePal.pal_get_playtime   (_sm_display_data[slot].header, _sm_display_data[slot].play_time);
            FhSavePal.pal_get_chapter    (_sm_display_data[slot].header, _sm_display_data[slot].chapter);
            FhSavePal.pal_get_completion (_sm_display_data[slot].header, _sm_display_data[slot].completion);
            FhSavePal.pal_get_lm_job     (_sm_display_data[slot].header, _sm_display_data[slot].lm_job);
            FhSavePal.pal_get_lm_level   (_sm_display_data[slot].header, _sm_display_data[slot].lm_level);

            _ = Encoding.UTF8.GetBytes($"{slot}\0", _sm_display_data[slot].slot);
            _ = Encoding.UTF8.GetBytes($"{save_file.LastWriteTimeUtc:yyyy/MM/dd HH:mm:ss}\0", _sm_display_data[slot].create_time);

            _sm_active_set_slots[slot]          = 1;
            _sm_active_set_saves[_sm_active_set_count++] = slot;

            _sm_display_data[slot].valid = true;
        }

        _sm_active_set_saves[ _sm_active_set_count .. ].AsSpan().Fill(-1);
    }

    /// <summary>
    ///     Queries the disk for available save sets.
    /// </summary>
    private void _sm_query_sets() {
        _sm_sets.Clear();

        foreach (string dir in Directory.EnumerateDirectories(_sm_path_base)) {
            string set_name = Path.GetFileName(dir);
            _sm_sets.Add(set_name);
        }

        /* [fkelava 19/01/26 14:50]
         * Sets can be modified on the disk under us. The user can create a new one, delete the
         * active one, or even delete the _default_ set, which is meant to be a system invariant.
         *
         * We can't stop that, but we can disregard their nonsense. The default set is forcibly
         * regenerated if it does not exist, and we fall back to it if the active set was torched.
         */

        if (!_sm_sets.Contains(_sm_active_set)) {
            _logger.Warning($"Active set {_sm_active_set} was deleted; falling back to default.");
            _sm_active_set = FhSavePal.DEFAULT_SET_NAME;
        }

        if (!_sm_sets.Contains(FhSavePal.DEFAULT_SET_NAME)) {
            _logger.Error("Default set was deleted; forcibly re-generating.");

            _sm_create_default_set();
            _sm_query_sets();
        }
    }

    internal string                          get_active_set()   => _sm_active_set;
    internal ReadOnlySpan<FhSaveDisplayData> get_display_data() => _sm_display_data;

    internal IReadOnlySet<string> get_sets() {
        _sm_query_sets();
        return _sm_sets;
    }

    /// <summary>
    ///     Sets the active save set to <paramref name="set_name"/>, then indexes it.
    /// </summary>
    internal void switch_active_set(string set_name) {
        if (Interlocked.CompareExchange(ref _sm_lock, 1, 0) != 0)
            return;

        _sm_active_set = set_name;
        _sm_index_active_set();

        Interlocked.Decrement(ref _sm_lock);
    }

    /// <summary>
    ///     For a given <paramref name="slot"/>, gets the full path of the corresponding save file.
    /// </summary>
    internal string get_save_path_for_slot(int slot) {
        string base_dir = string.Equals(FhEnvironment.StateHash, FhEnvironment.DEFAULT_STATE_HASH, StringComparison.OrdinalIgnoreCase)
            ? FhSavePal.pal_get_save_default_folder()
            : Path.Join(_sm_path_base, _sm_active_set);

        return Path.Join(base_dir, FhSavePal.pal_get_save_subfolder(), FhSavePal.pal_get_save_name_for_slot(slot));
    }

    /// <summary>
    ///     Get the number of used up slots in the current set.
    /// </summary>
    internal int get_slots_used() {
        return _sm_active_set_count;
    }

    /// <summary>
    ///     Get the total number of slots in the current set.
    /// </summary>
    internal int get_slots_total() {
        return _sm_active_set_slots.Length;
    }

    /// <summary>
    ///     For a given <paramref name="menu_index"/>, returns the slot number being loaded from.
    /// </summary>
    internal int get_slot_load(int menu_index) {
        return _sm_active_set_saves[menu_index];
    }

    /// <summary>
    ///     For a given <paramref name="menu_index"/>, returns the slot number being saved to.
    /// </summary>
    internal int get_slot_save(int menu_index) {
        // This method is not re-entrant.
        ReadOnlySpan<int> slots = _sm_active_set_slots;
        ReadOnlySpan<int> saves = _sm_active_set_saves;

        int target_slot = menu_index != 0
            ? saves[menu_index]
            : slots.IndexOf(-1);
        Debug.Assert(target_slot != -1);

        _sm_active_set_slots[target_slot] = 1;
        _sm_active_set_saves[target_slot] = target_slot;

        if (menu_index == 0) _sm_active_set_count++;

        return target_slot;
    }
}
