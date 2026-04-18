// SPDX-License-Identifier: MIT

using System.Text;

namespace Fahrenheit.Tools.Atelier;

internal static class Program {

    private static int Main(string[] args) {

        // ---
        // ARGPARSE START
        // ---
        Option<string> opt_src_path  = new Option<string>("--src")
            { Description = "Set the path to the ATEL source file to compile." };
        Option<string> opt_dest_path = new Option<string>("--dest")
            { Description = "Set the folder where the compiled ATEL script should be written." };

        opt_src_path .Required = true;
        opt_dest_path.Required = true;

        RootCommand root_cmd = new RootCommand("Process a ATEL source file and compile it.") {
            opt_src_path,
            opt_dest_path,
        };

        ParseResult argparse_result = root_cmd.Parse(args);

        string src_path  = argparse_result.GetValue(opt_src_path)  ?? "";
        string dest_path = argparse_result.GetValue(opt_dest_path) ?? "";
        // ---
        // ARGPARSE END
        // ---

        _run(src_path);
        return 0;
    }

    private static void _run(string src_path) {
        AtelLexer       scanner = new(File.ReadAllText(src_path, Encoding.UTF8));
        List<AtelToken> tokens  = scanner.get_tokens();

        foreach (AtelToken token in tokens) {
            Console.WriteLine(token);
        }

        AtelParser parser = new AtelParser(tokens);

        try {
            List<AtelStmt> statements = parser.parse();
        }
        catch (AtelParseException ex) {
            Console.WriteLine(ex.Message);
            return;
        }

        AtelAssembler asm = new AtelAssembler();
    }
}
