using System.Diagnostics;

using FFXCutsceneRemover.ComponentUtil;

namespace FFXCutsceneRemover;

class ShoopufTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        Process process = MemoryWatchers.Process;

        if (MemoryWatchers.Dialogue1.Current == 2 && MemoryWatchers.DialogueBoxOpen.Current == 1 && Stage == 0)
        {
            base.Execute();
            Stage += 1;

        }
        else if (MemoryWatchers.Dialogue1.Current == 2 && MemoryWatchers.DialogueBoxOpen.Current == 0 && MemoryWatchers.DialogueOption.Current == 1 && Stage == 1)
        {
            process.Suspend();

            new Transition { RoomNumber = 99, Description = "All Aboards!" }.Execute();

            Stage += 1;

            process.Resume();
        }
        else if (MemoryWatchers.Dialogue1.Current == 2 && MemoryWatchers.DialogueBoxOpen.Current == 0 && MemoryWatchers.DialogueOption.Current == 1 && Stage == 1)
        {
            Stage = 0;
        }
    }
}