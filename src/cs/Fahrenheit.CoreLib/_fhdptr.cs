using System;
using System.Runtime.CompilerServices;

namespace Fahrenheit.CoreLib;

public enum FhStringType {
    Uni  = 1,
    UTF8 = 2,
    Ansi = 3
}

public record struct FhPointerDeref(nint Offset, bool AsPtr);

/* [fkelava 23/6/23 13:12]
 * Any use of FhPointer is _inherently_ unsafe in the sense that you will always be subject to possible AVs.
 * Since we have no control over execution, a pointer can become invalid in the instant _after_ internal validation
 * and _before_ a e.g. string deref. Hence, you MUST appropriately guard FhPointer operations lest you crash the game.
 */
public unsafe readonly struct FhPointer {
    private readonly FhPointerDeref[] _derefs;

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
    private static void ThrowIfTInvalid<T>() where T : unmanaged {
        if (Marshal.SizeOf<T>() != Unsafe.SizeOf<T>()) throw new Exception($"FH_E_DPTR_UNSAFE_OPERATION: {typeof(T).FullName}");
    }

    private nint DerefOffsetsInternal() {
        nint ptr = nint.Zero;

        foreach (FhPointerDeref deref in _derefs) {
            ptr = deref.AsPtr ? Marshal.ReadIntPtr(ptr + deref.Offset) : ptr + deref.Offset;
            if (ptr == nint.Zero) return nint.MaxValue;
        }

        return ptr;
    }

    public bool DerefOffsets(out nint vptr) {
        return (vptr = DerefOffsetsInternal()) != nint.MaxValue;
    }

    public T DerefPrimitive<T>() where T : unmanaged {
        ThrowIfTInvalid<T>();
        return DerefOffsets(out nint vptr) ? Unsafe.Read<T>(vptr.ToPointer()) : default;
    }

    public void WritePrimitive<T>(T value) where T : unmanaged {
        ThrowIfTInvalid<T>();
        if (!DerefOffsets(out nint vptr)) return;
        Unsafe.Write(vptr.ToPointer(), value);
    }

    public string DerefString(FhStringType strtype) {
        if (!DerefOffsets(out nint vptr)) return string.Empty;

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
        if (!DerefOffsets(out nint vptr)) {
            FhLog.Warning($"AwaitValue was called but the supplied pointer resolved to nothing.");
            return false;
        }

        void* maptr = vptr.ToPointer(); // Void pointer to monitored addr.
        T     maval = *(T*)maptr;       // Value at monitored addr at call start.

        if (maval.Equals(target))
            return true;

        T  cur    = maval;
        T* curptr = &cur;

        while (!cur.Equals(target))
        {
            FhPInvoke.WaitOnAddress(curptr, maptr, sizeof(T), 1);
            cur = *(T*)maptr;
        }

        return true;
    }

    public bool AwaitValues<T>(in ReadOnlySpan<T> targets, out T match) where T : unmanaged {
        match = default;

        if (!DerefOffsets(out nint vptr)) {
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

        while (!hasMatched)
        {
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