namespace FFXCutsceneRemover;

class BahamutTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        if (MemoryWatchers.BahamutTransition.Current > 0)
        {
            if (Stage == 0)
            {
                base.Execute();

                BaseCutsceneValue = MemoryWatchers.BahamutTransition.Current;

                Stage = 1;

            }
            else if (MemoryWatchers.BahamutTransition.Current >= (BaseCutsceneValue + 0x680) && Stage == 1)
            {
                WriteValue<int>(MemoryWatchers.BahamutTransition, BaseCutsceneValue + 0x86E);
                Stage = 2;
            }
        }
    }
}