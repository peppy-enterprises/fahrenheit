// SPDX-License-Identifier: MIT

/* [fkelava 23/6/25 13:47]
 * This is exclusively permitted to the runtime library so it can fulfill the contracts specified
 * in the Fahrenheit API. If you need access to something currently marked internal, open an issue
 * or contact the developers and explain the use case instead of extending IVT to your mod.
 */
[assembly: InternalsVisibleTo("fhruntime")]

namespace Fahrenheit.Core;

/// <summary>
///     The accessor for objects and helpers that compose the public Fahrenheit API.
/// </summary>
public static class FhApi {
    public static          FhModController       Mods          = new FhModController();
    public static readonly FhLocalizationManager Localization  = new FhLocalizationManager();
    public static readonly FhResourceLoader      Resources     = new FhResourceLoader();
    public static readonly FhImGuiHelper         ImGuiHelper   = new FhImGuiHelper();
    public static readonly FhInput               Input         = new FhInput();
}

/// <summary>
///     An accessor for objects private to the Fahrenheit core and runtime libraries.
/// </summary>
internal static class FhInternal {
    // The initialization order here is not incidental. Objects higher in the list may not rely on objects below them in their constructor.
    public static readonly FhPathFinder       PathFinder       = new FhPathFinder();
    public static readonly FhLogger           Log              = new FhLogger($"{FhUtil.get_timestamp_string()}__core.log");
    public static readonly FhLoader           Loader           = new FhLoader();
    public static readonly FhMethodAddressMap MethodAddressMap = new FhMethodAddressMap();
}
