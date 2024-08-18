using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Fahrenheit.CoreLib;

namespace Fahrenheit.CLRHost;

public class FhMethodHandle<T> where T : Delegate {
    private readonly FhModule _handleOwner;
    private readonly nint     _offset;
    private          T?       _origFptr;
    private readonly T        _hookFptr;

    public FhMethodHandle(FhModule owner,
                          nint     offset,
                          T        fptr) {
        _handleOwner = owner;
        _offset      = offset;
        _origFptr    = Marshal.GetDelegateForFunctionPointer<T>(FhGlobal.base_addr + _offset);
        _hookFptr    = fptr;
    }

    public bool try_get_original_fptr([NotNullWhen(true)] out T? handle) {
        return (handle = _origFptr) != null;
    }

    public bool try_get_hook_fptr([NotNullWhen(true)] out T? handle) {
        return (handle = _hookFptr) != null;
    }

    public bool hook() {
        return FhCLRHost.hook(_offset, _hookFptr, out _origFptr);
    }

    public bool unhook() {
        return FhCLRHost.unhook(_offset, _hookFptr, out _origFptr);
    }
}
