using System;

namespace Fahrenheit.CoreLib.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x12FC0)]
public unsafe struct LpAbilityMapEngine {
    [FieldOffset(0x0)]     public  short                  cluster_count;
    [FieldOffset(0x2)]     public  short                  node_count;
    [FieldOffset(0x4)]     public  short                  link_count;
    [FieldOffset(0x8)]     private SphereGridCluster      _clusters_start;
    [FieldOffset(0x808)]   private SphereGridNode         _nodes_start;
    [FieldOffset(0xA808)]  private SphereGridLink         _links_start;
    [FieldOffset(0xF808)]  private Vec2s16                _cluster_sizes_start;
    [FieldOffset(0xF828)]  private SphereGridNodeTypeInfo _node_type_infos_start;
    [FieldOffset(0x11088)] private SphereGridChrInfo      _party_infos_start;
    [FieldOffset(0x112FC)] public  ushort                 selected_node_idx;

    public Span<SphereGridCluster>      clusters        => MemoryMarshal.CreateSpan(ref _clusters_start,        128);
    public Span<SphereGridNode>         nodes           => MemoryMarshal.CreateSpan(ref _nodes_start,           1024);
    public Span<SphereGridLink>         links           => MemoryMarshal.CreateSpan(ref _links_start,           1024);
    public Span<Vec2s16>                cluster_sizes   => MemoryMarshal.CreateSpan(ref _cluster_sizes_start,   8);
    public Span<SphereGridNodeTypeInfo> node_type_infos => MemoryMarshal.CreateSpan(ref _node_type_infos_start, 130);
    public Span<SphereGridChrInfo>      party_infos     => MemoryMarshal.CreateSpan(ref _party_infos_start,     7);
}