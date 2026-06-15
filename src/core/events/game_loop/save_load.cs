// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

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
