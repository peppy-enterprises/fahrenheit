using System;
using System.Runtime.CompilerServices;

namespace Fahrenheit.CoreLib;

public enum FhStringDerefType
{
    Unicode = 1,
    UTF8    = 2,
    Ansi    = 3
}

public record struct FhPointerDeref(nint Offset, bool AsPtr);

/* [fkelava 23/6/23 13:12]
 * Any use of FhPointer is _inherently_ unsafe in the sense that you will always be subject to possible AVs.
 * Since we have no control over execution, a pointer can become invalid in the instant _after_ internal validation
 * and _before_ a e.g. string deref. Hence, you MUST appropriately guard FhPointer operations lest you crash the game.
 */
public unsafe readonly ref struct FhPointer
{
    private readonly ReadOnlySpan<FhPointerDeref> _derefs;

    public FhPointer(ReadOnlySpan<FhPointerDeref> derefs)
    {
        _derefs = derefs;
    }

    /* [fkelava 23/6/23 14:03]
     * Some one-liners require a hefty explanation. This one does. READ IT CAREFULLY.
     * 
     * Unsafe.Read<T> does not require a constraint. The constraint is there to make evident that
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
     * TestStructB is invalid. A `bool` marshaled as Win32 BOOL is 1 byte managed, 4 bytes unmanaged. The request is invalid and DerefPrimitive will crash.
     */
    public T DerefPrimitive<T>() where T : struct
    {
        int us = Marshal.SizeOf<T>();
        int ms = Unsafe.SizeOf<T>();

        if (us != ms)
        {
            FhLog.Log(LogLevel.Error, $"Invalid use of DerefPrimitive for type {typeof(T).FullName} - type unmanaged size {us} != type managed size {ms}. Crashing the game before the inevitable happens.");
            throw new Exception("FH_E_DPTR_UNSAFE_PRIMITIVE_DEREF");
        }

        return DerefOffsets(out nint vptr) ? Unsafe.Read<T>(vptr.ToPointer()) : default;
    }

    public string DerefString(FhStringDerefType strtype)
    {
        if (!DerefOffsets(out nint vptr)) return string.Empty;

        return strtype switch
        {
            FhStringDerefType.Unicode => Marshal.PtrToStringUni(vptr)!,
            FhStringDerefType.UTF8    => Marshal.PtrToStringUTF8(vptr)!,
            FhStringDerefType.Ansi    => Marshal.PtrToStringAnsi(vptr)!,
            _                         => throw new Exception("FH_E_DPTR_UNDEFINED_STRING_DEREF_TYPE")
        };
    }

    private nint DerefOffsetsInternal()
    {
        nint ptr = nint.Zero;

        foreach (FhPointerDeref deref in _derefs)
        {
            ptr = deref.AsPtr ? Marshal.ReadIntPtr(ptr + deref.Offset) : ptr + deref.Offset;
            if (ptr == nint.Zero) return nint.MaxValue;
        }

        return ptr;
    }

    public bool DerefOffsets(out nint vptr)
    {
        return (vptr = DerefOffsetsInternal()) != nint.MaxValue;
    }
}