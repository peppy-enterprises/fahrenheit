// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit;

#pragma warning disable CS0649
public struct LVec3f {
    public float current;
    public float target;
    public float latch;
}

public struct Vector4i {
    public int x;
    public int y;
    public int z;
    public int w;
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
