using Aetherium.Bindings.Metal;
using static Aetherium.Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Aetherium.Bindings.ObjectiveC;

public struct NSObject
{
    public readonly nint NativePtr;

    public NSObject(nint ptr) => NativePtr = ptr;

    public Bool8 IsKindOfClass(nint @class) => bool8_objc_msgSend(NativePtr, sel_isKindOfClass, @class);

    private static readonly Selector sel_isKindOfClass = "isKindOfClass:";
}