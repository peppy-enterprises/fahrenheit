using System.Reflection;
using System.Runtime.Loader;

using HexaGen.Runtime;

namespace Fahrenheit.Core;

/// <summary>
///     Instructs the <see cref="FhLoader"/> that this module type should be instantiated,
///     providing the <paramref name="supported_game_type"/> matches the currently executing game.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class FhLoadAttribute(FhGameType supported_game_type) : Attribute {
    public readonly FhGameType supported_game_type = supported_game_type;
}

/// <summary>
///     Resolves the .NET or native dependencies of a given Fahrenheit DLL by adding its directory to the library search path.
/// </summary>
internal class FhLoadContext(string fh_dll_path) : AssemblyLoadContext {
    private readonly AssemblyDependencyResolver _resolver = new AssemblyDependencyResolver(fh_dll_path);

    protected override Assembly? Load(AssemblyName assembly_name) {
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
public class FhLoader {
    public FhLoader() {
        // Loading `fhcore` into ALC.Default ensures it does not 'leak' into plugins' load contexts, causing type identity mismatches.
        AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Join(FhInternal.PathFinder.Binaries.Path, "fhcore.dll"));
        LibraryLoader.CustomLoadFolders.Add(FhInternal.PathFinder.Binaries.Path); // required for Hexa.NET.ImGui's assembly-probing logic
    }

    public void load_mod(string mod_name, FhManifest manifest, out List<FhModuleContext> modules) {
        modules = [];

        foreach (string fh_dll_name in manifest.DllList) {
            FhLoaderPathInfo fh_dll_paths    = FhInternal.PathFinder.create_loader_paths(mod_name, fh_dll_name);
            FhLoadContext    fh_load_context = new FhLoadContext(fh_dll_paths.DllPath);
            Assembly         fh_dll          = fh_load_context.LoadFromAssemblyPath(fh_dll_paths.DllPath);

            foreach (Type type in fh_dll.GetExportedTypes()) {
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

                FhModule         module       = Activator.CreateInstance(type) as FhModule ?? throw new Exception("FH_E_MODULE_TYPE_ACTIVATION_FAILED");
                FhModulePathInfo module_paths = FhInternal.PathFinder.create_module_paths(mod_name, module.ModuleType);

                modules.Add(new FhModuleContext(module, module_paths));
            }
        }

        FhInternal.Log.Info($"Loaded mod {mod_name} with {modules.Count} modules.");
    }
}
