using Bindings.ObjectiveC;
using static Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Bindings.Metal;

public struct MTLVertexAttributeDescriptorArray
{
    public readonly nint NativePtr;

    public MTLVertexAttributeDescriptor this[uint index]
    {
        get
        {
            nint value = IntPtr_objc_msgSend(NativePtr, Selectors.objectAtIndexedSubscript, index);
            return new MTLVertexAttributeDescriptor(value);
        }
        set => objc_msgSend(NativePtr, Selectors.setObjectAtIndexedSubscript, value.NativePtr, index);
    }
}