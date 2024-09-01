namespace FFXCutsceneRemover;

class UnderwaterRuinsTransition2 : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        if (Stage == 0)
        {
            base.Execute();

            BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;
            Stage += 1;

        }
        else if (MemoryWatchers.UnderwaterRuinsTransition2.Current >= (BaseCutsceneValue + 0x3870) && Stage == 1)
        {
            WriteValue<int>(MemoryWatchers.UnderwaterRuinsTransition2, BaseCutsceneValue + 0x3AB2);
            Stage += 1;
        }
    }
}