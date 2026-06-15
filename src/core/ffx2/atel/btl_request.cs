// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* [fkelava 11/5/25 08:11]
 * source: Switch ver.
 * header: _ms_header_battle_actor
 *
 * /ffx_ps2/ffx2/master/jppc/battle/header/btlactor.h
 */

namespace Fahrenheit.FFX2.Atel;

[StructLayout(LayoutKind.Sequential)]
public readonly struct AtelBtlRequestData {
    public readonly short space;
    public readonly short binhdr;
    public readonly short actor;
    public readonly short tag;
}
