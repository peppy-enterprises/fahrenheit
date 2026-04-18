// SPDX-License-Identifier: MIT

namespace Fahrenheit.Runtime.Events;

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
        _h_main_loop = new(this, _location_main_loop, raise_update_events);

        //TODO: Support `PostReturnToTitle` in FFX-2
        if (FhGlobal.game_id == FhGameId.FFX) {
            _h_jump_to_event = new(this, "FFX.exe", __addr_AtelSetEventJump2, handle_warp);
        }
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        return _h_main_loop.hook()
            && (_h_jump_to_event?.hook() ?? true);
    }

    /// <summary>
    ///     Runs around the game's main loop to raise the
    ///     <see cref="Fahrenheit.Events.GameLoopEvents.PreUpdate">PreUpdate</see>
    ///     and <see cref="Fahrenheit.Events.GameLoopEvents.PostUpdate">PostUpdate</see>
    ///     events before and after every iteration, respectively.
    /// </summary>
    private void raise_update_events(float delta) {
        FhApi.Events.Common.GameLoop.PreUpdate.invoke(new() { delta = delta });

        _h_main_loop.orig_fptr(delta);

        FhApi.Events.Common.GameLoop.PostUpdate.invoke(new() { delta = delta });
    }

    /// <summary>
    ///     Runs after the player warps to a new room to raise the
    ///     <see cref="Fahrenheit.Events.GameLoopEvents.PostReturnToTitle">PostReturnToTitle</see>
    ///     event.
    /// </summary>
    /// <param name="room">The room we are warping to</param>
    /// <param name="entrance">The entrance the main character will spawn at</param>
    /// <param name="do_fade">Non-zero if we should fade, zero if not</param>
    private void handle_warp(int room, int entrance, int do_fade) {
        _h_jump_to_event!.orig_fptr(room, entrance, do_fade);

        if (room == 23 && entrance == 0) {
            // Warping to the title screen
            FhApi.Events.Common.GameLoop.PostReturnToTitle.invoke(EventArgs.Empty);
        }
    }
}
