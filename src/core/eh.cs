// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit;

/// <summary>
///     Provides exception handling methods; in our case, 'handling' means 'logging'.
/// </summary>
internal static class FhExceptionHandler {
    private static int _lock_eh_first_chance = 0;

    /* [fkelava 11/06/26 23:02]
     * Normally only an unhandled exception logger would be provided, if not for the mildly annoying fact
     * it sporadically won't fire. For this reason we also log first-chance exceptions, even though that
     * can result in double (or even triple) logging of certain exceptions.
     *
     * Note also that severe faults such as AVs are not interceptible in modern .NET.
     * These only function for run-of-the-mill managed exceptions.
     */

    internal static void eh_first_chance(object? sender, FirstChanceExceptionEventArgs e) {
        /* [fkelava 11/06/26 11:28]
         * See https://learn.microsoft.com/en-us/dotnet/api/system.appdomain.firstchanceexception?view=net-10.0#remarks.
         * > You must handle all exceptions that occur in the event handler
         * > for the FirstChanceException event. Otherwise, FirstChanceException is raised recursively.
         */
        if (Interlocked.CompareExchange(ref _lock_eh_first_chance, 1, 0) == 1) return;

        try {
            FhInternal.Log.Log(FhLogLevel.Fatal, "================================");
            FhInternal.Log.Log(FhLogLevel.Fatal, $"First-chance exception at: {TimeProvider.System.GetUtcNow():u}");
            FhInternal.Log.Log(FhLogLevel.Fatal, e.Exception.ToString());
            FhInternal.Log.Log(FhLogLevel.Fatal, "================================");
        }
        catch   { }
        finally { Interlocked.CompareExchange(ref _lock_eh_first_chance, 0, 1); }
    }

    internal static void eh_unhandled(object? sender, UnhandledExceptionEventArgs e) {
        try {
            Exception ex = (Exception) e.ExceptionObject;

            FhInternal.Log.Log(FhLogLevel.Fatal, "================================");
            FhInternal.Log.Log(FhLogLevel.Fatal, $"Unhandled exception at: {TimeProvider.System.GetUtcNow():u}");
            FhInternal.Log.Log(FhLogLevel.Fatal, ex.ToString());
            FhInternal.Log.Log(FhLogLevel.Fatal, "=============================");
        }
        catch { }
    }
}
