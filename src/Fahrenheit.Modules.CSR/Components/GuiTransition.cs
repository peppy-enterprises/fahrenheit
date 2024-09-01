using System.Collections.Generic;
using System.Diagnostics;

using FFXCutsceneRemover.ComponentUtil;

namespace FFXCutsceneRemover;

class GuiTransition : Transition
{
    static private byte[] GuiFormation = { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0xFF, 0xFF, 0xFF, 0xFF };

    static private List<short> CutsceneAltList2 = new List<short>(new short[] { 4449, 4450 });

    public override void Execute(string defaultDescription = "")
    {
        Process process = MemoryWatchers.Process;

        if (MemoryWatchers.Dialogue1.Current == 95 && MemoryWatchers.DialogueBoxOpen_Gui.Current == 1 && Stage == 0)
        {
            Stage += 1;
        }
        else if (MemoryWatchers.DialogueOption_Gui.Current == 0 && MemoryWatchers.DialogueBoxOpen_Gui.Current == 0 && Stage == 1)
        {
            Stage -= 1;
        }
        else if (MemoryWatchers.DialogueOption_Gui.Current == 1 && MemoryWatchers.DialogueBoxOpen_Gui.Current == 0 && Stage == 1)
        {
            process.Suspend();
            
            new Transition
            {
                EncounterMapID = 27,
                EncounterFormationID2 = 0,
                ScriptedBattleFlag1 = 1,
                ScriptedBattleFlag2 = 1,
                ScriptedBattleVar1 = 0x00004501,
                ScriptedBattleVar3 = 0x00000000,
                ScriptedBattleVar4 = 0x00000000,
                EncounterTrigger = 2,
                Description = "Sinspawn Gui 1",
                ForceLoad = false
            }.Execute();

            // Reposition Party Members just off screen to run into battle
            Transition actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, PartyTarget_x = 427.0f, PartyTarget_z = 3350.0f, PositionPartyOffScreen = true };
            actorPositions.Execute();

            Stage += 1;

            process.Resume();
        }
        else if (MemoryWatchers.HpEnemyA.Current == 12000 && Stage == 2)
        {
            Stage += 1;
        }
        else if (MemoryWatchers.BattleState2.Current == 0 && Stage == 3)
        {
            process.Suspend();

            GuiFormation = process.ReadBytes(MemoryWatchers.Formation.Address, 10);

            new Transition
            {
                RoomNumber = 247,
                Storyline = 865,
                EncounterMapID = 29,
                EncounterFormationID2 = 0,
                ScriptedBattleFlag1 = 0,
                ScriptedBattleFlag2 = 1,
                ScriptedBattleVar1 = 0x00014504,
                ScriptedBattleVar3 = 0x00000129,
                ScriptedBattleVar4 = 0x00000014,
                EncounterTrigger = 2,
                FormationSwitch = formations.PreGui2,
                Description = "Sinspawn Gui 2",
                ForceLoad = false
            }.Execute();

            Stage += 1;

            process.Resume();
        }
        else if (MemoryWatchers.Menu.Current == 1 && Stage == 4)
        {
            process.Suspend();

            new Transition { Formation = GuiFormation, FormationSwitch = formations.PostGui, Description = "Fix Party", ForceLoad = false }.Execute();

            Stage += 1;

            process.Resume();
        }
    }
}
