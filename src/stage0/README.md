The "Stage 0" loader creates a suspended target process and injects the ["Stage 1" loader](https://github.com/fahrenheit-crew/fahrenheit/tree/main/src/stage1)), which bootstraps .NET, and then Fahrenheit. 

It then acts as the game's parent process and standard input/output pipe.

In order, it:
- Creates the target process in ``CREATE_SUSPENDED`` state.
- Modifies the process' PE header and IAT to:
    - Add the Fahrenheit "Stage 1" Loader to the head of the import list, so it executes first.
	- This process is performed using MS Detours' [``DetourCreateProcessWithDll``](https://github.com/microsoft/detours/wiki/DetourCreateProcessWithDll).
- Captures console output from the game.
- Waits for the game process to exit, capturing its exit code.
