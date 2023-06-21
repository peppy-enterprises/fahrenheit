using System.Diagnostics;

namespace FFXCutsceneRemover;

class KilikaAntechamberTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        Process process = MemoryWatchers.Process;
        int baseAddress = MemoryWatchers.GetBaseAddress();


        if (Stage == 0)
        {
            base.Execute();

            BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;

            Stage += 1;
        }
        else if (MemoryWatchers.KilikaAntechamberTransition.Current == (BaseCutsceneValue + 0X3CDA) && Stage == 1)
        {
            WriteValue<int>(MemoryWatchers.KilikaAntechamberTransition, BaseCutsceneValue + 0X3DC6);

            Stage += 1;
        }
    }
}