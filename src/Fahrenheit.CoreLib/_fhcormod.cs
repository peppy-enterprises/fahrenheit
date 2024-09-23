using System;
using System.Diagnostics.CodeAnalysis;

using static Fahrenheit.CoreLib.FhHookDelegates;

namespace Fahrenheit.CoreLib;

public sealed record FhCoreModuleConfig : FhModuleConfig {
    public FhCoreModuleConfig(string configName,
                              bool   configEnabled) : base(configName, configEnabled) {
    }

    public override bool TrySpawnModule([NotNullWhen(true)] out FhModule? fm) {
        fm = new FhCoreModule(this);
        return fm.ModuleState == FhModuleState.InitSuccess;
    }
}

public unsafe class FhCoreModule : FhModule {
    private readonly FhCoreModuleConfig                   _moduleConfig;
    private readonly FhMethodHandle<Sg_MainLoop>          _mainLoop;
    private readonly FhMethodHandle<PrintfVarargDelegate> _printf_22F6B0;
    private readonly FhMethodHandle<PrintfVarargDelegate> _printf_22FDA0;
    private readonly FhMethodHandle<PrintfVarargDelegate> _printf_473C10;
    private readonly FhMethodHandle<PrintfVarargDelegate> _printf_473C20;
    private          nint                                 _o_WndProcPtr;
    private readonly WndProcDelegate                      _h_WndProc;
    private          nint                                 _h_WndProcPtr;
    private          bool                                 _wndProc_init;

    public FhCoreModule(FhCoreModuleConfig cfg) : base(cfg) {
        _moduleConfig  = cfg;
        _mainLoop      = new FhMethodHandle<Sg_MainLoop>         (this, "FFX.exe", new Sg_MainLoop         (main_loop)    , offset: 0x420C00);
        _printf_22F6B0 = new FhMethodHandle<PrintfVarargDelegate>(this, "FFX.exe", new PrintfVarargDelegate(h_printf_ansi), offset: 0x22F6B0);
        _printf_22FDA0 = new FhMethodHandle<PrintfVarargDelegate>(this, "FFX.exe", new PrintfVarargDelegate(h_printf_ansi), offset: 0x22FDA0);
        _printf_473C10 = new FhMethodHandle<PrintfVarargDelegate>(this, "FFX.exe", new PrintfVarargDelegate(h_printf_ansi), offset: 0x473C10);
        _printf_473C20 = new FhMethodHandle<PrintfVarargDelegate>(this, "FFX.exe", new PrintfVarargDelegate(h_printf_ansi), offset: 0x473C20);
        _h_WndProc     = new WndProcDelegate(h_wndproc);
        _moduleState   = FhModuleState.InitSuccess;
    }

    public void main_loop(float delta) {
        _mainLoop.orig_fptr.Invoke(delta);

        foreach (nint addr in FhPointer.get_pending_wait_addresses()) {
            FhPInvoke.WakeByAddressAll(addr.ToPointer());
        }

        if (_wndProc_init) return;
        bool rv = try_hook_wndproc();
        _wndProc_init = true;
    }

    public nint h_wndproc(nint hWnd, uint msg, nint wParam, nint lParam) {
        if (_o_WndProcPtr == 0) throw new Exception("FH_E_OWNDPROC_NUL");
        return FhPInvoke.CallWindowProcW(_o_WndProcPtr, hWnd, msg, wParam, lParam);
    }

    private bool try_hook_wndproc() {
        _h_WndProcPtr = Marshal.GetFunctionPointerForDelegate(_h_WndProc);
        nint hWnd     = FhPInvoke.FindWindow(null, "FINAL FANTASY X"); // shitfuck hack

        if (hWnd == 0) {
            FhLog.Log(LogLevel.Error, "Failed in FindWindow.");
            return false;
        }

        _o_WndProcPtr = FhPInvoke.GetWindowLongA(hWnd, FhPInvoke.GWLP_WNDPROC);
        FhPInvoke.SetWindowLongA(hWnd, FhPInvoke.GWLP_WNDPROC, _h_WndProcPtr);

        if (_o_WndProcPtr == 0) {
            FhLog.Log(LogLevel.Error, "Failed in GetWindowLongA.");
            return false;
        }

        return true;
    }

    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    public static void h_printf_ansi(string fmt, nint va0, nint va1, nint va2, nint va3, nint va4, nint va5, nint va6, nint va7, nint va8, nint va9, nint va10, nint va11, nint va12, nint va13, nint va14, nint va15) {
        int argc = 0;
        fmt = fmt.Trim();

        for (int i = 0; i < fmt.Length; i++) if (fmt[i] == '%') argc++;

        int bl = argc switch {
            0  => FhPInvoke._scprintf(fmt, __arglist()),
            1  => FhPInvoke._scprintf(fmt, __arglist(va0)),
            2  => FhPInvoke._scprintf(fmt, __arglist(va0, va1)),
            3  => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2)),
            4  => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3)),
            5  => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4)),
            6  => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5)),
            7  => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6)),
            8  => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7)),
            9  => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8)),
            10 => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9)),
            11 => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10)),
            12 => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11)),
            13 => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12)),
            14 => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12, va13)),
            15 => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12, va13, va14)),
            16 => FhPInvoke._scprintf(fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12, va13, va14, va15)),
            _  => throw new Exception("FH_E_PFHOOK_RAH_OVERREACH")
        };

        nint buf = Marshal.AllocHGlobal(bl + 1);

        try {
            int rv = argc switch {
                0  => FhPInvoke.sprintf(buf, fmt, __arglist()),
                1  => FhPInvoke.sprintf(buf, fmt, __arglist(va0)),
                2  => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1)),
                3  => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2)),
                4  => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3)),
                5  => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4)),
                6  => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5)),
                7  => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6)),
                8  => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7)),
                9  => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8)),
                10 => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9)),
                11 => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10)),
                12 => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11)),
                13 => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12)),
                14 => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12, va13)),
                15 => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12, va13, va14)),
                16 => FhPInvoke.sprintf(buf, fmt, __arglist(va0, va1, va2, va3, va4, va5, va6, va7, va8, va9, va10, va11, va12, va13, va14, va15)),
                _  => throw new Exception("FH_E_PFHOOK_RAH_OVERREACH")
            };

            FhLog.Info(Marshal.PtrToStringAnsi(buf) ?? "FH_E_PFHOOK_STRING_NUL");
        }
        finally {
            Marshal.FreeHGlobal(buf);
        }
    }

    public override bool FhModuleInit() {
        return _printf_22F6B0.hook() &&
               _printf_22FDA0.hook() &&
               _printf_473C10.hook() &&
               _printf_473C20.hook();
    }

    public override bool FhModuleOnError() {
        return true;
    }
}