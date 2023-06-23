/* [fkelava 29/5/23 15:21]
 * Taken mostly wholesale from https://github.com/citronneur/detours.net/, 
 * with adjustments made to the .NET runtime hosting bit to make it compatible with .NET Core/.NET 5+.
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using Fahrenheit.CoreLib;

namespace Fahrenheit.CLRHost;

public static class FhDelegateStore
{
    public static readonly Dictionary<MethodBase, Delegate> Real = new Dictionary<MethodBase, Delegate>();
    public static readonly Dictionary<MethodBase, Delegate> Mine = new Dictionary<MethodBase, Delegate>();
}

public sealed record FhHookRecord(MethodInfo Method, Type DelegateType, nint Offset, FhHookTarget Target);

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
                    FhLog.Log(LogLevel.Info, $"{method.Name} -> 0x{attr.Offset.ToString("X")}.");
                    callerList.Add(new FhHookRecord(method, attr.DelegateType, attr.Offset, attr.Target));
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
        List<FhHookRecord> fhrlist = new List<FhHookRecord>();

        foreach (Assembly assem in FhLoader.LoadedPluginAssembliesCache)
        {
            FhLog.Log(LogLevel.Info, $"Entering host init for assembly {assem.GetName().Name?.ToUpperInvariant()}.");

            assem.GetEligibleMethods(fhrlist);

            foreach (FhHookRecord fhr in fhrlist)
            {
                MethodInfo method = fhr.Method;
                Type       dtype  = fhr.DelegateType;
                nint       offset = fhr.Offset;
                string     mname  = fhr.Target switch 
                {
                    FhHookTarget.FFX  => "FFX.exe",
                    FhHookTarget.FFX2 => "FFX-2.exe",
                    _                 => throw new Exception("FH_E_CLRHOST_UNDEFINED_HOOK_TARGET")
                };

                FhDelegateStore.Mine[method] = Delegate.CreateDelegate(dtype, method);

                nint mbase = FhPInvoke.GetModuleHandle(mname);
                if (mbase == nint.Zero) continue;
                nint addr  = mbase + offset;

                FhLog.Log(LogLevel.Info, $"Now applying hook {fhr.Method.Name}; targeted module addr: 0x{mbase.ToString("X8")}, final address: 0x{addr.ToString("X8")}.");

                FhPInvoke.DetourTransactionBegin();
                FhPInvoke.DetourUpdateThread(FhPInvoke.GetCurrentThread());
                FhPInvoke.DetourAttach(ref addr, Marshal.GetFunctionPointerForDelegate(FhDelegateStore.Mine[method]));
                FhPInvoke.DetourTransactionCommit();
                FhPInvoke.DetoursPatchIAT(FhPInvoke.GetModuleHandle("coreclr.dll"), mbase, addr);

                FhDelegateStore.Real[method] = Marshal.GetDelegateForFunctionPointer(addr, dtype);
            }
        }

        return 0;
    }
}