using System;
using System.Collections.Generic;

namespace Fahrenheit.Core;

internal static class FhMethodAddressRegistry {
    private static readonly Dictionary<nint, nint> _method_addr_map;
    private static readonly object                 _method_addr_map_lock;

    static FhMethodAddressRegistry() {
        _method_addr_map      = new Dictionary<nint, nint>();
        _method_addr_map_lock = new object();
    }

    public static nint Get(nint fn_addr_initial) {
        lock (_method_addr_map_lock) {
            return _method_addr_map.TryGetValue(fn_addr_initial, out nint fn_addr_current)
                ? fn_addr_current
                : fn_addr_initial;
        }
    }

    public static void Set(nint fn_addr_initial, nint fn_addr_new) {
        lock (_method_addr_map_lock) {
            _method_addr_map[fn_addr_initial] = fn_addr_new;
        }
    }
}

public unsafe class FhMethodHandle<T> where T : Delegate {
    private readonly nint fn_addr;

    public FhModule handle_owner { get; init;        }
    public T        orig_fptr    { get; private set; } // Do not store the value of this field.
    public T        hook_fptr    { get; init;        }

    public FhMethodHandle(FhModule owner,
                          string   module_name,
                          T        hook,
                          nint     offset  = int.MaxValue,
                          string?  fn_name = default)
    {
        fn_addr      = calc_initial_fnaddr_or_throw(module_name, offset, fn_name);
        handle_owner = owner;
        orig_fptr    = Marshal.GetDelegateForFunctionPointer<T>(FhMethodAddressRegistry.Get(fn_addr));
        hook_fptr    = hook;
    }

    public FhMethodHandle(FhModule owner,
                          nint     abs_fnaddr,
                          T        hook)
    {
        fn_addr      = abs_fnaddr;
        handle_owner = owner;
        orig_fptr    = Marshal.GetDelegateForFunctionPointer<T>(fn_addr);
        hook_fptr    = hook;
    }

    private nint calc_initial_fnaddr_or_throw(string module_name, nint offset, string? fn_name) {
        nint mod_addr = FhPInvoke.GetModuleHandle(module_name);
        if (mod_addr == 0) throw new Exception("FH_E_METHOD_HANDLE_GETMODULEHANDLE_FAILED");

        nint fn_addr_initial = fn_name == null
            ? mod_addr + offset
            : FhPInvoke.GetProcAddress(mod_addr, fn_name);
        if (fn_addr_initial == 0) throw new Exception("FH_E_METHOD_HANDLE_GETPROCADDR_FAILED");

        return fn_addr_initial;
    }

    public bool hook() {
        nint target_addr   = FhMethodAddressRegistry.Get(fn_addr);
        nint hook_addr     = Marshal.GetFunctionPointerForDelegate(hook_fptr);
        nint original_addr = 0;

        FhLog.Info($"Hook {hook_fptr.Method.Name}; original -> 0x{target_addr.ToString("X8")}, hook -> 0x{hook_addr.ToString("X8")}.");

        if (FhPInvoke.MH_CreateHook(target_addr, hook_addr, &original_addr) != 0) throw new Exception("FH_E_NATIVE_HOOK_CREATE_FAILED");
        if (FhPInvoke.MH_EnableHook(target_addr)                            != 0) throw new Exception("FH_E_NATIVE_HOOK_ENABLE_FAILED");

        orig_fptr = Marshal.GetDelegateForFunctionPointer<T>(original_addr);
        FhMethodAddressRegistry.Set(fn_addr, original_addr);

        return true;
    }
}
