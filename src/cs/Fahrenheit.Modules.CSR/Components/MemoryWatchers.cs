using System;
using System.Diagnostics;
using System.Reflection;

using FFXCutsceneRemover.ComponentUtil;
using FFXCutsceneRemover.Logging;
using FFXCutsceneRemover.Resources;

namespace FFXCutsceneRemover;

/* This class contains all the memory watchers used in the program. All watchers should be added here. */
public static class MemoryWatchers
{
    private const string MODULE = "FFX.exe";

    private static int processBaseAddress;

    public static Process Process;
    public static MemoryWatcherList Watchers = new MemoryWatcherList();

    public static MemoryWatcher<byte> Language;

    public static MemoryWatcher<short> RoomNumber;
    public static MemoryWatcher<short> Storyline;
    public static MemoryWatcher<byte> ForceLoad;
    public static MemoryWatcher<byte> SpawnPoint;
    public static MemoryWatcher<short> BattleState;
    public static MemoryWatcher<short> BattleState2;
    public static MemoryWatcher<short> Input;
    public static MemoryWatcher<byte> Menu;
    public static MemoryWatcher<byte> MenuLock;
    public static MemoryWatcher<short> Intro;
    public static MemoryWatcher<sbyte> State;
    public static MemoryWatcher<float> XCoordinate;
    public static MemoryWatcher<float> YCoordinate;
    public static MemoryWatcher<byte> Camera;
    public static MemoryWatcher<float> Camera_x;
    public static MemoryWatcher<float> Camera_y;
    public static MemoryWatcher<float> Camera_z;
    public static MemoryWatcher<float> CameraRotation;
    public static MemoryWatcher<byte> EncounterStatus;
    public static MemoryWatcher<byte> MovementLock;
    public static MemoryWatcher<byte> ActiveMusicId;
    public static MemoryWatcher<byte> MusicId;
    public static MemoryWatcher<byte> RoomNumberAlt;
    public static MemoryWatcher<short> CutsceneAlt;
    public static MemoryWatcher<short> AirshipDestinations;
    public static MemoryWatcher<byte> AuronOverdrives;
    public static MemoryWatcher<int> Gil;
    public static MemoryWatcher<int> TargetFramerate;
    public static MemoryWatcher<int> Dialogue1;
    public static MemoryWatcher<byte> DialogueOption;
    public static MemoryWatcher<byte> DialogueBoxOpen;
    public static MemoryWatcher<byte> DialogueOption_Gui;
    public static MemoryWatcher<byte> DialogueBoxOpen_Gui;
    public static MemoryWatcher<byte> PlayerTurn;
    public static MemoryWatcher<int> FrameCounterFromLoad;

    // Event File
    public static MemoryWatcher<int> EventFileStart;

    // Deep Pointers
    public static MemoryWatcher<int> HpEnemyA;
    public static MemoryWatcher<byte> GuadoCount;
    public static MemoryWatcher<short> NPCLastInteraction;
    public static MemoryWatcher<byte> TidusActionCount;
    public static MemoryWatcher<float> TidusXCoordinate;
    public static MemoryWatcher<float> TidusYCoordinate;
    public static MemoryWatcher<float> TidusZCoordinate;
    public static MemoryWatcher<float> TidusRotation;
    public static MemoryWatcher<byte> DialogueFile;
    public static MemoryWatcher<byte> CutsceneTiming;
    public static MemoryWatcher<int> AuronTransition;
    public static MemoryWatcher<int> AmmesTransition;
    public static MemoryWatcher<int> TankerTransition;
    public static MemoryWatcher<int> InsideSinTransition;
    public static MemoryWatcher<int> DiveTransition;
    public static MemoryWatcher<int> GeosTransition;
    public static MemoryWatcher<int> KlikkTransition;
    public static MemoryWatcher<int> AlBhedBoatTransition;
    public static MemoryWatcher<int> UnderwaterRuinsTransition;
    public static MemoryWatcher<int> UnderwaterRuinsTransition2;
    public static MemoryWatcher<int> BeachTransition;
    public static MemoryWatcher<int> LagoonTransition1;
    public static MemoryWatcher<int> LagoonTransition2;
    public static MemoryWatcher<int> ValeforTransition;
    public static MemoryWatcher<int> KimahriTransition;
    public static MemoryWatcher<int> YunaBoatTransition;
    public static MemoryWatcher<int> SinFinTransition;
    public static MemoryWatcher<int> EchuillesTransition;
    public static MemoryWatcher<int> GeneauxTransition;
    public static MemoryWatcher<int> KilikaTrialsTransition;
    public static MemoryWatcher<int> KilikaAntechamberTransition;
    public static MemoryWatcher<int> IfritTransition;
    public static MemoryWatcher<int> IfritTransition2;
    public static MemoryWatcher<int> JechtShotTransition;
    public static MemoryWatcher<int> OblitzeratorTransition;
    public static MemoryWatcher<int> BlitzballTransition;
    public static MemoryWatcher<int> SahaginTransition;
    public static MemoryWatcher<int> GarudaTransition;
    public static MemoryWatcher<int> RinTransition;
    public static MemoryWatcher<int> ChocoboEaterTransition;
    public static MemoryWatcher<int> GuiTransition;
    public static MemoryWatcher<int> Gui2Transition;
    public static MemoryWatcher<int> DjoseTransition;
    public static MemoryWatcher<int> IxionTransition;
    public static MemoryWatcher<int> ExtractorTransition;
    public static MemoryWatcher<int> SeymoursHouseTransition1;
    public static MemoryWatcher<int> SeymoursHouseTransition2;
    public static MemoryWatcher<int> FarplaneTransition1;
    public static MemoryWatcher<int> FarplaneTransition2;
    public static MemoryWatcher<int> TromellTransition;
    public static MemoryWatcher<int> CrawlerTransition;
    public static MemoryWatcher<int> SeymourTransition;
    public static MemoryWatcher<int> SeymourTransition2;
    public static MemoryWatcher<int> WendigoTransition;
    public static MemoryWatcher<int> SpherimorphTransition;
    public static MemoryWatcher<int> UnderLakeTransition;
    public static MemoryWatcher<int> BikanelTransition;
    public static MemoryWatcher<int> HomeTransition;
    public static MemoryWatcher<int> EvraeTransition;
    public static MemoryWatcher<int> EvraeAirshipTransition;
    public static MemoryWatcher<int> GuardsTransition;
    public static MemoryWatcher<int> BahamutTransition;
    public static MemoryWatcher<int> IsaaruTransition;
    public static MemoryWatcher<int> AltanaTransition;
    public static MemoryWatcher<int> NatusTransition;
    public static MemoryWatcher<int> DefenderXTransition;
    public static MemoryWatcher<int> RonsoTransition;
    public static MemoryWatcher<int> FluxTransition;
    public static MemoryWatcher<int> SanctuaryTransition;
    public static MemoryWatcher<int> SpectralKeeperTransition;
    public static MemoryWatcher<int> SpectralKeeperTransition2;
    public static MemoryWatcher<int> YunalescaTransition;
    public static MemoryWatcher<int> FinsTransition;
    public static MemoryWatcher<int> FinsAirshipTransition;
    public static MemoryWatcher<int> SinCoreTransition;
    public static MemoryWatcher<int> OverdriveSinTransition;
    public static MemoryWatcher<int> OmnisTransition;
    public static MemoryWatcher<int> BFATransition;
    public static MemoryWatcher<int> AeonTransition;
    public static MemoryWatcher<int> YuYevonTransition;
    public static MemoryWatcher<int> YojimboFaythTransition;
    public static MemoryWatcher<int> CutsceneProgress_Max;
    public static MemoryWatcher<int> CutsceneProgress_uVar1;
    public static MemoryWatcher<int> CutsceneProgress_uVar2;
    public static MemoryWatcher<int> CutsceneProgress_uVar3;

    // Encounters
    public static MemoryWatcher<byte> EncounterMapID;
    public static MemoryWatcher<byte> EncounterFormationID1;
    public static MemoryWatcher<byte> EncounterFormationID2;
    public static MemoryWatcher<byte> ScriptedBattleFlag1;
    public static MemoryWatcher<byte> ScriptedBattleFlag2;
    public static MemoryWatcher<int> ScriptedBattleVar1;
    public static MemoryWatcher<int> ScriptedBattleVar3;
    public static MemoryWatcher<int> ScriptedBattleVar4;
    public static MemoryWatcher<byte> EncounterTrigger;

    // Party Configuration
    public static MemoryWatcher<byte> Formation;
    public static MemoryWatcher<byte> RikkuName;
    public static MemoryWatcher<byte> EnableTidus;
    public static MemoryWatcher<byte> EnableYuna;
    public static MemoryWatcher<byte> EnableAuron;
    public static MemoryWatcher<byte> EnableKimahri;
    public static MemoryWatcher<byte> EnableWakka;
    public static MemoryWatcher<byte> EnableLulu;
    public static MemoryWatcher<byte> EnableRikku;
    public static MemoryWatcher<byte> EnableSeymour;
    public static MemoryWatcher<byte> EnableValefor;
    public static MemoryWatcher<byte> EnableIfrit;
    public static MemoryWatcher<byte> EnableIxion;
    public static MemoryWatcher<byte> EnableShiva;
    public static MemoryWatcher<byte> EnableBahamut;
    public static MemoryWatcher<byte> EnableAnima;
    public static MemoryWatcher<byte> EnableYojimbo;
    public static MemoryWatcher<byte> EnableMagus;

    // Encounter Rate
    public static MemoryWatcher<byte> EncountersActiveFlag;
    public static MemoryWatcher<float> TotalDistance;
    public static MemoryWatcher<float> CycleDistance;

    // HP/MP
    public static MemoryWatcher<int> TidusHP;
    public static MemoryWatcher<short> TidusMP;
    public static MemoryWatcher<int> TidusMaxHP;
    public static MemoryWatcher<short> TidusMaxMP;
    public static MemoryWatcher<int> YunaHP;
    public static MemoryWatcher<short> YunaMP;
    public static MemoryWatcher<int> YunaMaxHP;
    public static MemoryWatcher<short> YunaMaxMP;
    public static MemoryWatcher<int> AuronHP;
    public static MemoryWatcher<short> AuronMP;
    public static MemoryWatcher<int> AuronMaxHP;
    public static MemoryWatcher<short> AuronMaxMP;
    public static MemoryWatcher<int> WakkaHP;
    public static MemoryWatcher<short> WakkaMP;
    public static MemoryWatcher<int> WakkaMaxHP;
    public static MemoryWatcher<short> WakkaMaxMP;
    public static MemoryWatcher<int> KimahriHP;
    public static MemoryWatcher<short> KimahriMP;
    public static MemoryWatcher<int> KimahriMaxHP;
    public static MemoryWatcher<short> KimahriMaxMP;
    public static MemoryWatcher<int> LuluHP;
    public static MemoryWatcher<short> LuluMP;
    public static MemoryWatcher<int> LuluMaxHP;
    public static MemoryWatcher<short> LuluMaxMP;
    public static MemoryWatcher<int> RikkuHP;
    public static MemoryWatcher<short> RikkuMP;
    public static MemoryWatcher<int> RikkuMaxHP;
    public static MemoryWatcher<short> RikkuMaxMP;
    public static MemoryWatcher<int> ValeforHP;
    public static MemoryWatcher<short> ValeforMP;
    public static MemoryWatcher<int> ValeforMaxHP;
    public static MemoryWatcher<short> ValeforMaxMP;

    // HP/MP Aeons


    // Special Flags
    public static MemoryWatcher<short> FangirlsOrKidsSkip;
    public static MemoryWatcher<byte> BaajFlag1;
    public static MemoryWatcher<byte> BesaidFlag1;
    public static MemoryWatcher<byte> SSWinnoFlag1;
    public static MemoryWatcher<byte> KilikaMapFlag;
    public static MemoryWatcher<byte> SSWinnoFlag2;
    public static MemoryWatcher<byte> LucaFlag;
    public static MemoryWatcher<byte> LucaFlag2;
    public static MemoryWatcher<byte> BlitzballFlag;
    public static MemoryWatcher<byte> MiihenFlag1;
    public static MemoryWatcher<byte> MiihenFlag2;
    public static MemoryWatcher<byte> MiihenFlag3;
    public static MemoryWatcher<byte> MiihenFlag4;
    public static MemoryWatcher<byte> MRRFlag1;
    public static MemoryWatcher<byte> MRRFlag2;
    public static MemoryWatcher<byte> MoonflowFlag;
    public static MemoryWatcher<byte> MoonflowFlag2;
    public static MemoryWatcher<byte> RikkuOutfit;
    public static MemoryWatcher<byte> TidusWeaponDamageBoost;
    public static MemoryWatcher<byte> GuadosalamShopFlag;
    public static MemoryWatcher<byte> ThunderPlainsFlag;
    public static MemoryWatcher<byte> MacalaniaFlag;
    public static MemoryWatcher<byte> BikanelFlag;
    public static MemoryWatcher<byte> Sandragoras;
    public static MemoryWatcher<byte> ViaPurificoPlatform;
    public static MemoryWatcher<byte> NatusFlag;
    public static MemoryWatcher<ushort> CalmLandsFlag;
    public static MemoryWatcher<byte> WantzFlag;
    public static MemoryWatcher<short> GagazetCaveFlag;
    public static MemoryWatcher<byte> OmegaRuinsFlag;
    public static MemoryWatcher<byte> WantzMacalaniaFlag;

    // Blitzball Abilities
    public static MemoryWatcher<byte> AurochsTeamBytes;
    public static MemoryWatcher<byte> BlitzballBytes;
    public static MemoryWatcher<byte> AurochsPlayer1;

    // Battle Rewards
    public static MemoryWatcher<int> GilBattleRewards;
    public static MemoryWatcher<byte> BattleRewardItemCount;
    public static MemoryWatcher<short> BattleRewardItem1;
    public static MemoryWatcher<short> BattleRewardItem2;
    public static MemoryWatcher<short> BattleRewardItem3;
    public static MemoryWatcher<short> BattleRewardItem4;
    public static MemoryWatcher<short> BattleRewardItem5;
    public static MemoryWatcher<short> BattleRewardItem6;
    public static MemoryWatcher<short> BattleRewardItem7;
    public static MemoryWatcher<short> BattleRewardItem8;
    public static MemoryWatcher<byte> BattleRewardItemQty1;
    public static MemoryWatcher<byte> BattleRewardItemQty2;
    public static MemoryWatcher<byte> BattleRewardItemQty3;
    public static MemoryWatcher<byte> BattleRewardItemQty4;
    public static MemoryWatcher<byte> BattleRewardItemQty5;
    public static MemoryWatcher<byte> BattleRewardItemQty6;
    public static MemoryWatcher<byte> BattleRewardItemQty7;
    public static MemoryWatcher<byte> BattleRewardItemQty8;
    public static MemoryWatcher<byte> BattleRewardEquipCount;
    public static MemoryWatcher<byte> BattleRewardEquip1;
    public static MemoryWatcher<byte> BattleRewardEquip2;
    public static MemoryWatcher<byte> BattleRewardEquip3;
    public static MemoryWatcher<byte> BattleRewardEquip4;
    public static MemoryWatcher<byte> BattleRewardEquip5;
    public static MemoryWatcher<byte> BattleRewardEquip6;
    public static MemoryWatcher<byte> BattleRewardEquip7;
    public static MemoryWatcher<byte> BattleRewardEquip8;

    // Items
    public static MemoryWatcher<byte> ItemsStart;
    public static MemoryWatcher<byte> ItemsQtyStart;

    // AP
    public static MemoryWatcher<byte> CharacterAPFlags;

    // Menu Values
    public static MemoryWatcher<int> MenuTriggerValue;

    public static MemoryWatcher<int> MenuValue1;
    public static MemoryWatcher<int> MenuValue2;

    public static MemoryWatcher<int> MenuValue3;
    public static MemoryWatcher<int> MenuValue4;
    public static MemoryWatcher<byte> MenuValue5;
    public static MemoryWatcher<int> MenuValue6;
    public static MemoryWatcher<byte> MenuValue7;

    public static MemoryWatcher<int> SpeedBoostAmount;
    public static MemoryWatcher<int> SpeedBoostVar1;

    public static MemoryWatcher<int> ActorArrayLength;

    public static MemoryWatcher<byte> RNGArrayOpBytes;

    public static void Initialize(Process process)
    {
        Process = process;
        processBaseAddress = process.Modules[0].BaseAddress.ToInt32();
        DiagnosticLog.Information($"Process base address: {processBaseAddress:X8}");

        Language = GetMemoryWatcher<byte>(MemoryLocations.Language);

        RoomNumber = GetMemoryWatcher<short>(MemoryLocations.RoomNumber);
        Storyline = GetMemoryWatcher<short>(MemoryLocations.Storyline);
        ForceLoad = GetMemoryWatcher<byte>(MemoryLocations.ForceLoad);
        SpawnPoint = GetMemoryWatcher<byte>(MemoryLocations.SpawnPoint);
        BattleState = GetMemoryWatcher<short>(MemoryLocations.BattleState);
        BattleState2 = GetMemoryWatcher<short>(MemoryLocations.BattleState2);
        Input = GetMemoryWatcher<short>(MemoryLocations.Input);
        Menu = GetMemoryWatcher<byte>(MemoryLocations.Menu);
        MenuLock = GetMemoryWatcher<byte>(MemoryLocations.MenuLock);
        Intro = GetMemoryWatcher<short>(MemoryLocations.Intro);
        State = GetMemoryWatcher<sbyte>(MemoryLocations.State);
        XCoordinate = GetMemoryWatcher<float>(MemoryLocations.XCoordinate);
        YCoordinate = GetMemoryWatcher<float>(MemoryLocations.YCoordinate);
        Camera = GetMemoryWatcher<byte>(MemoryLocations.Camera);
        Camera_x = GetMemoryWatcher<float>(MemoryLocations.Camera_x);
        Camera_y = GetMemoryWatcher<float>(MemoryLocations.Camera_y);
        Camera_z = GetMemoryWatcher<float>(MemoryLocations.Camera_z);
        CameraRotation = GetMemoryWatcher<float>(MemoryLocations.CameraRotation);
        EncounterStatus = GetMemoryWatcher<byte>(MemoryLocations.EncounterStatus);
        MovementLock = GetMemoryWatcher<byte>(MemoryLocations.MovementLock);
        ActiveMusicId = GetMemoryWatcher<byte>(MemoryLocations.ActiveMusicId);
        MusicId = GetMemoryWatcher<byte>(MemoryLocations.MusicId);
        RoomNumberAlt = GetMemoryWatcher<byte>(MemoryLocations.RoomNumberAlt);
        CutsceneAlt = GetMemoryWatcher<short>(MemoryLocations.CutsceneAlt);
        AirshipDestinations = GetMemoryWatcher<short>(MemoryLocations.AirshipDestinations);
        AuronOverdrives = GetMemoryWatcher<byte>(MemoryLocations.AuronOverdrives);
        Gil = GetMemoryWatcher<int>(MemoryLocations.Gil);
        TargetFramerate = GetMemoryWatcher<int>(MemoryLocations.TargetFramerate);
        Dialogue1 = GetMemoryWatcher<int>(MemoryLocations.Dialogue1);
        DialogueOption = GetMemoryWatcher<byte>(MemoryLocations.DialogueOption);
        DialogueBoxOpen = GetMemoryWatcher<byte>(MemoryLocations.DialogueBoxOpen);
        DialogueOption_Gui = GetMemoryWatcher<byte>(MemoryLocations.DialogueOption_Gui);
        DialogueBoxOpen_Gui = GetMemoryWatcher<byte>(MemoryLocations.DialogueBoxOpen_Gui);
        PlayerTurn = GetMemoryWatcher<byte>(MemoryLocations.PlayerTurn);
        FrameCounterFromLoad = GetMemoryWatcher<int>(MemoryLocations.FrameCounterFromLoad);

        // Event File
        EventFileStart = GetMemoryWatcher<int>(MemoryLocations.EventFileStart);

        // Deep Pointers
        HpEnemyA = GetMemoryWatcher<int>(MemoryLocations.HpEnemyA);
        GuadoCount = GetMemoryWatcher<byte>(MemoryLocations.GuadoCount);
        NPCLastInteraction = GetMemoryWatcher<short>(MemoryLocations.NPCLastInteraction);
        TidusActionCount = GetMemoryWatcher<byte>(MemoryLocations.TidusActionCount);
        TidusXCoordinate = GetMemoryWatcher<float>(MemoryLocations.TidusXCoordinate);
        TidusYCoordinate = GetMemoryWatcher<float>(MemoryLocations.TidusYCoordinate);
        TidusZCoordinate = GetMemoryWatcher<float>(MemoryLocations.TidusZCoordinate);
        TidusRotation = GetMemoryWatcher<float>(MemoryLocations.TidusRotation);
        DialogueFile = GetMemoryWatcher<byte>(MemoryLocations.DialogueFile);
        CutsceneTiming = GetMemoryWatcher<byte>(MemoryLocations.CutsceneTiming);
        AuronTransition = GetMemoryWatcher<int>(MemoryLocations.AuronTransition);
        AmmesTransition = GetMemoryWatcher<int>(MemoryLocations.AmmesTransition);
        TankerTransition = GetMemoryWatcher<int>(MemoryLocations.TankerTransition);
        InsideSinTransition = GetMemoryWatcher<int>(MemoryLocations.InsideSinTransition);
        DiveTransition = GetMemoryWatcher<int>(MemoryLocations.DiveTransition);
        GeosTransition = GetMemoryWatcher<int>(MemoryLocations.GeosTransition);
        KlikkTransition = GetMemoryWatcher<int>(MemoryLocations.KlikkTransition);
        AlBhedBoatTransition = GetMemoryWatcher<int>(MemoryLocations.AlBhedBoatTransition);
        UnderwaterRuinsTransition = GetMemoryWatcher<int>(MemoryLocations.UnderwaterRuinsTransition);
        UnderwaterRuinsTransition2 = GetMemoryWatcher<int>(MemoryLocations.UnderwaterRuinsTransition2);
        BeachTransition = GetMemoryWatcher<int>(MemoryLocations.BeachTransition);
        LagoonTransition1 = GetMemoryWatcher<int>(MemoryLocations.LagoonTransition1);
        LagoonTransition2 = GetMemoryWatcher<int>(MemoryLocations.LagoonTransition2);
        ValeforTransition = GetMemoryWatcher<int>(MemoryLocations.ValeforTransition);
        KimahriTransition = GetMemoryWatcher<int>(MemoryLocations.KimahriTransition);
        YunaBoatTransition = GetMemoryWatcher<int>(MemoryLocations.YunaBoatTransition);
        SinFinTransition = GetMemoryWatcher<int>(MemoryLocations.SinFinTransition);
        EchuillesTransition = GetMemoryWatcher<int>(MemoryLocations.EchuillesTransition);
        GeneauxTransition = GetMemoryWatcher<int>(MemoryLocations.GeneauxTransition);
        KilikaTrialsTransition = GetMemoryWatcher<int>(MemoryLocations.KilikaTrialsTransition);
        KilikaAntechamberTransition = GetMemoryWatcher<int>(MemoryLocations.KilikaAntechamberTransition);
        IfritTransition = GetMemoryWatcher<int>(MemoryLocations.IfritTransition);
        IfritTransition2 = GetMemoryWatcher<int>(MemoryLocations.IfritTransition2);
        JechtShotTransition = GetMemoryWatcher<int>(MemoryLocations.JechtShotTransition);
        OblitzeratorTransition = GetMemoryWatcher<int>(MemoryLocations.OblitzeratorTransition);
        BlitzballTransition = GetMemoryWatcher<int>(MemoryLocations.BlitzballTransition);
        SahaginTransition = GetMemoryWatcher<int>(MemoryLocations.SahaginTransition);
        GarudaTransition = GetMemoryWatcher<int>(MemoryLocations.GarudaTransition);
        RinTransition = GetMemoryWatcher<int>(MemoryLocations.RinTransition);
        ChocoboEaterTransition = GetMemoryWatcher<int>(MemoryLocations.ChocoboEaterTransition);
        GuiTransition = GetMemoryWatcher<int>(MemoryLocations.GuiTransition);
        Gui2Transition = GetMemoryWatcher<int>(MemoryLocations.Gui2Transition);
        DjoseTransition = GetMemoryWatcher<int>(MemoryLocations.DjoseTransition);
        IxionTransition = GetMemoryWatcher<int>(MemoryLocations.IxionTransition);
        ExtractorTransition = GetMemoryWatcher<int>(MemoryLocations.ExtractorTransition);
        SeymoursHouseTransition1 = GetMemoryWatcher<int>(MemoryLocations.SeymoursHouseTransition1);
        SeymoursHouseTransition2 = GetMemoryWatcher<int>(MemoryLocations.SeymoursHouseTransition2);
        FarplaneTransition1 = GetMemoryWatcher<int>(MemoryLocations.FarplaneTransition1);
        FarplaneTransition2 = GetMemoryWatcher<int>(MemoryLocations.FarplaneTransition2);
        TromellTransition = GetMemoryWatcher<int>(MemoryLocations.TromellTransition);
        CrawlerTransition = GetMemoryWatcher<int>(MemoryLocations.CrawlerTransition);
        SeymourTransition = GetMemoryWatcher<int>(MemoryLocations.SeymourTransition);
        SeymourTransition2 = GetMemoryWatcher<int>(MemoryLocations.SeymourTransition2);
        WendigoTransition = GetMemoryWatcher<int>(MemoryLocations.WendigoTransition);
        SpherimorphTransition = GetMemoryWatcher<int>(MemoryLocations.SpherimorphTransition);
        UnderLakeTransition = GetMemoryWatcher<int>(MemoryLocations.UnderLakeTransition);
        BikanelTransition = GetMemoryWatcher<int>(MemoryLocations.BikanelTransition);
        HomeTransition = GetMemoryWatcher<int>(MemoryLocations.HomeTransition);
        EvraeTransition = GetMemoryWatcher<int>(MemoryLocations.EvraeTransition);
        EvraeAirshipTransition = GetMemoryWatcher<int>(MemoryLocations.EvraeAirshipTransition);
        GuardsTransition = GetMemoryWatcher<int>(MemoryLocations.GuardsTransition);
        BahamutTransition = GetMemoryWatcher<int>(MemoryLocations.BahamutTransition);
        IsaaruTransition = GetMemoryWatcher<int>(MemoryLocations.IsaaruTransition);
        AltanaTransition = GetMemoryWatcher<int>(MemoryLocations.AltanaTransition);
        NatusTransition = GetMemoryWatcher<int>(MemoryLocations.NatusTransition);
        DefenderXTransition = GetMemoryWatcher<int>(MemoryLocations.DefenderXTransition);
        RonsoTransition = GetMemoryWatcher<int>(MemoryLocations.RonsoTransition);
        FluxTransition = GetMemoryWatcher<int>(MemoryLocations.FluxTransition);
        SanctuaryTransition = GetMemoryWatcher<int>(MemoryLocations.SanctuaryTransition);
        SpectralKeeperTransition = GetMemoryWatcher<int>(MemoryLocations.SpectralKeeperTransition);
        SpectralKeeperTransition2 = GetMemoryWatcher<int>(MemoryLocations.SpectralKeeperTransition2);
        YunalescaTransition = GetMemoryWatcher<int>(MemoryLocations.YunalescaTransition);
        FinsTransition = GetMemoryWatcher<int>(MemoryLocations.FinsTransition);
        FinsAirshipTransition = GetMemoryWatcher<int>(MemoryLocations.FinsAirshipTransition);
        SinCoreTransition = GetMemoryWatcher<int>(MemoryLocations.SinCoreTransition);
        OverdriveSinTransition = GetMemoryWatcher<int>(MemoryLocations.OverdriveSinTransition);
        OmnisTransition = GetMemoryWatcher<int>(MemoryLocations.OmnisTransition);
        BFATransition = GetMemoryWatcher<int>(MemoryLocations.BFATransition);
        AeonTransition = GetMemoryWatcher<int>(MemoryLocations.AeonTransition);
        YuYevonTransition = GetMemoryWatcher<int>(MemoryLocations.YuYevonTransition);
        YojimboFaythTransition = GetMemoryWatcher<int>(MemoryLocations.YojimboFaythTransition);
        CutsceneProgress_Max = GetMemoryWatcher<int>(MemoryLocations.CutsceneProgress_Max);
        CutsceneProgress_uVar1 = GetMemoryWatcher<int>(MemoryLocations.CutsceneProgress_uVar1);
        CutsceneProgress_uVar2 = GetMemoryWatcher<int>(MemoryLocations.CutsceneProgress_uVar2);
        CutsceneProgress_uVar3 = GetMemoryWatcher<int>(MemoryLocations.CutsceneProgress_uVar3);


        //Encounters
        EncounterMapID = GetMemoryWatcher<byte>(MemoryLocations.EncounterMapID);
        EncounterFormationID1 = GetMemoryWatcher<byte>(MemoryLocations.EncounterFormationID1);
        EncounterFormationID2 = GetMemoryWatcher<byte>(MemoryLocations.EncounterFormationID2);
        ScriptedBattleFlag1 = GetMemoryWatcher<byte>(MemoryLocations.ScriptedBattleFlag1);
        ScriptedBattleFlag2 = GetMemoryWatcher<byte>(MemoryLocations.ScriptedBattleFlag2);
        ScriptedBattleVar1 = GetMemoryWatcher<int>(MemoryLocations.ScriptedBattleVar1);
        ScriptedBattleVar3 = GetMemoryWatcher<int>(MemoryLocations.ScriptedBattleVar3);
        ScriptedBattleVar4 = GetMemoryWatcher<int>(MemoryLocations.ScriptedBattleVar4);
        EncounterTrigger = GetMemoryWatcher<byte>(MemoryLocations.EncounterTrigger);

        // Party Configuration
        Formation = GetMemoryWatcher<byte>(MemoryLocations.Formation);
        RikkuName = GetMemoryWatcher<byte>(MemoryLocations.RikkuName);
        EnableTidus = GetMemoryWatcher<byte>(MemoryLocations.EnableTidus);
        EnableYuna = GetMemoryWatcher<byte>(MemoryLocations.EnableYuna);
        EnableAuron = GetMemoryWatcher<byte>(MemoryLocations.EnableAuron);
        EnableKimahri = GetMemoryWatcher<byte>(MemoryLocations.EnableKimahri);
        EnableWakka = GetMemoryWatcher<byte>(MemoryLocations.EnableWakka);
        EnableLulu = GetMemoryWatcher<byte>(MemoryLocations.EnableLulu);
        EnableRikku = GetMemoryWatcher<byte>(MemoryLocations.EnableRikku);
        EnableSeymour = GetMemoryWatcher<byte>(MemoryLocations.EnableSeymour);
        EnableValefor = GetMemoryWatcher<byte>(MemoryLocations.EnableValefor);
        EnableIfrit = GetMemoryWatcher<byte>(MemoryLocations.EnableIfrit);
        EnableIxion = GetMemoryWatcher<byte>(MemoryLocations.EnableIxion);
        EnableShiva = GetMemoryWatcher<byte>(MemoryLocations.EnableShiva);
        EnableBahamut = GetMemoryWatcher<byte>(MemoryLocations.EnableBahamut);
        EnableAnima = GetMemoryWatcher<byte>(MemoryLocations.EnableAnima);
        EnableYojimbo = GetMemoryWatcher<byte>(MemoryLocations.EnableYojimbo);
        EnableMagus = GetMemoryWatcher<byte>(MemoryLocations.EnableMagus);

        // Encounter Rate
        EncountersActiveFlag = GetMemoryWatcher<byte>(MemoryLocations.EncountersActiveFlag);
        TotalDistance = GetMemoryWatcher<float>(MemoryLocations.TotalDistance);
        CycleDistance = GetMemoryWatcher<float>(MemoryLocations.CycleDistance);

        // HP/MP
        TidusHP = GetMemoryWatcher<int>(MemoryLocations.TidusHP);
        TidusMP = GetMemoryWatcher<short>(MemoryLocations.TidusMP);
        TidusMaxHP = GetMemoryWatcher<int>(MemoryLocations.TidusMaxHP);
        TidusMaxMP = GetMemoryWatcher<short>(MemoryLocations.TidusMaxMP);
        YunaHP = GetMemoryWatcher<int>(MemoryLocations.YunaHP);
        YunaMP = GetMemoryWatcher<short>(MemoryLocations.YunaMP);
        YunaMaxHP = GetMemoryWatcher<int>(MemoryLocations.YunaMaxHP);
        YunaMaxMP = GetMemoryWatcher<short>(MemoryLocations.YunaMaxMP);
        AuronHP = GetMemoryWatcher<int>(MemoryLocations.AuronHP);
        AuronMP = GetMemoryWatcher<short>(MemoryLocations.AuronMP);
        AuronMaxHP = GetMemoryWatcher<int>(MemoryLocations.AuronMaxHP);
        AuronMaxMP = GetMemoryWatcher<short>(MemoryLocations.AuronMaxMP);
        WakkaHP = GetMemoryWatcher<int>(MemoryLocations.WakkaHP);
        WakkaMP = GetMemoryWatcher<short>(MemoryLocations.WakkaMP);
        WakkaMaxHP = GetMemoryWatcher<int>(MemoryLocations.WakkaMaxHP);
        WakkaMaxMP = GetMemoryWatcher<short>(MemoryLocations.WakkaMaxMP);
        KimahriHP = GetMemoryWatcher<int>(MemoryLocations.KimahriHP);
        KimahriMP = GetMemoryWatcher<short>(MemoryLocations.KimahriMP);
        KimahriMaxHP = GetMemoryWatcher<int>(MemoryLocations.KimahriMaxHP);
        KimahriMaxMP = GetMemoryWatcher<short>(MemoryLocations.KimahriMaxMP);
        LuluHP = GetMemoryWatcher<int>(MemoryLocations.LuluHP);
        LuluMP = GetMemoryWatcher<short>(MemoryLocations.LuluMP);
        LuluMaxHP = GetMemoryWatcher<int>(MemoryLocations.LuluMaxHP);
        LuluMaxMP = GetMemoryWatcher<short>(MemoryLocations.LuluMaxMP);
        RikkuHP = GetMemoryWatcher<int>(MemoryLocations.RikkuHP);
        RikkuMP = GetMemoryWatcher<short>(MemoryLocations.RikkuMP);
        RikkuMaxHP = GetMemoryWatcher<int>(MemoryLocations.RikkuMaxHP);
        RikkuMaxMP = GetMemoryWatcher<short>(MemoryLocations.RikkuMaxMP);
        ValeforHP = GetMemoryWatcher<int>(MemoryLocations.ValeforHP);
        ValeforMP = GetMemoryWatcher<short>(MemoryLocations.ValeforMP);
        ValeforMaxHP = GetMemoryWatcher<int>(MemoryLocations.ValeforMaxHP);
        ValeforMaxMP = GetMemoryWatcher<short>(MemoryLocations.ValeforMaxMP);

        // Special Flags
        FangirlsOrKidsSkip = GetMemoryWatcher<short>(MemoryLocations.FangirlsOrKidsSkip);
        BaajFlag1 = GetMemoryWatcher<byte>(MemoryLocations.BaajFlag1);
        BesaidFlag1 = GetMemoryWatcher<byte>(MemoryLocations.BesaidFlag1);
        SSWinnoFlag1 = GetMemoryWatcher<byte>(MemoryLocations.SSWinnoFlag1);
        KilikaMapFlag = GetMemoryWatcher<byte>(MemoryLocations.KilikaMapFlag);
        SSWinnoFlag2 = GetMemoryWatcher<byte>(MemoryLocations.SSWinnoFlag2);
        LucaFlag = GetMemoryWatcher<byte>(MemoryLocations.LucaFlag);
        LucaFlag2 = GetMemoryWatcher<byte>(MemoryLocations.LucaFlag2);
        BlitzballFlag = GetMemoryWatcher<byte>(MemoryLocations.BlitzballFlag);
        MiihenFlag1 = GetMemoryWatcher<byte>(MemoryLocations.MiihenFlag1);
        MiihenFlag2 = GetMemoryWatcher<byte>(MemoryLocations.MiihenFlag2);
        MiihenFlag3 = GetMemoryWatcher<byte>(MemoryLocations.MiihenFlag3);
        MiihenFlag4 = GetMemoryWatcher<byte>(MemoryLocations.MiihenFlag4);
        MRRFlag1 = GetMemoryWatcher<byte>(MemoryLocations.MRRFlag1);
        MRRFlag2 = GetMemoryWatcher<byte>(MemoryLocations.MRRFlag2);
        MoonflowFlag = GetMemoryWatcher<byte>(MemoryLocations.MoonflowFlag);
        MoonflowFlag2 = GetMemoryWatcher<byte>(MemoryLocations.MoonflowFlag2);
        RikkuOutfit = GetMemoryWatcher<byte>(MemoryLocations.RikkuOutfit);
        TidusWeaponDamageBoost = GetMemoryWatcher<byte>(MemoryLocations.TidusWeaponDamageBoost);
        GuadosalamShopFlag = GetMemoryWatcher<byte>(MemoryLocations.GuadosalamShopFlag);
        ThunderPlainsFlag = GetMemoryWatcher<byte>(MemoryLocations.ThunderPlainsFlag);
        MacalaniaFlag = GetMemoryWatcher<byte>(MemoryLocations.MacalaniaFlag);
        BikanelFlag = GetMemoryWatcher<byte>(MemoryLocations.BikanelFlag);
        Sandragoras = GetMemoryWatcher<byte>(MemoryLocations.Sandragoras);
        ViaPurificoPlatform = GetMemoryWatcher<byte>(MemoryLocations.ViaPurificoPlatform);
        NatusFlag = GetMemoryWatcher<byte>(MemoryLocations.NatusFlag);
        CalmLandsFlag = GetMemoryWatcher<ushort>(MemoryLocations.CalmLandsFlag);
        WantzFlag = GetMemoryWatcher<byte>(MemoryLocations.WantzFlag);
        GagazetCaveFlag = GetMemoryWatcher<short>(MemoryLocations.GagazetCaveFlag);
        OmegaRuinsFlag = GetMemoryWatcher<byte>(MemoryLocations.OmegaRuinsFlag);
        WantzMacalaniaFlag = GetMemoryWatcher<byte>(MemoryLocations.WantzMacalaniaFlag);

        // Blitzball Abilities
        AurochsTeamBytes = GetMemoryWatcher<byte>(MemoryLocations.AurochsTeamBytes);
        BlitzballBytes = GetMemoryWatcher<byte>(MemoryLocations.BlitzballBytes);
        AurochsPlayer1 = GetMemoryWatcher<byte>(MemoryLocations.AurochsPlayer1);

        // Battle Rewards
        GilBattleRewards = GetMemoryWatcher<int>(MemoryLocations.GilBattleRewards);
        BattleRewardItemCount = GetMemoryWatcher<byte>(MemoryLocations.BattleRewardItemCount);
        BattleRewardItem1 = GetMemoryWatcher<short>(MemoryLocations.BattleRewardItem1);
        BattleRewardItem2 = GetMemoryWatcher<short>(MemoryLocations.BattleRewardItem2);
        BattleRewardItem3 = GetMemoryWatcher<short>(MemoryLocations.BattleRewardItem3);
        BattleRewardItem4 = GetMemoryWatcher<short>(MemoryLocations.BattleRewardItem4);
        BattleRewardItem5 = GetMemoryWatcher<short>(MemoryLocations.BattleRewardItem5);
        BattleRewardItem6 = GetMemoryWatcher<short>(MemoryLocations.BattleRewardItem6);
        BattleRewardItem7 = GetMemoryWatcher<short>(MemoryLocations.BattleRewardItem7);
        BattleRewardItem8 = GetMemoryWatcher<short>(MemoryLocations.BattleRewardItem8);
        BattleRewardItemQty1 = GetMemoryWatcher<byte>(MemoryLocations.BattleRewardItemQty1);
        BattleRewardItemQty2 = GetMemoryWatcher<byte>(MemoryLocations.BattleRewardItemQty2);
        BattleRewardItemQty3 = GetMemoryWatcher<byte>(MemoryLocations.BattleRewardItemQty3);
        BattleRewardItemQty4 = GetMemoryWatcher<byte>(MemoryLocations.BattleRewardItemQty4);
        BattleRewardItemQty5 = GetMemoryWatcher<byte>(MemoryLocations.BattleRewardItemQty5);
        BattleRewardItemQty6 = GetMemoryWatcher<byte>(MemoryLocations.BattleRewardItemQty6);
        BattleRewardItemQty7 = GetMemoryWatcher<byte>(MemoryLocations.BattleRewardItemQty7);
        BattleRewardItemQty8 = GetMemoryWatcher<byte>(MemoryLocations.BattleRewardItemQty8);
        BattleRewardEquipCount = GetMemoryWatcher<byte>(MemoryLocations.BattleRewardEquipCount);
        BattleRewardEquip1 = GetMemoryWatcher<byte>(MemoryLocations.BattleRewardEquip1);
        BattleRewardEquip2 = GetMemoryWatcher<byte>(MemoryLocations.BattleRewardEquip2);
        BattleRewardEquip3 = GetMemoryWatcher<byte>(MemoryLocations.BattleRewardEquip3);
        BattleRewardEquip4 = GetMemoryWatcher<byte>(MemoryLocations.BattleRewardEquip4);
        BattleRewardEquip5 = GetMemoryWatcher<byte>(MemoryLocations.BattleRewardEquip5);
        BattleRewardEquip6 = GetMemoryWatcher<byte>(MemoryLocations.BattleRewardEquip6);
        BattleRewardEquip7 = GetMemoryWatcher<byte>(MemoryLocations.BattleRewardEquip7);
        BattleRewardEquip8 = GetMemoryWatcher<byte>(MemoryLocations.BattleRewardEquip8);

        //Items
        ItemsStart = GetMemoryWatcher<byte>(MemoryLocations.ItemsStart);
        ItemsQtyStart = GetMemoryWatcher<byte>(MemoryLocations.ItemsQtyStart);

        // AP
        CharacterAPFlags = GetMemoryWatcher<byte>(MemoryLocations.CharacterAPFlags);

        // Menu Values
        MenuTriggerValue = GetMemoryWatcher<int>(MemoryLocations.MenuTriggerValue);

        MenuValue1 = GetMemoryWatcher<int>(MemoryLocations.MenuValue1);
        MenuValue2 = GetMemoryWatcher<int>(MemoryLocations.MenuValue2);

        MenuValue3 = GetMemoryWatcher<int>(MemoryLocations.MenuValue3);
        MenuValue4 = GetMemoryWatcher<int>(MemoryLocations.MenuValue4);
        MenuValue5 = GetMemoryWatcher<byte>(MemoryLocations.MenuValue5);
        MenuValue6 = GetMemoryWatcher<int>(MemoryLocations.MenuValue6);
        MenuValue7 = GetMemoryWatcher<byte>(MemoryLocations.MenuValue7);

        SpeedBoostAmount = GetMemoryWatcher<int>(MemoryLocations.SpeedBoostAmount);
        SpeedBoostVar1 = GetMemoryWatcher<int>(MemoryLocations.SpeedBoostVar1);

        ActorArrayLength = GetMemoryWatcher<int>(MemoryLocations.ActorArrayLength);

        // RNGMod
        RNGArrayOpBytes = GetMemoryWatcher<byte>(MemoryLocations.RNGArrayOpBytes);

        HpEnemyA.FailAction = MemoryWatcher.ReadFailAction.SetZeroOrNull;

        Watchers.Clear();

        foreach (FieldInfo field in typeof(MemoryWatchers).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(MemoryWatcher<>))
            {
                Watchers.Add(field.GetValue(null) as MemoryWatcher);
            }
        }
    }

    private static MemoryWatcher<T> GetMemoryWatcher<T>(MemoryLocation data) where T : struct
    {
        MemoryWatcher<T> watcher = data.Offsets.Length == 0
            ? new MemoryWatcher<T>(GetPointerAddress(data.BaseAddress))
            : new MemoryWatcher<T>(GetDeepPointer(data.BaseAddress, data.Offsets));

        watcher.Name = data.Name;
        return watcher;
    }

    private static IntPtr GetPointerAddress(int offset)
    {
        return new IntPtr(processBaseAddress + offset);
    }

    private static DeepPointer GetDeepPointer(int baseAddress, int[] offsets)
    {
        return new DeepPointer(MODULE, baseAddress, offsets);
    }

    public static int GetBaseAddress()
    {
        return processBaseAddress;
    }
}
