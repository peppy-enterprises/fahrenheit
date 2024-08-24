#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
// Windows Header Files
#include <windows.h>
#include <detours/detours.h>
#include <iostream>
#include <conio.h>

int wmain(int argc, wchar_t* argv[ ]) {
    LPCSTR              szDllPath  = "fhclrldr.dll";
    LPWSTR              cmdLineStr = GetCommandLineW();
    PROCESS_INFORMATION pi;
    STARTUPINFO         si = { 0 };

    si.cb = sizeof(si);

    if (!DetourCreateProcessWithDlls(
        argv[1],
        cmdLineStr,
        NULL,
        NULL,
        FALSE,
        CREATE_SUSPENDED,
        NULL,
        NULL,
        &si,
        &pi,
        1,
        &szDllPath,
        NULL
    )) {
        std::cerr << "[!]" << std::endl;
        return 1;
    }

    std::cout << "Ready. You can now attach a debugger; press any key to launch.";
    int i = _getch();

    ResumeThread(pi.hThread);
    WaitForSingleObject(pi.hProcess, INFINITE);

    DWORD exitCode;
    BOOL  result = GetExitCodeProcess(pi.hProcess, &exitCode);

    CloseHandle(pi.hProcess);
    CloseHandle(pi.hThread);

    return 0;
}
