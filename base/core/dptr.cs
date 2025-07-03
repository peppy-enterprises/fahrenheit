namespace Fahrenheit.Core;

public enum FhStringType {
    Uni  = 1,
    UTF8 = 2,
    Ansi = 3
}

public record struct FhPointerDeref(nint Offset, bool AsPtr);

/// <summary>
///     A small helper structure for reading from and writing to pointers.
/// </summary>
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
     * FhPointer only operates correctly on blittable types. Blittable types have an identical representation
     * in unmanaged and managed code. https://learn.microsoft.com/en-us/dotnet/framework/interop/blittable-and-non-blittable-types
     *
     * There is no definitive way to know if a type is blittable short of following the spec.
     * This simply checks for obvious cases like Win32 BOOL vs. .NET 'bool'. Do not assume validity just because this passes.
     */
    private static void throw_if_type_parameter_invalid<T>() where T : unmanaged {
        if (Marshal.SizeOf<T>() != Unsafe.SizeOf<T>()) throw new Exception($"FH_E_DPTR_TYPE_NOT_BLITTABLE: {typeof(T).FullName}");
    }

    public static IEnumerable<nint> get_pending_wait_addresses() {
        lock (_pending_waits_lock) { // I'm not entirely sure the lock is necessary, but better safe than sorry...
            foreach (nint addr in _pending_waits) yield return addr;
        }
    }

    public bool deref_offsets(out nint vptr) {
        vptr = nint.Zero;

        foreach (FhPointerDeref deref in _derefs) {
            vptr = deref.AsPtr ? Marshal.ReadIntPtr(vptr + deref.Offset) : vptr + deref.Offset;
            if (vptr == nint.Zero) return false;
        }

        return true;
    }

    public T deref_primitive<T>() where T : unmanaged {
        throw_if_type_parameter_invalid<T>();
        return deref_offsets(out nint vptr) ? Unsafe.Read<T>(vptr.ToPointer()) : default;
    }

    public T[] deref_primitive<T>(int length) where T : unmanaged {
        throw_if_type_parameter_invalid<T>();
        return deref_offsets(out nint vptr) ? new ReadOnlySpan<T>(vptr.ToPointer(), length).ToArray() : [];
    }

    public void write_primitive<T>(T value) where T : unmanaged {
        throw_if_type_parameter_invalid<T>();
        if (!deref_offsets(out nint vptr)) return;
        Unsafe.Write(vptr.ToPointer(), value);
    }

    public void write_primitive<T>(T[] values) where T : unmanaged {
        throw_if_type_parameter_invalid<T>();
        if (!deref_offsets(out nint vptr)) return;
        values.CopyTo(new Span<T>(vptr.ToPointer(), values.Length));
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
