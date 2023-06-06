namespace Fahrenheit.CoreLib;

/* [fkelava 6/6/23 21:29]
 * P/Invokes cannot be declared here because of https://github.com/dotnet/runtime/issues/87188.
 * 
 * Until this is fixed, P/Invokes must be declared in the hook assembly itself.
 */

public static partial class FhWin32
{
    //[LibraryImport("msvcrt.dll", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
    //[UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    //public static partial int _vscprintf(string format, IntPtr ptr);

    //[LibraryImport("msvcrt.dll", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
    //[UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    //public static partial int vsprintf(IntPtr buffer, string format, IntPtr args);

    //[DllImport("msvcrt.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    //public static extern int sprintf(IntPtr buffer, string format, __arglist);

    //[DllImport("msvcrt.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    //public static extern int _scprintf(string format, __arglist);
}
