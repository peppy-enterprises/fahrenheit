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

using Fahrenheit.Atel;
using Fahrenheit.FFX.Battle;
using Fahrenheit.FFX.Ids;

namespace Fahrenheit.FFX;

/// <summary>
///     An accessor for "FFX.exe" function calls exclusive to FF X.
/// </summary>
public static unsafe partial class FhCall {
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public unsafe delegate bool openFile(nint _this, nint filename, bool readOnly, nint unknown_1, nint unknown_2, nint unknown_3);
    public static FhMethodHandle<openFile> h_openFile
        => new( new FhMethodLocation("FFX.exe", 0x208100) );

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public unsafe delegate uint readFile(nint _this, nint buffer, uint max_len);
    public static FhMethodHandle<readFile> h_readFile
        => new( new FhMethodLocation("FFX.exe", 0x208250) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int sceClose(void* p1);
    public static FhMethodHandle<sceClose> h_sceClose
        => new( new FhMethodLocation("FFX.exe", 0x22F7C0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int sceLseek(void* p1, int p2, int p3);
    public static FhMethodHandle<sceLseek> h_sceLseek
        => new( new FhMethodLocation("FFX.exe", 0x22FA90) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void* sceOpen(byte* p1, int p2);
    public static FhMethodHandle<sceOpen> h_sceOpen
        => new( new FhMethodLocation("FFX.exe", 0x22FBE0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int sceRead(void* p1, void* dst, int amount);
    public static FhMethodHandle<sceRead> h_sceRead
        => new( new FhMethodLocation("FFX.exe", 0x22FDB0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate bool graphicInitFMVPlayer(int movie_id, int param_2);
    public static FhMethodHandle<graphicInitFMVPlayer> h_graphicInitFMVPlayer
        => new( new FhMethodLocation("FFX.exe", 0x241840) );

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public unsafe delegate void FUN_00656c90(int param_1, int param_2, char* fileName);
    public static FhMethodHandle<FUN_00656c90> h_FUN_00656c90
        => new( new FhMethodLocation("FFX.exe", 0x256C90) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void fiosUnifyFilename(nint in_string, nint out_buffer, int buffer_size);
    public static FhMethodHandle<fiosUnifyFilename> h_fiosUnifyFilename
        => new( new FhMethodLocation("FFX.exe", 0x2799D0) );

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public unsafe delegate void LocalizationManager_Initialize(LocalizationManager* localizationManager);
    public static FhMethodHandle<LocalizationManager_Initialize> h_LocalizationManager_Initialize
        => new( new FhMethodLocation("FFX.exe", 0x2DB1C0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal unsafe delegate void FUN_2EFFF0();
    internal static FhMethodHandle<FUN_2EFFF0> h_FUN_2EFFF0
        => new( new FhMethodLocation("FFX.exe", 0x2EFFF0) );

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public unsafe delegate void FfxFmod_soundInit(nint ffxFmod);
    public static FhMethodHandle<FfxFmod_soundInit> h_FfxFmod_soundInit
        => new( new FhMethodLocation("FFX.exe", 0x307170) );

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public unsafe delegate int FmodVoice_dataChange(nint FmodVoice, int event_id, nint param_2);
    public static FhMethodHandle<FmodVoice_dataChange> h_FmodVoice_dataChange
        => new( new FhMethodLocation("FFX.exe", 0x30A720) );

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public unsafe delegate void FmodVoice_initList(nint fmodVoice);
    public static FhMethodHandle<FmodVoice_initList> h_FmodVoice_initList
        => new( new FhMethodLocation("FFX.exe", 0x30AC80) );

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public unsafe delegate uint FUN_0070aec0(nint _this, uint voice_id, uint param_2);
    public static FhMethodHandle<FUN_0070aec0> h_FUN_0070aec0
        => new( new FhMethodLocation("FFX.exe", 0x30AEC0) );

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public unsafe delegate void FfxFmod_soundInit_setLang(nint ffxFmod, int lang);
    public static FhMethodHandle<FfxFmod_soundInit_setLang> h_FfxFmod_soundInit_setLang
        => new( new FhMethodLocation("FFX.exe", 0x30B4E0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsBattleEncountExe(int field_id, int group_idx, float walked_delta);
    public static FhMethodHandle<MsBattleEncountExe> h_MsBattleEncountExe
        => new( new FhMethodLocation("FFX.exe", 0x380DE0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void ResetEncountExe(int p1);
    public static FhMethodHandle<ResetEncountExe> h_ResetEncountExe
        => new( new FhMethodLocation("FFX.exe", 0x3810C0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsBattleExe(uint param_1, int field_idx, int group_idx, int formation_idx);
    public static FhMethodHandle<MsBattleExe> h_MsBattleExe
        => new( new FhMethodLocation("FFX.exe", 0x3810F0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsBattleLabelExe(uint encounter_id, byte param_2, byte screen_transition);
    public static FhMethodHandle<MsBattleLabelExe> h_MsBattleLabelExe
        => new( new FhMethodLocation("FFX.exe", 0x381D60) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsBtlReadManage();
    public static FhMethodHandle<MsBtlReadManage> h_MsBtlReadManage
        => new( new FhMethodLocation("FFX.exe", 0x3830D0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_00783bb0(byte mon_idx);
    public static FhMethodHandle<FUN_00783bb0> h_FUN_00783bb0
        => new( new FhMethodLocation("FFX.exe", 0x383BB0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate byte MsBtlReadSetScene();
    public static FhMethodHandle<MsBtlReadSetScene> h_MsBtlReadSetScene
        => new( new FhMethodLocation("FFX.exe", 0x383ED0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsGetSaveCommand(int chr_id, uint com_id);
    public static FhMethodHandle<MsGetSaveCommand> h_MsGetSaveCommand
        => new( new FhMethodLocation("FFX.exe", 0x3850E0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsGetSavePartyMember(uint* param_1, uint* param_2, uint* param_3);
    public static FhMethodHandle<MsGetSavePartyMember> h_MsGetSavePartyMember
        => new( new FhMethodLocation("FFX.exe", 0x3853B0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsPayGIL(int param_1);
    public static FhMethodHandle<MsPayGIL> h_MsPayGIL
        => new( new FhMethodLocation("FFX.exe", 0x385A60) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsSetSaveCommand(int chr_id, uint param_2, int param_3);
    public static FhMethodHandle<MsSetSaveCommand> h_MsSetSaveCommand
        => new( new FhMethodLocation("FFX.exe", 0x385D10) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsSetSaveParam(uint chr_id);
    public static FhMethodHandle<MsSetSaveParam> h_MsSetSaveParam
        => new( new FhMethodLocation("FFX.exe", 0x3861B0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsSetSaveParamAll();
    public static FhMethodHandle<MsSetSaveParamAll> h_MsSetSaveParamAll
        => new( new FhMethodLocation("FFX.exe", 0x3869C0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsAliveProcess(int chr_id, Chr* chr);
    public static FhMethodHandle<MsAliveProcess> h_MsAliveProcess
        => new( new FhMethodLocation("FFX.exe", 0x389220) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsBlowProcess(int chr_id, Chr* chr);
    public static FhMethodHandle<MsBlowProcess> h_MsBlowProcess
        => new( new FhMethodLocation("FFX.exe", 0x389270) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsCalcCommand(AttackCue* param_1, int param_2);
    public static FhMethodHandle<MsCalcCommand> h_MsCalcCommand
        => new( new FhMethodLocation("FFX.exe", 0x3893A0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate uint FUN_0078bb30(int param_1, byte* param_2, byte* param_3, Command* param_4, uint param_5, uint* param_6, int* param_7);
    public static FhMethodHandle<FUN_0078bb30> h_FUN_0078bb30
        => new( new FhMethodLocation("FFX.exe", 0x38BB30) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsDamageCheckDeath(int attacker_id, int target_id, int param_3, int targeting_self);
    public static FhMethodHandle<MsDamageCheckDeath> h_MsDamageCheckDeath
        => new( new FhMethodLocation("FFX.exe", 0x38C800) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsDamageSetMotion(int chr_id, int param_2, int targeting_self);
    public static FhMethodHandle<MsDamageSetMotion> h_MsDamageSetMotion
        => new( new FhMethodLocation("FFX.exe", 0x38CAE0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate Command* MsGetCommand(int chr_id, int unused, int quit_on_idx, AttackCommandInfo* param_4, uint* param_5);
    public static FhMethodHandle<MsGetCommand> h_MsGetCommand
        => new( new FhMethodLocation("FFX.exe", 0x38CF10) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate uint FUN_0078d100(Chr* chr);
    public static FhMethodHandle<FUN_0078d100> h_FUN_0078d100
        => new( new FhMethodLocation("FFX.exe", 0x38D100) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsLimitStatusProcess(int chr_id, Chr* chr, uint param_3);
    public static FhMethodHandle<MsLimitStatusProcess> h_MsLimitStatusProcess
        => new( new FhMethodLocation("FFX.exe", 0x38D330) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsSetChrWeak(int chr_id, int new_weak_level);
    public static FhMethodHandle<MsSetChrWeak> h_MsSetChrWeak
        => new( new FhMethodLocation("FFX.exe", 0x38D8B0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate bool MsAutoRelifeProcess(int attacker_id, Chr* attacker, int target_id, Chr* target);
    public static FhMethodHandle<MsAutoRelifeProcess> h_MsAutoRelifeProcess
        => new( new FhMethodLocation("FFX.exe", 0x38D990) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsStoneProcess(int chr_id, Chr* chr);
    public static FhMethodHandle<MsStoneProcess> h_MsStoneProcess
        => new( new FhMethodLocation("FFX.exe", 0x38E210) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsSubCTB(int chr_id, Chr* chr, int param_3, int param_4, uint param_5, uint param_6);
    public static FhMethodHandle<MsSubCTB> h_MsSubCTB
        => new( new FhMethodLocation("FFX.exe", 0x38E2A0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsSubHP(int chr_id, Chr* chr, int param_3, int param_4, int param_5, uint param_6, uint param_7);
    public static FhMethodHandle<MsSubHP> h_MsSubHP
        => new( new FhMethodLocation("FFX.exe", 0x38E2F0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsSubMP(int chr_id, Chr* chr, int param_3, int param_4, int param_5, uint param_6, uint param_7);
    public static FhMethodHandle<MsSubMP> h_MsSubMP
        => new( new FhMethodLocation("FFX.exe", 0x38E400) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsThreatProcess(int chr_id, Chr* chr);
    public static FhMethodHandle<MsThreatProcess> h_MsThreatProcess
        => new( new FhMethodLocation("FFX.exe", 0x38E4B0) );

    // Unofficial naming
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate uint AfterDamageProcess(int attacker_id, uint param_2, int target_id, uint* param_4, uint param_5);
    public static FhMethodHandle<AfterDamageProcess> h_AfterDamageProcess
        => new( new FhMethodLocation("FFX.exe", 0x38F0B0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate byte* MsGetRomBtlText(int param_1, int param_2);
    public static FhMethodHandle<MsGetRomBtlText> h_MsGetRomBtlText
        => new( new FhMethodLocation("FFX.exe", 0x38F940) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsMenuCloseTitleWindow(int param_1);
    public static FhMethodHandle<MsMenuCloseTitleWindow> h_MsMenuCloseTitleWindow
        => new( new FhMethodLocation("FFX.exe", 0x38FA80) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsSaveItemUse(uint item_id, int amount);
    public static FhMethodHandle<MsSaveItemUse> h_MsSaveItemUse
        => new( new FhMethodLocation("FFX.exe", 0x3905A0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate byte* MsImportantName(uint key_item_idx);
    public static FhMethodHandle<MsImportantName> h_MsImportantName
        => new( new FhMethodLocation("FFX.exe", 0x3908B0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate AutoAbility* MsGetRomAbility(uint a_ability_id, int* ref_data_end);
    public static FhMethodHandle<MsGetRomAbility> h_MsGetRomAbility
        => new( new FhMethodLocation("FFX.exe", 0x3909C0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate CustomizationRecipe* MsGetRomKaizou(int *size);
    public static FhMethodHandle<MsGetRomKaizou> h_MsGetRomKaizou
        => new( new FhMethodLocation("FFX.exe", 0x390A60) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate AeonAbilityRecipe* MsGetRomSummonGrow(int* size);
    public static FhMethodHandle<MsGetRomSummonGrow> h_MsGetRomSummonGrow
        => new( new FhMethodLocation("FFX.exe", 0x390B00) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate bool MsMonsterCapture(int target_id, int arena_idx);
    public static FhMethodHandle<MsMonsterCapture> h_MsMonsterCapture
        => new( new FhMethodLocation("FFX.exe", 0x390B80) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_00791820();
    public static FhMethodHandle<FUN_00791820> h_FUN_00791820
        => new( new FhMethodLocation("FFX.exe", 0x391820) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate uint MsGetBattleEndStatus();
    public static FhMethodHandle<MsGetBattleEndStatus> h_MsGetBattleEndStatus
        => new( new FhMethodLocation("FFX.exe", 0x3928F0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate Chr* MsGetChr(int chr_id);
    public static FhMethodHandle<MsGetChr> h_MsGetChr
        => new( new FhMethodLocation("FFX.exe", 0x394030) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate Chr* MsGetMon(byte mon_idx);
    public static FhMethodHandle<MsGetMon> h_MsGetMon
        => new( new FhMethodLocation("FFX.exe", 0x395AB0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int brnd(int rng_idx);
    public static FhMethodHandle<brnd> h_brnd
        => new( new FhMethodLocation("FFX.exe", 0x398900) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate uint MsApUp(int chr_id, Chr* chr, int base_ap_add, uint param_4);
    public static FhMethodHandle<MsApUp> h_MsApUp
        => new( new FhMethodLocation("FFX.exe", 0x398A10) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate uint MsFieldItemGet(int treasure_id);
    public static FhMethodHandle<MsFieldItemGet> h_MsFieldItemGet
        => new( new FhMethodLocation("FFX.exe", 0x398FE0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsSetWeaponName(Equipment* gear);
    public static FhMethodHandle<MsSetWeaponName> h_MsSetWeaponName
        => new( new FhMethodLocation("FFX.exe", 0x3993C0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_007993f0(BtlRewardData* param_1, int param_2);
    public static FhMethodHandle<FUN_007993f0> h_FUN_007993f0
        => new( new FhMethodLocation("FFX.exe", 0x3993F0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsCheckRange(int param_1, int param_2, int param_3);
    public static FhMethodHandle<MsCheckRange> h_MsCheckRange
        => new( new FhMethodLocation("FFX.exe", 0x39A0D0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate Command* MsGetComData(int com_id, byte** param_2);
    public static FhMethodHandle<MsGetComData> h_MsGetComData
        => new( new FhMethodLocation("FFX.exe", 0x39A4C0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_0079b480(int chr_id, int com_id, int is_disabled);
    public static FhMethodHandle<FUN_0079b480> h_FUN_0079b480
        => new( new FhMethodLocation("FFX.exe", 0x39B480) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsSetRamChrAbility(int chr_id, Chr* chr);
    public static FhMethodHandle<MsSetRamChrAbility> h_MsSetRamChrAbility
        => new( new FhMethodLocation("FFX.exe", 0x39BB70) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsSetRamChrParam(uint chr_id);
    public static FhMethodHandle<MsSetRamChrParam> h_MsSetRamChrParam
        => new( new FhMethodLocation("FFX.exe", 0x39C610) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate byte MsMessageCueProcess();
    public static FhMethodHandle<MsMessageCueProcess> h_MsMessageCueProcess
        => new( new FhMethodLocation("FFX.exe", 0x39CE10) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsMessageCueRegist(uint type, int param_2, int param_3, byte param_4, byte param_5);
    public static FhMethodHandle<MsMessageCueRegist> h_MsMessageCueRegist
        => new( new FhMethodLocation("FFX.exe", 0x39CFF0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate BtlBinEncounter* MsBtlListEncount(int field_idx);
    public static FhMethodHandle<MsBtlListEncount> h_MsBtlListEncount
        => new( new FhMethodLocation("FFX.exe", 0x39D190) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate BtlBinField* MsBtlListField(int field_idx);
    public static FhMethodHandle<MsBtlListField> h_MsBtlListField
        => new( new FhMethodLocation("FFX.exe", 0x39D1B0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsBtlListFieldNum(int field_id);
    public static FhMethodHandle<MsBtlListFieldNum> h_MsBtlListFieldNum
        => new( new FhMethodLocation("FFX.exe", 0x39D1E0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate BtlBinGroup* MsBtlListGroup(int field_idx, int group_idx);
    public static FhMethodHandle<MsBtlListGroup> h_MsBtlListGroup
        => new( new FhMethodLocation("FFX.exe", 0x39D230) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsSetStealEffect(int param_1, int param_2);
    public static FhMethodHandle<MsSetStealEffect> h_MsSetStealEffect
        => new( new FhMethodLocation("FFX.exe", 0x39ED20) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsSetStealGillEffect(int param_1, int param_2);
    public static FhMethodHandle<MsSetStealGillEffect> h_MsSetStealGillEffect
        => new( new FhMethodLocation("FFX.exe", 0x39ED40) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsStatusDefenseEffect(int attacker_id, int target_id, int dmg_calc_flags);
    public static FhMethodHandle<MsStatusDefenseEffect> h_MsStatusDefenseEffect
        => new( new FhMethodLocation("FFX.exe", 0x39EE40) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsStatusEffectCheck(int chr_id);
    public static FhMethodHandle<MsStatusEffectCheck> h_MsStatusEffectCheck
        => new( new FhMethodLocation("FFX.exe", 0x39F010) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsNumberRegist(int param_1, int param_2, int param_3, int param_4, int param_5, uint param_6, uint param_7);
    public static FhMethodHandle<MsNumberRegist> h_MsNumberRegist
        => new( new FhMethodLocation("FFX.exe", 0x39FA20) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate uint MsRegSEplay(byte p1, int p2);
    public static FhMethodHandle<MsRegSEplay> h_MsRegSEplay
        => new( new FhMethodLocation("FFX.exe", 0x3A0120) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate uint MsRegSEplay2(int param_1, uint param_2);
    public static FhMethodHandle<MsRegSEplay2> h_MsRegSEplay2
        => new( new FhMethodLocation("FFX.exe", 0x3A0160) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_007a0c70(ushort name_id, byte owner, int unknown, ushort* model_id_pointer);
    public static FhMethodHandle<FUN_007a0c70> h_FUN_007a0c70
        => new( new FhMethodLocation("FFX.exe", 0x3A0C70) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate ushort FUN_007a0d10(Equipment* param_1);
    public static FhMethodHandle<FUN_007a0d10> h_FUN_007a0d10
        => new( new FhMethodLocation("FFX.exe", 0x3A0D10) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate byte* FUN_007ab890(int param_1, short* param_2, int param_3);
    public static FhMethodHandle<FUN_007ab890> h_FUN_007ab890
        => new( new FhMethodLocation("FFX.exe", 0x3AB890) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate nint MsGetExcelData(int req_elem_idx, nint excel_data_ptr, int* ref_data_end);
    public static FhMethodHandle<MsGetExcelData> h_MsGetExcelData
        => new( new FhMethodLocation("FFX.exe", 0x3AB890) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int FUN_007ab930(Equipment* param_1);
    public static FhMethodHandle<FUN_007ab930> h_FUN_007ab930
        => new( new FhMethodLocation("FFX.exe", 0x3AB930) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate Equipment* MsGetSaveWeapon(uint gear_inv_idx, nint ref_name);
    public static FhMethodHandle<MsGetSaveWeapon> h_MsGetSaveWeapon
        => new( new FhMethodLocation("FFX.exe", 0x3ABBF0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsBtlGetPos(int param_1, Chr* chr, int btl_pos_a, int btl_pos_b, int btl_pos_c, Vector4* out_pos);
    public static FhMethodHandle<MsBtlGetPos> h_MsBtlGetPos
        => new( new FhMethodLocation("FFX.exe", 0x3AC000) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsPopBtlPos(Chr* chr);
    public static FhMethodHandle<MsPopBtlPos> h_MsPopBtlPos
        => new( new FhMethodLocation("FFX.exe", 0x3AC620) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsActionRequest(int target_id, int attacker_id, int param_3, int param_4, int param_5, void* param_6);
    public static FhMethodHandle<MsActionRequest> h_MsActionRequest
        => new( new FhMethodLocation("FFX.exe", 0x3ACEC0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsLimitTidusLearn(int chr_id);
    public static FhMethodHandle<MsLimitTidusLearn> h_MsLimitTidusLearn
        => new( new FhMethodLocation("FFX.exe", 0x3B0CE0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsLimitTypeDamageCheck(int attacker_id, Chr* attacker, int target_id, Chr* target, int param_5, int param_6, int param_7);
    public static FhMethodHandle<MsLimitTypeDamageCheck> h_MsLimitTypeDamageCheck
        => new( new FhMethodLocation("FFX.exe", 0x3B0D60) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsLimitTypeStatusCheck(int attacker_id, Chr* attacker, int target_id, Chr* target, int param_5, uint param_6);
    public static FhMethodHandle<MsLimitTypeStatusCheck> h_MsLimitTypeStatusCheck
        => new( new FhMethodLocation("FFX.exe", 0x3B12D0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsAutoCureProcess(int target_id, Chr* target, int attacker_id, int poison, int zombie, int darkness, int silence);
    public static FhMethodHandle<MsAutoCureProcess> h_MsAutoCureProcess
        => new( new FhMethodLocation("FFX.exe", 0x3B2520) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int MsAutoPotionProcess(int target_id, Chr* target, int attacker_id);
    public static FhMethodHandle<MsAutoPotionProcess> h_MsAutoPotionProcess
        => new( new FhMethodLocation("FFX.exe", 0x3B2860) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void achievementUnlockAchievement(int ach_id);
    public static FhMethodHandle<achievementUnlockAchievement> h_achievementUnlockAchievement
        => new( new FhMethodLocation("FFX.exe", 0x422410) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void Sg_FadeInW(int p1);
    public static FhMethodHandle<Sg_FadeInW> h_Sg_FadeInW
        => new( new FhMethodLocation("FFX.exe", 0x42CC20) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int UpgradeBrotherhood(int level);
    public static FhMethodHandle<UpgradeBrotherhood> h_UpgradeBrotherhood
        => new( new FhMethodLocation("FFX.exe", 0x4596A0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_008671d0(byte opcode, AtelWorkThread* thread, AtelBasicWorker* work, AtelStack* stack);
    public static FhMethodHandle<FUN_008671d0> h_FUN_008671d0
        => new( new FhMethodLocation("FFX.exe", 0x4671D0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int FUN_00867370(byte opcode, AtelBasicWorker* work, AtelWorkThread* thread, AtelStack* stack, uint param_5);
    public static FhMethodHandle<FUN_00867370> h_FUN_00867370
        => new( new FhMethodLocation("FFX.exe", 0x467370) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate nint AtelGetCurCtrlWork();
    public static FhMethodHandle<AtelGetCurCtrlWork> h_AtelGetCurCtrlWork
        => new( new FhMethodLocation("FFX.exe", 0x46AF80) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate short FUN_0086bea0(int param_1);
    public static FhMethodHandle<FUN_0086bea0> h_FUN_0086bea0
        => new( new FhMethodLocation("FFX.exe", 0x46BEA0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate byte* FUN_0086bec0(int param_1);
    public static FhMethodHandle<FUN_0086bec0> h_FUN_0086bec0
        => new( new FhMethodLocation("FFX.exe", 0x46BEC0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void AtelInitTotal();
    public static FhMethodHandle<AtelInitTotal> h_AtelInitTotal
        => new( new FhMethodLocation("FFX.exe", 0x46D660) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int AtelPopStackInteger(int* param_1, AtelStack* atelStack);
    public static FhMethodHandle<AtelPopStackInteger> h_AtelPopStackInteger
        => new( new FhMethodLocation("FFX.exe", 0x46DE90) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void AtelSetEventJump2(int room, int entrance, int do_fade);
    public static FhMethodHandle<AtelSetEventJump2> h_AtelSetEventJump2
        => new( new FhMethodLocation("FFX.exe", 0x46FED0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void AtelEventSetUp(int event_id);
    public static FhMethodHandle<AtelEventSetUp> h_AtelEventSetUp
        => new( new FhMethodLocation("FFX.exe", 0x472E90) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MsSetSaveCommandWithPrefix(int chr_id, int com_id, int param_3);
    public static FhMethodHandle<MsSetSaveCommandWithPrefix> h_MsSetSaveCommandWithPrefix
        => new( new FhMethodLocation("FFX.exe", 0x474190) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void AtelSetUpCallFunc(int nameSpaceId, nint nameSpacePtr);
    public static FhMethodHandle<AtelSetUpCallFunc> h_AtelSetUpCallFunc
        => new( new FhMethodLocation("FFX.exe", 0x477800) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate char* AtelGetEventName(uint event_id);
    public static FhMethodHandle<AtelGetEventName> h_AtelGetEventName
        => new( new FhMethodLocation("FFX.exe", 0x4796E0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void SndSepPlaySimple(uint param_1);
    public static FhMethodHandle<SndSepPlaySimple> h_SndSepPlaySimple
        => new( new FhMethodLocation("FFX.exe", 0x486DE0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void TkMsImportantSet(uint param_1);
    public static FhMethodHandle<TkMsImportantSet> h_TkMsImportantSet
        => new( new FhMethodLocation("FFX.exe", 0x48E700) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void TkVU1SyncPath();
    public static FhMethodHandle<TkVU1SyncPath> h_TkVU1SyncPath
        => new( new FhMethodLocation("FFX.exe", 0x48EBD0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void TOBtlCloseSimpleHelpMes();
    public static FhMethodHandle<TOBtlCloseSimpleHelpMes> h_TOBtlCloseSimpleHelpMes
        => new( new FhMethodLocation("FFX.exe", 0x490E60) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int TOBtlDrawCaptureMonsterMessageWindow(int mon_id, int text_id);
    public static FhMethodHandle<TOBtlDrawCaptureMonsterMessageWindow> h_TOBtlDrawCaptureMonsterMessageWindow
        => new( new FhMethodLocation("FFX.exe", 0x4927E0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int TOBtlDrawFirstStrikeEnemyMessageWindow();
    public static FhMethodHandle<TOBtlDrawFirstStrikeEnemyMessageWindow> h_TOBtlDrawFirstStrikeEnemyMessageWindow
        => new( new FhMethodLocation("FFX.exe", 0x493440) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int TOBtlDrawFirstStrikePlayerMessageWindow();
    public static FhMethodHandle<TOBtlDrawFirstStrikePlayerMessageWindow> h_TOBtlDrawFirstStrikePlayerMessageWindow
        => new( new FhMethodLocation("FFX.exe", 0x493460) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int TOBtlDrawGetItemMessageWindow(byte* item_name, int amount);
    public static FhMethodHandle<TOBtlDrawGetItemMessageWindow> h_TOBtlDrawGetItemMessageWindow
        => new( new FhMethodLocation("FFX.exe", 0x493480) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int TOBtlDrawGetLimitTypeMessageWindow(int ply_id, int limit_mode);
    public static FhMethodHandle<TOBtlDrawGetLimitTypeMessageWindow> h_TOBtlDrawGetLimitTypeMessageWindow
        => new( new FhMethodLocation("FFX.exe", 0x493560) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int TOBtlDrawGetMoneyMessageWindow(int amount);
    public static FhMethodHandle<TOBtlDrawGetMoneyMessageWindow> h_TOBtlDrawGetMoneyMessageWindow
        => new( new FhMethodLocation("FFX.exe", 0x4935D0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int TOBtlDrawLearningMessageWindow(int ply_id, int com_id);
    public static FhMethodHandle<TOBtlDrawLearningMessageWindow> h_TOBtlDrawLearningMessageWindow
        => new( new FhMethodLocation("FFX.exe", 0x495290) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int TOBtlDrawStdChrNameMessageWindow(int chr_id, int text_id);
    public static FhMethodHandle<TOBtlDrawStdChrNameMessageWindow> h_TOBtlDrawStdChrNameMessageWindow
        => new( new FhMethodLocation("FFX.exe", 0x497170) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int FUN_0089db10(int p1, byte* text);
    public static FhMethodHandle<FUN_0089db10> h_FUN_0089db10
        => new( new FhMethodLocation("FFX.exe", 0x49DB10) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate byte TkMenuGetCurrentSummon();
    public static FhMethodHandle<TkMenuGetCurrentSummon> h_TkMenuGetCurrentSummon
        => new( new FhMethodLocation("FFX.exe", 0x4A9830) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate TkWindow* TkMenuMainAllocWindow();
    public static FhMethodHandle<TkMenuMainAllocWindow> h_TkMenuMainAllocWindow
        => new( new FhMethodLocation("FFX.exe", 0x4AA150) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate TkWindow* TkMenuMainRegistWindow(TkWindow* window);
    public static FhMethodHandle<TkMenuMainRegistWindow> h_TkMenuMainRegistWindow
        => new( new FhMethodLocation("FFX.exe", 0x4AAAB0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void TkMsGetRomItem(uint param_1, int* param_2);
    public static FhMethodHandle<TkMsGetRomItem> h_TkMsGetRomItem
        => new( new FhMethodLocation("FFX.exe", 0x4AB230) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate byte* TOGetSaveChrName(int chr_id);
    public static FhMethodHandle<TOGetSaveChrName> h_TOGetSaveChrName
        => new( new FhMethodLocation("FFX.exe", 0x4AC800) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_008b4460(TkWindow* window);
    public static FhMethodHandle<FUN_008b4460> h_FUN_008b4460
        => new( new FhMethodLocation("FFX.exe", 0x4B4460) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void TOBtlSetMacroCommandType(int param_1, int param_2, byte param_3);
    public static FhMethodHandle<TOBtlSetMacroCommandType> h_TOBtlSetMacroCommandType
        => new( new FhMethodLocation("FFX.exe", 0x4B5770) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void TOBtlSetMacroCommandValue(int param_1, int param_2, byte* param_3);
    public static FhMethodHandle<TOBtlSetMacroCommandValue> h_TOBtlSetMacroCommandValue
        => new( new FhMethodLocation("FFX.exe", 0x4B57A0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_008b8910(int window_idx, int variable_idx, int type); // setMessageWindowVariableType (0: text*, 1: int)
    public static FhMethodHandle<FUN_008b8910> h_FUN_008b8910
        => new( new FhMethodLocation("FFX.exe", 0x4B8910) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate byte* FUN_008bda20(uint window_idx); // getMenuText
    public static FhMethodHandle<FUN_008bda20> h_FUN_008bda20
        => new( new FhMethodLocation("FFX.exe", 0x4BDA20) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_008b8930(int window_idx, int variable_idx, int value); // setMessageWindowVariable
    public static FhMethodHandle<FUN_008b8930> h_FUN_008b8930
        => new( new FhMethodLocation("FFX.exe", 0x4B8930) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_0086a0c0();
    public static FhMethodHandle<FUN_0086a0c0> h_FUN_0086a0c0
        => new( new FhMethodLocation("FFX.exe", 0x46A0C0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate TOMesWinWork* AtelGetMesWinWork(int idx);
    public static FhMethodHandle<AtelGetMesWinWork> h_AtelGetMesWinWork
        => new( new FhMethodLocation("FFX.exe", 0x46BE20) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate byte* FUN_008bee80(uint param_1);
    public static FhMethodHandle<FUN_008bee80> h_FUN_008bee80
        => new( new FhMethodLocation("FFX.exe", 0x4BEE80) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void TkMn2DrawCrossCursor(float x, float y, int param_3);
    public static FhMethodHandle<TkMn2DrawCrossCursor> h_TkMn2DrawCrossCursor
        => new( new FhMethodLocation("FFX.exe", 0x4C0640) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void TkMn2DrawKickSyncPacket();
    public static FhMethodHandle<TkMn2DrawKickSyncPacket> h_TkMn2DrawKickSyncPacket
        => new( new FhMethodLocation("FFX.exe", 0x4C0C90) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_008c0f40(int param_1, int param_2, int param_3, int param_4);
    public static FhMethodHandle<FUN_008c0f40> h_FUN_008c0f40
        => new( new FhMethodLocation("FFX.exe", 0x4C0F40) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_008c1350_DrawScissor512x416();
    public static FhMethodHandle<FUN_008c1350_DrawScissor512x416> h_FUN_008c1350_DrawScissor512x416
        => new( new FhMethodLocation("FFX.exe", 0x4C1350) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int TkMn2GetSummonGrowMax();
    public static FhMethodHandle<TkMn2GetSummonGrowMax> h_TkMn2GetSummonGrowMax
        => new( new FhMethodLocation("FFX.exe", 0x4C1C20) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_008c1c70(int param_1, int param_2, uint param_3, int param_4);
    public static FhMethodHandle<FUN_008c1c70> h_FUN_008c1c70
        => new( new FhMethodLocation("FFX.exe", 0x4C1C70) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_008c2370(MenuList menu_list_id, Equipment* gear);
    public static FhMethodHandle<FUN_008c2370> h_FUN_008c2370 // PrepareMenuList
        => new( new FhMethodLocation("FFX.exe", 0x4C2370) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_008c2c40(int param_1, int param_2, byte* param_3);
    public static FhMethodHandle<FUN_008c2c40> h_FUN_008c2c40
        => new( new FhMethodLocation("FFX.exe", 0x4C2C40) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int TkSetLegendAbility(int chr_id, int level);
    public static FhMethodHandle<TkSetLegendAbility> h_TkSetLegendAbility
        => new( new FhMethodLocation("FFX.exe", 0x4C3150) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_008cc120(int param_1);
    public static FhMethodHandle<FUN_008cc120> h_FUN_008cc120
        => new( new FhMethodLocation("FFX.exe", 0x4CC120) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void TkMenuCtrl(TkMenu* menu, int arg);
    public static FhMethodHandle<TkMenuCtrl> h_TkMenuCtrlSummon
        => new( new FhMethodLocation("FFX.exe", 0x4CC300) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_008cd960(TkWindow* window, int param_2, int param_3, float param_4, float param_5);
    public static FhMethodHandle<FUN_008cd960> h_FUN_008cd960
        => new( new FhMethodLocation("FFX.exe", 0x4CD960) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_008cd9f0(TkWindow* window, int param_2, int param_3);
    public static FhMethodHandle<FUN_008cd9f0> h_FUN_008cd9f0
        => new( new FhMethodLocation("FFX.exe", 0x4CD9F0) );

    // DrawAeonCustomizationMenu
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_008cdb70(TkWindow* window);
    public static FhMethodHandle<FUN_008cdb70> h_FUN_008cdb70
        => new( new FhMethodLocation("FFX.exe", 0x4CDB70) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_008d4140(uint param_1, int param_2);
    public static FhMethodHandle<FUN_008d4140> h_FUN_008d4140
        => new( new FhMethodLocation("FFX.exe", 0x4D4140) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate uint FUN_008d48e0();
    public static FhMethodHandle<FUN_008d48e0> h_FUN_008d48e0
        => new( new FhMethodLocation("FFX.exe", 0x4D48E0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate bool FUN_008d5720(uint gear_id, int param_2);
    public static FhMethodHandle<FUN_008d5720> h_FUN_008d5720
        => new( new FhMethodLocation("FFX.exe", 0x4D5720) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void UpdateGearCustomizationMenuState(TkWindow* window);
    public static FhMethodHandle<UpdateGearCustomizationMenuState> h_UpdateGearCustomizationMenuState
        => new( new FhMethodLocation("FFX.exe", 0x4D5800) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_008d5d20(TkWindow* window, int param_2, int param_3, int param_4, int param_5);
    public static FhMethodHandle<FUN_008d5d20> h_FUN_008d5d20
        => new( new FhMethodLocation("FFX.exe", 0x4D5D20) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_008d5dc0(TkWindow* window, int param_2, int param_3);
    public static FhMethodHandle<FUN_008d5dc0> h_FUN_008d5dc0
        => new( new FhMethodLocation("FFX.exe", 0x4D5DC0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void DrawGearCustomizationMenu(TkWindow* window);
    public static FhMethodHandle<DrawGearCustomizationMenu> h_DrawGearCustomizationMenu
        => new( new FhMethodLocation("FFX.exe", 0x4D5F30) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_008d6630(int param_1, int param_2, int param_3);
    public static FhMethodHandle<FUN_008d6630> h_FUN_008d6630
        => new( new FhMethodLocation("FFX.exe", 0x4D6630) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void TkMenuAppearMainCmdWindow(int param_1, int param_2);
    public static FhMethodHandle<TkMenuAppearMainCmdWindow> h_TkMenuAppearMainCmdWindow
        => new( new FhMethodLocation("FFX.exe", 0x4E1C60) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_008e2de0();
    public static FhMethodHandle<FUN_008e2de0> h_FUN_008e2de0
        => new( new FhMethodLocation("FFX.exe", 0x4E2DE0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int FUN_008e33a0(byte* text, byte* param_2, byte* param_3);
    public static FhMethodHandle<FUN_008e33a0> h_FUN_008e33a0
        => new( new FhMethodLocation("FFX.exe", 0x4E33A0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void DrawCrossMenuScrollParts(float param_1, float param_2, float param_3, float param_4, int param_5, int param_6, int param_7);
    public static FhMethodHandle<DrawCrossMenuScrollParts> h_DrawCrossMenuScrollParts
        => new( new FhMethodLocation("FFX.exe", 0x4E6CC0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_008e71d0(int param_1);
    public static FhMethodHandle<FUN_008e71d0> h_FUN_008e71d0
        => new( new FhMethodLocation("FFX.exe", 0x4E71D0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void TODrawMenuPlateXYWHType(float x, float y, float w, float h, int type);
    public static FhMethodHandle<TODrawMenuPlateXYWHType> h_TODrawMenuPlateXYWHType
        => new( new FhMethodLocation("FFX.exe", 0x4F5F70) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_008f8bb0(int param_1, float param_2, float param_3, float param_4, float param_5);
    public static FhMethodHandle<FUN_008f8bb0> h_FUN_008f8bb0
        => new( new FhMethodLocation("FFX.exe", 0x4F8BB0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void TODrawScissorXYWH(int x, int y, int w, int h);
    public static FhMethodHandle<TODrawScissorXYWH> h_TODrawScissorXYWH
        => new( new FhMethodLocation("FFX.exe", 0x4F9230) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_008ff490(uint param_1, float param_2, float param_3);
    public static FhMethodHandle<FUN_008ff490> h_FUN_008ff490
        => new( new FhMethodLocation("FFX.exe", 0x4FF490) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void TOMkpCrossExtMesFontLClut(int param_1, byte* text, float x, float y, byte color, float scale, float p7_unused);
    public static FhMethodHandle<TOMkpCrossExtMesFontLClut> h_TOMkpCrossExtMesFontLClut
        => new( new FhMethodLocation("FFX.exe", 0x5016B0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void TOMkpCrossExtMesFontLClutTypeRGBA(uint p1, byte* text, float x, float y, byte color, byte p6, byte tint_r, byte tint_g, byte tint_b, byte tint_a, float scale, float _);
    public static FhMethodHandle<TOMkpCrossExtMesFontLClutTypeRGBA> h_TOMkpCrossExtMesFontLClutTypeRGBA
        => new( new FhMethodLocation("FFX.exe", 0x501700) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void TOMkpShapeXYWHUV(int param_1, float x, float y, float w, float h, float uv_x1, float uv_y1, float uv_x2, float uv_y2);
    public static FhMethodHandle<TOMkpShapeXYWHUV> h_TOMkpShapeXYWHUV
        => new( new FhMethodLocation("FFX.exe", 0x503BB0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void ToGetCrossExtMesFontWidth(int param_1, byte* param_2, float* param_3, float param_4, float param_5);
    public static FhMethodHandle<ToGetCrossExtMesFontWidth> h_ToGetCrossExtMesFontWidth
        => new( new FhMethodLocation("FFX.exe", 0x505320) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void ToMakeBtlEasyFont(byte* text, float x, float y, byte alpha, float scale);
    public static FhMethodHandle<ToMakeBtlEasyFont> h_ToMakeBtlEasyFont
        => new( new FhMethodLocation("FFX.exe", 0x505AB0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void abmap_get_panel(int ply_id, int node_idx);
    public static FhMethodHandle<abmap_get_panel> h_abmap_get_panel
        => new( new FhMethodLocation("FFX.exe", 0x6458A0) );

    // Sphere-grid state-machine entry points
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void abmap_ctrl();
    public static FhMethodHandle<abmap_ctrl> h_AbmapState_ChangingNode
        => new( new FhMethodLocation("FFX.exe", 0x647D50) );
    public static FhMethodHandle<abmap_ctrl> h_AbmapState_Warping
        => new( new FhMethodLocation("FFX.exe", 0x647F00) );
    public static FhMethodHandle<abmap_ctrl> h_AbmapState_MovingToTarget
        => new( new FhMethodLocation("FFX.exe", 0x659990) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_00a48910(uint chr_id, int node_idx);
    public static FhMethodHandle<FUN_00a48910> h_FUN_00a48910
        => new( new FhMethodLocation("FFX.exe", 0x648910) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void eiAbmParaGet();
    public static FhMethodHandle<eiAbmParaGet> h_eiAbmParaGet
        => new( new FhMethodLocation("FFX.exe", 0x654860) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FUN_00a56160(int p1, int p2, int p3);
    public static FhMethodHandle<FUN_00a56160> h_FUN_00a56160
        => new( new FhMethodLocation("FFX.exe", 0x656160) );

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public unsafe delegate nint FMOD_EventSystem_load(nint param_1, nint file_path, nint param_3, nint bank);
    public static FhMethodHandle<FMOD_EventSystem_load> h_FMOD_EventSystem_load
        => new( new FhMethodLocation("FFX.exe", 0x70C75C) );

}
