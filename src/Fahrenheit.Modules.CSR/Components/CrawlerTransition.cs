using System.Collections.Generic;
using System.Diagnostics;

using FFXCutsceneRemover.ComponentUtil;

namespace FFXCutsceneRemover;

class CrawlerTransition : Transition
{
    static private List<short> CutsceneAltList = new List<short>(new short[] { 1839 });
    public override void Execute(string defaultDescription = "")
    {
        Process process = MemoryWatchers.Process;

        if (MemoryWatchers.CrawlerTransition.Current > 0)
        {
            if (CutsceneAltList.Contains(MemoryWatchers.CutsceneAlt.Current) && Stage == 0)
            {
                base.Execute();

                //BaseCutsceneValue = MemoryWatchers.CrawlerTransition.Current;
                BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;

                Stage += 1;

            }
            //First 4 stages are an attempt to emulate the logic from the PS2 Pnach. Values don't line up perfectly but it works.
            else if (MemoryWatchers.CrawlerTransition.Current == (BaseCutsceneValue + 0xEBC2) && Stage == 1)
            {
                WriteValue<int>(MemoryWatchers.CrawlerTransition, BaseCutsceneValue + 0xF017);
                Stage += 1;
            }
            else if (MemoryWatchers.CrawlerTransition.Current >= (BaseCutsceneValue + 0xF02D) && Stage == 2)
            {
                WriteValue<int>(MemoryWatchers.CrawlerTransition, BaseCutsceneValue + 0xF0F3);
                Stage += 1;
            }
            else if (MemoryWatchers.CrawlerTransition.Current >= (BaseCutsceneValue + 0xF0FC) && Stage == 3)
            {
                WriteValue<int>(MemoryWatchers.CrawlerTransition, BaseCutsceneValue + 0xF113);
                Stage += 1;
            }
            else if (MemoryWatchers.CrawlerTransition.Current >= (BaseCutsceneValue + 0xF113) && Stage == 4)
            {
                WriteValue<int>(MemoryWatchers.CrawlerTransition, BaseCutsceneValue + 0xF19E);
                Stage += 1;
            }
            else if (MemoryWatchers.CrawlerTransition.Current == (BaseCutsceneValue + 0xF1E6) && MemoryWatchers.HpEnemyA.Current < 16000 && MemoryWatchers.HpEnemyA.Old == 16000 && Stage == 5)
            {
                WriteValue<int>(MemoryWatchers.CrawlerTransition, BaseCutsceneValue + 0xF437);
                Stage += 1;
            }

            //A value of +0x886 launches the end of the fight straight into results screen which seems to be the only way to not crash the game post battle.
            //This causes a menu glitch if game is allowed to progress past the item rewards screen so the next stage removes the menu flag once gil has finished
            //ticking and the game will process Crawler post boss logic

            else if (MemoryWatchers.Gil.Current > MemoryWatchers.Gil.Old && Stage == 6)
            {
                Stage += 1;
            }
            else if (MemoryWatchers.Gil.Current == MemoryWatchers.Gil.Old && Stage == 7)
            {
                process.Suspend();

                Transition FormationSwitch = new Transition { ForceLoad = false, ConsoleOutput = false, FormationSwitch = Transition.formations.PostCrawler, Description = "Fix party after Crawler" };
                FormationSwitch.Execute();

                Transition ExitMenu = new Transition { Menu = 0, Description = "Exit Menu", ForceLoad = false };
                ExitMenu.Execute();

                Stage += 1;

                process.Resume();
            }
        }
    }
}