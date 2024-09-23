using System;
using System.Collections.Generic;

namespace Fahrenheit.CoreLib;

public sealed record FhModuleContext(FhModule Module, FhModuleConfig ModuleConfig);

public static class FhModuleController {
    private static readonly object                _moduleManipLock;
    private static readonly List<FhModuleContext> _moduleContexts;

    static FhModuleController() {
        _moduleContexts  = new List<FhModuleContext>(8);
        _moduleManipLock = new object();
    }

    private static FhModuleContext? get_context_for_module(in FhModule module) {
        foreach (FhModuleContext fmctx in _moduleContexts)
            if (fmctx.Module == module) return fmctx;

        return default;
    }

    public static IEnumerable<FhModuleContext> find_all() {
        lock (_moduleManipLock) {
            foreach (FhModuleContext fmctx in _moduleContexts) yield return fmctx;
        }
    }

    public static FhModuleContext? find<TModule>() where TModule : FhModule {
        lock (_moduleManipLock) {
            foreach (FhModuleContext fmctx in _moduleContexts) {
                if (fmctx.Module is TModule tfm) return fmctx;
            }
        }
        return null;
    }

    public static IEnumerable<FhModuleContext> find_all<TModule>() where TModule : FhModule {
        lock (_moduleManipLock) {
            foreach (FhModuleContext fmctx in _moduleContexts) {
                if (fmctx.Module is TModule tfm) yield return fmctx;
            }
        }
    }

    public static FhModuleContext? find<TModule>(Predicate<TModule> match) where TModule : FhModule {
        lock (_moduleManipLock) {
            foreach (FhModuleContext fmctx in _moduleContexts) {
                if (fmctx.Module is TModule tfm && match(tfm)) return fmctx;
            }
        }
        return null;
    }

    public static IEnumerable<FhModuleContext> find_all<TModule>(Predicate<TModule> match) where TModule : FhModule {
        lock (_moduleManipLock) {
            foreach (FhModuleContext fmctx in _moduleContexts) {
                if (fmctx.Module is TModule tfm && match(tfm)) yield return fmctx;
            }
        }
    }

    public static bool initialize_modules(in List<FhModuleConfig> moduleConfigs) {
        bool retval = true;

        lock (_moduleManipLock) {
            foreach (FhModuleConfig fmcfg in moduleConfigs) {
                if (!fmcfg.ConfigEnabled) {
                    FhLog.Log(LogLevel.Warning, $"Module {fmcfg.ConfigName} [{fmcfg.Type}] is disabled in configuration. Suppressing.");
                    continue;
                }

                if (!fmcfg.TrySpawnModule(out FhModule? fm)) {
                    FhLog.Log(LogLevel.Error, $"Module {fmcfg.ConfigName} [{fmcfg.Type}] constructor failed. Suppressing.");

                    retval = false;
                    continue;
                }

                if (!fm.FhModuleInit()) {
                    FhLog.Log(LogLevel.Warning, $"Module {fmcfg.ConfigName} [{fmcfg.Type}] initializer callback failed. Suppressing.");

                    retval = false;
                    continue;
                }

                FhLog.Log(LogLevel.Warning, $"Initialized module {fmcfg.ConfigName} [{fmcfg.Type}].");
                _moduleContexts.Add(new FhModuleContext(fm, fmcfg));
            }
        }

        return retval;
    }
}
