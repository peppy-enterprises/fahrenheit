using System.Diagnostics;

using FFXCutsceneRemover.ComponentUtil;

namespace FFXCutsceneRemover;

class SanctuaryTransition : Transition
{
    static private byte[] formation = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 };

    public override void Execute(string defaultDescription = "")
    {
        Process process = MemoryWatchers.Process;

        if (MemoryWatchers.SanctuaryTransition.Current > 0)
        {
            formation = process.ReadBytes(MemoryWatchers.Formation.Address, 7);

            if (Stage == 0)
            {
                process.Suspend();

                new Transition
                {
                    EncounterMapID = 68,
                    EncounterFormationID2 = 0,
                    ScriptedBattleFlag1 = 0,
                    ScriptedBattleFlag2 = 1,
                    ScriptedBattleVar1 = 0x00010504,
                    ScriptedBattleVar3 = 0x0000013C,
                    ScriptedBattleVar4 = 0x00000014,
                    EncounterTrigger = 2,
                    Description = "Sanctuary Keeper",
                    ForceLoad = false
                }.Execute();

                Stage += 1;

                process.Resume();
            }
            else if (MemoryWatchers.BattleState2.Current == 22 && Stage == 1)
            {
                Transition actorPositions;
                //Position Party Member 1
                actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { (short)(formation[0] + 1) }, Target_x = 1173.339f, Target_y = -30.048f, Target_z = -1097.457f };
                actorPositions.Execute();

                //Position Party Member 2
                actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { (short)(formation[1] + 1) }, Target_x = 1178.746f, Target_y = -30.048f, Target_z = -1135.846f };
                actorPositions.Execute();

                //Position Party Member 3
                actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { (short)(formation[2] + 1) }, Target_x = 1173.225f, Target_y = -30.048f, Target_z = -1180.447f };
                actorPositions.Execute();

                Stage += 1;
            }
        }
    }
}