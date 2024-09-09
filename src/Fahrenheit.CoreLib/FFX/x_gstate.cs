namespace Fahrenheit.CoreLib.FFX;

public static unsafe class Globals {
    public static class Map {
        public static int*        tri_count { get { return FhUtil.ptr_at<int>       (0xF01A48); } }
        public static VpaTri*     tris      { get { return FhUtil.ptr_at<VpaTri>    (0xF01A44); } }
        public static VpaVertex*  vertices  { get { return FhUtil.ptr_at<VpaVertex> (0xF01A4C); } }
        public static float*      scale     { get { return FhUtil.ptr_at<float>     (0xF01A50); } }
        public static VpaNavMesh* navmesh   { get { return FhUtil.ptr_at<VpaNavMesh>(0xF01A54); } }
    }

    public static class SphereGrid {
        public static LpAbilityMapEngine* lpamng { get { return (LpAbilityMapEngine*)FhUtil.get_at<nint>(0x1F05834); } }
    }

    public static class Battle {
        public static Chr* player_characters  { get { return (Chr*)FhUtil.get_at<nint>(0xD334CC); } }
        public static Chr* monster_characters { get { return (Chr*)FhUtil.get_at<nint>(0xD34460); } }
    }

    public static class OverdriveInfo {
        public static class Times {
            public static float* spiral_cut      { get { return FhUtil.ptr_at<float>(0x886BF0); } }
            public static float* slice_and_dice  { get { return FhUtil.ptr_at<float>(0x886BF4); } }
            public static float* energy_rain     { get { return FhUtil.ptr_at<float>(0x886BF8); } }
            public static float* blitz_ace       { get { return FhUtil.ptr_at<float>(0x886BFC); } }

            public static float* dragon_fang     { get { return FhUtil.ptr_at<float>(0x886B60); } }
            public static float* shooting_star   { get { return FhUtil.ptr_at<float>(0x886B64); } }
            public static float* banishing_blade { get { return FhUtil.ptr_at<float>(0x886B68); } }
            public static float* tornado         { get { return FhUtil.ptr_at<float>(0x886B6C); } }
        }
    }

    public static class Input {
        public static readonly InputAction l2 = new(0x1);
        public static readonly InputAction r2 = new(0x2);
        public static readonly InputAction l1 = new(0x4);
        public static readonly InputAction r1 = new(0x8);

        public static readonly InputAction square   = new(0x10);
        public static readonly InputAction confirm  = new(0x20);
        public static readonly InputAction cancel   = new(0x40);
        public static readonly InputAction triangle = new(0x80);

        public static readonly InputAction select = new(0x100);
        public static readonly InputAction start  = new(0x800);

        public static readonly InputAction up    = new(0x1000);
        public static readonly InputAction right = new(0x2000);
        public static readonly InputAction down  = new(0x4000);
        public static readonly InputAction left  = new(0x8000);

        public static ushort* raw { get { return FhUtil.ptr_at<ushort>(0xF27080); } }

        private static ushort previous;

        public static void update() {
            previous = *raw;
        }

        public static void consume_all() {
            previous = *raw = 0;
        }

        public class InputAction {
            private ushort mask;

            public InputAction(ushort mask) {
                this.mask = mask;
            }

            public (bool held, bool just_pressed, bool just_released) consume() {
                (bool, bool, bool) ret = (held, just_pressed, just_released);
                *raw &= (ushort)~mask;
                return ret;
            }

            public bool held          { get { return (*raw & mask) != 0;                           } }
            public bool just_pressed  { get { return (*raw & mask) != 0 && (previous & mask) == 0; } }
            public bool just_released { get { return (*raw & mask) == 0 && (previous & mask) != 0; } }
        }
    }

    public static Btl*             btl             { get { return FhUtil.ptr_at<Btl>            (0xD2A8D0);  } }
    public static BtlRewardData*   btl_reward_data { get { return FhUtil.ptr_at<BtlRewardData>  (0x1F10EA0); } }
    public static SaveData*        save_data       { get { return FhUtil.ptr_at<SaveData>       (0xD2CA90);  } }
    //public static ParamDataStruct* param_data      { get { return FhUtil.ptr_at<ParamDataStruct>(0x1F11240); } }

    public static int*         atel_request_count { get { return FhUtil.ptr_at<int>(0xD34564);         } }
    public static AtelRequest* atel_request_list  { get { return FhUtil.ptr_at<AtelRequest>(0xD35D68); } }

    public static AtelWorkerController* atel_ctrl_workers { get { return FhUtil.ptr_at<AtelWorkerController>(0xF25B60); } }

    public static AtelBasicWorker*      cur_atel_worker      { get { return FhUtil.ptr_at<AtelBasicWorker>     (0xF270A4); } }
    public static AtelWorkerController* cur_ctrl_atel_worker { get { return FhUtil.ptr_at<AtelWorkerController>(0xF26AE8); } }

    public static BtlWindow* btl_windows    { get { return FhUtil.ptr_at<BtlWindow>         (0xF3C910);  } }
    public static BtlWindow* cur_btl_window { get { return btl_windows + FhUtil.get_at<byte>(0x1FCC092); } }

    public static BtlStatusWindow* btl_status_windows { get { return FhUtil.ptr_at<BtlStatusWindow>(0xF3F798); } }

    public static byte* hit_chance_table { get { return FhUtil.ptr_at<byte>(0x8421E0); } }

}