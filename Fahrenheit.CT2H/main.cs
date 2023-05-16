using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml.Serialization;

namespace Fahrenheit.CT2H;

public static class FhCheatEntryExtensions
{
    public static bool IsStructDef(this FhCheatEntry ce)
    {
        return ce.GroupHeader == 1;
    }

    public static bool IsVar(this FhCheatEntry ce)
    {
        return ce.TryGetVarTypeName(out _);
    }

    public static bool IsVarBitfield(this FhCheatEntry ce)
    {
        return ce.VariableType == "Binary";
    }

    public static bool TryConstructSyntaxNode(this FhCheatEntry ce, [NotNullWhen(true)] out FhSyntaxNode? sn)
    {
        sn = default;

        if (ce.TryConstructStructNode(out FhStructNode? stn))
        {
            sn = stn;
            return true;
        }

        if (ce.TryConstructVarNode(out FhVarNode? vn))
        {
            sn = vn;
            return true;
        }

        return false;
    }

    public static bool TryConstructStructNode(this FhCheatEntry ce, [NotNullWhen(true)] out FhStructNode? sn)
    {
        sn = default;
        if (!ce.IsStructDef()) return false;

        sn = new FhStructNode(ce.VarNameFromDescr(), ce.StructSizeFromDescr(), new List<FhSyntaxNode>());
        return true;
    }

    public static bool TryConstructVarNode(this FhCheatEntry ce, [NotNullWhen(true)] out FhVarNode? vn)
    {
        vn = default;
        if (!ce.IsVar()) return false;

        string vname = ce.VarNameFromDescr();
        if (!ce.TryGetFieldOffset(out string voffs)) return false;

        vn = ce.VariableType == "Binary"
            ? new FhBitfieldVarNode(vname, voffs, ce.BitStart, ce.BitLength)
            : new FhStandardVarNode(vname, voffs, ce.TryGetVarTypeName(out string vtype) ? vtype : throw new Exception("E_NO_TYPE"));
        return true;
    }

    public static bool TryGetVarTypeName(this FhCheatEntry ce, out string varTypeName)
    {
        varTypeName = ce.VariableType switch
        {
            "Byte"    => ce.ShowAsSigned == 1 ? "sbyte" : "byte",
            "2 Bytes" => ce.ShowAsSigned == 1 ? "short" : "ushort",
            "4 Bytes" => ce.ShowAsSigned == 1 ? "int" : "uint",
            "8 Bytes" => ce.ShowAsSigned == 1 ? "long" : "ulong",
            "Float"   => "float",
            _         => string.Empty
        };

        return varTypeName != string.Empty;
    }

    public static string VarNameFromDescr(this FhCheatEntry ce)
    {
        string descr = ce.Description;
        return descr[0..descr.IndexOf(' ')];
    }

    public static string StructSizeFromDescr(this FhCheatEntry ce)
    {
        string descr = ce.Description;
        return descr[descr.IndexOf('(')..descr.IndexOf(')')];
    }

    public static bool TryGetFieldOffset(this FhCheatEntry ce, out string offsetStr)
    {
        string addr = ce.Address;
        offsetStr = addr[1..];
        return addr[0] == '+' && uint.TryParse(offsetStr, NumberStyles.HexNumber, null, out _);
    }
}

public        record FhSyntaxNode(string Name);
public sealed record FhStructNode(string Name, string Size, List<FhSyntaxNode> Fields) : FhSyntaxNode(Name);
public        record FhVarNode(string Name, string Offset) : FhSyntaxNode(Name);
public sealed record FhBitfieldVarNode(string Name, string Offset, uint BitStart, uint BitLength) : FhVarNode(Name, Offset);
public sealed record FhStandardVarNode(string Name, string Offset, string TypeName) : FhVarNode(Name, Offset);

[XmlType("CheatEntry")]
public sealed class FhCheatEntry
{
    public uint               ID           { get; init; }
    public string             Description  { get; init; } = string.Empty;
    public uint               ShowAsSigned { get; init; }
    public string             VariableType { get; init; } = string.Empty;
    public string             Address      { get; init; } = string.Empty;
    public List<FhCheatEntry> CheatEntries { get; init; } = new List<FhCheatEntry>();
    public uint               BitStart     { get; init; }
    public uint               BitLength    { get; init; }
    public uint               GroupHeader  { get; init; }
}

internal class Program
{
    static void Main(string[] args)
    {
        // Placeholder.
    }
}
