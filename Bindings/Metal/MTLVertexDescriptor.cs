using Bindings.ObjectiveC;
using static Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Bindings.Metal;

public struct MTLVertexDescriptor
{
    public readonly nint NativePtr;

    public MTLVertexBufferLayoutDescriptorArray layouts
        => objc_msgSend<MTLVertexBufferLayoutDescriptorArray>(NativePtr, sel_layouts);

    public MTLVertexAttributeDescriptorArray attributes
        => objc_msgSend<MTLVertexAttributeDescriptorArray>(NativePtr, sel_attributes);

    private static readonly Selector sel_layouts = "layouts";
    private static readonly Selector sel_attributes = "attributes";
}