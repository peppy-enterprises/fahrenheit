[assembly: System.Runtime.CompilerServices.InternalsVisibleToAttribute("fhruntime")]

namespace Fahrenheit.Core;

internal static class FhInternal {
    public static FhModuleController ModuleController = new FhModuleController();
    public static FhLoader           Loader           = new FhLoader();
    public static FhMethodAddressMap MethodAddressMap = new FhMethodAddressMap();
}
