using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Binding;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using FFXCutsceneRemover.Logging;

namespace FFXCutsceneRemover;

internal sealed class CsrConfigBinder : BinderBase<CsrConfig>
{
    private readonly Option<bool?> _optCsrOn;
    private readonly Option<bool?> _optRngOn;
    private readonly Option<int?> _optMtSleepInterval;

    public CsrConfigBinder(Option<bool?> optCsrOn,
                           Option<bool?> optRngOn,
                           Option<int?> optMtSleepInterval)
    {
        _optCsrOn = optCsrOn;
        _optRngOn = optRngOn;
        _optMtSleepInterval = optMtSleepInterval;
    }

    private static bool ResolveMandatoryBoolArg(Option<bool?> opt)
    {
        Console.WriteLine(opt.Description);
        return Console.ReadLine().ToUpper()[0] == 'Y';
    }

    protected override CsrConfig GetBoundValue(BindingContext bindingContext)
    {
        return new CsrConfig
        {
            CsrOn = bindingContext.ParseResult.GetValueForOption(_optCsrOn) ?? ResolveMandatoryBoolArg(_optCsrOn),
            RngOn = bindingContext.ParseResult.GetValueForOption(_optRngOn) ?? ResolveMandatoryBoolArg(_optRngOn),
            MtSleepInterval = bindingContext.ParseResult.GetValueForOption(_optMtSleepInterval) ?? 16,
        };
    }
}

internal sealed record CsrConfig
{
    public bool CsrOn { get; init; }
    public bool RngOn { get; init; }
    public int  MtSleepInterval { get; init; }
};

class Program
{
    private static CsrConfig csrConfig;
    private static CutsceneRemover cutsceneRemover = null;
    private static RNGMod rngMod = null;

    private static Process Game = null;

    private static bool newGameMenuUpdated = false;

    // Cutscene Remover Version Number, 0x30 - 0x39 = 0 - 9, 0x48 = decimal point
    private const int majorID = 1;
    private const int minorID = 4;
    private const int patchID = 2;
    private static List<(string, byte)> startGameText;

    static void Main(string[] args)
    {
        DiagnosticLog.Information($"Cutscene Remover for Final Fantasy X, version {majorID}.{minorID}.{patchID}");
        if (args.Length > 0) DiagnosticLog.Information($"!!! LAUNCHED WITH COMMAND-LINE OPTIONS: {string.Join(' ', args)} !!!");

        Option<bool?> optCsrOn           = new Option<bool?>("--csr", "Enable CSR? [Y/N]");
        Option<bool?> optRngOn           = new Option<bool?>("--rngfix", "Enable RNGfix? [Y/N]");
        Option<int?>  optMtSleepInterval = new Option<int?>("--mt_sleep_interval", "Specify the main thread sleep interval. [ms]");

        RootCommand rootCmd = new RootCommand("Launches the FFX Cutscene Remover.")
        {
            optCsrOn,
            optRngOn,
            optMtSleepInterval
        };

        rootCmd.SetHandler(MainLoop, new CsrConfigBinder(optCsrOn, optRngOn, optMtSleepInterval));

        rootCmd.Invoke(args);
        return;
    }

    private static void MainLoop(CsrConfig config)
    {
        csrConfig = config;

        while (true)
        {
            Game = ConnectToTarget("FFX");

            if (Game == null)
            {
                continue;
            }

            startGameText = new List<(string, byte)> { };

            if (csrConfig.CsrOn)
            {
                cutsceneRemover = new CutsceneRemover(csrConfig.MtSleepInterval);
                cutsceneRemover.Game = Game;
                startGameText.Add(($"[Cutscene Remover v{majorID}.{minorID}.{patchID}]", 0x41));
            }

            if (csrConfig.RngOn)
            {
                rngMod = new RNGMod();
                rngMod.Game = Game;
                startGameText.Add(($"[RNG Fix Mod Enabled]", 0x45));
            }

            startGameText.Add(($"Start Game?", 0x49));

            MemoryWatchers.Initialize(Game);

            DiagnosticLog.Information("Starting main loop!");

            while (!Game.HasExited)
            {
                MemoryWatchers.Watchers.UpdateAll(Game);

                if (!newGameMenuUpdated && MemoryWatchers.RoomNumber.Current == 0 && MemoryWatchers.Storyline.Current == 0 && MemoryWatchers.Dialogue1.Current == 6)
                {
                    new NewGameTransition { ForceLoad = false, ConsoleOutput = false, startGameText = startGameText }.Execute();
                    newGameMenuUpdated = true;
                }
                if (newGameMenuUpdated && MemoryWatchers.RoomNumber.Current == 23)
                {
                    newGameMenuUpdated = false;
                }

                if (csrConfig.CsrOn)
                {
                    cutsceneRemover.MainLoop();
                }

                if (csrConfig.RngOn)
                {
                    rngMod.MainLoop();
                }

                // Sleep for a bit so we don't destroy CPUs
                Thread.Sleep(csrConfig.MtSleepInterval);
            }
        }
    }

    private static Process ConnectToTarget(string TargetName)
    {
        Process Game = null;

        try
        {
            Game = Process.GetProcessesByName(TargetName).OrderByDescending(x => x.StartTime)
                     .FirstOrDefault(x => !x.HasExited);
        }
        catch (Win32Exception e)
        {
            DiagnosticLog.Information("Exception: " + e.Message);
        }

        if (Game == null || Game.HasExited)
        {
            Game = null;
            Console.Write("\rWaiting to connect to the game. Please launch the game if you haven't yet.");

            Thread.Sleep(500);
        }
        else
        {
            Console.Write("\n");
            DiagnosticLog.Information("Connected to FFX!");
        }

        return Game;
    }
}
