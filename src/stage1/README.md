# Fahrenheit "Stage 1" Loader

The "Stage 1" Loader, in order:
- Undoes all changes to the IAT and PE header made by the "Stage 0" Loader
    - This is done using MS Detours' [``DetourRestoreAfterWith``](https://github.com/microsoft/detours/wiki/DetourRestoreAfterWith).
- If necessary, overrides the target binary's default exception handler.
- Hooks the entry-point of the target binary.
- Attaches the target process to the "Stage 0" Loader's console.
- Using the API set out in ``hostfxr.h`` and ``nethost.h``, loads the .NET Runtime into the process.
- Jumps to Fahrenheit's boot function, loading Fahrenheit mods.
- After returning to native code, calls the original entry-point of the target, proceeding as normal.

In short, the "Stage 1" Loader is responsible for initializing .NET and subsequently Fahrenheit.
