using System.Collections.Generic;

namespace FFXCutsceneRemover;

class OmnisTransition : Transition
{
    static private List<short> CutsceneAltList = new List<short>(new short[] { 5331 });
    public override void Execute(string defaultDescription = "")
    {
        int baseAddress = MemoryWatchers.GetBaseAddress();
        if (MemoryWatchers.OmnisTransition.Current > 0)
        {
            if (CutsceneAltList.Contains(MemoryWatchers.CutsceneAlt.Current) && Stage == 0)
            {
                base.Execute();

                BaseCutsceneValue = MemoryWatchers.OmnisTransition.Current;
                Stage += 1;

            }
            else if (MemoryWatchers.OmnisTransition.Current >= (BaseCutsceneValue + 0x7C) && Stage == 1)
            {
                WriteValue<int>(MemoryWatchers.OmnisTransition, BaseCutsceneValue + 0xA16);

                Stage += 1;
            }
            else if (MemoryWatchers.OmnisTransition.Current >= (BaseCutsceneValue + 0xA52) && MemoryWatchers.HpEnemyA.Current < 80000 && MemoryWatchers.HpEnemyA.Old == 80000 && Stage == 2)
            {
                WriteValue<int>(MemoryWatchers.OmnisTransition, BaseCutsceneValue + 0x1050);

                Stage += 1;
            }
        }
    }
}