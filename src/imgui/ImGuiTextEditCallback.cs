using System.Runtime.InteropServices;

namespace Fahrenheit.Core.ImGui.NET;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate int ImGuiInputTextCallback(ImGuiInputTextCallbackData* data);
