using System.Collections.Generic;

namespace FFXCutsceneRemover;

class DjoseTransition : Transition
{
    static private List<short> CutsceneAltList = new List<short>(new short[] { 1623, 16, 96 });
    public override void Execute(string defaultDescription = "")
    {
        int baseAddress = MemoryWatchers.GetBaseAddress();
        if (MemoryWatchers.DjoseTransition.Current > 0)
        {
            if (CutsceneAltList.Contains(MemoryWatchers.CutsceneAlt.Current) && Stage == 0)
            {
                base.Execute();

                BaseCutsceneValue = MemoryWatchers.DjoseTransition.Current;

                Stage += 1;

            }
            else if (MemoryWatchers.DjoseTransition.Current == (BaseCutsceneValue + 0x160) && Stage == 1) // 160
            {
                WriteValue<int>(MemoryWatchers.DjoseTransition, BaseCutsceneValue + 0x4ED);

                Transition actorPositions;
                //Position Tidus
                actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { 1 }, Target_x = 1.628f, Target_y = 0.0f, Target_z = -6.528f };
                actorPositions.Execute();

                Stage += 1;
            }
        }
    }
}