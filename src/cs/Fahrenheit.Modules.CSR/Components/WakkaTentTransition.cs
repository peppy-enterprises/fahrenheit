using System.Diagnostics;

using FFXCutsceneRemover.ComponentUtil;

namespace FFXCutsceneRemover;

class WakkaTentTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        Process process = MemoryWatchers.Process;

        if (MemoryWatchers.Dialogue1.Current == 4 && MemoryWatchers.DialogueBoxOpen.Current == 1 && Stage == 0)
        {
            base.Execute();
            Stage += 1;

        }
        else if (MemoryWatchers.Dialogue1.Current == 4 && MemoryWatchers.DialogueBoxOpen.Current == 0 && MemoryWatchers.DialogueOption.Current == 0 && Stage == 1)
        {
            process.Suspend();

            new Transition { Storyline = 154, Description = "Priest enters Wakka's tent" }.Execute();

            Stage += 1;

            process.Resume();
        }
        else if (MemoryWatchers.Dialogue1.Current == 4 && MemoryWatchers.DialogueBoxOpen.Current == 0 && MemoryWatchers.DialogueOption.Current == 1 && Stage == 1)
        {
            Stage = 0;
        }
    }
}