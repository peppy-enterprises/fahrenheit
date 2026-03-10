// SPDX-License-Identifier: MIT

using Fahrenheit.Events.Common.GameLoop;

namespace Fahrenheit.Events.FFX;

public partial class GameLoopEvents {
    /// <summary>Event raised before a game is saved.</summary>
    //TODO: Implement PreSaveGame
    public FhEvent<SaveLoadEventArgs> PreSaveGame = new();

    /// <summary>Event raised after a game is loaded.</summary>
    public FhEvent<SaveLoadEventArgs> PostLoadGame = new();
}
