/* [fkelava 17/5/23 02:48]
 * A shitty, quick tool to emit (mostly?!) valid C# from a C header. 
 * 
 * Only and specifically used to convert #defines to C# enums for constant imports.
 */

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Text;

namespace Fahrenheit.H2CS;

internal class Program
{
    static void Main(string[] args)
    {
        Option<string> optDefNs      = new Option<string>("--ns", "Set the namespace of the resulting C# file.");
        Option<bool>   optEmitProlog = new Option<bool>("--emit-prologue", "Whether to emit a prologue. Used when compiling multiple headers into a single C# file.");
        Option<bool>   optEmitDedup  = new Option<bool>("--emit-dedup", "Whether to deduplicate entries. Used when compiling multiple headers into a single C# file.");
        Option<string> optTypeName   = new Option<string>("--typedef", "Set the type alias for the resulting enum.");
        Option<string> optFilePath   = new Option<string>("--src", "Set the path to the source file.");
        Option<string> optDestPath   = new Option<string>("--dest", "Set the folder where the C# file should be written.");

        optDefNs.IsRequired      = true;
        optEmitProlog.IsRequired = true;
        optEmitDedup.IsRequired  = true;
        optTypeName.IsRequired   = true;
        optFilePath.IsRequired   = true;
        optDestPath.IsRequired   = true;

        RootCommand rootCmd = new RootCommand("Process a C header and create a C# code file.")
        {
            optDefNs,
            optEmitProlog,
            optEmitDedup,
            optTypeName,
            optFilePath,
            optDestPath
        };

        rootCmd.SetHandler(H2CSMain, new H2CSArgsBinder(
            optDefNs, 
            optEmitProlog,
            optEmitDedup,
            optTypeName,
            optFilePath,
            optDestPath));

        rootCmd.Invoke(args);
        return;
    }

    static void H2CSMain(FhH2CSArgs config)
    {
        H2CSConfig.CLIRead(config);

        StringBuilder sb = new StringBuilder();
        string        fn = Path.Join(H2CSConfig.DestPath, $"{Path.GetFileName(config.SrcPath)}-{Guid.NewGuid()}.g.cs");

        FhHeaderFile? hf     = default;
        string[]      lines  = File.ReadAllLines(H2CSConfig.SrcPath);
        string        hgline = lines[0];

        if (!hgline.StartsWith("#ifndef") || !hgline.ConstructHeaderGuard(out FhHeaderGuardNode? hg)) 
            throw new Exception("FH_E_H2CS_HG_ILLEGIBLE");
        
        hf = new FhHeaderFile(hg, new List<FhDefineNode>());
        
        foreach (string line in lines[2..])
        {
            if (line.StartsWith("#define") && line.ConstructDefineNode(out FhDefineNode? define)) 
                hf.Defines.Add(define);
        }

        if (!hf.TryEmitHeader(sb))
            throw new Exception("FH_E_H2CS_INTERNAL_FAULT");

        using (FileStream fs = File.Open(fn, FileMode.CreateNew))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(sb.ToString());
            }
        }

        Console.WriteLine(sb.ToString());
    }
}
