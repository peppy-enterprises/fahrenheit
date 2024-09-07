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

public class FhModuleStateChangeEventArgs : EventArgs {
    public FhModuleStateChangeEventArgs(FhModuleState oldstate, FhModuleState newstate) {
        OldState = oldstate;
        NewState = newstate;
    }

    public FhModuleState OldState { get; }
    public FhModuleState NewState { get; }
}

public abstract class FhModule : IEquatable<FhModule> {
    protected string         _moduleName;
    protected FhModuleState  _moduleState;

    protected FhModule(FhModuleConfig moduleConfig) {
        _moduleName = moduleConfig.ConfigName;
    }

    internal string ModuleType {
        get { return GetType().FullName ?? throw new Exception("FH_E_MODULE_TYPE_UNIDENTIFIABLE"); }
    }

    public string ModuleName {
        get { return _moduleName; }
    }

    /* [fkelava 27/3/23 01:59]
     * ModuleState is the _only_ way for modules to declare that they've suffered a hard fault.
     * Modules may UNDER NO CIRCUMSTANCE WHATSOEVER throw exceptions, because they're loaded into the same
     * AppDomain/address space/process as everything else.
     *
     * Why is that? Why not interprocess communication?
     *   1) The IPC methods at our disposal are, effectively, memory-mapped files and named pipes; neither are easy to get right.
     *   2) The IPC methods at our disposal would require strong interprocess locking, which I find difficult for end-users;
     *   3) I'm not sure whether either of mmap'ing or named pipes can be as efficient as this...? (I'm probably wrong.)
     *
     * Hence ModuleState. Just `ModuleState = ModuleState.Fault;` and your DI'd IFhModuleController will
     * transparently handle the fault and propagate it to all dependent modules as well.
     */

    public FhModuleState ModuleState {
        get { return _moduleState; }
        protected set {
            FhModuleController.ModuleStateChangeHandler(this, new(_moduleState, value));
            _moduleState = value;
        }
    }

    public abstract FhModuleConfig ModuleConfiguration { get; }

    public abstract bool FhModuleInit();
    public abstract bool FhModuleStart();
    public abstract bool FhModuleStop();
    public abstract bool FhModuleOnError();

    public bool Equals(FhModule? other) {
        if (other is null)                return false;
        if (ReferenceEquals(this, other)) return true;

        return _moduleName.Equals(other.ModuleName);
    }

    public override bool Equals(object? obj) {
        if (obj is null)                return false;
        if (ReferenceEquals(this, obj)) return true;

        return obj.GetType() == GetType() && Equals((FhModule)obj);
    }

    public override int GetHashCode() {
        return _moduleName.GetHashCode();
    }

    public static bool operator ==(FhModule? left, FhModule? right) {
        return Equals(left, right);
    }

    public static bool operator !=(FhModule? left, FhModule? right) {
        return !Equals(left, right);
    }
}