# Fahrenheit Symbol Table Parser (STEP)

STEP is a standalone utility that processes CSV output from Ghidra,
alongside supplementary information, to generate `FhCall` delegates
for all functions in the game executables.

It is not shipped alongside Fahrenheit releases; it is only used for,
and therefore only available during, development.

### Usage

The following commands and arguments are available:
```
Usage:
  fhstep [options]

Options:
  -?, -h, --help                                   Show help and usage information
  --version                                        Show version information
  -d, --data (REQUIRED)                            Set the path to the file containing data definitions.
  -f, --functions (REQUIRED)                       Set the path to the file containing function definitions.
  -o, --output (REQUIRED)                          Set the folder where the C# file should be written.
  -m, --map                                        Set the path to a Ghidra -> Fh type map.
  -ne, --no-emit                                   Specify a set of addresses for which calls shall not be emitted.
  -g, --game-id <FFX|FFX2|FFX2LM|NULL> (REQUIRED)  Declare which game STEP is generating for.

```

The simplest and recommended way to use STEP is to place arguments
into a response file, one argument per line, then invoke STEP as such:
```
PS> cat test.rsp
-d
"data/globals.ffx.csv"
-f
"data/functions.ffx.csv"
-m
"data/typemap.ffx.json"
-ne
@data/noemit.ffx.rsp
-g
"FFX"
-o
.
```
```
.\fhstep.exe '@test.rsp'
```

At a minimum you must specify input globals (`-d`), input functions (`-f`),
game type (`-g`), and an output folder (`-o`). The output folder must already exist.

You may optionally provide a set of no-emit addresses that STEP shall ignore (`-ne`)
and a type map for argument type remapping (`-m`).

## Input file formats

No-emit addresses must be given in form `0x{ADDR:X}`, ex. `0x207D80`.
An example is given [here](https://github.com/fahrenheit-crew/fahrenheit/blob/main/src/step/data/noemit.ffx.rsp).

A type map file is a plain JSON dictionary with string keys and values. An example is given
[here](https://github.com/fahrenheit-crew/fahrenheit/blob/main/src/step/data/typemap.ffx.json).

A function input CSV must conform to `FhFuncDecl`.
See [here](https://github.com/fahrenheit-crew/fahrenheit/blob/main/src/step/main.cs#L15).

A data label/global input CSV must conform to `FhDataLabelDecl`.
See [here](https://github.com/fahrenheit-crew/fahrenheit/blob/main/src/step/main.cs#L34).
