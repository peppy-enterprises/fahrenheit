using Fahrenheit.CoreLib;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;
using static Fahrenheit.CoreLib.FhHookDelegates;
using static Fahrenheit.Modules.Debug.Delegates;

namespace Fahrenheit.Modules.Debug;

public unsafe partial class DebugModule {
    private static FhMethodHandle<FUN_00a594c0> _FUN_00a594c0;
    private static FhMethodHandle<FUN_00a5a640> _FUN_00a5a640;

    private static FhMethodHandle<PrintfVarargDelegate> _printf_22F6B0;
    private static FhMethodHandle<PrintfVarargDelegate> _printf_22FDA0;
    private static FhMethodHandle<PrintfVarargDelegate> _printf_473C10;
    private static FhMethodHandle<PrintfVarargDelegate> _printf_473C20;

    public void init_hooks() {
        const string game = "FFX.exe";

        // Sphere Grid Editor
        _FUN_00a594c0 = new FhMethodHandle<FUN_00a594c0>(this, game, render_sphere_grid,     offset: 0x6594c0);
        _FUN_00a5a640 = new FhMethodHandle<FUN_00a5a640>(this, game, update_node_type_early, offset: 0x65a640);

        _printf_22F6B0 = new(this, "FFX.exe", h_printf_ansi, offset: 0x22F6B0);
        _printf_22FDA0 = new(this, "FFX.exe", h_printf_ansi, offset: 0x22FDA0);
        _printf_473C10 = new(this, "FFX.exe", h_printf_ansi, offset: 0x473C10);
        _printf_473C20 = new(this, "FFX.exe", h_printf_ansi, offset: 0x473C20);
    }

    public bool hook() {
        return _FUN_00a594c0.hook()
            && _FUN_00a5a640.hook();
    }

    private static void render_sphere_grid(u8* text, i32 p2, i32 p3) {
        _FUN_00a594c0.orig_fptr(text, p2, p3);

        //SphereGridEditor.render();
    }

    private static void update_node_type_early(i32 new_node_type, i32 node_idx) {
        _FUN_00a5a640.orig_fptr(new_node_type, node_idx);

        SphereGridEditor.update_node_type();
    }

    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static void h_printf_ansi(string fmt, nint va0, nint va1, nint va2, nint va3, nint va4, nint va5, nint va6, nint va7, nint va8, nint va9, nint va10, nint va11, nint va12, nint va13, nint va14, nint va15) {
        int argc = 0;
        fmt = fmt.Trim();

        for (int i = 0; i < fmt.Length; i++) if (fmt[i] == '%') argc++;

        int bl = argc switch {
            0  => PInvoke._scprintf(fmt, __arglist()),
            1  => PInvoke._scprintf(fmt, __arglist(va0)),
            2  => PInvoke._scprintf(fmt, __arglist(va0, va1)),
            3  => PInvoke._scprintf(fmt, __arglist(va0, va1, va2)),
            4  => PInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3)),
            5  => PInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4)),
            6  => PInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5)),
            7  => PInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6)),
            8  => PInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7)),
            9  => PInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8)),
            10 => PInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9)),
            11 => PInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10)),
            12 => PInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11)),
            13 => PInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12)),
            14 => PInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12, va13)),
            15 => PInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12, va13, va14)),
            16 => PInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12, va13, va14, va15)),
            _  => throw new Exception("FH_E_PFHOOK_RAH_OVERREACH")
        };

        nint buf = Marshal.AllocHGlobal(bl + 1);

        try {
            int rv = argc switch {
                0  => PInvoke.sprintf(buf, fmt, __arglist()),
                1  => PInvoke.sprintf(buf, fmt, __arglist(va0)),
                2  => PInvoke.sprintf(buf, fmt, __arglist(va0, va1)),
                3  => PInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2)),
                4  => PInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3)),
                5  => PInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4)),
                6  => PInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5)),
                7  => PInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6)),
                8  => PInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7)),
                9  => PInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8)),
                10 => PInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9)),
                11 => PInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10)),
                12 => PInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11)),
                13 => PInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12)),
                14 => PInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12, va13)),
                15 => PInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12, va13, va14)),
                16 => PInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12, va13, va14, va15)),
                _  => throw new Exception("FH_E_PFHOOK_RAH_OVERREACH")
            };

            FhLog.Info(Marshal.PtrToStringAnsi(buf) ?? "FH_E_PFHOOK_STRING_NUL");
        }
        finally {
            Marshal.FreeHGlobal(buf);
        }
    }
}
