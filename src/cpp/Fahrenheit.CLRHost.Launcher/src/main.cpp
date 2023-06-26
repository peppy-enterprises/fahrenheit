#include <iostream>
#include <conio.h>
#include "fhdetour.h"

int wmain(int argc, wchar_t* argv[ ])
{
    LPCSTR szDllPath = "fhclrldr.dll";

    PROCESS_INFORMATION processInfo;
    STARTUPINFO         startupInfo = { 0 };

    startupInfo.cb = sizeof(startupInfo);

    std::wstring commandLine = argv[1];
    for (int i = 2; i < argc; i++)
    {
        commandLine += TEXT(" ") + std::wstring(argv[i]);
    }

    if (!DetourCreateProcessWithDlls(
        NULL,
        const_cast<wchar_t*>(commandLine.c_str()),
        NULL,
        NULL,
        FALSE,
        CREATE_SUSPENDED,
        NULL,
        NULL,
        &startupInfo,
        &processInfo,
        1,
        &szDllPath,
        NULL
    ))
    {
        std::cerr << "[!]" << std::endl;
        return 1;
    }

    std::cout << "Ready. You can now attach a debugger; press any key to launch.";
    int i = _getch();

    ResumeThread(processInfo.hThread);
    WaitForSingleObject(processInfo.hProcess, INFINITE);

    DWORD exitCode;
    BOOL  result = GetExitCodeProcess(processInfo.hProcess, &exitCode);

    CloseHandle(processInfo.hProcess);
    CloseHandle(processInfo.hThread);

	return 0;
}
