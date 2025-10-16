// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core;

/// <summary>
///     Contains Fahrenheit boot logic. When the bootstrapper completes, game execution commences.
/// </summary>
public static class FhBootstrapper {
    /* [fkelava 25/4/24 18:47]
     * This class and method are referenced by Stage1. Updating or renaming either requires a Stage1 update.
     */
    [UnmanagedCallersOnly]
    public static void bootstrap() {
        FhModContext[] mods = FhInternal.Loader.load_mods();

        FhApi.Mods = new(mods);
        FhApi.Localization.initialize(mods);
        FhApi.Mods.initialize_mods();
    }
}

/// <summary>
///     Instructs the <see cref="FhLoader"/> that this module type should be instantiated,
///     providing the <paramref name="supported_game_type"/> matches the currently executing game.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class FhLoadAttribute(FhGameType supported_game_type) : Attribute {
    public readonly FhGameType supported_game_type = supported_game_type;
}

/// <summary>
///     Resolves the .NET or native dependencies of a given Fahrenheit mod by adding its directory to the library search path.
/// </summary>
internal sealed class FhLoadContext(string context_name, string fh_dll_path) : AssemblyLoadContext(context_name) {
    private readonly AssemblyDependencyResolver _resolver = new AssemblyDependencyResolver(fh_dll_path);

    protected override Assembly? Load(AssemblyName assembly_name) {
        Assembly? shared_assembly = FhInternal.Loader.get_shared_assembly(assembly_name);
        if (shared_assembly != null) return shared_assembly;

        string? assembly_path = _resolver.ResolveAssemblyToPath(assembly_name);
        return assembly_path != null ? LoadFromAssemblyPath(assembly_path) : null;
    }

    protected override nint LoadUnmanagedDll(string dll_name) {
        string? dll_path = _resolver.ResolveUnmanagedDllToPath(dll_name);
        return dll_path != null ? LoadUnmanagedDllFromPath(dll_path) : nint.Zero;
    }
}

/// <summary>
///     Contains the information the <see cref="FhLoader"/> needs to process a mod.
/// </summary>
internal sealed record FhModLoadInfo(
    string   ModName,
    string[] ModDllList
    );

/// <summary>
///     Loads Fahrenheit DLLs and their .NET or native dependencies into the game process,
///     and instantiates any <see cref="FhModule"/> with a valid <see cref="FhLoadAttribute"/> on them.
/// </summary>
public sealed class FhLoader {
    private readonly Dictionary<string, FhLoadContext> _load_contexts = [];

    public FhLoader() {
        // Loading the core libraries into ALC.Default ensures they do not 'leak' into plugins' load contexts, causing type identity mismatches.
        AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Join(FhInternal.PathFinder.Binaries.Path, "fhcore.dll"));
        AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Join(FhInternal.PathFinder.Binaries.Path, "fhx.dll"));
        AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Join(FhInternal.PathFinder.Binaries.Path, "fhx2.dll"));
        // required for Hexa.NET.ImGui's assembly-probing logic
        HexaGen.Runtime.LibraryLoader.CustomLoadFolders.Add(FhInternal.PathFinder.Binaries.Path);
    }

    /// <summary>
    ///     Reads the load order for this session from disk.
    /// </summary>
    private string[] _get_load_order() {
        string   load_order_path = Path.Join(FhInternal.PathFinder.Mods.Path, "loadorder");
        string[] load_order      = [ "fhruntime", .. File.ReadAllLines(load_order_path) ];

        return load_order;
    }

    /// <summary>
    ///     Reads the <see cref="FhManifest"/> for a mod from disk, throwing if that fails.
    /// </summary>
    private FhManifest _get_manifest(FhModPaths mod_paths) {
        return JsonSerializer.Deserialize<FhManifest>(File.OpenRead(mod_paths.ManifestPath), FhUtil.InternalJsonOpts)
            ?? throw new Exception($"Failed to load manifest at {mod_paths.ManifestPath}");
    }

    /// <summary>
    ///     Attempts to map a <see cref="AssemblyName"/> to an already loaded Fahrenheit DLL <see cref="Assembly"/>.
    ///     <para/>
    ///     This is because Fahrenheit mod DLLs are not permitted to bundle other mod DLLs they depend on;
    ///     whichever version of the dependency the user actually has installed will be loaded instead.
    /// </summary>
    internal Assembly? get_shared_assembly(AssemblyName assembly_name) {
        if (!_load_contexts.TryGetValue(assembly_name.Name ?? "", out FhLoadContext? load_context)) return null;

        foreach (Assembly assembly in load_context.Assemblies) {
            AssemblyName loaded_assembly_name = assembly.GetName();
            if (loaded_assembly_name.FullName == assembly_name.FullName) return assembly;
        }

        return null;
    }

    /// <summary>
    ///     Performs DLL loading for a mod, returning the <see cref="FhModuleContext"/>s of the instantiated mods.
    /// </summary>
    private IEnumerable<FhModuleContext> _load_mod(FhModLoadInfo mod_info) {
        foreach (string dll_name in mod_info.ModDllList) {
            FhDllPaths    dll_paths    = FhInternal.PathFinder.get_paths_dll(mod_info.ModName, dll_name);
            FhLoadContext dll_load_ctx = new FhLoadContext(dll_name, dll_paths.DllPath);
            Assembly      dll          = dll_load_ctx.LoadFromAssemblyPath(dll_paths.DllPath);

            _load_contexts[dll_name] = dll_load_ctx;

            foreach (Type type in dll.GetExportedTypes()) {
                if (type.BaseType != typeof(FhModule)) continue;

                FhLoadAttribute? loader_args = type.GetCustomAttribute<FhLoadAttribute>();

                if (loader_args == null) {
                    FhInternal.Log.Warning($"Loader ignored module type {type.FullName} without [{nameof(FhLoadAttribute)}] applied. This may be an oversight.");
                    continue;
                }

                if (!loader_args.supported_game_type.HasFlag(FhGlobal.game_type)) {
                    FhInternal.Log.Warning($"Loader ignored module type {type.FullName} that does not declare support for this game.");
                    continue;
                }

                FhModule      module       = Activator.CreateInstance(type) as FhModule ?? throw new Exception($"Constructor of module {type.FullName} threw or faulted.");
                FhModulePaths module_paths = FhInternal.PathFinder.get_paths_module(mod_info.ModName, module.ModuleType);

                yield return new FhModuleContext(module, module_paths);
            }
        }
    }

    /// <summary>
    ///     Processes the load order for this session, performs DLL loading,
    ///     and returns the <see cref="FhModContext"/>s of the instantiated mods.
    /// </summary>
    internal FhModContext[] load_mods() {
        string[]       load_order = _get_load_order();
        FhModContext[] mods       = new FhModContext[load_order.Length];

        for (int i = 0; i < load_order.Length; i++) {
            string        mod_name      = load_order[i];
            FhModPaths    mod_paths     = FhInternal.PathFinder.get_paths_mod(mod_name);
            FhManifest    mod_manifest  = _get_manifest(mod_paths);
            FhModLoadInfo mod_load_info = new(mod_name, mod_manifest.DllList);

            mods[i] = new FhModContext(mod_manifest, mod_paths, [ .. _load_mod(mod_load_info) ]);
        }

        return mods;
    }
}
