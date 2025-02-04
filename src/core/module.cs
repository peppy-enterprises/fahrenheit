namespace Fahrenheit.Core;

public abstract class FhModule {
    protected readonly string _module_name;
    protected readonly string _module_type_name;

    protected FhModule(FhModuleConfig moduleConfig) {
        _module_name      = moduleConfig.Name;
        _module_type_name = GetType().FullName ?? throw new Exception("FH_E_MODULE_TYPE_UNIDENTIFIABLE");
    }

    internal string ModuleType {
        get { return _module_type_name; }
    }

    public string ModuleName {
        get { return _module_name; }
    }

    public abstract bool init();

    public virtual void pre_update()   { }
    public virtual void post_update()  { }
    public virtual void handle_input() { }
    public virtual void render_imgui() { }
    public virtual void render_game()  { }
}