using System;
using System.Diagnostics.CodeAnalysis;

namespace Fahrenheit.CoreLib;

public static class FhHookEngine {
    public static bool hook<T>(nint addr, T hook_fn, [NotNullWhen(true)] out T? orig) where T : Delegate {
        orig = default;

        nint origAddr = addr;
        nint hookAddr = Marshal.GetFunctionPointerForDelegate(hook_fn);
        FhLog.Log(LogLevel.Info, $"Applying hook {hook_fn.Method.Name}; T -> 0x{addr.ToString("X8")}.");

        if (FhPInvoke.MH_CreateHook(addr, hookAddr, ref origAddr) != 0) return false;
        if (FhPInvoke.MH_EnableHook(addr)                         != 0) return false;

        orig = Marshal.GetDelegateForFunctionPointer<T>(origAddr);
        FhLog.Log(LogLevel.Info, $"H {hook_fn.Method.Name}; O -> 0x{addr.ToString("X8")}, H -> 0x{hookAddr.ToString("X8")}.");

        return true;
    }

    public static bool unhook<T>(nint addr, T hook_fn, [NotNullWhen(true)] out T? orig) where T : Delegate {
        orig = default;

        nint hookAddr = Marshal.GetFunctionPointerForDelegate(hook_fn);
        FhLog.Log(LogLevel.Info, $"Removing hook {hook_fn.Method.Name}; targeted module addr: 0x{addr.ToString("X8")}, final address: 0x{addr.ToString("X8")}.");

        if (FhPInvoke.MH_DisableHook(addr) != 0) return false;
        if (FhPInvoke.MH_RemoveHook(addr)  != 0) return false;

        orig = Marshal.GetDelegateForFunctionPointer<T>(addr);
        FhLog.Log(LogLevel.Info, $"H {hook_fn.Method.Name}; O -> 0x{addr.ToString("X8")}, H -> 0x{hookAddr.ToString("X8")}.");

        return true;
    }
}