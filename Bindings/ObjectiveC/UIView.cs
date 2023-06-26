using Bindings.Metal;
using static Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Bindings.ObjectiveC;

public struct UIView
{
    public readonly nint NativePtr;
    public UIView(nint ptr) => NativePtr = ptr;

    public CALayer layer => objc_msgSend<CALayer>(NativePtr, "layer");

    public CGRect frame => CGRect_objc_msgSend(NativePtr, "frame");
}