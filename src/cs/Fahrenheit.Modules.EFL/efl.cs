using Fahrenheit.CLRHost;
using Fahrenheit.CoreLib;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

using FFX = Fahrenheit.CoreLib.FFX;
using FFX2 = Fahrenheit.CoreLib.FFX2;

using static Fahrenheit.Modules.EFL.NativeConstants;
using Fahrenheit.Hooks.Generic;

// This place is not a place of safety
// No highly esteemed code is written here
// Nothing sane is here
namespace Fahrenheit.Modules.EFL;

public sealed record EFLModuleConfig : FhModuleConfig {
    public List<string> ModPaths { get; }

    [JsonConstructor]
    public EFLModuleConfig(string configName, uint configVersion, bool configEnabled, List<string> modPaths)
            : base(configName, configVersion, configEnabled) {
        ModPaths = modPaths;
    }

    public override bool TrySpawnModule([NotNullWhen(true)] out FhModule? fm) {
        fm = new EFLModule(this);
        return fm.ModuleState == FhModuleState.InitSuccess;
    }
}

public unsafe class EFLModule : FhModule {
    private readonly EFLModuleConfig _moduleConfig;
    private List<string> modPaths = new();

    /*===== Delegates =====*/
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate int OpenFile(int *handle, char *filename, bool readOnly);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate int CheckExists(nint thisPtr, char *p1);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void CheckVBF_Parent();

    // So mysterious....
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void StdVoidV();

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void StdVoidS(string p1);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate nint StdPtrV();

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int StdIntV();

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate void ThisVoidV(nint thisPtr);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate void ThisVoidS(nint thisPtr, string p1);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate int ThisIntV(nint thisPtr);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate int ThisIntS(nint thisPtr, string p1);

    /*===== Hooks =====*/
    private FhMethodHandle<CheckExists> hCheckExists;
    private FhMethodHandle<OpenFile> hOpenFile;
    private FhMethodHandle<CheckVBF_Parent> hCheckVBF_Parent;

    public EFLModule(EFLModuleConfig cfg) : base(cfg) {
        _moduleConfig = cfg;

        foreach (string path in cfg.ModPaths) {
            AddModPath(path);
        }

        // Poor gal's "which game is loaded",
        //TODO: please update with an actual enum
        if (FFX.Globals.game_base != 0) { // FFX
            hCheckExists = new FhMethodHandle<CheckExists>(this, 0x21c000, HCheckExists);
            hOpenFile = new FhMethodHandle<OpenFile>(this, 0x208100, HOpenFile);
            hCheckVBF_Parent = new FhMethodHandle<CheckVBF_Parent>(this, 0x279760, HCheckVBF_Parent);
        } else { // FFX-2
            throw new System.NotImplementedException("External File Loader for FFX-2 is not implemented yet.");
        }

        _moduleState  = FhModuleState.InitSuccess;
    }

    public override FhModuleConfig ModuleConfiguration => _moduleConfig;

    /// <summary>
    /// Adds a path for the EFL to check for files
    /// </summary>
    /// <param name="modPath">The path to add</param>
    public void AddModPath(string modPath) {
        modPaths.Add(modPath);
    }

    /// <summary>
    /// Removes a path for the EFL to check for files
    /// </summary>
    /// <param name="modPath">The path to add</param>
    /// <returns>Whether the path was successfully removed</returns>
    public bool RemoveModPath(string modPath) {
        return modPaths.Remove(modPath);
    }

    public override bool FhModuleOnError() {
        return true;
    }

    public override bool FhModuleInit() {
        return true;
    }

    public override bool FhModuleStart() {
        return hCheckExists.ApplyHook() && hCheckVBF_Parent.ApplyHook() && hOpenFile.ApplyHook();
    }

    public override bool FhModuleStop() {
        return hCheckExists.RemoveHook() && hCheckVBF_Parent.RemoveHook() && hOpenFile.RemoveHook();
    }

    /*===== Utility Functions =====*/
    private string StripFilepath(string filepath) {
        if (filepath[..9] == "../../../") return filepath[9..];
        else if (filepath[..8] == "../../..") return filepath[8..];
        else if (filepath[0] == '/') return filepath[1..];
        return filepath;
    }

    private string? GetModdedFilepath(string filepath) {
        string? existsIn = null;

        foreach (string modPath in modPaths) {
            if (File.Exists(modPath + "/" + filepath)) {
                if (existsIn is not null) {
                    FhLog.Warning($"File conflict at {filepath} between {existsIn} and {modPath}");
                    continue;
                }
                existsIn = modPath;
            }
        }

        if (existsIn is null) return null;
        return $"{existsIn}/{filepath}";
    }

    // Wrapper function to make HOpenFile's code a bit prettier :3
    private int FinalizeOpenFile(int *handle, char *path) {
        *(uint*)(handle + 1) = EFLPInvoke.GetDiskFreeSpace(
                path,
                out uint sectorsPerCluster,
                out uint bytesPerSector,
                out uint numberOfFreeClusters,
                out uint totalNumberOfClusters)
            ? bytesPerSector
            : 2048;
        fixed (byte *code = GetSecSkipWithRet(0)) {
            if (!VirtualProtectEx(FhCLRHost.RetrieveMbaseOrThrow(), (nint)code, 0x8, 0x40, out uint _)) {
                throw new System.Exception($"{GetLastError()}");
            }

            Marshal.GetDelegateForFunctionPointer<SkipSecCheck>((nint)code)();
        }
        //FhUtil.GetFPtr<FhXDelegates.SecCookieCheck>(0x549240)(FFX.Globals.security_cookie);
        return 0;
    }

    /*===== I feel dirty..... =====*/
    [DllImport("kernel32.dll")]
    private static extern bool GetModuleHandleEx(int flags, string moduleName, out nint handle);

    [DllImport("kernel32.dll")]
    private static extern int GetLastError();

    [System.Security.SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void SkipSecCheck();

    [DllImport("kernel32.dll")]
    private static extern bool VirtualProtectEx(nint process, nint address, nuint size, uint newProtect, out uint oldProtect);

    private static byte[] GetSecSkipWithRet(int value) {
        byte[] _value = { (byte)(value & 0xFF), (byte)((value >>= 0x8) & 0xFF), (byte)((value >>= 0x8) & 0xFF), (byte)((value >>= 0x8) & 0xFF) };
        return new byte[] {
            0xB8, _value[3], _value[2], _value[1], _value[0], // mov eax, value;
            0x89, 0xEC, // mov esp, ebp;
            0xC3        // ret;
        };
    }

    byte[] SecSkip = {
        0x89, 0xEC, // mov esp, ebp;
        0xC3        // ret;
    };

    /*===== Hooks =====*/
    #pragma warning disable CS8600
    #pragma warning disable CS8604

    public int HCheckExists(nint thisPtr, char *path) {
        string filepath = Marshal.PtrToStringAnsi((nint)path);
        filepath = StripFilepath(filepath);
        filepath = GetModdedFilepath(filepath);

        if (filepath is not null) {
            fixed (byte *code = GetSecSkipWithRet(1)) {
                if (!VirtualProtectEx(FhCLRHost.RetrieveMbaseOrThrow(), (nint)code, 0x8, 0x40, out uint _)) {
                    throw new System.Exception($"{GetLastError()}");
                }

                Marshal.GetDelegateForFunctionPointer<SkipSecCheck>((nint)code)();
            }
            return 1;
        }

        if (hCheckExists.GetOriginalFptrSafe(out CheckExists? fptr)) {
            FhLog.Debug($"Passing non-modded path to vanilla game: \"{Marshal.PtrToStringAnsi((nint)path)}\"");
            return fptr.Invoke(thisPtr, path);
        }

        fixed (byte *code = GetSecSkipWithRet(0)) {
            if (!VirtualProtectEx(FhCLRHost.RetrieveMbaseOrThrow(), (nint)code, 0x8, 0x40, out uint _)) {
                throw new System.Exception($"{GetLastError()}");
            }

            Marshal.GetDelegateForFunctionPointer<SkipSecCheck>((nint)code)();
        }
        //FhUtil.GetFPtr<FhXDelegates.SecCookieCheck>(0x549240)(FFX.Globals.security_cookie);
        return 0;
    }

    public void HCheckVBF_Parent() {
        FhUtil.GetFPtr<StdVoidS>(0x207F00)("../../..");
        FhUtil.GetFPtr<StdVoidV>(0x21B750)();
        nint ptr = FhUtil.GetFPtr<StdPtrV>(0x21BF70)();
        FhUtil.GetFPtr<ThisVoidS>(0x21C560)(ptr, "../../../");
        int ignored = FhUtil.GetFPtr<ThisIntS>(0x21C310)(ptr, "data\\FFX_Data.vbf");
        // Skipping `if` block
        FhUtil.GetFPtr<StdVoidV>(0x2DB0F0)();
        ptr = FhUtil.GetFPtr<StdPtrV>(0x2DB1A0)();
        FhUtil.GetFPtr<ThisIntV>(0x2DB1C0)(ptr);
        // non-op switch statement ???
        FhUtil.GetFPtr<StdIntV>(0x207EF0)();
        FhUtil.GetFPtr<StdIntV>(0x2F9C40)();
    }

    public int HOpenFile(int *handle, char *path, bool readOnly) {
        if (path is null) {
            FhLog.Error("Couldn't open a file with no path");
            throw new System.ArgumentNullException(nameof(path), "Couldn't open a file with no path");
        }

        string filepath = Marshal.PtrToStringAnsi((nint)path);
        filepath = StripFilepath(filepath);
        filepath = GetModdedFilepath(filepath);

        if (filepath is null) {
            if (hOpenFile.GetOriginalFptrSafe(out OpenFile? fptr)) {
                return fptr.Invoke(handle, path, readOnly);
            }
        }

        if (readOnly) {
            *handle = (int)EFLPInvoke.CreateFile(filepath, FILE_READ_DATA, FILE_SHARE_READ, 0, OPEN_EXISTING, FILE_FLAG_SEQUENTIAL_SCAN, 0);
            if (*handle != INVALID_HANDLE_VALUE) return FinalizeOpenFile(handle, path);
        } else {
            *handle = (int)EFLPInvoke.CreateFile(filepath, FILE_WRITE_DATA, 0, 0, OPEN_ALWAYS, FILE_FLAG_SEQUENTIAL_SCAN, 0);
            if (*handle != INVALID_HANDLE_VALUE) return FinalizeOpenFile(handle, path);

            // File is protected against writing, gonna try our best anyway
            *handle = (int)EFLPInvoke.CreateFile(filepath, FILE_READ_DATA, FILE_SHARE_READ, 0, OPEN_EXISTING, FILE_FLAG_SEQUENTIAL_SCAN, 0);
            if (*handle != INVALID_HANDLE_VALUE) {
                EFLPInvoke.CloseHandle(*handle);
                *handle = INVALID_HANDLE_VALUE;

                fixed (byte *code = GetSecSkipWithRet(12)) {
                    if (!VirtualProtectEx(FhCLRHost.RetrieveMbaseOrThrow(), (nint)code, 0x8, 0x40, out uint _)) {
                        throw new System.Exception($"{GetLastError()}");
                    }

                    Marshal.GetDelegateForFunctionPointer<SkipSecCheck>((nint)code)();
                }
                //FhUtil.GetFPtr<FhXDelegates.SecCookieCheck>(0x549240)(FFX.Globals.security_cookie);
                return 12;
            }
        }

        fixed (byte *code = GetSecSkipWithRet(10)) {
            if (!VirtualProtectEx(FhCLRHost.RetrieveMbaseOrThrow(), (nint)code, 0x8, 0x40, out uint _)) {
                throw new System.Exception($"{GetLastError()}");
            }

            Marshal.GetDelegateForFunctionPointer<SkipSecCheck>((nint)code)();
        }
        //FhUtil.GetFPtr<FhXDelegates.SecCookieCheck>(0x549240)(FFX.Globals.security_cookie);
        return 10;
    }
}