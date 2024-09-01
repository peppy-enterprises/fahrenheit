using System.Collections.Generic;

namespace FFXCutsceneRemover;

class JechtShotTransition : Transition
{
    static private List<short> CutsceneAltList = new List<short>(new short[] { 70, 71, 75, 76 });
    public override void Execute(string defaultDescription = "")
    {
        int baseAddress = MemoryWatchers.GetBaseAddress();

        if (MemoryWatchers.JechtShotTransition.Current > 0)
        {
            
            if (Stage == 0)
            {
                base.Execute();

                BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;
                Stage += 1;

            }
            else if (MemoryWatchers.JechtShotTransition.Current == (BaseCutsceneValue + 0xF3A7) && Stage == 1)
            {
                WriteValue<int>(MemoryWatchers.JechtShotTransition, BaseCutsceneValue + 0xF9E4);

                Stage += 1;
            }
            else if (MemoryWatchers.JechtShotTransition.Current == (BaseCutsceneValue + 0xFAAE) && Stage == 2)
            {
                Transition actorPositions;
                //Position Tidus
                actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { 1 }, Target_x = 1.0f, Target_y = -49.996f, Target_z = 172.000f, Target_var1 = 100 };
                actorPositions.Execute();

                //Position Yuna
                actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { 2 }, Target_x = 22.700f, Target_y = -49.996f, Target_z = 104.599f, Target_var1 = 120 };
                actorPositions.Execute();

                Stage += 1;
            }
        }
    }
}