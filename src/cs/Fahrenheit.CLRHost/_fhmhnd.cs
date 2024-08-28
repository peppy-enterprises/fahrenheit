using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Fahrenheit.CoreLib;

namespace Fahrenheit.CLRHost;

public class FhMethodHandle<T> where T : Delegate {
    private readonly FhModule _handle_owner;
    private readonly bool     _handle_valid;
    private readonly nint     _fn_addr;
    private          T?       _orig_fn;
    private readonly T        _hook_fn;

    public FhMethodHandle(FhModule owner,
                          string   module_name,
                          nint     offset,
                          T        hook_fn)
    {
        _handle_owner = owner;
        _handle_valid = calc_fnaddr(module_name, offset, out _fn_addr);
        _orig_fn      = _handle_valid ? Marshal.GetDelegateForFunctionPointer<T>(_fn_addr) : null;
        _hook_fn      = hook_fn;
    }

    public FhMethodHandle(FhModule owner,
                          string   module_name,
                          string   fn_name,
                          T        hook_fn)
    {
        _handle_owner = owner;
        _handle_valid = calc_fnaddr(module_name, fn_name, out _fn_addr);
        _orig_fn      = _handle_valid ? Marshal.GetDelegateForFunctionPointer<T>(_fn_addr) : null;
        _hook_fn      = hook_fn;
    }

    private bool calc_fnaddr(string module_name, nint offset, out nint fn_addr) {
        nint mod_addr  = FhPInvoke.GetModuleHandle(module_name);
        fn_addr        = mod_addr + offset;
        return mod_addr != 0;
    }

    private bool calc_fnaddr(string moduleName, string fn_name, out nint fn_addr) {
        fn_addr = FhPInvoke.GetProcAddress(FhPInvoke.GetModuleHandle(moduleName), fn_name);
        return fn_addr != 0;
    }

    public bool try_get_original_fptr([NotNullWhen(true)] out T? handle) { return (handle = _orig_fn) != null; }

    public bool hook()   { return _handle_valid && FhCLRHost.hook  (_fn_addr, _hook_fn, out _orig_fn); }
    public bool unhook() { return _handle_valid && FhCLRHost.unhook(_fn_addr, _hook_fn, out _orig_fn); }
}
