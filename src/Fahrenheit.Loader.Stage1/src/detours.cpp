/* [fkelava 25/8/24 01:42]
 * Substantively copied from MS Detours' `modules.cpp`, `detours.cpp` (https://github.com/microsoft/Detours/), used under the MIT license.
 *
 * See THIRD-PARTY-NOTICES for the license.
 */

#include "fhstage1.h"

const GUID DETOUR_EXE_RESTORE_GUID = {
    0xbda26f34, 0xbc82, 0x4829,
    { 0x9e, 0x64, 0x74, 0x2c, 0x4, 0xc8, 0x4f, 0xa0 } };

BOOL WINAPI DetourAreSameGuid(
    _In_ REFGUID left,
    _In_ REFGUID right)
{
    return
        left.Data1    == right.Data1    &&
        left.Data2    == right.Data2    &&
        left.Data3    == right.Data3    &&
        left.Data4[0] == right.Data4[0] &&
        left.Data4[1] == right.Data4[1] &&
        left.Data4[2] == right.Data4[2] &&
        left.Data4[3] == right.Data4[3] &&
        left.Data4[4] == right.Data4[4] &&
        left.Data4[5] == right.Data4[5] &&
        left.Data4[6] == right.Data4[6] &&
        left.Data4[7] == right.Data4[7];
}

static PDETOUR_LOADED_BINARY WINAPI GetPayloadSectionFromModule(HMODULE hModule)
{
    PIMAGE_DOS_HEADER pDosHeader = (PIMAGE_DOS_HEADER)hModule;
    if (hModule == NULL) {
        pDosHeader = (PIMAGE_DOS_HEADER)GetModuleHandleW(NULL);
    }

    __try {
#pragma warning(suppress:6011) // GetModuleHandleW(NULL) never returns NULL.
        if (pDosHeader->e_magic != IMAGE_DOS_SIGNATURE) {
            SetLastError(ERROR_BAD_EXE_FORMAT);
            return NULL;
        }

        PIMAGE_NT_HEADERS pNtHeader = (PIMAGE_NT_HEADERS)((PBYTE)pDosHeader +
                                                          pDosHeader->e_lfanew);
        if (pNtHeader->Signature != IMAGE_NT_SIGNATURE) {
            SetLastError(ERROR_INVALID_EXE_SIGNATURE);
            return NULL;
        }
        if (pNtHeader->FileHeader.SizeOfOptionalHeader == 0) {
            SetLastError(ERROR_EXE_MARKED_INVALID);
            return NULL;
        }

        PIMAGE_SECTION_HEADER pSectionHeaders
            = (PIMAGE_SECTION_HEADER)((PBYTE)pNtHeader
                                      + sizeof(pNtHeader->Signature)
                                      + sizeof(pNtHeader->FileHeader)
                                      + pNtHeader->FileHeader.SizeOfOptionalHeader);

        for (DWORD n = 0; n < pNtHeader->FileHeader.NumberOfSections; n++) {
            if (strcmp((PCHAR)pSectionHeaders[n].Name, ".detour") == 0) {
                if (pSectionHeaders[n].VirtualAddress == 0 ||
                    pSectionHeaders[n].SizeOfRawData == 0) {
                    break;
                }

                PBYTE pbData = (PBYTE)pDosHeader + pSectionHeaders[n].VirtualAddress;
                DETOUR_SECTION_HEADER *pHeader = (DETOUR_SECTION_HEADER *)pbData;
                if (pHeader->cbHeaderSize <  sizeof(DETOUR_SECTION_HEADER) ||
                    pHeader->nSignature   != DETOUR_SECTION_HEADER_SIGNATURE) {

                    break;
                }

                if (pHeader->nDataOffset == 0) {
                    pHeader->nDataOffset = pHeader->cbHeaderSize;
                }
                SetLastError(NO_ERROR);
                return (PBYTE)pHeader;
            }
        }
        SetLastError(ERROR_EXE_MARKED_INVALID);
        return NULL;
    }
    __except(GetExceptionCode() == EXCEPTION_ACCESS_VIOLATION ?
             EXCEPTION_EXECUTE_HANDLER : EXCEPTION_CONTINUE_SEARCH) {
        SetLastError(ERROR_EXE_MARKED_INVALID);
        return NULL;
    }
}

_Writable_bytes_(*pcbData)
_Readable_bytes_(*pcbData)
_Success_       (return != NULL)
PVOID WINAPI DetourFindPayload(
    _In_opt_  HMODULE hModule,
    _In_      REFGUID rguid,
    _Out_opt_ DWORD   *pcbData)
{
    PBYTE pbData = NULL;
    if (pcbData) {
        *pcbData = 0;
    }

    PDETOUR_LOADED_BINARY pBinary = GetPayloadSectionFromModule(hModule);
    if (pBinary == NULL) {
        // Error set by GetPayloadSectionFromModule.
        return NULL;
    }

    __try {
        DETOUR_SECTION_HEADER *pHeader = (DETOUR_SECTION_HEADER *)pBinary;
        if (pHeader->cbHeaderSize < sizeof(DETOUR_SECTION_HEADER) ||
            pHeader->nSignature != DETOUR_SECTION_HEADER_SIGNATURE) {

            SetLastError(ERROR_INVALID_EXE_SIGNATURE);
            return NULL;
        }

        PBYTE pbBeg = ((PBYTE)pHeader) + pHeader->nDataOffset;
        PBYTE pbEnd = ((PBYTE)pHeader) + pHeader->cbDataSize;

        for (pbData = pbBeg; pbData < pbEnd;) {
            DETOUR_SECTION_RECORD *pSection = (DETOUR_SECTION_RECORD *)pbData;

            if (DetourAreSameGuid(pSection->guid, rguid)) {
                if (pcbData) {
                    *pcbData = pSection->cbBytes - sizeof(*pSection);
                }
                SetLastError(NO_ERROR);
                return (PBYTE)(pSection + 1);
            }

            pbData = (PBYTE)pSection + pSection->cbBytes;
        }
        SetLastError(ERROR_INVALID_HANDLE);
        return NULL;
    }
    __except(GetExceptionCode() == EXCEPTION_ACCESS_VIOLATION
        ? EXCEPTION_EXECUTE_HANDLER
        : EXCEPTION_CONTINUE_SEARCH) {
        SetLastError(ERROR_INVALID_HANDLE);
        return NULL;
    }
}

HMODULE WINAPI DetourEnumerateModules(_In_opt_ HMODULE hModuleLast)
{
    PBYTE pbLast = (PBYTE)hModuleLast + MM_ALLOCATION_GRANULARITY;

    MEMORY_BASIC_INFORMATION mbi;
    ZeroMemory(&mbi, sizeof(mbi));

    // Find the next memory region that contains a mapped PE image.
    //
    for (;; pbLast = (PBYTE)mbi.BaseAddress + mbi.RegionSize) {
        if (VirtualQuery(pbLast, &mbi, sizeof(mbi)) <= 0) {
            break;
        }

        // Skip uncommitted regions and guard pages.
        //
        if ((mbi.State != MEM_COMMIT)               ||
            ((mbi.Protect & 0xff) == PAGE_NOACCESS) ||
            (mbi.Protect & PAGE_GUARD)) {
            continue;
        }

        __try {
            PIMAGE_DOS_HEADER pDosHeader = (PIMAGE_DOS_HEADER)pbLast;
            if (pDosHeader->e_magic != IMAGE_DOS_SIGNATURE ||
                (DWORD)pDosHeader->e_lfanew > mbi.RegionSize ||
                (DWORD)pDosHeader->e_lfanew < sizeof(*pDosHeader)) {
                continue;
            }

            PIMAGE_NT_HEADERS pNtHeader = (PIMAGE_NT_HEADERS)((PBYTE)pDosHeader +
                                                              pDosHeader->e_lfanew);
            if (pNtHeader->Signature != IMAGE_NT_SIGNATURE) {
                continue;
            }

            SetLastError(NO_ERROR);
            return (HMODULE)pDosHeader;
        }
#pragma prefast(suppress:28940, "A bad pointer means this probably isn't a PE header.")
        __except(GetExceptionCode() == EXCEPTION_ACCESS_VIOLATION ?
                 EXCEPTION_EXECUTE_HANDLER : EXCEPTION_CONTINUE_SEARCH) {
            continue;
        }
    }
    return NULL;
}

_Writable_bytes_(*pcbData)
_Readable_bytes_(*pcbData)
_Success_       (return != NULL)
PVOID WINAPI DetourFindPayloadEx(
    _In_      REFGUID rguid,
    _Out_opt_ DWORD   *pcbData)
{
    for (HMODULE hMod = NULL; (hMod = DetourEnumerateModules(hMod)) != NULL;) {
        PVOID pvData;

        pvData = DetourFindPayload(hMod, rguid, pcbData);
        if (pvData != NULL) {
            return pvData;
        }
    }
    SetLastError(ERROR_MOD_NOT_FOUND);
    return NULL;
}

HMODULE WINAPI DetourGetContainingModule(_In_ PVOID pvAddr)
{
    MEMORY_BASIC_INFORMATION mbi;
    ZeroMemory(&mbi, sizeof(mbi));

    __try {
        if (VirtualQuery(pvAddr, &mbi, sizeof(mbi)) <= 0) {
            SetLastError(ERROR_BAD_EXE_FORMAT);
            return NULL;
        }

        // Skip uncommitted regions and guard pages.
        //
        if ((mbi.State != MEM_COMMIT)               ||
            ((mbi.Protect & 0xff) == PAGE_NOACCESS) ||
            (mbi.Protect & PAGE_GUARD)) {
            SetLastError(ERROR_BAD_EXE_FORMAT);
            return NULL;
        }

        PIMAGE_DOS_HEADER pDosHeader = (PIMAGE_DOS_HEADER)mbi.AllocationBase;
        if (pDosHeader->e_magic != IMAGE_DOS_SIGNATURE) {
            SetLastError(ERROR_BAD_EXE_FORMAT);
            return NULL;
        }

        PIMAGE_NT_HEADERS pNtHeader = (PIMAGE_NT_HEADERS)((PBYTE)pDosHeader +
                                                          pDosHeader->e_lfanew);
        if (pNtHeader->Signature != IMAGE_NT_SIGNATURE) {
            SetLastError(ERROR_INVALID_EXE_SIGNATURE);
            return NULL;
        }
        if (pNtHeader->FileHeader.SizeOfOptionalHeader == 0) {
            SetLastError(ERROR_EXE_MARKED_INVALID);
            return NULL;
        }
        SetLastError(NO_ERROR);

        return (HMODULE)pDosHeader;
    }
    __except(GetExceptionCode() == EXCEPTION_ACCESS_VIOLATION
        ? EXCEPTION_EXECUTE_HANDLER
        : EXCEPTION_CONTINUE_SEARCH) {
        SetLastError(ERROR_INVALID_EXE_SIGNATURE);
        return NULL;
    }
}

BOOL WINAPI DetourFreePayload(_In_ PVOID pvData)
{
    BOOL fSucceeded = FALSE;

    // If you have any doubts about the following code, please refer to the comments in DetourCopyPayloadToProcess.
    HMODULE hModule = DetourGetContainingModule(pvData);

    if (hModule != NULL) {
        fSucceeded = VirtualFree(hModule, 0, MEM_RELEASE);

        if (fSucceeded) {
            hModule = NULL;
        }
    }

    return fSucceeded;
}

static DWORD DetourPageProtectAdjustExecute(
    _In_  DWORD dwOldProtect,
    _In_  DWORD dwNewProtect)
//  Copy EXECUTE from dwOldProtect to dwNewProtect.
{
    bool const fOldExecute = ((dwOldProtect & DETOUR_PAGE_EXECUTE_ALL) != 0);
    bool const fNewExecute = ((dwNewProtect & DETOUR_PAGE_EXECUTE_ALL) != 0);

    if (fOldExecute && !fNewExecute) {
        dwNewProtect = ((dwNewProtect & DETOUR_PAGE_NO_EXECUTE_ALL) << 4)
            | (dwNewProtect & DETOUR_PAGE_ATTRIBUTES);
    }
    else if (!fOldExecute && fNewExecute) {
        dwNewProtect = ((dwNewProtect & DETOUR_PAGE_EXECUTE_ALL) >> 4)
            | (dwNewProtect & DETOUR_PAGE_ATTRIBUTES);
    }
    return dwNewProtect;
}

_Success_(return != FALSE)
BOOL WINAPI DetourVirtualProtectSameExecuteEx(
    _In_  HANDLE hProcess,
    _In_  PVOID  pAddress,
    _In_  SIZE_T nSize,
    _In_  DWORD  dwNewProtect,
    _Out_ PDWORD pdwOldProtect)
// Some systems do not allow executability of a page to change. This function applies
// dwNewProtect to [pAddress, nSize), but preserving the previous executability.
// This function is meant to be a drop-in replacement for some uses of VirtualProtectEx.
// When "restoring" page protection, there is no need to use this function.
{
    MEMORY_BASIC_INFORMATION mbi;

    // Query to get existing execute access.

    ZeroMemory(&mbi, sizeof(mbi));

    if (VirtualQueryEx(hProcess, pAddress, &mbi, sizeof(mbi)) == 0) {
        return FALSE;
    }
    return VirtualProtectEx(hProcess, pAddress, nSize,
                            DetourPageProtectAdjustExecute(mbi.Protect, dwNewProtect),
                            pdwOldProtect);
}

_Success_(return != FALSE)
BOOL WINAPI DetourVirtualProtectSameExecute(
    _In_  PVOID  pAddress,
    _In_  SIZE_T nSize,
    _In_  DWORD  dwNewProtect,
    _Out_ PDWORD pdwOldProtect)
{
    return DetourVirtualProtectSameExecuteEx(GetCurrentProcess(), pAddress, nSize, dwNewProtect, pdwOldProtect);
}

BOOL WINAPI DetourRestoreAfterWithEx(
    _In_reads_bytes_(cbData) PVOID pvData,
    _In_                     DWORD cbData)
{
    PDETOUR_EXE_RESTORE pder = (PDETOUR_EXE_RESTORE)pvData;

    if (pder->cb != sizeof(*pder) || pder->cb > cbData) {
        SetLastError(ERROR_BAD_EXE_FORMAT);
        return FALSE;
    }

    DWORD dwPermIdh = ~0u;
    DWORD dwPermInh = ~0u;
    DWORD dwPermClr = ~0u;
    DWORD dwIgnore;
    BOOL fSucceeded = FALSE;
    BOOL fUpdated32To64 = FALSE;

    if (pder->pclr != NULL && pder->clr.Flags != ((PDETOUR_CLR_HEADER)pder->pclr)->Flags) {
        // If we had to promote the 32/64-bit agnostic IL to 64-bit, we can't restore
        // that.
        fUpdated32To64 = TRUE;
    }

    if (DetourVirtualProtectSameExecute(pder->pidh, pder->cbidh,
                                        PAGE_EXECUTE_READWRITE, &dwPermIdh)) {
        if (DetourVirtualProtectSameExecute(pder->pinh, pder->cbinh,
                                            PAGE_EXECUTE_READWRITE, &dwPermInh)) {

            CopyMemory(pder->pidh, &pder->idh, pder->cbidh);
            CopyMemory(pder->pinh, &pder->inh, pder->cbinh);

            if (pder->pclr != NULL && !fUpdated32To64) {
                if (DetourVirtualProtectSameExecute(pder->pclr, pder->cbclr,
                                                    PAGE_EXECUTE_READWRITE, &dwPermClr)) {
                    CopyMemory(pder->pclr, &pder->clr, pder->cbclr);
                    VirtualProtect(pder->pclr, pder->cbclr, dwPermClr, &dwIgnore);
                    fSucceeded = TRUE;
                }
            }
            else {
                fSucceeded = TRUE;
            }
            VirtualProtect(pder->pinh, pder->cbinh, dwPermInh, &dwIgnore);
        }
        VirtualProtect(pder->pidh, pder->cbidh, dwPermIdh, &dwIgnore);
    }
    // Delete the payload after successful recovery to prevent repeated restore
    if (fSucceeded) {
        DetourFreePayload(pder);
        pder = NULL;
    }
    return fSucceeded;
}

BOOL WINAPI DetourRestoreAfterWith()
{
    PVOID pvData;
    DWORD cbData;

    pvData = DetourFindPayloadEx(DETOUR_EXE_RESTORE_GUID, &cbData);

    if (pvData != NULL && cbData != 0) {
        return DetourRestoreAfterWithEx(pvData, cbData);
    }

    SetLastError(ERROR_MOD_NOT_FOUND);
    return FALSE;
}

VOID CALLBACK DetourFinishHelperProcess(
    _In_ HWND,
    _In_ HINSTANCE,
    _In_ LPSTR,
    _In_ INT)
{

}
