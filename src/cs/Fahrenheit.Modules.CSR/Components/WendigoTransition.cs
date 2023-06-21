using System.Collections.Generic;

namespace FFXCutsceneRemover;

class WendigoTransition : Transition
{
    static private List<short> CutsceneAltList = new List<short>(new short[] { 1137 });
    public override void Execute(string defaultDescription = "")
    {
        if (MemoryWatchers.WendigoTransition.Current > 0)
        {
            if (Stage == 0)
            {
                base.Execute();

                BaseCutsceneValue = MemoryWatchers.WendigoTransition.Current;
                Stage += 1;

            }
            else if (MemoryWatchers.WendigoTransition.Current == (BaseCutsceneValue + 0x19BF) && Stage == 1) // 486
            {
                WriteValue<int>(MemoryWatchers.WendigoTransition, BaseCutsceneValue + 0x1B1E);// 1B44

                Transition actorPositions;
                //Position Party Members off screen
                actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, PositionPartyOffScreen = true, PartyTarget_x = 205.0f, PartyTarget_z = -480.0f };
                actorPositions.Execute();

                //Position Guados
                actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { 4309 }, Target_x = -100.0f, Target_z = -350.0f };
                actorPositions.Execute();

                //Position Wendigo
                actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { 4308 }, Target_x = -100.0f, Target_z = -350.0f };
                actorPositions.Execute();

                Stage += 1;
            }
            else if (MemoryWatchers.WendigoTransition.Current == (BaseCutsceneValue + 0x1B7E) && MemoryWatchers.HpEnemyA.Current < 1200 && MemoryWatchers.HpEnemyA.Old == 1200 && Stage == 2) // 1200 is HP of Guado
            {
                WriteValue<int>(MemoryWatchers.WendigoTransition, BaseCutsceneValue + 0x1E31);// 1E34
                Stage += 1;
            }
        }
    }
}