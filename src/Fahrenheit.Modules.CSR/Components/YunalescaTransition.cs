using System.Collections.Generic;
using System.Diagnostics;

using FFXCutsceneRemover.ComponentUtil;

namespace FFXCutsceneRemover;

class YunalescaTransition : Transition
{
    static private List<short> CutsceneAltList = new List<short>(new short[] { 70, 71, 75, 76 });
    public override void Execute(string defaultDescription = "")
    {
        int baseAddress = MemoryWatchers.GetBaseAddress();

        if (MemoryWatchers.YunalescaTransition.Current > 0)
        {
            Process process = MemoryWatchers.Process;

            if (Stage == 0)
            {
                base.Execute();

                BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;
                Stage += 1;

            }//*/
            else if (MemoryWatchers.YunalescaTransition.Current == (BaseCutsceneValue + 0x5DF0) && Stage == 1)
            {
                WriteValue<int>(MemoryWatchers.YunalescaTransition, BaseCutsceneValue + 0x645A);
                Stage += 1;
            }
            else if (MemoryWatchers.YunalescaTransition.Current == (BaseCutsceneValue + 0x64BA) && Stage == 2)
            {
                WriteValue<int>(MemoryWatchers.YunalescaTransition, BaseCutsceneValue + 0x67B4);
                WriteValue<byte>(MemoryWatchers.CutsceneTiming, 0);
                Stage += 1;
            }
            else if (MemoryWatchers.BattleState2.Current == 1 && Stage == 3)
            {
                WriteValue<int>(MemoryWatchers.YunalescaTransition, BaseCutsceneValue + 0x6C8D);
                Stage += 1;
            }
            else if (MemoryWatchers.Gil.Current > MemoryWatchers.Gil.Old && Stage == 4)
            {
                Stage += 1;
            }
            else if (MemoryWatchers.Gil.Current == MemoryWatchers.Gil.Old && Stage == 5)
            {
                process.Suspend();

                Transition ExitMenu = new Transition { Menu = 0, Description = "Exit Menu", ForceLoad = false };
                ExitMenu.Execute();

                Stage += 1;

                process.Resume();
            }
        }
    }
}