// SPDX-License-Identifier: MIT

namespace Fahrenheit;

/* [fkelava 23/03/26 00:41]
 * Unlike 'petypes.cs' and 'petypes.g.cs', which are actual Phyre types which have either been
 * manually corrected and annotated (the former) or automatically generated based on class descriptors
 * extant in the binary (the latter), this file contains abstractions and helper types specific
 * to Fahrenheit which provide an idiomatic way to _use_ said Phyre types.
 */

/// <summary>
///     Allows iteration over a Phyre doubly-linked list of <typeparamref name="T"/>.
/// </summary>
internal unsafe ref struct FhPDoubleListIterator<T>(PSimpleDoubleListElement<T>* head, bool forward = true) where T : unmanaged {
    private readonly PSimpleDoubleListElement<T>* _m_head    = head;
    private          PSimpleDoubleListElement<T>* _m_current = forward ? head->next(head) : head->prev(head);

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
     *
     * See also https://devblogs.microsoft.com/oldnewthing/20040209-00/?p=40713.
     */

    private static T* abi_adjust_ptr(PSimpleDoubleListElement<T>* ptr_object) {
        // Ugly because we cannot switch on the generic type parameter/System.Type properly.
        return 0 switch {
            _ when typeof(T) == typeof(PClassDescriptor) => (T*)((nint)ptr_object - sizeof(PType)),
            _ when typeof(T) == typeof(PClassDataMember) => (T*)((nint)ptr_object - sizeof(nint )),
            _                                            => (T*)ptr_object
        };
    }

    /* [fkelava 02/04/26 01:30]
     * The signature must be `out T*` instead of `out T` because Phyre objects necessarily belong in unmanaged memory
     * (either having been constructed by the game, or having been placed there explicitly by us).
     *
     * Consider iterating over a doubly-linked list of a Phyre object that has a PSimpleDoubleListElement<T> field.
     * If you 'materialize' the object by `out T` then attempt to construct an iterator for said field, the head pointer
     * will be to whatever storage C# gave the materialized object, and iteration will crash the process; as the
     * head pointer is used to identify the end of the list, you will quickly begin reading garbage from managed memory.
     */

    public bool next(out T* item) {
        item = default;

        if (_m_current == null)
            return false;

        item = FhPDoubleListIterator<T>.abi_adjust_ptr(_m_current);
        _m_current = forward
            ? _m_current->next(_m_head)
            : _m_current->prev(_m_head);

        return true;
    }
}

/* [fkelava 25/04/26 14:55]
 * This unnamed structure is used as input to calls which filter instance lists.
 *
 * The calls are specialized by type/class descriptor. They iterate through a doubly-linked list
 * of instance lists, finding any object matching the supplied class descriptor.
 *
 * See +464FF0 (PTexture2D), +465060 (PTexture3D) et al.
 */

/// <summary>
///     A structure used to search a set of <see cref="PInstanceList"/> for all objects of type <typeparamref name="T"/>.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 0x20, Pack = 4)]
internal unsafe ref struct PFilteredInstanceList<T>(PInstanceList* ptr_instance_list) where T : unmanaged {
    public T*                object_base_ptr        = null;                                      // 0x00
    public int               _0x04                  = 0;
    public int               object_size            = 0;                                         // 0x08
    public int               object_count           = 0;                                         // 0x0C
    public int               _0x10                  = 0;                                         // an offset of some manner
    public PInstanceList*    ptr_instance_list      = ptr_instance_list;                         // 0x14
    public PInstanceList*    ptr_instance_list_next = ptr_instance_list;                         // 0x18
    public PClassDescriptor* ptr_class_descriptor   = FhPhyreUtil.get_class_descriptor_for<T>(); // 0x1C
}

/// <summary>
///     Allows iteration over a filtered Phyre instance list of <typeparamref name="T"/>.
/// </summary>
internal unsafe ref struct FhPFilteredInstanceListIterator<T> where T : unmanaged {
    private readonly delegate* unmanaged[Thiscall]<PFilteredInstanceList<T>*, void> _fnptr_filter;
    private readonly PFilteredInstanceList<T>                                       _filtered_list;

    private int _index;

    public FhPFilteredInstanceListIterator(PFilteredInstanceList<T> filtered_list) {
        _fnptr_filter  = FhPhyreUtil.get_instance_list_filter_for<T>();
        _fnptr_filter(&filtered_list);
        _filtered_list = filtered_list;

        _index = 0;
    }

    public bool next(out T* item) {
        item = default;

        if (_index >= _filtered_list.object_count)
            return false;

        item = _filtered_list.object_base_ptr + (_filtered_list.object_size * _index++);
        return true;
    }
}

/// <summary>
///     Internal functions for the core and runtime when dealing with Phyre objects.
/// </summary>
// TODO: X-2+LM support
internal static unsafe class FhPhyreUtil {

    public static delegate* unmanaged[Thiscall]<PFilteredInstanceList<T>*, void> get_instance_list_filter_for<T>() where T : unmanaged {
        return (delegate* unmanaged[Thiscall]<PFilteredInstanceList<T>*, void>)(FhEnvironment.BaseAddr + (0 switch {
            _ when typeof(T) == typeof(PTexture2D)
                || typeof(T) == typeof(PTexture2DD3D11)
                || typeof(T) == typeof(PTexture2DBase) => FhUtil.select(0x64FF0, 0x0, 0x0),
            _ when typeof(T) == typeof(PTexture3D)
                || typeof(T) == typeof(PTexture3DD3D11)
                || typeof(T) == typeof(PTexture3DBase) => FhUtil.select(0x65060, 0x0, 0x0),
            _                                          => throw new NotImplementedException(),
        }));
    }

    public static PClassDescriptor* get_class_descriptor_for<T>() where T : unmanaged {
        return FhUtil.ptr_at<PClassDescriptor>(0 switch {
            _ when typeof(T) == typeof(D3D11_BLEND_DESC)                                                => FhUtil.select(0x8B47E8, 0x0, 0x0),
            _ when typeof(T) == typeof(D3D11_RENDER_TARGET_BLEND_DESC)                                  => FhUtil.select(0x8B45D0, 0x0, 0x0),
            _ when typeof(T) == typeof(D3D11_DEPTH_STENCIL_DESC)                                        => FhUtil.select(0x8B43C8, 0x0, 0x0),
            _ when typeof(T) == typeof(D3D11_DEPTH_STENCILOP_DESC)                                      => FhUtil.select(0x8B4268, 0x0, 0x0),
            _ when typeof(T) == typeof(D3D11_RASTERIZER_DESC)                                           => FhUtil.select(0x8B4008, 0x0, 0x0),
            _ when typeof(T) == typeof(PMatrix4)                                                        => FhUtil.select(0x894CB0, 0x0, 0x0),
           /*
            _ when typeof(T) == typeof(Matrix3)                                                         => FhUtil.select(0x894B00, 0x0, 0x0),
            _ when typeof(T) == typeof(Point3)                                                          => FhUtil.select(0x8946C0, 0x0, 0x0),
            _ when typeof(T) == typeof(Quat)                                                            => FhUtil.select(0x894988, 0x0, 0x0),
            _ when typeof(T) == typeof(Vector2)                                                         => FhUtil.select(0x894448, 0x0, 0x0),
            */
            _ when typeof(T) == typeof(PTimer)                                                          => FhUtil.select(0x8901B0, 0x0, 0x0),
           /*
            _ when typeof(T) == typeof(Vector4)                                                         => FhUtil.select(0x894810, 0x0, 0x0),
            _ when typeof(T) == typeof(Vector3)                                                         => FhUtil.select(0x894570, 0x0, 0x0),
            _ when typeof(T) == typeof(Matrix4)                                                         => FhUtil.select(0x894BD8, 0x0, 0x0),
            _ when typeof(T) == typeof(PMatrix4x3)                                                      => FhUtil.select(0x894DD8, 0x0, 0x0),
            */
            _ when typeof(T) == typeof(PBase)                                                           => FhUtil.select(0x8902E8, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsCharacterCamera)                                         => FhUtil.select(0x8BE940, 0x0, 0x0),
            _ when typeof(T) == typeof(PScriptCallbackHandler)                                          => FhUtil.select(0x8BE2F0, 0x0, 0x0),
            _ when typeof(T) == typeof(PDataBlockBufferD3D11)                                           => FhUtil.select(0x8B11B8, 0x0, 0x0),
            _ when typeof(T) == typeof(PIndexDataBlockBufferD3D11)                                      => FhUtil.select(0x8B1088, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PDataBlockD3D11>)                                         => FhUtil.select(0x8B1468, 0x0, 0x0),
            _ when typeof(T) == typeof(PSharray<PIndexDataBlockBufferD3D11>)                            => FhUtil.select(0x8B1588, 0x0, 0x0),
            _ when typeof(T) == typeof(PSharray<PDataBlockBufferD3D11>)                                 => FhUtil.select(0x8B16A8, 0x0, 0x0),
            _ when typeof(T) == typeof(PStreamInputLayoutD3D11)                                         => FhUtil.select(0x8B3610, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PStreamInputDescD3D11>)                                   => FhUtil.select(0x8B4C58, 0x0, 0x0),
            _ when typeof(T) == typeof(PStreamInputDescD3D11)                                           => FhUtil.select(0x8B3578, 0x0, 0x0),
            _ when typeof(T) == typeof(PInstanceListHeader)                                             => FhUtil.select(0x897338, 0x0, 0x0),
            _ when typeof(T) == typeof(PClusterHeaderBase)                                              => FhUtil.select(0x896AA8, 0x0, 0x0),
            _ when typeof(T) == typeof(PClusterHeaderD3D11)                                             => FhUtil.select(0x8970F8, 0x0, 0x0),
            _ when typeof(T) == typeof(PClusterHeader)                                                  => FhUtil.select(0x896B40, 0x0, 0x0),
            _ when typeof(T) == typeof(PDynamicMesh)                                                    => FhUtil.select(0x89A7D0, 0x0, 0x0),
            _ when typeof(T) == typeof(PModifierNetworkDynamicMesh)                                     => FhUtil.select(0x8BB300, 0x0, 0x0),
            _ when typeof(T) == typeof(ShadowDynamicMesh)                                               => FhUtil.select(0x8CC158, 0x0, 0x0),
            _ when typeof(T) == typeof(BrokenScreenPolygonDynamicMesh)                                  => FhUtil.select(0x8CC058, 0x0, 0x0),
            _ when typeof(T) == typeof(RadialLineDynamicMesh)                                           => FhUtil.select(0x8CBE88, 0x0, 0x0),
            _ when typeof(T) == typeof(DistortionGridDynamicMesh)                                       => FhUtil.select(0x8CBBE0, 0x0, 0x0),
            _ when typeof(T) == typeof(ClassDynamicMesh)                                                => FhUtil.select(0x8CBA18, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PDynamicSegmentDesc>)                                     => FhUtil.select(0x89AA10, 0x0, 0x0),
            _ when typeof(T) == typeof(PDynamicSegmentDesc)                                             => FhUtil.select(0x89A738, 0x0, 0x0),
            _ when typeof(T) == typeof(PVertexStream)                                                   => FhUtil.select(0x898730, 0x0, 0x0),
            _ when typeof(T) == typeof(PMeshSegmentBase)                                                => FhUtil.select(0x899298, 0x0, 0x0),
            _ when typeof(T) == typeof(PMeshSegmentD3D11)                                               => FhUtil.select(0x8B0FF0, 0x0, 0x0),
            _ when typeof(T) == typeof(PMeshSegment)                                                    => FhUtil.select(0x899330, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PSkinBoneRemap>)                                          => FhUtil.select(0x899590, 0x0, 0x0),
            _ when typeof(T) == typeof(PSkinBoneRemap)                                                  => FhUtil.select(0x899200, 0x0, 0x0),
            _ when typeof(T) == typeof(PMesh)                                                           => FhUtil.select(0x899C80, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PSkeletonJointBounds>)                                    => FhUtil.select(0x89A2D0, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PMeshSegment>)                                            => FhUtil.select(0x89A3F0, 0x0, 0x0),
            _ when typeof(T) == typeof(PSkeletonJointBounds)                                            => FhUtil.select(0x899BE8, 0x0, 0x0),
            _ when typeof(T) == typeof(PShape)                                                          => FhUtil.select(0x89AB98, 0x0, 0x0),
           /*
            _ when typeof(T) == typeof(PArray<PUInt8,4>)                                                => FhUtil.select(0x89AD48, 0x0, 0x0),
            */
            _ when typeof(T) == typeof(PMaterialSet)                                                    => FhUtil.select(0x8998E8, 0x0, 0x0),
           /*
            _ when typeof(T) == typeof(PSharray<PMaterial *>)                                           => FhUtil.select(0x899A30, 0x0, 0x0),
            */
            _ when typeof(T) == typeof(PDynamicDataBlock)                                               => FhUtil.select(0x898D18, 0x0, 0x0),
            _ when typeof(T) == typeof(PIndexDataBlockBase)                                             => FhUtil.select(0x898EC8, 0x0, 0x0),
            _ when typeof(T) == typeof(PIndexDataBlockD3D11)                                            => FhUtil.select(0x8B1120, 0x0, 0x0),
            _ when typeof(T) == typeof(PIndexDataBlock)                                                 => FhUtil.select(0x898F60, 0x0, 0x0),
            _ when typeof(T) == typeof(PDataBlockBase)                                                  => FhUtil.select(0x8988B0, 0x0, 0x0),
            _ when typeof(T) == typeof(PDataBlockD3D11)                                                 => FhUtil.select(0x8B1250, 0x0, 0x0),
            _ when typeof(T) == typeof(PDataBlock)                                                      => FhUtil.select(0x898948, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PVertexStream>)                                           => FhUtil.select(0x898A98, 0x0, 0x0),
           /*
            _ when typeof(T) == typeof(PSharray<PMeshInstanceSegmentStreamBinding *>)                   => FhUtil.select(0x8A79A8, 0x0, 0x0),
            */
            _ when typeof(T) == typeof(PIndirectArgsBufferBase)                                         => FhUtil.select(0x8A9038, 0x0, 0x0),
            _ when typeof(T) == typeof(PIndirectArgsBufferD3D11)                                        => FhUtil.select(0x8B3D48, 0x0, 0x0),
            _ when typeof(T) == typeof(PIndirectArgsBuffer)                                             => FhUtil.select(0x8A90D0, 0x0, 0x0),
            _ when typeof(T) == typeof(PStructuredBufferBase)                                           => FhUtil.select(0x8A8D30, 0x0, 0x0),
            _ when typeof(T) == typeof(PStructuredBufferD3D11)                                          => FhUtil.select(0x8B3CB0, 0x0, 0x0),
            _ when typeof(T) == typeof(PStructuredBuffer)                                               => FhUtil.select(0x8A8DC8, 0x0, 0x0),
            _ when typeof(T) == typeof(PLight)                                                          => FhUtil.select(0x89AF80, 0x0, 0x0),
            _ when typeof(T) == typeof(PShadowCaster)                                                   => FhUtil.select(0x8A44C8, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PShadowSplit>)                                            => FhUtil.select(0x8A4680, 0x0, 0x0),
            _ when typeof(T) == typeof(PShadowSplit)                                                    => FhUtil.select(0x8A4430, 0x0, 0x0),
            _ when typeof(T) == typeof(PRenderTargetBase)                                               => FhUtil.select(0x8A4168, 0x0, 0x0),
            _ when typeof(T) == typeof(PRenderTargetD3D11)                                              => FhUtil.select(0x8B3B68, 0x0, 0x0),
            _ when typeof(T) == typeof(PRenderTarget)                                                   => FhUtil.select(0x8A4200, 0x0, 0x0),
            _ when typeof(T) == typeof(PMeshInstanceBounds)                                             => FhUtil.select(0x8A89B8, 0x0, 0x0),
           /*
            _ when typeof(T) == typeof(PArray<PModifierNetworkInstance *>)                              => FhUtil.select(0x8A80F0, 0x0, 0x0),
            */
            _ when typeof(T) == typeof(PDynamicMeshInstance)                                            => FhUtil.select(0x8A7F28, 0x0, 0x0),
            _ when typeof(T) == typeof(PModifierNetworkDynamicMeshInstance)                             => FhUtil.select(0x8A8020, 0x0, 0x0),
            _ when typeof(T) == typeof(BrokenScreenPolygonDynamicMeshInstance)                          => FhUtil.select(0x8CBF88, 0x0, 0x0),
            _ when typeof(T) == typeof(RadialLineDynamicMeshInstance)                                   => FhUtil.select(0x8CBDB8, 0x0, 0x0),
            _ when typeof(T) == typeof(DistortionGridDynamicMeshInstance)                               => FhUtil.select(0x8CBCE0, 0x0, 0x0),
            _ when typeof(T) == typeof(ClassDynamicMeshInstance)                                        => FhUtil.select(0x8CBB10, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderStreamDefinition)                                         => FhUtil.select(0x89C7E0, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderProgramBase)                                              => FhUtil.select(0x89C748, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderProgramD3D11)                                             => FhUtil.select(0x8B36A8, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderComputeProgramD3D11)                                      => FhUtil.select(0x8B3908, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderComputeProgram)                                           => FhUtil.select(0x89CA40, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderGeometryProgramD3D11)                                     => FhUtil.select(0x8B3870, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderGeometryProgram)                                          => FhUtil.select(0x89C9A8, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderFragmentProgramD3D11)                                     => FhUtil.select(0x8B37D8, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderFragmentProgram)                                          => FhUtil.select(0x89C910, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderVertexProgramD3D11)                                       => FhUtil.select(0x8B3740, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderVertexProgram)                                            => FhUtil.select(0x89C878, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderProgramParamsAndStreams)                                  => FhUtil.select(0x89C6B0, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderSource)                                                   => FhUtil.select(0x89C618, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderParameterCaptureBufferTextureBase)                        => FhUtil.select(0x89BDC8, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderParameterCaptureBufferRWTexture3D)                        => FhUtil.select(0x89C580, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderParameterCaptureBufferRWTexture2D)                        => FhUtil.select(0x89C4E8, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderParameterCaptureBufferIndexDataBlock)                     => FhUtil.select(0x89C450, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderParameterCaptureBufferDataBlock)                          => FhUtil.select(0x89C3B8, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderParameterCaptureConstantBuffer)                           => FhUtil.select(0x89BE60, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderParameterCaptureBufferRWByteAddressBuffer)                => FhUtil.select(0x89C320, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderParameterCaptureBufferByteAddressBuffer)                  => FhUtil.select(0x89C288, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderParameterCaptureBufferRWStructuredBuffer)                 => FhUtil.select(0x89C1F0, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderParameterCaptureBufferStructuredBuffer)                   => FhUtil.select(0x89C158, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderParameterCaptureBufferSampler)                            => FhUtil.select(0x89C0C0, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderParameterCaptureBufferTextureCubeMap)                     => FhUtil.select(0x89C028, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderParameterCaptureBufferTexture3D)                          => FhUtil.select(0x89BF90, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderParameterCaptureBufferTexture2D)                          => FhUtil.select(0x89BEF8, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderParameterCaptureBufferStream)                             => FhUtil.select(0x89BD30, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PShaderParameterCaptureBufferLocationTypeConstantBuffer>) => FhUtil.select(0x8A19E0, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PShaderParameterCaptureBufferLocationType>)               => FhUtil.select(0x8A18C0, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderPassParameterLocationTypesBase)                           => FhUtil.select(0x8A1338, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderPassParameterLocationTypesConstantBuffer)                 => FhUtil.select(0x8A1468, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderPassParameterLocationTypes)                               => FhUtil.select(0x8A13D0, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderParameterDefinition)                                      => FhUtil.select(0x89B980, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderParameterCaptureBufferLocation)                           => FhUtil.select(0x89B720, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderParameterCaptureBufferLocationTypeConstantBuffer)         => FhUtil.select(0x89B8E8, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderParameterCaptureBufferLocationType)                       => FhUtil.select(0x89B850, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderParameterCaptureBufferLocationSize)                       => FhUtil.select(0x89B7B8, 0x0, 0x0),
            _ when typeof(T) == typeof(PSamplerStateBase)                                               => FhUtil.select(0x8A2898, 0x0, 0x0),
            _ when typeof(T) == typeof(PSamplerStateD3D11)                                              => FhUtil.select(0x8B3C00, 0x0, 0x0),
            _ when typeof(T) == typeof(PSamplerState)                                                   => FhUtil.select(0x8A2930, 0x0, 0x0),
            _ when typeof(T) == typeof(PTextureCommonBase)                                              => FhUtil.select(0x8A3530, 0x0, 0x0),
            _ when typeof(T) == typeof(PTextureCubeMapBase)                                             => FhUtil.select(0x8A3E58, 0x0, 0x0),
            _ when typeof(T) == typeof(PTextureCubeMapD3D11)                                            => FhUtil.select(0x8B3AD0, 0x0, 0x0),
            _ when typeof(T) == typeof(PTextureCubeMap)                                                 => FhUtil.select(0x8A3EF0, 0x0, 0x0),
            _ when typeof(T) == typeof(PTexture3DBase)                                                  => FhUtil.select(0x8A3AC8, 0x0, 0x0),
            _ when typeof(T) == typeof(PTexture3DD3D11)                                                 => FhUtil.select(0x8B3A38, 0x0, 0x0),
            _ when typeof(T) == typeof(PTexture3D)                                                      => FhUtil.select(0x8A3B60, 0x0, 0x0),
            _ when typeof(T) == typeof(PTexture2DBase)                                                  => FhUtil.select(0x8A3738, 0x0, 0x0),
            _ when typeof(T) == typeof(PTexture2DD3D11)                                                 => FhUtil.select(0x8B39A0, 0x0, 0x0),
            _ when typeof(T) == typeof(PTexture2D)                                                      => FhUtil.select(0x8A37D0, 0x0, 0x0),
            _ when typeof(T) == typeof(PShader)                                                         => FhUtil.select(0x8A2548, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PShaderStreamDefinition>)                                 => FhUtil.select(0x89D200, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PShaderPass>)                                             => FhUtil.select(0x8A26D8, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderPassStateBase)                                            => FhUtil.select(0x8A1630, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderPassStateD3D11)                                           => FhUtil.select(0x8B34E0, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderPassBase)                                                 => FhUtil.select(0x8A1500, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderPassD3D11)                                                => FhUtil.select(0x8B3448, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderPass)                                                     => FhUtil.select(0x8A1598, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PShaderParameterCaptureBufferLocation>)                   => FhUtil.select(0x8A1B00, 0x0, 0x0),
            _ when typeof(T) == typeof(PContextVariantFoldingTable)                                     => FhUtil.select(0x8A5660, 0x0, 0x0),
            _ when typeof(T) == typeof(PShaderPassInfo)                                                 => FhUtil.select(0x8A55C8, 0x0, 0x0),
            _ when typeof(T) == typeof(PSceneRenderPass)                                                => FhUtil.select(0x8A5530, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PContextVariantFoldingTable>)                             => FhUtil.select(0x8A5AA0, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PShaderPassInfo>)                                         => FhUtil.select(0x8A5BC0, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PShader>)                                                 => FhUtil.select(0x8A5CE0, 0x0, 0x0),
            _ when typeof(T) == typeof(PEffectVariant)                                                  => FhUtil.select(0x8A5FF0, 0x0, 0x0),
           /*
            _ when typeof(T) == typeof(PArray<PSceneRenderPass *>)                                      => FhUtil.select(0x8A6240, 0x0, 0x0),
            */
            _ when typeof(T) == typeof(PArray<PSceneRenderPass>)                                        => FhUtil.select(0x8A6360, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PMaterialSwitch>)                                         => FhUtil.select(0x8A6480, 0x0, 0x0),
            _ when typeof(T) == typeof(PMeshInstanceAttachPoint)                                        => FhUtil.select(0x8A8BD8, 0x0, 0x0),
            _ when typeof(T) == typeof(PMeshInstance)                                                   => FhUtil.select(0x8A7228, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PMeshInstanceSegmentContext>)                             => FhUtil.select(0x8A7AC8, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PMatrix4>)                                                => FhUtil.select(0x89A1B0, 0x0, 0x0),
            _ when typeof(T) == typeof(PMeshInstanceSegmentStreamBinding)                               => FhUtil.select(0x8A70F8, 0x0, 0x0),
            _ when typeof(T) == typeof(PMeshSegmentContext)                                             => FhUtil.select(0x8A4E28, 0x0, 0x0),
            _ when typeof(T) == typeof(PMeshInstanceSegmentContext)                                     => FhUtil.select(0x8A7190, 0x0, 0x0),
            _ when typeof(T) == typeof(PEffect)                                                         => FhUtil.select(0x8A66E8, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PNodeContext>)                                            => FhUtil.select(0x8A6990, 0x0, 0x0),
           /*
            _ when typeof(T) == typeof(PArray<PContextSwitch*>)                                         => FhUtil.select(0x8A6AB0, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PShadowCasterType*>)                                      => FhUtil.select(0x8A6BD0, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PLightType*>)                                             => FhUtil.select(0x8A6CF0, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PEffectVariant*>)                                         => FhUtil.select(0x8A6E10, 0x0, 0x0),
            */
            _ when typeof(T) == typeof(PNodeContext)                                                    => FhUtil.select(0x8A4BC8, 0x0, 0x0),
            _ when typeof(T) == typeof(PSharray<uint>)                                                  => FhUtil.select(0x8A4C98, 0x0, 0x0),
            _ when typeof(T) == typeof(PMaterialSwitch)                                                 => FhUtil.select(0x8A51D0, 0x0, 0x0),
            _ when typeof(T) == typeof(PMaterial)                                                       => FhUtil.select(0x8A5268, 0x0, 0x0),
            _ when typeof(T) == typeof(PConstantBufferBase)                                             => FhUtil.select(0x89B4C8, 0x0, 0x0),
            _ when typeof(T) == typeof(PConstantBufferD3D11)                                            => FhUtil.select(0x8B3DE0, 0x0, 0x0),
            _ when typeof(T) == typeof(PConstantBuffer)                                                 => FhUtil.select(0x89B560, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PShaderParameterDefinition>)                              => FhUtil.select(0x89D0E0, 0x0, 0x0),
            _ when typeof(T) == typeof(PParameterBufferBase)                                            => FhUtil.select(0x8A4FA8, 0x0, 0x0),
            _ when typeof(T) == typeof(PSceneWideParameterBuffer)                                       => FhUtil.select(0x8A8298, 0x0, 0x0),
            _ when typeof(T) == typeof(PParameterBuffer)                                                => FhUtil.select(0x8A5040, 0x0, 0x0),
            _ when typeof(T) == typeof(PNode)                                                           => FhUtil.select(0x8A91F8, 0x0, 0x0),
            _ when typeof(T) == typeof(PAnimationSlotListIndex)                                         => FhUtil.select(0x8A9778, 0x0, 0x0),
            _ when typeof(T) == typeof(PAnimationSlotFilterDeferredLoad)                                => FhUtil.select(0x8AC0B8, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PAnimationSlotFilterDeferredLoad>)                        => FhUtil.select(0x8AC2D8, 0x0, 0x0),
            _ when typeof(T) == typeof(PAnimationSlotArray)                                             => FhUtil.select(0x8A9810, 0x0, 0x0),
            _ when typeof(T) == typeof(PAnimationSet)                                                   => FhUtil.select(0x8AAEA8, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PAnimationSlotListIndex>)                                 => FhUtil.select(0x8AB058, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PAnimationChannelTarget>)                                 => FhUtil.select(0x8AB178, 0x0, 0x0),
           /*
            _ when typeof(T) == typeof(PSharray<PAnimationClip *>)                                      => FhUtil.select(0x8AB298, 0x0, 0x0),
            */
            _ when typeof(T) == typeof(PAnimationNetworkInstanceTarget)                                 => FhUtil.select(0x8ACDB0, 0x0, 0x0),
            _ when typeof(T) == typeof(PAnimationNetworkInstance)                                       => FhUtil.select(0x8ACE48, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PAnimationSlotArray>)                                     => FhUtil.select(0x8AD158, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PAnimationNetworkInstanceTarget>)                         => FhUtil.select(0x8AD398, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PAnimationDataSourceListEntry>)                           => FhUtil.select(0x8AD4B8, 0x0, 0x0),
            _ when typeof(T) == typeof(PAnimationHierarchyNode)                                         => FhUtil.select(0x8AB858, 0x0, 0x0),
            _ when typeof(T) == typeof(PTimeController)                                                 => FhUtil.select(0x8AB920, 0x0, 0x0),
            _ when typeof(T) == typeof(PTimeScaleOffsetController)                                      => FhUtil.select(0x8ABBA8, 0x0, 0x0),
            _ when typeof(T) == typeof(PTimeIntervalController)                                         => FhUtil.select(0x8ABA08, 0x0, 0x0),
            _ when typeof(T) == typeof(PAnimationDataSource)                                            => FhUtil.select(0x8ABD28, 0x0, 0x0),
            _ when typeof(T) == typeof(PAnimationTargetBlenderController)                               => FhUtil.select(0x8ACC00, 0x0, 0x0),
            _ when typeof(T) == typeof(PAnimationSpuTargetBlenderController)                            => FhUtil.select(0x8ACB00, 0x0, 0x0),
            _ when typeof(T) == typeof(PAnimationSlotFilter)                                            => FhUtil.select(0x8AC150, 0x0, 0x0),
            _ when typeof(T) == typeof(PAnimationEventController)                                       => FhUtil.select(0x8ABE20, 0x0, 0x0),
            _ when typeof(T) == typeof(PAnimationController)                                            => FhUtil.select(0x8ABF68, 0x0, 0x0),
            _ when typeof(T) == typeof(PAnimationBlenderController)                                     => FhUtil.select(0x8AC618, 0x0, 0x0),
            _ when typeof(T) == typeof(PAnimationWeightedBlenderController)                             => FhUtil.select(0x8AC898, 0x0, 0x0),
            _ when typeof(T) == typeof(PAnimationAdditiveBlenderController)                             => FhUtil.select(0x8AC9D0, 0x0, 0x0),
            _ when typeof(T) == typeof(PAnimationEventList)                                             => FhUtil.select(0x8AB580, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PAnimationEvent>)                                         => FhUtil.select(0x8AB6A8, 0x0, 0x0),
            _ when typeof(T) == typeof(PAnimationEvent)                                                 => FhUtil.select(0x8AB4E8, 0x0, 0x0),
            _ when typeof(T) == typeof(PAnimationDataSourceListEntry)                                   => FhUtil.select(0x8ACD18, 0x0, 0x0),
            _ when typeof(T) == typeof(PAnimationClip)                                                  => FhUtil.select(0x8AA7E8, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PAnimationConstantChannel>)                               => FhUtil.select(0x8AAB28, 0x0, 0x0),
           /*
            _ when typeof(T) == typeof(PArray<PAnimationChannel *>)                                     => FhUtil.select(0x8AAC48, 0x0, 0x0),
            */
            _ when typeof(T) == typeof(PAnimationChannelTimes)                                          => FhUtil.select(0x8A99B8, 0x0, 0x0),
            _ when typeof(T) == typeof(PAnimationChannelTarget)                                         => FhUtil.select(0x8A9C28, 0x0, 0x0),
            _ when typeof(T) == typeof(PAnimationChannelBase)                                           => FhUtil.select(0x8A9F30, 0x0, 0x0),
            _ when typeof(T) == typeof(PAnimationConstantChannel)                                       => FhUtil.select(0x8AA298, 0x0, 0x0),
            _ when typeof(T) == typeof(PAnimationChannel)                                               => FhUtil.select(0x8AA0B8, 0x0, 0x0),
           /*
            _ when typeof(T) == typeof(PArray<PAnimationDataSource *>)                                  => FhUtil.select(0x8AC748, 0x0, 0x0),
            */
            _ when typeof(T) == typeof(PAnimationClipBindingDataBlockCache)                             => FhUtil.select(0x8AA4C0, 0x0, 0x0),
            _ when typeof(T) == typeof(PAnimationClipBindingChannelMap)                                 => FhUtil.select(0x8AA428, 0x0, 0x0),
            _ when typeof(T) == typeof(PAnimationClipBinding)                                           => FhUtil.select(0x8AA558, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PBlendableAnimationSource>)                               => FhUtil.select(0x8AE060, 0x0, 0x0),
            _ when typeof(T) == typeof(PBlendableAnimationSource)                                       => FhUtil.select(0x8AD9A8, 0x0, 0x0),
            _ when typeof(T) == typeof(PTriggerReceiverTypeCallbackData)                                => FhUtil.select(0x8AF1E8, 0x0, 0x0),
            _ when typeof(T) == typeof(PTrigger)                                                        => FhUtil.select(0x8AED58, 0x0, 0x0),
           /*
            _ when typeof(T) == typeof(PSharray<PTriggerReceiverComponent *>)                           => FhUtil.select(0x8AEF78, 0x0, 0x0),
            */
            _ when typeof(T) == typeof(PSpline)                                                         => FhUtil.select(0x8AE7B8, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<Vector3>)                                                 => FhUtil.select(0x8AEAA8, 0x0, 0x0),
            _ when typeof(T) == typeof(PLocator)                                                        => FhUtil.select(0x8AE550, 0x0, 0x0),
            _ when typeof(T) == typeof(PSpriteAnimationInfoInstance)                                    => FhUtil.select(0x8B3158, 0x0, 0x0),
            _ when typeof(T) == typeof(PSpriteAnimationInfo)                                            => FhUtil.select(0x8B1EB8, 0x0, 0x0),
            _ when typeof(T) == typeof(PSpriteAnimationInfoChar)                                        => FhUtil.select(0x8B1F50, 0x0, 0x0),
            _ when typeof(T) == typeof(PTextureAtlasInfo)                                               => FhUtil.select(0x8B1E20, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PSubTextureInfo>)                                         => FhUtil.select(0x8B2A60, 0x0, 0x0),
            _ when typeof(T) == typeof(PSubTextureInfo)                                                 => FhUtil.select(0x8B1D88, 0x0, 0x0),
            _ when typeof(T) == typeof(PSpriteAttributes)                                               => FhUtil.select(0x8B1C58, 0x0, 0x0),
            _ when typeof(T) == typeof(PSpriteCollection)                                               => FhUtil.select(0x8B1CF0, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<uint>)                                                    => FhUtil.select(0x8AC3F8, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PModifierNetworkDynamicMeshSegment>)                      => FhUtil.select(0x8BB520, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PRenderStream>)                                           => FhUtil.select(0x8BB640, 0x0, 0x0),
           /*
            _ when typeof(T) == typeof(PArray<PUInt8 *>)                                                => FhUtil.select(0x8BB760, 0x0, 0x0),
            */
            _ when typeof(T) == typeof(PModifierNetworkDynamicMeshSegment)                              => FhUtil.select(0x8BB268, 0x0, 0x0),
            _ when typeof(T) == typeof(PModifierNetworkInstance)                                        => FhUtil.select(0x8BBB38, 0x0, 0x0),
           /*
            _ when typeof(T) == typeof(PArray<PUInt128>)                                                => FhUtil.select(0x8AD278, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PBase *>)                                                 => FhUtil.select(0x8BBD20, 0x0, 0x0),
            */
            _ when typeof(T) == typeof(PModifierNetworkInstancePacketInput)                             => FhUtil.select(0x8BBAA0, 0x0, 0x0),
            _ when typeof(T) == typeof(PRenderStream)                                                   => FhUtil.select(0x8BBFF8, 0x0, 0x0),
            _ when typeof(T) == typeof(PModifierNetwork)                                                => FhUtil.select(0x8BC420, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PModifierNetworkBuffer>)                                  => FhUtil.select(0x8BCD40, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PModifierAndInputs>)                                      => FhUtil.select(0x8BCE60, 0x0, 0x0),
            _ when typeof(T) == typeof(PModifierNetworkInfoPacket)                                      => FhUtil.select(0x8BC388, 0x0, 0x0),
            _ when typeof(T) == typeof(PModifierNetworkInfoPacket_Buffer)                               => FhUtil.select(0x8BC2F0, 0x0, 0x0),
            _ when typeof(T) == typeof(PModifierNetworkInfoPacket_ModifierInstance)                     => FhUtil.select(0x8BC258, 0x0, 0x0),
            _ when typeof(T) == typeof(PModifierNetworkInfoPacket_ModifierCode)                         => FhUtil.select(0x8BC1C0, 0x0, 0x0),
            _ when typeof(T) == typeof(PModifierAndInputs)                                              => FhUtil.select(0x8BC128, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PRenderStreamInput>)                                      => FhUtil.select(0x8BCC20, 0x0, 0x0),
            _ when typeof(T) == typeof(PModifierNetworkBuffer)                                          => FhUtil.select(0x8BC090, 0x0, 0x0),
            _ when typeof(T) == typeof(PRenderStreamInput)                                              => FhUtil.select(0x8BBF60, 0x0, 0x0),
            _ when typeof(T) == typeof(PModifierNetworkInstanceInput)                                   => FhUtil.select(0x8BBA08, 0x0, 0x0),
            _ when typeof(T) == typeof(PMorphModifierWeightsUserDataObject)                             => FhUtil.select(0x8BD390, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<float>)                                                   => FhUtil.select(0x8A9AB0, 0x0, 0x0),
            _ when typeof(T) == typeof(PLODGroup)                                                       => FhUtil.select(0x8BAB20, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PLODLevel>)                                               => FhUtil.select(0x8BAF50, 0x0, 0x0),
            _ when typeof(T) == typeof(PLODLevel)                                                       => FhUtil.select(0x8BAA88, 0x0, 0x0),
           /*
            _ when typeof(T) == typeof(PSharray<PMeshInstance *>)                                       => FhUtil.select(0x8BAE30, 0x0, 0x0),
            */
            _ when typeof(T) == typeof(PAsyncProcessHeader)                                             => FhUtil.select(0x8C9910, 0x0, 0x0),
            _ when typeof(T) == typeof(PScheduler)                                                      => FhUtil.select(0x8BD9C8, 0x0, 0x0),
            _ when typeof(T) == typeof(PScript)                                                         => FhUtil.select(0x8BE410, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PString>)                                                 => FhUtil.select(0x89A090, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsCallbackData)                                            => FhUtil.select(0x8BF190, 0x0, 0x0),
            _ when typeof(T) == typeof(PRaycastResult)                                                  => FhUtil.select(0x8BF6E8, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsCharacterControllerBase)                                 => FhUtil.select(0x8BE9D8, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsCharacterControllerBullet)                               => FhUtil.select(0x8BFE08, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsCharacterController)                                     => FhUtil.select(0x8BEA70, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsInterfaceBase)                                           => FhUtil.select(0x8BEC38, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsInterfaceBullet)                                         => FhUtil.select(0x8BFD70, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsInterface)                                               => FhUtil.select(0x8BECD0, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsRigidBodyBase)                                           => FhUtil.select(0x8BF0F8, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsRigidBodyBullet)                                         => FhUtil.select(0x8BFC40, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsRigidBody)                                               => FhUtil.select(0x8BF228, 0x0, 0x0),
           /*
            _ when typeof(T) == typeof(PSharray<PPhysicsShape *>)                                       => FhUtil.select(0x8C5558, 0x0, 0x0),
            */
            _ when typeof(T) == typeof(PPhysicsWorldBase)                                               => FhUtil.select(0x8BF650, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsWorldBullet)                                             => FhUtil.select(0x8BFCD8, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsWorld)                                                   => FhUtil.select(0x8BF780, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsShapeBase)                                               => FhUtil.select(0x8BF2C0, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsShapeBullet)                                             => FhUtil.select(0x8BF818, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsShape)                                                   => FhUtil.select(0x8BF358, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsTaperedCylinder)                                         => FhUtil.select(0x8BF5B8, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsTaperedCapsule)                                          => FhUtil.select(0x8BF520, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsSphereBase)                                              => FhUtil.select(0x8BF3F0, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsSphereBullet)                                            => FhUtil.select(0x8BFBA8, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsSphere)                                                  => FhUtil.select(0x8BF488, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsPlaneBase)                                               => FhUtil.select(0x8BEFC8, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsPlaneBullet)                                             => FhUtil.select(0x8BFB10, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsPlane)                                                   => FhUtil.select(0x8BF060, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsMeshBase)                                                => FhUtil.select(0x8BEE00, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsMeshBullet)                                              => FhUtil.select(0x8BFA78, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsMesh)                                                    => FhUtil.select(0x8BEE98, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsCylinderBase)                                            => FhUtil.select(0x8BEB08, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsCylinderBullet)                                          => FhUtil.select(0x8BF9E0, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsCylinder)                                                => FhUtil.select(0x8BEBA0, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsCapsuleBase)                                             => FhUtil.select(0x8BE810, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsCapsuleBullet)                                           => FhUtil.select(0x8BF948, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsCapsule)                                                 => FhUtil.select(0x8BE8A8, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsBoxBase)                                                 => FhUtil.select(0x8BE6E0, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsBoxBullet)                                               => FhUtil.select(0x8BF8B0, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsBox)                                                     => FhUtil.select(0x8BE778, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsModel)                                                   => FhUtil.select(0x8BEF30, 0x0, 0x0),
            _ when typeof(T) == typeof(PPhysicsMaterial)                                                => FhUtil.select(0x8BED68, 0x0, 0x0),
            _ when typeof(T) == typeof(PInputMap)                                                       => FhUtil.select(0x1541910, 0x0, 0x0),
           /*
            _ when typeof(T) == typeof(PArray<PInputAction *>)                                          => FhUtil.select(0x1541E98, 0x0, 0x0),
            */
            _ when typeof(T) == typeof(PInputAction)                                                    => FhUtil.select(0x1541878, 0x0, 0x0),
           /*
            _ when typeof(T) == typeof(PArray<PInputSource *>)                                          => FhUtil.select(0x1541D78, 0x0, 0x0),
            */
            _ when typeof(T) == typeof(PInputSource)                                                    => FhUtil.select(0x1540AD0, 0x0, 0x0),
            _ when typeof(T) == typeof(PInputSourceMotionLinearAccelerationZ)                           => FhUtil.select(0x15417E0, 0x0, 0x0),
            _ when typeof(T) == typeof(PInputSourceMotionLinearAccelerationY)                           => FhUtil.select(0x1541748, 0x0, 0x0),
            _ when typeof(T) == typeof(PInputSourceMotionLinearAccelerationX)                           => FhUtil.select(0x15416B0, 0x0, 0x0),
            _ when typeof(T) == typeof(PInputSourceMotionAngularVelocityZ)                              => FhUtil.select(0x1541618, 0x0, 0x0),
            _ when typeof(T) == typeof(PInputSourceMotionAngularVelocityY)                              => FhUtil.select(0x1541580, 0x0, 0x0),
            _ when typeof(T) == typeof(PInputSourceMotionAngularVelocityX)                              => FhUtil.select(0x15414E8, 0x0, 0x0),
            _ when typeof(T) == typeof(PInputSourceMotionQuatW)                                         => FhUtil.select(0x1541450, 0x0, 0x0),
            _ when typeof(T) == typeof(PInputSourceMotionQuatZ)                                         => FhUtil.select(0x15413B8, 0x0, 0x0),
            _ when typeof(T) == typeof(PInputSourceMotionQuatY)                                         => FhUtil.select(0x1541320, 0x0, 0x0),
            _ when typeof(T) == typeof(PInputSourceMotionQuatX)                                         => FhUtil.select(0x1541288, 0x0, 0x0),
            _ when typeof(T) == typeof(PInputSourceTouchDragY)                                          => FhUtil.select(0x15410C0, 0x0, 0x0),
            _ when typeof(T) == typeof(PInputSourceTouchDragX)                                          => FhUtil.select(0x1541028, 0x0, 0x0),
            _ when typeof(T) == typeof(PInputSourceTouchTwoFingerDragY)                                 => FhUtil.select(0x15411F0, 0x0, 0x0),
            _ when typeof(T) == typeof(PInputSourceTouchTwoFingerDragX)                                 => FhUtil.select(0x1541158, 0x0, 0x0),
            _ when typeof(T) == typeof(PInputSourceTouchPinch)                                          => FhUtil.select(0x1540F90, 0x0, 0x0),
            _ when typeof(T) == typeof(PInputSourceTouchRotate)                                         => FhUtil.select(0x1540EF8, 0x0, 0x0),
            _ when typeof(T) == typeof(PInputSourceMouseDeltaY)                                         => FhUtil.select(0x1540E60, 0x0, 0x0),
            _ when typeof(T) == typeof(PInputSourceMouseDeltaX)                                         => FhUtil.select(0x1540DC8, 0x0, 0x0),
            _ when typeof(T) == typeof(PInputSourceMouseButton)                                         => FhUtil.select(0x1540D30, 0x0, 0x0),
            _ when typeof(T) == typeof(PInputSourceJoypadAxis)                                          => FhUtil.select(0x1540C98, 0x0, 0x0),
            _ when typeof(T) == typeof(PInputSourceJoypadButton)                                        => FhUtil.select(0x1540C00, 0x0, 0x0),
            _ when typeof(T) == typeof(PInputSourceKey)                                                 => FhUtil.select(0x1540B68, 0x0, 0x0),
            _ when typeof(T) == typeof(PGameSettings)                                                   => FhUtil.select(0x16855B8, 0x0, 0x0),
           /*
            _ when typeof(T) == typeof(PArray<PInputMap *>)                                             => FhUtil.select(0x16856B0, 0x0, 0x0),
            */
            _ when typeof(T) == typeof(PPostEffectManager)                                              => FhUtil.select(0x15448A0, 0x0, 0x0),
           /*
            _ when typeof(T) == typeof(PArray<PPostEffectBase *>)                                       => FhUtil.select(0x1545010, 0x0, 0x0),
            */
            _ when typeof(T) == typeof(PPostEffectBase)                                                 => FhUtil.select(0x15435E8, 0x0, 0x0),
            _ when typeof(T) == typeof(PScreenSpaceReflectionBase)                                      => FhUtil.select(0x15446D8, 0x0, 0x0),
            _ when typeof(T) == typeof(PScreenSpaceReflectionD3D11)                                     => FhUtil.select(0x1544808, 0x0, 0x0),
            _ when typeof(T) == typeof(PScreenSpaceReflection)                                          => FhUtil.select(0x1544770, 0x0, 0x0),
            _ when typeof(T) == typeof(PScreenSpaceAmbientOcclusionBase)                                => FhUtil.select(0x1543BD8, 0x0, 0x0),
            _ when typeof(T) == typeof(PScreenSpaceAmbientOcclusionD3D11)                               => FhUtil.select(0x1543D08, 0x0, 0x0),
            _ when typeof(T) == typeof(PScreenSpaceAmbientOcclusion)                                    => FhUtil.select(0x1543C70, 0x0, 0x0),
            _ when typeof(T) == typeof(PMotionBlurBase)                                                 => FhUtil.select(0x1543A10, 0x0, 0x0),
            _ when typeof(T) == typeof(PMotionBlurD3D11)                                                => FhUtil.select(0x1543B40, 0x0, 0x0),
            _ when typeof(T) == typeof(PMotionBlur)                                                     => FhUtil.select(0x1543AA8, 0x0, 0x0),
            _ when typeof(T) == typeof(PMLAABase)                                                       => FhUtil.select(0x1543DF0, 0x0, 0x0),
            _ when typeof(T) == typeof(PMLAAD3D11)                                                      => FhUtil.select(0x1543F20, 0x0, 0x0),
            _ when typeof(T) == typeof(PMLAA)                                                           => FhUtil.select(0x1543E88, 0x0, 0x0),
            _ when typeof(T) == typeof(PGlowBase)                                                       => FhUtil.select(0x1544050, 0x0, 0x0),
            _ when typeof(T) == typeof(PGlowGPUBase)                                                    => FhUtil.select(0x15440E8, 0x0, 0x0),
            _ when typeof(T) == typeof(PGlowD3D11)                                                      => FhUtil.select(0x1544218, 0x0, 0x0),
            _ when typeof(T) == typeof(PGlow)                                                           => FhUtil.select(0x1544180, 0x0, 0x0),
            _ when typeof(T) == typeof(PFXAABase)                                                       => FhUtil.select(0x1544510, 0x0, 0x0),
            _ when typeof(T) == typeof(PFXAAD3D11)                                                      => FhUtil.select(0x1544640, 0x0, 0x0),
            _ when typeof(T) == typeof(PFXAA)                                                           => FhUtil.select(0x15445A8, 0x0, 0x0),
            _ when typeof(T) == typeof(PDepthOfFieldBase)                                               => FhUtil.select(0x1543848, 0x0, 0x0),
            _ when typeof(T) == typeof(PDepthOfFieldD3D11)                                              => FhUtil.select(0x1543978, 0x0, 0x0),
            _ when typeof(T) == typeof(PDepthOfField)                                                   => FhUtil.select(0x15438E0, 0x0, 0x0),
            _ when typeof(T) == typeof(PDeferredLightingBase)                                           => FhUtil.select(0x1543680, 0x0, 0x0),
            _ when typeof(T) == typeof(PDeferredLightingD3D11)                                          => FhUtil.select(0x15437B0, 0x0, 0x0),
            _ when typeof(T) == typeof(PDeferredLighting)                                               => FhUtil.select(0x1543718, 0x0, 0x0),
            _ when typeof(T) == typeof(PBitmapFontCharInfo)                                             => FhUtil.select(0x1684C88, 0x0, 0x0),
            _ when typeof(T) == typeof(PBitmapFont)                                                     => FhUtil.select(0x1684D20, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<int>)                                                     => FhUtil.select(0x899F70, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PBitmapFontCharInfo>)                                     => FhUtil.select(0x16850B0, 0x0, 0x0),
            _ when typeof(T) == typeof(PRandomGenerator)                                                => FhUtil.select(0x894350, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<PTypedObject>)                                            => FhUtil.select(0x893C58, 0x0, 0x0),
            _ when typeof(T) == typeof(PClusterDependencyList)                                          => FhUtil.select(0x891958, 0x0, 0x0),
            _ when typeof(T) == typeof(PSharray<PString>)                                               => FhUtil.select(0x891A28, 0x0, 0x0),
            _ when typeof(T) == typeof(PTypedObject)                                                    => FhUtil.select(0x890F30, 0x0, 0x0),
            _ when typeof(T) == typeof(PAssetReferenceImport)                                           => FhUtil.select(0x894030, 0x0, 0x0),
            _ when typeof(T) == typeof(PAssetReference)                                                 => FhUtil.select(0x893F98, 0x0, 0x0),
            _ when typeof(T) == typeof(PString)                                                         => FhUtil.select(0x891048, 0x0, 0x0),
            _ when typeof(T) == typeof(PCamera)                                                         => FhUtil.select(0x8922C0, 0x0, 0x0),
            _ when typeof(T) == typeof(PCameraProjection)                                               => FhUtil.select(0x892358, 0x0, 0x0),
            _ when typeof(T) == typeof(PCameraOrthographic)                                             => FhUtil.select(0x8930E8, 0x0, 0x0),
            _ when typeof(T) == typeof(PCameraPerspective)                                              => FhUtil.select(0x893608, 0x0, 0x0),
            _ when typeof(T) == typeof(PWorldMatrix)                                                    => FhUtil.select(0x892090, 0x0, 0x0),
            _ when typeof(T) == typeof(PComponent)                                                      => FhUtil.select(0x891C18, 0x0, 0x0),
            _ when typeof(T) == typeof(PAnimatableComponent)                                            => FhUtil.select(0x8ADA40, 0x0, 0x0),
            _ when typeof(T) == typeof(PAttachableComponent)                                            => FhUtil.select(0x8B0AC0, 0x0, 0x0),
            _ when typeof(T) == typeof(PScriptableComponent)                                            => FhUtil.select(0x8AF780, 0x0, 0x0),
            _ when typeof(T) == typeof(PScriptedComponent)                                              => FhUtil.select(0x8AF8D0, 0x0, 0x0),
            _ when typeof(T) == typeof(PCameraControllerComponent)                                      => FhUtil.select(0x8AFF10, 0x0, 0x0),
            _ when typeof(T) == typeof(PTimerComponent)                                                 => FhUtil.select(0x8AFB70, 0x0, 0x0),
            _ when typeof(T) == typeof(PQuarryComponent)                                                => FhUtil.select(0x8AF540, 0x0, 0x0),
            _ when typeof(T) == typeof(PTriggerReceiverComponent)                                       => FhUtil.select(0x8AF388, 0x0, 0x0),
            _ when typeof(T) == typeof(PSplineFollowerComponent)                                        => FhUtil.select(0x8AFD60, 0x0, 0x0),
            _ when typeof(T) == typeof(PNameComponent)                                                  => FhUtil.select(0x893E38, 0x0, 0x0),
            _ when typeof(T) == typeof(PInstancesComponent)                                             => FhUtil.select(0x893AE8, 0x0, 0x0),
            _ when typeof(T) == typeof(PEntity)                                                         => FhUtil.select(0x891DD8, 0x0, 0x0),
            _ when typeof(T) == typeof(PWorld)                                                          => FhUtil.select(0x890E30, 0x0, 0x0),
            _ when typeof(T) == typeof(PCluster)                                                        => FhUtil.select(0x890B90, 0x0, 0x0),
            _ when typeof(T) == typeof(PArray<byte>)                                                    => FhUtil.select(0x8915C8, 0x0, 0x0),
            _ when typeof(T) == typeof(PClassDescriptor)                                                => FhUtil.select(0x890760, 0x0, 0x0),
            _ when typeof(T) == typeof(PClassDescriptorDynamic)                                         => FhUtil.select(0x891330, 0x0, 0x0),
            _ when typeof(T) == typeof(PClassMember)                                                    => FhUtil.select(0x890610, 0x0, 0x0),
            _ when typeof(T) == typeof(PClassCallableMethodScript)                                      => FhUtil.select(0x8C9A40, 0x0, 0x0),
            _ when typeof(T) == typeof(PClassDataMemberDynamic)                                         => FhUtil.select(0x891180, 0x0, 0x0),
            _ when typeof(T) == typeof(PNamespace)                                                      => FhUtil.select(0x890A18, 0x0, 0x0),
            _ when typeof(T) == typeof(PInputMapper)                                                    => FhUtil.select(0x8C9E50, 0x0, 0x0),
            _ when typeof(T) == typeof(PApplicationViewport)                                            => FhUtil.select(0x8C9DB8, 0x0, 0x0),
            _ when typeof(T) == typeof(PApplication)                                                    => FhUtil.select(0x8C9D20, 0x0, 0x0),
            _                                                                                           => throw new NotImplementedException($"No class descriptor is known for type {typeof(T).Name}") }
        );
    }

}
