using System.Diagnostics;

using FFXCutsceneRemover.ComponentUtil;

namespace FFXCutsceneRemover;

class RonsoTransition : Transition
{
    static private byte[] RonsoFormation = { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0xFF, 0xFF, 0xFF, 0xFF };
    public override void Execute(string defaultDescription = "")
    {
        Process process = MemoryWatchers.Process;
        byte EncountersActive = 0x01;

        if (MemoryWatchers.RonsoTransition.Current > 0)
        {
            if (MemoryWatchers.MovementLock.Current == 0x20 && Stage == 0)
            {
                base.Execute();

                BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;
                EncountersActive = MemoryWatchers.EncountersActiveFlag.Current;

                Stage += 1;

            }
            else if (MemoryWatchers.RonsoTransition.Current > (BaseCutsceneValue + 0x12907) && Stage == 1)
            {
                RonsoFormation = process.ReadBytes(MemoryWatchers.Formation.Address, 10);

                WriteValue<int>(MemoryWatchers.RonsoTransition, BaseCutsceneValue + 0x13F9F);
                Stage += 1;
            }
            else if (MemoryWatchers.RonsoTransition.Current == (BaseCutsceneValue + 0x14056) && MemoryWatchers.Menu.Current == 1 && Stage == 2)
            {
                Transition FormationSwitch = new Transition { ForceLoad = false, ConsoleOutput = true, EncountersActiveFlag = EncountersActive, FormationSwitch = Transition.formations.PostBiranYenke, Formation = RonsoFormation, Description = "Fix party after Biran and Yenke" };
                FormationSwitch.Execute();

                Stage += 1;
            }
        }
    }
}