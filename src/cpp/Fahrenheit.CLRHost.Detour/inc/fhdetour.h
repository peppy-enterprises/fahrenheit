#pragma once
#include "pch.h"

#ifdef __cplusplus
extern "C"  {
#endif
	__declspec(dllexport) BOOL FhDetourPatchIAT(HMODULE hModule, PVOID pFunction, PVOID pReal);
	__declspec(dllexport) BOOL FhDetourUnpatchIAT(HMODULE hModule, PVOID pFunction, PVOID pReal);
#ifdef __cplusplus
}
#endif