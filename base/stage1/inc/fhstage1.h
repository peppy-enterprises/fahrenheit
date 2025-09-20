/* [fkelava 25/8/24 01:42]
 * Substantively copied from the .NET Hosting samples (https://github.com/dotnet/samples/), used under the MIT license.
 *
 * See THIRD-PARTY-NOTICES for the licenses.
 */

#pragma once

#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers

#include <stdio.h>
#include <stdint.h>
#include <stdlib.h>
#include <string.h>
#include <assert.h>
#include <iostream>
#include <fstream>
#include <direct.h>

#ifdef _WIN32
#include <windows.h>
#include <winternl.h>

#define STR(s) L ## s
#define CH(c)  L ## c
#define DIR_SEPARATOR L'\\'

#else
#include <dlfcn.h>
#include <limits.h>

#define STR(s) s
#define CH(c) c
#define DIR_SEPARATOR '/'
#define MAX_PATH PATH_MAX

#endif

 // .NET hosting headers
#include <nethost.h>
#include <coreclr_delegates.h>
#include <hostfxr.h>

// IAT patching
#include <detours/detours.h>

// Hooking library
#include <MinHook.h>
