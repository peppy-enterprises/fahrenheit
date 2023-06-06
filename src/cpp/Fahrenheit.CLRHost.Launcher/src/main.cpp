#include <iostream>
#include <conio.h>
#include "fhdetour.h"

int wmain(int argc, wchar_t* argv[ ])
{
    PROCESS_INFORMATION processInfo;
    STARTUPINFO         startupInfo = { 0 };

    LPCWSTR szExePath = L"..\\..\\ffx.exe";
    LPCSTR  szDllPath = "fhclrldr.dll";

    if (!DetourCreateProcessWithDlls(
        szExePath,
        NULL,
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
	return 0;
}
