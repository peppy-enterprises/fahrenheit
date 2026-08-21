// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* [fkelava 25/8/24 01:42]
 * Substantively copied from the .NET Hosting samples (https://github.com/dotnet/samples/), used under the MIT license.
 *
 * See THIRD-PARTY-NOTICES for the licenses.
 */

#pragma once
#pragma comment(lib, "dbghelp.lib")
#pragma comment(lib, "pathcch.lib")

#define WIN32_LEAN_AND_MEAN // Exclude rarely-used stuff from Windows headers

#include <iostream>

// Win32 API
#include <windows.h>
#include <DbgHelp.h>
#include <PathCch.h>

// .NET hosting headers
#include <nethost.h>
#include <coreclr_delegates.h>
#include <hostfxr.h>

// IAT patching
#include <detours/detours.h>

// Hooking library
#include <MinHook.h>
