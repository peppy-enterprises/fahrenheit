using System.Collections.Generic;

namespace FFXCutsceneRemover;

class KlikkTransition : Transition
{
    static private List<short> CutsceneAltList = new List<short>(new short[] { 1137 });
    public override void Execute(string defaultDescription = "")
    {
        if (CutsceneAltList.Contains(MemoryWatchers.CutsceneAlt.Current) && Stage == 0)
        {
            base.Execute();

            BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;
            Stage += 1;

        }
        else if (MemoryWatchers.KlikkTransition.Current == (BaseCutsceneValue + 0xA523) && Stage == 1)
        {
            WriteValue<int>(MemoryWatchers.KlikkTransition, BaseCutsceneValue + 0xA7E3);
            Stage += 1;
        }
        else if (MemoryWatchers.KlikkTransition.Current > (BaseCutsceneValue + 0xA7E3) && Stage == 2)
        {
            WriteValue<int>(MemoryWatchers.KlikkTransition, BaseCutsceneValue + 0xA847);
            Stage += 1;
        }
        else if (MemoryWatchers.BattleState2.Current == 1 && Stage == 3)
        {
            WriteValue<int>(MemoryWatchers.KlikkTransition, BaseCutsceneValue + 0xAF27);
            Stage += 1;
        }
    }
}