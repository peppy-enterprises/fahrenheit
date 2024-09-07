/* [fkelava 25/8/24 01:42]
 * Substantively copied from MS Detours' `uimports.cpp`, `creatwth.cpp` (https://github.com/microsoft/Detours/), used under the MIT license.
 *
 * See THIRD-PARTY-NOTICES for the license.
 */

#include "fhstage0.h"

typedef BOOL(WINAPI* LPFN_ISWOW64PROCESS)(HANDLE, PBOOL);

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

static PBYTE FindAndAllocateNearBase(
    HANDLE hProcess,
    PBYTE  pbModule,
    PBYTE  pbBase,
    DWORD  cbAlloc)
{
    MEMORY_BASIC_INFORMATION mbi;
    ZeroMemory(&mbi, sizeof(mbi));

    PBYTE pbLast = pbBase;
    for (;; pbLast = (PBYTE)mbi.BaseAddress + mbi.RegionSize) {

        ZeroMemory(&mbi, sizeof(mbi));
        if (VirtualQueryEx(hProcess, (PVOID)pbLast, &mbi, sizeof(mbi)) == 0) {
            if (GetLastError() == ERROR_INVALID_PARAMETER) {
                break;
            }
            break;
        }
        // Usermode address space has such an unaligned region size always at the
        // end and only at the end.
        //
        if ((mbi.RegionSize & 0xfff) == 0xfff) {
            break;
        }

        // Skip anything other than a pure free region.
        //
        if (mbi.State != MEM_FREE) {
            continue;
        }

        // Use the max of mbi.BaseAddress and pbBase, in case mbi.BaseAddress < pbBase.
        PBYTE pbAddress = (PBYTE)mbi.BaseAddress > pbBase ? (PBYTE)mbi.BaseAddress : pbBase;

        // Round pbAddress up to the nearest MM allocation boundary.
        const DWORD_PTR mmGranularityMinusOne = (DWORD_PTR)(MM_ALLOCATION_GRANULARITY -1);
        pbAddress = (PBYTE)(((DWORD_PTR)pbAddress + mmGranularityMinusOne) & ~mmGranularityMinusOne);

#ifdef _WIN64
        // The offset from pbModule to any replacement import must fit into 32 bits.
        // For simplicity, we check that the offset to the last byte fits into 32 bits,
        // instead of the largest offset we'll actually use. The values are very similar.
        const size_t GB4 = ((((size_t)1) << 32) - 1);
        if ((size_t)(pbAddress + cbAlloc - 1 - pbModule) > GB4) {
            return NULL;
        }
#else
        UNREFERENCED_PARAMETER(pbModule);
#endif

        for (; pbAddress < (PBYTE)mbi.BaseAddress + mbi.RegionSize; pbAddress += MM_ALLOCATION_GRANULARITY) {
            PBYTE pbAlloc = (PBYTE)VirtualAllocEx(hProcess, pbAddress, cbAlloc,
                                                  MEM_RESERVE | MEM_COMMIT, PAGE_READWRITE);
            if (pbAlloc == NULL) {
                continue;
            }
#ifdef _WIN64
            // The offset from pbModule to any replacement import must fit into 32 bits.
            if ((size_t)(pbAddress + cbAlloc - 1 - pbModule) > GB4) {
                return NULL;
            }
#endif
            return pbAlloc;
        }
    }
    return NULL;
}


static inline DWORD PadToDword(DWORD dw)
{
    return (dw + 3) & ~3u;
}

static inline DWORD PadToDwordPtr(DWORD dw)
{
    return (dw + 7) & ~7u;
}

static inline HRESULT ReplaceOptionalSizeA(
    _Inout_z_count_(cchDest) LPSTR  pszDest,
    _In_                     size_t cchDest,
    _In_z_                   LPCSTR pszSize)
{
    if (cchDest == 0       ||
        pszDest == NULL    ||
        pszSize == NULL    ||
        pszSize[0] == '\0' ||
        pszSize[1] == '\0' ||
        pszSize[2] != '\0') {
        // can not write into empty buffer or with string other than two chars.
        return ERROR_INVALID_PARAMETER;
    }

    for (; cchDest >= 2; cchDest--, pszDest++) {
        if (pszDest[0] == '?' && pszDest[1] == '?') {
            pszDest[0] = pszSize[0];
            pszDest[1] = pszSize[1];
            break;
        }
    }

    return S_OK;
}

// UPDATE_IMPORTS_XX with 64-path excised
static BOOL UpdateImports32(
                       HANDLE  hProcess,
                       HMODULE hModule,
    __in_ecount(nDlls) LPCSTR* plpDlls,
                       DWORD   nDlls)
{
    BOOL  fSucceeded = FALSE;
    DWORD cbNew = 0;

    BYTE*  pbNew = NULL;
    DWORD  i;
    SIZE_T cbRead;
    DWORD  n;

    PBYTE pbModule = (PBYTE)hModule;

    IMAGE_DOS_HEADER idh;
    ZeroMemory(&idh, sizeof(idh));
    if (!ReadProcessMemory(hProcess, pbModule, &idh, sizeof(idh), &cbRead)
        || cbRead < sizeof(idh)) {
    finish:
        if (pbNew != NULL) {
            delete[] pbNew;
            pbNew = NULL;
        }
        return fSucceeded;
    }

    IMAGE_NT_HEADERS32 inh;
    ZeroMemory(&inh, sizeof(inh));

    if (!ReadProcessMemory(hProcess, pbModule + idh.e_lfanew, &inh, sizeof(inh), &cbRead) ||
        cbRead < sizeof(inh)) {
        goto finish;
    }

    if (inh.OptionalHeader.Magic != IMAGE_NT_OPTIONAL_HDR32_MAGIC) {
        SetLastError(ERROR_INVALID_BLOCK);
        goto finish;
    }

    // Zero out the bound table so loader doesn't use it instead of our new table.
    inh.BOUND_DIRECTORY.VirtualAddress = 0;
    inh.BOUND_DIRECTORY.Size = 0;

    // Find the size of the mapped file.
    DWORD dwSec = idh.e_lfanew +
        FIELD_OFFSET(IMAGE_NT_HEADERS32, OptionalHeader) +
        inh.FileHeader.SizeOfOptionalHeader;

    for (i = 0; i < inh.FileHeader.NumberOfSections; i++) {
        IMAGE_SECTION_HEADER ish;
        ZeroMemory(&ish, sizeof(ish));

        if (!ReadProcessMemory(hProcess, pbModule + dwSec + sizeof(ish) * i, &ish, sizeof(ish), &cbRead) ||
            cbRead < sizeof(ish)) {
            goto finish;
        }

        // If the linker didn't suggest an IAT in the data directories, the
        // loader will look for the section of the import directory to be used
        // for this instead. Since we put out new IMPORT_DIRECTORY outside any
        // section boundary, the loader will not find it. So we provide one
        // explicitly to avoid the search.
        //
        if (inh.IAT_DIRECTORY   .VirtualAddress == 0 &&
            inh.IMPORT_DIRECTORY.VirtualAddress >= ish.VirtualAddress &&
            inh.IMPORT_DIRECTORY.VirtualAddress <  ish.VirtualAddress + ish.SizeOfRawData) {

            inh.IAT_DIRECTORY.VirtualAddress = ish.VirtualAddress;
            inh.IAT_DIRECTORY.Size           = ish.SizeOfRawData;
        }
    }

    if (inh.IMPORT_DIRECTORY.VirtualAddress != 0 && inh.IMPORT_DIRECTORY.Size == 0) {

        // Don't worry about changing the PE file,
        // because the load information of the original PE header has been saved and will be restored.
        // The change here is just for the following code to work normally

        PIMAGE_IMPORT_DESCRIPTOR pImageImport = (PIMAGE_IMPORT_DESCRIPTOR)(pbModule + inh.IMPORT_DIRECTORY.VirtualAddress);

        do {
            IMAGE_IMPORT_DESCRIPTOR ImageImport;
            if (!ReadProcessMemory(hProcess, pImageImport, &ImageImport, sizeof(ImageImport), NULL)) {
                goto finish;
            }
            inh.IMPORT_DIRECTORY.Size += sizeof(IMAGE_IMPORT_DESCRIPTOR);
            if (!ImageImport.Name) {
                break;
            }
            ++pImageImport;
        } while (TRUE);

        DWORD dwLastError = GetLastError();
        OutputDebugString(TEXT("[This PE file has an import table, but the import table size is marked as 0. This is an error.")
            TEXT("If it is not repaired, the launched program will not work properly, Detours has automatically repaired its import table size for you! ! !]\r\n"));
        if (GetLastError() != dwLastError) {
            SetLastError(dwLastError);
        }
    }

    // Calculate new import directory size.  Note that since inh is from another
    // process, inh could have been corrupted. We need to protect against
    // integer overflow in allocation calculations.
    DWORD nOldDlls = inh.IMPORT_DIRECTORY.Size / sizeof(IMAGE_IMPORT_DESCRIPTOR);
    DWORD obRem;
    if (DWordMult(sizeof(IMAGE_IMPORT_DESCRIPTOR), nDlls, &obRem) != S_OK) {
        goto finish;
    }
    DWORD obOld;
    if (DWordAdd(obRem, sizeof(IMAGE_IMPORT_DESCRIPTOR) * nOldDlls, &obOld) != S_OK) {
        goto finish;
    }
    DWORD obTab = PadToDwordPtr(obOld);
    // Check for integer overflow.
    if (obTab < obOld) {
        goto finish;
    }
    DWORD stSize;
    if (DWordMult(sizeof(DWORD32) * 4, nDlls, &stSize) != S_OK) {
        goto finish;
    }
    DWORD obDll;
    if (DWordAdd(obTab, stSize, &obDll) != S_OK) {
        goto finish;
    }
    DWORD obStr = obDll;
    cbNew = obStr;
    for (n = 0; n < nDlls; n++) {
        if (DWordAdd(cbNew, PadToDword((DWORD)strlen(plpDlls[n]) + 1), &cbNew) != S_OK) {
            goto finish;
        }
    }
    pbNew = new BYTE[cbNew];
    if (pbNew == NULL) {
        goto finish;
    }
    ZeroMemory(pbNew, cbNew);

    PBYTE pbBase = pbModule;
    PBYTE pbNext = pbBase
        + inh.OptionalHeader.BaseOfCode
        + inh.OptionalHeader.SizeOfCode
        + inh.OptionalHeader.SizeOfInitializedData
        + inh.OptionalHeader.SizeOfUninitializedData;
    if (pbBase < pbNext) {
        pbBase = pbNext;
    }

    PBYTE pbNewIid = FindAndAllocateNearBase(hProcess, pbModule, pbBase, cbNew);
    if (pbNewIid == NULL) {
        goto finish;
    }

    PIMAGE_IMPORT_DESCRIPTOR piid = (PIMAGE_IMPORT_DESCRIPTOR)pbNew;
    IMAGE_THUNK_DATA32*      pt   = NULL;

    DWORD obBase   = (DWORD)(pbNewIid - pbModule);
    DWORD dwProtect = 0;

    if (inh.IMPORT_DIRECTORY.VirtualAddress != 0) {
        // Read the old import directory if it exists.
        if (!ReadProcessMemory(hProcess, pbModule + inh.IMPORT_DIRECTORY.VirtualAddress, &piid[nDlls], nOldDlls * sizeof(IMAGE_IMPORT_DESCRIPTOR), &cbRead) ||
            cbRead < nOldDlls * sizeof(IMAGE_IMPORT_DESCRIPTOR)) {
            goto finish;
        }
    }

    for (n = 0; n < nDlls; n++) {
        HRESULT hrRet = StringCchCopyA((char*)pbNew + obStr, cbNew - obStr, plpDlls[n]);
        if (FAILED(hrRet)) {
            goto finish;
        }

        // After copying the string, we patch up the size "??" bits if any.
        hrRet = ReplaceOptionalSizeA((char*)pbNew + obStr,
            cbNew - obStr,
            DETOURS_STRINGIFY(32));
        if (FAILED(hrRet)) {
            goto finish;
        }

        DWORD nOffset = obTab + (sizeof(IMAGE_THUNK_DATA32) * (4 * n));
        piid[n].OriginalFirstThunk = obBase + nOffset;

        // We need 2 thunks for the import table and 2 thunks for the IAT.
        // One for an ordinal import and one to mark the end of the list.
        pt = ((IMAGE_THUNK_DATA32*)(pbNew + nOffset));
        pt[0].u1.Ordinal = IMAGE_ORDINAL_FLAG32 + 1;
        pt[1].u1.Ordinal = 0;

        nOffset = obTab + (sizeof(IMAGE_THUNK_DATA32) * ((4 * n) + 2));
        piid[n].FirstThunk = obBase + nOffset;
        pt = ((IMAGE_THUNK_DATA32*)(pbNew + nOffset));
        pt[0].u1.Ordinal = IMAGE_ORDINAL_FLAG32 + 1;
        pt[1].u1.Ordinal = 0;
        piid[n].TimeDateStamp = 0;
        piid[n].ForwarderChain = 0;
        piid[n].Name = obBase + obStr;

        obStr += PadToDword((DWORD)strlen(plpDlls[n]) + 1);
    }
    _Analysis_assume_(obStr <= cbNew);

#if 0
    for (i = 0; i < nDlls + nOldDlls; i++) {
        if (piid[i].OriginalFirstThunk == 0 && piid[i].FirstThunk == 0) {
            break;
        }
    }
#endif

    if (!WriteProcessMemory(hProcess, pbNewIid, pbNew, obStr, NULL)) {
        goto finish;
    }

    // In this case the file didn't have an import directory in first place,
    // so we couldn't fix the missing IAT above. We still need to explicitly
    // provide an IAT to prevent to loader from looking for one.
    //
    if (inh.IAT_DIRECTORY.VirtualAddress == 0) {
        inh.IAT_DIRECTORY.VirtualAddress = obBase;
        inh.IAT_DIRECTORY.Size = cbNew;
    }

    inh.IMPORT_DIRECTORY.VirtualAddress = obBase;
    inh.IMPORT_DIRECTORY.Size = cbNew;

    /////////////////////// Update the NT header for the new import directory.
    //
    if (!DetourVirtualProtectSameExecuteEx(hProcess, pbModule, inh.OptionalHeader.SizeOfHeaders, PAGE_EXECUTE_READWRITE, &dwProtect)) {
        goto finish;
    }

    inh.OptionalHeader.CheckSum = 0;

    if (!WriteProcessMemory(hProcess, pbModule, &idh, sizeof(idh), NULL)) {
        goto finish;
    }

    if (!WriteProcessMemory(hProcess, pbModule + idh.e_lfanew, &inh, sizeof(inh), NULL)) {
        goto finish;
    }

    if (!VirtualProtectEx(hProcess, pbModule, inh.OptionalHeader.SizeOfHeaders, dwProtect, &dwProtect)) {
        goto finish;
    }

    fSucceeded = TRUE;
    goto finish;
}

_Success_(return != NULL)
PVOID WINAPI DetourCopyPayloadToProcessEx(_In_ HANDLE hProcess,
                                          _In_ REFGUID rguid,
                                          _In_reads_bytes_(cbData) LPCVOID pvData,
                                          _In_ DWORD cbData)
{
    if (hProcess == NULL) {
        SetLastError(ERROR_INVALID_HANDLE);
        return NULL;
    }

    DWORD cbTotal = (sizeof(IMAGE_DOS_HEADER)      +
                     sizeof(IMAGE_NT_HEADERS)      +
                     sizeof(IMAGE_SECTION_HEADER)  +
                     sizeof(DETOUR_SECTION_HEADER) +
                     sizeof(DETOUR_SECTION_RECORD) +
                     cbData);

    PBYTE pbBase = (PBYTE)VirtualAllocEx(hProcess, NULL, cbTotal,
                                         MEM_COMMIT, PAGE_READWRITE);
    if (pbBase == NULL) {
        return NULL;
    }

    // As you can see in the following code,
    // the memory layout of the payload range "[pbBase, pbBase+cbTotal]" is a PE executable file,
    // so DetourFreePayload can use "DetourGetContainingModule(Payload pointer)" to get the above "pbBase" pointer,
    // pbBase: the memory block allocated by VirtualAllocEx will be released in DetourFreePayload by VirtualFree.

    PBYTE                 pbTarget = pbBase;
    IMAGE_DOS_HEADER      idh;
    IMAGE_NT_HEADERS      inh;
    IMAGE_SECTION_HEADER  ish;
    DETOUR_SECTION_HEADER dsh;
    DETOUR_SECTION_RECORD dsr;
    SIZE_T                cbWrote = 0;

    ZeroMemory(&idh, sizeof(idh));
    idh.e_magic  = IMAGE_DOS_SIGNATURE;
    idh.e_lfanew = sizeof(idh);
    if (!WriteProcessMemory(hProcess, pbTarget, &idh, sizeof(idh), &cbWrote) ||
        cbWrote != sizeof(idh)) {
        return NULL;
    }
    pbTarget += sizeof(idh);

    ZeroMemory(&inh, sizeof(inh));
    inh.Signature                       = IMAGE_NT_SIGNATURE;
    inh.FileHeader.SizeOfOptionalHeader = sizeof(inh.OptionalHeader);
    inh.FileHeader.Characteristics      = IMAGE_FILE_DLL;
    inh.FileHeader.NumberOfSections     = 1;
    inh.OptionalHeader.Magic            = IMAGE_NT_OPTIONAL_HDR_MAGIC;
    if (!WriteProcessMemory(hProcess, pbTarget, &inh, sizeof(inh), &cbWrote) ||
        cbWrote != sizeof(inh)) {
        return NULL;
    }
    pbTarget += sizeof(inh);

    ZeroMemory(&ish, sizeof(ish));
    memcpy(ish.Name, ".detour", sizeof(ish.Name));
    ish.VirtualAddress = (DWORD)((pbTarget + sizeof(ish)) - pbBase);
    ish.SizeOfRawData  = (sizeof(DETOUR_SECTION_HEADER) +
                          sizeof(DETOUR_SECTION_RECORD) +
                          cbData);
    if (!WriteProcessMemory(hProcess, pbTarget, &ish, sizeof(ish), &cbWrote) ||
        cbWrote != sizeof(ish)) {
        return NULL;
    }
    pbTarget += sizeof(ish);

    ZeroMemory(&dsh, sizeof(dsh));
    dsh.cbHeaderSize = sizeof(dsh);
    dsh.nSignature   = DETOUR_SECTION_HEADER_SIGNATURE;
    dsh.nDataOffset  = sizeof(DETOUR_SECTION_HEADER);
    dsh.cbDataSize   = (sizeof(DETOUR_SECTION_HEADER) +
                        sizeof(DETOUR_SECTION_RECORD) +
                        cbData);
    if (!WriteProcessMemory(hProcess, pbTarget, &dsh, sizeof(dsh), &cbWrote) ||
        cbWrote != sizeof(dsh)) {
        return NULL;
    }
    pbTarget += sizeof(dsh);

    ZeroMemory(&dsr, sizeof(dsr));
    dsr.cbBytes   = cbData + sizeof(DETOUR_SECTION_RECORD);
    dsr.nReserved = 0;
    dsr.guid      = rguid;
    if (!WriteProcessMemory(hProcess, pbTarget, &dsr, sizeof(dsr), &cbWrote) ||
        cbWrote != sizeof(dsr)) {
        return NULL;
    }
    pbTarget += sizeof(dsr);

    if (!WriteProcessMemory(hProcess, pbTarget, pvData, cbData, &cbWrote) ||
        cbWrote != cbData) {
        return NULL;
    }

    SetLastError(NO_ERROR);
    return pbTarget;
}

BOOL WINAPI DetourCopyPayloadToProcess(
    _In_                     HANDLE  hProcess,
    _In_                     REFGUID rguid,
    _In_reads_bytes_(cbData) LPCVOID pvData,
    _In_                     DWORD   cbData)
{
    return DetourCopyPayloadToProcessEx(hProcess, rguid, pvData, cbData) != NULL;
}

static BOOL IsWow64ProcessHelper(
    HANDLE hProcess,
    PBOOL  Wow64Process)
{
#ifdef _X86_
    if (Wow64Process == NULL) {
        return FALSE;
    }

    // IsWow64Process is not available on all supported versions of Windows.
    //
    HMODULE hKernel32 = LoadLibraryW(L"KERNEL32.DLL");
    if (hKernel32 == NULL) {
        return FALSE;
    }

    LPFN_ISWOW64PROCESS pfnIsWow64Process = (LPFN_ISWOW64PROCESS)
        GetProcAddress(hKernel32, "IsWow64Process");

    if (pfnIsWow64Process == NULL) {
        return FALSE;
    }
    return pfnIsWow64Process(hProcess, Wow64Process);
#else
    return IsWow64Process(hProcess, Wow64Process);
#endif
}

/* [fkelava 25/8/2024 01:25]
 * Copied verbatim from Detours.
 */
static PVOID LoadNtHeaderFromProcess(
    _In_  HANDLE              hProcess,
    _In_  HMODULE             hModule,
    _Out_ PIMAGE_NT_HEADERS32 pNtHeader)
{
    ZeroMemory(pNtHeader, sizeof(*pNtHeader));
    PBYTE pbModule = (PBYTE)hModule;

    if (pbModule == NULL) {
        SetLastError(ERROR_INVALID_PARAMETER);
        return NULL;
    }

    MEMORY_BASIC_INFORMATION mbi;
    ZeroMemory(&mbi, sizeof(mbi));

    if (VirtualQueryEx(hProcess, hModule, &mbi, sizeof(mbi)) == 0) {
        return NULL;
    }

    IMAGE_DOS_HEADER idh;
    if (!ReadProcessMemory(hProcess, pbModule, &idh, sizeof(idh), NULL)) {
        return NULL;
    }

    if (idh.e_magic != IMAGE_DOS_SIGNATURE   ||
        (DWORD)idh.e_lfanew > mbi.RegionSize ||
        (DWORD)idh.e_lfanew < sizeof(idh)) {
        SetLastError(ERROR_BAD_EXE_FORMAT);
        return NULL;
    }

    if (!ReadProcessMemory(hProcess, pbModule + idh.e_lfanew, pNtHeader, sizeof(*pNtHeader), NULL)) {
        return NULL;
    }

    if (pNtHeader->Signature != IMAGE_NT_SIGNATURE) {
        SetLastError(ERROR_BAD_EXE_FORMAT);
        return NULL;
    }

    return pbModule + idh.e_lfanew;
}

/* [fkelava 25/8/2024 01:25]
 * Copied verbatim from Detours.
 */
static HMODULE EnumerateModulesInProcess(
    _In_      HANDLE              hProcess,
    _In_opt_  HMODULE             hModuleLast,
    _Out_     PIMAGE_NT_HEADERS32 pNtHeader,
    _Out_opt_ PVOID               *pRemoteNtHeader)
{
    ZeroMemory(pNtHeader, sizeof(*pNtHeader));
    if (pRemoteNtHeader) {
        *pRemoteNtHeader = NULL;
    }

    PBYTE pbLast = (PBYTE)hModuleLast + MM_ALLOCATION_GRANULARITY;

    MEMORY_BASIC_INFORMATION mbi;
    ZeroMemory(&mbi, sizeof(mbi));

    // Find the next memory region that contains a mapped PE image.
    //

    for (;; pbLast = (PBYTE)mbi.BaseAddress + mbi.RegionSize) {
        if (VirtualQueryEx(hProcess, (PVOID)pbLast, &mbi, sizeof(mbi)) == 0) {
            break;
        }

        // Usermode address space has such an unaligned region size always at the
        // end and only at the end.
        //
        if ((mbi.RegionSize & 0xfff) == 0xfff) {
            break;
        }
        if (((PBYTE)mbi.BaseAddress + mbi.RegionSize) < pbLast) {
            break;
        }

        // Skip uncommitted regions and guard pages.
        //
        if ((mbi.State != MEM_COMMIT)               ||
            ((mbi.Protect & 0xff) == PAGE_NOACCESS) ||
            (mbi.Protect & PAGE_GUARD)) {
            continue;
        }

        PVOID remoteHeader = LoadNtHeaderFromProcess(hProcess, (HMODULE)pbLast, pNtHeader);

        if (remoteHeader) {
            if (pRemoteNtHeader) {
                *pRemoteNtHeader = remoteHeader;
            }

            return (HMODULE)pbLast;
        }
    }
    return NULL;
}

static BOOL RecordExeRestore(HANDLE hProcess, HMODULE hModule, DETOUR_EXE_RESTORE& der)
{
    // Save the various headers for DetourRestoreAfterWith.
    ZeroMemory(&der, sizeof(der));
    der.cb = sizeof(der);

    der.pidh  = (PBYTE)hModule;
    der.cbidh = sizeof(der.idh);

    if (!ReadProcessMemory(hProcess, der.pidh, &der.idh, sizeof(der.idh), NULL)) {
        return FALSE;
    }

    // We read the NT header in two passes to get the full size.
    // First we read just the Signature and FileHeader.
    der.pinh  = der.pidh + der.idh.e_lfanew;
    der.cbinh = FIELD_OFFSET(IMAGE_NT_HEADERS, OptionalHeader);

    if (!ReadProcessMemory(hProcess, der.pinh, &der.inh, der.cbinh, NULL)) {
        return FALSE;
    }

    // Second we read the OptionalHeader and Section headers.
    der.cbinh = (FIELD_OFFSET(IMAGE_NT_HEADERS, OptionalHeader) +
                 der.inh.FileHeader.SizeOfOptionalHeader +
                 der.inh.FileHeader.NumberOfSections * sizeof(IMAGE_SECTION_HEADER));

    if (der.cbinh > sizeof(der.raw)) {
        return FALSE;
    }

    if (!ReadProcessMemory(hProcess, der.pinh, &der.inh, der.cbinh, NULL)) {
        return FALSE;
    }

    // Third, we read the CLR header

    if (der.inh.OptionalHeader.Magic == IMAGE_NT_OPTIONAL_HDR32_MAGIC) {
        if (der.inh32.CLR_DIRECTORY.VirtualAddress != 0 &&
            der.inh32.CLR_DIRECTORY.Size != 0) {

            der.pclr = ((PBYTE)hModule) + der.inh32.CLR_DIRECTORY.VirtualAddress;
        }
    }
    else if (der.inh.OptionalHeader.Magic == IMAGE_NT_OPTIONAL_HDR64_MAGIC) {
        if (der.inh64.CLR_DIRECTORY.VirtualAddress != 0 &&
            der.inh64.CLR_DIRECTORY.Size != 0) {

            der.pclr = ((PBYTE)hModule) + der.inh64.CLR_DIRECTORY.VirtualAddress;
        }
    }

    if (der.pclr != 0) {
        der.cbclr = sizeof(der.clr);
        if (!ReadProcessMemory(hProcess, der.pclr, &der.clr, der.cbclr, NULL)) {
            return FALSE;
        }
    }

    return TRUE;
}

BOOL WINAPI DetourUpdateProcessWithDllEx(
    _In_              HANDLE  hProcess,
    _In_              HMODULE hModule,
    _In_              BOOL    bIs32BitProcess,
    _In_reads_(nDlls) LPCSTR* rlpDlls,
    _In_              DWORD   nDlls)
{
    // Find the next memory region that contains a mapped PE image.
    //
    BOOL bIs32BitExe = FALSE;

    IMAGE_NT_HEADERS32 inh;

    if (hModule == NULL || !LoadNtHeaderFromProcess(hProcess, hModule, &inh)) {
        SetLastError(ERROR_INVALID_OPERATION);
        return FALSE;
    }

    if (inh.OptionalHeader.Magic == IMAGE_NT_OPTIONAL_HDR32_MAGIC
        && inh.FileHeader.Machine != 0) {
        bIs32BitExe = TRUE;
    }

    if (hModule == NULL) {
        SetLastError(ERROR_INVALID_OPERATION);
        return FALSE;
    }

    // Save the various headers for DetourRestoreAfterWith.
    //
    DETOUR_EXE_RESTORE der;

    if (!RecordExeRestore(hProcess, hModule, der)) {
        return FALSE;
    }

    if (bIs32BitProcess) {
        // 32-bit native or 32-bit managed process on any platform.
        if (!UpdateImports32(hProcess, hModule, rlpDlls, nDlls)) {
            return FALSE;
        }
    }
    else {
        // 64-bit native or 64-bit managed process.
        //
        // Can't detour a 64-bit process with 32-bit code.
        // Note: This happens for 32-bit PE binaries containing only
        // manage code that have been marked as 64-bit ready.
        //
        SetLastError(ERROR_INVALID_HANDLE);
        return FALSE;
    }

    ////////////////////////////////////////////////// Update the CLR header.
    //
    if (der.pclr != NULL) {
        DETOUR_CLR_HEADER clr;
        CopyMemory(&clr, &der.clr, sizeof(clr));
        clr.Flags &= ~COMIMAGE_FLAGS_ILONLY;    // Clear the IL_ONLY flag.

        DWORD dwProtect;
        if (!DetourVirtualProtectSameExecuteEx(hProcess, der.pclr, sizeof(clr), PAGE_READWRITE, &dwProtect)) {
            return FALSE;
        }

        if (!WriteProcessMemory(hProcess, der.pclr, &clr, sizeof(clr), NULL)) {
            return FALSE;
        }

        if (!VirtualProtectEx(hProcess, der.pclr, sizeof(clr), dwProtect, &dwProtect)) {
            return FALSE;
        }
    }

    //////////////////////////////// Save the undo data to the target process.
    //
    if (!DetourCopyPayloadToProcess(hProcess, DETOUR_EXE_RESTORE_GUID, &der, sizeof(der))) {
        return FALSE;
    }
    return TRUE;
}

/* [fkelava 25/8/2024 01:25]
 * Copied verbatim from Detours.
 */
BOOL WINAPI DetourUpdateProcessWithDll(
    _In_              HANDLE hProcess,
    _In_reads_(nDlls) LPCSTR *rlpDlls,
    _In_              DWORD  nDlls)
{
    // Find the next memory region that contains a mapped PE image.
    //
    BOOL    bIs32BitProcess;
    BOOL    bIs64BitOS      = FALSE;
    HMODULE hModule         = NULL;
    HMODULE hLast           = NULL;

    for (;;) {
        IMAGE_NT_HEADERS32 inh;

        if ((hLast = EnumerateModulesInProcess(hProcess, hLast, &inh, NULL)) == NULL) {
            break;
        }

        if ((inh.FileHeader.Characteristics & IMAGE_FILE_DLL) == 0) {
            hModule = hLast;
        }
    }

    if (hModule == NULL) {
        SetLastError(ERROR_INVALID_OPERATION);
        return FALSE;
    }

    // Determine if the target process is 32bit or 64bit. This is a two-stop process:
    //
    // 1. First, determine if we're running on a 64bit operating system.
    //   - If we're running 64bit code (i.e. _WIN64 is defined), this is trivially true.
    //   - If we're running 32bit code (i.e. _WIN64 is not defined), test if
    //   we're running under Wow64. If so, it implies that the operating system
    //   is 64bit.
    //
#ifdef _WIN64
    bIs64BitOS = TRUE;
#else
    if (!IsWow64ProcessHelper(GetCurrentProcess(), &bIs64BitOS)) {
        return FALSE;
    }
#endif

    // 2. With the operating system bitness known, we can now consider the target process:
    //   - If we're running on a 64bit OS, the target process is 32bit in case
    //   it is running under Wow64. Otherwise, it's 64bit, running natively
    //   (without Wow64).
    //   - If we're running on a 32bit OS, the target process must be 32bit, too.
    //
    if (bIs64BitOS) {
        if (!IsWow64ProcessHelper(hProcess, &bIs32BitProcess)) {
            return FALSE;
        }
    }
    else {
        bIs32BitProcess = TRUE;
    }

    return DetourUpdateProcessWithDllEx(hProcess,
                                        hModule,
                                        bIs32BitProcess,
                                        rlpDlls,
                                        nDlls);
}
