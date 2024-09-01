namespace FFXCutsceneRemover;

class EchuillesTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        if (MemoryWatchers.EchuillesTransition.Current > 0)
        {
            if (Stage == 0)
            {
                base.Execute();

                BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;
                Stage += 1;

            }
            else if (MemoryWatchers.EchuillesTransition.Current >= (BaseCutsceneValue + 0x20D0) && Stage == 1)
            {
                WriteValue<int>(MemoryWatchers.EchuillesTransition, BaseCutsceneValue + 0x248A); // 0x2490

                Transition actorPositions;

                //Position Echuilles
                actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { 4210 }, Target_x = 0.0f, Target_y = -124.0f, Target_z = -40.0f };
                actorPositions.Execute();

                Stage += 1;
            }
        }
    }
}