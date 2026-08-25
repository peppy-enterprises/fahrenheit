// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit;

using SaveCounts = Dictionary<FhGameId, Dictionary<string, int>>;

/// <summary>
///     Allows multiple sets of saves to exist.
/// </summary>
internal sealed class FhSaves {

    /* [fkelava 07/11/25 15:01]
     * Fh computes a load-order sensitive hash over all mods that declare 'separate saves'.
     * It creates a 'default' save set for that hash. The user may create manual sets.
     * At runtime, you may swap between all sets for the hash.
     */

    private          int                     _sm_lock;
    private readonly HashSet<string>         _sm_sets;
    private readonly SaveCounts              _sm_set_save_counts;
    private readonly HashSet<int>            _sm_occupied_slots;
    private          string                  _sm_active_set;
    private readonly string                  _sm_path_base;
    private readonly string                  _sm_path_default_set;
    private readonly List<FhSaveDisplayData> _sm_display_data;

    public FhSaves() {
        _sm_path_base        = Path.Join(FhEnvironment.Finder.Saves.FullName, FhInternal.Hasher.SaveSetHash);
        _sm_path_default_set = Path.Join(_sm_path_base, FhSavePal.DEFAULT_SET_NAME, FhSavePal.pal_get_save_subfolder());
        _sm_occupied_slots   = [];
        _sm_sets             = [];
        _sm_active_set       = FhSavePal.DEFAULT_SET_NAME;
        _sm_display_data     = [];

        _sm_set_save_counts  = new() {
            { FhGameId.FFX,    [] },
            { FhGameId.FFX2,   [] },
            { FhGameId.FFX2LM, [] },
        };

        /* [fkelava 19/01/26 18:13]
         * Save PAL is not ready to perform indexing operations at 'init' time because
         * AtelGetSaveDicName & co. are not usable before game initialization has run.
         *
         * Set indexing is performed just in time when SEM transitions to save/load mode.
         */

        _sm_create_default_set();
        _sm_query_sets();
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
        _sm_display_data  .Clear();
        _sm_occupied_slots.Clear();

        /* [fkelava 08/02/26 15:00]
         * Save sets mirror the base game save directory's structure, i.e. the subfolders
         * 'FINAL FANTASY X', 'FINAL FANTASY X-2', 'FINAL FANTASY X-2 LAST MISSION' exist.
         *
         * When the user creates a set manually (not through the Fh API or mod manager),
         * it is possible they forgot to create these subdirectories. When loading this fails
         * safely because the saves will simply never be found. When saving this is lethal
         * because SMM assumes the path it is writing to exists for simplicity.
         *
         * In this case we silently correct their error. I/O faults are still propagated
         * because that is not something we can gracefully handle.
         */

        string path_set_folder = Path.Join(
            _sm_path_base,
            _sm_active_set,
            FhSavePal.pal_get_save_subfolder());

        _ = Directory.CreateDirectory(path_set_folder);

        foreach (var save_file in Directory.EnumerateFiles(path_set_folder, FhUtil.select("ffx_*", "ffx2_*", "ffx2_*"))) {
            FhSaveDisplayData  display_data = new();
            ReadOnlySpan<char> slot_str     = save_file[ (save_file.LastIndexOf('_') + 1) .. ];

            if (!int.TryParse(slot_str, out int slot))
                continue;

            using (FileStream save_file_stream = File.OpenRead(save_file)) {
                save_file_stream.ReadExactly(display_data.header);
            }

            FhSavePal.pal_get_location   (display_data.header, display_data.location);
            FhSavePal.pal_get_icon_map   (display_data.header, display_data.icon_map);
            FhSavePal.pal_get_player_name(display_data.header, display_data.player_name);
            FhSavePal.pal_get_playtime   (display_data.header, display_data.play_time);
            FhSavePal.pal_get_chapter    (display_data.header, display_data.chapter);
            FhSavePal.pal_get_completion (display_data.header, display_data.completion);
            FhSavePal.pal_get_lm_job     (display_data.header, display_data.lm_job);
            FhSavePal.pal_get_lm_level   (display_data.header, display_data.lm_level);

            _ = Encoding.UTF8.GetBytes($"{slot}\0", display_data.slot_str);
            _ = Encoding.UTF8.GetBytes($"{File.GetLastWriteTimeUtc(save_file):yyyy/MM/dd HH:mm:ss}\0", display_data.create_time);

            display_data.slot = slot;

            _sm_occupied_slots.Add(slot);
            _sm_display_data  .Add(display_data);
        }
    }

    /// <summary>
    ///     Queries the disk for available save sets.
    /// </summary>
    private void _sm_query_sets() {
        _sm_sets.Clear();

        _sm_set_save_counts[FhGameId.FFX]   .Clear();
        _sm_set_save_counts[FhGameId.FFX2]  .Clear();
        _sm_set_save_counts[FhGameId.FFX2LM].Clear();

        foreach (string dir in Directory.EnumerateDirectories(_sm_path_base)) {
            string set_name = Path.GetFileName(dir);
            _sm_sets.Add(set_name);

            string x_dir  = Path.Join(dir, "FINAL FANTASY X");
            string x2_dir = Path.Join(dir, "FINAL FANTASY X-2");
            string lm_dir = Path.Join(dir, "FINAL FANTASY X-2 LAST MISSION");

            DirectoryInfo x_dir  = Directory.CreateDirectory(Path.Join(dir, "FINAL FANTASY X"));
            DirectoryInfo x2_dir = Directory.CreateDirectory(Path.Join(dir, "FINAL FANTASY X-2"));
            DirectoryInfo lm_dir = Directory.CreateDirectory(Path.Join(dir, "FINAL FANTASY X-2 LAST MISSION"));

            _sm_set_save_counts[FhGameId.FFX   ][set_name] = x_dir .GetFiles("ffx_*") .Length;
            _sm_set_save_counts[FhGameId.FFX2  ][set_name] = x2_dir.GetFiles("ffx2_*").Length;
            _sm_set_save_counts[FhGameId.FFX2LM][set_name] = lm_dir.GetFiles("ffx2_*").Length;
        }

        /* [fkelava 19/01/26 14:50]
         * Sets can be modified on the disk under us. The user can create a new one, delete the
         * active one, or even delete the _default_ set, which is meant to be a system invariant.
         *
         * We can't stop that, but we can disregard their nonsense. The default set is forcibly
         * regenerated if it does not exist, and we fall back to it if the active set was torched.
         */

        if (!_sm_sets.Contains(_sm_active_set)) {
            FhInternal.Log.Warning($"Active set {_sm_active_set} was deleted; falling back to default.");
            _sm_active_set = FhSavePal.DEFAULT_SET_NAME;
        }

        if (!_sm_sets.Contains(FhSavePal.DEFAULT_SET_NAME)) {
            FhInternal.Log.Error("Default set was deleted; forcibly re-generating.");

            _sm_create_default_set();
            _sm_query_sets();
        }
    }

    internal string                  get_active_set()   => _sm_active_set;
    internal List<FhSaveDisplayData> get_display_data() => _sm_display_data;

    internal IReadOnlySet<string> get_sets() {
        _sm_query_sets();
        return _sm_sets;
    }

    internal IReadOnlyDictionary<string, int> get_save_counts() {
        return _sm_set_save_counts[FhGlobal.game_id];
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
    ///     Reindexes the active save set.
    /// </summary>
    internal void index_active_set() {
        if (Interlocked.CompareExchange(ref _sm_lock, 1, 0) != 0)
            return;

        _sm_index_active_set();

        Interlocked.Decrement(ref _sm_lock);
    }

    /// <summary>
    ///     For a given <paramref name="slot"/>, gets the full path of the corresponding save file.
    /// </summary>
    internal string get_save_path_for_slot(int slot) {
        return Path.Join(
            _sm_path_base,
            _sm_active_set,
            FhSavePal.pal_get_save_subfolder(),
            FhSavePal.pal_get_save_name_for_slot(slot));
    }

    /// <summary>
    ///     Get the number of used slots in the current set.
    /// </summary>
    internal int get_slots_used() {
        return _sm_occupied_slots.Contains(0)
            ? _sm_display_data.Count - 1
            : _sm_display_data.Count;
    }

    /// <summary>
    ///     For a given <paramref name="menu_index"/>, returns the slot number being saved to.
    /// </summary>
    internal int get_slot_save(int menu_index) {
        // This method is not re-entrant.
        if (menu_index != 0) return menu_index;

        int target_slot = 1;
        while (_sm_occupied_slots.Contains(target_slot)) { target_slot++; }

        _sm_occupied_slots.Add(target_slot);
        return target_slot;
    }
}
