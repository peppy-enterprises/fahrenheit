// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

using Fahrenheit.Events;
using Fahrenheit.FFX;
using Fahrenheit.FFX.Ids;

namespace Fahrenheit.Runtime;

/// <summary>
///     Patches game systems to be (more) independent of the target framerate.
/// </summary>
[FhLoad(FhGameId.FFX | FhGameId.FFX2 | FhGameId.FFX2LM)]
[SupportedOSPlatform("windows5.1.2600")]
public unsafe sealed class PWarpModule : FhModule {

    private sbyte _Sg_KeepFps = 0;

    private static uint ppvUserStopPartF {
        get => FhUtil.get_at<uint>(FhUtil.select(0x1F0FD34, 0x18F679C, 0x18F679C));
        set => FhUtil.set_at      (FhUtil.select(0x1F0FD34, 0x18F679C, 0x18F679C), value);
    }

    private static uint sg_count {
        get => FhUtil.get_at<uint>(FhUtil.select(0x1FCBBF0, 0x16CDD60, 0x16CDD60));
        set => FhUtil.set_at      (FhUtil.select(0x1FCBBF0, 0x16CDD60, 0x16CDD60), value);
    }

    /* [fkelava 09/08/26 22:41]
     * You would think that if SetFlipVSyncInterval is a thing, there'd be a getter method. You'd be wrong.
     */
    private static uint s_flipVSyncInterval {
        get => FhUtil.get_at<uint>(FhUtil.select(0x830E88, 0x9B34B0, 0x9B34B0));
        set => FhUtil.set_at      (FhUtil.select(0x830E88, 0x9B34B0, 0x9B34B0), value);
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        bool is_ffx = FhGlobal.game_id is FhGameId.FFX;

        return FhApi.Events.Common.GameLoop.PreUpdate.subscribe(h_pre)
            && FhCall.Phyre_PFramework_PApplication_frame                   .hook(this, h_frame)
            && FhCall.Phyre_PFramework_PWindowWin32Base_SetFlipVSyncInterval.hook(this, h_set_vsync)
            && FhCall.MsCameraMoveFrame                                     .hook(this, h_MsCameraMoveFrame)
            && FhCall.MsCameraMoveAcc                                       .hook(this, h_MsCameraMoveAcc)
            && FhCall.Sg_SetKeepFps                                         .hook(this, h_Sg_SetKeepFps)
            && FhCall.pppFpStopStatus                                       .hook(this, h_pppFpStopStatus)
            && FhCall.Sg_Flash                                              .hook(this, h_Sg_Flash)
            && FhCall.Sg_Fade_Common                                        .hook(this, h_Sg_Fade_Common)
            && FhCall.enableGameControlTextureAnimation                     .hook(this, h_enableGameControlTextureAnimation)
            && FhCall.PhyFMVPlayerManager_UpdateTexture                     .hook(this, h_fmv_UpdateTexture)
            && (!is_ffx || FFX. FhCall.MsEffectSetSpeed                     .hook(this, h_MsEffectSetSpeed))
            && (!is_ffx || FFX .FhCall.FUN_003B4AA0                         .hook(this, h_FUN_003B4AA0))
            && (!is_ffx || FFX .FhCall.MsSetChrStatInfo                     .hook(this, h_MsSetChrStatInfo))
            && (!is_ffx || FFX .FhCall.Sg_AccSetAlpha                       .hook(this, h_Sg_AccSetAlpha))
            && (!is_ffx || FFX .FhCall.CT_0000_Init                         .hook(this, h_CT_0000_Init))
            && (!is_ffx || FFX .FhCall.graphicDrawMainMenuWaterEffect       .hook(this, h_graphicDrawMainMenuWaterEffect))
            && (!is_ffx || FFX .FhCall.Ch_SetMotionSpeed                    .hook(this, h_Ch_SetMotionSpeed))
            && ( is_ffx || FFX2.FhCall.Ch_SetMotionSpeed                    .hook(this, h_Ch_SetMotionSpeed_2))
            && (!is_ffx || FFX .FhCall.TOBtlCtrlLimitTimer                  .hook(this, h_TOBtlCtrlLimitTimer))
            && (!is_ffx || FFX .FhCall.Ch_CalcMain                          .hook(this, h_Ch_CalcMain));
    }

    // TODO : explain affected CTs
    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void h_MsEffectSetSpeed(byte chr_id, ushort speed) {
        speed /= 2;

        FFX.FhCall.MsEffectSetSpeed.chain_from(h_MsEffectSetSpeed).fnptr!(chr_id, speed);
    }

    // TODO : explain affected CTs
    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void h_MsSetChrStatInfo(uint chr_id, uint stat_id, uint target_id, uint value) {
        value = stat_id switch {
            ChrStatId.STAT_ATTACK_INC_SPEED    or
            ChrStatId.STAT_ATTACK_DEC_SPEED    => value / 2,
            ChrStatId.STAT_ATTACK_NORMAL_FRAME or
            ChrStatId.STAT_ATTACK_NEAR_FRAME   or
            ChrStatId.STAT_ATTACK_MOTION_FRAME => value * 2,
            _                                  => value
        };

        FFX.FhCall.MsSetChrStatInfo.chain_from(h_MsSetChrStatInfo).fnptr!(chr_id, stat_id, target_id, value);
    }

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void h_FUN_003B4AA0(uint chr_id, uint stat_id, float value) {
        value = stat_id switch {
            0x03 or            // MOTION_RUN_SPEED
            0x04 or            // MOTION_RUN_SPEED_RETURN
            0x05 or            // MOTION_RUN_SPEED_V0
            0x06 => value / 2, // MOTION_RUN_SPEED_ACC
            _    => value
        };

        FFX.FhCall.FUN_003B4AA0.chain_from(h_FUN_003B4AA0).fnptr!(chr_id, stat_id, value);
    }

    /* [fkelava 10/08/26 22:14]
     * Unlike most other systems where it is possible to pre-emptively retime a wait for the target framerate,
     * particles have full control over their own timing. Patching them all is impossible. Instead, we have
     * to selectively 'drop frames' from their perspective by asserting `ppvUserStopPartF` every other frame.
     */

    private void h_pre(UpdateLoopEventArgs args) {
        FhCall.enableGameControlTextureAnimation.chain_from(h_enableGameControlTextureAnimation).fnptr!(
            sg_count % 2 == 0
                ? 0U
                : 1U);
    }

    /* [fkelava 13/08/26 22:13]
     * Texture animation control is suppressed and (re)activated every other frame.
     */

    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private void h_enableGameControlTextureAnimation(uint enable) { }

    // TODO : explain affected CTs

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void h_Sg_AccSetAlpha(ushort alpha, ushort frame_count) {
        frame_count *= 2;

        FFX.FhCall.Sg_AccSetAlpha.chain_from(h_Sg_AccSetAlpha).fnptr!(alpha, frame_count);
    }

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void h_Sg_Flash(ushort frame_count, byte arg2, byte arg3, byte arg4) {
        frame_count *= 2;

        FhCall.Sg_Flash.chain_from(h_Sg_Flash).fnptr!(frame_count, arg2, arg3, arg4);
    }

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void h_Sg_Fade_Common(ushort frame_count, uint mode_in, uint mode_w) {
        frame_count *= 2;

        FhCall.Sg_Fade_Common.chain_from(h_Sg_Fade_Common).fnptr!(frame_count, mode_in, mode_w);
    }

    /* [fkelava 12/08/26 13:33]
     * FMVs run at double speed when the game does, as the game hardcodes a framerate of 29.97
     * when calculating the time-step in the video update loop.
     *
     * We thus frameskip that update loop, which produces a optically correct result. If the FMV
     * were recoded to a different framerate, this would break; but such a user would have to patch
     * that constant in `.rdata` anyway. Warp can provide support for this when the need arises...
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvThiscall) ] )]
    private void h_fmv_UpdateTexture(uint ptr_this) {
        if (sg_count % 2 == 0) {
            FhCall.PhyFMVPlayerManager_UpdateTexture.chain_from(h_fmv_UpdateTexture).fnptr!(ptr_this);
        }
    }

    /* [fkelava 11/08/26 00:08]
     * The water effect in the FF X main menu is a series of images scrolling by, one per frame.
     * We have to 'frame skip' by having it render the same one for two frames.
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void h_graphicDrawMainMenuWaterEffect() {
        FFX.FhCall.graphicDrawMainMenuWaterEffect.chain_from(h_graphicDrawMainMenuWaterEffect).fnptr!();

        byte water_current_frame = FhUtil.get_at<byte>(0x8CBA09);

        if (sg_count % 2 == 0) {
            FhUtil.set_at(0x8CBA09, byte.Clamp(water_current_frame--, 0, 69));
        }
    }

    /* [fkelava 10/08/26 22:14]
     * If the game explicitly disables particles, we should not interfere.
     *
     * An example from FF X in Zanarkand - Harbour:
     *
     * 022C | AE0100 D86680 | call Map.setGfxPausedGlobal [8066h](paused=true [01h]);
     * 059E | AE0000 D86680 | call Map.setGfxPausedGlobal [8066h](paused=false [00h]);
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void h_pppFpStopStatus(uint arg1) {
        ppvUserStopPartF = arg1;
    }

    /* [fkelava 10/08/26 15:28]
     * Motion speed is proportional to framerate, and animation speed is coupled to motion speed _unless_
     * `Sg_GetKeepFps` is asserted or an individual actor's `Ch_GetKeepFps` is asserted.
     *
     * Thus, retiming _motions_ also retimes _animations_, which is required to preserve cutscene pacing.
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void h_Ch_SetMotionSpeed(Actor* ptr_actor, ushort speed) {
        if (_Sg_KeepFps == 0 || !ptr_actor->chr_flags.HasFlag(ActorFlags.KEEP_FPS)) {
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

        *storage = rv != 1
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
     * We let the game retime its own display/frame loop by setting the
     * flip V-Sync interval to one frame instead of two.
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvThiscall) ] )]
    private uint h_frame(PApplication* ptr_this) {
        s_flipVSyncInterval = 1U;

        return FhCall.Phyre_PFramework_PApplication_frame.chain_from(h_frame).fnptr!(ptr_this);
    }
}
