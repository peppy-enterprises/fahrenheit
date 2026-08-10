// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

using Fahrenheit.Events;
using Fahrenheit.FFX;

namespace Fahrenheit.Runtime;

/// <summary>
///     Patches game systems to be (more) independent of the target framerate.
/// </summary>
[FhLoad(FhGameId.FFX | FhGameId.FFX2 | FhGameId.FFX2LM)]
public unsafe sealed class FhSpdCtrlModule : FhModule {

    private sbyte _Sg_KeepFps       = 0;
    private uint  _ppvUserStopPartF = 0;

    private static uint sg_count {
        get => FhUtil.get_at<uint>(FhUtil.select(0x1FCBBF0, 0x16CDD60, 0x16CDD60));
        set => FhUtil.set_at      (FhUtil.select(0x1FCBBF0, 0x16CDD60, 0x16CDD60), value);
    }

    private static uint sg_vcount {
        get => FhUtil.get_at<uint>(FhUtil.select(0xEFB7A8, 0x9F7430, 0x9F7430));
        set => FhUtil.set_at      (FhUtil.select(0xEFB7A8, 0x9F7430, 0x9F7430), value);
    }

    private static uint sg_vcount2 {
        get => FhUtil.get_at<uint>(FhUtil.select(0xEFB7AC, 0x9F7434, 0x9F7434));
        set => FhUtil.set_at      (FhUtil.select(0xEFB7AC, 0x9F7434, 0x9F7434), value);
    }

    /* [fkelava 09/08/26 22:41]
     * You would think that if SetFlipVSyncInterval is a thing, there'd be a getter method. You'd be wrong.
     */
    private static uint s_flipVSyncInterval {
        get => FhUtil.get_at<uint>(FhUtil.select(0x830E88, 0x9B34B0, 0x9B34B0));
        set => FhUtil.set_at      (FhUtil.select(0x830E88, 0x9B34B0, 0x9B34B0), value);
    }

    private static uint sFMVPlayerManager {
        get => FhUtil.get_at<uint>(FhUtil.select(0x8DED2C, 0x9C6960, 0x9C6960));
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        bool is_ffx = FhGlobal.game_id is FhGameId.FFX;

        return FhApi.Events.Common.GameLoop.PreUpdate.subscribe(h_pre)
            && FhCall.Phyre_PFramework_PApplication_frame                   .hook(this, h_frame)
            && FhCall.Phyre_PFramework_PWindowWin32Base_SetFlipVSyncInterval.hook(this, h_set_vsync)
            && FhCall.MsCameraMoveFrame                                     .hook(this, h_MsCameraMoveFrame)
            && FhCall.MsCameraMoveAcc                                       .hook(this, h_MsCameraMoveAcc)
            && FhCall.Sg_SetKeepFps                                         .hook(this, h_Sg_SetKeepFps)
            && FhCall.FUN_00821F90_00606930                                 .hook(this, h_FUN_00821F90_00606930)
            && (!is_ffx || FFX.FhCall._set_ppvUserStopPartF                 .hook(this, h__set_ppvUserStopPartF))
            && (!is_ffx || FFX.FhCall.CT_0000_Init                          .hook(this, h_CT_0000_Init))
            && (!is_ffx || FFX.FhCall.graphicDrawMainMenuWaterEffect        .hook(this, h_graphicDrawMainMenuWaterEffect))
            && (!is_ffx || FFX .FhCall.Ch_SetMotionSpeed                    .hook(this, h_Ch_SetMotionSpeed))
            && ( is_ffx || FFX2.FhCall.Ch_SetMotionSpeed                    .hook(this, h_Ch_SetMotionSpeed_2))
            && (!is_ffx || FFX.FhCall.TOBtlCtrlLimitTimer                   .hook(this, h_TOBtlCtrlLimitTimer))
            && (!is_ffx || FFX.FhCall.Ch_CalcMain                           .hook(this, h_Ch_CalcMain));
    }

    /* [fkelava 11/08/26 00:08]
     * The water effect in the FF X main menu is a series of images scrolling by, one per frame.
     * We have to 'frame skip' by having it render the same one for two frames.
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void h_graphicDrawMainMenuWaterEffect() {
        FFX.FhCall.graphicDrawMainMenuWaterEffect.chain_from(h_graphicDrawMainMenuWaterEffect).fnptr!();

        byte water_current_frame = FhUtil.get_at<byte>(0x8CBA09);

        if (s_flipVSyncInterval == 1 && sg_count % 2 == 0) {
            FhUtil.set_at(0x8CBA09, byte.Clamp(water_current_frame--, 0, 69));
        }
    }

    /* [fkelava 10/08/26 22:14]
     * Unlike most other systems where it is possible to pre-emptively retime a wait for the target framerate,
     * particles have full control over their own timing. Patching them all is impossible. Instead, we have
     * to selectively 'drop frames' from their perspective by asserting `ppvUserStopPartF` every other frame.
     */

    private void h_pre(UpdateLoopEventArgs args) {
        if (Interlocked.CompareExchange(ref _ppvUserStopPartF, 1, 1) == 1) return;

        uint stop_particles_this_frame = s_flipVSyncInterval == 1 && sg_count % 2 == 0
            ? 1U
            : 0U;

        FFX.FhCall._set_ppvUserStopPartF.chain_from(h__set_ppvUserStopPartF).fnptr!(stop_particles_this_frame);
    }

    /* [fkelava 10/08/26 22:14]
     * FF X explicitly disables particles at only one instance, in Zanarkand - Harbour:
     *
     * 022C | AE0100 D86680 | call Map.setGfxPausedGlobal [8066h](paused=true [01h]);
     * 059E | AE0000 D86680 | call Map.setGfxPausedGlobal [8066h](paused=false [00h]);
     *
     * We intercept this so we know not to interfere during this time.
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void h__set_ppvUserStopPartF(uint arg1) {
        Interlocked.Exchange(ref _ppvUserStopPartF, arg1);

        FFX.FhCall._set_ppvUserStopPartF.chain_from(h__set_ppvUserStopPartF).fnptr!(arg1);
    }

    /* [fkelava 10/08/26 15:28]
     * Motion speed is proportional to framerate, and animation speed is coupled to motion speed _unless_ `Sg_GetKeepFps` is asserted.
     *
     * Thus, retiming _motions_ also retimes _animations_, which is required to preserve cutscene pacing.
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void h_Ch_SetMotionSpeed(Actor* ptr_actor, ushort speed) {
        if (_Sg_KeepFps == 0) {
            speed /= 2;
        }

        FFX.FhCall.Ch_SetMotionSpeed.chain_from(h_Ch_SetMotionSpeed).fnptr!(ptr_actor, speed);
    }

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void h_Ch_SetMotionSpeed_2(uint ptr_actor, ushort speed) {
        if (_Sg_KeepFps == 0) {
            speed /= 2;
        }

        FFX2.FhCall.Ch_SetMotionSpeed.chain_from(h_Ch_SetMotionSpeed_2).fnptr!(ptr_actor, speed);
    }

    /* [fkelava 10/08/26 15:28]
     * The game in `Sg_MainCalcRate` computes, every frame, `sg_rate{f}` - the ratio of vertical (`sg_vcount`)
     * to horizontal (`sg_count`) blanks. We already, in `h_FUN_00821F90_{...}`, ensure we properly count up
     * vertical blanks instead of assuming two per horizontal blank.
     *
     * If `Sg_GetKeepFps` is asserted, the game uses `sg_rate` to retime animations. See `Ch_Anim`. This provides
     * the correct result, but then we have to be careful not to separately retime motions, which would cause
     * a double slowdown.
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private sbyte h_Sg_SetKeepFps(sbyte arg1) {
        _Sg_KeepFps = arg1;

        return FhCall.Sg_SetKeepFps.chain_from(h_Sg_SetKeepFps).fnptr!(arg1);
    }

    /* [fkelava 09/08/26 22:07]
     * The game assumes there are always two vertical blanks for each horizontal blank.
     * Correcting for this is required to properly retime animations.
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void h_FUN_00821F90_00606930(float delta) {
        FhCall.FUN_00821F90_00606930.chain_from(h_FUN_00821F90_00606930).fnptr!(delta);

        sg_vcount  = sg_count;
        sg_vcount2 = sg_count;
    }

    /* [fkelava 10/08/26 14:40]
     * The camera predominantly (or even entirely?) uses frame-based waits.
     * If they are not retimed, cutscenes will fall out of sync because they can synchronize on a `camWait`.
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void h_MsCameraMoveFrame(
        uint camera_id,
        uint arg2,
        uint arg3,
        uint frame_count,
        uint arg5
    ) {
        frame_count *= 2;

        FhCall.MsCameraMoveFrame.chain_from(h_MsCameraMoveFrame).fnptr!(camera_id, arg2, arg3, frame_count, arg5);
    }

    /* [fkelava 10/08/26 14:40]
     * TODO: Check if all four arguments should, in fact, be retimed.
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void h_MsCameraMoveAcc(
        uint camera_id,
        uint mode_non_ref,
        uint mode_polar,
        uint arg4,
        uint arg5,
        uint arg6,
        uint arg7
    ) {
        arg4 *= 2;
        arg5 *= 2;
        arg6 *= 2;
        arg7 *= 2;

        FhCall.MsCameraMoveAcc.chain_from(h_MsCameraMoveAcc).fnptr!(camera_id, mode_non_ref, mode_polar, arg4, arg5, arg6, arg7);
    }

    /* [fkelava 10/08/26 00:11]
     * Frame-based ATEL waits have to be doubled if we're in 60FPS mode,
     * except trivial one-frame waits which are used as 'idle' loops.
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void h_CT_0000_Init(AtelBasicWorker* work, int* storage, AtelStack* stack) {
        int rv = FFX.FhCall.AtelPopStackInteger.fnptr!((int*)work, stack);

        *storage = s_flipVSyncInterval == 1U && rv != 1
            ? rv * 2
            : rv;
    }

    /* [fkelava 09/08/26 22:07]
     * The game invokes this with fixed `delta = 0.033373334`.
     * We adjust for the target framerate.
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void h_Ch_CalcMain(float delta) {
        FFX.FhCall.Ch_CalcMain.chain_from(h_Ch_CalcMain).fnptr!(1F / (60 / s_flipVSyncInterval));
    }

    /* [fkelava 09/08/26 22:07]
     * The target framerate is (60 / s_FlipVSyncInterval).
     *
     * The game normally uses an interval of 1 in the menus, and 2 everywhere else.
     * Since we control this by hand, we simply ignore all requests to set it.
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void h_set_vsync(uint param_1) { }

    /* [fkelava 09/08/26 22:07]
     * When timing limits, the game uses either 30 or 25 as a fixed framerate divisor.
     * We adjust for the target framerate so they don't run at double speed.
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void h_TOBtlCtrlLimitTimer() {
        if (FhUtil.get_at<uint>(0xF3F73C) != 2) return;

        float* limit_timer_frames  = (float*)(FhEnvironment.BaseAddr + 0xF3F748);
        float* limit_timer_base    = (float*)(FhEnvironment.BaseAddr + 0xF3F74C);
        float* limit_timer_raw     = (float*)(FhEnvironment.BaseAddr + 0xF3F750);
        float* limit_timer_rounded = (float*)(FhEnvironment.BaseAddr + 0xF3F754);

        *limit_timer_frames = *limit_timer_frames + 1F;
        *limit_timer_raw    = *limit_timer_base   - (*limit_timer_frames / (60 / s_flipVSyncInterval));

        if (*limit_timer_raw <= 0F) {
            *limit_timer_raw     = 0;
            *limit_timer_rounded = 0;
            return;
        }

        /* [fkelava 09/08/26 22:07]
         * Instead of simply rounding the raw time, the game effectively chooses
         * a random final digit to give the impression the timer is updated
         * faster than it actually is. We preserve this quirk for compatibility.
         */

        uint iVar2 = FhCall.rnd.fnptr!();
        uint uVar3 = uint.CreateSaturating( *limit_timer_raw * 10 );

        FhUtil.set_at(0xF3F754, ((iVar2 % 10) + (uVar3 * 10)) / 100F);
    }

    /* [fkelava 10/08/26 14:43]
     * FMVs run at double speed when we're removing the frame limiter. We adapt by
     * lowering the framerate back to 30 when a video is set to play.
     *
     * Note that 'graphicIsVideoPlaying' begins firing at a variable delay before the FMV
     * actually begins. This is a bit ugly, but better slow down ahead of time than mid-FMV.
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvThiscall) ] )]
    private uint h_frame(PApplication* ptr_this) {
        /* [fkelava 09/08/26 23:05]
         * You would think 'graphicIsVideoPlaying' would check whether
         * sFMVPlayerManager is null before dereferencing its fields. Nope.
         */
        s_flipVSyncInterval = sFMVPlayerManager == 0 || FhCall.graphicIsVideoPlaying.fnptr!() == 1
            ? 2U
            : 1U;

        return FhCall.Phyre_PFramework_PApplication_frame.chain_from(h_frame).fnptr!(ptr_this);
    }
}
