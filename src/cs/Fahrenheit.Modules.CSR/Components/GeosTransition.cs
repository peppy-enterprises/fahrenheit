using System.Collections.Generic;
using System.Diagnostics;

using FFXCutsceneRemover.ComponentUtil;

namespace FFXCutsceneRemover;

class GeosTransition : Transition
{
    static private List<short> CutsceneAltList = new List<short>(new short[] { 1137 });
    public override void Execute(string defaultDescription = "")
    {
        Process process = MemoryWatchers.Process;

        if (Stage == 0)
        {
            base.Execute();

            BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;
            Stage += 1;

        }
        else if (MemoryWatchers.GeosTransition.Current == (BaseCutsceneValue + 0xA4F8) && Stage == 1)
        {
            process.Suspend();

            new Transition { EncounterMapID = 1, EncounterFormationID2 = 0, ScriptedBattleFlag1 = 1, ScriptedBattleFlag2 = 1, ScriptedBattleVar1 = 0x00014501, EncounterTrigger = 2, Description = "Sahagins", ForceLoad = false }.Execute();

            Transition actorPositions;
            //Position Tidus
            actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { 1 }, Target_x = 0.0f, Target_y = -50.0f, Target_z = -20.0f };
            actorPositions.Execute();

            Stage += 1;

            process.Resume();
        }
        else if (MemoryWatchers.BattleState2.Current == 22 && Stage == 2)
        {
            Stage += 1;
        }
        else if (MemoryWatchers.BattleState2.Current == 0 && Stage == 3)
        {
            process.Suspend();

            new Transition { EncounterMapID = 1, EncounterFormationID2 = 1, ScriptedBattleFlag1 = 1, ScriptedBattleFlag2 = 1, ScriptedBattleVar1 = 0x00010501, EncounterTrigger = 2, Description = "Geosgaeno", ForceLoad = false }.Execute();

            Stage += 1;

            process.Resume();
        }
        else if (MemoryWatchers.BattleState2.Current == 22 && Stage == 4)
        {
            Transition actorPositions;
            //Position Tidus
            actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { 1 }, Target_x = 0.0f, Target_y = -50.0f, Target_z = -20.0f };
            actorPositions.Execute();

            Stage += 1;
        }
        else if (MemoryWatchers.BattleState2.Current == 0 && Stage == 5)
        {
            process.Suspend();

            new Transition { RoomNumber = 50, Storyline = 48, SpawnPoint = 0, Description = "Escape from Geogaesno" }.Execute();

            Stage += 1;

            process.Resume();
        }
    }
}