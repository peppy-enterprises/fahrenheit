namespace FFXCutsceneRemover;

class SpectralKeeperTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        int baseAddress = MemoryWatchers.GetBaseAddress();
        
        if (MemoryWatchers.SpectralKeeperTransition.Current > 0)
        {

            if (Stage == 0)
            {
                base.Execute();
                
                new Transition { EncounterMapID = 71, EncounterFormationID2 = 0, ScriptedBattleFlag1 = 0, ScriptedBattleFlag2 = 1, ScriptedBattleVar1 = 0x00000501, EncounterTrigger = 2, Description = "Spectral Keeper", ForceLoad = false }.Execute();
                BaseCutsceneValue = MemoryWatchers.SpectralKeeperTransition.Current;

                Stage = 1;

            }
        }
    }
}