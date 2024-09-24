using System;

namespace Fahrenheit.CoreLib;

// TODO: Not yet fully defined/fleshed out
public enum FhModuleState {
    InitWaiting,
    InitSuccess,
    Started,
    Stopped,
    Fault
}

public abstract class FhModule {
    protected string        _moduleName;
    protected FhModuleState _moduleState;

    protected FhModule(FhModuleConfig moduleConfig) {
        _moduleName = moduleConfig.ConfigName;
    }

    internal string ModuleType {
        get { return GetType().FullName ?? throw new Exception("FH_E_MODULE_TYPE_UNIDENTIFIABLE"); }
    }

    public string ModuleName {
        get { return _moduleName; }
    }

    public FhModuleState ModuleState {
        get           { return _moduleState;  }
        protected set { _moduleState = value; }
    }

    public virtual bool FhModuleInit()    => true;
    public virtual bool FhModuleOnError() => true;

    public virtual void pre_update()   { }
    public virtual void post_update()  { }
    public virtual void handle_input() { }
    public virtual void render_imgui() { }
    public virtual void render_game()  { }
}