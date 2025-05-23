using System.Threading;

namespace Fahrenheit.Core;

/// <summary>
///     Internally tracks the address at which the next hook requested by a <see cref="FhMethodHandle{T}"/>
///     must be inserted, if multiple hooks are created for the same method.
/// </summary>
internal class FhMethodAddressMap {
    private readonly Dictionary<nint, nint> _map;
    private readonly Lock                   _lock;

    public FhMethodAddressMap() {
        _map  = [];
        _lock = new Lock();
    }

    public nint get(nint addr_initial) {
        lock (_lock) {
            return _map.TryGetValue(addr_initial, out nint addr_current)
                ? addr_current
                : addr_initial;
        }
    }

    public void set(nint addr_initial, nint addr_new) {
        lock (_lock) {
            _map[addr_initial] = addr_new;
        }
    }
}

/// <summary>
///     Provides access to a method whose signature is equal to <typeparamref name="T"/>.
///     You may then invoke or hook the function.
/// </summary>
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
        orig_fptr    = Marshal.GetDelegateForFunctionPointer<T>(FhInternal.MethodAddressMap.get(fn_addr));
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
        nint addr_target   = FhInternal.MethodAddressMap.get(fn_addr);
        nint addr_hook     = Marshal.GetFunctionPointerForDelegate(hook_fptr);
        nint addr_original = 0;

        FhInternal.Log.Info($"Hook {hook_fptr.Method.Name}; original -> 0x{addr_target.ToString("X8")}, hook -> 0x{addr_hook.ToString("X8")}.");

        if (FhPInvoke.MH_CreateHook(addr_target, addr_hook, &addr_original) != 0) throw new Exception("FH_E_NATIVE_HOOK_CREATE_FAILED");
        if (FhPInvoke.MH_EnableHook(addr_target)                            != 0) throw new Exception("FH_E_NATIVE_HOOK_ENABLE_FAILED");

        orig_fptr = Marshal.GetDelegateForFunctionPointer<T>(addr_original);
        FhInternal.MethodAddressMap.set(fn_addr, addr_original);

        return true;
    }
}
