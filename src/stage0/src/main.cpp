#include "fhstage0.h"

int wmain(int argc, wchar_t* argv[ ]) {
    LPCSTR              szDllPath  = "fhstage1.dll";
    LPWSTR              cmdLineStr = GetCommandLineW();
    PROCESS_INFORMATION pi;
    STARTUPINFO         si = { 0 };

    si.cb = sizeof(si);

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
        std::cerr << "Failed to create target process.\n";
        return 1;
    }

    if (argc > 2 && wcsncmp(argv[2], L"--nodebug", 9) == 0) {
        std::wcout << "Stage 0 Loader is ready. You can now attach a debugger; press any key to attempt launch.\n";
        int i = _getch();
    } else {
        std::wcout << "Stage 0 Loader is ready.\n";
    }

    // copied verbatim for safety's sake, stupid as it is
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

    ResumeThread       (pi.hThread);
    WaitForSingleObject(pi.hProcess, INFINITE);

    DWORD exitCode;
    BOOL  result = GetExitCodeProcess(pi.hProcess, &exitCode);

    CloseHandle(pi.hProcess);
    CloseHandle(pi.hThread);

    std::cout << "Process exited with code " << exitCode << std::endl;
    return exitCode;
}
