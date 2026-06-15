// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

using Fahrenheit.Atel;

namespace Fahrenheit.FFX;

public static unsafe class Call {
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void AtelInitCallFunc(
        uint             func_selector,
        AtelBasicWorker* work,
        nint*            storage,
        AtelStack*       stack);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate nint AtelExecCallFunc(
        uint             func_selector,
        AtelBasicWorker* work,
        AtelStack*       stack);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void AtelResultCallFunc(
        uint             func_selector,
        AtelBasicWorker* work,
        nint*            storage,
        AtelStack*       stack);

    /*===== CALL TARGETS =====*/
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void CallTargetInit(
        AtelBasicWorker* work,
        nint*            storage,
        AtelStack*       stack);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void CallTargetExec(
        AtelBasicWorker* work,
        AtelStack*       stack);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate float CallTargetResultFloat(
        AtelBasicWorker* work,
        nint*            storage,
        AtelStack*       stack);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int CallTargetResultInt(
        AtelBasicWorker* work,
        nint*            storage,
        AtelStack*       stack);
}
