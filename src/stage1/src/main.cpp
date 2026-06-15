// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* [fkelava 29/5/23 18:15]
 * Uses code from the .NET NativeHost sample, used under the MIT license.
 *
 * See THIRD-PARTY-NOTICES.
 */

#include "fhstage1.h"

typedef void (CORECLR_DELEGATE_CALLTYPE* fh_ldr_managed_init)();

using string_t = std::basic_string<char_t>;
using main_t   = int(*)(void);

main_t pMainOriginal = nullptr; // Original program entrypoint.
main_t pMainTarget   = nullptr; // Detour of program entrypoint.

namespace {
    // Globals to hold hostfxr exports
    hostfxr_initialize_for_runtime_config_fn init_fptr;
    hostfxr_get_runtime_delegate_fn          get_delegate_fptr;
    hostfxr_close_fn                         close_fptr;

    // Forward declarations
    bool load_hostfxr();
}

/********************************************************************************************
 * Function used to load and activate .NET Core
 ********************************************************************************************/

namespace {
    // Forward declarations
    void *load_library(const char_t *);
    void *get_export(void *, const char *);

#ifdef _WIN32
    void *load_library(const char_t *path)
    {
        HMODULE h = ::LoadLibraryW(path);
        assert(h != nullptr);
        return (void*)h;
    }
    void *get_export(void *h, const char *name)
    {
        void *f = ::GetProcAddress((HMODULE)h, name);
        assert(f != nullptr);
        return f;
    }
#else
    void *load_library(const char_t *path)
    {
        void *h = dlopen(path, RTLD_LAZY | RTLD_LOCAL);
        assert(h != nullptr);
        return h;
    }
    void *get_export(void *h, const char *name)
    {
        void *f = dlsym(h, name);
        assert(f != nullptr);
        return f;
    }
#endif

    // Using the nethost library, discover the location of hostfxr and get exports
    bool load_hostfxr() {
        // Pre-allocate a large buffer for the path to hostfxr
        char_t buffer[MAX_PATH];
        size_t buffer_size = sizeof(buffer) / sizeof(char_t);

        int rc = get_hostfxr_path(buffer, &buffer_size, nullptr);
        if (rc != 0) {
            std::wcerr << "get_hostfxr_path() failed, error code: " << rc << std::endl;
            return false;
        }

        // Load hostfxr and get desired exports
        void* lib         = load_library(buffer);
        init_fptr         = (hostfxr_initialize_for_runtime_config_fn)get_export(lib, "hostfxr_initialize_for_runtime_config");
        get_delegate_fptr = (hostfxr_get_runtime_delegate_fn)         get_export(lib, "hostfxr_get_runtime_delegate");
        close_fptr        = (hostfxr_close_fn)                        get_export(lib, "hostfxr_close");

        return (init_fptr && get_delegate_fptr && close_fptr);
    }
}

static int DetourMain(void) {
    //
    // STEP 1:
    // Attach to the Stage0 console and forward stdout/stderr to it.
    //
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

    //
    // STEP 2:
    // Determine the current working directory and the location
    // of the executable being launched, to which we will swap
    // the working directory later.
    //
    char_t host_path_buf[MAX_PATH]; // where is the game?
    char_t cwd_path_buf [MAX_PATH]; // where are _we_?

    auto size     = ::GetModuleFileName  (NULL, host_path_buf, sizeof(host_path_buf) / sizeof(char_t));
    auto cwd_size = ::GetCurrentDirectory(sizeof(cwd_path_buf) / sizeof(char_t), cwd_path_buf);

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

    //
    // STEP 3:
    // Declare the name, type, and location of the bootstrap method to invoke.
    //
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

    //
    // STEP 4:
    // Load HostFxr. This library will locate the .NET runtime for us.
    //
    if (!load_hostfxr()) {
        std::wcerr << "hostfxr: failed to load" << std::endl;
        std::wcerr << "Fahrenheit failed to load the .NET Runtime. Ensure it is installed as per the setup guide." << std::endl;
        exit(EXIT_FAILURE);
    }

    //
    // STEP 5:
    // Initialize and start the .NET runtime.
    //
    void*          load_assembly_fptr        = nullptr;
    void*          get_function_pointer_fptr = nullptr;
    hostfxr_handle cxt                       = nullptr;

    int rc = init_fptr(clrhost_config_path.c_str(), nullptr, &cxt);
    if (rc != 0 || cxt == nullptr) {
        std::wcerr << "hostfxr: initialize_for_runtime_config() failed" << std::endl;
        std::wcerr << "This is an uncommon error. Please contact the Fahrenheit developers at https://github.com/fahrenheit-crew/fahrenheit." << std::endl;

        close_fptr(cxt);
        exit(rc);
    }

    //
    // STEP 6:
    // Get function pointers to HostFxr's `load_assembly()` and `get_function_pointer()`.
    //
    rc = get_delegate_fptr(
        cxt,
        hdt_load_assembly,
        &load_assembly_fptr);

    if (rc != 0 || load_assembly_fptr == nullptr) {
        std::wcerr << "hostfxr: failed to obtain fnptr (hdt_load_assembly)" << std::endl;
        std::wcerr << "This is an uncommon error. Please contact the Fahrenheit developers at https://github.com/fahrenheit-crew/fahrenheit." << std::endl;
        exit(rc);
    }

    rc = get_delegate_fptr(
        cxt,
        hdt_get_function_pointer,
        &get_function_pointer_fptr);

    if (rc != 0 || get_function_pointer_fptr == nullptr) {
        std::wcerr << "hostfxr: failed to obtain fnptr (hdt_get_function_pointer)"  << std::endl;
        std::wcerr << "This is an uncommon error. Please contact the Fahrenheit developers at https://github.com/fahrenheit-crew/fahrenheit." << std::endl;
        exit(rc);
    }

    close_fptr(cxt);

    load_assembly_fn        load_assembly        = (load_assembly_fn)       load_assembly_fptr;
    get_function_pointer_fn get_function_pointer = (get_function_pointer_fn)get_function_pointer_fptr;

    //
    // STEP 7:
    // Load managed assembly and get function pointer to bootstrap function.
    //
    fh_ldr_managed_init fh_init = nullptr;

    rc = load_assembly(
        clrhost_lib_path.c_str(),
        nullptr,
        nullptr);

    if (rc != 0) {
        std::wcerr << "hostfxr: load_assembly() failed" << std::endl;
        std::wcerr << "Could not load the Fahrenheit DLL. It is in an unexpected place, or does not exist. Double-check your install." << std::endl;
        exit(rc);
    }

    rc = get_function_pointer(
        clrhost_type,
        clrhost_init_method,
        UNMANAGEDCALLERSONLY_METHOD,
        nullptr,
        nullptr,
        (void**)&fh_init);

    if (rc != 0 || fh_init == nullptr) {
        std::wcerr << "hostfxr: get_function_pointer() failed" << std::endl;
        std::wcerr << "Failed to locate the Fahrenheit boot function. You made a change to the bootloader, but forgot to update Stage1." << std::endl;
        exit(rc);
    }

    //
    // STEP 8:
    // Boot Fahrenheit by invoking the boot function in `fh.dll`.
    //

    // TRANSITION: NATIVE -> MANAGED
    fh_init();
    // TRANSITION: MANAGED -> NATIVE

    //
    // STEP 9:
    // Change the working directory to the targeted executable's location,
    // now that we have finished initialization.
    //
    rc = _wchdir(host_path.c_str());
    if (rc != 0) {
        std::wcerr << "Failed to switch to the game's working directory." << std::endl;
        exit(rc);
    }

    std::wcout << "Stage 1 Loader complete. The game is now executing." << std::endl;
    return pMainOriginal();
}

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     ) {
    switch (ul_reason_for_call) {
        case DLL_PROCESS_ATTACH: {
            if (!DetourRestoreAfterWith()) exit(GetLastError());

            auto hMainModule    = reinterpret_cast<HMODULE>          (NtCurrentTeb()->ProcessEnvironmentBlock->Reserved3[1]);
            auto pImgDosHeaders = reinterpret_cast<PIMAGE_DOS_HEADER>(hMainModule);
            if (pImgDosHeaders->e_magic  != IMAGE_DOS_SIGNATURE) return TRUE;

            auto pImgNTHeaders = reinterpret_cast<PIMAGE_NT_HEADERS> ((reinterpret_cast<LPBYTE>(pImgDosHeaders) + pImgDosHeaders->e_lfanew));
            if (pImgNTHeaders->Signature != IMAGE_NT_SIGNATURE) return TRUE;

            pMainTarget = reinterpret_cast<main_t>(pImgNTHeaders->OptionalHeader.AddressOfEntryPoint + reinterpret_cast<LPBYTE>(hMainModule));

            if (MH_Initialize()                                                                   != MH_OK) return TRUE;
            if (MH_CreateHook(pMainTarget, &DetourMain, reinterpret_cast<void**>(&pMainOriginal)) != MH_OK) return TRUE;
            if (MH_EnableHook(pMainTarget)                                                        != MH_OK) return TRUE;
        }
        case DLL_THREAD_ATTACH:
        case DLL_THREAD_DETACH:
        case DLL_PROCESS_DETACH:
            break;
    }
    return TRUE;
}

