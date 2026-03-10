// SPDX-License-Identifier: MIT

namespace Fahrenheit.Events.GameLoop;

/// <summary>
///     Event arguments for<br/>
///     <see cref="FFX.GameLoopEvents.PreUpdate"/>,<br/>
///     <see cref="FFX2.GameLoopEvents.PreUpdate"/>,<br/>
///     <see cref="FFX.GameLoopEvents.PostUpdate"/>,<br/>
///     <see cref="FFX2.GameLoopEvents.PostUpdate"/>.
/// </summary>
public struct UpdateLoopEventArgs {
    /// <summary>The time elapsed since the previous frame.</summary>
    public float delta;
}
