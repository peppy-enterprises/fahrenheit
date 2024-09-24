using System;
using System.Diagnostics.CodeAnalysis;

namespace Fahrenheit.CoreLib;

public class FhMethodHandle<T> where T : Delegate {
    public FhModule handle_owner { get; init;        }
    public nint     fn_addr      { get; private set; }
    public T        orig_fptr    { get; private set; } // Do not store the value of this field; it can and will change between hook() calls.
    public T        hook_fptr    { get; init;        }

    public FhMethodHandle(FhModule owner,
                          string   module_name,
                          T        hook,
                          nint     offset  = int.MaxValue,
                          string?  fn_name = default)
    {
        calc_fnaddr_or_throw(module_name, offset, fn_name);

        handle_owner = owner;
        orig_fptr    = Marshal.GetDelegateForFunctionPointer<T>(fn_addr);
        hook_fptr    = hook;
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

    private void calc_fnaddr_or_throw(string module_name, nint offset, string? fn_name) {
        nint mod_addr = FhPInvoke.GetModuleHandle(module_name);
        if (mod_addr == 0) throw new Exception("FH_E_METHOD_HANDLE_MODULE_UNKNOWN");

        fn_addr = fn_name == null
            ? mod_addr + offset
            : FhPInvoke.GetProcAddress(mod_addr, fn_name);
        if (fn_addr == 0) throw new Exception("FH_E_METHOD_HANDLE_GETPROCADDR_FAILED");
    }

    public bool hook() {
        nint origAddr = fn_addr;
        nint hookAddr = Marshal.GetFunctionPointerForDelegate(hook_fptr);
        FhLog.Log(LogLevel.Info, $"Applying hook {hook_fptr.Method.Name} to address 0x{fn_addr.ToString("X8")}.");

        if (FhPInvoke.MH_CreateHook(fn_addr, hookAddr, ref origAddr) != 0) throw new Exception("FH_E_NATIVE_HOOK_CREATE_FAILED");
        if (FhPInvoke.MH_EnableHook(fn_addr)                         != 0) throw new Exception("FH_E_NATIVE_HOOK_ENABLE_FAILED");

        orig_fptr = Marshal.GetDelegateForFunctionPointer<T>(origAddr);
        FhLog.Log(LogLevel.Info, $"Hook {hook_fptr.Method.Name}; original -> 0x{fn_addr.ToString("X8")}, hook -> 0x{hookAddr.ToString("X8")}.");

        return true;
    }
}
