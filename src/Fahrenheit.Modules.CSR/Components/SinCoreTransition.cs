using System.Diagnostics;

using FFXCutsceneRemover.ComponentUtil;

namespace FFXCutsceneRemover;

class SinCoreTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        Process process = MemoryWatchers.Process;

        if (Stage == 0)
        {
            process.Suspend();

            new Transition { EncounterMapID = 75, EncounterFormationID2 = 0, ScriptedBattleFlag1 = 0, ScriptedBattleFlag2 = 1, ScriptedBattleVar1 = 0x00010501, EncounterTrigger = 2, Description = "Sin Core", ForceLoad = false }.Execute();

            Stage += 1;

            process.Resume();
        }
    }
}