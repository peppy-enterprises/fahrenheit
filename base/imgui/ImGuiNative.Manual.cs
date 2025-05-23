using System;
using System.Runtime.InteropServices;

namespace Fahrenheit.Core.ImGuiNET;

public static unsafe partial class ImGuiNative
{
    [DllImport(FhConst.cimgui_lib_name, CallingConvention = CallingConvention.Cdecl)]
    public static extern void ImGuiPlatformIO_Set_Platform_GetWindowPos(ImGuiPlatformIO* platform_io, IntPtr funcPtr);
    [DllImport(FhConst.cimgui_lib_name, CallingConvention = CallingConvention.Cdecl)]
    public static extern void ImGuiPlatformIO_Set_Platform_GetWindowSize(ImGuiPlatformIO* platform_io, IntPtr funcPtr);
}
