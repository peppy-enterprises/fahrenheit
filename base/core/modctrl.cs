namespace Fahrenheit.Core;

/// <summary>
///     Stores all loaded mods' <see cref="FhModContext"/> for runtime lookup,
///     and exposes facilities to find <see cref="FhModule"/>s at runtime.
/// </summary>
public class FhModController {
    // != 0 indicates the mod array is populated and immutable for the rest of program execution.
    private int            _init = 0;
    private FhModContext[] _mods = [];

    /// <summary>
    ///     Returns the <see cref="FhModContext"/>s of all loaded mods for this session.
    /// </summary>
    public IEnumerable<FhModContext> get_mods() {
        if (Interlocked.CompareExchange(ref _init, 0, 0) == 0) yield break;

        foreach (FhModContext ctx in _mods) {
            yield return ctx;
        }
    }

    /// <summary>
    ///     Returns the <see cref="FhModuleContext"/>s of all loaded modules for this session.
    /// </summary>
    public IEnumerable<FhModuleContext> get_modules() {
        if (Interlocked.CompareExchange(ref _init, 0, 0) == 0) yield break;

        foreach (FhModContext mod_ctx in _mods) {
            foreach (FhModuleContext module_ctx in mod_ctx.Modules) {
                yield return module_ctx;
            }
        }
    }

    /// <summary>
    ///     Searches all modules for one of type <typeparamref name="TModule"/>,
    ///     then returns its <see cref="FhModuleContext"/> if it was found.
    /// </summary>
    public FhModuleContext? get_module<TModule>() where TModule : FhModule {
        foreach (FhModuleContext module_ctx in get_modules()) {
            if (module_ctx.Module is TModule) return module_ctx;
        }
        return null;
    }

    internal void load_mods() {
        _mods = [ .. FhInternal.Loader.load_mods() ];
        Interlocked.Increment(ref _init); // _mods is now locked for the rest of program execution.
    }

    internal void initialize_mods() {
        foreach (FhModContext mod_ctx in _mods) {
            foreach (FhModuleContext module_ctx in mod_ctx.Modules) {
                FhModule   fm       = module_ctx.Module;
                FileStream fm_state = File.Open(module_ctx.Paths.GlobalStatePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                if (!fm.init(mod_ctx, fm_state)) {
                    FhInternal.Log.Warning($"Module {fm.ModuleType} initializer callback failed. Suppressing.");
                    continue;
                }

                FhInternal.Log.Info($"Initialized module {fm.ModuleType}.");
            }
        }
    }
}
