using Aetherium.Bindings.ObjectiveC;
using static Aetherium.Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Aetherium.Bindings.Metal;

public struct MTLRenderPassStencilAttachmentDescriptor
{
    public readonly nint NativePtr;

    public MTLTexture texture
    {
        get => objc_msgSend<MTLTexture>(NativePtr, Selectors.texture);
        set => objc_msgSend(NativePtr, Selectors.setTexture, value.NativePtr);
    }

    public MTLLoadAction loadAction
    {
        get => (MTLLoadAction)uint_objc_msgSend(NativePtr, Selectors.loadAction);
        set => objc_msgSend(NativePtr, Selectors.setLoadAction, (uint)value);
    }

    public MTLStoreAction storeAction
    {
        get => (MTLStoreAction)uint_objc_msgSend(NativePtr, Selectors.storeAction);
        set => objc_msgSend(NativePtr, Selectors.setStoreAction, (uint)value);
    }

    public uint clearStencil
    {
        get => uint_objc_msgSend(NativePtr, sel_clearStencil);
        set => objc_msgSend(NativePtr, sel_setClearStencil, value);
    }

    public nuint slice
    {
        get => UIntPtr_objc_msgSend(NativePtr, Selectors.slice);
        set => objc_msgSend(NativePtr, Selectors.setSlice, value);
    }

    private static readonly Selector sel_clearStencil = "clearStencil";
    private static readonly Selector sel_setClearStencil = "setClearStencil:";
}