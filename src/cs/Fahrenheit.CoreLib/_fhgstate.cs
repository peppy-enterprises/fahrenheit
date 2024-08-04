namespace Fahrenheit.CoreLib;

public static unsafe class FhGlobal
{
    static FhGlobal()
    {
        base_addr = FhUtil.get_mbase_or_throw();
    }

    public static nint base_addr { get; }
}