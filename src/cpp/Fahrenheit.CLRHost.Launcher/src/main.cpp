#include "fhlaunch.h"

int wmain(int argc, wchar_t* argv[ ]) {
    LPCSTR              szDllPath     = "fhclrldr.dll";
    LPWSTR              cmdLineStr    = GetCommandLineW();
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
        std::cerr << "Failed to create target process." << std::endl;
        return 1;
    }

    std::cout << "Ready. You can now attach a debugger; press any key to attempt launch.";
    int i = _getch();

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

    ResumeThread       (pi.hThread);
    WaitForSingleObject(pi.hProcess, INFINITE);

    DWORD exitCode;
    BOOL  result = GetExitCodeProcess(pi.hProcess, &exitCode);

    CloseHandle(pi.hProcess);
    CloseHandle(pi.hThread);

    return exitCode;
}
