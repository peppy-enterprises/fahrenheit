#pragma once
#include "pch.h"

#ifdef __cplusplus
extern "C"
{
#endif

	__declspec(dllexport) BOOL DetoursPatchIAT(HMODULE hModule, PVOID pFunction, PVOID pReal);

#ifdef __cplusplus
}
#endif