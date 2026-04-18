// SPDX-License-Identifier: MIT

namespace Fahrenheit.Events;

/// <summary>
///     Event arguments for <see cref="GameLoopEvents.PreSaveGame">PreSaveGame</see>
///     and <see cref="GameLoopEvents.PostLoadGame">PostLoadGame</see>.
/// </summary>
public readonly ref struct SaveLoadEventArgs {
    /// <summary>The index of the slot being saved to or loaded.</summary>
    public int save_slot_idx { get; internal init; }

    public bool is_autosave => save_slot_idx == 0;
}

public partial class GameLoopEvents {
    /// <summary>Event raised before a game is saved.</summary>
    //TODO: Implement PreSaveGame
    public FhEvent<SaveLoadEventArgs> PreSaveGame = new();

    /// <summary>Event raised after a game is loaded.</summary>
    public FhEvent<SaveLoadEventArgs> PostLoadGame = new();
}
