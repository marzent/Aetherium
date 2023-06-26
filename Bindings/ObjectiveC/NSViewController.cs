using static Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Bindings.ObjectiveC;

public struct NSViewController
{
    public readonly nint NativePtr;
    public static implicit operator nint(NSViewController nsViewController) => nsViewController.NativePtr;

    public NSViewController(nint ptr) => NativePtr = ptr;

    public NSView View
    {
        get => objc_msgSend<NSView>(NativePtr, "view");
        set => objc_msgSend(NativePtr, "setView:", value);
    }
    
}