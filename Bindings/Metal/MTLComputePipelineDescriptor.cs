using Bindings.ObjectiveC;
using static Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Bindings.Metal;

public struct MTLComputePipelineDescriptor
{
    public readonly nint NativePtr;

    public MTLFunction computeFunction
    {
        get => objc_msgSend<MTLFunction>(NativePtr, sel_computeFunction);
        set => objc_msgSend(NativePtr, sel_setComputeFunction, value.NativePtr);
    }

    public MTLPipelineBufferDescriptorArray buffers
        => objc_msgSend<MTLPipelineBufferDescriptorArray>(NativePtr, sel_buffers);

    private static readonly Selector sel_computeFunction = "computeFunction";
    private static readonly Selector sel_setComputeFunction = "setComputeFunction:";
    private static readonly Selector sel_buffers = "buffers";
}