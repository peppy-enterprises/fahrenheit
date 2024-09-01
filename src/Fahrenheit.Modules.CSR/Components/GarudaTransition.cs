namespace FFXCutsceneRemover;

class GarudaTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        int baseAddress = MemoryWatchers.GetBaseAddress();
        if (MemoryWatchers.GarudaTransition.Current > 0)
        {
            if (MemoryWatchers.BattleState.Current == 10 && Stage == 0)
            {
                base.Execute();

                BaseCutsceneValue = MemoryWatchers.GarudaTransition.Current;

                Stage = 1;

            }
            else if (MemoryWatchers.GarudaTransition.Current == (BaseCutsceneValue + 0x21) && MemoryWatchers.HpEnemyA.Current == 0 && Stage == 1)
            {
                WriteValue<int>(MemoryWatchers.GarudaTransition, BaseCutsceneValue + 0x2A9);
                Stage = 2;
            }
            else if (MemoryWatchers.GarudaTransition.Current == (BaseCutsceneValue + 0x566) && MemoryWatchers.HpEnemyA.Current == 0 && Stage == 2) //0x428 Allows for a faster skip but causes motion blur. Need a way to turn off.
            {
                WriteValue<int>(MemoryWatchers.GarudaTransition, BaseCutsceneValue + 0x84C);
                Stage = 3;
            }
        }
    }
}