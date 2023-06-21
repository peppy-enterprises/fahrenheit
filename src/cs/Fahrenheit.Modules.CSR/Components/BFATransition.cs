namespace FFXCutsceneRemover;

class BFATransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        if (base.Stage == 0)
        {
            base.Execute();

            BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;
            base.Stage += 1;

        }
        else if (MemoryWatchers.BFATransition.Current >= (BaseCutsceneValue + 0x42) && Stage == 1)
        {
            //WriteValue<int>(MemoryWatchers.BFATransition, BaseCutsceneValue + 0xE1); //Not currently working as desired
            Stage += 1;
        }
        else if (MemoryWatchers.BFATransition.Current >= (BaseCutsceneValue + 0xC310) && Stage == 2) // 0x478
        {
            WriteValue<int>(MemoryWatchers.BFATransition, BaseCutsceneValue + 0xCDB2); // 0xD7F
            Stage += 1;
        }
        else if (MemoryWatchers.BFATransition.Current >= (BaseCutsceneValue + 0xCDCA) && Stage == 3) // 0xD80
        {
            WriteValue<int>(MemoryWatchers.BFATransition, BaseCutsceneValue + 0xCF98); // 0x10BA
            Stage += 1;
        }
        else if (MemoryWatchers.BFATransition.Current >= (BaseCutsceneValue + 0xD00D) && Stage == 4) // 0xD80
        {
            WriteValue<int>(MemoryWatchers.BFATransition, BaseCutsceneValue + 0xD126); // 0x10BA
            Stage += 1;
        }
        else if (MemoryWatchers.BFATransition.Current >= (BaseCutsceneValue + 0xD150) && Stage == 5) // 0x1135
        {
            WriteValue<int>(MemoryWatchers.BFATransition, BaseCutsceneValue + 0xD350); // 0x1442 D31D
            Stage += 1;
        }
    }
}