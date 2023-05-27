using System;
using System.Runtime.CompilerServices;

namespace Fahrenheit.CoreLib;

public static class FhMarshal
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ToBytes<T>(in ReadOnlySpan<T> src, in Span<byte> dest, out int bytesWritten) where T : struct
    {
        bytesWritten = Unsafe.SizeOf<T>() * src.Length;
        MemoryMarshal.AsBytes(src).CopyTo(dest);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void FromBytes<T>(in ReadOnlySpan<byte> src, in Span<T> dest, int srcLen, out int count) where T : struct
    {
        count = srcLen / Unsafe.SizeOf<T>();
        MemoryMarshal.Cast<byte, T>(src).CopyTo(dest);
    }
}
