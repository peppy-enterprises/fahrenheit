using System;

namespace Fahrenheit.CoreLib;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class FhHookAttribute : Attribute
{
    public int  Address      { get; init; }
    public Type DelegateType { get; init; }

    public FhHookAttribute(int addr, Type delegateType)
    {
        Address      = addr;
        DelegateType = delegateType;
    }
}