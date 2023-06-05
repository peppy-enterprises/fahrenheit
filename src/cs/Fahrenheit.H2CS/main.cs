/* [fkelava 17/5/23 02:48]
 * A shitty, quick tool to emit (mostly?!) valid C# from a C header. 
 * 
 * Only and specifically used to convert #defines to C# enums for constant imports.
 */

using System;
using System.CommandLine;
using System.IO;
using System.Text;

namespace Fahrenheit.H2CS;

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

        RootCommand rootCmd = new RootCommand("Process a C header and create a C# code file.")
        {
            optDefNs, 
            optFilePath,
            optDestPath
        };

        rootCmd.SetHandler(H2CSMain, new H2CSArgsBinder(
            optDefNs, 
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
