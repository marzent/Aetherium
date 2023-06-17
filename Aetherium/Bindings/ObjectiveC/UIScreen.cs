using Aetherium.Bindings.Metal;
using static Aetherium.Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Aetherium.Bindings.ObjectiveC;

public struct UIScreen
{
    public readonly nint NativePtr;
    public UIScreen(nint ptr)
    {
        NativePtr = ptr;
    }

    public CGFloat nativeScale => CGFloat_objc_msgSend(NativePtr, "nativeScale");

    public static UIScreen mainScreen
        => objc_msgSend<UIScreen>(new ObjCClass(nameof(UIScreen)), "mainScreen");
}