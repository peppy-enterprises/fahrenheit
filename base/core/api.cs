namespace Fahrenheit.Core;

public static class FhApi {
    public static FhModController       ModController       = new FhModController();
    public static FhLocalizationManager LocalizationManager = new FhLocalizationManager();
    public static FhResourceLoader      ResourceLoader      = new FhResourceLoader();
    public static FhImGuiHelper         ImGuiHelper         = new FhImGuiHelper();
    public static FhInput               Input               = new FhInput();
}
