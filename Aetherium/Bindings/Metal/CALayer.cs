using static Aetherium.Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Aetherium.Bindings.Metal;

public struct CALayer
{
    public readonly nint NativePtr;
    public static implicit operator nint(CALayer c) => c.NativePtr;

    public CALayer(nint ptr) => NativePtr = ptr;

    public void addSublayer(nint layer)
    {
        objc_msgSend(NativePtr, "addSublayer:", layer);
    }
}