using System;
using System.Diagnostics.CodeAnalysis;

namespace Fahrenheit.CoreLib;

public class FhModuleHandle<T> where T : FhModule
{
    private readonly FhModule     _handleOwner;
    private          T?           _rawHandle;
    private readonly Predicate<T> _matchFunc;

    public FhModuleHandle(FhModule owner,
                          Predicate<T> match)
    {
        _handleOwner = owner;
        _matchFunc   = match;
    }

    public bool GetHandleSafe([NotNullWhen(true)] out T? handle)
    {
        return (handle = _rawHandle) != null;
    }

    public bool TryAcquire()
    {
        return (_rawHandle = FhModuleController.Find(_matchFunc)) != default;
    }

    public bool MarkDependency()
    {
        if (!GetHandleSafe(out T? handle)) return false;
        return FhModuleController.RegisterModuleDependency(_handleOwner, handle);
    }

    public bool UnmarkDependency()
    {
        if (!GetHandleSafe(out T? handle)) return false;
        return FhModuleController.UnregisterModuleDependency(_handleOwner, handle);
    }
}
