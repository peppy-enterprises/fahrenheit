// SPDX-License-Identifier: MIT

namespace Fahrenheit;

/* [fkelava 19/03/26 03:01]
 * .ctor -> FFX.exe+3A170
 */

/* [fkelava 20/03/26 03:12]
 * This structure currently suffers from a strange off-by-one
 * that can't be seen from Ghidra. Work is being done to rectify this.
 */

/// <summary>
///     Describes a unique member of a given Phyre class.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 0x14, Pack = 4)]
internal unsafe struct PClassMember {
    public PSimpleDoubleListElement<PClassMember> base_PSimpleDoubleListElement;
    public PClassDescriptor*                      m_classDescriptor; // name: FFX.exe+70EA88 -> FFX.exe+3A362
    public nint                                   ptr_name;          // null-terminated ANSI/UTF-8 string
    public uint                                   m_flags;           // name: FFX.exe+70EA80 -> FFX.exe+3A316
}

/* [fkelava 17/03/26 04:13]
 * ctors of this class are all inlined; name inferred from RTTI metadata of instantiations:
 * FFX.exe+80B138 - Phyre::PClassData<>
 */

[StructLayout(LayoutKind.Sequential, Size = 0x18, Pack = 4)]
internal unsafe struct PClassData {
    public PClassMember base_PClassMember;
    public PType*       m_type; // name: FFX.exe+70F994 -> FFX.exe+465A6
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
///     Allows for the association of 'annotations', arbitrary tags, to Phyre types.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 0x8, Pack = 4)]
internal struct PAnnotatable {
    public PSimpleDoubleListElement<PAnnotation> base_PSimpleDoubleListElement;
}

/* [fkelava 20/03/26 03:12]
 * This structure currently suffers from a strange off-by-one
 * that can't be seen from Ghidra. Work is being done to rectify this.
 */

/// <summary>
///     Describes a unique 'data' member of a given Phyre class.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 0x2C, Pack = 4)]
internal unsafe struct PClassDataMember {
    private nint         vftable;
    public  PClassData   base_PClassData;   // 0x4
    public  PAnnotatable base_PAnnotatable; // 0x1C
    public  nint         m_offset;          // name: FFX.exe+70F99C -> FFX.exe+465F2
    public  nint         __0x28;
}

/* [fkelava 13/03/26 21:49]
 * See Phyre::TypeDispenser::GetType<Phyre::PType>
 * (FFX.exe+375D0)
 *
 * Almost certainly the 'unknown' fields belong to an inlined base type.
 * RTTI metadata does not explain _which_, however.
 */

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
///     Concretely describes a Phyre class; its name, inheritance chain, and layout.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 0x94, Pack = 4)]
internal unsafe struct PClassDescriptor {
    public PType                                      base_PType;
    public PSimpleDoubleListElement<PUnknown>         _0x2C;
    public PSimpleDoubleListElement<PUnknown>         _0x34;
    public PNamespace*                                _0x3C_namespace;
    public PClassDescriptor*                          m_parent;                      // 0x40 - name: FFX.exe+70FB5C, FFX.exe+47F02
    public PSimpleDoubleListElement<PClassDataMember> _0x44;
    public PSimpleDoubleListElement<PClassDataMember> _0x4C;
    public PSimpleDoubleListElement<PClassDataMember> _0x54;
    public PSimpleDoubleListElement<PClassDataMember> _0x5C;
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
}

/* [fkelava 11/03/26 20:00]
 * .ctor -> FFX.exe+3DE70
 */

[StructLayout(LayoutKind.Sequential, Size = 0x1C, Pack = 4)]
internal struct PNamespace {
    public PSimpleDoubleListElement<PNamespace>       base_PSimpleDoubleListElement;
    public PSimpleDoubleListElement<PClassDescriptor> _0x08_class_descriptors;
    public nint                                       m_index;              // name: FFX.exe+24FE9
    public PSimpleDoubleListElement<PNamespace>       _0x14_sub_namespaces;
}

/* [fkelava 19/03/26 16:20]
 * size: FFX.exe+40812
 */

/// <summary>
///     A 'cluster' is believed to be a Phyre asset group.
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

    public bool next(out T item) {
        item = default;

        if (_m_current == null)
            return false;

        item = *_m_current;
        _m_current = forward
            ? ((PSimpleDoubleListElement<T>*)_m_current)->next(_m_head)
            : ((PSimpleDoubleListElement<T>*)_m_current)->prev(_m_head);

        return true;
    }
}
