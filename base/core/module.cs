namespace Fahrenheit.Core;

/// <summary>
///     A representation of a fully loaded mod.
///     Contains its <see cref="FhManifest"/> and a <see cref="FhModuleContext"/> for each of its constituent modules.
/// </summary>
public sealed record FhModContext {
    public readonly FhModPathInfo         Paths;
    public readonly FhManifest            Manifest;
    public readonly List<FhModuleContext> Modules;

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
///     A 'module' is the base unit of functionality in Fahrenheit. Modules are used to logically partition your functionality.
///     <para></para>
///     'Modules' are not the same as mods or even DLLs. Mods can be executed as one or several DLLs,
///     and one DLL can contain any number of modules.
/// </summary>
public abstract class FhModule {
    protected readonly string   _module_type_name;
    protected readonly FhLogger _logger;
    public FhSettingsCategory? settings { get; protected internal set; }

    protected FhModule() {
        _module_type_name = GetType().FullName ?? throw new Exception("FH_E_MODULE_TYPE_UNIDENTIFIABLE");
        _logger           = new FhLogger($"{FhUtil.get_timestamp_string()}_{_module_type_name}.log");
    }

    internal string ModuleType {
        get { return _module_type_name; }
    }

    /// <summary>
    ///     Your module should perform all Fahrenheit-related initialization here. At the time this is called, all mods have been loaded, and:
    ///     <br/>
    ///     <br/> - you may call <see cref="FhModuleHandle{T}.try_acquire"/> to obtain references to other modules;
    ///     <br/> - you receive a copy of the containing mod's <see cref="FhModContext"/>;
    ///     <br/> - you receive a <see cref="FileStream"/> of the global state file for your module;
    /// </summary>
    /// <returns>Whether initialization succeeded. If <see cref="false"/>, an error is shown to the user, but execution continues.</returns>
    public abstract bool init(FhModContext mod_context, FileStream global_state_file);

    /// <summary>
    ///     Called when the game saves, allowing the module to save state specific to that save game.
    /// </summary>
    public virtual void save_local_state(FileStream local_state_file) { }

    /// <summary>
    ///     Called when the game loads, allowing the module to load state specific to that save game.
    /// </summary>
    public virtual void load_local_state(FileStream local_state_file, FhLocalStateInfo local_state_info) { }

    /// <summary>
    ///     Called before every main loop execution.
    /// </summary>
    public virtual void pre_update() { }

    /// <summary>
    ///     Called after every main loop execution.
    /// </summary>
    public virtual void post_update() { }

    /// <summary>
    ///     Called just before <see href="https://learn.microsoft.com/en-us/windows/win32/api/dxgi/nf-dxgi-idxgiswapchain-present">
    ///     IDXGISwapChain::Present</see> time. You may freely invoke ImGui methods here, and <i>only</i> here.
    /// </summary>
    public virtual void render_imgui() { }
    public virtual void render_game()  { }
    public virtual void handle_input() { }
}
