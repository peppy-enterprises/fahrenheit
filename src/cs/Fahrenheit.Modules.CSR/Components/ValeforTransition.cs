namespace FFXCutsceneRemover;

class ValeforTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        int baseAddress = MemoryWatchers.GetBaseAddress();
        if (MemoryWatchers.ValeforTransition.Current > 0)
        {
            if (Stage == 0)
            {
                base.Execute();

                BaseCutsceneValue = MemoryWatchers.ValeforTransition.Current;
                WriteValue<int>(MemoryWatchers.ValeforTransition, BaseCutsceneValue + 0xAA4);

                Stage = 1;

            }
        }
    }
}