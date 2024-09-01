using FFXCutsceneRemover.Logging;

namespace FFXCutsceneRemover;

class DefenderXTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        int baseAddress = MemoryWatchers.GetBaseAddress();

        if (Stage == 0 && MemoryWatchers.FrameCounterFromLoad.Current < 10)
        {
            base.Execute();

            BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;
            DiagnosticLog.Information(BaseCutsceneValue.ToString("X8"));
            Stage = 1;

        }
        else if (MemoryWatchers.DefenderXTransition.Current >= (BaseCutsceneValue + 0x5451) && Stage == 1)
        {
            WriteValue<int>(MemoryWatchers.DefenderXTransition, BaseCutsceneValue + 0x586F);
            Stage = 2;
        }
    }
}