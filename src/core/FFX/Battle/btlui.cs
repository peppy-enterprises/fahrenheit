namespace Fahrenheit.Core.FFX.Battle;

public enum BtlWindowType : ushort {
    Main        = 0x00,
    BlackMagic  = 0x01,
    WhiteMagic  = 0x02,
    Skill       = 0x03,
    Overdrive   = 0x04,
    Summon      = 0x05,
    Item        = 0x06,
    Weapon      = 0x07,
    Change      = 0x0A,
    Left        = 0x0C,
    Right       = 0x0D,
    Special     = 0x0E,
    Armor       = 0x0F,
    Use         = 0x11,
    Mix         = 0x14,
    SpareChange = 0x15,
    YojimboPay  = 0x16,
}

[StructLayout(LayoutKind.Explicit, Pack = 0x4, Size = 0xF0)]
public unsafe struct BtlWindow {
    [FieldOffset(0x01)] public byte          state;
    [FieldOffset(0x06)] public BtlWindowType type;
    [FieldOffset(0x08)] public byte          cur_chr_id;
    [FieldOffset(0x0B)] public byte          submenu_depth;
    [FieldOffset(0x0C)] public ushort        window_command_id;
    [FieldOffset(0x0E)] public byte          sel_idx_render;
    [FieldOffset(0x16)] public ushort        sel_command_id_render;
    [FieldOffset(0x1E)] public ushort        sel_command_id;
    [FieldOffset(0x20)] public ushort*       command_list; // Array of len 'command_list_len'
    [FieldOffset(0x24)] public uint          command_list_len;
    [FieldOffset(0x40)] public byte          scroll_pos;
    [FieldOffset(0x42)] public byte          sel_idx;
    [FieldOffset(0x44)] public uint          valid_sel_idx_amount;
    [FieldOffset(0x68)] public float         commands_base_x;
    [FieldOffset(0x6C)] public float         mask_start_y;
    [FieldOffset(0x70)] public float         mask_end_x;
    [FieldOffset(0x74)] public float         mask_end_y;
    [FieldOffset(0x78)] public float         mask_start_x;
    [FieldOffset(0x7C)] public float         commands_base_y;
    [FieldOffset(0xD8)] public ushort        sel_chrs;
    [FieldOffset(0xDA)] public ushort        sel_mons;
    [FieldOffset(0xE0)] public float         seconds_spent_in_menu;
}

[StructLayout(LayoutKind.Explicit, Pack = 0x4, Size = 0x90)]
public struct BtlStatusWindow {
    [FieldOffset(0x0C)] public uint hp;
    [FieldOffset(0x10)] public uint hp2;
    [FieldOffset(0x19)] public byte font_color;
    [FieldOffset(0x24)] public uint mp;
    [FieldOffset(0x28)] public uint mp2;
    [FieldOffset(0x34)] public uint charged_limit_bar_width;
    [FieldOffset(0x38)] public uint charged_limit_bar_width2;
}
