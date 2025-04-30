[assembly: InternalsVisibleToAttribute("fhruntime")]

namespace Fahrenheit.Core;

internal static class FhInternal {
    public static FhModController    ModController    = new FhModController();
    public static FhLoader           Loader           = new FhLoader();
    public static FhMethodAddressMap MethodAddressMap = new FhMethodAddressMap();
    public static FhPathFinder       PathFinder       = new FhPathFinder();
}
