using Aetherium.Bindings.ObjectiveC;
using static Aetherium.Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Aetherium.Bindings.Metal;

public struct MTLVertexBufferLayoutDescriptorArray
{
    public readonly nint NativePtr;

    public MTLVertexBufferLayoutDescriptor this[uint index]
    {
        get
        {
            nint value = IntPtr_objc_msgSend(NativePtr, Selectors.objectAtIndexedSubscript, index);
            return new MTLVertexBufferLayoutDescriptor(value);
        }
        set => objc_msgSend(NativePtr, Selectors.setObjectAtIndexedSubscript, value.NativePtr, index);
    }
}