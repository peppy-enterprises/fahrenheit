using System.Diagnostics;

namespace FFXCutsceneRemover;

class KilikaPrayTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        Process process = MemoryWatchers.Process;

        if (MemoryWatchers.NPCLastInteraction.Current == 2 && MemoryWatchers.DialogueBoxOpen.Current == 1 && Stage == 0)
        {
            Stage += 1;
        }
        else if (MemoryWatchers.NPCLastInteraction.Current == 2 && MemoryWatchers.DialogueBoxOpen.Current == 0 && Stage == 1)
        {
            new Transition { RoomNumber = 96, Storyline = 335, SpawnPoint = 0, Description = "Pray or Stand and Watch", PositionTidusAfterLoad = true, Target_x = -17.879f, Target_z = 43.657f, Target_var1 = 74 }.Execute();
            Stage += 1;
        }
    }
}
