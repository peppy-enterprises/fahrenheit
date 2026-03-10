// SPDX-License-Identifier: MIT

namespace Fahrenheit.Modules.Runtime.Events;

//TODO: Support FFX-2
[FhLoad(FhGameId.FFX)]
public class GameLoopEventsImpl : FhModule {
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void AtelSetEventJump2(int room, int entrance, int do_fade);
    private const nint __addr_AtelSetEventJump2 = 0x46fed0;

    private FhMethodHandle<AtelSetEventJump2> _h_jump_to_event;

    public GameLoopEventsImpl() {
        _h_jump_to_event = new(this, "FFX.exe", __addr_AtelSetEventJump2, handle_warp);
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        return _h_jump_to_event.hook();
    }

    private void handle_warp(int room, int entrance, int do_fade) {
        if (room == 23 && entrance == 0) {
            // Warping to the title screen
            FhApi.Events.FFX.GameLoop.PostReturnToTitle.invoke(EventArgs.Empty);
        }
    }
}
