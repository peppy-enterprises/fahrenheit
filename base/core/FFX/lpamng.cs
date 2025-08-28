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
    [InlineArray(128)]
    public struct SphereGridClusterArray {
        private SphereGridCluster _data;
    }

    [InlineArray(1024)]
    public struct SphereGridNodeArray {
        private SphereGridNode _data;
    }

    [InlineArray(1024)]
    public struct SphereGridLinkArray {
        private SphereGridLink _data;
    }

    [InlineArray(8)]
    public struct SphereGridClusterSizesArray {
        private Vec2s16 _data;
    }

    [InlineArray(130)]
    public struct SphereGridNodeTypeInfoArray {
        private SphereGridNodeTypeInfo _data;
    }

    [InlineArray(7)]
    public struct SphereGridChrInfoArray {
        private SphereGridChrInfo _data;
    }

    [FieldOffset(0x0)]     public short                  cluster_count;
    [FieldOffset(0x2)]     public short                  node_count;
    [FieldOffset(0x4)]     public short                  link_count;
    [FieldOffset(0x8)]     public SphereGridClusterArray clusters;
    [FieldOffset(0x808)]   public SphereGridNodeArray    nodes;
    [FieldOffset(0xA808)]  public SphereGridLinkArray    links;
    [FieldOffset(0xF808)]  public SphereGridClusterSizesArray cluster_sizes;
    [FieldOffset(0xF828)]  public SphereGridNodeTypeInfoArray node_type_infos;
    [FieldOffset(0x11088)] public SphereGridChrInfoArray      party_infos;
    [FieldOffset(0x112FC)] public ushort                 selected_node_idx;
    [FieldOffset(0x11308)] public Vector4                cam_desired_pos;
    [FieldOffset(0x11318)] public Vector4                cam_limited_pos;
    [FieldOffset(0x11350)] public Vector4                zoom_vector; // Only .x matters
    [FieldOffset(0x115CB)] public SphereGridTilt         tilt_level;
    [FieldOffset(0x115CC)] public SphereGridZoom         zoom_level;
    [FieldOffset(0x115D0)] public ushort                 zoom_time_left; // in frames
    [FieldOffset(0x115DC)] public float                  start_zoom;
    [FieldOffset(0x115E0)] public float                  target_zoom;
    [FieldOffset(0x116A8)] public int                    should_update_node; // -1 updates all nodes
    [FieldOffset(0x116AC)] public int                    should_update;


    public float current_zoom {
        get => zoom_vector.X;
        set => zoom_vector.X = zoom_vector.Y = zoom_vector.Z = value;
    }
}
