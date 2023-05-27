/* [fkelava 17/5/23 02:48]
 * A shitty, quick tool to emit (mostly?!) valid C# from swidx C header. 
 * 
 * Only and specifically used to convert #defines to C# enums for constant imports.
 */

using System;
using System.CommandLine;
using System.IO;
using System.Text;

using Fahrenheit.CoreLib;

namespace Fahrenheit.DEdit;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine($"{nameof(Fahrenheit)}.{nameof(DEdit)}\n");
        Console.WriteLine($"Started with args: {string.Join(' ', args)}\n");

        Option<FhDEditMode> optMode     = new Option<FhDEditMode>("--mode", "Select the DEdit operating mode.");
        Option<string>      optDefNs    = new Option<string>("--ns", "Set the namespace of the resulting C# file, if reading a charset.");
        Option<string>      optFilePath = new Option<string>("--src", "Set the path to the source file.");
        Option<string>      optDestPath = new Option<string>("--dest", "Set the folder where the C#/text file should be written.");
        Option<FhCharsetId> optCharSet  = new Option<FhCharsetId>("--cs", "Set the charset that should be used for the input file.");

        optMode.IsRequired     = true;
        optFilePath.IsRequired = true;
        optDestPath.IsRequired = true;

        RootCommand rootCmd = new RootCommand("Perform various operations on FFX dialogue files and character sets.")
        {
            optMode,
            optDefNs, 
            optFilePath,
            optDestPath,
            optCharSet
        };

        rootCmd.SetHandler(DEditMain, new DEditArgsBinder(
            optMode,
            optDefNs,
            optFilePath,
            optDestPath,
            optCharSet));

        rootCmd.Invoke(args);
        return;
    }

    static void DEditReadCharset()
    {
        string sfn = Path.GetFileName(DEditConfig.SrcPath);
        string dfn = Path.Join(DEditConfig.DestPath, $"{sfn}-{Guid.NewGuid()}.g.cs");
        string ns  = DEditConfig.CharsetReader?.DefaultNamespace ?? throw new Exception("FH_E_MISSING_NAMESPACE: Specify --ns at the command line.");
        string cs  = FhCharsetGenerator.EmitCharset(DEditConfig.SrcPath, ns);

        using (FileStream fs = File.Open(dfn, FileMode.CreateNew))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(cs);
            }
        }

        Console.WriteLine(cs);
        Console.WriteLine($"Charset {sfn}: Output is at {dfn}.");
    }

    static void DEditDecompile()
    {
        if (DEditConfig.Decompile?.CharSet == FhCharsetId.INVALID)
        {
            Console.WriteLine("E_MISSING_CHARSET: Specify --cs at the command line.");
            return;
        }

        string sfn       = Path.GetFileName(DEditConfig.SrcPath);
        bool   isMDict   = sfn == "macrodic.dcp";
        string dfnSuffix = isMDict ? "FFX_MACRODICT" : "FFX_DIALOGUE";
        string dfn       = Path.Join(DEditConfig.DestPath, $"{sfn}-{Guid.NewGuid()}.{dfnSuffix}");

        ReadOnlySpan<byte> dialogue = File.ReadAllBytes(DEditConfig.SrcPath);

        string diastr = isMDict 
            ? DEditDecompileMacroDict(dialogue) 
            : DEditDecompileDialogue(dialogue);

        using (FileStream fs = File.Open(dfn, FileMode.CreateNew))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(diastr);
            }
        }

        Console.WriteLine(diastr);
        Console.WriteLine($"{(isMDict ? "Macro dictionary" : "Dialogue")} {sfn}: Output is at {dfn}.");
    }

    static string DEditDecompileDialogue(in ReadOnlySpan<byte> dialogue)
    {
        int               idxCount = dialogue.GetDialogueIndexCount();
        FhDialogueIndex[] idxArray = new FhDialogueIndex[idxCount];

        dialogue.ReadDialogueIndices(in idxArray, out int readCount);

        if (readCount != idxCount) throw new Exception("E_MARSHAL_FAULT");

        return dialogue.ReadDialogue(idxArray);
    }

    static string DEditDecompileMacroDict(in ReadOnlySpan<byte> dialogue)
    {
        FhMacroDictHeader header = dialogue.GetMacroDictHeader();
        StringBuilder     sb     = new StringBuilder();

        unsafe
        {
            for (int i = 0; i < FhMacroDictHeader.MACRO_DICT_SECTION_NB; i++)
            {
                sb.AppendLine($"\n--- SECTION {i} ---");

                int                offset = header.sectionOffset[i];
                ReadOnlySpan<byte> slice  = dialogue[offset..];

                int                idxCount = slice.GetMacroDictIndexCount();
                FhMacroDictIndex[] idxArray = new FhMacroDictIndex[idxCount];

                slice.ReadMacroDictIndices(in idxArray, out int readCount);

                if (readCount != idxCount) throw new Exception("E_MARSHAL_FAULT");

                sb.Append(slice.ReadMacroDict(idxArray));
                sb.AppendLine($"--- END SECTION {i} ---\n");
            }
        }

        return sb.ToString();
    }

    static void DEditMain(FhDEditArgs config)
    {
        DEditConfig.CLIRead(config);

        if (!File.Exists(DEditConfig.SrcPath))
            throw new Exception("E_INVALID_PATH");

        switch (DEditConfig.Mode)
        {
            case FhDEditMode.ReadCharsets:
                DEditReadCharset(); break;
            case FhDEditMode.Decompile:
                DEditDecompile(); break;
        }
    }
}
