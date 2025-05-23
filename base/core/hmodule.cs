namespace Fahrenheit.Core;

/// <summary>
///     Provides access to a <see cref="FhModule"/> of type <typeparamref name="T"/>.
///     You may then access its <see cref="FhModuleContext"/>.
/// </summary>
public class FhModuleHandle<T> where T : FhModule {
    private readonly FhModule         _owner;
    private          FhModuleContext? _match_ctx;
    private readonly Predicate<T>     _match_func;

    public FhModuleHandle(FhModule     owner,
                          Predicate<T> match) {
        _owner      = owner;
        _match_func = match;
    }

    public bool try_get_handle([NotNullWhen(true)] out FhModuleContext? handle) {
        return (handle = _match_ctx) != null;
    }

    public bool try_acquire() {
        return (_match_ctx = FhApi.ModController.find_module(_match_func)) != null;
    }
}
