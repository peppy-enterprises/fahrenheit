namespace Fahrenheit.Core;

public static class FhBootstrapper {
    // Stage1 references this delegate and method by name. If you change them or move them to a different type, Stage1 must be updated.
    public delegate void FhBootstrapDelegate();

    // The point at which Fahrenheit execution begins. Stage1 calls into this method directly.
    public static void bootstrap() {
        string   load_order_path = Path.Join(FhRuntimeConst.Modules.LinkPath, "loadorder");
        string[] load_order      = File.ReadAllLines(load_order_path);
        string   runtime_path    = Path.Join(FhRuntimeConst.Binaries.LinkPath, "fhruntime.dll");

        FhInternal.Loader.load(runtime_path, out List<FhModuleConfig> module_configs);
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

