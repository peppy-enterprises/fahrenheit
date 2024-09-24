namespace Fahrenheit.CoreLib.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x10)]
public unsafe struct SphereGridCluster {
    [FieldOffset(0x0)] public short x;
    [FieldOffset(0x2)] public short y;
    [FieldOffset(0x6)] public short type;
}

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x14)]
public unsafe struct SphereGridLink {
    [FieldOffset(0x0)]  public ushort               node_a_idx;
    [FieldOffset(0x2)]  public ushort               node_b_idx;
    [FieldOffset(0x4)]  public ushort               anchor_idx;
    [FieldOffset(0xC)]  public byte                 activated_by;
    [FieldOffset(0xD)]  public byte                 point_count;
    [FieldOffset(0x10)] public SphereGridLinkPoint* points;

    public readonly SphereGridNode node_a => Globals.SphereGrid.lpamng->nodes[node_a_idx];
    public readonly SphereGridNode node_b => Globals.SphereGrid.lpamng->nodes[node_b_idx];
    public readonly SphereGridNode anchor => Globals.SphereGrid.lpamng->nodes[anchor_idx];
}

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x18)]
public unsafe struct SphereGridLinkPoint {
    [FieldOffset(0x0)]  public float x;
    [FieldOffset(0x4)]  public float y;
    [FieldOffset(0x8)]  public float offset_x1;
    [FieldOffset(0xC)]  public float offset_y2;
    [FieldOffset(0x10)] public float offset_x2;
    [FieldOffset(0x14)] public float offset_y1;
}

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x28)]
public unsafe struct SphereGridNode {
    [FieldOffset(0x0)]  public       short  x;
    [FieldOffset(0x2)]  public       short  y;
    [FieldOffset(0x6)]  public       ushort node_type;
    [FieldOffset(0xC)]  public fixed uint   links_ptr[5];
    [FieldOffset(0x21)] public       byte   activated_by;
}

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x30)]
public unsafe struct SphereGridNodeTypeInfo {
    [FieldOffset(0xC)] public short width;
    [FieldOffset(0xE)] public short height;
}

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x50)]
public unsafe struct SphereGridChrInfo {
    [FieldOffset(0x0)]  public Vec4f  pos;
    [FieldOffset(0x10)] public Vec4f  label_pos;
    [FieldOffset(0x2C)] public byte*  chr_name;
    [FieldOffset(0x30)] public short  name_width; // min 32
    [FieldOffset(0x3C)] public float  pos_circle_radius;
    [FieldOffset(0x44)] public ushort current_node_idx;
}
