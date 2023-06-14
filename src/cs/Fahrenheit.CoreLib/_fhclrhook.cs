using System;

namespace Fahrenheit.CoreLib;

public enum HookTarget
{
    X  = 1,
    X2 = 2
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class FhHookAttribute : Attribute
{
    public HookTarget Target       { get; init; }
    public int        Offset       { get; init; }
    public Type       DelegateType { get; init; }

    public FhHookAttribute(HookTarget target, int addr, Type delegateType)
    {
        Target       = target;
        Offset       = addr;
        DelegateType = delegateType;
    }
}