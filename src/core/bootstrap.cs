namespace Fahrenheit.Core;

/// <summary>
///     Contains Fahrenheit boot logic. The execution of the C# segment of Fahrenheit begins here.
/// </summary>
public static class FhBootstrapper {
    /* [fkelava 25/4/24 18:47]
     * This class, delegate (and its signature), and method are all referenced by Stage1.
     * Updating or renaming any of them requires a Stage1 update.
     *
     * The delegate's signature is arbitrary. S1 can, if necessary, pass parameters to C#.
     * It is only mandatory that bootstrap()'s signature matches the delegate.
     */
    public delegate void FhBootstrapDelegate();

    public static void bootstrap() {
        string   load_order_path = Path.Join(FhRuntimeConst.Modules.LinkPath, "loadorder");
        string[] load_order      = File.ReadAllLines(load_order_path);
        string   runtime_path    = Path.Join(FhRuntimeConst.Binaries.LinkPath, "fhruntime.dll");

        FhInternal.Loader.load(runtime_path, out List<FhModuleConfig> module_configs); // Entirety of FhInternal is initialized at this point.
        FhInternal.ModuleController.spawn_modules(module_configs);

        foreach (string module in load_order) {
            string module_path = Path.Join(FhRuntimeConst.Modules.LinkPath, module, $"{module}.dll");
            FhInternal.Loader.load(module_path, out module_configs);
            FhInternal.ModuleController.spawn_modules(module_configs);
        }

        FhLocalizationManager.construct_localization_map();
        FhInternal.ModuleController.initialize_modules();
    }
}
