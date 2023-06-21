namespace FFXCutsceneRemover;

class IsaaruTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        int baseAddress = MemoryWatchers.GetBaseAddress();

            if (Stage == 0)
            {
                base.Execute();

                BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;

                Stage += 1;

            }
            else if (MemoryWatchers.IsaaruTransition.Current == BaseCutsceneValue + 0x7F6C + 0x32 && Stage == 1)
            {
                Formation = new byte[] { 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
                ConsoleOutput = false;
                FullHeal = true;
                base.Execute();

                WriteValue<int>(MemoryWatchers.IsaaruTransition, BaseCutsceneValue + 0x7F6C + 0x2F8);
                Stage += 1;
            }
            else if (MemoryWatchers.IsaaruTransition.Current == (BaseCutsceneValue + 0x7F6C + 0x37A) && Stage == 2)
            {
                WriteValue<int>(MemoryWatchers.IsaaruTransition, BaseCutsceneValue + 0x7F6C + 0x5C4);
                Stage += 1;
            }
    }
}