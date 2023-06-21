namespace FFXCutsceneRemover;

class SpherimorphTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        if (MemoryWatchers.SpherimorphTransition.Current > 0)
        {
            if (MemoryWatchers.CutsceneAlt.Current == 355 && Stage == 0)
            {
                base.Execute();

                BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;
                Stage += 1;

            }
            else if (MemoryWatchers.SpherimorphTransition.Current == (BaseCutsceneValue + 0x3477) && Stage == 1) // 486
            {
                WriteValue<int>(MemoryWatchers.SpherimorphTransition, BaseCutsceneValue + 0x35A5);// 1B44

                Transition actorPositions;
                //Position Wendigo
                actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { 4217 }, Target_x = 0.0f, Target_y = -14.0f, Target_z = 140.0f };
                actorPositions.Execute();

                Stage += 1;
            }
            else if (MemoryWatchers.BattleState2.Current == 1 && Stage == 2)
            {
                WriteValue<int>(MemoryWatchers.SpherimorphTransition, BaseCutsceneValue + 0x36CC);// 1E34
                Stage += 1;
            }
        }
    }
}