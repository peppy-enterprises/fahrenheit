using System.Collections.Generic;
using System.Diagnostics;

using FFXCutsceneRemover.ComponentUtil;


namespace FFXCutsceneRemover;

class NatusTransition : Transition
{
    static private byte[] formation = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0xFF, 0xFF, 0xFF };

    static private List<short> CutsceneAltList = new List<short>(new short[] { 3751 });
    public override void Execute(string defaultDescription = "")
    {
        Process process = MemoryWatchers.Process;

        if (MemoryWatchers.NatusTransition.Current > 0)
        {
            if (CutsceneAltList.Contains(MemoryWatchers.CutsceneAlt.Current) && Stage == 0)
            {
                base.Execute();

                BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;
                Stage += 1;

            }
            else if (MemoryWatchers.NatusTransition.Current == (BaseCutsceneValue + 0xE0F0) && Stage == 1) // 1893
            {

                Transition FormationSwitch = new Transition { ForceLoad = false, ConsoleOutput = true, FormationSwitch = Transition.formations.PreNatus, Description = "Fix party before Natus" };
                FormationSwitch.Execute();

                formation = process.ReadBytes(MemoryWatchers.Formation.Address, 10);

                WriteValue<int>(MemoryWatchers.NatusTransition, BaseCutsceneValue + 0xE2DF);//

                Transition actorPositions;
                //Position Party Member 1
                actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { (short)(formation[0] + 1) }, Target_x = -31.0f, Target_y = 0.0f, Target_z = -25.0f };
                actorPositions.Execute();

                //Position Party Member 2
                actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { (short)(formation[1] + 1) }, Target_x = 0.0f, Target_y = 0.0f, Target_z = -13.0f };
                actorPositions.Execute();

                //Position Party Member 3
                actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { (short)(formation[2] + 1) }, Target_x = 31.0f, Target_y = 0.0f, Target_z = -25.0f };
                actorPositions.Execute();

                //Position Natus
                actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { 4222 }, Target_x = 0.0f, Target_y = -29.0f, Target_z = -100.0f };
                actorPositions.Execute();

                Stage += 1;
            }
            else if (MemoryWatchers.NatusTransition.Current == (BaseCutsceneValue + 0xE2FD) && MemoryWatchers.HpEnemyA.Current < 36000 && MemoryWatchers.HpEnemyA.Old == 36000 && Stage == 2)
            {
                WriteValue<int>(MemoryWatchers.NatusTransition, BaseCutsceneValue + 0xE395);
                Stage += 1;
            }
        }
    }
}