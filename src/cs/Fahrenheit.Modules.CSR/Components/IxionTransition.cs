using System.Collections.Generic;

namespace FFXCutsceneRemover;

class IxionTransition : Transition
{
    static private List<short> CutsceneAltList = new List<short>(new short[] { 1641, 16, 96 });
    public override void Execute(string defaultDescription = "")
    {
        if (MemoryWatchers.IxionTransition.Current > 0)
        {
            if (MemoryWatchers.State.Current != -1 && Stage == 0)
            {
                base.Execute();

                BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;
                Stage += 1;

            }
            else if (MemoryWatchers.IxionTransition.Current == (BaseCutsceneValue + 0x2241) && Stage == 1)
            {
                WriteValue<int>(MemoryWatchers.IxionTransition, BaseCutsceneValue + 0x2529);
                Stage += 1;
            }
        }
    }
}