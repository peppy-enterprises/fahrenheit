using System;
using System.Runtime.CompilerServices;

namespace Fahrenheit.CoreLib;

public enum FhStringType {
    Uni  = 1,
    UTF8 = 2,
    Ansi = 3
}

public record struct FhPtrDeref(nint offset, bool as_ptr);

public unsafe readonly struct FhPtr {
    private readonly FhPtrDeref[] _derefs;

    public FhPtr(FhPtrDeref[] derefs) {
        _derefs = derefs;
    }

    /* Make sure managed and managed size is the same for safety */
    private static void check_safety<T>() where T : unmanaged {
        if (Marshal.SizeOf<T>() != Unsafe.SizeOf<T>()) throw new Exception($"FH_E_DPTR_UNSAFE_OPERATION: {typeof(T).FullName}");
    }

    public bool try_calc(out nint vptr) {
        nint ptr = 0;

        foreach (FhPtrDeref deref in _derefs) {
            ptr = deref.as_ptr
                    ? Marshal.ReadIntPtr(ptr + deref.offset)
                    : ptr + deref.offset;
            if (ptr == 0) break;
        }

        return (vptr = ptr) != 0;
    }

    public T read<T>() where T : unmanaged {
        check_safety<T>();
        return try_calc(out nint vptr) ? Unsafe.Read<T>(vptr.ToPointer()) : default;
    }

    public void write<T>(T value) where T : unmanaged {
        check_safety<T>();
        if (!try_calc(out nint vptr)) return;
        Unsafe.Write(vptr.ToPointer(), value);
    }

    public string read_string(FhStringType strtype) {
        if (!try_calc(out nint vptr)) return String.Empty;

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

    public bool AwaitValue<T>(T target) where T : unmanaged {
        if (!try_calc(out nint vptr)) {
            FhLog.Warning($"AwaitValue was called but the supplied pointer resolved to nothing.");
            return false;
        }

        void* maptr = vptr.ToPointer(); // Void pointer to monitored addr.
        T     maval = *(T*)maptr;       // Value at monitored addr at call start.

        if (maval.Equals(target))
            return true;

        T  cur    = maval;
        T* curptr = &cur;

        while (!cur.Equals(target)) {
            FhPInvoke.WaitOnAddress(curptr, maptr, sizeof(T), 1);
            cur = *(T*)maptr;
        }

        return true;
    }

    public bool AwaitValues<T>(in ReadOnlySpan<T> targets, out T match) where T : unmanaged {
        match = default;

        if (!try_calc(out nint vptr)) {
            FhLog.Warning($"AwaitValue was called but the supplied pointer resolved to nothing.");
            return false;
        }

        void* maptr = vptr.ToPointer(); // `m`onitored `a`ddress pointer.
        T     maval = *(T*)maptr;       // `m`onitored `a`ddress value.

        foreach (T target in targets) {
            if (maval.Equals(target))
                return true;
        }

        T    cur    = maval;
        T*   curptr = &cur;
        bool hasMatched  = false;

        while (!hasMatched) {
            FhPInvoke.WaitOnAddress(curptr, maptr, sizeof(T), 1);
            cur = *(T*)maptr;

            foreach (T target in targets) {
                if (hasMatched = cur.Equals(target)) {
                    match = target;
                    break;
                }
            }
        }

        return true;
    }
}