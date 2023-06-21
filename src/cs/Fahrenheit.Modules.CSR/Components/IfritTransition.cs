namespace FFXCutsceneRemover;

class IfritTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        int baseAddress = MemoryWatchers.GetBaseAddress();
        if (MemoryWatchers.IfritTransition.Current > 0)
        {
            if (Stage == 0)
            {
                base.Execute();

                BaseCutsceneValue = MemoryWatchers.IfritTransition.Current;
                Stage = 1;

            }
            else if (MemoryWatchers.IfritTransition.Current >= (BaseCutsceneValue + 0x34) && Stage == 1)
            {
                WriteValue<int>(MemoryWatchers.IfritTransition, BaseCutsceneValue + 0x1E4);

                Stage = 2;
            }
        }
    }
}