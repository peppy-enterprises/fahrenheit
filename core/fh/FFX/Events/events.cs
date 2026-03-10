// SPDX-License-Identifier: MIT

namespace Fahrenheit.FFX.Events;

public class FhXEvents {
    public GameLoopEvents GameLoop = new();
}

/// <summary>
///     Events linked to the game's update loop. The update loop runs 60 times every second, corresponding to FPS.<br/>
///     These can be useful, but you should consider more specific events if possible.
/// </summary>
public partial class GameLoopEvents { }
