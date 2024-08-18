namespace Fahrenheit.CoreLib.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x12FC0)]
public unsafe struct LpAbilityMapEngine {
    [FieldOffset(0x0)] public short cluster_count;
    [FieldOffset(0x2)] public short node_count;
    [FieldOffset(0x4)] public short link_count;
    [FieldOffset(0x8)] private SphereGridCluster _clusters_start;
    public System.Span<SphereGridCluster> clusters => _clusters_start.array(128);

    [FieldOffset(0x808)] private SphereGridNode _nodes_start;
    public System.Span<SphereGridNode> nodes => _nodes_start.array(1024);

    [FieldOffset(0xA808)] private SphereGridLink _links_start;
    public System.Span<SphereGridLink> links => _links_start.array(1024);

    [FieldOffset(0xF808)] private Vec2s16 _cluster_sizes_start;
    public System.Span<Vec2s16> cluster_sizes => _cluster_sizes_start.array(8);

    [FieldOffset(0xF828)] private SphereGridNodeTypeInfo _node_type_infos_start;
    public System.Span<SphereGridNodeTypeInfo> node_type_infos => _node_type_infos_start.array(130);

    [FieldOffset(0x11088)] private SphereGridChrInfo _party_infos_start;
    public System.Span<SphereGridChrInfo> party_infos => _party_infos_start.array(7);

    [FieldOffset(0x112FC)] public ushort selected_node_idx;
}
