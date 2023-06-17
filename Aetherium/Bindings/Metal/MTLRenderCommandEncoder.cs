using System.Runtime.InteropServices;
using Aetherium.Bindings.ObjectiveC;
using static Aetherium.Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Aetherium.Bindings.Metal;

[StructLayout(LayoutKind.Sequential)]
public struct MTLRenderCommandEncoder
{
    public readonly nint NativePtr;
    public MTLRenderCommandEncoder(nint ptr) => NativePtr = ptr;
    public bool IsNull => NativePtr == nint.Zero;

    public void setRenderPipelineState(MTLRenderPipelineState pipelineState)
        => objc_msgSend(NativePtr, sel_setRenderPipelineState, pipelineState.NativePtr);

    public void setVertexBuffer(MTLBuffer buffer, nuint offset, nuint index)
        => objc_msgSend(NativePtr, sel_setVertexBuffer,
            buffer.NativePtr,
            offset,
            index);

    public void setFragmentBuffer(MTLBuffer buffer, nuint offset, nuint index)
        => objc_msgSend(NativePtr, sel_setFragmentBuffer,
            buffer.NativePtr,
            offset,
            index);

    public void setVertexTexture(MTLTexture texture, nuint index)
        => objc_msgSend(NativePtr, sel_setVertexTexture, texture.NativePtr, index);
    public void setFragmentTexture(MTLTexture texture, nuint index)
        => objc_msgSend(NativePtr, sel_setFragmentTexture, texture.NativePtr, index);

    public void setVertexSamplerState(MTLSamplerState sampler, nuint index)
        => objc_msgSend(NativePtr, sel_setVertexSamplerState, sampler.NativePtr, index);

    public void setFragmentSamplerState(MTLSamplerState sampler, nuint index)
        => objc_msgSend(NativePtr, sel_setFragmentSamplerState, sampler.NativePtr, index);

    public void drawPrimitives(
        MTLPrimitiveType primitiveType,
        nuint vertexStart,
        nuint vertexCount,
        nuint instanceCount,
        nuint baseInstance)
        => objc_msgSend(NativePtr, sel_drawPrimitives0,
            primitiveType, vertexStart, vertexCount, instanceCount, baseInstance);

    public void drawPrimitives(
        MTLPrimitiveType primitiveType,
        nuint vertexStart,
        nuint vertexCount,
        nuint instanceCount)
        => objc_msgSend(NativePtr, sel_drawPrimitives2,
            primitiveType, vertexStart, vertexCount, instanceCount);
    
    public void drawPrimitives(
        MTLPrimitiveType primitiveType,
        nuint vertexStart,
        nuint vertexCount)
        => objc_msgSend(NativePtr, sel_drawPrimitives3,
            primitiveType, vertexStart, vertexCount);

    public void drawPrimitives(MTLPrimitiveType primitiveType, MTLBuffer indirectBuffer, nuint indirectBufferOffset)
        => objc_msgSend(NativePtr, sel_drawPrimitives1,
            primitiveType, indirectBuffer, indirectBufferOffset);

    public void drawIndexedPrimitives(
        MTLPrimitiveType primitiveType,
        nuint indexCount,
        MTLIndexType indexType,
        MTLBuffer indexBuffer,
        nuint indexBufferOffset,
        nuint instanceCount)
        => objc_msgSend(NativePtr, sel_drawIndexedPrimitives0,
            primitiveType, indexCount, indexType, indexBuffer.NativePtr, indexBufferOffset, instanceCount);

    public void drawIndexedPrimitives(
        MTLPrimitiveType primitiveType,
        nuint indexCount,
        MTLIndexType indexType,
        MTLBuffer indexBuffer,
        nuint indexBufferOffset,
        nuint instanceCount,
        nint baseVertex,
        nuint baseInstance)
        => objc_msgSend(
            NativePtr,
            sel_drawIndexedPrimitives1,
            primitiveType, indexCount, indexType, indexBuffer.NativePtr, indexBufferOffset, instanceCount, baseVertex, baseInstance);

    public void drawIndexedPrimitives(
        MTLPrimitiveType primitiveType,
        MTLIndexType indexType,
        MTLBuffer indexBuffer,
        nuint indexBufferOffset,
        MTLBuffer indirectBuffer,
        nuint indirectBufferOffset)
        => objc_msgSend(NativePtr, sel_drawIndexedPrimitives2,
            primitiveType,
            indexType,
            indexBuffer,
            indexBufferOffset,
            indirectBuffer,
            indirectBufferOffset);

    public void setViewport(MTLViewport viewport)
        => objc_msgSend(NativePtr, sel_setViewport, viewport);

    public unsafe void setViewports(MTLViewport* viewports, nuint count)
        => objc_msgSend(NativePtr, sel_setViewports, viewports, count);

    public void setScissorRect(MTLScissorRect scissorRect)
        => objc_msgSend(NativePtr, sel_setScissorRect, scissorRect);

    public unsafe void setScissorRects(MTLScissorRect* scissorRects, nuint count)
        => objc_msgSend(NativePtr, sel_setScissorRects, scissorRects, count);

    public void setCullMode(MTLCullMode cullMode)
        => objc_msgSend(NativePtr, sel_setCullMode, (uint)cullMode);

    public void setFrontFacing(MTLWinding frontFaceWinding)
        => objc_msgSend(NativePtr, sel_setFrontFacingWinding, (uint)frontFaceWinding);

    public void setDepthStencilState(MTLDepthStencilState depthStencilState)
        => objc_msgSend(NativePtr, sel_setDepthStencilState, depthStencilState.NativePtr);

    public void setDepthClipMode(MTLDepthClipMode depthClipMode)
        => objc_msgSend(NativePtr, sel_setDepthClipMode, (uint)depthClipMode);

    public void endEncoding() => objc_msgSend(NativePtr, sel_endEncoding);

    public void setStencilReferenceValue(uint stencilReference)
        => objc_msgSend(NativePtr, sel_setStencilReferenceValue, stencilReference);

    public void setBlendColor(float red, float green, float blue, float alpha)
        => objc_msgSend(NativePtr, sel_setBlendColor, red, green, blue, alpha);

    public void setTriangleFillMode(MTLTriangleFillMode fillMode)
        => objc_msgSend(NativePtr, sel_setTriangleFillMode, (uint)fillMode);

    public void pushDebugGroup(NSString @string)
        => objc_msgSend(NativePtr, Selectors.pushDebugGroup, @string.NativePtr);

    public void popDebugGroup() => objc_msgSend(NativePtr, Selectors.popDebugGroup);

    public void insertDebugSignpost(NSString @string)
        => objc_msgSend(NativePtr, Selectors.insertDebugSignpost, @string.NativePtr);

    private static readonly Selector sel_setRenderPipelineState = "setRenderPipelineState:";
    private static readonly Selector sel_setVertexBuffer = "setVertexBuffer:offset:atIndex:";
    private static readonly Selector sel_setFragmentBuffer = "setFragmentBuffer:offset:atIndex:";
    private static readonly Selector sel_setVertexTexture = "setVertexTexture:atIndex:";
    private static readonly Selector sel_setFragmentTexture = "setFragmentTexture:atIndex:";
    private static readonly Selector sel_setVertexSamplerState = "setVertexSamplerState:atIndex:";
    private static readonly Selector sel_setFragmentSamplerState = "setFragmentSamplerState:atIndex:";
    private static readonly Selector sel_drawPrimitives0 = "drawPrimitives:vertexStart:vertexCount:instanceCount:baseInstance:";
    private static readonly Selector sel_drawPrimitives1 = "drawPrimitives:indirectBuffer:indirectBufferOffset:";
    private static readonly Selector sel_drawPrimitives2 = "drawPrimitives:vertexStart:vertexCount:instanceCount:";
    private static readonly Selector sel_drawPrimitives3 = "drawPrimitives:vertexStart:vertexCount:";
    private static readonly Selector sel_drawIndexedPrimitives0 = "drawIndexedPrimitives:indexCount:indexType:indexBuffer:indexBufferOffset:instanceCount:";
    private static readonly Selector sel_drawIndexedPrimitives1 = "drawIndexedPrimitives:indexCount:indexType:indexBuffer:indexBufferOffset:instanceCount:baseVertex:baseInstance:";
    private static readonly Selector sel_drawIndexedPrimitives2 = "drawIndexedPrimitives:indexType:indexBuffer:indexBufferOffset:indirectBuffer:indirectBufferOffset:";
    private static readonly Selector sel_setViewport = "setViewport:";
    private static readonly Selector sel_setViewports = "setViewports:count:";
    private static readonly Selector sel_setScissorRect = "setScissorRect:";
    private static readonly Selector sel_setScissorRects = "setScissorRects:count:";
    private static readonly Selector sel_setCullMode = "setCullMode:";
    private static readonly Selector sel_setFrontFacingWinding = "setFrontFacingWinding:";
    private static readonly Selector sel_setDepthStencilState = "setDepthStencilState:";
    private static readonly Selector sel_setDepthClipMode = "setDepthClipMode:";
    private static readonly Selector sel_endEncoding = "endEncoding";
    private static readonly Selector sel_setStencilReferenceValue = "setStencilReferenceValue:";
    private static readonly Selector sel_setBlendColor = "setBlendColorRed:green:blue:alpha:";
    private static readonly Selector sel_setTriangleFillMode = "setTriangleFillMode:";
}