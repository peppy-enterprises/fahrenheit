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
        FhInternal.Log.Info($"{_owner.ModuleType} -> {typeof(T).FullName}");
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
///     Represents a method with signature <typeparamref name="T"/>. It may then be invoked or hooked.
/// </summary>
public ref struct FhMethodHandle<T> where T : Delegate {

    private readonly nint _ptr_target;

    /// <summary>
    ///     A pointer to the target function. By default, this includes all hooks.
    ///     <para/>
    ///     To execute only part of the function's call chain, use <see cref="chain_from(T)"/>.
    /// </summary>
    public T? fnptr;

    public FhMethodHandle(FhMethodLocation location) {
        if (location.try_resolve(out _ptr_target)) {
            fnptr = FhInternal.MethodTable.get_fnptr<T>(_ptr_target);
        }
    }

    /// <summary>
    ///     Retargets the handle to only execute hooks subsequent to the given <paramref name="hook"/>.
    /// </summary>
    public FhMethodHandle<T> chain_from(T hook) {
        fnptr = FhInternal.MethodTable.get_fnptr_chain(hook);
        return this;
    }

    /// <summary>
    ///     Attempts to insert the given <paramref name="hook"/> into the hook chain of the target method.
    /// </summary>
    public readonly bool hook(FhModule owner, T hook) {
        FhHookContext hook_info = new(owner, hook);

        return _ptr_target != 0 && FhInternal.MethodTable.fnptr_chain_add<T>(_ptr_target, hook_info);
    }
}

/// <summary>
///     Pairs a hook with its owner. A stack of these constitutes the complete hook chain of a method.
/// </summary>
internal sealed record FhHookContext(
    FhModule owner,
    Delegate fnptr);

/// <summary>
///     Pairs an original game method with its hook stack and auxiliary data required to track hook insertion.
/// </summary>
internal sealed class FhMethodContext {
    internal readonly Stack<FhHookContext> stack   = [];
    internal          bool                 tainted = false; // The target is locked for further modification.
}

/// <summary>
///     Keeps track of the global hook state of functions.
/// </summary>
internal sealed class FhMethodTable {

    private readonly Dictionary<nint,     Delegate>        _fnptrs     = []; // Any function -> cached delegate
    private readonly Dictionary<nint,     FhMethodContext> _methods    = []; // Original     -> all hooks (for debug/keep-alive)
    private readonly Dictionary<nint,     nint>            _chain_next = []; // Original     -> next chain insertion address
    private readonly Dictionary<Delegate, nint>            _chain      = []; // Hook         -> next function in chain

    private          int  _lock_commit = 0;
    private readonly Lock _lock_chains = new Lock();

    /// <summary>
    ///     Caches a delegate for the function of type <typeparamref name="T"/>
    ///     at <paramref name="ptr_target"/>, or returns the cached one if it already exists.
    /// </summary>
    public T get_fnptr<T>(nint ptr_target) where T : Delegate {
        if (_fnptrs.TryGetValue(ptr_target, out Delegate? fnptr) && fnptr is T t_fnptr)
            return t_fnptr;

        t_fnptr = Marshal.GetDelegateForFunctionPointer<T>(ptr_target);
        _fnptrs[ptr_target] = t_fnptr;
        return t_fnptr;
    }

    /* [fkelava 04/06/26 23:28]
     * Locking should not be required because _chain_next is only manipulated
     * in a function under lock, and chain_from() which reads _chain is only
     * valid in contexts where no further hooks may be inserted.
     */

    /// <summary>
    ///     For the function at <paramref name="ptr_target"/>, obtains the address at which
    ///     the next function in the chain must be inserted.
    /// </summary>
    public nint get_fnptr_chain_next(nint ptr_target) {
        return _chain_next.TryGetValue(ptr_target, out nint ptr_next)
            ? ptr_next
            : ptr_target;
    }

    /// <summary>
    ///     For a given <paramref name="hook"/>, obtains the next link in its hook chain (if any exists).
    /// </summary>
    public T? get_fnptr_chain<T>(T hook) where T : Delegate {
        return _chain.TryGetValue(hook, out nint chain_fnptr)
            ? get_fnptr<T>(chain_fnptr)
            : null;
    }

    /* [fkelava 02/06/26 18:55]
     * MinHook creates a problem for us here; it will not install two hooks for the same function.
     *
     * Given MH_CreateHook(pTarget, pDetour, &ppOriginal), we can sequence `h1`, `h2` and `h3` over a function `f` as such:
     * > MH_CreateHook(&f,             &h1, &trampoline_f);
     * > MH_CreateHook(&trampoline_f,  &h2, &trampoline_h1);
     * > MH_CreateHook(&trampoline_h1, &h3, &trampoline_h2);
     *
     * Execution follows insertion order. Earlier hooks can pre-empt later ones.
     * This goes directly against _our_ LIFO load order where we want subsequent hooks to take priority.
     *
     * One way of proceeding would be to unwind and reapply the entire hook chain, but I could not get it to work.
     * I assume this is due to https://github.com/TsudaKageyu/minhook/issues/78#issuecomment-485101354.
     *
     * Thus we impose the following rules:
     * - Hooks inserted at `init` time are queued for application.
     * - Hooks are inserted in the proper order after all modules have initialized.
     * - Hook insertion after `init` is prohibited over a function that already has any.
     * - Hooks inserted after `init` revert to executing in insertion order.
     */

    /// <summary>
    ///     Attempts to insert a given <paramref name="hook"/>
    ///     into the chain of the function at <paramref name="ptr_target"/>.
    /// </summary>
    /// <remarks><see cref="_lock_chains" /> must be held by the caller.</remarks>
    private bool fnptr_chain_insert<T>(nint ptr_target, T hook) where T : Delegate {
        // _lock_chains must be held by this method's caller.
        nint pDetour;
        nint pTarget    = get_fnptr_chain_next(ptr_target);
        nint ppOriginal = 0;

        try {
            pDetour = Marshal.GetFunctionPointerForDelegate(hook);
        }
        catch (Exception e) {
            FhInternal.Log.Error(e.ToString());
            return false;
        }

        // SAFETY: &ppOriginal is used as an out parameter and stack allocated
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

        FhInternal.Log.Info($"(0x{ptr_target:X}) -> {hook.Method.Name}");
        return true;
    }

    /// <inheritdoc cref="fnptr_chain_insert{T}(nint, T)" />
    public bool fnptr_chain_add<T>(nint ptr_target, FhHookContext hook) where T : Delegate {
        lock (_lock_chains) {
            if (_methods.TryGetValue(ptr_target, out FhMethodContext? target)) {
                if (target.tainted) {
                    FhInternal.Log.Error($"(0x{ptr_target:X}) - rejected late insertion of {hook.fnptr.Method.Name}");
                    return false;
                }

                target.stack.Push(hook);
                return Interlocked.CompareExchange(ref _lock_commit, 0, 0) == 0 || fnptr_chain_insert(ptr_target, hook.fnptr);
            }

            target = new();
            target.stack.Push(hook);

            _methods[ptr_target] = target;
            return Interlocked.CompareExchange(ref _lock_commit, 0, 0) == 0 || fnptr_chain_insert(ptr_target, hook.fnptr);
        }
    }

    /// <summary>
    ///     Applies all hooks registered at the time of calling and
    ///     prohibits further insertion over functions with any hooks registered.
    /// </summary>
    public bool commit() {
        lock (_lock_chains) {
            if (Interlocked.CompareExchange(ref _lock_commit, 1, 0) == 1)
                return true; // reject repeat calls

            foreach ((nint target_ptr, FhMethodContext target) in _methods) {
                Stack<FhHookContext> target_stack = target.stack;

                foreach (FhHookContext hook in target_stack) {
                    if (!fnptr_chain_insert(target_ptr, hook.fnptr))
                        return false;
                }

                target.tainted = true;
            }
        }

        return true;
    }

}
