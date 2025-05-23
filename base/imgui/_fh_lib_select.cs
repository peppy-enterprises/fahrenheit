namespace Fahrenheit.Core.ImGuiNET;

internal static class FhConst {
#if DEBUG
    public const string cimgui_lib_name = "cimguid.dll";
#else
    public const string cimgui_lib_name = "cimgui.dll";
#endif
}