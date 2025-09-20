// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core;

public enum FhGameType {
    FFX  = 1,
    FFX2 = 2
}

public static unsafe class FhGlobal {
    static FhGlobal() {
        base_addr = NativeLibrary.GetMainProgramHandle();
        game_type = FhUtil.get_game_type();
    }

    public static FhGameType game_type { get; }
    public static nint       base_addr { get; }
}
