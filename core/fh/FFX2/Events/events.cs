// SPDX-License-Identifier: MIT

namespace Fahrenheit.FFX2.Events;

public class FhX2Events {
    public GameLoopEvents GameLoop = new();
}

/// <summary>
///     Events linked to the game's update loop. The update loop runs 60 times every second, corresponding to FPS.<br/>
///     These can be useful, but you should consider more specific events if possible.
/// </summary>
public partial class GameLoopEvents { }
