// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x1C)]
public struct LocalizationManager {
    [FieldOffset(0x4)] public FhLangId current_language;
}
