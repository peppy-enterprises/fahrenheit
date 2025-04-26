using System.Reflection;
using System.Runtime.Loader;

namespace Fahrenheit.Core;

public class FhLoader {
    // https://learn.microsoft.com/en-us/dotnet/core/dependency-loading/understanding-assemblyloadcontext
    private readonly AssemblyLoadContext _load_context;

    public FhLoader() {
        _load_context = AssemblyLoadContext.GetLoadContext(typeof(FhLoader).Assembly) ?? throw new Exception("FH_E_CANNOT_GET_OWN_ALC");
    }

    private bool _is_loaded(string dll_name) {
        foreach (Assembly assembly in _load_context.Assemblies) {
            if (assembly.GetName().Name?.ToUpperInvariant() == dll_name)
                return true;
        }

        return false;
    }

    private bool _should_load(string dir_path, string dll_name, out string dll_full_path) {
        dll_full_path = Path.Join(dir_path, $"{dll_name}.dll");

        return File.Exists(dll_full_path) && !_is_loaded(dll_name);
    }

    public void load_mod(string mod_name, FhManifest manifest, out List<FhModuleContext> modules) {
        modules = [];

        foreach (string fh_dll in manifest.DllList) {
            FhModulePathInfo fh_dll_paths = FhInternal.PathFinder.create_module_paths(mod_name, fh_dll);

            load(fh_dll_paths.DllPath);

            string           fh_dll_config  = FhInternal.PathFinder.fix_paths(File.ReadAllText(fh_dll_paths.ConfigPath));
            FhModuleConfig[] fh_dll_configs = JsonSerializer.Deserialize<FhModuleConfig[]>(fh_dll_config, FhUtil.JsonOpts) ??
                throw new Exception($"FH_E_CONF_DESERIALIZE_FAULT: {fh_dll}.");

            foreach (FhModuleConfig module_config in fh_dll_configs) {
                modules.Add(new FhModuleContext(module_config.SpawnModule(), module_config, fh_dll_paths));
            }
        }

        FhLog.Info($"Loaded mod {mod_name} with {modules.Count} modules.");
    }

    public void load(string dll_full_path) {
        string dir_name = Path.GetDirectoryName           (dll_full_path) ?? throw new Exception("FH_E_MODULE_DIR_UNIDENTIFIABLE");
        string dll_name = Path.GetFileNameWithoutExtension(dll_full_path).ToUpperInvariant();

        if (_is_loaded(dll_name)) return;

        Assembly this_assembly = _load_context.LoadFromAssemblyPath(dll_full_path);
        FhLog.Log(LogLevel.Info, $"Loaded DLL {dll_name}.");

        // Load dependencies first.
        foreach (AssemblyName dependency in this_assembly.GetReferencedAssemblies()) {
            if (_should_load(dir_name, dependency.Name ?? throw new Exception("FH_E_REF_DLL_NAME_NULL"), out string dependency_full_path)) {
                load(dependency_full_path);
            }
        }
    }

    /* [fkelava 27/2/23 00:12]
     * Resolves a full type name in the "Type" variable of a given FhModuleConfig JSON
     * to an actual Type instance, verifying that it is in fact derived from FhModuleConfig.
     */
    public Type resolve_type(ref Utf8JsonReader reader, Type target_type, bool strict = false) {
        string type_name = reader.deserialize_and_advance<string>("Type");

        foreach (Assembly dll in _load_context.Assemblies) {
            if (dll.GetType(type_name) is Type type
                && (strict
                    ? type == target_type
                    : type.IsAssignableTo(target_type)))
                return type;
        }

        throw new Exception("FH_E_TYPE_RESOLUTION_FAILED");
    }
}
