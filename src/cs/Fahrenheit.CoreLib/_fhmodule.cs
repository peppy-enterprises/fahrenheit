using System;

namespace Fahrenheit.CoreLib;

// TODO: Not yet fully defined/fleshed out
public enum ModuleState
{
    InitWaiting,
    InitSuccess,
    Started,
    Stopped,
    Fault,
    Abort
}

public class FiModuleStateChangeEventArgs : EventArgs
{
    public FiModuleStateChangeEventArgs(ModuleState oldstate, ModuleState newstate, FhModule fm)
    {
        OldState = oldstate;
        NewState = newstate;
        Caller   = fm;
    }

    public ModuleState OldState { get; }
    public ModuleState NewState { get; }
    public FhModule    Caller   { get; }
}

public abstract class FhModule : IEquatable<FhModule>
{
    private ModuleState _moduleState;

    protected FhModule(IFhModuleController parent, FhModuleConfig moduleConfig)
    {
        Controller          = parent;
        ModuleConfiguration = moduleConfig;
        Name                = moduleConfig.ConfigName;

        OnModuleStateChange += parent.ModuleStateChangeHandler;
    }

    protected      IFhModuleController Controller          { get; }
    public         string              Name                { get; }
    public virtual FhModuleConfig      ModuleConfiguration { get; }

    /* [fkelava 27/3/23 01:59]
     * ModuleState is the _only_ legal way for modules to declare that they've suffered a hard fault.
     * Modules may UNDER NO CIRCUMSTANCE WHATSOEVER throw exceptions, because they're loaded into the same 
     * AppDomain/address space/process as everything else.
     * 
     * Why is that? Why not interprocess communication?
     *   1) The IPC methods at our disposal are, effectively, memory-mapped files and named pipes; neither are easy to get right.
     *   2) The IPC methods at our disposal would require strong interprocess locking, which I find difficult for end-users;
     *   3) I'm not sure whether either of mmap'ing or named pipes can be as efficient as this...? (I'm probably wrong.)
     *   
     * Hence ModuleState. Just `ModuleState = ModuleState.Fault;` and your DI'd IFiModuleController will 
     * transparently handle the fault and propagate it to all dependent modules as well.
     * 
     * TODO: It would be nice if FiModules were also forced to specify restart callbacks in case they fault.
     */
    public ModuleState ModuleState
    {
        get { return _moduleState; }
        protected set {
#pragma warning disable CS8602
            // intentionally fail with a null reference if an IFiModuleController has not hooked this method.
            OnModuleStateChange(this, new FiModuleStateChangeEventArgs(ModuleState, value, this));
            _moduleState = value;
#pragma warning restore CS8602
        }
    }

    /// <summary>
    ///     Transparently hooked by the module's injected parent <see cref="IFiModuleController"/> to handle any changes to ModuleState.
    /// </summary>
    public event EventHandler<FiModuleStateChangeEventArgs>? OnModuleStateChange;

    public bool Equals(FhModule? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Name.Equals(other.Name);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj.GetType() == GetType() && Equals((FhModule)obj);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }

    public static bool operator ==(FhModule? left, FhModule? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(FhModule? left, FhModule? right)
    {
        return !Equals(left, right);
    }
}