// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit.FFX2;

/// <summary>
///     Commands with these flags set deal quadruple daamge to the respective fiend type.
/// </summary>
[Flags]
public enum SpeciesEffectiveness : ushort {
    NONE    = 0,
    MACHINA = 1 << 0,
    MECH    = 1 << 1,
    LIZARD  = 1 << 2,
    ELEMENT = 1 << 3,
    DRAKE   = 1 << 4,
    DEVIL   = 1 << 5, // Imps and Evil Eyes
    FLAN    = 1 << 6,
    WOLF    = 1 << 7,
    WING    = 1 << 8, // Birds and Wasps
    HELM    = 1 << 9,
}

public static partial class FhEnumExt {
    extension(SpeciesEffectiveness flags) {
        public bool machina {
            get { return flags.HasFlag(SpeciesEffectiveness.MACHINA); }
            set { if (value) flags |= SpeciesEffectiveness.MACHINA; else flags &= ~SpeciesEffectiveness.MACHINA; }
        }

        public bool mech {
            get { return flags.HasFlag(SpeciesEffectiveness.MECH); }
            set { if (value) flags |= SpeciesEffectiveness.MECH; else flags &= ~SpeciesEffectiveness.MECH; }
        }

        public bool lizard {
            get { return flags.HasFlag(SpeciesEffectiveness.LIZARD); }
            set { if (value) flags |= SpeciesEffectiveness.LIZARD; else flags &= ~SpeciesEffectiveness.LIZARD; }
        }

        public bool element {
            get { return flags.HasFlag(SpeciesEffectiveness.ELEMENT); }
            set { if (value) flags |= SpeciesEffectiveness.ELEMENT; else flags &= ~SpeciesEffectiveness.ELEMENT; }
        }

        public bool drake {
            get { return flags.HasFlag(SpeciesEffectiveness.DRAKE); }
            set { if (value) flags |= SpeciesEffectiveness.DRAKE; else flags &= ~SpeciesEffectiveness.DRAKE; }
        }

        public bool devil {
            get { return flags.HasFlag(SpeciesEffectiveness.DEVIL); }
            set { if (value) flags |= SpeciesEffectiveness.DEVIL; else flags &= ~SpeciesEffectiveness.DEVIL; }
        }

        public bool flan {
            get { return flags.HasFlag(SpeciesEffectiveness.FLAN); }
            set { if (value) flags |= SpeciesEffectiveness.FLAN; else flags &= ~SpeciesEffectiveness.FLAN; }
        }

        public bool wolf {
            get { return flags.HasFlag(SpeciesEffectiveness.WOLF); }
            set { if (value) flags |= SpeciesEffectiveness.WOLF; else flags &= ~SpeciesEffectiveness.WOLF; }
        }

        public bool wing {
            get { return flags.HasFlag(SpeciesEffectiveness.WING); }
            set { if (value) flags |= SpeciesEffectiveness.WING; else flags &= ~SpeciesEffectiveness.WING; }
        }

        public bool helm {
            get { return flags.HasFlag(SpeciesEffectiveness.HELM); }
            set { if (value) flags |= SpeciesEffectiveness.HELM; else flags &= ~SpeciesEffectiveness.HELM; }
        }
    }
}
