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
 */

namespace Fahrenheit;

/* [fkelava 19/03/26 03:01]
 * .ctor -> FFX.exe+3A170
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
    public PClassDescriptor*                      m_classDescriptor; // name: FFX.exe+70EA88 -> FFX.exe+3A362
    public nint                                   ptr_name;          // null-terminated ANSI/UTF-8 string
    public uint                                   m_flags;           // name: FFX.exe+70EA80 -> FFX.exe+3A316

    public override string ToString() {
        return $"{(*m_classDescriptor).base_PType}::{Marshal.PtrToStringAnsi(ptr_name)}";
    }
}

/* [fkelava 17/03/26 04:13]
 * ctors of this class are all inlined; name inferred from RTTI metadata of instantiations:
 * FFX.exe+80B138 - Phyre::PClassData<>
 */

/// <summary>
///     Describes a unique member of a Phyre class which has a concrete <see cref="PType"/>.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 0x18, Pack = 4)]
internal unsafe struct PClassData {
    public PClassMember base_PClassMember;
    public PType*       m_type; // name: FFX.exe+70F994 -> FFX.exe+465A6

    public override string ToString() {
        return $"{*m_type} {base_PClassMember}";
    }
}

/* [fkelava 19/03/26 03:01]
 * .ctor -> FFX.exe+1759B0
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
 * ctors of this class are all inlined; name inferred from RTTI metadata:
 * FFX.exe+80A1AC - Phyre::PAnnotatable<>
 */

/// <summary>
///     Indicates that a Phyre type can have <see cref="PAnnotation"/>s, arbitrary tags, associated with it.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 0x8, Pack = 4)]
internal struct PAnnotatable {
    public PSimpleDoubleListElement<PAnnotation> base_PSimpleDoubleListElement;
}

/// <summary>
///     Describes a unique field of a Phyre class which has a concrete <see cref="PType"/>, offset, and may have <see cref="PAnnotation"/>s associated with it.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 0x2C, Pack = 4)]
internal unsafe struct PClassDataMember {
    public nint         vftable;
    public PClassData   base_PClassData;   // 0x4
    public PAnnotatable base_PAnnotatable; // 0x1C
    public nint         m_offset;          // name: FFX.exe+70F99C -> FFX.exe+465F2
    public nint         __0x28;

    public override string ToString() {
        return $"{base_PClassData} at offset 0x{m_offset:X}";
    }
}

/* [fkelava 13/03/26 21:49]
 * See Phyre::TypeDispenser::GetType<Phyre::PType>
 * (FFX.exe+375D0)
 *
 * Almost certainly the 'unknown' fields belong to an inlined base type.
 * RTTI metadata does not explain _which_, however.
 */

/// <summary>
///     Describes a unique type in the Phyre type system- its name, size, alignment, and more.
///     <para/>
///     Classes have a derived <see cref="PClassDescriptor"/> instead, providing information about their members, layout and inheritance chain.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 0x2C, Pack = 4)]
internal struct PType {
    public nint vftable;
    public nint _0x04_unknown;
    public nint _0x08_unknown;
    public nint _0x0C_unknown;
    public nint _0x10_unknown;
    public nint _0x14_unknown;   // often equal to the type name, but why?
    public nint _0x18_type_name; // null-terminated ANSI/UTF-8 string
    public nint _0x1C_type_size;
    public nint _0x20_type_alignment;
    public nint fnptr_fixup_get;
    public nint fnptr_fixup_resolve;

    public override string ToString() {
        return $"{Marshal.PtrToStringAnsi(_0x18_type_name)} (sz 0x{_0x1C_type_size:X}, align 0x{_0x20_type_alignment:X})";
    }
}

[StructLayout(LayoutKind.Sequential, Size = 0x4C, Pack = 4)]
internal unsafe struct PInstanceList {
    public PSimpleDoubleListElement<PInstanceList> base_PSimpleDoubleListElement;
    public PFreeList<PUnknown>                     _0x08_free_list;
    public PSimpleDoubleListElement<PUnknown>      _0x20;
    public PCluster*                               _0x28_cluster;
    public PClassDescriptor*                       _0x2C_class_descriptor;
    public nint                                    _0x30;
    public nint                                    _0x34;
    public nint                                    _0x38;
    public nint                                    _0x3C;
    public nint                                    _0x40;
    public nint                                    _0x44;
    public nint                                    _0x48;

    public override string ToString() {
        return $"{nameof(PInstanceList)}<{(*_0x2C_class_descriptor).base_PType}>";
    }
}

/* [fkelava 17/03/26 04:13]
 * ctors of this class are all inlined; name inferred from RTTI metadata of instantiations:
 * FFX.exe+824D90 - Phyre::PNamedSemanticDescriptorForType<PAnnotationSemantic>
 * FFX.exe+1758A0 - Phyre::PTypeDispenser::GetType<Phyre::PAnnotationSemantic>
 */

[StructLayout(LayoutKind.Sequential, Size = 0x8, Pack = 4)]
internal struct PAnnotationSemantic {
    public nint _0x00;
    public nint _0x04;
}

/// <summary>
///     Describes a Phyre class- its layout, members, and inheritance chain.
///     <para/>
///     Basic type information such as name, size and alignment is provided by the base class <see cref="PType"/>.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 0x94, Pack = 4)]
internal unsafe struct PClassDescriptor {
    public PType                                      base_PType;
    public PSimpleDoubleListElement<PClassDescriptor> base_PSimpleDoubleListElement;
    public PSimpleDoubleListElement<PUnknown>         _0x34;
    public PNamespace*                                _0x3C_namespace;
    public PClassDescriptor*                          m_parent;                      // 0x40 - name: FFX.exe+70FB5C, FFX.exe+47F02
    public PSimpleDoubleListElement<PClassData>       _0x44;
    public PSimpleDoubleListElement<PClassMember>     _0x4C;
    public PSimpleDoubleListElement<PClassMember>     _0x54;
    public PSimpleDoubleListElement<PClassData>       _0x5C;
    public nint                                       _0x64_buffer_default;
    public nint                                       _0x68_buffer_validation;
    public nint                                       _0x6C_buffer_write_mask;
    public nint                                       _0x70;
    public nint                                       _0x74;
    public nint                                       _0x78;
    public nint                                       _0x7C;
    public nint                                       _0x80;
    public nint                                       m_offsetToParent;               // 0x84 - name: FFX.exe+70FB68, FFX.exe+47F50
    public nint                                       m_offsetToBase;                 // 0x88 - name: FFX.exe+70FB7C, FFX.exe+47F9F
    public nint                                       m_offsetToBaseInAllocatedBlock; // 0x8C - name: FFX.exe+70FB8C, FFX.exe+47FEE
    public nint                                       _0x90_flags;                    // 0x90

    public override string ToString() {
        return $"{nameof(PClassDescriptor)}<{base_PType}>";
    }
}

/// <inheritdoc cref="PClassDescriptor"/>
[StructLayout(LayoutKind.Sequential, Size = 0xB0, Pack = 4)]
internal struct PClassDescriptorDynamic {
    public PClassDescriptor base_PClassDescriptor; // 0x90
    public nint             m_actualSize;          // 0x94 - name: FFX.exe+70FBC4, FFX.exe+4808C
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

/* [fkelava 17/03/26 04:13]
 * ctors of this class are all inlined; name inferred from RTTI metadata of instantiations:
 * FFX.exe+80A160, 80B180, 80B620, 80E448, 8312F8 - Phyre::PSimpleDoubleListElement<>
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
 * .ctor -> FFX.exe+9C550
 * .dtor -> FFX.exe+9C680
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
 * .ctor -> FFX.exe+3DE70
 */

/// <summary>
///     A namespace is a group of <see cref="PClassDescriptor"/>s. It can contain nested namespaces as well.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 0x1C, Pack = 4)]
internal struct PNamespace {
    public PSimpleDoubleListElement<PNamespace>       base_PSimpleDoubleListElement;
    public PSimpleDoubleListElement<PClassDescriptor> _0x08_class_descriptors;
    public nint                                       m_index;              // name: FFX.exe+24FE9
    public PSimpleDoubleListElement<PNamespace>       _0x14_sub_namespaces;
}

/* [fkelava 19/03/26 16:20]
 * .ctor -> FFX.exe+3F910
 */

/// <summary>
///     A cluster is a
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 0x50, Pack = 4)]
internal struct PCluster {
    public PNamespace                              _0x00_namespace;
    public PSimpleDoubleListElement<PInstanceList> _0x1C_instance_lists;
    public nint                                    _0x24;
    public PFreeList<PInstanceList>                _0x28_free_list_PInstanceList;
    public nint                                    _0x40;
    public nint                                    _0x44;
    public PSimpleDoubleListElement<PUnknown>      _0x48;
}

/* [fkelava 19/03/26 16:20]
 * .ctor -> FFX.exe+1A0390
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
     * Phyre types have complex inheritance graphs, and often inherit multiple base
     * classes. The problem is that the compiler is free to order such base classes as it
     * desires from struct to struct within the same compilation unit.
     *
     * Many structs inherit from PSimpleDoubleListElement. Pointers in such doubly-linked lists
     * are offset by where PSimpleDoubleListElement is laid out in a given type. Successfully iterating such a
     * list requires us to manually correct, which is exactly what the compiler does under the hood.
     *
     * An example of a class that suffers from this is PClassDescriptor, where PType goes first.
     * e.g. in Phyre::PNamespace::InitializeGlobalClassDescriptors (FFX.exe+3E530)
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
