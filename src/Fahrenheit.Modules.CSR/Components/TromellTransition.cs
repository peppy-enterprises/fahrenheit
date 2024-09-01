using System.Collections.Generic;

namespace FFXCutsceneRemover;

class TromellTransition : Transition
{
    static private List<short> CutsceneAltList = new List<short>(new short[] { 190, 661, 21 });
    public override void Execute(string defaultDescription = "")
    {
        int baseAddress = MemoryWatchers.GetBaseAddress();
        if (MemoryWatchers.TromellTransition.Current > 0)
        {
            if (CutsceneAltList.Contains(MemoryWatchers.CutsceneAlt.Current) && Stage == 0)
            {
                base.Execute();

                BaseCutsceneValue = MemoryWatchers.TromellTransition.Current;

                Stage += 1;

            }
            else if (MemoryWatchers.TromellTransition.Current >= (BaseCutsceneValue + 0x01) && Stage == 1)
            {
                WriteValue<int>(MemoryWatchers.TromellTransition, BaseCutsceneValue + 0xA90);
                Stage += 1;
            }
        }
    }
}