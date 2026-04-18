# Fahrenheit "Stage 1" Loader

The "Stage 1" Loader executes the following processes, in order:
- Undoes all changes to the PE header made by the "Stage 0" Loader
    - This is done in a completely equivalent manner to MS Detours' ``DetourRestoreAfterWith``.
- Hooks the entry-point of the game executable
- Attaches the game process to the "Stage 0" Loader's console
- Using the API set out in ``hostfxr.h`` and ``nethost.h``, loads the .NET Runtime into the process
- Executes a native-to-managed transition to ``Fahrenheit.CoreLib.FhLoader.ldr_bootstrap``, loading Fahrenheit modules
- After returning to native code, calls the original entry-point of the game, proceeding as normal

In short, it is the "Stage 1" Loader that is responsible for initializing Fahrenheit.
