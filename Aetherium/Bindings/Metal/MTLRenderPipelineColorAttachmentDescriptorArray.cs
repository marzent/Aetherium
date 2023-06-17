using System.Runtime.InteropServices;
using Aetherium.Bindings.ObjectiveC;
using static Aetherium.Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Aetherium.Bindings.Metal;

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