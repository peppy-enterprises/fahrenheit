using System.Diagnostics;

using FFXCutsceneRemover.ComponentUtil;

namespace FFXCutsceneRemover;

class FinLeftTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        Process process = MemoryWatchers.Process;

        if (MemoryWatchers.FinsTransition.Current > 0)
        {
            if (Stage == 0)
            {
                process.Suspend();

                new Transition { EncounterMapID = 73, EncounterFormationID2 = 0, ScriptedBattleFlag1 = 0, ScriptedBattleFlag2 = 1, ScriptedBattleVar1 = 0x00010501, EncounterTrigger = 2, Description = "Left Fin", ForceLoad = false }.Execute();

                Stage += 1;

                process.Resume();
            }
            else if (MemoryWatchers.BattleState2.Current > 0 && Stage == 1)
            {
                Transition actorPositions;
                //Position Sin
                actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { 4232 }, Target_x = 1207.0f, Target_y = -440.0f, Target_z = 428.0f };
                actorPositions.Execute();

                Stage += 1;
            }
        }
    }
}