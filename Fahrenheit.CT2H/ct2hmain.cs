/* [fkelava 17/5/23 02:48]
 * A shitty, quick tool to emit (mostly?!) valid C# from a properly formatted cheat table.
 */

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Fahrenheit.CT2H;

internal class Program
{
    static void Main(string[] args)
    {
        Option<string> optDefaultNamespace = new Option<string>("--ns", "Set the namespace of the resulting C# file.");
        Option<string> optFilePath         = new Option<string>("--src", "Set the path to the source file.");

        optDefaultNamespace.IsRequired = true;
        optFilePath.IsRequired         = true;

        RootCommand rootCmd = new RootCommand("Process a cheat table and create a C# code file.")
        {
            optDefaultNamespace, 
            optFilePath
        };

        rootCmd.SetHandler(CT2HMain, new CT2HArgsBinder(
            optDefaultNamespace, 
            optFilePath));

        rootCmd.Invoke(args);
        return;
    }

    static void CT2HMain(FhCT2HArgs config)
    {
        CT2HConfig.CLIRead(config);

        using (XmlReader xmlr = XmlReader.Create(config.FilePath))
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
                string        fn = $"{node.Name}-{Guid.NewGuid()}.cs";

                if (!stn.TryEmitStruct(sb))
                    throw new Exception("E_EMIT_FAULT");

                //using (FileStream fs = File.Open(fn, FileMode.CreateNew))
                //{
                //    using (StreamWriter sw = new StreamWriter(fs))
                //    {
                //        sw.Write(sb.ToString());
                //    }
                //}

                Console.WriteLine(sb.ToString());
                Console.WriteLine($"Struct {stn.Name}: Output is at {fn}.");
            }
        }
    }
}
