// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core;

/* [fkelava 08/11/25 21:45]
 * These are direct mappings of game structures.
 * We reuse them for simplicity, ignoring the stored creation date/time logic in favor of OS API calls.
 */

/// <summary>
///     Represents the possible states of the game's save data manager.
/// </summary>
public enum FhSaveSystemState : int {
    IDLE           = 0x00,
    SAVE           = 0x01,
    SAVE_SUCCEED   = 0x02,
    LOAD           = 0x03,
    LOAD_SUCCEED   = 0x04,
    DELETE         = 0x05,
    DELETE_SUCCEED = 0x06
}

public enum FhSaveDialogState {
    CLOSED = 0x00,
    UNK5   = 0x05
}

public enum FhSaveScreenState {
    CLOSED  = 0x00,
    OPENING = 0x01,
    OPEN    = 0x02,
    UNK3    = 0x03,
    UNK4    = 0x04
}

[StructLayout(LayoutKind.Sequential, Size = 0x488)]
public unsafe struct FhSaveDataManager {
    public int                    save_enabled;
    public int                    cb_result;
    public FhSaveSystemState      state;
    public int                    __0xC;
    public InlineArray64 <byte>   game_name;
    public InlineArray128<byte>   description;
    public InlineArray512<byte>   description_detailed;
    public InlineArray64 <byte>   path_icon_1;
    public InlineArray64 <byte>   path_icon_2;
    public byte*                  ref_buffer;
    public int                    ref_buffer_size;
    public int                    operation_canceled;
}

[StructLayout(LayoutKind.Sequential, Size = 0x4B0)]
public unsafe struct FhSaveDataManager2 {
    public FhSaveSystemState      state;
    public int                    save_enabled;
    public int                    cb_result;
    public InlineArray64 <byte>   game_name;
    public InlineArray128<byte>   description;
    public InlineArray512<byte>   description_detailed;
    public InlineArray64 <byte>   path_icon_1;
    public InlineArray64 <byte>   path_icon_2;
    public byte*                  ref_buffer;
    public int                    ref_buffer_size;
    public int                    operation_canceled;
}

/* [fkelava 22/12/25 15:54]
 * This enum is not used, but is provided as documentation of base game behavior.
 */

public enum FhIggySaveUiState {
    // NOP, flows from SAVE_TERMINATING
    // Default state on boot, and in-game when last action was save
    SAVE_TERMINATED     = 0x00,
    // "Loading. Do not close the game."
    // Input blocked
    SAVE_LISTING        = 0x01,
    // Listing complete, handling input
    SAVE_IDLE           = 0x02,
    // "Save this game? Yes/No"
    SAVE_PROMPT_IDLE    = 0x03,
    // "Saving. Do not close the game."
    //` h_save` invoked on this/the next main loop
    SAVE_REQUESTED      = 0x04,
    // `h_save` finished
    // Dismisses "Saving. Do not close the game"
    // Listing triggered
    SAVE_COMPLETE       = 0x05,
    // Destroy menu, swap to state 0x00
    SAVE_TERMINATING    = 0x06,
    // NOP, flows from LOAD_TERMINATING
    // Default state in-game when last action was load
    LOAD_TERMINATED     = 0x0A,
    // "Loading. Do not close the game."
    // Input blocked
    LOAD_LISTING        = 0x0B,
    // Listing complete, handling input
    LOAD_IDLE           = 0x0C,
    // "Load this game? Yes/No"
    LOAD_PROMPT_IDLE    = 0x0D,
    // "Loading. Do not close the game."
    // `h_load` invoked on this/the next main loop
    LOAD_REQUESTED      = 0x0E,
    // `h_load` finished
    // Dismisses "Loading. Do not close the game"
    // CRC/player name checks triggered
    LOAD_POSTPROCESS    = 0x0F,
    // CRC corrupt or player name needs reset
    // Wait for player to acknowledge this - CRC is fatal, playername is non-fatal fault
    LOAD_FAILED         = 0x10,
    // Destroy menu, swap to state 0x0A
    LOAD_TERMINATING    = 0x11,

    // FF X only, Al-Bhed Compilation Sphere modes
    ALBHED_TERMINATED    = 0x14,
    ALBHED_LISTING       = 0x15,
    ALBHED_IDLE          = 0x16,
    ALBHED_REQUESTED     = 0x17,
    ALBHED_COMPLETE      = 0x18,
    ALBHED_POSTPROCESS   = 0x19,
    ALBHED_TERMINATING_A = 0x1A,
    ALBHED_TERMINATING_B = 0x1B,
}

[StructLayout(LayoutKind.Sequential)]
internal struct FhSaveListEntry(int slot_nb) {
    public int slot          = slot_nb;
    public int creation_date = -1;
    public int creation_time = -1;
}

[InlineArray(0x20)]
internal struct FhSavePlayerName {
    private byte _b;
}

[StructLayout(LayoutKind.Sequential)]
internal struct FhSaveHeader {
    public uint             _0x00;
    public byte             _0x04;
    public byte             id_chr1;
    public byte             id_chr2;
    public byte             id_chr3;
    public uint             _0x08;
    public uint             _0x0c;
    public uint             playtime_sec;
    public uint             _0x14;
    public ushort           id_location;
    public ushort           _0x1a;
    public ushort           _0x1c;
    public ushort           _0x1e;
    public FhSavePlayerName player_name;
};

[StructLayout(LayoutKind.Sequential)]
internal struct FhSaveHeader2 {
    public uint   _0x00;
    public byte   _0x04;
    public byte   id_chr1;
    public byte   id_chr2;
    public byte   id_chr3;
    public byte   _0x08;
    public byte   _0x09;
    public byte   _0x0A;
    public byte   chapter;
    public byte   completion;
    public byte   id_chr1_dress;
    public byte   id_chr2_dress;
    public byte   id_chr3_dress;
    public uint   playtime_secs;
    public uint   _0x14;
    public ushort location_id;
    public ushort _0x1A;
    public uint   _0x1C;
    public byte   _0x20;
    public byte   _0x21;
    public byte   _0x22;
    public byte   _0x23;
    public byte   _0x24;
    public byte   _0x25;
    public byte   _0x26;
    public byte   _0x27;
    public byte   _0x28;
    public byte   _0x29;
    public byte   _0x2A;
    public byte   _0x2B;
    public byte   _0x2C;
}

/// <summary>
///     Extends the game's save system by allowing multiple sets of saves to exist.
/// </summary>
internal class FhSaveManager {

    /* [fkelava 07/11/25 15:01]
     * Fh computes a load-order sensitive hash over all mods that declare 'separate saves'.
     * It creates a 'default' save set for that hash. The user may create manual sets.
     * At runtime, you may swap between all sets for the hash.
     */

    private const string DEFAULT_STATE_HASH = "0";
    private const string DEFAULT_SET_NAME   = "default";
    private const int    DEFAULT_SET_SIZE   = 500;

    private readonly string[]          _sets;
    private readonly int[]             _slots;
    private readonly FhSaveListEntry[] _saves;
    private          int               _save_count;
    private          int               _active_set;
    private          int               _locked;

    internal readonly string StateHash;

    public FhSaveManager() {
        StateHash = _init_state_hash(FhEnvironment.Manifests);

        _slots = new int            [DEFAULT_SET_SIZE];
        _saves = new FhSaveListEntry[DEFAULT_SET_SIZE];
        _sets  = _init_sets();

        _init_index();
    }

    /// <summary>
    ///     Computes the hash of all mods which carry local state or request
    ///     separate saves, such that they may be isolated from others.
    /// </summary>
    private static string _init_state_hash(FhManifest[] manifests) {
        StringBuilder stateful_mods = new();

        foreach (FhManifest manifest in manifests) {
            if (manifest.Flags.HasFlag(FhManifestFlags.SEPARATE_SAVES))
                stateful_mods.Append(manifest.Id);
        }

        if (stateful_mods.Length == 0)
            return DEFAULT_STATE_HASH;

        byte[] stateful_mods_utf8 = Encoding.UTF8.GetBytes(stateful_mods.ToString());
        return Convert.ToHexString(SHA256.HashData(stateful_mods_utf8));
    }

    /// <summary>
    ///     Probes the save directory for all sets available for this game session.
    /// </summary>
    private string[] _init_sets() {
        string base_path = Path.Join(FhEnvironment.Finder.Saves.FullName, StateHash);

        Directory.CreateDirectory(Path.Join(base_path, DEFAULT_SET_NAME, get_save_subfolder()));

        return [ .. Directory.EnumerateDirectories(base_path) ];
    }

    public int                  get_active_set() => _active_set;
    public ReadOnlySpan<string> get_sets()       => _sets;

    internal bool set_swap_locked() {
        return Interlocked.CompareExchange(ref _locked, 0, 0) != 0;
    }

    /// <summary>
    ///     Sets the active save set to the <paramref name="set_index"/>'th set.
    /// </summary>
    internal void set_swap(int set_index) {
        if (Interlocked.CompareExchange(ref _locked, 1, 0) != 0)
            return;

        _active_set = set_index;
        _init_index();

        Interlocked.Decrement(ref _locked);
    }

    /// <summary>
    ///     Indexes the active save set's directory. This function must be called under lock.
    /// </summary>
    private void _init_index() {

        /* [fkelava 08/11/25 21:21]
         * As in the base game, `-1` indicates the absence and `1` the presence of a save in a slot.
         */

        _save_count = 0;
        _slots.AsSpan().Fill(-1);

        for (int slot = 0; slot < DEFAULT_SET_SIZE; slot++) {
            if (File.Exists(get_save_path_for_slot(slot))) {
                _slots[slot]          = 1;
                _saves[_save_count++] = new (slot);
            }
        }

        _saves[ _save_count .. ].AsSpan().Fill(new (-1));
    }

    /// <summary>
    ///     For a given <paramref name="slot"/>, gets the full path of the corresponding save file.
    /// </summary>
    internal string get_save_path_for_slot(int slot) {
        string base_dir = string.Equals(StateHash, DEFAULT_STATE_HASH, StringComparison.OrdinalIgnoreCase)
            ? get_save_default_folder()
            : _sets[_active_set];

        return Path.Join(base_dir, get_save_subfolder(), get_save_name_for_slot(slot));
    }

    /// <summary>
    ///     Gets the currently executing game's default save game folder.
    /// </summary>
    internal static string get_save_default_folder() {
        return Path.Join(
            Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            "SQUARE ENIX",
            "FINAL FANTASY X&X-2 HD Remaster");
    }

    /// <summary>
    ///     Gets the name of the save game folder for the currently loaded game.
    /// </summary>
    internal static string get_save_subfolder() {
        return FhGlobal.game_id switch {
            FhGameId.FFX    => "FINAL FANTASY X",
            FhGameId.FFX2   => "FINAL FANTASY X-2",
            FhGameId.FFX2LM => "FINAL FANTASY X-2 LAST MISSION",
            _               => throw new NotImplementedException("Invalid game type"),
        };
    }

    /// <summary>
    ///     Gets the filename prefix of a save game for the currently loaded game.
    /// </summary>
    internal static string get_save_name_prefix() {
        return FhGlobal.game_id switch {
            FhGameId.FFX    => "ffx",
            FhGameId.FFX2   or
            FhGameId.FFX2LM => "ffx2",
            _               => throw new NotImplementedException("Invalid game type"),
        };
    }

    /// <summary>
    ///     Gets the file name of the save file in the given <paramref name="slot"/>.
    /// </summary>
    internal static string get_save_name_for_slot(int slot) {
        string prefix = get_save_name_prefix();
        return $"{prefix}_{slot:D3}";
    }

    /// <summary>
    ///     Get the number of used up slots in the current set.
    /// </summary>
    internal int get_slots_used() {
        return _save_count;
    }

    /// <summary>
    ///     Get the total number of slots in the current set.
    /// </summary>
    internal int get_slots_total() {
        return _slots.Length;
    }

    /// <summary>
    ///     For a given <paramref name="menu_index"/>, returns the slot number being loaded from.
    /// </summary>
    internal int get_slot_load(int menu_index) {
        return _saves[menu_index].slot;
    }

    /// <summary>
    ///     For a given <paramref name="menu_index"/>, returns the slot number being saved to.
    /// </summary>
    internal int get_slot_save(int menu_index) {
        // TODO: can this method ever be re-entrant? if so, locking is req'd
        if (menu_index == 0) menu_index++;

        ReadOnlySpan<int>             slots = _slots;
        ReadOnlySpan<FhSaveListEntry> saves = _saves;

        bool is_overwrite = saves[menu_index].slot != -1;
        int  target_slot  = is_overwrite
            ? saves[menu_index].slot
            : slots.IndexOf(-1);

        _slots[target_slot]      = 1;
        _saves[target_slot].slot = target_slot;

        if (!is_overwrite) _save_count++;

        return target_slot;
    }
}
