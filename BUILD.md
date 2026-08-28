Ensure the folder you cloned to does not contain special characters. This can cause build errors.

Building **requires** Visual Studio 2026 (full IDE or Build Tools only) with the following workloads:
- .NET desktop development (latest version)
- C++ desktop development (latest version)

Once you have installed Visual Studio, enable the `vcpkg` package manager
by issuing `vcpkg integrate install` at a Developer PowerShell prompt.
Restart Visual Studio if it was open during this process.

To build at a Developer PowerShell:
```
msbuild .\Fahrenheit.slnx /t:Restore /p:Configuration=Release
msbuild .\Fahrenheit.slnx /p:Configuration=Release
```
If using Visual Studio, `Build Solution` performs everything for you.
For a Debug build, change the `Configuration` parameter to `Debug`.

To install/test your development build:
- Create a subfolder in your game directory (where ``FFX.exe`` is) named ``fahrenheit``.
- In the directory in which you cloned Fahrenheit, navigate to ``artifacts\deploy``, then ``dbg`` or ``rel`` depending on build type (Debug or Release).
- Copy its contents (the folders ``bin``, ``mods``, etc.) to the ``fahrenheit`` subfolder in the game directory.
- Create an empty, extensionless file called `loadorder` in the `mods` directory. Add the manifest names of mods you wish to load, each on their own line.
- Open a terminal in ``fahrenheit/bin``, then issue ``.\fhstage0.exe ..\..\FFX.exe``.
- Debugging can be performed from Visual Studio. Attach to either ``fhstage0.exe`` or ``FFX.exe``,
and make sure to enable [mixed-mode debugging](https://learn.microsoft.com/en-us/visualstudio/debugger/how-to-debug-managed-and-native-code?view=vs-2022).
