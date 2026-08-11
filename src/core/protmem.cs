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
public unsafe readonly ref struct FhVirtualProtectScope<T> where T : unmanaged {
    private readonly T*                    _address;
    private readonly PAGE_PROTECTION_FLAGS _protection_flags_old;

    /// <summary>
    ///     Changes the page protection at the given <paramref name="address"/> to <paramref name="protection_flags"/>.
    /// </summary>
    public FhVirtualProtectScope(T* address, PAGE_PROTECTION_FLAGS protection_flags) {
        PAGE_PROTECTION_FLAGS flOldProtect;
        PInvoke.VirtualProtect(address, (nuint) sizeof(T), protection_flags, &flOldProtect);

        _address              = address;
        _protection_flags_old = flOldProtect;
    }

    /// <summary>
    ///     Reverts the page protection at the target address to its previous state.
    /// </summary>
    public void Dispose() {
        PAGE_PROTECTION_FLAGS discard;
        PInvoke.VirtualProtect(_address, (nuint) sizeof(T), _protection_flags_old, &discard);
    }
}

