// SPDX-License-Identifier: MIT

/* [fkelava 26/04/26 15:26]
 * Unlike `call.g.cs`, which contains source-generated delegates with no guarantee of accuracy,
 * this file contains manually annotated calls with proper Fahrenheit types that are vetted for functionality.
 *
 * This file is for calls which are exclusive to FF X and not shared with X-2/LM.
 */

namespace Fahrenheit.FFX;

public static unsafe partial class FhCall {
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte MsMessageCueProcess();
    public const nint __addr_MsMessageCueProcess = 0x39CE10;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void TOBtlCloseSimpleHelpMes();
    public const nint __addr_TOBtlCloseSimpleHelpMes = 0x490E60;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int TOBtlDrawStdChrNameMessageWindow(int chr_id, int text_id);
    public const nint __addr_TOBtlDrawStdChrNameMessageWindow = 0x497170;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int TOBtlDrawFirstStrikePlayerMessageWindow();
    public const nint __addr_TOBtlDrawFirstStrikePlayerMessageWindow = 0x493460;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int TOBtlDrawFirstStrikeEnemyMessageWindow();
    public const nint __addr_TOBtlDrawFirstStrikeEnemyMessageWindow = 0x493440;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int TOBtlDrawGetItemMessageWindow(byte* item_name, int amount);
    public const nint __addr_TOBtlDrawGetItemMessageWindow = 0x493480;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int TOBtlDrawCaptureMonsterMessageWindow(int mon_id, int text_id);
    public const nint __addr_TOBtlDrawCaptureMonsterMessageWindow = 0x4927E0;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int TOBtlDrawLearningMessageWindow(int ply_id, int com_id);
    public const nint __addr_TOBtlDrawLearningMessageWindow = 0x495290;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int TOBtlDrawGetLimitTypeMessageWindow(int ply_id, int limit_mode);
    public const nint __addr_TOBtlDrawGetLimitTypeMessageWindow = 0x493560;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int TOBtlDrawGetMoneyMessageWindow(int amount);
    public const nint __addr_TOBtlDrawGetMoneyMessageWindow = 0x4935D0;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint MsRegSEplay(byte p1, int p2);
    public const nint __addr_MsRegSEplay = 0x3A0120;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int FUN_0089db10(int p1, byte* text);
    public const nint __addr_FUN_0089db10 = 0x49DB10;
}
