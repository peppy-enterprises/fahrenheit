/* [fkelava 29/5/23 15:21]
 * Taken mostly wholesale from https://github.com/citronneur/detours.net/, 
 * with adjustments made to the .NET runtime hosting bit to make it compatible with .NET Core/.NET 5+.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Fahrenheit.CoreLib;

namespace Fahrenheit.CLRHost;

public static class FhCLRHost
{
    internal static nint RetrieveMbaseOrThrow()
    {
        nint mbase;
        if ((mbase = FhPInvoke.GetModuleHandle("FFX.exe")) == nint.Zero)
        {
            if ((mbase = FhPInvoke.GetModuleHandle("FFX-2.exe")) == nint.Zero)
                throw new Exception("FH_E_HOOK_TARGET_INDETERMINATE");
        }
        return mbase;
    }

    public static bool CLRHostHook<T>(nint offset, T hook, [NotNullWhen(true)] out T? orig) where T : Delegate
    {
        nint mbase    = RetrieveMbaseOrThrow();
        nint origAddr = mbase + offset;
        nint iatAddr  = origAddr;
        nint hookAddr = Marshal.GetFunctionPointerForDelegate(hook);

        FhLog.Log(LogLevel.Info, $"Applying hook {hook.Method.Name}; M -> 0x{mbase.ToString("X8")}, T -> 0x{origAddr.ToString("X8")}.");

        FhPInvoke.DetourTransactionBegin();
        FhPInvoke.DetourUpdateThread(FhPInvoke.GetCurrentThread());
        FhPInvoke.DetourAttach(ref origAddr, hookAddr);
        bool rv = FhPInvoke.DetourTransactionCommit() == 0;
        FhPInvoke.FhDetourPatchIAT(FhPInvoke.GetModuleHandle("coreclr.dll"), iatAddr, origAddr);

        orig = Marshal.GetDelegateForFunctionPointer<T>(origAddr);
        FhLog.Log(LogLevel.Info, $"H {hook.Method.Name}; O -> 0x{origAddr.ToString("X8")}, H -> 0x{hookAddr.ToString("X8")}.");

        return rv;
    }

    public static bool CLRHostUnhook<T>(nint offset, T hook, [NotNullWhen(true)] out T? orig) where T : Delegate
    {
        nint mbase    = RetrieveMbaseOrThrow();
        nint origAddr = mbase + offset;
        nint iatAddr  = origAddr;
        nint hookAddr = Marshal.GetFunctionPointerForDelegate(hook);

        FhLog.Log(LogLevel.Info, $"Removing hook {hook.Method.Name}; targeted module addr: 0x{mbase.ToString("X8")}, final address: 0x{origAddr.ToString("X8")}.");

        FhPInvoke.DetourTransactionBegin();
        FhPInvoke.DetourUpdateThread(FhPInvoke.GetCurrentThread());
        FhPInvoke.DetourDetach(ref origAddr, hookAddr);
        bool rv = FhPInvoke.DetourTransactionCommit() == 0;
        FhPInvoke.FhDetourUnpatchIAT(FhPInvoke.GetModuleHandle("coreclr.dll"), iatAddr, origAddr);

        orig = Marshal.GetDelegateForFunctionPointer<T>(origAddr);
        FhLog.Log(LogLevel.Info, $"H {hook.Method.Name}; O -> 0x{origAddr.ToString("X8")}, H -> 0x{hookAddr.ToString("X8")}.");

        return rv;
    }

    /* [fkelava 3/6/23 15:13]
     * The signature is intentional. You can edit it, but that requires a change in the CLR hosting code as well.
     * 
     * https://learn.microsoft.com/en-us/dotnet/core/tutorials/netcore-hosting#step-3---load-managed-assembly-and-get-function-pointer-to-a-managed-method
     * `public delegate int ComponentEntryPoint(IntPtr args, int sizeBytes);`
     */
    public static int CLRHostInit(IntPtr args, int size)
    {
        if (!FhLoader.LoadModules(FhRuntimeConst.CLRHooksDir.Path, out List<FhModuleConfigCollection>? moduleConfigs))
            throw new Exception("FH_E_CLRHOST_MODULE_LOAD_FAILED");

        FhModuleController.Initialize(moduleConfigs);
        foreach (bool rv in FhModuleController.StartAll()) { }
        return 0;
    }
}