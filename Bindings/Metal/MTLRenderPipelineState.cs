using Bindings.ObjectiveC;

namespace Bindings.Metal;

public struct MTLRenderPipelineState
{
    public readonly nint NativePtr;
    public MTLRenderPipelineState(nint ptr) => NativePtr = ptr;

    public void Release() => ObjectiveCRuntime.release(NativePtr);
}