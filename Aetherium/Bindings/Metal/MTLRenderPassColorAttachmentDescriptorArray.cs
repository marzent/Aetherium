using System.Runtime.InteropServices;
using Aetherium.Bindings.ObjectiveC;
using static Aetherium.Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Aetherium.Bindings.Metal;

[StructLayout(LayoutKind.Sequential)]
public struct MTLRenderPassColorAttachmentDescriptorArray
{
    public readonly nint NativePtr;

    public MTLRenderPassColorAttachmentDescriptor this[uint index]
    {
        get
        {
            nint value = IntPtr_objc_msgSend(NativePtr, Selectors.objectAtIndexedSubscript, (nuint)index);
            return new MTLRenderPassColorAttachmentDescriptor(value);
        }
        set
        {
            objc_msgSend(NativePtr, Selectors.setObjectAtIndexedSubscript, value.NativePtr, (nuint)index);
        }
    }
}