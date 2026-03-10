// SPDX-License-Identifier: MIT

namespace Fahrenheit.Events.Common.GameLoop;

/// <summary>
///     Event arguments for:<br/>
///     <see cref="FFX.GameLoopEvents.OnSaveState"/>,<br/>
///     <see cref="FFX2.GameLoopEvents.OnSaveState"/>,<br/>
///     <see cref="FFX.GameLoopEvents.OnLoadState"/>,<br/>
///     <see cref="FFX2.GameLoopEvents.OnLoadState"/>.
/// </summary>
public struct SaveLoadEventArgs {
    public readonly int save_slot_idx;

    public readonly bool is_autosave => save_slot_idx == 0;

    internal SaveLoadEventArgs(int slot_idx) {
        save_slot_idx = slot_idx;
    }
}
