namespace FFXCutsceneRemover.Resources;

/* Create MemoryLocation objects for all of your new memory addresses. You can add a deep pointer by specifying 1 or
 * more additional offsets. 
 *      new MemoryLocation("Example", 0x00F2FD00, 0x124);
 *      
 * To specify more than 1 additional offset, pass the additional offsets as an array:
 *      new MemoryLocation("Example", 0x00F2FD00, { 0x124, 0xD24, ... }); */
static class MemoryLocations
{
    public static MemoryLocation Language = new MemoryLocation("Language", 0x8DED48, 0x04);

    public static MemoryLocation RoomNumber          = new MemoryLocation("RoomNumber", 0xD2CA90);
    public static MemoryLocation Storyline           = new MemoryLocation("Storyline", 0xD2D67C);
    public static MemoryLocation ForceLoad           = new MemoryLocation("ForceLoad", 0xF3080C);
    public static MemoryLocation SpawnPoint          = new MemoryLocation("SpawnPoint", 0xD2CA9C);
    public static MemoryLocation BattleState         = new MemoryLocation("BattleState", 0xD2C9F0);
    public static MemoryLocation BattleState2        = new MemoryLocation("BattleState2", 0xD2A8E0);
    public static MemoryLocation Input               = new MemoryLocation("Input", 0x8CB170);
    public static MemoryLocation Menu                = new MemoryLocation("Menu", 0xF407E4);
    public static MemoryLocation MenuLock            = new MemoryLocation("MenuLock", 0xF25B61);
    public static MemoryLocation Intro               = new MemoryLocation("Intro", 0x922D64);
    public static MemoryLocation State               = new MemoryLocation("State", 0xD381AC);
    public static MemoryLocation XCoordinate         = new MemoryLocation("XCoordinate", 0xF25D80);
    public static MemoryLocation YCoordinate         = new MemoryLocation("YCoordinate", 0xF25D78);
    public static MemoryLocation Camera              = new MemoryLocation("Camera", 0xD3818C);
    public static MemoryLocation Camera_x            = new MemoryLocation("Camera_x", 0xD37B7C);
    public static MemoryLocation Camera_y            = new MemoryLocation("Camera_y", 0xD37B80);
    public static MemoryLocation Camera_z            = new MemoryLocation("Camera_z", 0xD37B84);
    public static MemoryLocation CameraRotation      = new MemoryLocation("CameraRotation", 0x8A858C);
    public static MemoryLocation EncounterStatus     = new MemoryLocation("EncounterStatus", 0xF25D70);
    public static MemoryLocation MovementLock        = new MemoryLocation("MovementLock", 0xF25B63);
    public static MemoryLocation ActiveMusicId       = new MemoryLocation("ActiveMusicId", 0xF270F0);
    public static MemoryLocation MusicId             = new MemoryLocation("MusicId", 0xF2FF1C);
    public static MemoryLocation RoomNumberAlt       = new MemoryLocation("RoomNumberAlt", 0xD2Ca92);
    public static MemoryLocation CutsceneAlt         = new MemoryLocation("CutsceneAlt", 0xD27C88);
    public static MemoryLocation AirshipDestinations = new MemoryLocation("AirshipDestinations", 0xD2D710);
    public static MemoryLocation AuronOverdrives     = new MemoryLocation("AuronOverdrives", 0xD307FC);
    public static MemoryLocation Gil                 = new MemoryLocation("Gil", 0xD307D8);
    //This is target framerate 0 = Uncapped??? / 1 = 60 / 2 = 30 / 3 = 20 / 4 = 15 / 5 = 12 / 6 = 10. Formula appears to be Target Framerate = 60 / this value
    public static MemoryLocation TargetFramerate = new MemoryLocation("TargetFramerate", 0x830E88);

    public static MemoryLocation PlayerTurn           = new MemoryLocation("PlayerTurn", 0xF3F77B);
    public static MemoryLocation FrameCounterFromLoad = new MemoryLocation("FrameCounterFromLoad", 0xF25D54);

    // Dialogue
    public static MemoryLocation Dialogue1           = new MemoryLocation("Dialogue1", 0xF25A80);
    public static MemoryLocation DialogueOption      = new MemoryLocation("DialogueOption", 0x146780A);
    public static MemoryLocation DialogueBoxOpen     = new MemoryLocation("DialogueBoxOpen", 0x1465CC2);
    public static MemoryLocation DialogueOption_Gui  = new MemoryLocation("DialogueOption_Gui", 0x1467942);
    public static MemoryLocation DialogueBoxOpen_Gui = new MemoryLocation("DialogueBoxOpen_Gui", 0x1465CDE);

    // Deep Pointers
    public static MemoryLocation HpEnemyA           = new MemoryLocation("HpEnemyA", 0xD34460, 0x5D0);
    public static MemoryLocation GuadoCount         = new MemoryLocation("GuadoCount", 0xF2FF14, 0x120);
    public static MemoryLocation NPCLastInteraction = new MemoryLocation("NPCLastInteraction", 0xF26AE8, 0x1E8);
    public static MemoryLocation TidusActionCount   = new MemoryLocation("TidusActionCount", 0xD334CC, 0x6DF);
    public static MemoryLocation TidusXCoordinate   = new MemoryLocation("TidusXCoordinate", 0x1FC44E4, 0x0C);
    public static MemoryLocation TidusYCoordinate   = new MemoryLocation("TidusYCoordinate", 0x1FC44E4, 0x10);
    public static MemoryLocation TidusZCoordinate   = new MemoryLocation("TidusZCoordinate", 0x1FC44E4, 0x14);
    public static MemoryLocation TidusRotation      = new MemoryLocation("TidusRotation", 0x1FC44E4, 0x168);
    public static MemoryLocation DialogueFile       = new MemoryLocation("DialogueFile", 0xF270E8, 0x00);
    public static MemoryLocation CutsceneTiming     = new MemoryLocation("CutsceneTiming", 0x8E9004, 0x1C);

    // Event File
    public static MemoryLocation EventFileStart = new MemoryLocation("EventFileStart", 0xF270B8);

    //Bespoke Transitions
    public static MemoryLocation AuronTransition             = new MemoryLocation("AuronTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x23 - 0x1 - 0x5 - 0x3) + 0x12C + 0x4C + 0x18 });
    public static MemoryLocation AmmesTransition             = new MemoryLocation("AmmesTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x4F - 0x1 - 0x9 - 0x0) + 0x12C + 0x4C + 0x18 + 0x558 });
    public static MemoryLocation TankerTransition            = new MemoryLocation("TankerTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x38 - 0x0 - 0x0 - 0x0) + 0x12C + 0x4C + 0x18 + 0x558 });
    public static MemoryLocation InsideSinTransition         = new MemoryLocation("InsideSinTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0xC - 0x2 - 0x5 - 0x0) + 0x12C + 0x4C + 0x18 + 0x558 });
    public static MemoryLocation DiveTransition              = new MemoryLocation("DiveTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x31 - 0xA - 0x2 - 0xA) + 0x12C + 0x4C + 0x18 });
    public static MemoryLocation GeosTransition              = new MemoryLocation("GeosTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x31 - 0xA - 0x2 - 0xA) + 0x12C + 0x4C + 0x18 + 0x4C });
    public static MemoryLocation KlikkTransition             = new MemoryLocation("KlikkTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x30 - 0x9 - 0x4 - 0x8) + 0x12C + 0x4C + 0x18 });
    public static MemoryLocation AlBhedBoatTransition        = new MemoryLocation("AlBhedBoatTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x23 - 0x0 - 0x3 - 0x3) + 0x12C + 0x4C + 0x18 });
    public static MemoryLocation UnderwaterRuinsTransition   = new MemoryLocation("UnderwaterRuinsTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x13 - 0x0 - 0x0 - 0x0) + 0x12C + 0x4C + 0x18 });
    public static MemoryLocation UnderwaterRuinsTransition2  = new MemoryLocation("UnderwaterRuinsTransition2", 0xF25B60 + 0x1C, new int[] { 0x18148 });
    public static MemoryLocation BeachTransition             = new MemoryLocation("BeachTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x43 - 0x1 - 0xD - 0x22) + 0x12C + 0x4C + 0x18 });
    public static MemoryLocation LagoonTransition1           = new MemoryLocation("LagoonTransition1", 0xF25B60 + 0x1C, new int[] { 0x068D8 });
    public static MemoryLocation LagoonTransition2           = new MemoryLocation("LagoonTransition2", 0xF25B60 + 0x1C, new int[] { 0x07398 });
    public static MemoryLocation ValeforTransition           = new MemoryLocation("ValeforTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x39 - 0x9 - 0x6 - 0x11) + 0x12C + 0x4C + 0x18 + 0x1560 });
    public static MemoryLocation KimahriTransition           = new MemoryLocation("KimahriTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x17 - 0x4 - 0x2 - 0x0) + 0x12C + 0x4C + 0x18 - 0x5A28 });
    public static MemoryLocation YunaBoatTransition          = new MemoryLocation("YunaBoatTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x52 - 0xE - 0xA - 0xA) + 0x12C + 0x4C + 0x18 + 0x558 });
    public static MemoryLocation SinFinTransition            = new MemoryLocation("SinFinTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x29 - 0x2 - 0x4 - 0xA) + 0x12C + 0x4C + 0x18 + 0x558 });
    public static MemoryLocation EchuillesTransition         = new MemoryLocation("EchuillesTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x11 - 0x0 - 0x4 - 0x0) + 0x12C + 0x4C + 0x18 + 0x558});
    public static MemoryLocation GeneauxTransition           = new MemoryLocation("GeneauxTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x18 - 0x2 - 0x4 - 0x3) + 0x12C + 0x4C + 0x18 });
    public static MemoryLocation KilikaTrialsTransition      = new MemoryLocation("KilikaTrialsTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x30 - 0xD - 0xC - 0x7) + 0x12C + 0x18 });
    public static MemoryLocation KilikaAntechamberTransition = new MemoryLocation("KilikaAntechamberTransition", 0xF25B60 + 0x1C, new int[] { 0xD918 });
    public static MemoryLocation IfritTransition             = new MemoryLocation("IfritTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x1B - 0x3 - 0x3 - 0x2) + 0x12C + 0x4C + 0x18 });
    public static MemoryLocation IfritTransition2            = new MemoryLocation("IfritTransition2", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x1B - 0x3 - 0x3 - 0x2) + 0x12C + 0x4C + 0x18 - 0x23D0 });
    public static MemoryLocation JechtShotTransition         = new MemoryLocation("JechtShotTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x31 - 0x7 - 0xB - 0xB) + 0x12C + 0x4C + 0x18 + 0xAB0 });
    public static MemoryLocation OblitzeratorTransition      = new MemoryLocation("OblitzeratorTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x13 - 0x0 - 0x0 - 0x0) + 0x12C + 0x4C + 0x18 });
    public static MemoryLocation BlitzballTransition         = new MemoryLocation("BlitzballTransition", 0xF25B60 + 0x1C, new int[] { 0x144});
    public static MemoryLocation SahaginTransition           = new MemoryLocation("SahaginTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x10 - 0x0 - 0x2 - 0x0) + 0x12C + 0x18 });
    public static MemoryLocation GarudaTransition            = new MemoryLocation("GarudaTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x1E - 0x0 - 0x5 - 0x0) + 0x12C + 0x18 });
    public static MemoryLocation RinTransition               = new MemoryLocation("RinTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x21 - 0x5 - 0x5 - 0x2) + 0x12C + 0x4C + 0x18 });
    public static MemoryLocation ChocoboEaterTransition      = new MemoryLocation("ChocoboEaterTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x35 - 0x3 - 0x8 - 0xA) + 0x12C + 0x4C + 0x18 });
    public static MemoryLocation GuiTransition               = new MemoryLocation("GuiTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x39 - 0x4 - 0x8 - 0xA) + 0x12C + 0x4C + 0x18 });
    public static MemoryLocation Gui2Transition              = new MemoryLocation("Gui2Transition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x16 - 0x1 - 0x6 - 0x5) + 0x12C + 0x4C + 0x18 });
    public static MemoryLocation DjoseTransition             = new MemoryLocation("DjoseTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x1D - 0x2 - 0x5 - 0x1) + 0x12C + 0x4C + 0x18 });
    public static MemoryLocation IxionTransition             = new MemoryLocation("IxionTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x1E - 0x2 - 0x7 - 0x0) + 0x12C + 0x4C + 0x18 });
    public static MemoryLocation ExtractorTransition         = new MemoryLocation("ExtractorTransition", 0xF25B60 + 0x1C, new int[] { 0x675C });
    public static MemoryLocation SeymoursHouseTransition1    = new MemoryLocation("SeymoursHouseTransition1", 0xF25B60 + 0x1C, new int[] { 0xD77C });
    public static MemoryLocation SeymoursHouseTransition2    = new MemoryLocation("SeymoursHouseTransition2", 0xF25B60 + 0x1C, new int[] { 0xD270 });
    public static MemoryLocation FarplaneTransition1         = new MemoryLocation("FarplaneTransition1", 0xF25B60 + 0x1C, new int[] { 0xDD7C });
    public static MemoryLocation FarplaneTransition2         = new MemoryLocation("FarplaneTransition2", 0xF25B60 + 0x1C, new int[] { 0xDDC8 });
    public static MemoryLocation TromellTransition           = new MemoryLocation("TromellTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x23 - 0x7 - 0x4 - 0x0) + 0x12C + 0x4C + 0x18 + 0xAB0 });
    public static MemoryLocation CrawlerTransition           = new MemoryLocation("CrawlerTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x1F - 0x2 - 0x5 - 0x0) + 0x12C + 0x4C + 0x18 + 0xFBC});
    public static MemoryLocation SeymourTransition           = new MemoryLocation("SeymourTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x14 - 0x0 - 0x0 - 0x0) + 0x12C + 0x4C + 0x18 });
    public static MemoryLocation SeymourTransition2          = new MemoryLocation("SeymourTransition2", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x14 - 0x0 - 0x0 - 0x0) + 0x12C + 0x4C + 0x18 + 0x4C });
    public static MemoryLocation WendigoTransition           = new MemoryLocation("WendigoTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x18 - 0x0 - 0x0 - 0x0) + 0x12C + 0x4C + 0x18 + 0xFBC });
    public static MemoryLocation SpherimorphTransition       = new MemoryLocation("SpherimorphTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x14 - 0x1 - 0x6 - 0x2) + 0x12C + 0x4C + 0x18 });
    public static MemoryLocation UnderLakeTransition         = new MemoryLocation("UnderLakeTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0xC - 0x0 - 0x0 - 0x0) + 0x12C + 0x4C + 0x18 });
    public static MemoryLocation BikanelTransition           = new MemoryLocation("BikanelTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x42 - 0x6 - 0xC - 0x18) + 0x12C + 0x4C + 0x18 });
    public static MemoryLocation HomeTransition              = new MemoryLocation("HomeTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x2D - 0x6 - 0x9 - 0xA) + 0x12C + 0x4C + 0x18 });
    public static MemoryLocation EvraeTransition             = new MemoryLocation("EvraeTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x26 - 0x2 - 0x6 - 0x1) + 0x12C + 0x18 });
    public static MemoryLocation EvraeAirshipTransition      = new MemoryLocation("EvraeAirshipTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x20 - 0x3 - 0x5 - 0x5) + 0x12C + 0x18 });
    public static MemoryLocation GuardsTransition            = new MemoryLocation("GuardsTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x3C - 0x6 - 0x2 - 0x5) + 0x12C + 0x4C + 0x18 - 0x11BE4 });
    public static MemoryLocation BahamutTransition           = new MemoryLocation("BahamutTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x27 - 0x1 - 0x2 - 0x0) + 0x12C + 0x4C + 0x18 - 0x9F1C });
    public static MemoryLocation IsaaruTransition            = new MemoryLocation("IsaaruTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x43 - 0xE - 0x5 - 0x17) + 0x12C + 0x4C + 0x18 + 0x1B9C});
    public static MemoryLocation AltanaTransition            = new MemoryLocation("AltanaTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x17 - 0x0 - 0x0 - 0x0) + 0x12C + 0x4C + 0x18 + 0xB94 });
    public static MemoryLocation NatusTransition             = new MemoryLocation("NatusTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x1D - 0x0 - 0x0 - 0x0) + 0x12C + 0x4C + 0x18 - 0x4378 });
    public static MemoryLocation DefenderXTransition         = new MemoryLocation("DefenderXTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x20 - 0x3 - 0x7 - 0x9) + 0x12C + 0x4C + 0x18 + 0x4C });
    public static MemoryLocation RonsoTransition             = new MemoryLocation("RonsoTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x27 - 0x5 - 0x6 - 0x2) + 0x12C + 0x4C + 0x18 });
    public static MemoryLocation FluxTransition              = new MemoryLocation("FluxTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x15 - 0x2 - 0x6 - 0x1) + 0x12C + 0x4C + 0x18});
    public static MemoryLocation SanctuaryTransition         = new MemoryLocation("SanctuaryTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x15 - 0x2 - 0x7 - 0x1) + 0x12C + 0x4C + 0x18 });
    public static MemoryLocation SpectralKeeperTransition    = new MemoryLocation("SpectralKeeperTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x11 - 0x0 - 0x6 - 0x0) + 0x12C + 0x4C + 0x18});
    public static MemoryLocation SpectralKeeperTransition2   = new MemoryLocation("SpectralKeeperTransition2", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x55 - 0xF - 0x7 - 0x1E) + 0x12C + 0x4C + 0x18});
    public static MemoryLocation YunalescaTransition         = new MemoryLocation("YunalescaTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x1B - 0x1 - 0x7 - 0x2) + 0x12C + 0x4C + 0x18});
    public static MemoryLocation FinsTransition              = new MemoryLocation("FinsTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x12 - 0x0 - 0x2 - 0x0) + 0x12C + 0x18 });
    public static MemoryLocation FinsAirshipTransition       = new MemoryLocation("FinsAirshipTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x1F - 0x3 - 0x5 - 0x5) + 0x12C + 0x18 });
    public static MemoryLocation SinCoreTransition           = new MemoryLocation("SinCoreTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0xF - 0x0 - 0x0 - 0x0) + 0x12C + 0x4C + 0x18 - 0x3904 });
    public static MemoryLocation OverdriveSinTransition      = new MemoryLocation("OverdriveSinTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x22 - 0x2 - 0x5 - 0x1) + 0x12C + 0x4C + 0x18 - 0x4C });
    public static MemoryLocation OmnisTransition             = new MemoryLocation("OmnisTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x18 - 0x2 - 0x3 - 0x1) + 0x12C + 0x4C + 0x18 });
    public static MemoryLocation BFATransition               = new MemoryLocation("BFATransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x1B - 0x0 - 0x5 - 0x1) + 0x12C + 0x4C + 0x18 });
    public static MemoryLocation AeonTransition              = new MemoryLocation("AeonTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x1D - 0x0 - 0x3 - 0x0) + 0x12C + 0x4C + 0x18 });
    public static MemoryLocation YuYevonTransition           = new MemoryLocation("YuYevonTransition", 0xF25B60 + 0x1C, new int[] { 0xB58 * (0x1D - 0x0 - 0x3 - 0x0) + 0x12C + 0x4C + 0x18 + 0x4C });

    public static MemoryLocation YojimboFaythTransition = new MemoryLocation("YojimboFaythTransition", 0xF25B60 + 0x1C, new int[] { 0x9508 });

    public static MemoryLocation CutsceneProgress_Max   = new MemoryLocation("CutsceneProgress_Max", 0xF26AE8, 0xC);
    public static MemoryLocation CutsceneProgress_uVar1 = new MemoryLocation("CutsceneProgress_uVar1", 0xF26AE8, 0x14);
    public static MemoryLocation CutsceneProgress_uVar2 = new MemoryLocation("CutsceneProgress_uVar2", 0xF26AE8, 0x16);
    public static MemoryLocation CutsceneProgress_uVar3 = new MemoryLocation("CutsceneProgress_uVar3", 0xF26AE8, 0x18);

    // Encounters
    public static MemoryLocation EncounterMapID        = new MemoryLocation("EncounterMapID", 0xD2C256);
    public static MemoryLocation EncounterFormationID1 = new MemoryLocation("EncounterFormationID1", 0xD2C258);
    public static MemoryLocation EncounterFormationID2 = new MemoryLocation("EncounterFormationID2", 0xD2C259);
    public static MemoryLocation ScriptedBattleFlag1   = new MemoryLocation("ScriptedBattleFlag1", 0xD2A9D4); // Setting to 0 triggers a post battle rewards screen
    public static MemoryLocation ScriptedBattleFlag2   = new MemoryLocation("ScriptedBattleFlag2", 0xD2A9D5); // 0 = Screen crack effect, 1 = Boss transition effect
    public static MemoryLocation ScriptedBattleVar1    = new MemoryLocation("ScriptedBattleVar1", 0xF26B08); // Set to the right value to make the game recognise battle as scripted and not random
    public static MemoryLocation ScriptedBattleVar3    = new MemoryLocation("ScriptedBattleVar3", 0xF26B10); // Set at the start of a scripted battle
    public static MemoryLocation ScriptedBattleVar4    = new MemoryLocation("ScriptedBattleVar4", 0xF26B14); // Set at the start of a scripted battle
    public static MemoryLocation EncounterTrigger      = new MemoryLocation("EncounterTrigger", 0xD2A8E2); // Set to 2 to trigger boss encounter

    // Party Configuration
    public static MemoryLocation Formation     = new MemoryLocation("Formation", 0xD307E8);
    public static MemoryLocation RikkuName     = new MemoryLocation("RikkuName", 0xD32E54);
    public static MemoryLocation EnableTidus   = new MemoryLocation("EnableTidus", 0xD32088);
    public static MemoryLocation EnableYuna    = new MemoryLocation("EnableYuna", 0xD3211C);
    public static MemoryLocation EnableAuron   = new MemoryLocation("EnableAuron", 0xD321B0);
    public static MemoryLocation EnableKimahri = new MemoryLocation("EnableKimahri", 0xD32244);
    public static MemoryLocation EnableWakka   = new MemoryLocation("EnableWakka", 0xD322D8);
    public static MemoryLocation EnableLulu    = new MemoryLocation("EnableLulu", 0xD3236C);
    public static MemoryLocation EnableRikku   = new MemoryLocation("EnableRikku", 0xD32400);
    public static MemoryLocation EnableSeymour = new MemoryLocation("EnableSeymour", 0xD32494);
    public static MemoryLocation EnableValefor = new MemoryLocation("EnableValefor", 0xD32528);
    public static MemoryLocation EnableIfrit   = new MemoryLocation("EnableIfrit", 0xD325BC);
    public static MemoryLocation EnableIxion   = new MemoryLocation("EnableIxion", 0xD32650);
    public static MemoryLocation EnableShiva   = new MemoryLocation("EnableShiva", 0xD326E4);
    public static MemoryLocation EnableBahamut = new MemoryLocation("EnableBahamut", 0xD32778);
    public static MemoryLocation EnableAnima   = new MemoryLocation("EnableYojimbo", 0xD3280C);
    public static MemoryLocation EnableYojimbo = new MemoryLocation("EnableAnima", 0xD328A0);
    public static MemoryLocation EnableMagus   = new MemoryLocation("EnableMagus", 0xD32934);

    // HP/MP TODO: Find a better method for full party restore to clean this up
    public static MemoryLocation TidusHP    = new MemoryLocation("TidusHP", 0xD32078);
    public static MemoryLocation TidusMaxHP = new MemoryLocation("TidusMaxHP", 0xD32080);
    public static MemoryLocation TidusMP    = new MemoryLocation("TidusMP", 0xD3207C);
    public static MemoryLocation TidusMaxMP = new MemoryLocation("TidusMaxMP", 0xD32084);
    
    public static MemoryLocation YunaHP    = new MemoryLocation("YunaHP", 0xD3210C);
    public static MemoryLocation YunaMaxHP = new MemoryLocation("YunaMaxHP", 0xD32114);
    public static MemoryLocation YunaMP    = new MemoryLocation("YunaMP", 0xD32110);
    public static MemoryLocation YunaMaxMP = new MemoryLocation("YunaMaxMP", 0xD32118);
    
    public static MemoryLocation AuronHP    = new MemoryLocation("AuronHP", 0xD321A0);
    public static MemoryLocation AuronMaxHP = new MemoryLocation("AuronMaxHP", 0xD321A8);
    public static MemoryLocation AuronMP    = new MemoryLocation("AuronMP", 0xD321A4);
    public static MemoryLocation AuronMaxMP = new MemoryLocation("AuronMaxMP", 0xD321AC);
    
    public static MemoryLocation KimahriHP    = new MemoryLocation("KimahriHP", 0xD32234);
    public static MemoryLocation KimahriMaxHP = new MemoryLocation("KimahriMaxHP", 0xD3223C);
    public static MemoryLocation KimahriMP    = new MemoryLocation("KimahriMP", 0xD32238);
    public static MemoryLocation KimahriMaxMP = new MemoryLocation("KimahriMaxMP", 0xD32240);
    
    public static MemoryLocation WakkaHP    = new MemoryLocation("WakkaHP", 0xD322C8);
    public static MemoryLocation WakkaMaxHP = new MemoryLocation("WakkaMaxHP", 0xD322D0);
    public static MemoryLocation WakkaMP    = new MemoryLocation("WakkaMP", 0xD322CC);
    public static MemoryLocation WakkaMaxMP = new MemoryLocation("WakkaMaxMP", 0xD322D4);
    
    public static MemoryLocation LuluHP    = new MemoryLocation("LuluHP", 0xD3235C);
    public static MemoryLocation LuluMaxHP = new MemoryLocation("LuluMaxHP", 0xD32364);
    public static MemoryLocation LuluMP    = new MemoryLocation("LuluMP", 0xD32360);
    public static MemoryLocation LuluMaxMP = new MemoryLocation("LuluMaxMP", 0xD32368);
    
    public static MemoryLocation RikkuHP    = new MemoryLocation("RikkuHP", 0xD323F0);
    public static MemoryLocation RikkuMaxHP = new MemoryLocation("RikkuMaxHP", 0xD323F8);
    public static MemoryLocation RikkuMP    = new MemoryLocation("RikkuMP", 0xD323F4);
    public static MemoryLocation RikkuMaxMP = new MemoryLocation("RikkuMaxMP", 0xD323FC);
    
    // Aeons
    // Only adding Valefor (for now?), as this is currently the only aeon that could affect speedruns
    public static MemoryLocation ValeforHP    = new MemoryLocation("ValeforHP", 0xD32518);
    public static MemoryLocation ValeforMaxHP = new MemoryLocation("ValeforMaxHP", 0xD32520);
    public static MemoryLocation ValeforMP    = new MemoryLocation("ValeforMP", 0xD3251C);
    public static MemoryLocation ValeforMaxMP = new MemoryLocation("ValeforMaxMP", 0xD32524);

    // Encounter Rate
    public static MemoryLocation EncountersActiveFlag = new MemoryLocation("EncountersActiveFlag", 0xD2A9D7);
    public static MemoryLocation TotalDistance        = new MemoryLocation("TotalDistance", 0xD2A9DC);
    public static MemoryLocation CycleDistance        = new MemoryLocation("CycleDistance", 0xD2A9D8);

    // Special Flags
    public static MemoryLocation FangirlsOrKidsSkip     = new MemoryLocation("FangirlsOrKidsSkip", 0xD2CE7C);
    public static MemoryLocation BaajFlag1              = new MemoryLocation("BaajFlag1", 0xD2CE0C);
    public static MemoryLocation BesaidFlag1            = new MemoryLocation("BesaidFlag1", 0xF25AB3);
    public static MemoryLocation SSWinnoFlag1           = new MemoryLocation("SSWinnoFlag1", 0xD2CE7D);
    public static MemoryLocation KilikaMapFlag          = new MemoryLocation("KilikaMapFlag", 0xD2CE7E);
    public static MemoryLocation SSWinnoFlag2           = new MemoryLocation("SSWinnoFlag2", 0xD2CE7F);
    public static MemoryLocation LucaFlag               = new MemoryLocation("LucaFlag", 0xD2CDE5);
    public static MemoryLocation LucaFlag2              = new MemoryLocation("LucaFlag2", 0xD2CDE4);
    public static MemoryLocation BlitzballFlag          = new MemoryLocation("BlitzballFlag", 0xD2E10A);
    public static MemoryLocation MiihenFlag1            = new MemoryLocation("MiihenFlag1", 0xD2CCFE);
    public static MemoryLocation MiihenFlag2            = new MemoryLocation("MiihenFlag2", 0xD2CCFF);
    public static MemoryLocation MiihenFlag3            = new MemoryLocation("MiihenFlag3", 0xD2CD00);
    public static MemoryLocation MiihenFlag4            = new MemoryLocation("MiihenFlag4", 0xD2CD04);
    public static MemoryLocation MRRFlag1               = new MemoryLocation("MRRFlag1", 0xD2CD07);
    public static MemoryLocation MRRFlag2               = new MemoryLocation("MRRFlag2", 0xD2CD08);
    public static MemoryLocation MoonflowFlag           = new MemoryLocation("MoonflowFlag", 0xD2CC7F);
    public static MemoryLocation MoonflowFlag2          = new MemoryLocation("MoonflowFlag2", 0xD2CC83);
    public static MemoryLocation RikkuOutfit            = new MemoryLocation("RikkuOutfit", 0xD2CB61);
    public static MemoryLocation TidusWeaponDamageBoost = new MemoryLocation("TidusWeaponDamageBoost", 0x1F11240);
    public static MemoryLocation GuadosalamShopFlag     = new MemoryLocation("GuadosalamShopFlag", 0xD2CD84);
    public static MemoryLocation ThunderPlainsFlag      = new MemoryLocation("ThunderPlainsFlag", 0xD2CE81);
    public static MemoryLocation MacalaniaFlag          = new MemoryLocation("MacalaniaFlag", 0xD2CD16);
    public static MemoryLocation BikanelFlag            = new MemoryLocation("BikanelFlag", 0xD2CD4B);
    public static MemoryLocation Sandragoras            = new MemoryLocation("Sandragoras", 0xD2CD4E);
    public static MemoryLocation ViaPurificoPlatform    = new MemoryLocation("ViaPurificoPlatform", 0xD2CC89);
    public static MemoryLocation NatusFlag              = new MemoryLocation("NatusFlag", 0xD2CC7C);
    public static MemoryLocation CalmLandsFlag          = new MemoryLocation("CalmLandsFlag", 0xD2CD09);
    public static MemoryLocation WantzFlag              = new MemoryLocation("WantzFlag", 0xD2CF06);
    public static MemoryLocation GagazetCaveFlag        = new MemoryLocation("GagazetCaveFlag", 0xD2CD55);
    public static MemoryLocation OmegaRuinsFlag         = new MemoryLocation("OmegaRuinsFlag", 0xD2CE4E);
    public static MemoryLocation WantzMacalaniaFlag     = new MemoryLocation("WantzMacalaniaFlag", 0xD2D6E1);


    // Blitzball
    public static MemoryLocation AurochsTeamBytes = new MemoryLocation("AurochsTeamBytes", 0xD2D704);
    public static MemoryLocation BlitzballBytes   = new MemoryLocation("BlitzballBytes", 0xD2DC7C);
    public static MemoryLocation AurochsPlayer1   = new MemoryLocation("AurochsPlayer1", 0xD2E0BE);

    // Battle Rewards
    public static MemoryLocation GilBattleRewards       = new MemoryLocation("GilBattleRewards", 0x1F10F6C);
    public static MemoryLocation BattleRewardItemCount  = new MemoryLocation("BattleRewardItemCount", 0x1F10F70);
    public static MemoryLocation BattleRewardItem1      = new MemoryLocation("BattleRewardItem1", 0x1F10F74);
    public static MemoryLocation BattleRewardItem2      = new MemoryLocation("BattleRewardItem2", 0x1F10F76);
    public static MemoryLocation BattleRewardItem3      = new MemoryLocation("BattleRewardItem3", 0x1F10F78);
    public static MemoryLocation BattleRewardItem4      = new MemoryLocation("BattleRewardItem4", 0x1F10F7A);
    public static MemoryLocation BattleRewardItem5      = new MemoryLocation("BattleRewardItem5", 0x1F10F7C);
    public static MemoryLocation BattleRewardItem6      = new MemoryLocation("BattleRewardItem6", 0x1F10F7E);
    public static MemoryLocation BattleRewardItem7      = new MemoryLocation("BattleRewardItem7", 0x1F10F80);
    public static MemoryLocation BattleRewardItem8      = new MemoryLocation("BattleRewardItem8", 0x1F10F82);
    public static MemoryLocation BattleRewardItemQty1   = new MemoryLocation("BattleRewardItemQty1", 0x1F10F84);
    public static MemoryLocation BattleRewardItemQty2   = new MemoryLocation("BattleRewardItemQty2", 0x1F10F85);
    public static MemoryLocation BattleRewardItemQty3   = new MemoryLocation("BattleRewardItemQty3", 0x1F10F86);
    public static MemoryLocation BattleRewardItemQty4   = new MemoryLocation("BattleRewardItemQty4", 0x1F10F87);
    public static MemoryLocation BattleRewardItemQty5   = new MemoryLocation("BattleRewardItemQty5", 0x1F10F88);
    public static MemoryLocation BattleRewardItemQty6   = new MemoryLocation("BattleRewardItemQty6", 0x1F10F89);
    public static MemoryLocation BattleRewardItemQty7   = new MemoryLocation("BattleRewardItemQty7", 0x1F10F8A);
    public static MemoryLocation BattleRewardItemQty8   = new MemoryLocation("BattleRewardItemQty8", 0x1F10F8B);
    public static MemoryLocation BattleRewardEquipCount = new MemoryLocation("BattleRewardItemCount", 0x1F10F72);
    public static MemoryLocation BattleRewardEquip1     = new MemoryLocation("BattleRewardEquip1", 0x1F10F9E);
    public static MemoryLocation BattleRewardEquip2     = new MemoryLocation("BattleRewardEquip2", 0x1F10FC0);
    public static MemoryLocation BattleRewardEquip3     = new MemoryLocation("BattleRewardEquip3", 0x1F10FE2);
    public static MemoryLocation BattleRewardEquip4     = new MemoryLocation("BattleRewardEquip4", 0x1F11004);
    public static MemoryLocation BattleRewardEquip5     = new MemoryLocation("BattleRewardEquip5", 0x1F11026);
    public static MemoryLocation BattleRewardEquip6     = new MemoryLocation("BattleRewardEquip6", 0x1F11048);
    public static MemoryLocation BattleRewardEquip7     = new MemoryLocation("BattleRewardEquip7", 0x1F1106A);
    public static MemoryLocation BattleRewardEquip8     = new MemoryLocation("BattleRewardEquip8", 0x1F1108C);

    // Items
    public static MemoryLocation ItemsStart    = new MemoryLocation("ItemsStart", 0xD3095C);
    public static MemoryLocation ItemsQtyStart = new MemoryLocation("ItemsQtyStart", 0xD30B5C);

    // AP
    public static MemoryLocation CharacterAPFlags = new MemoryLocation("CharacterAPFlags", 0x1F10EC4);

    // Menu Trigger
    public static MemoryLocation MenuTriggerValue = new MemoryLocation("MenuTriggerValue", 0xEFBBF0);

    // Menu Values - These are values which the game sets during the battle rewards menu which don't get cleaned out properly when we skip straight to the rewards screen.
    //               We set the values to 0x00000000 to clean these out after some skips
    public static MemoryLocation MenuValue1 = new MemoryLocation("MenuValue1", 0x14408AC);
    public static MemoryLocation MenuValue2 = new MemoryLocation("MenuValue2", 0xF27F10);

    // Menu Values - These are values which need to be set correctly to avoid menu bug where the menu can't open
    public static MemoryLocation MenuValue3 = new MemoryLocation("MenuValue3", 0x840E18);
    public static MemoryLocation MenuValue4 = new MemoryLocation("MenuValue4", 0xD2A00C);
    public static MemoryLocation MenuValue5 = new MemoryLocation("MenuValue5", 0x8DED2C, 0x6D0);
    public static MemoryLocation MenuValue6 = new MemoryLocation("MenuValue6", 0x8DED2C, 0x704);
    public static MemoryLocation MenuValue7 = new MemoryLocation("MenuValue7", 0x8CB9D8, 0x10D2E);

    // Booster Values
    public static MemoryLocation SpeedBoostAmount = new MemoryLocation("SpeedBoostAmount", 0x8E82A4);
    public static MemoryLocation SpeedBoostVar1   = new MemoryLocation("SpeedBoostVar1", 0x85A068);

    // Actor Model Positions
    public static MemoryLocation ActorArrayLength = new MemoryLocation("ActorArrayLength", 0x1FC44E0);

    // RNGmod
    public static MemoryLocation RNGArrayOpBytes = new MemoryLocation("RNGArrayOpBytes", 0x398903);

}

public struct MemoryLocation
{
    public string Name;
    public int BaseAddress;
    // For DeepPointers
    public int[] Offsets;

    public MemoryLocation(string name, int baseAddress, int offset)
    {
        Name = name;
        BaseAddress = baseAddress;
        Offsets = new int[] { offset };
    }

    public MemoryLocation(string name, int baseAddress, int[] offsets = null)
    {
        Name = name;
        BaseAddress = baseAddress;
        Offsets = offsets ?? new int[0];
    }
}
