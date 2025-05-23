namespace Fahrenheit.Core;

public enum FhStringType {
    Uni  = 1,
    UTF8 = 2,
    Ansi = 3
}

public record struct FhPointerDeref(nint Offset, bool AsPtr);

public unsafe readonly struct FhPointer {

    private readonly static List<nint>       _pending_waits;
    private readonly static object           _pending_waits_lock;
    private readonly        FhPointerDeref[] _derefs;

    static FhPointer() {
        _pending_waits      = new List<nint>(31);
        _pending_waits_lock = new object();
    }

    public FhPointer(FhPointerDeref[] derefs) {
        _derefs = derefs;
    }

    /* [fkelava 23/6/23 14:03]
     * Some one-liners require a hefty explanation. This one does. READ IT CAREFULLY.
     *
     * Unsafe.Read/Write<T> does not require a constraint. The constraint is there to make evident that
     * > there must be SizeOf<T>() bytes of readable memory available starting at the location pointed to {...}
     *
     * The SizeOf<T> in question is `Unsafe.SizeOf<T>`, which returns the size of the `managed` view of T.
     * > If T is a reference type, the return value is the size of the reference itself (sizeof(void*)) {...}
     *
     * Hence `where T : struct`. HOWEVER, additionally, the struct in question _must_ _exactly_
     * represent its unmanaged equivalent (or at least be blittable). In short,
     * `Marshal.SizeOf<T>` and `Unsafe.SizeOf<T>` must be equal. For instance:
     *
     * public struct TestStructA { public uint A; }
     * public struct TestStructB { [MarshalAs(UnmanagedType.Bool)] public bool A; }
     *
     * TestStructA is valid. An `uint` is blittable, 4 bytes in managed and unmanaged view.
     * TestStructB is invalid. A `bool` marshaled as Win32 BOOL is 1 byte managed, 4 bytes unmanaged. The request is invalid and Fahrenheit will crash the game before you do.
     */
    private static void throw_if_type_parameter_invalid<T>() where T : unmanaged {
        if (Marshal.SizeOf<T>() != Unsafe.SizeOf<T>()) throw new Exception($"FH_E_DPTR_UNSAFE_OPERATION: {typeof(T).FullName}");
    }

    public static IEnumerable<nint> get_pending_wait_addresses() {
        lock (_pending_waits_lock) { // I'm not entirely sure the lock is necessary, but better safe than sorry...
            foreach (nint addr in _pending_waits) yield return addr;
        }
    }

    private nint deref_offsets_internal() {
        nint ptr = nint.Zero;

        foreach (FhPointerDeref deref in _derefs) {
            ptr = deref.AsPtr ? Marshal.ReadIntPtr(ptr + deref.Offset) : ptr + deref.Offset;
            if (ptr == nint.Zero) return nint.MaxValue;
        }

        return ptr;
    }

    public bool deref_offsets(out nint vptr) {
        return (vptr = deref_offsets_internal()) != nint.MaxValue;
    }

    public T deref_primitive<T>() where T : unmanaged {
        throw_if_type_parameter_invalid<T>();
        return deref_offsets(out nint vptr) ? Unsafe.Read<T>(vptr.ToPointer()) : default;
    }

    public void write_primitive<T>(T value) where T : unmanaged {
        throw_if_type_parameter_invalid<T>();
        if (!deref_offsets(out nint vptr)) return;
        Unsafe.Write(vptr.ToPointer(), value);
    }

    public string deref_string(FhStringType strtype) {
        if (!deref_offsets(out nint vptr)) return string.Empty;

        return strtype switch {
            FhStringType.Uni  => Marshal.PtrToStringUni(vptr)!,
            FhStringType.UTF8 => Marshal.PtrToStringUTF8(vptr)!,
            FhStringType.Ansi => Marshal.PtrToStringAnsi(vptr)!,
            _                 => throw new Exception("FH_E_DPTR_UNDEFINED_STRTYPE")
        };
    }

    /* [fkelava 26/6/23 11:03]
     * Mandatory reading: https://devblogs.microsoft.com/oldnewthing/20180118-00/?p=97825,
     * https://learn.microsoft.com/en-us/windows/win32/api/synchapi/nf-synchapi-waitonaddress.
     */

    public bool try_await_value<T>(T target) where T : unmanaged {
        if (!deref_offsets(out nint vptr)) {
            FhInternal.Log.Warning($"AwaitValue was called but the supplied pointer resolved to nothing.");
            return false;
        }

        lock (_pending_waits_lock) { _pending_waits.Add(vptr); }

        void* maptr = vptr.ToPointer(); // Void pointer to monitored addr.
        T     maval = *(T*)maptr;       // Value at monitored addr at call start.

        if (maval.Equals(target))
            return true;

        T  cur    = maval;
        T* curptr = &cur;

        while (!cur.Equals(target)) {
            FhPInvoke.WaitOnAddress(curptr, maptr, sizeof(T), FhPInvoke.INFINITE);
            cur = *(T*)maptr;
        }

        lock (_pending_waits_lock) { return _pending_waits.Remove(vptr); }
    }

    public bool try_await_values<T>(in ReadOnlySpan<T> targets, out T match) where T : unmanaged {
        match = default;

        if (!deref_offsets(out nint vptr)) {
            FhInternal.Log.Warning($"AwaitValue was called but the supplied pointer resolved to nothing.");
            return false;
        }

        lock (_pending_waits_lock) { _pending_waits.Add(vptr); }

        void* maptr = vptr.ToPointer(); // `m`onitored `a`ddress pointer.
        T     maval = *(T*)maptr;       // `m`onitored `a`ddress value.

        foreach (T target in targets) {
            if (maval.Equals(target))
                return true;
        }

        T    cur        = maval;
        T*   curptr     = &cur;
        bool hasMatched = false;

        while (!hasMatched) {
            FhPInvoke.WaitOnAddress(curptr, maptr, sizeof(T), FhPInvoke.INFINITE);
            cur = *(T*)maptr;

            foreach (T target in targets) {
                if (hasMatched = cur.Equals(target)) {
                    match = target;
                    break;
                }
            }
        }

        lock (_pending_waits_lock) { return _pending_waits.Remove(vptr); }
    }
}
