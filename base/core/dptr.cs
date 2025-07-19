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
public unsafe readonly struct FhPointer(FhPointerDeref[] derefs) {

    private readonly FhPointerDeref[] _derefs = derefs;

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
}
