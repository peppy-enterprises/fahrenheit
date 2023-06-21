using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

/* Please don't mess with this. */
namespace FFXCutsceneRemover.ComponentUtil;

using SizeT = UIntPtr;

public class ProcessModuleWow64Safe
{
    public IntPtr BaseAddress { get; set; }
    public IntPtr EntryPointAddress { get; set; }
    public string FileName { get; set; }
    public int ModuleMemorySize { get; set; }
    public string ModuleName { get; set; }
    public FileVersionInfo FileVersionInfo
    {
        get { return FileVersionInfo.GetVersionInfo(FileName); }
    }
    public override string ToString()
    {
        return ModuleName ?? base.ToString();
    }
}

public enum ReadStringType
{
    AutoDetect,
    ASCII,
    UTF8,
    UTF16
}

public static class ExtensionMethods
{
    private static Dictionary<int, ProcessModuleWow64Safe[]> ModuleCache = new Dictionary<int, ProcessModuleWow64Safe[]>();

    public static ProcessModuleWow64Safe MainModuleWow64Safe(this Process p)
    {
        return p.ModulesWow64Safe().First();
    }

    public static ProcessModuleWow64Safe[] ModulesWow64Safe(this Process p)
    {
        if (ModuleCache.Count > 100)
            ModuleCache.Clear();

        const int LIST_MODULES_ALL = 3;
        const int MAX_PATH = 260;

        var hModules = new IntPtr[1024];

        uint cb = (uint)IntPtr.Size * (uint)hModules.Length;

        if (!WinAPI.EnumProcessModulesEx(p.Handle, hModules, cb, out uint cbNeeded, LIST_MODULES_ALL))
            throw new Win32Exception();
        uint numMods = cbNeeded / (uint)IntPtr.Size;

        int hash = p.StartTime.GetHashCode() + p.Id + (int)numMods;
        if (ModuleCache.ContainsKey(hash))
            return ModuleCache[hash];

        var ret = new List<ProcessModuleWow64Safe>();

        // everything below is fairly expensive, which is why we cache!
        var sb = new StringBuilder(MAX_PATH);
        for (int i = 0; i < numMods; i++)
        {
            sb.Clear();
            if (WinAPI.GetModuleFileNameEx(p.Handle, hModules[i], sb, (uint)sb.Capacity) == 0)
                throw new Win32Exception();
            string fileName = sb.ToString();

            sb.Clear();
            if (WinAPI.GetModuleBaseName(p.Handle, hModules[i], sb, (uint)sb.Capacity) == 0)
                throw new Win32Exception();
            string baseName = sb.ToString();

            var moduleInfo = new WinAPI.MODULEINFO();
            if (!WinAPI.GetModuleInformation(p.Handle, hModules[i], out moduleInfo, (uint)Marshal.SizeOf(moduleInfo)))
                throw new Win32Exception();

            ret.Add(new ProcessModuleWow64Safe()
            {
                FileName = fileName,
                BaseAddress = moduleInfo.lpBaseOfDll,
                ModuleMemorySize = (int)moduleInfo.SizeOfImage,
                EntryPointAddress = moduleInfo.EntryPoint,
                ModuleName = baseName
            });
        }

        ModuleCache.Add(hash, ret.ToArray());

        return ret.ToArray();
    }

    public static bool Is64Bit(this Process process)
    {
        WinAPI.IsWow64Process(process.Handle, out bool procWow64);
        return Environment.Is64BitOperatingSystem && !procWow64;
    }

    public static bool ReadValue<T>(this Process process, IntPtr addr, out T val)
    {
        val = default;
        int size = Unsafe.SizeOf<T>();

        if (!ReadBytes(process, addr, size, out byte[] bytes))
            return false;

        val = ResolveToType<T>(bytes);
        return true;
    }

    public static bool ReadBytes(this Process process, IntPtr addr, int count, out byte[] val)
    {
        var bytes = new byte[count];
        val = WinAPI.ReadProcessMemory(process.Handle, addr, bytes, (SizeT)bytes.Length, out SizeT read) ? bytes : null;
        return val != null;
    }

    public static bool ReadPointer(this Process process, IntPtr addr, out IntPtr val)
    {
        return ReadPointer(process, addr, process.Is64Bit(), out val);
    }

    public static bool ReadPointer(this Process process, IntPtr addr, bool is64Bit, out IntPtr val)
    {
        var bytes = new byte[is64Bit ? 8 : 4];

        val = IntPtr.Zero;
        if (!WinAPI.ReadProcessMemory(process.Handle, addr, bytes, (SizeT)bytes.Length, out SizeT read)
            || read != (SizeT)bytes.Length)
            return false;

        val = is64Bit ? (IntPtr)BitConverter.ToInt64(bytes, 0) : (IntPtr)BitConverter.ToUInt32(bytes, 0);

        return true;
    }

    public static bool ReadString(this Process process, IntPtr addr, int numBytes, out string str)
    {
        return ReadString(process, addr, ReadStringType.AutoDetect, numBytes, out str);
    }

    public static bool ReadString(this Process process, IntPtr addr, ReadStringType type, int numBytes, out string str)
    {
        var sb = new StringBuilder(numBytes);
        str = ReadString(process, addr, type, sb) ? sb.ToString() : string.Empty;
        return str != string.Empty;
    }

    public static bool ReadString(this Process process, IntPtr addr, StringBuilder sb)
    {
        return ReadString(process, addr, ReadStringType.AutoDetect, sb);
    }

    public static bool ReadString(this Process process, IntPtr addr, ReadStringType type, StringBuilder sb)
    {
        var bytes = new byte[sb.Capacity];
        if (!WinAPI.ReadProcessMemory(process.Handle, addr, bytes, (SizeT)bytes.Length, out SizeT read)
            || read != (SizeT)bytes.Length)
            return false;

        sb.Append(type switch
        {
            ReadStringType.AutoDetect => read.ToUInt64() >= 2 && bytes[1] == '\x0' 
                ? Encoding.Unicode.GetString(bytes) 
                : Encoding.UTF8.GetString(bytes),
            ReadStringType.UTF8       => Encoding.UTF8.GetString(bytes),
            ReadStringType.UTF16      => Encoding.Unicode.GetString(bytes),
            _                         => Encoding.ASCII.GetString(bytes)
        });

        for (int i = 0; i < sb.Length; i++)
        {
            if (sb[i] == '\0')
            {
                sb.Remove(i, sb.Length - i);
                break;
            }
        }

        return true;
    }

    public static T ReadValue<T>(this Process process, IntPtr addr, T defval = default) where T : struct
    {
        return process.ReadValue(addr, out T val) ? val : defval;
    }

    public static byte[] ReadBytes(this Process process, IntPtr addr, int count)
    {
        return process.ReadBytes(addr, count, out byte[] bytes) ? bytes : null;
    }

    public static IntPtr ReadPointer(this Process process, IntPtr addr, IntPtr defval = default)
    {
        return process.ReadPointer(addr, out nint ptr) ? ptr : defval;
    }

    public static string ReadString(this Process process, IntPtr addr, int numBytes, string defval = null)
    {
        return process.ReadString(addr, numBytes, out string str) ? str : defval;
    }

    public static string ReadString(this Process process, IntPtr addr, ReadStringType type, int numBytes, string defval = null)
    {
        return process.ReadString(addr, type, numBytes, out string str) ? str : defval;
    }

    public static bool WriteValue<T>(this Process process, IntPtr addr, T obj) where T : struct
    {
        int size = Marshal.SizeOf(obj);
        byte[] arr = new byte[size];

        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(obj, ptr, true);
        Marshal.Copy(ptr, arr, 0, size);
        Marshal.FreeHGlobal(ptr);

        return process.WriteBytes(addr, arr);
    }

    public static bool WriteBytes(this Process process, IntPtr addr, byte[] bytes)
    {
        return WinAPI.WriteProcessMemory(process.Handle, addr, bytes, (SizeT)bytes.Length, out SizeT written)
               && written == (SizeT)bytes.Length;
    }

    static T ResolveToType<T>(ReadOnlySpan<byte> bytes)
    {
        return Unsafe.As<byte, T>(ref MemoryMarshal.GetReference(bytes));
    }

    public static IntPtr AllocateMemory(this Process process, int size)
    {
        return WinAPI.VirtualAllocEx(process.Handle, IntPtr.Zero, (SizeT)size, (uint)MemPageState.MEM_COMMIT,
            MemPageProtect.PAGE_EXECUTE_READWRITE);
    }

    public static bool FreeMemory(this Process process, IntPtr addr)
    {
        const uint MEM_RELEASE = 0x8000;
        return WinAPI.VirtualFreeEx(process.Handle, addr, SizeT.Zero, MEM_RELEASE);
    }

    public static bool VirtualProtect(this Process process, IntPtr addr, int size, MemPageProtect protect,
        out MemPageProtect oldProtect)
    {
        return WinAPI.VirtualProtectEx(process.Handle, addr, (SizeT)size, protect, out oldProtect);
    }

    public static bool VirtualProtect(this Process process, IntPtr addr, int size, MemPageProtect protect)
    {
        return WinAPI.VirtualProtectEx(process.Handle, addr, (SizeT)size, protect, out MemPageProtect oldProtect);
    }

    public static IntPtr CreateThread(this Process process, IntPtr startAddress, IntPtr parameter)
    {
        return WinAPI.CreateRemoteThread(process.Handle, IntPtr.Zero, (SizeT)0, startAddress, parameter, 0, out nint threadId);
    }

    public static IntPtr CreateThread(this Process process, IntPtr startAddress)
    {
        return CreateThread(process, startAddress, IntPtr.Zero);
    }

    public static void Suspend(this Process process)
    {
        WinAPI.NtSuspendProcess(process.Handle);
    }

    public static void Resume(this Process process)
    {
        WinAPI.NtResumeProcess(process.Handle);
    }

    public static float ToFloatBits(this uint i)
    {
        return BitConverter.ToSingle(BitConverter.GetBytes(i), 0);
    }

    public static uint ToUInt32Bits(this float f)
    {
        return BitConverter.ToUInt32(BitConverter.GetBytes(f), 0);
    }

    public static bool BitEquals(this float f, float o)
    {
        return ToUInt32Bits(f) == ToUInt32Bits(o);
    }
}
