using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

/* Please don't mess with this. */
namespace FFXCutsceneRemover.ComponentUtil;

using OffsetT = Int32;

public class DeepPointer
{
    public enum DerefType { Auto, Bit32, Bit64 }

    private IntPtr _absoluteBase;
    private bool _usingAbsoluteBase;
    private DerefType _derefType;

    private OffsetT _base;
    private List<OffsetT> _offsets;
    private string _module;

    public DeepPointer(IntPtr absoluteBase, params OffsetT[] offsets)
        : this(absoluteBase, DerefType.Auto, offsets) { }

    public DeepPointer(IntPtr absoluteBase, DerefType derefType, params OffsetT[] offsets)
    {
        _absoluteBase = absoluteBase;
        _usingAbsoluteBase = true;
        _derefType = derefType;

        InitializeOffsets(offsets);
    }

    public DeepPointer(string module, OffsetT base_, params OffsetT[] offsets)
        : this(module, base_, DerefType.Auto, offsets) { }

    public DeepPointer(string module, OffsetT base_, DerefType derefType, params OffsetT[] offsets)
        : this(base_, derefType, offsets)
    {
        _module = module.ToLower();
    }

    public DeepPointer(OffsetT base_, params OffsetT[] offsets)
        : this(base_, DerefType.Auto, offsets) { }

    public DeepPointer(OffsetT base_, DerefType derefType, params OffsetT[] offsets)
    {
        _base = base_;
        _derefType = derefType;
        InitializeOffsets(offsets);
    }

    public T Deref<T>(Process process, T defval = default) where T : struct // all value types including structs
    {
        return Deref(process, out T val) ? val : defval;
    }

    public bool Deref<T>(Process process, out T value) where T : struct
    {
        value = default;
        return DerefOffsets(process, out nint ptr) && process.ReadValue(ptr, out value);
    }

    public byte[] DerefBytes(Process process, int count)
    {
        return DerefBytes(process, count, out byte[] bytes) ? bytes : null;
    }

    public bool DerefBytes(Process process, int count, out byte[] value)
    {
        value = null;
        return DerefOffsets(process, out nint ptr) && process.ReadBytes(ptr, count, out value);
    }

    public string DerefString(Process process, int numBytes, string defval = null)
    {
        return DerefString(process, ReadStringType.AutoDetect, numBytes, out string str) ? str : defval;
    }

    public string DerefString(Process process, ReadStringType type, int numBytes, string defval = null)
    {
        return DerefString(process, type, numBytes, out string str) ? str : defval;
    }

    public bool DerefString(Process process, int numBytes, out string str)
    {
        return DerefString(process, ReadStringType.AutoDetect, numBytes, out str);
    }

    public bool DerefString(Process process, ReadStringType type, int numBytes, out string str)
    {
        var sb = new StringBuilder(numBytes);
        if (!DerefString(process, type, sb))
        {
            str = null;
            return false;
        }
        str = sb.ToString();
        return true;
    }

    public bool DerefString(Process process, StringBuilder sb)
    {
        return DerefString(process, ReadStringType.AutoDetect, sb);
    }

    public bool DerefString(Process process, ReadStringType type, StringBuilder sb)
    {
        return DerefOffsets(process, out nint ptr) && process.ReadString(ptr, type, sb);
    }

    public bool DerefOffsets(Process process, out IntPtr ptr)
    {
        bool is64Bit;
        if (_derefType == DerefType.Auto)
            is64Bit = process.Is64Bit();
        else
            is64Bit = _derefType == DerefType.Bit64;

        if (!string.IsNullOrEmpty(_module))
        {
            ProcessModuleWow64Safe module = process.ModulesWow64Safe()
                .FirstOrDefault(m => m.ModuleName.ToLower() == _module);
            if (module == null)
            {
                ptr = IntPtr.Zero;
                return false;
            }

            ptr = module.BaseAddress + _base;
        }
        else if (_usingAbsoluteBase)
        {
            ptr = _absoluteBase;
        }
        else
        {
            ptr = process.MainModuleWow64Safe().BaseAddress + _base;
        }

        for (int i = 0; i < _offsets.Count - 1; i++)
        {
            if (!process.ReadPointer(ptr + _offsets[i], is64Bit, out ptr)
                || ptr == IntPtr.Zero)
            {
                return false;
            }
        }

        ptr = ptr + _offsets[_offsets.Count - 1];
        return true;
    }

    private void InitializeOffsets(params OffsetT[] offsets)
    {
        _offsets = new List<OffsetT> { 0 }; // deref base first
        _offsets.AddRange(offsets);
    }
}
