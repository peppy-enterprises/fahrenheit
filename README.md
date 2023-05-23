# Fahrenheit
---

## What is Fahrenheit?
As a project, Fahrenheit **aims to reverse-engineer Final Fantasy X and X-2**, encompassing a
**decompilation effort** combined with **analysis of the binary files that make up the core game logic**.

As this repository, Fahrenheit is a **collection of tools and libraries relating to Final Fantasy X and X-2**,
and the **canonical implementation of information gathered through the analysis process**.

The knowledge gathered by the project underpins many tools for the game, such as the 
[AI/VI TAS](https://github.com/coderwilson/FFX_TAS_Python) and 
[Cutscene Remover](https://github.com/erickt420/FFXCutsceneRemover) mod. Consumers usually
do not reference Fahrenheit libraries directly; instead, they reimplement the bits they need
and tune them for their specific purposes.

Fahrenheit, generally, is the place where mod/tool developers find the specific information they need. If you
want neatly packaged tools, you are probably looking for something that _implements_ Fahrenheit rather
than this project itself.

## Why?
Many people have made tremendous efforts to tackle this game over the years, and some have even
succeeded in creating amazing mods for these timeless games. Unfortunately, almost all such efforts were
made by lone individuals who chose not to publicize their methods and share their knowledge. 
Even when they did, their knowledge may have been lost to time and is exceptionally hard to find today.

Fahrenheit is an attempt to put an end to this. It aims not only to exceed all these efforts in scope,
but also to finally place them in the open where they will be free for others to analyze, improve, learn from,
and use for whatever purpose.

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

## Can I contribute?
Yes. Feel free to join us in the #modding channel of the 
[Final Fantasy X speedrunning Discord](https://discord.gg/tSvM6PUggU).

## License
Fahrenheit is licensed under the [MIT](https://github.com/fkelava/fahrenheit/blob/main/LICENSE.txt) license.

## Associated and/or derived projects
You should check out the following projects if you want finished mods or approachable tools.
- [Karifean](https://github.com/Karifean)'s [FFXDataParser](https://github.com/Karifean/FFXDataParser).
- The [Cutscene Remover mod](https://github.com/erickt420/FFXCutsceneRemover) for FFX by B3dIntruder,
Cereth, CrimsonInferno, Flobber, Roosta, shenef, and myself.

## Some older, still useful projects
- [Farplane](https://github.com/topher-au/Farplane) by [topher-au](https://github.com/topher-au).

