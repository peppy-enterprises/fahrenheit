using System.Collections.Generic;
using System.Diagnostics;

using FFXCutsceneRemover.ComponentUtil;

namespace FFXCutsceneRemover;

class UnderLakeTransition : Transition
{
    new float TidusXCoordinate = 0.0f;
    new float TidusYCoordinate = 0.0f;
    new float TidusZCoordinate = 0.0f;
    new float TidusRotation = 0.0f;

    static private List<short> CutsceneAltList = new List<short>(new short[] { 1137 });
    public override void Execute(string defaultDescription = "")
    {
        Process process = MemoryWatchers.Process;

        if (MemoryWatchers.Storyline.Current == 1600 && MemoryWatchers.State.Current == 0 && MemoryWatchers.NPCLastInteraction.Current == 7) //Rikku is Character 7 on this screen
        {
            process.Suspend();

            base.Execute();

            new Transition { Storyline = 1607, Description = "Rikku wants to be like Yuna" }.Execute();

            process.Resume();

        }
        if (MemoryWatchers.Storyline.Current == 1607 && MemoryWatchers.MovementLock.Current == 0x20 && Stage == 0)
        {
            BaseCutsceneValue = MemoryWatchers.UnderLakeTransition.Current;
            Stage += 1;
        }
        else if (MemoryWatchers.State.Current == 0 && Stage == 1)
        {
            process.Suspend();

            TidusXCoordinate = MemoryWatchers.TidusXCoordinate.Current;
            TidusYCoordinate = MemoryWatchers.TidusYCoordinate.Current;
            TidusZCoordinate = MemoryWatchers.TidusZCoordinate.Current;
            TidusRotation = MemoryWatchers.TidusRotation.Current;

            new Transition { Storyline = 1610, SpawnPoint = 0, Description = "The Hymn is Yevon's gift", PositionTidusAfterLoad = true, Target_x = TidusXCoordinate, Target_y = TidusYCoordinate, Target_z = TidusZCoordinate, Target_rot = TidusRotation }.Execute();

            Stage += 1;

            process.Resume();
        }
        else if (MemoryWatchers.State.Current == 0 && MemoryWatchers.NPCLastInteraction.Current == 5 && Stage == 2) // Auron is character 5 on this screen
        {
            process.Suspend();

            TidusXCoordinate = MemoryWatchers.TidusXCoordinate.Current;
            TidusYCoordinate = MemoryWatchers.TidusYCoordinate.Current;
            TidusZCoordinate = MemoryWatchers.TidusZCoordinate.Current;
            TidusRotation = MemoryWatchers.TidusRotation.Current;

            new Transition { RoomNumber = 129, Storyline = 1704, SpawnPoint = 0, Description = "Bikanel Intro", FormationSwitch = Transition.formations.BikanelStart }.Execute();

            Stage += 1;

            process.Resume();
        }
    }
}