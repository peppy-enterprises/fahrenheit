// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit.FFX;

[Flags]
public enum ElementFlags : byte {
    NONE    = 0,
    FIRE    = 1 << 0,
    ICE     = 1 << 1,
    THUNDER = 1 << 2,
    WATER   = 1 << 3,
    HOLY    = 1 << 4,
}

public static partial class FhEnumExt {
    extension(ElementFlags flags) {
        public bool fire    => flags.HasFlag(ElementFlags.FIRE);
        public bool ice     => flags.HasFlag(ElementFlags.ICE);
        public bool thunder => flags.HasFlag(ElementFlags.THUNDER);
        public bool water   => flags.HasFlag(ElementFlags.WATER);
        public bool holy    => flags.HasFlag(ElementFlags.HOLY);
    }
}
