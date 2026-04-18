# Fahrenheit "Stage 0" Loader

The "Stage 0" Loader executes the following processes, in order:
- Creates the game process in ``CREATE_SUSPENDED`` state.
- Allows for a debugger to be attached to either the game or the Fahrenheit binaries within it.
- Modifies the process' PE header, IAT, and other headers to:
    - Add the Fahrenheit "Stage 1" Loader to the import list
	- Ensure the Fahrenheit "Stage 1" Loader executes before any game code
	- This process is performed in a completely equivalent manner to MS Detours' ``DetourCreateProcessWithDll``.
- Captures console output from the game, which the "Stage 1" Loader initializes
- Then awaits for the game process to exit, capturing its exit code.

In short, the "Stage 0" Loader is responsible for ensuring the Fahrenheit "Stage 1" Loader runs before game code, and acts as the game's parent process and standard input/output pipe.
