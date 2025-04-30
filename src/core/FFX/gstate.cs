using Fahrenheit.Core.FFX.Atel;
using Fahrenheit.Core.FFX.Battle;

namespace Fahrenheit.Core.FFX;

public unsafe static class Globals {
    public static LocalizationManager* localization_manager => (LocalizationManager*)FhUtil.get_at<nint>(0x8DED48);

    public static class Map {
        public static int*        tri_count => FhUtil.ptr_at<int>       (0xF01A48);
        public static VpaTri*     tris      => FhUtil.ptr_at<VpaTri>    (0xF01A44);
        public static VpaVertex*  vertices  => FhUtil.ptr_at<VpaVertex> (0xF01A4C);
        public static float*      scale     => FhUtil.ptr_at<float>     (0xF01A50);
        public static VpaNavMesh* navmesh   => FhUtil.ptr_at<VpaNavMesh>(0xF01A54);
    }

    public static class Magic {
        public static i32* current_id           => FhUtil.ptr_at<i32>(0x864CA0);
        public static i32* current_handle       => FhUtil.ptr_at<i32>(0x864CA8);
        public static i32* to_be_deleted_id     => FhUtil.ptr_at<i32>(0x864CA4);
        public static i32* to_be_deleted_handle => FhUtil.ptr_at<i32>(0x864CAC);
        public static i32* effect_ptr           => FhUtil.ptr_at<i32>(0xD33360);
        public static  u8* effect_status_flag   => FhUtil.ptr_at<u8> (0xD33364);
    }

    public static class Atel {
        public static int*                  request_count      => FhUtil.ptr_at<int>                 (0xD34564);
        public static AtelRequest*          request_list       => FhUtil.ptr_at<AtelRequest>         (0xD35D68);
        public static AtelWorkerController* controllers        => FhUtil.ptr_at<AtelWorkerController>(0xF25B60);
        public static AtelBasicWorker*      current_worker     => FhUtil.ptr_at<AtelBasicWorker>     (0xF270A4);
        public static AtelWorkerController* current_controller => FhUtil.ptr_at<AtelWorkerController>(0xF26AE8);
    }

    public static class SphereGrid {
        public static LpAbilityMapEngine* lpamng => (LpAbilityMapEngine*)FhUtil.get_at<nint>(0x1F05834);
    }

    public static class Battle {
        public static Chr* player_characters  => (Chr*)FhUtil.get_at<nint>(0xD334CC);
        public static Chr* monster_characters => (Chr*)FhUtil.get_at<nint>(0xD34460);
    }

    public static class OverdriveInfo {
        public static class Times {
            public static float* spiral_cut      => FhUtil.ptr_at<float>(0x886BF0);
            public static float* slice_and_dice  => FhUtil.ptr_at<float>(0x886BF4);
            public static float* energy_rain     => FhUtil.ptr_at<float>(0x886BF8);
            public static float* blitz_ace       => FhUtil.ptr_at<float>(0x886BFC);

            public static float* dragon_fang     => FhUtil.ptr_at<float>(0x886B60);
            public static float* shooting_star   => FhUtil.ptr_at<float>(0x886B64);
            public static float* banishing_blade => FhUtil.ptr_at<float>(0x886B68);
            public static float* tornado         => FhUtil.ptr_at<float>(0x886B6C);
        }
    }

    public static class Input {
        public static readonly InputAction l2       = new(0x1);
        public static readonly InputAction r2       = new(0x2);
        public static readonly InputAction l1       = new(0x4);
        public static readonly InputAction r1       = new(0x8);
        public static readonly InputAction square   = new(0x10);
        public static readonly InputAction confirm  = new(0x20);
        public static readonly InputAction cancel   = new(0x40);
        public static readonly InputAction triangle = new(0x80);
        public static readonly InputAction select   = new(0x100);
        public static readonly InputAction start    = new(0x800);
        public static readonly InputAction up       = new(0x1000);
        public static readonly InputAction right    = new(0x2000);
        public static readonly InputAction down     = new(0x4000);
        public static readonly InputAction left     = new(0x8000);

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

    public static Actor*         actors          => (Actor*)*FhUtil.ptr_at<nint>(0x1FC44E4);
    public static Btl*           btl             => FhUtil.ptr_at<Btl>          (0xD2A8D0);
    public static BtlRewardData* btl_reward_data => FhUtil.ptr_at<BtlRewardData>(0x1F10EA0);
    public static SaveData*      save_data       => FhUtil.ptr_at<SaveData>     (0xD2CA90);

    public static BtlWindow*       btl_windows        => FhUtil.ptr_at<BtlWindow>(0xF3C910);
    public static BtlWindow*       cur_btl_window     => btl_windows + FhUtil.get_at<byte>(0x1FCC092);
    public static BtlStatusWindow* btl_status_windows => FhUtil.ptr_at<BtlStatusWindow>(0xF3F798);

    public static byte* hit_chance_table => FhUtil.ptr_at<byte>(0x8421E0);

    public static i32* event_id => FhUtil.ptr_at<i32>(0xefbbf8);
}
