namespace FFXCutsceneRemover;

class LagoonTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        if (MemoryWatchers.LagoonTransition2.Current > 0)
        {
            if (Stage == 0)
            {
                base.Execute();

                BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;
                Stage += 1;

            }
            else if (MemoryWatchers.LagoonTransition2.Current == (BaseCutsceneValue + 0x3304) && Stage == 1)
            {
                WriteValue<int>(MemoryWatchers.LagoonTransition2, BaseCutsceneValue + 0x33E6);

                Stage += 1;
            }
        }
    }
}