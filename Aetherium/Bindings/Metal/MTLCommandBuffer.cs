using System.Runtime.InteropServices;
using Aetherium.Bindings.ObjectiveC;
using static Aetherium.Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Aetherium.Bindings.Metal;

[StructLayout(LayoutKind.Sequential)]
public struct MTLCommandBuffer
{
    public MTLCommandBuffer(nint ptr) => NativePtr = ptr;

    public readonly nint NativePtr;

    public MTLRenderCommandEncoder renderCommandEncoderWithDescriptor(MTLRenderPassDescriptor desc)
    {
        return new MTLRenderCommandEncoder(
            IntPtr_objc_msgSend(NativePtr, sel_renderCommandEncoderWithDescriptor, desc.NativePtr));
    }

    public void presentDrawable(nint drawable) => objc_msgSend(NativePtr, sel_presentDrawable, drawable);
    
    public void presentDrawable(CAMetalDrawable drawable) => objc_msgSend(NativePtr, sel_presentDrawable, drawable.NativePtr);

    public void commit() => objc_msgSend(NativePtr, sel_commit);

    public MTLBlitCommandEncoder blitCommandEncoder()
        => objc_msgSend<MTLBlitCommandEncoder>(NativePtr, sel_blitCommandEncoder);

    public MTLComputeCommandEncoder computeCommandEncoder()
        => objc_msgSend<MTLComputeCommandEncoder>(NativePtr, sel_computeCommandEncoder);

    public void waitUntilCompleted() => objc_msgSend(NativePtr, sel_waitUntilCompleted);

    public void addCompletedHandler(MTLCommandBufferHandler block)
        => objc_msgSend(NativePtr, sel_addCompletedHandler, block);
    public void addCompletedHandler(nint block)
        => objc_msgSend(NativePtr, sel_addCompletedHandler, block);

    public MTLCommandBufferStatus status => (MTLCommandBufferStatus)uint_objc_msgSend(NativePtr, sel_status);

    private static readonly Selector sel_renderCommandEncoderWithDescriptor = "renderCommandEncoderWithDescriptor:";
    private static readonly Selector sel_presentDrawable = "presentDrawable:";
    private static readonly Selector sel_commit = "commit";
    private static readonly Selector sel_blitCommandEncoder = "blitCommandEncoder";
    private static readonly Selector sel_computeCommandEncoder = "computeCommandEncoder";
    private static readonly Selector sel_waitUntilCompleted = "waitUntilCompleted";
    private static readonly Selector sel_addCompletedHandler = "addCompletedHandler:";
    private static readonly Selector sel_status = "status";
}