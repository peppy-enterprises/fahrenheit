# Fahrenheit Dialogue Editor (DEdit)

DEdit is a standalone application that utilizes Fahrenheit's
``FhEncoding`` API to perform transformations between UTF-8
and game encoding. Resulting dialogue files can be distributed
alongside Fahrenheit mods and replace original game dialogue
using the External File Loader.

All transformations DEdit offers are also available at runtime for Fahrenheit mods.

### Obtaining releases

Releases are available in the
[main Fahrenheit repository](https://github.com/fahrenheit-crew/fahrenheit/releases).

### Building from source

DEdit is built alongside Fahrenheit. Clone the
[main Fahrenheit repository](https://github.com/fahrenheit-crew/fahrenheit/releases) to begin,
and build and deploy it using the instructions in that repository.

### Usage

The simplest and recommended way to use DEdit is to place arguments
into a response file, one argument per line, then invoke DEdit as such:
```
PS> cat test.rsp
decompile
-s
4:811400
-e
UTF8
-o
.
-l
English
-g
FFX
-it
I32_X1
-i
"..\env\ffx_ps2\ffx\proj\battle\jp\cddata\cdrom.fnd"
```
```
.\fhdedit.exe '@test.rsp'
```

At a minimum you must specify any number of input files (`-i`),
encoding mode (`-e`), game type (`-g`), game language (`-l`), index type
(`-it`), and an output folder (`-o`). The output folder must already exist.

For examples, refer to any scripts marked `dedit_*.rsp` in the Fahrenheit
repository's [scripts directory](https://github.com/fahrenheit-crew/fahrenheit/tree/main/scripts).

The following commands and arguments are available:
```
Usage:
  fhdedit [command] [options]

Options:
  -?, -h, --help                                              Show help and usage information
  --version                                                   Show version information
  -i, --input                                                 Input file(s) to process.
  -o, --output                                                What folder to emit outputs to. The folder must already
                                                              exist.
  -s, --segment                                               Which part of the file to interpret. Specify byte offsets
                                                              as START:END.
  -e, --encoding <GAME|NULL|UTF8>                             What encoding to interpret the input file as having.
  -l, --lang                                                  Set the language to interpret the input file as.
  <Chinese|Debug|English|French|German|Italian|Japanese|Kore
  an|Spanish>
  -it, --index-type <COM_X1|COM_X2|I16_X1|I16_X2|I32_X1>      Set the indexing type for the dialogue file in question.
  -g, --game-type <FFX|FFX2|FFX2LM|NULL>                      Set the game type for the dialogue file in question.
  -m, --macro-dict                                            The dictionary to use to resolve any encountered macro
                                                              refs.

Commands:
  decompile  Decompiles a dialogue file.
  compile    Compiles a dialogue file.
```
