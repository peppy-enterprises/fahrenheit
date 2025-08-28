namespace Fahrenheit.Core;

/// <summary>
///     Provides runtime binding to a <see cref="FhModule"/> of type <typeparamref name="T"/>.
///     You may then access its <see cref="FhModuleContext"/>.
/// </summary>
public class FhModuleHandle<T>(FhModule owner) where T : FhModule {
    private readonly FhModule         _owner = owner;
    private          FhModuleContext? _match;

    /// <summary>
    ///     Queries the <see cref="FhModController"/> for a module of type <typeparamref name="T"/>,
    ///     caching the match if found, and returns its <see cref="FhModuleContext"/>.
    /// </summary>
    public bool try_get([NotNullWhen(true)] out FhModuleContext? handle) {
        return (handle = (_match ??= FhApi.ModController.get_module<T>())) != null;
    }
}
