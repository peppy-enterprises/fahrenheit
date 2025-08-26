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
///     Used to target functions which are analogous but located at different offsets in FFX and FFX-2.
/// </summary>
public sealed record FhMethodLocation(
    nint OffsetX,
    nint OffsetX2);

/// <summary>
///     Provides access to a method whose signature is equal to <typeparamref name="T"/>.
///     You may then invoke or hook the function.
/// </summary>
public unsafe class FhMethodHandle<T> where T : Delegate {
    private readonly nint fn_addr;

    public FhModule handle_owner { get; init;        }
    public T        orig_fptr    { get; private set; } // Do not store the value of this field.
    public T        hook_fptr    { get; init;        }

    public FhMethodHandle(FhModule         owner,
                          FhMethodLocation location,
                          T                hook)
    {
        bool is_ffx = FhGlobal.game_type == FhGameType.FFX;

        string module_name = is_ffx ? "FFX.exe"        : "FFX-2.exe";
        nint   offset      = is_ffx ? location.OffsetX : location.OffsetX2;

        fn_addr      = calc_fnaddr(module_name, offset);
        handle_owner = owner;
        orig_fptr    = Marshal.GetDelegateForFunctionPointer<T>(FhInternal.MethodAddressMap.get(fn_addr));
        hook_fptr    = hook;
    }

    public FhMethodHandle(FhModule owner,
                          string   module_name,
                          string   fn_name,
                          T        hook)
    {
        fn_addr      = calc_fnaddr(module_name, fn_name);
        handle_owner = owner;
        orig_fptr    = Marshal.GetDelegateForFunctionPointer<T>(FhInternal.MethodAddressMap.get(fn_addr));
        hook_fptr    = hook;
    }

    public FhMethodHandle(FhModule owner,
                          string   module_name,
                          nint     offset,
                          T        hook)
    {
        fn_addr      = calc_fnaddr(module_name, offset);
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

    private nint calc_fnaddr(string module_name, string fn_name) {
        nint mod_addr = FhPInvoke.GetModuleHandle(module_name);
        if (mod_addr == 0) throw new Exception($"Module {module_name} not loaded in memory.");

        nint fn_addr  = NativeLibrary.GetExport(mod_addr, fn_name);
        if (fn_addr == 0) throw new Exception($"Method {fn_name} not found in module {module_name}");

        return fn_addr;
    }

    private nint calc_fnaddr(string module_name, nint offset) {
        nint mod_addr = FhPInvoke.GetModuleHandle(module_name);
        if (mod_addr == 0) throw new Exception($"Module {module_name} not loaded in memory.");

        return mod_addr + offset;
    }

    public bool hook() {
        nint addr_target   = FhInternal.MethodAddressMap.get(fn_addr);
        nint addr_hook     = Marshal.GetFunctionPointerForDelegate(hook_fptr);
        nint addr_original = 0;

        FhInternal.Log.Info($"{hook_fptr.Method.Name}; 0x{addr_target.ToString("X8")} -> 0x{addr_hook.ToString("X8")}.");

        if (FhPInvoke.MH_CreateHook(addr_target, addr_hook, &addr_original) != 0) throw new Exception("FH_E_NATIVE_HOOK_CREATE_FAILED");
        if (FhPInvoke.MH_EnableHook(addr_target)                            != 0) throw new Exception("FH_E_NATIVE_HOOK_ENABLE_FAILED");

        orig_fptr = Marshal.GetDelegateForFunctionPointer<T>(addr_original);
        FhInternal.MethodAddressMap.set(fn_addr, addr_original);

        return true;
    }
}
