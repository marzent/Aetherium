namespace Bindings.Metal;

public struct MTLComputePipelineState
{
    public readonly nint NativePtr;
    public MTLComputePipelineState(nint ptr) => NativePtr = ptr;
    public bool IsNull => NativePtr == nint.Zero;
}