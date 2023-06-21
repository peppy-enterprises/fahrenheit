namespace FFXCutsceneRemover;

class HomeTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        if (Stage == 0 && MemoryWatchers.CutsceneAlt.Current == 5043)
        {
            base.Execute();

            BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;
            Stage += 1;

        }
        else if (MemoryWatchers.HomeTransition.Current == (BaseCutsceneValue + 0x5FDB) && Stage == 1)
        {
            WriteValue<int>(MemoryWatchers.HomeTransition, BaseCutsceneValue + 0x61CE);
            Stage += 1;
        }
        else if (MemoryWatchers.BattleState2.Current == 1 && Stage == 2)
        {
            WriteValue<int>(MemoryWatchers.Camera, 0);
            Stage += 1;
        }
        else if (MemoryWatchers.HomeTransition.Current == (BaseCutsceneValue + 0x63AA) && Stage == 3)
        {
            WriteValue<byte>(MemoryWatchers.CutsceneTiming, 0);

            new Transition
            {
                EncounterMapID = 87,
                EncounterFormationID2 = 3,
                ScriptedBattleFlag1 = 0,
                ScriptedBattleFlag2 = 0,
                ScriptedBattleVar1 = 0x00000500,
                ScriptedBattleVar3 = 0x00000000,
                ScriptedBattleVar4 = 0x00000000,
                EncounterTrigger = 1,
                Description = "Home Chimeras",
                ForceLoad = false
            }.Execute();

            Stage += 1;
        }

        if (MemoryWatchers.RoomNumber.Current == 275 || MemoryWatchers.RoomNumber.Current == 286)
        {
            Stage = 99;
        }
        else if (MemoryWatchers.RoomNumber.Current == 280 && Stage == 99)
        {
            BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;
            Stage = 3;
        }
    }
}