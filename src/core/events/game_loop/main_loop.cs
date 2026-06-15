// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit.Events;

/// <summary>
///     Event arguments for <see cref="GameLoopEvents.PreUpdate">PreUpdate</see>
///     and <see cref="GameLoopEvents.PostUpdate">PostUpdate</see>.
/// </summary>
public readonly ref struct UpdateLoopEventArgs {
    /// <summary>The time elapsed since the previous frame.</summary>
    public float delta { get; internal init; }
}

public partial class GameLoopEvents {
    /// <summary>Raised before the game's main update loop.</summary>
    public FhEvent<UpdateLoopEventArgs> PreUpdate = new();

    /// <summary>Raised after the game's main update loop</summary>
    public FhEvent<UpdateLoopEventArgs> PostUpdate = new();
}
