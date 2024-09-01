using System.Diagnostics;

using FFXCutsceneRemover.ComponentUtil;

namespace FFXCutsceneRemover;

class ChocoboEaterTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        Process process = MemoryWatchers.Process;

        if (MemoryWatchers.ChocoboEaterTransition.Current > 0)
        {
            if (MemoryWatchers.MovementLock.Current == 0x20 && Stage == 0)
            {
                base.Execute();

                BaseCutsceneValue = MemoryWatchers.ChocoboEaterTransition.Current;
                Stage += 1;

            }
            else if (MemoryWatchers.ChocoboEaterTransition.Current >= (BaseCutsceneValue + 0x7A5) && Stage == 1) // 21B , EC
            {
                WriteValue<int>(MemoryWatchers.ChocoboEaterTransition, BaseCutsceneValue + 0xC96);// 30A

                byte[] ActiveParty = process.ReadBytes(MemoryWatchers.Formation.Address, 3);

                for (int i = 0; i < 3; i++)
                {
                    if (ActiveParty[i] == 0x00)
                    {
                        Transition actorPositions;
                        //Position Tidus if he is in the party
                        actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { 1 }, Target_x = 100.0f, Target_y = 0.0f, Target_z = 30.0f };
                        actorPositions.Execute();
                    }
                }

                Stage += 1;
            }
        }
    }
}