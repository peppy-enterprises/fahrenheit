using System.IO;

using ImGuiNET;

using Fahrenheit.CoreLib;
using Fahrenheit.CoreLib.FFX;
using static Fahrenheit.Modules.Debug.FuncLib;

namespace Fahrenheit.Modules.Debug;

public static unsafe class SphereGridEditor {
    public enum NodeType {
        Lock_3 = 0x00,
        EmptyNode = 0x01,
        Str_1 = 0x02,
        Str_2 = 0x03,
        Str_3 = 0x04,
        Str_4 = 0x05,
        Def_1 = 0x06,
        Def_2 = 0x07,
        Def_3 = 0x08,
        Def_4 = 0x09,
        Mag_1 = 0x0A,
        Mag_2 = 0x0B,
        Mag_3 = 0x0C,
        Mag_4 = 0x0D,
        MDef_1 = 0x0E,
        MDef_2 = 0x0F,
        MDef_3 = 0x10,
        MDef_4 = 0x11,
        Agi_1 = 0x12,
        Agi_2 = 0x13,
        Agi_3 = 0x14,
        Agi_4 = 0x15,
        Lck_1 = 0x16,
        Lck_2 = 0x17,
        Lck_3 = 0x18,
        Lck_4 = 0x19,
        Eva_1 = 0x1A,
        Eva_2 = 0x1B,
        Eva_3 = 0x1C,
        Eva_4 = 0x1D,
        Acc_1 = 0x1E,
        Acc_2 = 0x1F,
        Acc_3 = 0x20,
        Acc_4 = 0x21,
        HP_200 = 0x22,
        HP_300 = 0x23,
        MP_40 = 0x24,
        MP_20 = 0x25,
        MP_10 = 0x26,
        Lock_1 = 0x27,
        Lock_2 = 0x28,
        Lock_4 = 0x29,
        DelayAttack = 0x2A,
        DelayBuster = 0x2B,
        SleepAttack = 0x2C,
        SilenceAttack = 0x2D,
        DarkAttack = 0x2E,
        ZombieAttack = 0x2F,
        SleepBuster = 0x30,
        SilenceBuster = 0x31,
        DarkBuster = 0x32,
        TripleFoul = 0x33,
        PowerBreak = 0x34,
        MagicBreak = 0x35,
        ArmorBreak = 0x36,
        MentalBreak = 0x37,
        Mug = 0x38,
        QuickHit = 0x39,
        Steal = 0x3A,
        Use = 0x3B,
        Flee = 0x3C,
        Pray = 0x3D,
        Cheer = 0x3E,
        Focus = 0x3F,
        Reflex = 0x40,
        Aim = 0x41,
        Luck = 0x42,
        Jinx = 0x43,
        Lancet = 0x44,
        Guard = 0x45,
        Sentinel = 0x46,
        SpareChange = 0x47,
        Threaten = 0x48,
        Provoke = 0x49,
        Entrust = 0x4A,
        Copycat = 0x4B,
        Doublecast = 0x4C,
        Bribe = 0x4D,
        Cure = 0x4E,
        Cura = 0x4F,
        Curaga = 0x50,
        NulFrost = 0x51,
        NulBlaze = 0x52,
        NulShock = 0x53,
        NulTide = 0x54,
        Scan = 0x55,
        Esuna = 0x56,
        Life = 0x57,
        FullLife = 0x58,
        Haste = 0x59,
        Hastega = 0x5A,
        Slow = 0x5B,
        Slowga = 0x5C,
        Shell = 0x5D,
        Protect = 0x5E,
        Reflect = 0x5F,
        Dispel = 0x60,
        Regen = 0x61,
        Holy = 0x62,
        AutoLife = 0x63,
        Blizzard = 0x64,
        Fire = 0x65,
        Thunder = 0x66,
        Water = 0x67,
        Fira = 0x68,
        Blizzara = 0x69,
        Thundara = 0x6A,
        Watera = 0x6B,
        Firaga = 0x6C,
        Blizzaga = 0x6D,
        Thundaga = 0x6E,
        Waterga = 0x6F,
        Bio = 0x70,
        Demi = 0x71,
        Death = 0x72,
        Drain = 0x73,
        Osmose = 0x74,
        Flare = 0x75,
        Ultima = 0x76,
        PilferGil = 0x77,
        FullBreak = 0x78,
        ExtractPower = 0x79,
        ExtractMana = 0x7A,
        ExtractSpeed = 0x7B,
        ExtractAbility = 0x7C,
        NabGil = 0x7D,
        QuickPockets = 0x7E,
        Null = 0xFF,
    }

    public static System.Collections.Generic.LinkedList<NodeType> NODE_TYPE_ORDER = new(
        new NodeType[] {
            NodeType.EmptyNode,
            NodeType.Lock_1, NodeType.Lock_2, NodeType.Lock_3, NodeType.Lock_4,

            NodeType.HP_200, NodeType.HP_300,
            NodeType.MP_10, NodeType.MP_20, NodeType.MP_40,

            NodeType.Str_1, NodeType.Str_2, NodeType.Str_3, NodeType.Str_4,
            NodeType.Mag_1, NodeType.Mag_2, NodeType.Mag_3, NodeType.Mag_4,
            NodeType.Def_1, NodeType.Def_2, NodeType.Def_3, NodeType.Def_4,
            NodeType.MDef_1, NodeType.MDef_2, NodeType.MDef_3, NodeType.MDef_4,
            NodeType.Acc_1, NodeType.Acc_2, NodeType.Acc_3, NodeType.Acc_4,
            NodeType.Eva_1, NodeType.Eva_2, NodeType.Eva_3, NodeType.Eva_4,
            NodeType.Agi_1, NodeType.Agi_2, NodeType.Agi_3, NodeType.Agi_4,
            NodeType.Lck_1, NodeType.Lck_2, NodeType.Lck_3, NodeType.Lck_4,

            NodeType.Fire, NodeType.Blizzard, NodeType.Water, NodeType.Thunder,
            NodeType.Fira, NodeType.Blizzara, NodeType.Watera, NodeType.Thundara,
            NodeType.Firaga, NodeType.Blizzaga, NodeType.Waterga, NodeType.Thundaga,
            NodeType.Bio, NodeType.Death,
            NodeType.Flare, NodeType.Ultima,
            NodeType.Doublecast,
            NodeType.Demi,
            NodeType.Drain, NodeType.Osmose, NodeType.Lancet,

            NodeType.DarkAttack, NodeType.DarkBuster,
            NodeType.SilenceAttack, NodeType.SilenceBuster,
            NodeType.SleepAttack, NodeType.SleepBuster,
            NodeType.TripleFoul,
            NodeType.ZombieAttack,

            NodeType.DelayAttack, NodeType.DelayBuster,
            NodeType.QuickHit,

            NodeType.Steal, NodeType.Mug,
            NodeType.Use, NodeType.QuickPockets,
            NodeType.PilferGil, NodeType.NabGil,
            NodeType.SpareChange, NodeType.Bribe,
            NodeType.Copycat,

            NodeType.Guard, NodeType.Sentinel,
            NodeType.Provoke, NodeType.Threaten,
            NodeType.Entrust,

            NodeType.Esuna,
            NodeType.Cure, NodeType.Cura, NodeType.Curaga,
            NodeType.Life, NodeType.FullLife, NodeType.AutoLife,
            NodeType.NulBlaze, NodeType.NulFrost, NodeType.NulTide, NodeType.NulShock,
            NodeType.Protect, NodeType.Shell,
            NodeType.Reflect, NodeType.Dispel,
            NodeType.Pray, NodeType.Regen,
            NodeType.Holy,

            NodeType.Scan,
            NodeType.Cheer, NodeType.Focus,
            NodeType.Aim, NodeType.Reflex,
            NodeType.Luck, NodeType.Jinx,

            NodeType.Haste, NodeType.Hastega,
            NodeType.Slow, NodeType.Slowga,
            NodeType.Flee,

            NodeType.PowerBreak, NodeType.MagicBreak,
            NodeType.ArmorBreak, NodeType.MentalBreak,
            NodeType.FullBreak,

            NodeType.ExtractPower, NodeType.ExtractMana,
            NodeType.ExtractSpeed, NodeType.ExtractAbility,
        }
    );

    public const string OUT_PATH_STANDARD = ".\\sphere-grid-standard.bin";
    public const string OUT_PATH_EXPERT = ".\\sphere-grid-expert.bin";
    public const string OUT_PATH_ORIGINAL = ".\\sphere-grid-original.bin";
    public const int MAX_NODE_COUNT = 1024;
    private const int INPUT_FRAME_DELAY = 5;

    private static LpAbilityMapEngine* lpamng => Globals.SphereGrid.lpamng;

    private static int input_delay = 0;
    private static bool enabled = false;
    private static int node_count;
    private static NodeType new_node_type;

    public static void handle_input() {
        if (input_delay > 0) {
            input_delay--;
            if (enabled) {
                Globals.Input.l1.consume();
                Globals.Input.r1.consume();
                Globals.Input.select.consume();
                Globals.Input.square.consume();
                Globals.Input.confirm.consume();
                Globals.Input.cancel.consume();
            }
            return;
        }

        if (/* abmap is open && */ Globals.Input.start.held && Globals.Input.triangle.just_pressed) {
            enabled = !enabled;
            end_input();
            return;
        }

        if (!enabled) return;

        if (Globals.Input.l1.held) {
            cycle_node_type_prev();
            end_input();
            return;
        }

        if (Globals.Input.r1.held) {
            cycle_node_type_next();
            end_input();
            return;
        }

        if (Globals.Input.triangle.held && Globals.Input.start.just_pressed) {
            save();
            end_input();
            return;
        }

        Globals.Input.l1.consume();
        Globals.Input.r1.consume();
        Globals.Input.select.consume();
        Globals.Input.square.consume();
        Globals.Input.confirm.consume();
        Globals.Input.cancel.consume();
    }

    private static void end_input() {
        Globals.Input.consume_all();
        input_delay = INPUT_FRAME_DELAY;
    }

    private static void cycle_node_type_next() {
        i32 cur_idx = lpamng->selected_node_idx;

        NodeType cur_node = (NodeType)lpamng->nodes[cur_idx].node_type;
        System.Collections.Generic.LinkedListNode<NodeType> next_node = NODE_TYPE_ORDER.Find(cur_node).Next;
        next_node ??= NODE_TYPE_ORDER.First;

        new_node_type = next_node.Value;

        FUN_00a48740((i32)new_node_type, cur_idx);
        SndSepPlaySimple(0x8000006d);
    }

    private static void cycle_node_type_prev() {
        i32 cur_idx = lpamng->selected_node_idx;

        NodeType cur_node = (NodeType)lpamng->nodes[cur_idx].node_type;
        System.Collections.Generic.LinkedListNode<NodeType> next_node = NODE_TYPE_ORDER.Find(cur_node).Previous;
        next_node ??= NODE_TYPE_ORDER.Last;

        new_node_type = next_node.Value;

        FUN_00a48740((i32)new_node_type, cur_idx);
        SndSepPlaySimple(0x8000006d);
    }

    public static void update_node_type() {
        lpamng->nodes[lpamng->selected_node_idx].node_type = (u16)new_node_type;
    }

    public static void render() {
        if (!enabled) return;

         TOMkpCrossExtMesFontLClut(
                0, FhCharset.Us.to_bytes(lpamng->selected_node_idx.ToString()),
                50f, 50f, 0x00, 0, 0.69f, 0);

        ImGui.ImGui_ImplDX11_NewFrame();
        ImGui.ImGui_ImplWin32_NewFrame();
        ImGui.NewFrame();

        ImGui.ShowDemoWindow();

        ImGui.Render();

        // Hope it works?
        ImGui.ImGui_ImplDX11_RenderDrawData(ImGui.GetDrawData());
    }

    public static void save() {
        string path = Globals.save_data->config_grid_type == 2 ? OUT_PATH_EXPERT : Globals.save_data->config_grid_type == 1 ? OUT_PATH_STANDARD : OUT_PATH_ORIGINAL;
        byte[] data = new byte[lpamng->node_count];

        for (int i = 0; i < lpamng->node_count; i++) {
            data[i] = (u8)lpamng->nodes[i].node_type;
        }

        using (FileStream fs = new(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)) {
            fs.Write(data);
            fs.SetLength(data.Length);
        }

        SndSepPlaySimple(0x80000070);

    }
}
