// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x1C)]
public struct LocalizationManager {
    [FieldOffset(0x00)] public FhLangId video; // Also voice?
    [FieldOffset(0x04)] public FhLangId current_language;
    [FieldOffset(0x08)] public FhLangId voice; // Unused?
}
