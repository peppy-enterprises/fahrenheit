# Fahrenheit
---

## What is Fahrenheit?
To borrow and slightly reword the definition and aim of the excellent [OpenKH](https://github.com/OpenKH/OpenKh) project,

**Fahrenheit aims to centralize all the technical knowledge of the 'Final Fantasy X' and 'Final Fantasy X-2' games in one place, 
providing documentation, tools, code libraries, and the foundation for modding the commercial games.**

The knowledge gathered by the project underpins many tools for the game, such as the 
[AI/VI TAS](https://github.com/coderwilson/FFX_TAS_Python), [Cutscene Remover](https://github.com/erickt420/FFXCutsceneRemover) mod,
[Karifean](https://github.com/Karifean)'s [FFXDataParser](https://github.com/Karifean/FFXDataParser), and more.

Typically, consumers do not reference Fahrenheit libraries directly; instead, they reimplement 
the bits they need and tune them for their specific purposes.

For mod/tool developers, Fahrenheit is where they find the specific information they need.

For end-users, Fahrenheit is a mod loader and base framework that other tools rely upon.

## Why?
Many people have made tremendous efforts to tackle this game over the years, and some have even
succeeded in creating amazing mods for these timeless games. Unfortunately, almost all such efforts were
made by lone individuals who chose not to publicize their methods and share their knowledge. 
Even when they did, their knowledge may have been lost to time and is exceptionally hard to find today.

Fahrenheit is an attempt to put an end to this. It aims not only to exceed all these efforts in scope,
but also to finally place them in the open where they will be free for others to analyze, improve, learn from,
and use.

## What is currently included?
Please consult each individual project's `README` for specific information.

Abridged summaries of each included project are:

- `Fahrenheit.CoreLib`: Provides game constants, structures, and methods to manipulate them. 
The canonical implementation of all game knowledge available to us at present.
- `Fahrenheit.CT2CS`: Converts specially formatted cheat tables to C#. 
Intended only for `CoreLib` development.
- `Fahrenheit.H2CS`: Converts specific C headers to C#.
Intended only for `CoreLib` development.
- `Fahrenheit.DEdit`: A dialogue and character set viewer/editor.
- `Fahrenheit.CLRHost.*`: A reimplementation of [citronneur/detours.net](https://github.com/citronneur/detours.net)
for .NET (Core). Allows detouring with C# code.

## Can I contribute?
Yes. Feel free to join us in the #modding channel of the 
[Final Fantasy X speedrunning Discord](https://discord.gg/tSvM6PUggU).

## License
Fahrenheit is licensed under the [MIT](https://github.com/fkelava/fahrenheit/blob/main/LICENSE.txt) license.

For third-party code license notices, please see 
[THIRD-PARTY-NOTICES](https://github.com/fkelava/fahrenheit/blob/main/THIRD-PARTY-NOTICES.txt).

## Associated and/or derived projects
You should check out the following projects if you want finished mods or approachable tools.
- [Karifean](https://github.com/Karifean)'s [FFXDataParser](https://github.com/Karifean/FFXDataParser).
- The [Cutscene Remover mod](https://github.com/erickt420/FFXCutsceneRemover) for FFX by B3dIntruder,
Cereth, CrimsonInferno, Flobber, Roosta, shenef, and myself.

## Some older, still useful projects
- [Farplane](https://github.com/topher-au/Farplane) by [topher-au](https://github.com/topher-au).

