[assembly: InternalsVisibleTo("fhruntime")]

namespace Fahrenheit.Core;

// The initialization order here is not incidental. Objects higher in the list may not rely on objects below them in their constructor.
internal static class FhInternal {
    public static FhPathFinder       PathFinder       = new FhPathFinder();
    public static FhLogger           Log              = new FhLogger($"{FhUtil.get_timestamp_string()}__core.log");
    public static FhModController    ModController    = new FhModController();
    public static FhLoader           Loader           = new FhLoader();
    public static FhMethodAddressMap MethodAddressMap = new FhMethodAddressMap();
}
