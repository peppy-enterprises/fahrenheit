// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core;

[Flags]
public enum FhGameId {
    NULL = 0,
    FFX  = 1,
    FFX2 = 2,
}

public static class FhGlobal {
    static FhGlobal() {
        base_addr = NativeLibrary.GetMainProgramHandle();
        game_id   = FhUtil.get_game_id();
        lang_id   = FhUtil.get_lang_id();
    }

    public static readonly FhLangId lang_id;
    public static readonly FhGameId game_id;
    public static readonly nint     base_addr;
}
