namespace Fahrenheit.Core;

/// <summary>
///     Contains Fahrenheit boot logic. When the bootstrapper completes, game execution commences.
/// </summary>
public static class FhBootstrapper {
    /* [fkelava 25/4/24 18:47]
     * This class and method are referenced by Stage1. Updating or renaming either requires a Stage1 update.
     */
    [UnmanagedCallersOnly]
    public static void bootstrap() {
        FhApi.ModController = new(FhInternal.Loader.load_mods());

        FhApi.LocalizationManager.construct_localization_map();
        FhApi.ModController.initialize_mods();
    }
}
