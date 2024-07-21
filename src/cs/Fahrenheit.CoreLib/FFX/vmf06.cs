/* [fkelava 7/5/23 11:17]
 * Source: MS Store ver.
 * Header: __ATEL_FUNCTION_LIST_6__
 * 
 * /ffx_ps2/ffx/proj2/chr/ath/battle/atelcam.ath
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
 * This is funcspace 6 (F06), camera functions. This header is not guaranteed to be complete.
 * 
 * Currently useless per se. Requires the function locations to be mapped, which is non-trivial.
 * Additionally, the calling convention must be specified for each function.
 */

namespace Fahrenheit.CoreLib.FFX;

public static unsafe partial class VMCall {
    public delegate int   camSleep(int arg1);
    public delegate int   camWakeUp(int arg1);
    public delegate int   camSetPos(float arg1, float arg2, float arg3);
    public delegate int   camGetPos(float* arg1, float* arg2, float* arg3);
    public delegate int   camSetPolar(float arg1, float arg2, float arg3);
    public delegate int   camSetPolarOffset(float arg1, float arg2, float arg3);
    public delegate int   camSetHypot(float arg1, float arg2, float arg3, float arg4, float arg5, float arg6);
    public delegate int   camSetHypot2(float arg1, float arg2, float arg3, float arg4, float arg5, float arg6);
    public delegate int   camSetHypot3(float arg1, float arg2, float arg3, float arg4, float arg5, float arg6);
    public delegate int   camSetAct(int arg1, int arg2, float arg3);
    public delegate int   camSetFilter(float arg1, float arg2, float arg3, float arg4, float arg5);
    public delegate int   camSetFilter2(float arg1, float arg2, float arg3, float arg4, float arg5);
    public delegate int   camSetFilterY(float arg1, float arg2, float arg3);
    public delegate int   camSetFilterY2(float arg1, float arg2, float arg3);
    public delegate int   camSleepFilter(int arg1);
    public delegate int   camResetFilter();
    public delegate int   camMove(int arg1);
    public delegate int   camMovePolar(int arg1);
    public delegate int   camMoveCos(int arg1);
    public delegate int   camMovePolarCos(int arg1);
    public delegate int   camMoveAcc(int arg1, int arg2, int arg3, int arg4);
    public delegate int   camMovePolarAcc(int arg1, int arg2, int arg3, int arg4);
    public delegate int   camResetMove();
    public delegate int   camSetInertia(float arg1, float arg2, float arg3, float arg4);
    public delegate int   camSetDirVector(float arg1, float arg2, float arg3);
    public delegate int   camResetDirVector();
    public delegate int   camWait();
    public delegate int   camCheck();
    public delegate int   camSetDataPoint(int* arg1, float arg2);
    public delegate int   camSetDataPointHypot(int* arg1, float arg2, float arg3, float arg4);
    public delegate int   camSetDataPoint2(int* arg1, float arg2);
    public delegate int   camSetDataPointHypot2(int* arg1, float arg2, float arg3, float arg4);
    public delegate int   refSetPos(float arg1, float arg2, float arg3);
    public delegate int   refGetPos(float* arg1, float* arg2, float* arg3);
    public delegate int   refSetPolar(float arg1, float arg2, float arg3);
    public delegate int   refSetPolarOffset(float arg1, float arg2, float arg3);
    public delegate int   refSetHypot(float arg1, float arg2, float arg3, float arg4, float arg5, float arg6);
    public delegate int   refSetHypot2(float arg1, float arg2, float arg3, float arg4, float arg5, float arg6);
    public delegate int   refSetHypot3(float arg1, float arg2, float arg3, float arg4, float arg5, float arg6);
    public delegate int   refSetAct(int arg1, int arg2, float arg3);
    public delegate int   refSetFilter(float arg1, float arg2, float arg3, float arg4, float arg5);
    public delegate int   refSetFilter2(float arg1, float arg2, float arg3, float arg4, float arg5);
    public delegate int   refSetFilterY(float arg1, float arg2, float arg3);
    public delegate int   refSetFilterY2(float arg1, float arg2, float arg3);
    public delegate int   refSleepFilter(int arg1);
    public delegate int   refResetFilter();
    public delegate int   refMove(int arg1);
    public delegate int   refMovePolar(int arg1);
    public delegate int   refMoveCos(int arg1);
    public delegate int   refMovePolarCos(int arg1);
    public delegate int   refMoveAcc(int arg1, int arg2, int arg3, int arg4);
    public delegate int   refMovePolarAcc(int arg1, int arg2, int arg3, int arg4);
    public delegate int   refResetMove();
    public delegate int   refSetInertia(float arg1, float arg2, float arg3, float arg4);
    public delegate int   refSetDirVector(float arg1, float arg2, float arg3);
    public delegate int   refResetDirVector();
    public delegate int   refWait();
    public delegate int   refCheck();
    public delegate int   camSetRoll(float arg1);
    public delegate int   camSetScrDpt(float arg1);
    public delegate int   camSetAct2(float arg1, float arg2, float arg3, float arg4);
    public delegate int   refSetAct2(float arg1, float arg2, float arg3, float arg4);
    public delegate int   camSetBtl(int arg1, int arg2, float arg3);
    public delegate int   refSetBtl(int arg1, int arg2, float arg3);
    public delegate int   camSetBtlPolar(int arg1, int arg2, float arg3, float arg4, float arg5, float arg6);
    public delegate int   refSetBtlPolar(int arg1, int arg2, float arg3, float arg4, float arg5, float arg6);
    public delegate int   refMoveStat(int arg1);
    public delegate int   camMoveStat(int arg1);
    public delegate int   camSetBtlPolar2(int arg1, int arg2, float arg3, float arg4, float arg5, float arg6);
    public delegate int   refSetBtlPolar2(int arg1, int arg2, float arg3, float arg4, float arg5, float arg6);
    public delegate int   camSetSpline(int arg1, float arg2, float arg3, int arg4);
    public delegate int   refSetSpline(int arg1, float arg2, float arg3, int arg4);
    public delegate int   camStartSpline();
    public delegate int   camRegSpline();
    public delegate int   refStartSpline();
    public delegate int   refRegSpline();
    public delegate int   camSetChrPolar(int arg1, int arg2, float arg3, float arg4, float arg5, float arg6);
    public delegate int   camSetChrPolar2(int arg1, int arg2, float arg3, float arg4, float arg5, float arg6);
    public delegate int   camScrSet(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6);
    public delegate int   camScrOff(int arg1);
    public delegate int   camDrawSet(int arg1, int arg2, int arg3, int arg4, int arg5);
    public delegate int   camDrawLink(int arg1, int arg2);
    public delegate int   camScrLink(int arg1, int arg2);
    public delegate int   camScrMove(int arg1, int arg2);
    public delegate int   camScrMoveCos(int arg1, int arg2);
    public delegate int   camScrMoveAcc(int arg1, int arg2, int arg3, int arg4);
    public delegate int   camDrawMove(int arg1, int arg2);
    public delegate int   camDrawMoveCos(int arg1, int arg2);
    public delegate int   camDrawMoveAcc(int arg1, int arg2, int arg3, int arg4);
    public delegate int   refSetSplineFilter(int* arg1, int arg2);
    public delegate int   refSetSplineFilter2(int* arg1, float arg2, int arg3);
    public delegate int   camSetSpline2(int* arg1, int arg2);
    public delegate int   refSetShake(int arg1, float arg2, int arg3, int arg4, int arg5);
    public delegate int   camSetShake(int arg1, float arg2, int arg3, int arg4, int arg5);
    public delegate int   camSetScreenShake(int arg1, int arg2, float arg3, int arg4, int arg5, int arg6);
    public delegate int   refResetShake();
    public delegate int   camResetShake();
    public delegate int   camResetScreenShake(int arg1);
    public delegate int   refWaitShake();
    public delegate int   camWaitShake();
    public delegate int   camWaitScreenShake(int arg1);
    public delegate int   camPriority(int arg1);
    public delegate int   refSetShakeB(int arg1, float arg2, int arg3, int arg4, int arg5);
    public delegate int   camSetShakeB(int arg1, float arg2, int arg3, int arg4, int arg5);
    public delegate int   camSetScreenShakeB(int arg1, int arg2, float arg3, int arg4, int arg5, int arg6);
    public delegate int   refSetShake2(int arg1, float arg2, int arg3, int arg4, int arg5);
    public delegate int   camSetShake2(int arg1, float arg2, int arg3, int arg4, int arg5);
    public delegate int   camSetScreenShake2(int arg1, int arg2, float arg3, int arg4, int arg5, int arg6);
    public delegate int   refSetShake2B(int arg1, float arg2, int arg3, int arg4, int arg5);
    public delegate int   camSetShake2B(int arg1, float arg2, int arg3, int arg4, int arg5);
    public delegate int   camSetScreenShake2B(int arg1, int arg2, float arg3, int arg4, int arg5, int arg6);
    public delegate int   refSetShake3(int arg1, float arg2, int arg3, int arg4, int arg5);
    public delegate int   camSetShake3(int arg1, float arg2, int arg3, int arg4, int arg5);
    public delegate int   camSetScreenShake3(int arg1, int arg2, float arg3, int arg4, int arg5, int arg6);
    public delegate int   refSetShake3B(int arg1, float arg2, int arg3, int arg4, int arg5);
    public delegate int   camSetShake3B(int arg1, float arg2, int arg3, int arg4, int arg5);
    public delegate int   camSetScreenShake3B(int arg1, int arg2, float arg3, int arg4, int arg5, int arg6);
    public delegate int   camScrSetCam(int arg1, int arg2);
    public delegate int   camFreeBattle();
    public delegate float camGetRoll();
    public delegate float camGetScrDpt();
    public delegate int   camScrResetMove(int arg1);
    public delegate int   camDrawResetMove(int arg1);
    public delegate int   camScrWait(int arg1);
    public delegate int   camDrawWait(int arg1);
    public delegate int   camBlur(int arg1);
    public delegate int   camFocus(int arg1);
    public delegate int   camSetFocus(int arg1, int arg2);
    public delegate int   camRand(int arg1);
    public delegate int   refSetShake4(int arg1, float arg2, int arg3, int arg4, int arg5);
    public delegate int   camSetShake4(int arg1, float arg2, int arg3, int arg4, int arg5);
    public delegate int   refSetShake5(int arg1, float arg2, int arg3, int arg4, int arg5);
    public delegate int   camSetShake5(int arg1, float arg2, int arg3, int arg4, int arg5);
    public delegate int   camGetRealPos(float* arg1, float* arg2, float* arg3);
    public delegate int   refGetRealPos(float* arg1, float* arg2, float* arg3);
    public delegate int   refReset();
    public delegate int   camReset();
}
