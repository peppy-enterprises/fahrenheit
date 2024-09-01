namespace FFXCutsceneRemover;

class OverdriveSinTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        if (MemoryWatchers.FrameCounterFromLoad.Current >= 10 && Stage == 0)
        {
            base.Execute();

            BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;
            Stage += 1;

        }
        else if (MemoryWatchers.OverdriveSinTransition.Current >= (BaseCutsceneValue + 0x5BF8) && Stage == 1)
        {
            WriteValue<int>(MemoryWatchers.OverdriveSinTransition, BaseCutsceneValue + 0x5E2E);
            Stage += 1;
        }
        else if (MemoryWatchers.BattleState2.Current == 1 && Stage == 2)
        {
            WriteValue<int>(MemoryWatchers.OverdriveSinTransition, BaseCutsceneValue + 0x5F79);
            Stage += 1;
        }
    }
}