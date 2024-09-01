namespace FFXCutsceneRemover;

class BikanelTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        if (MemoryWatchers.BikanelTransition.Current > 0)
        {
            if (MemoryWatchers.MovementLock.Current == 0x20 && Stage == 0)
            {
                base.Execute();

                BaseCutsceneValue = MemoryWatchers.BikanelTransition.Current;
                Stage += 1;

            }
            else if (MemoryWatchers.BikanelTransition.Current == (BaseCutsceneValue + 0x11F) && Stage == 1)
            {
                WriteValue<int>(MemoryWatchers.BikanelTransition, BaseCutsceneValue + 0x1DC); // 1DC

                Transition actorPositions;
                // After the transition Kimahri's model is still visible so we bin him off to Narnia
                actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { 4 }, Target_x = 1000.0f, Target_y = 0.0f, Target_z = -1000.0f };
                actorPositions.Execute();

                Stage += 1;
            }
        }
    }
}