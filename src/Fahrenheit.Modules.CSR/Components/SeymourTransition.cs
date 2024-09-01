using System.Diagnostics;

using FFXCutsceneRemover.ComponentUtil;

namespace FFXCutsceneRemover;

class SeymourTransition : Transition
{
    static private byte[] formation = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0xFF };
    public override void Execute(string defaultDescription = "")
    {
        Process process = MemoryWatchers.Process;

        if (MemoryWatchers.MovementLock.Current == 0 && Stage == 0)
        {
            process.Suspend();

            base.Execute();

            BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;

            Stage += 1;

            process.Resume();
        }
        else if (MemoryWatchers.MovementLock.Current == 0x10 && Stage == 1)
        {
            process.Suspend();

            //WriteValue<int>(MemoryWatchers.SeymourTransition, BaseCutsceneValue + 0x75DF);
            WriteValue<byte>(MemoryWatchers.CutsceneTiming, 0);

            formation = process.ReadBytes(MemoryWatchers.Formation.Address, 7);

            new Transition
            {
                EncounterMapID = 41,
                EncounterFormationID2 = 0,
                ScriptedBattleFlag1 = 0,
                ScriptedBattleFlag2 = 1,
                ScriptedBattleVar1 = 0x00000501,
                ScriptedBattleVar3 = 0x00000000,
                ScriptedBattleVar4 = 0x00000000,
                EncounterTrigger = 2,
                EnableShiva = 0x11,
                Description = "Seymour",
                ForceLoad = false
            }.Execute();

            Transition actorPositions;
            //Position Party Member 1
            actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { (short)(formation[0] + 1) }, Target_x = 35.668f, Target_y = 0.0f, Target_z = -42.0f };
            actorPositions.Execute();

            //Position Party Member 2
            actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { (short)(formation[1] + 1) }, Target_x = 3.668f, Target_y = 0.0f, Target_z = -55.0f };
            actorPositions.Execute();

            //Position Party Member 3
            actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { (short)(formation[2] + 1) }, Target_x = -28.332f, Target_y = 0.0f, Target_z = -42.0f };
            actorPositions.Execute();

            Stage += 1;

            process.Resume();
        }
    }
}