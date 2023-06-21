using System.Collections.Generic;

namespace FFXCutsceneRemover;

class BlitzballTransition2 : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        int baseAddress = MemoryWatchers.GetBaseAddress();

        List<short> CutsceneAltList = new List<short>(new short[] { 347, 2235 });

        if (MemoryWatchers.BlitzballTransition.Current > 0)
        {
            if (CutsceneAltList.Contains(MemoryWatchers.CutsceneAlt.Current) && Stage == 0)
            {
                base.Execute();

                BaseCutsceneValue = MemoryWatchers.BlitzballTransition.Current;
                Stage = 1;

            }
            else if (MemoryWatchers.BlitzballTransition.Current == (BaseCutsceneValue + 0x2EA1) && Stage == 1)
            {
                WriteValue<int>(MemoryWatchers.BlitzballTransition, BaseCutsceneValue + 0x301C);
                Stage = 2;
            }
        }
    }
}