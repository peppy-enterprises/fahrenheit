using System.Collections.Generic;

namespace FFXCutsceneRemover;

class GuardsTransition : Transition
{
    static private List<short> CutsceneAltList = new List<short>(new short[] { 70, 71, 75, 76 });
    public override void Execute(string defaultDescription = "")
    {
        if (MemoryWatchers.State.Current == 1 && Stage == 0)
        {
            base.Execute();

            BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;
            Stage += 1;

        }//*/
        else if (MemoryWatchers.GuardsTransition.Current > (BaseCutsceneValue + 0x86CA) && Stage == 1)
        {
            WriteValue<int>(MemoryWatchers.GuardsTransition, BaseCutsceneValue + 0x90DB);

            Stage += 1;
        }
    }
}