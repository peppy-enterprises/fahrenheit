using System;

namespace Fahrenheit.Core;

public abstract class FhModule {
    protected string _moduleName;

    protected FhModule(FhModuleConfig moduleConfig) {
        _moduleName = moduleConfig.ConfigName;
    }

    internal string ModuleType {
        get { return GetType().FullName ?? throw new Exception("FH_E_MODULE_TYPE_UNIDENTIFIABLE"); }
    }

    public string ModuleName {
        get { return _moduleName; }
    }

    public abstract bool init();

    public virtual void pre_update()   { }
    public virtual void post_update()  { }
    public virtual void handle_input() { }
    public virtual void render_imgui() { }
    public virtual void render_game()  { }
}