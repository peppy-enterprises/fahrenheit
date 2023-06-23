using System;

namespace Fahrenheit.CoreLib;

public enum FhHookTarget
{
    FFX  = 1,
    FFX2 = 2
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class FhHookAttribute : Attribute
{
    public FhHookTarget Target       { get; init; }
    public int          Offset       { get; init; }
    public Type         DelegateType { get; init; }

    public FhHookAttribute(FhHookTarget target, int addr, Type delegateType)
    {
        Target       = target;
        Offset       = addr;
        DelegateType = delegateType;
    }
}