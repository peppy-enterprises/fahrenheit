/* [fkelava 17/5/23 02:48]
 * A shitty, quick tool to emit (mostly?!) valid C# from swidx C header. 
 * 
 * Only and specifically used to convert #defines to C# enums for constant imports.
 */

using System;
using System.CommandLine;
using System.IO;

namespace Fahrenheit.DEdit;

internal class Program
{
    static void Main(string[] args)
    {
        Option<FhDEditMode> optMode     = new Option<FhDEditMode>("--mode", "Select the DEdit operating mode.");
        Option<string>      optDefNs    = new Option<string>("--ns", "Set the namespace of the resulting C# file, if reading a charset.");
        Option<string>      optFilePath = new Option<string>("--src", "Set the path to the source file.");
        Option<string>      optDestPath = new Option<string>("--dest", "Set the folder where the C#/text file should be written.");

        optMode.IsRequired     = true;
        optDefNs.IsRequired    = true;
        optFilePath.IsRequired = true;
        optDestPath.IsRequired = true;

        RootCommand rootCmd = new RootCommand("Perform various operations on FFX dialogue files and character sets.")
        {
            optMode,
            optDefNs, 
            optFilePath,
            optDestPath
        };

        rootCmd.SetHandler(DEditMain, new DEditArgsBinder(
            optMode,
            optDefNs,
            optFilePath,
            optDestPath));

        rootCmd.Invoke(args);
        return;
    }

    static void DEditReadCharset()
    {
        if (!File.Exists(DEditConfig.SrcPath))
            throw new Exception("E_INVALID_PATH");

        string sfn = Path.GetFileName(DEditConfig.SrcPath);
        string dfn = Path.Join(DEditConfig.DestPath, $"{sfn}-{Guid.NewGuid()}.g.cs");
        string cs  = FhDEditCharsets.EmitCharset();

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

    static void DEditMain(FhDEditArgs config)
    {
        DEditConfig.CLIRead(config);

        switch (DEditConfig.Mode)
        {
            case FhDEditMode.ReadCharsets:
                DEditReadCharset(); break;
        }
    }
}
