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
    DELETE         = 0x05, // Effectively unused on PC
    DELETE_SUCCEED = 0x06  // Effectively unused on PC
}

/// <summary>
///     Represents the possible states of the dialog box displayed in the save/load screen.
/// </summary>
public enum FhSaveDialogState {
    CLOSED = 0x00,
    UNK5   = 0x05
}

/// <summary>
///     Represents the possible states of the game's save data screen.
/// </summary>
public enum FhSaveScreenState {
    CLOSED  = 0x00,
    OPENING = 0x01,
    OPEN    = 0x02,
    UNK3    = 0x03,
    UNK4    = 0x04
}

/// <summary>
///     Represents the possible states of the game's default Iggy-based save/load UI.
///     <para/>
///     This is not used when Fahrenheit is overriding the save UI.
/// </summary>
public enum FhSaveUiState {
    /// <summary>
    ///     No-op. Flows from <see cref="SAVE_TERMINATING"/>.
    ///     Default state on boot, and in-game if the last action was a save.
    /// </summary>
    SAVE_TERMINATED     = 0x00,
    /// <summary>
    ///     Input is blocked. The disk is queried for existing save games.
    /// </summary>
    // "Loading. Do not close the game."
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

/* [fkelava 10/01/26 16:53]
 * The save PAL, being a binding to implementation details of each game,
 * is virtually illegible without consulting the original method bodies.
 *
 * For your convenience, most PAL methods are annotated with a source line you can look up.
 */

/// <summary>
///     Abstracts the game's save data system.
/// </summary>
internal static unsafe class FhSavePal {

    /* [fkelava 01/01/26 15:04]
     * TODO:
     * When FhCall is improved, this can be drastically simplified and
     * all the delegates and pal_addr_* functions can be removed.
     */

    internal const string DEFAULT_SET_NAME = "default";
    internal const int    DEFAULT_SET_SIZE = 500;

    // Game functions required by the PAL
    private static readonly delegate* unmanaged[Cdecl] <ushort, uint, byte*> _fnptr_AtelGetSaveDicName    = pal_fnaddr_AtelGetSaveDicName();
    private static readonly delegate* unmanaged[Cdecl] <ushort, int>         _fnptr_fix_mappic            = pal_fnaddr_fix_mappic();
    private static readonly delegate* unmanaged[Cdecl] <int>                 _fnptr_isNeedShowJapanLogo   = pal_fnaddr_isNeedShowJapanLogo();
    private static readonly delegate* unmanaged[Cdecl] <int, byte*, void>    _fnptr_SaveDataGetLoc        = pal_fnaddr_SaveDataGetLoc();
    private static readonly delegate* unmanaged[Cdecl] <byte, byte, byte*>   _fnptr_GetLastMissionJobName =
        (delegate* unmanaged[Cdecl] <byte, byte, byte*>)(FhEnvironment.BaseAddr + 0x368570); // X-2 LM exclusive; address abstraction not required

    internal static FhSaveDialogState pal_get_dialog_state()                        => FhUtil.get_at<FhSaveDialogState>(pal_addr_dialog_state());
    internal static void              pal_set_dialog_state(FhSaveDialogState value) => FhUtil.set_at(pal_addr_dialog_state(), value);

    internal static FhSaveScreenState pal_get_screen_state()                        => FhUtil.get_at<FhSaveScreenState>(pal_addr_screen_state());
    internal static void              pal_set_screen_state(FhSaveScreenState value) => FhUtil.set_at(pal_addr_screen_state(), value);

    /// <summary>
    ///     Sets the state of the in-game save manager to the given <paramref name="state"/>.
    /// </summary>
    internal static void pal_set_system_state(FhSaveSystemState state) {
        FhSaveDataManager*  mgr_x  = *(FhSaveDataManager **)pal_addr_save_mgr();
        FhSaveDataManager2* mgr_x2 = *(FhSaveDataManager2**)pal_addr_save_mgr();

        if (FhGlobal.game_id is FhGameId.FFX) {
            mgr_x->state = state;
            return;
        }

        mgr_x2->state = state;
    }

    /// <summary>
    ///     Signals to the game whether a save/load operation has been canceled by the user.
    /// </summary>
    internal static void pal_set_cancel_state(int cancel_state) {
        FhSaveDataManager*  mgr_x  = *(FhSaveDataManager **)pal_addr_save_mgr();
        FhSaveDataManager2* mgr_x2 = *(FhSaveDataManager2**)pal_addr_save_mgr();

        if (FhGlobal.game_id is FhGameId.FFX) {
            mgr_x->operation_canceled = cancel_state;
            return;
        }

        mgr_x2->operation_canceled = cancel_state;
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
    internal static void pal_fill_template(Span<byte> template, byte fill) {
        Span<byte> scratch = stackalloc byte[8];

        int fill_length = Encoding.UTF8.GetBytes($"{fill}", scratch);
        int fill_target = template.IndexOf((byte)0x05);

        _ = FhEncoding.encode(scratch[ .. fill_length ], template[ (fill_target + 1) .. ]);

        template [ (fill_target + 1) .. ].CopyTo(template[ fill_target .. ]);
    }

    /* [fkelava 18/11/25 21:27]
     * Both Iggy and ImGui take null-terminated UTF-8 strings. Neither FhEncoding nor Encoding.UTF8
     * emit terminating null bytes, so we take care to do so manually in the PAL.
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
    internal static void pal_get_icon_map(in ReadOnlySpan<byte> header, in Span<byte> dest) {
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
    internal static void pal_get_chapter(in ReadOnlySpan<byte> header, in Span<byte> dest) {
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
    internal static void pal_get_completion(in ReadOnlySpan<byte> header, in Span<byte> dest) {
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
    internal static void pal_get_playtime(in ReadOnlySpan<byte> header, in Span<byte> dest) {
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
    internal static void pal_get_player_name(in ReadOnlySpan<byte> header, in Span<byte> dest) {
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
    internal static void pal_get_lm_job(in ReadOnlySpan<byte> header, in Span<byte> dest) {
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
    internal static void pal_get_lm_level(in ReadOnlySpan<byte> header, in Span<byte> dest) {
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
    internal static void pal_get_location(in ReadOnlySpan<byte> header, in Span<byte> dest) {
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
    internal static void pal_get_icon_chr(in ReadOnlySpan<byte> header, in Span<byte> dest, int index) {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, 2); // inform JIT of true access bounds

        int len_icon_chr = FhGlobal.game_id switch {
            FhGameId.FFX    => Encoding.UTF8.GetBytes($"_{header[0x05 + index] + 1}",                       dest),
            FhGameId.FFX2   or
            FhGameId.FFX2LM => Encoding.UTF8.GetBytes($"{header[0x05 + index] + 1}_{header[0x0D + index]}", dest),
            _               => throw new Exception("Invalid game type")
        };

        dest [ len_icon_chr ] = 0x00;
    }

    /// <summary>
    ///     Gets the currently executing game's default save game folder.
    /// </summary>
    internal static string pal_get_save_default_folder() {
        return Path.Join(
            Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            "SQUARE ENIX",
            "FINAL FANTASY X&X-2 HD Remaster");
    }

    /// <summary>
    ///     Gets the filename prefix of a save game for the currently loaded game.
    /// </summary>
    internal static string pal_get_save_name_prefix() {
        return FhUtil.select("ffx", "ffx2", "ffx2");
    }

    /// <summary>
    ///     Gets the name of the save game folder for the currently loaded game.
    /// </summary>
    internal static string pal_get_save_subfolder() {
        return FhUtil.select(
            "FINAL FANTASY X",
            "FINAL FANTASY X-2",
            "FINAL FANTASY X-2 LAST MISSION");
    }

    /// <summary>
    ///     Gets the file name of the save file in the given <paramref name="slot"/>.
    /// </summary>
    internal static string pal_get_save_name_for_slot(int slot) {
        string prefix = pal_get_save_name_prefix();
        return $"{prefix}_{slot:D3}";
    }

    /* [fkelava 14/11/25 01:52]
     * The rest of the PAL are address or struct offset mappings between the same calls in
     * different games. You can go to these addresses in Ghidra and navigate up the XREFs/call graph.
     */

    internal static nint pal_addr_save_mgr() {
        return FhEnvironment.BaseAddr + FhUtil.select(0x8E81E4, 0x9EDABC, 0x9EDABC);
    }

    internal static delegate* unmanaged[Cdecl]<ushort, int> pal_fnaddr_fix_mappic() {
        return (delegate* unmanaged[Cdecl]<ushort, int>)
        (FhEnvironment.BaseAddr + FhUtil.select(0x2EF830, 0x11C9B0, 0x11C9B0));
    }

    internal static delegate* unmanaged[Cdecl]<int> pal_fnaddr_isNeedShowJapanLogo() {
        return (delegate* unmanaged[Cdecl]<int>)
        (FhEnvironment.BaseAddr + FhUtil.select(0x387450, 0x20F500, 0x20F500));
    }

    internal static delegate* unmanaged[Cdecl]<ushort, uint, byte*> pal_fnaddr_AtelGetSaveDicName() {
        return (delegate* unmanaged[Cdecl]<ushort, uint, byte*>)
        (FhEnvironment.BaseAddr + FhUtil.select(0x46C3C0, 0x326B80, 0x326B80));
    }

    internal static delegate* unmanaged[Cdecl]<int, byte*, void> pal_fnaddr_SaveDataGetLoc() {
        return (delegate* unmanaged[Cdecl]<int, byte*, void>)
        (FhEnvironment.BaseAddr + FhUtil.select(0x2480E0, 0x87CB0, 0x87CB0));
    }

    internal static delegate* unmanaged[Cdecl]<byte*, nint> pal_fnaddr_SaveDataWriteCrc() {
        return (delegate* unmanaged[Cdecl]<byte*, nint>)
        (FhEnvironment.BaseAddr + FhUtil.select(0x2490D0, 0x889C0, 0x889C0));
    }

    internal static delegate* unmanaged[Cdecl]<int> pal_fnaddr_SaveDataCheckCrc() {
        return (delegate* unmanaged[Cdecl]<int>)
        (FhEnvironment.BaseAddr + FhUtil.select(0x247F20, 0x87B10, 0x87B10));
    }

    internal static delegate* unmanaged[Cdecl]<void> pal_fnaddr__SetUpDefaultSaveFolder() {
        return (delegate* unmanaged[Cdecl]<void>)
        (FhEnvironment.BaseAddr + FhUtil.select(0x2F0470, 0x11D310, 0x11D310));
    }

    internal static delegate* unmanaged[Cdecl]<byte, bool> pal_fnaddr_isNeedRenamePlayer() {
        return (delegate* unmanaged[Cdecl]<byte, bool>)
        (FhEnvironment.BaseAddr + FhUtil.select(0x387430, 0x20F4E0, 0x20F4E0));
    }

    internal static delegate* unmanaged[Cdecl]<FhSaveSystemState, void> pal_fnaddr_SaveDataSaveLoadSucceed() {
        return (delegate* unmanaged[Cdecl]<FhSaveSystemState, void>)
        (FhEnvironment.BaseAddr + FhUtil.select(0x2486F0, 0x88290, 0x88290));
    }

    internal static nint pal_addr_buf_player_name_encoded() {
        return FhGlobal.game_id switch {
            FhGameId.FFX2   => 0x9ED628,
            FhGameId.FFX2LM => 0x9ED358,
            _               => throw new NotImplementedException("Invalid game type"),
        };
    }

    internal static nint pal_addr_buf_playtime_prefix_encoded() {
        return FhUtil.select(0x8E8058, 0x9ED948, 0x9ED480);
    }

    internal static nint pal_addr_screen_state() {
        return FhUtil.select(0x8CB994, 0x9CEA50, 0x9CEA50);
    }

    internal static nint pal_addr_dialog_state() {
        return FhUtil.select(0x8CB998, 0x9CEA54, 0x9CEA54);
    }

    internal static unsafe byte* pal_addr_buf_save() {
        return FhUtil.ptr_at<byte>(FhUtil.select(0x1197F30, 0xF9E500, 0xF9E500));
    }

    internal static int pal_sz_buf_save() {
        return FhUtil.select(0x6900, 0x166A0, 0x166A0);
    }

    internal static nint pal_addr_force_player_rename() {
        return FhUtil.select(0xD33350, 0xA0FB70, 0xA0FB70);
    }

    internal static int pal_header_offset_playerrename() {
        // FFX.exe+2F022E, FFX-2.exe+11D0BE
        return FhUtil.select(0x0C, 0x28, 0x28);
    }

    internal static int pal_header_offset_locationid() {
        return FhGlobal.game_id switch {
            FhGameId.FFX  => 0x18, // FFX.exe+2F0E8D
            FhGameId.FFX2 => 0x2A, // FFX-2.exe+11E2A3
            _             => throw new NotImplementedException("Invalid game type"),
        };
    }

    internal static int pal_id_playtime_prefix_SaveDataGetLoc() {
        // FFX.exe+2F0F8E, FFX-2.exe+11E203, FFX-2.exe+11DF4D
        return FhUtil.select(0x52, 0x5C, 0x5C);
    }

    internal static int pal_id_map_icon_clear() {
        return FhGlobal.game_id switch {
            FhGameId.FFX  => 0x00, // FFX.exe+2F1039
            FhGameId.FFX2 => 0x17, // FFX-2.exe+11E2AD
            _             => throw new NotImplementedException("Invalid game type"),
        };
    }
}

