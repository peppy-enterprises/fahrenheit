// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit;

/// <summary>
///     A disposable structure which changes the protection flags on a memory location
///     while it remains in scope, reverting to the original flags when disposed.
/// </summary>
/// <remarks>
///     This is useful for rewriting constants embedded into read-only process sections,
///     which would normally cause an access violation.
/// </remarks>
[SupportedOSPlatform("windows5.1.2600")]
internal unsafe readonly ref struct FhVirtualProtectScope<T> where T : unmanaged {
    private readonly T*                    _lpAddress;
    private readonly PAGE_PROTECTION_FLAGS _flOldProtect;

    public FhVirtualProtectScope(T* lpAddress, PAGE_PROTECTION_FLAGS flNewProtect) {
        PAGE_PROTECTION_FLAGS flOldProtect;
        PInvoke.VirtualProtect(lpAddress, (nuint) sizeof(T), flNewProtect, &flOldProtect);

        _lpAddress    = lpAddress;
        _flOldProtect = flOldProtect;
    }

    public void Dispose() {
        PAGE_PROTECTION_FLAGS discard;
        PInvoke.VirtualProtect(_lpAddress, (nuint) sizeof(T), _flOldProtect, &discard);
    }
}

