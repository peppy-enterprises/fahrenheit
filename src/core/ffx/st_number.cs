// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit.FFX;

/// <summary>
///     Responsible for displaying commands the player has unlocked in menus.
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 0x4)]
public struct StNumber {
    /// <summary>
    ///     Can either be a valid <see cref="PlySaveId"> id or submenu id
    /// </summary>
    [FieldOffset(0x0)] public byte  category;   
    
    [FieldOffset(0x1)] public byte  type;
    [FieldOffset(0x2)] public ushort command_id; // Can also be an Aeon id if category is 0x1 (Yuna)
}
