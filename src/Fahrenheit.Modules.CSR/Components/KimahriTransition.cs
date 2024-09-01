namespace FFXCutsceneRemover;

class KimahriTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        if (MemoryWatchers.MovementLock.Current == 0x20 && Stage == 0)
        {
            base.Execute();

            BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;

            Stage += 1;

        }
        else if (MemoryWatchers.KimahriTransition.Current >= (BaseCutsceneValue + 0x231A) && Stage == 1)
        {
            WriteValue<int>(MemoryWatchers.KimahriTransition, BaseCutsceneValue + 0x23F3);
            Stage += 1;
        }
        else if (MemoryWatchers.BattleState2.Current == 1 && Stage == 2)
        {
            WriteValue<int>(MemoryWatchers.KimahriTransition, BaseCutsceneValue + 0x2AE3);
            Stage += 1;
        }
    }
}