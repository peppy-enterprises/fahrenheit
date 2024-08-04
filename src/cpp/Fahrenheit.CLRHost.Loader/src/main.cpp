/* [fkelava 29/5/23 18:15]
 * A mashup of Sylvain Peyrefitte's Detours.NET and the .NET NativeHost sample.
 * Both are licensed MIT, as is this repository. See THIRD-PARTY-NOTICES.
 */

// Fahrenheit headers
#include "pch.h"
#include "fhdetour.h"
#include "pinvokecache.h"

// Standard headers
#include <stdio.h>
#include <stdint.h>
#include <stdlib.h>
#include <string.h>
#include <assert.h>
#include <iostream>
#include <fstream>
#include <direct.h>

// .NET hosting headers
#include <nethost.h>
#include <coreclr_delegates.h>
#include <hostfxr.h>

#ifdef _WIN32
#include <Windows.h>

#define STR(s) L ## s
#define CH(c) L ## c
#define DIR_SEPARATOR L'\\'

#else
#include <dlfcn.h>
#include <limits.h>

#define STR(s) s
#define CH(c) c
#define DIR_SEPARATOR '/'
#define MAX_PATH PATH_MAX

#endif

using string_t = std::basic_string<char_t>;

namespace
{
    // Globals to hold hostfxr exports
    hostfxr_initialize_for_runtime_config_fn init_fptr;
    hostfxr_get_runtime_delegate_fn          get_delegate_fptr;
    hostfxr_close_fn                         close_fptr;

    // Forward declarations
    bool                                      load_hostfxr();
    load_assembly_and_get_function_pointer_fn get_dotnet_load_assembly(const char_t* assembly);
}

/********************************************************************************************
 * Function used to load and activate .NET Core
 ********************************************************************************************/

namespace
{
    // Forward declarations
    void* load_library(const char_t*);
    void* get_export(void*, const char*);

#ifdef _WIN32
    void* load_library(const char_t* path)
    {
        HMODULE h = ::LoadLibraryW(path);
        assert(h != nullptr);
        return (void*)h;
    }
    void* get_export(void* h, const char* name)
    {
        void* f = ::GetProcAddress((HMODULE)h, name);
        assert(f != nullptr);
        return f;
    }
#else
    void* load_library(const char_t* path)
    {
        void* h = dlopen(path, RTLD_LAZY | RTLD_LOCAL);
        assert(h != nullptr);
        return h;
    }
    void* get_export(void* h, const char* name)
    {
        void* f = dlsym(h, name);
        assert(f != nullptr);
        return f;
    }
#endif

    // <SnippetLoadHostFxr>
    // Using the nethost library, discover the location of hostfxr and get exports
    bool load_hostfxr()
    {
        // Pre-allocate a large buffer for the path to hostfxr
        char_t buffer[MAX_PATH];
        size_t buffer_size = sizeof(buffer) / sizeof(char_t);
        int rc = get_hostfxr_path(buffer, &buffer_size, nullptr);
        if (rc != 0)
            return false;

        // Load hostfxr and get desired exports
        void* lib         = load_library(buffer);
        init_fptr         = (hostfxr_initialize_for_runtime_config_fn)get_export(lib, "hostfxr_initialize_for_runtime_config");
        get_delegate_fptr = (hostfxr_get_runtime_delegate_fn)get_export(lib, "hostfxr_get_runtime_delegate");
        close_fptr        = (hostfxr_close_fn)get_export(lib, "hostfxr_close");

        return (init_fptr && get_delegate_fptr && close_fptr);
    }
    // </SnippetLoadHostFxr>

    // <SnippetInitialize>
    // Load and initialize .NET Core and get desired function pointer for scenario
    load_assembly_and_get_function_pointer_fn get_dotnet_load_assembly(const char_t* config_path)
    {
        // Load .NET Core
        void*          load_assembly_and_get_function_pointer = nullptr;
        hostfxr_handle cxt                                    = nullptr;

        int rc = init_fptr(config_path, nullptr, &cxt);
        if (rc != 0 || cxt == nullptr)
        {
            std::cerr << "Init failed: " << std::hex << std::showbase << rc << std::endl;
            close_fptr(cxt);
            return nullptr;
        }

        // Get the load assembly function pointer
        rc = get_delegate_fptr(
            cxt,
            hdt_load_assembly_and_get_function_pointer,
            &load_assembly_and_get_function_pointer);
        if (rc != 0 || load_assembly_and_get_function_pointer == nullptr)
            std::cerr << "Get delegate failed: " << std::hex << std::showbase << rc << std::endl;

        close_fptr(cxt);
        return (load_assembly_and_get_function_pointer_fn)load_assembly_and_get_function_pointer;
    }
    // </SnippetInitialize>
}

static FARPROC WINAPI GetProcAddressCLR(HMODULE module, LPCSTR funcName)
{
    // if ordinal
    if ((reinterpret_cast<ULONGLONG>(funcName) & 0xffffffffffff0000) == 0) {
        return GetProcAddress(module, funcName);
    }

    auto real = FhCLRHost::PInvokeCache::GetInstance().find(module, funcName);

    // already hooked
    if (real != nullptr) {
        return reinterpret_cast<FARPROC>(real);
    }

    return GetProcAddress(module, funcName);
}

/*!
 *	@brief	Function from Detours.dll (managed) to indicate new hook
 *	@param	hModule		source module
 *	@param	lpProcName	name of function
 *	@param	pReal		real address
 */
extern "C"
__declspec(dllexport) void DetoursCLRSetGetProcAddressCache(HMODULE module, LPCSTR funcName, PVOID real)
{
    FhCLRHost::PInvokeCache::GetInstance().update(module, funcName, real);
}

using EntryPoint_T = int(*)(void);

EntryPoint_T ffxMain = NULL;

static int DetourMain(void)
{
    AttachConsole(ATTACH_PARENT_PROCESS);

    // Get the current executable's directory
    // This sample assumes the managed assembly to load and its runtime configuration file are next to the host
    char_t host_path[MAX_PATH];
#if _WIN32
    auto size = ::GetModuleFileName(NULL, host_path, sizeof(host_path) / sizeof(char_t));
    assert(size != 0);
#else
    auto resolved = realpath(argv[0], host_path);
    assert(resolved != nullptr);
#endif

    string_t root_path = host_path;
    auto     pos       = root_path.find_last_of(DIR_SEPARATOR);

    assert(pos != string_t::npos);
    root_path = root_path.substr(0, pos + 1);

    string_t fh_bin_path = root_path + L"\\fahrenheit\\bin\\";

    //
    // STEP 1: Load HostFxr and get exported hosting functions
    //
    if (!load_hostfxr())
    {
        assert(false && "Failure: load_hostfxr()");
        return EXIT_FAILURE;
    }

    //
    // STEP 2: Initialize and start the .NET Core runtime
    //
    const string_t config_path = fh_bin_path + STR("fhclrhost.runtimeconfig.json");

    load_assembly_and_get_function_pointer_fn load_assembly_and_get_function_pointer = nullptr;
    load_assembly_and_get_function_pointer = get_dotnet_load_assembly(config_path.c_str());
    assert(load_assembly_and_get_function_pointer != nullptr && "Failure: get_dotnet_load_assembly()");

    FhDetourPatchIAT(GetModuleHandle(TEXT("coreclr.dll")), GetProcAddress, GetProcAddressCLR);

    //
    // STEP 3: Load managed assembly and get function pointer to a managed method
    //
    const string_t dotnetlib_path     = fh_bin_path + STR("fhclrhost.dll");
    const char_t*  dotnet_type        = STR("Fahrenheit.CLRHost.FhCLRHost, fhclrhost");
    const char_t*  dotnet_type_method = STR("clrhost_init");

    // <SnippetLoadAndGet>
    // Function pointer to managed delegate
    component_entry_point_fn clrhostinit = nullptr;
    int rc = load_assembly_and_get_function_pointer(
        dotnetlib_path.c_str(),
        dotnet_type,
        dotnet_type_method,
        nullptr /*delegate_type_name*/,
        nullptr,
        (void**)&clrhostinit);
    // </SnippetLoadAndGet>
    assert(rc == 0 && clrhostinit != nullptr && "Failure: load_assembly_and_get_function_pointer()");

    clrhostinit(nullptr, 0);
    // return the working directory to the executable, now that bootstrapping is complete
    assert(_wchdir(root_path.c_str()) == 0);

    return ffxMain();
}

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
            DetourUpdateThread(GetCurrentThread());
            DetourAttach(&reinterpret_cast<PVOID&>(ffxMain), DetourMain);
            DetourTransactionCommit();
        }
        case DLL_THREAD_ATTACH:
        case DLL_THREAD_DETACH:
        case DLL_PROCESS_DETACH:
            break;
    }
    return TRUE;
}

