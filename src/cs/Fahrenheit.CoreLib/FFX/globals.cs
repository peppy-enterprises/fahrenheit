namespace Fahrenheit.CoreLib.FFX;

public static unsafe class Globals {
    public static nint game_base = FhPInvoke.GetModuleHandle("FFX.exe");
    private static nint ptr(nint address)
            => game_base + address;
    private static T get<T>(nint address) where T: unmanaged
            => *(T*)ptr(address);
    private static T set<T>(nint address, T value) where T: unmanaged
            => *(T*)ptr(address) = value;

    public static class Input {
        public static void update() {
            previous = *raw;
        }

        private static ushort previous;

        public static ushort* raw => (ushort*)ptr(0xF27080);

        public class InputAction {
            private ushort mask;

            public InputAction(ushort mask) {
                this.mask = mask;
            }

            public bool held => (*raw & mask) != 0;
            public bool just_pressed => (*raw & mask) != 0 && (previous & mask) == 0;
            public bool just_released => (*raw & mask) == 0 && (previous & mask) != 0;
        }

        public static readonly InputAction confirm = new(0x20);
        public static readonly InputAction cancel = new(0x40);
        public static readonly InputAction up = new(0x1000);
        public static readonly InputAction right = new(0x2000);
        public static readonly InputAction down = new(0x4000);
        public static readonly InputAction left = new(0x8000);
    }

    public static uint security_cookie => get<uint>(0x8613D8);

    public static class Map {
        public static int* tri_count => (int*)ptr(0xF01A48);
        public static VpaTri* tris => (VpaTri*)ptr(0xF01A44);
        public static VpaVertex* vertices => (VpaVertex*)ptr(0xF01A4C);
        public static float* scale => (float*)ptr(0xF01A50);
        public static VpaNavMesh* navmesh => (VpaNavMesh*)ptr(0xF01A54);
    }

    public static BtlStruct* btl => (BtlStruct*)ptr(0xD2A8D0);
    public static BtlDataStruct* btl_data => (BtlDataStruct*)ptr(0x1F10EA0);
    public static SaveDataStruct* save_data => (SaveDataStruct*)ptr(0xD2CA90);
    public static ParamDataStruct* param_data => (ParamDataStruct*)ptr(0x1F11240);

    public static int* atel_request_count => (int*)ptr(0xD34564);
    public static AtelRequest* atel_request_list => (AtelRequest*)ptr(0xD35D68);

    public static AtelWorkerController* atel_ctrl_workers => (AtelWorkerController*)ptr(0xF25B60);

    public static AtelBasicWorker* cur_atel_worker => (AtelBasicWorker*)ptr(0xF270A4);
    public static AtelWorkerController* cur_ctrl_atel_worker => (AtelWorkerController*)ptr(0xF26AE8);

    public static BtlComWindow* btl_com_windows => (BtlComWindow*)ptr(0xF3C910);
    public static BtlStatusWindow* btl_status_windows => (BtlStatusWindow*)ptr(0xF3F798);

    public static class OverdriveInfo {
        public static class Times {
            public static float* spiral_cut => (float*)ptr(0x886BF0);
            public static float* slice_and_dice => (float*)ptr(0x886BF4);
            public static float* energy_rain => (float*)ptr(0x886BF8);
            public static float* blitz_ace => (float*)ptr(0x886BFC);

            public static float* dragon_fang => (float*)ptr(0x886B60);
            public static float* shooting_star => (float*)ptr(0x886B64);
            public static float* banishing_blade => (float*)ptr(0x886B68);
            public static float* tornado => (float*)ptr(0x886B6C);
        }
    }

    public static byte* hit_chance_table => (byte*)ptr(0x8421E0);
}
