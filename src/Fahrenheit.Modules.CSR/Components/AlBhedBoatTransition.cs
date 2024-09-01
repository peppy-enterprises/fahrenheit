using System.Collections.Generic;

namespace FFXCutsceneRemover;

class AlBhedBoatTransition : Transition
{
    static private List<short> CutsceneAltList = new List<short>(new short[] { 1281 });
    public override void Execute(string defaultDescription = "")
    {
        if (MemoryWatchers.AlBhedBoatTransition.Current > 0)
        {
            if (CutsceneAltList.Contains(MemoryWatchers.CutsceneAlt.Current) && Stage == 0)
            {
                base.Execute();

                BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;

                Stage += 1;

            }
            else if (MemoryWatchers.AlBhedBoatTransition.Current == (BaseCutsceneValue + 0xEF32) && Stage == 1)
            {
                WriteValue<int>(MemoryWatchers.AlBhedBoatTransition, BaseCutsceneValue + 0xF127);
                Stage += 1;
            }
        }
    }
}