# Fahrenheit

A Final Fantasy X/X-2 reverse-engineering project and mod framework.

![banner](https://raw.githubusercontent.com/fahrenheit-crew/fahrenheit/refs/heads/main/assets/fh_banner.png)

## What is Fahrenheit?
Fahrenheit is a reverse-engineering project and mod framework for the [Final Fantasy X and X-2 HD Remasters](https://store.steampowered.com/app/359870/).

It allows you to freely hook game functions and distribute mods in the form of loadable DLLs.
Fahrenheit hosts the [.NET runtime](https://dotnet.microsoft.com/en-us/download)
within the games, allowing you to write mods in any compatible language.

The knowledge gathered by the project underpins many tools and mods for the game, such as the
[AI/VI TAS](https://github.com/coderwilson/FFX_TAS_Python), [Cutscene Remover](https://github.com/erickt420/FFXCutsceneRemover) mod,
[Karifean](https://github.com/Karifean)'s [FFXDataParser](https://github.com/Karifean/FFXDataParser), and more.
Fahrenheit, like all of these tools, is free for you to analyze, improve, learn from and use- now and forever.

## Build and deploy
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
- In the directory in which you cloned Fahrenheit, navigate to the ``artifacts\deploy`` subdirectory.
- Depending on build type (Debug or Release), navigate to the ``dbg`` or ``rel`` subdirectory.
- Copy the contents of that directory (the folders ``bin``, ``mods``, etc.) to the ``fahrenheit`` subfolder in the game directory.
- Create an empty, extensionless file called `loadorder` in the `mods` directory. Add the manifest names of mods you wish to load, each on their own line.
- Open a terminal in ``fahrenheit/bin``, then issue ``.\fhstage0.exe ..\..\FFX.exe``.
- Debugging can be performed from Visual Studio. Attach to either ``fhstage0.exe`` or ``FFX.exe``,
and make sure to enable [mixed-mode debugging](https://learn.microsoft.com/en-us/visualstudio/debugger/how-to-debug-managed-and-native-code?view=vs-2022).

## Compatibility notes
Fahrenheit is incompatible with ffgriever's
[External File Loader for FFX/FFX-2](https://gitlab.com/ffgriever/ffx-x-2-hd-external-file-loader).
Fahrenheit comes with an integrated external file loader. Existing file-based mods must be converted to Fahrenheit format.

If you use [Untitled Project X](https://github.com/Kaldaien/UnX) with Fahrenheit,
you **must** patch the game executable to be large-address-aware (apply the "4GB patch"). If you don't, you will run out of memory at boot.

## What's next?
Time permitting, the goals (in no specific order) of the project are:
- Provide actual code-behind, helper functions, and tooling to make various modding tasks approachable.
- Provide a mod manager for end users who simply want to enjoy the game.
- Provide quality documentation for various implementation-specific details and game systems.
- In general, _polish_ every aspect of the solution.

## Can I contribute?
Yes. Feel free to join us in [Cid's Salvage Ship](https://discord.gg/AGx2grw9nD), a Discord server that supports Fahrenheit and related efforts.

## License
Fahrenheit source code is licensed under the [LGPL 3.0 or later](https://github.com/fahrenheit-crew/fahrenheit/blob/main/COPYING.LESSER) license.

Assets (the contents of the ``assets`` folder) may be used in forks of Fahrenheit, but _not_ for any other purpose!

For third-party code license notices, please see
[THIRD-PARTY-NOTICES](https://github.com/fahrenheit-crew/fahrenheit/blob/main/THIRD-PARTY-NOTICES).
