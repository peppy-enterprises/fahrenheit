namespace Fahrenheit.Core;

public sealed record FhModuleContext(
    FhModule       Module,
    FhModuleConfig ModuleConfig);

public class FhModuleController {
    private readonly object                _lock;
    private readonly List<FhModuleContext> _contexts;

    public FhModuleController() {
        _contexts = [];
        _lock     = new object();
    }

    public IEnumerable<FhModuleContext> find_all() {
        lock (_lock) {
            foreach (FhModuleContext ctx in _contexts) yield return ctx;
        }
    }

    public FhModuleContext? find<TModule>() where TModule : FhModule {
        lock (_lock) {
            foreach (FhModuleContext ctx in _contexts) {
                if (ctx.Module is TModule) return ctx;
            }
        }
        return null;
    }

    public IEnumerable<FhModuleContext> find_all<TModule>() where TModule : FhModule {
        lock (_lock) {
            foreach (FhModuleContext ctx in _contexts) {
                if (ctx.Module is TModule) yield return ctx;
            }
        }
    }

    public FhModuleContext? find<TModule>(Predicate<TModule> match) where TModule : FhModule {
        lock (_lock) {
            foreach (FhModuleContext ctx in _contexts) {
                if (ctx.Module is TModule tfm && match(tfm)) return ctx;
            }
        }
        return null;
    }

    public IEnumerable<FhModuleContext> find_all<TModule>(Predicate<TModule> match) where TModule : FhModule {
        lock (_lock) {
            foreach (FhModuleContext ctx in _contexts) {
                if (ctx.Module is TModule tfm && match(tfm)) yield return ctx;
            }
        }
    }

    public void spawn_modules(in List<FhModuleConfig> configs) {
        lock (_lock) {
            foreach (FhModuleConfig config in configs) {
                _contexts.Add(new FhModuleContext(config.SpawnModule(), config));
                FhLog.Log(LogLevel.Warning, $"Initialized module {config.Name} [{config.Type}].");
            }
        }
    }

    public void initialize_modules() {
        lock (_lock) {
            foreach (FhModuleContext context in _contexts) {
                FhModuleConfig fmcfg = context.ModuleConfig;
                FhModule       fm    = context.Module;

                if (!fm.init()) {
                    FhLog.Log(LogLevel.Warning, $"Module {fmcfg.Name} [{fmcfg.Type}] initializer callback failed. Suppressing.");
                    continue;
                }
            }
        }
    }
}
