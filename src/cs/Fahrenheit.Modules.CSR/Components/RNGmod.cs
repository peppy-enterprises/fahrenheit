using System;
using System.ComponentModel;
using System.Diagnostics;

using FFXCutsceneRemover.ComponentUtil;
using FFXCutsceneRemover.Logging;

/*
 * Main loops for the Rngfix Mod.
 */
namespace FFXCutsceneRemover;

class RNGMod
{
    public Process Game;

    private bool rngPatched = false;
    private bool patchUndone = false;
    private byte[] patchBytes = new byte[] { 0x31, 0xD2, 0x90 };
    private byte[] currentBytes;

    public void MainLoop()
    {
        if (!rngPatched)
        {
            if (MemoryWatchers.RoomNumber.Current == 23)
            {
                new Transition { ForceLoad = false, RNGArrayOpBytes = new byte[] { 0x31, 0xD2, 0x90 } , Description = "RNG patch applied!" }.Execute();
                rngPatched = true;
            }
        }
        else
        {
            try
            {
                currentBytes = Game.ReadBytes(MemoryWatchers.RNGArrayOpBytes.Address, 3);

                for (int i = 0; i < 3; i++)
                {
                    if (currentBytes[i] != patchBytes[i])
                    {
                        patchUndone = true;
                    }
                }
            }
            catch (Win32Exception e)
            {
                DiagnosticLog.Information("Exception: " + e.Message);
            }
            catch (NullReferenceException e)
            {
                DiagnosticLog.Information("Exception: " + e.Message);
            }

            if (patchUndone)
            {
                Console.Clear();

                Console.WriteLine("What's going on here? [The RNG function patch was overwritten]");
                Game.Kill();

                Console.ReadKey();
            }
        }
        return;
    }
}
