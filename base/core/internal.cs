/* [fkelava 23/6/25 13:47]
 * This is exclusively permitted to the runtime library so it can fulfill the contracts specified
 * in the Fahrenheit API. If you need access to something currently marked internal, open an issue
 * or contact the developers and explain the use case instead of extending IVT to your mod.
 */
[assembly: InternalsVisibleTo("fhruntime")]

namespace Fahrenheit.Core;

// The initialization order here is not incidental. Objects higher in the list may not rely on objects below them in their constructor.

/// <summary>
///     An accessor for objects private to the Fahrenheit core and runtime libraries.
/// </summary>
internal static class FhInternal {
    public static FhPathFinder       PathFinder       = new FhPathFinder();
    public static FhLogger           Log              = new FhLogger($"{FhUtil.get_timestamp_string()}__core.log");
    public static FhLoader           Loader           = new FhLoader();
    public static FhMethodAddressMap MethodAddressMap = new FhMethodAddressMap();
}
