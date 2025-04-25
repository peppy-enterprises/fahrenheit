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

    /// <summary>
    ///     Replaces instances of well-known directory substitution strings in a config JSON being loaded
    ///     with the actual locations of these directories. This is provided so you can use the same, well-defined
    ///     file path relative to the binary for any file across all supported platforms.
    /// <para></para>
    ///     e.g. $resdir (Linux) -> /opt/fahrenheit/resources, $resdir (Windows) -> C:\Users\USER1\fahrenheit\resources
    /// </summary>
    private static string _fix_up_link_symbols(string configJson) {
        return configJson.Replace(FhRuntimeConst.Binaries.LinkSymbol, FhRuntimeConst.Binaries.LinkPath).
                          Replace(FhRuntimeConst.Modules .LinkSymbol, FhRuntimeConst.Modules .LinkPath).
                          Replace(FhRuntimeConst.Logs    .LinkSymbol, FhRuntimeConst.Logs    .LinkPath).
                          Replace(FhRuntimeConst.State   .LinkSymbol, FhRuntimeConst.State   .LinkPath).
                          Replace(FhRuntimeConst.Saves   .LinkSymbol, FhRuntimeConst.Saves   .LinkPath);
    }

    // Loads a DLL while ensuring its references, if detected, are loaded before it.
    public void load(string dll_full_path, out List<FhModuleConfig> module_configs) {
        module_configs = [];

        string module_dir_name    = Path.GetDirectoryName           (dll_full_path) ?? throw new Exception("FH_E_MODULE_DIR_UNIDENTIFIABLE");
        string module_dll_name    = Path.GetFileNameWithoutExtension(dll_full_path).ToUpperInvariant();
        string module_conf_name   = dll_full_path.Replace(".dll", ".config.json");
        bool   should_load_config = File.Exists(module_conf_name);

        if (_is_loaded(module_dll_name)) return;

        FhLog.Log(LogLevel.Info, $"Loading module {module_dll_name}.");
        Assembly this_assembly = _load_context.LoadFromAssemblyPath(dll_full_path);

        // Load dependencies first.
        foreach (AssemblyName dependency in this_assembly.GetReferencedAssemblies()) {
            if (_should_load(module_dir_name, dependency.Name ?? throw new Exception("FH_E_REF_DLL_NAME_NULL"), out string dependency_full_path)) {
                load(dependency_full_path, out List<FhModuleConfig> dependency_module_configs);
                module_configs.AddRange(dependency_module_configs);
            }
        }

        // Dependency DLLs may not be Fahrenheit modules themselves and thus have no config.
        if (!should_load_config) {
            FhLog.Log(LogLevel.Info, $"{module_dll_name} loaded.");
            return;
        }

        // Load configuration.
        string patched_json = _fix_up_link_symbols(File.ReadAllText(module_conf_name));

        List<FhModuleConfig> configs =
            JsonSerializer.Deserialize<List<FhModuleConfig>>(patched_json, FhUtil.JsonOpts) ??
            throw new Exception($"FH_E_CONF_DESERIALIZE_FAULT: {module_dll_name}.");

        FhLog.Log(LogLevel.Info, $"{module_dll_name} loaded, parsing {configs.Count.ToString()} configurations.");
        module_configs.AddRange(configs);
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
