using Aetherium.Bindings.ObjectiveC;
using static Aetherium.Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Aetherium.Bindings.Metal;

public struct MTLPipelineBufferDescriptorArray
{
    public readonly nint NativePtr;

    public MTLPipelineBufferDescriptor this[uint index]
    {
        get
        {
            nint value = IntPtr_objc_msgSend(NativePtr, Selectors.objectAtIndexedSubscript, (nuint)index);
            return new MTLPipelineBufferDescriptor(value);
        }
        set
        {
            objc_msgSend(NativePtr, Selectors.setObjectAtIndexedSubscript, value.NativePtr, (nuint)index);
        }
    }
}