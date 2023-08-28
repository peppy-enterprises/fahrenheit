using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Fahrenheit.CoreLib;

namespace Fahrenheit.CLRHost;

public class FhMethodHandle<T> where T : Delegate
{
    private readonly FhModule _handleOwner;
    private readonly nint     _addr;
    private          T?       _origFptr;
    private readonly T        _hookFptr;

    public FhMethodHandle(FhModule owner,
                          nint     addr,
                          T        fptr)
    {
        _handleOwner = owner;
        _addr        = addr;
        _origFptr    = Marshal.GetDelegateForFunctionPointer<T>(FhCLRHost.RetrieveMbaseOrThrow() + _addr);
        _hookFptr    = fptr;
    }

    public bool GetOriginalFptrSafe([NotNullWhen(true)] out T? handle)
    {
        return (handle = _origFptr) != null;
    }

    public bool GetHookFptrSafe([NotNullWhen(true)] out T? handle)
    {
        return (handle = _hookFptr) != null;
    }

    public bool ApplyHook()
    {
        return FhCLRHost.CLRHostHook(_addr, _hookFptr, out _origFptr);
    }

    public bool RemoveHook()
    {
        return FhCLRHost.CLRHostUnhook(_addr, _hookFptr, out _origFptr);
    }
}
