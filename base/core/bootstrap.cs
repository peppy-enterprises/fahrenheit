namespace Fahrenheit.Core;

/// <summary>
///     Contains Fahrenheit boot logic. The execution of the C# segment of Fahrenheit begins here.
///     When the bootstrapper completes, game execution commences.
/// </summary>
public static class FhBootstrapper {
    /* [fkelava 25/4/24 18:47]
     * This class, delegate (and its signature), and method are all referenced by Stage1.
     * Updating or renaming any of them requires a Stage1 update.
     *
     * The delegate's signature is arbitrary. S1 can, if necessary, pass parameters to C#.
     * It is only mandatory that bootstrap()'s signature matches the delegate.
     */
    public delegate void FhBootstrapDelegate();

    public static void bootstrap() {
        FhApi.ModController.load_mods();
        FhApi.LocalizationManager.construct_localization_map();
        FhApi.ModController.initialize_mods();
    }
}
