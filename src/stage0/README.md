# Fahrenheit "Stage 0" Loader

The "Stage 0" Loader, in order:
- Creates the target process in ``CREATE_SUSPENDED`` state.
- Allows for a debugger to be attached to either the target or the Fahrenheit binaries within it.
- Modifies the process' PE header and IAT to:
    - Add the Fahrenheit "Stage 1" Loader to the import list
	- Ensure the Fahrenheit "Stage 1" Loader executes first
	- This process is performed using MS Detours' [``DetourCreateProcessWithDll``](https://github.com/microsoft/detours/wiki/DetourCreateProcessWithDll).
- Captures console output from the game, which the "Stage 1" Loader initializes.
- Then awaits for the game process to exit, capturing its exit code.

In short, the "Stage 0" Loader is responsible for ensuring the "Stage 1" Loader runs before game code, and acts as the game's parent process and standard input/output pipe.

This is done so we do not conflict with mods that use the 'standard' technique of hijacking a DLL already in the target's imports.

We explicitly want to avoid this; the target should launch in a pure, unmodified state when Fahrenheit is not in use.
