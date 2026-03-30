// SPDX-License-Identifier: MIT

/* [fkelava 23/03/26 00:41]
 * The game's remasters all utilize the cross-platform Phyre game engine. All assets have been processed
 * in some way, resulting in ``*.phyre`` files which are not inspectable or loadable using standard tooling.
 *
 * Fahrenheit allows custom textures to be used in ImGui flows. It would be desirable to use game assets as well,
 * but we can't directly load Phyre-processed assets. While tools such as Roelin's Asset Converter
 * (https://www.nexusmods.com/finalfantasy12/mods/288) can 'un-Phyre' files, it is ABSOLUTELY PROHIBITED
 * to distribute them with mods. However, we _can_ ask the game to load them for us at runtime! These types exist to enable this.
 *
 * Phyre types are generally self-describing. That is to say, Phyre classes have 'class descriptors', which
 * contain information about the type such as its name and layout. Their constructors, destructors, and vftables
 * all remain in the executable's RTTI metadata, and that information was used to construct these interop types.
 *
 * Once the basic set of types was reversed sufficiently, the 'global' Phyre namespace was queried for all of its class
 * descriptors, and the information they yielded was used to reconstruct the rest of the type graph.
 *
 * Sources within the executable are annotated for every attribute, and all are relative to FFX.exe.
 */

namespace Fahrenheit;

/* [fkelava 19/03/26 03:01]
 * .ctor                  -> +3A170
 * RTTI inheritance graph -> +793EE8
 */

/// <summary>
///     Describes a unique member of a Phyre class.
///     <para/>
///     A <see cref="PClassMember"/> can be a field, function or method. Thus, it has no <see cref="PType"/>.
///     <para/>
///     For fields, derived types <see cref="PClassData"/> and/or <see cref="PClassDataMember"/> carry type information.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 0x14, Pack = 4)]
internal unsafe struct PClassMember {
    public PSimpleDoubleListElement<PClassMember> base_PSimpleDoubleListElement;
    public PClassDescriptor*                      m_classDescriptor; // name: +70EA88 -> +3A362
    public nint                                   ptr_name;          // null-terminated ANSI/UTF-8 string
    public uint                                   m_flags;           // name: +70EA80 -> +3A316

    public override string ToString() {
        return $"{(*m_classDescriptor).base_PType}::{Marshal.PtrToStringAnsi(ptr_name)}";
    }
}

/* [fkelava 17/03/26 04:13]
 * ctors of this class are all inlined; name inferred from RTTI inheritance graph of PClassDataMember
 *
 * RTTI inheritance graph -> +793E88
 */

/// <summary>
///     Describes a unique member of a Phyre class which has a concrete <see cref="PType"/>.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 0x18, Pack = 4)]
internal unsafe struct PClassData {
    public PClassMember base_PClassMember;
    public PType*       m_type; // name: +70F994 -> +465A6

    public override string ToString() {
        return $"{*m_type} {base_PClassMember}";
    }
}

/* [fkelava 19/03/26 03:01]
 * .ctor                  -> +1759B0
 * RTTI inheritance graph -> +79459C
 */

/// <summary>
///     Annotations are arbitrary tags attached to Phyre types.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 0x18, Pack = 4)]
internal unsafe struct PAnnotation {
    public nint                                  vftable;
    public PSimpleDoubleListElement<PAnnotation> base_PSimpleDoubleListElement;
    public PAnnotationSemantic*                  _0x0C;
    public PType*                                _0x10_type;
    public nint                                  _0x14;
}

/* [fkelava 17/03/26 04:13]
 * ctors of this class are all inlined; name inferred from RTTI inheritance graph of PClassDescriptor
 *
 * RTTI inheritance graph -> +7928DC
 */

/// <summary>
///     Indicates that a Phyre type can have <see cref="PAnnotation"/>s, arbitrary tags, associated with it.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 0x8, Pack = 4)]
internal struct PAnnotatable {
    public PSimpleDoubleListElement<PAnnotation> _0x00;
}

/* [fkelava 19/03/26 03:01]
 * .ctor                  -> +175410
 * RTTI inheritance graph -> +793E3C
 */

/// <summary>
///     Describes a unique field of a Phyre class which has a concrete <see cref="PType"/>, offset, and may have <see cref="PAnnotation"/>s associated with it.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 0x2C, Pack = 4)]
internal unsafe struct PClassDataMember {
    public nint         vftable;
    public PClassData   base_PClassData;   // 0x4
    public PAnnotatable base_PAnnotatable; // 0x1C
    public nint         m_offset;          // name: +70F99C -> +465F2
    public nint         __0x28;

    public override string ToString() {
        return $"{base_PClassData} at offset 0x{m_offset:X}";
    }
}

/* [fkelava 25/03/26 14:49]
 * ctors of this class are all inlined; name inferred from RTTI inheritance graph of PType
 *
 * RTTI inheritance graph -> +792758
 */

[StructLayout(LayoutKind.Sequential, Size = 0x8, Pack = 4)]
internal unsafe struct PHashEntryConstKey {
    public nint _0x00;
    public nint _0x04;
}

/* [fkelava 25/03/26 14:49]
 * ctors of this class are all inlined; name inferred from RTTI inheritance graph of PType
 *
 * RTTI inheritance graph -> +7926C4
 */

[StructLayout(LayoutKind.Sequential, Size = 0x14, Pack = 4)]
internal unsafe struct PHashEntryConst {
    public PRedBlackTreeNode<PUnknown> base_PRedBlackTreeNode;
    public PHashEntryConstKey          base_PHashEntryConstKey;
}

/* [fkelava 25/03/26 14:49]
 * ctors of this class are all inlined; name inferred from RTTI inheritance graph of PType
 *
 * RTTI inheritance graph -> +792720
 */

[StructLayout(LayoutKind.Sequential, Size = 0xC, Pack = 4)]
internal unsafe struct PRedBlackTreeNode<T> where T : unmanaged {

    /* [fkelava 25/03/26 16:36]
     * https://www.eecs.umich.edu/courses/eecs380/ALG/red_black.html
     * These must be a 'left', 'right', and 'parent' pointer,
     * but which is which is unknown.
     */

    public PRedBlackTreeNode<T>* _0x00;
    public PRedBlackTreeNode<T>* _0x04;
    public PRedBlackTreeNode<T>* _0x08;
}

/* [fkelava 13/03/26 21:49]
 * See Phyre::TypeDispenser::GetType<Phyre::PType>
 * (+375D0)
 *
 * RTTI inheritance graph -> +79265C
 */

/// <summary>
///     Describes a unique type in the Phyre type system- its name, size, alignment, and more.
///     <para/>
///     Classes have a derived <see cref="PClassDescriptor"/> instead, providing information about their members, layout and inheritance chain.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 0x2C, Pack = 4)]
internal struct PType {
    public nint            vftable;
    public PHashEntryConst base_PHashEntryConst;
    public nint            type_name; // null-terminated ANSI/UTF-8 string
    public nint            type_size;
    public nint            type_alignment;
    public nint            fnptr_fixup_get;
    public nint            fnptr_fixup_resolve;

    public override string ToString() {
        return $"{Marshal.PtrToStringAnsi(type_name)} (sz 0x{type_size:X}, align 0x{type_alignment:X})";
    }
}

/* [fkelava 25/03/26 22:29]
 * .ctor -> +1763A0
 */

[StructLayout(LayoutKind.Sequential, Size = 0x4C, Pack = 4)]
internal unsafe struct PInstanceList {
    public PSimpleDoubleListElement<PInstanceList> base_PSimpleDoubleListElement;
    public PFreeList<PUnknown>                     _0x08_free_list;
    public PSimpleDoubleListElement<PUnknown>      _0x20;
    public PCluster*                               ptr_cluster;          // 0x28
    public PClassDescriptor*                       ptr_class_descriptor; // 0x2C
    public nint                                    _0x30;
    public nint                                    _0x34;
    public nint                                    _0x38;
    public nint                                    _0x3C;
    public nint                                    _0x40;
    public nint                                    _0x44;
    public nint                                    _0x48;

    public override string ToString() {
        return $"{nameof(PInstanceList)}<{(*ptr_class_descriptor).base_PType}>";
    }
}

/* [fkelava 17/03/26 04:13]
 * ctors of this class are all inlined; name inferred from RTTI metadata of instantiations:
 *
 * +824D90 - Phyre::PNamedSemanticDescriptorForType<PAnnotationSemantic>
 * +1758A0 - Phyre::PTypeDispenser::GetType<Phyre::PAnnotationSemantic>
 *
 * size inferred from +5758D3 - PNamedSemanticDescriptor::.ctor()
 */

[StructLayout(LayoutKind.Sequential, Size = 0x8, Pack = 4)]
internal struct PAnnotationSemantic {
    public nint _0x00;
    public nint _0x04;
}

/* [fkelava 17/03/26 04:13]
 * .ctor                  -> +3B0A0, +3B190
 * RTTI inheritance graph -> +792834
 */

/// <summary>
///     Describes a Phyre class- its layout, members, and inheritance chain.
///     <para/>
///     Basic type information such as name, size and alignment is provided by the base class <see cref="PType"/>.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 0x94, Pack = 4)]
internal unsafe struct PClassDescriptor {

    /* [fkelava 25/03/26 17:25]
     * Plenty of things remain unclear.
     *
     * - What are the 'default', 'validation' and 'write mask' buffers?
     * - 'Offset to base' is ambiguous; which base? Most classes have multiple inheritance.
     * - How and when does 'offset to base' vary from 'offset to base in allocated block'?
     * - Why are there two function and two field doubly-linked lists?
     */

    public PType                                      base_PType;
    public PSimpleDoubleListElement<PClassDescriptor> base_PSimpleDoubleListElement;
    public PAnnotatable                               base_PAnnotatable;
    public PNamespace*                                _0x3C_namespace;
    public PClassDescriptor*                          m_parent;                      // 0x40 - name: +70FB5C -> +47F02
    public PSimpleDoubleListElement<PClassDataMember> _0x44;                         // seems to be used for field members?
    public PSimpleDoubleListElement<PClassMember>     _0x4C;                         // seems to be used for function members?
    public PSimpleDoubleListElement<PClassMember>     _0x54;                         // seems to be used for function members?
    public PSimpleDoubleListElement<PClassDataMember> _0x5C;                         // seems to be used for field members?
    public nint                                       buffer_default;                // 0x64 - name: +3D600 (Phyre::PClassDescriptor::setDefaultBuffer)
    public nint                                       buffer_validation;             // 0x68 - name: +3D6A0 (Phyre::PClassDescriptor::setValidationBuffer)
    public nint                                       buffer_write_mask;             // 0x6C - name: +3D6B0 (Phyre::PClassDescriptor::setWriteMaskBuffer)
    public nint                                       _0x70;
    public nint                                       _0x74;
    public nint                                       _0x78;
    public nint                                       _0x7C;
    public nint                                       _0x80;
    public nint                                       m_offsetToParent;               // 0x84 - name: +70FB68 -> +47F50
    public nint                                       m_offsetToBase;                 // 0x88 - name: +70FB7C -> +47F9F
    public nint                                       m_offsetToBaseInAllocatedBlock; // 0x8C - name: +70FB8C -> +47FEE
    public nint                                       _0x90_flags;                    // 0x90

    public override string ToString() {
        return $"{nameof(PClassDescriptor)}<{base_PType}>";
    }
}

/// <inheritdoc cref="PClassDescriptor"/>
[StructLayout(LayoutKind.Sequential, Size = 0xB0, Pack = 4)]
internal struct PClassDescriptorDynamic {
    public PClassDescriptor base_PClassDescriptor; // 0x90
    public nint             m_actualSize;          // 0x94 - name: +70FBC4, +4808C
}

/* [fkelava 17/03/26 04:13]
 * ctors of this class are all inlined; name inferred from RTTI metadata of numerous derivations
 *
 * +80A160, 80B180, 80B620, 80E448, 8312F8 - Phyre::PSimpleDoubleListElement<>
 */

/// <summary>
///     An element of a doubly-linked list of <typeparamref name="T"/>.
///     <para/>
///     Unlike typical implementations, the absence of an element is not denoted by <c>null</c>,
///     but by the next and/or previous pointer being equal to the head pointer.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 0x8, Pack = 4)]
internal unsafe readonly struct PSimpleDoubleListElement<T> where T : unmanaged {
    public readonly PSimpleDoubleListElement<T>* ptr_next;
    public readonly PSimpleDoubleListElement<T>* ptr_prev;

    public readonly T* next(PSimpleDoubleListElement<T>* head) {
        if (ptr_next == head) return null;
        return (T*)ptr_next;
    }

    public readonly T* prev(PSimpleDoubleListElement<T>* head) {
        if (ptr_prev == head) return null;
        return (T*)ptr_prev;
    }
}

/* [fkelava 11/03/26 20:00]
 * .ctor -> +9C550
 * .dtor -> +9C680
 */

/// <summary>
///     A free list of <typeparamref name="T"/>.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 0x18, Pack = 4)]
internal unsafe struct PFreeList<T> where T : unmanaged {
    public T*   _0x00_head;       // inferred from various iterator methods
    public nint _0x04_size;
    public nint _0x08_block_size;
    public nint _0x0C_alignment;
    public nint _0x10_name;       // null-terminated ANSI/UTF-8 string
    public nint _0x14;

    public override string ToString() {
        return $"{nameof(PFreeList<>)}<{Marshal.PtrToStringAnsi(_0x10_name)}>";
    }
}

/* [fkelava 11/03/26 20:00]
 * .ctor -> +3DE70
 */

/// <summary>
///     A namespace is a group of <see cref="PClassDescriptor"/>s. It can contain nested namespaces as well.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 0x1C, Pack = 4)]
internal struct PNamespace {
    public PSimpleDoubleListElement<PNamespace>       base_PSimpleDoubleListElement;
    public PSimpleDoubleListElement<PClassDescriptor> _0x08_class_descriptors;
    public nint                                       m_index;              // name: +24FE9
    public PSimpleDoubleListElement<PNamespace>       _0x14_sub_namespaces;
}

/* [fkelava 19/03/26 16:20]
 * .ctor -> +3F910
 */

/// <summary>
///     A cluster is, roughly spoken, a Phyre asset container.
///     <para/>
///     On disk, a cluster contains the asset in question, references to other
///     assets that must be simultaneously loaded, and more.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 0x50, Pack = 4)]
internal unsafe struct PCluster {
    public PNamespace                              _0x00_namespace;
    public PSimpleDoubleListElement<PInstanceList> _0x1C_instance_lists;
    public nint                                    _0x24;
    public PFreeList<PInstanceList>                _0x28_free_list_PInstanceList;
    public PWorld*                                 ptr_world; // deduced from Phyre::PWorld::addInstanceList (+433F0)
    public nint                                    _0x44;
    public PSimpleDoubleListElement<PUnknown>      _0x48;
}

/* [fkelava 19/03/26 16:20]
 * .ctor -> +1A0390
 */

/* [fkelava 25/03/26 17:01]
 * The purpose of this class is not entirely clear, because only the constructor remains
 * but _not_ the RTTI inheritance graph which would perhaps shed more light.
 */

[StructLayout(LayoutKind.Sequential, Size = 0x34, Pack = 4)]
internal struct PNamedSemanticDescriptor {
    public PType                              base_PType;
    public PSimpleDoubleListElement<PUnknown> base_PSimpleDoubleListElement;
}

/// <summary>
///     Allows iteration over a Phyre doubly-linked list of <typeparamref name="T"/>.
/// </summary>
internal unsafe struct FhPDoubleListIterator<T>(PSimpleDoubleListElement<T>* head, bool forward = true) where T : unmanaged {
    private PSimpleDoubleListElement<T>* _m_head    = head;
    private T*                           _m_current = forward ? head->next(head) : head->prev(head);

    /* [fkelava 23/03/26 15:29]
     * Here we encounter an unfortunate C++ standard/implementation detail.
     *
     * https://en.cppreference.com/w/cpp/language/derived_class.html
     * > Each direct and indirect base class is present, as base class subobject,
     * > within the object representation of the derived class at an ABI-dependent offset.
     *
     * Phyre types have complex inheritance graphs, and often inherit multiple classes.
     * The problem is that the compiler is free to order such base classes as it desires
     * from struct to struct within the same compilation unit.
     *
     * Many structs inherit from PSimpleDoubleListElement. Pointers in such doubly-linked lists
     * are offset by where PSimpleDoubleListElement is laid out in a given type. Successfully iterating
     * the list requires us to manually correct, which is exactly what the compiler does under the hood.
     *
     * An example of a class that suffers from this is PClassDescriptor, where PType goes first.
     * e.g. in Phyre::PNamespace::InitializeGlobalClassDescriptors (+3E530)
     *
     * 0043e54e 83 c6 d4        ADD        ESI,-0x2c // subtract size of PType to get to beginning of struct
     * 0043e551 74 17           JZ         LAB_0043e56a
     *                      LAB_0043e553                                    XREF[1]:     0043e568(j)
     * 0043e553 8b ce           MOV        ECX,ESI
     * 0043e555 e8 76 f2        CALL       Phyre::PClassDescriptor::updateBaseOffsets
     *          ff ff
     */

    private static T* abi_fixup(T* ptr_object) {
        return ptr_object switch {
            _ when typeof(T) == typeof(PClassDescriptor) => (T*)((nint)ptr_object - sizeof(PType)),
            _ when typeof(T) == typeof(PClassDataMember) => (T*)((nint)ptr_object - sizeof(nint)),
            _                                            => ptr_object,
        };
    }

    public bool next(out T* item) {
        item = default;

        if (_m_current == null)
            return false;

        item = FhPDoubleListIterator<T>.abi_fixup(_m_current);
        _m_current = forward
            ? ((PSimpleDoubleListElement<T>*)_m_current)->next(_m_head)
            : ((PSimpleDoubleListElement<T>*)_m_current)->prev(_m_head);

        return true;
    }

    public bool next(out T item) {
        item = default;

        if (_m_current == null)
            return false;

        item = *FhPDoubleListIterator<T>.abi_fixup(_m_current);
        _m_current = forward
            ? ((PSimpleDoubleListElement<T>*)_m_current)->next(_m_head)
            : ((PSimpleDoubleListElement<T>*)_m_current)->prev(_m_head);

        return true;
    }
}

/// <summary>
///     Used when a target Phyre type is not known, as a placeholder.
///     <para/>
///     This type is a stub and corresponds to no Phyre type.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 0x40, Pack = 4)]
internal struct PUnknown {
    public nint _0x00;
    public nint _0x04;
    public nint _0x08;
    public nint _0x0C;
    public nint _0x10;
    public nint _0x14;
    public nint _0x18;
    public nint _0x1C;
    public nint _0x20;
    public nint _0x24;
    public nint _0x28;
    public nint _0x2C;
    public nint _0x30;
    public nint _0x34;
    public nint _0x38;
    public nint _0x3C;
}

/* [fkelava 25/03/26 17:04]
 * Types from this point onward were obtained by querying the global PNamespace for its class descriptors.
 *
 * Call Phyre::PNamespace::GetGlobalNamespace (+3E3E0), iterate over its class descriptors, then recurse over all sub-namespaces.
 */

/* [fkelava 25/03/26 18:30]
 * Derived from:
 *
 * PClassDescriptor<PShaderParameterCaptureBufferLocation> -> +89B720
 */

[StructLayout(LayoutKind.Sequential, Size = 0x2, Pack = 0x2)]
internal struct PShaderParameterCaptureBufferLocation {
    public ushort m_offset;
}

/* [fkelava 25/03/26 18:30]
 * Derived from:
 *
 * PClassDescriptor<PShaderParameterCaptureBufferLocationSize> -> +89B788
 */

[StructLayout(LayoutKind.Sequential, Size = 0x4, Pack = 0x4)]
internal struct PShaderParameterCaptureBufferLocationSize {
    public PShaderParameterCaptureBufferLocation base_PShaderParameterCaptureBufferLocation;
    public ushort                                m_size;
}

/* [fkelava 25/03/26 18:30]
 * Derived from:
 *
 * PClassDescriptor<PShaderParameterDefinition> -> +89B980
 */

[StructLayout(LayoutKind.Sequential, Size = 0x10, Pack = 0x4)]
internal struct PShaderParameterDefinition {
    public ushort                                    m_arrayElementCount;      // 0x0 - name: + -> +
    public byte                                      m_parameterType;          // 0x2 - name: + -> +
    public byte                                      m_dataType;               // 0x3 - name: + -> +
    public PString                                   m_name;                   // 0x4 - name: + -> +
    public PShaderParameterCaptureBufferLocationSize m_bufferLoc;              // 0x8 - name: + -> +
    public nint                                      m_constantBufferLocation; // 0xC - name: + -> +
}


/* [fkelava 25/03/26 18:30]
 * Derived from:
 *
 * PClassDescriptor<PString> -> +891048
 */

/// <summary>
///     A simple null-terminated string.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 0x4, Pack = 0x4)]
internal unsafe struct PString {

    /* [fkelava 26/03/26 13:45]
     * For future reference, I know that _assuming_ sizeof(nint) == 4 is silly,
     * but we currently only support the Steam Windows release, which is 32-bit.
     *
     * If they ever recompile it, we're going to have to remap every address anyway,
     * so we may as well adjust structures that abuse 'nint' then.
     */

    public nint m_buffer;

    public override string ToString() {
        return Marshal.PtrToStringAnsi(m_buffer) ?? string.Empty;
    }
}

/* [fkelava 25/03/26 18:30]
 * Derived from:
 *
 * PClassDescriptor<PEffect> -> +8A66E8
 */

[StructLayout(LayoutKind.Sequential, Size = 0x40, Pack = 0x4)]
internal struct PEffect {
    public uint             m_supportedLightMask;          // 0x0
    public uint             m_supportedShadowCasterMask;   // 0x4
    public PString          m_effectFile;                  // 0x8

    public PArray<PUnknown> m_contextVariantSwitches;      // 0x2C
    public PString          m_effectSource;                // 0x34
    public uint             m_maxLightCount;               // 0x38
    public uint             m_numSupportedShaderLODLevels; // 0x3C
}

/* [fkelava 25/03/26 18:30]
 * Derived from:
 *
 * PClassDescriptor<PEffectVariant> -> +8A5FF0
 */

[StructLayout(LayoutKind.Sequential, Size = 0x34, Pack = 0x4)]
internal unsafe struct PEffectVariant {
    public PEffect*         m_effect;                                 // 0x0  - name: + -> +
    public PArray<PUnknown> m_switches;                               // 0x4  - name: + -> +
    public PArray<PUnknown> m_sceneRenderPasses;                      // 0xC  - name: + -> +
    public PArray<PUnknown> m_sceneRenderPassLookup;                  // 0x14 - name: + -> +
    public ushort           m_largestShaderPassCount;                 // 0x1C - name: + -> +
    public ushort           _0x1E;                                    // padding
    public PArray<PUnknown> m_tweakableShaderParameterDefinitions;    // 0x20 - name: + -> +
    public PArray<PUnknown> m_untweakableShaderParameterDefinitions;  // 0x28 - name: + -> +
    public ushort           m_tweakableParameterBufferSize;           // 0x30 - name: + -> +
    public ushort           m_untweakableParameterBufferSize;         // 0x32 - name: + -> +
}

/* [fkelava 25/03/26 18:30]
 * Derived from:
 *
 * PClassDescriptor<PParameterBufferBase> -> +8A4FA8
 */

[StructLayout(LayoutKind.Sequential, Size = 0x4, Pack = 0x4)]
internal struct PParameterBufferBase {
    public uint m_parameterBufferSize;
}

/* [fkelava 25/03/26 18:30]
 * Derived from:
 *
 * PClassDescriptor<PParameterBuffer> -> +8A5040
 */

[StructLayout(LayoutKind.Sequential, Size = 0x10, Pack = 0x4)]
internal struct PParameterBuffer {
    public PParameterBufferBase               base_PParameterBufferBase;             // 0x0
    public PEffectVariant                     m_effectVariant;                       // 0x4
    public PArray<PShaderParameterDefinition> m_tweakableShaderParameterDefinitions; // 0x8
}

/* [fkelava 25/03/26 18:30]
 * Derived from all class descriptors which are template instantiations of PArray, e.g.
 *
 * +1545010 (PArray<PPostEffectBase*>)
 * +15448A0 (PArray<PPostEffectManager>)
 * +8A9AB0  (PArray<float>) etc.
 *
 * In Ghidra each specialization must be presented explicitly.
 */

[StructLayout(LayoutKind.Sequential, Size = 0x8, Pack = 0x4)]
internal unsafe struct PArray<T> where T : unmanaged {
    public uint m_count; // name: +70FD3C -> +90AE0 (and many others)
    public T*   m_els;   // name: +70FD80 -> +90BC1 (and many others)

    [UnscopedRef]
    public readonly Span<T> as_span() => new Span<T>(m_els, int.CreateChecked(m_count));
}

[StructLayout(LayoutKind.Sequential, Size = 0x14, Pack = 0x4)]
internal struct PDataBlockBufferD3D11 { }

[StructLayout(LayoutKind.Sequential, Size = 0x14, Pack = 0x4)]
internal struct PIndexDataBlockBufferD3D11 { }

/* [fkelava 25/03/26 18:30]
 * Derived from:
 *
 * Phyre::PFramework::PInputMapper::Bind() -> +222D20
 * PClassDescriptor<PInputMapper>          -> +8C9E50
 */

[StructLayout(LayoutKind.Sequential, Size = 0xB8, Pack = 0x4)]
internal struct PInputMapper { }

/* [fkelava 25/03/26 18:30]
 * Derived from:
 *
 * Phyre::PFramework::PApplication::Bind() -> +222210
 * PClassDescriptor<PApplication>          -> +8C9D20
 */

[StructLayout(LayoutKind.Sequential, Size = 0x3A0, Pack = 0x10)]
internal struct PApplication { }

/* [fkelava 25/03/26 18:30]
 * Derived from:
 *
 * Phyre::PFramework::PApplicationViewport::Bind() -> +222970
 * PClassDescriptor<PApplicationViewport>          -> +8C9DB8
 */

[StructLayout(LayoutKind.Sequential, Size = 0x28, Pack = 0x4)]
internal struct PApplicationViewport { }

/* [fkelava 25/03/26 18:30]
 * Derived from:
 *
 * Phyre::PSerialization::PBinary::PInternal::PInstanceListHeader::Bind() -> +6A400
 * PClassDescriptor<PInstanceListHeader>                                  -> +897338
 */

[StructLayout(LayoutKind.Sequential, Size = 0x24, Pack = 0x4)]
internal struct PInstanceListHeader {
    public uint m_classID;                // 0x0  - name: +713330 -> +6A446
    public uint m_count;                  // 0x4  - name: +70FD3C -> +6A492
    public uint m_size;                   // 0x8  - name: +70FB54 -> +6A4DE
    public uint m_objectsSize;            // 0xC  - name: +71333C -> +6A52A
    public uint m_arraysSize;             // 0x10 - name: +71334C -> +6A576
    public uint m_pointersInArraysCount;  // 0x14 - name: +712F84 -> +6A5C2
    public uint m_arrayFixupCount;        // 0x18 - name: +712F14 -> +6A60E
    public uint m_pointerFixupCount;      // 0x1C - name: +712F3C -> +6A65C
    public uint m_pointerArrayFixupCount; // 0x20 - name: +712F68 -> +6A6AD
}

/* [fkelava 25/03/26 18:30]
 * Derived from:
 *
 * Phyre::PSerialization::PBinary::PClusterHeaderBase::Bind() -> +68950
 * PClassDescriptor<PClusterHeaderBase>                       -> +896AA8
 */

[StructLayout(LayoutKind.Sequential, Size = 0x48, Pack = 0x4)]
internal struct PClusterHeaderBase {
    public uint m_phyreMarker;              // 0x0  - name: +712EC4 -> +68996
    public uint m_size;                     // 0x4  - name: +70FB54 -> +689E2
    public uint m_packedNamespaceSize;      // 0x8  - name: +712EE8 -> +68A2E
    public uint m_platformID;               // 0xC  - name: +713008 -> +68E85
    public uint m_instanceListCount;        // 0x10 - name: +712ED4 -> +68A7A
    public uint m_arrayFixupSize;           // 0x14 - name: +712F00 -> +68AC6
    public uint m_arrayFixupCount;          // 0x18 - name: +712F14 -> +68B12
    public uint m_pointerFixupSize;         // 0x1C - name: +712F28 -> +68B5E
    public uint m_pointerFixupCount;        // 0x20 - name: +712F3C -> +68BAC
    public uint m_pointerArrayFixupSize;    // 0x24 - name: +712F50 -> +68BFD
    public uint m_pointerArrayFixupCount;   // 0x28 - name: +712F68 -> +68C4E
    public uint m_pointersInArraysCount;    // 0x2C - name: +712F84 -> +68C9F
    public uint m_userFixupCount;           // 0x30 - name: +712F9C -> +68CF0
    public uint m_userFixupDataSize;        // 0x34 - name: +712FB0 -> +68D41
    public uint m_totalDataSize;            // 0x38 - name: +712FC4 -> +68D92
    public uint m_headerClassInstanceCount; // 0x3C - name: +712FD4 -> +68DE3
    public uint m_headerClassChildCount;    // 0x40 - name: +712FF0 -> +68E34
    public uint m_physicsEngineID;          // 0x44 - name: +713018 -> +68ED6
}

/* [fkelava 25/03/26 21:00]
 * Derived from appearance in the PClassDescriptor of PTextureCommonBase.
 *
 * Neither the constructor nor class descriptor are known at this time.
 */

[StructLayout(LayoutKind.Sequential, Size = 0x14, Pack = 0x4)]
internal struct PTextureFormatBase {
    public uint _0x00;
    public uint _0x04;
    public uint _0x08;
    public uint _0x0C;
    public uint _0x10;
}

/* [fkelava 25/03/26 18:30]
 * Derived from:
 *
 * Phyre::PRendering::PTextureCommonBase::Bind() -> +C20C0
 * PClassDescriptor<PTextureCommonBase>          -> +8A3530
 */

[StructLayout(LayoutKind.Sequential, Size = 0x1C, Pack = 0x4)]
internal unsafe struct PTextureCommonBase {
    public PTextureFormatBase* m_format;       // 0x0
    public byte                m_memoryType;   // 0x5
    public byte                _0x06;
    public byte                _0x07;
    public uint                _0x08;
    public uint                m_mipmapCount;  // 0xC
    public uint                m_maxMipLevel;  // 0x10
    public int                 m_textureFlags; // 0x14
    public uint                _0x18;
}

/* [fkelava 25/03/26 18:30]
 * Derived from:
 *
 * Phyre::PRendering::PTexture2DBase::Bind() -> +C3510
 * PClassDescriptor<PTexture2DBase>          -> +8A3738
 */

[StructLayout(LayoutKind.Sequential, Size = 0x24, Pack = 0x4)]
internal struct PTexture2DBase {
    public PTextureCommonBase base_PTextureCommonBase;
    public uint               m_width;  // 0x1C
    public uint               m_height; // 0x20
}

/* [fkelava 25/03/26 18:30]
 * Derived from:
 *
 * PClassDescriptor<PTexture2DD3D11> -> +8B39A0
 */

[StructLayout(LayoutKind.Sequential, Size = 0x74, Pack = 0x4)]
internal struct PTexture2DD3D11 {
    public PTexture2DBase base_PTexture2DBase;
    public uint           _0x28;
    public uint           _0x2C;
    public uint           _0x30;
    public uint           _0x34;
    public uint           _0x38;
    public uint           _0x3C;
    public uint           _0x40;
    public uint           _0x44;
    public uint           _0x48;
    public uint           _0x4C;
    public uint           _0x50;
    public uint           _0x54;
    public uint           _0x58;
    public uint           _0x5C;
    public uint           _0x60;
    public uint           _0x64;
    public uint           _0x68;
    public uint           _0x6C;
    public uint           _0x70;
}

/* [fkelava 25/03/26 18:30]
 * Derived from:
 *
 * Phyre::PRendering::PTexture2D::Bind() -> +C34F0
 * PClassDescriptor<PTexture2D>          -> +8A37D0
 */

[StructLayout(LayoutKind.Sequential, Size = 0x74, Pack = 0x4)]
internal struct PTexture2D {
    public PTexture2DD3D11 base_PTexture2DD3D11;
}

/* [fkelava 25/03/26 18:30]
 * Derived from:
 *
 * Phyre::PWorld::Bind()    -> +42F00
 * PClassDescriptor<PWorld> -> +890E30
 */

[StructLayout(LayoutKind.Sequential, Size = 0x10, Pack = 0x4)]
internal struct PWorld {
    public uint _0x00;
    public uint _0x04;
    public uint _0x08;
    public uint _0x0C;
}
