using System.Runtime.InteropServices;

namespace Fahrenheit.Modules.CSR;

internal unsafe static class Removers {
    [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
    private static extern isize memset(isize dst, i32 value, isize size);

    public static void init() {
        CSRModule.removers.Add("bsil0300", lagoon_remover);
    }

    private static void remove(u8* code_ptr, i32 from, i32 to) {
        memset((isize)code_ptr + from, 0, to - from);
    }

    internal static void intro_remover(u8* code_ptr) {
        remove(code_ptr, 0x487C, 0x4956);
    }

    internal static void lagoon_remover(u8* code_ptr) {
        remove(code_ptr, 0x1B29, 0x1C43);
    }
}
