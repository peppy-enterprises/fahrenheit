// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core;

public enum FhGameType {
    NULL = 0,
    FFX  = 1,
    FFX2 = 2
}

public static unsafe class FhGlobal {
    static FhGlobal() {
        base_addr = NativeLibrary.GetMainProgramHandle();
        game_type = FhUtil.get_game_type();
        game_lang = FhUtil.get_game_lang();
    }

    public static readonly FhLangId   game_lang;
    public static readonly FhGameType game_type;
    public static readonly nint       base_addr;
}
