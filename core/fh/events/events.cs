// SPDX-License-Identifier: MIT

namespace Fahrenheit.Events;

public class FhEvents {
    // The initialization order here matters.
    // FFX and FFX2 mirror some Common events in their structures to improve end-user experience,
    // so Common needs to be initialized first.
    public readonly Fahrenheit.Events.FhCommonEvents  Common = new();
    public readonly Fahrenheit.FFX.Events.FhXEvents   FFX    = new();
    public readonly Fahrenheit.FFX2.Events.FhX2Events FFX2   = new();
}

// Events shared between the two games
public class FhCommonEvents {
    public readonly GameLoopEvents GameLoop = new();
}

/// <summary>
///     Events linked to the game's update loop. The update loop runs every frame.<br/>
///     These can be useful, but you should consider more specific events if possible.
/// </summary>
public partial class GameLoopEvents {}
