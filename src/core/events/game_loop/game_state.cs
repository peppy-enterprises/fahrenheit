// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

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
