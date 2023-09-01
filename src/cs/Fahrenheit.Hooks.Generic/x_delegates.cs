using Fahrenheit.CoreLib;
using System.Runtime.InteropServices;

namespace Fahrenheit.Hooks.Generic;

public static unsafe class FhXDelegates {
	/*===== Util =====*/

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	public delegate int MsCheckRange(int n, int min, int max);

	/*===== RNG =====*/

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate uint getChrRngIdx(uint chr_id, uint type);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate uint brnd(uint rng_seed_idx);


	/*===== Save Data =====*/
	
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate int MsSaveItemUse(uint item_id, int amount);

	/*===== Excel Data =====*/

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate nint MsGetExcelData(uint req_elem_idx, nint excel_data_ptr, nint storage_var);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate FhXMemCom* MsGetRomItem(uint item_id, nint param_2);

	/*===== MsChr =====*/

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate FhXChr* MsGetChr(uint chr_id);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate FhXChr* MsGetMon(byte mon_id);

	/*===== Battle =====*/

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate nint MsApUp(uint chr_id, FhXChr* chr, int base_ap_add, nint param_4);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate int MsCalcDamage(FhXChr* user, FhXChr* target, FhXMemCom* command,
		byte dmg_formula, int power, byte target_status__0x606, byte targetted_stat,
		uint should_vary, ref byte used_def, ref byte used_mdef, int base_dmg);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate bool MsSetRikkuLimit(uint chr_id, FhXChr* chr, nint param_3);

	/*===== ATEL =====*/

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	public delegate void AtelJumpGameOver();

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate nint AtelExecCallFunc(uint func_selector, nint* work, nint* stack);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void AtelResultCallFunc(uint func_selector, nint* work, nint* storage, nint* stack_ptr);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate int AtelPopStackInteger(nint* work, nint* stack_ptr);

	/*===== CALL TARGETS =====*/

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	public delegate void CallTargetInit(nint* work, nint* storage, nint* stack);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	public delegate void CallTargetExec(nint* work, nint* stack);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate float CallTargetResultFloat(nint* work, nint* storage, nint* stack);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate int CallTargetResultInt(nint* work, nint* storage, nint* stack);
}