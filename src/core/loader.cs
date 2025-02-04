using System.Reflection;
using System.Runtime.Loader;

namespace Fahrenheit.Core;

public static class FhLoader {
    // https://learn.microsoft.com/en-us/dotnet/core/dependency-loading/understanding-assemblyloadcontext
    private readonly static AssemblyLoadContext _load_context;

    static FhLoader() {
        _load_context = AssemblyLoadContext.GetLoadContext(typeof(FhLoader).Assembly) ?? throw new Exception("FH_E_CANNOT_GET_OWN_ALC");
    }

    // Stage1 references this delegate and method by name. If you change them or move them to a different type, Stage1 must be updated.
    public delegate void FhInitDelegate();

    // The point at which Fahrenheit execution begins. Stage1 calls into this method directly.
    public static void ldr_bootstrap() {
        string   load_order_path = Path.Join(FhRuntimeConst.Modules.LinkPath, "loadorder");
        string[] load_order      = [ "fhruntime", .. File.ReadAllLines(load_order_path) ];

        foreach (string module in load_order) {
            string module_path = Path.Join(FhRuntimeConst.Modules.LinkPath, module, $"{module}.dll");
            if (!_load(module_path, out List<FhModuleConfig>? module_configs)) continue;
            FhModuleController.spawn_modules(module_configs);
        }

        FhModuleController.initialize_modules();
    }

    private static bool _is_loaded(string refAssemName) {
        foreach (Assembly assembly in _load_context.Assemblies) {
            if (assembly.GetName().Name?.ToUpperInvariant() == refAssemName)
                return true;
        }

        return false;
    }

    private static bool _should_load(string dirPath, string refAssemName, out string refAssemFullPath) {
        refAssemFullPath = Path.Join(dirPath, $"{refAssemName}.dll");

        return File.Exists(refAssemFullPath) && !_is_loaded(refAssemName);
    }

    /// <summary>
    ///     Loads a module while ensuring its references, if detected, are loaded before it.
    /// </summary>
    private static bool _load(string dll_full_path, [NotNullWhen(true)] out List<FhModuleConfig>? module_configs) {
        module_configs = [];

        string module_dir_name    = Path.GetDirectoryName           (dll_full_path) ?? throw new Exception("FH_E_MODULE_DIR_UNIDENTIFIABLE");
        string module_dll_name    = Path.GetFileNameWithoutExtension(dll_full_path).ToUpperInvariant();
        string module_conf_name   = dll_full_path.Replace(".dll", ".conf.json");
        bool   should_load_config = File.Exists(module_conf_name);

        if (_is_loaded(module_dll_name)) return true;

        FhLog.Log(LogLevel.Info, $"Loading module {module_dll_name}.");
        Assembly this_assembly = _load_context.LoadFromAssemblyPath(dll_full_path);

        // --> LOAD ORDERING; LOAD REFERENCED ASSEMBLIES FIRST <--
        foreach (AssemblyName dependency in this_assembly.GetReferencedAssemblies()) {
            if (_should_load(module_dir_name, dependency.Name ?? throw new Exception("FH_E_REF_DLL_NAME_NULL"), out string dependency_full_path)) {
                if (!_load(dependency_full_path, out List<FhModuleConfig>? dependency_module_configs))
                    throw new Exception("FH_E_REF_ASSEM_LOAD_FAULT");

                module_configs.AddRange(dependency_module_configs);
            }
        }
        // --> END LOAD ORDERING <--

        if (!should_load_config) {
            FhLog.Log(LogLevel.Info, $"{module_dll_name} loaded.");
            return true;
        }

        // --> LOAD CONFIGURATIONS <--
        string patched_json = File.ReadAllText(module_conf_name)._fix_up_link_symbols();

        List<FhModuleConfig> configs =
            JsonSerializer.Deserialize<List<FhModuleConfig>>(patched_json, FhUtil.JsonOpts) ??
            throw new Exception($"FH_E_CONF_DESERIALIZE_FAULT: {module_dll_name}.");

        FhLog.Log(LogLevel.Info, $"{module_dll_name} loaded, parsing {configs.Count.ToString()} configurations.");
        module_configs.AddRange(configs);
        // --> END LOAD CONFIGURATIONS <--

        return true;
    }

    /// <summary>
    ///     Replaces instances of well-known directory substitution strings in a config JSON being loaded
    ///     with the actual locations of these directories. This is provided so you can use the same, well-defined
    ///     file path relative to the binary for any file across all supported platforms.
    /// <para></para>
    ///     e.g. $resdir (Linux) -> /opt/fahrenheit/resources, $resdir (Windows) -> C:\Users\USER1\fahrenheit\resources
    /// </summary>
    private static string _fix_up_link_symbols(this string configJson) {
        return configJson.Replace(FhRuntimeConst.Binaries .LinkSymbol, FhRuntimeConst.Binaries .LinkPath).
                          Replace(FhRuntimeConst.Modules  .LinkSymbol, FhRuntimeConst.Modules  .LinkPath).
                          Replace(FhRuntimeConst.Logs     .LinkSymbol, FhRuntimeConst.Logs     .LinkPath).
                          Replace(FhRuntimeConst.Resources.LinkSymbol, FhRuntimeConst.Resources.LinkPath);
    }

    /* [fkelava 27/2/23 00:12]
     * Resolves a full type name in the "Type" variable of a given FhModuleConfig JSON
     * to an actual Type instance, verifying that it is in fact derived from FhModuleConfig.
     */
    public static bool resolve_descendant_of(                    this ref Utf8JsonReader reader,
                                                                          Type           type_to_convert,
                                             [NotNullWhen(true)]      out Type?          actual_type) {
        string type_name = reader.deserialize_and_advance<string>("Type");
        actual_type      = null;

        foreach (Assembly dll in _load_context.Assemblies) {
            if ((actual_type = dll.GetType(type_name)) != null) break;
        }

        return actual_type != null && actual_type.IsAssignableTo(type_to_convert);
    }

    public static bool resolve_exact(                    this ref Utf8JsonReader reader,
                                                                  Type           type_to_convert,
                                     [NotNullWhen(true)]      out Type?          actual_type) {
        string type_name = reader.deserialize_and_advance<string>("Type");
        actual_type      = null;

        foreach (Assembly dll in _load_context.Assemblies) {
            if ((actual_type = dll.GetType(type_name)) != null) break;
        }

        return actual_type != null && actual_type == type_to_convert;
    }
}