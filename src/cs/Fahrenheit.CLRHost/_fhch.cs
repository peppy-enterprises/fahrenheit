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

public static class FhCLRHost {
    public static bool hook<T>(nint addr, T hook_fn, [NotNullWhen(true)] out T? orig) where T : Delegate {
        nint hookAddr = Marshal.GetFunctionPointerForDelegate(hook_fn);
        FhLog.Info($"Applying hook {hook_fn.Method.Name}; T -> 0x{addr.ToString("X8")}.");

        FhPInvoke.DetourTransactionBegin();
        FhPInvoke.DetourUpdateThread(FhPInvoke.GetCurrentThread());
        FhPInvoke.DetourAttach(ref addr, hookAddr);
        bool rv = FhPInvoke.DetourTransactionCommit() == 0;

        orig = Marshal.GetDelegateForFunctionPointer<T>(addr);
        FhLog.Info($"H {hook_fn.Method.Name}; O -> 0x{addr.ToString("X8")}, H -> 0x{hookAddr.ToString("X8")}.");

        return rv;
    }

    public static bool unhook<T>(nint addr, T hook_fn, [NotNullWhen(true)] out T? orig) where T : Delegate {
        nint hookAddr = Marshal.GetFunctionPointerForDelegate(hook_fn);
        FhLog.Info($"Removing hook {hook_fn.Method.Name}; targeted module addr: 0x{addr.ToString("X8")}, final address: 0x{addr.ToString("X8")}.");

        FhPInvoke.DetourTransactionBegin();
        FhPInvoke.DetourUpdateThread(FhPInvoke.GetCurrentThread());
        FhPInvoke.DetourDetach(ref addr, hookAddr);
        bool rv = FhPInvoke.DetourTransactionCommit() == 0;

        orig = Marshal.GetDelegateForFunctionPointer<T>(addr);
        FhLog.Info($"H {hook_fn.Method.Name}; O -> 0x{addr.ToString("X8")}, H -> 0x{hookAddr.ToString("X8")}.");

        return rv;
    }

    /* [fkelava 3/6/23 15:13]
     * The signature is intentional. You can edit it, but that requires a change in the CLR hosting code as well.
     *
     * https://learn.microsoft.com/en-us/dotnet/core/tutorials/netcore-hosting#step-3---load-managed-assembly-and-get-function-pointer-to-a-managed-method
     * `public delegate int ComponentEntryPoint(IntPtr args, int sizeBytes);`
     */
    public static int clrhost_init(IntPtr args, int size) {
        if (!FhLoader.LoadModules(FhRuntimeConst.ModulesDir.Path, out List<FhModuleConfigCollection>? moduleConfigs))
            throw new Exception("FH_E_CLRHOST_MODULE_LOAD_FAILED");

        FhModuleController.Initialize(moduleConfigs);
        foreach (bool rv in FhModuleController.StartAll()) { }
        return 0;
    }
}