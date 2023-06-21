using System;
using System.Diagnostics;
using System.Reflection;

using FFXCutsceneRemover.ComponentUtil;
using FFXCutsceneRemover.Logging;

namespace FFXCutsceneRemover;

/* Represents a change in current state of the game's memory. Create one of these objects
 * with the values you care about, and Execute() will set the game's state to match this object. */
public class Transition
{
    private static string DEFAULT_DESCRIPTION = "Executing Transition - No Description";
    private static FieldInfo[] publicFields = typeof(Transition).GetFields(BindingFlags.Public | BindingFlags.Instance);

    private Process process;

    public bool ConsoleOutput = true;
    public bool ForceLoad = true;
    public bool FullHeal = false;
    public bool MenuCleanup = false;
    public bool AddRewardItems = false;
    public bool AddSinLocation = false;
    public bool RemoveSinLocation = false;
    public bool PositionPartyOffScreen = false;
    public bool PositionTidusAfterLoad = false;
    public string Description = null;
    public int BaseCutsceneValue = 0;
    public int BaseCutsceneValue2 = 0;
    public bool Repeatable = false;
    public bool Suspendable = true;
    public int Stage = 0;
    public (byte itemref, byte itemqty)[] AddItemsToInventory = null;

    public int? ActorArrayLength = null;
    public short[] TargetActorIDs = null;
    public float? Target_x = null;
    public float? Target_y = null;
    public float? Target_z = null;
    public float? Target_rot = null;
    public short? Target_var1 = null;
    public byte? MoveFrame = 8; // Default to 8 Frames as this seems to work for most transitions
    public float? PartyTarget_x = null;
    public float? PartyTarget_y = null;
    public float? PartyTarget_z = null;

    public formations? FormationSwitch = null;

    /* Only add members here for memory addresses that we want to write the value to.
     * If we only ever read the value then there is no need to add it here. */
    public short? RoomNumber = null;
    public short? Storyline = null;
    public byte? SpawnPoint = null;
    public short? BattleState = null;
    public short? BattleState2 = null;
    public byte? Menu = null;
    public byte? MenuLock = null;
    public short? Intro = null;
    public short? FangirlsOrKidsSkip = null;
    public sbyte? State = null;
    public float? XCoordinate = null;
    public float? YCoordinate = null;
    public byte? Camera = null;
    public float? Camera_x = null;
    public float? Camera_y = null;
    public float? Camera_z = null;
    public float? CameraRotation = null;
    public byte? EncounterStatus = null;
    public byte? MovementLock = null;
    public byte? ActiveMusicId = null;
    public byte? MusicId = null;
    public short? RoomNumberAlt = null;
    public short? CutsceneAlt = null;
    public short? AirshipDestinations = null;
    public short? AirshipDestinationChange = null;
    public byte? AuronOverdrives = null;
    public int? TargetFramerate = null;
    public byte? PartyMembers = null;
    public byte? Sandragoras = null;
    public short? EncounterMapID = null;
    public byte? EncounterFormationID1 = null;
    public byte? EncounterFormationID2 = null;
    public byte? ScriptedBattleFlag1 = null;
    public byte? ScriptedBattleFlag2 = null;
    public int? ScriptedBattleVar1 = null;
    public int? ScriptedBattleVar3 = null;
    public int? ScriptedBattleVar4 = null;
    public byte? EncounterTrigger = null;
    public int? HpEnemyA = null;
    public byte? GuadoCount = null;
    public float? TidusXCoordinate = null;
    public float? TidusYCoordinate = null;
    public float? TidusZCoordinate = null;
    public float? TidusRotation = null;
    public byte[] DialogueFile = null;
    public byte? CutsceneTiming = null;

    // Bespoke Transitions
    public int? AuronTransition = null;
    public int? AmmesTransition = null;
    public int? TankerTransition = null;
    public int? InsideSinTransition = null;
    public int? DiveTransition = null;
    public int? GeosTransition = null;
    public int? KlikkTransition = null;
    public int? AlBhedBoatTransition = null;
    public int? UnderwaterRuinsTransition = null;
    public int? UnderwaterRuinsTransition2 = null;
    public int? BeachTransition = null;
    public int? LagoonTransition1 = null;
    public int? LagoonTransition2 = null;
    public int? ValeforTransition = null;
    public int? KimahriTransition = null;
    public int? YunaBoatTransition = null;
    public int? SinFinTransition = null;
    public int? EchuillesTransition = null;
    public int? GeneauxTransition = null;
    public int? KilikaTrialsTransition = null;
    public int? KilikaAntechamberTransition = null;
    public int? IfritTransition = null;
    public int? IfritTransition2 = null;
    public int? JechtShotTransition = null;
    public int? OblitzeratorTransition = null;
    public int? BlitzballTransition = null;
    public int? SahaginTransition = null;
    public int? GarudaTransition = null;
    public int? RinTransition = null;
    public int? ChocoboEaterTransition = null;
    public int? GuiTransition = null;
    public int? Gui2Transition = null;
    public int? DjoseTransition = null;
    public int? IxionTransition = null;
    public int? ExtractorTransition = null;
    public int? SeymoursHouseTransition1 = null;
    public int? SeymoursHouseTransition2 = null;
    public int? FarplaneTransition1 = null;
    public int? FarplaneTransition2 = null;
    public int? TromellTransition = null;
    public int? CrawlerTransition = null;
    public int? SeymourTransition = null;
    public int? SeymourTransition2 = null;
    public int? WendigoTransition = null;
    public int? SpherimorphTransition = null;
    public int? UnderLakeTransition = null;
    public int? BikanelTransition = null;
    public int? HomeTransition = null;
    public int? EvraeTransition = null;
    public int? EvraeAirshipTransition = null;
    public int? GuardsTransition = null;
    public int? BahamutTransition = null;
    public int? IsaaruTransition = null;
    public int? AltanaTransition = null;
    public int? NatusTransition = null;
    public int? DefenderXTransition = null;
    public int? RonsoTransition = null;
    public int? FluxTransition = null;
    public int? SanctuaryTransition = null;
    public int? SpectralKeeperTransition = null;
    public int? SpectralKeeperTransition2 = null;
    public int? YunalescaTransition = null;
    public int? FinsTransition = null;
    public int? FinsAirshipTransition = null;
    public int? SinCoreTransition = null;
    public int? OverdriveSinTransition = null;
    public int? OmnisTransition = null;
    public int? BFATransition = null;
    public int? AeonTransition = null;
    public int? YuYevonTransition = null;
    public int? YojimboFaythTransition = null;

    public byte? EnableTidus = null;
    public byte? EnableYuna = null;
    public byte? EnableAuron = null;
    public byte? EnableKimahri = null;
    public byte? EnableWakka = null;
    public byte? EnableLulu = null;
    public byte? EnableRikku = null;
    public byte? EnableSeymour = null;
    public byte? EnableValefor = null;
    public byte? EnableIfrit = null;
    public byte? EnableIxion = null;
    public byte? EnableShiva = null;
    public byte? EnableBahamut = null;
    public byte? EnableAnima = null;
    public byte? EnableYojimbo = null;
    public byte? EnableMagus = null;

    public byte? EncountersActiveFlag = null;
    public float? TotalDistance = null;
    public float? CycleDistance = null;

    public byte? BaajFlag1 = null;

    public byte? SSWinnoFlag1 = null;
    public byte? KilikaMapFlag = null;
    public byte? SSWinnoFlag2 = null;

    public byte? LucaFlag = null;
    public byte? LucaFlag2 = null;
    public byte? BlitzballFlag = null;

    public byte? MiihenFlag1 = null;
    public byte? MiihenFlag2 = null;
    public byte? MiihenFlag3 = null;
    public byte? MiihenFlag4 = null;

    public byte? MRRFlag1 = null;
    public byte? MRRFlag2 = null;

    public byte? MoonflowFlag = null;
    public byte? MoonflowFlag2 = null;
    public byte? RikkuOutfit = null;
    public byte? TidusWeaponDamageBoost = null;
    public byte? GuadosalamShopFlag = null;
    public byte? ThunderPlainsFlag = null;
    public byte? MacalaniaFlag = null;
    public byte? BikanelFlag = null;

    public byte[] Formation = null;
    public byte[] RikkuName = null;

    public byte? ViaPurificoPlatform = null;
    public byte? NatusFlag = null;
    public ushort? CalmLandsFlag = null;
    public short? GagazetCaveFlag = null;
    public byte? WantzFlag = null;
    public byte? OmegaRuinsFlag = null;
    public byte? WantzMacalaniaFlag = null;

    public byte[] AurochsTeamBytes = null;
    public byte[] BlitzballBytes = null;
    public byte? AurochsPlayer1 = null;

    public int? GilBattleRewards = null;
    public byte? BattleRewardItemCount = null;
    public short? BattleRewardItem1 = null;
    public short? BattleRewardItem2 = null;
    public short? BattleRewardItem3 = null;
    public short? BattleRewardItem4 = null;
    public short? BattleRewardItem5 = null;
    public short? BattleRewardItem6 = null;
    public short? BattleRewardItem7 = null;
    public short? BattleRewardItem8 = null;
    public byte? BattleRewardItemQty1 = null;
    public byte? BattleRewardItemQty2 = null;
    public byte? BattleRewardItemQty3 = null;
    public byte? BattleRewardItemQty4 = null;
    public byte? BattleRewardItemQty5 = null;
    public byte? BattleRewardItemQty6 = null;
    public byte? BattleRewardItemQty7 = null;
    public byte? BattleRewardItemQty8 = null;
    public byte? BattleRewardEquipCount = null;
    public byte[] BattleRewardEquip1 = null;
    public byte[] BattleRewardEquip2 = null;
    public byte[] BattleRewardEquip3 = null;
    public byte[] BattleRewardEquip4 = null;
    public byte[] BattleRewardEquip5 = null;
    public byte[] BattleRewardEquip6 = null;
    public byte[] BattleRewardEquip7 = null;
    public byte[] BattleRewardEquip8 = null;

    public byte[] ItemsStart = null;
    public byte[] ItemsQtyStart = null;

    public byte[] CharacterAPFlags = null;

    public int? MenuValue1 = null;
    public int? MenuValue2 = null;
    public int? MenuTriggerValue = null;

    public byte[] RNGArrayOpBytes = null;

    // Bitmask Addition
    public int? AddCalmLandsBitmask = null;

    public virtual void Execute(string defaultDescription = "")
    {
        if (ConsoleOutput)
        {
            DiagnosticLog.Information(
                !string.IsNullOrEmpty(Description) ? Description :
                !string.IsNullOrEmpty(defaultDescription) ? defaultDescription :
                DEFAULT_DESCRIPTION);
        }
        // Always update to get the latest process
        process = MemoryWatchers.Process;

        WriteValue(MemoryWatchers.RoomNumber, RoomNumber);
        WriteValue(MemoryWatchers.Storyline, Storyline);
        WriteValue(MemoryWatchers.SpawnPoint, SpawnPoint);
        WriteValue(MemoryWatchers.BattleState, BattleState);
        WriteValue(MemoryWatchers.BattleState2, BattleState2);
        WriteValue(MemoryWatchers.Menu, Menu);
        WriteValue(MemoryWatchers.MenuLock, MenuLock);
        WriteValue(MemoryWatchers.Intro, Intro);
        WriteValue(MemoryWatchers.FangirlsOrKidsSkip, FangirlsOrKidsSkip);
        WriteValue(MemoryWatchers.State, State);
        WriteValue(MemoryWatchers.XCoordinate, XCoordinate);
        WriteValue(MemoryWatchers.YCoordinate, YCoordinate);
        WriteValue(MemoryWatchers.Camera, Camera);
        WriteValue(MemoryWatchers.Camera_x, Camera_x);
        WriteValue(MemoryWatchers.Camera_y, Camera_y);
        WriteValue(MemoryWatchers.Camera_z, Camera_z);
        WriteValue(MemoryWatchers.CameraRotation, CameraRotation);
        WriteValue(MemoryWatchers.EncounterStatus, EncounterStatus);
        WriteValue(MemoryWatchers.MovementLock, MovementLock);
        WriteValue(MemoryWatchers.ActiveMusicId, ActiveMusicId);
        WriteValue(MemoryWatchers.MusicId, MusicId);
        WriteValue(MemoryWatchers.RoomNumberAlt, RoomNumberAlt);
        WriteValue(MemoryWatchers.CutsceneAlt, CutsceneAlt);
        WriteValue(MemoryWatchers.AirshipDestinations, AirshipDestinations);
        WriteValue(MemoryWatchers.AuronOverdrives, AuronOverdrives);
        WriteValue(MemoryWatchers.TargetFramerate, TargetFramerate);
        WriteValue(MemoryWatchers.Sandragoras, Sandragoras);
        WriteValue(MemoryWatchers.EncounterMapID, EncounterMapID);
        WriteValue(MemoryWatchers.EncounterFormationID1, EncounterFormationID1);
        WriteValue(MemoryWatchers.EncounterFormationID2, EncounterFormationID2);
        WriteValue(MemoryWatchers.ScriptedBattleFlag1, ScriptedBattleFlag1);
        WriteValue(MemoryWatchers.ScriptedBattleFlag2, ScriptedBattleFlag2);
        WriteValue(MemoryWatchers.ScriptedBattleVar1, ScriptedBattleVar1);
        WriteValue(MemoryWatchers.ScriptedBattleVar3, ScriptedBattleVar3);
        WriteValue(MemoryWatchers.ScriptedBattleVar4, ScriptedBattleVar4);
        WriteValue(MemoryWatchers.EncounterTrigger, EncounterTrigger);
        WriteValue(MemoryWatchers.HpEnemyA, HpEnemyA);
        WriteValue(MemoryWatchers.GuadoCount, GuadoCount);
        WriteValue(MemoryWatchers.TidusXCoordinate, TidusXCoordinate);
        WriteValue(MemoryWatchers.TidusYCoordinate, TidusYCoordinate);
        WriteValue(MemoryWatchers.TidusZCoordinate, TidusZCoordinate);
        WriteValue(MemoryWatchers.TidusRotation, TidusRotation);
        WriteBytes(MemoryWatchers.DialogueFile, DialogueFile);
        WriteValue(MemoryWatchers.CutsceneTiming, CutsceneTiming);
        WriteValue(MemoryWatchers.AuronTransition, AuronTransition);
        WriteValue(MemoryWatchers.AmmesTransition, AmmesTransition);
        WriteValue(MemoryWatchers.TankerTransition, TankerTransition);
        WriteValue(MemoryWatchers.InsideSinTransition, InsideSinTransition);
        WriteValue(MemoryWatchers.DiveTransition, DiveTransition);
        WriteValue(MemoryWatchers.GeosTransition, GeosTransition);
        WriteValue(MemoryWatchers.KlikkTransition, KlikkTransition);
        WriteValue(MemoryWatchers.AlBhedBoatTransition, AlBhedBoatTransition);
        WriteValue(MemoryWatchers.UnderwaterRuinsTransition, UnderwaterRuinsTransition);
        WriteValue(MemoryWatchers.UnderwaterRuinsTransition2, UnderwaterRuinsTransition2);
        WriteValue(MemoryWatchers.BeachTransition, BeachTransition);
        WriteValue(MemoryWatchers.LagoonTransition1, LagoonTransition1);
        WriteValue(MemoryWatchers.LagoonTransition2, LagoonTransition2);
        WriteValue(MemoryWatchers.ValeforTransition, ValeforTransition);
        WriteValue(MemoryWatchers.KimahriTransition, KimahriTransition);
        WriteValue(MemoryWatchers.YunaBoatTransition, YunaBoatTransition);
        WriteValue(MemoryWatchers.SinFinTransition, SinFinTransition);
        WriteValue(MemoryWatchers.EchuillesTransition, EchuillesTransition);
        WriteValue(MemoryWatchers.GeneauxTransition, GeneauxTransition);
        WriteValue(MemoryWatchers.KilikaTrialsTransition, KilikaTrialsTransition);
        WriteValue(MemoryWatchers.KilikaAntechamberTransition, KilikaAntechamberTransition);
        WriteValue(MemoryWatchers.IfritTransition, IfritTransition);
        WriteValue(MemoryWatchers.IfritTransition2, IfritTransition2);
        WriteValue(MemoryWatchers.JechtShotTransition, JechtShotTransition);
        WriteValue(MemoryWatchers.OblitzeratorTransition, OblitzeratorTransition);
        WriteValue(MemoryWatchers.BlitzballTransition, BlitzballTransition);
        WriteValue(MemoryWatchers.SahaginTransition, SahaginTransition);
        WriteValue(MemoryWatchers.GarudaTransition, GarudaTransition);
        WriteValue(MemoryWatchers.RinTransition, RinTransition);
        WriteValue(MemoryWatchers.ChocoboEaterTransition, ChocoboEaterTransition);
        WriteValue(MemoryWatchers.GuiTransition, GuiTransition);
        WriteValue(MemoryWatchers.Gui2Transition, Gui2Transition);
        WriteValue(MemoryWatchers.DjoseTransition, DjoseTransition);
        WriteValue(MemoryWatchers.IxionTransition, IxionTransition);
        WriteValue(MemoryWatchers.ExtractorTransition, ExtractorTransition);
        WriteValue(MemoryWatchers.SeymoursHouseTransition1, SeymoursHouseTransition1);
        WriteValue(MemoryWatchers.SeymoursHouseTransition2, SeymoursHouseTransition2);
        WriteValue(MemoryWatchers.FarplaneTransition1, FarplaneTransition1);
        WriteValue(MemoryWatchers.FarplaneTransition2, FarplaneTransition2);
        WriteValue(MemoryWatchers.TromellTransition, TromellTransition);
        WriteValue(MemoryWatchers.CrawlerTransition, CrawlerTransition);
        WriteValue(MemoryWatchers.SeymourTransition, SeymourTransition);
        WriteValue(MemoryWatchers.SeymourTransition2, SeymourTransition2);
        WriteValue(MemoryWatchers.WendigoTransition, WendigoTransition);
        WriteValue(MemoryWatchers.SpherimorphTransition, SpherimorphTransition);
        WriteValue(MemoryWatchers.UnderLakeTransition, UnderLakeTransition);
        WriteValue(MemoryWatchers.BikanelTransition, BikanelTransition);
        WriteValue(MemoryWatchers.HomeTransition, HomeTransition);
        WriteValue(MemoryWatchers.EvraeTransition, EvraeTransition);
        WriteValue(MemoryWatchers.EvraeAirshipTransition, EvraeAirshipTransition);
        WriteValue(MemoryWatchers.GuardsTransition, GuardsTransition);
        WriteValue(MemoryWatchers.BahamutTransition, BahamutTransition);
        WriteValue(MemoryWatchers.IsaaruTransition, IsaaruTransition);
        WriteValue(MemoryWatchers.AltanaTransition, AltanaTransition);
        WriteValue(MemoryWatchers.NatusTransition, NatusTransition);
        WriteValue(MemoryWatchers.DefenderXTransition, DefenderXTransition);
        WriteValue(MemoryWatchers.RonsoTransition, RonsoTransition);
        WriteValue(MemoryWatchers.FluxTransition, FluxTransition);
        WriteValue(MemoryWatchers.SanctuaryTransition, SanctuaryTransition);
        WriteValue(MemoryWatchers.SpectralKeeperTransition, SpectralKeeperTransition);
        WriteValue(MemoryWatchers.SpectralKeeperTransition2, SpectralKeeperTransition2);
        WriteValue(MemoryWatchers.YunalescaTransition, YunalescaTransition);
        WriteValue(MemoryWatchers.FinsTransition, FinsTransition);
        WriteValue(MemoryWatchers.FinsAirshipTransition, FinsAirshipTransition);
        WriteValue(MemoryWatchers.SinCoreTransition, SinCoreTransition);
        WriteValue(MemoryWatchers.OverdriveSinTransition, OverdriveSinTransition);
        WriteValue(MemoryWatchers.OmnisTransition, OmnisTransition);
        WriteValue(MemoryWatchers.BFATransition, BFATransition);
        WriteValue(MemoryWatchers.AeonTransition, AeonTransition);
        WriteValue(MemoryWatchers.YuYevonTransition, YuYevonTransition);
        WriteValue(MemoryWatchers.YojimboFaythTransition, YojimboFaythTransition);
        WriteValue(MemoryWatchers.EnableTidus, EnableTidus);
        WriteValue(MemoryWatchers.EnableYuna, EnableYuna);
        WriteValue(MemoryWatchers.EnableAuron, EnableAuron);
        WriteValue(MemoryWatchers.EnableKimahri, EnableKimahri);
        WriteValue(MemoryWatchers.EnableWakka, EnableWakka);
        WriteValue(MemoryWatchers.EnableLulu, EnableLulu);
        WriteValue(MemoryWatchers.EnableRikku, EnableRikku);
        WriteValue(MemoryWatchers.EnableSeymour, EnableSeymour);
        WriteValue(MemoryWatchers.EnableValefor, EnableValefor);
        WriteValue(MemoryWatchers.EnableIfrit, EnableIfrit);
        WriteValue(MemoryWatchers.EnableIxion, EnableIxion);
        WriteValue(MemoryWatchers.EnableShiva, EnableShiva);
        WriteValue(MemoryWatchers.EnableBahamut, EnableBahamut);
        WriteValue(MemoryWatchers.EnableAnima, EnableAnima);
        WriteValue(MemoryWatchers.EnableYojimbo, EnableYojimbo);
        WriteValue(MemoryWatchers.EnableMagus, EnableMagus);

        WriteValue(MemoryWatchers.BaajFlag1, BaajFlag1);
        WriteValue(MemoryWatchers.SSWinnoFlag1, SSWinnoFlag1);
        WriteValue(MemoryWatchers.KilikaMapFlag, KilikaMapFlag);
        WriteValue(MemoryWatchers.SSWinnoFlag2, SSWinnoFlag2);

        WriteValue(MemoryWatchers.LucaFlag, LucaFlag);
        WriteValue(MemoryWatchers.LucaFlag2, LucaFlag2);
        WriteValue(MemoryWatchers.BlitzballFlag, BlitzballFlag);
        WriteValue(MemoryWatchers.MiihenFlag1, MiihenFlag1);
        WriteValue(MemoryWatchers.MiihenFlag2, MiihenFlag2);
        WriteValue(MemoryWatchers.MiihenFlag3, MiihenFlag3);
        WriteValue(MemoryWatchers.MiihenFlag4, MiihenFlag4);
        WriteValue(MemoryWatchers.MRRFlag1, MRRFlag1);
        WriteValue(MemoryWatchers.MRRFlag2, MRRFlag2);
        WriteValue(MemoryWatchers.MoonflowFlag, MoonflowFlag);
        WriteValue(MemoryWatchers.MoonflowFlag2, MoonflowFlag2);
        WriteValue(MemoryWatchers.RikkuOutfit, RikkuOutfit);
        WriteValue(MemoryWatchers.TidusWeaponDamageBoost, TidusWeaponDamageBoost);
        WriteValue(MemoryWatchers.GuadosalamShopFlag, GuadosalamShopFlag);
        WriteValue(MemoryWatchers.ThunderPlainsFlag, ThunderPlainsFlag);
        WriteValue(MemoryWatchers.MacalaniaFlag, MacalaniaFlag);
        WriteValue(MemoryWatchers.BikanelFlag, BikanelFlag);
        WriteBytes(MemoryWatchers.RikkuName, RikkuName);
        WriteValue(MemoryWatchers.ViaPurificoPlatform, ViaPurificoPlatform);
        WriteValue(MemoryWatchers.NatusFlag, NatusFlag);
        WriteValue(MemoryWatchers.CalmLandsFlag, CalmLandsFlag);
        WriteValue(MemoryWatchers.WantzFlag, WantzFlag);
        WriteValue(MemoryWatchers.GagazetCaveFlag, GagazetCaveFlag);
        WriteValue(MemoryWatchers.OmegaRuinsFlag, OmegaRuinsFlag);
        WriteValue(MemoryWatchers.WantzMacalaniaFlag, WantzMacalaniaFlag);

        WriteBytes(MemoryWatchers.AurochsTeamBytes, AurochsTeamBytes);
        WriteBytes(MemoryWatchers.BlitzballBytes, BlitzballBytes);
        WriteValue(MemoryWatchers.AurochsPlayer1, AurochsPlayer1);

        WriteValue(MemoryWatchers.GilBattleRewards, GilBattleRewards);
        WriteValue(MemoryWatchers.BattleRewardItemCount, BattleRewardItemCount);
        WriteValue(MemoryWatchers.BattleRewardItem1, BattleRewardItem1);
        WriteValue(MemoryWatchers.BattleRewardItem2, BattleRewardItem2);
        WriteValue(MemoryWatchers.BattleRewardItem3, BattleRewardItem3);
        WriteValue(MemoryWatchers.BattleRewardItem4, BattleRewardItem4);
        WriteValue(MemoryWatchers.BattleRewardItem5, BattleRewardItem5);
        WriteValue(MemoryWatchers.BattleRewardItem6, BattleRewardItem6);
        WriteValue(MemoryWatchers.BattleRewardItem7, BattleRewardItem7);
        WriteValue(MemoryWatchers.BattleRewardItem8, BattleRewardItem8);
        WriteValue(MemoryWatchers.BattleRewardItemQty1, BattleRewardItemQty1);
        WriteValue(MemoryWatchers.BattleRewardItemQty2, BattleRewardItemQty2);
        WriteValue(MemoryWatchers.BattleRewardItemQty3, BattleRewardItemQty3);
        WriteValue(MemoryWatchers.BattleRewardItemQty4, BattleRewardItemQty4);
        WriteValue(MemoryWatchers.BattleRewardItemQty5, BattleRewardItemQty5);
        WriteValue(MemoryWatchers.BattleRewardItemQty6, BattleRewardItemQty6);
        WriteValue(MemoryWatchers.BattleRewardItemQty7, BattleRewardItemQty7);
        WriteValue(MemoryWatchers.BattleRewardItemQty8, BattleRewardItemQty8);
        WriteValue(MemoryWatchers.BattleRewardEquipCount, BattleRewardEquipCount);
        WriteBytes(MemoryWatchers.BattleRewardEquip1, BattleRewardEquip1);
        WriteBytes(MemoryWatchers.BattleRewardEquip2, BattleRewardEquip2);
        WriteBytes(MemoryWatchers.BattleRewardEquip3, BattleRewardEquip3);
        WriteBytes(MemoryWatchers.BattleRewardEquip4, BattleRewardEquip4);
        WriteBytes(MemoryWatchers.BattleRewardEquip5, BattleRewardEquip5);
        WriteBytes(MemoryWatchers.BattleRewardEquip6, BattleRewardEquip6);
        WriteBytes(MemoryWatchers.BattleRewardEquip7, BattleRewardEquip7);
        WriteBytes(MemoryWatchers.BattleRewardEquip8, BattleRewardEquip8);

        WriteValue(MemoryWatchers.MenuValue1, MenuValue1);
        WriteValue(MemoryWatchers.MenuValue2, MenuValue2);
        WriteValue(MemoryWatchers.MenuTriggerValue, MenuTriggerValue);

        WriteBytes(MemoryWatchers.RNGArrayOpBytes, RNGArrayOpBytes);

        // Update Bitmasks
        WriteValue(MemoryWatchers.CalmLandsFlag, MemoryWatchers.CalmLandsFlag.Current | AddCalmLandsBitmask);

        if (ForceLoad)
        {
            ForceGameLoad();
            FixMenuBug();
            FixSpeedBoosterBug();
        }

        if (FullHeal)
        {
            FullPartyHeal();
        }

        if (MenuCleanup)
        {
            CleanMenuValues();
            ClearAllBattleRewards();
        }

        if (AddSinLocation)
        {
            AddSin();
        }

        if (RemoveSinLocation)
        {
            RemoveSin();
        }

        if (PositionPartyOffScreen)
        {
            PartyOffScreen();
        }

        if (!(AddItemsToInventory is null))
        {
            AddItems(AddItemsToInventory);
        }    

        UpdateFormation(Formation);

        if (PositionTidusAfterLoad)
        {
            process.Resume();
            MemoryWatchers.ForceLoad.Update(process);
            MemoryWatchers.State.Update(process);
            while (MemoryWatchers.ForceLoad.Current == 1 || MemoryWatchers.State.Current == -1) // Wait for loading to finish and black screen to end
            {
                MemoryWatchers.ForceLoad.Update(process);
                MemoryWatchers.State.Update(process);
            }
            MemoryWatchers.FrameCounterFromLoad.Update(process);
            while (MemoryWatchers.FrameCounterFromLoad.Current < MoveFrame)
            {
                MemoryWatchers.FrameCounterFromLoad.Update(process);
            }
            process.Suspend();
            SetActorPosition(1, Target_x, Target_y, Target_z, Target_rot, Target_var1);
            SetActorPosition(101, Target_x, Target_y, Target_z, Target_rot, Target_var1); // In Besaid Temple Tidus is ID 101 for some reason, also some other locations.
            process.Resume();
            while (MemoryWatchers.FrameCounterFromLoad.Current < MoveFrame + 3)
            {
                MemoryWatchers.FrameCounterFromLoad.Update(process);
            }
            process.Suspend();
            WriteValue<float>(MemoryWatchers.TotalDistance, 0.0f);
            WriteValue<float>(MemoryWatchers.CycleDistance, 0.0f);
            process.Resume();
        }
        else
        {
            if (TargetActorIDs != null)
            {
                foreach (short TargetActorID in TargetActorIDs)
                {
                    SetActorPosition(TargetActorID, Target_x, Target_y, Target_z, Target_rot, Target_var1);
                }
            }
        }
    }

    /* Set the force load bit. Will immediately cause a fade and load. */
    private void ForceGameLoad()
    {
        WriteValue<byte>(MemoryWatchers.ForceLoad, 1);
        MemoryWatchers.ForceLoad.Update(process);
    }

    protected void WriteValue<T>(MemoryWatcher watcher, T? value) where T : struct
    {
        if (value.HasValue)
        {
            var dbgAddr = watcher.Address - MemoryWatchers.GetBaseAddress();

            if (watcher.AddrType == MemoryWatcher.AddressType.Absolute)
            {
                DiagnosticLog.Debug($"w {watcher.Name}: write {value.Value} to addr {dbgAddr:X8}.");
                process.WriteValue(watcher.Address, value.Value);
                return;
            }
            else
            {
                // To write to a deep pointer we need to dereference its pointer path.
                // Then we write to the final pointer.
                if (!watcher.DeepPtr.DerefOffsets(process, out IntPtr finalPointer))
                {
                    DiagnosticLog.Information("Couldn't read the pointer path for: " + watcher.Name);
                }

                DiagnosticLog.Debug($"w {watcher.Name}: write {value.Value} to addr {finalPointer:X8}.");
                process.WriteValue(finalPointer, value.Value);
            }
        }
    }

    protected void WriteBytes(MemoryWatcher watcher, byte[] bytes)
    {
        if (bytes != null)
        {
            var hexstring = Convert.ToHexString(bytes);
            var dbgAddr   = watcher.Address - MemoryWatchers.GetBaseAddress();

            if (watcher.AddrType == MemoryWatcher.AddressType.Absolute)
            {
                DiagnosticLog.Debug($"w {watcher.Name}: write {hexstring} to addr {dbgAddr:X8}.");
                process.WriteBytes(watcher.Address, bytes);
                return;
            }
            else
            {
                // To write to a deep pointer we need to dereference its pointer path.
                // Then we write to the final pointer.
                if (!watcher.DeepPtr.DerefOffsets(process, out IntPtr finalPointer))
                {
                    DiagnosticLog.Information("Couldn't read the pointer path for: " + watcher.Name);
                }

                DiagnosticLog.Debug($"w {watcher.Name}: write {hexstring} to addr {finalPointer:X8}.");
                process.WriteBytes(finalPointer, bytes);
            }
        }
    }

    public override bool Equals(object obj)
    {
        // If they're the exact same object then we're good.
        if (Equals(this, obj))
        {
            return true;
        }

        // Otherwise check all of the public fields to verify if they are the same transition
        return this == (Transition)obj;
    }

    public static bool operator ==(Transition first, Transition second)
    {
        foreach (var property in publicFields)
        {
            var thisValue = first is null ? null : property.GetValue(first);
            var otherValue = second is null ? null : property.GetValue(second);
            if (!Equals(thisValue, otherValue))
            {
                return false;
            }
        }
        return true;
    }

    public static bool operator !=(Transition first, Transition second)
    {
        return !(first == second);
    }

    public override int GetHashCode()
    {
        int hashCode = 3;
        foreach (var property in publicFields)
        {
            // This line *should* be getting the hashcode of the actual property value
            // NOT the property type.
            hashCode *= property.GetValue(this).GetHashCode();
        }
        return hashCode;
    }

    private void FullPartyHeal()
    {
        Process process = MemoryWatchers.Process;

        int baseAddress = MemoryWatchers.GetBaseAddress();

        for (int i = 0; i < 18; i++)
        {
            MemoryWatcher<int> CharacterHP_Current = new MemoryWatcher<int>(new IntPtr(baseAddress + 0xD32078 + 0x94 * i));
            MemoryWatcher<int> CharacterHP_Max = new MemoryWatcher<int>(new IntPtr(baseAddress + 0xD32080 + 0x94 * i));
            MemoryWatcher<int> CharacterMP_Current = new MemoryWatcher<int>(new IntPtr(baseAddress + 0xD3207C + 0x94 * i));
            MemoryWatcher<int> CharacterMP_Max = new MemoryWatcher<int>(new IntPtr(baseAddress + 0xD32084 + 0x94 * i));
            MemoryWatcher<byte> BattlesUntilReady = new MemoryWatcher<byte>(new IntPtr(baseAddress + 0xD32099 + 0x94 * i)); // Reset battles until ready counter for aeons

            CharacterHP_Current.Update(process);
            CharacterHP_Max.Update(process);
            CharacterMP_Current.Update(process);
            CharacterMP_Max.Update(process);
            BattlesUntilReady.Update(process);

            WriteValue<int>(CharacterHP_Current, CharacterHP_Max.Current);
            WriteValue<int>(CharacterMP_Current, CharacterMP_Max.Current);
            WriteValue<byte>(BattlesUntilReady, 0);
        }
    }

    private void CleanMenuValues()
    {
        WriteValue<int>(MemoryWatchers.MenuValue1, 0);
        WriteValue<int>(MemoryWatchers.MenuValue2, 0);
    }

    private void FixMenuBug()
    {
        WriteValue<int>(MemoryWatchers.MenuValue3, unchecked((int)0xFFFFFFFF));
        WriteValue<int>(MemoryWatchers.MenuValue4, 0x00000000);
        WriteValue<byte>(MemoryWatchers.MenuValue5, 0x00);
        WriteValue<int>(MemoryWatchers.MenuValue6, 0x00000001);
        WriteValue<byte>(MemoryWatchers.MenuValue7, 0x00);
    }

    private void FixSpeedBoosterBug()
    {
        WriteValue<int>(MemoryWatchers.SpeedBoostVar1, 1);
    }

    private void ClearAllBattleRewards()
    {
        // Clear Gil
        WriteValue<int>(MemoryWatchers.GilBattleRewards, 0);

        if (AddRewardItems)
        {
            byte[] items = process.ReadBytes(MemoryWatchers.ItemsStart.Address, 224);
            byte[] itemsQty = process.ReadBytes(MemoryWatchers.ItemsQtyStart.Address, 112);
            byte[] itemRewards = process.ReadBytes(MemoryWatchers.BattleRewardItem1.Address, 16);
            byte[] itemRewardsQty = process.ReadBytes(MemoryWatchers.BattleRewardItemQty1.Address, 8);

            int rewardCount = MemoryWatchers.BattleRewardItemCount.Current;

            bool alreadyExists;

            for (int i = 0; i < rewardCount; i++)
            {
                alreadyExists = false;

                for (int j = 0; j < 112; j++)
                {
                    if (items[2 * j] == itemRewards[2 * i] && itemsQty[j] > 0)
                    {
                        alreadyExists = true;
                        itemsQty[j] += itemRewardsQty[i];

                        break;
                    }
                }

                if (alreadyExists == false)
                {
                    for (int j = 0; j < 112; j++)
                    {
                        if (items[2 * j] == 0xFF && itemsQty[j] == 0)
                        {
                            alreadyExists = true;
                            items[2 * j] = itemRewards[2 * i];
                            items[2 * j + 1] = itemRewards[2 * i + 1];
                            itemsQty[j] = itemRewardsQty[i];

                            break;
                        }
                    }
                }
            }

            WriteBytes(MemoryWatchers.ItemsStart, items);
            WriteBytes(MemoryWatchers.ItemsQtyStart, itemsQty);
        }

        // Clear Items
        WriteValue<byte>(MemoryWatchers.BattleRewardItemCount, 0);
        WriteValue<short>(MemoryWatchers.BattleRewardItem1, 0);
        WriteValue<short>(MemoryWatchers.BattleRewardItem2, 0);
        WriteValue<short>(MemoryWatchers.BattleRewardItem3, 0);
        WriteValue<short>(MemoryWatchers.BattleRewardItem4, 0);
        WriteValue<short>(MemoryWatchers.BattleRewardItem5, 0);
        WriteValue<short>(MemoryWatchers.BattleRewardItem6, 0);
        WriteValue<short>(MemoryWatchers.BattleRewardItem7, 0);
        WriteValue<short>(MemoryWatchers.BattleRewardItem8, 0);
        WriteValue<byte>(MemoryWatchers.BattleRewardItemQty1, 0);
        WriteValue<byte>(MemoryWatchers.BattleRewardItemQty2, 0);
        WriteValue<byte>(MemoryWatchers.BattleRewardItemQty3, 0);
        WriteValue<byte>(MemoryWatchers.BattleRewardItemQty4, 0);
        WriteValue<byte>(MemoryWatchers.BattleRewardItemQty5, 0);
        WriteValue<byte>(MemoryWatchers.BattleRewardItemQty6, 0);
        WriteValue<byte>(MemoryWatchers.BattleRewardItemQty7, 0);
        WriteValue<byte>(MemoryWatchers.BattleRewardItemQty8, 0);

        //Clear Equipment -- Equipment Arrays are 22 bytes long
        WriteValue<byte>(MemoryWatchers.BattleRewardEquipCount, 0);
        WriteBytes(MemoryWatchers.BattleRewardEquip1, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        WriteBytes(MemoryWatchers.BattleRewardEquip2, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        WriteBytes(MemoryWatchers.BattleRewardEquip3, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        WriteBytes(MemoryWatchers.BattleRewardEquip4, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        WriteBytes(MemoryWatchers.BattleRewardEquip5, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        WriteBytes(MemoryWatchers.BattleRewardEquip6, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        WriteBytes(MemoryWatchers.BattleRewardEquip7, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        WriteBytes(MemoryWatchers.BattleRewardEquip8, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });

        // Clear AP Flags
        WriteBytes(MemoryWatchers.CharacterAPFlags, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
    }

    private void AddItems((byte itemref, byte itemqty)[] AddItemsToInventory)
    {
        byte[] items = process.ReadBytes(MemoryWatchers.ItemsStart.Address, 224);
        byte[] itemsQty = process.ReadBytes(MemoryWatchers.ItemsQtyStart.Address, 112);

        bool alreadyExists;

        for (int i = 0; i < AddItemsToInventory.Length; i++)
        {
            alreadyExists = false;

            for (int j = 0; j < 112; j++)
            {
                if (items[2 * j] == AddItemsToInventory[i].itemref)
                {
                    alreadyExists = true;
                    itemsQty[j] += AddItemsToInventory[i].itemqty;

                    break;
                }
            }

            if (alreadyExists == false)
            {
                for (int j = 0; j < 112; j++)
                {
                    if (items[2 * j] == 0xFF && itemsQty[j] == 0)
                    {
                        DiagnosticLog.Information($"Adding Item: {AddItemsToInventory[i].itemref}");
                        items[2 * j] = AddItemsToInventory[i].itemref;
                        items[2 * j + 1] = 0x20;
                        itemsQty[j] = AddItemsToInventory[i].itemqty;

                        break;
                    }
                }
            }
        }

        WriteBytes(MemoryWatchers.ItemsStart, items);
        WriteBytes(MemoryWatchers.ItemsQtyStart, itemsQty);
    }

    private void AddSin()
    {
        WriteValue<short>(MemoryWatchers.AirshipDestinations, (short)(MemoryWatchers.AirshipDestinations.Current + 512));
    }

    private void RemoveSin()
    {
        WriteValue<short>(MemoryWatchers.AirshipDestinations, (short)(MemoryWatchers.AirshipDestinations.Current - 512));
    }

    private void PartyOffScreen()
    {
        if (MemoryWatchers.EnableTidus.Current == 17)
        {
            SetActorPosition(1, PartyTarget_x, PartyTarget_y, PartyTarget_z);
        }
        if (MemoryWatchers.EnableYuna.Current == 17)
        {
            SetActorPosition(2, PartyTarget_x, PartyTarget_y, PartyTarget_z);
        }
        if (MemoryWatchers.EnableAuron.Current == 17)
        {
            SetActorPosition(3, PartyTarget_x, PartyTarget_y, PartyTarget_z);
        }
        if (MemoryWatchers.EnableKimahri.Current == 17)
        {
            SetActorPosition(4, PartyTarget_x, PartyTarget_y, PartyTarget_z);
        }
        if (MemoryWatchers.EnableWakka.Current == 17)
        {
            SetActorPosition(5, PartyTarget_x, PartyTarget_y, PartyTarget_z);
        }
        if (MemoryWatchers.EnableLulu.Current == 17)
        {
            SetActorPosition(6, PartyTarget_x, PartyTarget_y, PartyTarget_z);
        }
        if (MemoryWatchers.EnableRikku.Current == 17)
        {
            SetActorPosition(7, PartyTarget_x, PartyTarget_y, PartyTarget_z);
        }
        if (MemoryWatchers.EnableSeymour.Current == 17)
        {
            SetActorPosition(8, PartyTarget_x, PartyTarget_y, PartyTarget_z);
        }
    }

    private bool SetActorPosition(short? TargetActorID = null, float? Target_x = null, float? Target_y = null, float? Target_z = null, float? Target_rot = null, short? Target_var1 = null)
    {
        Process process = MemoryWatchers.Process;

        MemoryWatchers.ActorArrayLength.Update(process);
        int ActorCount = MemoryWatchers.ActorArrayLength.Current;
        int baseAddress = MemoryWatchers.GetBaseAddress();
        bool actorFound = false;

        if (!(TargetActorID is null))
        {

            for (int i = 0; i < ActorCount; i++)
            {
                MemoryWatcher<short> characterIndex = new MemoryWatcher<short>(new DeepPointer(new IntPtr(baseAddress + 0x1FC44E4), new int[] { 0x00 + 0x880 * i }));
                characterIndex.Update(process);

                short ActorID = characterIndex.Current;

                if (ActorID == TargetActorID)
                {
                    actorFound = true;

                    MemoryWatcher<float> characterPos_x = new MemoryWatcher<float>(new DeepPointer(new IntPtr(baseAddress + 0x1FC44E4), new int[] { 0x0C + 0x880 * i }));
                    MemoryWatcher<float> characterPos_y = new MemoryWatcher<float>(new DeepPointer(new IntPtr(baseAddress + 0x1FC44E4), new int[] { 0x10 + 0x880 * i }));
                    MemoryWatcher<float> characterPos_z = new MemoryWatcher<float>(new DeepPointer(new IntPtr(baseAddress + 0x1FC44E4), new int[] { 0x14 + 0x880 * i }));
                    MemoryWatcher<float> characterPos_floor = new MemoryWatcher<float>(new DeepPointer(new IntPtr(baseAddress + 0x1FC44E4), new int[] { 0x16C + 0x880 * i }));
                    MemoryWatcher<float> characterPos_rot = new MemoryWatcher<float>(new DeepPointer(new IntPtr(baseAddress + 0x1FC44E4), new int[] { 0x168 + 0x880 * i }));
                    MemoryWatcher<short> characterPos_var1 = new MemoryWatcher<short>(new DeepPointer(new IntPtr(baseAddress + 0x1FC44E4), new int[] { 0x824 + 0x880 * i }));

                    if (!(Target_x is null))
                    {
                        WriteValue<float>(characterPos_x, Target_x);
                    }
                    if (!(Target_y is null))
                    {
                        WriteValue<float>(characterPos_floor, Target_y);
                        WriteValue<float>(characterPos_y, Target_y);
                    }
                    if (!(Target_z is null))
                    {
                        WriteValue<float>(characterPos_z, Target_z);
                    }
                    if (!(Target_rot is null))
                    {
                        WriteValue<float>(characterPos_rot, Target_rot);
                    }
                    if (!(Target_var1 is null))
                    {
                        WriteValue<short>(characterPos_var1, Target_var1);
                    }
                }
            }
        }
        return actorFound;
    }

    // Formation Functions

    public enum formations
    {
        Klikk2,
        PreKimahri,
        PostKimahri,
        BoardingSSLiki,
        PostEchuilles,
        MachinaFights,
        PreOblitzerator,
        PostOblitzerator,
        PreSahagins,
        AuronJoinsTheParty,
        PreGui2,
        PostGui,
        MeetRikku,
        PostCrawler,
        PreSeymour,
        BikanelStart,
        PostZu,
        BikanelRikku,
        ViaPurificoStart,
        HighbridgeStart,
        PreNatus,
        PostBiranYenke
    }

    private void UpdateFormation(byte[] initialFormation = null)
    {
        byte[] formation = process.ReadBytes(MemoryWatchers.Formation.Address, 10);
        byte initialPosition1 = 0xFF;
        byte initialPosition2 = 0xFF;
        byte initialPosition3 = 0xFF;

        if (!(initialFormation is null))
        {
            initialPosition1 = initialFormation[0];
            initialPosition2 = initialFormation[1];
            initialPosition3 = initialFormation[2];
        }

        if (FormationSwitch.HasValue)
        {
            switch (FormationSwitch)
            {
                case formations.Klikk2:
                    formation = new byte[] { 0x00, 0x06, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
                    break;
                case formations.PreKimahri:
                    formation = new byte[] { 0x00, 0xFF, 0xFF, 0x01, 0x04, 0x05, 0xFF, 0xFF, 0xFF, 0xFF };
                    break;
                case formations.PostKimahri:
                    formation = new byte[] { 0x00, 0x01, 0x04, 0xFF, 0xFF, 0x05, 0xFF, 0xFF, 0xFF, 0xFF };
                    break;
                case formations.BoardingSSLiki:
                    formation = AddCharacter(formation, 0x03);
                    break;
                case formations.PostEchuilles:
                    formation = new byte[] { 0x05, 0x04, 0x00, 0x01, 0x03, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
                    WriteValue<byte>(MemoryWatchers.EnableYuna, 17);
                    WriteValue<byte>(MemoryWatchers.EnableKimahri, 17);
                    WriteValue<byte>(MemoryWatchers.EnableLulu, 17);
                    break;
                case formations.MachinaFights:
                    formation = new byte[] { 0x05, 0x00, 0x03, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
                    WriteValue<byte>(MemoryWatchers.EnableYuna, 16);
                    WriteValue<byte>(MemoryWatchers.EnableWakka, 16);
                    break;
                case formations.PreOblitzerator:
                    formation = new byte[] { 0x00, 0x05, 0x03, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
                    break;
                case formations.PostOblitzerator:
                    formation = new byte[] { 0x05, 0x00, 0x03, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
                    break;
                case formations.PreSahagins:
                    WriteValue<byte>(MemoryWatchers.EnableKimahri, 16);
                    WriteValue<byte>(MemoryWatchers.EnableLulu, 16);
                    WriteValue<byte>(MemoryWatchers.EnableWakka, 17);
                    formation = new byte[] { 0x04, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
                    break;
                case formations.AuronJoinsTheParty:
                    WriteValue<byte>(MemoryWatchers.EnableTidus, 17);
                    WriteValue<byte>(MemoryWatchers.EnableYuna, 17);
                    WriteValue<byte>(MemoryWatchers.EnableAuron, 17);
                    WriteValue<byte>(MemoryWatchers.EnableKimahri, 17);
                    WriteValue<byte>(MemoryWatchers.EnableWakka, 17);
                    WriteValue<byte>(MemoryWatchers.EnableLulu, 17);
                    formation = new byte[] { 0x00, 0x04, 0x01, 0x02, 0xFF, 0xFF, 0x05, 0x03, 0xFF, 0xFF };
                    break;
                case formations.PreGui2:
                    formation = SwapPositionWithFirstEmptyReservePosition(formation, 0);
                    formation = SwapPositionWithFirstEmptyReservePosition(formation, 1);
                    formation = SwapPositionWithFirstEmptyReservePosition(formation, 2);
                    formation = RemoveCharacter(formation, 0);
                    formation = RemoveCharacter(formation, 1);
                    formation = RemoveCharacter(formation, 4);
                    formation = RemoveCharacter(formation, 3);
                    formation = RemoveCharacter(formation, 5);
                    formation = RemoveCharacter(formation, 2);
                    formation = AddCharacter(formation, 7);
                    formation = AddCharacter(formation, 1);
                    formation = AddCharacter(formation, 2);
                    formation = SwapCharacterWithPosition(formation, 1, 0);
                    formation = SwapCharacterWithPosition(formation, 7, 1);
                    formation = SwapCharacterWithPosition(formation, 2, 2);
                    break;
                case formations.PostGui:
                    WriteValue<byte>(MemoryWatchers.EnableTidus, 17);
                    WriteValue<byte>(MemoryWatchers.EnableKimahri, 17);
                    WriteValue<byte>(MemoryWatchers.EnableWakka, 17);
                    WriteValue<byte>(MemoryWatchers.EnableLulu, 17);
                    WriteValue<byte>(MemoryWatchers.EnableSeymour, 16);
                    byte[] newformation = new byte[] { 0x01, 0xFF, 0x02, 0x00, 0x03, 0x04, 0x05, 0xFF, 0xFF, 0xFF };
                    newformation = SwapCharacterWithPosition(newformation, initialPosition1, 0);
                    newformation = SwapCharacterWithPosition(newformation, initialPosition2, 1);
                    newformation = SwapCharacterWithPosition(newformation, initialPosition3, 2);
                    formation = newformation;
                    break;
                case formations.MeetRikku:
                    WriteValue<byte>(MemoryWatchers.EnableRikku, 17);
                    formation = AddCharacter(formation, 0x06);
                    formation = SwapCharacterWithPosition(formation, 0x06, 3);
                    break;
                case formations.PostCrawler:
                    formation = RemoveCharacter(formation, 0x01);
                    WriteValue<byte>(MemoryWatchers.EnableYuna, 0);
                    formation = FillMainPartySlotIfEmpty(formation, 0x00);
                    break;
                case formations.PreSeymour:
                    WriteValue<byte>(MemoryWatchers.EnableYuna, 17);
                    formation = AddCharacter(formation, 0x01);
                    formation = SwapCharacterWithPosition(formation, 0x00, 0);
                    formation = SwapCharacterWithPosition(formation, 0x01, 1);
                    formation = SwapCharacterWithPosition(formation, 0x03, 2);
                    break;
                case formations.BikanelStart:
                    formation = RemoveAll(formation);
                    WriteValue<byte>(MemoryWatchers.EnableTidus, 17);
                    formation = AddCharacter(formation, 0x00);
                    formation = SwapCharacterWithPosition(formation, 0x00, 0);
                    break;
                case formations.PostZu:
                    WriteValue<byte>(MemoryWatchers.EnableWakka, 17);
                    formation = AddCharacter(formation, 0x04);
                    break;
                case formations.BikanelRikku:
                    WriteValue<byte>(MemoryWatchers.EnableRikku, 17);
                    formation = AddCharacter(formation, 0x06);
                    break;
                case formations.ViaPurificoStart:
                    formation = RemoveAll(formation);
                    WriteValue<byte>(MemoryWatchers.EnableYuna, 17);
                    formation = AddCharacter(formation, 0x01);
                    formation = SwapCharacterWithPosition(formation, 0x01, 0);
                    break;
                case formations.HighbridgeStart:
                    WriteValue<byte>(MemoryWatchers.EnableYuna, 17);
                    WriteValue<byte>(MemoryWatchers.EnableAuron, 17);
                    WriteValue<byte>(MemoryWatchers.EnableLulu, 17);
                    formation = AddCharacter(formation, 0x01);
                    formation = AddCharacter(formation, 0x02);
                    formation = AddCharacter(formation, 0x05);
                    formation = SwapCharacterWithPosition(formation, 0x00, 0);
                    formation = SwapCharacterWithPosition(formation, 0x01, 1);
                    formation = SwapCharacterWithPosition(formation, 0x04, 2);
                    break;
                case formations.PreNatus:
                    WriteValue<byte>(MemoryWatchers.EnableKimahri, 17);
                    formation = AddCharacter(formation, 0x03);
                    formation = SwapCharacterWithPosition(formation, 0x00, 0);
                    formation = SwapCharacterWithPosition(formation, 0x01, 2);
                    formation = SwapCharacterWithPosition(formation, 0x03, 1);
                    break;
                case formations.PostBiranYenke:
                    WriteValue<byte>(MemoryWatchers.EnableTidus, 17);
                    WriteValue<byte>(MemoryWatchers.EnableYuna, 17);
                    WriteValue<byte>(MemoryWatchers.EnableAuron, 17);
                    WriteValue<byte>(MemoryWatchers.EnableWakka, 17);
                    WriteValue<byte>(MemoryWatchers.EnableLulu, 17);
                    WriteValue<byte>(MemoryWatchers.EnableRikku, 17);
                    formation = AddCharacter(formation, 0x00);
                    formation = AddCharacter(formation, 0x01);
                    formation = AddCharacter(formation, 0x02);
                    formation = AddCharacter(formation, 0x04);
                    formation = AddCharacter(formation, 0x05);
                    formation = AddCharacter(formation, 0x06);
                    formation = SwapCharacterWithPosition(formation, initialPosition1, 0);
                    formation = SwapCharacterWithPosition(formation, initialPosition2, 1);
                    formation = SwapCharacterWithPosition(formation, initialPosition3, 2);
                    break;
            }
            WriteBytes(MemoryWatchers.Formation, formation);
        }
    }

    public int GetFirstEmptyReservePosition(byte[] formation)
    {
        int formationSize = formation.Length;

        for (int i = 3; i < formationSize; i++)
        {
            if (formation[i] == 0xFF)
            {
                return i;
            }
        }
        return 0;
    }

    public byte[] RemoveCharacter(byte[] formation, byte Character)
    {
        int formationSize = formation.Length;

        for (int i = 0; i < formationSize; i++)
        {
            if (formation[i] == Character)
            {
                formation[i] = 0xFF;
            }
        }
        return formation;
    }

    public byte[] RemoveAll(byte[] formation)
    {
        int formationSize = formation.Length;

        for (int i = 0; i < formationSize; i++)
        {
            formation[i] = 0xFF;
        }

        WriteValue<byte>(MemoryWatchers.EnableTidus, 0);
        WriteValue<byte>(MemoryWatchers.EnableYuna, 0);
        WriteValue<byte>(MemoryWatchers.EnableAuron, 0);
        WriteValue<byte>(MemoryWatchers.EnableKimahri, 0);
        WriteValue<byte>(MemoryWatchers.EnableWakka, 0);
        WriteValue<byte>(MemoryWatchers.EnableLulu, 0);
        WriteValue<byte>(MemoryWatchers.EnableRikku, 0);

        return formation;
    }

    public byte[] AddCharacter(byte[] formation, byte Character)
    {
        if (Array.IndexOf(formation,0x01) == -1)
        {
            int Position = GetFirstEmptyReservePosition(formation);

            formation[Position] = Character;
        }
        return formation;
    }

    public byte[] SwapCharacterWithPosition(byte[] formation, byte Character, int newPosition)
    {
        int oldposition = Array.IndexOf(formation, Character);

        byte temp = formation[oldposition];
        formation[oldposition] = formation[newPosition];
        formation[newPosition] = temp;

        return formation;
    }

    public byte[] SwapPositionWithFirstEmptyReservePosition(byte[] formation, int Position)
    {
        int newPosition = GetFirstEmptyReservePosition(formation);

        formation[newPosition] = formation[Position];
        formation[Position] = 0xFF;

        return formation;
    }

    public byte[] FillMainPartySlotIfEmpty(byte[] formation, byte Character)
    {
        for (int i = 0; i < 3; i++)
        {
            if (formation[i] == 0xFF)
            {
                formation = SwapCharacterWithPosition(formation, Character, i);
                return formation;
            }
        }
        return formation;
    }

}
