using System.Diagnostics;

using FFXCutsceneRemover.ComponentUtil;

namespace FFXCutsceneRemover;

class AmmesTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        Process process = MemoryWatchers.Process;
        if (MemoryWatchers.TidusActionCount.Current == 1 && Stage == 0)
        {
            base.Execute();

            BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;
            Stage = 1;

        }
        else if (MemoryWatchers.AmmesTransition.Current == (BaseCutsceneValue + 0x97FA) && Stage == 1)
        {
            Transition actorPositions;
            //Position Ammes
            actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { 4255 }, Target_x = 843.5f, Target_y = -42.0f, Target_z = -126.7f };
            actorPositions.Execute();

            WriteValue<int>(MemoryWatchers.AmmesTransition, BaseCutsceneValue + 0x9936);// 2AB , 255 , 21A

            Stage += 1;
        }
        else if (MemoryWatchers.AmmesTransition.Current == (BaseCutsceneValue + 0x9A2C) && Stage == 2)
        {
            process.Suspend();

            new Transition{ Storyline = 16, SpawnPoint = 1, Description = "Sinscales to Ammes"}.Execute();

            Stage += 1;

            process.Resume();
        }
        else if (Stage == 3)
        {
            Transition actorPositions;
            //Position Tidus
            actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { 1 }, Target_x = 749.636f, Target_y = -41.589f, Target_z = -71.674f };
            actorPositions.Execute();
            //Position Ammes
            actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { 4255 }, Target_x = 843.5f, Target_y = -42.0f, Target_z = -126.7f };
            actorPositions.Execute();

            Stage += 1;
        }
    }
}