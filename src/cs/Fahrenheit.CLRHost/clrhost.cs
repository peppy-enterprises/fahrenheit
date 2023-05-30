/* [fkelava 29/5/23 15:21]
 * Taken mostly wholesale from https://github.com/citronneur/detours.net/, 
 * with adjustments made to the .NET runtime hosting bit to make it compatible with .NET Core/.NET 5+.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

using Fahrenheit.CoreLib;

namespace Fahrenheit.CLRHost;

public class FhCLRHostDelegateStore
{
    public static Dictionary<MethodBase, Delegate> Real = new Dictionary<MethodBase, Delegate>();
    public static Dictionary<MethodBase, Delegate> Mine = new Dictionary<MethodBase, Delegate>();

    public static Delegate GetReal(MethodBase method)
    {
        return Real[method];
    }
}

public static class FhCLRHost
{
    private static void GetEligibleMethods(this Assembly assembly, in List<MethodInfo> callerList)
    {
        Type hookAttr = typeof(FhHookAttribute);

        foreach (Type type in assembly.GetExportedTypes())
        {
            foreach (MethodInfo method in type.GetMethods())
            {
                if (method.GetCustomAttribute(hookAttr) == null) continue;
                callerList.Add(method);
            }
        }
    }

    // public delegate int ComponentEntryPoint(IntPtr args, int sizeBytes);
    public static int CLRHostInit(IntPtr args, int size)
    {
        using (FileStream fs = File.Open("clrhost.log", FileMode.OpenOrCreate))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} - CLRHOSTINIT");
            }
        }

        return 0;
    }

    [DllImport("kernel32.dll")]                                                             private static extern nint GetProcAddress(nint hModule, string lpProcName);
    [DllImport("kernel32.dll")]                                                             private static extern nint GetCurrentThread();
    [DllImport("kernel32.dll", EntryPoint = "LoadLibraryW", CharSet = CharSet.Unicode)]     private static extern nint LoadLibrary(string lpModuleName);
    [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", CharSet = CharSet.Unicode)] private static extern nint GetModuleHandle(string lpModuleName);
    [DllImport("fhdetour.dll")]                                                             private static extern long DetourAttach(ref nint a, nint b);
    [DllImport("fhdetour.dll")]                                                             private static extern long DetourUpdateThread(nint a);
    [DllImport("fhdetour.dll")]                                                             private static extern long DetourTransactionBegin();
    [DllImport("fhdetour.dll")]                                                             private static extern long DetourTransactionCommit();
    [DllImport("fhdetour.dll")]                                                             private static extern bool DetoursPatchIAT(nint hModule, nint import, nint real);
    [DllImport("fhclrldr.dll", CharSet = CharSet.Ansi)]                                     private static extern void DetoursCLRSetGetProcAddressCache(nint hModule, string procName, nint real);
}