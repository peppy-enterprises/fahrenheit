// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* [fkelava 26/04/26 15:26]
 * Unlike `call.g.cs`, which contains source-generated delegates with no guarantee of accuracy,
 * this file contains manually annotated calls with proper Fahrenheit types that are vetted for functionality.
 *
 * This file is for calls which are exclusive to FF X and not shared with X-2/LM.
 */

using Fahrenheit.FFX.Battle;

namespace Fahrenheit.FFX;

/// <summary>
///     An accessor for game function calls exclusive to FF X.
/// </summary>
public static unsafe partial class FhCall {
    private const string GAME = "FFX.exe";

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void FUN_2EFFF0();
    internal static FhMethodHandle<FUN_2EFFF0> h_FUN_2EFFF0
        => new( new FhMethodLocation(GAME, 0x2EFFF0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsGetSaveCommand(int chr_id, uint com_id);
    public static FhMethodHandle<MsGetSaveCommand> h_MsGetSaveCommand
        => new( new FhMethodLocation(GAME, 0x3850E0));

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsPayGIL(int param_1);
    public static FhMethodHandle<MsPayGIL> h_MsPayGIL 
        => new( new FhMethodLocation(GAME, 0x385A60));

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsSetSaveCommand(int chr_id, uint param_2, int param_3);
    public static FhMethodHandle<MsSetSaveCommand> h_MsSetSaveCommand
        => new( new FhMethodLocation(GAME, 0x385D10) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsAliveProcess(int chr_id, Chr* chr);
    public static FhMethodHandle<MsAliveProcess> h_MsAliveProcess
        => new( new FhMethodLocation(GAME, 0x389220) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsBlowProcess(int chr_id, Chr* chr);
    public static FhMethodHandle<MsBlowProcess> h_MsBlowProcess
        => new( new FhMethodLocation(GAME, 0x389270) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsDamageCheckDeath(int attacker_id, int target_id, int param_3, int targeting_self);
    public static FhMethodHandle<MsDamageCheckDeath> h_MsDamageCheckDeath
        => new( new FhMethodLocation(GAME, 0x38C800) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsDamageSetMotion(int chr_id, int param_2, int targeting_self);
    public static FhMethodHandle<MsDamageSetMotion> h_MsDamageSetMotion
        => new( new FhMethodLocation(GAME, 0x38CAE0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsLimitStatusProcess(int chr_id, Chr* chr, uint param_3);
    public static FhMethodHandle<MsLimitStatusProcess> h_MsLimitStatusProcess
        => new( new FhMethodLocation(GAME, 0x38D330) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsSetChrWeak(int chr_id, int new_weak_level);
    public static FhMethodHandle<MsSetChrWeak> h_MsSetChrWeak
        => new( new FhMethodLocation(GAME, 0x38D8B0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate bool MsAutoRelifeProcess(int attacker_id, Chr* attacker, int target_id, Chr* target);
    public static FhMethodHandle<MsAutoRelifeProcess> h_MsAutoRelifeProcess
        => new( new FhMethodLocation(GAME, 0x38D990) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsStoneProcess(int chr_id, Chr* chr);
    public static FhMethodHandle<MsStoneProcess> h_MsStoneProcess
        => new( new FhMethodLocation(GAME, 0x38E210) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsSubCTB(int chr_id, Chr* chr, int param_3, int param_4, uint param_5, uint param_6);
    public static FhMethodHandle<MsSubCTB> h_MsSubCTB
        => new( new FhMethodLocation(GAME, 0x38E2A0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsSubHP(int chr_id, Chr* chr, int param_3, int param_4, int param_5, uint param_6, uint param_7);
    public static FhMethodHandle<MsSubHP> h_MsSubHP
        => new( new FhMethodLocation(GAME, 0x38E2F0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsSubMP(int chr_id, Chr* chr, int param_3, int param_4, int param_5, uint param_6, uint param_7);
    public static FhMethodHandle<MsSubMP> h_MsSubMP
        => new( new FhMethodLocation(GAME, 0x38E400) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsThreatProcess(int chr_id, Chr* chr);
    public static FhMethodHandle<MsThreatProcess> h_MsThreatProcess
        => new( new FhMethodLocation(GAME, 0x38E4B0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate uint MsAfterDamageProcess(int attacker_id, uint param_2, int target_id, uint* param_4, uint param_5);
    public static FhMethodHandle<MsAfterDamageProcess> h_MsAfterDamageProcess
        => new( new FhMethodLocation(GAME, 0x38F0B0));

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate byte* MsGetRomBtlText(int param_1, int param_2);
    public static FhMethodHandle<MsGetRomBtlText> h_MsGetRomBtlText
        => new( new FhMethodLocation(GAME, 0x38F940) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsMenuCloseTitleWindow(int param_1);
    public static FhMethodHandle<MsMenuCloseTitleWindow> h_MsMenuCloseTitleWindow 
        => new( new FhMethodLocation(GAME, 0x38FA80));

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate Chr* MsGetChr(int chr_id);
    public static FhMethodHandle<MsGetChr> h_MsGetChr
        => new( new FhMethodLocation(GAME, 0x394030));

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int brnd(int rng_idx);
    public static FhMethodHandle<brnd> h_brnd
        => new( new FhMethodLocation(GAME, 0x398900) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsCheckRange(int param_1, int param_2, int param_3);
    public static FhMethodHandle<MsCheckRange> h_MsCheckRange
        => new( new FhMethodLocation(GAME, 0x39A0D0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate Command* MsGetComData(int com_id, byte** param_2);
    public static FhMethodHandle<MsGetComData> h_MsGetComData
        => new( new FhMethodLocation(GAME, 0x39A4C0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsSetRamChrAbility(int chr_id, Chr* chr);
    public static FhMethodHandle<MsSetRamChrAbility> h_MsSetRamChrAbility 
        => new( new FhMethodLocation(GAME, 0x39BB70));

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte MsMessageCueProcess();
    public static FhMethodHandle<MsMessageCueProcess> h_MsMessageCueProcess
        => new( new FhMethodLocation(GAME, 0x39CE10) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsMessageCueRegist(uint type, int param_2, int param_3, byte param_4, byte param_5);
    public static FhMethodHandle<MsMessageCueRegist> h_MsMessageCueRegist 
        => new( new FhMethodLocation(GAME, 0x39CFF0));

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsSetStealEffect(int param_1, int param_2);
    public static FhMethodHandle<MsSetStealEffect> h_MsSetStealEffect 
        => new( new FhMethodLocation(GAME, 0x39ED20));

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsSetStealGillEffect(int param_1, int param_2);
    public static FhMethodHandle<MsSetStealGillEffect> h_MsSetStealGillEffect
        => new( new FhMethodLocation(GAME, 0x39ED40) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsStatusDefenseEffect(int attacker_id, int target_id, int dmg_calc_flags);
    public static FhMethodHandle<MsStatusDefenseEffect> h_MsStatusDefenseEffect
        => new( new FhMethodLocation(GAME, 0x39EE40) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsStatusEffectCheck(int chr_id);
    public static FhMethodHandle<MsStatusEffectCheck> h_MsStatusEffectCheck
        => new( new FhMethodLocation(GAME, 0x39F010) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsNumberRegist(int param_1, int param_2, int param_3, int param_4, int param_5, uint param_6, uint param_7);
    public static FhMethodHandle<MsNumberRegist> h_MsNumberRegist
        => new( new FhMethodLocation(GAME, 0x39FA20) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint MsRegSEplay(byte p1, int p2);
    public static FhMethodHandle<MsRegSEplay> h_MsRegSEplay
        => new( new FhMethodLocation(GAME, 0x3A0120) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate uint MsRegSEplay2(int param_1, uint param_2);
    public static FhMethodHandle<MsRegSEplay2> h_MsRegSEplay2 
        => new( new FhMethodLocation(GAME, 0x3A0160));

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsPopBtlPos(Chr* chr);
    public static FhMethodHandle<MsPopBtlPos> h_MsPopBtlPos
        => new( new FhMethodLocation(GAME, 0x3AC620) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsActionRequest(int target_id, int attacker_id, int param_3, int param_4, int param_5, void* param_6);
    public static FhMethodHandle<MsActionRequest> h_MsActionRequest
        => new( new FhMethodLocation(GAME, 0x3ACEC0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsLimitTidusLearn(int chr_id);
    public static FhMethodHandle<MsLimitTidusLearn> h_MsLimitTidusLearn 
        => new( new FhMethodLocation(GAME, 0x3B0CE0));

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsLimitTypeDamageCheck(int attacker_id, Chr* attacker, int target_id, Chr* target, int param_5, int param_6, int param_7);
    public static FhMethodHandle<MsLimitTypeDamageCheck> h_MsLimitTypeDamageCheck
        => new( new FhMethodLocation(GAME, 0x3B0D60) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsLimitTypeStatusCheck(int attacker_id, Chr* attacker, int target_id, Chr* target, int param_5, uint param_6);
    public static FhMethodHandle<MsLimitTypeStatusCheck> h_MsLimitTypeStatusCheck
        => new( new FhMethodLocation(GAME, 0x3B12D0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsAutoCureProcess(int target_id, Chr* target, int attacker_id, int poison, int zombie, int darkness, int silence);
    public static FhMethodHandle<MsAutoCureProcess> h_MsAutoCureProcess
        => new( new FhMethodLocation(GAME, 0x3B2520) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsAutoPotionProcess(int target_id, Chr* target, int attacker_id);
    public static FhMethodHandle<MsAutoPotionProcess> h_MsAutoPotionProcess
        => new( new FhMethodLocation(GAME, 0x3B2860) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void achievementUnlockAchievement(int ach_id);
    public static FhMethodHandle<achievementUnlockAchievement> h_achievementUnlockAchievement
        => new( new FhMethodLocation(GAME, 0x422410) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void AtelSetEventJump2(int room, int entrance, int do_fade);
    public static FhMethodHandle<AtelSetEventJump2> h_AtelSetEventJump2
        => new( new FhMethodLocation(GAME, 0x46FED0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsSetSaveCommandWithPrefix(int chr_id, int com_id, int param_3);
    public static FhMethodHandle<MsSetSaveCommandWithPrefix> h_MsSetSaveCommandWithPrefix
        => new( new FhMethodLocation(GAME, 0x474190) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void TOBtlCloseSimpleHelpMes();
    public static FhMethodHandle<TOBtlCloseSimpleHelpMes> h_TOBtlCloseSimpleHelpMes
        => new( new FhMethodLocation(GAME, 0x490E60) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int TOBtlDrawCaptureMonsterMessageWindow(int mon_id, int text_id);
    public static FhMethodHandle<TOBtlDrawCaptureMonsterMessageWindow> h_TOBtlDrawCaptureMonsterMessageWindow
        => new( new FhMethodLocation(GAME, 0x4927E0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int TOBtlDrawFirstStrikeEnemyMessageWindow();
    public static FhMethodHandle<TOBtlDrawFirstStrikeEnemyMessageWindow> h_TOBtlDrawFirstStrikeEnemyMessageWindow
        => new( new FhMethodLocation(GAME, 0x493440) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int TOBtlDrawFirstStrikePlayerMessageWindow();
    public static FhMethodHandle<TOBtlDrawFirstStrikePlayerMessageWindow> h_TOBtlDrawFirstStrikePlayerMessageWindow
        => new( new FhMethodLocation(GAME, 0x493460) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int TOBtlDrawGetItemMessageWindow(byte* item_name, int amount);
    public static FhMethodHandle<TOBtlDrawGetItemMessageWindow> h_TOBtlDrawGetItemMessageWindow
        => new( new FhMethodLocation(GAME, 0x493480) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int TOBtlDrawGetLimitTypeMessageWindow(int ply_id, int limit_mode);
    public static FhMethodHandle<TOBtlDrawGetLimitTypeMessageWindow> h_TOBtlDrawGetLimitTypeMessageWindow
        => new( new FhMethodLocation(GAME, 0x493560) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int TOBtlDrawGetMoneyMessageWindow(int amount);
    public static FhMethodHandle<TOBtlDrawGetMoneyMessageWindow> h_TOBtlDrawGetMoneyMessageWindow
        => new( new FhMethodLocation(GAME, 0x4935D0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int TOBtlDrawLearningMessageWindow(int ply_id, int com_id);
    public static FhMethodHandle<TOBtlDrawLearningMessageWindow> h_TOBtlDrawLearningMessageWindow
        => new( new FhMethodLocation(GAME, 0x495290) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int TOBtlDrawStdChrNameMessageWindow(int chr_id, int text_id);
    public static FhMethodHandle<TOBtlDrawStdChrNameMessageWindow> h_TOBtlDrawStdChrNameMessageWindow
        => new( new FhMethodLocation(GAME, 0x497170) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int FUN_0089db10(int p1, byte* text);
    public static FhMethodHandle<FUN_0089db10> h_FUN_0089db10
        => new( new FhMethodLocation(GAME, 0x49DB10) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate byte* TOGetSaveChrName(int chr_id);
    public static FhMethodHandle<TOGetSaveChrName> h_TOGetSaveChrName
        => new( new FhMethodLocation(GAME, 0x4AC800) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void TOBtlSetMacroCommandType(int param_1, int param_2, byte param_3);
    public static FhMethodHandle<TOBtlSetMacroCommandType> h_TOBtlSetMacroCommandType
        => new( new FhMethodLocation(GAME, 0x4B5770) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void TOBtlSetMacroCommandValue(int param_1, int param_2, byte* param_3);
    public static FhMethodHandle<TOBtlSetMacroCommandValue> h_TOBtlSetMacroCommandValue
        => new( new FhMethodLocation(GAME, 0x4B57A0) );

}
