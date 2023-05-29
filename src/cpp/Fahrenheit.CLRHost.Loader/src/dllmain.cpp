#include "pch.h"
#include "fhdetour.h"

using EntryPoint_T = int(*)(void);

EntryPoint_T ffxMain = NULL;

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    if (DetourIsHelperProcess()) 
    {
        return TRUE;
    }

    switch (ul_reason_for_call)
    {
        case DLL_PROCESS_ATTACH:
        {
            DetourRestoreAfterWith();

            auto hMainModule = reinterpret_cast<HMODULE>(NtCurrentTeb()->ProcessEnvironmentBlock->Reserved3[1]);

            auto pImgDosHeaders = reinterpret_cast<PIMAGE_DOS_HEADER>(hMainModule);
            if (pImgDosHeaders->e_magic != IMAGE_DOS_SIGNATURE) return TRUE;

            auto pImgNTHeaders = reinterpret_cast<PIMAGE_NT_HEADERS>((reinterpret_cast<LPBYTE>(pImgDosHeaders) + pImgDosHeaders->e_lfanew));
            if (pImgNTHeaders->Signature != IMAGE_NT_SIGNATURE) return TRUE;

            ffxMain = reinterpret_cast<EntryPoint_T>(pImgNTHeaders->OptionalHeader.AddressOfEntryPoint + reinterpret_cast<LPBYTE>(hMainModule));

            DetourTransactionBegin();
            DetourTransactionCommit();
        }
        case DLL_THREAD_ATTACH:
        case DLL_THREAD_DETACH:
        case DLL_PROCESS_DETACH:
            break;
    }
    return TRUE;
}

