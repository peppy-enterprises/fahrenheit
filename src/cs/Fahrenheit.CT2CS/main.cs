/* [fkelava 17/5/23 02:48]
 * A shitty, quick tool to emit (mostly?!) valid C# from a properly formatted cheat table.
 */

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Fahrenheit.CT2CS;

internal class Program
{
    static void Main(string[] args)
    {
        Option<string> optDefNs    = new Option<string>("--ns", "Set the namespace of the resulting C# file.");
        Option<string> optFilePath = new Option<string>("--src", "Set the path to the source file.");
        Option<string> optDestPath = new Option<string>("--dest", "Set the folder where the C# file should be written.");

        optDefNs.IsRequired    = true;
        optFilePath.IsRequired = true;
        optDestPath.IsRequired = true;

        RootCommand rootCmd = new RootCommand("Process a cheat table and create a C# code file.")
        {
            optDefNs, 
            optFilePath,
            optDestPath
        };

        rootCmd.SetHandler(CT2CSMain, new CT2CSArgsBinder(
            optDefNs, 
            optFilePath,
            optDestPath));

        rootCmd.Invoke(args);
        return;
    }

    static void CT2CSMain(FhCT2CSArgs config)
    {
        CT2CSConfig.CLIRead(config);

        using (XmlReader xmlr = XmlReader.Create(config.SrcPath))
        {
            xmlr.ReadToDescendant("CheatEntry");

            XmlSerializer xmls = new XmlSerializer(typeof(FhCtEntry), new XmlRootAttribute("CheatEntry"));
            FhCtEntry     ct   = xmls.Deserialize(xmlr) as FhCtEntry ?? throw new Exception("E_CAST_FAILED");

            List<FhSyntaxNode> ast = new List<FhSyntaxNode>
            {
                ct.TryConstructSyntaxNode(out FhSyntaxNode? rsn) ? rsn : throw new Exception("E_MALFORMED_CHEAT_TABLE")
            };

            foreach (FhCtEntry entry in ct.CheatEntries)
            {
                if (entry.TryConstructSyntaxNode(out FhSyntaxNode? sn))
                    ast.Add(sn);
            }

            foreach (FhSyntaxNode node in ast)
            {
                if (node is not FhStructNode stn)
                    continue;

                StringBuilder sb = new StringBuilder();
                string        fn = Path.Join(CT2CSConfig.DestPath, $"{Path.GetFileName(config.SrcPath)}-{Guid.NewGuid()}.g.cs");

                if (!stn.TryEmitStruct(sb))
                    throw new Exception("E_EMIT_FAULT");

                using (FileStream fs = File.Open(fn, FileMode.CreateNew))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write(sb.ToString());
                    }
                }

                Console.WriteLine(sb.ToString());
                Console.WriteLine($"Struct {stn.Name}: Output is at {fn}.");
            }
        }
    }
}
