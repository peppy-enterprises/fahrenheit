// SPDX-License-Identifier: MIT

using Fahrenheit.Events.GameLoop;

namespace Fahrenheit.FFX2.Events;

public partial class GameLoopEvents {
    /// <summary>Event raised before a game is saved.</summary>
    //TODO: Implement PreSaveGame
    public FhEvent<SaveLoadEventArgs> PreSaveGame = new();

    /// <summary>Event raised after a game is loaded.</summary>
    public FhEvent<SaveLoadEventArgs> PostLoadGame = new();
}
