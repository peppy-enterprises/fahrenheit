namespace Fahrenheit.Core;

public sealed record FhManifest(
    string   Name,
    string   Desc,
    string   Authors,
    string   Link,
    string[] DllList,
    string[] Dependencies,
    string[] LoadAfter
    );

public sealed record FhModPathInfo(
    string        ManifestPath,
    DirectoryInfo ModuleDir,
    DirectoryInfo ResourcesDir,
    DirectoryInfo EflDir,
    DirectoryInfo LangDir,
    DirectoryInfo StateDir);

public sealed record FhModulePathInfo(
    string DllPath,
    string ConfigPath);

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

    public abstract bool init();

    public virtual void pre_update()   { }
    public virtual void post_update()  { }
    public virtual void handle_input() { }
    public virtual void render_imgui() { }
    public virtual void render_game()  { }
}
