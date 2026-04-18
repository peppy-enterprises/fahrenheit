// SPDX-License-Identifier: MIT

namespace Fahrenheit.Events;

public partial class GameLoopEvents {
    /// <summary>
    ///     Raised after the player starts a new game.<br/>
    ///     In FFX, this occurs after the Sphere Grid and music selection.
    /// </summary>
    //TODO: Implement PostNewGame
    public FhEvent<EventArgs> PostNewGame = new();

    /// <summary>Raised after the player returns to the main menu.</summary>
    public FhEvent<EventArgs> PostReturnToTitle = new();

    /// <summary>Raised after the player game overs.</summary>
    //TODO: Implement PostGameOver event
    public FhEvent<EventArgs> PostGameOver = new();
}
