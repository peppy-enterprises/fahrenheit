/* [fkelava 29/5/23 15:21]
 * Taken mostly wholesale from https://github.com/citronneur/detours.net/, 
 * with adjustments made to the .NET runtime hosting bit to make it compatible with .NET Core/.NET 5+.
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

using Fahrenheit.CoreLib;

namespace Fahrenheit.CLRHost;

public class FhCLRHostDelegateStore
{
    public static readonly Dictionary<MethodBase, Delegate> Real = new Dictionary<MethodBase, Delegate>();
    public static readonly Dictionary<MethodBase, Delegate> Mine = new Dictionary<MethodBase, Delegate>();

    public static Delegate GetReal(MethodBase method)
    {
        return Real[method];
    }
}

public sealed record FhHookRecord(MethodInfo Method, FhHookAttribute Attribute);

public static partial class FhCLRHost
{
    private static void GetEligibleMethods(this Assembly assembly, in List<FhHookRecord> callerList)
    {
        callerList.Clear();

        foreach (Type type in assembly.GetExportedTypes())
        {
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public))
            {
                foreach (FhHookAttribute attr in method.GetCustomAttributes<FhHookAttribute>())
                {
                    FhLog.Log(LogLevel.Info, $"Detected valid hook {method.Name}.");
                    callerList.Add(new FhHookRecord(method, attr));
                }
            }
        }
    }

    /* [fkelava 3/6/23 15:13]
     * The signature is intentional. You can edit it, but that requires a change in the CLR hosting code as well.
     * 
     * https://learn.microsoft.com/en-us/dotnet/core/tutorials/netcore-hosting#step-3---load-managed-assembly-and-get-function-pointer-to-a-managed-method
     * `public delegate int ComponentEntryPoint(IntPtr args, int sizeBytes);`
     */
    public static int CLRHostInit(IntPtr args, int size)
    {
        List<FhHookRecord> records = new List<FhHookRecord>();

        foreach (Assembly assem in FhLoader.LoadedPluginAssembliesCache)
        {
            FhLog.Log(LogLevel.Info, $"Entering host init for assembly {assem.GetName().Name?.ToUpperInvariant()}.");

            assem.GetEligibleMethods(records);

            foreach (FhHookRecord record in records)
            {
                MethodInfo method = record.Method;
                Type       dtype  = record.Attribute.DelegateType;
                string     mname  = record.Attribute.Target switch 
                {
                    HookTarget.X  => "FFX.exe",
                    HookTarget.X2 => "FFX-2.exe",
                    _             => throw new Exception("E_UNDEFINED_HOOK_TARGET")
                };

                FhLog.Log(LogLevel.Info, $"Hook record: module {mname} at 0x{record.Attribute.Offset.ToString("X8")}, apply hook {record.Method.Name}.");

                FhCLRHostDelegateStore.Mine[method] = Delegate.CreateDelegate(dtype, method);

                nint mbase = GetModuleHandle(mname);
                if (mbase == nint.Zero) continue;

                nint addr = mbase + record.Attribute.Offset;

                FhLog.Log(LogLevel.Info, $"Module addr: 0x{mbase.ToString("X8")}, final address: 0x{addr.ToString("X8")}.");

                DetourTransactionBegin();
                DetourUpdateThread(GetCurrentThread());
                DetourAttach(ref addr, Marshal.GetFunctionPointerForDelegate(FhCLRHostDelegateStore.Mine[method]));
                DetourTransactionCommit();

                // and so on patch IAT of clr module
                DetoursPatchIAT(GetModuleHandle("coreclr.dll"), mbase, addr);

                FhCLRHostDelegateStore.Real[method] = Marshal.GetDelegateForFunctionPointer(addr, dtype);
            }
        }

        return 0;
    }

    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]                                                             
    private static partial nint GetProcAddress(nint hModule, string lpProcName);
    [LibraryImport("kernel32.dll")]                                                             
    private static partial nint GetCurrentThread();
    [LibraryImport("kernel32.dll", EntryPoint = "LoadLibraryW", StringMarshalling = StringMarshalling.Utf16)]     
    private static partial nint LoadLibrary(string lpModuleName);
    [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW", StringMarshalling = StringMarshalling.Utf16)] 
    private static partial nint GetModuleHandle(string lpModuleName);
    [LibraryImport("fhdetour.dll")]                                                             
    private static partial long DetourAttach(ref nint a, nint b);
    [LibraryImport("fhdetour.dll")]                                                             
    private static partial long DetourUpdateThread(nint a);
    [LibraryImport("fhdetour.dll")]                                                             
    private static partial long DetourTransactionBegin();
    [LibraryImport("fhdetour.dll")]                                                             
    private static partial long DetourTransactionCommit();
    [LibraryImport("fhdetour.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool DetoursPatchIAT(nint hModule, nint import, nint real);
    [LibraryImport("fhclrldr.dll", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller))]                                     
    private static partial void DetoursCLRSetGetProcAddressCache(nint hModule, string procName, nint real);
}