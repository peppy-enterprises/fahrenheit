using System;
using System.Collections.Generic;
using System.IO;

namespace Fahrenheit.CoreLib;

internal sealed class FhModuleContext {
    public FhModuleContext(FhModule fm, FhModuleConfig fmcfg) {
        Module           = fm;
        ModuleConfig     = fmcfg;
        DependentModules = new List<FhModule>(3);
    }

    public FhModule       Module           { get; }
    public FhModuleConfig ModuleConfig     { get; }
    public List<FhModule> DependentModules { get; }
}

public static class FhModuleController {
    private static readonly object                _moduleManipLock;
    private static readonly List<FhModuleContext> _moduleContexts;

    static FhModuleController() {
        _moduleContexts  = new List<FhModuleContext>(8);
        _moduleManipLock = new object();
    }

    public static bool Initialize(IEnumerable<FhModuleConfigCollection> moduleConfigCollections) {
        bool rv = true;

        foreach (FhModuleConfigCollection moduleConfigCollection in moduleConfigCollections) {
            if (!InitializeModules(moduleConfigCollection.ModuleConfigs)) rv = false;
        }

        return rv;
    }

    private static FhModuleContext? GetContextForModule(in FhModule module) {
        foreach (FhModuleContext fmctx in _moduleContexts)
            if (fmctx.Module == module) return fmctx;

        return default;
    }

    private static bool StopIncludingDependents(in FhModuleContext fmctx) {
        bool retval = true;

        foreach (FhModule dependent in fmctx.DependentModules) {
            FhLog.Log(LogLevel.Info, $"Halting threads of module {dependent.ModuleName} because module {fmctx.Module.ModuleName} it depends on faulted.");
            if (!dependent.FhModuleStop()) retval = false;
        }

        if (!fmctx.Module.FhModuleStop()) retval = false;

        return retval;
    }

    private static bool StartIncludingDependents(in FhModuleContext fmctx) {
        bool retval = true;

        foreach (FhModule dependent in fmctx.DependentModules) {
            if (!dependent.FhModuleStart()) retval = false;
        }

        if (!fmctx.Module.FhModuleStart()) retval = false;

        return retval;
    }

    public static void ModuleStateChangeHandler(FhModule sender, FhModuleStateChangeEventArgs e) {
        lock (_moduleManipLock) {
            FhLog.Log(LogLevel.Info, $"Module {sender.ModuleName} changes state from {e.OldState} to {e.NewState}.");

            FhModuleContext fmctx = GetContextForModule(sender) ?? throw new Exception("FH_E_NO_FMCTX_FOR_MODULE");

            if (!(e.NewState switch {
                FhModuleState.Fault   => StopIncludingDependents(fmctx),
                FhModuleState.Started => StartIncludingDependents(fmctx),
                _ => false
            })) {
                FhLog.Log(LogLevel.Error, $"Internal error in {ModuleStateChangeHandler} invoked by module {sender.ModuleName}.");
            }
        }
    }

    public static bool SaveFileToRunDir(string filePath) {
        try {
            File.Copy(filePath, Path.Join(FhRuntimeConst.ByRunDir.Path, Path.GetFileName(filePath)));
        }
        catch {
            return false;
        }

        return true;
    }

    public static IEnumerable<FhModule> FindAll() {
        lock (_moduleManipLock) {
            foreach (FhModuleContext fmctx in _moduleContexts) yield return fmctx.Module;
        }
    }

    public static TModule? Find<TModule>() where TModule : FhModule {
        lock (_moduleManipLock) {
            foreach (FhModuleContext fmctx in _moduleContexts) {
                if (fmctx.Module is TModule tfm) return tfm;
            }
        }
        return null;
    }

    public static IEnumerable<TModule> FindAll<TModule>() where TModule : FhModule {
        lock (_moduleManipLock) {
            foreach (FhModuleContext fmctx in _moduleContexts) {
                if (fmctx.Module is TModule tfm) yield return tfm;
            }
        }
    }

    public static TModule? Find<TModule>(Predicate<TModule> match) where TModule : FhModule {
        lock (_moduleManipLock) {
            foreach (FhModuleContext fmctx in _moduleContexts) {
                if (fmctx.Module is TModule tfm && match(tfm)) return tfm;
            }
        }
        return null;
    }

    public static IEnumerable<TModule> FindAll<TModule>(Predicate<TModule> match) where TModule : FhModule {
        lock (_moduleManipLock) {
            foreach (FhModuleContext fmctx in _moduleContexts) {
                if (fmctx.Module is TModule tfm && match(tfm)) yield return tfm;
            }
        }
    }

    public static IEnumerable<bool> StartAll() {
        lock (_moduleManipLock) {
            foreach (FhModuleContext fmctx in _moduleContexts) yield return Start(fmctx.Module);
        }
    }

    public static bool Start(FhModule fm) {
        lock (_moduleManipLock) {
            FhLog.Log(LogLevel.Info, $"Starting module {fm.ModuleName}.");
            return StartIncludingDependents(GetContextForModule(fm) ?? throw new Exception("FH_E_NO_FMCTX_FOR_MODULE"));
        }
    }

    public static IEnumerable<bool> Start(IEnumerable<FhModule> fms) {
        lock (_moduleManipLock) {
            foreach (FhModule fm in fms)
                yield return Start(fm);
        }
    }

    public static IEnumerable<bool> StopAll() {
        lock (_moduleManipLock) {
            foreach (FhModuleContext fmctx in _moduleContexts)
                yield return Stop(fmctx.Module);
        }
    }

    public static bool Stop(FhModule fm) {
        lock (_moduleManipLock) {
            FhLog.Log(LogLevel.Info, $"Stopping module {fm.ModuleName}.");
            return StopIncludingDependents(GetContextForModule(fm) ?? throw new Exception("FH_E_NO_FMCTX_FOR_MODULE"));
        }
    }

    public static IEnumerable<bool> Stop(IEnumerable<FhModule> fms) {
        lock (_moduleManipLock) {
            foreach (FhModule fm in fms)
                yield return Stop(fm);
        }
    }

    /// <summary>
    ///     Specifies that <paramref name="caller"/> is dependent on <paramref name="target"/>, and so should
    ///     be shut down if <paramref name="target"/> faults. To allow for this, <paramref name="caller"/>'s
    ///     threads are temporarily paused while the <see cref="IFhModuleController"/> locates <paramref name="target"/>.
    /// </summary>
    /// <returns>
    ///     Whether the dependency was successfully registered.
    /// </returns>
    public static bool RegisterModuleDependency(FhModule caller, FhModule target) {
        lock (_moduleManipLock) {
            FhModuleContext  callerfmctx = GetContextForModule(caller) ?? throw new Exception("FH_E_NO_FMCTX_FOR_MODULE");
            FhModuleContext? targetfmctx = GetContextForModule(target);

            if (targetfmctx == default) {
                FhLog.Log(LogLevel.Error, $"Target FhModule not found in RegisterModuleDependency, caller {caller}.");
                return false;
            }

            StopIncludingDependents(callerfmctx);
            targetfmctx.DependentModules.Add(caller);
            return StartIncludingDependents(callerfmctx);
        }
    }

    /// <summary>
    ///     Specifies that <paramref name="caller"/> is no longer dependent on <paramref name="target"/>, and so should
    ///     continue operating if <paramref name="target"/> faults. To allow for this, <paramref name="caller"/>'s
    ///     threads are temporarily paused while the <see cref="IFhModuleController"/> locates <paramref name="target"/>.
    /// </summary>
    /// <returns>
    ///     Whether the dependency was successfully unregistered.
    /// </returns>
    public static bool UnregisterModuleDependency(FhModule caller, FhModule target) {
        lock (_moduleManipLock) {
            FhModuleContext  callerfmctx = GetContextForModule(caller) ?? throw new Exception("FH_E_NO_FMCTX_FOR_MODULE");
            FhModuleContext? targetfmctx = GetContextForModule(target);

            if (targetfmctx == default) {
                FhLog.Log(LogLevel.Error, $"Caller FhModule not found or not in dependency list of target, caller {caller}.");
                return false;
            }

            StopIncludingDependents(callerfmctx);
            return targetfmctx.DependentModules.Remove(caller) && StartIncludingDependents(callerfmctx);
        }
    }

    /// <summary>
    ///     Instantiates <see cref="FhModule"/>s from their <see cref="FhModuleConfig"/>s.
    ///     Required before the first call to any variant of <see cref="Start(FhModule)"/>.
    /// </summary>
    private static bool InitializeModules(in List<FhModuleConfig> moduleConfigs) {
        bool retval = true;

        lock (_moduleManipLock) {
            foreach (FhModuleConfig fmcfg in moduleConfigs) {
                if (!fmcfg.ConfigEnabled) {
                    FhLog.Log(LogLevel.Warning, $"Module {fmcfg.ConfigName} [{fmcfg.GetType().Name}] is disabled in configuration. Suppressing.");
                    continue;
                }

                if (!fmcfg.TrySpawnModule(out FhModule? fm)) {
                    FhLog.Log(LogLevel.Error, $"Module {fmcfg.ConfigName} [{fmcfg.GetType().Name}] constructor failed. Suppressing.");

                    retval = false;
                    continue;
                }

                if (!fm.FhModuleInit()) {
                    FhLog.Log(LogLevel.Warning, $"Module {fmcfg.ConfigName} [{fmcfg.GetType().Name}] initializer callback failed. Suppressing.");

                    retval = false;
                    continue;
                }

                FhLog.Log(LogLevel.Warning, $"Initialized module {fmcfg.ConfigName} [{fmcfg.GetType().Name}].");
                _moduleContexts.Add(new FhModuleContext(fm, fmcfg));
            }
        }

        return retval;
    }
}
