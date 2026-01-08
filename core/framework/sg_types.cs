// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core;

[InlineArray(0x20)]
public struct InlineArray32<T> {
    private T _b;
}

[InlineArray(0x40)]
public struct InlineArray64<T> {
    private T _b;
}

[InlineArray(0x80)]
public struct InlineArray128<T> {
    private T _b;
}

[InlineArray(0x200)]
public struct InlineArray512<T> {
    private T _b;
}

/* [fkelava 23/9/25 18:56]
 * These types are copied verbatim from ClangSharp `generate-helper-types` source.
 * They are used to preserve information about the source native type ClangSharp projected to C#.
 */

/// <summary>Defines the annotation found in a native declaration.</summary>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
[Conditional("DEBUG")]
public sealed partial class NativeAnnotationAttribute : Attribute {
    private readonly string _annotation;

    /// <summary>Initializes a new instance of the <see cref="NativeAnnotationAttribute" /> class.</summary>
    /// <param name="annotation">The annotation that was used in the native declaration.</param>
    public NativeAnnotationAttribute(string annotation) {
        _annotation = annotation;
    }

    /// <summary>Gets the annotation that was used in the native declaration.</summary>
    public string Annotation => _annotation;
}

/// <summary>Defines the type of a member as it was used in the native signature.</summary>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = true)]
[Conditional("DEBUG")]
public sealed partial class NativeTypeNameAttribute : Attribute {
    private readonly string _name;

    /// <summary>Initializes a new instance of the <see cref="NativeTypeNameAttribute" /> class.</summary>
    /// <param name="name">The name of the type that was used in the native signature.</param>
    public NativeTypeNameAttribute(string name) {
        _name = name;
    }

    /// <summary>Gets the name of the type that was used in the native signature.</summary>
    public string Name => _name;
}
