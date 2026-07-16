// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit;

/* [fkelava 23/9/25 18:56]
 * For sizes up to 16, the standard library provides inline array types.
 * We use those where possible. For larger sizes, we define our own here.
 */

// TODO: Create customized types for the uses of non-power-of-two InlineArrays, and replace all uses thereof.

[InlineArray(17)]
public struct InlineArray17<T> {
    private T _t;
}

[InlineArray(20)]
public struct InlineArray20<T> {
    private T _t;
}

[InlineArray(30)]
public struct InlineArray30<T> {
    private T _t;
}

[InlineArray(31)]
public struct InlineArray31<T> {
    private T _t;
}

[InlineArray(32)]
public struct InlineArray32<T> {
    private T _t;
}

[InlineArray(37)]
public struct InlineArray37<T> {
    private T _t;
}

[InlineArray(42)]
public struct InlineArray42<T> {
    private T _t;
}

[InlineArray(64)]
public struct InlineArray64<T> {
    private T _t;
}

[InlineArray(68)]
public struct InlineArray68<T> {
    private T _t;
}

[InlineArray(128)]
public struct InlineArray128<T> {
    private T _t;
}

[InlineArray(370)]
public struct InlineArray370<T> {
    private T _t;
}

[InlineArray(512)]
public struct InlineArray512<T> {
    private T _t;
}

[InlineArray(16384)]
public struct InlineArray16384<T> {
    private T _t;
}
