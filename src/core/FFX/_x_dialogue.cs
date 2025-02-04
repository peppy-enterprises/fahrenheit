// Deprecated until further notice.

using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;

using Fahrenheit.Core.FFX;

namespace Fahrenheit.Core;

public static class FhDialogueUtil {
    public static FhMacroDictHeader GetMacroDictHeader(this in ReadOnlySpan<byte> dialogue) {
        FhMacroDictHeader header = new FhMacroDictHeader();

        FhUtil.cast_from_bytes(dialogue[0..FhMacroDictHeader.MD_HEADER_SIZE], header.SectionOffsets.AsSpan(), FhMacroDictHeader.MD_HEADER_SIZE, out _);

        return header;
    }

    public static int GetDialogueIndexCount(this in ReadOnlySpan<byte> dialogue) {
        return BinaryPrimitives.ReadUInt16LittleEndian(dialogue[0..2]) / Unsafe.SizeOf<FhDialogueIndex>();
    }

    public static int GetMacroDictIndexCount(this in ReadOnlySpan<byte> dialogue) {
        return BinaryPrimitives.ReadUInt16LittleEndian(dialogue[0..2]) / Unsafe.SizeOf<FhMacroDictIndex>();
    }

    public static void ReadDialogueIndices(this in ReadOnlySpan<byte> dialogue, in FhDialogueIndex[] callerArray, out int count) {
        int endpos = callerArray.Length * Unsafe.SizeOf<FhDialogueIndex>();
        FhUtil.cast_from_bytes<FhDialogueIndex>(dialogue[0..endpos], callerArray, endpos, out count);
    }

    public static void ReadMacroDictIndices(this in ReadOnlySpan<byte> dialogue, in FhMacroDictIndex[] callerArray, out int count) {
        int endpos = callerArray.Length * Unsafe.SizeOf<FhMacroDictIndex>();
        FhUtil.cast_from_bytes<FhMacroDictIndex>(dialogue[0..endpos], callerArray, endpos, out count);
    }

    internal static void ReadLineInternal(this in ReadOnlySpan<byte> dialogue, in FhLangId cs, in StringBuilder sb, T_FhDialoguePos start, T_FhDialoguePos end) {
        ReadOnlySpan<byte> slice = dialogue[start..end];

        if (slice.Length == 0) return;

        foreach (byte b in slice) {
            if (b < 0x30)
                sb.Append($"\\0x{b:X} ");
            else if (b < 0xFF)
                sb.Append(ResolveChar(cs, b));
            else throw new Exception("E_MALFORMED_INPUT");
        }

        sb.AppendLine();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static char ResolveChar(FhLangId cs, byte b) {
        // We check that CharSet is not invalid in DEditDecompile().
        return cs switch {
            FhLangId.English => FhCharset.Us.to_char(b),
            _                => throw new Exception("E_INVALID_CHARSET_ID")
        };
    }

    public static string ReadDialogue(this ReadOnlySpan<byte> dialogue, FhLangId cs, in FhDialogueIndex[] indices) {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < indices.Length - 1; i++)
            dialogue.ReadLineInternal(cs, sb, indices[i].index, indices[i + 1].index);

        return sb.ToString();
    }

    public static string ReadMacroDict(this ReadOnlySpan<byte> dialogue, FhLangId cs, in FhMacroDictIndex[] indices) {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < indices.Length - 1; i++)
            dialogue.ReadLineInternal(cs, sb, indices[i].index, indices[i + 1].index);

        return sb.ToString();
    }
}