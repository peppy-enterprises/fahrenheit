// SPDX-License-Identifier: MIT

namespace Fahrenheit.Modules.Runtime.Events;

[FhLoad(FhGameId.FFX | FhGameId.FFX2 | FhGameId.FFX2LM)]
public class GameLoopEventsImplModule : FhModule {
    // Delegates for method handles
    //TODO: Change to using FhCall delegates once it's updated
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void Sg_MainLoop(float delta);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void AtelSetEventJump2(int room, int entrance, int do_fade);
    private const nint __addr_AtelSetEventJump2 = 0x46FED0;

    // Method locations
    private readonly FhMethodLocation _location_main_loop = new(0x420C00, 0x205150);

    // Method handles
    private readonly FhMethodHandle<Sg_MainLoop>        _h_main_loop;
    private readonly FhMethodHandle<AtelSetEventJump2>? _h_jump_to_event;

    public GameLoopEventsImplModule() {
        _h_main_loop = new(this, _location_main_loop, main_loop);

        //TODO: Support `PostReturnToTitle` in FFX-2, if it's a thing
        if (FhGlobal.game_id == FhGameId.FFX) {
            _h_jump_to_event = new(this, "FFX.exe", __addr_AtelSetEventJump2, handle_warp);
        }
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        return _h_main_loop.hook()
            && (_h_jump_to_event?.hook() ?? true);
    }

    /// <summary>
    ///     Overrides the game's main loop to raise<br/>
    ///     - <see cref="FFX.Events.GameLoopEvents.PreUpdate"/><br/>
    ///     - <see cref="FFX2.Events.GameLoopEvents.PreUpdate"/><br/>
    ///     and<br/>
    ///     - <see cref="FFX.Events.GameLoopEvents.PostUpdate"/><br/>
    ///     - <see cref="FFX2.Events.GameLoopEvents.PostUpdate"/><br/>
    ///     events before and after every iteration, respectively.
    /// </summary>
    private void main_loop(float delta) {
        FhUtil.select(
            FhApi.Events.FFX.GameLoop.PreUpdate,
            FhApi.Events.FFX2.GameLoop.PreUpdate,
            FhApi.Events.FFX2.GameLoop.PreUpdate
        ).invoke(new() { delta = delta });

        _h_main_loop.orig_fptr(delta);

        FhUtil.select(
            FhApi.Events.FFX.GameLoop.PostUpdate,
            FhApi.Events.FFX2.GameLoop.PostUpdate,
            FhApi.Events.FFX2.GameLoop.PostUpdate
        ).invoke(new() { delta = delta });
    }

    private void handle_warp(int room, int entrance, int do_fade) {
        _h_jump_to_event!.orig_fptr(room, entrance, do_fade);

        if (room == 23 && entrance == 0) {
            // Warping to the title screen
            FhApi.Events.FFX.GameLoop.PostReturnToTitle.invoke(EventArgs.Empty);
        }
    }
}
