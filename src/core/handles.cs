// SPDX-License-Identifier: MIT

namespace Fahrenheit;

/// <summary>
///     Provides runtime binding to a <see cref="FhModule"/> of type <typeparamref name="T"/>.
///     You may then access the module or its <see cref="FhModuleContext"/>.
/// </summary>
public sealed class FhModuleHandle<T>(FhModule owner) where T : FhModule {
    private readonly FhModule         _owner = owner;
    private          FhModuleContext? _match;

    /// <summary>
    ///     Searches for a module of type <typeparamref name="T"/>,
    ///     caching the match if found, and returns its <see cref="FhModuleContext"/>.
    /// </summary>
    public bool try_get_context([NotNullWhen(true)] out FhModuleContext? target_context) {
        FhInternal.Log.Info($"{_owner.ModuleType} acquiring handle to {typeof(T).FullName}");
        return (target_context = (_match ??= FhApi.Mods.get_module<T>())) != null;
    }

    /// <summary>
    ///     Searches for a module of type <typeparamref name="T"/>,
    ///     caching the match if found, and returns it.
    /// </summary>
    public bool try_get_module([NotNullWhen(true)] out T? target) {
        target = default;
        return try_get_context(out FhModuleContext? target_context) && (target = target_context.Module as T) != null;
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
///     A helper to obtain the absolute address of a given function.
/// </summary>
public readonly ref struct FhMethodLocation {

    private readonly nint _ptr_target;

    /// <summary>
    ///     Use this constructor for functions which are analogous between FF X and X-2.
    ///     The handle implicitly targets the currently running game and selects the appropriate offset.
    /// </summary>
    public FhMethodLocation(nint offset_x, nint offset_x2) {
        bool is_ffx = FhGlobal.game_id == FhGameId.FFX;

        string module_name = is_ffx ? "FFX.exe" : "FFX-2.exe";
        nint   offset      = is_ffx ? offset_x  : offset_x2;

        _ptr_target = calc_addr(module_name, offset);
    }

    /// <summary>
    ///     Use this constructor for exported functions in external modules, such as D3D11.dll.
    /// </summary>
    public FhMethodLocation(string module_name, string fn_name) {
        _ptr_target = calc_addr(module_name, fn_name);
    }

    /// <summary>
    ///     Use this constructor for private/non-exported functions in external modules,
    ///     such as D3D11.dll, or functions exclusive to either FF X or X-2.
    /// </summary>
    public FhMethodLocation(string module_name, nint offset) {
        _ptr_target = calc_addr(module_name, offset);
    }

    /// <summary>
    ///     Use this constructor for member functions or vtable entries of objects, such as
    ///     <see cref="IDXGISwapChain.Present(uint, DXGI_PRESENT)"/>.
    ///     <para/>
    ///     Unlike other constructors, no validation is performed on the input address.
    /// </summary>
    public FhMethodLocation(nint abs_addr) {
        _ptr_target = abs_addr;
    }

    /// <inheritdoc cref="FhMethodLocation(nint)" />
    unsafe public FhMethodLocation(void* abs_addr) {
        _ptr_target = (nint)abs_addr;
    }

    /// <summary>
    ///     Obtains the absolute address of export <paramref name="fn_name"/>
    ///     in module <paramref name="module_name"/>.
    /// </summary>
    private static nint calc_addr(string module_name, string fn_name) {
        nint module_addr = FhPInvoke.GetModuleHandle(module_name);
        return module_addr != 0 && NativeLibrary.TryGetExport(module_addr, fn_name, out nint fn_addr)
            ? fn_addr
            : 0;
    }

    /// <summary>
    ///     Obtains the absolute address of the function at <paramref name="offset"/>
    ///     in module <paramref name="module_name"/>.
    /// </summary>
    private static nint calc_addr(string module_name, nint offset) {
        nint module_addr = FhPInvoke.GetModuleHandle(module_name);
        return module_addr != 0
            ? module_addr + offset
            : 0;
    }

    public bool try_resolve(out nint ptr_target) {
        return (ptr_target = _ptr_target) != 0;
    }
}

/// <summary>
///     Pairs a hook with its owner. A stack of these constitutes the complete hook chain of a method.
/// </summary>
internal sealed record FhHookContext(FhModule owner, Delegate hook);

/// <summary>
///     Represents a method with signature <typeparamref name="T"/>. You may then invoke or hook it.
/// </summary>
public ref struct FhMethodHandle<T> where T : Delegate {

    private readonly nint _ptr_target;

    /// <summary>
    ///     The function at the target address, including any hooks.
    /// </summary>
    public readonly T? fnptr;

    /// <summary>
    ///     The next function in the current hook chain.
    ///     Only valid from within a hook, and only after <see cref="chain_from(T)"/>.
    /// </summary>
    public T? fnptr_chain { get; private set; }

    public FhMethodHandle(FhMethodLocation location) {
        if (location.try_resolve(out _ptr_target)) {
            fnptr = FhInternal.MethodTable.get_fnptr<T>(_ptr_target);
        }
    }

    /// <summary>
    ///     Returns the next function in the chain of the given <paramref name="hook"/>, if any exists.
    /// </summary>
    public T? chain_from(T hook) {
        return fnptr_chain = FhInternal.MethodTable.get_fnptr_chain(hook);
    }

    /// <summary>
    ///     Attempts to insert the given <paramref name="hook"/> into the hook chain of the target method.
    /// </summary>
    public bool hook(FhModule owner, T hook) {
        FhHookContext hook_info = new(owner, hook);

        return _ptr_target != 0 && FhInternal.MethodTable.set_fnptr_chain<T>(_ptr_target, hook_info);
    }
}

/// <summary>
///     Keeps track of the global hook state of functions.
/// </summary>
internal sealed class FhMethodTable {

    private readonly Dictionary<nint,     Delegate>             _fnptrs     = []; // Original or hook -> cached delegate
    private readonly Dictionary<nint,     Stack<FhHookContext>> _hooks      = []; // Original         -> all hooks (for debug/keep-alive)
    private readonly Dictionary<nint,     nint>                 _chain_next = []; // Original         -> next chain insertion address
    private readonly Dictionary<Delegate, nint>                 _chain      = []; // Hook             -> next function in chain

    private readonly Lock _lock_chains = new Lock();

    /// <summary>
    ///     Caches a delegate for the function of type <typeparamref name="T"/> at <paramref name="ptr_target"/>,
    ///     or returns the cached delegate if one already exists.
    /// </summary>
    public T get_fnptr<T>(nint ptr_target) where T : Delegate {
        if (_fnptrs.TryGetValue(ptr_target, out Delegate? fnptr) && fnptr is T t_fnptr)
            return t_fnptr;

        t_fnptr = Marshal.GetDelegateForFunctionPointer<T>(ptr_target);
        _fnptrs[ptr_target] = t_fnptr;
        return t_fnptr;
    }

    /// <summary>
    ///     For the function at <paramref name="ptr_target"/>, obtains the address at which
    ///     the next function in the chain must be inserted.
    /// </summary>
    public nint get_fnptr_chain_insert(nint ptr_target) {
        lock (_lock_chains) {
            return _chain_next.TryGetValue(ptr_target, out nint addr_current)
                ? addr_current
                : ptr_target;
        }
    }

    /// <summary>
    ///     For a given <paramref name="hook"/>, obtains the next link in its hook chain (if any exists).
    /// </summary>
    public T? get_fnptr_chain<T>(T hook) where T : Delegate {
        lock (_lock_chains) {
            return _chain.TryGetValue(hook, out nint chain_fnptr)
                ? get_fnptr<T>(chain_fnptr)
                : null;
        }
    }

    /// <summary>
    ///     Inserts the hook located at <paramref name="ptr_chain_link"/> and described by
    ///     <paramref name="context"/> into the chain of the function at <paramref name="ptr_target"/>.
    /// </summary>
    public bool set_fnptr_chain<T>(nint ptr_target, FhHookContext context) where T : Delegate {
        lock (_lock_chains) {
            Delegate hook = context.hook;

            // Where do we have to insert the next hook for this chain?
            nint pTarget = get_fnptr_chain_insert(ptr_target);
            nint pDetour;

            try {
                pDetour = Marshal.GetFunctionPointerForDelegate(hook);
            }
            catch (Exception e) {
                FhInternal.Log.Error(e.ToString());
                return false;
            }

            nint ppOriginal = 0;

            unsafe {
                FhPInvoke.MH_STATUS rv_create = FhPInvoke.MH_CreateHook(pTarget, pDetour, &ppOriginal);

                if (rv_create != FhPInvoke.MH_STATUS.MH_OK) {
                    FhInternal.Log.Error($"MH_CreateHook() failed for {hook.Method.Name} - {rv_create}");
                    return false;
                }
            }

            FhPInvoke.MH_STATUS rv_enable = FhPInvoke.MH_EnableHook(pTarget);

            if (rv_enable != FhPInvoke.MH_STATUS.MH_OK) {
                FhInternal.Log.Error($"MH_EnableHook() failed for {hook.Method.Name} - {rv_enable}");
                return false;
            }

            _chain_next[ptr_target] = ppOriginal;
            _chain     [hook]       = ppOriginal;

            FhInternal.Log.Info($"(0x{ptr_target:X}) - inserted {hook.Method.Name}");

            if (_hooks.TryGetValue(ptr_target, out Stack<FhHookContext>? hooks)) {
                hooks.Push(context);
                return true;
            }

            hooks = [];
            hooks.Push(context);

            _hooks[ptr_target] = hooks;
            return true;
        }
    }

}
