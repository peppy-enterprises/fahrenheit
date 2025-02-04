using System.Runtime.InteropServices;

namespace Fahrenheit.Core.ImGuiNET;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate int ImGuiInputTextCallback(ImGuiInputTextCallbackData* data);
