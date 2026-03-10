// SPDX-License-Identifier: MIT

using Fahrenheit.Events.Common.GameLoop;

namespace Fahrenheit.Events.FFX2;

public partial class GameLoopEvents {
    /// <summary>Raised before the game's main update loop.</summary>
    public FhEvent<UpdateLoopEventArgs> PreUpdate = new();

    /// <summary>Raised after the game's main update loop</summary>
    public FhEvent<UpdateLoopEventArgs> PostUpdate = new();
}
