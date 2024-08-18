namespace Fahrenheit.CoreLib;

#pragma warning disable CS0649
public struct LVec3f {
    /* [fkelava 24/4/23 09:38]
     * `latched` vec3f
     */

    public float current;
    public float target;
    public float latch;
}

public struct Vec3f {
    public float x;
    public float y;
    public float z;
}

public struct Vec4f {
    public float x;
    public float y;
    public float z;
    public float w;
}

public struct Mat4f {
    public Vec4f x;
    public Vec4f y;
    public Vec4f z;
    public Vec4f w;
}
#pragma warning restore CS0649
