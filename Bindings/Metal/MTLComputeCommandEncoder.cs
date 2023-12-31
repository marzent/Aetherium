using Bindings.ObjectiveC;
using static Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Bindings.Metal;

public struct MTLComputeCommandEncoder
{
    public readonly nint NativePtr;
    public bool IsNull => NativePtr == nint.Zero;

    private static readonly Selector sel_setComputePipelineState = "setComputePipelineState:";
    private static readonly Selector sel_setBuffer = "setBuffer:offset:atIndex:";
    private static readonly Selector sel_dispatchThreadgroups0 = "dispatchThreadgroups:threadsPerThreadgroup:";
    private static readonly Selector sel_dispatchThreadgroups1 = "dispatchThreadgroupsWithIndirectBuffer:indirectBufferOffset:threadsPerThreadgroup:";
    private static readonly Selector sel_endEncoding = "endEncoding";
    private static readonly Selector sel_setTexture = "setTexture:atIndex:";
    private static readonly Selector sel_setSamplerState = "setSamplerState:atIndex:";
    private static readonly Selector sel_setBytes = "setBytes:length:atIndex:";

    public void setComputePipelineState(MTLComputePipelineState state)
        => objc_msgSend(NativePtr, sel_setComputePipelineState, state.NativePtr);

    public void setBuffer(MTLBuffer buffer, nuint offset, nuint index)
        => objc_msgSend(NativePtr, sel_setBuffer,
            buffer.NativePtr,
            offset,
            index);

    public unsafe void setBytes(void* bytes, nuint length, nuint index)
        => objc_msgSend(NativePtr, sel_setBytes, bytes, length, index);

    public void dispatchThreadGroups(MTLSize threadgroupsPerGrid, MTLSize threadsPerThreadgroup)
        => objc_msgSend(NativePtr, sel_dispatchThreadgroups0, threadgroupsPerGrid, threadsPerThreadgroup);

    public void dispatchThreadgroupsWithIndirectBuffer(
        MTLBuffer indirectBuffer,
        nuint indirectBufferOffset,
        MTLSize threadsPerThreadgroup)
        => objc_msgSend(NativePtr, sel_dispatchThreadgroups1,
            indirectBuffer.NativePtr,
            indirectBufferOffset,
            threadsPerThreadgroup);

    public void endEncoding() => objc_msgSend(NativePtr, sel_endEncoding);

    public void setTexture(MTLTexture texture, nuint index)
        => objc_msgSend(NativePtr, sel_setTexture, texture.NativePtr, index);

    public void setSamplerState(MTLSamplerState sampler, nuint index)
        => objc_msgSend(NativePtr, sel_setSamplerState, sampler.NativePtr, index);

    public void pushDebugGroup(NSString @string)
        => objc_msgSend(NativePtr, Selectors.pushDebugGroup, @string.NativePtr);

    public void popDebugGroup() => objc_msgSend(NativePtr, Selectors.popDebugGroup);

    public void insertDebugSignpost(NSString @string)
        => objc_msgSend(NativePtr, Selectors.insertDebugSignpost, @string.NativePtr);
}