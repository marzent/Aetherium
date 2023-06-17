using Aetherium.Bindings.ObjectiveC;
using static Aetherium.Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Aetherium.Bindings.Metal;

public struct MTLVertexBufferLayoutDescriptor
{
    public readonly nint NativePtr;

    public MTLVertexBufferLayoutDescriptor(nint ptr) => NativePtr = ptr;

    public MTLVertexStepFunction stepFunction
    {
        get => (MTLVertexStepFunction)uint_objc_msgSend(NativePtr, sel_stepFunction);
        set => objc_msgSend(NativePtr, sel_setStepFunction, (uint)value);
    }

    public nuint stride
    {
        get => UIntPtr_objc_msgSend(NativePtr, sel_stride);
        set => objc_msgSend(NativePtr, sel_setStride, value);
    }

    public nuint stepRate
    {
        get => UIntPtr_objc_msgSend(NativePtr, sel_stepRate);
        set => objc_msgSend(NativePtr, sel_setStepRate, value);
    }

    private static readonly Selector sel_stepFunction = "stepFunction";
    private static readonly Selector sel_setStepFunction = "setStepFunction:";
    private static readonly Selector sel_stride = "stride";
    private static readonly Selector sel_setStride = "setStride:";
    private static readonly Selector sel_stepRate = "stepRate";
    private static readonly Selector sel_setStepRate = "setStepRate:";
}