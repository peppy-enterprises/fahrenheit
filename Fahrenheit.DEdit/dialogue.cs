using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;

using Fahrenheit.CoreLib;

namespace Fahrenheit.DEdit;

public readonly struct FhDialogueIndex
{
    public readonly T_FhDialoguePos         index;
    public readonly byte                    __0x03;
    public readonly T_FhDialogueOptionCount optionCount;
}

public unsafe struct FhMacroDictHeader
{
    public const int MACRO_DICT_SECTION_NB = 16;
    public fixed int sectionOffset[MACRO_DICT_SECTION_NB];
}

public readonly struct FhMacroDictIndex
{
    public readonly T_FhDialoguePos index;
}

internal static class FhDialogueExtensions
{
    public static FhMacroDictHeader GetMacroDictHeader(this in ReadOnlySpan<byte> dialogue)
    {
        FhMacroDictHeader       header;
        Span<FhMacroDictHeader> destSpan = new Span<FhMacroDictHeader>(ref header);

        FhMarshal.FromBytes(dialogue[0..0x40], destSpan, 0x40, out int count);

        return destSpan[0];
    }

    public static int GetDialogueIndexCount(this in ReadOnlySpan<byte> dialogue)
    {
        return BinaryPrimitives.ReadUInt16LittleEndian(dialogue[0..2]) / Unsafe.SizeOf<FhDialogueIndex>();
    }

    public static int GetMacroDictIndexCount(this in ReadOnlySpan<byte> dialogue)
    {
        return BinaryPrimitives.ReadUInt16LittleEndian(dialogue[0..2]) / Unsafe.SizeOf<FhMacroDictIndex>();
    }

    public static void ReadDialogueIndices(this in ReadOnlySpan<byte> dialogue, in FhDialogueIndex[] callerArray, out int count)
    {
        int endpos = callerArray.Length * Unsafe.SizeOf<FhDialogueIndex>();
        FhMarshal.FromBytes<FhDialogueIndex>(dialogue[0..endpos], callerArray, endpos, out count);
    }

    public static void ReadMacroDictIndices(this in ReadOnlySpan<byte> dialogue, in FhMacroDictIndex[] callerArray, out int count)
    {
        int endpos = callerArray.Length * Unsafe.SizeOf<FhMacroDictIndex>();
        FhMarshal.FromBytes<FhMacroDictIndex>(dialogue[0..endpos], callerArray, endpos, out count);
    }

    internal static void ReadLineInternal(this in ReadOnlySpan<byte> dialogue, in StringBuilder sb, T_FhDialoguePos start, T_FhDialoguePos end)
    {
        ReadOnlySpan<byte> slice = dialogue[start..end];
        
        if (slice.Length == 0) return;
        
        foreach (byte b in slice)
        {
            if (b < 0x30)
                sb.Append($"\\0x{b:X} ");
            else if (b < 0xFF)
                sb.Append(FhCharset.Us.ToChar(b));
            else throw new Exception("E_MALFORMED_INPUT");
        }
        
        sb.AppendLine();
    }

    public static string ReadDialogue(this ReadOnlySpan<byte> dialogue, in FhDialogueIndex[] indices)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < indices.Length - 1; i++)
            dialogue.ReadLineInternal(sb, indices[i].index, indices[i + 1].index);
        
        return sb.ToString();
    }

    public static string ReadMacroDict(this ReadOnlySpan<byte> dialogue, in FhMacroDictIndex[] indices)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < indices.Length - 1; i++)
            dialogue.ReadLineInternal(sb, indices[i].index, indices[i + 1].index);

        return sb.ToString();
    }
}
