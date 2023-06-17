using Aetherium.Bindings.ObjectiveC;
using static Aetherium.Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Aetherium.Bindings.Metal;

public struct MTLPipelineBufferDescriptor
{
    public readonly nint NativePtr;

    public MTLPipelineBufferDescriptor(nint ptr) => NativePtr = ptr;

    public MTLMutability mutability
    {
        get => (MTLMutability)uint_objc_msgSend(NativePtr, sel_mutability);
        set => objc_msgSend(NativePtr, sel_setMutability, (uint)value);
    }

    private static readonly Selector sel_mutability = "mutability";
    private static readonly Selector sel_setMutability = "setMutability:";
}