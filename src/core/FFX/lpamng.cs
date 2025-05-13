namespace Fahrenheit.Core.FFX;

public enum SphereGridTilt : byte {
    Flat,
    SlightTilt,
    FarTilt,
}

public enum SphereGridZoom : byte {
    Close,
    Medium,
    Far,
    VeryFar, // supported but not allowed by vanilla
}

public static class SphereGridZoomExt {
    public static float get_zoom(this SphereGridZoom zoom_level) {
        return zoom_level switch {
            SphereGridZoom.VeryFar => 0.125f,
            SphereGridZoom.Far     => 0.25f,
            SphereGridZoom.Medium  => 0.5f,
            SphereGridZoom.Close   => 1.0f,
            _                      => 0.5f,
        };
    }

    public static SphereGridZoom get_closest(float zoom, bool allow_very_far = false) {
        return zoom switch {
             <= 0.1875f => allow_very_far ? SphereGridZoom.VeryFar : SphereGridZoom.Far,
             <= 0.375f  => SphereGridZoom.Far,
             <= 0.75f   => SphereGridZoom.Medium,
            _           => SphereGridZoom.Close,
        };
    }
}

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
    [FieldOffset(0x11308)] public  Vec4f                  cam_desired_pos;
    [FieldOffset(0x11318)] public  Vec4f                  cam_limited_pos;
    [FieldOffset(0x11350)] public  Vec4f                  zoom_vector; // Only .x matters
    [FieldOffset(0x115CB)] public  SphereGridTilt         tilt_level;
    [FieldOffset(0x115CC)] public  SphereGridZoom         zoom_level;
    [FieldOffset(0x115D0)] public  ushort                 zoom_time_left; // in frames
    [FieldOffset(0x115DC)] public  float                  start_zoom;
    [FieldOffset(0x115E0)] public  float                  target_zoom;

    public Span<SphereGridCluster>      clusters        => MemoryMarshal.CreateSpan(ref _clusters_start,        128);
    public Span<SphereGridNode>         nodes           => MemoryMarshal.CreateSpan(ref _nodes_start,           1024);
    public Span<SphereGridLink>         links           => MemoryMarshal.CreateSpan(ref _links_start,           1024);
    public Span<Vec2s16>                cluster_sizes   => MemoryMarshal.CreateSpan(ref _cluster_sizes_start,   8);
    public Span<SphereGridNodeTypeInfo> node_type_infos => MemoryMarshal.CreateSpan(ref _node_type_infos_start, 130);
    public Span<SphereGridChrInfo>      party_infos     => MemoryMarshal.CreateSpan(ref _party_infos_start,     7);

    public float current_zoom {
        get {
            return zoom_vector.x;
        }

        set {
            zoom_vector.x = zoom_vector.y = zoom_vector.z = value;
        }
    }
}
