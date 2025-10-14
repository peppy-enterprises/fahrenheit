// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core;

/// <summary>
///     Provides runtime binding to a <see cref="FhModule"/> of type <typeparamref name="TTarget"/>.
///     You may then access its <see cref="FhModuleContext"/>.
/// </summary>
public class FhModuleHandle<TTarget>(FhModule owner) where TTarget : FhModule {
    private readonly FhModule         _owner = owner;
    private          FhModuleContext? _match;

    /// <summary>
    ///     Queries the <see cref="FhModController"/> for a module of type <typeparamref name="TTarget"/>,
    ///     caching the match if found, and returns its <see cref="FhModuleContext"/>.
    /// </summary>
    public bool try_get([NotNullWhen(true)] out FhModuleContext? target_context) {
        FhInternal.Log.Info($"{_owner.ModuleType} acquiring handle to {typeof(TTarget).FullName}");
        return (target_context = (_match ??= FhApi.Mods.get_module<TTarget>())) != null;
    }
}

/// <summary>
///     Represents an object of type <typeparamref name="T"/> initialized at runtime.
/// </summary>
internal sealed class FhRuntimeHandle<T> {
    private readonly Lock _impl_lock = new Lock();
    private          T?   _impl;

    public bool get_impl([NotNullWhen(true)] out T? impl) {
        lock (_impl_lock) {
            return (impl = _impl) != null;
        }
    }

    public void set_impl(T impl) {
        lock (_impl_lock) {
            FhInternal.Log.Info(typeof(T).Name);
            _impl = impl;
        }
    }
}

/// <summary>
///     Represents a method whose signature is equal to <typeparamref name="T"/>.
///     You may then invoke or hook the function.
/// </summary>
public unsafe class FhMethodHandle<T> where T : Delegate {
    private readonly nint fn_addr;

    public FhModule handle_owner { get; init;        }
    public T        orig_fptr    { get; private set; } // Do not store the value of this field.
    public T        hook_fptr    { get; init;        }

    /// <summary>
    ///     This constructor is used for functions which are analogous between FF X and X-2.
    ///     The handle implicitly targets the currently running game and selects the appropriate
    ///     offset from <paramref name="location"/>.
    /// </summary>
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

    /// <summary>
    ///     This constructor is used for exported functions in external modules, such as D3D11.dll.
    /// </summary>
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

    /// <summary>
    ///     This constructor is used for private/non-exported functions in external modules, such as D3D11.dll,
    ///     or functions exclusive to either FF X or X-2.
    /// </summary>
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

    /// <summary>
    ///     This constructor is used for member functions or vtable entries of objects, such as
    ///     <see cref="TerraFX.Interop.DirectX.IDXGISwapChain.Present(uint, uint)"/>.
    /// </summary>
    public FhMethodHandle(FhModule owner,
                          nint     abs_fnaddr,
                          T        hook)
    {
        fn_addr      = abs_fnaddr;
        handle_owner = owner;
        orig_fptr    = Marshal.GetDelegateForFunctionPointer<T>(fn_addr);
        hook_fptr    = hook;
    }

    /// <summary>
    ///     Obtains the absolute address of export <paramref name="fn_name"/>
    ///     in module <paramref name="module_name"/>.
    /// </summary>
    private nint calc_fnaddr(string module_name, string fn_name) {
        nint mod_addr = FhPInvoke.GetModuleHandle(module_name);
        if (mod_addr == 0) throw new Exception($"Module {module_name} not loaded in memory.");

        nint fn_addr = NativeLibrary.GetExport(mod_addr, fn_name);
        if (fn_addr == 0) throw new Exception($"Method {fn_name} not found in module {module_name}");

        return fn_addr;
    }

    /// <summary>
    ///     Obtains the absolute address of the function at <paramref name="offset"/>
    ///     in module <paramref name="module_name"/>.
    /// </summary>
    private nint calc_fnaddr(string module_name, nint offset) {
        nint mod_addr = FhPInvoke.GetModuleHandle(module_name);
        if (mod_addr == 0) throw new Exception($"Module {module_name} not loaded in memory.");

        return mod_addr + offset;
    }

    /// <summary>
    ///     Inserts the hook specified at construction time into the hook chain of the targeted method.
    /// </summary>
    public bool hook() {
        nint addr_target   = FhInternal.MethodAddressMap.get(fn_addr);
        nint addr_hook     = Marshal.GetFunctionPointerForDelegate(hook_fptr);
        nint addr_original = 0;

        FhInternal.Log.Info($"{hook_fptr.Method.Name}; 0x{addr_target:X8} -> 0x{addr_hook:X8}.");

        if (FhPInvoke.MH_CreateHook(addr_target, addr_hook, &addr_original) != 0) throw new Exception("FH_E_NATIVE_HOOK_CREATE_FAILED");
        if (FhPInvoke.MH_EnableHook(addr_target)                            != 0) throw new Exception("FH_E_NATIVE_HOOK_ENABLE_FAILED");

        orig_fptr = Marshal.GetDelegateForFunctionPointer<T>(addr_original);
        FhInternal.MethodAddressMap.set(fn_addr, addr_original);

        return true;
    }
}

/// <summary>
///     Tracks the address at which the next hook requested by a <see cref="FhMethodHandle{T}"/>
///     must be inserted, if multiple hooks are created for the same method.
/// </summary>
internal class FhMethodAddressMap {
    private readonly Dictionary<nint, nint> _map  = [];
    private readonly Lock                   _lock = new Lock();

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
///     Used by <see cref="FhMethodHandle{T}"/> to target functions which are
///     analogous but located at different offsets in FF X and FF X-2.
/// </summary>
public sealed record FhMethodLocation(
    nint OffsetX,
    nint OffsetX2);
