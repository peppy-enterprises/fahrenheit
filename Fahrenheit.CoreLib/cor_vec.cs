namespace Fahrenheit.CoreLib;

#pragma warning disable CS0649
internal struct LVec3f
{
    /* [fkelava 24/4/23 09:38]
     * `latched` vec3f
     */

    public float current;
    public float target;
    public float latch;
}

internal struct Vec3f
{
    public float x;
    public float y;
    public float z;
}

internal struct Vec4f
{
    public float x;
    public float y;
    public float z;
    public float w;
}

internal struct Mat4f
{
    public Vec4f x;
    public Vec4f y;
    public Vec4f z;
    public Vec4f w;
}
#pragma warning restore CS0649
