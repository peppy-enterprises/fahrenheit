/* [fkelava 29/5/23 18:15]
 * Uses code from the .NET NativeHost sample, used under the MIT license.
 *
 * See THIRD-PARTY-NOTICES.
 *
 * For the HostFXR bits, see https://github.com/dotnet/samples/blob/main/core/hosting/src/NativeHost/nativehost.cpp.
 * For the exception handling, see:
 * - http://code.aaronballman.com/minidumper/MiniDump.cpp
 * - https://github.com/folgerwang/UnrealEngine/blob/release/Engine/Source/Runtime/Core/Private/Windows/WindowsPlatformCrashContext.cpp
 * - https://github.com/goatcorp/Dalamud/blob/master/Dalamud.Boot/veh.cpp
 *
 * Portions of VEH handling (C) Dalamud, under the terms of the AGPL.
 */

#include "fhstage1.h"

// Function pointer to managed delegate with our own signature
typedef void (CORECLR_DELEGATE_CALLTYPE* fh_init)();

using string_t = std::basic_string<char_t>;
using main_fn  = int(*)(void);
using eh_fn    = LONG(*)(EXCEPTION_POINTERS*);

// Globals to hold original and detour addresses of program entrypoint and SEH filter
main_fn g_fnptr_main_original = nullptr;
main_fn g_fnptr_main_target   = nullptr;
eh_fn   g_fnptr_eh_original   = nullptr;

// Globals for EH override
DWORD               g_eh_thread_faulting_id;
DWORD               g_eh_thread_handler_id;
HANDLE              g_eh_thread_handler;
EXCEPTION_POINTERS* g_eh_exception_ptr;

// Globals to hold hostfxr exports
hostfxr_initialize_for_runtime_config_fn g_fnptr_hostfxr_init;
hostfxr_get_runtime_delegate_fn          g_fnptr_hostfxr_get_delegate;
hostfxr_close_fn                         g_fnptr_hostfxr_close;

// Using the nethost library, discover the location of hostfxr and get exports
static bool load_hostfxr() {
    // Pre-allocate a large buffer for the path to hostfxr
    char_t buffer[MAX_PATH];
    size_t buffer_size = sizeof(buffer) / sizeof(char_t);

    int rc = get_hostfxr_path(buffer, &buffer_size, nullptr);
    if (rc != 0) {
        std::wcerr << "get_hostfxr_path() failed, error code: " << rc << std::endl;
        return false;
    }

    // Load hostfxr and get desired exports
    HMODULE lib = ::LoadLibraryW(buffer);

    if (lib == nullptr)
        return FALSE;

    g_fnptr_hostfxr_init         = (hostfxr_initialize_for_runtime_config_fn)::GetProcAddress(lib, "hostfxr_initialize_for_runtime_config");
    g_fnptr_hostfxr_get_delegate = (hostfxr_get_runtime_delegate_fn)         ::GetProcAddress(lib, "hostfxr_get_runtime_delegate");
    g_fnptr_hostfxr_close        = (hostfxr_close_fn)                        ::GetProcAddress(lib, "hostfxr_close");

    return (g_fnptr_hostfxr_init && g_fnptr_hostfxr_get_delegate && g_fnptr_hostfxr_close);
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

// Since there is no reasonable general list of exceptions to exempt from VEH, I used Dalamud's.
// https://github.com/goatcorp/Dalamud/blob/7e980a0a5e312c65d22f724703715e0679d2ef8a/Dalamud.Boot/veh.cpp#L40-L80
static bool stage1_eh_whitelist_exception(const DWORD code)
{
    switch (code)
    {
        case STATUS_ACCESS_VIOLATION:
        case STATUS_IN_PAGE_ERROR:
        case STATUS_INVALID_HANDLE:
        case STATUS_INVALID_PARAMETER:
        case STATUS_NO_MEMORY:
        case STATUS_ILLEGAL_INSTRUCTION:
        case STATUS_NONCONTINUABLE_EXCEPTION:
        case STATUS_INVALID_DISPOSITION:
        case STATUS_ARRAY_BOUNDS_EXCEEDED:
        case STATUS_FLOAT_DENORMAL_OPERAND:
        case STATUS_FLOAT_DIVIDE_BY_ZERO:
        case STATUS_FLOAT_INEXACT_RESULT:
        case STATUS_FLOAT_INVALID_OPERATION:
        case STATUS_FLOAT_OVERFLOW:
        case STATUS_FLOAT_STACK_CHECK:
        case STATUS_FLOAT_UNDERFLOW:
        case STATUS_INTEGER_DIVIDE_BY_ZERO:
        case STATUS_INTEGER_OVERFLOW:
        case STATUS_PRIVILEGED_INSTRUCTION:
        case STATUS_STACK_OVERFLOW:
        case STATUS_DLL_NOT_FOUND:
        case STATUS_ORDINAL_NOT_FOUND:
        case STATUS_ENTRYPOINT_NOT_FOUND:
        case STATUS_DLL_INIT_FAILED:
        case STATUS_CONTROL_STACK_VIOLATION:
        case STATUS_FLOAT_MULTIPLE_FAULTS:
        case STATUS_FLOAT_MULTIPLE_TRAPS:
        case STATUS_HEAP_CORRUPTION:
        case STATUS_STACK_BUFFER_OVERRUN:
        case STATUS_INVALID_CRUNTIME_PARAMETER:
        case STATUS_THREAD_NOT_RUNNING:
        case STATUS_ALREADY_REGISTERED:
        case 0xE0434352: // CLR exception
            return true;
        default:
            return false;
    }
}

// Filters the core dump to exclude objects which we do not want to record.
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

// Writes a customized core dump.
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

// The Stage1 exception handler.
static LONG WINAPI stage1_eh(EXCEPTION_POINTERS* ptr_exception_info) {
    g_eh_exception_ptr      = ptr_exception_info;
    g_eh_thread_faulting_id = GetCurrentThreadId();

    ::ResumeThread       (g_eh_thread_handler);
    ::WaitForSingleObject(g_eh_thread_handler, INFINITE);
    ::CloseHandle        (g_eh_thread_handler);

    return EXCEPTION_CONTINUE_SEARCH;
}

// The Stage1 vectored exception handler.
static LONG NTAPI stage1_veh(EXCEPTION_POINTERS* ptr_exception_info) {
    // TODO: remove logging after testruns
    auto ec = ptr_exception_info->ExceptionRecord->ExceptionCode;
    std::wcout << "VEH: " << std::hex << ec << std::endl;

    /* [fkelava 15/06/26 00:43]
     * Per Passant (https://stackoverflow.com/a/12300563):
     * > Exception codes with values less than 0x80000000 are
     * > just informal and never an indicator of real trouble.
     */
    if (ec < 0x80000000 || !stage1_eh_whitelist_exception(ec))
        return EXCEPTION_CONTINUE_SEARCH;

    return stage1_eh(ptr_exception_info);
}

// Ignores the game's attempt to install its own exception handler.
static LPTOP_LEVEL_EXCEPTION_FILTER WINAPI stage1_eh_set_filter(LPTOP_LEVEL_EXCEPTION_FILTER fnptr_exception_filter) {
    return &stage1_eh;
}

// If necessary, replaces the game's EH filter with a Stage1 custom one.
static BOOL stage1_eh_install(LPBYTE ptr_main_module) {
    char_t exe_full_name_buf[MAX_PATH];
    auto size = ::GetModuleFileNameW(NULL, exe_full_name_buf, sizeof(exe_full_name_buf) / sizeof(char_t));

    string_t exe_full_name       = exe_full_name_buf;
    size_t   exe_name_dirsep_pos = exe_full_name.find_last_of(DIR_SEPARATOR) + 1;

    if (exe_name_dirsep_pos == string_t::npos) {
        std::wcerr << "The path to the target binary is invalid." << std::endl;
        return FALSE;
    }

    string_t exe_name = exe_full_name.substr(exe_name_dirsep_pos, exe_full_name.length());

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
    AddVectoredExceptionHandler(TRUE, &stage1_veh);

    if (MH_CreateHookApi(L"kernel32.dll", "SetUnhandledExceptionFilter", &stage1_eh_set_filter, reinterpret_cast<void**>(&g_fnptr_eh_original)) != MH_OK
    ||  MH_EnableHook   (&SetUnhandledExceptionFilter)                                                                                          != MH_OK) {
        std::wcerr << "Failed to install EH hook for " << exe_name << std::endl;
        return FALSE;
    }

    std::wcout << "Installed EH hook for " << exe_name << std::endl;
    return TRUE;
}

// Runs before the program's own entrypoint, setting up Fahrenheit.
static int stage1_main(void) {
    // STEP 1:
    // Attach to the Stage0 console and forward stdout/stderr to it.
    if (!AttachConsole(ATTACH_PARENT_PROCESS)) {
        std::wcerr << "Failed to attach to the Stage0 console." << std::endl;
        exit(GetLastError());
    }

    FILE* parent_stdout;
    FILE* parent_stderr;

    if (freopen_s(&parent_stdout, "CONOUT$", "w", stdout) != 0 ||
        freopen_s(&parent_stderr, "CONOUT$", "w", stderr) != 0) {
        std::wcerr << "Failed to redirect standard output and error pipes to Stage0 console." << std::endl;
        exit(EXIT_FAILURE);
    }

    // STEP 2:
    // If supported, install an EH override which allows us to capture
    // a customized core dump for easier debugging.
    HMODULE hMainModule = GetModuleHandleW(nullptr);
    LPBYTE  pMainModule = reinterpret_cast<LPBYTE>(hMainModule);

    if (!stage1_eh_install(pMainModule)) {
        std::wcerr << "Failed to install EH hook." << std::endl;
        exit(EXIT_FAILURE);
    }

    // STEP 3:
    // Determine the current working directory and the location
    // of the executable being launched, to which we will swap
    // the working directory later.
    char_t host_path_buf[MAX_PATH]; // where is the game?
    char_t cwd_path_buf [MAX_PATH]; // where are _we_?

    auto size     = ::GetModuleFileNameW  (NULL, host_path_buf, sizeof(host_path_buf) / sizeof(char_t));
    auto cwd_size = ::GetCurrentDirectoryW(sizeof(cwd_path_buf) / sizeof(char_t), cwd_path_buf);

    if (size == 0) {
        std::wcerr << "GetModuleFileName() failed." << std::endl;
        exit(GetLastError());
    }

    if (cwd_size == 0) {
        std::wcerr << "GetCurrentDirectory() failed." << std::endl;
        exit(GetLastError());
    }

    string_t host_path = host_path_buf;
    string_t cwd_path  = cwd_path_buf;

    // STEP 4:
    // Declare the name, type, and location of the bootstrap method to invoke.
    const string_t clrhost_config_path = cwd_path + STR("\\fh.runtimeconfig.json");
    const string_t clrhost_lib_path    = cwd_path + STR("\\fh.dll");
    const char_t*  clrhost_type        = STR("Fahrenheit.FhEnvironment, fh");
    const char_t*  clrhost_init_method = STR("boot");

    auto host_dirsep_pos = host_path.find_last_of(DIR_SEPARATOR);

    if (host_dirsep_pos == string_t::npos) {
        std::wcerr << "The path to the target binary is invalid." << std::endl;
        exit(EXIT_FAILURE);
    }

    std::wcout << "Stage 1 Loader executing for: " << host_path << std::endl;

    host_path = host_path.substr(0, host_dirsep_pos + 1);

    // STEP 5:
    // Load HostFxr. This library will locate the .NET runtime for us.
    if (!load_hostfxr()) {
        std::wcerr << "hostfxr: failed to load" << std::endl;
        std::wcerr << "Fahrenheit failed to load the .NET Runtime. Ensure it is installed as per the setup guide." << std::endl;
        exit(EXIT_FAILURE);
    }

    // STEP 6:
    // Initialize and start the .NET runtime.
    void*          ptr_hostfxr_load_assembly        = nullptr;
    void*          ptr_hostfxr_get_function_pointer = nullptr;
    hostfxr_handle cxt                              = nullptr;

    int rc = g_fnptr_hostfxr_init(clrhost_config_path.c_str(), nullptr, &cxt);
    if (rc != 0 || cxt == nullptr) {
        std::wcerr << "hostfxr: initialize_for_runtime_config() failed" << std::endl;
        std::wcerr << "This is an uncommon error. Please contact the Fahrenheit developers at https://github.com/fahrenheit-crew/fahrenheit." << std::endl;

        g_fnptr_hostfxr_close(cxt);
        exit(rc);
    }

    // STEP 7:
    // Get function pointers to HostFxr's `load_assembly()` and `get_function_pointer()`.
    rc = g_fnptr_hostfxr_get_delegate(
        cxt,
        hdt_load_assembly,
        &ptr_hostfxr_load_assembly);

    if (rc != 0 || ptr_hostfxr_load_assembly == nullptr) {
        std::wcerr << "hostfxr: failed to obtain fnptr (hdt_load_assembly)" << std::endl;
        std::wcerr << "This is an uncommon error. Please contact the Fahrenheit developers at https://github.com/fahrenheit-crew/fahrenheit." << std::endl;
        exit(rc);
    }

    rc = g_fnptr_hostfxr_get_delegate(
        cxt,
        hdt_get_function_pointer,
        &ptr_hostfxr_get_function_pointer);

    if (rc != 0 || ptr_hostfxr_get_function_pointer == nullptr) {
        std::wcerr << "hostfxr: failed to obtain fnptr (hdt_get_function_pointer)"  << std::endl;
        std::wcerr << "This is an uncommon error. Please contact the Fahrenheit developers at https://github.com/fahrenheit-crew/fahrenheit." << std::endl;
        exit(rc);
    }

    g_fnptr_hostfxr_close(cxt);

    load_assembly_fn        fnptr_hostfxr_load_assembly        = (load_assembly_fn)       ptr_hostfxr_load_assembly;
    get_function_pointer_fn fnptr_hostfxr_get_function_pointer = (get_function_pointer_fn)ptr_hostfxr_get_function_pointer;

    // STEP 8:
    // Load managed assembly and get function pointer to bootstrap function.
    fh_init fnptr_fh_init = nullptr;

    rc = fnptr_hostfxr_load_assembly(
        clrhost_lib_path.c_str(),
        nullptr,
        nullptr);

    if (rc != 0) {
        std::wcerr << "hostfxr: load_assembly() failed" << std::endl;
        std::wcerr << "Could not load the Fahrenheit DLL. It is in an unexpected place, or does not exist. Double-check your install." << std::endl;
        exit(rc);
    }

    rc = fnptr_hostfxr_get_function_pointer(
        clrhost_type,
        clrhost_init_method,
        UNMANAGEDCALLERSONLY_METHOD,
        nullptr,
        nullptr,
        (void**)&fnptr_fh_init);

    if (rc != 0 || fnptr_fh_init == nullptr) {
        std::wcerr << "hostfxr: get_function_pointer() failed" << std::endl;
        std::wcerr << "Failed to locate the Fahrenheit boot function. You made a change to the bootloader, but forgot to update Stage1." << std::endl;
        exit(rc);
    }

    // STEP 9:
    // Boot Fahrenheit by invoking the boot function in `fh.dll`.

    // TRANSITION: NATIVE -> MANAGED
    fnptr_fh_init();
    // TRANSITION: MANAGED -> NATIVE

    // STEP 10:
    // Change the working directory to the targeted executable's location,
    // now that we have finished initialization.
    rc = _wchdir(host_path.c_str());
    if (rc != 0) {
        std::wcerr << "Failed to switch to the game's working directory." << std::endl;
        exit(rc);
    }

    // STEP 11:
    // Let the game run. Enjoy!
    std::wcout << "Stage 1 Loader complete. The game is now executing." << std::endl;
    return g_fnptr_main_original();
}

BOOL APIENTRY DllMain(
    HMODULE hdll,
    DWORD   reason,
    LPVOID  ptr_reserved
) {
    switch (reason) {
        case DLL_PROCESS_ATTACH: {
            // Return the IAT to its original self.
            if (!DetourRestoreAfterWith())
                exit(GetLastError());

            // Override the program's entrypoint. We need to host the .NET Runtime and boot Fahrenheit first.
            HMODULE hMainModule = GetModuleHandleW(nullptr);
            LPBYTE  pMainModule = reinterpret_cast<LPBYTE>(hMainModule);

            PIMAGE_DOS_HEADER pImgDosHeaders = reinterpret_cast<PIMAGE_DOS_HEADER>(hMainModule);
            if (pImgDosHeaders->e_magic  != IMAGE_DOS_SIGNATURE)
                return FALSE;

            PIMAGE_NT_HEADERS pImgNTHeaders  = reinterpret_cast<PIMAGE_NT_HEADERS>((pMainModule + pImgDosHeaders->e_lfanew));
            if (pImgNTHeaders->Signature != IMAGE_NT_SIGNATURE)
                return FALSE;

            g_fnptr_main_target = reinterpret_cast<main_fn>(pMainModule + pImgNTHeaders->OptionalHeader.AddressOfEntryPoint);

            if (MH_Initialize()                                                                                    != MH_OK
            ||  MH_CreateHook(g_fnptr_main_target, &stage1_main, reinterpret_cast<void**>(&g_fnptr_main_original)) != MH_OK
            ||  MH_EnableHook(g_fnptr_main_target)                                                                 != MH_OK)
                return FALSE;
        }
        case DLL_THREAD_ATTACH:
        case DLL_THREAD_DETACH:
        case DLL_PROCESS_DETACH:
            break;
    }
    return TRUE;
}
