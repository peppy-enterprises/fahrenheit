namespace FFXCutsceneRemover;

class AuronTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        if (MemoryWatchers.MovementLock.Current == 0x20 && Stage == 0)
        {
            base.Execute();

            BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;
            Stage = 1;

        }
        else if (MemoryWatchers.AuronTransition.Current == (BaseCutsceneValue + 0x4233) && Stage == 1)
        {
            WriteValue<int>(MemoryWatchers.AuronTransition, BaseCutsceneValue + 0x42EE);
            Stage += 1;
        }
    }
}