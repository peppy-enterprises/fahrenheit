// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* [fkelava 26/04/26 15:26]
 * Unlike `call.g.cs`, which contains source-generated delegates with no guarantee of accuracy,
 * this file contains manually annotated calls with proper Fahrenheit types that are vetted for functionality.
 *
 * This file is for calls which are exclusive to FF X-2/LM and not shared with X.
 */

namespace Fahrenheit.FFX2;

/// <summary>
///     An accessor for game function calls exclusive to FF X-2/LM.
/// </summary>
public static unsafe partial class FhCall {

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate byte* GetLastMissionJobName(byte arg1, byte arg2);
    internal static FhMethodHandle<GetLastMissionJobName> h_GetLastMissionJobName
        => new( new FhMethodLocation("FFX-2.exe", 0x368570) );

}
