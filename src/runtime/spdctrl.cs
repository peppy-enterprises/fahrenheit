// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit.Runtime;

/// <summary>
///     Patches game systems to be (more) independent of the target framerate.
/// </summary>
[FhLoad(FhGameId.FFX | FhGameId.FFX2 | FhGameId.FFX2LM)]
public unsafe sealed class FhSpdCtrlModule : FhModule {

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

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate sbyte d_Sg_GetKeepFps();
    internal static FhMethodHandle<d_Sg_GetKeepFps> Sg_GetKeepFps =>
        new( new FhMethodLocation("FFX.exe", 0x4206B0) );

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        bool is_ffx = FhGlobal.game_id is FhGameId.FFX;

        return FhCall.Phyre_PFramework_PApplication_frame                   .hook(this, h_frame)
            && FhCall.Phyre_PFramework_PWindowWin32Base_SetFlipVSyncInterval.hook(this, h_set_vsync)
            && (!is_ffx || FFX.FhCall.CT_0000_Init                          .hook(this, h_CT_0000_Init))
            && (!is_ffx || FFX.FhCall.TOBtlCtrlLimitTimer                   .hook(this, h_TOBtlCtrlLimitTimer))
            && (!is_ffx || FFX.FhCall.Ch_CalcMain                           .hook(this, h_Ch_CalcMain))
            && Sg_GetKeepFps                                                .hook(this, h_Sg_GetKeepFps)
            && FhCall.MsCameraMoveFrame                                     .hook(this, h_MsCameraMoveFrame)
            && FhCall.MsCameraMoveAcc                                       .hook(this, h_MsCameraMoveAcc)
            && FhCall.FUN_00821F90_00606930                                 .hook(this, h_FUN_00821F90_00606930);
    }

    /* [fkelava 10/08/26 15:28]
     * For animations, we take advantage of a provision the game already has.
     *
     * The game in `Sg_MainCalcRate` computes, every frame, `sg_rate{f}` - the ratio of vertical (`sg_vcount`)
     * to horizontal (`sg_count`) blanks. We already, in `h_FUN_00821F90_{...}`, ensure we properly count up
     * vertical blanks instead of assuming two per horizontal blank.
     *
     * If `Sg_GetKeepFps` is asserted, the game uses `sg_rate` to retime animations. See `Ch_Anim`.
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private sbyte h_Sg_GetKeepFps() {
        return 1;
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
        if (s_flipVSyncInterval == 1) {
            frame_count *= 2;
        }

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
        if (s_flipVSyncInterval == 1) {
            arg4 *= 2;
            arg5 *= 2;
            arg6 *= 2;
            arg7 *= 2;
        }

        FhCall.MsCameraMoveAcc.chain_from(h_MsCameraMoveAcc).fnptr!(camera_id, mode_non_ref, mode_polar, arg4, arg5, arg6, arg7);
    }

    /* [fkelava 10/08/26 00:11]
     * Frame-based ATEL waits have to be doubled if we're in 60FPS mode,
     * except trivial one-frame waits which are used as 'idle' loops.
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void h_CT_0000_Init(AtelBasicWorker* work, int* storage, AtelStack* stack) {
        int rv = FFX.FhCall.AtelPopStackInteger.fnptr!((int*)work, &work->stack);

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

    /* [fkelava 09/08/26 22:07]
     * The game assumes there will always be two vertical blanks
     * for each horizontal blank. Correcting for this at least partly
     * 'fixes' animations, since they use `sg_vcount` for synchronization
     * when Sg_SetKeepFps is not active.
     */

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ] )]
    private void h_FUN_00821F90_00606930(float delta) {
        FhCall.FUN_00821F90_00606930.chain_from(h_FUN_00821F90_00606930).fnptr!(delta);

        if (s_flipVSyncInterval == 1) {
            sg_vcount  = sg_count;
            sg_vcount2 = sg_count;
        }
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
