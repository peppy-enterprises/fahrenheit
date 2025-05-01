using System.Reflection;
using System.Runtime.Loader;

namespace Fahrenheit.Core;

[AttributeUsage(AttributeTargets.Class)]
public class FhLoaderMarkAttribute : Attribute;

public class FhLoadContext(string fh_dll_path) : AssemblyLoadContext {
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

public class FhLoader {
    public FhLoader() {
        // Loading `fhcore` into ALC.Default ensures it does not 'leak' into plugins' load contexts, causing type identity mismatches.
        AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Join(FhInternal.PathFinder.Binaries.Path, "fhcore.dll"));
    }

    public void load_mod(string mod_name, FhManifest manifest, out List<FhModuleContext> modules) {
        modules = [];

        foreach (string fh_dll_name in manifest.DllList) {
            FhModulePathInfo fh_dll_paths    = FhInternal.PathFinder.create_module_paths(mod_name, fh_dll_name);
            FhLoadContext    fh_load_context = new FhLoadContext(fh_dll_paths.DllPath);
            Assembly         fh_dll          = fh_load_context.LoadFromAssemblyPath(fh_dll_paths.DllPath);

            foreach (Type type in fh_dll.GetExportedTypes()) {
                if (type.BaseType != typeof(FhModule)) continue;

                if (type.GetCustomAttribute<FhLoaderMarkAttribute>() == null) {
                    FhLog.Warning($"Loader ignored module type {type.FullName} without [FhLoaderMark] applied. This may be an oversight.");
                    continue;
                }

                FhModule module = Activator.CreateInstance(type) as FhModule ?? throw new Exception("FH_E_MODULE_TYPE_ACTIVATION_FAILED");
                modules.Add(new FhModuleContext(module, fh_dll_paths));
            }
        }

        FhLog.Info($"Loaded mod {mod_name} with {modules.Count} modules.");
    }
}
