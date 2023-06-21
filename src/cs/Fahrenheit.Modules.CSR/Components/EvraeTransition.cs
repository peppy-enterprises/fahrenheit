using System.Diagnostics;

using FFXCutsceneRemover.ComponentUtil;

namespace FFXCutsceneRemover;

class EvraeTransition : Transition
{
    static private byte[] formation = new byte[] { 0x00, 0x02, 0x03, 0x04, 0x05, 0x06, 0xFF, 0xFF };
    public override void Execute(string defaultDescription = "")
    {
        Process process = MemoryWatchers.Process;

        if (MemoryWatchers.FrameCounterFromLoad.Current < 5 && MemoryWatchers.State.Current == 0 && Stage == 0)
        {
            process.Suspend();

            new Transition
            {
                EncounterMapID = 52,
                EncounterFormationID2 = 0,
                ScriptedBattleFlag1 = 0,
                ScriptedBattleFlag2 = 1,
                ScriptedBattleVar1 = 0x00014503,
                EncounterTrigger = 2,
                Description = "Evrae",
                ForceLoad = false
            }.Execute();

            Transition actorPositions;
            //Position Tidus
            actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { 4215 }, Target_x = -140.0f, Target_y = -35.0f, Target_z = 80.0f };
            actorPositions.Execute();

            Stage += 1;

            process.Resume();
        }
    }
}