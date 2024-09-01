using System.Collections.Generic;

namespace FFXCutsceneRemover;

class SahaginTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        int baseAddress = MemoryWatchers.GetBaseAddress();

        List<short> CutsceneAltList = new List<short>(new short[] { 780, 347, 2253 });

        if (MemoryWatchers.SahaginTransition.Current > 0)
        {
            if (CutsceneAltList.Contains(MemoryWatchers.CutsceneAlt.Current) && Stage == 0)
            {
                base.Execute();

                BaseCutsceneValue = MemoryWatchers.SahaginTransition.Current;
                Stage = 1;

            }
            else if (MemoryWatchers.SahaginTransition.Current == (BaseCutsceneValue + 0xC9) && Stage == 1)
            {
                WriteValue<int>(MemoryWatchers.SahaginTransition, BaseCutsceneValue + 0x45A);

                Transition actorPositions;
                //Position Wakka
                actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { 5 }, Target_x = -20.0f, Target_y = -510.0f, Target_z = 0.0f };
                actorPositions.Execute();

                //Position Tidus
                actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { 1 }, Target_x = 20.0f, Target_y = -510.0f, Target_z = 0.0f };
                actorPositions.Execute();

                //Position Sahagins
                actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { 4252 }, Target_x = 0.0f, Target_y = -510.0f, Target_z = -60.0f };
                actorPositions.Execute();

                Stage = 2;
            }
            else if (MemoryWatchers.SahaginTransition.Current == (BaseCutsceneValue + 0x491) && MemoryWatchers.TidusActionCount.Current == 1 && Stage == 2)
            {
                WriteValue<int>(MemoryWatchers.SahaginTransition, BaseCutsceneValue + 0x556);
                Stage = 3;
            }
        }
    }
}