// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* [fkelava 29/5/23 18:15]
 * See THIRD-PARTY-NOTICES.
 *
 * For the exception handling, see:
 * - http://code.aaronballman.com/minidumper/MiniDump.cpp
 * - https://github.com/folgerwang/UnrealEngine/blob/release/Engine/Source/Runtime/Core/Private/Windows/WindowsPlatformCrashContext.cpp
 */

#include "fhstage1.h"

using eh_fn = LONG(*)(EXCEPTION_POINTERS*);

eh_fn g_fnptr_eh_original = nullptr; // A function pointer to the game's original SEH filter.

DWORD               g_eh_thread_faulting_id; // The ID of the thread that threw a structured exception.
DWORD               g_eh_thread_handler_id;  // The ID of the thread handling structured exceptions.
HANDLE              g_eh_thread_handler;     // The handle to the thread handling structured exceptions.
EXCEPTION_POINTERS* g_eh_exception_ptr;      // A pointer to the exception bringing the process down.

/*
 * Filters a core dump to exclude objects which we do not want to record.
 */

static BOOL CALLBACK stage1_eh_filter_dump(
          PVOID                     ptr_callback_param,
    const PMINIDUMP_CALLBACK_INPUT  ptr_callback_input,
          PMINIDUMP_CALLBACK_OUTPUT ptr_callback_output) {
    if (!ptr_callback_input || !ptr_callback_output) return FALSE;

    switch (ptr_callback_input->CallbackType) {
        case CancelCallback:
            return FALSE;

        case IncludeThreadCallback: {
            // Exclude the thread which writes the minidump.
            return ptr_callback_input->IncludeThread.ThreadId != g_eh_thread_handler_id;
        } break;
    }

    return TRUE;
}

/*
 * Writes a customized core dump.
 */

static DWORD CALLBACK stage1_eh_create_dump(LPVOID ptr_thread_parameter) {
    HANDLE hFile = CreateFileW(
        L"crash_dump.dmp",
        GENERIC_READ | GENERIC_WRITE,
        0,
        nullptr,
        CREATE_ALWAYS,
        FILE_ATTRIBUTE_NORMAL,
        nullptr);

    if (hFile == NULL || hFile == INVALID_HANDLE_VALUE) {
        std::wcerr << "Failed to open a file to write the core dump to." << std::endl;
        return 1;
    }

    HANDLE        hProcess  = GetCurrentProcess();
    DWORD         ProcessId = GetProcessId(hProcess);
    MINIDUMP_TYPE DumpType  = (MINIDUMP_TYPE)(
                              MiniDumpNormal
                            | MiniDumpWithDataSegs
                            | MiniDumpWithHandleData
                            | MiniDumpWithFullMemoryInfo
                            | MiniDumpWithThreadInfo
                            | MiniDumpWithProcessThreadData
                            | MiniDumpWithUnloadedModules);

    /* [fkelava 11/06/26 21:24]
     * For ClientPointers:
     * https://learn.microsoft.com/en-us/windows/win32/api/minidumpapiset/ns-minidumpapiset-minidump_exception_information#members
     * > If you are accessing local memory (in the calling process) you should not set this member to TRUE.
     */
    MINIDUMP_EXCEPTION_INFORMATION mdei = { 0 };
    mdei.ThreadId          = g_eh_thread_faulting_id;
    mdei.ExceptionPointers = g_eh_exception_ptr;
    mdei.ClientPointers    = FALSE;

    MINIDUMP_CALLBACK_INFORMATION mci = { 0 };
    mci.CallbackRoutine = (MINIDUMP_CALLBACK_ROUTINE)stage1_eh_filter_dump;
    mci.CallbackParam   = nullptr;

    PMINIDUMP_EXCEPTION_INFORMATION ExceptionParam = g_eh_exception_ptr != nullptr ? &mdei : nullptr;
    PMINIDUMP_CALLBACK_INFORMATION  CallbackParam  = &mci;

    std::wcerr << "Dumping process core. Please wait." << std::endl;

    BOOL rv = MiniDumpWriteDump(
        hProcess,
        ProcessId,
        hFile,
        DumpType,
        ExceptionParam,
        nullptr,
        CallbackParam);

    if (!rv) {
        std::wcerr << "Failed to capture a core dump." << std::endl;
        return 1;
    }

    CloseHandle(hFile);
    return 0;
}

/* [fkelava 11/06/26 16:27]
 * An exception handler which behaves the same as the game's,
 * except that it _unconditionally_ emits a customized core dump.
 *
 * See:
 * - https://learn.microsoft.com/en-us/windows/win32/api/minidumpapiset/nf-minidumpapiset-minidumpwritedump
 * - https://learn.microsoft.com/en-us/windows/win32/api/errhandlingapi/nf-errhandlingapi-unhandledexceptionfilter
 * - FFX.exe+226A90
 * - https://www.debuginfo.com/examples/src/effminidumps/MiniDump.cpp
 */

static LONG WINAPI stage1_eh(EXCEPTION_POINTERS* ptr_exception_info) {
    g_eh_exception_ptr      = ptr_exception_info;
    g_eh_thread_faulting_id = GetCurrentThreadId();

    ::ResumeThread       (g_eh_thread_handler);
    ::WaitForSingleObject(g_eh_thread_handler, INFINITE);
    ::CloseHandle        (g_eh_thread_handler);

    return EXCEPTION_CONTINUE_SEARCH;
}

/* [fkelava 11/06/26 16:27]
 * Ignores the game's attempt to install its own exception handler.
 */

static LPTOP_LEVEL_EXCEPTION_FILTER WINAPI stage1_eh_set_filter(LPTOP_LEVEL_EXCEPTION_FILTER fnptr_exception_filter) {
    return &stage1_eh;
}

/* [fkelava 11/06/26 16:27]
 * If necessary, replaces the game's EH filter with a Stage1 custom one.
 */

BOOL stage1_eh_install(LPBYTE ptr_main_module) {
    char_t exe_full_name_buf[MAX_PATH];
    auto size = ::GetModuleFileNameW(NULL, exe_full_name_buf, sizeof(exe_full_name_buf) / sizeof(char_t));

    std::basic_string<char_t> exe_full_name       = exe_full_name_buf;
    size_t                    exe_name_dirsep_pos = exe_full_name.find_last_of(L'\\') + 1;

    if (exe_name_dirsep_pos == std::basic_string<char_t>::npos) {
        std::wcerr << "The path to the target binary is invalid." << std::endl;
        return FALSE;
    }

    std::basic_string<char_t> exe_name = exe_full_name.substr(exe_name_dirsep_pos, exe_full_name.length());

    // This can be generalized for other games in the future.
    if (exe_name.compare(L"FFX.exe")   != 0
    &&  exe_name.compare(L"FFX-2.exe") != 0)
        return TRUE;

    g_eh_thread_handler = ::CreateThread(
        nullptr,
        0,
        stage1_eh_create_dump,
        nullptr,
        CREATE_SUSPENDED,
        &g_eh_thread_handler_id);

    if (g_eh_thread_handler == nullptr || g_eh_thread_handler == INVALID_HANDLE_VALUE) {
        std::wcerr << "Failed to create EH thread for " << exe_name << std::endl;
        return FALSE;
    }

    ::SetThreadDescription(g_eh_thread_handler, L"Fahrenheit EH");
    SetUnhandledExceptionFilter(&stage1_eh);

    if (MH_CreateHookApi(L"kernel32.dll", "SetUnhandledExceptionFilter", &stage1_eh_set_filter, reinterpret_cast<void**>(&g_fnptr_eh_original)) != MH_OK
    ||  MH_EnableHook   (&SetUnhandledExceptionFilter)                                                                                          != MH_OK) {
        std::wcerr << "Failed to install EH hook for " << exe_name << std::endl;
        return FALSE;
    }

    std::wcout << "Installed EH hook for " << exe_name << std::endl;
    return TRUE;
}
