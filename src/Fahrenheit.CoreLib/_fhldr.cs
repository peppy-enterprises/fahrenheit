using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;

namespace Fahrenheit.CoreLib;

public static class FhLoader {
    public delegate void FhInitDelegate();

    public static void ldr_bootstrap() {
        if (!load_modules(FhRuntimeConst.ModulesDir.Path, out List<FhModuleConfigCollection>? moduleConfigs))
            throw new Exception("FH_E_LDR_MODULE_LOAD_FAILED");

        FhModuleController.Initialize(moduleConfigs);
        foreach (bool rv in FhModuleController.StartAll()) { }
    }

    private static List<Assembly> loaded_plugin_assemblies_cache { get; } = new List<Assembly>();

    /// <summary>
    ///     Loosely infers whether a module _could_ be a valid Fahrenheit module. It does so by checking whether the plugin has a .runtimeconfig.json and .deps.json file.
    /// <para></para>
    ///     This is because a properly exported plugin will mark &lt;EnableDynamicLoading&gt;true&lt;/EnableDynamicLoading&gt; in its project, hence have a *.runtimeconfig.json and *.deps.json.
    /// </summary>
    private static bool is_module(string dirEntry) {
        return dirEntry.EndsWith(".dll")
               && File.Exists(dirEntry.Replace(".dll", ".runtimeconfig.json"))
               && File.Exists(dirEntry.Replace(".dll", ".deps.json"));
    }

    private static bool is_assem_loaded(string refAssemName) {
        foreach (Assembly loadedAssembly in loaded_plugin_assemblies_cache) {
            if (loadedAssembly.GetName().Name?.ToUpperInvariant() == refAssemName)
                return true;
        }

        return false;
    }

    /// <summary>
    ///     Loosely infers whether a referenced assembly <i>should</i> be loaded by Fahrenheit instead of being lazy-loaded by the .NET runtime.
    /// </summary>
    private static bool try_resolve_module_ref(string dirPath, string refAssemName, out string refAssemFullPath) {
        refAssemFullPath = Path.Join(dirPath, $"{refAssemName}.dll");

        return File.Exists(refAssemFullPath) && !is_assem_loaded(refAssemName);
    }

    /// <summary>
    ///     Loads a module while ensuring its references, if detected, are loaded before it.
    /// </summary>
    private static bool load_single_module(string fullPath, [NotNullWhen(true)] out List<FhModuleConfigCollection>? moduleConfigCollections) {
        moduleConfigCollections = new List<FhModuleConfigCollection>();

        string dirName        = Path.GetDirectoryName(fullPath) ?? throw new Exception("FH_E_MODULE_DIR_UNIDENTIFIABLE");
        string moduleName     = Path.GetFileNameWithoutExtension(fullPath).ToUpperInvariant();
        string configJsonName = Path.Join(FhRuntimeConst.ConfigDir.Path, Path.GetFileName(fullPath).Replace(".dll", ".conf.json"));

        bool shouldLoadConfig = File.Exists(configJsonName);

        if (is_assem_loaded(moduleName)) {
            FhLog.Log(LogLevel.Info, $"{moduleName} already loaded; skipping.");
            return true;
        }

        FhLog.Log(LogLevel.Info, $"Loading module {moduleName}.");

        AssemblyLoadContext asmLoadCtx            = AssemblyLoadContext.GetLoadContext(typeof(FhLoader).Assembly) ?? throw new Exception("E_CANNOT_GET_OWN_ALC");
        Assembly            currentlyLoadingAssem = asmLoadCtx.LoadFromAssemblyPath(fullPath);

        loaded_plugin_assemblies_cache.Add(currentlyLoadingAssem);

        // --> LOAD ORDERING; LOAD REFERENCED ASSEMBLIES FIRST <--
        foreach (AssemblyName refAssem in currentlyLoadingAssem.GetReferencedAssemblies()) {
            if (try_resolve_module_ref(dirName, refAssem.Name ?? throw new Exception("FH_E_REF_ASSEM_NAME_NULL"), out string refAssemFullPath)) {
                FhLog.Log(LogLevel.Info, $"{moduleName} depends on {refAssem.Name.ToUpperInvariant()}; trying to load it first.");

                if (!load_single_module(refAssemFullPath, out List<FhModuleConfigCollection>? refAssemConfigCollection))
                    throw new Exception("FH_E_REF_ASSEM_LOAD_FAULT");

                moduleConfigCollections.AddRange(refAssemConfigCollection);
            }
        }
        // --> END LOAD ORDERING <--

        // --> LOAD CONFIGURATIONS <--
        if (shouldLoadConfig) {
            string patchedJson = File.ReadAllText(configJsonName).apply_rtconst_dir_redirects();

            FhModuleConfigCollection configCollection =
                JsonSerializer.Deserialize<FhModuleConfigCollection>(patchedJson, FhUtil.JsonOpts) ??
                throw new Exception($"FH_E_CONF_DESERIALIZE_FAULT: {moduleName}.");

            FhLog.Log(LogLevel.Info, $"{moduleName} loaded successfully, parsing {configCollection.ModuleConfigs.Count.ToString()} configurations.");

            moduleConfigCollections.Add(configCollection);
        }
        else FhLog.Log(LogLevel.Info, $"{moduleName} loaded successfully.");
        // --> END LOAD CONFIGURATIONS <--

        return true;
    }

    /// <summary>
    ///     Loads and instantiates all eligible modules in the module directory.
    ///     An eligible module has a defined configuration named *.conf.json.
    ///     Referenced modules are also resolved and loaded at the same time if required.
    /// </summary>
    public static bool load_modules(string dirPath, [NotNullWhen(true)] out List<FhModuleConfigCollection>? moduleConfigCollections) {
        moduleConfigCollections = new List<FhModuleConfigCollection>();

        foreach (string dirEntry in Directory.EnumerateFiles(dirPath)) {
            if (!is_module(dirEntry) || !load_single_module(dirEntry, out List<FhModuleConfigCollection>? singleModuleConfigCollection)) continue;
            moduleConfigCollections.AddRange(singleModuleConfigCollection);
        }

        return true;
    }

    /// <summary>
    ///     Replaces instances of well-known directory substitution strings in a config JSON being loaded
    ///     with the actual locations of these directories. This is provided so you can use the same, well-defined
    ///     file path relative to the binary for any file across all supported platforms.
    /// <para></para>
    ///     e.g. $resdir (Linux) -> /opt/fahrenheit/resources, $resdir (Windows) -> C:\Users\USER1\fahrenheit\resources
    /// <para></para>
    ///     For the actual directories and how they are computed, see <see cref="FhRuntimeConst"/>.
    /// </summary>
    internal static string apply_rtconst_dir_redirects(this string configJson) {
        foreach (FieldInfo field in typeof(FhRuntimeConst).GetFields(BindingFlags.Public | BindingFlags.Static)) {
            if (field.FieldType == typeof(FhDirLink)) {
                FhDirLink? dirLink = field.GetValue(null) as FhDirLink ?? throw new Exception("FH_E_DIR_LINK_FAULT");
                configJson = configJson.Replace(dirLink.LinkSymbol, dirLink.Path);
            }
        }

        return configJson;
    }

    /* [fkelava 27/2/23 00:12]
     * In systems such as ours where JSON-serialized objects contain their full type name
     * to allow for polymorphic deserialization, we must prevent the spawning of arbitrary types.
     *
     * These methods take the "Type" variable of a given JSON configuration and try to resolve it
     * to an actual Type instance while making sure that it not only exists, but is what we expect-
     * usually a derived type of FhModuleConfig.
     */
    public static bool strict_resolve_descendant_of<T>(this ref Utf8JsonReader reader,
                                                                Type           typeToConvert,
                                                            out Type           actualType) {
        string typename = reader.deserialize_and_advance<string>("Type");

        foreach (Assembly assembly in loaded_plugin_assemblies_cache) {
            foreach (Type type in assembly.GetExportedTypes()) {
                if (type.FullName != typename || !typeof(T).IsAssignableFrom(type) || type == typeof(T)) continue;

                if (!typeToConvert.IsAssignableFrom(type)) {
                    throw new JsonException($"E_TYPE_MISMATCH: {typeToConvert.FullName} is not assignable from {type.FullName}.");
                }

                actualType = type;
                return true;
            }
        }

        throw new JsonException($"E_TYPE_NOTFOUND: type {typename} was not found in any loaded assembly. Check your configuration.");
    }

    public static bool strict_resolve_exact_type<T>(this ref Utf8JsonReader reader,
                                                             Type           typeToConvert,
                                                         out Type           actualType) {
        string typename = reader.deserialize_and_advance<string>("Type");

        foreach (Assembly assembly in loaded_plugin_assemblies_cache) {
            foreach (Type type in assembly.GetTypes()) {
                if (type.FullName != typename || type != typeof(T)) continue;

                if (type != typeToConvert) {
                    throw new JsonException($"E_TYPE_MISMATCH: expected {typeToConvert.FullName} but got {type.FullName}");
                }

                actualType = type;
                return true;
            }
        }

        throw new JsonException($"E_TYPE_NOTFOUND: type {typename} was not found in any loaded assembly. Check your configuration.");
    }
}