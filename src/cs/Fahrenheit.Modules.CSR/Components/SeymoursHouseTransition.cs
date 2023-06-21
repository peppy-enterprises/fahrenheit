using System;
using System.Diagnostics;

namespace FFXCutsceneRemover;

class SeymoursHouseTransition : Transition
{
    Boolean TalkedToAuron = false;
    Boolean TalkedToWakka = false;
    Boolean TalkedToLulu = false;
    Boolean TalkedToRikku = false;
    public override void Execute(string defaultDescription = "")
    {
        Process process = MemoryWatchers.Process;
        
        if (Stage == 0)
        {
            base.Execute();

            BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;

            Stage += 1;
        }
        else if (MemoryWatchers.SeymoursHouseTransition1.Current == (BaseCutsceneValue + 0x730E) && Stage == 1)
        {
            WriteValue<int>(MemoryWatchers.SeymoursHouseTransition1, BaseCutsceneValue + 0x7372);
            TalkedToAuron = true;
            Stage += 1;
        }
        else if (MemoryWatchers.SeymoursHouseTransition2.Current == (BaseCutsceneValue + 0x7122) && Stage == 2)
        {
            WriteValue<int>(MemoryWatchers.SeymoursHouseTransition2, BaseCutsceneValue + 0x7263);
            TalkedToAuron = true;
            //Stage += 1;
        }
        else if (MemoryWatchers.SeymoursHouseTransition2.Current == (BaseCutsceneValue + 0x6EC1) && Stage == 2)
        {
            WriteValue<int>(MemoryWatchers.SeymoursHouseTransition2, BaseCutsceneValue + 0x70B5);
            TalkedToLulu = true;
            //Stage += 1;
        }
        else if (MemoryWatchers.NPCLastInteraction.Current == 2 && Stage == 2)
        {
            TalkedToWakka = true;
            //Stage += 1;
        }
        else if (MemoryWatchers.NPCLastInteraction.Current == 5 && Stage == 2)
        {
            TalkedToRikku = true;
            //Stage += 1;
        }

        if (TalkedToAuron && TalkedToWakka && TalkedToLulu && TalkedToRikku && MemoryWatchers.NPCLastInteraction.Current == 1)
        {
            new Transition { RoomNumber = 197, Description = "Lady Yuna, this way." }.Execute();
        }
    }
}