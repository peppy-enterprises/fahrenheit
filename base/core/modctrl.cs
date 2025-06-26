namespace Fahrenheit.Core;

/// <summary>
///     Stores all loaded mods' <see cref="FhModContext"/> for runtime lookup, and exposes facilities to find <see cref="FhModule"/>s at runtime.
/// </summary>
public class FhModController {
    private readonly object             _lock;
    private readonly List<FhModContext> _contexts;

    public FhModController() {
        _contexts = [];
        _lock     = new object();
    }

    public IEnumerable<FhModContext> get_all() {
        lock (_lock) {
            foreach (FhModContext ctx in _contexts) yield return ctx;
        }
    }

    public FhModuleContext? find_module<TModule>() where TModule : FhModule {
        lock (_lock) {
            foreach (FhModContext mod_ctx in _contexts) {
                foreach (FhModuleContext module_ctx in mod_ctx.Modules) {
                    if (module_ctx.Module is TModule) return module_ctx;
                }
            }
        }
        return null;
    }

    public IEnumerable<FhModuleContext> find_all_modules<TModule>() where TModule : FhModule {
        lock (_lock) {
            foreach (FhModContext mod_ctx in _contexts) {
                foreach (FhModuleContext module_ctx in mod_ctx.Modules) {
                    if (module_ctx.Module is TModule) yield return module_ctx;
                }
            }
        }
    }

    public FhModuleContext? find_module<TModule>(Predicate<TModule> match) where TModule : FhModule {
        lock (_lock) {
            foreach (FhModContext mod_ctx in _contexts) {
                foreach (FhModuleContext module_ctx in mod_ctx.Modules) {
                    if (module_ctx.Module is TModule tfm && match(tfm)) return module_ctx;
                }
            }
        }
        return null;
    }

    public IEnumerable<FhModuleContext> find_all_modules<TModule>(Predicate<TModule> match) where TModule : FhModule {
        lock (_lock) {
            foreach (FhModContext mod_ctx in _contexts) {
                foreach (FhModuleContext module_ctx in mod_ctx.Modules) {
                    if (module_ctx.Module is TModule tfm && match(tfm)) yield return module_ctx;
                }
            }
        }
    }

    internal void load_mods() {
        string   load_order_path = Path.Join(FhInternal.PathFinder.Mods.Path, "loadorder");
        string[] load_order      = [ "fhruntime", .. File.ReadAllLines(load_order_path) ];

        foreach (string mod_name in load_order) {
            FhModPathInfo    paths    = FhInternal.PathFinder.create_mod_paths(mod_name);
            FhManifest       manifest = JsonSerializer.Deserialize<FhManifest>(File.OpenRead(paths.ManifestPath), FhUtil.InternalJsonOpts)
                ?? throw new Exception("FH_E_MANIFEST_LOAD_FAILED");

            FhInternal.Loader.load_mod(mod_name, manifest, out List<FhModuleContext> modules);
            _contexts.Add(new FhModContext(manifest, paths, modules));
        }
    }

    internal void run_initializers() {
        lock (_lock) {
            foreach (FhModContext mod_ctx in _contexts) {
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
}
