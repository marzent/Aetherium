using Aetherium.Bindings.ObjectiveC;
using static Aetherium.Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Aetherium.Bindings.Metal;

public struct MTLVertexAttributeDescriptor
{
    public readonly nint NativePtr;

    public MTLVertexAttributeDescriptor(nint ptr) => NativePtr = ptr;

    public MTLVertexFormat format
    {
        get => (MTLVertexFormat)uint_objc_msgSend(NativePtr, sel_format);
        set => objc_msgSend(NativePtr, sel_setFormat, (uint)value);
    }

    public nuint offset
    {
        get => UIntPtr_objc_msgSend(NativePtr, sel_offset);
        set => objc_msgSend(NativePtr, sel_setOffset, value);
    }

    public nuint bufferIndex
    {
        get => UIntPtr_objc_msgSend(NativePtr, sel_bufferIndex);
        set => objc_msgSend(NativePtr, sel_setBufferIndex, value);
    }

    private static readonly Selector sel_format = "format";
    private static readonly Selector sel_setFormat = "setFormat:";
    private static readonly Selector sel_offset = "offset";
    private static readonly Selector sel_setOffset = "setOffset:";
    private static readonly Selector sel_bufferIndex = "bufferIndex";
    private static readonly Selector sel_setBufferIndex = "setBufferIndex:";
}