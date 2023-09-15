namespace Fahrenheit.CoreLib;

public static unsafe class FhXGlobals {
	public static nint game_base = FhPInvoke.GetModuleHandle("FFX.exe");
	public static ushort* pad_input = (ushort*)(game_base + 0xF27080);
	public static FhXBtlStruct* btl = (FhXBtlStruct*)(game_base + 0xD2A8D0);
	public static FhXBtlDataStruct* btl_data = (FhXBtlDataStruct*)(game_base + 0x1F10EA0);
	public static FhXSaveDataStruct* save_data = (FhXSaveDataStruct*)(game_base + 0xD2CA90);
	public static FhXParamDataStruct* param_data = (FhXParamDataStruct*)(game_base + 0x1F11240);
	public static FhXAtelBasicWorker* cur_atel_worker = (FhXAtelBasicWorker*)(game_base + 0xF270A4);
	public static FhXBtlComWindow* btl_com_windows = (FhXBtlComWindow*)(game_base + 0xF3C910);
	public static FhXBtlStatusWindow* btl_status_windows = (FhXBtlStatusWindow*)(game_base + 0xF3F798);
	public static float* tidus_limit_times = (float*)(game_base + 0x886BF0);
	public static float* auron_limit_times = (float*)(game_base + 0x886B60);
	public static byte* hit_chance_table = (byte*)(game_base + 0x8421E0);
}

