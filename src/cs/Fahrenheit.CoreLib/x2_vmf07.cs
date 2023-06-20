/* [fkelava 7/5/23 11:17]
 * Source: Steam ver.
 * Header: __ATEL_FUNCTION_LIST_7__
 * 
 * /ffx_ps2/ffx2/master/jppc/battle/header/btlatel.ath
 */

/* [fkelava 7/5/23 17:00]
 * The internal VM call table is split into 16 segments called 'funcspaces'.
 * 
 * F00 -> "CommFunc"
 * F01 -> "MathFunc"
 * F02 -> 
 * F03 -> 
 * F04 -> "SgEvent"
 * F05 -> "ChEvent"
 * F06 -> "Camera"
 * F07 -> "Battle"
 * F08 -> "MapFunc"
 * F09 -> "MnFunc"
 * F10 -> 
 * F11 -> "MovieFunc"
 * F12 -> "DebugFunc"
 * F13 -> "AbilityMap"
 * F14 ->
 * F15 ->
 * 
 * where the otherwise unmarked fspaces are filled with the same generic set of ATEL calls.
 * 
 * This is funcspace 7 (F07), battle functions. This header is not guaranteed to be complete.
 * 
 * Currently useless per se. Requires the function locations to be mapped, which is non-trivial.
 * Additionally, the calling convention must be specified for each function.
 */

namespace Fahrenheit.CoreLib;

public static unsafe partial class FhX2VMCall
{
    public delegate int   btlTerminateAction();
    public delegate int   btlSetRandPosFlag(int arg1);
    public delegate int   btlExe(int arg1, int arg2);
    public delegate int   btlDirTarget(int arg1, int arg2);
    public delegate int   btlSetDirRate(float arg1);
    public delegate int   btlGetWater();
    public delegate int   btlDirBasic(int arg1, int arg2);
    public delegate int   btlSetMotion(int arg1);
    public delegate int   btlWaitMotion();
    public delegate int   btlSetGravity(int arg1);
    public delegate int   btlSetHeight(int arg1, float arg2);
    public delegate int   btlSetDirectCommand(int arg1, int arg2);
    public delegate int   btlMove(int arg1, float arg2, int arg3, int arg4, int arg5, float arg6, float arg7, float arg8);
    public delegate int   btlDirPos(int arg1, int arg2);
    public delegate int   btlSetDamage(int arg1);
    public delegate int   btlGetStat(int arg1, int arg2);
    public delegate int   btlSearchChr(int arg1, int arg2, int arg3, int arg4, int arg5);
    public delegate int   btlCameraMode(int arg1);
    public delegate int   btlTerminateEffect();
    public delegate int   btlChrSp(float arg1);
    public delegate int   btlGetComNum();
    public delegate int   btlPrint(int arg1);
    public delegate int   btlTerminateMotion(int arg1);
    public delegate int   btlSetNormalEffect(int arg1, int arg2);
    public delegate int   btlSetStat(int arg1, int arg2, int arg3);
    public delegate int   btlGetReCom();
    public delegate int   btlGetComInfo(int arg1, int arg2);
    public delegate int   btlChangeReCom(int arg1, int arg2);
    public delegate int   btlSetMotionLevel(int arg1);
    public delegate int   btlGetMotionLevel();
    public delegate int   btlCountChr(int arg1, int arg2);
    public delegate int   btlChgWaitMotion(int arg1);
    public delegate int   btlCheckStartEffect();
    public delegate int   btlGetChrNum(int arg1);
    public delegate int   btlSetFirstAttack(int arg1);
    public delegate int   btlDistTarget(int arg1);
    public delegate int   btlGetBtlScene();
    public delegate int   btlSearchChr2(int arg1, int arg2, int arg3, int arg4);
    public delegate int   btlSetWeak(int arg1);
    public delegate int   btlGetWeak();
    public delegate int   btlSetScale(float arg1, float arg2, float arg3);
    public delegate int   btlSetFly(int arg1);
    public delegate int   btlCheckBtlPos();
    public delegate int   btlCheckMotion();
    public delegate int   btlSetHoming(int arg1, int arg2);
    public delegate int   btlResetMove();
    public delegate float btlMoveTargetDist(int arg1);
    public delegate int   btlOut(int arg1, int arg2);
    public delegate int   btlGetMoveFlag();
    public delegate int   btlStartMotion();
    public delegate int   btlResetBtlPosDir(int arg1, float arg2);
    public delegate int   btlSetEnMapID(int arg1);
    public delegate int   btlComplete(int arg1);
    public delegate int   btlGetCompInfo();
    public delegate int   btlSetTrans(float arg1, float arg2, int arg3);
    public delegate int   btlAddCom(int arg1, int arg2);
    public delegate int   btlDelCom(int arg1, int arg2);
    public delegate int   btlTerminateDeath();
    public delegate int   btlSetSpeed(int arg1);
    public delegate int   btlSetCommandUse(int arg1, int arg2, int arg3);
    public delegate int   btlOff();
    public delegate int   btlOn();
    public delegate int   btlWait();
    public delegate int   camReq(int arg1, int arg2);
    public delegate int   btlMagicStart(int arg1);
    public delegate int   btlMagicEnd();
    public delegate int   btlMes(int arg1, int arg2, int arg3, int arg4, int arg5);
    public delegate int   btlMesWait(int arg1);
    public delegate int   btlMesClose(int arg1);
    public delegate int   btlDistTargetFrame(int arg1, int arg2, float arg3);
    public delegate int   btlSplineStart(int arg1);
    public delegate int   btlSplineRegist(int arg1, int arg2);
    public delegate int   btlSplineRegistPos(int arg1, float arg2, float arg3, float arg4);
    public delegate int   btlSplineMove(int arg1, int arg2, float arg3, int arg4);
    public delegate int   btlCheckMove(int arg1);
    public delegate int   btlReqMotion(int arg1, int arg2, int arg3);
    public delegate int   btlWaitReqMotion(int arg1);
    public delegate int   btlSetDeathLevel(int arg1);
    public delegate int   btlSetDeathPattern(int arg1);
    public delegate int   btlSetEventChrFlag(int arg1, int arg2);
    public delegate int   btlResetParam(int arg1);
    public delegate int   btlWaitNormalEffect();
    public delegate int   btlChrLink(int arg1, int arg2, int arg3);
    public delegate int   btlMoveJump(int arg1, float arg2, int arg3, int arg4, int arg5, float arg6, float arg7, float arg8, float arg9);
    public delegate int   btlSetChrPosElem(int arg1, int arg2, int arg3);
    public delegate int   btlSetBodyHit(int arg1);
    public delegate int   btlSetSpecialBattle(int arg1);
    public delegate int   btlDirMove(int arg1, int arg2, float arg3, int arg4, int arg5);
    public delegate int   btlCheckMotionNum(int arg1, int arg2);
    public delegate float btlMoveTargetDist2D(int arg1);
    public delegate int   btlSetAbsCommand(int arg1, int arg2);
    public delegate float btlGetCamWidth(int arg1);
    public delegate float btlGetCamHeight(int arg1);
    public delegate int   btlSetBindEffect(int arg1, int arg2);
    public delegate int   btlResetBindEffect();
    public delegate int   btlPrintF(float arg1);
    public delegate int   btlSetStatEff();
    public delegate int   btlClearStatEff();
    public delegate int   btlSetHitEffect(int arg1, int arg2);
    public delegate int   btlWaitHitEffect();
    public delegate int   btlVoiceStandby(int arg1);
    public delegate int   btlVoiceStart();
    public delegate int   btlVoiceStop();
    public delegate int   btlGetVoiceStatus();
    public delegate int   btlVoiceSync();
    public delegate int   btlSearchChrCamera(int arg1, int arg2, int arg3, int arg4);
    public delegate int   btlCheckTargetOwn(int arg1);
    public delegate int   btlSetModelHide(int arg1, int arg2, int arg3);
    public delegate int   btlSoundEffectNormal(int arg1, int arg2);
    public delegate int   btlSoundStreamNormal(int arg1, int arg2);
    public delegate int   btlReqVoice(int arg1, int arg2);
    public delegate int   btlSetMotionAction(int arg1);
    public delegate int   btlStatusOn();
    public delegate int   btlStatusOff();
    public delegate int   btlmes2(int arg1, int arg2);
    public delegate int   btlAttachWeapon(int arg1);
    public delegate int   btlDetachWeapon(int arg1);
    public delegate int   btlReqWeaponMotion(int arg1, int arg2, int arg3, int arg4);
    public delegate int   btlSetAttackStartEffect();
    public delegate int   btlSoundEffectStartAttack(int arg1, int arg2);
    public delegate int   btlGetComInfo2(int arg1, int arg2, int arg3);
    public delegate int   btlResetWeapon();
    public delegate int   btlGetCalcResult(int arg1);
    public delegate int   btlSoundEffect(int arg1, int arg2);
    public delegate int   btlWaitSound();
    public delegate int   btlSetDebug(int arg1, int arg2);
    public delegate int   btlGetDebug(int arg1);
    public delegate int   btlResetBtlPos(int arg1);
    public delegate int   btlSetEffectResidentMagic();
    public delegate int   btlWaitExe();
    public delegate int   btlSetFreeEffect(int arg1, int arg2);
    public delegate int   btlSetAfterImage(int arg1, int arg2);
    public delegate int   btlResetAfterImage();
    public delegate int   btlMoveAttack(int arg1, int arg2);
    public delegate int   btlUseChrMpLimit();
    public delegate int   btlSoundEffectFade(int arg1, int arg2, int arg3);
    public delegate int   btlRegSoundEffect(int arg1, int arg2);
    public delegate int   btlRegSoundEffectFade(int arg1, int arg2, int arg3);
    public delegate int   btlInitEncount(int arg1);
    public delegate int   btlGetEncount(int arg1);
    public delegate int   btlSetEncount(int arg1, int arg2);
    public delegate int   btlGetLastActionChr();
    public delegate int   btlCheckBtlPos2();
    public delegate int   btlDirPosBasic(int arg1);
    public delegate int   btlSetCriticalEffect(int arg1);
    public delegate int   btlChangeChrName(int arg1, int arg2);
    public delegate int   btlGetGroundDist(int arg1);
    public delegate int   btlCheckDirFlag();
    public delegate int   btlSetTransVisible(float arg1, float arg2, int arg3);
    public delegate int   btlGetMoveFrameRest();
    public delegate int   btlGetReflect();
    public delegate int   btlOff2();
    public delegate int   btlCheckDefenseMotion();
    public delegate int   btlSetCursorType(int arg1);
    public delegate int   btlCheckPoison();
    public delegate float btlGetChrPosY(int arg1);
    public delegate float btlGetTargetDir(int arg1, int arg2);
    public delegate int   btlWaitMotion_avoid();
    public delegate int   btlSetMotionSignal(int arg1, int arg2, int arg3);
    public delegate float btlGetChrTargetDir(int arg1);
    public delegate int   btlSetUpVectorFlag(int arg1);
    public delegate int   btlGetChrNum2(int arg1);
    public delegate int   btlMotionRead(int arg1);
    public delegate int   btlSetMotionAbs(int arg1);
    public delegate int   btlMotionDispose();
    public delegate int   btlSetMapCenter(float arg1, float arg2, float arg3);
    public delegate int   btlSetEscape(int arg1);
    public delegate float btlGetMotionData(int arg1, int arg2);
    public delegate int   btlSetMotionData(int arg1, int arg2, float arg3);
    public delegate int   btlMesWait_Voice(int arg1);
    public delegate int   btlGetStat2(int arg1);
    public delegate int   btlSetStat2(int arg1, int arg2);
    public delegate float btlGetMotionData2(int arg1);
    public delegate int   btlSetMotionSignalIgnore(int arg1);
    public delegate int   btlGetLastDeathChr();
    public delegate int   btlGetVoiceFlag();
    public delegate int   btlDistTargetFrame2(int arg1);
    public delegate int   btlPrintSp(int arg1);
    public delegate int   btlSetMotionData2(int arg1, float arg2);
    public delegate int   btlVoiceSet(int arg1);
    public delegate int   btlFadeOutWeapon();
    public delegate int   btlResetMotionSpeed();
    public delegate int   btlDistTargetFrameSpd(int arg1);
    public delegate int   btlMesA(int arg1, int arg2);
    public delegate int   btlSetSkipMode(int arg1);
    public delegate float btlGetCamWidth2(int arg1);
    public delegate float btlGetCamHeight2(int arg1);
    public delegate int   btlMoveLeave(int arg1);
    public delegate int   btlWaitNomEff(int arg1);
    public delegate int   btlWaitHitEff(int arg1);
    public delegate float btlGetChrDir(int arg1);
    public delegate int   btlSetBindScale(float arg1);
    public delegate float btlGetHeight(int arg1);
    public delegate int   btlDistTarget2(int arg1, int arg2);
    public delegate float btlGetTargetDirH(int arg1, int arg2);
    public delegate float btlGetChrTargetDir2(int arg1);
    public delegate int   btlGetDeathReaction();
    public delegate int   btlCheckRetBtlPos();
    public delegate int   btlGetCameraBuffer(int arg1);
    public delegate float btlGetCameraBufferFloat(int arg1);
    public delegate int   btlSoundEffect2(int arg1, int arg2);
    public delegate int   btlSoundEffect3(int arg1, int arg2);
    public delegate int   btlRegSoundEffect2(int arg1, int arg2);
    public delegate int   btlRegSoundEffect3(int arg1, int arg2);
    public delegate int   btlSetOwnTarget(int arg1);
    public delegate int   btlAddOwnTarget(int arg1);
    public delegate int   btlSubOwnTarget(int arg1);
    public delegate int   btlGetReverbe();
    public delegate int   btlSetCameraSelectMode(int arg1);
    public delegate int   btlGetNomEff(int arg1);
    public delegate int   btlGetHitEff(int arg1);
    public delegate int   btlSetNomEff(int arg1, int arg2, int arg3);
    public delegate int   btlSetHitEff(int arg1, int arg2, int arg3);
    public delegate int   btlSetMotionTagExe();
    public delegate int   btlGetAssumeDamage(int arg1, int arg2, int arg3);
    public delegate int   btlSetMotionDamage(int arg1);
    public delegate int   btlSetAnimaChainOff(int arg1);
    public delegate int   btlExeAnimaChainOff();
    public delegate int   btlGetFirstAttack();
    public delegate int   btlGetAnimaChainOff();
    public delegate int   btlChangeChrNameID(int arg1, int arg2);
    public delegate int   btlSetDebugCount(int arg1);
    public delegate int   btlGetSubTitle();
    public delegate int   btlCheckBtlScene(int arg1);
    public delegate int   btlGetReaction();
    public delegate int   btlGetNormalAttack();
    public delegate int   btlSetTexAnime(int arg1, int arg2);
    public delegate int   btlGetEffectMemory(int arg1);
    public delegate int   btlGetCalcResultLimit(int arg1, int arg2);
    public delegate int   btlSetNomEffReg(int arg1, int arg2, int arg3);
    public delegate int   btlSetHitEffReg(int arg1, int arg2, int arg3);
    public delegate int   btlSetRandomTarget(int arg1);
    public delegate int   btlGetGold();
    public delegate int   btlSetBattleFadeOutWin(int arg1);
    public delegate int   btlSetBattleFadeOutLose(int arg1);
    public delegate int   btlSetVoiceID(int arg1);
    public delegate int   btlGetItemNum(int arg1);
    public delegate int   btlGetItem(int arg1, int arg2);
    public delegate int   btlSetTrans2(float arg1, int arg2);
    public delegate int   btlSendEffSignal(int arg1, int arg2);
    public delegate float btlGetCameraCount();
    public delegate int   btlCommandClear();
    public delegate int   btlCommandSet(int arg1);
    public delegate int   btlSetBattleFadeOutEscape(int arg1);
    public delegate int   btlGetCommandTarget(int arg1, int arg2);
    public delegate int   btlCheckUseCommand(int arg1, int arg2);
    public delegate int   btlInitCommandBuffer();
    public delegate int   btlSetCommandBuffer(int arg1);
    public delegate int   btlGetCommandBuffer();
    public delegate int   btlSearchChr3(int arg1, int arg2, int arg3, int arg4, int arg5);
    public delegate int   btlSetMegasRandomCommand(int arg1);
    public delegate int   btlGetCommandTargetSearch(int arg1, int arg2, int arg3, int arg4, int arg5);
    public delegate int   btlStrongNewGame();
    public delegate int   btlSetUpLimit(int arg1, int arg2);
    public delegate int   btlSetUpLimit2(int arg1, int arg2, int arg3, int arg4);
    public delegate int   btlSetFriendMonster();
    public delegate int   btlCheckReqMotion(int arg1);
    public delegate int   btlGetFullCommand();
    public delegate int   btlFaseTarget(int arg1, int arg2);
    public delegate int   btlFaseTargetXYZ(int arg1, float arg2, float arg3, float arg4);
    public delegate int   btlSetCommandEffect(int arg1, int arg2, int arg3);
    public delegate int   btlWaitStone();
    public delegate int   btlCheckGetCommand(int arg1, int arg2);
    public delegate int   btlDirPosBasic2(int arg1);
    public delegate int   btlDirBasic2(int arg1, int arg2);
    public delegate int   btlSetAppear(int arg1, int arg2, float arg3);
    public delegate int   btlDirTarget2(int arg1, int arg2);
    public delegate int   get_btl_member(int arg1);
    public delegate int   btlTerminateStone();
    public delegate int   btlDefensePosOff();
    public delegate int   btlCheckMapRange();
    public delegate int   btlWaitMotionIdle();
    public delegate int   btlMouseOn(int arg1);
    public delegate int   btlMouseOff(int arg1);
    public delegate int   btlDirMove2(int arg1, float arg2, int arg3, int arg4);
    public delegate int   btlMonsterFarm();
    public delegate int   btlSphereMonitor();
    public delegate int   btlDirResetLeave();
    public delegate int   btlSetDefenseEffect();
    public delegate int   btlSetDeathCommand(int arg1, int arg2);
    public delegate int   btlSetSummonGameOver(int arg1);
    public delegate int   btlSetCounterFlag(int arg1);
    public delegate int   btlSetWind(int arg1, float arg2, float arg3, float arg4);
    public delegate int   btlSetCameraStandard(int arg1);
    public delegate int   btlSetGameOverEffNum(int arg1);
    public delegate int   btlSetShadowHeight(float arg1);
    public delegate int   btlGetDoubleMagic();
    public delegate int   btlSysMes(int arg1, int arg2, int arg3, int arg4, int arg5);
    public delegate int   btlMesSetSys(int arg1);
    public delegate int   btlSysMesA(int arg1, int arg2);
    public delegate int   btlMouseOn2(int arg1);
    public delegate int   btlResetCommand();
    public delegate int   btlMesSet(int arg1);
    public delegate int   btlSetBtlPosEscape();
    public delegate int   btlSetNonDisposeChr(int arg1);
    public delegate int   btlMotionSetLoop(int arg1);
    public delegate int   btlHitFrameCheck();
    public delegate int   btlDirCheck(int arg1);
    public delegate int   btlDirMoveAttack(int arg1, int arg2);
    public delegate int   btlDirMoveReturn(int arg1, int arg2, int arg3, int arg4);
    public delegate int   btlGetMotionFrame(int arg1);
    public delegate int   btlHomingCheck();
    public delegate int   btlChangeBtlPos();
    public delegate int   btlDirMoveStep(int arg1, int arg2, int arg3, int arg4);
    public delegate int   btlChangeBtlPosForward();
    public delegate int   btlMoveStep(int arg1, int arg2, int arg3, int arg4);
    public delegate int   btlResetPos();
    public delegate int   btlGetBattleMusic(int arg1);
    public delegate int   btlCheckActionDamage();
    public delegate int   btlChangeMotionLevel(int arg1);
    public delegate int   btlAutoCameraOffOld();
    public delegate int   btlAutoCameraOnOld();
    public delegate int   btlAutoCameraInitOld();
    public delegate int   btlAutoCameraNextOld();
    public delegate float btlAddChrDir(int arg1);
    public delegate int   btlDirCheck2(int arg1, int arg2);
    public delegate int   btlChangeBtlPos2(float arg1);
    public delegate int   btlDirMoveStep2(int arg1, int arg2, int arg3, int arg4);
    public delegate float btlGetScale();
    public delegate int   btlEvtReq(int arg1);
    public delegate int   btlGetOverSoul(int arg1);
    public delegate int   btlGetOverSoulDefeat(int arg1);
    public delegate int   btlGetOverSoulCount(int arg1);
    public delegate int   btlGetOverSoulCountMax(int arg1);
    public delegate int   btlMotionSetWait(int arg1);
    public delegate int   btlSetEncountDistMul(float arg1);
    public delegate float btlGetEncountDistMul();
    public delegate int   btlGetMonsterDefeat(int arg1);
    public delegate int   btlGetAvoid();
    public delegate int   btlCheckMapOut();
    public delegate int   btlMotionTransSet(int arg1, int arg2);
    public delegate int   btlMotionIdle(int arg1);
    public delegate int   btlWaitMotionID(int arg1);
    public delegate int   btlGetMonsterMeet(int arg1);
    public delegate int   btlWaitMotionAbs();
    public delegate int   btlSetLinkCommand(int arg1, int arg2);
    public delegate int   btlSetRapidWindow();
    public delegate int   btlGetRapidShotSave();
    public delegate int   btlGetBtlMap();
    public delegate int   btlWaitEffect();
    public delegate int   btlAbilityEncountIgnore(int arg1);
    public delegate int   btlMesReset();
    public delegate int   add_mon(int arg1);
    public delegate int   remove_mon(int arg1);
    public delegate int   is_mon_party_join(int arg1);
    public delegate int   get_mon_party(int arg1);
    public delegate int   is_mon_party_member(int arg1);
    public delegate int   get_mon_party_member(int arg1);
    public delegate int   get_mon_level(int arg1);
    public delegate int   btlCountChrPlayer(int arg1, int arg2);
    public delegate int   exchange_mon(int arg1, int arg2);
    public delegate float get_mon_scale(int arg1);
    public delegate int   btlChrLink2(int arg1, int arg2, int arg3);
    public delegate int   DgnProgLoad();
    public delegate int   DgnProgDestroy();
    public delegate int   reset_party_member();
    public delegate int   btlCheckFriend();
}
