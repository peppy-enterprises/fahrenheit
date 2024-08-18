// Deprecated until further notice.

namespace Fahrenheit.CoreLib.FFX;

public readonly struct FhDialogueIndex {
    public readonly T_FhDialoguePos         index;
    public readonly byte                    __0x03;
    public readonly T_FhDialogueOptionCount optionCount;
}

public readonly struct FhMacroDictHeader {
    public const int MD_SECTION_NR  = 16;
    public const int MD_HEADER_SIZE = MD_SECTION_NR * sizeof(int);

    public readonly int[] SectionOffsets;

    public FhMacroDictHeader() {
        SectionOffsets = new int[MD_SECTION_NR];
    }
}

public readonly struct FhMacroDictIndex {
    public readonly T_FhDialoguePos index;
}

public readonly struct FhMacroDictSection {
    public readonly FhMacroDictIndex[] Indices;

    public FhMacroDictSection(in FhMacroDictIndex[] indices) {
        Indices = indices;
    }
}
