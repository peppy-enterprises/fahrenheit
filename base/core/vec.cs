namespace Fahrenheit.Core;

#pragma warning disable CS0649
public struct LVec3f {
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

    public Vector2 xy => new(x, y);
}
#pragma warning restore CS0649
