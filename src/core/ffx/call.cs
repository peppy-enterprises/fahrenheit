// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* [fkelava 26/04/26 15:26]
 * Unlike `call.g.cs`, which contains source-generated delegates with no guarantee of accuracy,
 * this file contains manually annotated calls with proper Fahrenheit types that are vetted for functionality.
 *
 * This file is for calls which are exclusive to FF X and not shared with X-2/LM.
 */

namespace Fahrenheit.FFX;

/// <summary>
///     An accessor for game function calls exclusive to FF X.
/// </summary>
public static unsafe partial class FhCall {

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void FUN_2EFFF0();
    internal static FhMethodHandle<FUN_2EFFF0> h_FUN_2EFFF0
        => new( new FhMethodLocation("FFX.exe", 0x2EFFF0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte MsMessageCueProcess();
    public static FhMethodHandle<MsMessageCueProcess> h_MsMessageCueProcess
        => new( new FhMethodLocation("FFX.exe", 0x39CE10) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void TOBtlCloseSimpleHelpMes();
    public static FhMethodHandle<TOBtlCloseSimpleHelpMes> h_TOBtlCloseSimpleHelpMes
        => new( new FhMethodLocation("FFX.exe", 0x490E60) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int TOBtlDrawStdChrNameMessageWindow(int chr_id, int text_id);
    public static FhMethodHandle<TOBtlDrawStdChrNameMessageWindow> h_TOBtlDrawStdChrNameMessageWindow
        => new ( new FhMethodLocation("FFX.exe", 0x497170) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int TOBtlDrawFirstStrikePlayerMessageWindow();
    public static FhMethodHandle<TOBtlDrawFirstStrikePlayerMessageWindow> h_TOBtlDrawFirstStrikePlayerMessageWindow
        => new ( new FhMethodLocation("FFX.exe", 0x493460) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int TOBtlDrawFirstStrikeEnemyMessageWindow();
    public static FhMethodHandle<TOBtlDrawFirstStrikeEnemyMessageWindow> h_TOBtlDrawFirstStrikeEnemyMessageWindow
        => new ( new FhMethodLocation("FFX.exe", 0x493440) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int TOBtlDrawGetItemMessageWindow(byte* item_name, int amount);
    public static FhMethodHandle<TOBtlDrawGetItemMessageWindow> h_TOBtlDrawGetItemMessageWindow
        => new( new FhMethodLocation("FFX.exe", 0x493480) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int TOBtlDrawCaptureMonsterMessageWindow(int mon_id, int text_id);
    public static FhMethodHandle<TOBtlDrawCaptureMonsterMessageWindow> h_TOBtlDrawCaptureMonsterMessageWindow
        => new ( new FhMethodLocation("FFX.exe", 0x4927E0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int TOBtlDrawLearningMessageWindow(int ply_id, int com_id);
    public static FhMethodHandle<TOBtlDrawLearningMessageWindow> h_TOBtlDrawLearningMessageWindow
        => new ( new FhMethodLocation("FFX.exe", 0x495290) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int TOBtlDrawGetLimitTypeMessageWindow(int ply_id, int limit_mode);
    public static FhMethodHandle<TOBtlDrawGetLimitTypeMessageWindow> h_TOBtlDrawGetLimitTypeMessageWindow
        => new ( new FhMethodLocation("FFX.exe", 0x493560) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int TOBtlDrawGetMoneyMessageWindow(int amount);
    public static FhMethodHandle<TOBtlDrawGetMoneyMessageWindow> h_TOBtlDrawGetMoneyMessageWindow
        => new ( new FhMethodLocation("FFX.exe", 0x4935D0) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint MsRegSEplay(byte p1, int p2);
    public static FhMethodHandle<MsRegSEplay> h_MsRegSEplay
        => new ( new FhMethodLocation("FFX.exe", 0x3A0120) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int FUN_0089db10(int p1, byte* text);
    public static FhMethodHandle<FUN_0089db10> h_FUN_0089db10
        => new ( new FhMethodLocation("FFX.exe", 0x49DB10) );

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void AtelSetEventJump2(int room, int entrance, int do_fade);
    public static FhMethodHandle<AtelSetEventJump2> h_AtelSetEventJump2
        => new ( new FhMethodLocation("FFX.exe", 0x46FED0) );

}
