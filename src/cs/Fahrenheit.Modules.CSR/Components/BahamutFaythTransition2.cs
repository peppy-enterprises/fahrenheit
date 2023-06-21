using System.Collections.Generic;
using System.Diagnostics;

using FFXCutsceneRemover.ComponentUtil;

namespace FFXCutsceneRemover;

class BahamutFaythTransition2 : Transition
{
    static private List<short> CutsceneAltList2 = new List<short>(new short[] { 4449, 4450 });

    public override void Execute(string defaultDescription = "")
    {
        Process process = MemoryWatchers.Process;

        if (MemoryWatchers.Dialogue1.Current == 30 && Stage == 0)
        {
            process.Suspend();

            new Transition { RoomNumber = 255, Storyline = 2970, SpawnPoint = 0, AddSinLocation = true, PositionTidusAfterLoad = true, Target_x = -242.858f, Target_y = 12.126f, Target_z = 160.448f, Target_rot = 1.556f, Target_var1 = 1390, Description = "Return from Highbridge" }.Execute();

            Stage += 1;

            process.Resume();
        }
    }
}
