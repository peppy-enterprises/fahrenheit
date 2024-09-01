using System;
using System.Collections.Generic;

namespace FFXCutsceneRemover.Resources;

/* This class contains most of the transitions. Transitions added here are automatically evalutated in the main loop. */
static class Transitions
{
    static readonly AmmesTransition AmmesTransition = new AmmesTransition { ForceLoad = false, Description = "Sinspawn Ammes", Suspendable = false, Repeatable = true };
    static readonly DiveTransition DiveTransition = new DiveTransition { ForceLoad = false, Description = "Tidus falls into water", Suspendable = false, Repeatable = true };
    static readonly GeosTransition GeosTransition = new GeosTransition { ForceLoad = false, Description = "Geosgaeno", Suspendable = false, Repeatable = true };
    static readonly SinFinTransition SinFinTransition = new SinFinTransition { ForceLoad = false, Description = "Pre Sin Fin", Suspendable = false, Repeatable = true };
    static readonly EchuillesTransition EchuillesTransition = new EchuillesTransition { ForceLoad = false, Description = "Echuilles", Suspendable = false, Repeatable = true };
    static readonly GuiTransition GuiTransition = new GuiTransition { ForceLoad = false, Description = "Sinspawn Gui", Suspendable = false, Repeatable = true };
    static readonly FarplaneTransition FarplaneTransition = new FarplaneTransition { ForceLoad = false, Description = "Farplane", Suspendable = false, Repeatable = true };
    static readonly SeymourTransition SeymourTransition = new SeymourTransition { ForceLoad = false, Description = "Pre-Seymour", FormationSwitch = Transition.formations.PreSeymour, Suspendable = false, Repeatable = true };
    static readonly UnderLakeTransition UnderLakeTransition = new UnderLakeTransition { ForceLoad = false, Description = "Under Macalania Lake", Suspendable = false, Repeatable = true };
    static readonly BikanelTransition BikanelTransition = new BikanelTransition { ForceLoad = false, Description = "Bikanel Desert", Suspendable = false, Repeatable = true };
    static readonly HomeTransition HomeTransition = new HomeTransition { ForceLoad = false, Description = "Home Fights", Suspendable = false, Repeatable = true };

    private static byte[] BlitzballBytes = new byte[]
    {
        0x08, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x01, 0x80, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x80, 0x00, 0x20, 0x00, 0x80, 0x04, 0x00, 0x00, 0x00,
        0x04, 0x00, 0x00, 0x00, 0x04, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x80, 0x00, 0x20, 0x08, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01,
        0x82, 0x20, 0x00, 0x00, 0x82, 0x61, 0x00, 0x00, 0x02, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x08, 0x80, 0x00, 0x00, 0x08, 0x80, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x40, 0x20, 0x00, 0x00,
        0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x12, 0x00, 0x00, 0x00, 0x30, 0x00, 0x00, 0x40, 0x12, 0x00, 0x00, 0x40, 0x12, 0x04, 0x00, 0x00, 0x00, 0x24, 0x00, 0x40, 0x12, 0x00, 0x00, 0x80, 0x00, 0x00, 0x10, 0x01, 0x02, 0x00,
        0x00, 0x00, 0x10, 0x2C, 0x00, 0x00, 0x06, 0x04, 0x08, 0x0B, 0x00, 0x00, 0x40, 0x00, 0x80, 0x03, 0x20, 0x08, 0x08, 0x00, 0x00, 0x40, 0x02, 0x00, 0x00, 0x04, 0x80, 0x00, 0x00, 0x40, 0x18, 0x00, 0x00, 0x00, 0x00, 0x24, 0x00, 0x00, 0x00,
        0x24, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x20, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x04, 0x00, 0x40, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x42, 0x00, 0x00, 0x50, 0x10, 0x00, 0x00, 0x00,
        0x01, 0x00, 0x20, 0x49, 0x92, 0x24, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x40, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x08, 0x00, 0x04, 0x30, 0x08, 0x04, 0x00, 0x30, 0x00,
        0x00, 0x00, 0x08, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x04, 0x00, 0x00, 0x40, 0x04, 0x00, 0x04, 0x40, 0x10,
        0x00, 0x0C, 0x00, 0x20, 0x00, 0x2C, 0x40, 0x00, 0x00, 0x00, 0x54, 0x09, 0x00, 0x00, 0x54, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x10, 0x00, 0x04, 0x00, 0x20, 0x08, 0x00, 0x00, 0x08, 0x20, 0x00, 0x01,
        0x40, 0x02, 0x60, 0x00, 0x00, 0x18, 0x26, 0x00, 0x10, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x01, 0xE4, 0x54, 0x21, 0x00, 0x00, 0x08, 0x00, 0x08, 0x00, 0x41, 0x80, 0x00, 0x04,
        0x00, 0x10, 0x04, 0x10, 0xC0, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x2C, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x08, 0x00, 0x84, 0x08, 0x00, 0x00, 0xC0, 0x00, 0x00,
        0x04, 0x08, 0x00, 0x00, 0x00, 0x02, 0x10, 0x00, 0x24, 0x55, 0x01, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x01, 0x80, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x20, 0x49, 0x92,
        0x24, 0x20, 0x49, 0x92, 0x24, 0x20, 0x00, 0x80, 0x00, 0x20, 0x00, 0x80, 0x04, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x04, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x49, 0x92, 0x24, 0x20, 0x49, 0x92, 0x24, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x24, 0x55, 0x01, 0x00, 0x24, 0x55, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00,
        0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x24, 0x55, 0x01, 0x00, 0x24, 0x55, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1A, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x00, 0x00, 0x00, 0x1A, 0x28, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x11, 0x00, 0x00, 0x00, 0x00, 0x1D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x17, 0x00,
        0x00, 0x00, 0x00, 0x0B, 0x00, 0x00, 0x00, 0x00, 0x15, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x31, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x24, 0x00, 0x00, 0x00, 0x00, 0x1F, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x25, 0x00, 0x00, 0x00, 0x00, 0x17, 0x0A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x01, 0x01, 0x02, 0x00, 0x00, 0x01, 0x01, 0x01,
        0x00, 0x00, 0x00, 0x01, 0x01, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x01, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02,
        0x03, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x03, 0x01, 0x02, 0x01, 0x03, 0x03, 0x03, 0x07, 0x02, 0x01, 0x03, 0x03, 0x03, 0x01, 0x01, 0x01, 0x03, 0x03, 0x03, 0x01, 0x01, 0x03, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x05, 0x05, 0x03,
        0x03, 0x01, 0x01, 0x01, 0x05, 0x07, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x04, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x07, 0x08, 0x09,
        0x0A, 0x0B, 0x0C, 0x3C, 0x3C, 0x0D, 0x0E, 0x0F, 0x10, 0x11, 0x12, 0x3C, 0x3C, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x3C, 0x3C, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x3C, 0x3C, 0x1F, 0x20, 0x21, 0x22, 0x23, 0x24, 0x3C, 0x3C, 0x00, 0x02,
        0x03, 0x04, 0x05, 0x06, 0x3C, 0x3C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x5C, 0x5C, 0x5C,
        0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C,
        0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C,
        0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0xFF, 0xFF, 0x0F, 0x0A,
        0x08, 0x07, 0x10, 0x0A, 0x11, 0x14, 0x12, 0x13, 0x16, 0x16, 0x1E, 0x08, 0x07, 0x0F, 0x12, 0x0A, 0x04, 0x09, 0x0A, 0x08, 0x18, 0x1E, 0x20, 0x23, 0x1C, 0x27, 0x1E, 0x0F, 0x05, 0x0E, 0x0A, 0x06, 0x13, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0B, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x13, 0x00,
        0x00, 0x00, 0x0A, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x0D, 0x00, 0x13, 0x00, 0x38, 0x00, 0x07, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x0D, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0D, 0x00, 0x0D, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x21, 0x00, 0x0D, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x39, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x16, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE8, 0x03, 0x01, 0x00, 0x1E, 0x00,
        0x28, 0x00, 0x0A, 0x00, 0x32, 0x00, 0x3C, 0x00, 0x46, 0x00, 0x78, 0x00, 0xC8, 0x00, 0x64, 0x00, 0x6E, 0x00, 0x0A, 0x00, 0x46, 0x00, 0x78, 0x00, 0x32, 0x00, 0x32, 0x00, 0x5A, 0x00, 0x96, 0x00, 0xB4, 0x00, 0x82, 0x00, 0x1E, 0x00, 0x32,
        0x00, 0x14, 0x00, 0x64, 0x00, 0xAC, 0x0D, 0xC8, 0x00, 0x96, 0x00, 0x8A, 0x02, 0xC2, 0x01, 0x2C, 0x01, 0xE8, 0x03, 0x5A, 0x00, 0x64, 0x00, 0x78, 0x00, 0x0A, 0x00, 0x50, 0x00, 0x64, 0x00, 0x40, 0x01, 0x2C, 0x01, 0x50, 0x00, 0x3C, 0x00,
        0x64, 0x00, 0xA0, 0x00, 0xC8, 0x00, 0x84, 0x03, 0x96, 0x00, 0x96, 0x00, 0x54, 0x01, 0x90, 0x01, 0xC8, 0x00, 0x82, 0x00, 0x40, 0x01, 0xBE, 0x00, 0x96, 0x00, 0x64, 0x00, 0xF4, 0x01, 0xD2, 0x00, 0x58, 0x02, 0xC8, 0x00
    };

    public static readonly Dictionary<Func<bool>, Transition> StandardTransitions = new Dictionary<Func<bool>, Transition>()
        {
        // SPECIAL
#if DEBUG
        {
            () => { return MemoryWatchers.Input.Current == 2304 && MemoryWatchers.BattleState.Current == 10; },
            new Transition { BattleState = 778, ForceLoad = false, Description = "End any battle by holding start + select"}
        },
#endif
        {
            () => { return MemoryWatchers.RoomNumber.Current == 348; },
            new Transition { RoomNumber = 23, Description = "Skip Intro", Repeatable = true}
        },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 349; },
            new Transition { RoomNumber = 23, Description = "Skip Intro", Repeatable = true}
        },
        // START OF ZANARKAND
        {
            () => { return MemoryWatchers.RoomNumber.Current == 132 && MemoryWatchers.Storyline.Current == 0; },
            new Transition { RoomNumber = 368, Storyline = 3, SpawnPoint = 0, Description = "Beginning"}
        },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 368 && MemoryWatchers.Storyline.Current == 3 && MemoryWatchers.FangirlsOrKidsSkip.Current == 0 && MemoryWatchers.NPCLastInteraction.Current == 29; },
            new Transition { MenuTriggerValue = 0x40080000, ForceLoad = false, Description = "Naming Tidus"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 368 && MemoryWatchers.Storyline.Current == 3 && MemoryWatchers.FangirlsOrKidsSkip.Current == 0 && MemoryWatchers.NPCLastInteraction.Current == 30; },
            new Transition { MenuTriggerValue = 0x40080000, ForceLoad = false, Description = "Naming Tidus"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 368 && MemoryWatchers.Storyline.Current == 3 && MemoryWatchers.NPCLastInteraction.Current == 29 && MemoryWatchers.Menu.Current == 1 && MemoryWatchers.MenuValue1.Current == 0x4000; },
            new Transition {FangirlsOrKidsSkip = 1, PositionTidusAfterLoad = true, Target_x = 15.0f, Target_y = -14.960f, Target_z = 12.0f, Target_var1 = 127, Description = "Kids"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 368 && MemoryWatchers.Storyline.Current == 3 && MemoryWatchers.NPCLastInteraction.Current == 30 && MemoryWatchers.Menu.Current == 1 && MemoryWatchers.MenuValue1.Current == 0x4000; },
            new Transition {FangirlsOrKidsSkip = 2, PositionTidusAfterLoad = true, Target_x = 15.0f, Target_y = -14.960f, Target_z = -8.0f, Target_var1 = 123, Description = "Fangirls"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 368 && MemoryWatchers.Storyline.Current == 3 && MemoryWatchers.FangirlsOrKidsSkip.Current == 2 && MemoryWatchers.NPCLastInteraction.Current == 29; },
            new Transition {FangirlsOrKidsSkip = 3, PositionTidusAfterLoad = true, Target_x = 15.0f, Target_y = -14.960f, Target_z = 12.0f, Target_var1 = 127, Description = "Kids"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 368 && MemoryWatchers.Storyline.Current == 3 && MemoryWatchers.FangirlsOrKidsSkip.Current == 1 && MemoryWatchers.NPCLastInteraction.Current == 30; },
            new Transition {FangirlsOrKidsSkip = 3, PositionTidusAfterLoad = true, Target_x = 15.0f, Target_y = -14.960f, Target_z = -8.0f, Target_var1 = 123, Description = "Fangirls"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 368 && MemoryWatchers.Storyline.Current == 3 && MemoryWatchers.FangirlsOrKidsSkip.Current == 1 && MemoryWatchers.NPCLastInteraction.Current == 8; },
            new Transition {FangirlsOrKidsSkip = 3, PositionTidusAfterLoad = true, Target_x = 15.0f, Target_y = -14.960f, Target_z = -8.0f, Target_var1 = 123, Description = "Fangirls"} },
        //{ () => { return MemoryWatchers.RoomNumber.Current == 368 && MemoryWatchers.Storyline.Current == 4; },
        //  new Transition { RoomNumber = 376, Storyline = 4, SpawnPoint = 0, Description = "Tidus leaves fans"} }, // Removed and replaced with custom check in cutsceneremover.cs
        {
            () => { return MemoryWatchers.RoomNumber.Current == 376 && MemoryWatchers.Storyline.Current == 4; },
            new Transition { RoomNumber = 376, Storyline = 5, SpawnPoint = 0, Description = "Tidus looks at Jecht sign"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 371 && MemoryWatchers.Storyline.Current == 6; },
            new Transition { RoomNumber = 370, Storyline = 7, SpawnPoint = 0, Description = "Blitzball FMV"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 370 && MemoryWatchers.Storyline.Current == 7; },
            new AuronTransition {ForceLoad = false, Description = "Tidus wakes up and follows Auron", Suspendable = false, Repeatable = true} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 366 && MemoryWatchers.Storyline.Current == 8; },
            new Transition { RoomNumber = 389, Storyline = 12, SpawnPoint = 0, Description = "Tidus sees the fayth and follows Auron" } },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 389 && MemoryWatchers.Storyline.Current == 13; },
            AmmesTransition },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 389 && MemoryWatchers.Storyline.Current == 14; },
            AmmesTransition },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 389 && MemoryWatchers.Storyline.Current == 15; },
            AmmesTransition },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 389 && MemoryWatchers.Storyline.Current == 16; },
            AmmesTransition },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 367 && MemoryWatchers.Storyline.Current == 16; },
            new Transition { RoomNumber = 367, Storyline = 18, SpawnPoint = 0, Description = "Tidus sees Jecht sign again" } },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 367 && MemoryWatchers.Storyline.Current == 18; },
            new TankerTransition {ForceLoad = false, Description = "Tanker", Suspendable = false, Repeatable = true} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 367 && MemoryWatchers.Storyline.Current == 19; },
            new Transition { RoomNumber = 367, Storyline = 20, SpawnPoint = 0, Description = "Tidus and Auron run as bridge explodes"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 367 && MemoryWatchers.Storyline.Current == 20; },
            new Transition { RoomNumber = 384, Storyline = 20, SpawnPoint = 0, Description = "This is your story FMV" } },
        //{ () => { return MemoryWatchers.RoomNumber.Current == 384 && MemoryWatchers.Storyline.Current == 20; },
        //  new InsideSinTransition {ForceLoad = false, Description = "Inside Sin", Suspendable = false, Repeatable = true} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 384 && MemoryWatchers.Storyline.Current == 20 && MemoryWatchers.Dialogue1.Current == 3; },
            new Transition { ForceLoad = false, MenuValue1 = 0, MenuValue2 = 0, Description = "Fix menu bug", ConsoleOutput = false} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 384 && MemoryWatchers.Storyline.Current == 20 && MemoryWatchers.State.Current == 0; },
            new Transition { RoomNumber = 385, Description = "Tidus swimming looking at himself as a child"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 385 && MemoryWatchers.Storyline.Current == 20; },
            new Transition { RoomNumber = 48, Storyline = 30, SpawnPoint = 0, Description = "A dream of being alone" } },
        // END OF ZANARKAND
        // START OF BAAJ TEMPLE
        {
            () => { return MemoryWatchers.RoomNumber.Current == 48 && MemoryWatchers.Storyline.Current == 30; },
            new Transition { RoomNumber = 48, Storyline = 42, SpawnPoint = 1, PositionTidusAfterLoad = true, Target_x = 116.063f, Target_y = -92.497f, Target_z = -448.592f, Target_var1 = 43, Description = "Tidus wakes up"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 48 && MemoryWatchers.Storyline.Current == 42; },
            new Transition { ForceLoad = false, TargetActorIDs = new short[] { 0x01 }, Target_x = 57.642f, Target_z = -480.248f, Target_var1 = 810, ConsoleOutput = false, Description = "Tidus wakes up - Reposition"} },
        //{ () => { return MemoryWatchers.RoomNumber.Current == 49 && MemoryWatchers.Storyline.Current == 42; },
        //  DiveTransition }, // Currently doesn't work because Tidus is unable to move after the skip. Need a way to change movement lock value.
        //{ () => { return MemoryWatchers.RoomNumber.Current == 49 && MemoryWatchers.Storyline.Current == 44; },
        //  DiveTransition }, //
        {
            () => { return MemoryWatchers.RoomNumber.Current == 49 && MemoryWatchers.Storyline.Current == 44; },
            GeosTransition },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 49 && MemoryWatchers.Storyline.Current == 46; },
            GeosTransition },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 49 && MemoryWatchers.Storyline.Current == 48; },
            new Transition { RoomNumber = 50, Storyline = 48, SpawnPoint = 0, Description = "Escape from Geogaesno"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 50 && MemoryWatchers.Storyline.Current == 48; },
            new Transition { RoomNumber = 50, Storyline = 50, SpawnPoint = 0, Description = "Tidus in a collapsed corridor"} }, // Bug: Boss music still playing
        {
            () => { return MemoryWatchers.RoomNumber.Current == 63 && MemoryWatchers.Storyline.Current == 50; },
            new Transition { RoomNumber = 63, Storyline = 52, SpawnPoint = 1, PositionTidusAfterLoad = true, Target_x = -48.996f, Target_y = 0.0f, Target_z = -0.585f, Target_rot = -0.004f, Target_var1 = 127, Description = "Tidus needs fire"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 63 && MemoryWatchers.Storyline.Current == 54; },
            new Transition { RoomNumber = 63, Storyline = 55, SpawnPoint = 0, Description = "Tidus makes fire"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 165 && MemoryWatchers.Storyline.Current == 55; },
            new Transition { RoomNumber = 63, Storyline = 55, SpawnPoint = 0, Description = "Tidus has a dream about Auron"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 63 && MemoryWatchers.Storyline.Current == 55; },
            new KlikkTransition {ForceLoad = false, Description = "Klikk", Suspendable = false, Repeatable = true} },
        //{ () => { return MemoryWatchers.RoomNumber.Current == 63 && MemoryWatchers.Storyline.Current == 58; },
        //  new Transition { RoomNumber = 71, Storyline = 60, SpawnPoint = 0, Description = "Rikku punches Tidus"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 71 && MemoryWatchers.Storyline.Current == 60; },
            new Transition { RoomNumber = 71, Storyline = 66, SpawnPoint = 0, BaajFlag1 = 1, PositionTidusAfterLoad = true, Target_x = -34.561f, Target_y = 0.0f, Target_z = -34.231f, Target_rot = 2.268f, Target_var1 = 180, Description = "Tidus wakes up on boat + Sphere Grid tutorial"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 71 && MemoryWatchers.Storyline.Current == 66; },
            new AlBhedBoatTransition {ForceLoad = false, Description = "Rikku explains the mission", Suspendable = false, Repeatable = true} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 64 && MemoryWatchers.Storyline.Current == 70; },
            new Transition { RoomNumber = 64, Storyline = 74, SpawnPoint = 0, Description = "They enter the submerged ruins" } },
                                        // Tidus bashes the console
        {
            () => { return MemoryWatchers.RoomNumber.Current == 64 && MemoryWatchers.Storyline.Current == 76; },
            new UnderwaterRuinsTransition {ForceLoad = false, Description = "Underwater Ruins", Suspendable = false, Repeatable = true} }, //Needs work
                                        // Tidus bashes the machine + Tros arrives
                                        // They leave the submerged ruins
        {
            () => { return MemoryWatchers.RoomNumber.Current == 380 && MemoryWatchers.Storyline.Current == 84; },
            new UnderwaterRuinsTransition2 {ForceLoad = false, Description = "Underwater Ruins Outside", Suspendable = false, Repeatable = true} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 380 && MemoryWatchers.Storyline.Current == 84 && MemoryWatchers.State.Current == 0; },
            new Transition { RoomNumber = 71, Storyline = 90, SpawnPoint = 0, Description = "Airship is shown" } },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 71 && MemoryWatchers.Storyline.Current == 90 && MemoryWatchers.State.Current == 1; },
            new Transition { RoomNumber = 71, Storyline = 100, SpawnPoint = 0, PositionTidusAfterLoad = true, Target_x = -40.772f, Target_y = 0.0f, Target_z = -20.171f, Target_rot = 0.0f, Target_var1 = 180, Description = "Tidus gets back onto the boat"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 71 && MemoryWatchers.Storyline.Current == 100; },
            new RikkuNameTransition { ForceLoad = false, ConsoleOutput = false } },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 71 && MemoryWatchers.Storyline.Current == 100 && MemoryWatchers.State.Current == 1; },
            new Transition { RoomNumber = 70, Storyline = 110, Description = "Rikku suggests going to Luca"} },
        // END OF BAAJ TEMPLE
        // START OF BESAID
        {
            () => { return MemoryWatchers.RoomNumber.Current == 70 && MemoryWatchers.Storyline.Current == 111; },
            new Transition { Storyline = 116, SpawnPoint = 0, PositionTidusAfterLoad = true, Target_x = 64.785f, Target_y = -5.037f, Target_z = -73.665f, Target_var1 = 86, Description = "Tidus wakes up in the sea"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 70 && MemoryWatchers.Storyline.Current == 116; },
            new Transition { ForceLoad = false, PositionTidusAfterLoad = true, Target_x = -184.754f, Target_y = 26.5f, Target_z = -46.699f, Target_rot = -1.821f, Target_var1 = 393, ConsoleOutput = false, Description = "Tidus wakes up in the sea - Reposition"} },
        //{ () => { return MemoryWatchers.RoomNumber.Current == 41 && MemoryWatchers.Storyline.Current == 119; },
        //  new LagoonTransition {ForceLoad = false, Description = "Lagoon", Suspendable = false, Repeatable = true} }, // Causes menu to be unable to open and menu crashing
        {
            () => { return MemoryWatchers.RoomNumber.Current == 41 && MemoryWatchers.Storyline.Current == 119 && MemoryWatchers.CutsceneAlt.Current == 73; },
            new Transition { RoomNumber = 67, Storyline = 124, Description = "Wakka asks Tidus to join his team"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 67 && MemoryWatchers.Storyline.Current == 124; },
            new Transition { RoomNumber = 69, Storyline = 130, SpawnPoint = 0, PositionTidusAfterLoad = true, Target_x = 116.225f, Target_y = -94.288f, Target_z = 34.441f, Target_rot = -0.281f, Target_var1 = 591, Description = "Wakka explains his life story"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 133 && MemoryWatchers.Storyline.Current == 130; },
            new Transition { RoomNumber = 17, Storyline = 134, SpawnPoint = 3, PositionTidusAfterLoad = true, Target_x = -18.200f, Target_y = -9.553f, Target_z = 462.0f, Target_rot = -1.570f, Target_var1 = 714, Description = "Tidus arrives at Besaid Village" } },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 84 && MemoryWatchers.Storyline.Current == 134; },
            new Transition { RoomNumber = 84, Storyline = 136, SpawnPoint = 0, PositionTidusAfterLoad = true, Target_x = -7.442f, Target_y = 0.0f, Target_z = -56.783f, Target_rot = 1.570f, Target_var1 = 229, Description = "Tidus enters the temple"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 84 && MemoryWatchers.Storyline.Current == 136 && MemoryWatchers.State.Current == 1; },
            new Transition { RoomNumber = 42, Storyline = 140, PositionTidusAfterLoad = true, Target_x = 38.068f, Target_y = 0.0f, Target_z = 27.899f, Target_rot = 0.687f, Target_var1 = 247, Description = "Tidus speaks to the priest" } },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 145 && MemoryWatchers.Storyline.Current == 140; },
            new WakkaTentTransition { ForceLoad = false, Description = "Priest enters Wakka's tent", Suspendable = false, Repeatable = true } },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 191 && MemoryWatchers.Storyline.Current == 152; },
            new Transition { RoomNumber = 145, Storyline = 154, SpawnPoint = 0, Description = "Tidus dreams about a flashback"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 42 && MemoryWatchers.Storyline.Current == 154 && MemoryWatchers.State.Current == 1; },
            new Transition { RoomNumber = 122, Storyline = 162, Description = "Tidus goes back into the temple"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 122 && MemoryWatchers.Storyline.Current == 162 && MemoryWatchers.State.Current == 1 && MemoryWatchers.BesaidFlag1.Current == 8; },
            new Transition { RoomNumber = 103, Storyline = 164, Description = "Wakka catches up with Tidus in trials"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 103 && MemoryWatchers.Storyline.Current == 164; },
            new Transition { RoomNumber = 42, Storyline = 170, EnableValefor = 17, Description = "Tidus meets Lulu and Kimahri + FMV "} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 42 && MemoryWatchers.Storyline.Current == 170; },
            new Transition { RoomNumber = 42, Storyline = 172, SpawnPoint = 0, PositionTidusAfterLoad = true, Target_x = -0.000f, Target_y = 0.0f, Target_z = 57.367f, Target_rot = -1.570f, Target_var1 = 170, Description = "The gang leave the cloister of trials"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 83 && MemoryWatchers.Storyline.Current == 172 && MemoryWatchers.State.Current == 1; },
            new ValeforTransition {ForceLoad = false, Description = "Naming Valefor", Suspendable = false, Repeatable = true} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 68 && MemoryWatchers.Storyline.Current == 184; },
            new Transition { RoomNumber = 252, Storyline = 190, Description = "Tidus sleeping"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 252 && MemoryWatchers.Storyline.Current == 190 && MemoryWatchers.State.Current == 1; },
            new Transition { RoomNumber = 60, Storyline = 196, Description = "Tidus has a dream about Yuna, Tidus wakes up + FMV" } },
                                        // Tidus wakes up again (Party healed at this point)
        {
            () => { return MemoryWatchers.RoomNumber.Current == 17 && MemoryWatchers.Storyline.Current == 200; },
            new BrotherhoodTransition { RoomNumber = 69, Storyline = 210, SpawnPoint = 3, EnableYuna = 17, EnableLulu = 17, TidusWeaponDamageBoost = 5, Description = "Yuna says goodbye to Besaid" } },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 67 && MemoryWatchers.Storyline.Current == 210; },
            new Transition { RoomNumber = 67, Storyline = 214, SpawnPoint = 3, Description = "Yuna says goodbye to Besaid again"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 21 && MemoryWatchers.Storyline.Current == 214; },
            new KimahriTransition {ForceLoad = false, Description = "Kimahri", FormationSwitch = Transition.formations.PreKimahri, Suspendable = false, Repeatable = true} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 21 && MemoryWatchers.Storyline.Current == 216; },
            new Transition {ForceLoad = false, Description = "Post-Kimahri", FormationSwitch = Transition.formations.PostKimahri} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 19 && MemoryWatchers.Storyline.Current == 218 && MemoryWatchers.State.Current == 0; },
            new Transition { RoomNumber = 301, Storyline = 220, FormationSwitch = Transition.formations.BoardingSSLiki, Description = "S.S. Liki departs" } },
        // END OF BESAID
        // START OF SS LIKI
        {
            () => { return MemoryWatchers.RoomNumber.Current == 301 && MemoryWatchers.Storyline.Current == 220; },
            new Transition { RoomNumber = 301, Storyline = 228, SpawnPoint = 0, SSWinnoFlag1 = 0x56, PositionTidusAfterLoad = true, Target_x = 44.0f, Target_y = -49.997f, Target_z = 92.599f, Target_rot = 2.722f, Target_var1 = 120, Description = "Tidus goofing around" } },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 301 && MemoryWatchers.Storyline.Current == 240 && MemoryWatchers.SSWinnoFlag1.Current == 0x5E; },
            new Transition { RoomNumber = 301, SpawnPoint = 0, PositionTidusAfterLoad = true, Target_x = 3.932f, Target_y = -49.997f, Target_z = 189.901f, Target_rot = 1.676f, Target_var1 = 94, Description = "Lord Braska's daughter?" } },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 301 && MemoryWatchers.Storyline.Current == 240 && MemoryWatchers.SSWinnoFlag1.Current == 0x7E; },
            new Transition { RoomNumber = 301, Storyline = 242, SpawnPoint = 0, PositionTidusAfterLoad = true, Target_x = -29.5f, Target_y = -49.997f, Target_z = 86.0f, Target_rot = -0.241f, Target_var1 = 41, Description = "She's the daughter of High Summoner Braska" } },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 61 && MemoryWatchers.Storyline.Current == 244 && MemoryWatchers.SSWinnoFlag1.Current == 0xFE; },
            new Transition {Storyline = 248, Description = "Tidus talks to Yuna" } },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 61 && MemoryWatchers.Storyline.Current == 248; },
            SinFinTransition},
        {
            () => { return MemoryWatchers.RoomNumber.Current == 61 && MemoryWatchers.Storyline.Current == 260; },
            SinFinTransition},
        {
            () => { return MemoryWatchers.RoomNumber.Current == 282 && MemoryWatchers.Storyline.Current == 272; },
            EchuillesTransition },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 282 && MemoryWatchers.Storyline.Current == 280; },
            EchuillesTransition },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 220 && MemoryWatchers.Storyline.Current == 287; },
            new Transition { RoomNumber = 139, Storyline = 290, KilikaMapFlag = 0x01, Description = "Recovering on the boat"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 139 && MemoryWatchers.Storyline.Current == 290; },
            new Transition { RoomNumber = 43, Storyline = 292, Description = "Map shown"} },
        // END OF SS LIKI
        // START OF KILIKA
        {
            () => { return MemoryWatchers.RoomNumber.Current == 43 && MemoryWatchers.Storyline.Current == 292; },
            new Transition { RoomNumber = 43, Storyline = 294, SpawnPoint = 0, FormationSwitch = Transition.formations.PostEchuilles, Description = "Undocking in Kilika" } },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 53 && MemoryWatchers.Storyline.Current == 294 && MemoryWatchers.State.Current == 0; },
            new Transition { RoomNumber = 152, Storyline = 300, Description = "Sending" } },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 152 && MemoryWatchers.Storyline.Current == 300; },
            new Transition { RoomNumber = 152, Storyline = 304, SpawnPoint = 0, FullHeal = true, Description = "Tidus wakes up" } },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 16 && MemoryWatchers.Storyline.Current == 304 && MemoryWatchers.State.Current == 1; },
            new Transition { Storyline = 308, SpawnPoint = 2, PositionTidusAfterLoad = true, Target_x = -39.442f, Target_y = -17.898f, Target_z = -206.446f, Target_rot = 1.618f, Target_var1 = 2, Description = "Tidus speaks to Wakka"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 18 && MemoryWatchers.Storyline.Current == 308; },
            new Transition { RoomNumber = 18, Storyline = 312, SpawnPoint = 0, Description = "Camera pan + Yuna wants a new guardian" } },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 65 && MemoryWatchers.Storyline.Current == 315; },
            new Transition { RoomNumber = 65, Storyline = 322, SpawnPoint = 0, Description = "Race up the stairs"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 65 && MemoryWatchers.Storyline.Current == 322; },
            new GeneauxTransition { ForceLoad = false, Description = "Pre-Geneaux", Suspendable = false, Repeatable = true} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 65 && MemoryWatchers.Storyline.Current == 326 && MemoryWatchers.State.Current == 1; },
            new Transition { RoomNumber = 78, Storyline = 328, SpawnPoint = 1, Description = "No replacement for Chappu"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 78 && MemoryWatchers.Storyline.Current == 328; },
            new Transition { RoomNumber = 78, Storyline = 330, SpawnPoint = 1, PositionTidusAfterLoad = true, Target_x = -0.361f, Target_y = 5.801f, Target_z = -91.049f, Target_rot = 1.558f, Target_var1 = 287, Description = "Arrival at temple"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 96 && MemoryWatchers.Storyline.Current == 330; },
            new Transition { RoomNumber = 96, Storyline = 333, SpawnPoint = 0, Description = "Camera pan in Kilika Temple" } },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 96 && MemoryWatchers.Storyline.Current == 333; },
            new KilikaPrayTransition { ForceLoad = false, Description = "Pray or Stand", Suspendable = false, Repeatable = true } },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 44 && MemoryWatchers.Storyline.Current == 335; },
            new Transition { RoomNumber = 108, Storyline = 340, SpawnPoint = 0, Description = "Tidus is denied access" } },
        //{ () => { return MemoryWatchers.RoomNumber.Current == 108 && MemoryWatchers.Storyline.Current == 340; },
        //  new KilikaTrialsTransition {ForceLoad = false, ConsoleOutput = false, Description = "Camera Pan", Suspendable = false, Repeatable = true} }, // Camera pan inside the trials -Doesn't work yet
        {
            () => { return MemoryWatchers.RoomNumber.Current == 45 && MemoryWatchers.Storyline.Current == 340; },
            new KilikaAntechamberTransition { ForceLoad = false, Description = "Guardians are annoyed at Tidus", Suspendable = false, Repeatable = true }  },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 45 && MemoryWatchers.Storyline.Current == 344 && MemoryWatchers.State.Current == 1; },
            new Transition { Storyline = 346, SpawnPoint = 1, PositionTidusAfterLoad = true, Target_x = -12.839f, Target_y = 0.0f, Target_z = -5.327f, Target_var1 = 31, Description = "Fayth explanation"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 45 && MemoryWatchers.Storyline.Current == 346 && MemoryWatchers.State.Current == 1; },
            new Transition { MenuTriggerValue = 0x40080009, EnableIfrit = 0x11, ForceLoad = false, Description = "Naming Ifrit"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 45 && MemoryWatchers.Storyline.Current == 346 && MemoryWatchers.Menu.Current == 1 && MemoryWatchers.MenuValue1.Current == 0x4000; },
            new Transition { RoomNumber = 78, Storyline = 348, Description = "Exit Temple"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 78 && MemoryWatchers.Storyline.Current == 348 && MemoryWatchers.State.Current == 1; },
            new Transition { RoomNumber = 18, Storyline = 360, SpawnPoint = 1, Description = "Tidus misses home" } },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 16 && MemoryWatchers.Storyline.Current == 360 && MemoryWatchers.State.Current == 1; },
            new Transition { RoomNumber = 94, Storyline = 370, SpawnPoint = 0, Description = "Setting off to Luca"} },
        // END OF KILIKA
        // START OF SS WINNO
        {
            () => { return MemoryWatchers.RoomNumber.Current == 94 && MemoryWatchers.Storyline.Current == 370; },
            new Transition { RoomNumber = 167, Storyline = 372, SSWinnoFlag2 = 1, Description = "Opening scenes"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 237 && MemoryWatchers.Storyline.Current == 372 && MemoryWatchers.SSWinnoFlag2.Current == 1; },
            new Transition { RoomNumber = 237, Storyline = 372, SSWinnoFlag1 = 170, SSWinnoFlag2 = 9, Description = "Meet O'aka"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 94 && MemoryWatchers.Storyline.Current == 380 && MemoryWatchers.SSWinnoFlag2.Current == 25; },
            new Transition { Storyline = 380, SSWinnoFlag2 = 31, SpawnPoint = 0, PositionTidusAfterLoad = true, Target_x = -33.610f, Target_y = -49.996f, Target_z = -67.556f, Target_rot = -3.135f, Target_var1 = 278, Description = "Eavesdropping on Lulu and Wakka"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 94 && MemoryWatchers.Storyline.Current == 385; },
            new Transition { RoomNumber = 191, SpawnPoint = 0, Description = "Tidus looks at the blitzball"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 191 && MemoryWatchers.Storyline.Current == 385; },
            new Transition { RoomNumber = 94, Storyline = 387, SpawnPoint = 0, Description = "Zanarkand flashback"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 94 && MemoryWatchers.Storyline.Current == 390; },
            new JechtShotTransition { ForceLoad = false, Description = "Jecht Shot Failed", Suspendable = false, Repeatable = true} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 94 && MemoryWatchers.Storyline.Current == 395 && MemoryWatchers.State.Current == 0; },
            new Transition { RoomNumber = 267, Storyline = 402, FullHeal = true, Description = "Tidus speaks to Yuna"} },
        // END OF WINNO
        // START OF LUCA
        {
            () => { return MemoryWatchers.RoomNumber.Current == 267 && MemoryWatchers.Storyline.Current == 402; },
            new Transition { RoomNumber = 377, Storyline = 404, Description = "Luca FMV + Kilika Beasts undock"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 377 && MemoryWatchers.Storyline.Current == 404; },
            new Transition { RoomNumber = 267, Storyline = 405, Description = "Inside the Cafe"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 267 && MemoryWatchers.Storyline.Current == 405; },
            new Transition { RoomNumber = 267, Storyline = 425, SpawnPoint = 2, PositionTidusAfterLoad = true, Target_x = 135.110f, Target_y = 0.348f, Target_z = 419.457f, Target_rot = 2.094f, Target_var1 = 307, Description = "Besaid Aurochs undock"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 268 && MemoryWatchers.Storyline.Current == 427 && MemoryWatchers.State.Current == 0; },
            new Transition { RoomNumber = 355, Storyline = 430, Description = "Seymour arrives" } },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 72 && MemoryWatchers.Storyline.Current == 430 && MemoryWatchers.State.Current == 0; },
            new Transition { Storyline = 440, SpawnPoint = 2, PositionTidusAfterLoad = true, Target_x = -0.896f, Target_y = -0.297f, Target_z = -2.731f, Target_rot = -1.504f, Target_var1 = 33, Description = "Yuna enters the changing room"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 72 && MemoryWatchers.Storyline.Current == 440 && MemoryWatchers.State.Current == 0; },
            new Transition { RoomNumber = 123, Storyline = 450, SpawnPoint = 4, Description = "Speaking to the Al Bhed"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 123 && MemoryWatchers.Storyline.Current == 450 && MemoryWatchers.LucaFlag.Current == 0; },
            new Transition { LucaFlag = 8, ForceLoad = false, Description = "Camera pan"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 77 && MemoryWatchers.Storyline.Current == 450; },
            new Transition { Storyline = 455, SpawnPoint = 1, PositionTidusAfterLoad = true, Target_x = 74.378f, Target_y = -49.984f, Target_z = 10.625f, Target_rot = 0.0f, Target_var1 = 98, Description = "Crowd mob Yuna"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 77 && MemoryWatchers.Storyline.Current == 455; },
            new Transition { ForceLoad = false, TargetActorIDs = new short[] { 2 }, Target_x = 81.469f, Target_y = -49.984f, Target_z = 10.779f, Target_rot = 3.054f, Target_var1 = 98, ConsoleOutput = false, Description = "Crowd mob Yuna"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 104 && MemoryWatchers.Storyline.Current == 455 && MemoryWatchers.LucaFlag2.Current == 0; },
            new Transition { LucaFlag2 = 2, ForceLoad = false, PositionTidusAfterLoad = true, Target_x = 18.981f, Target_y = 4.984f, Target_z = -76.611f, Target_rot = 2.764f, Target_var1 = 299, Description = "Tidus and Yuna talk about Luca"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 104 && MemoryWatchers.Storyline.Current == 455 && MemoryWatchers.LucaFlag2.Current == 0; },
            new Transition { LucaFlag2 = 2, ForceLoad = false, TargetActorIDs = new short[] { 2 }, Target_x = 6.618f, Target_y = 4.576f, Target_z = -71.714f, Target_rot = -0.377f, Target_var1 = 301, Description = "Tidus and Yuna talk about Luca"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 159 && MemoryWatchers.Storyline.Current == 455; },
            new Transition { RoomNumber = 57, Storyline = 484, Description = "Tidus and Yuna at the cafe"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 57 && MemoryWatchers.Storyline.Current == 484; },
            new Transition { RoomNumber = 121, Storyline = 486, Description = "Mika begins the tournament"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 121 && MemoryWatchers.Storyline.Current == 486; },
            new Transition { RoomNumber = 159, Storyline = 488, Description = "Al Bhed Auroch game starts"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 159 && MemoryWatchers.Storyline.Current == 488; },
            new Transition { RoomNumber = 104, Storyline = 490, Description = "Kimahri Yuna's gone"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 104 && MemoryWatchers.Storyline.Current == 490; },
            new Transition { RoomNumber = 77, Storyline = 492, SpawnPoint = 1, FormationSwitch = Transition.formations.MachinaFights, Description = "Lulu explains the situation" } },
                                        // Machina fights
        {
            () => { return MemoryWatchers.RoomNumber.Current == 121 && MemoryWatchers.Storyline.Current == 492; },
            new Transition { RoomNumber = 88, Storyline = 500, LucaFlag = 9, SpawnPoint = 2, PositionTidusAfterLoad = true, Target_x = 35.235f, Target_y = 0.363f, Target_z = -312.158f, Target_rot = 0.523f, Target_var1 = 3, Description = "Wakka takes a beating"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 88 && MemoryWatchers.Storyline.Current == 500; },
            new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] {0x04, 0x06}, Target_x = 0, Target_y = 0, Target_z = 0} }, // Hide Kimahri and Lulu Models
        {
            () => { return MemoryWatchers.RoomNumber.Current == 299 && MemoryWatchers.Storyline.Current == 502; },
            new Transition { RoomNumber = 113, Storyline = 502, Description = "They jump on the boat"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 113 && MemoryWatchers.Storyline.Current == 502; },
            new OblitzeratorTransition {ForceLoad = false, Description = "Oblitzerator", Suspendable = false, Repeatable = true} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 121 && MemoryWatchers.Storyline.Current == 508; },
            new Transition { RoomNumber = 88, Storyline = 514, SpawnPoint = 0, PositionTidusAfterLoad = true, Target_x = 36.668f, Target_y = 0.363f, Target_z = -316.273f, Target_rot = -3.054f, Target_var1 = 3, Description = "Aurochs win the game" } },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 72 && MemoryWatchers.Storyline.Current == 514; },
            new Transition { Storyline = 518, LucaFlag = 11, SpawnPoint = 2, PositionTidusAfterLoad = true, Target_x = -4.458f, Target_y = -0.297f, Target_z = -11.090f, Target_rot = -1.396f, Target_var1 = 33, Description = "Wakka is injured"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 72 && MemoryWatchers.Storyline.Current == 518 && MemoryWatchers.State.Current == 0; },
            new Transition { Storyline = 520, Description = "Wakka subs himself"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 72 && MemoryWatchers.Storyline.Current == 520; },
            new Transition { RoomNumber = 124, Storyline = 535, Description = "Lulu speaks to Wakka" } },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 124 && MemoryWatchers.Storyline.Current == 535; },
            new Transition { RoomNumber = 62, Description = "Pre-Blitzball", AurochsTeamBytes = new byte[] { 0x06, 0x06, 0x06, 0x06, 0x06, 0x06 }, BlitzballBytes = BlitzballBytes} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 72 && MemoryWatchers.Storyline.Current == 540; },
            new Transition { RoomNumber = 347, Storyline = 560, Description = "Halftime talk"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 124 && MemoryWatchers.Storyline.Current == 560; },
            new Transition { RoomNumber = 250, Storyline = 565, Description = "Fans are getting impatient"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 124 && MemoryWatchers.Storyline.Current == 562; },
            new Transition { RoomNumber = 250, Storyline = 565, Description = "Fans are getting impatient"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 250 && MemoryWatchers.Storyline.Current == 565; },
            new Transition { RoomNumber = 124, Storyline = 575, Description = "Wakka chants"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 124 && MemoryWatchers.Storyline.Current == 575; },
            new BlitzballTransition2 {ForceLoad = false, Description = "Ah! It's Wakka!", Suspendable = false, Repeatable = true} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 250 && MemoryWatchers.Storyline.Current == 582; },
            new Transition { RoomNumber = 125, Storyline = 583, AurochsPlayer1 = 0, Description = "Aurochs win/lose the game"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 125 && MemoryWatchers.Storyline.Current == 583; },
            new SahaginTransition {ForceLoad = false, Description = "Pre-Sahagins", FormationSwitch = Transition.formations.PreSahagins, Suspendable = false, Repeatable = true} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 57 && MemoryWatchers.Storyline.Current == 588; },
            new Transition {Storyline = 600, Description = "Lulu what's happening"}},
        {
            () => { return MemoryWatchers.RoomNumber.Current == 57 && MemoryWatchers.Storyline.Current == 600; },
            new GarudaTransition {ForceLoad = false, Description = "Vouivre to Anima", Suspendable = false, Repeatable = true} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 104 && MemoryWatchers.Storyline.Current == 610; },
            new Transition { RoomNumber = 107, Storyline = 615, Description = "Wakka quits the Aurochs" } },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 107 && MemoryWatchers.Storyline.Current == 615; },
            new Transition { RoomNumber = 89, Storyline = 616, Description = "Wakka joins Yuna"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 89 && MemoryWatchers.Storyline.Current == 616; },
            new Transition { Storyline = 617, SpawnPoint = 1, FullHeal = true, FormationSwitch = Transition.formations.AuronJoinsTheParty, PositionTidusAfterLoad = true, Target_x = -454.963f, Target_y = 0.0f, Target_z = -321.963f, Target_rot = 0.610f, Target_var1 = 189, Description = "Tidus shouts at Auron"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 107 && MemoryWatchers.Storyline.Current == 617; },
            new Transition { Storyline = 630, SpawnPoint = 1, LucaFlag = 15, PositionTidusAfterLoad = true, Target_x = -84.944f, Target_y = -159.997f, Target_z = -3.714f, Target_rot = -2.635f, Target_var1 = 50, Description = "Tidus and Auron join the group"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 107 && MemoryWatchers.Storyline.Current == 630 && MemoryWatchers.State.Current == 0; },
            new Transition { RoomNumber = 95, Storyline = 730, SpawnPoint = 0, Description = "HA HA HA HA"} },
        // END OF LUCA
        // START OF MI'IHEN
        {
            () => { return MemoryWatchers.RoomNumber.Current == 95 && MemoryWatchers.Storyline.Current == 730; },
            new Transition { Storyline = 734, MiihenFlag1 = 5, MiihenFlag2 = 4, PositionTidusAfterLoad = true, Target_x = -3.003f, Target_y = 0.0f, Target_z = -8.161f, Target_rot = 1.576f, Target_var1 = 235, Description = "Tidus runs up the stairs"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 120 && MemoryWatchers.Storyline.Current == 734 && MemoryWatchers.MiihenFlag1.Current == 133; },
            new Transition { MiihenFlag1 = 141, ForceLoad = false, Description = "Meet Calli"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 127 && MemoryWatchers.Storyline.Current == 734 && MemoryWatchers.MiihenFlag1.Current == 141; },
            new Transition { MiihenFlag1 = 221, MiihenFlag2 = 148, ForceLoad = false, Description = "Luzzu, Gatta and Shelinda scenes"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 58 && MemoryWatchers.Storyline.Current == 734 && MemoryWatchers.State.Current == 0; },
            new Transition { RoomNumber = 171, Storyline = 755, Description = "Auron is tired"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 112 && MemoryWatchers.Storyline.Current == 755; },
            new Transition { RoomNumber = 171, Storyline = 760, SpawnPoint = 0, FullHeal = true, Description = "Tidus chats with Yuna" } },
                                        // Tidus chats to a guy
        {
            () => { return MemoryWatchers.RoomNumber.Current == 171 && MemoryWatchers.Storyline.Current == 762; },
            new RinTransition {ForceLoad = false, Description = "Meet Rin", Suspendable = false, Repeatable = true} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 58 && MemoryWatchers.Storyline.Current == 767 && MemoryWatchers.MiihenFlag3.Current == 0; },
            new Transition { MiihenFlag3 = 1 , ForceLoad = false, PositionTidusAfterLoad = true, Target_x = 0.137f, Target_y = 0.0f, Target_z = -227.553f, Target_rot = -2.772f, Target_var1 = 195, Description = "To the chocobo corral"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 58 && MemoryWatchers.Storyline.Current == 767 && MemoryWatchers.MiihenFlag3.Current == 1; },
            new ChocoboEaterTransition {ForceLoad = false, Description = "Chocobo Eater", Suspendable = false, Repeatable = true} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 58 && MemoryWatchers.Storyline.Current == 770 && MemoryWatchers.MiihenFlag3.Current == 3; },
            new Transition { RoomNumber = 115, Storyline = 772, SpawnPoint = 3, Description = "Chocobo Eater loss - Fall down the cliff"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 58 && MemoryWatchers.Storyline.Current == 770 && MemoryWatchers.MiihenFlag3.Current == 5; },
            new Transition { Storyline = 772, SpawnPoint = 2, MiihenFlag3 = 13, Description = "Chocobo Eater win - Rin thanks the party"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 116 && MemoryWatchers.Storyline.Current == 772 && MemoryWatchers.MiihenFlag4.Current == 0; },
            new Transition { MiihenFlag4 = 7 , ForceLoad = false, Description = "Luzzu and Gatta move a cart"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 59 && MemoryWatchers.Storyline.Current == 777 && MemoryWatchers.State.Current == 0; },
            new Transition { Storyline = 787, SpawnPoint = 3, PositionTidusAfterLoad = true, Target_x = -42.826f, Target_y = 0.969f, Target_z = 204.842f, Target_rot = 1.250f, Target_var1 = 4, Description = "Seymour helps out"} },
        // END OF MI'IHEN
        // START OF MUSHROOM ROCK ROAD
        {
            () => { return MemoryWatchers.RoomNumber.Current == 79 && MemoryWatchers.Storyline.Current == 787; },
            new Transition { RoomNumber = 79, Storyline = 825, SpawnPoint = 0, PositionTidusAfterLoad = true, Target_x = -60.826f, Target_y = -6.881f, Target_z = -837.562f, Target_rot = 1.245f, Target_var1 = 979, MoveFrame = 8, Description = "Tidus distrusts Seymour"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 92 && MemoryWatchers.Storyline.Current == 825; },
            new Transition { Storyline = 802, Description = "Send Clasko to the shadow realm"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 119 && MemoryWatchers.Storyline.Current == 802; },
            new Transition { Storyline = 845, MRRFlag1 = 0x02, MRRFlag2 = 0x01, Description = "Preparing for Sin" } },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 119 && MemoryWatchers.Storyline.Current == 845; },
            GuiTransition },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 247 && MemoryWatchers.Storyline.Current == 865; },
            GuiTransition },
                                        // Post-Sinspawn Gui 2
        {
            () => { return MemoryWatchers.RoomNumber.Current == 254 && MemoryWatchers.Storyline.Current == 882; },
            new Transition { RoomNumber = 254, Storyline = 893, Description = "Tidus wakes up + sees Gatta"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 254 && MemoryWatchers.Storyline.Current == 893; },
            new Transition { RoomNumber = 247, Storyline = 899, Description = "Sin FMV + Tidus chases after Sin"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 247 && MemoryWatchers.Storyline.Current == 899; },
            new Transition { RoomNumber = 218, Storyline = 902, Description = "Yuna tries to summon"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 218 && MemoryWatchers.Storyline.Current == 902; },
            new Transition { RoomNumber = 341, Storyline = 910, Description = "Tidus is swimming"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 341 && MemoryWatchers.Storyline.Current == 910; },
            new Transition { RoomNumber = 134, Storyline = 910, Description = "Nucleus"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 134 && MemoryWatchers.Storyline.Current == 910; },
            new Transition { RoomNumber = 131, Storyline = 910, Description = "Zanarkand flashback"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 131 && MemoryWatchers.Storyline.Current == 802; },
            new Transition { RoomNumber = 131, Storyline = 922, SpawnPoint = 3, PositionTidusAfterLoad = true, Target_x = 783.620f, Target_y = -2.361f, Target_z = -132.723f, Target_rot = -2.005f, Target_var1 = 161, Description = "Tidus monologue on beach (Terra Skip)"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 131 && MemoryWatchers.Storyline.Current == 910; },
            new Transition { RoomNumber = 131, Storyline = 922, SpawnPoint = 3, PositionTidusAfterLoad = true, Target_x = 783.620f, Target_y = -2.361f, Target_z = -132.723f, Target_rot = -2.005f, Target_var1 = 161, Description = "Tidus monologue on beach"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 131 && MemoryWatchers.Storyline.Current == 922 && MemoryWatchers.State.Current == 0; },
            new Transition { RoomNumber = 131, Storyline = 928, SpawnPoint = 3, PositionTidusAfterLoad = true, Target_x = 493.227f, Target_y = -15.797f, Target_z = -190.388f, Target_rot = 1.981f, Target_var1 = 94, Description = "Kinoc retreats"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 131 && MemoryWatchers.Storyline.Current == 928 && MemoryWatchers.State.Current == 0; },
            new Transition { RoomNumber = 131, Storyline = 938, SpawnPoint = 0, PositionTidusAfterLoad = true, Target_x = 371.602f, Target_y = -22.725f, Target_z = -167.794f, Target_rot = 0.507f, Target_var1 = 88, Description = "Tidus speaks to Auron"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 131 && MemoryWatchers.Storyline.Current == 938; },
            new AftermathTransition {ForceLoad = false, Description = "Leaving Mushroom Rock Road", Suspendable = false, Repeatable = true } },
        // END OF MRR
        // START OF DJOSE HIGHROAD
        {
            () => { return MemoryWatchers.RoomNumber.Current == 93 && MemoryWatchers.Storyline.Current == 960; },
            new Transition { RoomNumber = 93, Storyline = 961, SpawnPoint = 0, PositionTidusAfterLoad = true, Target_x = -244.694f, Target_y = -56.461f, Target_z = -810.146f, Target_rot = 1.698f, Target_var1 = 190, Description = "Kimahri speaks"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 93 && MemoryWatchers.Storyline.Current == 961 && MemoryWatchers.CutsceneAlt.Current == 1678; },
            new Transition { RoomNumber = 76, Storyline = 962, SpawnPoint = 1, Description = "Tidus is eager to go to Zanarkand"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 76 && MemoryWatchers.Storyline.Current == 962; },
            new Transition { Storyline = 970, SpawnPoint = 0, Description = "Tidus whoa"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 82 && MemoryWatchers.Storyline.Current == 970; },
            new Transition { Storyline = 971, SpawnPoint = 0, Description = "Arrival at Djose Temple"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 81 && MemoryWatchers.Storyline.Current == 971; },
            new Transition { Storyline = 985, SpawnPoint = 0, Description = "Meet Isaaru"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 214 && MemoryWatchers.Storyline.Current == 990; },
            new Transition { RoomNumber = 214, Storyline = 995, SpawnPoint = 0, Description = "Entering the Djose trials"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 90 && MemoryWatchers.Storyline.Current == 998; },
            new Transition { MenuTriggerValue = 0x4008000A, EnableIxion = 0x11, ForceLoad = false, Description = "Naming Ixion"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 90 && MemoryWatchers.Storyline.Current == 998 && MemoryWatchers.Menu.Current == 1 && MemoryWatchers.MenuValue1.Current == 0x4000; },
            new Transition { RoomNumber = 82, Storyline = 1010, SpawnPoint = 4, FullHeal = true, Description = "Tidus wakes up"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 82 && MemoryWatchers.Storyline.Current == 1015 && MemoryWatchers.State.Current == 0; },
            new Transition { RoomNumber = 82, Storyline = 1020, SpawnPoint = 2, Description = "Yuna has bed hair"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 76 && MemoryWatchers.Storyline.Current == 1020 && MemoryWatchers.State.Current == 0; },
            new Transition { RoomNumber = 76, Storyline = 1025, SpawnPoint = 0, Description = "Clasko wait for me"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 93 && MemoryWatchers.Storyline.Current == 1028; },
            new Transition { RoomNumber = 93, Storyline = 1030, SpawnPoint = 1, Description = "Moonflow here we come"} },
        // END OF DJOSE HIGHROAD
        // START OF MOONFLOW
        {
            () => { return MemoryWatchers.RoomNumber.Current == 75 && MemoryWatchers.Storyline.Current == 1030; },
            new Transition { RoomNumber = 75, Storyline = 1032, MoonflowFlag = 1, Description = "Biran and Yenke taunt"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 105 && MemoryWatchers.Storyline.Current == 1032 && MemoryWatchers.State.Current == 0; },
            new Transition { RoomNumber = 105, Storyline = 1040, SpawnPoint = 1, Description = "Tidus sees the Moonflow river"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 105 && MemoryWatchers.Storyline.Current == 1040; },
            new Transition { RoomNumber = 187, Storyline = 1045, SpawnPoint = 0, Description = "Tidus sees a shoopuf"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 235 && MemoryWatchers.Storyline.Current == 1045 && MemoryWatchers.MoonflowFlag.Current == 1; },
            new Transition { MoonflowFlag = 65, Description = "The chocobos cannot cross"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 188 && MemoryWatchers.Storyline.Current == 1045; },
            new ShoopufTransition { ForceLoad = false, Description = "All Aboards!", Suspendable = false, Repeatable = true } },
        //{ () => { return MemoryWatchers.RoomNumber.Current == 291 && MemoryWatchers.Storyline.Current == 1045; },
        //  new Transition { RoomNumber = 99, Storyline = 1045, Description = "Shoopuf launching + map"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 99 && MemoryWatchers.Storyline.Current == 1045; },
            new Transition { RoomNumber = 291, Storyline = 1048, Description = "A sunken city"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 291 && MemoryWatchers.Storyline.Current == 1048; },
            new Transition { RoomNumber = 99, Storyline = 1060, Description = "A debate about machina"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 99 && MemoryWatchers.Storyline.Current == 1060; },
            new ExtractorTransition {ForceLoad = false, Description = "Extractor", Suspendable = false, Repeatable = true } },
                                        // Post-Extractor
        {
            () => { return MemoryWatchers.RoomNumber.Current == 291 && MemoryWatchers.Storyline.Current == 1060; },
            new Transition { RoomNumber = 236, Storyline = 1070, SpawnPoint = 0, Description = "Back on the shoopuf"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 109 && MemoryWatchers.Storyline.Current == 1070; },
            new Transition { RoomNumber = 109, Storyline = 1085, SpawnPoint = 0, FormationSwitch = Transition.formations.MeetRikku, MoonflowFlag2 = 36, RikkuOutfit = 0, Description = "Reunite with Rikku + FMV"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 97 && MemoryWatchers.Storyline.Current == 1085 && MemoryWatchers.MoonflowFlag2.Current == 36; },
            new Transition { MoonflowFlag2 = 100, Description = "Map shown"} },
        // END OF MOONFLOW
        // START OF GUADOSALAM
        {
            () => { return MemoryWatchers.RoomNumber.Current == 135 && MemoryWatchers.Storyline.Current == 1085; },
            new Transition { RoomNumber = 135, Storyline = 1096, SpawnPoint = 0, PositionTidusAfterLoad = true, Target_x = 13.501f, Target_y = 0.119f, Target_z = -49.981f, Target_rot = 2.705f, Target_var1 = 306, Description = "Meet Tromell + Customise tutorial"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 163 && MemoryWatchers.Storyline.Current == 1096 && MemoryWatchers.State.Current == 1; },
            new Transition { RoomNumber = 163, Storyline = 1104, SpawnPoint = 1, PositionTidusAfterLoad = true, Target_x = 11.144f, Target_y = 3.620f, Target_z = 4.971f, Target_rot = 1.576f, Target_var1 = 21, Description = "Tromell invites the gang in"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 141 && MemoryWatchers.Storyline.Current == 1104; },
            new SeymoursHouseTransition {ForceLoad = false, Description = "Seymour's House", Suspendable = false, Repeatable = true } },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 197 && MemoryWatchers.Storyline.Current == 1104; },
            new Transition { RoomNumber = 217, Storyline = 1118, Description = "Seymour proposes to Yuna"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 217 && MemoryWatchers.Storyline.Current == 1118; },
            new Transition { RoomNumber = 163, Storyline = 1126, SpawnPoint = 1, Description = "Yuna drinks a glass of water"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 135 && MemoryWatchers.Storyline.Current == 1126 && MemoryWatchers.State.Current == 1; },
            new Transition { RoomNumber = 135, Storyline = 1132, SpawnPoint = 1, PositionTidusAfterLoad = true, Target_x = 8.317f, Target_y = -7.668f, Target_z = 107.409f, Target_rot = -1.360f, Target_var1 = 265, Description = "The gang discuss the proposal"} },
        {
            () => { return MemoryWatchers.RoomNumber.Current == 257 && MemoryWatchers.Storyline.Current == 1132; }, 
            new Transition { RoomNumber = 257, Storyline = 1138, SpawnPoint = 0, Description = "Tidus freaks out about the undead"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 257 && MemoryWatchers.Storyline.Current == 1138; }, 
            new Transition { RoomNumber = 257, Storyline = 1154, SpawnPoint = 0, Description = "Auron and Rikku stay behind"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 257 && MemoryWatchers.Storyline.Current == 1154 && Math.Abs(MemoryWatchers.XCoordinate.Current - 233.304f) < 0.5f; }, 
            new Transition { RoomNumber = 193, Description = "Tidus enters the Farplane"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 193 && MemoryWatchers.Storyline.Current == 1154; },
            FarplaneTransition },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 193 && MemoryWatchers.Storyline.Current == 1156; }, 
            FarplaneTransition },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 134 && MemoryWatchers.Storyline.Current == 1170; },
            new Transition { RoomNumber = 193, Storyline = 1172, Description = "Zanarkand flashback"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 193 && MemoryWatchers.Storyline.Current == 1172; },
            new Transition { RoomNumber = 364, Storyline = 1176, Description = "Tidus is embarrassed"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 364 && MemoryWatchers.Storyline.Current == 1176; },
            new Transition { RoomNumber = 175, Storyline = 1184, Description = "Jyscal returns"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 175 && MemoryWatchers.Storyline.Current == 1184; }, 
            new Transition { RoomNumber = 135, Storyline = 1190, SpawnPoint = 2, Description = "Tidus asks about Jyscal"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 135 && MemoryWatchers.Storyline.Current == 1190 && MemoryWatchers.State.Current == 1; }, 
            new Transition { RoomNumber = 135, Storyline = 1194, SpawnPoint = 2, GuadosalamShopFlag = 16, PositionTidusAfterLoad = true, Target_x = 9.356f, Target_y = -79.885f, Target_z = 32.475f, Target_rot = 1.640f, Target_var1 = 174, Description = "Affection scene"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 135 && MemoryWatchers.Storyline.Current == 1194 && MemoryWatchers.State.Current == 1; }, 
            new Transition { RoomNumber = 135, Storyline = 1196, SpawnPoint = 4, PositionTidusAfterLoad = true, Target_x = -16.089f, Target_y = -0.114f, Target_z = -9.910f, Target_rot = 1.728f, Target_var1 = 341, Description = "Tidus speaks to Shelinda"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 135 && MemoryWatchers.Storyline.Current == 1196 && MemoryWatchers.State.Current == 1; }, 
            new Transition { RoomNumber = 135, Storyline = 1200, Description = "Yuna asks Jyscal what she can do"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 135 && MemoryWatchers.Storyline.Current == 1200; },
            new Transition { RoomNumber = 135, Storyline = 1210, SpawnPoint = 1, PositionTidusAfterLoad = true, Target_x = -9.995f, Target_y = -6.983f, Target_z = 135.763f, Target_rot = 0.556f, Target_var1 = 267, Description = "Macarena Temple"} },       
        // END OF GUADOSALAM
        // START OF THUNDER PLAINS
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 140 && MemoryWatchers.Storyline.Current == 1300; }, 
            new Transition { RoomNumber = 140, Storyline = 1310, SpawnPoint = 0, Description = "Map + Rikku afraid + tutorial"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 140 && MemoryWatchers.Storyline.Current == 1310 && MemoryWatchers.ThunderPlainsFlag.Current == 0x08; },
            new Transition { RoomNumber = 256, Storyline = 1310, SpawnPoint = 0, ThunderPlainsFlag = 0x18, Description = "Rikku freaks out"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 140 && MemoryWatchers.Storyline.Current == 1310 && MemoryWatchers.ThunderPlainsFlag.Current == 0x09; },
            new Transition { RoomNumber = 256, Storyline = 1310, SpawnPoint = 0, ThunderPlainsFlag = 0x19, Description = "Rikku freaks out"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 140 && MemoryWatchers.Storyline.Current == 1310 && MemoryWatchers.ThunderPlainsFlag.Current == 0x0A; }, 
            new Transition { RoomNumber = 256, Storyline = 1310, SpawnPoint = 0, ThunderPlainsFlag = 0x1A, Description = "Rikku freaks out"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 140 && MemoryWatchers.Storyline.Current == 1310 && MemoryWatchers.ThunderPlainsFlag.Current == 0x0B; }, 
            new Transition { RoomNumber = 256, Storyline = 1310, SpawnPoint = 0, ThunderPlainsFlag = 0x1B, Description = "Rikku freaks out"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 263 && MemoryWatchers.Storyline.Current == 1315; }, 
            new Transition { RoomNumber = 263, Storyline = 1315, SpawnPoint = 0, Description = "Rikku asks to go to the agency"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 264 && MemoryWatchers.Storyline.Current == 1320; }, 
            new Transition { RoomNumber = 263, Storyline = 1325, FullHeal = true, Description = "Tidus barges in Yuna's room + Sleep"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 263 && MemoryWatchers.Storyline.Current == 1335; }, 
            new Transition { RoomNumber = 256, Storyline = 1340, Description = "Leaving the agency"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 162 && MemoryWatchers.Storyline.Current == 1340; },
            new Transition { RoomNumber = 162, Storyline = 1375, Description = "Yuna decides to marry Seymour"} },
        // END OF THUNDER PLAINS
            // START OF MACALANIA
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 110 && MemoryWatchers.Storyline.Current == 1375; },
            new Transition { RoomNumber = 110, Storyline = 1400, Description = "Arriving at Macalania"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 110 && MemoryWatchers.Storyline.Current == 1400 && MemoryWatchers.State.Current == 0; }, 
            new Transition { RoomNumber = 110, Storyline = 1407, SpawnPoint = 5, Description = "Tidus is worried about Yuna"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 241 && MemoryWatchers.Storyline.Current == 1407; }, 
            new Transition { RoomNumber = 241, Storyline = 1413, MacalaniaFlag = 32, Description = "Barthello has lost Dona + Butterfly guy"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 221 && MemoryWatchers.Storyline.Current == 1413; }, 
            new Transition { RoomNumber = 221, Storyline = 1420, SpawnPoint = 0, Description = "Pre-Spherimorph Auron Smash"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 248 && MemoryWatchers.Storyline.Current == 1420; }, 
            new SpherimorphTransition {ForceLoad = false , Description = "Spherimorph", Suspendable = false, Repeatable = true} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 164 && MemoryWatchers.Storyline.Current == 1470; },
            new TromellTransition {ForceLoad = false , Description = "Tromell leads Yuna away", Suspendable = false, Repeatable = true} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 102 && MemoryWatchers.Storyline.Current == 1485; }, 
            new CrawlerTransition {ForceLoad = false, Description = "Pre Crawler", Suspendable = false, Repeatable = true} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 106 && MemoryWatchers.Storyline.Current == 1504; },
            new Transition { Storyline = 1530, Description = "Jysscal Skip"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 80 && MemoryWatchers.Storyline.Current == 1530 && MemoryWatchers.Menu.Current == 0; }, SeymourTransition},
        //{ () => { return MemoryWatchers.RoomNumber.Current == 80 && MemoryWatchers.Storyline.Current == 1540; },
        //  SeymourTransition},
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 80 && MemoryWatchers.Storyline.Current == 1530 && MemoryWatchers.Menu.Current == 1 && MemoryWatchers.MenuValue1.Current == 0x20000; }, 
            new Transition { MenuTriggerValue = 0x4008000B, ForceLoad = false, Description = "Naming Shiva"}},
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 80 && MemoryWatchers.Storyline.Current == 1530 && MemoryWatchers.Menu.Current == 1 && MemoryWatchers.MenuValue1.Current == 0x4000; },
            new Transition { ConsoleOutput = false, Storyline = 1545, PositionTidusAfterLoad = true, Target_x = 1.470f, Target_y = 0.0f, Target_z = 0.388f, Target_rot = -0.119f, Target_var1 = 53}},
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 239 && MemoryWatchers.Storyline.Current == 1545; }, 
            new Transition { Storyline = 1557, CutsceneTiming = 1, /*ForceLoad = false,*/ Description = "Backflip Skip"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 102 && MemoryWatchers.Storyline.Current == 1570; },
            new WendigoTransition {ForceLoad = false, Description = "Wendigo", Suspendable = false, Repeatable = true} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 54 && MemoryWatchers.Storyline.Current == 1600; },
            UnderLakeTransition },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 54 && MemoryWatchers.Storyline.Current == 1607; },
            UnderLakeTransition },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 54 && MemoryWatchers.Storyline.Current == 1610; },
            UnderLakeTransition },
            
            // START OF BIKANEL
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 136 && MemoryWatchers.Storyline.Current == 1715; },
            BikanelTransition },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 136 && MemoryWatchers.Storyline.Current == 1718 && MemoryWatchers.EnableRikku.Current == 0 && MemoryWatchers.State.Current == 1; },
            new Transition { Storyline = 1720, SpawnPoint = 3, FormationSwitch = Transition.formations.BikanelRikku, BikanelFlag = 32, Description = "Wakka Glare" } },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 138 && MemoryWatchers.Storyline.Current == 1720 && MemoryWatchers.State.Current == 1 && MemoryWatchers.MovementLock.Current == 0x30; }, 
            new Transition { RoomNumber = 130, Storyline = 1800, SpawnPoint = 0, PositionTidusAfterLoad = true, Target_x = -15.831f, Target_y = -0.493f, Target_z = -98.586f, Target_rot = 2.518f, Target_var1 = 432, Description = "Sanubia to Home"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 280 && MemoryWatchers.Storyline.Current == 1820; },
            HomeTransition},
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 280 && MemoryWatchers.Storyline.Current == 1885; },
            HomeTransition},
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 275; },
            HomeTransition},
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 286; },
            HomeTransition},
        { 
            () => { return MemoryWatchers.Storyline.Current == 1940 && MemoryWatchers.EncounterStatus.Current == 89; }, 
            new Transition { EncounterStatus = 88, ForceLoad = false, Description = "Disabling Encounters"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 261 && MemoryWatchers.Storyline.Current == 1940; }, 
            new Transition { RoomNumber = 194, Storyline = 1950, SpawnPoint = 1, PositionTidusAfterLoad = true, Target_x = -228.570f, Target_y = 12.302f, Target_z = 363.249f, Target_rot = -2.722f, Target_var1 = 1251, Description = "Home to Airship"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 194 && MemoryWatchers.Storyline.Current == 1990; },
            new Transition { Storyline = 2000, SpawnPoint = 1, PositionTidusAfterLoad = true, Target_x = -242.490f, Target_y = 12.110f, Target_z = 290.946f, Target_rot = 1.592f, Target_var1 = 1302, Description = "Airship Bridge Cutscene"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 351 && MemoryWatchers.Storyline.Current == 2020; }, 
            new Transition { Storyline = 2040, ForceLoad = false, Description = "Red carpet has teeth"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 277 && MemoryWatchers.Storyline.Current == 2040; },
            new EvraeTransition {ForceLoad = false, Description = "Pre Evrae", Suspendable = false, Repeatable = true} },

            // END OF HOME
            // START OF BEVELLE
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 205 && MemoryWatchers.Storyline.Current == 2060 && MemoryWatchers.MusicId.Current == 181; },
            new Transition { Storyline = 2075, SpawnPoint = 0, Description = "Evrae to Guards"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 205 && MemoryWatchers.Storyline.Current == 2080; }, 
            new GuardsTransition { ForceLoad = false, Description = "Guards", Suspendable = false, Repeatable = true} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 205 && MemoryWatchers.Storyline.Current == 2085; }, 
            new Transition { RoomNumber = 180, Storyline = 2135, Description = "Bevelle Guards to Trials"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 226 && MemoryWatchers.Storyline.Current == 2135; }, 
            new Transition { MenuTriggerValue = 0x4008000C, EnableBahamut = 0x11, ForceLoad = false, Description = "Trials to Bahamut naming"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 226 && MemoryWatchers.Storyline.Current == 2135 && MemoryWatchers.Menu.Current == 1 && MemoryWatchers.MenuValue1.Current == 0x4000; }, 
            new Transition { RoomNumber = 198, Storyline = 2220, SpawnPoint = 0, FormationSwitch = Transition.formations.ViaPurificoStart, Description = "Bahamut to Via Purifico", ViaPurificoPlatform = 1 } },
        //{ () => { return MemoryWatchers.RoomNumber.Current == 226 && MemoryWatchers.Storyline.Current == 2135},
        //  new Transition { Storyline = 2150, Description = "Trials to Bahamut naming"} },
        //{ () => { return MemoryWatchers.RoomNumber.Current == 226 && MemoryWatchers.Storyline.Current == 2150},
        //  new BahamutTransition { ForceLoad = false, Description = "Naming Bahamut", Suspendable = false, Repeatable = true} },
        //{ () => { return MemoryWatchers.RoomNumber.Current == 226 && MemoryWatchers.Storyline.Current == 2155 && MemoryWatchers.Menu.Current == 0},
        //  new Transition { RoomNumber = 198, Storyline = 2220, SpawnPoint = 0, FormationSwitch = Transition.formations.ViaPurificoStart, Description = "Bahamut to Via Purifico", ViaPurificoPlatform = 1 } },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 198 && MemoryWatchers.Storyline.Current == 2220 && MemoryWatchers.EnableAuron.Current == 17; }, 
            new IsaaruTransition {ForceLoad = false, Description = "Isaaru", Suspendable = false, Repeatable = true} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 209 && MemoryWatchers.Storyline.Current == 2220; }, 
            new AltanaTransition {ForceLoad = false, Description = "Evrae Altana", Suspendable = false, Repeatable = true} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 208 && MemoryWatchers.Storyline.Current == 2220; },
            new Transition { Storyline = 2275, SpawnPoint = 2, FormationSwitch = Transition.formations.HighbridgeStart, Description = "Enter Highbridge" } },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 208 && MemoryWatchers.Storyline.Current == 2280; }, 
            new NatusTransition { ForceLoad = false, Description = "Seymour Natus", Suspendable = false, Repeatable = true } },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 183 && MemoryWatchers.Storyline.Current == 2290; }, 
            new Transition { RoomNumber = 183, Storyline = 2300, SpawnPoint = 0, NatusFlag = 0x00, ForceLoad = false, Description = "Natus Death"} },
            // END OF BEVELLE
            // START OF CALM LANDS & GAGAZET
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 206 && MemoryWatchers.Storyline.Current == 2300 && MemoryWatchers.CutsceneAlt.Current == 3712; }, 
            new Transition { RoomNumber = 177, Storyline = 2385, SpawnPoint = 1, MacalaniaFlag = 162, Description = "Lake Scene"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 223 && MemoryWatchers.Storyline.Current == 2385 && MemoryWatchers.State.Current == 0;/* && TidusZCoordinate = -1856.596f*/},
            new Transition { Storyline = 2400, CalmLandsFlag = 33572, PositionTidusAfterLoad = true, Target_x = 342.047f, Target_y = -162.136f, Target_z = -1589.261f, Target_rot = -3.089f, Target_var1 = 494, Description = "Calm Lands Intro + Gorge flag"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 223 && MemoryWatchers.Storyline.Current == 2400 && MemoryWatchers.CutsceneAlt.Current == 885; }, 
            new Transition { SpawnPoint = 0, AddCalmLandsBitmask = 0x08, PositionTidusAfterLoad = true, Target_x = -656.641f, Target_y = 40.625f, Target_z = -122.844f, Target_rot = 1.060f, Target_var1 = 7179, Description = "Father Zuke"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 279 && MemoryWatchers.Storyline.Current == 2400; },
            new DefenderXTransition {ForceLoad = false, Description = "Defender X", Suspendable = false, Repeatable = true} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 279 && MemoryWatchers.Storyline.Current == 2420 && MemoryWatchers.MovementLock.Current == 48 && Math.Abs(MemoryWatchers.XCoordinate.Current - 265.377f) < 0.5f; },
            new Transition { RoomNumber = 259, Storyline = 2510, RoomNumberAlt = 266, SpawnPoint = 0, Description = "Yuna reflects"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 259 && MemoryWatchers.Storyline.Current == 2510; },
            new RonsoTransition {ForceLoad = false, Description = "Biran + Yenke", Suspendable = false, Repeatable = true} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 244 && MemoryWatchers.Storyline.Current == 2528 && MemoryWatchers.State.Current == 1; },
            new Transition {Storyline = 2530, SpawnPoint = 0, WantzFlag = 1, WantzMacalaniaFlag = 1, PositionTidusAfterLoad = true, Target_x = 13.495f, Target_y = -3.161f, Target_z = 19.213f, Target_rot = 1.570f, Target_var1 = 1243, Description = "Ronso Singing"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 285 && MemoryWatchers.Storyline.Current == 2530; },
            new FluxTransition {ForceLoad = false, Description = "Seymour Flux", Suspendable = false, Repeatable = true} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 309 && MemoryWatchers.Storyline.Current == 2560; },
            new Transition { RoomNumber = 134, Storyline = 2585, Description = "Fayth FMV + Tidus collapses"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 134 && MemoryWatchers.Storyline.Current == 2585; }, 
            new Transition { Storyline = 2590, SpawnPoint = 0, Description = "Tidus wakes up in Zanarkand"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 165 && MemoryWatchers.Storyline.Current == 2595 && MemoryWatchers.State.Current == 1; }, 
            new Transition { RoomNumber = 249, Storyline = 2610, Description = "Tidus enters his house"} },
        //{ () => { return MemoryWatchers.RoomNumber.Current == 134 && MemoryWatchers.Storyline.Current == 2600 && MemoryWatchers.State.Current == 1; },
        //  new Transition { RoomNumber = 249, Storyline = 2610, Description = "The fayth asks Tidus to end the dream"} }, // Bug: Game softlocks with Tidus entering house skip, so this little bit of movement is lost for now
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 249 && MemoryWatchers.Storyline.Current == 2610; }, 
            new Transition { RoomNumber = 309, Storyline = 2610, Description = "The dream disintegrates"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 309 && MemoryWatchers.Storyline.Current == 2610; }, 
            new Transition { Storyline = 2585, Description = "Tidus wakes up"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 272 && MemoryWatchers.Storyline.Current == 2585 && MemoryWatchers.GagazetCaveFlag.Current == 0; },
            new Transition { GagazetCaveFlag = 29120, Description = "Gagazet Cave scenes"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 311 && MemoryWatchers.Storyline.Current == 2585; }, 
            new SanctuaryTransition {ForceLoad = false, Description = "Sanctuary Keeper", Suspendable = false, Repeatable = true} },
            // END OF GAGAZET
            // START OF ZANARKAND
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 132 && MemoryWatchers.Storyline.Current == 2680 && MemoryWatchers.State.Current == 0; }, 
            new Transition { RoomNumber = 363, Storyline = 2767, SpawnPoint = 0, PositionTidusAfterLoad = true, Target_x = 48.325f, Target_y = -2.791f, Target_z = 40.530f, Target_rot = -2.217f, Target_var1 = 364, Description = "Zanarkand Campfire"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 320 && MemoryWatchers.Storyline.Current == 2767; }, 
            new Transition { SpawnPoint = 0, Description = "Zanarkand Trials Begin"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 317 && MemoryWatchers.Storyline.Current == 2775; }, 
            new SpectralKeeperTransition {ForceLoad = false, Description = "Pre Spectral Keeper", Suspendable = false, Repeatable = true} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 318 && MemoryWatchers.Storyline.Current == 2790; }, 
            new Transition { Storyline = 2815, Description = "Spectral Keeper to Yunalesca"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 270 && MemoryWatchers.Storyline.Current == 2815; }, 
            new YunalescaTransition {ForceLoad = false, Description = "Pre-Yunalesca", Suspendable = false, Repeatable = true} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 315 && MemoryWatchers.Storyline.Current == 2850; }, 
            new Transition { RoomNumber = 194, Storyline = 2900, SpawnPoint = 2, PositionTidusAfterLoad = true, Target_x = -264.336f, Target_y = 12.163f, Target_z = 365.005f, Target_rot = -0.663f, Target_var1 = 1230, Description = "End of Zanarkand"} },
            // END OF ZANARKAND
            // START OF SIN
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 211 && MemoryWatchers.Storyline.Current == 2900 && Math.Abs(MemoryWatchers.XCoordinate.Current - (-9.918f)) < 0.5f; },
            new Transition { Storyline = 2915, SpawnPoint = 7, AirshipDestinations = 2048, OmegaRuinsFlag = 0x01, PositionTidusAfterLoad = true, Target_x = -30.525f, Target_y = 0.0f, Target_z = 39.000f, Target_rot = -0.866f, Target_var1 = 82, Description = "Yuna/Kimahri talk about defeating Sin"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 208 && MemoryWatchers.Storyline.Current == 2920 && MemoryWatchers.CutsceneAlt.Current == 91; }, 
            new Transition { RoomNumber = 227, Storyline = 2945, SpawnPoint = 0, Description = "Shelinda + Mika"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 227 && MemoryWatchers.Storyline.Current == 2945; }, 
            new BahamutFaythTransition2 { ForceLoad=false, Description = "Bahamut Fayth", Suspendable = false, Repeatable = true } },
        //{ () => { return MemoryWatchers.RoomNumber.Current == 208 && MemoryWatchers.Storyline.Current == 2920 && MemoryWatchers.CutsceneAlt.Current == 91},
        //  new Transition { RoomNumber = 255, Storyline = 2970, SpawnPoint = 0, AirshipDestinations = 2560, PositionTidusAfterLoad = true, Target_x = -242.858f, Target_y = 12.126f, Target_z = 160.448f, Target_rot = 1.556f, Target_var1 = 1390, Description = "Return from Highbridge"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 255 && MemoryWatchers.Storyline.Current == 2990; }, 
            new Transition { RoomNumber = 255, Storyline = 3010, SpawnPoint = 0, RemoveSinLocation = true, Description = "Sin destination cutscene"} }, //Bug (Minor): Wrong area/spawn
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 277 && MemoryWatchers.Storyline.Current == 3010; }, 
            new Transition { RoomNumber = 199, Storyline = 3085, Description = "Left Fin" } },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 199 && MemoryWatchers.Storyline.Current == 3085; }, 
            new FinLeftTransition {ForceLoad=false, Description = "Left fin Pre-Boss", Suspendable = false, Repeatable = true} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 200 && MemoryWatchers.Storyline.Current == 3085; }, 
            new FinRightTransition {ForceLoad=false, Description = "Right Fin Pre-Boss", Suspendable = false, Repeatable = true} },
        //{ () => { return MemoryWatchers.RoomNumber.Current == 200 && MemoryWatchers.Storyline.Current == 3100},
        //  new Transition { RoomNumber = 201, Storyline = 3105, Description = "Star Players First!" } },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 201 && MemoryWatchers.Storyline.Current == 3105; }, 
            new SinCoreTransition {ForceLoad = false, Description = "Sin Core", Suspendable = false, Repeatable = true} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 202 && MemoryWatchers.Storyline.Current == 3125 && Math.Abs(MemoryWatchers.XCoordinate.Current - 18.585f) < 0.5f && MemoryWatchers.State.Current == 0; }, 
            new Transition { RoomNumber = 374, Storyline = 3135, SpawnPoint = 0, Description = "Yuna monologue"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 202 && MemoryWatchers.Storyline.Current == 3135; }, 
            new OverdriveSinTransition {ForceLoad = false, Description = "Overdrive Sin", Suspendable = false, Repeatable = true} }, // Doesn't work yet
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 296 && MemoryWatchers.Storyline.Current == 3205; }, 
            new OmnisTransition {ForceLoad = false, Description = "Pre-BFA", Suspendable = false, Repeatable = true} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 327 && MemoryWatchers.Storyline.Current == 3250 && MemoryWatchers.CutsceneAlt.Current == 5889; }, 
            new Transition { RoomNumber = 324, Storyline = 3250, SpawnPoint = 0, Description = "Enter Tower of the Dead"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 325 && MemoryWatchers.Storyline.Current == 3270; }, 
            new BFATransition {ForceLoad = false, Description = "Pre-BFA", Suspendable = false, Repeatable = true} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 325 && MemoryWatchers.Storyline.Current == 3300 && MemoryWatchers.CutsceneAlt.Current == 1180; }, 
            new Transition { RoomNumber = 326, Storyline = 3360, Description = "BFA Death. GG!"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 326 && MemoryWatchers.Storyline.Current == 3360 && MemoryWatchers.HpEnemyA.Current == 0; }, 
            new YuYevonTransition {ForceLoad=false, Description = "Contest of Aeons!", Suspendable = false, Repeatable = true} },

        // Miscellaneous transitions

        { 
            () => { return MemoryWatchers.RoomNumber.Current == 56; }, 
            new YojimboTransition { ForceLoad=false, Description = "Lady Ginnem Attacks", Suspendable = false, Repeatable = true } },
        //{ () => { return MemoryWatchers.RoomNumber.Current == 283 }, new YojimboFaythTransition {ForceLoad=false, Description = "Yojimbo Fayth Intro", Suspendable = false, Repeatable = true} }
    };

    public static readonly Dictionary<Func<bool>, Transition> PostBossBattleTransitions = new Dictionary<Func<bool>, Transition>()
    {
        { 
            () => { return MemoryWatchers.EncounterMapID.Current == 2 && MemoryWatchers.EncounterFormationID2.Current == 1 && MemoryWatchers.RoomNumber.Current == 63 && MemoryWatchers.Storyline.Current == 55; },
            new Transition { RoomNumber = 71, Storyline = 60, Description = "Klikk"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 83 && MemoryWatchers.Storyline.Current == 172 && MemoryWatchers.State.Current == 1 && MemoryWatchers.CutsceneAlt.Current == 0; }, 
            new Transition { RoomNumber = 68, Storyline = 184, Description = "Tidus joins the Aurochs"} },
        { 
            () => { return MemoryWatchers.HpEnemyA.Current == 3000 && MemoryWatchers.Storyline.Current == 322; }, 
            new Transition { SpawnPoint = 1, Storyline = 326, PositionTidusAfterLoad = true, Target_x = -11.760f, Target_y = -159.978f, Target_z = 541.001f, Target_rot = 1.698f, Target_var1 = 88, MoveFrame = 5, Description = "Post-Geneaux"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 45 && MemoryWatchers.Storyline.Current == 346 && MemoryWatchers.State.Current == 1; }, 
            new Transition { RoomNumber = 78, Storyline = 348, SpawnPoint = 0, EnableIfrit = 17, Description = "Ifrit"} },
        { 
            () => { return MemoryWatchers.HpEnemyA.Current == 300 && MemoryWatchers.RoomNumber.Current == 88 && MemoryWatchers.Storyline.Current == 492; }, 
            new Transition { RoomNumber = 121, Description = "Machina"} },
        { 
            () => { return MemoryWatchers.HpEnemyA.Current == 6000 && MemoryWatchers.Storyline.Current == 502; },
            new Transition { RoomNumber = 121, Storyline = 508, FormationSwitch = Transition.formations.PostOblitzerator, Description = "Oblitzerator"} },
        { 
            () => { return MemoryWatchers.HpEnemyA.Current == 6000 && MemoryWatchers.Storyline.Current == 865; },
            new Transition { RoomNumber = 254, Storyline = 882, Description = "Sinspawn Gui 2"} },
        { 
            () => { return MemoryWatchers.HpEnemyA.Current == 12000 && MemoryWatchers.Storyline.Current == 1420; }, 
            new Transition { RoomNumber = 248, Storyline = 1470, SpawnPoint = 2, PositionTidusAfterLoad = true, Target_x = -12.163f, Target_y = 0.816f, Target_z = 34.410f, Target_rot = -1.454f, Target_var1 = 135, Description = "Spherimorph", AuronOverdrives = 49 } },
        { 
            () => { return MemoryWatchers.HpEnemyA.Current == 16000 && MemoryWatchers.Storyline.Current == 1485; }, 
            new Transition { RoomNumber = 192, Storyline = 1504, SpawnPoint = 1, TargetFramerate = 2, MenuCleanup = true, AddRewardItems = true, Description = "Crawler"} },
        { 
            () => { return MemoryWatchers.HpEnemyA.Current == 1200 && MemoryWatchers.RoomNumber.Current == 102 && MemoryWatchers.Storyline.Current == 1570; }, 
            new Transition { RoomNumber = 54, Storyline = 1600, SpawnPoint = 0, Description = "Wendigo"} }, // HP Value is the Guard
        { 
            () => { return MemoryWatchers.HpEnemyA.Current == 12000 && MemoryWatchers.Storyline.Current == 1704; }, 
            new Transition { RoomNumber = 129, Storyline = 1715, SpawnPoint = 0, Description = "Bikanel Zu", FormationSwitch = Transition.formations.PostZu, PositionTidusAfterLoad = true, Target_x = -20.052f, Target_y = -2.300f, Target_z = -247.011f, Target_rot = -1.570f, Target_var1 = 1819 } },
        { 
            () => { return MemoryWatchers.HpEnemyA.Current == 2600 && MemoryWatchers.Storyline.Current == 1820; }, 
            new Transition { RoomNumber = 276, Storyline = 1820, SpawnPoint = 0, Description = "Home Bombs", PositionTidusAfterLoad = true, Target_x = -0.037f, Target_y = 0.399f, Target_z = -5.576f, Target_rot = -1.884f, Target_var1 = 7} },
        { 
            () => { return MemoryWatchers.EncounterMapID.Current == 87 && MemoryWatchers.EncounterFormationID2.Current == 2 && MemoryWatchers.RoomNumber.Current == 280 && MemoryWatchers.Storyline.Current == 1820; },
            new Transition { ForceLoad = false, RoomNumber = 280, Storyline = 1885, SpawnPoint = 0, Description = "Home Dual Horns", PositionTidusAfterLoad = true, Target_x = -2.414f, Target_y = 0.0f, Target_z = 76.495f, Target_rot = -4.692f, Target_var1 = 243} },
        { 
            () => { return MemoryWatchers.EncounterMapID.Current == 87 && MemoryWatchers.EncounterFormationID2.Current == 3 && MemoryWatchers.RoomNumber.Current == 280 && MemoryWatchers.Storyline.Current == 1885; }, 
            new Transition { RoomNumber = 280, Storyline = 1940, SpawnPoint = 4, Description = "Home Chimera"} },
        { 
            () => { return MemoryWatchers.EncounterMapID.Current == 52 && MemoryWatchers.EncounterFormationID2.Current == 0 && MemoryWatchers.RoomNumber.Current == 277 && MemoryWatchers.Storyline.Current == 2040; },
            new Transition { RoomNumber = 205, Storyline = 2080, TargetFramerate = 2, Description = "Evrae"} },
        { 
            () => { return MemoryWatchers.HpEnemyA.Current == 36000 && MemoryWatchers.Storyline.Current == 2280; }, 
            new Transition { RoomNumber = 183, Storyline = 2290, SpawnPoint = 4, Description = "Seymour Natus"} },
        { 
            () => { return MemoryWatchers.HpEnemyA.Current == 64000 && MemoryWatchers.EncounterMapID.Current == 61 && MemoryWatchers.EncounterFormationID1.Current == 0 && MemoryWatchers.EncounterFormationID2.Current == 0 && MemoryWatchers.RoomNumber.Current == 279 && MemoryWatchers.Storyline.Current == 2400; }, 
            new Transition { ForceLoad = false, Storyline = 2420, AddCalmLandsBitmask = 0x01, Description = "Defender X"} },
        { 
            () => { return MemoryWatchers.RoomNumber.Current == 259 && MemoryWatchers.Storyline.Current == 2510 && MemoryWatchers.State.Current == 2; }, 
            new Transition { RoomNumber = 259, Storyline = 2528, SpawnPoint = 1, PositionTidusAfterLoad = true, Target_x = 53.675f, Target_y = -36.270f, Target_z = 316.892f, Target_rot = 1.570f, Target_var1 = 475, Description = "Biran + Yenke"} },
        { 
            () => { return MemoryWatchers.HpEnemyA.Current == 70000 && MemoryWatchers.Storyline.Current == 2530; }, 
            new Transition { RoomNumber = 285, Storyline = 2560, SpawnPoint = 2, TargetFramerate = 2, MenuCleanup = true, AddRewardItems = true, PositionTidusAfterLoad = true, Target_x = 31.580f, Target_y = -19.991f, Target_z = -202.674f, Target_rot = -0.509f, Target_var1 = 662, Description = "Seymour Flux"} },
        { 
            () => { return MemoryWatchers.EncounterMapID.Current == 68 && MemoryWatchers.EncounterFormationID2.Current == 0 && MemoryWatchers.RoomNumber.Current == 311 && MemoryWatchers.Storyline.Current == 2585; }, 
            new Transition { RoomNumber = 311, Storyline = 2680, SpawnPoint = 0, PositionTidusAfterLoad = true, Target_x = 1167.635f, Target_y = -30.038f, Target_z = -1127.956f, Target_rot = -2.844f, Target_var1 = 794, Description = "Sanctuary Keeper"} },
        { 
            () => { return MemoryWatchers.EncounterMapID.Current == 71 && MemoryWatchers.EncounterFormationID2.Current == 0 && MemoryWatchers.RoomNumber.Current == 317 && MemoryWatchers.Storyline.Current == 2775; }, 
            new Transition { RoomNumber = 318, Storyline = 2815, Description = "Spectral Keeper to Yunalesca"} },
        { 
            () => { return MemoryWatchers.HpEnemyA.Current == 24000 && MemoryWatchers.Storyline.Current == 2815; }, 
            new Transition { RoomNumber = 270, Storyline = 2850, SpawnPoint = 0, TargetFramerate = 2, MenuCleanup = true, AddRewardItems = true, PositionTidusAfterLoad = true, Target_x = 1.958f, Target_y = 0.0f, Target_z = -68.687f, Target_rot = -1.438f, Target_var1 = 86, Description = "Yunalesca"} },
        { 
            () => { return MemoryWatchers.EncounterMapID.Current == 73 && MemoryWatchers.EncounterFormationID2.Current == 0 && MemoryWatchers.RoomNumber.Current == 199 && MemoryWatchers.Storyline.Current == 3085; }, 
            new Transition { RoomNumber = 200, Storyline = 3085, Description = "Left Fin"} },
        { 
            () => { return MemoryWatchers.EncounterMapID.Current == 74 && MemoryWatchers.EncounterFormationID2.Current == 0 && MemoryWatchers.RoomNumber.Current == 200 && MemoryWatchers.Storyline.Current == 3085; }, 
            new Transition { RoomNumber = 201, Storyline = 3105, Description = "Right Fin"} },
        { 
            () => { return MemoryWatchers.HpEnemyA.Current == 20000 && MemoryWatchers.RoomNumber.Current == 201 && MemoryWatchers.Storyline.Current == 3105; }, 
            new Transition { RoomNumber = 374, Storyline = 3125, SpawnPoint = 1, PositionTidusAfterLoad = true, Target_x = -258.472f, Target_y = 12.310f, Target_z = 380.587f, Target_rot = -1.021f, Target_var1 = 1412, Description = "Sin Core"} },
        { 
            () => { return MemoryWatchers.HpEnemyA.Current == 80000 && MemoryWatchers.Storyline.Current == 3205; }, 
            new Transition { Storyline = 3250, ForceLoad = false, Description = "Seymour Omnis"} },

        // Miscellaneous
        { 
            () => { return MemoryWatchers.EncounterMapID.Current == 63 && MemoryWatchers.EncounterFormationID1.Current == 1 && MemoryWatchers.EncounterFormationID2.Current == 0; },
            new Transition { EncounterFormationID1 = 2, RoomNumber = 56, SpawnPoint = 4, AddCalmLandsBitmask = 0x02, PositionTidusAfterLoad = true, Target_x = 94.667f, Target_y = 141.147f, Target_z = 1928.194f, Target_rot = 1.568f, Target_var1 = 80, Description = "Yojimbo"} }
    };
}
