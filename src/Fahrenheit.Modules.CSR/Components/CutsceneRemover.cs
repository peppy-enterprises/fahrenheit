using System;
using System.Collections.Generic;
using System.Diagnostics;

using FFXCutsceneRemover.ComponentUtil;
using FFXCutsceneRemover.Logging;
using FFXCutsceneRemover.Resources;

/*
 * Main loops for the Cutscene Remover program.
 */
namespace FFXCutsceneRemover;

class CutsceneRemover
{
    public Process Game;
    private bool InBossFight = false;
    private bool tidusReset = false;
    private Transition PostBossFightTransition;
    // Keep track of the previously executed transition
    // so we don't execute the same transition twice
    private Transition PreviouslyExecutedTransition;
    private int LoopSleepMillis;

    public CutsceneRemover(int loopSleepMillis)
    {
        LoopSleepMillis = loopSleepMillis;
    }

    public void MainLoop()
    {
        /* This loop iterates over the list of standard transitions
         * and applies them when necessary. Most transitions can be performed here. */
        Dictionary<Func<bool>, Transition> standardTransitions = Transitions.StandardTransitions;
        
        foreach (var transition in standardTransitions)
        {
            if (transition.Key.Invoke() && MemoryWatchers.ForceLoad.Current == 0)
            {
                ExecuteTransition(transition.Value, "Executing Standard Transition - No Description");
            }
        }

        if (MemoryWatchers.RoomNumber.Current == 23)
        {
            foreach (var transition in standardTransitions)
            {
                transition.Value.Stage = 0;
            }
        }

        /* Loop for post boss fights transitions. Once we enter the fight we set the boss bit and the transition
            * to perform once we exit the AP menu. */
        Dictionary<Func<bool>, Transition> postBossBattleTransitions = Transitions.PostBossBattleTransitions;
        
        if (!InBossFight)
        {
            foreach (var transition in postBossBattleTransitions)
            {
                if (transition.Key.Invoke())
                {
                    InBossFight = true;
                    PostBossFightTransition = transition.Value;
                    DiagnosticLog.Information("Entered Boss Fight: " + transition.Value.Description);
                }
            }
        }
        else if (InBossFight && MemoryWatchers.RoomNumber.Current == 23)
        {
            DiagnosticLog.Information("Main menu detected. Exiting boss loop (This means you died or soft-reset)");
            InBossFight = false;
        }
        else if (MemoryWatchers.Menu.Current == 0 && MemoryWatchers.Menu.Old == 1)
        {
            ExecuteTransition(PostBossFightTransition, "Executing Post Boss Fight Transition - No Description");
            DiagnosticLog.Information("Post Boss Fight");
            InBossFight = false;
            PostBossFightTransition = null;
        }

        // SPECIAL CHECKS
        /*
            * A GameState object is created in order to verify the current state of the game
            * based on the inputs provided. Inputs not provided will be ignored.
            * Once the CheckState() returns true, indicating the game is in the state we want,
            * then a Transition object is created with inputs required to execute the transition.
            * The Execute() method causes the transition to write the updated values to memory.
            *
            * IF YOU ONLY NEED TO CHECK IF ADDRESSES ARE CERTAIN VALUES (ALMOST ALL CASES), THEN ADD YOUR
            * TRANSITION INTO THE 'Resources\Transitions.cs' FILE.
            *
            * Rarely there are conditions where the check we want is not equality. In that case you can write your
            * own condition. An example is below.
            * 
            * Make sure to call ExecuteTransition() instead of calling the Transition.Execute() method directly.
            */
        // Soft reset by holding L1 R1 L2 R2 + Start - Disabled in battle because game crashes
#if DEBUG
        if (MemoryWatchers.Input.Current == 2063 && MemoryWatchers.BattleState.Current != 10)
        {
            ExecuteTransition(new Transition { RoomNumber = 23, BattleState = 778, Description = "Soft reset by holding L1 R1 L2 R2 + Start", Repeatable = true });
        }
#endif

        // Custom Check - Airship
        if (MemoryWatchers.RoomNumber.Current == 194 && MemoryWatchers.Storyline.Current == 2000 && MemoryWatchers.State.Current == 0 && MemoryWatchers.XCoordinate.Current > 300f)
        {
            ExecuteTransition(new Transition {RoomNumber = 194, Storyline = 2020, SpawnPoint = 1, PositionTidusAfterLoad = true, Target_x = -242.667f, Target_y = 12.514f, Target_z = 398.095f, Target_rot = -1.659f, Target_var1 = 1463, Description = "Zoom in on Bevelle"});
        }

        // Custom Check - Djose
        if (MemoryWatchers.RoomNumber.Current == 161 && MemoryWatchers.Storyline.Current == 1010 && MemoryWatchers.MovementLock.Current == 48 && MemoryWatchers.YCoordinate.Current > 10.0f)
        {
            ExecuteTransition(new Transition { RoomNumber = 82, Storyline = 1015, SpawnPoint = 2, Description = "Tidus wakes Yuna up"});
        }

        // Custom Check - Zanarkand 
        if (MemoryWatchers.RoomNumber.Current == 368 && MemoryWatchers.Storyline.Current == 3 && MemoryWatchers.FangirlsOrKidsSkip.Current == 3)
        {
            if (MemoryWatchers.TidusXCoordinate.Current < 5.0f && MemoryWatchers.TidusZCoordinate.Current < 8.0f && MemoryWatchers.TidusZCoordinate.Current > -8.0f)
            {
                ExecuteTransition(new Transition { RoomNumber = 376, Storyline = 4, SpawnPoint = 0, Description = "Tidus leaves fans" });
            }
        }
        /*/
        if (!tidusReset && !InBossFight && MemoryWatchers.Storyline.Current != 2080 && MemoryWatchers.FrameCounterFromLoad.Current >= 20)
        {
            new Transition { ForceLoad = false, TargetActorIDs = new short[] { 1 }, Target_var1 = -1 , ConsoleOutput = false}.Execute();
            tidusReset = true;
        }
        else if (tidusReset && (MemoryWatchers.FrameCounterFromLoad.Current < 20))
        {
            tidusReset = false;
        }
        //*/
    }

    // Save the previous transition so that we don't execute the same transition multiple times in a row.
    private void ExecuteTransition(Transition transition, string defaultDescription = "")
    {
        bool suspended = false;

        if (transition != PreviouslyExecutedTransition || transition.Repeatable)
        {
            if (transition.Suspendable)
            {
                Game.Suspend();
                suspended = true;
            }

            transition.Execute(defaultDescription);

            PreviouslyExecutedTransition = transition;

            if (suspended)
            {
                Game.Resume();
            }
        }
    }
}
