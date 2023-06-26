using System.Runtime.InteropServices;
using Bindings.ObjectiveC;
using static Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Bindings.Metal;

[StructLayout(LayoutKind.Sequential)]
public struct MTLRenderPipelineColorAttachmentDescriptorArray
{
    public readonly nint NativePtr;

    public MTLRenderPipelineColorAttachmentDescriptor this[uint index]
    {
        get
        {
            nint ptr = IntPtr_objc_msgSend(NativePtr, Selectors.objectAtIndexedSubscript, index);
            return new MTLRenderPipelineColorAttachmentDescriptor(ptr);
        }
        set
        {
            objc_msgSend(NativePtr, Selectors.setObjectAtIndexedSubscript, value.NativePtr, index);
        }
    }
}