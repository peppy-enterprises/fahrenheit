namespace Fahrenheit.Core;

#pragma warning disable CS0649
public struct LVec3f {
    /* [fkelava 24/4/23 09:38]
     * `latched` vec3f
     */

    public float current;
    public float target;
    public float latch;
}

public struct Vec2s16 {
    public short x;
    public short y;

    public short this[int i] {
        get {
            return i switch {
                0 => x,
                1 => y,
                _ => throw new IndexOutOfRangeException(),
           };
        }

        set {
            switch (i) {
                case 0: x = value; break;
                case 1: y = value; break;
                default: throw new IndexOutOfRangeException();
           }
        }
    }
}

public struct Vec2f {
    public float x;
    public float y;

    public float this[int i] {
        get {
            return i switch {
                0 => x,
                1 => y,
                _ => throw new IndexOutOfRangeException(),
           };
        }

        set {
            switch (i) {
                case 0: x = value; break;
                case 1: y = value; break;
                default: throw new IndexOutOfRangeException();
           }
        }
    }
}

public struct Vec3f {
    public float x;
    public float y;
    public float z;

    public float this[int i] {
        get {
            return i switch {
                0 => x,
                1 => y,
                2 => z,
                _ => throw new IndexOutOfRangeException(),
           };
        }

        set {
            switch (i) {
                case 0: x = value; break;
                case 1: y = value; break;
                case 2: z = value; break;
                default: throw new IndexOutOfRangeException();
           }
        }
    }
}

public struct Vec4f {
    public float x;
    public float y;
    public float z;
    public float w;

    public float this[int i] {
        get {
            return i switch {
                0 => x,
                1 => y,
                2 => z,
                3 => w,
                _ => throw new IndexOutOfRangeException(),
           };
        }

        set {
            switch (i) {
                case 0: x = value; break;
                case 1: y = value; break;
                case 2: z = value; break;
                case 3: w = value; break;
                default: throw new IndexOutOfRangeException();
           }
        }
    }
}

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x40)]
public unsafe struct Mat4f {
    [FieldOffset(0x00)] public Vec4f x;
    [FieldOffset(0x10)] public Vec4f y;
    [FieldOffset(0x20)] public Vec4f z;
    [FieldOffset(0x30)] public Vec4f w;

    // Provide indexing
    public Vec4f this[int i] {
        get {
            if (i < 0 || i > 3) throw new IndexOutOfRangeException();
            fixed (Vec4f* p = &x)
            return *(p + i);
        }
    }

    // Matrix Multiplication
    public static Mat4f operator *(Mat4f l, Mat4f r) {
        return new Mat4f {
            x = new Vec4f {
                x = l[0][0] * r[0][0] + l[0][1] * r[1][0] + l[0][2] * r[2][0] + l[0][3] * r[3][0],
                y = l[0][0] * r[0][1] + l[0][1] * r[1][1] + l[0][2] * r[2][1] + l[0][3] * r[3][1],
                z = l[0][0] * r[0][2] + l[0][1] * r[1][2] + l[0][2] * r[2][2] + l[0][3] * r[3][2],
                w = l[0][0] * r[0][3] + l[0][1] * r[1][3] + l[0][2] * r[2][3] + l[0][3] * r[3][3],
            },
            y = new Vec4f {
                x = l[1][0] * r[0][0] + l[1][1] * r[1][0] + l[1][2] * r[2][0] + l[1][3] * r[3][0],
                y = l[1][0] * r[0][1] + l[1][1] * r[1][1] + l[1][2] * r[2][1] + l[1][3] * r[3][1],
                z = l[1][0] * r[0][2] + l[1][1] * r[1][2] + l[1][2] * r[2][2] + l[1][3] * r[3][2],
                w = l[1][0] * r[0][3] + l[1][1] * r[1][3] + l[1][2] * r[2][3] + l[1][3] * r[3][3],
            },
            z = new Vec4f {
                x = l[2][0] * r[0][0] + l[2][1] * r[1][0] + l[2][2] * r[2][0] + l[2][3] * r[3][0],
                y = l[2][0] * r[0][1] + l[2][1] * r[1][1] + l[2][2] * r[2][1] + l[2][3] * r[3][1],
                z = l[2][0] * r[0][2] + l[2][1] * r[1][2] + l[2][2] * r[2][2] + l[2][3] * r[3][2],
                w = l[2][0] * r[0][3] + l[2][1] * r[1][3] + l[2][2] * r[2][3] + l[2][3] * r[3][3],
            },
            w = new Vec4f {
                x = l[3][0] * r[0][0] + l[3][1] * r[1][0] + l[3][2] * r[2][0] + l[3][3] * r[3][0],
                y = l[3][0] * r[0][1] + l[3][1] * r[1][1] + l[3][2] * r[2][1] + l[3][3] * r[3][1],
                z = l[3][0] * r[0][2] + l[3][1] * r[1][2] + l[3][2] * r[2][2] + l[3][3] * r[3][2],
                w = l[3][0] * r[0][3] + l[3][1] * r[1][3] + l[3][2] * r[2][3] + l[3][3] * r[3][3],
            },
        };
    }
}
#pragma warning restore CS0649
