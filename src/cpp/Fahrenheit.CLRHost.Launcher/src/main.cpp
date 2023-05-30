#include <iostream>
#include "fhdetour.h"

int wmain(int argc, wchar_t* argv[ ])
{
    PROCESS_INFORMATION processInfo;
    STARTUPINFO         startupInfo = { 0 };

    LPCWSTR szExePath = L"D:\\ffx\\ffx.exe";
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

    ResumeThread(processInfo.hThread);
	return 0;
}
