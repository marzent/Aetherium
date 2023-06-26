using System.Runtime.InteropServices;
using Bindings.ObjectiveC;
using static Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Bindings.Metal;

[StructLayout(LayoutKind.Sequential)]
public struct MTLRenderPassDescriptor
{
    private static readonly ObjCClass s_class = new ObjCClass(nameof(MTLRenderPassDescriptor));
    public readonly nint NativePtr;
    public static implicit operator nint(MTLRenderPassDescriptor descriptor) => descriptor.NativePtr;
    public static MTLRenderPassDescriptor New() => s_class.AllocInit<MTLRenderPassDescriptor>();
    public void Release() => release(NativePtr);

    public MTLRenderPassColorAttachmentDescriptorArray colorAttachments
        => objc_msgSend<MTLRenderPassColorAttachmentDescriptorArray>(NativePtr, sel_colorAttachments);

    public MTLRenderPassDepthAttachmentDescriptor depthAttachment
        => objc_msgSend<MTLRenderPassDepthAttachmentDescriptor>(NativePtr, sel_depthAttachment);

    public MTLRenderPassStencilAttachmentDescriptor stencilAttachment
        => objc_msgSend<MTLRenderPassStencilAttachmentDescriptor>(NativePtr, sel_stencilAttachment);
    

    private static readonly Selector sel_colorAttachments = "colorAttachments";
    private static readonly Selector sel_depthAttachment = "depthAttachment";
    private static readonly Selector sel_stencilAttachment = "stencilAttachment";
}