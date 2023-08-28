#include "pch.h"
#include "fhdetour.h"

typedef struct _DetoursUnsandboxContext {
	PVOID pImport;
	PVOID pReal;
} DetoursUnsandboxContext;

static BOOL CALLBACK Unsandbox(_In_opt_ PVOID pContext, _In_ ULONG nOrdinal, _In_opt_ PCSTR pszName, _In_opt_ PVOID* pvFunc)
{
	DetoursUnsandboxContext* context = reinterpret_cast<DetoursUnsandboxContext*>(pContext);
	
	if (pvFunc != nullptr && *pvFunc == context->pImport)
	{
		DWORD oldrights = 0;
		if (VirtualProtect(pvFunc, sizeof(LPVOID), PAGE_READWRITE, &oldrights))
		{
			*pvFunc = context->pReal;
			VirtualProtect(pvFunc, sizeof(LPVOID), oldrights, &oldrights);
		}
		return FALSE;
	}

	return TRUE;
}

static BOOL CALLBACK Sandbox(_In_opt_ PVOID pContext, _In_ ULONG nOrdinal, _In_opt_ PCSTR pszName, _In_opt_ PVOID* pvFunc)
{
	DetoursUnsandboxContext* context = reinterpret_cast<DetoursUnsandboxContext*>(pContext);

	if (pvFunc != nullptr && *pvFunc == context->pReal)
	{
		DWORD oldrights = 0;
		if (VirtualProtect(pvFunc, sizeof(LPVOID), PAGE_READWRITE, &oldrights))
		{
			*pvFunc = context->pImport;
			VirtualProtect(pvFunc, sizeof(LPVOID), oldrights, &oldrights);
		}
		return FALSE;
	}

	return TRUE;
}

BOOL FhDetourPatchIAT(HMODULE hModule, PVOID pFunction, PVOID pReal)
{
	DetoursUnsandboxContext context = { pFunction, pReal };

	return DetourEnumerateImportsEx(hModule, &context, nullptr, Unsandbox);
}

BOOL FhDetourUnpatchIAT(HMODULE hModule, PVOID pFunction, PVOID pReal)
{
	DetoursUnsandboxContext context = { pFunction, pReal };

	return DetourEnumerateImportsEx(hModule, &context, nullptr, Sandbox);
}
