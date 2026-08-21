// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit.Gui;

/// <summary>Represents alignment of some element on one axis.</summary>
public enum Alignment {
    BEGIN  = 0,
    CENTER = 1,
    END    = 2,
}

/// <summary>Represents alignment of some element in 2D.</summary>
/// <param name="h">The horizontal alignment of the element.</param>
/// <param name="v">The vertical alignment of the element.</param>
public readonly record struct Alignment2D (
    Alignment h,
    Alignment v
);
