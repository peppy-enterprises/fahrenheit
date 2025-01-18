using System;
using System.Diagnostics.CodeAnalysis;

namespace Fahrenheit.Core;

public class FhModuleHandle<T> where T : FhModule {
    private readonly FhModule         _handleOwner;
    private          FhModuleContext? _matchCtx;
    private readonly Predicate<T>     _matchFunc;

    public FhModuleHandle(FhModule     owner,
                          Predicate<T> match) {
        _handleOwner = owner;
        _matchFunc   = match;
    }

    public bool try_get_handle([NotNullWhen(true)] out FhModuleContext? handle) {
        return (handle = _matchCtx) != null;
    }

    public bool try_acquire() {
        return (_matchCtx = FhModuleController.find(_matchFunc)) != null;
    }
}
