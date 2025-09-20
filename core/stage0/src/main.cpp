#include "fhstage0.h"

int wmain(int argc, wchar_t* argv[ ]) {
    LPCSTR              szDllPath  = "fhstage1.dll";
    LPWSTR              cmdLineStr = GetCommandLineW();
    PROCESS_INFORMATION pi;
    STARTUPINFO         si = { 0 };

    si.cb = sizeof(si);

    //
    // STEP 1:
    // Create process in PROCESS_SUSPENDED state.
    //

    if (!CreateProcess(
        argv[1],
        cmdLineStr,
        NULL,
        NULL,
        FALSE,
        CREATE_SUSPENDED,
        NULL,
        NULL,
        &si,
        &pi
    )) {
        std::wcerr << "Failed to create target process.\n";
        return 1;
    }

    //
    // STEP 2:
    // Pause for debugger attach if `--debug` arg is passed.
    //

    if (argc > 2 && wcsncmp(argv[2], L"--debug", 9) == 0) {
        std::wcout << "Stage 0 Loader is ready. You can now attach a debugger; press any key to attempt launch.\n";
        int i = _getch();
    } else {
        std::wcout << "Stage 0 Loader is ready.\n";
    }

    //
    // STEP 3:
    // Patch IAT of suspended process to inject Stage 1 DLL at position 1.
    //

    LPCSTR rlpDlls[2] {};
    DWORD  nDlls = 0;
    if (szDllPath != NULL) {
        rlpDlls[nDlls++] = szDllPath;
    }

    if (!DetourUpdateProcessWithDll(pi.hProcess, rlpDlls, nDlls)) {
        TerminateProcess(pi.hProcess, ~0u);
        return FALSE;
    }

    std::wcout << "Stage 0 Loader complete. Moving to Stage 1.\n";

    //
    // STEP 4:
    // Stage 1 loads first, hooks program entrypoint and performs .NET hosting and
    // initialization, undoes IAT changes, pipes process stdout/stderr to Stage 0
    // console, then program execution proceeds.
    //

    ResumeThread       (pi.hThread);
    WaitForSingleObject(pi.hProcess, INFINITE);

    //
    // STEP 5:
    // Wait for program to (un)naturally terminate.
    //

    DWORD exitCode;
    BOOL  result = GetExitCodeProcess(pi.hProcess, &exitCode);

    CloseHandle(pi.hProcess);
    CloseHandle(pi.hThread);

    if (exitCode != 0) {
        std::wcout << "Process exited with code " << exitCode << std::endl;
        std::wcout << "If reporting an issue, please include any core dump (*.dmp) you see in the game directory.\n";
    }
    else {
        std::wcout << "Process ended by user.\n";
    }

    return exitCode;
}
