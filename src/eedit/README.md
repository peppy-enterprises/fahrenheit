# Fahrenheit Excel File Editor (EEdit)

EEdit is a standalone application that utilizes Fahrenheit's
``{WIP}`` API to perform transformations on 'Excel' files,
structure containers that contain data on things like accessories,
monsters, commands and much more. Resulting files can be distributed
alongside Fahrenheit mods and replace original game files using the External File Loader.

All transformations EEdit offers are also available at runtime for Fahrenheit mods.

### Obtaining releases

Releases are available in the
[main Fahrenheit repository](https://github.com/fahrenheit-crew/fahrenheit/releases).

### Building from source

EEdit is built alongside Fahrenheit. Clone the
[main Fahrenheit repository](https://github.com/fahrenheit-crew/fahrenheit/releases) to begin,
and build it using the instructions in that repository.

### Usage

The simplest and recommended way to use EEdit is to place arguments
into a response file, one argument per line, then invoke EEdit as such:
```
PS> cat test.rsp
{WIP}
```
```
.\fheedit.exe '@test.rsp'
```

At a minimum you must specify {WIP}.

The following commands and arguments are available:
```
Usage:
  fheedit [command] [options]

{WIP}
```
