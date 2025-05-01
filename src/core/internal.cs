[assembly: InternalsVisibleToAttribute("fhruntime")]

namespace Fahrenheit.Core;

internal static class FhInternal {
    public static FhPathFinder       PathFinder       = new FhPathFinder();
    public static FhModController    ModController    = new FhModController();
    public static FhLoader           Loader           = new FhLoader();
    public static FhMethodAddressMap MethodAddressMap = new FhMethodAddressMap();
}
