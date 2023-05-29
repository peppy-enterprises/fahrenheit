using System;

namespace Fahrenheit.CoreLib;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class FhHookAttribute : Attribute
{
    public nint Address      { get; init; }
    public Type DelegateType { get; init; }

    public FhHookAttribute(nint addr, Type delegateType)
    {
        Address      = addr;
        DelegateType = delegateType;
    }
}