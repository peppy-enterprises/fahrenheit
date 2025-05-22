namespace Fahrenheit.Core;

/// <summary>
///     A representation of a fully loaded mod.
///     Contains its <see cref="FhManifest"/> and a <see cref="FhModuleContext"/> for each of its constituent modules.
/// </summary>
public sealed record FhModContext {
    internal readonly FhModPathInfo         Paths;
    public   readonly FhManifest            Manifest;
    public   readonly List<FhModuleContext> Modules;

    internal FhModContext(
        FhManifest            manifest,
        FhModPathInfo         paths,
        List<FhModuleContext> modules
    ) {
        Paths    = paths;
        Manifest = manifest;
        Modules  = modules;
    }
}

/// <summary>
///     A representation of a loaded module.
///     Contains a reference to it, and any metadata relating to it.
/// </summary>
public sealed record FhModuleContext {
    internal readonly FhModulePathInfo Paths;
    public   readonly FhModule         Module;

    internal FhModuleContext(
        FhModule         module,
        FhModulePathInfo paths
    ) {
        Paths  = paths;
        Module = module;
    }
}

/// <summary>
///     Contains metadata about a given local state file, such as the version of a module last used to write to it.
/// </summary>
public sealed record FhLocalStateInfo(
    string Version
    );

/// <summary>
///     Describes a unique Fahrenheit mod, consisting of zero to N DLLs, each containing zero to N <see cref="FhModule"/>.
/// </summary>
public sealed record FhManifest(
    string   Name,
    string   Desc,
    string   Authors,
    string   Version,
    string   Link,
    string[] DllList,
    string[] Dependencies,
    string[] LoadAfter
    );

/// <summary>
///     A 'module' is the base unit of functionality in Fahrenheit.
///     <para></para>
///     'Modules' are not the same as mods or even DLLs. Mods can be executed as one or several modules,
///     and one DLL can contain any number of modules. Modules can be used to logically partition your functionality.
/// </summary>
public abstract class FhModule {
    protected readonly string   _module_type_name;
    protected readonly FhLogger _logger;

    protected FhModule() {
        _module_type_name = GetType().FullName ?? throw new Exception("FH_E_MODULE_TYPE_UNIDENTIFIABLE");
        _logger           = new FhLogger($"{FhUtil.get_timestamp_string()}_{_module_type_name}.log");
    }

    internal string ModuleType {
        get { return _module_type_name; }
    }

    public abstract bool init(FileStream global_state_file);

    /// <summary>
    ///     Called when the game saves, allowing the module to save state specific to that save game.
    /// </summary>
    public virtual void save_local_state(FileStream local_state_file) { }

    /// <summary>
    ///     Called when the game loads, allowing the module to load state specific to that save game.
    /// </summary>
    public virtual void load_local_state(FileStream local_state_file, FhLocalStateInfo local_state_info) { }

    public virtual void pre_update()   { }
    public virtual void post_update()  { }
    public virtual void handle_input() { }
    public virtual void render_imgui() { }
    public virtual void render_game()  { }
}
