// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit;

/// <summary>
///     Contains Fahrenheit boot logic and basic internal runtime constants.
/// </summary>
internal static class FhEnvironment {

    internal static readonly FhFinder     Finder;
    internal static readonly nint         BaseAddr;
    internal static readonly string[]     LoadOrder;
    internal static readonly FhModPaths[] ModPaths;
    internal static readonly FhManifest[] Manifests;

    static FhEnvironment() {
        /* [fkelava 10/06/26 18:53]
         * https://learn.microsoft.com/en-us/dotnet/api/system.appdomain?view=net-10.0#remarks
         * > On .NET Core and .NET 5+ {...} These versions have exactly one AppDomain.
         *
         * In other words, AppDomain.CurrentDomain is the _only_ domain in existence.
         * See the comments on FhExceptionHandler for the peculiar nature of events here.
         */
        ExceptionHandling.SetUnhandledExceptionHandler(FhExceptionHandler.eh_unhandled);
        // ExceptionHandling.SetFatalErrorHandler(FhExceptionHandler.eh_fatal); // Uncomment when added in .NET 11/12.

        AppDomain.CurrentDomain.FirstChanceException += FhExceptionHandler.eh_first_chance;

        Finder    = new();
        BaseAddr  = NativeLibrary.GetMainProgramHandle();
        LoadOrder = _init_load_order();
        ModPaths  = _init_mod_paths();
        Manifests = _init_manifests();
    }

    /* [fkelava 25/4/24 18:47]
     * This class and the `boot` method are referenced by Stage1.
     * Updating or renaming either requires a Stage1 update.
     */

    [UnmanagedCallersOnly]
    public static void boot() {
        FhApi.Mods.load_mods();
        FhApi.Localization.initialize();
        FhApi.Mods.initialize_mods();

        // post-init - may require later editing
        FhInternal.MethodTable.commit();
    }

    /// <summary>
    ///     Reads the load order for this session from disk.
    /// </summary>
    private static string[] _init_load_order() {
        string load_order_path = Path.Join(Finder.Mods.FullName, "loadorder");
        return [ "fhr", .. File.ReadAllLines(load_order_path) ];
    }

    /// <summary>
    ///     Creates path information for the current load order.
    /// </summary>
    private static FhModPaths[] _init_mod_paths() {
        FhModPaths[] result = new FhModPaths[LoadOrder.Length];

        for (int i = 0; i < LoadOrder.Length; i++) {
            result[i] = Finder.get_for_mod(LoadOrder[i]);
        }

        return result;
    }

    /// <summary>
    ///     Reads the <see cref="FhManifest"/>s for the current load order from disk, throwing if that fails.
    /// </summary>
    private static FhManifest[] _init_manifests() {
        FhManifest[] result = new FhManifest[ModPaths.Length];

        for (int i = 0; i < ModPaths.Length; i++) {
            using FileStream manifest_file = File.OpenRead(ModPaths[i].ManifestPath);

            try {
                result[i] = JsonSerializer.Deserialize<FhManifest>(manifest_file, FhUtil.InternalJsonOpts)
                    ?? throw new Exception($"Manifest at {ModPaths[i].ManifestPath} was empty or uninterpretable.");
            }
            catch {
                FhInternal.Log.Error($"While attempting to read manifest at {ModPaths[i].ManifestPath}:");
                throw;
            }
        }

        return result;
    }
}

/// <summary>
///     Instructs the <see cref="FhLoader"/> that this module type should be instantiated,
///     providing the currently executing game is one of <paramref name="supported_games"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class FhLoadAttribute(FhGameId supported_games) : Attribute {
    public readonly FhGameId supported_games = supported_games;
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
///     Loads Fahrenheit DLLs and their .NET or native dependencies into the game process,
///     and instantiates any <see cref="FhModule"/> with a valid <see cref="FhLoadAttribute"/> on them.
/// </summary>
internal sealed class FhLoader {

    private readonly Dictionary<string, FhLoadContext> _load_contexts = [];

    internal FhLoader() {
        // Loading the core library into ALC.Default ensures it does not 'leak' into plugins' load contexts, causing type identity mismatches.
        Assembly self = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Join(FhEnvironment.Finder.Binaries.FullName, "fh.dll"));
        FhInternal.Log.Info($"Fahrenheit {FileVersionInfo.GetVersionInfo(self.Location).ProductVersion}");

        // required for Hexa.NET.ImGui's assembly-probing logic
        HexaGen.Runtime.LibraryLoader.CustomLoadFolders.Add(FhEnvironment.Finder.Binaries.FullName);
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
    internal IEnumerable<FhModuleContext> load_mod(FhManifest manifest) {
        string dll_path = FhEnvironment.Finder.get_for_dll(manifest.Id);

        /* [fkelava 02/04/26 00:44]
         * Fahrenheit supports file-only mods. Such mods contain no DLL, ergo there are no modules in their mod context.
         */

        if (!File.Exists(dll_path))
            yield break;

        FhLoadContext load_context = new FhLoadContext(manifest.Id, dll_path);
        Assembly      assembly     = load_context.LoadFromAssemblyPath(dll_path);

        _load_contexts[manifest.Id] = load_context;

        foreach (Type type in assembly.GetExportedTypes()) {
            if (type.BaseType != typeof(FhModule)) continue;

            FhLoadAttribute? loader_args = type.GetCustomAttribute<FhLoadAttribute>();

            if (loader_args == null) {
                FhInternal.Log.Warning($"Loader ignored module type {type.FullName} without [{nameof(FhLoadAttribute)}] applied. This may be an oversight.");
                continue;
            }

            if (!loader_args.supported_games.HasFlag(FhGlobal.game_id)) {
                FhInternal.Log.Warning($"Loader ignored module type {type.FullName} that does not declare support for this game.");
                continue;
            }

            FhModule      module;
            FhModulePaths module_paths;

            try {
                /* [fkelava 02/04/26 01:00]
                 * https://learn.microsoft.com/en-us/dotnet/api/system.activator.createinstance?view=net-10.0#system-activator-createinstance(system-type)
                 *
                 * > Returns {...} a reference to the newly created object, or null for Nullable<T> instances.
                 * > Nullable<T> represents a value type that can be assigned 'null'.
                 *
                 * FhModule is a class, thus value types cannot inherit from it, so the return value must be non-null.
                 */

                module       = (Activator.CreateInstance(type) as FhModule)!;
                module_paths = FhEnvironment.Finder.get_for_module(manifest.Id, module.ModuleType);
            }
            catch {
                FhInternal.Log.Error($"Constructor of module {type.FullName} threw or faulted.");
                throw;
            }

            yield return new FhModuleContext(module, module_paths);
        }

        FhInternal.Log.Info($"--- load context dump for {manifest.Id} ---");
        FhInternal.Log.Info($"  {string.Join("\n  ", load_context.Assemblies)}");
        FhInternal.Log.Info($"---");
    }
}
